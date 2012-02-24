using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FourDO.Resources;

namespace FourDO.UI
{
	public partial class Main
	{
		private void Localize()
		{
			this.fileMenuItem.Text = Strings.MainMenuFile;
			this.closeGameMenuItem.Text = Strings.MainMenuFileCloseRom;
			this.openCDImageMenuItem.Text = Strings.MainMenuFileOpenCDImage;
			this.loadLastGameMenuItem.Text = Strings.MainMenuFileAutoLoadLastGame;
			this.chooseBiosRomMenuItem.Text = Strings.MainMenuFileChooseBiosRom;
			this.exitMenuItem.Text = Strings.MainMenuFileExit;

			this.consoleMenuItem.Text = Strings.MainMenuConsole;
			this.saveStateMenuItem.Text = Strings.MainMenuConsoleSaveState;
			this.loadStateMenuItem.Text = Strings.MainMenuConsoleLoadState;
			this.loadLastSaveMenuItem.Text = Strings.MainMenuConsoleAutoLoadLastSave;
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
			this.helpToolStripMenuItem.Text = Strings.MainMenuHelp;
			this.gameInfoMenuItem.Text = Strings.MainMenuHelpGameInfo;
			this.aboutMenuItem.Text = Strings.MainMenuHelpAbout;

			this.quickDisplayDropDownButton.Text = Strings.MainMessageDisplayOptions;
		}
	}
}
