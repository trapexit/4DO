using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FourDO.Emulation.Plugins.Input
{
	public enum ConsoleEvent
	{
		StateSave = 0,
		StateLoad,
		StateSlotPrevious,
		StateSlotNext,
		FullScreen,
		ScreenShot,
		Pause,
		AdvanceBySingleFrame,
		Reset,
		Exit
	}
}
