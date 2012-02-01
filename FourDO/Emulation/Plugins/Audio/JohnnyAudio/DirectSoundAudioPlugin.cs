using SlimDX.DirectSound;
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
	internal class DirectSoundAudioPlugin : IAudioPlugin
	{
		private bool initialized = false;

		SlimDX.Multimedia.WaveFormat bufferFormat;
		SoundBufferDescription bufferDescription;

		DirectSound directSound;
		SecondarySoundBuffer playBuffer;

		private double volumeLinear = 1.0;
		private double volumeLogarithmic = 1.0;

		internal DirectSoundAudioPlugin()
		{
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

		private int currentWritePosition;

		private const int TEMP_BUFFER_SIZE = 256;
		private int currentTempBufferPosition = 0;
		private uint[] tempBuffer = new uint[TEMP_BUFFER_SIZE];

		public void PushSample(uint dspSample)
		{
			if (playBuffer == null)
				return;

			tempBuffer[currentTempBufferPosition] = dspSample;
			currentTempBufferPosition++;
			if (currentTempBufferPosition == TEMP_BUFFER_SIZE)
			{
				currentTempBufferPosition = 0;

				playBuffer.Write<uint>(tempBuffer, currentWritePosition, LockFlags.None);
				
				currentWritePosition += TEMP_BUFFER_SIZE * sizeof(uint);
				if (currentWritePosition == bufferDescription.SizeInBytes)
					currentWritePosition = 0;
			}
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
		}

		#endregion // IAudioPlugin Implementation

		private void InternalStart()
		{
			this.Initialize();

			if (this.playBuffer != null)
				this.playBuffer.Play(0, PlayFlags.Looping);
		}

		private void InternalStop()
		{
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

			bufferDescription = new SoundBufferDescription();
			bufferDescription.Flags = BufferFlags.ControlVolume | BufferFlags.GlobalFocus;
			bufferDescription.Format = bufferFormat;
			bufferDescription.SizeInBytes = 1024 * 64;

			playBuffer = new SecondarySoundBuffer(directSound, bufferDescription);
			playBuffer.Volume = -2000;

			currentWritePosition = bufferDescription.SizeInBytes / 2;
		}
	}
}
