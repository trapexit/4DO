using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FourDO.Resources;

namespace FourDO.UI.DiscBrowser
{
	public partial class Browser
	{
		private void Localize()
		{
			this.Text = Strings.BrowserWindowTitle;
			this.DirectoryUpButton.Text = Strings.BrowserUp;
			this.FileListView.Columns[0].Text = Strings.BrowserColumnName;
			this.FileListView.Columns[1].Text = Strings.BrowserColumnBytes;
			this.FileListView.Columns[2].Text = Strings.BrowserColumnID;
			this.FileListView.Columns[3].Text = Strings.BrowserColumnExtension;
			this.ExtractMenuItem.Text = Strings.BrowserMenuExtract;
			this.ExtractDiscMenuItem.Text = Strings.BrowserMenuExtractDisc;
			this.ExtractDirectoryMenuItem.Text = Strings.BrowserMenuExtractDirectory;
			this.SelectAllToolStripMenuItem.Text = Strings.BrowserMenuSelectAll;
			this.OpenMenuItem.Text = Strings.BrowserMenuOpenDirectory;
			this.DirectoryNotFoundLabel.Text = Strings.BrowserDirectoryNotFound;
			this.CancelButton.Text = Strings.SettingsCancel;
		}
	}
}
