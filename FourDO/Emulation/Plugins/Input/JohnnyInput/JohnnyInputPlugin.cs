//////////////////////////////////////////////////////////
// JMK NOTES:
// I figured the quickest way to get controls in would just be
// to use global keyboard hooks. This is a bit lazy, especially
// because I don't realy think I want them global. Oh well.
//////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FourDO.Utilities;
using System.Runtime.InteropServices;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	internal class JohnnyInputPlugin : IInputPlugin
	{
		private const string BINDINGS_FILE_NAME = "JohnnyInputBindings.xml";

		[DllImport("user32.dll")]
		private static extern short GetKeyState(int keyCode);
		private const int KEY_PRESSED = 0x8000;

		private JoyInputChecker joyChecker = new JoyInputChecker();
		private InputBindingDevices devices;
		private static string bindingsFilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), BINDINGS_FILE_NAME);

		public JohnnyInputPlugin()
		{
			this.LoadKeys();
		}

		#region IInputPlugin Implementation

		public void Destroy() { }

		public bool GetHasSettings()
		{
			return true;
		}

		public void ShowSettings(IWin32Window owner)
		{
			using (var settingsForm = new JohnnyInputSettings(this.devices, JohnnyInputPlugin.bindingsFilePath))
			{
				if (settingsForm.ShowDialog(owner) != DialogResult.Cancel)
				{
					this.LoadKeys();
				}
			}
		}

		static int frameNumber = 0;
		public byte[] GetPbusData()
		{
			frameNumber++;
			if (frameNumber % 60 == 0)
				this.joyChecker.UpdateDeviceCache(); // NOTE: This is once every 60 frames... we could do it less often?
			this.joyChecker.UpdateValueCache();

			////////////////////////////
			// Set up raw data to return.
			byte[] data = new byte[16];
			data[0x0] = 0x00;
			data[0x1] = 0x48;
			data[0x2] = this.CalculateDeviceLowByte(0);
			data[0x3] = this.CalculateDeviceHighByte(0);
			data[0x4] = this.CalculateDeviceLowByte(2);
			data[0x5] = this.CalculateDeviceHighByte(2);
			data[0x6] = this.CalculateDeviceLowByte(1);
			data[0x7] = this.CalculateDeviceHighByte(1);
			data[0x8] = this.CalculateDeviceLowByte(4);
			data[0x9] = this.CalculateDeviceHighByte(4);
			data[0xA] = this.CalculateDeviceLowByte(3);
			data[0xB] = this.CalculateDeviceHighByte(3);
			data[0xC] = 0x00;
			data[0xD] = 0x80;
			data[0xE] = this.CalculateDeviceLowByte(5);
			data[0xF] = this.CalculateDeviceHighByte(5);

			return data;
		}

		private byte CalculateDeviceLowByte(int deviceNumber)
		{
			byte returnValue = 0;
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

		private byte CalculateDeviceHighByte(int deviceNumber)
		{
			byte returnValue = 0;
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
			JohnnyInputPlugin.bindingsFilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), BINDINGS_FILE_NAME);

			InputBindingDevices newDevices = null;
			try
			{
				newDevices = InputBindingDevices.LoadFromFile(JohnnyInputPlugin.bindingsFilePath);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed when loading bindings: " + ex.ToString());
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
			if (triggers == null)
				return false;

			foreach (InputTrigger trigger in triggers)
			{
				if (trigger is KeyboardInputTrigger)
				{
					if ((GetKeyState((int)((KeyboardInputTrigger)trigger).Key) & KEY_PRESSED) > 0)
						return true;
				}
				else if (trigger is JoystickTrigger)
				{
					JoystickTrigger joyTrigger = (JoystickTrigger)trigger;
					if (this.joyChecker.CheckTrigger(joyTrigger))
						return true;
				}
			}
  
			return false;
		}
	}
}
