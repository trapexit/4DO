using System.ComponentModel;

namespace FourDO.Utilities
{
	public static class DesignerHelper
	{
		public static bool IsInRuntimeMode(IComponent component)
		{
			var ret = IsInDesignMode(component);
			return !ret;
		}

		public static bool IsInDesignMode(IComponent component)
		{
			var ret = false;
			if (null != component)
			{
				ISite site = component.Site;
				if (null != site)
				{
					ret = site.DesignMode;
				}
				else if (component is System.Windows.Forms.Control)
				{
					IComponent parent = ((System.Windows.Forms.Control)component).Parent;
					ret = IsInDesignMode(parent);
				}
			}

			return ret;
		}

        public static bool IsInDesignMode()
        {
            return (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
        }
	}
}
