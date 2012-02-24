using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FourDO.Resources;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	public partial class ControllerPreview
	{
		private void Localize()
		{
			this.APanel.Tag = JohnnyInputStrings.PreviewAButton;
			this.BPanel.Tag = JohnnyInputStrings.PreviewBButton;
			this.CPanel.Tag = JohnnyInputStrings.PreviewCButton;
			this.LPanel.Tag = JohnnyInputStrings.PreviewLButton;
			this.RPanel.Tag = JohnnyInputStrings.PreviewRButton;
			this.XPanel.Tag = JohnnyInputStrings.PreviewXButton;
			this.PPanel.Tag = JohnnyInputStrings.PreviewPButton;
			this.UpPanel.Tag = JohnnyInputStrings.PreviewUpButton;
			this.DownPanel.Tag = JohnnyInputStrings.PreviewDownButton;
			this.LeftPanel.Tag = JohnnyInputStrings.PreviewLeftButton;
			this.RightPanel.Tag = JohnnyInputStrings.PreviewRightButton;
		}
	}
}
