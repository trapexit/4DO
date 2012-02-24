using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FourDO.Resources;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	public partial class JohnnyInputSettings
	{
		private void Localize()
		{
			this.Text = JohnnyInputStrings.WindowTitle;

			this.MainTabControl.TabPages[0].Text = JohnnyInputStrings.Player + " 1";
			this.MainTabControl.TabPages[1].Text = JohnnyInputStrings.Player + " 2";
			this.MainTabControl.TabPages[2].Text = JohnnyInputStrings.Player + " 3";
			this.MainTabControl.TabPages[3].Text = JohnnyInputStrings.Player + " 4";
			this.MainTabControl.TabPages[4].Text = JohnnyInputStrings.Player + " 5";
			this.MainTabControl.TabPages[5].Text = JohnnyInputStrings.Player + " 6";

			this.OKButton.Text = JohnnyInputStrings.OK;
			this.CloseButton.Text = JohnnyInputStrings.Cancel;
		}
	}
}
