using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FourDO.Emulation;

namespace FourDO.UI
{
    public partial class Main : Form
    {
        private const int BASE_WIDTH = 320;
        private const int BASE_HEIGHT = 240;

        private SizeGuard sizeGuard = new SizeGuard();

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            // Some basic form updates.
            this.sizeBox.BaseWidth = BASE_WIDTH;
            this.sizeBox.BaseHeight = BASE_HEIGHT;
            
            this.sizeGuard.BaseWidth = BASE_WIDTH;
            this.sizeGuard.BaseHeight = BASE_HEIGHT;
            this.sizeGuard.WindowExtraWidth = this.Width - this.ClientSize.Width;
            this.sizeGuard.WindowExtraHeight = (this.Height - this.ClientSize.Height) + this.MainMenuBar.Height + this.MainStatusStrip.Height;

            // Initial form size.
            int savedWidth = Properties.Settings.Default.WindowWidth;
            int savedHeight = Properties.Settings.Default.WindowHeight;
            this.Width = (savedWidth > 0) ? savedWidth : this.sizeGuard.BaseWidth * 2 + this.sizeGuard.WindowExtraWidth;
            this.Height = (savedHeight > 0) ? savedHeight : this.sizeGuard.BaseHeight * 2 + this.sizeGuard.WindowExtraHeight;
            if (savedWidth > 0 || savedHeight > 0)
                this.DoSaveWindowSize();
            this.sizeBox.Visible = false; // Shut that damn thing up.

            // Copy some menu items to the quick display settings menu.
            foreach (ToolStripItem item in this.displayMenuItem.DropDownItems)
            {
                ToolStripItem newItem = null;
                if (item is ToolStripSeparator)
                    newItem = new ToolStripSeparator();
                else if (item is ToolStripMenuItem)
                {
                    newItem = new ToolStripMenuItem();
                    newItem.Text = item.Text;

                    if (item == fullScreenMenuItem)
                        newItem.Click += new EventHandler(this.fullScreenMenuItem_Click);

                    if (item == snapWindowMenuItem)
                        newItem.Click += new EventHandler(this.snapWindowMenuItem_Click);

                    if (item == preserveRatioMenuItem)
                        newItem.Click += new EventHandler(this.preserveRatioMenuItem_Click);
                }
                newItem.Tag = item;

                this.quickDisplayDropDownButton.DropDownItems.Add(newItem);
            }

            /////////////
            // Handle ROM file nag box
            if (!File.Exists(Properties.Settings.Default.BiosRomFile))
            {
                Properties.Settings.Default.BiosRomFile = "";
                Properties.Settings.Default.Save();
                this.DoShowRomNag();
            }

            //////////////
            // Add save slot menu items.
            if (Properties.Settings.Default.SaveStateSlot < 0 || Properties.Settings.Default.SaveStateSlot > 9)
            {
                Properties.Settings.Default.SaveStateSlot = 0;
                Properties.Settings.Default.Save();
            }

            for (int x = 0; x < 10; x++)
            {
                ToolStripMenuItem newItem = new ToolStripMenuItem("Slot " + x.ToString());
                newItem.Name = "saveSlotMenuItem" + x.ToString();
                newItem.Tag = x;
                newItem.Click += new EventHandler(saveSlotMenuItem_Click);
                saveStateSlotMenuItem.DropDownItems.Add(newItem);
            }

            // Clear the last loaded game if they don't want us to automatically loading games.
            if (Properties.Settings.Default.AutoLoadGameFile == false)
            {
                Properties.Settings.Default.GameRomFile = null;
                Properties.Settings.Default.Save();
            }
            
            //////////////
            Properties.Settings.Default.PropertyChanged += new PropertyChangedEventHandler(Settings_PropertyChanged);

            this.DoConsoleStart();

            if (Properties.Settings.Default.AutoLoadLastSave)
                this.DoLoadState();

            this.UpdateUI();
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            GameConsole.Instance.Stop();
            GameConsole.Instance.Destroy();
        }

