using FourDO.Emulation.GameSource;
using FourDO.Emulation.FreeDO;
using FourDO.Emulation.Plugins;
using FourDO.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Timers;
using System.Threading;
using PerformanceCounter = FourDO.Utilities.PerformanceCounter;

namespace FourDO.Emulation
{
	public enum ConsoleState
	{
		Stopped = 0,
		Paused = 1,
		Running = 2
	}

	public class ConsoleStateChangeEventArgs : EventArgs
	{
		public ConsoleState NewState {get; private set;}

		public ConsoleStateChangeEventArgs(ConsoleState newState)
		{
			this.NewState = newState;
		}
	}

	public delegate void ConsoleStateChangeHandler(ConsoleStateChangeEventArgs e);

	internal class GameConsole
	{
		public event EventHandler FrameDone;
		public event ConsoleStateChangeHandler ConsoleStateChange;

		public class BadBiosRomException : Exception {};
		public class BadGameRomException : Exception {};
		public class BadNvramFileException : Exception {};

		#region Private Variables

		private readonly bool doFreeDOMultitask = true;

		private const int ROM_SIZE = 1 * 1024 * 1024;
		private const int NVRAM_SIZE = 32 * 1024;

		private const int PBUS_DATA_MAX_SIZE = 16;

		private const int TARGET_FRAMES_PER_SECOND = 60;

		private byte[] biosRomCopy;

		private byte[] frame;
		private IntPtr framePtr;
		private GCHandle frameHandle;

		private byte[] pbusData;
		private IntPtr pbusDataPtr;
		private GCHandle pbusDataHandle;

		private volatile object nvramCopySemaphore = new object();
		private volatile string nvramFileName;
		private volatile byte[] nvramCopy;
		private volatile IntPtr nvramCopyPtr;
		private GCHandle nvramCopyHandle;
		private volatile System.Timers.Timer nvramTimer = new System.Timers.Timer(250);

		private Thread workerThread;
		private volatile bool stopWorkerSignal = false;

		private int currentSector = 0;
		private bool isSwapFrameSignaled = false;

		private int audioBufferMilliseconds = 100; // The real default should be set externally before startup. This is here just in case.
		private long? maxLagTicks;

		private object clockSpeedSemaphore = new object();
		private int? cpuClockHertz;

		private bool? renderHighResolution;

		private volatile FrameSpeedCalculator speedCalculator = new FrameSpeedCalculator(10);

		private IAudioPlugin audioPlugin = PluginLoader.GetAudioPlugin();
		private IInputPlugin inputPlugin = PluginLoader.GetInputPlugin();

		#endregion //Private Variables

		#region Singleton Implementation

		private static GameConsole instance;

		public static GameConsole Instance
		{
			get 
			{
				if (instance == null)
				{
					instance = new GameConsole();
				}
				return instance;
			}
		}

		#endregion // Singleton Implementation

