using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
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

			var device = newSettings.AddDevice();
			var set = device.BindingSets.AddSet();
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

			return newSettings;
		}

		public static InputBindingDevices LoadFromFile(string fileName)
		{
			if (!File.Exists(fileName))
				return null;

			InputBindingDevices result = new InputBindingDevices();
			CustomXmlSerializer serializer = new CustomXmlSerializer();
			serializer.ReadXml(fileName, result);
			return result;
		}

		#endregion // Public static methods

		protected List<InputBindingDevice> devices = new List<InputBindingDevice>();

		#region Public Functions

		public void SaveToFile(string fileName)
		{
			// Create the directory if necessary.
			string directoryName = Path.GetDirectoryName(fileName);
			if (!Directory.Exists(directoryName))
				Directory.CreateDirectory(directoryName);
			
			CustomXmlSerializer serializer = new CustomXmlSerializer();
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
				return (InputBindingDevices)formatter.Deserialize(stream);
			}
		}

		#endregion // ICloneable Implementation
	}
}
