using FourDO.Emulation.GameSource;
using FourDO.Emulation.FreeDO;
using FourDO.Emulation.Plugins;
using FourDO.Utilities;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Timers;
using System.Threading;

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

		private volatile FrameSpeedCalculator speedCalculator = new FrameSpeedCalculator(4);

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

		public void Start(string biosRomFileName, IGameSource gameSource, string nvramFileName)
		{
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
			// Start the core.
			FreeDOCore.Initialize();
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
			this.workerThread = new Thread(new ThreadStart(this.WorkerThread));
			this.workerThread.Priority = ThreadPriority.Highest;
			this.workerThread.Start();
			this.audioPlugin.Start();

			if (singleFrame == false)
				this.State = ConsoleState.Running;
		}

		private void InternalPause()
		{
			// Signal a shutdown and wait for it.
			stopWorkerSignal = true;
			this.workerThread.Join();
			this.workerThread = null;
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

		private void ExternalInterface_PushSample(uint dspSample)
		{
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
			long targetPeriod = PerformanceCounter.Frequency / 60;
			int lastFrameCount = 0;

			int sleepTime = 0;
			do
			{
				///////////
				// Identify how long to sleep.
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
					long currentRemaining = currentTarget - currentSample;

					speedCalculator.AddSample((currentDelta / (double)lastFrameCount) / (double)PerformanceCounter.Frequency);

					// Are we behind schedule?
					if (currentRemaining < 0)
					{
						// We ARE behind schedule. Oh crap!
						if (currentRemaining < -(targetPeriod * lastFrameCount) * 10)
						{
							// We're REALLY behind schedule. HOLY SHIT!
							// There's no way we can catch up. Just give up and give a new target.
							currentTarget = currentSample;
							sleepTime = 0;
						}
						else
						{
							// We're not that far behind schedule. Phew!
							sleepTime = 0;
						}
					}
					else
					{
						// Not behind schedule.
						double wait = (currentRemaining) / (double)PerformanceCounter.Frequency;
						sleepTime = (int)(wait * 1000);
					}

					// Save the values for next time.
					lastSample = currentSample;
					lastTarget = currentTarget;
				}

				// Sleep, but always use a value of at least 0.
				// (We're a high priority thread, but we can't just choke everyone.)
				if (sleepTime < 0)
					sleepTime = 0;
				Thread.Sleep(sleepTime);

				///////////
				// We're awake! Wreak havoc!

				// Execute a frame.
				isSwapFrameSignaled = false;
				lastFrameCount = 0;
				do
				{
					if (this.doFreeDOMultitask)
						FreeDOCore.DoExecuteFrameMultitask(this.framePtr);
					else
						FreeDOCore.DoExecuteFrame(this.framePtr);

					lastFrameCount++;
				} while (isSwapFrameSignaled == false && lastFrameCount < MAXIMUM_FRAME_COUNT);
				
				// Signal completion.
				if (FrameDone != null)
					FrameDone(this, new EventArgs());
			} while (stopWorkerSignal == false);
		}

		#endregion // Worker Thread
	}
}
