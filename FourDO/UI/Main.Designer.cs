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
            this.StatusStripItem = new System.Windows.Forms.ToolStripStatusLabel();
            this.FPSStripItem = new System.Windows.Forms.ToolStripStatusLabel();
            this.quickDisplayDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.MainMenuBar = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openCDImageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadLastGameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveStateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadStateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveStateSlotMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previousSlotMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextSlotMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.loadLastSaveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.chooseBiosRomMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullScreenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.snapWindowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preserveRatioMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RefreshFpsTimer = new System.Windows.Forms.Timer(this.components);
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
            this.StatusStripItem,
            this.FPSStripItem,
            this.quickDisplayDropDownButton});
            this.MainStatusStrip.Location = new System.Drawing.Point(0, 581);
            this.MainStatusStrip.Name = "MainStatusStrip";
            this.MainStatusStrip.Size = new System.Drawing.Size(744, 22);
            this.MainStatusStrip.TabIndex = 0;
            // 
            // StatusStripItem
            // 
            this.StatusStripItem.Name = "StatusStripItem";
            this.StatusStripItem.Size = new System.Drawing.Size(84, 17);
            this.StatusStripItem.Text = "FourDO 1.0.0.5";
            // 
            // FPSStripItem
            // 
            this.FPSStripItem.Name = "FPSStripItem";
            this.FPSStripItem.Size = new System.Drawing.Size(526, 17);
            this.FPSStripItem.Spring = true;
            // 
            // quickDisplayDropDownButton
            // 
            this.quickDisplayDropDownButton.BackColor = System.Drawing.SystemColors.Window;
            this.quickDisplayDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("quickDisplayDropDownButton.Image")));
            this.quickDisplayDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.quickDisplayDropDownButton.Name = "quickDisplayDropDownButton";
            this.quickDisplayDropDownButton.Size = new System.Drawing.Size(119, 20);
            this.quickDisplayDropDownButton.Text = "Display Options";
            // 
            // MainMenuBar
            // 
            this.MainMenuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.displayMenuItem,
            this.optionsMenuItem});
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
            this.openCDImageMenuItem,
            this.loadLastGameMenuItem,
            this.toolStripSeparator1,
            this.saveStateMenuItem,
            this.loadStateMenuItem,
            this.saveStateSlotMenuItem,
            this.loadLastSaveMenuItem,
            this.toolStripSeparator3,
            this.chooseBiosRomMenuItem,
            this.toolStripSeparator2,
            this.exitMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileMenuItem.Text = "&File";
            // 
            // openCDImageMenuItem
            // 
            this.openCDImageMenuItem.Name = "openCDImageMenuItem";
            this.openCDImageMenuItem.Size = new System.Drawing.Size(253, 22);
            this.openCDImageMenuItem.Text = "Open CD &Image File (*.iso)...";
            this.openCDImageMenuItem.Click += new System.EventHandler(this.openCDImageMenuItem_Click);
            // 
            // loadLastGameMenuItem
            // 
            this.loadLastGameMenuItem.Checked = true;
            this.loadLastGameMenuItem.CheckOnClick = true;
            this.loadLastGameMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.loadLastGameMenuItem.Name = "loadLastGameMenuItem";
            this.loadLastGameMenuItem.Size = new System.Drawing.Size(253, 22);
            this.loadLastGameMenuItem.Text = "Load Last Game on Startup";
            this.loadLastGameMenuItem.Click += new System.EventHandler(this.loadLastGameMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(250, 6);
            // 
            // saveStateMenuItem
            // 
            this.saveStateMenuItem.Name = "saveStateMenuItem";
            this.saveStateMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.saveStateMenuItem.Size = new System.Drawing.Size(253, 22);
            this.saveStateMenuItem.Text = "&Save State";
            this.saveStateMenuItem.Click += new System.EventHandler(this.saveStateMenuItem_Click);
            // 
            // loadStateMenuItem
            // 
            this.loadStateMenuItem.Name = "loadStateMenuItem";
            this.loadStateMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F8;
            this.loadStateMenuItem.Size = new System.Drawing.Size(253, 22);
            this.loadStateMenuItem.Text = "&Load State";
            this.loadStateMenuItem.Click += new System.EventHandler(this.loadStateMenuItem_Click);
            // 
            // saveStateSlotMenuItem
            // 
            this.saveStateSlotMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.previousSlotMenuItem,
            this.nextSlotMenuItem,
            this.toolStripSeparator4});
            this.saveStateSlotMenuItem.Name = "saveStateSlotMenuItem";
            this.saveStateSlotMenuItem.Size = new System.Drawing.Size(253, 22);
            this.saveStateSlotMenuItem.Text = "Save State Slot";
            // 
            // previousSlotMenuItem
            // 
            this.previousSlotMenuItem.Name = "previousSlotMenuItem";
            this.previousSlotMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.previousSlotMenuItem.Size = new System.Drawing.Size(195, 22);
            this.previousSlotMenuItem.Text = "Select &Previous Slot";
            this.previousSlotMenuItem.Click += new System.EventHandler(this.previousSlotMenuItem_Click);
            // 
            // nextSlotMenuItem
            // 
            this.nextSlotMenuItem.Name = "nextSlotMenuItem";
            this.nextSlotMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F7;
            this.nextSlotMenuItem.Size = new System.Drawing.Size(195, 22);
            this.nextSlotMenuItem.Text = "Select &Next Slot";
            this.nextSlotMenuItem.Click += new System.EventHandler(this.nextSlotMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(192, 6);
            // 
            // loadLastSaveMenuItem
            // 
            this.loadLastSaveMenuItem.Checked = true;
            this.loadLastSaveMenuItem.CheckOnClick = true;
            this.loadLastSaveMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.loadLastSaveMenuItem.Name = "loadLastSaveMenuItem";
            this.loadLastSaveMenuItem.Size = new System.Drawing.Size(253, 22);
            this.loadLastSaveMenuItem.Text = "Load Last Save (of slot) on Startup";
            this.loadLastSaveMenuItem.Click += new System.EventHandler(this.loadLastSaveMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(250, 6);
            // 
            // chooseBiosRomMenuItem
            // 
            this.chooseBiosRomMenuItem.Name = "chooseBiosRomMenuItem";
            this.chooseBiosRomMenuItem.Size = new System.Drawing.Size(253, 22);
            this.chooseBiosRomMenuItem.Text = "Choose &BIOS Rom File...";
            this.chooseBiosRomMenuItem.Click += new System.EventHandler(this.chooseBiosRomMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(250, 6);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(253, 22);
            this.exitMenuItem.Text = "E&xit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // displayMenuItem
            // 
            this.displayMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fullScreenMenuItem,
            this.toolStripSeparator5,
            this.snapWindowMenuItem,
            this.preserveRatioMenuItem});
            this.displayMenuItem.Name = "displayMenuItem";
            this.displayMenuItem.Size = new System.Drawing.Size(57, 20);
            this.displayMenuItem.Text = "&Display";
            // 
            // fullScreenMenuItem
            // 
            this.fullScreenMenuItem.Name = "fullScreenMenuItem";
            this.fullScreenMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.fullScreenMenuItem.Size = new System.Drawing.Size(256, 22);
            this.fullScreenMenuItem.Text = "Full Screen";
            this.fullScreenMenuItem.Click += new System.EventHandler(this.fullScreenMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(253, 6);
            // 
            // snapWindowMenuItem
            // 
            this.snapWindowMenuItem.Checked = true;
            this.snapWindowMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.snapWindowMenuItem.Name = "snapWindowMenuItem";
            this.snapWindowMenuItem.Size = new System.Drawing.Size(256, 22);
            this.snapWindowMenuItem.Text = "Snap Window to Clean Increments";
            this.snapWindowMenuItem.Click += new System.EventHandler(this.snapWindowMenuItem_Click);
            // 
            // preserveRatioMenuItem
            // 
            this.preserveRatioMenuItem.Checked = true;
            this.preserveRatioMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.preserveRatioMenuItem.Name = "preserveRatioMenuItem";
            this.preserveRatioMenuItem.Size = new System.Drawing.Size(256, 22);
            this.preserveRatioMenuItem.Text = "Preserve Aspect Ratio";
            this.preserveRatioMenuItem.Click += new System.EventHandler(this.preserveRatioMenuItem_Click);
            // 
            // optionsMenuItem
            // 
            this.optionsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsMenuItem});
            this.optionsMenuItem.Name = "optionsMenuItem";
            this.optionsMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsMenuItem.Text = "&Options";
            // 
            // settingsMenuItem
            // 
            this.settingsMenuItem.Name = "settingsMenuItem";
            this.settingsMenuItem.Size = new System.Drawing.Size(125, 22);
            this.settingsMenuItem.Text = "&Settings...";
            this.settingsMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // RefreshFpsTimer
            // 
            this.RefreshFpsTimer.Enabled = true;
            this.RefreshFpsTimer.Interval = 300;
            this.RefreshFpsTimer.Tick += new System.EventHandler(this.RefreshFpsTimer_Tick);
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
            this.gameCanvas.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("gameCanvas.BackgroundImage")));
            this.gameCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gameCanvas.Location = new System.Drawing.Point(0, 24);
            this.gameCanvas.Name = "gameCanvas";
            this.gameCanvas.PreserveAspectRatio = true;
            this.gameCanvas.Size = new System.Drawing.Size(744, 557);
            this.gameCanvas.TabIndex = 3;
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
            this.Name = "Main";
            this.Text = "FourDO";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
            this.Load += new System.EventHandler(this.Main_Load);
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
        private System.Windows.Forms.ToolStripStatusLabel StatusStripItem;
        private NagBox RomNagBox;
        private System.Windows.Forms.MenuStrip MainMenuBar;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chooseBiosRomMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private GameCanvas gameCanvas;
        private System.Windows.Forms.ToolStripStatusLabel FPSStripItem;
        private System.Windows.Forms.Timer RefreshFpsTimer;
        private System.Windows.Forms.ToolStripMenuItem openCDImageMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadLastGameMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveStateMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadStateMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveStateSlotMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nextSlotMenuItem;
        private System.Windows.Forms.ToolStripMenuItem previousSlotMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadLastSaveMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem optionsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsMenuItem;
        private SizeBox sizeBox;
        private System.Windows.Forms.ToolStripDropDownButton quickDisplayDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem displayMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fullScreenMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem snapWindowMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preserveRatioMenuItem;

    }
}

