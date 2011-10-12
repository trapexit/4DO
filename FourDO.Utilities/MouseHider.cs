using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FourDO.UI
{
	public static class MouseHider
	{
		private static bool isMouseShown = true;
		private static volatile object mouseShownLock = new object();

		public static void Show()
		{
			lock (MouseHider.mouseShownLock)
			{
				if (!MouseHider.isMouseShown)
				{
					Cursor.Show();
					MouseHider.isMouseShown = true;
				}
			}
		}

		public static void Hide()
		{
			lock (MouseHider.mouseShownLock)
			{
				if (MouseHider.isMouseShown)
				{
					Cursor.Hide();
					MouseHider.isMouseShown = false;
				}
			}
		}
	}
}
