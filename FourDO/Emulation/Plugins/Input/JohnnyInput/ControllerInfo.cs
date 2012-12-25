using System;
using System.Windows.Forms;
using FourDO.Resources;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	public partial class ControllerInfo : UserControl
	{
		private int deviceCount = int.MinValue;

		public event EventHandler LinkMouseEnter;
		public event EventHandler LinkMouseLeave;

		public int DeviceCount
		{
			get
			{
				return this.deviceCount;
			}

			set 
			{
				if (this.deviceCount != value)
				{
					if (value <= 0)
						this.DevicesLabel.Text = JohnnyInputStrings.InfoNoDevices;
					else if (value == 1)
						this.DevicesLabel.Text = "1 " + JohnnyInputStrings.InfoDevice;
					else
						this.DevicesLabel.Text = value.ToString() + " " + JohnnyInputStrings.InfoDevices;
				}

				this.deviceCount = value;
			}
		}

		public ControllerInfo()
		{
			InitializeComponent();

			this.DeviceCount = 1;
		}

		private void MouseViewLabel_MouseEnter(object sender, EventArgs e)
		{
			if (this.LinkMouseEnter != null)
				this.LinkMouseEnter(this, new EventArgs());
		}

		private void MouseViewLabel_MouseLeave(object sender, EventArgs e)
		{
			if (this.LinkMouseLeave != null)
				this.LinkMouseLeave(this, new EventArgs());
		}

		private void ControllerInfo_Load(object sender, EventArgs e)
		{
			this.Localize();
		}

		private void ControllerInfo_Resize(object sender, System.EventArgs e)
		{
			tableLayoutPanel1.Left = 0;
			tableLayoutPanel2.Left = 0;
			EscapeLabel.Left = 0;
			MouseViewLabel.Left = 0;

			tableLayoutPanel1.Width = this.Width;
			tableLayoutPanel2.Width = this.Width;
			EscapeLabel.Width = this.Width;
			MouseViewLabel.Width = this.Width;
		}
	}
}
