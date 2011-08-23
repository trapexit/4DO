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
		private const int PULSALE_FRAME_PERIOD = 50; // milliseconds
		private const int PULSATE_ALPHA_MIN = 50;
		private const int PULSATE_ALPHA_MAX = 200;
		private const int PULSATE_ALPHA_STEP = 20;

		private JohnnyInputButton? highlightedButton;
		private int currentAlpha = PULSATE_ALPHA_MIN;
		private bool increaseAlpha = true;

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
		}

		private void ControllerPreview_Resize(object sender, EventArgs e)
		{
			this.RepositionPositionTable();
		}

		public JohnnyInputButton? HighlightedButton
		{
			get
			{
				return this.highlightedButton;
			}
			set
			{
				this.highlightedButton = value;
				this.UpdateUI();
			}
		}

		private void UpdateUI()
		{
			
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			Rectangle bounds = this.GetDescendantBounds(this.LPanel);
			e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(this.currentAlpha, 255, 0, 0)), bounds);
			bounds.X--;
			bounds.Y--;
			bounds.Width+=1;
			bounds.Height+=1;
			e.Graphics.DrawRectangle(new Pen(Color.FromArgb(Math.Max(this.currentAlpha-50,0), 255, 0, 0)), bounds);
			bounds.X--;
			bounds.Y--;
			bounds.Width+=2;
			bounds.Height+=2;
			e.Graphics.DrawRectangle(new Pen(Color.FromArgb(Math.Max(this.currentAlpha-100,0), 255, 0, 0)), bounds);
		}

		// THIS IS WRONG!
		private Rectangle GetDescendantBounds(Control control)
		{
			Point originLocation = ButtonPositionPanel.PointToScreen(new Point(0,0));
			Point controlLocation = control.PointToScreen(new Point(0,0));

			Rectangle bounds = control.Bounds;

			bounds.X = controlLocation.X - originLocation.X;
			bounds.Y = controlLocation.Y - originLocation.Y;

			bounds.X += (this.Width - this.ButtonPositionPanel.Width) / 2;
			bounds.Y += (this.Height - this.ButtonPositionPanel.Height) / 2;

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

			this.Invalidate();
		}

		private void RepositionPositionTable() 
		{
			this.ButtonPositionPanel.Top = this.Height;
			this.ButtonPositionPanel.Left = this.Width;
		}
	}
}
