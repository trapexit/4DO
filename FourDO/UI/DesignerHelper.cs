﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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
