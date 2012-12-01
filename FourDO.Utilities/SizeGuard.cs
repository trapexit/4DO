using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FourDO.Utilities
{
	public class SizeGuard
	{
		private const int WMSZ_LEFT = 1;
		private const int WMSZ_RIGHT = 2;
		private const int WMSZ_TOP = 3;
		private const int WMSZ_TOPLEFT = 4;
		private const int WMSZ_TOPRIGHT = 5;
		private const int WMSZ_BOTTOM = 6;
		private const int WMSZ_BOTTOMLEFT = 7;
		private const int WMSZ_BOTTOMRIGHT = 8;

		private const int WM_SIZING = 0x214;
		private const int WM_SIZE = 0x5;
		private const int SIZE_MAXIMIZED = 2;

		public int BaseHeight { get; set; }
		public int BaseWidth { get; set; }

		public int WindowExtraHeight { get; set; }
		public int WindowExtraWidth { get; set; }

		public bool Enabled { get; set; }

		public SizeGuard()
		{
			this.Enabled = true;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct SIZERECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		public void WatchForResize(ref Message m)
		{
			if (!Enabled)
				return;

			if (m.Msg != WM_SIZING)
				return;

			SIZERECT rect = (SIZERECT)Marshal.PtrToStructure(m.LParam, typeof(SIZERECT));

			int widthIncrement = this.BaseWidth / 2;
			int heightIncrement = this.BaseHeight / 2;

			int targetWidth = ((rect.Right - rect.Left - this.WindowExtraWidth + (widthIncrement/2)) / widthIncrement) * widthIncrement;
			if (targetWidth == 0)
				targetWidth = widthIncrement;
			targetWidth += this.WindowExtraWidth;

			int targetHeight = ((rect.Bottom - rect.Top - this.WindowExtraHeight + (heightIncrement/2)) / heightIncrement) * heightIncrement;
			if (targetHeight == 0)
				targetHeight = heightIncrement;
			targetHeight += this.WindowExtraHeight;

			int wParam = m.WParam.ToInt32();
			
			if (wParam == WMSZ_RIGHT || wParam == WMSZ_TOPRIGHT || wParam == WMSZ_BOTTOMRIGHT)
				rect.Right = rect.Left + targetWidth;
			else if (wParam == WMSZ_LEFT || wParam == WMSZ_TOPLEFT || wParam == WMSZ_BOTTOMLEFT)
				rect.Left = rect.Right - targetWidth;

			if (wParam == WMSZ_BOTTOM || wParam == WMSZ_BOTTOMLEFT || wParam == WMSZ_BOTTOMRIGHT)
				rect.Bottom = rect.Top + targetHeight;
			else if (wParam == WMSZ_TOP || wParam == WMSZ_TOPLEFT || wParam == WMSZ_TOPRIGHT)
				rect.Top = rect.Bottom - targetHeight;

			Marshal.StructureToPtr(rect, m.LParam, false);
		}
	}
}
