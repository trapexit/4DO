using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FourDO.Emulation.Plugins.Audio
{
	internal class EmptyAudioPlugin : IAudioPlugin
	{
		public void Destroy()
		{
		}

		public bool GetHasSettings()
		{
			return false;
		}

		public bool GetSupportsVolume()
		{
			return false;
		}

		public void ShowSettings(System.Windows.Forms.IWin32Window owner)
		{
		}

		public void Start()
		{
		}

		public void Stop()
		{
		}

		public void PushSample(uint dspSample)
		{
		}

		public void FrameDone(long currentOvershoot, long scheduleAdjustment)
		{
		}

		public double Volume
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public int BufferMilliseconds
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
	}
}
