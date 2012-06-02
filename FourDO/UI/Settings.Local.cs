using System;
using FourDO.Resources;

namespace FourDO.UI
{
	public partial class Settings
	{
		private void Localize()
		{
			this.Text = Strings.SettingsWindowTitle;

			this.InstructionsLabel.Text = Strings.SettingsWindowInstructions;
			this.tabMain.TabPages[0].Text = Strings.SettingsTabGeneral;
			this.tabMain.TabPages[1].Text = Strings.SettingsTabDisplay;
			this.tabMain.TabPages[2].Text = Strings.SettingsTabAdvanced;

			this.StartupGroupBox.Text = Strings.SettingsTabGeneralStartupOptions;
			this.chkLoadLastGame.Text = Strings.SettingsTabGeneralAutoLoadLastGame;
			this.BehaviorGroupBox.Text = Strings.SettingsTabGeneralBehaviorOptions;
			this.chkLoadLastSave.Text = Strings.SettingsTabGeneralAutoLoadLastSave;
			this.InactiveGroupBox.Text = Strings.SettingsTabGeneralInactiveOptions;
			this.chkInactivePauseEmulation.Text = Strings.SettingsTabGeneralInactivePauseEmulation;
			this.chkInactiveIgnoreInput.Text = Strings.SettingsTabGeneralInactiveIgnoreKeyboard;

			this.DisplayGeneralGroupBox.Text = Strings.SettingsTabDisplay;
			this.chkSmoothImageResize.Text = Strings.SettingsTabDisplaySmoothResizing;
			this.chkPreserveAspectRatio.Text = Strings.SettingsTabDisplayPreserveAspectRatio;
			this.chkSnapWindowIncrements.Text = Strings.SettingsTabDisplaySnapResizing;
			this.chkAutoCropImage.Text = Strings.SettingsTabDisplayAutoCrop;
			this.DisplayVoidGroupBox.Text = Strings.SettingsTabDisplayVoidOptions;
			this.chkDrawGrayBorder.Text = Strings.SettingsTabDisplayDrawBorder;
			this.VoidPatternLabel.Text = Strings.SettingsTabDisplayVoidPattern;
			this.Background4DOOption.Text = Strings.SettingsTabDisplayVoidPattern4DO;
			this.BackgroundBumpsOption.Text = Strings.SettingsTabDisplayVoidPatternBumps;
			this.BackgroundMetalOption.Text = Strings.SettingsTabDisplayVoidPatternMetal;
			this.BackgroundNoneOption.Text = Strings.SettingsTabDisplayVoidPatternNone;

			this.optScaleDoubleRes.Text = Strings.SettingsTabDisplayScalingModeDoubleRes;
			this.HighResolutionLabel.Text = Strings.SettingsTabDisplayScalingModeDoubleResMessage;
			this.ScalingModeGroupBox.Text = Strings.SettingsTabDisplayScalingModeFrame;
			this.optScaleHq2x.Text = Strings.SettingsTabDisplayScalingModeHq2x;
			this.optScaleHq3x.Text = Strings.SettingsTabDisplayScalingModeHq3x;
			this.optScaleHq4x.Text = Strings.SettingsTabDisplayScalingModeHq4x;
			this.HQXMessageLabel.Text = Strings.SettingsTabDisplayScalingModeHqxMessage;
			this.ScalingModeMessageLabel.Text = Strings.SettingsTabDisplayScalingModeMessage;
			this.optScaleNone.Text = Strings.SettingsTabDisplayScalingModeNone;
			this.ScalingModeWarningLabel.Text = Strings.SettingsTabDisplayScalingModeWarning;

			this.ResetDefaultsLabel.Text = Strings.SettingsTabAdvancedDefaultsMessage;
			this.AdvancedResetButton.Text = Strings.SettingsTabAdvancedResetDefaults;
			this.CpuClockGroupBox.Text = Strings.SettingsTabAdvancedClockFrame;
			this.ClockMessageLabel.Text = Strings.SettingsTabAdvancedClockMessage;
			this.ClockNormalFpsLabel.Text = Strings.SettingsTabAdvancedClockNormalFPS;
			this.ClockHigherFpsLabel.Text = Strings.SettingsTabAdvancedClockHigherFPS;
			this.AudioBufferGroupBox.Text = Strings.SettingsTabAdvancedAudioBufferFrame;
			this.AudioBufferMessageLabel.Text = Strings.SettingsTabAdvancedAudioBufferMessage;
			this.AudioLessLagLabel.Text = 
					Strings.SettingsTabAdvancedAudioBufferLessLag 
					+ Environment.NewLine
					+ Strings.SettingsTabAdvancedAudioBufferMoreGlitches;
			this.AudioMoreLagLabel.Text = 
					Strings.SettingsTabAdvancedAudioBufferMoreLag
					+ Environment.NewLine
					+ Strings.SettingsTabAdvancedAudioBufferFewerGlitches;

			this.OKButton.Text = Strings.SettingsOK;
			this.CloseButton.Text = Strings.SettingsCancel;
		}
	}
}
