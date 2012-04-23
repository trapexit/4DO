﻿using System;
using System.Windows.Forms;

namespace FourDO.UI
{
	public partial class About : Form
	{
		public About()
		{
			InitializeComponent();
		}

		private void About_Load(object sender, EventArgs e)
		{
			this.Localize();
			VersionTextBox.Text = Application.ProductVersion;
		}
	}
}