		private GameConsole()
		{
			FreeDOCore.ReadRomEvent = new FreeDOCore.ReadRomDelegate(ExternalInterface_ReadRom);
			FreeDOCore.ReadNvramEvent = new FreeDOCore.ReadNvramDelegate(ExternalInterface_ReadNvram);
			FreeDOCore.WriteNvramEvent = new FreeDOCore.WriteNvramDelegate(ExternalInterface_WriteNvram);
			FreeDOCore.SwapFrameEvent = new FreeDOCore.SwapFrameDelegate(ExternalInterface_SwapFrame);
			FreeDOCore.PushSampleEvent = new FreeDOCore.PushSampleDelegate(ExternalInterface_PushSample);
			FreeDOCore.GetPbusLengthEvent = new FreeDOCore.GetPbusLengthDelegate(ExternalInterface_GetPbusLength);
			FreeDOCore.GetPbusDataEvent = new FreeDOCore.GetPbusDataDelegate(ExternalInterface_GetPbusData);
			FreeDOCore.KPrintEvent = new FreeDOCore.KPrintDelegate(ExternalInterface_KPrint);
			FreeDOCore.DebugPrintEvent = new FreeDOCore.DebugPrintDelegate(ExternalInterface_DebugPrint);
			FreeDOCore.FrameTriggerEvent = new FreeDOCore.FrameTriggerDelegate(ExternalInterface_FrameTrigger);
			FreeDOCore.Read2048Event = new FreeDOCore.Read2048Delegate(ExternalInterface_Read2048);
			FreeDOCore.GetDiscSizeEvent = new FreeDOCore.GetDiscSizeDelegate(ExternalInterface_GetDiscSize);
			FreeDOCore.OnSectorEvent = new FreeDOCore.OnSectorDelegate(ExternalInterface_OnSector);

			// Set up NVRAM save timer.
			this.nvramTimer.Elapsed += new ElapsedEventHandler(nvramTimer_Elapsed);
			this.nvramTimer.Enabled = false;

			///////////////
			// Allocate the VDLFrames

			// Get the size of a VDLFrame (must be "unsafe").
			int VDLFrameSize;
			unsafe
			{
				VDLFrameSize = sizeof(VDLFrame);
			}

			this.frame = new byte[VDLFrameSize];
			this.frameHandle = GCHandle.Alloc(this.frame, GCHandleType.Pinned);
			this.framePtr = this.frameHandle.AddrOfPinnedObject();

			this.pbusData = new byte[PBUS_DATA_MAX_SIZE];
			this.pbusDataHandle = GCHandle.Alloc(this.pbusData, GCHandleType.Pinned);
			this.pbusDataPtr = this.pbusDataHandle.AddrOfPinnedObject();

			this.nvramCopy = new byte[NVRAM_SIZE];
			this.nvramCopyHandle = GCHandle.Alloc(this.nvramCopy, GCHandleType.Pinned);
			this.nvramCopyPtr = this.nvramCopyHandle.AddrOfPinnedObject();
		}

		public IAudioPlugin AudioPlugin
		{
			get
			{
				return this.audioPlugin;
			}
		}

		public IInputPlugin InputPlugin
		{
			get
			{
				return this.inputPlugin;
			}
		}

		public int NvramSize
		{
			get
			{
				return NVRAM_SIZE;
			}
		}

		public void Destroy()
		{
			if (audioPlugin != null)
				audioPlugin.Destroy();

			if (inputPlugin != null)
				inputPlugin.Destroy();
		}

		private ConsoleState internalConsoleState;
		public ConsoleState State 
		{ 
			get
			{
				return internalConsoleState;
			}

			set
			{
				internalConsoleState = value;
				if (ConsoleStateChange != null)
					this.ConsoleStateChange(new ConsoleStateChangeEventArgs(value));
			}
		}

		public IntPtr CurrentFrame
		{
			get
			{
				return framePtr;
			}
		}

		public double CurrentFrameSpeed
		{
			get
			{
				return speedCalculator.CurrentAverage;
			}
		}

		public IGameSource GameSource { get; private set; }

		public string NvramFileName
		{
			get
			{
				return nvramFileName;
			}
		}

		public int AudioBufferMilliseconds
		{
			get
			{
				return audioBufferMilliseconds;
			}
			set
			{
				this.audioBufferMilliseconds = value;

				this.audioPlugin.BufferMilliseconds = this.audioBufferMilliseconds;

				// If emulation gets too far behind, we will "give up" on the schedule, and
				// reset our timings so that we accept a new schedule. This value determines
				// how far behind we can get before we "give up".

				// NOTE: We use the audio buffer size to determine how far off schedule we can go.
				//       If we're behind schedule more than a certain amount, we may as well give up!
				int milliseconds = Math.Max(this.audioBufferMilliseconds - 30, 25);
				this.maxLagTicks = milliseconds * PerformanceCounter.Frequency / 1000;
			}
		}

