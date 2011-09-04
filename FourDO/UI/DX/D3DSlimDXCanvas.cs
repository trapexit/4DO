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
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;

using FourDO.Emulation;
using FourDO.Emulation.FreeDO;
using System.IO;

namespace FourDO.UI.DX
{
	public partial class D3DSlimDXCanvas : UserControl
	{
		public bool ImageSmoothing
		{
			get
			{
				return true;
			}
			set
			{
				//this.interpolationMode = value ? D2D.InterpolationMode.Linear : D2D.InterpolationMode.NearestNeighbor;
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

		//protected D2D.Bitmap dxBitmap;

		protected Bitmap bitmapA = new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format32bppRgb);
		protected Bitmap bitmapB = new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format32bppRgb);
		protected Bitmap bitmapC = new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format32bppRgb);

		protected Bitmap currentFrontendBitmap;
		protected Bitmap lastDrawnBackgroundBitmap;

		protected object bitmapSemaphore = new object();

		Device device;
		DeviceContext context;
		SwapChain swapChain;
		ShaderSignature inputSignature;
		VertexShader vertexShader;
		PixelShader pixelShader;
		DataStream vertices;
		SlimDX.Direct3D11.Buffer vertexBuffer;
		RenderTargetView renderTarget;
		InputLayout layout;
		Effect effect;

		Texture2D[] texes = new Texture2D[5];
		
		protected Color4 colorBlack = new Color4(0, 0, 0);
		
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

		public D3DSlimDXCanvas()
		{
			InitializeComponent();

			// hook the application's idle event
			Application.Idle += new EventHandler(OnApplicationIdle);

			GameConsole.Instance.FrameDone += new EventHandler(GameConsole_FrameDone);

			Size maxSize = new Size(bitmapWidth, bitmapHeight);
			foreach (var screen in Screen.AllScreens)
			{
				maxSize.Width = Math.Max(maxSize.Width, screen.Bounds.Width);
				maxSize.Height = Math.Max(maxSize.Height, screen.Bounds.Height);
			}

			ModeDescription modeDesc = new ModeDescription();
			modeDesc.Width = maxSize.Width;
			modeDesc.Height = maxSize.Height;
			modeDesc.Format = Format.R8G8B8A8_UNorm;

			var description = new SwapChainDescription()
			{
				BufferCount = 2,
				Usage = Usage.RenderTargetOutput,
				OutputHandle = this.Handle,
				IsWindowed = true,
				ModeDescription = modeDesc,
				SampleDescription = new SampleDescription(1, 0),
				Flags = SwapChainFlags.AllowModeSwitch,
				SwapEffect = SwapEffect.Discard
			};
			
			Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, description, out device, out swapChain);

			// create a view of our render target, which is the backbuffer of the swap chain we just created
			using (var resource = Resource.FromSwapChain<Texture2D>(swapChain, 0))
				renderTarget = new RenderTargetView(device, resource);

			// Grab the context.
			context = device.ImmediateContext;
			
			// Set as output merger render target.
			context.OutputMerger.SetTargets(renderTarget);

			// setting a viewport is required if you want to actually see anything
			var viewport = new Viewport(0.0f, 0.0f, maxSize.Width, maxSize.Height);
			context.Rasterizer.SetViewports(viewport);

			ShaderBytecode effectByteCode = ShaderBytecode.CompileFromFile("shader.fx", "Render", "fx_5_0", ShaderFlags.None, EffectFlags.None);
			effect = new Effect(device, effectByteCode);

			// Create sampler
			var samplerDesc = new SamplerDescription();
			samplerDesc.Filter = Filter.MinMagMipLinear;
			samplerDesc.AddressU = TextureAddressMode.Wrap;
			samplerDesc.AddressV = TextureAddressMode.Wrap;
			samplerDesc.AddressW = TextureAddressMode.Wrap;
			samplerDesc.ComparisonFunction = Comparison.Never;
			samplerDesc.MinimumLod = 0;
			samplerDesc.MaximumLod = float.MaxValue;

			SamplerState samplerState = SamplerState.FromDescription(this.device, samplerDesc);
			effect.GetVariableByName("TextureSampler").AsSampler().SetSamplerState(0, samplerState);
			this.context.PixelShader.SetSampler(samplerState, 0);
			

