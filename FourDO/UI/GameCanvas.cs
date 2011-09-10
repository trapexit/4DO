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
	internal partial class GameCanvas : UserControl
	{
		private const InterpolationMode SMOOTH_SCALING_MODE = InterpolationMode.Low;
		
		private const int bitmapWidth = 320;
		private const int bitmapHeight = 240;

		private Pen screenBorderPen = new Pen(Color.FromArgb(50, 50, 50));
	
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
					newBackground = Properties.Resources.VoidImage4DO;
				else if (this.voidAreaPattern == VoidAreaPattern.Bumps)
					newBackground = Properties.Resources.VoidImageBumps;
				else if (this.voidAreaPattern == VoidAreaPattern.Metal)
					newBackground = Properties.Resources.VoidImageMetal;

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
				return (this.slimDXCanvas.ImageSmoothing);
			}
			set
			{
				this.slimDXCanvas.ImageSmoothing = value;
			}
		}

		public bool IsInResizeMode
		{
			get
			{
				return (this.slimDXCanvas.IsInResizeMode);
			}
			set
			{
				this.slimDXCanvas.IsInResizeMode = value;
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

		public GameCanvas()
		{
			InitializeComponent();

			this.isConsoleStopped = true;
			this.pnlBlack.Dock = DockStyle.Fill;

			GameConsole.Instance.ConsoleStateChange += new ConsoleStateChangeHandler(GameConsole_ConsoleStateChange);
		}

		/// <summary>
		/// Explicitly destroy the canvas to allow for cleanup.
		/// </summary>
		public void Destroy()
		{
			this.slimDXCanvas.Destroy();
		}

		private void GameCanvas_Resize(object sender, EventArgs e)
		{
			slimDXCanvas.Bounds = this.getCanvasRect();
		}

		private void GameCanvas_Paint(object sender, PaintEventArgs e)
		{
			if (this.voidAreaBorder)
			{
				Graphics g = e.Graphics;
				Rectangle gameRect = this.slimDXCanvas.Bounds;
				g.DrawRectangle(this.screenBorderPen, gameRect.X - 1, gameRect.Y - 1, gameRect.Width + 1, gameRect.Height + 1);
			}
		}

		private void GameConsole_ConsoleStateChange(ConsoleStateChangeEventArgs e)
		{
			this.IsConsoleStopped = (e.NewState == ConsoleState.Stopped);
		}

		private Rectangle getCanvasRect()
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

		private void slimDXCanvas_MouseEnter(object sender, EventArgs e)
		{
			HideMouseTimer.Enabled = false;
			HideMouseTimer.Enabled = true;
		}

		private void slimDXCanvas_MouseLeave(object sender, EventArgs e)
		{
			HideMouseTimer.Enabled = false;
			MouseHider.Show();
		}

		private void slimDXCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			MouseHider.Show();
			HideMouseTimer.Enabled = false;
			HideMouseTimer.Enabled = true;
		}
	}
}