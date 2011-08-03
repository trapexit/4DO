using System;
using FourDO.Emulation.Plugins.Audio;

namespace FourDO.Emulation.Plugins
{
    internal static class PluginLoader
    {
        public static IAudioPlugin LoadAudioPlugin()
        {
            return new DefaultAudioPlugin();
        }
    }
}
