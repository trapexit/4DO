//////////////////////////////////////////////////////////
// JMK NOTES:
// I only spent one night getting this hooked up. It was relatively easy to figure out the
// right sample rate just by listening to the resulting audio.
//
// I've used a libary from here to play the audio from a buffer
// http://www.codeproject.com/KB/audio-video/cswavplay.aspx
// (consent is pending, I've emailed the guy... maybe the email address is too old?)
//
// Things to do:
// * I just blindly selected BUFFER SIZES. This is pretty lazy of me. I believe it introduces audio lag too.
// * If I understand things right, copying bytes around could be FASTER by using RtlMoveMemory from kernel32.
// * Audio plugins will suffer if the system doesn't stay at 60fps internally. It's possible the audio plugins
//   should be made aware of these situations so it can do something. Though I don't know what it could do.
//////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
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

		private const int BUFFER_SIZE = 4098;

		private WaveOutPlayer player;
		private WaveFormat format;

		private readonly int bufferLengthInSamples;

		private byte[] buffer;
		private IntPtr bufferPtr;
		private GCHandle bufferHandle;
		private volatile int bufferWritePosition;
		private volatile Stream bufferReadStream;

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

		public bool GetHasSettings()
		{
			return false;
		}

		public void ShowSettings(IWin32Window owner)
		{
			return;
		}

		public void PushSample(uint dspSample)
		{
			unsafe
			{
				if (bytesPerSample == 4)
				{
					UInt32* bufferSamplePointer = (UInt32*)this.bufferPtr.ToPointer();
					bufferSamplePointer[this.bufferWritePosition++] = dspSample;
				}
				else if (bytesPerSample == 3)
				{
					byte* bufferSamplePointer = (byte*)this.bufferPtr.ToPointer();
					bufferSamplePointer[3 * this.bufferWritePosition + 0] = (byte)(dspSample >> 24);
					bufferSamplePointer[3 * this.bufferWritePosition + 1] = (byte)(dspSample >> 16);
					bufferSamplePointer[3 * this.bufferWritePosition + 2] = (byte)(dspSample >> 8);
					this.bufferWritePosition++;
				}
				else if (bytesPerSample == 2)
				{
					UInt16* bufferSamplePointer = (UInt16*)this.bufferPtr.ToPointer();
					bufferSamplePointer[this.bufferWritePosition++] = (UInt16)(dspSample >> 16);
				}
				else if (bytesPerSample == 1)
				{
					byte* bufferSamplePointer = (byte*)this.bufferPtr.ToPointer();
					bufferSamplePointer[this.bufferWritePosition++] = (byte)((dspSample >> 24) + 128);
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
			format = new WaveFormat(44100, 8 * bytesPerSample, 1);
			if (this.audioEnabled == false)
				return;

			try
			{
				player = new WaveOutPlayer(-1, format, BUFFER_SIZE, 3, new WaveLib.BufferFillEventHandler(this.FillerCallback));
			}
			catch
			{
				// TODO: Shouldn't get this if initialization worked right.
			}
		}

		private void InternalStop()
		{
			this.Destroy();
		}

		private void FillerCallback(IntPtr data, int size)
		{
			byte[] tempBuffer = new byte[size];
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
				this.bufferReadStream.Position = this.GetRealBufferPosition(currentWriteSample + (this.bufferLengthInSamples / 2)) * bytesPerSample;
			}

			// Also, if the write position is about to write over the 
			// read position, we want to move the read position ahead!
			else if ((currentWriteSample < currentReadSample && finalWriteSample > currentReadSample)
				|| (currentWriteSample > currentReadSample && finalWriteSample != realFinalWriteSample && realFinalWriteSample > currentReadSample))
			{
				// Move it up to current write position + 50% of buffer.
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
					int got = this.bufferReadStream.Read(tempBuffer, destIndex, bytesToGet);
					if (got < bytesToGet)
						this.bufferReadStream.Position = 0; // loop if the buffer ends
					destIndex += got;
				}
			}
			else
			{
				for (int i = 0; i < tempBuffer.Length; i++)
					tempBuffer[i] = 0;
			}
			Marshal.Copy(tempBuffer, 0, data, size);
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
			bytesPerSample = 4;

			do
			{
				try
				{
					format = new WaveFormat(44100, 8 * bytesPerSample, 1);
					using (WaveOutPlayer newPlayer = new WaveOutPlayer(-1, format, BUFFER_SIZE, 3, new WaveLib.BufferFillEventHandler(this.TestFillerCallback)))
					{
						working = true;
					}
				}
				catch
				{
					bytesPerSample--;
				}
			} while (!working && bytesPerSample > 0);

			if (bytesPerSample == 0)
			{
				this.audioEnabled = false;
				this.bytesPerSample = 1;
			}
			else
				this.audioEnabled = true;
		}

		private void TestFillerCallback(IntPtr data, int size)
		{
			// Black hole!
		}

		#endregion
	}
}
