using System;
using System.Drawing;
using System.Windows.Forms;
using FourDO.Emulation.FreeDO;

namespace FourDO.UI
{
	public partial class Settings : Form
	{
		public Settings()
		{
			InitializeComponent();
		}

		private void Settings_Load(object sender, EventArgs e)
		{
			this.Localize();

			Background4DOOption.Tag = VoidAreaPattern.FourDO;
			BackgroundBumpsOption.Tag = VoidAreaPattern.Bumps;
			BackgroundMetalOption.Tag = VoidAreaPattern.Metal;
			BackgroundNoneOption.Tag = VoidAreaPattern.None;

			chkLoadLastGame.Checked = Properties.Settings.Default.AutoOpenGameFile;
			chkLoadLastSave.Checked = Properties.Settings.Default.AutoLoadLastSave;
			chkShowMessages.Checked = Properties.Settings.Default.ShowInformationalMessages;
			chkInactivePauseEmulation.Checked = Properties.Settings.Default.InactivePauseEmulation;
			chkInactiveIgnoreInput.Checked = Properties.Settings.Default.InactiveIgnoreKeyboard;

			chkSmoothImageResize.Checked = Properties.Settings.Default.WindowImageSmoothing;
			chkPreserveAspectRatio.Checked = Properties.Settings.Default.WindowPreseveRatio;
			chkSnapWindowIncrements.Checked = Properties.Settings.Default.WindowSnapSize;
			chkAutoCropImage.Checked = Properties.Settings.Default.WindowAutoCrop;
			chkDrawGrayBorder.Checked = Properties.Settings.Default.VoidAreaBorder;

			Background4DOOption.Checked = (Properties.Settings.Default.VoidAreaPattern == (int)VoidAreaPattern.FourDO);
			BackgroundBumpsOption.Checked = (Properties.Settings.Default.VoidAreaPattern == (int)VoidAreaPattern.Bumps);
			BackgroundMetalOption.Checked = (Properties.Settings.Default.VoidAreaPattern == (int)VoidAreaPattern.Metal);
			BackgroundNoneOption.Checked = (Properties.Settings.Default.VoidAreaPattern == (int)VoidAreaPattern.None);

			optScaleNone.Checked = (Properties.Settings.Default.WindowScalingAlgorithm == (int)ScalingAlgorithm.None);
			optScaleHq2x.Checked = (Properties.Settings.Default.WindowScalingAlgorithm == (int)ScalingAlgorithm.Hq2X);
			optScaleHq3x.Checked = (Properties.Settings.Default.WindowScalingAlgorithm == (int)ScalingAlgorithm.Hq3X);
			optScaleHq4x.Checked = (Properties.Settings.Default.WindowScalingAlgorithm == (int)ScalingAlgorithm.Hq4X);
			optScaleDoubleRes.Checked = Properties.Settings.Default.RenderHighResolution;
			CpuClockBar.Value = Properties.Settings.Default.CpuClockHertz / 1000;
			AudioBufferBar.Value = Properties.Settings.Default.AudioBufferMilliseconds;

			tabMain.SelectedIndex = Properties.Settings.Default.SelectedOptionTab;

			this.UpdateUI();
		}

