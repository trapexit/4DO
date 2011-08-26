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
			this.CloseButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.OKButton = new System.Windows.Forms.Button();
			this.MainTabControl = new System.Windows.Forms.TabControl();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.panel6 = new System.Windows.Forms.Panel();
			this.ControlsGridView = new System.Windows.Forms.DataGridView();
			this.panel4 = new System.Windows.Forms.Panel();
			this.controllerPreview = new FourDO.Emulation.Plugins.Input.JohnnyInput.ControllerPreview();
			this.panel5 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.RemoveDeviceButton = new System.Windows.Forms.Button();
			this.DeviceTypeComboBox = new System.Windows.Forms.ComboBox();
			this.DeviceTypeLabel = new System.Windows.Forms.Label();
			this.JoystickTimer = new System.Windows.Forms.Timer(this.components);
			this.panel1.SuspendLayout();
			this.MainTabControl.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.panel6.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ControlsGridView)).BeginInit();
			this.panel4.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.CloseButton);
			this.panel1.Controls.Add(this.groupBox1);
			this.panel1.Controls.Add(this.OKButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 406);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(691, 42);
			this.panel1.TabIndex = 1;
			// 
			// CloseButton
			// 
			this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CloseButton.Location = new System.Drawing.Point(607, 10);
			this.CloseButton.Name = "CloseButton";
			this.CloseButton.Size = new System.Drawing.Size(75, 23);
			this.CloseButton.TabIndex = 1;
			this.CloseButton.Text = "&Cancel";
			this.CloseButton.UseVisualStyleBackColor = true;
			this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(691, 2);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			// 
			// OKButton
			// 
			this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.OKButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.OKButton.Location = new System.Drawing.Point(526, 10);
			this.OKButton.Name = "OKButton";
			this.OKButton.Size = new System.Drawing.Size(75, 23);
			this.OKButton.TabIndex = 0;
			this.OKButton.Text = "&OK";
			this.OKButton.UseVisualStyleBackColor = true;
			this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
			// 
			// MainTabControl
			// 
			this.MainTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.MainTabControl.Controls.Add(this.tabPage2);
			this.MainTabControl.Location = new System.Drawing.Point(12, 12);
			this.MainTabControl.Name = "MainTabControl";
			this.MainTabControl.SelectedIndex = 0;
			this.MainTabControl.Size = new System.Drawing.Size(667, 385);
			this.MainTabControl.TabIndex = 0;
			// 
			// tabPage2
			// 
			this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
			this.tabPage2.Controls.Add(this.panel6);
			this.tabPage2.Controls.Add(this.panel5);
			this.tabPage2.Controls.Add(this.panel2);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(659, 359);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Device #1 - Control Pad";
			// 
			// panel6
			// 
			this.panel6.Controls.Add(this.ControlsGridView);
			this.panel6.Controls.Add(this.panel4);
			this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel6.Location = new System.Drawing.Point(3, 39);
			this.panel6.Name = "panel6";
			this.panel6.Size = new System.Drawing.Size(650, 317);
			this.panel6.TabIndex = 20;
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
			this.ControlsGridView.Location = new System.Drawing.Point(270, 0);
			this.ControlsGridView.MultiSelect = false;
			this.ControlsGridView.Name = "ControlsGridView";
			this.ControlsGridView.ReadOnly = true;
			this.ControlsGridView.RowHeadersVisible = false;
			this.ControlsGridView.RowTemplate.Height = 18;
			this.ControlsGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
			this.ControlsGridView.ShowCellToolTips = false;
			this.ControlsGridView.Size = new System.Drawing.Size(380, 317);
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
			this.panel4.Controls.Add(this.controllerPreview);
			this.panel4.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel4.Location = new System.Drawing.Point(0, 0);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(270, 317);
			this.panel4.TabIndex = 19;
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
			this.controllerPreview.Size = new System.Drawing.Size(264, 317);
			this.controllerPreview.TabIndex = 0;
			// 
			// panel5
			// 
			this.panel5.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel5.Location = new System.Drawing.Point(653, 39);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(3, 317);
			this.panel5.TabIndex = 15;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.RemoveDeviceButton);
			this.panel2.Controls.Add(this.DeviceTypeComboBox);
			this.panel2.Controls.Add(this.DeviceTypeLabel);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(3, 3);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(653, 36);
			this.panel2.TabIndex = 0;
			// 
			// RemoveDeviceButton
			// 
			this.RemoveDeviceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.RemoveDeviceButton.Enabled = false;
			this.RemoveDeviceButton.Location = new System.Drawing.Point(520, 5);
			this.RemoveDeviceButton.Name = "RemoveDeviceButton";
			this.RemoveDeviceButton.Size = new System.Drawing.Size(130, 23);
			this.RemoveDeviceButton.TabIndex = 2;
			this.RemoveDeviceButton.Text = "&Remove This Device";
			this.RemoveDeviceButton.UseVisualStyleBackColor = true;
			// 
			// DeviceTypeComboBox
			// 
			this.DeviceTypeComboBox.Enabled = false;
			this.DeviceTypeComboBox.FormattingEnabled = true;
			this.DeviceTypeComboBox.Items.AddRange(new object[] {
            "Control Pad"});
			this.DeviceTypeComboBox.Location = new System.Drawing.Point(80, 7);
			this.DeviceTypeComboBox.Name = "DeviceTypeComboBox";
			this.DeviceTypeComboBox.Size = new System.Drawing.Size(154, 21);
			this.DeviceTypeComboBox.TabIndex = 1;
			// 
			// DeviceTypeLabel
			// 
			this.DeviceTypeLabel.AutoSize = true;
			this.DeviceTypeLabel.Location = new System.Drawing.Point(3, 10);
			this.DeviceTypeLabel.Name = "DeviceTypeLabel";
			this.DeviceTypeLabel.Size = new System.Drawing.Size(71, 13);
			this.DeviceTypeLabel.TabIndex = 0;
			this.DeviceTypeLabel.Text = "Device Type:";
			// 
			// JoystickTimer
			// 
			this.JoystickTimer.Enabled = true;
			this.JoystickTimer.Tick += new System.EventHandler(this.JoystickTimer_Tick);
			// 
			// JohnnyInputSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(691, 448);
			this.Controls.Add(this.MainTabControl);
			this.Controls.Add(this.panel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "JohnnyInputSettings";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Configure Input Settings";
			this.Load += new System.EventHandler(this.JohnnyInputSettings_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JohnnyInputSettings_KeyDown);
			this.panel1.ResumeLayout(false);
			this.MainTabControl.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.panel6.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ControlsGridView)).EndInit();
			this.panel4.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button OKButton;
		private System.Windows.Forms.TabControl MainTabControl;
        private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button RemoveDeviceButton;
		private System.Windows.Forms.ComboBox DeviceTypeComboBox;
        private System.Windows.Forms.Label DeviceTypeLabel;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.DataGridView ControlsGridView;
        private System.Windows.Forms.Panel panel4;
        private ControllerPreview controllerPreview;
		private System.Windows.Forms.Timer JoystickTimer;
    }
}