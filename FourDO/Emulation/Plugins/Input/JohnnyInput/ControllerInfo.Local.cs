using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FourDO.Resources;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	public partial class ControllerInfo
	{
		private void Localize()
		{
			this.SetABindingLabel.Text = JohnnyInputStrings.InfoSetABinding;
			this.With1Label.Text = JohnnyInputStrings.InfoWith;
			this.SetButtonsLabel.Text = JohnnyInputStrings.InfoSetKeys;
			this.ClearABindingLabel.Text = JohnnyInputStrings.InfoClearABinding;
			this.With2Label.Text = JohnnyInputStrings.InfoWith;
			this.ClearButtonsLabel.Text = JohnnyInputStrings.InfoClearKeys;
			this.EscapeLabel.Text = JohnnyInputStrings.InfoEscapeToCancel;

			this.DetectedDevicesLabel.Text = JohnnyInputStrings.InfoDetectedDevices;
			this.MouseViewLabel.Text = JohnnyInputStrings.InfoMoveMouse;
		}
	}
}
