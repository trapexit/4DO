// This is the foster home of things that I want plugins to have access to.
// These types of things will need to move to a DLL that both 4DO and its plugins will reference.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FourDO.Utilities.Global
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
	}
}
