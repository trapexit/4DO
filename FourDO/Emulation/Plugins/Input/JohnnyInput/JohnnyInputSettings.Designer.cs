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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JohnnyInputSettings));
			this.panel1 = new System.Windows.Forms.Panel();
			this.CloseButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.OKButton = new System.Windows.Forms.Button();
			this.MainTabControl = new System.Windows.Forms.TabControl();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.ControlsGridView = new System.Windows.Forms.DataGridView();
			this.panel5 = new System.Windows.Forms.Panel();
			this.panel4 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.ClearAllButton = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.RemoveDeviceButton = new System.Windows.Forms.Button();
			this.DeviceTypeComboBox = new System.Windows.Forms.ComboBox();
			this.DeviceTypeLabel = new System.Windows.Forms.Label();
			this.controllerPreview1 = new FourDO.Emulation.Plugins.Input.JohnnyInput.ControllerPreview();
			this.panel1.SuspendLayout();
			this.MainTabControl.SuspendLayout();
			this.tabPage2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ControlsGridView)).BeginInit();
			this.panel4.SuspendLayout();
			this.panel3.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.CloseButton);
			this.panel1.Controls.Add(this.groupBox1);
			this.panel1.Controls.Add(this.OKButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 386);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(550, 42);
			this.panel1.TabIndex = 1;
			// 
			// CloseButton
			// 
			this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CloseButton.Location = new System.Drawing.Point(466, 10);
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
			this.groupBox1.Size = new System.Drawing.Size(550, 2);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			// 
			// OKButton
			// 
			this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.OKButton.Location = new System.Drawing.Point(385, 10);
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
			this.MainTabControl.Size = new System.Drawing.Size(526, 365);
			this.MainTabControl.TabIndex = 6;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.ControlsGridView);
			this.tabPage2.Controls.Add(this.panel5);
			this.tabPage2.Controls.Add(this.panel4);
			this.tabPage2.Controls.Add(this.panel3);
			this.tabPage2.Controls.Add(this.panel2);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(518, 339);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Device #1 - Control Pad";
			this.tabPage2.UseVisualStyleBackColor = true;
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
			this.ControlsGridView.Location = new System.Drawing.Point(261, 39);
			this.ControlsGridView.MultiSelect = false;
			this.ControlsGridView.Name = "ControlsGridView";
			this.ControlsGridView.ReadOnly = true;
			this.ControlsGridView.RowHeadersVisible = false;
			this.ControlsGridView.RowTemplate.Height = 18;
			this.ControlsGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
			this.ControlsGridView.ShowCellToolTips = false;
			this.ControlsGridView.Size = new System.Drawing.Size(251, 262);
			this.ControlsGridView.TabIndex = 6;
			// 
			// panel5
			// 
			this.panel5.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel5.Location = new System.Drawing.Point(512, 39);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(3, 262);
			this.panel5.TabIndex = 15;
			// 
			// panel4
			// 
			this.panel4.Controls.Add(this.controllerPreview1);
			this.panel4.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel4.Location = new System.Drawing.Point(3, 39);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(258, 262);
			this.panel4.TabIndex = 12;
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.ClearAllButton);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel3.Location = new System.Drawing.Point(3, 301);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(512, 35);
			this.panel3.TabIndex = 10;
			// 
			// ClearAllButton
			// 
			this.ClearAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ClearAllButton.Location = new System.Drawing.Point(434, 9);
			this.ClearAllButton.Name = "ClearAllButton";
			this.ClearAllButton.Size = new System.Drawing.Size(75, 23);
			this.ClearAllButton.TabIndex = 3;
			this.ClearAllButton.Text = "Cl&ear All";
			this.ClearAllButton.UseVisualStyleBackColor = true;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.RemoveDeviceButton);
			this.panel2.Controls.Add(this.DeviceTypeComboBox);
			this.panel2.Controls.Add(this.DeviceTypeLabel);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(3, 3);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(512, 36);
			this.panel2.TabIndex = 8;
			// 
			// RemoveDeviceButton
			// 
			this.RemoveDeviceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.RemoveDeviceButton.Enabled = false;
			this.RemoveDeviceButton.Location = new System.Drawing.Point(379, 5);
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
			// controllerPreview1
			// 
			this.controllerPreview1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.controllerPreview1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(205)))), ((int)(((byte)(230)))));
			this.controllerPreview1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("controllerPreview1.BackgroundImage")));
			this.controllerPreview1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.controllerPreview1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.controllerPreview1.HighlightedButton = null;
			this.controllerPreview1.Location = new System.Drawing.Point(6, 0);
			this.controllerPreview1.Name = "controllerPreview1";
			this.controllerPreview1.Size = new System.Drawing.Size(246, 262);
			this.controllerPreview1.TabIndex = 2;
			// 
			// JohnnyInputSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CloseButton;
			this.ClientSize = new System.Drawing.Size(550, 428);
			this.Controls.Add(this.MainTabControl);
			this.Controls.Add(this.panel1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "JohnnyInputSettings";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Configure Input Settings";
			this.Load += new System.EventHandler(this.JohnnyInputSettings_Load);
			this.panel1.ResumeLayout(false);
			this.MainTabControl.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ControlsGridView)).EndInit();
			this.panel4.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
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
		private System.Windows.Forms.DataGridView ControlsGridView;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button RemoveDeviceButton;
		private System.Windows.Forms.ComboBox DeviceTypeComboBox;
		private System.Windows.Forms.Label DeviceTypeLabel;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Button ClearAllButton;
		private System.Windows.Forms.Panel panel5;
		private ControllerPreview controllerPreview1;
    }
}