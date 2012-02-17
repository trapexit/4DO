using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	public partial class ControllerPreview : UserControl
	{
		private const int PULSALE_FRAME_PERIOD = 100; // milliseconds
		private const int PULSATE_ALPHA_MIN = 50;
		private const int PULSATE_ALPHA_MAX = 130;
		private const int PULSATE_ALPHA_STEP = 20;

		private const int HIGHLIGHT_R = 255;
		private const int HIGHLIGHT_G = 0;
		private const int HIGHLIGHT_B = 0;

		private const int GLOW_R = 255;
		private const int GLOW_G = 255;
		private const int GLOW_B = 0;

		private InputButton? lastHoverButton = null;

		private InputButton? highlightedButton;

		private IEnumerable<InputButton> glowingButtons;

		private int currentAlpha = PULSATE_ALPHA_MIN;
		private bool increaseAlpha = true;

		public delegate void MouseHoverButtonHandler(MouseHoverButtonEventArgs e);
		public event MouseHoverButtonHandler MouseHoverButton;

		public InputButton? HighlightedButton
		{
			get
			{
				return this.highlightedButton;
			}
			set
			{
				this.highlightedButton = value;
				this.UpdateUI();
				this.Invalidate();
			}
		}

		public IEnumerable<InputButton> GlowingButtons
		{
			get
			{
				return glowingButtons;
			}
		}
		public void SetGlowingButtons(IEnumerable<InputButton> buttons)
		{
			if (buttons != this.glowingButtons)
			{
				this.glowingButtons = buttons;
				this.UpdateUI();
				this.Invalidate();
			}
		}

		public InputButton? HoveredButton
		{
			get
			{
				return this.lastHoverButton;
			}
		}	

		public ControllerPreview()
		{
			InitializeComponent();
		}

		private void ControllerPreview_Load(object sender, EventArgs e)
		{
			this.tmrPulsate.Interval = PULSALE_FRAME_PERIOD;
			this.tmrPulsate.Enabled = true;

			this.BackgroundImageLayout = ImageLayout.Center;
			this.RepositionPositionTable();

			this.ButtonLabel.ForeColor = Color.FromArgb(HIGHLIGHT_R, HIGHLIGHT_G, HIGHLIGHT_B);
		}
		
		protected void OnMouseHoverButton(InputButton? button)
		{
			if (button != this.lastHoverButton)
			{
				if (this.MouseHoverButton != null)
					this.MouseHoverButton(new MouseHoverButtonEventArgs(button));
			}

			this.lastHoverButton = button;
		}

		private void ControllerPreview_Resize(object sender, EventArgs e)
		{
			this.RepositionPositionTable();
		}

		private Control GetHighlightedControl()
		{
			return this.GetPanelForButton(this.highlightedButton);
		}

		private void UpdateUI()
		{
			Control control = this.GetHighlightedControl();
			if (control == null)
				this.ButtonLabel.Text = null;
			else
				this.ButtonLabel.Text = (string)control.Tag;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (!this.HighlightedButton.HasValue && this.glowingButtons == null)
				return;

			if (this.HighlightedButton != null)
			{
				Control highlightControl = this.GetHighlightedControl();
				this.DrawPulsate(highlightControl, e, true);
			}

			if (glowingButtons != null)
			{
				foreach (var button in glowingButtons)
				{
					Control glowControl = this.GetPanelForButton(button);
					this.DrawPulsate(glowControl, e, false);
				}
			}
		}

		private void DrawPulsate(Control control, PaintEventArgs e, bool highlightColor)
		{
			Rectangle bounds = this.GetDescendantBounds(control);

			int colorR = highlightColor ? HIGHLIGHT_R : GLOW_R;
			int colorG = highlightColor ? HIGHLIGHT_G : GLOW_G;
			int colorB = highlightColor ? HIGHLIGHT_B : GLOW_B;

			this.DrawGlyph(control, e, new SolidBrush(Color.FromArgb(this.currentAlpha, colorR, colorG, colorB)), bounds);
			bounds.X--;
			bounds.Y--;
			bounds.Width += 1;
			bounds.Height += 1;
			this.DrawGlyph(control, e, new SolidBrush(Color.FromArgb(Math.Max(this.currentAlpha - 50, 0), colorR, colorG, colorB)), bounds);
			bounds.X--;
			bounds.Y--;
			bounds.Width += 2;
			bounds.Height += 2;
			this.DrawGlyph(control, e, new SolidBrush(Color.FromArgb(Math.Max(this.currentAlpha - 100, 0), colorR, colorG, colorB)), bounds);
		}

		private void DrawGlyph(Control control, PaintEventArgs e, SolidBrush brush, Rectangle bounds)
		{
			if ((control == this.UpPanel)
				|| (control == this.DownPanel)
				|| (control == this.LeftPanel)
				|| (control == this.RightPanel))
			{
				e.Graphics.FillRectangle(brush, bounds);
			}
			else if ((control == this.APanel)
				|| (control == this.BPanel)
				|| (control == this.CPanel)
				|| (control == this.XPanel)
				|| (control == this.PPanel))
			{
				e.Graphics.FillEllipse(brush, bounds);
			}
			else
			{
				const double HEIGHT_PERCENTAGE = .5;
				Point[] polyPoints = new Point[4];
				if (control == this.LPanel)
				{
					polyPoints[0].X = bounds.Left;
					polyPoints[0].Y = (int)(bounds.Bottom - (bounds.Height * HEIGHT_PERCENTAGE));
					polyPoints[1].X = bounds.Left;
					polyPoints[1].Y = bounds.Bottom;
					polyPoints[2].X = bounds.Right;
					polyPoints[2].Y = (int)(bounds.Top + (bounds.Height * HEIGHT_PERCENTAGE));
					polyPoints[3].X = bounds.Right;
					polyPoints[3].Y = bounds.Top;
					e.Graphics.FillPolygon(brush, polyPoints);
				}
				else if (control == this.RPanel)
				{
					polyPoints[0].X = bounds.Left;
					polyPoints[0].Y = bounds.Top;
					polyPoints[1].X = bounds.Left;
					polyPoints[1].Y = (int)(bounds.Top + (bounds.Height * HEIGHT_PERCENTAGE));
					polyPoints[2].X = bounds.Right;
					polyPoints[2].Y = bounds.Bottom;
					polyPoints[3].X = bounds.Right;
					polyPoints[3].Y = (int)(bounds.Bottom - (bounds.Height * HEIGHT_PERCENTAGE));
					e.Graphics.FillPolygon(brush, polyPoints);
				}
			}
		}

		private Rectangle GetDescendantBounds(Control control)
		{
			Point originLocation = ButtonPositionPanel.PointToScreen(new Point(0,0));
			Point controlLocation = control.PointToScreen(new Point(0,0));

			Rectangle bounds = control.Bounds;

			bounds.X = controlLocation.X - originLocation.X;
			bounds.Y = controlLocation.Y - originLocation.Y;

			bounds.X += (this.Width - this.ButtonPositionPanel.Width) / 2;
			bounds.Y += (this.Height - this.ButtonPositionPanel.Height) / 2;

			// Rounding errors
			if ((this.Width / 2) == 1)
			{
				bounds.X -= 1;
				bounds.Y -= 1;
			}

			return bounds;
		}

		private void tmrPulsate_Tick(object sender, EventArgs e)
		{
			if (this.increaseAlpha)
				this.currentAlpha += PULSATE_ALPHA_STEP;
			else
				this.currentAlpha -= PULSATE_ALPHA_STEP;

			if (this.currentAlpha + PULSATE_ALPHA_STEP > PULSATE_ALPHA_MAX || this.currentAlpha - PULSATE_ALPHA_STEP < PULSATE_ALPHA_MIN)
				this.increaseAlpha = !this.increaseAlpha;

			// Only invalidate if we're highlighting something.
			if (this.HighlightedButton.HasValue || (this.glowingButtons != null))
				this.Invalidate();
		}

		private void RepositionPositionTable() 
		{
			this.ButtonPositionPanel.Top = this.Height;
			this.ButtonPositionPanel.Left = this.Width;
		}

		private void ControllerPreview_MouseLeave(object sender, EventArgs e)
		{
			this.OnMouseHoverButton(null);
		}

		private void ControllerPreview_MouseMove(object sender, MouseEventArgs e)
		{
			var buttons = (InputButton[]) Enum.GetValues(typeof(InputButton));
			foreach (InputButton button in buttons)
			{
				Panel boundsPanel = this.GetPanelForButton(button);
				if (boundsPanel != null)
				{
					Rectangle bounds = this.GetDescendantBounds(boundsPanel);
					if (bounds.Contains(e.Location))
					{
						this.OnMouseHoverButton(button);
						return;
					}
				}
			}
			this.OnMouseHoverButton(null);
		}

		private Panel GetPanelForButton(InputButton? button)
		{
			if (!button.HasValue)
				return null;

			switch (button.Value)
			{
				case InputButton.A:
					return this.APanel;
				case InputButton.B:
					return this.BPanel;
				case InputButton.C:
					return this.CPanel;
				case InputButton.X:
					return this.XPanel;
				case InputButton.P:
					return this.PPanel;
				case InputButton.L:
					return this.LPanel;
				case InputButton.R:
					return this.RPanel;
				case InputButton.Left:
					return this.LeftPanel;
				case InputButton.Right:
					return this.RightPanel;
				case InputButton.Up:
					return this.UpPanel;
				case InputButton.Down:
					return this.DownPanel;
			}
			return null;
		}
	}

	public class MouseHoverButtonEventArgs : EventArgs
	{
		public InputButton? InputButton { get; set; }

		public MouseHoverButtonEventArgs(InputButton? button)
		{
			this.InputButton = button;
		}
	}
}
