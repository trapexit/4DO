using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

using FourDO.Emulation;
using FourDO.Emulation.FreeDO;
using FourDO.Utilities;
using FourDO.Utilities.Globals;
using FourDO.UI.Canvases;

namespace FourDO.UI
{
	internal partial class GameCanvas : UserControl
	{
		private const int STANDARD_WIDTH = 320;
		private const int STANDARD_HEIGHT = 240;
		private Size renderedSize = new Size(STANDARD_WIDTH, STANDARD_HEIGHT);

		private readonly Pen screenBorderPen = new Pen(Color.FromArgb(50, 50, 50));

		private readonly Control childCanvas;

		private bool preserveAspectRatio = true;

		private bool isConsoleStopped;

		private bool patternSetOnce = false;
		private VoidAreaPattern voidAreaPattern;

		private bool voidAreaBorder;

		public VoidAreaPattern VoidAreaPattern
		{
			get
			{
				return voidAreaPattern;
			}
			set
			{
				if (this.voidAreaPattern == value && this.patternSetOnce)
					return;
				
				this.voidAreaPattern = value;
				Bitmap newBackground = null;

				if (this.voidAreaPattern == VoidAreaPattern.FourDO)
					newBackground = Properties.Images.VoidImage4DO;
				else if (this.voidAreaPattern == VoidAreaPattern.Bumps)
					newBackground = Properties.Images.VoidImageBumps;
				else if (this.voidAreaPattern == VoidAreaPattern.Metal)
					newBackground = Properties.Images.VoidImageMetal;

				this.BackgroundImage = newBackground;
				this.patternSetOnce = true;
			}
		}

		public bool VoidAreaBorder
		{
			get
			{
				return voidAreaBorder;
			}
			set
			{
				if (voidAreaBorder == value)
					return;
				
				voidAreaBorder = value;
				this.Invalidate();
			}
		}

		public bool AutoCrop
		{
			get
			{
				return ((ICanvas)this.childCanvas).AutoCrop;
			}
			set
			{
				((ICanvas)this.childCanvas).AutoCrop = value;
			}
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
				this.OnResize(new EventArgs());
			}
		}

		public bool ImageSmoothing
		{
			get
			{
				return ((ICanvas)this.childCanvas).ImageSmoothing;
			}
			set
			{
				((ICanvas)this.childCanvas).ImageSmoothing = value;
			}
		}

		public bool IsInResizeMode
		{
			get
			{
				return ((ICanvas)this.childCanvas).IsInResizeMode;
			}
			set
			{
				((ICanvas)this.childCanvas).IsInResizeMode = value;
			}
		}

		public bool IsConsoleStopped
		{
			get
			{
				return (this.isConsoleStopped);
			}
			set
			{
				this.isConsoleStopped = value;
				this.pnlBlack.Visible = value;
			}
		}

		public bool RenderHighResolution
		{
			get
			{
				return ((ICanvas)this.childCanvas).RenderHighResolution;
			}
			set
			{
				((ICanvas)this.childCanvas).RenderHighResolution = value;
			}
		}

		public ScalingAlgorithm ScalingAlgorithm
		{
			get
			{
				return ((ICanvas)this.childCanvas).ScalingAlgorithm;
			}
			set
			{
				((ICanvas)this.childCanvas).ScalingAlgorithm = value;
			}
		}

		public GameCanvas()
		{
			// Create child canvas.
			this.childCanvas = this.CreateChildCanvas();
			this.Controls.Add(this.childCanvas);

			InitializeComponent();

			this.childCanvas.MouseEnter += childCanvas_MouseEnter;
			this.childCanvas.MouseLeave += childCanvas_MouseLeave;
			this.childCanvas.MouseMove += childCanvas_MouseMove;
			this.isConsoleStopped = true;

			((ICanvas)this.childCanvas).BeforeRender += new BeforeRenderEventHandler(ChildCanvas_BeforeRender);

			// Hook up to the console events.
			GameConsole.Instance.FrameDone += new EventHandler(GameConsole_FrameDone);
			GameConsole.Instance.ConsoleStateChange += new ConsoleStateChangeHandler(GameConsole_ConsoleStateChange);
		}

		/// <summary>
		/// Explicitly destroy the canvas to allow for cleanup.
		/// </summary>
		public void Destroy()
		{
			((ICanvas)this.childCanvas).Destroy();
		}

		public void DoScreenshot(string fileName)
		{
			if (fileName == null)
				return;

			var canvas = (ICanvas)this.childCanvas;
			if (canvas == null)
				return;

			var sourceBitmap = canvas.GetCurrentBitmap();
			if (sourceBitmap == null)
				return;

			///////////////
			// Save the bitmap to file.
			fileName = fileName + ".png";
			try
			{
				string directoryName = Path.GetDirectoryName(fileName);
				if (!Directory.Exists(directoryName))
					Directory.CreateDirectory(directoryName);

				sourceBitmap.Save(fileName, ImageFormat.Png);
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Failed to save screenshot. Error details were: " + ex.ToString());
			}
		}

		private void GameCanvas_Resize(object sender, EventArgs e)
		{
			this.ResizeUI();
		}

