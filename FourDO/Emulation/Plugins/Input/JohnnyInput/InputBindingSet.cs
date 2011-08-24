using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
    [Serializable]
    public class InputBindingSet
    {
        protected Dictionary<InputButton, InputBinding> bindings = new Dictionary<InputButton, InputBinding>();

        public void SetBinding(InputButton button, InputTrigger trigger)
        {
            if (trigger != null)
                this.bindings[button] = new InputBinding(button, trigger);
            else if (this.bindings.ContainsKey(button))
                this.bindings.Remove(button);
        }

        public InputTrigger GetButtonTrigger(InputButton button)
        {
            if (this.bindings.ContainsKey(button))
                return this.bindings[button].Trigger;
            else
                return null;
        }
    }
}
