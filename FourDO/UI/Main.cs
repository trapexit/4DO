using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FourDO.Emulation;
using FourDO.Emulation.FreeDO;
using FourDO.Emulation.Plugins.Input;
using FourDO.UI.DiscBrowser;
using FourDO.Utilities;
using FourDO.Utilities.Globals;
using FourDO.Utilities.MouseHook;
using CDLib;
using FourDO.Emulation.GameSource;
using FourDO.Emulation.Plugins;
using FourDO.Resources;

namespace FourDO.UI
{
	internal enum GameSourceType
	{
		None = 0,
		File,
		Disc,
		BinaryFile,
	}

	public partial class Main : Form
	{
		private const string FOURDO_NAME = "4DO";

		private const int BASE_WIDTH = 320;
		private const int BASE_HEIGHT = 240;

		private readonly SizeGuard sizeGuard = new SizeGuard();

		private Point pointBeforeFullScreen;
		private bool maximizedBeforeFullScreen = false;
		private bool isWindowFullScreen = false;

		private bool isPausedBeforeInactive = false;

		private readonly MouseHook mouseHook = new MouseHook();

		private readonly List<ToolStripMenuItem> openGameMenuItems = new List<ToolStripMenuItem>();
		private Controls.VolumeMenuItem volumeMenuItem;
		private readonly List<ToolStripMenuItem> languageMenuItems = new List<ToolStripMenuItem>();

		#region Load/Close Form Events

		#region Public Members

		public Main()
		{
			InitializeComponent();
		}

		#endregion // Public Members

		private void Main_Load(object sender, EventArgs e)
		{
			this.Localize();

			// Some basic form setup.
			this.sizeBox.BaseWidth = BASE_WIDTH;
			this.sizeBox.BaseHeight = BASE_HEIGHT;

			this.sizeGuard.BaseWidth = BASE_WIDTH;
			this.sizeGuard.BaseHeight = BASE_HEIGHT;
			this.sizeGuard.WindowExtraWidth = this.Width - this.ClientSize.Width;
			this.sizeGuard.WindowExtraHeight = (this.Height - this.ClientSize.Height) + this.MainMenuBar.Height + this.MainStatusStrip.Height;

			this.mouseHook.Install();
			this.mouseHook.MouseMove += new MouseHookEventHandler(mouseHook_MouseMove);
			this.mouseHook.MouseDown += new MouseHookEventHandler(mouseHook_MouseDown);

			this.VersionStripItem.Text = FOURDO_NAME + " " + Application.ProductVersion;

			GameConsole.Instance.ConsoleStateChange += new ConsoleStateChangeHandler(Instance_ConsoleStateChange);
			GameConsole.Instance.InputPlugin.ConsoleEventRaised += new ConsoleEventRaisedHandler(InputPlugin_ConsoleEventRaised);

			////////////////////
			// Form size and position.

			// Initial form size.
			int savedWidth = Properties.Settings.Default.WindowWidth;
			int savedHeight = Properties.Settings.Default.WindowHeight;
			this.Width = (savedWidth > 0) ? savedWidth : this.sizeGuard.BaseWidth * 2 + this.sizeGuard.WindowExtraWidth;
			this.Height = (savedHeight > 0) ? savedHeight : this.sizeGuard.BaseHeight * 2 + this.sizeGuard.WindowExtraHeight;

			// Initial form position.
			if (RunOptions.StartFullScreen)
			{
				Properties.Settings.Default.WindowFullScreen = true;
				Properties.Settings.Default.Save();
			}

			if (Properties.Settings.Default.WindowFullScreen)
			{
				// If they were in full screen when they exited, set ourselves at the top+left in the correct screen.
				int screenNum = 0;
				Screen screenToUse = null;
				foreach (Screen screen in Screen.AllScreens)
				{
					if (screenNum == Properties.Settings.Default.WindowFullScreenDevice)
					{
						screenToUse = screen;
						break;
					}
					screenNum++;
				}

				if (screenToUse != null)
					screenToUse = Screen.PrimaryScreen;

				this.Left = screenToUse.WorkingArea.Left;
				this.Top = screenToUse.WorkingArea.Top;
			}

			// Let's ensure we don't go off the bounds of the screen.
			Screen currentScreen = Utilities.DisplayHelper.GetCurrentScreen(this);
			if (this.Bottom > currentScreen.WorkingArea.Bottom)
			{
				this.Top -= (this.Bottom - currentScreen.WorkingArea.Bottom);
				this.Top = Math.Max(this.Top, currentScreen.WorkingArea.Top);
			}
			if (this.Right > currentScreen.WorkingArea.Right)
			{
				this.Left -= (this.Right - currentScreen.WorkingArea.Right);
				this.Left = Math.Max(this.Left, currentScreen.WorkingArea.Left);
			}

			// If we updated the size ourselves (we had no valid default) save it now.
			if (savedWidth <= 0 || savedHeight <= 0)
				this.DoSaveWindowSize();
			this.sizeBox.Visible = false; // and shut that damn thing up!

			///////////////////////////
			// Create dynamic form elements

			// Create "Open Disc" menu items for each CD Drive.
			int menuInsertIndex = fileMenuItem.DropDownItems.IndexOf(openCDImageMenuItem) + 1;
			foreach (char drive in CDDrive.GetCDDriveLetters())
			{
				var newItem = new ToolStripMenuItem("");
				newItem.Tag = drive;
				newItem.Click += new EventHandler(openFromDriveMenuItem_Click);
				fileMenuItem.DropDownItems.Insert(menuInsertIndex, newItem);
				openGameMenuItems.Add(newItem);
				menuInsertIndex++;
			}
			openGameMenuItems.Add(openCDImageMenuItem);

			// Add volume control menu item
			this.volumeMenuItem = new UI.Controls.VolumeMenuItem();
			this.volumeMenuItem.VolumeChanged += new EventHandler(volumeMenuItem_VolumeChanged);
			this.audioMenuItem.DropDownItems.Add(this.volumeMenuItem);

			// Set tags for certain menu items.
			this.Pattern4DOMenuItem.Tag = VoidAreaPattern.FourDO;
			this.PatternBumpsMenuItem.Tag = VoidAreaPattern.Bumps;
			this.PatternMetalMenuItem.Tag = VoidAreaPattern.Metal;
			this.PatternNoneMenuItem.Tag = VoidAreaPattern.None;

			// Add save slot menu items.
			if (Properties.Settings.Default.SaveStateSlot < 0 || Properties.Settings.Default.SaveStateSlot > 9)
			{
				Properties.Settings.Default.SaveStateSlot = 0;
				Properties.Settings.Default.Save();
			}

			for (int x = 0; x < 10; x++)
			{
				var newItem = new ToolStripMenuItem(Strings.MainMessageSlot + " " + x.ToString());
				newItem.Name = "saveSlotMenuItem" + x.ToString();
				newItem.Tag = x;
				newItem.Click += new EventHandler(saveSlotMenuItem_Click);
				saveStateSlotMenuItem.DropDownItems.Add(newItem);
			}

			// Add language menu items.
			foreach (var languageCode in this.supportedLanguageCodes)
			{
				var newItem = new ToolStripMenuItem("");
				newItem.Tag = languageCode;
				newItem.Click += new EventHandler(languageSelectionMenuItem_Click);
				languageMenuItem.DropDownItems.Add(newItem);
				this.languageMenuItems.Add(newItem);
			}

			//////////////
			// Localize
			this.Localize();

			///////////////////////////////////////////////////////////////////////////

			/////////////
			// Handle ROM file nag box
			if (!File.Exists(Properties.Settings.Default.BiosRomFile))
			{
				Properties.Settings.Default.BiosRomFile = "";
				Properties.Settings.Default.Save();
				this.DoShowRomNag();
			}

			///////////
			// Clear the last loaded game if they don't want us to automatically loading games.
			if (Properties.Settings.Default.AutoOpenGameFile == false)
			{
				Properties.Settings.Default.GameRomSourceType = (int)GameSourceType.None;
				Properties.Settings.Default.Save();
			}

			///////////
			// If we have startup options that specify what game to load, honor their preference now.
			if (RunOptions.StartLoadDrive != null)
			{
				Properties.Settings.Default.GameRomSourceType = (int)GameSourceType.Disc;
				Properties.Settings.Default.GameRomDrive = RunOptions.StartLoadDrive.Value;
			}

			if (!string.IsNullOrWhiteSpace(RunOptions.StartLoadFile))
			{
				Properties.Settings.Default.GameRomSourceType = (int)GameSourceType.File;
				Properties.Settings.Default.GameRomFile = RunOptions.StartLoadFile;
			}

			///////////
			// Now that settings have been mucked with, subscribe to their change event.
			Properties.Settings.Default.PropertyChanged += new PropertyChangedEventHandler(Settings_PropertyChanged);

			///////////////////////////
			// Fire her up!
			this.DoConsoleStart(true);

			if (RunOptions.StartupPaused)
			{
				// The window's not yet active. When it's activated, we want to make sure it doesn't resume.
				this.isPausedBeforeInactive = true;
				this.DoConsoleTogglePause();
			}

			this.UpdateUI();

			// Oh, and start automatically launch a form if requested.
			switch (RunOptions.StartupForm)
			{
				case RunOptions.StartupFormOption.ConfigureInput:
					this.DoShowConfigureInput();
					break;
			}
		}

