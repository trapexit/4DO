// This is the foster home of things that I want plugins to have access to.
// These types of things will need to move to a DLL that both 4DO and its plugins will reference.

using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace FourDO.Utilities.Globals
{
	public class Constants
	{
		private const string SETTINGS_SUBFOLDER_NAME = "Settings";

		public static string SettingsPath
		{
			get
			{
				return Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), SETTINGS_SUBFOLDER_NAME);
			}
		}

		private static CultureInfo systemDefaultCulture;
		public static CultureInfo SystemDefaultCulture
		{
			get
			{
				return systemDefaultCulture;
			}
		}
		
		public static void SetSystemDefaultCulture(CultureInfo info)
		{
			systemDefaultCulture = info;
		}
	}
}
