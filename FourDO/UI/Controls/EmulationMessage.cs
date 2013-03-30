using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace FourDO.UI.Controls
{
	public partial class EmulationMessage : UserControl
	{
		private bool _gotDpi = false;
		private float _dpiY = 0;
		private float _heightOffset = 0;

		public EmulationMessage()
		{
			InitializeComponent();
		}

		private void EmulationMessage_Resize(object sender, EventArgs e)
		{
			this.UpdateFont();
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			base.OnPaintBackground(e);

			var text = "ÐÙ¶çpQqwÙWypt";
			text = text + Environment.NewLine + text;

			var stringFormat = new StringFormat(StringFormat.GenericTypographic);
			stringFormat.LineAlignment = StringAlignment.Near;
			stringFormat.Alignment = StringAlignment.Near;

			e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			e.Graphics.DrawString(text, this.Font, new SolidBrush(Color.Red), 0, -_heightOffset, stringFormat);
		}

		private void EmulationMessage_Load(object sender, EventArgs e)
		{
			_dpiY = this.CreateGraphics().DpiY;
			_gotDpi = true;
		}

		private void UpdateFont()
		{
			if (!_gotDpi)
				return;

			var style = this.Font.Style;
			var font = this.Font;
			var family = font.FontFamily;

			float lineHeight = family.GetLineSpacing(style);
			float emHeight = family.GetEmHeight(style);

			float pixelsPerPoint = lineHeight / family.GetEmHeight(style);

			float newSize = this.ClientSize.Height * (emHeight / lineHeight) * (72 / _dpiY);
			newSize = Math.Max(newSize, 2);

			this.Font = new Font(font.FontFamily, newSize, font.Style);
		}
	}
}
