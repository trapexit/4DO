using SlimDX.DirectSound;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using PerformanceCounter = FourDO.Utilities.PerformanceCounter;

namespace FourDO.Emulation.Plugins.Audio.JohnnyAudio
{
	internal class JohnnyAudioPlugin : IAudioPlugin
	{
		private const string LOG_PREFIX = "JohnnyAudio - ";

		private int bufferDelayMilliseconds;

		// A guess as to how long it takes to copy to the play buffer.
		private const int PLAY_COPY_GUESS_MILLISECONDS = 30;
		private int play_copy_guess_offset;

		private const int BUFFER_MIN_MILLISECONDS = PLAY_COPY_GUESS_MILLISECONDS;
		private const int BUFFER_MAX_MILLISECONDS = 1000;
		private int buffer_min_offset;
		private int buffer_max_offset;
		private int buffer_base_offset;

		private bool initialized = false;

		SlimDX.Multimedia.WaveFormat bufferFormat;
		SoundBufferDescription bufferDescription;

		DirectSound directSound;
		SecondarySoundBuffer playBuffer;

		private double volumeLinear = 1.0;

		internal JohnnyAudioPlugin()
		{
		}

		#region IAudioPlugin Implementation

		public double Volume
		{
			get
			{
				return this.volumeLinear;
			}
			set
			{
				// DirectSound documentation says that the minimum is -10000, but I can't hear anything past -7000
				const int MIN_DIRECTSOUND_VOLUME = -7000;

				this.volumeLinear = value;
				if (this.playBuffer != null)
				{
					if (this.volumeLinear == 0)
						this.playBuffer.Volume = -10000;
					else
					{
						// I've also made the volume LESS logarithmic than DirectSound prefers to be.
						this.playBuffer.Volume = (int)(MIN_DIRECTSOUND_VOLUME * (1 - Math.Pow(volumeLinear, .5)));
					}
				}
			}
		}

		public int BufferMilliseconds
		{
			get
			{
				return bufferDelayMilliseconds;
			}
			set
			{
				bufferDelayMilliseconds = value;
				this.RecalculateBufferOffsets(this.bufferDelayMilliseconds);
			}
		}

		public bool GetHasSettings()
		{
			return false;
		}

		public bool GetSupportsVolume()
		{
			return true;
		}

		public void ShowSettings(IWin32Window owner)
		{
			return;
		}

		private int currentWritePosition;

		private bool scheduleAccepted = false;

		private const int TEMP_BUFFER_SIZE = 512;
		private int currentTempBufferPosition = 0;
		private uint[] tempBuffer = new uint[TEMP_BUFFER_SIZE];

		private uint[] emptyBuffer;

		public void PushSample(uint dspSample)
		{
			this.InternalPushSample(dspSample);
		}

		public void Destroy()
		{
			if (directSound != null)
				directSound.Dispose();
		}

		public void Start()
		{
			this.InternalStart();
		}

		public void Stop()
		{
			this.InternalStop();
		}

		public void FrameDone(long currentOvershoot, long adjustmentPosted)
		{
			this.InternalFrameDone(currentOvershoot, adjustmentPosted);
		}

		#endregion // IAudioPlugin Implementation

		private void InternalStart()
		{
			this.Initialize();

			this.scheduleAccepted = false;
			this.playBuffer.Play(0, PlayFlags.Looping);
		}

		private void InternalStop()
		{
			this.playBuffer.Stop();
			this.playBuffer.Write<uint>(this.emptyBuffer, 0, LockFlags.None);
		}

