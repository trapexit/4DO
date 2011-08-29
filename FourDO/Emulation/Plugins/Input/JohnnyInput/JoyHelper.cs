using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.DirectInput;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	internal class JoyHelper
	{
		private static DirectInput directInput = new DirectInput();

		public static List<DeviceInstance> GetJoystickDevices()
		{
			List<DeviceInstance> joystickDevices = new List<DeviceInstance>();

			var devices = directInput.GetDevices();
			
			// The least significant byte of the device's "Type" identifies the DeviceType.
			return devices.Where<DeviceInstance>(x => ((int)x.Type & 0xFF) == (int)DeviceType.Joystick).ToList<DeviceInstance>();
		}
	}
}
