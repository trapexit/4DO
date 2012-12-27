using System;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	[Serializable]
	public enum InputButton
	{
		// "Console" / emulation control buttons
		ConsoleStateSave,
		ConsoleStateLoad,
		ConsoleStateSlotPrevious,
		ConsoleStateSlotNext,

		ConsoleFullScreen,
		ConsoleScreenShot,
		ConsolePause,
		ConsoleAdvanceBySingleFrame,
		ConsoleReset,

		// "Regular" buttons
		Up,
		Down,
		Left,
		Right,
		A,
		B,
		C,
		X,
		P,
		L,
		R
	}
}
