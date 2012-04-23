using System;
using System.Runtime.InteropServices;

namespace FourDO.Utilities
{
	public class TimingHelper
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct TimeCaps
		{
			public UInt32 wPeriodMin;
			public UInt32 wPeriodMax;
		}; 

		[DllImport("winmm.dll", SetLastError = true)]
		private static extern UInt32 timeGetDevCaps(ref TimeCaps timeCaps, UInt32 sizeTimeCaps);

		[DllImport("winmm.dll")]
		public static extern uint timeBeginPeriod(uint uMilliseconds);

		[DllImport("winmm.dll")]
		public static extern uint timeEndPeriod(uint uMilliseconds);

		private static TimeCaps capabilities;
		
		static TimingHelper()
		{
			timeGetDevCaps(ref capabilities, (uint)Marshal.SizeOf(capabilities));
		}

		public static void MaximumResolutionPush()
		{
			timeBeginPeriod(capabilities.wPeriodMin);
		}

		public static void MaximumResolutionPop()
		{
			timeEndPeriod(capabilities.wPeriodMin);
		}

		public static int GetResolution()
		{
			return (int)capabilities.wPeriodMin;
		}
	}
}
