using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
    [Serializable]
    public class InputBinding
    {
        public InputBinding()
        {
        }

        public InputBinding(InputButton button, InputTrigger trigger)
        {
            this.Button = button;
            this.Trigger = trigger;
        }

        public InputButton Button { get; set; }
        public InputTrigger Trigger { get; set; }
    }
}
