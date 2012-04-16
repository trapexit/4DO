using System;
using FourDO.Emulation.Plugins.Audio.JohnnyAudio;
using FourDO.Emulation.Plugins.Input.JohnnyInput;
using FourDO.Emulation.Plugins.Audio;

namespace FourDO.Emulation.Plugins
{
	internal static class PluginLoader
	{
		private static IAudioPlugin currentAudioPlugin = null;
		private static IInputPlugin currentInputPlugin = null;

		public static IAudioPlugin GetAudioPlugin()
		{
			if (currentAudioPlugin == null)
				currentAudioPlugin = new JohnnyAudioPlugin();

			return currentAudioPlugin;
		}

		public static IInputPlugin GetInputPlugin()
		{
			if (currentInputPlugin == null)
				currentInputPlugin = new JohnnyInputPlugin();

			return currentInputPlugin;
		}
	}
}
