using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.DirectInput;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	internal class JoyWatcher
	{
		private class JoyCache
		{
			public Joystick JoyStick { get; set; }
			public JoystickState LastState { get; set; }
		}

		private List<JoyCache> joysticks = new List<JoyCache>();
		private DirectInput directInput = new DirectInput();

		public JoyWatcher() {}

		public JoystickTrigger WatchForTrigger()
		{
			JoystickTrigger newTrigger = null;

			// update list of joysticks.
			this.UpdateJoystickList();

			// For each cached device, watch for actions.
			List<JoyCache> deadJoysticks = new List<JoyCache>();
			foreach (var joyCache in this.joysticks)
			{
				// Get the state.
				JoystickState currentState = this.GetCurrentState(joyCache.JoyStick);

				// If we couldn't get the state for the joystick, skip it, but remember to kill it later.
				if (currentState == null)
				{
					deadJoysticks.Add(joyCache);
					continue;
				}
				
				// If this joystick has no previous state, skip it, but record the current state for next time.
				if (joyCache.LastState == null)
				{
					joyCache.LastState = currentState;
					continue;
				}

				//////////////
				// Watch for a trigger.
				newTrigger = this.SpotTrigger(joyCache.JoyStick, joyCache.LastState, currentState);
				joyCache.LastState = currentState;

				// if we found a trigger, we're done!
				if (newTrigger != null)
					break;
			}

			// Kill any joysticks that have passed away.
			deadJoysticks.ForEach(x => this.joysticks.Remove(x));

			// Return what we found. If we found nothing, it'll be null.
			return newTrigger;
		}

		private JoystickTrigger SpotTrigger(Joystick joystick, JoystickState oldState, JoystickState newState)
		{
			///////////////////////
			// Check the buttons
			bool[] oldButtons = oldState.GetButtons();
			bool[] newButtons = newState.GetButtons();
			for (int button = 0; button < joystick.Capabilities.ButtonCount; button++)
			{
				// it may be pressed, but it only counts if it's newly pressed.
				if (newButtons[button] && !oldButtons[button])
				{
					var newTrigger = new JoystickTrigger();
					newTrigger.Type = JoystickTriggerType.Button;
					newTrigger.ButtonNumber = button;
					return newTrigger;
				}
			}

			///////////////////////
			// Check each axis
			var axi = (JoystickTriggerAxis[]) Enum.GetValues(typeof(JoystickTriggerAxis));
			foreach (JoystickTriggerAxis axis in axi)
			{
				int oldAxisValue = JoyHelper.GetAxisValue(oldState, axis);
				int newAxisValue = JoyHelper.GetAxisValue(newState, axis);
				if (oldAxisValue != newAxisValue)
				{
					bool positive;
					if (newAxisValue < 16384)
						positive = false;
					else if (newAxisValue > 49152)
						positive = true;
					else
						continue; // the axis isn't far enough. Screw em!

					var newTrigger = new JoystickTrigger();
					newTrigger.Type = JoystickTriggerType.Axis;
					newTrigger.Axis = axis;
					newTrigger.AxisPositive = positive; 
					return newTrigger;
				}
			}

			///////////////////////
			// Check each Point-Of-View (POV) thingie.
			
			int[] oldPovs = oldState.GetPointOfViewControllers();
			int[] newPovs = newState.GetPointOfViewControllers();
			for (int pov = 0; pov < joystick.Capabilities.PovCount; pov++)
			{
				int newPov = newPovs[pov];
				if ((newPov != -1) && (newPov != oldPovs[pov]))
				{
					JoystickTriggerPovDirection direction;
					if (newPov == 0)
						direction = JoystickTriggerPovDirection.Up;
					else if (newPov == 9000)
						direction = JoystickTriggerPovDirection.Right;
					else if (newPov == 18000)
						direction = JoystickTriggerPovDirection.Down;
					else if (newPov == 27000)
						direction = JoystickTriggerPovDirection.Left;
					else
						continue; // we don't accept diagnols here.
						
					var newTrigger = new JoystickTrigger();
					newTrigger.Type = JoystickTriggerType.Pov;
					newTrigger.PovNumber = pov;
					newTrigger.PovDirection = direction;
					return newTrigger;
				}
			}

			// We checked everything. There's no new trigger to be found.
			return null;
		}

		private JoystickState GetCurrentState(Joystick joyStick)
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

		private void UpdateJoystickList()
		{
			// Get all joysticks.
			List<DeviceInstance> devices = JoyHelper.GetJoystickDevices();

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
		}
	}
}
