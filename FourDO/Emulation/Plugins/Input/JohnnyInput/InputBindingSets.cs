using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using FourDO.Utilities;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	[Serializable]
	public class InputBindingSets : IEnumerable<InputBindingSet>, ICloneable
	{
		#region Public static methods
		
		public static InputBindingSets LoadDefault()
		{
			var newSets = new InputBindingSets();

			var set = newSets.AddSet();
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
			set.SetBinding(InputButton.R, new KeyboardInputTrigger(Keys.Space));

			return newSets;
		}

		public static InputBindingSets LoadFromFile(string fileName)
		{
			InputBindingSets result = new InputBindingSets();
			CustomXmlSerializer serializer = new CustomXmlSerializer();
			serializer.ReadXml(fileName, result);
			return result;
		}

		#endregion // Public static methods

		protected List<InputBindingSet> sets = new List<InputBindingSet>();

		public InputBindingSets()
		{
			var newSet = new InputBindingSet();
		}

		public void SaveToFile(string fileName)
		{
			CustomXmlSerializer serializer = new CustomXmlSerializer();
			serializer.IncludeClassNameAttribute = true;
			serializer.WriteFile(this, fileName, true);
		}

		public void SetBinding(int setNumber, InputButton button, InputTrigger trigger)
		{
			if (setNumber >= this.sets.Count)
				return;

			var set = this.sets[setNumber];
			set.SetBinding(button, trigger);
		}

		public InputTrigger GetTrigger(int setNumber, InputButton button)
		{
			if (setNumber >= this.sets.Count)
				return null;

			var set = this.sets[setNumber];
			return set.GetButtonTrigger(button);
		}

		public List<InputTrigger> GetTriggers(InputButton button)
		{
			var returnValue = new List<InputTrigger>();
			foreach (InputBindingSet set in this.sets)
			{
				InputTrigger trigger = set.GetButtonTrigger(button);
				if (trigger != null)
					returnValue.Add(trigger);
			}
			return returnValue;
		}

		public IEnumerator<InputBindingSet> GetEnumerator()
		{
			return this.sets.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.sets.GetEnumerator();
		}

		public InputBindingSet AddSet()
		{
			var newSet = new InputBindingSet();
			this.sets.Add(newSet);
			return newSet;
		}

		public void RemoveSet(int index)
		{
			if (index >= 0 && index < this.sets.Count)
				this.RemoveSet(this.sets[index]);
		}

		public void RemoveSet(InputBindingSet set)
		{
			this.sets.Remove(set);
		}

		public InputBindingSet this[int index]
		{
			get
			{
				return this.sets[index];
			}
		}

		public void Clear()
		{
			this.sets.Clear();
		}

		public int Count
		{
			get 
			{
				return this.sets.Count;
			}
		}

		#region ICloneable Implementation

		public object Clone()
		{
			using (var stream = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(stream, this);
				stream.Position = 0;
				return (InputBindingSets)formatter.Deserialize(stream);
			}
		}

		#endregion // ICloneable Implementation

		#region Serialization Functions

		public void Add(InputBindingSet set)
		{
			this.sets.Add(set);
		}

		#endregion // Serialization Functions
	}
}
