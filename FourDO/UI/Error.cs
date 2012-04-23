using System.Windows.Forms;

namespace FourDO.UI
{
	public static class Error
	{
		public static void ShowError(string text)
		{
			Error.ShowError(text, false);
		}

		public static void ShowError(string text, bool severe)
		{
			MessageBox.Show(text, "FourDO", MessageBoxButtons.OK, severe ? MessageBoxIcon.Error : MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
		}
	}
}
