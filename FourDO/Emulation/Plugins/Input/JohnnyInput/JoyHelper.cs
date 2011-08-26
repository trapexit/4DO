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

		public static int GetAxisValue(JoystickState state, JoystickTriggerAxis axis)
		{
			if (state == null)
				return 0;

			switch (axis)
			{
				case JoystickTriggerAxis.AccelX: return state.AccelerationX;
				case JoystickTriggerAxis.AccelY: return state.AccelerationY;
				case JoystickTriggerAxis.AccelZ: return state.AccelerationZ;
				case JoystickTriggerAxis.AngAccelX: return state.AngularAccelerationX;
				case JoystickTriggerAxis.AngAccelY: return state.AngularAccelerationY;
				case JoystickTriggerAxis.AngAccelZ: return state.AngularAccelerationZ;
				case JoystickTriggerAxis.AngVelX: return state.AngularVelocityX;
				case JoystickTriggerAxis.AngVelY: return state.AngularVelocityY;
				case JoystickTriggerAxis.AngVelZ: return state.AngularVelocityZ;
				case JoystickTriggerAxis.ForceX: return state.ForceX;
				case JoystickTriggerAxis.ForceY: return state.ForceY;
				case JoystickTriggerAxis.ForceZ: return state.ForceZ;
				case JoystickTriggerAxis.RotX: return state.RotationX;
				case JoystickTriggerAxis.RotY: return state.RotationY;
				case JoystickTriggerAxis.RotZ: return state.RotationZ;
				case JoystickTriggerAxis.TorqX: return state.TorqueX;
				case JoystickTriggerAxis.TorqY: return state.TorqueY;
				case JoystickTriggerAxis.TorqZ: return state.TorqueZ;
				case JoystickTriggerAxis.VelX: return state.VelocityX;
				case JoystickTriggerAxis.VelY: return state.VelocityY;
				case JoystickTriggerAxis.VelZ: return state.VelocityZ;
				case JoystickTriggerAxis.X: return state.X;
				case JoystickTriggerAxis.Y: return state.Y;
				case JoystickTriggerAxis.Z: return state.Z;
				default: return 0;
			}
		}
    }
}
