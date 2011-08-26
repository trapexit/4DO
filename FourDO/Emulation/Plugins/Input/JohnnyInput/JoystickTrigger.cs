using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	[Serializable]
	public class JoystickTrigger : InputTrigger
    {
		// Empty constructor only for serialization
		public JoystickTrigger() { }

		public JoystickTrigger(JoystickTriggerType type)
		{
			this.Type = type;
		}

		public override string FriendlyName 
        {
            get
            {
				switch (this.Type)
				{
					case JoystickTriggerType.Button:
						return "Button " + this.ButtonNumber.ToString();

					case JoystickTriggerType.Axis:
						return (this.AxisPositive ? "+" : "-") + this.Axis.ToString() + " Axis";

					case JoystickTriggerType.Pov:
						return "PoV " + this.PovNumber.ToString() + " " +
							((this.PovDirection == JoystickTriggerPovDirection.Up) ? @"/\" :
							(this.PovDirection == JoystickTriggerPovDirection.Down) ? @"\/" :
							(this.PovDirection == JoystickTriggerPovDirection.Left) ? @"< " :
							(this.PovDirection == JoystickTriggerPovDirection.Right) ? @" >" : "");
				}
				return null;
            }
        }

		public Guid DeviceInstance { get; set; } // Unique ID for the device
		public JoystickTriggerType Type { get; set; }

		public int ButtonNumber { get; set; }
		public JoystickTriggerAxis Axis { get; set; }
		public bool AxisPositive { get; set; }
		public int PovNumber { get; set; }
		public JoystickTriggerPovDirection PovDirection { get; set; }
    }
}
