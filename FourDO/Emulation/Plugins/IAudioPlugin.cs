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
		
		/// <summary>
		/// Notify frame completion
		/// </summary>
		/// <param name="currentOvershoot">The current deviation from "standard" schedule.</param>
		/// <param name="adjustmentPosted">The schedule adjustment posted on this frame (if any).</param>
		void FrameDone(long currentOvershoot, long scheduleAdjustment);

		double Volume { get; set; }

		int BufferMilliseconds { get; set; }
	}
}
