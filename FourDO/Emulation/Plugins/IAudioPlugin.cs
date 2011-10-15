using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FourDO.Emulation.Plugins
{
	public interface IAudioPlugin
	{
		void Destroy();

		bool GetHasSettings();
		bool GetSupportsVolume();

		void ShowSettings(IWin32Window owner);

		void Start();
		void Stop();

		void PushSample(uint dspSample);

		double Volume { get; set; }
	}
}