		private void Main_FormClosed(object sender, FormClosedEventArgs e)
		{
			// Ignore console state changes from now on. The console will be shutting itself down.
			GameConsole.Instance.ConsoleStateChange -= new ConsoleStateChangeHandler(Instance_ConsoleStateChange);

			this.mouseHook.Uninstall();

			this.DoConsoleStop();

			GameConsole.Instance.Destroy();
			this.gameCanvas.Destroy();
		}

		#endregion // Load/Close Form Events

		#region Event Handlers

		private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			// This is here in case some option box in the future updates the setting for us.
			// We hide ourselves here in order to keep ourselves from looking like a liar.
			//
			// It would be a bad idea to add something like a prompt to restart here, since
			// this setting could presumably be changed anywhere. We'll leave prompting the
			// user to the event handlers.
			if (e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.BiosRomFile))
			{
				if (string.IsNullOrEmpty(Properties.Settings.Default.BiosRomFile) == false)
					this.DoHideRomNag();
			}

			if (e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.AutoOpenGameFile)
				|| e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.AutoLoadLastSave)
				|| e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.SaveStateSlot)
				|| e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.WindowFullScreen)
				|| e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.WindowPreseveRatio)
				|| e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.WindowSnapSize)
				|| e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.WindowScalingAlgorithm)
				|| e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.WindowImageSmoothing)
				|| e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.WindowAutoCrop)
				|| e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.GameRomSourceType)
				|| e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.GameRomLastDirectory)
				|| e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.GameRomFile)
				|| e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.GameRomDrive)
				|| e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.VoidAreaBorder)
				|| e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.VoidAreaPattern)
				|| e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.AudioVolume)
				|| e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.RenderHighResolution))
			{
				this.UpdateUI();
			}

			if (e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.AudioBufferMilliseconds))
				GameConsole.Instance.AudioBufferMilliseconds = Properties.Settings.Default.AudioBufferMilliseconds;

			if (e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.CpuClockHertz))
				GameConsole.Instance.CpuClockHertz = Properties.Settings.Default.CpuClockHertz;

			if (e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.WindowScalingAlgorithm))
				this.gameCanvas.ScalingAlgorithm = (ScalingAlgorithm)Properties.Settings.Default.WindowScalingAlgorithm;

			if (e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.RenderHighResolution))
			{
				GameConsole.Instance.RenderHighResolution = Properties.Settings.Default.RenderHighResolution;
				this.gameCanvas.RenderHighResolution = Properties.Settings.Default.RenderHighResolution;
			}
		}

		private void Main_Resize(object sender, EventArgs e)
		{
			if (this.isWindowFullScreen == true
				|| this.WindowState == FormWindowState.Maximized
				|| this.WindowState == FormWindowState.Minimized
				|| Utilities.DisplayHelper.IsFormDocked(this))
			{
				this.sizeBox.Visible = false;
				return;
			}

			this.SuspendLayout();
			sizeBox.UpdateSizeText(this.gameCanvas.Width, this.gameCanvas.Height);
			sizeBox.SetBounds(
					this.ClientSize.Width - 6 - this.sizeBox.PreferredSize.Width,
					this.ClientSize.Height - this.MainStatusStrip.Height - 6 - this.sizeBox.PreferredSize.Height,
					this.sizeBox.PreferredSize.Width,
					this.sizeBox.PreferredSize.Height);
			sizeBox.Visible = true;
			this.ResumeLayout();

			this.DoSaveWindowSize();
		}

		private void Main_Deactivate(object sender, EventArgs e)
		{
			// Remember console state paused vs. running, and pause the emulation if the user has specified this option.
			this.isPausedBeforeInactive = (GameConsole.Instance.State == ConsoleState.Paused);
			if (Properties.Settings.Default.InactivePauseEmulation && GameConsole.Instance.State == ConsoleState.Running)
				this.DoConsoleTogglePause();

			// Ignore keyboard input on inactive if the user has specified this option.
			if (Properties.Settings.Default.InactiveIgnoreKeyboard)
			{
				IInputPlugin inputPlugin = GameConsole.Instance.InputPlugin;
				inputPlugin.DisableKeyboardInput();
			}
		}

		private void Main_Activated(object sender, EventArgs e)
		{
			///////////////////////
			// Restore the state of some things when the window is active again.
			//
			// Note that we do these things if the options aren't selected. This is because the settings
			// may have changed after we paused the console!

			// Keyboard input is allowed again.
			IInputPlugin inputPlugin = GameConsole.Instance.InputPlugin;
			inputPlugin.EnableKeyboardInput();

			// Resume emulation if we paused it for the user.
			if (GameConsole.Instance.State == ConsoleState.Paused && !this.isPausedBeforeInactive)
				this.DoConsoleTogglePause();
		}

		protected override void WndProc(ref Message m)
		{
			this.sizeGuard.WatchForResize(ref m);
			base.WndProc(ref m);
		}

		private void MainMenuStrip_MenuActivate(object sender, EventArgs e)
		{
			this.UpdateUI();
		}

		private void RomNagBox_CloseClicked(object sender, EventArgs e)
		{
			this.DoHideRomNag();
		}

		private void RomNagBox_LinkClicked(object sender, EventArgs e)
		{
			this.DoChooseBiosRom1();
		}

		private void chooseBiosRom1MenuItem_Click(object sender, EventArgs e)
		{
			this.DoChooseBiosRom1();
		}

		private void chooseBiosRom2MenuItem_Click(object sender, EventArgs e)
		{
			this.DoChooseBiosRom2();
		}

		private void openCDImageMenuItem_Click(object sender, EventArgs e)
		{
			this.DoChooseGameRom();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void RefreshFpsTimer_Tick(object sender, EventArgs e)
		{
			this.DoUpdateStatus();
		}

		private void saveStateMenuItem_Click(object sender, EventArgs e)
		{
			this.DoSaveState();
		}

		private void loadStateMenuItem_Click(object sender, EventArgs e)
		{
			this.DoLoadState();
		}

		private void previousSlotMenuItem_Click(object sender, EventArgs e)
		{
			this.DoAdvanceSaveSlot(false);
		}

		private void nextSlotMenuItem_Click(object sender, EventArgs e)
		{
			this.DoAdvanceSaveSlot(true);
		}

		void saveSlotMenuItem_Click(object sender, EventArgs e)
		{
			Properties.Settings.Default.SaveStateSlot = (int)((ToolStripMenuItem)sender).Tag;
			Properties.Settings.Default.Save();
		}

		private void loadLastGameMenuItem_Click(object sender, EventArgs e)
		{
			Properties.Settings.Default.AutoOpenGameFile = !Properties.Settings.Default.AutoOpenGameFile;
			Properties.Settings.Default.Save();
		}

		private void loadLastSaveMenuItem_Click(object sender, EventArgs e)
		{
			Properties.Settings.Default.AutoLoadLastSave = !Properties.Settings.Default.AutoLoadLastSave;
			Properties.Settings.Default.Save();
		}

		private void volumeMenuItem_VolumeChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.AudioVolume = volumeMenuItem.Volume;
			Properties.Settings.Default.Save();
		}

		private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.DoShowSettings();
		}

		private void configureInputMenuItem_Click(object sender, EventArgs e)
		{
			this.DoShowConfigureInput();
		}

		private void fullScreenMenuItem_Click(object sender, EventArgs e)
		{
			this.DoToggleFullScreen();
		}

		private void snapWindowMenuItem_Click(object sender, EventArgs e)
		{
			this.DoToggleSnapWindow();
		}

		private void preserveRatioMenuItem_Click(object sender, EventArgs e)
		{
			this.DoTogglePreserveRatio();
		}

		private void smoothResizingMenuItem_Click(object sender, EventArgs e)
		{
			this.DoToggleImageSmoothing();
		}

		private void scalingModeNoneMenuItem_Click(object sender, EventArgs e)
		{
			this.DoSetScalingMode(ScalingAlgorithm.None, false);
		}

		private void scalingModeDoubleResMenuItem_Click(object sender, EventArgs e)
		{
			this.DoSetScalingMode(ScalingAlgorithm.None, true);
		}

		private void scalingModeHq2xMenuItem_Click(object sender, EventArgs e)
		{
			this.DoSetScalingMode(ScalingAlgorithm.Hq2X, false);
		}

		private void scalingModeHq3xMenuItem_Click(object sender, EventArgs e)
		{
			this.DoSetScalingMode(ScalingAlgorithm.Hq3X, false);
		}

		private void scalingModeHq4xMenuItem_Click(object sender, EventArgs e)
		{
			this.DoSetScalingMode(ScalingAlgorithm.Hq4X , false);
		}

		private void autoCropMenuItem_Click(object sender, EventArgs e)
		{
			this.DoToggleAutoCrop();
		}

		private void DrawBorderMenuItem_Click(object sender, EventArgs e)
		{
			this.DoToggleVoidAreaBorder();
		}

		private void languageDefaultMenuItem_Click(object sender, EventArgs e)
		{
			this.DoSetLanguage(null);
		}

		private void languageSelectionMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
			this.DoSetLanguage((string)menuItem.Tag);
		}

		private void Pattern4DOMenuItem_Click(object sender, EventArgs e)
		{
			if (sender is ToolStripDropDownItem && ((ToolStripDropDownItem)sender).Tag is ToolStripDropDownItem)
				sender = ((ToolStripDropDownItem)sender).Tag;
			this.DoSetVoidAreaPattern((VoidAreaPattern)((ToolStripMenuItem)sender).Tag);
		}

		private void PatternBumpsMenuItem_Click(object sender, EventArgs e)
		{
			if (sender is ToolStripDropDownItem && ((ToolStripDropDownItem)sender).Tag is ToolStripDropDownItem)
				sender = ((ToolStripDropDownItem)sender).Tag;
			this.DoSetVoidAreaPattern((VoidAreaPattern)((ToolStripMenuItem)sender).Tag);
		}

		private void PatternMetalMenuItem_Click(object sender, EventArgs e)
		{
			if (sender is ToolStripDropDownItem && ((ToolStripDropDownItem)sender).Tag is ToolStripDropDownItem)
				sender = ((ToolStripDropDownItem)sender).Tag;
			this.DoSetVoidAreaPattern((VoidAreaPattern)((ToolStripMenuItem)sender).Tag);
		}

		private void PatternNoneMenuItem_Click(object sender, EventArgs e)
		{
			if (sender is ToolStripDropDownItem && ((ToolStripDropDownItem)sender).Tag is ToolStripDropDownItem)
				sender = ((ToolStripDropDownItem)sender).Tag;
			this.DoSetVoidAreaPattern((VoidAreaPattern)((ToolStripMenuItem)sender).Tag);
		}

		private void Main_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter && e.Modifiers == Keys.Alt)
			{
				this.gameCanvas.Focus(); // Helps when spamming alt+enter, but doesn't completely fix the damn menu getting keyboard focus.
				this.DoToggleFullScreen();
			}

			if (e.KeyCode == Keys.Escape && this.isWindowFullScreen)
			{
				this.DoToggleFullScreen();
			}
		}

		void mouseHook_MouseMove(object sender, MouseHookEventArgs e)
		{
			if (this.isWindowFullScreen == false)
				return;

			Screen currentScreen = DisplayHelper.GetCurrentScreen(this);
			if ((e.Y - currentScreen.WorkingArea.Top) < (this.MainMenuBar.Height))
			{
				this.MainMenuBar.Visible = true;
				this.hideMenuTimer.Enabled = false;
				this.hideMenuTimer.Enabled = true;
			}
		}

		void mouseHook_MouseDown(object sender, MouseHookEventArgs e)
		{
			if (this.isWindowFullScreen == false)
				return;

			Screen currentScreen = DisplayHelper.GetCurrentScreen(this);
			if ((e.Y - currentScreen.WorkingArea.Top) >= (this.MainMenuBar.Height))
				this.MainMenuBar.Visible = (this.isWindowFullScreen == false);
		}

		private void hideMenuTimer_Tick(object sender, EventArgs e)
		{
			// Delay hiding the menu if the user is using the menus.
			if (this.MainMenuBar.Focused || this.MainMenuBar.Capture || this.MainMenuBar.ContainsFocus)
				return;

			this.MainMenuBar.Visible = (this.isWindowFullScreen == false);
			this.hideMenuTimer.Enabled = false;
		}

		private void checkInputTimer_Tick(object sender, EventArgs e)
		{
			GameConsole.Instance.InputPlugin.CheckConsoleEvents();
		}

		void InputPlugin_ConsoleEventRaised(Emulation.Plugins.Input.ConsoleEvent consoleEvent)
		{
			if (consoleEvent == ConsoleEvent.StateSave)
				this.DoSaveState();
			else if (consoleEvent == ConsoleEvent.StateLoad)
				this.DoLoadState();
			else if (consoleEvent == ConsoleEvent.StateSlotPrevious)
				this.DoAdvanceSaveSlot(false);
			else if (consoleEvent == ConsoleEvent.StateSlotNext)
				this.DoAdvanceSaveSlot(true);
			else if (consoleEvent == ConsoleEvent.FullScreen)
				this.DoToggleFullScreen();
			else if (consoleEvent == ConsoleEvent.ScreenShot)
				this.DoScreenShot();
			else if (consoleEvent == ConsoleEvent.Pause)
				this.DoConsoleTogglePause();
			else if (consoleEvent == ConsoleEvent.AdvanceBySingleFrame)
				this.DoConsoleAdvanceFrame();
			else if (consoleEvent == ConsoleEvent.Reset)
				this.DoConsoleReset(false);
			else if (consoleEvent == ConsoleEvent.Exit)
				this.Close();
		}

		private void gameInfoMenuItem_Click(object sender, EventArgs e)
		{
			IGameSource gameSource = GameConsole.Instance.GameSource;
			if (gameSource == null || gameSource is BiosOnlyGameSource)
				return;

			using (var gameInformation = new GameInformation())
			{
				gameInformation.GameSource = gameSource;
				gameInformation.ShowDialog(this);
			}
		}

		private void aboutMenuItem_Click(object sender, EventArgs e)
		{
			using (var aboutForm = new About())
			{
				aboutForm.ShowDialog(this);
			}
		}

		private void resetMenuItem_Click(object sender, EventArgs e)
		{
			this.DoConsoleReset(false);
		}

		private void screenshotMenuItem_Click(object sender, EventArgs e)
		{
			this.DoScreenShot();
		}

		private void pauseMenuItem_Click(object sender, EventArgs e)
		{
			this.DoConsoleTogglePause();
		}

		private void advanceFrameMenuItem_Click(object sender, EventArgs e)
		{
			this.DoConsoleAdvanceFrame();
		}

		private void Instance_ConsoleStateChange(ConsoleStateChangeEventArgs e)
		{
			if (e.NewState == ConsoleState.Running)
				Utilities.DisplayHelper.DisableScreenSaver();
			else
				Utilities.DisplayHelper.EnableScreenSaver();

			// Some menu items depend on console status.
			this.UpdateUI();
		}

		private void openFromDriveMenuItem_Click(object sender, EventArgs e)
		{
			this.DoOpenDiscDrive((char)((ToolStripMenuItem)sender).Tag);
		}

		private void closeGameMenuItem_Click(object sender, EventArgs e)
		{
			this.DoCloseGame();
		}

		private void Main_ResizeBegin(object sender, EventArgs e)
		{
			this.gameCanvas.IsInResizeMode = true;
		}

		private void Main_ResizeEnd(object sender, EventArgs e)
		{
			this.gameCanvas.IsInResizeMode = false;
		}

		private void DiscBrowserMenuItem_Click(object sender, EventArgs e)
		{
			IGameSource source = GameConsole.Instance.GameSource;
			if (source == null || source is BiosOnlyGameSource)
				return;

			using (var form = new Browser())
			{
				form.GameSource = source;
				form.ShowDialog(this);
			}
		}

		#endregion // Event Handlers

		#region Private Methods

		private void UpdateUI()
		{
			bool isValidBiosRomSelected = (string.IsNullOrEmpty(Properties.Settings.Default.BiosRomFile) == false);
			bool consoleActive = (GameConsole.Instance.State != ConsoleState.Stopped);
			bool biosOnly = (GameConsole.Instance.GameSource is BiosOnlyGameSource);

			// Title bar.
			string windowTitle = null;
			if (consoleActive && !biosOnly)
			{
				string gameName = GameConsole.Instance.GameSource.GetGameName();
				if (gameName == null)
					gameName = Strings.MainMessageUnknownGame;

				windowTitle += gameName + " - ";
			}
			windowTitle += FOURDO_NAME;

			this.Text = windowTitle.ToString();

			////////////////////////
			// CoreFile menu

			this.closeGameMenuItem.Enabled = consoleActive && !biosOnly;
			this.openCDImageMenuItem.Enabled = isValidBiosRomSelected;
			this.loadLastGameMenuItem.Enabled = true;
			this.chooseBiosRom1MenuItem.Enabled = true;
			this.chooseBiosRom2MenuItem.Enabled = true;
			this.exitMenuItem.Enabled = true;

			string biosRom1Name = Properties.Settings.Default.BiosRomFile;
			if (!string.IsNullOrWhiteSpace(biosRom1Name))
				biosRom1Name = Path.GetFileName(biosRom1Name);

			string biosRom2Name = Properties.Settings.Default.BiosRom2File;
			if (!string.IsNullOrWhiteSpace(biosRom2Name))
				biosRom2Name = Path.GetFileName(biosRom2Name);

			this.chooseBiosRom1MenuItem.Text = string.Format(Strings.MainMenuFileChooseBiosRom1, biosRom1Name);
			this.chooseBiosRom2MenuItem.Text = string.Format(Strings.MainMenuFileChooseBiosRom2, biosRom2Name);

			this.loadLastGameMenuItem.Checked = Properties.Settings.Default.AutoOpenGameFile;

			// Find the menu item that matches the currently loaded game.
			ToolStripMenuItem currentOpenMenu = null;
			if (Properties.Settings.Default.GameRomSourceType == (int)GameSourceType.File)
				currentOpenMenu = openCDImageMenuItem;
			else if (Properties.Settings.Default.GameRomSourceType == (int)GameSourceType.Disc)
			{
				foreach (ToolStripMenuItem item in this.openGameMenuItems)
				{
					if ((char?)item.Tag == Properties.Settings.Default.GameRomDrive)
					{
						currentOpenMenu = (ToolStripMenuItem)item;
						break;
					}
				}
			}

			// Accentuate the menu item identifying the currently open item.
			foreach (ToolStripMenuItem item in this.openGameMenuItems)
			{
				bool currentItem = (item == currentOpenMenu);
				item.Font = currentItem ? new Font(fileMenuItem.Font, FontStyle.Italic) : fileMenuItem.Font;
				item.Checked = currentItem;
				item.Enabled = isValidBiosRomSelected;
			}

			////////////////////////
			// Console menu

			this.saveStateMenuItem.Enabled = consoleActive;
			this.loadStateMenuItem.Enabled = consoleActive;
			this.loadLastSaveMenuItem.Enabled = true;
			this.saveStateSlotMenuItem.Enabled = true;
			foreach (ToolStripItem menuItem in this.saveStateSlotMenuItem.DropDownItems)
				menuItem.Enabled = true;
			this.screenshotMenuItem.Enabled = consoleActive;
			this.pauseMenuItem.Enabled = consoleActive;
			this.advanceFrameMenuItem.Enabled = consoleActive;
			this.resetMenuItem.Enabled = consoleActive;

			this.pauseMenuItem.Checked = (GameConsole.Instance.State == ConsoleState.Paused);
			this.loadLastSaveMenuItem.Checked = Properties.Settings.Default.AutoLoadLastSave;

			// Save slot
			foreach (ToolStripItem menuItem in saveStateSlotMenuItem.DropDownItems)
			{
				if (!(menuItem is ToolStripMenuItem))
					continue;

				if (menuItem.Tag != null)
					((ToolStripMenuItem)menuItem).Checked = (Properties.Settings.Default.SaveStateSlot == (int)menuItem.Tag);
			}

			////////////////////////
			// Display menus. (always enabled)

			this.fullScreenMenuItem.Checked = Properties.Settings.Default.WindowFullScreen;

			this.DrawBorderMenuItem.Checked = Properties.Settings.Default.VoidAreaBorder;

			// Scaling algorithm.
			var italicFont = new Font(scalingModeMenuItem.Font, FontStyle.Italic);
			ToolStripMenuItem currentAlgorithmItem = null;
			if (Properties.Settings.Default.RenderHighResolution)
				currentAlgorithmItem = this.scalingModeDoubleResMenuItem;
			else if (Properties.Settings.Default.WindowScalingAlgorithm == (int)ScalingAlgorithm.Hq4X)
				currentAlgorithmItem = this.scalingModeHq4xMenuItem;
			else if (Properties.Settings.Default.WindowScalingAlgorithm == (int)ScalingAlgorithm.Hq3X)
				currentAlgorithmItem = this.scalingModeHq3xMenuItem;
			else if (Properties.Settings.Default.WindowScalingAlgorithm == (int)ScalingAlgorithm.Hq2X)
				currentAlgorithmItem = this.scalingModeHq2xMenuItem;
			else
				currentAlgorithmItem = this.scalingModeNoneMenuItem;

			foreach (ToolStripItem menuItem in scalingModeMenuItem.DropDownItems)
			{
				if (!(menuItem is ToolStripMenuItem))
					continue;
				bool isCurrent = (menuItem == currentAlgorithmItem);
				((ToolStripMenuItem)menuItem).Checked = isCurrent;
				menuItem.Font = isCurrent ? new Font(scalingModeMenuItem.Font, FontStyle.Italic) : scalingModeMenuItem.Font;
			}

			this.smoothResizingMenuItem.Checked = Properties.Settings.Default.WindowImageSmoothing;
			this.gameCanvas.ImageSmoothing = this.smoothResizingMenuItem.Checked;

			this.snapWindowMenuItem.Checked = Properties.Settings.Default.WindowSnapSize;
			this.sizeGuard.Enabled = this.snapWindowMenuItem.Checked && (Properties.Settings.Default.WindowFullScreen == false);

			this.autoCropMenuItem.Checked = Properties.Settings.Default.WindowAutoCrop;
			this.gameCanvas.AutoCrop = this.autoCropMenuItem.Checked;

			this.preserveRatioMenuItem.Checked = Properties.Settings.Default.WindowPreseveRatio;
			this.gameCanvas.PreserveAspectRatio = this.preserveRatioMenuItem.Checked;

			// Individual void area patterns.
			bool rightPattern;

			rightPattern = ((int)Properties.Settings.Default.VoidAreaPattern == (int)this.Pattern4DOMenuItem.Tag);
			this.Pattern4DOMenuItem.Checked = rightPattern;
			this.Pattern4DOMenuItem.Font = new Font(this.Pattern4DOMenuItem.Font, rightPattern ? FontStyle.Italic : FontStyle.Regular);

			rightPattern = ((int)Properties.Settings.Default.VoidAreaPattern == (int)this.PatternBumpsMenuItem.Tag);
			this.PatternBumpsMenuItem.Checked = rightPattern;
			this.PatternBumpsMenuItem.Font = new Font(this.PatternBumpsMenuItem.Font, rightPattern ? FontStyle.Italic : FontStyle.Regular);

			rightPattern = ((int)Properties.Settings.Default.VoidAreaPattern == (int)this.PatternMetalMenuItem.Tag);
			this.PatternMetalMenuItem.Checked = rightPattern;
			this.PatternMetalMenuItem.Font = new Font(this.PatternMetalMenuItem.Font, rightPattern ? FontStyle.Italic : FontStyle.Regular);

			rightPattern = ((int)Properties.Settings.Default.VoidAreaPattern == (int)this.PatternNoneMenuItem.Tag);
			this.PatternNoneMenuItem.Checked = rightPattern;
			this.PatternNoneMenuItem.Font = new Font(this.PatternNoneMenuItem.Font, rightPattern ? FontStyle.Italic : FontStyle.Regular);

			this.gameCanvas.VoidAreaPattern = (VoidAreaPattern)Properties.Settings.Default.VoidAreaPattern;
			this.gameCanvas.VoidAreaBorder = Properties.Settings.Default.VoidAreaBorder;

			////////////////////////
			// Audio menus. (almost always enabled)
			IAudioPlugin audioPlugin = GameConsole.Instance.AudioPlugin;
			this.audioMenuItem.Enabled = (audioPlugin != null) && (audioPlugin.GetSupportsVolume() == true);
			if (audioPlugin.GetSupportsVolume())
			{
				this.volumeMenuItem.Volume = Properties.Settings.Default.AudioVolume;
				audioPlugin.Volume = Properties.Settings.Default.AudioVolume;
			}

			////////////////////////
			// Settings menus. (almost always enabled)
			this.settingsMenuItem.Enabled = true;
			this.configureInputMenuItem.Enabled = (GameConsole.Instance.InputPlugin != null)
					&& (GameConsole.Instance.InputPlugin.GetHasSettings());

			string language = Properties.Settings.Default.Language;
			this.languageDefaultMenuItem.Checked = string.IsNullOrWhiteSpace(language);
			foreach (var menuItem in this.languageMenuItems)
				menuItem.Checked = (language == (string)menuItem.Tag);

			////////////////////////
			// Tools menus.
			this.DiscBrowserMenuItem.Enabled = consoleActive && (!biosOnly);

			////////////////////////
			// Help menus.
			this.gameInfoMenuItem.Enabled = consoleActive && (!biosOnly);
			this.aboutMenuItem.Enabled = true;

			////////////////////////
			// Other non-menu stuff.

			// If we need to switch full screen status, do it now.
			if (this.isWindowFullScreen != Properties.Settings.Default.WindowFullScreen)
				this.SetFullScreen(Properties.Settings.Default.WindowFullScreen);

			// Misc form stuff.
			this.sizeGuard.Enabled = Properties.Settings.Default.WindowSnapSize;

			// Hide, but never show the rom nag box in this function. 
			// The rom nag box only makes itself visible on when emulation fails to start due to an invalid bios.
			if (isValidBiosRomSelected == true)
				this.DoHideRomNag();
		}

		#region Console Control

		private void DoConsoleStart(bool alsoAllowLoadState)
		{
			if (string.IsNullOrEmpty(Properties.Settings.Default.BiosRomFile) == true)
				return;

			////////////////
			// Set required settings.
			GameConsole.Instance.AudioBufferMilliseconds = Properties.Settings.Default.AudioBufferMilliseconds;
			GameConsole.Instance.CpuClockHertz = Properties.Settings.Default.CpuClockHertz;
			GameConsole.Instance.RenderHighResolution = Properties.Settings.Default.RenderHighResolution;
			gameCanvas.RenderHighResolution = Properties.Settings.Default.RenderHighResolution;
			gameCanvas.ScalingAlgorithm = (ScalingAlgorithm)Properties.Settings.Default.WindowScalingAlgorithm;

			////////////////
			// Ensure existence of an NVRAM file.
			string nvramFile = SaveHelper.GetNvramFilePath();
			if (System.IO.File.Exists(nvramFile) == false)
			{
				// No NVRAM! Initialize one.
				string directoryName = Path.GetDirectoryName(nvramFile);
				if (!Directory.Exists(directoryName))
					Directory.CreateDirectory(directoryName);
				var nvramStream = new FileStream(nvramFile, FileMode.CreateNew);
				var nvramBytes = new byte[GameConsole.Instance.NvramSize];
				unsafe
				{
					fixed (byte* nvramBytePointer = nvramBytes)
					{
						EmulationHelper.InitializeNvram(new IntPtr((int)nvramBytePointer));
					}
				}
				nvramStream.Write(nvramBytes, 0, nvramBytes.Length);
				nvramStream.Close();
			}

			////////////////
			// Create an appropriate game source
			IGameSource gameSource = this.CreateGameSource();

			////////////////
			try
			{
				GameConsole.Instance.Start(Properties.Settings.Default.BiosRomFile, Properties.Settings.Default.BiosRom2File, gameSource, nvramFile);
			}
			catch (GameConsole.BadBiosRom1Exception)
			{
				FourDO.UI.Error.ShowError(string.Format(Strings.MainErrorLoadBiosFile, Properties.Settings.Default.BiosRomFile));
				Properties.Settings.Default.BiosRomFile = "";
				Properties.Settings.Default.Save();
				this.DoShowRomNag();
			}
			catch (GameConsole.BadBiosRom2Exception)
			{
				FourDO.UI.Error.ShowError(string.Format(Strings.MainErrorLoadBiosFile, Properties.Settings.Default.BiosRom2File));
				Properties.Settings.Default.BiosRom2File = "";
				Properties.Settings.Default.Save();
			}
			catch (GameConsole.BadGameRomException)
			{
				string errorMessage = null;
				if (gameSource is FileGameSource)
					errorMessage = string.Format(Strings.MainErrorLoadGameFile, ((FileGameSource)gameSource).GameFilePath);
				else
					errorMessage = Strings.MainErrorLoadGame;
				FourDO.UI.Error.ShowError(errorMessage);

				// Since it failed to load, we want to un-remember this as the last loaded game.
				Properties.Settings.Default.GameRomSourceType = (int)GameSourceType.None;
				Properties.Settings.Default.Save();
			}
			catch (GameConsole.BadNvramFileException)
			{
				FourDO.UI.Error.ShowError(string.Format(Strings.MainErrorLoadNvramFile, nvramFile));
			}

			// Optionally load state.
			if (alsoAllowLoadState == true)
			{
				if (Properties.Settings.Default.AutoLoadLastSave == true)
					this.DoLoadState();
			}

			this.UpdateUI();
		}

		private IGameSource CreateGameSource()
		{
			// Determine the game rom source type.
			int sourceTypeNumber = Properties.Settings.Default.GameRomSourceType;
			GameSourceType sourceType = GameSourceType.None;
			try
			{
				sourceType = (GameSourceType)sourceTypeNumber;
			}
			catch
			{
				// If there was a problem, assume a type of "none".
				Properties.Settings.Default.GameRomSourceType = (int)GameSourceType.None;
				Properties.Settings.Default.Save();
			}

			if (sourceType == GameSourceType.None)
				return new BiosOnlyGameSource();

			if (sourceType == GameSourceType.File)
				return new FileGameSource(Properties.Settings.Default.GameRomFile);

			if (sourceType == GameSourceType.Disc)
				return new DiscGameSource(Properties.Settings.Default.GameRomDrive);

			// Must be a currently unsupported type.
			return new BiosOnlyGameSource();
		}

		private void DoConsoleReset(bool alsoAllowLoadState)
		{
			this.DoConsoleStop();

			// Restart, but don't allow it to load state.
			this.DoConsoleStart(alsoAllowLoadState);
		}

		private void DoScreenShot()
		{
			string screenshotFilePath = SaveHelper.GetScreenshotFilePath(GameConsole.Instance.GameSource);
			this.gameCanvas.DoScreenshot(screenshotFilePath);
		}

		private void DoConsoleTogglePause()
		{
			if (GameConsole.Instance.State == ConsoleState.Running)
				GameConsole.Instance.Pause();
			else if (GameConsole.Instance.State == ConsoleState.Paused)
				GameConsole.Instance.Resume();
		}

		private void DoConsoleAdvanceFrame()
		{
			if (GameConsole.Instance.State == ConsoleState.Stopped)
				return;

			// Pause it if we need to.
			if (GameConsole.Instance.State == ConsoleState.Running)
				GameConsole.Instance.Pause();

			if (GameConsole.Instance.State == ConsoleState.Paused)
				GameConsole.Instance.AdvanceSingleFrame();
		}

		private void DoConsoleStop()
		{
			GameConsole.Instance.Stop();
		}

		#endregion // Console Control

		private void DoCloseGame()
		{
			Properties.Settings.Default.GameRomSourceType = (int)GameSourceType.None;
			Properties.Settings.Default.Save();
			this.DoConsoleReset(false); // go back to start of bios.
		}

		private void DoShowRomNag()
		{
			RomNagBox.Visible = true;
		}

		private void DoHideRomNag()
		{
			RomNagBox.Visible = false;
		}

		private void DoChooseBiosRom2()
		{
			using (var openDialog = new OpenFileDialog())
			{
				openDialog.InitialDirectory = this.GetLastRomDirectory();
				openDialog.Filter =
					Strings.MainMessageBiosFiles + " (*.rom, *.font)|*.rom;*.font|" +
					Strings.MainMessageAllFiles + " (*.*)|*.*";
				openDialog.RestoreDirectory = true;

				if (openDialog.ShowDialog() == DialogResult.OK)
				{
					Properties.Settings.Default.BiosRom2File = openDialog.FileName;
					Properties.Settings.Default.GameRomLastDirectory = System.IO.Path.GetDirectoryName(openDialog.FileName);
					Properties.Settings.Default.Save();

				}
			}
		}

		private void DoChooseBiosRom1()
		{
			using (var openDialog = new OpenFileDialog())
			{
				openDialog.InitialDirectory = this.GetLastRomDirectory();
				openDialog.Filter =
					Strings.MainMessageBiosFiles + " (*.rom, *.bin)|*.rom;*.bin|" +
					Strings.MainMessageAllFiles + " (*.*)|*.*";
				openDialog.RestoreDirectory = true;

				if (openDialog.ShowDialog() == DialogResult.OK)
				{
					Properties.Settings.Default.BiosRomFile = openDialog.FileName;
					Properties.Settings.Default.GameRomLastDirectory = System.IO.Path.GetDirectoryName(openDialog.FileName);
					Properties.Settings.Default.Save();

					if (GameConsole.Instance.GameSource == null || GameConsole.Instance.GameSource is BiosOnlyGameSource)
					{
						bool allowReset = !(GameConsole.Instance.GameSource is BiosOnlyGameSource);
						this.DoConsoleReset(allowReset);
					}
				}
			}
		}

		private void DoChooseGameRom()
		{
			using (var openDialog = new OpenFileDialog())
			{
				openDialog.InitialDirectory = this.GetLastRomDirectory();
				openDialog.Filter =
					Strings.MainMessageCDImageFiles + " (*.iso, *.bin, *.cue)|*.iso;*.bin;*.cue|" +
					Strings.MainMessageAllFiles + " (*.*)|*.*";
				openDialog.RestoreDirectory = true;

				if (openDialog.ShowDialog() == DialogResult.OK)
				{
					Properties.Settings.Default.GameRomFile = openDialog.FileName;
					Properties.Settings.Default.GameRomSourceType = (int)GameSourceType.File;
					Properties.Settings.Default.GameRomLastDirectory = System.IO.Path.GetDirectoryName(openDialog.FileName);
					Properties.Settings.Default.Save();

					// Start it for them.
					// Some people may want a prompt. However, I never like this. So, screw em.
					this.DoConsoleReset(true);
				}
			}
		}

		private void DoOpenDiscDrive(char driveLetter)
		{
			Properties.Settings.Default.GameRomSourceType = (int)GameSourceType.Disc;
			Properties.Settings.Default.GameRomDrive = driveLetter;
			Properties.Settings.Default.Save();

			// Start it for them. Don't bother prompting, because you're a badass.
			this.DoConsoleReset(true);
		}

		private string GetLastRomDirectory()
		{
			string returnValue = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

			string lastDirectory = Properties.Settings.Default.GameRomLastDirectory;
			if (string.IsNullOrEmpty(lastDirectory) == false)
			{
				if (Directory.Exists(lastDirectory) == true)
				{
					returnValue = lastDirectory;
				}
				else
				{
					Properties.Settings.Default.GameRomLastDirectory = null;
					Properties.Settings.Default.Save();
				}
			}

			return returnValue;
		}

		private void DoSaveState()
		{
			if (GameConsole.Instance.State != ConsoleState.Stopped)
			{
				string saveStateFileName = SaveHelper.GetSaveStateFileName(GameConsole.Instance.GameSource, Properties.Settings.Default.SaveStateSlot);
				GameConsole.Instance.SaveState(saveStateFileName);
			}
		}

		private void DoLoadState()
		{
			if (GameConsole.Instance.State != ConsoleState.Stopped)
			{
				string saveStateFileName = SaveHelper.GetSaveStateFileName(GameConsole.Instance.GameSource, Properties.Settings.Default.SaveStateSlot);
				if (System.IO.File.Exists(saveStateFileName))
					GameConsole.Instance.LoadState(saveStateFileName);
			}
		}

		private void DoAdvanceSaveSlot(bool up)
		{
			int saveSlot = Properties.Settings.Default.SaveStateSlot;
			if (up == true)
				saveSlot++;
			else
				saveSlot--;

			if (saveSlot == -1)
				saveSlot = 9;

			if (saveSlot == 10)
				saveSlot = 0;

			Properties.Settings.Default.SaveStateSlot = saveSlot;
			Properties.Settings.Default.Save();
		}

		private EmulationHealth _lastHealth = EmulationHealth.None;
		private void DoUpdateStatus()
		{
			EmulationHealth health = EmulationHealth.None;

			if (GameConsole.Instance.State == ConsoleState.Running)
			{
				double fps = GameConsole.Instance.CurrentFrameSpeed;
				if (fps != 0)
				{
					fps = 1 / (fps);
					if (fps > 65.00) fps = 60.00 + (fps / 100); //fix fps because it be overflow
				}
				fps = Math.Min(fps, 999.99);
				var fpsString = fps.ToString("00.00");
				var extraPadding = new String(' ', 6 - fpsString.Length);
				FPSStripItem.Text = "   " + Strings.MainMessageCoreFPS + ": " + extraPadding + fpsString;

				health = GameConsole.Instance.CurrentEmulationHealth;
			}
			else
			{
				FPSStripItem.Text = "   " + Strings.MainMessageCoreFPS + ": ---.--";
				HealthStripItem.Image = Properties.Images.light_gray;
			}

			if (health != _lastHealth)
			{
				_lastHealth = health;
				if (health == EmulationHealth.None)
					HealthStripItem.Image = Properties.Images.light_gray;
				else if (health == EmulationHealth.Bad)
					HealthStripItem.Image = Properties.Images.light_red;
				else if (health == EmulationHealth.Okay)
					HealthStripItem.Image = Properties.Images.light_yellow;
				else
					HealthStripItem.Image = Properties.Images.light_green;
			}
		}

		private void DoShowSettings()
		{
			using (var settingsForm = new Settings())
			{
				settingsForm.ShowDialog(this);
			}
		}

		private void DoShowConfigureInput()
		{
			IInputPlugin plugin = GameConsole.Instance.InputPlugin;
			if (plugin != null && plugin.GetHasSettings() == true)
				plugin.ShowSettings(this);
		}

		private void DoToggleFullScreen()
		{
			Properties.Settings.Default.WindowFullScreen = !Properties.Settings.Default.WindowFullScreen;
			Properties.Settings.Default.Save();
		}

		private void DoToggleSnapWindow()
		{
			Properties.Settings.Default.WindowSnapSize = !Properties.Settings.Default.WindowSnapSize;
			Properties.Settings.Default.Save();
		}

		private void DoTogglePreserveRatio()
		{
			Properties.Settings.Default.WindowPreseveRatio = !Properties.Settings.Default.WindowPreseveRatio;
			Properties.Settings.Default.Save();
		}

		private void DoToggleImageSmoothing()
		{
			Properties.Settings.Default.WindowImageSmoothing = !Properties.Settings.Default.WindowImageSmoothing;
			Properties.Settings.Default.Save();
		}

		private void DoSetScalingMode(ScalingAlgorithm algorithm, bool highResolution)
		{
			Properties.Settings.Default.WindowScalingAlgorithm = (int)algorithm;
			Properties.Settings.Default.RenderHighResolution = highResolution;
			Properties.Settings.Default.Save();
		}

		private void DoToggleAutoCrop()
		{
			Properties.Settings.Default.WindowAutoCrop = !Properties.Settings.Default.WindowAutoCrop;
			Properties.Settings.Default.Save();
		}

		private void DoSaveWindowSize()
		{
			Properties.Settings.Default.WindowWidth = this.Width;
			Properties.Settings.Default.WindowHeight = this.Height;
			Properties.Settings.Default.Save();
		}

		private void DoToggleVoidAreaBorder()
		{
			Properties.Settings.Default.VoidAreaBorder = !Properties.Settings.Default.VoidAreaBorder;
			Properties.Settings.Default.Save();
		}

		private void DoSetVoidAreaPattern(VoidAreaPattern pattern)
		{
			Properties.Settings.Default.VoidAreaPattern = (int)pattern;
			Properties.Settings.Default.Save();
		}

		private void SetFullScreen(bool enableFullScreen)
		{
			// Keep the window from redrawing
			this.SuspendLayout();

			// Change border style (this causes a resize)
			// and set the full screen enabled flag.
			if (enableFullScreen)
			{
				this.isWindowFullScreen = enableFullScreen;
				this.FormBorderStyle = enableFullScreen ? FormBorderStyle.None : FormBorderStyle.Sizable;
			}
			else
			{
				this.FormBorderStyle = enableFullScreen ? FormBorderStyle.None : FormBorderStyle.Sizable;
				this.isWindowFullScreen = enableFullScreen;
			}

			//////////////////
			// Enable full screen
			if (enableFullScreen == true)
			{
				this.maximizedBeforeFullScreen = (this.WindowState == FormWindowState.Maximized);
				if (this.maximizedBeforeFullScreen)
					this.WindowState = FormWindowState.Normal;
				else
					this.pointBeforeFullScreen = new Point(this.Left, this.Top);

				// Find, use, and save the current screen.
				int screenNum;
				Screen currentScreen = Utilities.DisplayHelper.GetCurrentScreen(this, out screenNum);
				Properties.Settings.Default.WindowFullScreenDevice = screenNum;

				// Set form bounds.
				this.Bounds = currentScreen.Bounds;

				// Undock the main menu so it doesn't steal real estate from the game window. 
				this.MainMenuBar.Dock = DockStyle.None;
				this.MainMenuBar.SetBounds(
					this.ClientRectangle.Left,
					this.ClientRectangle.Top,
					this.ClientRectangle.Width,
					this.MainMenuBar.Height);
				this.MainMenuBar.BringToFront();
			}

			//////////////////
			// Otherwise, disable full screen.
			else
			{
				int savedWidth = Properties.Settings.Default.WindowWidth;
				int savedHeight = Properties.Settings.Default.WindowHeight;

				if (this.maximizedBeforeFullScreen)
				{
					// Restore size but not position.
					// Windows will remember this size for us when the user returns from maximized state
					this.Width = savedWidth;
					this.Height = savedHeight;
					this.WindowState = FormWindowState.Maximized;
				}
				else
				{
					this.SetBounds(
						this.pointBeforeFullScreen.X,
						this.pointBeforeFullScreen.Y,
						savedWidth,
						savedHeight);
				}

				this.MainMenuBar.Dock = DockStyle.Top;
				this.MainMenuBar.SendToBack();
			}

			this.MainMenuBar.Visible = (enableFullScreen == false);
			this.MainStatusStrip.Visible = (enableFullScreen == false);
			this.sizeBox.Visible = false;
			this.TopMost = enableFullScreen;

			this.Refresh();
			this.ResumeLayout();
		}

		private void DoSetLanguage(string language)
		{
			// Set current culture to the selection.
			System.Globalization.CultureInfo cultureToUse = null;
			if (string.IsNullOrWhiteSpace(language))
				cultureToUse = FourDO.Utilities.Globals.Constants.SystemDefaultCulture;
			else
				cultureToUse = System.Globalization.CultureInfo.GetCultureInfo(language);
			System.Threading.Thread.CurrentThread.CurrentUICulture = cultureToUse;

			// Save it.
			Properties.Settings.Default.Language = language;
			Properties.Settings.Default.Save();

			// Update interface.
			this.Localize();
			this.UpdateUI();
		}

		#endregion // Private Methods
	}
}