		private void OKButton_Click(object sender, EventArgs e)
		{
			Properties.Settings.Default.AutoOpenGameFile = chkLoadLastGame.Checked;
			Properties.Settings.Default.AutoLoadLastSave = chkLoadLastSave.Checked;
			Properties.Settings.Default.ShowInformationalMessages = chkShowMessages.Checked;
			Properties.Settings.Default.InactivePauseEmulation = chkInactivePauseEmulation.Checked;
			Properties.Settings.Default.InactiveIgnoreKeyboard = chkInactiveIgnoreInput.Checked;

			Properties.Settings.Default.WindowImageSmoothing = chkSmoothImageResize.Checked;
			Properties.Settings.Default.WindowPreseveRatio = chkPreserveAspectRatio.Checked;
			Properties.Settings.Default.WindowSnapSize = chkSnapWindowIncrements.Checked;
			Properties.Settings.Default.WindowAutoCrop = chkAutoCropImage.Checked;
			Properties.Settings.Default.VoidAreaBorder = chkDrawGrayBorder.Checked;

			if (this.Background4DOOption.Checked)
				Properties.Settings.Default.VoidAreaPattern = (int)VoidAreaPattern.FourDO;
			if (this.BackgroundBumpsOption.Checked)
				Properties.Settings.Default.VoidAreaPattern = (int)VoidAreaPattern.Bumps;
			if (this.BackgroundMetalOption.Checked)
				Properties.Settings.Default.VoidAreaPattern = (int)VoidAreaPattern.Metal;
			if (this.BackgroundNoneOption.Checked)
				Properties.Settings.Default.VoidAreaPattern = (int)VoidAreaPattern.None;

			if (this.optScaleNone.Checked)
				Properties.Settings.Default.WindowScalingAlgorithm = (int)ScalingAlgorithm.None;
			if (this.optScaleHq2x.Checked)
				Properties.Settings.Default.WindowScalingAlgorithm = (int)ScalingAlgorithm.Hq2X;
			if (this.optScaleHq3x.Checked)
				Properties.Settings.Default.WindowScalingAlgorithm = (int)ScalingAlgorithm.Hq3X;
			if (this.optScaleHq4x.Checked)
				Properties.Settings.Default.WindowScalingAlgorithm = (int)ScalingAlgorithm.Hq4X;

			Properties.Settings.Default.RenderHighResolution = optScaleDoubleRes.Checked;
			Properties.Settings.Default.CpuClockHertz = CpuClockBar.Value * 1000;
			Properties.Settings.Default.AudioBufferMilliseconds = AudioBufferBar.Value;

			Properties.Settings.Default.SelectedOptionTab = tabMain.SelectedIndex;

			Properties.Settings.Default.Save();

			this.Close();
		}

		private void CancelButton_Click(object sender, EventArgs e)
		{
			// Always save the tab they were on.
			Properties.Settings.Default.SelectedOptionTab = tabMain.SelectedIndex;
			Properties.Settings.Default.Save();

			this.Close();
		}

		private void Background4DOOption_CheckedChanged(object sender, EventArgs e)
		{
			this.UpdateUI();
		}

		private void BackgroundBumpsOption_CheckedChanged(object sender, EventArgs e)
		{
			this.UpdateUI();
		}

		private void BackgroundMetalOption_CheckedChanged(object sender, EventArgs e)
		{
			this.UpdateUI();
		}

		private void BackgroundNoneOption_CheckedChanged(object sender, EventArgs e)
		{
			this.UpdateUI();
		}

		private void chkDrawGrayBorder_CheckedChanged(object sender, EventArgs e)
		{
			this.UpdateUI();
		}

		private void AudioBufferBar_Scroll(object sender, EventArgs e)
		{
			this.UpdateUI();
		}

		private void AudioBufferBar_ValueChanged(object sender, EventArgs e)
		{
			this.UpdateUI();
		}
		
		private void CpuClockBar_Scroll(object sender, EventArgs e)
		{
			this.UpdateUI();
		}

		private void CpuClockBar_ValueChanged(object sender, EventArgs e)
		{
			this.UpdateUI();
		}

		private void AdvancedResetButton_Click(object sender, EventArgs e)
		{
			// TODO: I need to base this off the real defaults.
			CpuClockBar.Value = 12500;
			AudioBufferBar.Value = 100;
		}

		private void UpdateUI()
		{
			BorderPanel.BackColor = chkDrawGrayBorder.Checked ? Color.Gray : Color.Black;
			if (this.Background4DOOption.Checked)
				BackgroundPanel.BackgroundImage = Background4DOPicture.BackgroundImage;
			if (this.BackgroundBumpsOption.Checked)
				BackgroundPanel.BackgroundImage = BackgroundBumpsPicture.BackgroundImage;
			if (this.BackgroundMetalOption.Checked)
				BackgroundPanel.BackgroundImage = BackgroundMetalPicture.BackgroundImage;
			if (this.BackgroundNoneOption.Checked)
				BackgroundPanel.BackgroundImage = BackgroundNonePicture.BackgroundImage;
			AudioBufferValueLabel.Text = (AudioBufferBar.Value.ToString()) + "ms";

			decimal clockMegahertz = CpuClockBar.Value / (decimal)1000;
			int clockPercent = CpuClockBar.Value / 125;
			CpuClockValueLabel.Text = string.Format("{0:0.0}Mhz ({1}%)", clockMegahertz, clockPercent);
		}
	}
}

