using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
    [Serializable]
    public abstract class InputTrigger
    {
        public abstract string FriendlyName { get; }
    }
}
