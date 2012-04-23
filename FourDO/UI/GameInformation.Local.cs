using FourDO.Resources;

namespace FourDO.UI
{
	internal partial class GameInformation
	{
		private void Localize()
		{
			this.Text = Strings.HelpInfoWindowTitle;

			this.GameNameLabel.Text = Strings.HelpInfoGameName;
			this.GameIdLabel.Text = Strings.HelpInfoGameId;
			this.ReleaseYearLabel.Text = Strings.HelpInfoReleaseYear;
			this.PublisherLabel.Text = Strings.HelpInfoPublisher;
			this.RegionLabel.Text = Strings.HelpInfoRegion;
			this.ChecksumLabel.Text = Strings.HelpInfoChecksum;

			this.OKButton.Text = Strings.HelpInfoOK;
		}
	}
}
