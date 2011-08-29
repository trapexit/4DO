using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.DirectInput;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	internal class JoyInputWatcher : JoyInput
	{
		public JoyInputWatcher() {}

		public JoystickTrigger WatchForTrigger()
		{
			JoystickTrigger newTrigger = null;

			// update list of joysticks.
			this.UpdateJoystickList();

			// For each cached device, watch for actions.
			List<JoyCache> deadJoysticks = new List<JoyCache>();
			foreach (var joyCache in this.Joysticks)
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
			deadJoysticks.ForEach(x => this.Joysticks.Remove(x));

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
				int oldAxisValue = JoyInput.GetAxisValue(oldState, axis);
				int newAxisValue = JoyInput.GetAxisValue(newState, axis);
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
	}
}
