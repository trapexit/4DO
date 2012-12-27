using System.Windows.Forms;
using FourDO.Emulation.Plugins.Input;

namespace FourDO.Emulation.Plugins
{
	public delegate void ConsoleEventRaisedHandler(ConsoleEvent consoleEvent);

	public interface IInputPlugin
	{
		void Destroy();

		bool GetHasSettings();
		void ShowSettings(IWin32Window owner);

		void DisableKeyboardInput();
		void EnableKeyboardInput();

		byte[] GetPbusData();

		void CheckConsoleEvents();
		event ConsoleEventRaisedHandler ConsoleEventRaised;
	}
}
