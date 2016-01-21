using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security;
using SlimDX;
using SlimDX.Direct3D9;
using Device = SlimDX.Direct3D9.Device;
using FourDO.Emulation.FreeDO;

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

		public event BeforeRenderEventHandler BeforeRender;

		public bool AutoCrop { get; set; }

		public bool isScale { get; set; }

		public bool ImageSmoothing { get; set; }

		public bool RenderHighResolution
		{
			get
			{
				return this.bitmapBunch.HighResolution;
			}
			set
			{
				CreateNewBitmapBunch(value, this.bitmapBunch.ScalingAlgorithm);
			}
		}

		public ScalingAlgorithm ScalingAlgorithm
		{
			get
			{
				return this.bitmapBunch.ScalingAlgorithm;
			}
			set
			{
				CreateNewBitmapBunch(this.bitmapBunch.HighResolution, value);
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

		protected const int MAX_RES_WIDTH = 320*4;
		protected const int MAX_RES_HEIGHT = 240*4;

		private const int LOW_RES_WIDTH = 320;
		private const int LOW_RES_HEIGHT = 240;

		protected readonly int textureWidth = RoundUpToNextPowerOfTwo(MAX_RES_WIDTH);
		protected readonly int textureHeight = RoundUpToNextPowerOfTwo(MAX_RES_HEIGHT);
		
		protected readonly byte[] blackRow;
		protected readonly GCHandle blackRowHandle;
		protected readonly IntPtr blackRowPtr;

		protected BitmapBunch bitmapBunch;

		protected Color4 colorBlack = new Color4(0, 0, 0);

		protected VertexDeclaration vertexDeclaration;
		protected VertexBuffer vertexBuffer;
		protected Texture texture;
		protected Direct3D direct3D;
		protected Device device;

		protected bool initialized = false;

		protected CropHelper cropHelper = new CropHelper();

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
			this.blackRow = new byte[textureWidth*4];
			this.blackRowHandle = GCHandle.Alloc(this.blackRow, GCHandleType.Pinned);
			this.blackRowPtr = this.blackRowHandle.AddrOfPinnedObject();

			InitializeComponent();
		}

		public void Initialize()
		{
			// Get maximum screen size.
			Size maxSize = new Size(LOW_RES_WIDTH, LOW_RES_HEIGHT);
			foreach (var screen in Screen.AllScreens)
			{
				maxSize.Width = Math.Max(maxSize.Width, screen.Bounds.Width);
				maxSize.Height = Math.Max(maxSize.Height, screen.Bounds.Height);
			}

			this.CreateNewBitmapBunch(false, ScalingAlgorithm.None);

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
			BitmapBunch currentBunch = this.bitmapBunch;

			if (currentBunch == null)
				return null;
			return currentBunch.GetNextRenderBitmap().Bitmap;
		}

		protected void Render()
		{
			///////////////////////////
			// Update texture.
			BitmapBunch currentBunch = this.bitmapBunch;
			BitmapDefinition bitmapDefinition = currentBunch.GetNextRenderBitmap();
			if (bitmapDefinition == null)
				bitmapDefinition = currentBunch.GetBlackBitmap();

			Bitmap bitmapToRender = bitmapDefinition == null ? null : bitmapDefinition.Bitmap;

			Surface textureSurface = this.texture.GetSurfaceLevel(0);
			DataRectangle dataRect = textureSurface.LockRectangle(LockFlags.None);
			BitmapData bitmapData = bitmapToRender.LockBits(new Rectangle(0, 0, bitmapToRender.Width, bitmapToRender.Height), ImageLockMode.ReadOnly, bitmapToRender.PixelFormat);
			{
				DataStream stream = dataRect.Data;
				int stride = bitmapData.Stride;
				int bitDepth = bitmapData.Stride / bitmapData.Width;
				int sourceWidth = bitmapData.Width;
				IntPtr sourcePtr = bitmapData.Scan0;
				for (int y = 0; y < bitmapData.Height; y++)
				{
					stream.WriteRange(sourcePtr, stride);
					stream.WriteRange(this.blackRowPtr, 4); // This is okay, texture width always exceeds bitmap width by a lot
					stream.Position += ((textureWidth - sourceWidth) * bitDepth) - 4;

					sourcePtr += stride;
				}
				stream.WriteRange(this.blackRowPtr, (sourceWidth+1)*4);

			}
			bitmapToRender.UnlockBits(bitmapData);
			textureSurface.UnlockRectangle();

			///////////////////////////
			// Set up scaling algorithm.
			TextureFilter filter = this.ImageSmoothing ? TextureFilter.Linear : TextureFilter.Point;
			this.device.SetSamplerState(0, SamplerState.MinFilter, filter);
			this.device.SetSamplerState(0, SamplerState.MagFilter, filter);

			///////////////////////////
			// Update drawing size dependent on cropping and resolution.
			int bitmapWidth = bitmapToRender.Width;
			int bitmapHeight = bitmapToRender.Height;
			float bottom = bitmapHeight / (float)textureHeight;
			float right = bitmapWidth / (float)textureWidth;

			var crop = bitmapDefinition == null ? new BitmapCrop() : bitmapDefinition.Crop;
			Size renderedSize = new Size();
			renderedSize.Width = bitmapWidth - crop.Left - crop.Right;
			renderedSize.Height = bitmapHeight - crop.Top - crop.Bottom;

			if (this.BeforeRender != null)
				this.BeforeRender(renderedSize);

			float top = (crop.Top / (float)bitmapHeight) * bottom;
			float left = (crop.Left / (float)bitmapWidth) * right;
			right = right - (crop.Right / (float)bitmapWidth) * right;
			bottom = bottom - (crop.Bottom / (float)bitmapHeight) * bottom;

			var vertexStream = this.vertexBuffer.Lock(0, 0, LockFlags.None);
			vertexStream.WriteRange(new[]{
				new TexturedVertex(new Vector3(-1.0f, 1.0f, 0.0f), new Vector2(left + .0001f, top + .0001f))
				,new TexturedVertex(new Vector3( 1.0f,-1.0f, 0.0f), new Vector2(right + .0001f, bottom + .0001f))
				,new TexturedVertex(new Vector3(-1.0f,-1.0f, 0.0f), new Vector2(left + .0001f, bottom + .0001f))

				,new TexturedVertex(new Vector3( 1.0f,-1.0f, 0.0f), new Vector2(right + .0001f, bottom + .0001f))
				,new TexturedVertex(new Vector3(-1.0f, 1.0f, 0.0f), new Vector2(left + .0001f, top + .0001f))
				,new TexturedVertex(new Vector3( 1.0f, 1.0f, 0.0f), new Vector2(right + .0001f, top + .0001f))
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
			BitmapBunch currentBitmapBunch = this.bitmapBunch;
			BitmapDefinition bitmapToPrepare = currentBitmapBunch.GetNextPrepareBitmap();
			bool highResolution = currentBitmapBunch.HighResolution;

			// Determine how much of the image to copy.
			int copyHeight = highResolution ? LOW_RES_HEIGHT * 2 : LOW_RES_HEIGHT;
			int copyWidth = highResolution ? LOW_RES_WIDTH * 2 : LOW_RES_WIDTH;

			// Copy!
			CanvasHelper.CopyBitmap(currentFrame, bitmapToPrepare, copyWidth, copyHeight, true, true, this.AutoCrop, currentBitmapBunch.ScalingAlgorithm);

			// Consider the newly determined crop rectangle.
			cropHelper.ConsiderAlternateCrop(bitmapToPrepare.Crop);
			bitmapToPrepare.Crop.Mimic(cropHelper.CurrentCrop);

			// And.... we're done.
			currentBitmapBunch.SetLastPreparedBitmap(bitmapToPrepare);

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

        private void CreateNewBitmapBunch(bool highResolution, ScalingAlgorithm algorithm)
        {
            var newBunch = CanvasHelper.CreateNewBitmapBunch(
                    highResolution, algorithm, LOW_RES_WIDTH, LOW_RES_HEIGHT,
                    this.bitmapBunch, PixelFormat.Format32bppRgb);
            if (newBunch != null)
                this.bitmapBunch = newBunch;
        }
	}
}
