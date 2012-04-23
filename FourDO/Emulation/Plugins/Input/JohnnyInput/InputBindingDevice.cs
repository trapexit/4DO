using System;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	[Serializable]
	public class InputBindingDevice
	{
		public InputBindingSets BindingSets { get; set; }

		public InputBindingDevice()
		{
			BindingSets = new InputBindingSets();
		}
	}
}