			// load and compile the vertex shader
			using (var bytecode = ShaderBytecode.CompileFromFile("shader.fx", "vs_main", "vs_4_0", ShaderFlags.None, EffectFlags.None))
			{
				inputSignature = ShaderSignature.GetInputSignature(bytecode);
				vertexShader = new VertexShader(device, bytecode);
			}

			// load and compile the pixel shader
			using (var bytecode = ShaderBytecode.CompileFromFile("shader.fx", "ps_main", "ps_4_0", ShaderFlags.None, EffectFlags.None))
			{
				pixelShader = new PixelShader(device, bytecode);
			}

			// create test vertex data, making sure to rewind the stream afterward
			vertices = new DataStream(20 * 4, true, true);
			vertices.Write(new Vector3(-1.0f, -1.0f, 0.5f)); // bottom left
			vertices.Write(new Vector2(0.0f, 1.0f));
			
			vertices.Write(new Vector3(-1.0f, 1.0f, 0.5f)); // top left
			vertices.Write(new Vector2(0.0f, 0.0f));

			vertices.Write(new Vector3(1.0f, -1.0f, 0.5f)); // bottom right
			vertices.Write(new Vector2(1.0f, 1.0f));

			vertices.Write(new Vector3(1.0f, 1.0f, 0.5f)); // top right
			vertices.Write(new Vector2(1.0f, 0.0f));
			vertices.Position = 0;

