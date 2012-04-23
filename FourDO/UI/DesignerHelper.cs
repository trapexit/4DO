using System.ComponentModel;

namespace FourDO.UI
{
	internal static class DesignerHelper
	{
		public static bool IsInDesignMode()
		{
			return (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
		}
	}
}
