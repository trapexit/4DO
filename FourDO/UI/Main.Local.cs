using System.Collections.Generic;
using System.Windows.Forms;
using FourDO.Resources;
using FourDO.Utilities.Globals;

namespace FourDO.UI
{
	public partial class Main
	{
		private readonly List<string> supportedLanguageCodes = new List<string>{
				"en",
				"ru",
				"fr",
				"es",
				"pt",
				"zh-cn",
				"de"
			};

		private string GetLocalizedLanguageName(string supportedLanguageCode)
		{
			switch (supportedLanguageCode)
			{
				case "en":
					return Strings.MainMenuOptionsLanguageEnglish;
				case "ru":
					return Strings.MainMenuOptionsLanguageRussian;
				case "fr":
					return Strings.MainMenuOptionsLanguageFrench;
				case "es":
					return Strings.MainMenuOptionsLanguageSpanish;
				case "pt":
					return Strings.MainMenuOptionsLanguagePortuguese;
				case "zh-cn":
					return Strings.MainMenuOptionsLanguageChinese;
				case "de":
					return Strings.MainMenuOptionsLanguageGerman;
			}
			return null;
		}

		private void Localize()
		{
			this.fileMenuItem.Text = Strings.MainMenuFile;
			this.closeGameMenuItem.Text = Strings.MainMenuFileCloseRom;
			this.openCDImageMenuItem.Text = Strings.MainMenuFileOpenCDImage;
			this.loadLastGameMenuItem.Text = "    " + Strings.MainMenuFileAutoLoadLastGame;
			this.exitMenuItem.Text = Strings.MainMenuFileExit;

			this.consoleMenuItem.Text = Strings.MainMenuConsole;
			this.saveStateMenuItem.Text = Strings.MainMenuConsoleSaveState;
			this.loadStateMenuItem.Text = Strings.MainMenuConsoleLoadState;
			this.loadLastSaveMenuItem.Text = "    " + Strings.MainMenuConsoleAutoLoadLastSave;
			this.saveStateSlotMenuItem.Text = Strings.MainMenuConsoleSaveStateSlot;
			this.previousSlotMenuItem.Text = Strings.MainMenuConsoleSaveStateSlotPrevious;
			this.nextSlotMenuItem.Text = Strings.MainMenuConsoleSaveStateSlotNext;
			this.screenshotMenuItem.Text = Strings.MainMenuConsoleScreenshot;
			this.pauseMenuItem.Text = Strings.MainMenuConsolePause;
			this.advanceFrameMenuItem.Text = Strings.MainMenuConsoleAdvanceFrame;
			this.resetMenuItem.Text = Strings.MainMenuConsoleReset;

			this.displayMenuItem.Text = Strings.MainMenuDisplay;
			this.fullScreenMenuItem.Text = Strings.MainMenuDisplayFullscreen;
			this.VoidAreaMenuItem.Text = Strings.MainMenuDisplayVoidArea;
			this.DrawBorderMenuItem.Text = Strings.MainMenuDisplayVoidAreaDrawBorder;
			this.Pattern4DOMenuItem.Text = Strings.MainMenuDisplayVoidAreaPattern4DO;
			this.PatternBumpsMenuItem.Text = Strings.MainMenuDisplayVoidAreaPatternBumps;
			this.PatternMetalMenuItem.Text = Strings.MainMenuDisplayVoidAreaPatternMetal;
			this.PatternNoneMenuItem.Text = Strings.MainMenuDisplayVoidAreaPatternNone;
			this.smoothResizingMenuItem.Text = Strings.MainMenuDisplaySmoothResizing;
			this.preserveRatioMenuItem.Text = Strings.MainMenuDisplayPreserveRatio;
			this.autoCropMenuItem.Text = Strings.MainMenuDisplayAutoCrop;
			this.snapWindowMenuItem.Text = Strings.MainMenuDisplayResizeSnap;
			this.scalingModeMenuItem.Text = Strings.MainMenuDisplayScaling;
			this.scalingModeNoneMenuItem.Text = Strings.MainMenuDisplayScalingNone;
			this.scalingModeDoubleResMenuItem.Text = Strings.MainMenuDisplayScalingDoubleRes;
			this.scalingModeHq2xMenuItem.Text = Strings.MainMenuDisplayScalingHq2x;
			this.scalingModeHq3xMenuItem.Text = Strings.MainMenuDisplayScalingHq3x;
			this.scalingModeHq4xMenuItem.Text = Strings.MainMenuDisplayScalingHq4x;

			this.audioMenuItem.Text = Strings.MainMenuAudio;

			this.optionsMenuItem.Text = Strings.MainMenuOptions;
			this.settingsMenuItem.Text = Strings.MainMenuOptionsSettings;
			this.configureInputMenuItem.Text = Strings.MainMenuOptionsConfigureInput;

			this.languageMenuItem.Text = Strings.MainMenuOptionsLanguage;
			this.languageDefaultMenuItem.Text = Strings.MainMenuOptionsLanguageDefault;

			this.toolsMenuItem.Text = Strings.MainMenuTools;
			this.DiscBrowserMenuItem.Text = Strings.MainMenuToolsDiscBrowser;

			this.helpToolStripMenuItem.Text = Strings.MainMenuHelp;
			this.gameInfoMenuItem.Text = Strings.MainMenuHelpGameInfo;
			this.aboutMenuItem.Text = Strings.MainMenuHelpAbout;

			this.RomNagBox.LinkText = Strings.MainMessageChooseBiosRom;
			this.RomNagBox.MessageText = Strings.MainMessageNoBiosSelected;
			this.RomNagBox.HideText = Strings.MainMessageHide;

			if (this.volumeMenuItem != null)
				this.volumeMenuItem.Localize();

			// Bottom bar items.
			HealthLabelStripItem.Text = Strings.MainMessageHealth;

			// Update save state slot menu items.
			foreach (var item in saveStateSlotMenuItem.DropDownItems)
			{
				if (item is ToolStripMenuItem)
				{
					var menuItem = item as ToolStripMenuItem;
					if (menuItem.Tag != null && menuItem.Tag.GetType() == typeof(int))
						menuItem.Text = Strings.MainMessageSlot + " " + ((int)menuItem.Tag).ToString();
				}
			}

			// Update the disc menu items
			foreach (ToolStripMenuItem item in this.openGameMenuItems)
			{
				if (item.Tag != null && (item.Tag.GetType() == typeof(char)))
					item.Text = Strings.MainMenuFileOpenCDDrive + " - " + item.Tag + ":";
			}

			////////////////
			// Update language menu items.
			foreach (var item in this.languageMenuItems)
			{
				item.Text = this.GetLocalizedLanguageName((string)item.Tag);
			}
			
			// Append default culture translations to language menu items.
			// I do this in case someone picks a language they don't know and need to set it back.
			var currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
			System.Threading.Thread.CurrentThread.CurrentUICulture = Constants.SystemDefaultCulture;
			foreach (var item in this.languageMenuItems)
			{
				var defaultCultureName = this.GetLocalizedLanguageName((string)item.Tag);
				if (defaultCultureName != item.Text)
					item.Text += " (" + defaultCultureName + ")";
			}

			if (this.languageDefaultMenuItem.Text != Strings.MainMenuOptionsLanguageDefault)
				this.languageDefaultMenuItem.Text += " (" + Strings.MainMenuOptionsLanguageDefault + ")";
			if (this.languageMenuItem.Text != Strings.MainMenuOptionsLanguage)
				this.languageMenuItem.Text += " (" + Strings.MainMenuOptionsLanguage + ")";

			// Set culture back to normal.
			System.Threading.Thread.CurrentThread.CurrentUICulture = currentCulture;
		}
	}
}
