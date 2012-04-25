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
	internal partial class SlimDXCanvas : UserControl, ICanvas
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

		public bool RenderHighResolution { get; set; }

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

		protected const int HIGH_RES_WIDTH = 640;
		protected const int HIGH_RES_HEIGHT = 480;

		private const int LOW_RES_WIDTH = HIGH_RES_WIDTH / 2;
		private const int LOW_RES_HEIGHT = HIGH_RES_HEIGHT / 2;

		protected readonly int textureWidth = RoundUpToNextPowerOfTwo(HIGH_RES_WIDTH);
		protected readonly int textureHeight = RoundUpToNextPowerOfTwo(HIGH_RES_HEIGHT);

		protected readonly float maximumX = (HIGH_RES_WIDTH / (float)RoundUpToNextPowerOfTwo(HIGH_RES_WIDTH));
		protected readonly float maximumY = (HIGH_RES_HEIGHT / (float)RoundUpToNextPowerOfTwo(HIGH_RES_HEIGHT));

		protected BitmapBunch bitmapBunch;

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
			// Get maximum screen size.
			Size maxSize = new Size(HIGH_RES_WIDTH, HIGH_RES_HEIGHT);
			foreach (var screen in Screen.AllScreens)
			{
				maxSize.Width = Math.Max(maxSize.Width, screen.Bounds.Width);
				maxSize.Height = Math.Max(maxSize.Height, screen.Bounds.Height);
			}

			this.bitmapBunch = new BitmapBunch(HIGH_RES_WIDTH, HIGH_RES_HEIGHT, PixelFormat.Format32bppRgb);

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
			this.texture = new Texture(this.device, textureWidth, textureHeight, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);
			var desc = this.texture.GetLevelDescription(0);

			/////////////////
			// Set up vertex buffers
			this.vertexBuffer = new VertexBuffer(this.device
				, 6 * (Marshal.SizeOf(typeof(TexturedVertex)))
				, Usage.WriteOnly, VertexFormat.None, Pool.Default);

			this.vertexDeclaration = new VertexDeclaration(this.device, new[] {
				new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0), 
				new VertexElement(0, 12, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0), 
				VertexElement.VertexDeclarationEnd});

			initialized = true;
		}

		public void Destroy()
		{
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

		public Bitmap GetCurrentBitmap()
		{
			if (this.bitmapBunch == null)
				return null;
			return this.bitmapBunch.GetNextRenderBitmap().Bitmap;
		}

		protected void Render()
		{
			///////////////////////////
			// Update texture.
			BitmapDefinition bitmapDefinition = this.bitmapBunch.GetNextRenderBitmap();
			Bitmap bitmapToRender = bitmapDefinition.Bitmap;

			if (bitmapToRender != null)
			{
				Surface textureSurface = this.texture.GetSurfaceLevel(0);
				DataRectangle dataRect = textureSurface.LockRectangle(LockFlags.None);
				BitmapData bitmapData = bitmapToRender.LockBits(new Rectangle(0, 0, bitmapToRender.Width, bitmapToRender.Height), ImageLockMode.ReadOnly, bitmapToRender.PixelFormat);
				{
					DataStream stream = dataRect.Data;
					int stride = bitmapData.Stride;
					int bitDepth = bitmapData.Stride / bitmapData.Width;
					IntPtr sourcePtr = bitmapData.Scan0;
					for (int y = 0; y < bitmapData.Height; y++)
					{
						stream.WriteRange(sourcePtr, stride);
						stream.Position += (textureWidth - HIGH_RES_WIDTH) * bitDepth;
						sourcePtr += stride;
					}
				}
				bitmapToRender.UnlockBits(bitmapData);
				textureSurface.UnlockRectangle();
			}

			///////////////////////////
			// Set up scaling algorithm.
			TextureFilter filter = this.ImageSmoothing ? TextureFilter.Linear : TextureFilter.Point;
			this.device.SetSamplerState(0, SamplerState.MinFilter, filter);
			this.device.SetSamplerState(0, SamplerState.MagFilter, filter);

			// Update vertex buffer.
			var vertexStream = this.vertexBuffer.Lock(0, 0, LockFlags.None);

			float bitmapWidth;
			float bitmapHeight;
			float bottom;
			float right;

			if (this.RenderHighResolution)
			{
				bitmapWidth = 640;
				bitmapHeight = 480;
				bottom = maximumY;
				right = maximumX;
			}
			else
			{
				bitmapWidth = 320;
				bitmapHeight = 240;
				bottom = maximumY / 2;
				right = maximumX / 2;
			}

			float top = (bitmapDefinition.Crop.Top / bitmapHeight) * bottom;
			float left = (bitmapDefinition.Crop.Left / bitmapWidth) * right;
			right = right - (bitmapDefinition.Crop.Right / bitmapWidth) * right;
			bottom = bottom - (bitmapDefinition.Crop.Bottom / bitmapHeight) * bottom;

			vertexStream.WriteRange(new[]{
				new TexturedVertex(new Vector3(-1.0f, 1.0f, 0.0f), new Vector2(left, top))
				,new TexturedVertex(new Vector3( 1.0f,-1.0f, 0.0f), new Vector2(right, bottom))
				,new TexturedVertex(new Vector3(-1.0f,-1.0f, 0.0f), new Vector2(left, bottom))

				,new TexturedVertex(new Vector3( 1.0f,-1.0f, 0.0f), new Vector2(right - .0001f, bottom + .0001f))
				,new TexturedVertex(new Vector3(-1.0f, 1.0f, 0.0f), new Vector2(left - .0001f, top + .0001f))
				,new TexturedVertex(new Vector3( 1.0f, 1.0f, 0.0f), new Vector2(right - .0001f, top + .0001f))
				});

			vertexBuffer.Unlock();

			//////////////////////
			// Draw scene.
			this.device.Clear(ClearFlags.Target, colorBlack, 1f, 0);
			this.device.BeginScene();

			this.device.SetTexture(0, this.texture);
			this.device.SetStreamSource(0, vertexBuffer, 0, Marshal.SizeOf(typeof(TexturedVertex)));
			this.device.VertexDeclaration = this.vertexDeclaration;
			this.device.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);

			this.device.EndScene();
			this.device.Present();
		}

		public unsafe void PushFrame(IntPtr currentFrame)
		{
			/////////////// 
			// Choose the best bitmap to do a background render to
			BitmapDefinition bitmapToPrepare = this.bitmapBunch.GetNextPrepareBitmap();
			bool highResolution = this.RenderHighResolution;

			// Determine how much of the image to copy.
			int copyHeight = 0;
			int copyWidth = 0;

			if (highResolution)
			{
				copyHeight = HIGH_RES_HEIGHT;
				copyWidth = HIGH_RES_WIDTH;
			}
			else
			{
				copyHeight = LOW_RES_HEIGHT;
				copyWidth = LOW_RES_WIDTH;
			}

			// Copy!
			CanvasHelper.CopyBitmap(currentFrame, bitmapToPrepare, copyWidth, copyHeight, !highResolution, true, false);

			// And.... we're done.
			this.bitmapBunch.SetLastPreparedBitmap(bitmapToPrepare);

			// Refresh.
			this.TriggerRefresh();
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

		private void SlimDXCanvas_Paint(object sender, PaintEventArgs e)
		{
			if (!this.IsInResizeMode)
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

		private IAsyncResult previousRefreshResult = null;
		private void TriggerRefresh()
		{
			// Halt! Nobody triggers a refresh if one's already in the works.
			if (previousRefreshResult != null)
			{
				if (!previousRefreshResult.IsCompleted)
					return;

				this.previousRefreshResult = null;
			}

			this.previousRefreshResult = this.BeginInvoke(new Action(this.Render));
		}

		private void SlimDXCanvas_MouseDown(object sender, MouseEventArgs e)
		{
			double heightScale = this.ClientSize.Height / 240.0;
			double widthScale = this.ClientSize.Width / 320.0;

			int newX = (int)(e.X / heightScale);
			int newY = (int)(e.Y / heightScale);

			//System.Diagnostics.Trace.WriteLine(String.Format("Clicked in\tx:\t{0}\ty:\t{1}", newX, newY));
		}
	}
}