		private void Initialize()
		{
			if (this.initialized)
				return;

			this.initialized = true;

			this.directSound = new DirectSound(DirectSoundGuid.DefaultPlaybackDevice);
			this.directSound.SetCooperativeLevel(FourDO.Program.GetMainWindowHwnd(), CooperativeLevel.Normal);

			this.bufferFormat = new SlimDX.Multimedia.WaveFormat();
			this.bufferFormat.Channels = 2;
			this.bufferFormat.BitsPerSample = 16;
			this.bufferFormat.FormatTag = SlimDX.Multimedia.WaveFormatTag.Pcm;
			this.bufferFormat.SamplesPerSecond = 44100;
			this.bufferFormat.BlockAlignment = (short)(bufferFormat.Channels * (bufferFormat.BitsPerSample / 8));
			this.bufferFormat.AverageBytesPerSecond = bufferFormat.SamplesPerSecond * bufferFormat.BlockAlignment;

			this.bufferDescription = new SoundBufferDescription();
			this.bufferDescription.Flags = BufferFlags.ControlVolume | BufferFlags.GlobalFocus;
			this.bufferDescription.Format = this.bufferFormat;
			this.bufferDescription.SizeInBytes = 1024 * 128;

			if (bufferDescription.SizeInBytes % TEMP_BUFFER_SIZE != 0)
				throw new Exception("Audio buffer size needs to be a multiple of the temporary buffer size");

			this.playBuffer = new SecondarySoundBuffer(directSound, this.bufferDescription);

			this.emptyBuffer = new uint[this.bufferDescription.SizeInBytes / sizeof(uint)];

			this.play_copy_guess_offset = (int)(this.bufferFormat.SamplesPerSecond * (PLAY_COPY_GUESS_MILLISECONDS / (double)1000));
			this.play_copy_guess_offset *= this.bufferFormat.BlockAlignment;

			this.RecalculateBufferOffsets(this.bufferDelayMilliseconds);

			this.offTimeTicksThreshhold = (long)((TIME_OFF_DURATION_MILLISECONDS / ((double)1000) * PerformanceCounter.Frequency));
		}

		public void RecalculateBufferOffsets(int bufferMilliseconds)
		{
			if (!this.initialized)
				return;

			// We will accept only values within a range.
			int utilizedDelay = Math.Max(bufferMilliseconds, BUFFER_MIN_MILLISECONDS);
			utilizedDelay = Math.Min(bufferMilliseconds, BUFFER_MAX_MILLISECONDS);

			this.buffer_min_offset = (int)(this.bufferFormat.SamplesPerSecond * (BUFFER_MIN_MILLISECONDS / (double)1000));
			this.buffer_base_offset = (int)(this.bufferFormat.SamplesPerSecond * (utilizedDelay / (double)1000));
			this.buffer_max_offset = (int)(this.bufferFormat.SamplesPerSecond * (utilizedDelay + 50 / (double)1000));

			this.buffer_min_offset *= this.bufferFormat.BlockAlignment;
			this.buffer_max_offset *= this.bufferFormat.BlockAlignment;
			this.buffer_base_offset *= this.bufferFormat.BlockAlignment;
		}

		private void InternalPushSample(uint dspSample)
		{
			if (playBuffer == null)
				return;

			tempBuffer[currentTempBufferPosition] = dspSample;
			currentTempBufferPosition++;
			if (currentTempBufferPosition == TEMP_BUFFER_SIZE)
			{
				currentTempBufferPosition = 0;

				this.PushTempBlock();
			}
		}

