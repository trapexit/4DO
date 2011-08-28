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

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			JoystickTrigger other = obj as JoystickTrigger;

			if (other == null)
				return base.Equals(obj);

			// NOTE: This could (and probably should) check the type.
			//       But, I don't care. Less maintenence is my preference.
			if (this.Type != other.Type)
				return false;
			if (this.ButtonNumber != other.ButtonNumber)
				return false;
			if (this.Axis != other.Axis)
				return false;
			if (this.AxisPositive != other.AxisPositive)
				return false;
			if (this.PovNumber != other.PovNumber)
				return false;
			if (this.PovDirection != other.PovDirection)
				return false;

			return true;
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
