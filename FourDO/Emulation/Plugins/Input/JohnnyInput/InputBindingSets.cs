using System;
using System.Collections.Generic;
using System.Linq;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	[Serializable]
	public class InputBindingSets : IEnumerable<InputBindingSet>
	{
		protected List<InputBindingSet> sets = new List<InputBindingSet>();

		public InputBindingSets()
		{
			var newSet = new InputBindingSet();
		}

		#region Public Functions

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

		public int GetTriggerCount()
		{
		    return this.sets.Sum(set => set.Count);
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

		#endregion // Public Functions

		#region IEnumerable Implementation

		public IEnumerator<InputBindingSet> GetEnumerator()
		{
			return this.sets.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.sets.GetEnumerator();
		}

		#endregion // IEnumerable Implementation

		#region Serialization Functions

		public void Add(InputBindingSet set)
		{
			this.sets.Add(set);
		}

		#endregion // Serialization Functions
	}
}