		public int CpuClockHertz
		{
			get
			{
				if (this.cpuClockHertz.HasValue)
					return this.cpuClockHertz.Value;
				else
					return 0; // Meh... good enough. I'd rather do this than blow up or let them set us to null.
			}
			set
			{
				lock (this.clockSpeedSemaphore)
				{
					this.cpuClockHertz = Math.Max(Math.Min(value, 125000000), 1250000);
				}
			}
		}

		public bool RenderHighResolution
		{
			get
			{
				return this.renderHighResolution ?? false;
			}
			set
			{
				this.renderHighResolution = value;
			}
		}

		public void Start(string biosRomFileName, IGameSource gameSource, string nvramFileName)
		{
			///////////
			// If they haven't initialized us properly, complain!

			if (!this.maxLagTicks.HasValue)
				throw new InvalidOperationException("Audio buffer not set!");

			if (!this.cpuClockHertz.HasValue)
				throw new InvalidOperationException("CPU Clock speed not set!");

			if (!this.renderHighResolution.HasValue)
				throw new InvalidOperationException("High resolution setting not set!");

			// Are we already started?
			if (this.workerThread != null)
				return;

			this.GameSource = gameSource;
			
			//////////////
			// Attempt to load a copy of the rom. Make sure it's the right size!
			try
			{
				this.biosRomCopy = System.IO.File.ReadAllBytes(biosRomFileName);
			}
			catch 
			{
				throw new BadBiosRomException();
			}

			// Also get outta here if the rom file isn't the right length.
			if (this.biosRomCopy.Length != ROM_SIZE)
				throw new BadBiosRomException();

			//////////////
			// Load a copy of the nvram.
			try
			{
				// If we previously had a game open, and they have now loaded
				// a new game, we want to make sure the game that was previously
				// open does not accidentally save.
				lock (nvramCopySemaphore)
				{
					// Note that we copy to the OLD nvram file name, just in case.
					if (this.nvramTimer.Enabled == true)
					{
						Memory.WriteMemoryDump(this.nvramFileName, this.nvramCopyPtr, NVRAM_SIZE);
					}

					this.nvramTimer.Enabled = false;

					// Read the new NVRAM into memory.
					Memory.ReadMemoryDump(this.nvramCopy, nvramFileName, NVRAM_SIZE);
				}
			}
			catch 
			{
				throw new BadNvramFileException();
			}

			// Freak out if the nvram isn't the right size.
			if (this.nvramCopy.Length != NVRAM_SIZE)
				throw new BadNvramFileException();
			
			// Remember the file name.
			this.nvramFileName = nvramFileName;

			//////////////
			// Attempt to start up the game source (it is allowed to throw errors).
			try
			{
				this.GameSource.Open();
			}
			catch
			{
				throw new BadGameRomException();
			}

			/////////////////
			// Initialize the core
			FreeDOCore.Initialize();

			////////////////
			// Set fix mode
			int fixMode = 0;
			GameRecord record = GameRegistrar.GetGameRecordById(this.GameSource.GetGameId());
			if (record != null)
			{
				if (       record.Id == "127BF39C" // Tsuukai Gameshow - Twisted (JP)
						|| record.Id == "A7C4EE53" // Twisted - The Game Show (US)

						|| record.Id == "813E41B1" // Space Hulk - Vengeance of the Blood Angels (EU-US)
						|| record.Id == "638812DE" // Blood Angels - Space Hulk (JP)

						|| record.Id == "F3AF1B13" // Crash 'n Burn (JP)
						|| record.Id == "217344B0" // Crash 'n Burn (US)

             
						|| (record.Publisher == "American Laser Games"))
					fixMode = fixMode | (int)FixMode.FIX_BIT_TIMING_1;
                if (record.Id == "260DC12D" // Twisted - The Game Show (EU)
                    || record.Id == "1757408B" // Seal of the pharaoh
                    || record.Id == "A4B2B740"// Seal of the pharaoh(jp)
                    ) fixMode = fixMode | (int)FixMode.FIX_BIT_TIMING_4;
                if (record.Id == "C39E4193" /* Phoenix 3*/
                      || record.Id == "6A4523F3" // Immercenary
                    || record.Id == "DBB419FA" // Street Fighter 2
                    || record.Id == "7340307E" // Street Fighter 2
                    || record.Id == "5282889F" // Street Fighter 2
                    || record.Id == "07C32F10" // Street Fighter 2
                    ) fixMode = fixMode | (int)FixMode.FIX_BIT_TIMING_5;
                if(                record.Id == "6A7B3EAE" // Wing Commander 3 disc 1
                      || record.Id == "A407D519" // Wing Commander 3 disc 1
                      || record.Id == "074BDE30" // Wing Commander 3 disc 2
                    || record.Id == "3EA4804D" // Wing Commander 3 disc 2
                    || record.Id == "7DDF4025" // Wing Commander 3 disc 3
                    || record.Id == "1A35B4B3" // Wing Commander 3 disc 3
                    || record.Id == "1E8D4F45" // Wing Commander 3 disc 4
                    || record.Id == "5DA4FF7F" // Wing Commander 3 disc 4
                )fixMode = fixMode | (int)FixMode.FIX_BIT_TIMING_6;
                if(
                    	   record.Id == "B347EE6D" // Scramble Cobra (demo) (JP)
						|| record.Id == "6A3AE6B5" // Scramble Cobra (EU)
						|| record.Id == "99670115" // Scramble Cobra (JP)
						|| record.Id == "9B87E5D7" // Scramble Cobra (US)
                    ) fixMode = fixMode | (int)FixMode.FIX_BIT_TIMING_3;

				if (       record.Id == "BD2BC660" // Lost Eden (US)
						|| record.Id == "EBE0915C" // Novastorm (US)
						|| record.Id == "1F059B8F" // Nova-Storm (JP)
						|| record.Id == "1A370EBA" // Microcosm (JP)
						|| record.Id == "B35C911D" // Microcosm (US)
						)
					fixMode = fixMode | (int)FixMode.FIX_BIT_TIMING_2;
			}
			FreeDOCore.SetFixMode(fixMode);

			/////////////////
			// Start the core thread
			this.InternalResume(false);
		}

