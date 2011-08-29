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
			JohnnyInputSettings settingsForm = new JohnnyInputSettings(devices, JohnnyInputPlugin.bindingsFilePath);
			if (settingsForm.ShowDialog(owner) != DialogResult.Cancel)
			{
				this.LoadKeys();
			}
		}

		public byte[] GetPbusData()
		{
			byte[] data = new byte[16];
			byte calculatedByte = 0;

			data[0x0] = 0x00;
			data[0x1] = 0x49;

			calculatedByte = 0;
			calculatedByte |= this.CheckDownButton(InputButton.X) ? (byte)0x10 : (byte)0; // Positive!
			calculatedByte |= this.CheckDownButton(InputButton.P) ? (byte)0x20 : (byte)0; // Positive!
			calculatedByte |= this.CheckDownButton(InputButton.C) ? (byte)0x40 : (byte)0; // Positive!
			calculatedByte |= this.CheckDownButton(InputButton.B) ? (byte)0x80 : (byte)0; // Positive!
			//calculatedByte |= this.CheckDownButton(JohnnyInputButton.?) ? (byte)0x01 : (byte)0; // dunno
			//calculatedByte |= this.CheckDownButton(JohnnyInputButton.?) ? (byte)0x02 : (byte)0; // dunno
			calculatedByte |= this.CheckDownButton(InputButton.L) ? (byte)0x04 : (byte)0; // Positive!
			calculatedByte |= this.CheckDownButton(InputButton.R) ? (byte)0x08 : (byte)0; // Positive!
			data[0x2] = calculatedByte;

			calculatedByte = 0;
			calculatedByte |= this.CheckDownButton(InputButton.A) ? (byte)0x01 : (byte)0; // Positive!
			calculatedByte |= this.CheckDownButton(InputButton.Left) ? (byte)0x02 : (byte)0; // Positive!
			calculatedByte |= this.CheckDownButton(InputButton.Right) ? (byte)0x04 : (byte)0; // Positive!
			calculatedByte |= this.CheckDownButton(InputButton.Up) ? (byte)0x08 : (byte)0; // Positive!
			calculatedByte |= this.CheckDownButton(InputButton.Down) ? (byte)0x10 : (byte)0; // Positive!
			//calculatedByte |= this.CheckDownButton(JohnnyInputButton.?) ? (byte)0x20 : (byte)0; // dunno
			//calculatedByte |= this.CheckDownButton(JohnnyInputButton.?) ? (byte)0x40 : (byte)0; // dunno
			calculatedByte |= (byte)0x80;
			data[0x3] = calculatedByte;

			data[0x4] = 0xFF;
			data[0x5] = 0xFF;
			data[0x6] = 0x00;
			data[0x7] = 0x00;
			data[0x8] = 0xFF;
			data[0x9] = 0xFF;
			data[0xA] = 0xFF;
			data[0xB] = 0xFF;
			data[0xC] = 0xFF;
			data[0xD] = 0xFF;
			data[0xE] = 0xFF;
			data[0xF] = 0xFF;

			return data;
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

		private bool CheckDownButton(InputButton button)
		{
			if (this.devices == null)
				return false;

			List<InputTrigger> triggers = this.devices.GetTriggers(button);
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
				}
			}
  
			return false;
		}
	}
}
