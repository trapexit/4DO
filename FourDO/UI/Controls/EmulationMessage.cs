using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace FourDO.UI.Controls
{
	public partial class EmulationMessage : UserControl
	{
		private const int HIDE_MILLISECONDS = 1500;
		private const int OUTSIDE_BORDER_SIZE = 3;
		private const int INSIDE_BORDER_SIZE = 3;

		private readonly Color BACKGROUND_COLOR = Color.Black;
		private readonly Color BORDER_COLOR = Color.Gray;
		private readonly Color TEXT_COLOR = Color.Gray;

		private readonly Font rootFont = new Font(new FontFamily("Arial"), 16, FontStyle.Bold);
		private Font renderFont;

		private bool _gotDpi = false;
		private float _dpiY = 0;
		private float _heightOffset = 0;

		private string _message;

		public EmulationMessage()
		{
			InitializeComponent();

			this.HideTimer.Enabled = false;
			this.HideTimer.Interval = HIDE_MILLISECONDS;

			this.BackColor = BACKGROUND_COLOR;

			this.renderFont = new Font(rootFont, rootFont.Style);
		}

		public void PostMessage(string message)
		{
			_message = message;
			this.Invalidate();
			this.Visible = true;
			this.HideTimer.Enabled = false;
			this.HideTimer.Enabled = true;
		}

		void EmulationMessage_Resize(object sender, EventArgs e)
		{
			this.UpdateFont();
			this.Refresh();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			var text = _message;

			var stringFormat = new StringFormat(StringFormat.GenericTypographic);
			stringFormat.LineAlignment = StringAlignment.Near;
			stringFormat.Alignment = StringAlignment.Center;

			e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			var textBounds = new RectangleF(
				OUTSIDE_BORDER_SIZE + INSIDE_BORDER_SIZE + 1
				, (-_heightOffset) + INSIDE_BORDER_SIZE + OUTSIDE_BORDER_SIZE + 1
				, this.ClientSize.Width - INSIDE_BORDER_SIZE * 2 - OUTSIDE_BORDER_SIZE * 2 - 3
				, this.ClientSize.Height - INSIDE_BORDER_SIZE * 2 - OUTSIDE_BORDER_SIZE * 2 - 3);

			e.Graphics.DrawString(text, this.renderFont, new SolidBrush(TEXT_COLOR), textBounds, stringFormat);
			//e.Graphics.DrawRectangle(new Pen(Color.Green), textBounds.X, textBounds.Y, textBounds.Width, textBounds.Height);

			e.Graphics.DrawRectangle(
				new Pen(BORDER_COLOR, INSIDE_BORDER_SIZE)
				, OUTSIDE_BORDER_SIZE + (INSIDE_BORDER_SIZE / 2)
				, OUTSIDE_BORDER_SIZE + (INSIDE_BORDER_SIZE / 2)
				, this.ClientSize.Width - OUTSIDE_BORDER_SIZE * 2 - (INSIDE_BORDER_SIZE / 2) * 2 - 1
				, this.ClientSize.Height - OUTSIDE_BORDER_SIZE * 2 - (INSIDE_BORDER_SIZE / 2) * 2 - 1);
		}

		private void EmulationMessage_Load(object sender, EventArgs e)
		{
			_dpiY = this.CreateGraphics().DpiY;
			_gotDpi = true;
			this.UpdateFont();
		}

		private void UpdateFont()
		{
			if (!_gotDpi)
				return;

			var style = this.renderFont.Style;
			var font = this.renderFont;
			var family = font.FontFamily;

			var usedHeight = this.ClientSize.Height - INSIDE_BORDER_SIZE * 2 - OUTSIDE_BORDER_SIZE * 2 - 3;

			float lineHeight = family.GetLineSpacing(style);
			float emHeight = family.GetEmHeight(style);

			float newSize = usedHeight * (emHeight / lineHeight) * (72 / _dpiY);
			newSize = Math.Max(newSize, 2);

			this.renderFont = new Font(rootFont.FontFamily, newSize, font.Style);
		}

		private void HideTimer_Tick(object sender, EventArgs e)
		{
			this.HideTimer.Enabled = false;
			this.Visible = false;
		}
	}
}
