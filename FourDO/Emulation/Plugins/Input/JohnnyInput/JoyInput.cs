using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.DirectInput;
using System.Threading;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	abstract internal class JoyInput : IDisposable
	{
		protected class JoyCache
		{
			public Joystick JoyStick { get; set; }
			public JoystickState LastState { get; set; }
		}

		public JoyInput()
		{
			this.deviceSearchThread.Start();
		}

		public void Dispose()
		{
			this.deviceSearchThread.Stop();
			this.directInput.Dispose();
		}

		protected List<JoyCache> joysticks = new List<JoyCache>();
		protected DirectInput directInput = new DirectInput();
		private DeviceSearchThread deviceSearchThread = new DeviceSearchThread();

		protected JoystickState GetCurrentState(Joystick joyStick)
		{
			JoystickState currentState = new JoystickState();
			SlimDX.Result result = joyStick.GetCurrentState(ref currentState);

			// If we had a failure, try to revive the joystick.
			if (result.IsFailure)
			{
				if (result.Code != ResultCode.InputLost.Code)
					return null;
				else
				{
					// We were un-acquired. Attempt to re-acquire.
					if (joyStick.Acquire().IsFailure)
					{
						// We tried all we could.
						return null;
					}

					// We reacquired! Try to get the data again.
					if (joyStick.GetCurrentState(ref currentState).IsFailure)
						return null;
				}
			}

			// (if we got this far, we successfully acquired the data)

			return currentState;
		}

		protected void UpdateJoystickList()
		{
			// Get all joysticks.
			List<DeviceInstance> devices = this.deviceSearchThread.GetDevices();

			foreach (var device in devices)
			{
				//////////////////////
				// Add caches for anything we don't have.
				if (this.joysticks.Any<JoyCache>(x => x.JoyStick.Information.InstanceGuid == device.InstanceGuid) == false)
				{
					var joystick = new Joystick(this.directInput, device.InstanceGuid);
					if (joystick.Acquire().IsSuccess)
					{
						this.joysticks.Add(new JoyCache { JoyStick = joystick });
					}
				}
			}

			/////////////////////
			// Remove caches for any devices that no longer exist.

			// Identify removed items.
			var removedCaches = new List<JoyCache>();
			foreach (var joystick in this.joysticks)
			{
				if (devices.Any<DeviceInstance>(x => x.InstanceGuid == joystick.JoyStick.Information.InstanceGuid) == false)
					removedCaches.Add(joystick);
			}

			// Remove each item.
			foreach (var removedCache in removedCaches)
			{
				removedCache.JoyStick.Unacquire();
				this.joysticks.Remove(removedCache);
			}
		}

		protected static int GetAxisValue(JoystickState state, JoystickTriggerAxis axis)
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

		protected JoystickTriggerPovDirection GetPovDirection(int value, bool allowDiagonals = true)
		{
			if (value == 0)
				return JoystickTriggerPovDirection.Up;
			else if (value == 4500 && allowDiagonals)
				return JoystickTriggerPovDirection.Up | JoystickTriggerPovDirection.Right;
			else if (value == 9000)
				return JoystickTriggerPovDirection.Right;
			else if (value == 13500 && allowDiagonals)
				return JoystickTriggerPovDirection.Right | JoystickTriggerPovDirection.Down;
			else if (value == 18000)
				return JoystickTriggerPovDirection.Down;
			else if (value == 22500 && allowDiagonals)
				return JoystickTriggerPovDirection.Down | JoystickTriggerPovDirection.Left;
			else if (value == 27000)
				return JoystickTriggerPovDirection.Left;
			else if (value == 31500 && allowDiagonals)
				return JoystickTriggerPovDirection.Left | JoystickTriggerPovDirection.Up;

			return 0; // er... yikes.
		}
	}
}
