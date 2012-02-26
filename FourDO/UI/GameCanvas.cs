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
using FourDO.Utilities.Globals;
using FourDO.UI.Canvases;

namespace FourDO.UI
{
	internal partial class GameCanvas : UserControl
	{
		private const InterpolationMode SMOOTH_SCALING_MODE = InterpolationMode.Low;
		private const int STANDARD_WIDTH = 320;
		private const int STANDARD_HEIGHT = 240;

		private Pen screenBorderPen = new Pen(Color.FromArgb(50, 50, 50));

		private Control childCanvas;
	
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

		private void GameCanvas_Resize(object sender, EventArgs e)
		{
			this.childCanvas.Bounds = this.getCanvasRect();
			pnlBlack.Bounds = this.childCanvas.Bounds;
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
			double imageAspect = STANDARD_WIDTH / (double)STANDARD_HEIGHT;
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
					blitRect.Width = (int)(STANDARD_WIDTH * (this.Height / (double)STANDARD_HEIGHT));
					blitRect.Height = this.Height;
					blitRect.X = (this.Width - blitRect.Width) / 2;
					blitRect.Y = 0;
				}
				else
				{
					blitRect.Width = this.Width;
					blitRect.Height = (int)(STANDARD_HEIGHT * (this.Width / (double)STANDARD_WIDTH));
					blitRect.X = 0;
					blitRect.Y = (this.Height - blitRect.Height) / 2;
				}
			}

			return blitRect;
		}

		private void GameCanvas_MouseEnter(object sender, EventArgs e)
		{
			HideMouseTimer.Enabled = false;
			HideMouseTimer.Enabled = true;
		}

		private void GameCanvas_MouseLeave(object sender, EventArgs e)
		{
			HideMouseTimer.Enabled = false;
			MouseHider.Show();
		}

		private void GameCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			MouseHider.Show();
			HideMouseTimer.Enabled = false;
			HideMouseTimer.Enabled = true;
		}

		private void HideMouseTimer_Tick(object sender, EventArgs e)
		{
			HideMouseTimer.Enabled = false;
			MouseHider.Hide();
		}

		private void childCanvas_MouseEnter(object sender, EventArgs e)
		{
			HideMouseTimer.Enabled = false;
			HideMouseTimer.Enabled = true;
		}

		private void childCanvas_MouseLeave(object sender, EventArgs e)
		{
			HideMouseTimer.Enabled = false;
			MouseHider.Show();
		}

		private void childCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			MouseHider.Show();
			HideMouseTimer.Enabled = false;
			HideMouseTimer.Enabled = true;
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
			else if (DesignerHelper.IsInDesignMode())
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
	}
}