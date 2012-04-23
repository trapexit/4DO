using FourDO.Resources;

namespace FourDO.UI
{
	public partial class About
	{
		private void Localize()
		{
			this.FourDOMessageLabel.Text = Strings.HelpAbout4DOMessage;
			this.FourDOAuthorsLabel.Text = Strings.HelpAbout4DOAuthors;
			this.FourDOVersionLabel.Text = Strings.HelpAbout4DOVersion;

			this.ThreeDOPlayAuthorsMessage.Text = Strings.HelpAbout3DOPlayMessage;
			this.ThreeDOPlayAuthorsLabel.Text = Strings.HelpAbout3DOPlayAuthors;

			this.FreeDOMessageLabel.Text = Strings.HelpAboutFreeDOMessage;
			this.FreeDOAuthorsLabel.Text = Strings.HelpAboutFreeDOAuthors;

			this.JohnnyThanksMessage.Text = Strings.HelpAboutJohnnyThankMessage;
			this.JohnnyThanksLabel.Text = Strings.HelpAboutJohnnyThank;

			this.CloseButton.Text = Strings.HelpAboutClose;
		}
	}
}
