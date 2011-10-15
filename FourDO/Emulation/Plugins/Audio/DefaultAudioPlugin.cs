//////////////////////////////////////////////////////////
// JMK NOTES:
// I only spent one night getting this hooked up. It was relatively easy to figure out the
// right sample rate just by listening to the resulting audio.
//
// I've used a libary from here to play the audio from a buffer
// http://www.codeproject.com/KB/audio-video/cswavplay.aspx
// 
// Consent explicitly recieved on August 3rd via email (from Ianier to Johnny).
//
// Things to do:
// * I just blindly selected BUFFER SIZES. This is pretty lazy of me. I believe it introduces audio lag too.
// * If I understand things right, copying bytes around could be FASTER by using RtlMoveMemory from kernel32.
// * Audio plugins will suffer if the system doesn't stay at 60fps internally. It's possible the audio plugins
//   should be made aware of these situations so it can do something. Though I don't know what it could do.
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

namespace FourDO.Emulation.Plugins.Audio
{
	internal class DefaultAudioPlugin : IAudioPlugin
	{
		private bool audioEnabled;
		private int bytesPerSample;

		private double volumeLinear = 1.0;
		private double volumeLogarithmic = 1.0;

		private bool stopped = true;
		
		private const int FORMAT_SAMPLES_PER_SECOND = 44100;
		private const int FORMAT_CHANNELS = 2;

		private const int BUFFER_SIZE = 4096;

		private WaveOutPlayer player;
		private WaveFormat format;

		private readonly int bufferLengthInSamples;

		private byte[] buffer;
		private IntPtr bufferPtr;
		private GCHandle bufferHandle;
		private volatile int bufferWritePosition;
		private volatile Stream bufferReadStream;

		byte[] copyBuffer = new byte[BUFFER_SIZE];
		byte[] emptyBuffer = new byte[BUFFER_SIZE];

		internal DefaultAudioPlugin()
		{
			this.IdentifyBytesPerSample();

			// Create a buffer on our side to write into.
			this.buffer = new byte[BUFFER_SIZE * 3 * bytesPerSample];
			this.bufferHandle = GCHandle.Alloc(this.buffer, GCHandleType.Pinned);
			this.bufferPtr = this.bufferHandle.AddrOfPinnedObject();
			this.bufferWritePosition = 0;

			this.bufferLengthInSamples = this.buffer.Length / bytesPerSample;

			// Create a stream to our buffer that the audio player will be reading from.
			this.bufferReadStream = new MemoryStream(this.buffer);
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
					UInt32* bufferSamplePointer = (UInt32*)this.bufferPtr.ToPointer();
					bufferSamplePointer[this.bufferWritePosition++] = leftSample | rightSample;
				}
				else if (bytesPerChannel == 1)
				{
					UInt16* bufferSamplePointer = (UInt16*)this.bufferPtr.ToPointer();
					bufferSamplePointer[this.bufferWritePosition++] = (UInt16)(
							 (((rightSample & 0xFF000000) >> 16) + 0x8000)
							 | (((leftSample >> 8) + 0x80) & 0xFF)
							 );
 
				}
				if (this.bufferWritePosition == this.bufferLengthInSamples)
					this.bufferWritePosition = 0;
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

		#endregion // IAudioPlugin Implementation

		private void InternalStart()
		{
			format = new WaveFormat(FORMAT_SAMPLES_PER_SECOND, 8 * bytesPerSample / FORMAT_CHANNELS, FORMAT_CHANNELS);
			if (this.audioEnabled == false)
				return;

			try
			{
				if (player == null)
					player = new WaveOutPlayer(-1, format, BUFFER_SIZE, 3, new WaveLib.BufferFillEventHandler(this.FillerCallback));
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

		private void FillerCallback(IntPtr data, int size)
		{
			if (stopped)
			{
				Marshal.Copy(emptyBuffer, 0, data, size);
				return;
			}

			const int WriteSampleWatchSize = 512; // Looks X number of samples ahead.

			int currentWriteSample = this.bufferWritePosition; // get a COPY.
			int finalWriteSample = currentWriteSample + WriteSampleWatchSize;
			int realFinalWriteSample = this.GetRealBufferPosition(finalWriteSample);

			int currentReadSample = ((int)this.bufferReadStream.Position) / bytesPerSample;
			int finalReadSample = currentReadSample + (size / bytesPerSample);
			int realFinalReadSample = this.GetRealBufferPosition(finalReadSample);

			// If the read position is about to speed off ahead of 
			// the write position, we want to back off the read position!
			// (At the cost of a one-time audio echo/glitch).
			if ((currentReadSample < currentWriteSample && finalReadSample > currentWriteSample)
				|| (currentReadSample > currentWriteSample && finalReadSample != realFinalReadSample && realFinalReadSample > currentWriteSample))
			{
				// Kick it back to current write position + 50% of buffer.
				Trace.WriteLine(string.Format("Had to kick back current read position. ReadSample:{0}   WriteSample:{1}", currentReadSample, currentWriteSample));
				this.bufferReadStream.Position = this.GetRealBufferPosition(currentWriteSample + (this.bufferLengthInSamples / 2)) * bytesPerSample;
			}
			// Also, if the write position is about to write over the 
			// read position, we want to move the read position ahead!
			else if ((currentWriteSample < currentReadSample && finalWriteSample > currentReadSample)
				|| (currentWriteSample > currentReadSample && finalWriteSample != realFinalWriteSample && realFinalWriteSample > currentReadSample))
			{
				// Move it up to current write position + 50% of buffer.
				Trace.WriteLine(string.Format("Had to push ahead current read position. ReadSample:{0}   WriteSample:{1}", currentReadSample, currentWriteSample));
				this.bufferReadStream.Position = this.GetRealBufferPosition(currentWriteSample + (this.bufferLengthInSamples / 2)) * bytesPerSample;
			}

			///////////////
			// Now read the audio data.
			if (this.bufferReadStream != null)
			{
				int destIndex = 0;
				while (destIndex < size)
				{
					int bytesToGet = size - destIndex;
					int got = this.bufferReadStream.Read(copyBuffer, destIndex, bytesToGet);
					if (got < bytesToGet)
						this.bufferReadStream.Position = 0; // loop if the buffer ends
					destIndex += got;
				}
			}
			else
			{
				for (int i = 0; i < copyBuffer.Length; i++)
					copyBuffer[i] = 0;
			}
			Marshal.Copy(copyBuffer, 0, data, size);
		}

		/// <summary>
		/// This will "clamp" a position to a real buffer position.
		/// If it passes the end of the buffer, it'll be placed at
		/// the start.
		/// If it preceeds the beginning of the buffer, it'll be
		/// placed at the end.
		/// </summary>
		private int GetRealBufferPosition(int possiblyOutOfBoundsPosition)
		{
			if (possiblyOutOfBoundsPosition >= 0)
				return possiblyOutOfBoundsPosition % this.bufferLengthInSamples;
			else
			{
				int returnValue = this.bufferLengthInSamples - (-possiblyOutOfBoundsPosition % this.bufferLengthInSamples);
				if (returnValue == this.bufferLengthInSamples)
					return 0;
				else
					return returnValue;
			}

		}

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
					using (WaveOutPlayer newPlayer = new WaveOutPlayer(-1, format, BUFFER_SIZE, 3, new WaveLib.BufferFillEventHandler(this.TestFillerCallback)))
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
		}

		private void TestFillerCallback(IntPtr data, int size)
		{
			// Black hole!
		}

		#endregion
	}
}
