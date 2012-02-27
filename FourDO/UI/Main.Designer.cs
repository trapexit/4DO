namespace FourDO.UI
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
			this.MainStatusStrip = new System.Windows.Forms.StatusStrip();
			this.VersionStripItem = new System.Windows.Forms.ToolStripStatusLabel();
			this.FPSStripItem = new System.Windows.Forms.ToolStripStatusLabel();
			this.quickDisplayDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
			this.MainMenuBar = new System.Windows.Forms.MenuStrip();
			this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.closeGameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
			this.openCDImageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadLastGameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.chooseBiosRomMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.consoleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveStateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadStateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadLastSaveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.saveStateSlotMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.previousSlotMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.nextSlotMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.pauseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.advanceFrameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.resetMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.displayMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.fullScreenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.VoidAreaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.DrawBorderMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
			this.Pattern4DOMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.PatternBumpsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.PatternMetalMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.PatternNoneMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
			this.smoothResizingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.preserveRatioMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.snapWindowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.audioMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.settingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
			this.configureInputMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
			this.languageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.languageDefaultMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
			this.languageEnglishMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.languageRussianMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.languageFrenchMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.gameInfoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
			this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.refreshFpsTimer = new System.Windows.Forms.Timer(this.components);
			this.hideMenuTimer = new System.Windows.Forms.Timer(this.components);
			this.sizeBox = new FourDO.UI.SizeBox();
			this.RomNagBox = new FourDO.UI.NagBox();
			this.gameCanvas = new FourDO.UI.GameCanvas();
			this.MainStatusStrip.SuspendLayout();
			this.MainMenuBar.SuspendLayout();
			this.SuspendLayout();
			// 
			// MainStatusStrip
			// 
			this.MainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.VersionStripItem,
            this.FPSStripItem,
            this.quickDisplayDropDownButton});
			this.MainStatusStrip.Location = new System.Drawing.Point(0, 581);
			this.MainStatusStrip.Name = "MainStatusStrip";
			this.MainStatusStrip.Size = new System.Drawing.Size(744, 22);
			this.MainStatusStrip.TabIndex = 0;
			// 
			// VersionStripItem
			// 
			this.VersionStripItem.Name = "VersionStripItem";
			this.VersionStripItem.Size = new System.Drawing.Size(67, 17);
			this.VersionStripItem.Text = "4DO x.x.x.x";
			// 
			// FPSStripItem
			// 
			this.FPSStripItem.Name = "FPSStripItem";
			this.FPSStripItem.Size = new System.Drawing.Size(552, 17);
			this.FPSStripItem.Spring = true;
			// 
			// quickDisplayDropDownButton
			// 
			this.quickDisplayDropDownButton.BackColor = System.Drawing.SystemColors.Window;
			this.quickDisplayDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("quickDisplayDropDownButton.Image")));
			this.quickDisplayDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.quickDisplayDropDownButton.Name = "quickDisplayDropDownButton";
			this.quickDisplayDropDownButton.Size = new System.Drawing.Size(110, 20);
			this.quickDisplayDropDownButton.Text = "Display Options";
			// 
			// MainMenuBar
			// 
			this.MainMenuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.consoleMenuItem,
            this.displayMenuItem,
            this.audioMenuItem,
            this.optionsMenuItem,
            this.helpToolStripMenuItem});
			this.MainMenuBar.Location = new System.Drawing.Point(0, 0);
			this.MainMenuBar.Name = "MainMenuBar";
			this.MainMenuBar.Size = new System.Drawing.Size(744, 24);
			this.MainMenuBar.TabIndex = 2;
			this.MainMenuBar.Text = "menuStrip1";
			this.MainMenuBar.MenuActivate += new System.EventHandler(this.MainMenuStrip_MenuActivate);
			// 
			// fileMenuItem
			// 
			this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeGameMenuItem,
            this.toolStripSeparator8,
            this.openCDImageMenuItem,
            this.loadLastGameMenuItem,
            this.toolStripSeparator1,
            this.chooseBiosRomMenuItem,
            this.toolStripSeparator2,
            this.exitMenuItem});
			this.fileMenuItem.Name = "fileMenuItem";
			this.fileMenuItem.Size = new System.Drawing.Size(35, 20);
			this.fileMenuItem.Text = "&File";
			// 
			// closeGameMenuItem
			// 
			this.closeGameMenuItem.Name = "closeGameMenuItem";
			this.closeGameMenuItem.Size = new System.Drawing.Size(227, 22);
			this.closeGameMenuItem.Text = "&Close Game";
			this.closeGameMenuItem.Click += new System.EventHandler(this.closeGameMenuItem_Click);
			// 
			// toolStripSeparator8
			// 
			this.toolStripSeparator8.Name = "toolStripSeparator8";
			this.toolStripSeparator8.Size = new System.Drawing.Size(224, 6);
			// 
			// openCDImageMenuItem
			// 
			this.openCDImageMenuItem.Name = "openCDImageMenuItem";
			this.openCDImageMenuItem.Size = new System.Drawing.Size(227, 22);
			this.openCDImageMenuItem.Text = "Open CD &Image File...";
			this.openCDImageMenuItem.Click += new System.EventHandler(this.openCDImageMenuItem_Click);
			// 
			// loadLastGameMenuItem
			// 
			this.loadLastGameMenuItem.Checked = true;
			this.loadLastGameMenuItem.CheckOnClick = true;
			this.loadLastGameMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.loadLastGameMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
			this.loadLastGameMenuItem.Name = "loadLastGameMenuItem";
			this.loadLastGameMenuItem.Size = new System.Drawing.Size(227, 22);
			this.loadLastGameMenuItem.Text = "    After Startup, Open Last Game";
			this.loadLastGameMenuItem.Click += new System.EventHandler(this.loadLastGameMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(224, 6);
			// 
			// chooseBiosRomMenuItem
			// 
			this.chooseBiosRomMenuItem.Name = "chooseBiosRomMenuItem";
			this.chooseBiosRomMenuItem.Size = new System.Drawing.Size(227, 22);
			this.chooseBiosRomMenuItem.Text = "Choose &BIOS Rom File...";
			this.chooseBiosRomMenuItem.Click += new System.EventHandler(this.chooseBiosRomMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(224, 6);
			// 
			// exitMenuItem
			// 
			this.exitMenuItem.Name = "exitMenuItem";
			this.exitMenuItem.Size = new System.Drawing.Size(227, 22);
			this.exitMenuItem.Text = "E&xit";
			this.exitMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// consoleMenuItem
			// 
			this.consoleMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveStateMenuItem,
            this.loadStateMenuItem,
            this.loadLastSaveMenuItem,
            this.toolStripSeparator7,
            this.saveStateSlotMenuItem,
            this.toolStripSeparator3,
            this.pauseMenuItem,
            this.advanceFrameMenuItem,
            this.resetMenuItem});
			this.consoleMenuItem.Name = "consoleMenuItem";
			this.consoleMenuItem.Size = new System.Drawing.Size(57, 20);
			this.consoleMenuItem.Text = "&Console";
			// 
			// saveStateMenuItem
			// 
			this.saveStateMenuItem.Name = "saveStateMenuItem";
			this.saveStateMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.saveStateMenuItem.Size = new System.Drawing.Size(302, 22);
			this.saveStateMenuItem.Text = "&Save State";
			this.saveStateMenuItem.Click += new System.EventHandler(this.saveStateMenuItem_Click);
			// 
			// loadStateMenuItem
			// 
			this.loadStateMenuItem.Name = "loadStateMenuItem";
			this.loadStateMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F8;
			this.loadStateMenuItem.Size = new System.Drawing.Size(302, 22);
			this.loadStateMenuItem.Text = "&Load State";
			this.loadStateMenuItem.Click += new System.EventHandler(this.loadStateMenuItem_Click);
			// 
			// loadLastSaveMenuItem
			// 
			this.loadLastSaveMenuItem.Checked = true;
			this.loadLastSaveMenuItem.CheckOnClick = true;
			this.loadLastSaveMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.loadLastSaveMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
			this.loadLastSaveMenuItem.Name = "loadLastSaveMenuItem";
			this.loadLastSaveMenuItem.Size = new System.Drawing.Size(302, 22);
			this.loadLastSaveMenuItem.Text = "    After Game is Opened, L&oad Last Save (of Slot)";
			this.loadLastSaveMenuItem.Click += new System.EventHandler(this.loadLastSaveMenuItem_Click);
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new System.Drawing.Size(299, 6);
			// 
			// saveStateSlotMenuItem
			// 
			this.saveStateSlotMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.previousSlotMenuItem,
            this.nextSlotMenuItem,
            this.toolStripSeparator4});
			this.saveStateSlotMenuItem.Name = "saveStateSlotMenuItem";
			this.saveStateSlotMenuItem.Size = new System.Drawing.Size(302, 22);
			this.saveStateSlotMenuItem.Text = "Save State Slo&t";
			// 
			// previousSlotMenuItem
			// 
			this.previousSlotMenuItem.Name = "previousSlotMenuItem";
			this.previousSlotMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
			this.previousSlotMenuItem.Size = new System.Drawing.Size(187, 22);
			this.previousSlotMenuItem.Text = "Select &Previous Slot";
			this.previousSlotMenuItem.Click += new System.EventHandler(this.previousSlotMenuItem_Click);
			// 
			// nextSlotMenuItem
			// 
			this.nextSlotMenuItem.Name = "nextSlotMenuItem";
			this.nextSlotMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F7;
			this.nextSlotMenuItem.Size = new System.Drawing.Size(187, 22);
			this.nextSlotMenuItem.Text = "Select &Next Slot";
			this.nextSlotMenuItem.Click += new System.EventHandler(this.nextSlotMenuItem_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(184, 6);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(299, 6);
			// 
			// pauseMenuItem
			// 
			this.pauseMenuItem.Name = "pauseMenuItem";
			this.pauseMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F9;
			this.pauseMenuItem.Size = new System.Drawing.Size(302, 22);
			this.pauseMenuItem.Text = "&Pause";
			this.pauseMenuItem.Click += new System.EventHandler(this.pauseMenuItem_Click);
			// 
			// advanceFrameMenuItem
			// 
			this.advanceFrameMenuItem.Name = "advanceFrameMenuItem";
			this.advanceFrameMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F10;
			this.advanceFrameMenuItem.Size = new System.Drawing.Size(302, 22);
			this.advanceFrameMenuItem.Text = "&Advance by a Single Frame";
			this.advanceFrameMenuItem.Click += new System.EventHandler(this.advanceFrameMenuItem_Click);
			// 
			// resetMenuItem
			// 
			this.resetMenuItem.Name = "resetMenuItem";
			this.resetMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F12;
			this.resetMenuItem.Size = new System.Drawing.Size(302, 22);
			this.resetMenuItem.Text = "&Reset";
			this.resetMenuItem.Click += new System.EventHandler(this.resetMenuItem_Click);
			// 
			// displayMenuItem
			// 
			this.displayMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fullScreenMenuItem,
            this.toolStripSeparator5,
            this.VoidAreaMenuItem,
            this.toolStripSeparator10,
            this.smoothResizingMenuItem,
            this.preserveRatioMenuItem,
            this.toolStripSeparator6,
            this.snapWindowMenuItem});
			this.displayMenuItem.Name = "displayMenuItem";
			this.displayMenuItem.Size = new System.Drawing.Size(53, 20);
			this.displayMenuItem.Text = "&Display";
			// 
			// fullScreenMenuItem
			// 
			this.fullScreenMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
			this.fullScreenMenuItem.Name = "fullScreenMenuItem";
			this.fullScreenMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
			this.fullScreenMenuItem.Size = new System.Drawing.Size(232, 22);
			this.fullScreenMenuItem.Text = "&Full Screen";
			this.fullScreenMenuItem.Click += new System.EventHandler(this.fullScreenMenuItem_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(229, 6);
			// 
			// VoidAreaMenuItem
			// 
			this.VoidAreaMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DrawBorderMenuItem,
            this.toolStripSeparator11,
            this.Pattern4DOMenuItem,
            this.PatternBumpsMenuItem,
            this.PatternMetalMenuItem,
            this.PatternNoneMenuItem});
			this.VoidAreaMenuItem.Name = "VoidAreaMenuItem";
			this.VoidAreaMenuItem.Size = new System.Drawing.Size(232, 22);
			this.VoidAreaMenuItem.Text = "Blank (Void) Area";
			// 
			// DrawBorderMenuItem
			// 
			this.DrawBorderMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
			this.DrawBorderMenuItem.Name = "DrawBorderMenuItem";
			this.DrawBorderMenuItem.Size = new System.Drawing.Size(154, 22);
			this.DrawBorderMenuItem.Text = "Draw Gray Border";
			this.DrawBorderMenuItem.Click += new System.EventHandler(this.DrawBorderMenuItem_Click);
			// 
			// toolStripSeparator11
			// 
			this.toolStripSeparator11.Name = "toolStripSeparator11";
			this.toolStripSeparator11.Size = new System.Drawing.Size(151, 6);
			// 
			// Pattern4DOMenuItem
			// 
			this.Pattern4DOMenuItem.Image = global::FourDO.Properties.Images.VoidImage4DO;
			this.Pattern4DOMenuItem.Name = "Pattern4DOMenuItem";
			this.Pattern4DOMenuItem.Size = new System.Drawing.Size(154, 22);
			this.Pattern4DOMenuItem.Text = "Pattern : 4DO";
			this.Pattern4DOMenuItem.Click += new System.EventHandler(this.Pattern4DOMenuItem_Click);
			// 
			// PatternBumpsMenuItem
			// 
			this.PatternBumpsMenuItem.Image = global::FourDO.Properties.Images.VoidImageBumps;
			this.PatternBumpsMenuItem.Name = "PatternBumpsMenuItem";
			this.PatternBumpsMenuItem.Size = new System.Drawing.Size(154, 22);
			this.PatternBumpsMenuItem.Text = "Pattern : Bumps";
			this.PatternBumpsMenuItem.Click += new System.EventHandler(this.PatternBumpsMenuItem_Click);
			// 
			// PatternMetalMenuItem
			// 
			this.PatternMetalMenuItem.Image = global::FourDO.Properties.Images.VoidImageMetal;
			this.PatternMetalMenuItem.Name = "PatternMetalMenuItem";
			this.PatternMetalMenuItem.Size = new System.Drawing.Size(154, 22);
			this.PatternMetalMenuItem.Text = "Pattern : Metal";
			this.PatternMetalMenuItem.Click += new System.EventHandler(this.PatternMetalMenuItem_Click);
			// 
			// PatternNoneMenuItem
			// 
			this.PatternNoneMenuItem.Image = global::FourDO.Properties.Images.VoidImageNone;
			this.PatternNoneMenuItem.Name = "PatternNoneMenuItem";
			this.PatternNoneMenuItem.Size = new System.Drawing.Size(154, 22);
			this.PatternNoneMenuItem.Text = "No Pattern";
			this.PatternNoneMenuItem.Click += new System.EventHandler(this.PatternNoneMenuItem_Click);
			// 
			// toolStripSeparator10
			// 
			this.toolStripSeparator10.Name = "toolStripSeparator10";
			this.toolStripSeparator10.Size = new System.Drawing.Size(229, 6);
			// 
			// smoothResizingMenuItem
			// 
			this.smoothResizingMenuItem.Checked = true;
			this.smoothResizingMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.smoothResizingMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
			this.smoothResizingMenuItem.Name = "smoothResizingMenuItem";
			this.smoothResizingMenuItem.Size = new System.Drawing.Size(232, 22);
			this.smoothResizingMenuItem.Text = "&Smooth Image Resizing";
			this.smoothResizingMenuItem.Click += new System.EventHandler(this.smoothResizingMenuItem_Click);
			// 
			// preserveRatioMenuItem
			// 
			this.preserveRatioMenuItem.Checked = true;
			this.preserveRatioMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.preserveRatioMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
			this.preserveRatioMenuItem.Name = "preserveRatioMenuItem";
			this.preserveRatioMenuItem.Size = new System.Drawing.Size(232, 22);
			this.preserveRatioMenuItem.Text = "Preserve Aspect &Ratio";
			this.preserveRatioMenuItem.Click += new System.EventHandler(this.preserveRatioMenuItem_Click);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(229, 6);
			// 
			// snapWindowMenuItem
			// 
			this.snapWindowMenuItem.Checked = true;
			this.snapWindowMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.snapWindowMenuItem.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
			this.snapWindowMenuItem.Name = "snapWindowMenuItem";
			this.snapWindowMenuItem.Size = new System.Drawing.Size(232, 22);
			this.snapWindowMenuItem.Text = "Snap &Window to Clean Increments";
			this.snapWindowMenuItem.Click += new System.EventHandler(this.snapWindowMenuItem_Click);
			// 
			// audioMenuItem
			// 
			this.audioMenuItem.Name = "audioMenuItem";
			this.audioMenuItem.Size = new System.Drawing.Size(46, 20);
			this.audioMenuItem.Text = "&Audio";
			// 
			// optionsMenuItem
			// 
			this.optionsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsMenuItem,
            this.toolStripSeparator9,
            this.configureInputMenuItem,
            this.toolStripSeparator13,
            this.languageMenuItem});
			this.optionsMenuItem.Name = "optionsMenuItem";
			this.optionsMenuItem.Size = new System.Drawing.Size(56, 20);
			this.optionsMenuItem.Text = "&Options";
			// 
			// settingsMenuItem
			// 
			this.settingsMenuItem.Name = "settingsMenuItem";
			this.settingsMenuItem.Size = new System.Drawing.Size(162, 22);
			this.settingsMenuItem.Text = "4DO &Settings...";
			this.settingsMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
			// 
			// toolStripSeparator9
			// 
			this.toolStripSeparator9.Name = "toolStripSeparator9";
			this.toolStripSeparator9.Size = new System.Drawing.Size(159, 6);
			// 
			// configureInputMenuItem
			// 
			this.configureInputMenuItem.Name = "configureInputMenuItem";
			this.configureInputMenuItem.Size = new System.Drawing.Size(162, 22);
			this.configureInputMenuItem.Text = "Configure &Input...";
			this.configureInputMenuItem.Click += new System.EventHandler(this.configureInputMenuItem_Click);
			// 
			// toolStripSeparator13
			// 
			this.toolStripSeparator13.Name = "toolStripSeparator13";
			this.toolStripSeparator13.Size = new System.Drawing.Size(159, 6);
			// 
			// languageMenuItem
			// 
			this.languageMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.languageDefaultMenuItem,
            this.toolStripSeparator14,
            this.languageEnglishMenuItem,
            this.languageRussianMenuItem,
            this.languageFrenchMenuItem});
			this.languageMenuItem.Name = "languageMenuItem";
			this.languageMenuItem.Size = new System.Drawing.Size(162, 22);
			this.languageMenuItem.Text = "&Language";
			// 
			// languageDefaultMenuItem
			// 
			this.languageDefaultMenuItem.Name = "languageDefaultMenuItem";
			this.languageDefaultMenuItem.Size = new System.Drawing.Size(152, 22);
			this.languageDefaultMenuItem.Text = "Default";
			this.languageDefaultMenuItem.Click += new System.EventHandler(this.languageDefaultMenuItem_Click);
			// 
			// toolStripSeparator14
			// 
			this.toolStripSeparator14.Name = "toolStripSeparator14";
			this.toolStripSeparator14.Size = new System.Drawing.Size(149, 6);
			// 
			// languageEnglishMenuItem
			// 
			this.languageEnglishMenuItem.Name = "languageEnglishMenuItem";
			this.languageEnglishMenuItem.Size = new System.Drawing.Size(152, 22);
			this.languageEnglishMenuItem.Tag = "en";
			this.languageEnglishMenuItem.Text = "English";
			this.languageEnglishMenuItem.Click += new System.EventHandler(this.languageEnglishMenuItem_Click);
			// 
			// languageRussianMenuItem
			// 
			this.languageRussianMenuItem.Name = "languageRussianMenuItem";
			this.languageRussianMenuItem.Size = new System.Drawing.Size(152, 22);
			this.languageRussianMenuItem.Tag = "ru";
			this.languageRussianMenuItem.Text = "Russian";
			this.languageRussianMenuItem.Click += new System.EventHandler(this.languageRussianMenuItem_Click);
			// 
			// languageFrenchMenuItem
			// 
			this.languageFrenchMenuItem.Name = "languageFrenchMenuItem";
			this.languageFrenchMenuItem.Size = new System.Drawing.Size(152, 22);
			this.languageFrenchMenuItem.Tag = "fr";
			this.languageFrenchMenuItem.Text = "French";
			this.languageFrenchMenuItem.Click += new System.EventHandler(this.languageFrenchMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gameInfoMenuItem,
            this.toolStripSeparator12,
            this.aboutMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
			this.helpToolStripMenuItem.Text = "&Help";
			// 
			// gameInfoMenuItem
			// 
			this.gameInfoMenuItem.Name = "gameInfoMenuItem";
			this.gameInfoMenuItem.Size = new System.Drawing.Size(172, 22);
			this.gameInfoMenuItem.Text = "Game &Information...";
			this.gameInfoMenuItem.Click += new System.EventHandler(this.gameInfoMenuItem_Click);
			// 
			// toolStripSeparator12
			// 
			this.toolStripSeparator12.Name = "toolStripSeparator12";
			this.toolStripSeparator12.Size = new System.Drawing.Size(169, 6);
			// 
			// aboutMenuItem
			// 
			this.aboutMenuItem.Name = "aboutMenuItem";
			this.aboutMenuItem.Size = new System.Drawing.Size(172, 22);
			this.aboutMenuItem.Text = "&About 4DO...";
			this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
			// 
			// refreshFpsTimer
			// 
			this.refreshFpsTimer.Enabled = true;
			this.refreshFpsTimer.Interval = 300;
			this.refreshFpsTimer.Tick += new System.EventHandler(this.RefreshFpsTimer_Tick);
			// 
			// hideMenuTimer
			// 
			this.hideMenuTimer.Interval = 2000;
			this.hideMenuTimer.Tick += new System.EventHandler(this.hideMenuTimer_Tick);
			// 
			// sizeBox
			// 
			this.sizeBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
			this.sizeBox.BaseHeight = 0;
			this.sizeBox.BaseWidth = 0;
			this.sizeBox.Location = new System.Drawing.Point(494, 557);
			this.sizeBox.Name = "sizeBox";
			this.sizeBox.Size = new System.Drawing.Size(228, 24);
			this.sizeBox.TabIndex = 4;
			this.sizeBox.Visible = false;
			// 
			// RomNagBox
			// 
			this.RomNagBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.RomNagBox.BackColor = System.Drawing.SystemColors.Info;
			this.RomNagBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.RomNagBox.LinkText = "Choose BIOS Rom";
			this.RomNagBox.Location = new System.Drawing.Point(8, 545);
			this.RomNagBox.MessageText = "No BIOS rom selected.";
			this.RomNagBox.Name = "RomNagBox";
			this.RomNagBox.Size = new System.Drawing.Size(730, 28);
			this.RomNagBox.TabIndex = 1;
			this.RomNagBox.Visible = false;
			this.RomNagBox.CloseClicked += new System.EventHandler(this.RomNagBox_CloseClicked);
			this.RomNagBox.LinkClicked += new System.EventHandler(this.RomNagBox_LinkClicked);
			// 
			// gameCanvas
			// 
			this.gameCanvas.BackColor = System.Drawing.Color.Black;
			this.gameCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gameCanvas.ImageSmoothing = true;
			this.gameCanvas.IsConsoleStopped = true;
			this.gameCanvas.IsInResizeMode = false;
			this.gameCanvas.Location = new System.Drawing.Point(0, 24);
			this.gameCanvas.Name = "gameCanvas";
			this.gameCanvas.PreserveAspectRatio = true;
			this.gameCanvas.RenderHighResolution = false;
			this.gameCanvas.Size = new System.Drawing.Size(744, 557);
			this.gameCanvas.TabIndex = 3;
			this.gameCanvas.VoidAreaBorder = false;
			this.gameCanvas.VoidAreaPattern = FourDO.UI.VoidAreaPattern.None;
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(744, 603);
			this.Controls.Add(this.sizeBox);
			this.Controls.Add(this.RomNagBox);
			this.Controls.Add(this.gameCanvas);
			this.Controls.Add(this.MainStatusStrip);
			this.Controls.Add(this.MainMenuBar);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "Main";
			this.Text = "4DO";
			this.Activated += new System.EventHandler(this.Main_Activated);
			this.Deactivate += new System.EventHandler(this.Main_Deactivate);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
			this.Load += new System.EventHandler(this.Main_Load);
			this.ResizeBegin += new System.EventHandler(this.Main_ResizeBegin);
			this.ResizeEnd += new System.EventHandler(this.Main_ResizeEnd);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Main_KeyDown);
			this.Resize += new System.EventHandler(this.Main_Resize);
			this.MainStatusStrip.ResumeLayout(false);
			this.MainStatusStrip.PerformLayout();
			this.MainMenuBar.ResumeLayout(false);
			this.MainMenuBar.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip MainStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel VersionStripItem;
        private NagBox RomNagBox;
        private System.Windows.Forms.MenuStrip MainMenuBar;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chooseBiosRomMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private GameCanvas gameCanvas;
        private System.Windows.Forms.ToolStripStatusLabel FPSStripItem;
        private System.Windows.Forms.Timer refreshFpsTimer;
        private System.Windows.Forms.ToolStripMenuItem openCDImageMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadLastGameMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem optionsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsMenuItem;
        private SizeBox sizeBox;
        private System.Windows.Forms.ToolStripDropDownButton quickDisplayDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem displayMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fullScreenMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem snapWindowMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preserveRatioMenuItem;
        private System.Windows.Forms.ToolStripMenuItem smoothResizingMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.Timer hideMenuTimer;
        private System.Windows.Forms.ToolStripMenuItem consoleMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveStateMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadStateMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadLastSaveMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem saveStateSlotMenuItem;
        private System.Windows.Forms.ToolStripMenuItem previousSlotMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nextSlotMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem advanceFrameMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeGameMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem configureInputMenuItem;
		private System.Windows.Forms.ToolStripMenuItem VoidAreaMenuItem;
		private System.Windows.Forms.ToolStripMenuItem DrawBorderMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
		private System.Windows.Forms.ToolStripMenuItem Pattern4DOMenuItem;
		private System.Windows.Forms.ToolStripMenuItem PatternBumpsMenuItem;
		private System.Windows.Forms.ToolStripMenuItem PatternMetalMenuItem;
		private System.Windows.Forms.ToolStripMenuItem PatternNoneMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
		private System.Windows.Forms.ToolStripMenuItem gameInfoMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
		private System.Windows.Forms.ToolStripMenuItem audioMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
		private System.Windows.Forms.ToolStripMenuItem languageMenuItem;
		private System.Windows.Forms.ToolStripMenuItem languageDefaultMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
		private System.Windows.Forms.ToolStripMenuItem languageEnglishMenuItem;
		private System.Windows.Forms.ToolStripMenuItem languageRussianMenuItem;
		private System.Windows.Forms.ToolStripMenuItem languageFrenchMenuItem;

    }
}