			vertexBuffer = new SlimDX.Direct3D11.Buffer(device, vertices, 20 * 4, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

			// create the vertex layout and buffer
			var elements = new[] 
			{
				new InputElement("POSITION", 0, Format.R32G32B32_Float, 0), 
				new InputElement("textcoord", 0, Format.R32G32_Float, 12, 0) 
			};
			layout = new InputLayout(device, inputSignature, elements);

			// configure the Input Assembler portion of the pipeline with the vertex data
			context.InputAssembler.InputLayout = layout;
			context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
			context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, 20, 0));

			// set the shaders
			context.VertexShader.Set(vertexShader);
			context.PixelShader.Set(pixelShader);

			// prevent DXGI handling of alt+enter, which doesn't work properly with Winforms
			using (var factory = swapChain.GetParent<Factory>())
				factory.SetWindowAssociation(this.Handle, WindowAssociationFlags.IgnoreAltEnter);


			texes[0] = Texture2D.FromFile(this.device, @"C:\jmk\screens\new\frame1.jpg");
			texes[1] = Texture2D.FromFile(this.device, @"C:\jmk\screens\new\frame2.jpg");
			texes[2] = Texture2D.FromFile(this.device, @"C:\jmk\screens\new\frame3.jpg");
			texes[3] = Texture2D.FromFile(this.device, @"C:\jmk\screens\new\frame4.jpg");
			texes[4] = Texture2D.FromFile(this.device, @"C:\jmk\screens\new\frame5.jpg");


			initialized = true;
		}

		public void Destroy()
		{
			vertices.Close();
			vertexBuffer.Dispose();
			layout.Dispose();
			inputSignature.Dispose();
			vertexShader.Dispose();
			pixelShader.Dispose();
			renderTarget.Dispose();
			swapChain.Dispose();
			device.Dispose();
		}

		protected void OnApplicationIdle(object sender, EventArgs e)
		{
			while(this.GetIsAppStillIdle())
			{
				this.Render();
			}
		}

		protected unsafe void Render()
		{
			this.context.ClearRenderTargetView(renderTarget, new Color4(0x00003300));

			this.currentFrontendBitmap = this.lastDrawnBackgroundBitmap; // This keeps the background from updating it too.
			Bitmap bitmapToDraw = this.currentFrontendBitmap;
			if (bitmapToDraw != null)
			{
				Texture2D imageTexture;

				//imageTexture = Texture2D.FromFile(device, @"C:\jmk\screens\ihateit.bmp");

				//Surface surface = imageTexture.AsSurface();
				//DataRectangle dataRect = surface.Map(SlimDX.DXGI.MapFlags.Write);

				MemoryStream stream = new MemoryStream(bitmapToDraw.Width * bitmapToDraw.Height * 8);
				bitmapToDraw.Save(stream, ImageFormat.Bmp);
				stream.Position = 0;

				imageTexture = Texture2D.FromStream(this.device, stream, (int)stream.Length);

				/*
				ImageLoadInformation loadInfo = new ImageLoadInformation();
				loadInfo.BindFlags = BindFlags.ShaderResource;
				loadInfo.CpuAccessFlags = CpuAccessFlags.Write;
				loadInfo.Format = Format.R8G8B8A8_UNorm;
				loadInfo.MipLevels = 1;
				loadInfo.OptionFlags = ResourceOptionFlags.None;
				loadInfo.Usage = ResourceUsage.Dynamic;
				loadInfo.Width = this.bitmapSize.Width;
				loadInfo.Height = this.bitmapSize.Height;
				imageTexture = Texture2D.FromFile(device, @"C:\jmk\screens\ihateit.bmp", loadInfo);
				*/

	
				BitmapData bitmapData = bitmapToDraw.LockBits(new Rectangle(0, 0, bitmapToDraw.Width, bitmapToDraw.Height), ImageLockMode.ReadOnly, bitmapToDraw.PixelFormat);
				{

					/*
					Texture2DDescription textureDesc = new Texture2DDescription();
					textureDesc.BindFlags = BindFlags.ShaderResource;
					//textureDesc.CpuAccessFlags = CpuAccessFlags.Write;
					//textureDesc.Format = Format.R8G8B8A8_UNorm;
					//textureDesc.MipLevels = 1;
					//textureDesc.OptionFlags = ResourceOptionFlags.None;
					//textureDesc.Usage = ResourceUsage.Dynamic;
					textureDesc.Width = this.bitmapSize.Width;
					textureDesc.Height = this.bitmapSize.Height;
					//textureDesc.SampleDescription = new SampleDescription(1, 1);

					//UnmanagedMemoryStream stream = new UnmanagedMemoryStream((byte*)bitmapData.Scan0.ToInt32(), bitmapData.Stride * bitmapData.Height);
					DataStream dataStream = new DataStream(bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, true, false);
					DataRectangle rect = new DataRectangle(bitmapData.Stride, dataStream);

					//imageTexture = new Texture2D(this.device, textureDesc, rect);
					imageTexture = new Texture2D(this.device, textureDesc);
					*/

					/*
					ImageLoadInformation loadInfo = new ImageLoadInformation();
					loadInfo.BindFlags = BindFlags.ShaderResource;
					loadInfo.CpuAccessFlags = CpuAccessFlags.Write;
					loadInfo.Format = Format.R8G8B8A8_UNorm;
					loadInfo.MipLevels = 1;
					loadInfo.OptionFlags = ResourceOptionFlags.None;
					loadInfo.Usage = ResourceUsage.Dynamic;
					loadInfo.Width = this.bitmapSize.Width;
					loadInfo.Height = this.bitmapSize.Height;
					*/

					///UnmanagedMemoryStream stream = new UnmanagedMemoryStream((byte*)bitmapData.Scan0.ToInt32(), bitmapData.Stride * bitmapData.Height);
					//imageTexture = Texture2D.FromStream(this.device, stream, (int)stream.Length, loadInfo);

					/*
					FileStream filestream = new FileStream(@"C:\jmk\screens\ihateit.bmp", FileMode.Open);
					imageTexture = Texture2D.FromStream(this.device, filestream, (int)filestream.Length);
					*/
				}
				bitmapToDraw.UnlockBits(bitmapData);

				ShaderResourceView resourceView = new ShaderResourceView(device, imageTexture);
				effect.GetVariableByName("xTexture").AsResource().SetResource(resourceView);
				this.context.PixelShader.SetShaderResource(resourceView, 0);
			}

			// Die vsync. You little shit.
			/*
			texnum++;
			if (texnum == 5)
				texnum = 0;
			ShaderResourceView resourceViewover = new ShaderResourceView(device, texes[texnum]);
			effect.GetVariableByName("xTexture").AsResource().SetResource(resourceViewover);
			this.context.PixelShader.SetShaderResource(resourceViewover, 0);
			*/

			context.Draw(4, 0);
			swapChain.Present(1, PresentFlags.None);
		}
		static int texnum = 0;

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
