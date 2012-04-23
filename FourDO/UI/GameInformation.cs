using System;
using System.Windows.Forms;

using FourDO.Emulation.GameSource;

namespace FourDO.UI
{
	internal partial class GameInformation : Form
	{
		public GameInformation()
		{
			InitializeComponent();
		}

		public IGameSource GameSource { get; set; }

		private void GameInformation_Load(object sender, EventArgs e)
		{
			this.Localize();

			var record = GameRegistrar.GetGameRecordById(this.GameSource.GetGameId());

			this.GameNameTextBox.Text = record == null ? " - " : record.Name;
			this.GameNameTextBox.SelectionStart = 0;

			this.GameIdTextBox.Text = this.GameSource.GetGameId();
			this.GameIdTextBox.SelectionStart = 0;

			this.ReleaseYearTextBox.Text = record == null ? " - " : record.ReleaseYear;
			this.ReleaseYearTextBox.SelectionStart = 0;

			this.PublisherTextBox.Text = record == null ? " - " : record.Publisher;
			this.PublisherTextBox.SelectionStart = 0;

			this.RegionsTextBox.Text = record == null ? " - " : record.Regions;
			this.RegionsTextBox.SelectionStart = 0;

			if (record == null && (this.GameSource is GameSourceBase))
			{
				// This will read sectors, but oh well.
				this.ChecksumTextBox.Text = GameRegistrar.CalculateGameChecksum((GameSourceBase)this.GameSource);
			}
			else
			{
				// This will read sectors, but oh well.
				this.ChecksumTextBox.Text = record == null ? " - " : record.CheckSum;
			}
			this.ChecksumTextBox.SelectionStart = 0;
		}

		private void GameInformation_Shown(object sender, EventArgs e)
		{
			this.OKButton.Focus();
		}

		private void OKButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