		private void PushTempBlock()
		{
			// If we haven't recieved a schedule, we don't know where to place this on the play buffer.
			// Give up!
			if (!this.scheduleAccepted)
				return;

			const int TEMP_COPY_SIZE = TEMP_BUFFER_SIZE * sizeof(uint);

			// Check to see if we're about to write over the play buffer (an unforgiveable offense!!)
			int playPosition = this.playBuffer.CurrentPlayPosition;
			int checkPositionMin = this.AddToPosition(this.currentWritePosition, -this.play_copy_guess_offset);
			int checkPositionMax = this.AddToPosition(this.currentWritePosition, TEMP_COPY_SIZE);
			bool aboutToWriteOverPlayPosition = this.GetRealPositionInclusive(checkPositionMin, checkPositionMax, playPosition);
			if (aboutToWriteOverPlayPosition)
			{
				// WOAH! Don't write over the play buffer! That would suck! 
				// The system must be running slowly. Send the write buffer way back behind the play buffer.
				if (FourDO.Utilities.Globals.RunOptions.LogAudioDebug)
					Trace.WriteLine(LOG_PREFIX + "Resetting:About to write over play buffer");
				this.ResetWritePosition();
			}
			
			//////////////////////
			// Copy it wherever the current position is
			if (this.currentWritePosition + TEMP_COPY_SIZE > bufferDescription.SizeInBytes)
			{
				int firstWriteSize = (bufferDescription.SizeInBytes - this.currentWritePosition);
				playBuffer.Write<uint>(tempBuffer, 0, firstWriteSize / sizeof(uint), bufferDescription.SizeInBytes - firstWriteSize, LockFlags.None);
				playBuffer.Write<uint>(tempBuffer, firstWriteSize / sizeof(uint), (TEMP_COPY_SIZE - firstWriteSize) / sizeof(uint), 0, LockFlags.None);
			}
			else
			{
				playBuffer.Write<uint>(this.tempBuffer, this.currentWritePosition, LockFlags.None);
			}
			this.currentWritePosition += TEMP_COPY_SIZE;
			this.currentWritePosition = this.currentWritePosition % bufferDescription.SizeInBytes;
		}

		const int TIME_OFF_DURATION_MILLISECONDS = 500;
		private long offTimeTicksThreshhold;
		private bool offBuffer = false;
		private long? offBufferTimeStamp = null;

		public void InternalFrameDone(long currentOvershoot, long adjustmentPosted)
		{
			int? expectedPosition = this.GetExpectedWritePosition();
			
			this.SetExpectedWritePosition(this.currentWritePosition, currentOvershoot);

			int? newPos = this.GetExpectedWritePosition();
			if (!newPos.HasValue)
				return; // Not expected!... but might happen with multithreading?

			int playpos = this.playBuffer.CurrentPlayPosition;
			int playdiff = this.GetRealPositionDiff(newPos ?? 0, playpos);

			////////////////////
			// Figure out if we should reset the write position.
			bool resetPosition = false;

			// Always reset if:
			//   * we haven't already accepted a schedule
			//   * OR there was an adjustment in the core emulation timing
			if (!this.scheduleAccepted || adjustmentPosted != 0)
			{
				if (FourDO.Utilities.Globals.RunOptions.LogAudioDebug)
					Trace.WriteLine(LOG_PREFIX + "Resetting:Init or CPU hint");
				resetPosition = true;
			}
			else
			{
				// Check the position to ensure it's in "bounds".
				int maxPosition = this.AddToPosition(newPos.Value, -this.buffer_min_offset);
				int minPosition = this.AddToPosition(newPos.Value, -this.buffer_max_offset);
				if (this.GetRealPositionInclusive(minPosition, maxPosition, playpos))
				{
					// You win this round, play position.
					this.offBuffer = false;
				}
				else
				{
					// Well, it may be out of bounds, but we won't do anything about it until a certain time threshold hits.
					if (!this.offBuffer)
					{
						// First offense. They get a pass.
						this.offBufferTimeStamp = PerformanceCounter.Current;
						this.offBuffer = true;
					}
					else
					{
						// It was off last time too!
						
						// Check duration.
						if ((PerformanceCounter.Current - this.offBufferTimeStamp) > this.offTimeTicksThreshhold)
						{
							if (FourDO.Utilities.Globals.RunOptions.LogAudioDebug)
								Trace.WriteLine(LOG_PREFIX + "Resetting:Too long out of bounds");
							resetPosition = true;
							this.offBuffer = false;
						}
					}
				}
			}

			////////////////////
			// Reset the write position if required.
			if (resetPosition)
				this.ResetWritePosition();

			if (FourDO.Utilities.Globals.RunOptions.LogAudioTiming)
				Trace.WriteLine(string.Format(LOG_PREFIX + "OldExp:\t{0}\tNewExp\t{1}\tAdjust\t{2}\tPlayPos\t{3}\tPlayDiff\t{4}\tReset?\t{5}\tOffBuf?\t{6}", 
					expectedPosition.ToString(),
					newPos.ToString(), 
					(adjustmentPosted > 0).ToString(),
					playpos,
					playdiff,
					resetPosition,
					offBuffer
					));

			// (Now we're married to a schedule)
			this.scheduleAccepted = true;
		}

