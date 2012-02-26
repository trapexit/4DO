using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace FourDO.UI.Controls
{
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip | ToolStripItemDesignerAvailability.ContextMenuStrip)]
	public class VolumeMenuItem : ToolStripControlHost
	{
		public EventHandler VolumeChanged;

		private VolumeSlider trackBar;

		public void Localize()
		{
			this.trackBar.Localize();
		}

		public double Volume
		{
			get
			{
				return this.trackBar.Volume;
			}
			set
			{
				this.trackBar.Volume = value;
			}
		}

		public VolumeMenuItem()
			: base(new VolumeSlider())
		{
			this.trackBar = this.Control as VolumeSlider;
			this.trackBar.VolumeChanged += new System.EventHandler(this.trackBar_ValueChanged);
		}

		private void trackBar_ValueChanged(object sender, EventArgs e)
		{
			if (this.VolumeChanged != null)
				this.VolumeChanged(sender, e);
		}

	}
}
