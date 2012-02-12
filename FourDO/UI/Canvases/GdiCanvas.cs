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

namespace FourDO.UI.Canvases
{
	internal partial class GdiCanvas : UserControl, IDisposable, ICanvas
	{
		private const InterpolationMode SMOOTH_SCALING_MODE = InterpolationMode.Low;
		
		private const int bitmapWidth = 320;
		private const int bitmapHeight = 240;

		private Pen thickBlackPen = new Pen(Color.FromArgb(0, 0, 0), 5);
		private Pen screenBorderPen = new Pen(Color.FromArgb(50, 50, 50));

		protected BitmapBunch bitmapBunch;

		// I added a frameskip to help ensure that the goofy main form controls can update. Damn windows forms!!
		private volatile int refreshReliefSkips = 0; 
		private volatile int frameSkip = 1; 
		private long frameNum = 0;

		private long scanDrawTime = 0;

		private bool preserveAspectRatio = true;
		private InterpolationMode scalingMode = SMOOTH_SCALING_MODE;

		private bool isGraphicsIntensive = false;

		public GdiCanvas()
		{
			InitializeComponent();
		}

		public void Initialize()
		{
			this.bitmapBunch = new BitmapBunch(bitmapWidth, bitmapHeight, PixelFormat.Format24bppRgb);

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

		public bool IsInResizeMode { get; set; }

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

		public void Destroy()
		{
			this.Dispose();
		}

		private void GameCanvas_Paint(object sender, PaintEventArgs e)
		{
			long sampleBefore = Utilities.PerformanceCounter.Current;

			Bitmap bitmapToRender = this.bitmapBunch.GetNextRenderBitmap();
			if (bitmapToRender == null)
				bitmapToRender = this.bitmapBunch.GetBlackBitmap();

			Rectangle blitRect = new Rectangle(0, 0, this.Width, this.Height);
			Graphics g = e.Graphics;
			g.InterpolationMode = this.scalingMode;

			g.DrawImage(bitmapToRender, blitRect.X, blitRect.Y, blitRect.Width, blitRect.Height);

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

		public void PushFrame(IntPtr currentFrame)
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
			Bitmap bitmapToPrepare = this.bitmapBunch.GetNextPrepareBitmap();

			int bitmapHeight = bitmapToPrepare.Height;
			int bitmapWidth = bitmapToPrepare.Width;
			BitmapData bitmapData = bitmapToPrepare.LockBits(new Rectangle(0, 0, bitmapToPrepare.Width, bitmapToPrepare.Height), ImageLockMode.WriteOnly, bitmapToPrepare.PixelFormat);
			int bitmapStride = bitmapData.Stride;

			unsafe
			{
				byte* destPtr = (byte*)bitmapData.Scan0.ToPointer();
				VDLFrame* framePtr = (VDLFrame*)currentFrame.ToPointer();
				for (int line = 0; line < bitmapHeight; line++)
				{
					VDLLine* linePtr = (VDLLine*)&(framePtr->lines[sizeof(VDLLine) * line]);
					short* srcPtr = (short*)linePtr;
					for (int pix = 0; pix < bitmapWidth; pix++)
					{
						*destPtr++ = (byte)(linePtr->xCLUTB[(*srcPtr) & 0x1F]);
						*destPtr++ = linePtr->xCLUTG[((*srcPtr) >> 5) & 0x1F];
						*destPtr++ = linePtr->xCLUTR[(*srcPtr) >> 10 & 0x1F];
						srcPtr++;
					}
				}
			}

			bitmapToPrepare.UnlockBits(bitmapData);

			this.bitmapBunch.SetLastPreparedBitmap(bitmapToPrepare);

			this.Invalidate();
		}
	}
}