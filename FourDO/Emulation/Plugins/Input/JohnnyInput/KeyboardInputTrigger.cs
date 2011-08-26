using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	[Serializable]
	public class KeyboardInputTrigger : InputTrigger
	{
		// Empty constructor only for serialization
		public KeyboardInputTrigger() { }

		public KeyboardInputTrigger(Keys key)
		{
			this.Key = key;
		}

		public override string FriendlyName 
		{
			get
			{
				return this.Key.ToString();
			}
		}

		public Keys Key { get; set; }
	}
}
