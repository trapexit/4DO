using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
    [Serializable]
    public class KeyboardInputTrigger : InputTrigger
    {
        public override string FriendlyName 
        {
            get
            {
                return this.Key.ToString();
            }
        }

        // Empty constructor only for serialization
        public KeyboardInputTrigger() {}

        public KeyboardInputTrigger(Keys key)
        {
            this.Key = key;
        }

        public Keys Key { get; set; }
    }
}
