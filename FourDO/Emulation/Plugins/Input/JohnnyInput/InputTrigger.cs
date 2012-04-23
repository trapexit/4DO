using System;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	[Serializable]
	public abstract class InputTrigger
	{
		public abstract string FriendlyName { get; }
	}
}