		private void GameCanvas_Paint(object sender, PaintEventArgs e)
		{
			if (this.voidAreaBorder)
			{
				Graphics g = e.Graphics;
				Rectangle gameRect = this.childCanvas.Bounds;
				g.DrawRectangle(this.screenBorderPen, gameRect.X - 1, gameRect.Y - 1, gameRect.Width + 1, gameRect.Height + 1);
			}
		}

		private void GameConsole_ConsoleStateChange(ConsoleStateChangeEventArgs e)
		{
			this.IsConsoleStopped = (e.NewState == ConsoleState.Stopped);
		}

		private void GameConsole_FrameDone(object sender, EventArgs e)
		{
			((ICanvas)this.childCanvas).PushFrame(GameConsole.Instance.CurrentFrame);
		}

		private Rectangle getCanvasRect()
		{
			double imageAspect = renderedSize.Width / (double)renderedSize.Height;
			double screenAspect = this.Width / (double)this.Height;

			var blitRect = new Rectangle();

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
					blitRect.Width = (int)(renderedSize.Width * (this.Height / (double)renderedSize.Height));
					blitRect.Height = this.Height;
					blitRect.X = (this.Width - blitRect.Width) / 2;
					blitRect.Y = 0;
				}
				else
				{
					blitRect.Width = this.Width;
					blitRect.Height = (int)(renderedSize.Height * (this.Width / (double)renderedSize.Width));
					blitRect.X = 0;
					blitRect.Y = (this.Height - blitRect.Height) / 2;
				}
			}

			return blitRect;
		}

		private void GameCanvas_MouseEnter(object sender, EventArgs e)
		{
			this.HandleMouseMoved();
		}

		private void GameCanvas_MouseLeave(object sender, EventArgs e)
		{
			this.HandleMouseMoved();
		}

		private void GameCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			this.HandleMouseMoved();
		}

		private void HideMouseTimer_Tick(object sender, EventArgs e)
		{
			HideMouseTimer.Enabled = false;
			if (IsMouseOverUs(lastPosition))
				MouseHider.Hide();
			else
				MouseHider.Show();
		}

		private void childCanvas_MouseEnter(object sender, EventArgs e)
		{
			this.HandleMouseMoved();
		}

		private void childCanvas_MouseLeave(object sender, EventArgs e)
		{
			this.HandleMouseMoved();
		}

		private void childCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			this.HandleMouseMoved();
		}

		private Point lastPosition = new Point(-1, -1);
		private void HandleMouseMoved()
		{
			if (Cursor.Position.Equals(lastPosition))
				return;

			lastPosition = Cursor.Position;

			var screenRect = new Rectangle(0, 0, this.Width, this.Height);
			screenRect = this.RectangleToScreen(screenRect);

			if (IsMouseOverUs(lastPosition))
			{
				// The mouse moved, but not over us!
				MouseHider.Show();
			}

			if (screenRect.Contains(lastPosition))
			{
				MouseHider.Show();
				HideMouseTimer.Enabled = false;
				HideMouseTimer.Enabled = true;
			}
			else
			{
				HideMouseTimer.Enabled = false;
				MouseHider.Show();
			}
		}

		private bool IsMouseOverUs()
		{
			return IsMouseOverUs(Cursor.Position);
		}

		private bool IsMouseOverUs(Point mouseScreenPosition)
		{
			if (!this.IsHandleCreated
					|| this.childCanvas == null
					|| !this.childCanvas.IsHandleCreated)
				return false;

			var overHandle = MouseHider.GetHWndAtPoint(mouseScreenPosition);

			if (this.Handle == overHandle
					|| this.childCanvas.Handle == overHandle)
				return true;

			return false;
		}

		private Control CreateChildCanvas()
		{
			ICanvas createdControl = null;

			//////////////////////
			// Try creating a SlimDX canvas.
			bool success = false;
			if (RunOptions.ForceGdiRendering)
			{
				Trace.WriteLine("Video Render - Forcing use of GDI rendering due to run option.");
			}
			else if (DesignerHelper.IsInDesignMode(this))
			{
				// NOTE: (jmk) probably shouldn't happen at runtime, but I have it in here just in case something goes berserk.
				Trace.WriteLine("Video Render - Forcing use of GDI rendering due to being in design mode.");
			}
			else
			{
				try
				{
					createdControl = new SlimDXCanvas();
					createdControl.Initialize();
					success = true;
				}
				catch (Exception ex)
				{
					Trace.WriteLine("Video Render - DirectX canvas initialization failed! Will attempt to fall back to windows(GDI) rendering. Error was: " + ex.ToString());
					if (createdControl != null)
						createdControl.Destroy();
				} // We don't care what went wrong.
			}

			/////////////////////
			// If necessary, try a GDI/windows-native canvas.
			if (!success)
			{
				createdControl = new GdiCanvas();
				createdControl.Initialize();
			}
			
			return (Control)createdControl;
		}

		private void ChildCanvas_BeforeRender(Size newSize)
		{
			if (renderedSize.Equals(newSize))
				return;
			
			renderedSize = newSize;
			this.ResizeUI();
			this.Invalidate();
		}

		private void ResizeUI()
		{
			this.childCanvas.Bounds = this.getCanvasRect();
			pnlBlack.Bounds = this.childCanvas.Bounds;
		}
	}
}