		public void Stop()
		{
			// Are we already stopped?
			if (this.State == ConsoleState.Stopped)
				return;

			if (this.workerThread != null && this.workerThread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId)
				{} // The worker's trying to kill itself? I dunno what to do. Screw it.

			/////////////////
			// Stop the core.

			if (this.State == ConsoleState.Running)
				this.InternalPause();

			// Close the game source.
			try
			{
				this.GameSource.Close();
			}
			catch { } // Might happen, but I don't care. The game source shouldn't have done that.

			// Done!
			FreeDOCore.Destroy();
			this.State = ConsoleState.Stopped;
		}

		public void Pause()
		{
			if (this.State != ConsoleState.Running)
				return;

			this.InternalPause();
		}

		public void Resume()
		{
			if (this.State != ConsoleState.Paused)
				return;

			this.InternalResume(false);
		}

		public void AdvanceSingleFrame()
		{
			if (this.State != ConsoleState.Paused)
				return;

			this.InternalAdvanceSingleFrame();
		}

		public void SaveState(string saveStateFileName)
		{
			if (this.State == ConsoleState.Stopped)
				return;
			
			bool systemWasRunning = (this.State == ConsoleState.Running);
			if (systemWasRunning == true)
				this.InternalPause();

			BinaryWriter writer = null;
			try
			{
				string saveDirectory = Path.GetDirectoryName(saveStateFileName);
				if (!Directory.Exists(saveDirectory))
					Directory.CreateDirectory(saveDirectory);
				writer = new BinaryWriter(new FileStream(saveStateFileName, FileMode.Create));

				byte[] saveData = new byte[FreeDOCore.GetSaveSize()];
				unsafe
				{
					fixed (byte* saveDataPtr = saveData)
					{
						IntPtr pointer = new IntPtr(saveDataPtr);
						FreeDOCore.DoSave(pointer);
					}
				}
				writer.Write(saveData);
				writer.Close();
			}
			catch
			{
				throw;
			}
			finally
			{
				if (writer != null)
					writer.Close();

				if (systemWasRunning == true)
					this.InternalResume(false);
			}
		}

