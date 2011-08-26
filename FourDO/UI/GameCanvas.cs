// TODO: Currently, the invalidate rect does the whole screen. It could technically do just the rectangle including the image to blit.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FourDO.Emulation;
using FourDO.Emulation.FreeDO;

namespace FourDO.UI
{
	public partial class GameCanvas : UserControl
	{
		private const InterpolationMode SMOOTH_SCALING_MODE = InterpolationMode.Low;
		
		private const int bitmapWidth = 320;
		private const int bitmapHeight = 240;

		private Pen thickBlackPen = new Pen(Color.FromArgb(0, 0, 0), 5);
		private Pen screenBorderPen = new Pen(Color.FromArgb(50, 50, 50));

		private Bitmap bitmapA = new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format24bppRgb);
		private Bitmap bitmapB = new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format24bppRgb);
		private Bitmap bitmapC = new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format24bppRgb);
		private Bitmap bitmapEmpty = new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format24bppRgb);

		private Bitmap currentFrontendBitmap;
		private Bitmap lastDrawnBackgroundBitmap;
		
		private object bitmapSemaphore = new object();

		// I added a frameskip to help ensure that the goofy main form controls can update. Damn windows forms!!
		private volatile int refreshReliefSkips = 0; 
		private volatile int frameSkip = 1; 
		private long frameNum = 0;

		private long scanDrawTime = 0;

		private bool preserveAspectRatio = true;
		private InterpolationMode scalingMode = SMOOTH_SCALING_MODE;

		private bool isGraphicsIntensive = false;
		private bool isConsoleStopped = true;

		public GameCanvas()
		{
			InitializeComponent();

			currentFrontendBitmap = bitmapA;
			lastDrawnBackgroundBitmap = bitmapB;

			GameConsole.Instance.FrameDone += new EventHandler(GameConsole_FrameDone);
			GameConsole.Instance.ConsoleStateChange += new ConsoleStateChangeHandler(GameConsole_ConsoleStateChange);

			double maxRefreshRate = (double)Utilities.DisplayHelper.GetMaximumRefreshRate();
			this.scanDrawTime = (long)((1 / maxRefreshRate) * Utilities.PerformanceCounter.Frequency);
		}

		public bool PreserveAspectRatio
		{
			get
			{
				return this.preserveAspectRatio;
			}
			set
			{
				this.preserveAspectRatio = value;
				this.Invalidate();
			}
		}

		public bool ImageSmoothing
		{
			get
			{
				return (this.scalingMode == SMOOTH_SCALING_MODE);
			}
			set
			{
				this.scalingMode = value ? SMOOTH_SCALING_MODE : InterpolationMode.NearestNeighbor;
				this.Invalidate();
			}
		}

		/// <summary>
		/// Requests the draw canvas to skip a few frames so that base UI can update.
		/// </summary>
		/// <returns></returns>
		public void RequestRefresh()
		{
			// If we're nowhere near choking the UI, screw them.
			if (this.isGraphicsIntensive == false)
				return;

			refreshReliefSkips = 5;
		}

		private unsafe void GameConsole_FrameDone(object sender, EventArgs e)
		{
			// Skip frames?
			this.frameNum++;
			if (this.frameNum % (this.frameSkip + 1 + refreshReliefSkips) > 0)
			{
				if (refreshReliefSkips > 0)
					refreshReliefSkips--;
				return;
			}

			/////////////// 
			// Choose the best bitmap to do a background render to
			Bitmap bitmapToCalc;
			lock (bitmapSemaphore)
			{
				if ((bitmapA != currentFrontendBitmap) && (bitmapA != lastDrawnBackgroundBitmap))
					bitmapToCalc = bitmapA;
				else if ((bitmapB != currentFrontendBitmap) && (bitmapB != lastDrawnBackgroundBitmap))
					bitmapToCalc = bitmapB;
				else
					bitmapToCalc = bitmapC;
			}

			int frameNum = (bitmapToCalc == bitmapA) ? 1 : 2;

			int bitmapHeight = bitmapToCalc.Height;
			int bitmapWidth = bitmapToCalc.Width;
			BitmapData bitmapData = bitmapToCalc.LockBits(new Rectangle(0, 0, bitmapToCalc.Width, bitmapToCalc.Height), ImageLockMode.WriteOnly, bitmapToCalc.PixelFormat);
			int bitmapStride = bitmapData.Stride;

			byte* destPtr = (byte*)bitmapData.Scan0.ToPointer();
			VDLFrame* framePtr = (VDLFrame*)GameConsole.Instance.CurrentFrame.ToPointer();
			for (int line = 0; line < bitmapHeight; line++)
			{
				VDLLine* linePtr = (VDLLine*)&(framePtr->lines[sizeof(VDLLine) * line]);
				short* srcPtr = (short*)linePtr;
				for (int pix = 0; pix < bitmapWidth; pix++)
				{
					*destPtr++ = (byte)(linePtr->xCLUTG[(*srcPtr) & 0x1F]);
					*destPtr++ = linePtr->xCLUTG[((*srcPtr) >> 5) & 0x1F];
					*destPtr++ = linePtr->xCLUTR[(*srcPtr) >> 10 & 0x1F];
					srcPtr++;
				}
			}

			bitmapToCalc.UnlockBits(bitmapData);

			lastDrawnBackgroundBitmap = bitmapToCalc;

			this.Invalidate(this.getBlitRect());
		}

		private void GameCanvas_Paint(object sender, PaintEventArgs e)
		{
			long sampleBefore = Utilities.PerformanceCounter.Current;
			if (isConsoleStopped == true)
				currentFrontendBitmap = bitmapEmpty;
			else
				currentFrontendBitmap = lastDrawnBackgroundBitmap;

			Rectangle blitRect = this.getBlitRect();
			Graphics g = e.Graphics;
			g.InterpolationMode = this.scalingMode;

			int rectFudgeFactor = (blitRect.Width / bitmapWidth) / 4;
			g.DrawRectangle(this.thickBlackPen, blitRect.X + 2, blitRect.Y + 2, blitRect.Width - 4, blitRect.Height - 4);
			g.DrawImage(currentFrontendBitmap, blitRect.X, blitRect.Y, blitRect.Width, blitRect.Height);
			g.DrawRectangle(this.screenBorderPen, blitRect.X - 1, blitRect.Y - 1, blitRect.Width + 1, blitRect.Height + 1);

			// If we're taking longer than half of the scan time to draw, do a frame skip.
			if ((Utilities.PerformanceCounter.Current - sampleBefore) * 2 > scanDrawTime)
			{
				this.isGraphicsIntensive = true;
				this.frameSkip = 1;
			}
			else
			{
				this.isGraphicsIntensive = false;
				this.frameSkip = 0;
			}
		}

		private Rectangle getBlitRect()
		{
			double imageAspect = bitmapWidth / (double)bitmapHeight;
			double screenAspect = this.Width / (double)this.Height;

			Rectangle blitRect = new Rectangle();

			if (this.preserveAspectRatio == false)
			{
				blitRect.X = 0;
				blitRect.Y = 0;
				blitRect.Width = this.Width;
				blitRect.Height = this.Height;
			}
			else
			{
				if (screenAspect > imageAspect)
				{
					// window is wider than it should be.
					blitRect.Width = (int)(bitmapWidth * (this.Height / (double)bitmapHeight));
					blitRect.Height = this.Height;
					blitRect.X = (this.Width - blitRect.Width) / 2;
					blitRect.Y = 0;
				}
				else
				{
					blitRect.Width = this.Width;
					blitRect.Height = (int)(bitmapHeight * (this.Width / (double)bitmapWidth));
					blitRect.X = 0;
					blitRect.Y = (this.Height - blitRect.Height) / 2;
				}
			}

			return blitRect;
		}

		private void GameConsole_ConsoleStateChange(ConsoleStateChangeEventArgs e)
		{
			this.isConsoleStopped = (e.NewState == ConsoleState.Stopped);
		}
	}
}