        #region Event Handlers

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // This is here in case some option box in the future updates the setting for us.
            // We hide ourselves here in order to keep ourselves from looking like a liar.
            //
            // It would be a bad idea to add something like a prompt to restart here, since
            // this setting could presumably be changed anywhere. We'll leave prompting the
            // user to the event handlers.
            string RomFileProperty = Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.BiosRomFile);
            if (e.PropertyName == RomFileProperty)
            {
                if (string.IsNullOrEmpty(Properties.Settings.Default.BiosRomFile) == false)
                    this.DoHideRomNag();
            }

            if (e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.BiosRomFile)
                    || e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.GameRomFile)
                    || e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.AutoLoadGameFile)
                    || e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.AutoLoadLastSave)
                    || e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.SaveStateSlot)
                    || e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.WindowFullScreen)
                    || e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.WindowPreseveRatio)
                    || e.PropertyName == Utilities.Reflection.GetPropertyName(() => Properties.Settings.Default.WindowSnapSize))
            {
                this.UpdateUI();
            }
        }

        private void Main_Resize(object sender, EventArgs e)
        {
            sizeBox.UpdateSizeText(gameCanvas.Width, gameCanvas.Height);
            sizeBox.SetBounds(
                    this.ClientSize.Width - 6 - this.sizeBox.PreferredSize.Width,
                    this.ClientSize.Height - 28 - this.sizeBox.PreferredSize.Height,
                    this.sizeBox.PreferredSize.Width,
                    this.sizeBox.PreferredSize.Height);
            this.DoSaveWindowSize();
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
            this.DoChooseBiosRom();
        }

        private void chooseBiosRomMenuItem_Click(object sender, EventArgs e)
        {
            this.DoChooseBiosRom();
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
            this.DoUpdateFPS();
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
            Properties.Settings.Default.AutoLoadGameFile = !Properties.Settings.Default.AutoLoadGameFile;
            Properties.Settings.Default.Save();
        }

        private void loadLastSaveMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoLoadLastSave = !Properties.Settings.Default.AutoLoadLastSave;
            Properties.Settings.Default.Save();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DoShowSettings();
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

        #endregion // Event Handlers

        #region Private Methods

        private void UpdateUI()
        {
            bool isValidBiosRomSelected = (string.IsNullOrEmpty(Properties.Settings.Default.BiosRomFile) == false);
            bool consoleRunning = GameConsole.Instance.Running;

            ////////////////////////
            // File menu

            this.openCDImageMenuItem.Enabled = isValidBiosRomSelected;
            this.loadLastGameMenuItem.Enabled = true;
            this.saveStateMenuItem.Enabled = isValidBiosRomSelected && consoleRunning;
            this.loadStateMenuItem.Enabled = isValidBiosRomSelected && consoleRunning;
            this.saveStateSlotMenuItem.Enabled = true;
            foreach (ToolStripItem menuItem in this.saveStateSlotMenuItem.DropDownItems)
                menuItem.Enabled = true;
            this.loadLastSaveMenuItem.Enabled = true;
            this.chooseBiosRomMenuItem.Enabled = true;
            this.exitMenuItem.Enabled = true;

            // Various Checked/Unchecked
            loadLastGameMenuItem.Checked = Properties.Settings.Default.AutoLoadGameFile;
            loadLastSaveMenuItem.Checked = Properties.Settings.Default.AutoLoadLastSave;

            ////////////////////////
            // Display menus. (always enabled)

            this.fullScreenMenuItem.Checked = Properties.Settings.Default.WindowFullScreen;
            this.GetQuickDisplayMenuItem(fullScreenMenuItem).Checked = fullScreenMenuItem.Checked;

            this.snapWindowMenuItem.Checked = Properties.Settings.Default.WindowSnapSize;
            this.GetQuickDisplayMenuItem(snapWindowMenuItem).Checked = snapWindowMenuItem.Checked;
            this.sizeGuard.Enabled = this.snapWindowMenuItem.Checked;

            this.preserveRatioMenuItem.Checked = Properties.Settings.Default.WindowPreseveRatio;
            this.GetQuickDisplayMenuItem(preserveRatioMenuItem).Checked = preserveRatioMenuItem.Checked;
            this.gameCanvas.PreserveAspectRatio = this.preserveRatioMenuItem.Checked;
            
            // Save slot
            foreach (ToolStripItem menuItem in saveStateSlotMenuItem.DropDownItems)
            {
                if (!(menuItem is ToolStripMenuItem))
                    continue;

                if (menuItem.Tag != null)
                    ((ToolStripMenuItem)menuItem).Checked = (Properties.Settings.Default.SaveStateSlot == (int)menuItem.Tag);
            }

            // Misc form stuff.
            this.sizeGuard.Enabled = Properties.Settings.Default.WindowSnapSize;
            
            // Hide, but never show the rom nag box in this function. 
            // The rom nag box only makes itself visible on when emulation fails to start due to an invalid bios.
            if (isValidBiosRomSelected == true)
                this.DoHideRomNag();
        }

        private void DoConsoleStart()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.BiosRomFile) == true)
                return;

            ////////////////
            try
            {
                GameConsole.Instance.Start(Properties.Settings.Default.BiosRomFile, Properties.Settings.Default.GameRomFile);
            }
            catch (GameConsole.BadBiosRomException)
            {
                Utilities.Error.ShowError(string.Format("The selected bios file ({0}) was either missing or corrupt. Please choose another.", Properties.Settings.Default.BiosRomFile));
                Properties.Settings.Default.BiosRomFile = ""; // Changing this value will update the UI.
                Properties.Settings.Default.Save();
                this.DoShowRomNag();
            }
            catch (GameConsole.BadGameRomException)
            {
                Utilities.Error.ShowError(string.Format("The selected game file ({0}) was either missing or corrupt. Please choose another.", Properties.Settings.Default.GameRomFile));
                Properties.Settings.Default.GameRomFile = ""; // Changing this value will update the UI.
                Properties.Settings.Default.Save();
                this.DoShowRomNag();
            }

            this.UpdateUI();
        }

        private void DoConsoleReset()
        {
            GameConsole.Instance.Stop();
            this.DoConsoleStart();
        }
        
        private void DoShowRomNag()
        {
            RomNagBox.Visible = true;
        }

        private void DoHideRomNag()
        {
            RomNagBox.Visible = false;
        }

        private void DoChooseBiosRom()
        {
            OpenFileDialog openDialog = new OpenFileDialog();

            openDialog.InitialDirectory = Environment.CurrentDirectory;
            openDialog.Filter = "rom files (*.rom)|*.rom|All files (*.*)|*.*";
            openDialog.RestoreDirectory = true;

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.BiosRomFile = openDialog.FileName;
                Properties.Settings.Default.Save();

                this.DoConsoleStart();
            }
        }

        private void DoChooseGameRom()
        {
            OpenFileDialog openDialog = new OpenFileDialog();

            openDialog.InitialDirectory = Environment.CurrentDirectory;
            openDialog.Filter = "iso files (*.iso)|*.iso|All files (*.*)|*.*";
            openDialog.RestoreDirectory = true;

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.GameRomFile = openDialog.FileName;
                Properties.Settings.Default.Save();

                // Start it for them.
                // TODO: Some people may want a prompt. However, I never like this. It's a toss up.
                this.DoConsoleReset();
            }
        }

        private void DoSaveState()
        {
            if (GameConsole.Instance.Running)
            {
                string saveStateFileName = this.GetSaveStateFileName(GameConsole.Instance.GameRomFileName, Properties.Settings.Default.SaveStateSlot);
                GameConsole.Instance.SaveState(saveStateFileName);
            }
        }

        private void DoLoadState()
        {
            if (GameConsole.Instance.Running)
            {
                string saveStateFileName = this.GetSaveStateFileName(GameConsole.Instance.GameRomFileName, Properties.Settings.Default.SaveStateSlot);
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

        private string GetSaveStateFileName(string gameRomFileName, int saveStateSlot)
        {
            const string SAVE_STATE_EXTENSION = ".4dosav";
            return (gameRomFileName + "." + saveStateSlot.ToString() + SAVE_STATE_EXTENSION);
        }

        private void DoUpdateFPS()
        {
            if (GameConsole.Instance.Running)
            {
                double fps = GameConsole.Instance.CurrentFrameSpeed;
                if (fps != 0)
                {
                    fps = 1 / (fps);
                }
                if (fps > 999.99)
                {
                    fps = 999.99;
                }
                FPSStripItem.Text = string.Format("Core FPS: {0:000.00}", fps);
            }
            else
            {
                FPSStripItem.Text = "Core FPS: ---.--";
            }
        }

        private void DoShowSettings()
        {
            Settings settingsForm = new Settings();
            settingsForm.ShowDialog(this);
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

        private void DoSaveWindowSize()
        {
            Properties.Settings.Default.WindowWidth = this.Width;
            Properties.Settings.Default.WindowHeight = this.Height;
            Properties.Settings.Default.Save();
        }

        private ToolStripMenuItem GetQuickDisplayMenuItem(ToolStripMenuItem displayMenuItem)
        {
            foreach (ToolStripItem item in quickDisplayDropDownButton.DropDownItems)
            {
                if (item.Tag == displayMenuItem)
                    return (ToolStripMenuItem)item;
            }
            return null;
        }

        #endregion // Private Methods
    }
}
