using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
