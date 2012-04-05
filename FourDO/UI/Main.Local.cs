using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FourDO.Resources;
using FourDO.Utilities.Globals;

namespace FourDO.UI
{
	public partial class Main
	{
		private List<string> supportedLanguageCodes = new List<string>{
				"en",
				"ru",
				"fr",
				"es",
				"pt",
				"zh-cn"
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
			this.snapWindowMenuItem.Text = Strings.MainMenuDisplayResizeSnap;

			this.audioMenuItem.Text = Strings.MainMenuAudio;

			this.optionsMenuItem.Text = Strings.MainMenuOptions;
			this.settingsMenuItem.Text = Strings.MainMenuOptionsSettings;
			this.configureInputMenuItem.Text = Strings.MainMenuOptionsConfigureInput;

			this.languageMenuItem.Text = Strings.MainMenuOptionsLanguage;
			this.languageDefaultMenuItem.Text = Strings.MainMenuOptionsLanguageDefault;

			this.helpToolStripMenuItem.Text = Strings.MainMenuHelp;
			this.gameInfoMenuItem.Text = Strings.MainMenuHelpGameInfo;
			this.aboutMenuItem.Text = Strings.MainMenuHelpAbout;

			this.quickDisplayDropDownButton.Text = Strings.MainMessageDisplayOptions;

			this.RomNagBox.LinkText = Strings.MainMessageNoBiosSelected;
			this.RomNagBox.MessageText = Strings.MainMessageChooseBiosRom;
			this.RomNagBox.HideText = Strings.MainMessageHide;

			if (this.volumeMenuItem != null)
				this.volumeMenuItem.Localize();

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

			////////////////
			// Copy to quick display settings menu.
			ToolStripMenuItem voidAreaMenuItem = null;
			foreach (var item in quickDisplayDropDownButton.DropDownItems)
			{
				if (item is ToolStripMenuItem)
				{
					ToolStripMenuItem menuItem = item as ToolStripMenuItem;
					menuItem.Text = ((ToolStripItem)menuItem.Tag).Text;

					if (menuItem.Tag == this.VoidAreaMenuItem)
						voidAreaMenuItem = menuItem;
				}
			}
			if (voidAreaMenuItem != null)
			{
				foreach (var item in voidAreaMenuItem.DropDownItems)
				{
					if (item is ToolStripMenuItem)
					{
						ToolStripMenuItem menuItem = item as ToolStripMenuItem;
						menuItem.Text = ((ToolStripItem)menuItem.Tag).Text;
					}
				}
			}
		}
	}
}
