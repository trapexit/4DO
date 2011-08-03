using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FourDO.Emulation.Plugins
{
    public interface IAudioPlugin
    {
        void Destroy();

        void PushSample(uint dspSample);
    }
}
