using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FourDO.Resources;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	public partial class ControllerPreview : UserControl
	{
		public enum ViewModeEnum
		{
			Console,
			Controller,
		}

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

		private List<InputButton> glowingButtons;
		private InputButton? labeledGlowButton;

		private int currentAlpha = PULSATE_ALPHA_MIN;
		private bool increaseAlpha = true;

		private ViewModeEnum _viewMode;
		private Image _controllerImage;
		private Image _consoleImage;

		public delegate void MouseHoverButtonHandler(MouseHoverButtonEventArgs e);
		public event MouseHoverButtonHandler MouseHoverButton;

		public ViewModeEnum ViewMode 
		{ 
			get { return _viewMode; }
			set
			{
				if (_viewMode == value)
					return;

				_viewMode = value;

				if (_viewMode == ViewModeEnum.Console)
				{
					ButtonPositionPanel.Visible = false;
					this.BackgroundImage = this._consoleImage;
					this.OnMouseHoverButton(null);
				}
				else
				{
					ButtonPositionPanel.Visible = true;
					this.BackgroundImage = this._controllerImage;
				}
			}
		}

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
				List<InputButton> newGlowingButtons = null;
				if (buttons != null)
					newGlowingButtons = new List<InputButton>(buttons);

				////////////////
				// Figure out which button we'll give the textual preview.
				// We're going to prefer to give the most recently-added button.
				InputButton? newButton = null;
				if (newGlowingButtons != null)
				{
					foreach (var button in newGlowingButtons)
					{
						if (!newGlowingButtons.Contains(button))
						{
							newButton = button;
							break;
						}
					}
					if (!newButton.HasValue)
					{
						if (newGlowingButtons.Count > 0)
							newButton = newGlowingButtons[0];
					}
				}

				// Nowwe know which button we'll give the textual preview for.
				this.labeledGlowButton = newButton;

				this.glowingButtons = newGlowingButtons;
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
			this._viewMode = ViewModeEnum.Controller;
			this._controllerImage = this.BackgroundImage;
			this._consoleImage = this.LogoImagePanel.BackgroundImage;

			this.BackgroundImage = this._controllerImage;

			this.GlowLabel.Text = "";
		}

		private void ControllerPreview_Load(object sender, EventArgs e)
		{
			this.Localize();

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
			// Update glowing button text.
			Control glowingButton = this.GetPanelForButton(this.labeledGlowButton);
			if (glowingButton == null)
				this.GlowLabel.Text = GetButtonNameForInputButton(this.labeledGlowButton);
			else
				this.GlowLabel.Text = (string)glowingButton.Tag;

			// Update highlighted button text.
			Control highlightedControl = this.GetHighlightedControl();
			if (highlightedControl == null)
				this.ButtonLabel.Text = GetButtonNameForInputButton(this.HighlightedButton);
			else
				this.ButtonLabel.Text = (string)highlightedControl.Tag;
		}

		private string GetButtonNameForInputButton(InputButton? button)
		{
			if (!button.HasValue)
				return null;

			if (button.Value == InputButton.ConsoleAdvanceBySingleFrame)
				return JohnnyInputStrings.ButtonConsoleAdvanceFrame;

			if (button.Value == InputButton.ConsoleFullScreen)
				return JohnnyInputStrings.ButtonConsoleFullScreen;

			if (button.Value == InputButton.ConsolePause)
				return JohnnyInputStrings.ButtonConsolePause;

			if (button.Value == InputButton.ConsoleReset)
				return JohnnyInputStrings.ButtonConsoleReset;

			if (button.Value == InputButton.ConsoleScreenShot)
				return JohnnyInputStrings.ButtonConsoleScreenshot;

			if (button.Value == InputButton.ConsoleStateLoad)
				return JohnnyInputStrings.ButtonConsoleLoadState;

			if (button.Value == InputButton.ConsoleStateSave)
				return JohnnyInputStrings.ButtonConsoleSaveState;

			if (button.Value == InputButton.ConsoleStateSlotNext)
				return JohnnyInputStrings.ButtonConsoleStateSlotNext;

			if (button.Value == InputButton.ConsoleStateSlotPrevious)
				return JohnnyInputStrings.ButtonConsoleStateSlotPrevious;

			return null;
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
			if (control == null)
				return;

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
			if (this._viewMode == ViewModeEnum.Console)
				return;

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
