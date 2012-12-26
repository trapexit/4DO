using System;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	[Serializable]
	public enum InputButton
	{
		ConsoleStateSave,
		ConsoleStateLoad,
		ConsoleStateSlotPrevious,
		ConsoleStateSlotNext,

		ConsoleFullScreen,
		ConsoleScreenShot,
		ConsolePause,
		ConsoleAdvanceBySingleFrame,
		ConsoleReset,

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
