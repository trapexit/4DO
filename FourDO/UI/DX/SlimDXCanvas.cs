using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security;
using SlimDX;
using D2D = SlimDX.Direct2D;

using FourDO.Emulation;
using FourDO.Emulation.FreeDO;

namespace FourDO.UI.DX
{
	public partial class SlimDXCanvas : UserControl
	{
		public bool ImageSmoothing
		{
			get
			{
				return (this.interpolationMode == D2D.InterpolationMode.Linear);
			}
			set
			{
				this.interpolationMode = value ? D2D.InterpolationMode.Linear : D2D.InterpolationMode.NearestNeighbor;
			}
		}

		public bool IsInResizeMode
		{
			get
			{
				return this.ResizeRenderTimer.Enabled;
			}
			set
			{
				this.ResizeRenderTimer.Enabled = value;
			}
		}

		protected const int bitmapWidth = 320;
		protected const int bitmapHeight = 240;
		protected readonly Size bitmapSize = new Size(bitmapWidth, bitmapHeight);

		protected D2D.Bitmap dxBitmap;

		protected Bitmap bitmapA = new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format32bppRgb);
		protected Bitmap bitmapB = new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format32bppRgb);
		protected Bitmap bitmapC = new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format32bppRgb);

		protected Bitmap currentFrontendBitmap;
		protected Bitmap lastDrawnBackgroundBitmap;

		protected object bitmapSemaphore = new object();

		protected D2D.Factory factory;
		protected D2D.WindowRenderTarget renderTarget;
		
		protected Color4 colorBlack = new Color4(0, 0, 0);

		protected D2D.InterpolationMode interpolationMode = D2D.InterpolationMode.Linear;

		protected bool initialized = false;
		
		[StructLayout(LayoutKind.Sequential)]
		protected struct Message
		{
			public IntPtr hWnd;
			public uint msg;
			public IntPtr wParam;
			public IntPtr lParam;
			public uint time;
			public Point p;
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		protected static extern bool PeekMessage(out Message msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

		public SlimDXCanvas()
		{
			InitializeComponent();

			// hook the application's idle event
			Application.Idle += new EventHandler(OnApplicationIdle);

			GameConsole.Instance.FrameDone += new EventHandler(GameConsole_FrameDone);

			// Set up device.
			this.factory = new D2D.Factory();

			Size maxSize = new Size(bitmapWidth, bitmapHeight);
			foreach (var screen in Screen.AllScreens)
			{
				maxSize.Width = Math.Max(maxSize.Width, screen.Bounds.Width);
				maxSize.Height = Math.Max(maxSize.Height, screen.Bounds.Height);
			}
			
			this.renderTarget = new D2D.WindowRenderTarget(factory,
				new D2D.WindowRenderTargetProperties { Handle = this.Handle, PixelSize = maxSize});

			// Set up drawn image.
			D2D.BitmapProperties bitmapProperties = new D2D.BitmapProperties();
			bitmapProperties.PixelFormat = new D2D.PixelFormat(SlimDX.DXGI.Format.B8G8R8A8_UNorm, D2D.AlphaMode.Ignore);
			this.dxBitmap = new D2D.Bitmap(this.renderTarget, this.bitmapSize, bitmapProperties);

			initialized = true;
		}

		public void Destroy()
		{
			this.factory.Dispose();
			this.renderTarget.Dispose();
		}

		protected void OnApplicationIdle(object sender, EventArgs e)
		{
			while(this.GetIsAppStillIdle())
			{
				this.Render();
			}
		}

		protected void Render()
		{
			this.renderTarget.BeginDraw();
			this.renderTarget.Transform = Matrix3x2.Identity;
			this.renderTarget.Clear(colorBlack);

			this.currentFrontendBitmap = this.lastDrawnBackgroundBitmap; // This keeps the background from updating it too.
			Bitmap bitmapToDraw = this.currentFrontendBitmap;
			if (bitmapToDraw != null)
			{
				BitmapData bitmapData = bitmapToDraw.LockBits(new Rectangle(0, 0, bitmapToDraw.Width, bitmapToDraw.Height), ImageLockMode.ReadOnly, bitmapToDraw.PixelFormat);
				{
					dxBitmap.FromMemory(bitmapData.Scan0, bitmapData.Stride);
				}
				bitmapToDraw.UnlockBits(bitmapData);

				this.renderTarget.DrawBitmap(
						dxBitmap,
						new Rectangle(0, 0, (int)this.renderTarget.Size.Width, (int)this.renderTarget.Size.Height),
						1f,
						this.interpolationMode);
			}
			this.renderTarget.EndDraw();
		}

		protected unsafe void GameConsole_FrameDone(object sender, EventArgs e)
		{
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
					destPtr++;
					srcPtr++;
				}
			}

			bitmapToCalc.UnlockBits(bitmapData);

			lastDrawnBackgroundBitmap = bitmapToCalc;
		}

		protected bool GetIsAppStillIdle()
		{
			Message msg;
			return !PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
		}

		private void ResizeRenderTimer_Tick(object sender, EventArgs e)
		{
			if (this.IsInResizeMode && this.initialized)
				this.Render();
		}

		private void SlimDXCanvas_Resize(object sender, EventArgs e)
		{
			if (this.IsInResizeMode && this.initialized)
				this.Render();
		}
	}
}
