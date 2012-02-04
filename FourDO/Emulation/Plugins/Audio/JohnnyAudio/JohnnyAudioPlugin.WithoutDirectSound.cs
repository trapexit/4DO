//////////////////////////////////////////////////////////
// JMK NOTES:
//
////////////////////////
//     !!! WARNING !!!
// THIS CODE IS NOT TO BE TRUSTED!
// I've kept it around for diagnostic purposes.
//
// This plugin assumes that the playback "callback" occurs at
// regular intervals, but on some machines this isn't true. Thus, 
// it will produce "stuttering issues" on those machines.
//
////////////////////////
//
// The audio plugin has a large local buffer that soaks up any input
// from the core emulation. This buffer is populated continuously, and
// when the buffer is "exceeded", the write position is reset to the
// start of the buffer.
//
// Meanwhile, audio processing continues in another thread, and asks
// for data via a callback. This callback chooses the appropriate data
// from the large local buffer. In most cases, it is best to choose 
// the block of data immediately after the one previously selected.
// Any other selections will be noticeable as an audio "glitch".
//
// Unfortunately, the emulation is not guaranteed to always be on
// schedule. And so, the audio plugin "buffers" itself from the audio
// data. There is a certain allowable buffer range that is monitored.
// When the read position goes outside the buffer, it gets reset 
// (which knowingly causes a glitch).
//
// I've used a libary from here to play the audio from a buffer
// http://www.codeproject.com/KB/audio-video/cswavplay.aspx
// Consent explicitly recieved on August 3rd via email (from Ianier to Johnny).
//
//////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using WaveLib;
using PerformanceCounter = FourDO.Utilities.PerformanceCounter;

namespace FourDO.Emulation.Plugins.Audio.JohnnyAudio
{
	internal class JohnnyAudioPlugin : IAudioPlugin
	{
		private const string LOG_PREFIX = "JohnnyAudioPlugin - ";

		private bool audioEnabled;
		private int bytesPerSample;

		private double volumeLinear = 1.0;
		private double volumeLogarithmic = 1.0;

		private bool stopped = true;

		private const int FORMAT_SAMPLES_PER_SECOND = 44100;
		private const int FORMAT_CHANNELS = 2;

		private const int LOCAL_BUFFER_SAMPLES = 4096 * 18;

		private const int PLAY_BUFFER_SAMPLES = 1024;
		private const int PLAY_BUFFER_COUNT = 3;

		private WaveOutPlayer player;
		private WaveFormat format;

		private readonly int localBufferLengthInSamples;

		private byte[] localBuffer;
		private IntPtr localBufferPtr;
		private GCHandle localBufferHandle;
		private volatile int localBufferWritePosition;
		private volatile Stream localBufferReadStream;

		byte[] copyBuffer;
		byte[] emptyBuffer;

		internal JohnnyAudioPlugin()
		{
			this.IdentifyBytesPerSample();

			// Create a buffer on our side to write into.
			this.localBuffer = new byte[LOCAL_BUFFER_SAMPLES * this.bytesPerSample];
			this.localBufferHandle = GCHandle.Alloc(this.localBuffer, GCHandleType.Pinned);
			this.localBufferPtr = this.localBufferHandle.AddrOfPinnedObject();
			this.localBufferWritePosition = 0;

			this.localBufferLengthInSamples = this.localBuffer.Length / bytesPerSample;

			// Create a stream to our buffer that the audio player will be reading from.
			this.localBufferReadStream = new MemoryStream(this.localBuffer);

			// Create buffers we'll be using to copy bytes to the player.
			copyBuffer = new byte[PLAY_BUFFER_SAMPLES * this.bytesPerSample];
			emptyBuffer = new byte[PLAY_BUFFER_SAMPLES * this.bytesPerSample];
		}

		#region IAudioPlugin Implementation

