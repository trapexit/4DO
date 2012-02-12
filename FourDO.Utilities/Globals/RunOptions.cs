using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FourDO.Utilities.Globals
{
	/// <summary>
	/// Access to global, one-time-only run options (usually set at the command line).
	/// </summary>
	public class RunOptions
	{
		public enum StartupFormOption
		{
			None = 0,
			ConfigureInput = 1,
		}

		public static StartupFormOption StartupForm { get; set; }

		public static bool LogAudioDebug { get; set; }
		public static bool LogAudioTiming { get; set; }
		public static bool LogCPUTiming { get; set; }

		public static bool PrintKPrint { get; set; }

		public static bool ForceGdiRendering { get; set; }
	}
}
