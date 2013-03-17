using System.IO;
using System.Text;
using SlimDX.DirectSound;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using PerformanceCounter = FourDO.Utilities.PerformanceCounter;

namespace FourDO.Emulation.Plugins.Audio.FileWriterAudio
{
	internal class FileWriterAudioPlugin : IAudioPlugin
	{
		public FileWriterAudioPlugin()
		{
			var path = GetOutputFileName();
			try
			{
				if (File.Exists(path))
					File.Delete(path);
			}
			catch {}
		}

		#region IAudioPlugin Implementation

		public double Volume { get; set; }

		public int BufferMilliseconds { get; set; }

		public bool GetSupportsVolume()
		{
			return false; 
		}

		public bool GetHasSettings()
		{
			return false;
		}

		public void ShowSettings(IWin32Window owner)
		{
		}

		private const int TEMP_BUFFER_SIZE = 525; // in samples
		private int _currentTempBufferPosition = 0;
		private byte[] _tempBuffer = new byte[TEMP_BUFFER_SIZE * 4];

		private BinaryWriter _writer;

		public void PushSample(uint dspSample)
		{
			this.InternalPushSample(dspSample);
		}

		public void Destroy()
		{
		}

		public void Start()
		{
			_currentTempBufferPosition = 0;

			var path = GetOutputFileName();

			_writer = new BinaryWriter(new FileStream(path, FileMode.OpenOrCreate | FileMode.Append));
		}

		public void Stop()
		{
			_writer.Close();
		}

		public void FrameDone(long currentOvershoot, long adjustmentPosted)
		{
		}

		#endregion // IAudioPlugin Implementation

		private string GetOutputFileName()
		{
			var path = Application.ExecutablePath;
			path = Path.GetDirectoryName(path);
			path = Path.Combine(path, "Temp\\AudioOutput.raw");
			return path;
		}

		private void InternalPushSample(uint dspSample)
		{
			int bytePos = _currentTempBufferPosition * 4;
			_tempBuffer[bytePos] = (byte)(dspSample >> 24);
			_tempBuffer[bytePos + 1] = (byte)(dspSample >> 16);
			_tempBuffer[bytePos + 2] = (byte)(dspSample >> 8);
			_tempBuffer[bytePos + 3] = (byte)(dspSample);

			_currentTempBufferPosition++;
			if (_currentTempBufferPosition == TEMP_BUFFER_SIZE)
			{
				_currentTempBufferPosition = 0;

				this.PushTempBlock();
			}
		}

		private void PushTempBlock()
		{
			_writer.Write(_tempBuffer);
		}
	}
}
