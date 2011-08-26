using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FourDO.UI
{
	public partial class SizeBox : UserControl
	{
		private int width = 0;
		private int height = 0;

		public int BaseHeight {get; set;}

		public int BaseWidth {get; set;}

		public SizeBox()
		{
			this.InitializeComponent();
		}

		public new Size PreferredSize
		{
			get    
			{
				return new Size(this.HeightPixLabel.Right + 4, this.HorizontalBox.Height + 8);
			}
		}
		
		public void UpdateSizeText(int width, int height)
		{
			this.width = width;
			this.height = height;
			this.UpdateLabel();
		}

		public int HideDelay
		{
			get
			{
				return this.HideTimer.Interval;
			}
		}

		private void UpdateLabel()
		{
			WidthPixLabel.Text = width.ToString() + " ";
			WidthPercentLabel.Text = string.Format("{0:##}%", (width / (double)BaseWidth) * 100);
			HeightPixLabel.Text = height.ToString();
			HeightPercentLabel.Text = string.Format("{0:##}%", (height / (double)BaseHeight) * 100);

			this.HorizontalBox.Left = 4;
			this.HorizontalBox.Top = this.HorizontalBox.Left - 1;
			this.WidthPercentLabel.Left = this.HorizontalBox.Right + 1;
			this.WidthPercentLabel.Top = this.HorizontalBox.Top + 2;
			this.WidthPixLabel.Left = WidthPercentLabel.Right;
			this.WidthPixLabel.Top = WidthPercentLabel.Top;

			this.VerticalBox.Left = this.WidthPixLabel.Right + 3;
			this.VerticalBox.Top = this.HorizontalBox.Top + 1;
			this.HeightPercentLabel.Left = this.VerticalBox.Right - 2;
			this.HeightPercentLabel.Top = this.VerticalBox.Top + 1;
			this.HeightPixLabel.Left = HeightPercentLabel.Right;
			this.HeightPixLabel.Top = HeightPercentLabel.Top;
			
			this.HideTimer.Enabled = false;
			this.HideTimer.Enabled = true;
		}

		private void SizeBox_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawLine(new Pen(Color.White), this.WidthPixLabel.Right + 1, 0, this.WidthPixLabel.Right + 1, this.Height);
			e.Graphics.DrawRectangle(new Pen(Color.SteelBlue), 0, 0, this.Width - 1, this.Height - 1);
			e.Graphics.DrawRectangle(new Pen(Color.White), 1, 1, this.Width - 3, this.Height - 3);
		}

		private void HideTimer_Tick(object sender, EventArgs e)
		{
			this.Visible = false;
			this.HideTimer.Enabled = false;
		}

		private void SizeBox_Resize(object sender, EventArgs e)
		{
			this.Invalidate();
		}
	}
}
