//////////////////////////////////////////////////////////
// JMK NOTES:
// I figured the quickest way to get controls in would just be
// to use global keyboard hooks. This is a bit lazy, especially
// because I don't realy think I want them global. Oh well.
//////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	internal class JohnnyInputPlugin : IInputPlugin
	{
		private const string BINDINGS_FILE_NAME = "JohnnyInputBindings.xml";

		[DllImport("user32.dll")]
		private static extern short GetKeyState(int keyCode);
		private const int KEY_PRESSED = 0x8000;

		private object _joyCheckerSemaphore = new object();
		private readonly JoyInputChecker joyChecker = new JoyInputChecker();
		private InputBindingDevices devices;
		private static readonly string bindingsFilePath = Path.Combine(Utilities.Globals.Constants.SettingsPath, BINDINGS_FILE_NAME);
		Stopwatch _lastDeviceUpdate = Stopwatch.StartNew();

		private bool settingsShown = false;

		private bool keyboardInputEnabled = true;

		public event ConsoleEventRaisedHandler ConsoleEventRaised;

		public JohnnyInputPlugin()
		{
			this.LoadKeys();
		}

		#region IInputPlugin Implementation

		public void Destroy() 
		{
			this.joyChecker.Dispose();
		}

		public bool GetHasSettings()
		{
			return true;
		}

		public void DisableKeyboardInput()
		{
			this.keyboardInputEnabled = false;
		}

		public void EnableKeyboardInput()
		{
			this.keyboardInputEnabled = true;
		}

		public void ShowSettings(IWin32Window owner)
		{
			settingsShown = true;
			using (var settingsForm = new JohnnyInputSettings(this.devices, JohnnyInputPlugin.bindingsFilePath))
			{
				if (settingsForm.ShowDialog(owner) != DialogResult.Cancel)
				{
					this.LoadKeys();
				}
			}
			settingsShown = false;
		}

		private List<InputButton> _lastConsoleEvents = new List<InputButton>();
		public void CheckConsoleEvents()
		{
			if (settingsShown)
				return;

			var addedEvents = new List<InputButton>();
			var removedEvents = new List<InputButton>();
			var newEvents = new List<InputButton>();

			lock (_joyCheckerSemaphore)
			{
				if (_lastDeviceUpdate.Elapsed.TotalSeconds >= 1)
				{
					_lastDeviceUpdate = Stopwatch.StartNew();
					this.joyChecker.UpdateDeviceCache();
				}
				this.joyChecker.UpdateValueCache();

				this.AddConsoleEventIfNecessary(InputButton.ConsoleStateSave, newEvents);
				this.AddConsoleEventIfNecessary(InputButton.ConsoleStateLoad, newEvents);
				this.AddConsoleEventIfNecessary(InputButton.ConsoleStateSlotPrevious, newEvents);
				this.AddConsoleEventIfNecessary(InputButton.ConsoleStateSlotNext, newEvents);
				this.AddConsoleEventIfNecessary(InputButton.ConsoleFullScreen, newEvents);
				this.AddConsoleEventIfNecessary(InputButton.ConsoleScreenShot, newEvents);
				this.AddConsoleEventIfNecessary(InputButton.ConsolePause, newEvents);
				this.AddConsoleEventIfNecessary(InputButton.ConsoleAdvanceBySingleFrame, newEvents);
				this.AddConsoleEventIfNecessary(InputButton.ConsoleReset, newEvents);
				this.AddConsoleEventIfNecessary(InputButton.ConsoleExit, newEvents);
			}

			addedEvents = newEvents.Where(button => _lastConsoleEvents.All(x => x != button)).ToList();
			removedEvents = _lastConsoleEvents.Where(button => newEvents.All(x => x != button)).ToList();

			_lastConsoleEvents = newEvents;

			foreach (var addedEvent in addedEvents)
			{
				this.RaiseEvent(addedEvent);
			}
		}

		private void RaiseEvent(InputButton button)
		{
			ConsoleEvent function;

			switch (button)
			{
				case InputButton.ConsoleStateSave:
					function = ConsoleEvent.StateSave;
					break;
				case InputButton.ConsoleStateLoad:
					function = ConsoleEvent.StateLoad;
					break;
				case InputButton.ConsoleStateSlotPrevious:
					function = ConsoleEvent.StateSlotPrevious;
					break;
				case InputButton.ConsoleStateSlotNext:
					function = ConsoleEvent.StateSlotNext;
					break;
				case InputButton.ConsoleFullScreen:
					function = ConsoleEvent.FullScreen;
					break;
				case InputButton.ConsoleScreenShot:
					function = ConsoleEvent.ScreenShot;
					break;
				case InputButton.ConsolePause:
					function = ConsoleEvent.Pause;
					break;
				case InputButton.ConsoleAdvanceBySingleFrame:
					function = ConsoleEvent.AdvanceBySingleFrame;
					break;
				case InputButton.ConsoleReset:
					function = ConsoleEvent.Reset;
					break;
				case InputButton.ConsoleExit:
					function = ConsoleEvent.Exit;
					break;
				default:
					// Screw it
					return;
			}

			if (this.ConsoleEventRaised != null)
				this.ConsoleEventRaised(function);
		}

		private void AddConsoleEventIfNecessary(InputButton button, List<InputButton> newEvents)
		{
			if (this.CheckDownButton(0, button))
			{
				newEvents.Add(button);
			}
		}

		static int frameNumber = 0;
		public byte[] GetPbusData()
		{
			lock (_joyCheckerSemaphore)
			{
				frameNumber++;
				if (frameNumber%60 == 0 || _lastDeviceUpdate.Elapsed.TotalSeconds >= 1)
				{
					frameNumber = 0;
					_lastDeviceUpdate = Stopwatch.StartNew();
					this.joyChecker.UpdateDeviceCache();
				}
				this.joyChecker.UpdateValueCache();

				// Find out how many devices are connected.
				int deviceCount;
				for (deviceCount = this.devices.Count; deviceCount >= 1; deviceCount--)
				{
					if (this.devices[deviceCount - 1].BindingSets.GetTriggerCount() > 0)
						break;
				}

				////////////////////////////
				// Set up raw data to return.
				var data = new byte[16];
				data[0x0] = 0x00;
				data[0x1] = 0x48;
				data[0x2] = this.CalculateDeviceLowByte(1, deviceCount);
				data[0x3] = this.CalculateDeviceHighByte(1, deviceCount);
				data[0x4] = this.CalculateDeviceLowByte(3, deviceCount);
				data[0x5] = this.CalculateDeviceHighByte(3, deviceCount);
				data[0x6] = this.CalculateDeviceLowByte(2, deviceCount);
				data[0x7] = this.CalculateDeviceHighByte(2, deviceCount);
				data[0x8] = this.CalculateDeviceLowByte(5, deviceCount);
				data[0x9] = this.CalculateDeviceHighByte(5, deviceCount);
				data[0xA] = this.CalculateDeviceLowByte(4, deviceCount);
				data[0xB] = this.CalculateDeviceHighByte(4, deviceCount);
				data[0xC] = 0x00;
				data[0xD] = 0x80;
				data[0xE] = this.CalculateDeviceLowByte(6, deviceCount);
				data[0xF] = this.CalculateDeviceHighByte(6, deviceCount);

				return data;
			}
		}

		private byte CalculateDeviceLowByte(int deviceNumber, int deviceCount)
		{
			byte returnValue = 0;

			if (deviceNumber >= deviceCount)
				return returnValue;

			returnValue |= 0x01 & 0; // unknown
			returnValue |= 0x02 & 0; // unknown
			returnValue |= this.CheckDownButton(deviceNumber, InputButton.L) ? (byte)0x04 : (byte)0;
			returnValue |= this.CheckDownButton(deviceNumber, InputButton.R) ? (byte)0x08 : (byte)0;
			returnValue |= this.CheckDownButton(deviceNumber, InputButton.X) ? (byte)0x10 : (byte)0;
			returnValue |= this.CheckDownButton(deviceNumber, InputButton.P) ? (byte)0x20 : (byte)0;
			returnValue |= this.CheckDownButton(deviceNumber, InputButton.C) ? (byte)0x40 : (byte)0;
			returnValue |= this.CheckDownButton(deviceNumber, InputButton.B) ? (byte)0x80 : (byte)0;
			return returnValue;
		}

		private byte CalculateDeviceHighByte(int deviceNumber, int deviceCount)
		{
			byte returnValue = 0;

			if (deviceNumber >= deviceCount)
				return returnValue;

			returnValue |= this.CheckDownButton(deviceNumber, InputButton.A)     ? (byte)0x01 : (byte)0;
			returnValue |= this.CheckDownButton(deviceNumber, InputButton.Left)  ? (byte)0x02 : (byte)0;
			returnValue |= this.CheckDownButton(deviceNumber, InputButton.Right) ? (byte)0x04 : (byte)0;
			returnValue |= this.CheckDownButton(deviceNumber, InputButton.Up)    ? (byte)0x08 : (byte)0;
			returnValue |= this.CheckDownButton(deviceNumber, InputButton.Down)  ? (byte)0x10 : (byte)0;
			returnValue |= 0x20 & 0; // unknown
			returnValue |= 0x40 & 0; // unknown
			returnValue |= 0x80; // This last bit seems to indicate power and/or connectivity.
			return returnValue;
		}

		#endregion // IInputPlugin Implementation

		private void LoadKeys()
		{
			InputBindingDevices newDevices = null;
			try
			{
				newDevices = InputBindingDevices.LoadFromFile(JohnnyInputPlugin.bindingsFilePath);
			}
			catch (Exception ex)
			{
				var message = "Failed when loading bindings: " + ex.ToString();
				Trace.WriteLine(message);
				Console.WriteLine(message);
			}

			if (newDevices == null)
				newDevices = InputBindingDevices.LoadDefault();

			/////////////
			// Use the new bindings.

			// TODO: Does this need to be thread protected?
			this.devices = newDevices;
		}

		private bool CheckDownButton(int deviceNumber, InputButton button)
		{
			if (this.devices == null)
				return false;

			List<InputTrigger> triggers = this.devices.GetTriggers(deviceNumber, button);
			if (triggers == null || this.settingsShown)
				return false;

			foreach (InputTrigger trigger in triggers)
			{
				if (trigger is KeyboardInputTrigger)
				{
					if (this.keyboardInputEnabled)
					{
						if (JohnnyInputPlugin.IsKeyboardButtonDown(((KeyboardInputTrigger)trigger).Key))
							return true;
					}
				}
				else if (trigger is JoystickTrigger)
				{
					var joyTrigger = (JoystickTrigger)trigger;
					if (this.joyChecker.CheckTrigger(joyTrigger))
						return true;
				}
			}
  
			return false;
		}

		public static bool IsKeyboardButtonDown(Keys key)
		{
			return (GetKeyState((int)key) & KEY_PRESSED) > 0;
		}
	}
}
