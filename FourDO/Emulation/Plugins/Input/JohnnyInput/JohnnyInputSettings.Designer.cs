namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	partial class JohnnyInputSettings
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JohnnyInputSettings));
			this.panel1 = new System.Windows.Forms.Panel();
			this.ResetButton = new System.Windows.Forms.Button();
			this.CloseButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.OKButton = new System.Windows.Forms.Button();
			this.MainTabControl = new System.Windows.Forms.TabControl();
			this.ConsoleTab = new System.Windows.Forms.TabPage();
			this.Player1Tab = new System.Windows.Forms.TabPage();
			this.panel5 = new System.Windows.Forms.Panel();
			this.Player2Tab = new System.Windows.Forms.TabPage();
			this.Player3Tab = new System.Windows.Forms.TabPage();
			this.Player4Tab = new System.Windows.Forms.TabPage();
			this.Player5Tab = new System.Windows.Forms.TabPage();
			this.Player6Tab = new System.Windows.Forms.TabPage();
			this.JoystickWatchTimer = new System.Windows.Forms.Timer(this.components);
			this.RefreshJoystickListTimer = new System.Windows.Forms.Timer(this.components);
			this.ControllerPanel = new System.Windows.Forms.Panel();
			this.ControlsGridView = new System.Windows.Forms.DataGridView();
			this.panel4 = new System.Windows.Forms.Panel();
			this.CurrentDevicesLabel = new System.Windows.Forms.Label();
			this.controllerInfo = new FourDO.Emulation.Plugins.Input.JohnnyInput.ControllerInfo();
			this.controllerPreview = new FourDO.Emulation.Plugins.Input.JohnnyInput.ControllerPreview();
			this.panel1.SuspendLayout();
			this.MainTabControl.SuspendLayout();
			this.Player1Tab.SuspendLayout();
			this.ControllerPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ControlsGridView)).BeginInit();
			this.panel4.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.ResetButton);
			this.panel1.Controls.Add(this.CloseButton);
			this.panel1.Controls.Add(this.groupBox1);
			this.panel1.Controls.Add(this.OKButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 369);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(793, 42);
			this.panel1.TabIndex = 1;
			// 
			// ResetButton
			// 
			this.ResetButton.Location = new System.Drawing.Point(12, 10);
			this.ResetButton.Name = "ResetButton";
			this.ResetButton.Size = new System.Drawing.Size(120, 23);
			this.ResetButton.TabIndex = 0;
			this.ResetButton.Text = "&Reset All to Defaults";
			this.ResetButton.UseVisualStyleBackColor = true;
			this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
			// 
			// CloseButton
			// 
			this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CloseButton.Location = new System.Drawing.Point(709, 10);
			this.CloseButton.Name = "CloseButton";
			this.CloseButton.Size = new System.Drawing.Size(75, 23);
			this.CloseButton.TabIndex = 2;
			this.CloseButton.Text = "&Cancel";
			this.CloseButton.UseVisualStyleBackColor = true;
			this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(793, 2);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			// 
			// OKButton
			// 
			this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.OKButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.OKButton.Location = new System.Drawing.Point(628, 10);
			this.OKButton.Name = "OKButton";
			this.OKButton.Size = new System.Drawing.Size(75, 23);
			this.OKButton.TabIndex = 1;
			this.OKButton.Text = "&OK";
			this.OKButton.UseVisualStyleBackColor = true;
			this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
			// 
			// MainTabControl
			// 
			this.MainTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.MainTabControl.Controls.Add(this.ConsoleTab);
			this.MainTabControl.Controls.Add(this.Player1Tab);
			this.MainTabControl.Controls.Add(this.Player2Tab);
			this.MainTabControl.Controls.Add(this.Player3Tab);
			this.MainTabControl.Controls.Add(this.Player4Tab);
			this.MainTabControl.Controls.Add(this.Player5Tab);
			this.MainTabControl.Controls.Add(this.Player6Tab);
			this.MainTabControl.Location = new System.Drawing.Point(12, 12);
			this.MainTabControl.Name = "MainTabControl";
			this.MainTabControl.SelectedIndex = 0;
			this.MainTabControl.Size = new System.Drawing.Size(769, 348);
			this.MainTabControl.TabIndex = 0;
			this.MainTabControl.SelectedIndexChanged += new System.EventHandler(this.MainTabControl_SelectedIndexChanged);
			// 
			// ConsoleTab
			// 
			this.ConsoleTab.Location = new System.Drawing.Point(4, 22);
			this.ConsoleTab.Name = "ConsoleTab";
			this.ConsoleTab.Size = new System.Drawing.Size(761, 322);
			this.ConsoleTab.TabIndex = 7;
			this.ConsoleTab.Text = "Console";
			this.ConsoleTab.UseVisualStyleBackColor = true;
			// 
			// Player1Tab
			// 
			this.Player1Tab.BackColor = System.Drawing.Color.Transparent;
			this.Player1Tab.Controls.Add(this.panel5);
			this.Player1Tab.Location = new System.Drawing.Point(4, 22);
			this.Player1Tab.Name = "Player1Tab";
			this.Player1Tab.Padding = new System.Windows.Forms.Padding(3);
			this.Player1Tab.Size = new System.Drawing.Size(761, 322);
			this.Player1Tab.TabIndex = 1;
			this.Player1Tab.Text = "Player 1";
			this.Player1Tab.UseVisualStyleBackColor = true;
			// 
			// panel5
			// 
			this.panel5.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel5.Location = new System.Drawing.Point(755, 3);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(3, 316);
			this.panel5.TabIndex = 15;
			// 
			// Player2Tab
			// 
			this.Player2Tab.BackColor = System.Drawing.Color.Transparent;
			this.Player2Tab.Location = new System.Drawing.Point(4, 22);
			this.Player2Tab.Name = "Player2Tab";
			this.Player2Tab.Size = new System.Drawing.Size(761, 322);
			this.Player2Tab.TabIndex = 2;
			this.Player2Tab.Text = "Player 2";
			this.Player2Tab.UseVisualStyleBackColor = true;
			// 
			// Player3Tab
			// 
			this.Player3Tab.BackColor = System.Drawing.Color.Transparent;
			this.Player3Tab.Location = new System.Drawing.Point(4, 22);
			this.Player3Tab.Name = "Player3Tab";
			this.Player3Tab.Size = new System.Drawing.Size(761, 322);
			this.Player3Tab.TabIndex = 3;
			this.Player3Tab.Text = "Player 3";
			this.Player3Tab.UseVisualStyleBackColor = true;
			// 
			// Player4Tab
			// 
			this.Player4Tab.BackColor = System.Drawing.Color.Transparent;
			this.Player4Tab.Location = new System.Drawing.Point(4, 22);
			this.Player4Tab.Name = "Player4Tab";
			this.Player4Tab.Size = new System.Drawing.Size(761, 322);
			this.Player4Tab.TabIndex = 4;
			this.Player4Tab.Text = "Player 4";
			this.Player4Tab.UseVisualStyleBackColor = true;
			// 
			// Player5Tab
			// 
			this.Player5Tab.BackColor = System.Drawing.Color.Transparent;
			this.Player5Tab.Location = new System.Drawing.Point(4, 22);
			this.Player5Tab.Name = "Player5Tab";
			this.Player5Tab.Size = new System.Drawing.Size(761, 322);
			this.Player5Tab.TabIndex = 5;
			this.Player5Tab.Text = "Player 5";
			this.Player5Tab.UseVisualStyleBackColor = true;
			// 
			// Player6Tab
			// 
			this.Player6Tab.BackColor = System.Drawing.Color.Transparent;
			this.Player6Tab.Location = new System.Drawing.Point(4, 22);
			this.Player6Tab.Name = "Player6Tab";
			this.Player6Tab.Size = new System.Drawing.Size(761, 322);
			this.Player6Tab.TabIndex = 6;
			this.Player6Tab.Text = "Player 6";
			this.Player6Tab.UseVisualStyleBackColor = true;
			// 
			// JoystickWatchTimer
			// 
			this.JoystickWatchTimer.Enabled = true;
			this.JoystickWatchTimer.Interval = 50;
			this.JoystickWatchTimer.Tick += new System.EventHandler(this.JoystickWatchTimer_Tick);
			// 
			// RefreshJoystickListTimer
			// 
			this.RefreshJoystickListTimer.Enabled = true;
			this.RefreshJoystickListTimer.Interval = 1000;
			this.RefreshJoystickListTimer.Tick += new System.EventHandler(this.RefreshJoystickListTimer_Tick);
			// 
			// ControllerPanel
			// 
			this.ControllerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ControllerPanel.Controls.Add(this.ControlsGridView);
			this.ControllerPanel.Controls.Add(this.panel4);
			this.ControllerPanel.Location = new System.Drawing.Point(71, 55);
			this.ControllerPanel.Name = "ControllerPanel";
			this.ControllerPanel.Size = new System.Drawing.Size(576, 268);
			this.ControllerPanel.TabIndex = 21;
			// 
			// ControlsGridView
			// 
			this.ControlsGridView.AllowUserToAddRows = false;
			this.ControlsGridView.AllowUserToDeleteRows = false;
			this.ControlsGridView.AllowUserToResizeColumns = false;
			this.ControlsGridView.AllowUserToResizeRows = false;
			this.ControlsGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.ControlsGridView.BackgroundColor = System.Drawing.SystemColors.Window;
			this.ControlsGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.ControlsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.ControlsGridView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ControlsGridView.Location = new System.Drawing.Point(287, 0);
			this.ControlsGridView.MultiSelect = false;
			this.ControlsGridView.Name = "ControlsGridView";
			this.ControlsGridView.ReadOnly = true;
			this.ControlsGridView.RowHeadersVisible = false;
			this.ControlsGridView.RowTemplate.Height = 18;
			this.ControlsGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
			this.ControlsGridView.ShowCellToolTips = false;
			this.ControlsGridView.Size = new System.Drawing.Size(289, 268);
			this.ControlsGridView.StandardTab = true;
			this.ControlsGridView.TabIndex = 0;
			this.ControlsGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ControlsGridView_CellClick);
			this.ControlsGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ControlsGridView_CellDoubleClick);
			this.ControlsGridView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.ControlsGridView_CellEnter);
			this.ControlsGridView.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.ControlsGridView_CellMouseDown);
			this.ControlsGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ControlsGridView_KeyDown);
			this.ControlsGridView.Leave += new System.EventHandler(this.ControlsGridView_Leave);
			this.ControlsGridView.MouseLeave += new System.EventHandler(this.ControlsGridView_MouseLeave);
			this.ControlsGridView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ControlsGridView_MouseMove);
			// 
			// panel4
			// 
			this.panel4.Controls.Add(this.CurrentDevicesLabel);
			this.panel4.Controls.Add(this.controllerInfo);
			this.panel4.Controls.Add(this.controllerPreview);
			this.panel4.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel4.Location = new System.Drawing.Point(0, 0);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(287, 268);
			this.panel4.TabIndex = 19;
			// 
			// CurrentDevicesLabel
			// 
			this.CurrentDevicesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.CurrentDevicesLabel.BackColor = System.Drawing.Color.OldLace;
			this.CurrentDevicesLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.CurrentDevicesLabel.Location = new System.Drawing.Point(0, 0);
			this.CurrentDevicesLabel.Name = "CurrentDevicesLabel";
			this.CurrentDevicesLabel.Size = new System.Drawing.Size(170, 18);
			this.CurrentDevicesLabel.TabIndex = 21;
			this.CurrentDevicesLabel.Text = "Current Devices:";
			this.CurrentDevicesLabel.Visible = false;
			// 
			// controllerInfo
			// 
			this.controllerInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.controllerInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.controllerInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.controllerInfo.DeviceCount = 1;
			this.controllerInfo.Location = new System.Drawing.Point(0, 179);
			this.controllerInfo.Name = "controllerInfo";
			this.controllerInfo.Size = new System.Drawing.Size(281, 89);
			this.controllerInfo.TabIndex = 1;
			this.controllerInfo.LinkMouseEnter += new System.EventHandler(this.controllerInfo_LinkMouseEnter);
			this.controllerInfo.LinkMouseLeave += new System.EventHandler(this.controllerInfo_LinkMouseLeave);
			// 
			// controllerPreview
			// 
			this.controllerPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.controllerPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(205)))), ((int)(((byte)(230)))));
			this.controllerPreview.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("controllerPreview.BackgroundImage")));
			this.controllerPreview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.controllerPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.controllerPreview.HighlightedButton = null;
			this.controllerPreview.Location = new System.Drawing.Point(0, 0);
			this.controllerPreview.Name = "controllerPreview";
			this.controllerPreview.Size = new System.Drawing.Size(281, 173);
			this.controllerPreview.TabIndex = 0;
			this.controllerPreview.ViewMode = FourDO.Emulation.Plugins.Input.JohnnyInput.ControllerPreview.ViewModeEnum.Controller;
			this.controllerPreview.MouseHoverButton += new FourDO.Emulation.Plugins.Input.JohnnyInput.ControllerPreview.MouseHoverButtonHandler(this.controllerPreview_MouseHoverButton);
			this.controllerPreview.MouseClick += new System.Windows.Forms.MouseEventHandler(this.controllerPreview_MouseClick);
			this.controllerPreview.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.controllerPreview_MouseDoubleClick);
			// 
			// JohnnyInputSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(793, 411);
			this.Controls.Add(this.ControllerPanel);
			this.Controls.Add(this.MainTabControl);
			this.Controls.Add(this.panel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "JohnnyInputSettings";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Configure Input Settings";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.JohnnyInputSettings_FormClosed);
			this.Load += new System.EventHandler(this.JohnnyInputSettings_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JohnnyInputSettings_KeyDown);
			this.panel1.ResumeLayout(false);
			this.MainTabControl.ResumeLayout(false);
			this.Player1Tab.ResumeLayout(false);
			this.ControllerPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ControlsGridView)).EndInit();
			this.panel4.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button CloseButton;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button OKButton;
		private System.Windows.Forms.TabControl MainTabControl;
		private System.Windows.Forms.TabPage Player1Tab;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.Timer JoystickWatchTimer;
		private System.Windows.Forms.Timer RefreshJoystickListTimer;
		private System.Windows.Forms.TabPage Player2Tab;
		private System.Windows.Forms.TabPage Player3Tab;
		private System.Windows.Forms.TabPage Player4Tab;
		private System.Windows.Forms.TabPage Player5Tab;
		private System.Windows.Forms.TabPage Player6Tab;
		private System.Windows.Forms.Panel ControllerPanel;
		private System.Windows.Forms.DataGridView ControlsGridView;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Label CurrentDevicesLabel;
		private ControllerInfo controllerInfo;
		private ControllerPreview controllerPreview;
		private System.Windows.Forms.TabPage ConsoleTab;
		private System.Windows.Forms.Button ResetButton;
	}
}