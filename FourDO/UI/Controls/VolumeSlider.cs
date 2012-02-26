using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FourDO.Resources;

namespace FourDO.UI.Controls
{
	public partial class VolumeSlider : UserControl
	{
		public EventHandler VolumeChanged;

		private double volume;

		public void Localize()
		{
			this.VolumeMessageLabel.Text = Strings.MainMessageVolume + ":";
		}

		public double Volume
		{
			get
			{
				return this.volume;
			}
			set
			{
				this.volume = value;
				this.volumeTrackBar.Value = (int)(this.volume * 100);
			}
		}

		public VolumeSlider()
		{
			InitializeComponent();

			this.MinimumSize = this.Size;
			this.MaximumSize = this.Size;
		}

		private void volumeTrackBar_Scroll(object sender, EventArgs e)
		{
			this.volume = this.volumeTrackBar.Value / 100.0d;
			if (this.VolumeChanged != null)
				this.VolumeChanged(sender, e);
		}

		private void volumeTrackBar_ValueChanged(object sender, EventArgs e)
		{
			this.volumeLabel.Text = string.Format("{0}%", volumeTrackBar.Value);
		}

		private void VolumeSlider_Load(object sender, EventArgs e)
		{
			this.Localize();
		}
	}
}
