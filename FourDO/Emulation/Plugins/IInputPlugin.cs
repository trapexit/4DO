using System.Windows.Forms;

namespace FourDO.Emulation.Plugins
{
	public interface IInputPlugin
	{
		void Destroy();

		bool GetHasSettings();
		void ShowSettings(IWin32Window owner);

		void DisableKeyboardInput();
		void EnableKeyboardInput();

		byte[] GetPbusData();
	}
}