		public double Volume
		{
			get
			{
				return volumeLinear;
			}
			set
			{
				volumeLinear = value;
				this.volumeLogarithmic = volumeLinear * volumeLinear * volumeLinear;
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

		public void PushSample(uint dspSample)
		{
			uint rightSample = dspSample & 0xFFFF0000;
			uint leftSample = dspSample & 0x0000FFFF;
			if (volumeLogarithmic != 1)
			{
				rightSample = rightSample >> 16;
				rightSample = (uint)((short)rightSample * volumeLogarithmic);
				rightSample = rightSample << 16;
				leftSample = (uint)((short)leftSample * volumeLogarithmic) & 0x0000FFFF;
			}

			unsafe
			{
				int bytesPerChannel = bytesPerSample / FORMAT_CHANNELS;
				if (bytesPerChannel == 2)
				{
					UInt32* localBufferSamplePointer = (UInt32*)this.localBufferPtr.ToPointer();
					localBufferSamplePointer[this.localBufferWritePosition++] = leftSample | rightSample;
					//Trace.WriteLine(@"secondaryBuffer.Write<uint>(new uint[] { " + (leftSample | rightSample).ToString() + " }, x, LockFlags.None);");
				}
				else if (bytesPerChannel == 1)
				{
					UInt16* localBufferSamplePointer = (UInt16*)this.localBufferPtr.ToPointer();
					localBufferSamplePointer[this.localBufferWritePosition++] = (UInt16)(
							 (((rightSample & 0xFF000000) >> 16) + 0x8000)
							 | (((leftSample >> 8) + 0x80) & 0xFF)
							 );

				}
				if (this.localBufferWritePosition == this.localBufferLengthInSamples)
					this.localBufferWritePosition = 0;
			}
		}

		public void Destroy()
		{
			if (player != null)
				player.Dispose();
		}

		public void Start()
		{
			this.InternalStart();
		}

		public void Stop()
		{
			this.InternalStop();
		}

		/// <summary>
		/// Notify frame completion
		/// </summary>
		/// <param name="currentOvershoot">The current deviation from "standard" schedule.</param>
		/// <param name="adjustmentPosted">The schedule adjustment posted on this frame (if any).</param>
		public void FrameDone(long currentOvershoot, long adjustmentPosted)
		{
			this.SetExpectedWritePosition(this.localBufferWritePosition, currentOvershoot);
		}

		#endregion // IAudioPlugin Implementation

		private void InternalStart()
		{
			format = new WaveFormat(FORMAT_SAMPLES_PER_SECOND, 8 * bytesPerSample / FORMAT_CHANNELS, FORMAT_CHANNELS);
			if (this.audioEnabled == false)
				return;

			try
			{
				if (player == null)
					player = new WaveOutPlayer(-1, format, PLAY_BUFFER_SAMPLES * this.bytesPerSample, PLAY_BUFFER_COUNT, new WaveLib.BufferFillEventHandler(this.FillerCallback));
				stopped = false;
			}
			catch
			{
				// TODO: Shouldn't get this if initialization worked right.
			}
		}

		private void InternalStop()
		{
			stopped = true; // We'll start sending blank buffers to the audio thread.
		}

		private long? firstSample = null;
		private void FillerCallback(IntPtr destinationBuffer, int copySize)
		{
			if (firstSample.HasValue)
			{
				//Trace.WriteLine(string.Format("AUDVER - \t{0}\tFreq:\t{1}", PerformanceCounter.Current - firstSample.Value, PerformanceCounter.Frequency));
			}
			else
				firstSample = PerformanceCounter.Current;

			int? expectedPosition = this.GetExpectedWritePosition();

			// If we're stopped, just copy empty audio data.
			if (stopped || !expectedPosition.HasValue)
			{
				Marshal.Copy(emptyBuffer, 0, destinationBuffer, copySize);
				return;
			}

			///////////////
			// Determine the appropriate place to read.
			const int BUFFER_MIN_MILLISECONDS = 75;
			const int BUFFER_MAX_MILLISECONDS = 125;

			const int BUFFER_MIN_SAMPLES = FORMAT_SAMPLES_PER_SECOND * BUFFER_MIN_MILLISECONDS / 1000;
			const int BUFFER_MAX_SAMPLES = FORMAT_SAMPLES_PER_SECOND * BUFFER_MAX_MILLISECONDS / 1000;

			const int BUFFER_MEDIAN_SAMPLES = (BUFFER_MIN_SAMPLES + BUFFER_MAX_SAMPLES) / 2;

			int minAcceptableSample = this.GetRealBufferPosition(expectedPosition.Value - (BUFFER_MAX_SAMPLES));
			int maxAcceptableSample = this.GetRealBufferPosition(expectedPosition.Value - (BUFFER_MIN_SAMPLES));

			int theCurrentReadSample = ((int)this.localBufferReadStream.Position) / bytesPerSample;

			bool readPositionAdjusted = false;
			int newSample = 0;
			if (!this.GetRealSampleInclusive(minAcceptableSample, maxAcceptableSample, theCurrentReadSample))
			{
				readPositionAdjusted = true;
				newSample = this.GetRealBufferPosition(expectedPosition.Value - (BUFFER_MEDIAN_SAMPLES));
				this.localBufferReadStream.Position = newSample * bytesPerSample;
			}

			//////////////////////
			// Debug!!!
			int currentWriteSampleAlso = this.localBufferWritePosition;
			int oldDiffWriteGuess = this.GetRealSampleDiff(expectedPosition.Value, theCurrentReadSample);
			int newDiffWriteGuess = this.GetRealSampleDiff(expectedPosition.Value, newSample);
			int oldDiffWriteActual = this.GetRealSampleDiff(currentWriteSampleAlso, theCurrentReadSample);
			int newDiffWriteActual = this.GetRealSampleDiff(currentWriteSampleAlso, newSample);

			/*
			Trace.WriteLine(string.Format("AUDIO - \tAdjusted:\t{0}\tMinAccept:\t{1}\tMaxAccept:\t{2}\tOldSam:\t{3}\tNewSam:\t{4}\tOldDiffWriteGuess:\t{5}\tNewDiffWriteGuess:\t{6}\tOldDiffWriteAct:\t{7}\tNewDiffWriteAct:\t{8}"
					, readPositionAdjusted
					, minAcceptableSample
					, maxAcceptableSample 
					, theCurrentReadSample
					, newSample
					, oldDiffWriteGuess 
					, (readPositionAdjusted ? newDiffWriteGuess.ToString() : "-")
					, oldDiffWriteActual
					, (readPositionAdjusted ? newDiffWriteActual.ToString() : "-")
					));
			*/
			// Debug!!!
			//////////////////////

			///////////////
			// Determine if we're about to read past what is available.
			int copySamples = copySize / bytesPerSample;
			int currentReadSample = ((int)this.localBufferReadStream.Position) / bytesPerSample;
			int realFinalReadSample = this.GetRealBufferPosition(currentReadSample + copySamples);

			// If we're reading past the final write sample, avoid it!
			int currentWriteSample = this.localBufferWritePosition;
			if (this.GetRealSampleInclusive(currentReadSample, realFinalReadSample, currentWriteSample))
			{
				// Uh oh. The system must be running slow.
				//Trace.WriteLine("AUDIO - Awwww shit.");

				// There are a few options with what to do here, but I've just opted for an "echo" in this situation.
			}

			///////////////
			// Now read the audio data.
			if (this.localBufferReadStream != null)
			{
				int destIndex = 0;
				while (destIndex < copySize)
				{
					int bytesToGet = copySize - destIndex;
					int got = this.localBufferReadStream.Read(copyBuffer, destIndex, bytesToGet);
					if (got < bytesToGet)
						this.localBufferReadStream.Position = 0; // loop if the buffer ends
					destIndex += got;
				}
			}
			else
			{
				for (int i = 0; i < copyBuffer.Length; i++)
					copyBuffer[i] = 0;
			}
			Marshal.Copy(copyBuffer, 0, destinationBuffer, copySize);
		}

		#region Helper Functions

		#region General Functions

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
				return (int)(possiblyOutOfBoundsPosition % this.localBufferLengthInSamples);
			else
			{
				int returnValue = (int)(this.localBufferLengthInSamples - (-possiblyOutOfBoundsPosition % this.localBufferLengthInSamples));
				if (returnValue == this.localBufferLengthInSamples)
					return 0;
				else
					return returnValue;
			}
		}

		private int GetRealSampleDiff(int sampleNumHigher, int sampleNumLower)
		{
			if (sampleNumHigher > sampleNumLower)
				return sampleNumHigher - sampleNumLower;
			else
				return sampleNumHigher + (this.localBufferLengthInSamples - sampleNumLower);
		}

		private bool GetRealSampleInclusive(int sampleBoundMin, int sampleBoundMax, int sampleTest)
		{
			if (sampleBoundMax > sampleBoundMin)
				return (sampleTest > sampleBoundMin) && (sampleTest < sampleBoundMax);
			else
				return (sampleTest < sampleBoundMax) || (sampleTest > sampleBoundMin);
		}

		#endregion

		#region Super fidelity identification hack!

		private void IdentifyBytesPerSample()
		{
			bool working = false;
			int bytesPerChannel = 2;

			do
			{
				try
				{
					format = new WaveFormat(FORMAT_SAMPLES_PER_SECOND, 8 * bytesPerChannel, FORMAT_CHANNELS);
					using (WaveOutPlayer newPlayer = new WaveOutPlayer(-1, format, PLAY_BUFFER_SAMPLES * bytesPerChannel, PLAY_BUFFER_COUNT, new WaveLib.BufferFillEventHandler(this.TestFillerCallback)))
					{
						working = true;
					}
				}
				catch
				{
					bytesPerChannel--;
				}
			} while (!working && bytesPerChannel > 0);

			if (bytesPerChannel == 0)
			{
				this.audioEnabled = false;
				this.bytesPerSample = 1 * FORMAT_CHANNELS;
			}
			else
			{
				this.audioEnabled = true;
				this.bytesPerSample = bytesPerChannel * FORMAT_CHANNELS;
			}
			Trace.WriteLine("AUDIO - Bytes per sample will be: " + this.bytesPerSample.ToString());
		}

		private void TestFillerCallback(IntPtr data, int size)
		{
			// Black hole!
		}

		#endregion

		#region Write position estimation / management

		private int? expectedWritePositionSample;
		private long expectedWritePositionTimeStamp;
		private long expectedWritePositionOvershoot;
		private object expectedWritePositionSemaphore = new object();

		private void SetExpectedWritePosition(int newExpectedPosition, long currentOvershoot)
		{
			lock (expectedWritePositionSemaphore)
			{
				this.expectedWritePositionSample = newExpectedPosition;
				this.expectedWritePositionOvershoot = currentOvershoot;
				this.expectedWritePositionTimeStamp = PerformanceCounter.Current;
			}
		}

		private int? GetExpectedWritePosition()
		{
			lock (expectedWritePositionSemaphore)
			{
				// If we've gotten no hint, we have no known position!
				if (!this.expectedWritePositionSample.HasValue)
					return null;

				double hintDelaySeconds =
					(PerformanceCounter.Current - expectedWritePositionTimeStamp - this.expectedWritePositionOvershoot)
					/ (double)PerformanceCounter.Frequency;

				int hintDelaySamples = (int)(hintDelaySeconds * FORMAT_SAMPLES_PER_SECOND);
				int expectedWritePosition = this.GetRealBufferPosition(this.expectedWritePositionSample.Value + hintDelaySamples);

				return expectedWritePosition;
			}
		}

		#endregion

		#endregion
	}
}