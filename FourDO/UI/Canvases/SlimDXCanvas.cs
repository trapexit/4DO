using FourDO.Emulation.FreeDO;
using FourDO.Emulation;
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
using SlimDX.Direct3D9;
using SlimDX.Windows;

using Device = SlimDX.Direct3D9.Device;
using Resource = SlimDX.Direct3D9.Resource;
using DXGI = SlimDX.DXGI;

namespace FourDO.UI.Canvases
{
	public partial class SlimDXCanvas : UserControl , ICanvas
	{
		protected struct TexturedVertex
		{
			public TexturedVertex(Vector3 position, Vector2 texture)
			{
				this.Position = position;
				this.Texture = texture;
			}

			public Vector3 Position;
			public Vector2 Texture;
		}

		public bool ImageSmoothing { get; set; }

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
		protected readonly int textureWidth = RoundUpToNextPowerOfTwo(bitmapWidth);
		protected readonly int textureHeight = RoundUpToNextPowerOfTwo(bitmapHeight);

		protected Bitmap bitmapA = new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format32bppRgb);
		protected Bitmap bitmapB = new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format32bppRgb);
		protected Bitmap bitmapC = new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format32bppRgb);

		protected Bitmap currentFrontendBitmap;
		protected Bitmap lastDrawnBackgroundBitmap;

		protected object bitmapSemaphore = new object();

		protected Color4 colorBlack = new Color4(0, 0, 0);

		protected VertexDeclaration vertexDeclaration;
		protected VertexBuffer vertexBuffer;
		protected Texture texture;
		protected Direct3D direct3D;
		protected Device device;

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
		}

        public void Initialize()
        {
            // hook the application's idle event
            Application.Idle += new EventHandler(OnApplicationIdle);

            // Get maximum screen size.
            Size maxSize = new Size(bitmapWidth, bitmapHeight);
            foreach (var screen in Screen.AllScreens)
            {
                maxSize.Width = Math.Max(maxSize.Width, screen.Bounds.Width);
                maxSize.Height = Math.Max(maxSize.Height, screen.Bounds.Height);
            }

            /////////////////////////////////////////
            // Initialize direct3d 9

            this.direct3D = new Direct3D();

            var presentParams = new PresentParameters();
            presentParams.Windowed = true;
            presentParams.SwapEffect = SwapEffect.Discard;
            presentParams.DeviceWindowHandle = this.Handle;
            presentParams.BackBufferWidth = maxSize.Width;
            presentParams.BackBufferHeight = maxSize.Height;

            this.device = new Device(this.direct3D, 0, DeviceType.Hardware, this.Handle, CreateFlags.HardwareVertexProcessing, presentParams);

            this.device.SetRenderState(RenderState.Lighting, false);

            /////////////////
            // Set up texture.
            //this.texture = Texture.FromFile(this.device, @"C:\jmk\screens\ihateit.bmp");
            this.texture = new Texture(this.device, textureWidth, textureHeight, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);
            var desc = this.texture.GetLevelDescription(0);

            /////////////////
            // Set up vertex buffer
            this.vertexBuffer = new VertexBuffer(this.device
                , 4 * (Marshal.SizeOf(typeof(TexturedVertex)))
                , Usage.WriteOnly, VertexFormat.None, Pool.Default);
            DataStream vertexStream = this.vertexBuffer.Lock(0, 0, LockFlags.None);
            float maximumX = bitmapWidth / (float)textureWidth;
            float maximumY = bitmapHeight / (float)textureHeight;
            vertexStream.WriteRange(new[]{
				new TexturedVertex(new Vector3(-1.0f, 1.0f, 0.0f), new Vector2(0.0f,0.0f)),
				new TexturedVertex(new Vector3( 1.0f, 1.0f, 0.0f), new Vector2(maximumX,0.0f)),
				new TexturedVertex(new Vector3(-1.0f,-1.0f, 0.0f), new Vector2(0.0f,maximumY)),
				new TexturedVertex(new Vector3( 1.0f,-1.0f, 0.0f), new Vector2(maximumX,maximumY)) });

            this.vertexBuffer.Unlock();

            this.vertexDeclaration = new VertexDeclaration(this.device, new[] {
				new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0), 
				new VertexElement(0, 12, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0), 
				VertexElement.VertexDeclarationEnd});

            initialized = true;
        }

		public void Destroy()
        {
            Application.Idle -= new EventHandler(OnApplicationIdle);

            try
            {
                this.vertexBuffer.Dispose();
                this.texture.Dispose();
                this.vertexDeclaration.Dispose();
                this.device.Dispose();
                this.direct3D.Dispose();
            }
            catch { } // who cares.
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
			///////////////////////////
			// Update texture.
			this.currentFrontendBitmap = this.lastDrawnBackgroundBitmap; // This keeps the background from updating it too.
			Bitmap bitmapToDraw = this.currentFrontendBitmap;

			if (bitmapToDraw != null)
			{
				Surface textureSurface = this.texture.GetSurfaceLevel(0);
				DataRectangle dataRect = textureSurface.LockRectangle(LockFlags.None);
				BitmapData bitmapData = bitmapToDraw.LockBits(new Rectangle(0, 0, bitmapToDraw.Width, bitmapToDraw.Height), ImageLockMode.ReadOnly, bitmapToDraw.PixelFormat);
				{
					DataStream stream = dataRect.Data;
					int stride = bitmapData.Stride;
					int bitDepth = bitmapData.Stride / bitmapData.Width;
					IntPtr sourcePtr = bitmapData.Scan0;
					for (int y = 0; y < bitmapData.Height; y++) 
					{
						stream.WriteRange(sourcePtr, stride);
						stream.Position += (textureWidth - bitmapWidth) * bitDepth;
						sourcePtr += stride;
					}
				}
				bitmapToDraw.UnlockBits(bitmapData);
				textureSurface.UnlockRectangle();
			}

			///////////////////////////
			// Set up scaling algorithm.
			TextureFilter filter = this.ImageSmoothing ? TextureFilter.Linear : TextureFilter.Point;
			this.device.SetSamplerState(0, SamplerState.MinFilter, filter);
			this.device.SetSamplerState(0, SamplerState.MagFilter, filter);

			//////////////////////
			// Draw scene.
			this.device.Clear(ClearFlags.Target, colorBlack, 1f, 0);
			this.device.BeginScene();

			this.device.SetTexture(0, this.texture);
			this.device.SetStreamSource(0, this.vertexBuffer, 0, Marshal.SizeOf(typeof(TexturedVertex)));
			this.device.VertexDeclaration = this.vertexDeclaration;
			this.device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);

			this.device.EndScene();
			this.device.Present();
		}

		public unsafe void PushFrame(IntPtr currentFrame)
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
			VDLFrame* framePtr = (VDLFrame*)currentFrame.ToPointer();
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

		private static int RoundUpToNextPowerOfTwo(int x)
		{
			x--;
			x |= x >> 1;  // handle  2 bit numbers
			x |= x >> 2;  // handle  4 bit numbers
			x |= x >> 4;  // handle  8 bit numbers
			x |= x >> 8;  // handle 16 bit numbers
			x++;

			return x;
		}
	}
}
