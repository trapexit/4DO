using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FourDO.Utilities
{
	public static class MouseHider
	{
		[StructLayout(LayoutKind.Sequential)]
		private struct POINT
		{
			public int X;
			public int Y;

			public POINT(int x, int y)
			{
				this.X = x;
				this.Y = y;
			}

			public static implicit operator System.Drawing.Point(POINT p)
			{
				return new System.Drawing.Point(p.X, p.Y);
			}

			public static implicit operator POINT(System.Drawing.Point p)
			{
				return new POINT(p.X, p.Y);
			}
		}

		[DllImport("user32.dll")]
		private static extern IntPtr WindowFromPoint(POINT Point);

		public static IntPtr GetHWndAtPoint(System.Drawing.Point point)
		{
			var newPoint = new POINT(point.X, point.Y);
			return WindowFromPoint(newPoint);
		}

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