		public void LoadState(string saveStateFileName)
		{
			if (this.State == ConsoleState.Stopped)
				return;

			bool systemWasRunning = (this.State == ConsoleState.Running);
			if (systemWasRunning == true)
				this.InternalPause();

			BinaryReader reader = null;
			try
			{
				reader = new BinaryReader(new FileStream(saveStateFileName, FileMode.Open));
				byte[] saveData = reader.ReadBytes((int)FreeDOCore.GetSaveSize());
				unsafe
				{
					fixed (byte* saveDataPtr = saveData)
					{
						IntPtr pointer = new IntPtr(saveDataPtr);
						FreeDOCore.DoLoad(pointer);
					}
				}
				reader.Close();
			}
			catch
			{
				throw;
			}
			finally
			{
				if (reader != null)
					reader.Close();

				if (systemWasRunning == true)
					this.InternalResume(false);
			}
		}

		private void InternalResume(bool singleFrame)
		{
			stopWorkerSignal = singleFrame;

			// Start plugins
			this.audioPlugin.Start();

			// Start main emulation thread
			this.workerThread = new Thread(new ThreadStart(this.WorkerThread));
			this.workerThread.Priority = ThreadPriority.Highest;
			this.workerThread.Start();

			if (singleFrame == false)
				this.State = ConsoleState.Running;
		}

		private void InternalPause()
		{
			// Signal a shutdown and wait for it.
			stopWorkerSignal = true;
			this.workerThread.Join();
			this.workerThread = null;
			
			// Stop plugins.
			this.audioPlugin.Stop();

			this.State = ConsoleState.Paused;
		}

		private void InternalAdvanceSingleFrame()
		{
			this.InternalResume(true);
			this.InternalPause();
		}

		private void nvramTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			lock (this.nvramCopySemaphore)
			{
				Memory.WriteMemoryDump(this.nvramFileName, this.nvramCopyPtr, NVRAM_SIZE);
				this.nvramTimer.Enabled = false;
			}
		}

		#region FreeDO External Interface

		private void ExternalInterface_ReadRom(IntPtr romPointer)
		{
			////////
			// Now copy!
			unsafe
			{
				fixed (byte* sourceRomBytesPointer = this.biosRomCopy)
				{
					long* sourcePtr = (long*)sourceRomBytesPointer;
					long* destPtr = (long*)romPointer.ToPointer();
					for (int x = 0; x < ROM_SIZE / 8; x++)
					{
						*destPtr++ = *sourcePtr++;
					}
				}
			}
		}

		private unsafe void ExternalInterface_ReadNvram(IntPtr nvramPointer)
		{
			fixed (byte* sourcePtr = this.nvramCopy)
			{
				Utilities.Memory.CopyMemory(nvramPointer, new IntPtr((int)sourcePtr), NVRAM_SIZE);
			}
		}

		private unsafe void ExternalInterface_WriteNvram(IntPtr nvramPointer)
		{
			lock (this.nvramCopySemaphore)
			{
				Utilities.Memory.CopyMemory(new IntPtr((int)this.nvramCopyPtr), nvramPointer, NVRAM_SIZE);
				this.nvramTimer.Enabled = false;
				this.nvramTimer.Enabled = true;
			}
		}

		private IntPtr ExternalInterface_SwapFrame(IntPtr currentFrame)
		{
			// This get signaled in non-multi task mode at the end of each frame.
			// I'm not entirely certain why.

			this.isSwapFrameSignaled = true;
			return currentFrame;
		}

