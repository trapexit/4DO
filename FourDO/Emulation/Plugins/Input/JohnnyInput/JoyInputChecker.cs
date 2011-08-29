using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.DirectInput;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	internal class JoyInputChecker : JoyInput
	{
		public void UpdateDeviceCache()
		{
			this.UpdateJoystickList();
		}

		public void UpdateValueCache()
		{
			// For each cached device, update the cached values.
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

				// Remember the current state;
				joyCache.LastState = currentState;
			}

			// Kill any joysticks that have passed away.
			deadJoysticks.ForEach(x => this.Joysticks.Remove(x));
		}

		public bool CheckTrigger(JoystickTrigger trigger)
		{
			Guid deviceGuid = trigger.GetDeviceInstanceAsGuid();
			
			var cache = this.Joysticks.FirstOrDefault<JoyCache>(x => x.JoyStick.Information.InstanceGuid == deviceGuid);
			if (cache == null)
				return false;

			if (trigger.Type == JoystickTriggerType.Button)
			{
				return cache.LastState.GetButtons()[trigger.ButtonNumber];
			}
			else if (trigger.Type == JoystickTriggerType.Axis)
			{
				int value = JoyInput.GetAxisValue(cache.LastState, trigger.Axis);
				if (trigger.AxisPositive)
					return (value > 49152);
				else
					return (value < 16384);
			}
			else if (trigger.Type == JoystickTriggerType.Pov)
			{
				int[] povValues = cache.LastState.GetPointOfViewControllers();
				if (trigger.PovNumber >= povValues.Length)
					return false;

				JoystickTriggerPovDirection direction = this.GetPovDirection(povValues[trigger.PovNumber]);
				return ((int)trigger.PovDirection & (int)direction) > 0;
			}

			return false;
		}
	}
}
