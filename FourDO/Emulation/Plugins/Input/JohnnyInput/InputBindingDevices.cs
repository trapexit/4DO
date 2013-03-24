using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using FourDO.Utilities;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	[Serializable]
	public class InputBindingDevices : IEnumerable<InputBindingDevice>, ICloneable
	{
		#region Public static methods

		public static InputBindingDevices LoadDefault()
		{
			var newSettings = new InputBindingDevices();

			// Add blank emulation event device and set.
			var newDevice = newSettings.AddDevice();
			newDevice.BindingSets.AddSet();

			// Add default Player 1 setup.
			var player1device = newSettings.AddDevice();
			var set = player1device.BindingSets.AddSet();
			set.SetBinding(InputButton.Up, new KeyboardInputTrigger(Keys.W));
			set.SetBinding(InputButton.Down, new KeyboardInputTrigger(Keys.S));
			set.SetBinding(InputButton.Left, new KeyboardInputTrigger(Keys.A));
			set.SetBinding(InputButton.Right, new KeyboardInputTrigger(Keys.D));
			set.SetBinding(InputButton.A, new KeyboardInputTrigger(Keys.J));
			set.SetBinding(InputButton.B, new KeyboardInputTrigger(Keys.K));
			set.SetBinding(InputButton.C, new KeyboardInputTrigger(Keys.L));
			set.SetBinding(InputButton.X, new KeyboardInputTrigger(Keys.Q));
			set.SetBinding(InputButton.P, new KeyboardInputTrigger(Keys.E));
			set.SetBinding(InputButton.L, new KeyboardInputTrigger(Keys.LShiftKey));
			set.SetBinding(InputButton.R, new KeyboardInputTrigger(Keys.OemQuestion));

			// Add blank emulation event device and set.
			for (var playerNum = 2; playerNum <= 6; playerNum ++)
			{
				var extraDevice = newSettings.AddDevice();
				extraDevice.BindingSets.AddSet();
			}

			return newSettings;
		}

		public static InputBindingDevices LoadFromFile(string fileName)
		{
			if (!File.Exists(fileName))
				return null;

			var result = new InputBindingDevices();
			var serializer = new CustomXmlSerializer();
			serializer.ReadXml(fileName, result);

			// If there was no format version, assume it was version 1.
			if (result.FormatVersion <= 0)
			{
				result.FormatVersion = 1;
			}

			// Format Version 1 skipped the "Console" device.
			if (result.FormatVersion == 1)
			{
				result.devices.Insert(0, new InputBindingDevice());
			}

			return result;
		}

		#endregion // Public static methods

		protected List<InputBindingDevice> devices = new List<InputBindingDevice>();

		public int FormatVersion { get; set; }

		#region Public Functions

		public void SaveToFile(string fileName)
		{
			// Create the directory if necessary.
			string directoryName = Path.GetDirectoryName(fileName);
			if (!Directory.Exists(directoryName))
				Directory.CreateDirectory(directoryName);

			// We're at version #2.
			this.FormatVersion = 2;

			// Save it.
			var serializer = new CustomXmlSerializer();
			serializer.IncludeClassNameAttribute = true;
			serializer.WriteFile(this, fileName, true);
			
		}

		public void SetBinding(int deviceNumber, int setNumber, InputButton button, InputTrigger trigger)
		{
			if (deviceNumber < 0 || deviceNumber >= this.devices.Count)
				return;

			var device = this.devices[deviceNumber];
			device.BindingSets.SetBinding(setNumber, button, trigger);
		}

		public InputTrigger GetTrigger(int deviceNumber, int setNumber, InputButton button)
		{
			if (deviceNumber < 0 || deviceNumber >= this.devices.Count)
				return null;

			var device = this.devices[deviceNumber];
			return device.BindingSets.GetTrigger(setNumber, button);
		}

		public List<InputTrigger> GetTriggers(int deviceNumber, InputButton button)
		{
			if (deviceNumber < 0 || deviceNumber >= this.devices.Count)
				return null;

			var device = this.devices[deviceNumber];
			return device.BindingSets.GetTriggers(button);
		}

		public InputBindingDevice AddDevice()
		{
			var newDevice = new InputBindingDevice();
			this.devices.Add(newDevice);
			return newDevice;
		}

		public void RemoveDevice(int index)
		{
			if (index >= 0 && index < this.devices.Count)
				this.RemoveDevice(this.devices[index]);
		}

		public void RemoveDevice(InputBindingDevice device)
		{
			this.devices.Remove(device);
		}

		public InputBindingDevice this[int index]
		{
			get
			{
				return this.devices[index];
			}
		}

		public void Clear()
		{
			this.devices.Clear();
		}

		public int Count
		{
			get
			{
				return this.devices.Count;
			}
		}

		#endregion // Public Functions

		#region Serialization Functions

		public void Add(InputBindingDevice device)
		{
			this.devices.Add(device);
		}

		#endregion // Serialization Functions

		#region IEnumerable Implementation

		public IEnumerator<InputBindingDevice> GetEnumerator()
		{
			return this.devices.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.devices.GetEnumerator();
		}

		#endregion IEnumerable Implementation

		#region ICloneable Implementation

		public object Clone()
		{
			using (var stream = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(stream, this);
				stream.Position = 0;
				return formatter.Deserialize(stream);
			}
		}

		#endregion // ICloneable Implementation
	}
}