		private int lastAudioSampleCount = 0;
		private void ExternalInterface_PushSample(uint dspSample)
		{
			lastAudioSampleCount++;
			if (this.audioPlugin != null)
				this.audioPlugin.PushSample(dspSample);
		}

		private int ExternalInterface_GetPbusLength()
		{
			// Ask input plugin for Pbus data.
			byte[] pbusDataCopy = inputPlugin.GetPbusData();

			if (pbusDataCopy == null)
				return 0;

			//////////////////
			// Copy the pbus data. We'll return it to the core soon when it asks.
			int copyLength = pbusDataCopy.Length;
			if (copyLength > 16)
				copyLength = 16;

			unsafe
			{
				fixed (byte* srcPtr = pbusDataCopy)
				{
					Utilities.Memory.CopyMemory(this.pbusDataPtr, new IntPtr(srcPtr), copyLength);
				}
			}

			return (int)copyLength;
		}

		private IntPtr ExternalInterface_GetPbusData()
		{
			return this.pbusDataPtr;
		}

		private void ExternalInterface_KPrint(IntPtr value)
		{
			if (FourDO.Utilities.Globals.RunOptions.PrintKPrint)
				Console.Write((char)value);
		}

		private void ExternalInterface_DebugPrint(IntPtr value)
		{
		}

		private void ExternalInterface_FrameTrigger()
		{
			// We got a signal that multi-task mode has completed a frame!

			// Done with this frame.
			this.isSwapFrameSignaled = true;
			
			// Use of multi-task mode requires that we ask the core to update our VDL frame for us.
			FreeDOCore.DoFrameMultitask(this.framePtr);
		}

		private void ExternalInterface_Read2048(IntPtr buffer)
		{
			this.GameSource.ReadSector(buffer, this.currentSector);
		}

		private int ExternalInterface_GetDiscSize()
		{
			return this.GameSource.GetSectorCount();
		}

		private void ExternalInterface_OnSector(int sectorNumber)
		{
			currentSector = sectorNumber;
		}

		#endregion // FreeDO External Interface

		#region Worker Thread

