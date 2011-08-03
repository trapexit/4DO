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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.RefreshFpsTimer = new System.Windows.Forms.Timer(this.components);
            this.RomNagBox = new FourDO.UI.NagBox();
            this.gameCanvas1 = new FourDO.UI.GameCanvas();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainStatusStrip.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainStatusStrip
            // 
            this.MainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusStripItem,
            this.FPSStripItem});
            this.MainStatusStrip.Location = new System.Drawing.Point(0, 581);
            this.MainStatusStrip.Name = "MainStatusStrip";
            this.MainStatusStrip.Size = new System.Drawing.Size(744, 22);
            this.MainStatusStrip.TabIndex = 0;
            // 
            // StatusStripItem
            // 
            this.StatusStripItem.Name = "StatusStripItem";
            this.StatusStripItem.Size = new System.Drawing.Size(126, 17);
            this.StatusStripItem.Text = "FourDO Version 1.0.0.0";
            // 
            // FPSStripItem
            // 
            this.FPSStripItem.Name = "FPSStripItem";
            this.FPSStripItem.Size = new System.Drawing.Size(603, 17);
            this.FPSStripItem.Spring = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(744, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
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
            // RefreshFpsTimer
            // 
            this.RefreshFpsTimer.Enabled = true;
            this.RefreshFpsTimer.Interval = 300;
            this.RefreshFpsTimer.Tick += new System.EventHandler(this.RefreshFpsTimer_Tick);
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
            // gameCanvas1
            // 
            this.gameCanvas1.BackColor = System.Drawing.Color.Black;
            this.gameCanvas1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("gameCanvas1.BackgroundImage")));
            this.gameCanvas1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gameCanvas1.Location = new System.Drawing.Point(0, 24);
            this.gameCanvas1.Name = "gameCanvas1";
            this.gameCanvas1.Size = new System.Drawing.Size(744, 557);
            this.gameCanvas1.TabIndex = 3;
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.settingsToolStripMenuItem.Text = "&Settings...";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 603);
            this.Controls.Add(this.RomNagBox);
            this.Controls.Add(this.gameCanvas1);
            this.Controls.Add(this.MainStatusStrip);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FourDO";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
            this.Load += new System.EventHandler(this.Main_Load);
            this.MainStatusStrip.ResumeLayout(false);
            this.MainStatusStrip.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip MainStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel StatusStripItem;
        private NagBox RomNagBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chooseBiosRomMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private GameCanvas gameCanvas1;
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
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;

    }
}

