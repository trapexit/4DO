using System;
using FourDO.Emulation.Plugins.Audio;
using FourDO.Emulation.Plugins.Input;

namespace FourDO.Emulation.Plugins
{
    internal static class PluginLoader
    {
        private static IAudioPlugin currentAudioPlugin = null;
        private static IInputPlugin currentInputPlugin = null;

        public static IAudioPlugin GetAudioPlugin()
        {
            if (currentAudioPlugin == null)
                currentAudioPlugin = new DefaultAudioPlugin();

            return currentAudioPlugin;
        }

        public static IInputPlugin GetInputPlugin()
        {
            if (currentInputPlugin == null)
                currentInputPlugin = new DefaultInputPlugin();

            return currentInputPlugin;
        }
    }
}