		private void WorkerThread()
		{
			const int MAXIMUM_FRAME_COUNT = 100;

			long lastSample = 0;
			long lastTarget = 0;
			long targetPeriod = 0;
			int lastFrameCount = 0;
			int currentHertz = 12500000;

			bool highResolution = false;

			int overshootSleepThreshold = System.Math.Max(5, TimingHelper.GetResolution());

			// Calculate a default target period in case there are no
			// audio samples for the upcoming frame (which is pretty unlikely).
			targetPeriod = (long)(PerformanceCounter.Frequency / 60.0);

			int sleepTime = 0;
			do
			{
				// We're awake! Wreak havoc!

				/////////////////////
				// If we need to, update some core values.

				// Set arm clock?
				lock (this.clockSpeedSemaphore)
				{
					if (this.cpuClockHertz.HasValue && currentHertz != this.cpuClockHertz.Value)
					{
						currentHertz = this.cpuClockHertz.Value;
						FreeDOCore.SetArmClock(currentHertz);
					}
				}

				// Set high resolution mode?
				if (highResolution != this.renderHighResolution.Value)
				{
					highResolution = this.renderHighResolution.Value;
					FreeDOCore.SetTextureQuality(highResolution ? 1 : 0);
				}

				// Execute a frame.
				isSwapFrameSignaled = false;
				lastFrameCount = 0;

				var frameWatch = new PerformanceStopWatch();
				frameWatch.Start();
				do
				{
					if (this.doFreeDOMultitask)
						FreeDOCore.DoExecuteFrameMultitask(this.framePtr);
					else
						FreeDOCore.DoExecuteFrame(this.framePtr);

					lastFrameCount++;
				} while (isSwapFrameSignaled == false && lastFrameCount < MAXIMUM_FRAME_COUNT);
				frameWatch.Stop();

				///////////
				// Signal completion.
				var doneWatch = new PerformanceStopWatch();
				doneWatch.Start();
				if (FrameDone != null)
					FrameDone(this, new EventArgs());
				doneWatch.Stop();

				///////////
				// Set new target period depending on how many audio samples were pushed.
				if (lastAudioSampleCount > 0)
				{
					// Figure out how much time was emulated.
					double lastFrameSeconds = lastAudioSampleCount / (double)44100;

					// If there were multiple frames, account for this.
					if (lastFrameCount > 0)
						lastFrameSeconds = lastFrameSeconds / lastFrameCount;

					targetPeriod = (long)(PerformanceCounter.Frequency * lastFrameSeconds);

					lastAudioSampleCount = 0;
				}

				///////////
				// Identify how long to sleep (if at all).
				long currentOvershoot = 0;
				long cheatAmount = 0;
				if (lastSample == 0)
				{
					// This is the first sample; this one's a freebee.
					lastSample = PerformanceCounter.Current;
					lastTarget = lastSample; // Pretend we're right on target!
					sleepTime = 0;
				}
				else
				{
					long currentSample = PerformanceCounter.Current;
					long currentDelta = currentSample - lastSample;

					long currentTarget = lastTarget + targetPeriod * lastFrameCount;
					currentOvershoot = currentTarget - currentSample;

					speedCalculator.AddSample((currentDelta / (double)lastFrameCount) / (double)PerformanceCounter.Frequency);

					// Are we ahead of schedule?
					if (currentOvershoot > 0)
					{
						// We're AHEAD OF schedule.
						// This is easy. We'll sleep if we're far enough ahead.
						sleepTime = (int)(((currentOvershoot) / (double)PerformanceCounter.Frequency) * 1000);

						// However, Our sleeps must always be at least the system's minimum sleep granularity.
						if (sleepTime < overshootSleepThreshold)
							sleepTime = 0;
					}
					else
					{
						// We're BEHIND schedule! No sleeping!
						sleepTime = 0;

						if (currentOvershoot < -maxLagTicks)
						{
							// We're REALLY BEHIND schedule. HOLY SHIT!
							// There's no way we can catch up. Just "give up" and accept a new target schedule.
							cheatAmount = currentOvershoot;
							currentTarget = currentSample;
						}
					}

					// Save the values for next time.
					lastSample = currentSample;
					lastTarget = currentTarget;
				}

				// Let the audio plugin know the current "schedule".
				this.audioPlugin.FrameDone(cheatAmount > 0 ? 0 : currentOvershoot, cheatAmount);

				/////////////
				// Sleep if we've been instructed to.
				var sleepWatch = new PerformanceStopWatch();
				sleepWatch.Start();
				if (sleepTime > 0 && !this.stopWorkerSignal)
					Thread.Sleep(sleepTime);
				sleepWatch.Stop();
				
				// Some debug output.
				if (FourDO.Utilities.Globals.RunOptions.LogCPUTiming)
					Trace.WriteLine(string.Format("CpuTiming - Frames:\t{0}\tSleepReq:\t{1:00.00}\tSlept:\t{2:00.00}\tSleepScrewup:\t{3:00.00}\tFrameTm:\t{4:00.00}\tDoneTm:\t{5:00.00}\tTargetOff:\t{6:00.00}\tCheatTm:\t{7:00.00}"
						, lastFrameCount
						, sleepTime
						, sleepWatch.TotalMilliseconds
						, sleepWatch.TotalMilliseconds - sleepTime
						, frameWatch.TotalMilliseconds
						, doneWatch.TotalMilliseconds
						, ((currentOvershoot) / (double)PerformanceCounter.Frequency) * 1000
						, ((cheatAmount) / (double)PerformanceCounter.Frequency) * 1000
						));

			} while (this.stopWorkerSignal == false);
		}

		#endregion // Worker Thread
	}
}