		#region General Functions

		private void ResetWritePosition()
		{
			this.currentWritePosition = this.AddToPosition(this.playBuffer.CurrentPlayPosition, this.buffer_base_offset);
		}

		/// <summary>
		/// This will "clamp" a position to a real buffer position.
		/// If it passes the end of the buffer, it'll be placed at
		/// the start.
		/// If it preceeds the beginning of the buffer, it'll be
		/// placed at the end.
		/// </summary>
		private int GetRealBufferPosition(long possiblyOutOfBoundsPosition)
		{
			if (possiblyOutOfBoundsPosition >= 0)
				return (int)(possiblyOutOfBoundsPosition % this.bufferDescription.SizeInBytes);
			else
			{
				int returnValue = (int)(this.bufferDescription.SizeInBytes - (-possiblyOutOfBoundsPosition % this.bufferDescription.SizeInBytes));
				if (returnValue == this.bufferDescription.SizeInBytes)
					return 0;
				else
					return returnValue;
			}
		}

		private int AddToPosition(int currentPosition, int positionToAdd)
		{
			currentPosition = currentPosition + positionToAdd;
			currentPosition = currentPosition % this.bufferDescription.SizeInBytes;
			if (currentPosition >= 0)
				return currentPosition;
			return this.bufferDescription.SizeInBytes + currentPosition;
		}

		private int GetRealPositionDiff(int positionNumHigher, int positionNumLower)
		{
			if (positionNumHigher > positionNumLower)
				return positionNumHigher - positionNumLower;
			else
				return positionNumHigher + (this.bufferDescription.SizeInBytes - positionNumLower);
		}

		private bool GetRealPositionInclusive(int positionBoundMin, int positionBoundMax, int sampleTest)
		{
			if (positionBoundMax > positionBoundMin)
				return (sampleTest > positionBoundMin) && (sampleTest < positionBoundMax);
			else
				return (sampleTest < positionBoundMax) || (sampleTest > positionBoundMin);
		}

		#endregion

		#region Write position estimation / management

		private int? expectedWritePosition;
		private long expectedWritePositionTimeStamp;
		private long expectedWritePositionOvershoot;
		private object expectedWritePositionSemaphore = new object();

		private void SetExpectedWritePosition(int? newExpectedPosition, long currentOvershoot)
		{
			lock (expectedWritePositionSemaphore)
			{
				this.expectedWritePosition = newExpectedPosition;
				this.expectedWritePositionOvershoot = currentOvershoot;
				this.expectedWritePositionTimeStamp = PerformanceCounter.Current;
			}
		}

		private int? GetExpectedWritePosition()
		{
			lock (expectedWritePositionSemaphore)
			{
				// If we've gotten no hint, we have no known position!
				if (!this.expectedWritePosition.HasValue)
					return null;

				double hintDelaySeconds =
					(PerformanceCounter.Current - this.expectedWritePositionTimeStamp - this.expectedWritePositionOvershoot)
					/ (double)PerformanceCounter.Frequency;

				int hintDelaySamples = (int)(hintDelaySeconds * this.bufferDescription.Format.SamplesPerSecond);
				int expectedWritePosition = this.GetRealBufferPosition(this.expectedWritePosition.Value + hintDelaySamples * this.bufferDescription.Format.BlockAlignment);

				return expectedWritePosition;
			}
		}

		#endregion
	}
}
