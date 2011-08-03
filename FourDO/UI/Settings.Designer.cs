namespace FourDO.UI
{
    partial class Settings
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.CancelButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.OKButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.GraphicsPluginLabel = new System.Windows.Forms.Label();
            this.GraphicsPluginComboBox = new System.Windows.Forms.ComboBox();
            this.ConfigureGraphicsButton = new System.Windows.Forms.Button();
            this.ConfigureAudioButton = new System.Windows.Forms.Button();
            this.AudioPluginComboBox = new System.Windows.Forms.ComboBox();
            this.AudioPluginLabel = new System.Windows.Forms.Label();
            this.ConfigureInputButton = new System.Windows.Forms.Button();
            this.InputPluginComboBox = new System.Windows.Forms.ComboBox();
            this.InputPluginLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.CancelButton);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.OKButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 389);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(468, 50);
            this.panel1.TabIndex = 0;
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButton.Location = new System.Drawing.Point(381, 16);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 1;
            this.CancelButton.Text = "&Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(468, 10);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.Location = new System.Drawing.Point(300, 16);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 0;
            this.OKButton.Text = "&OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Window;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Controls.Add(this.TitleLabel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(468, 61);
            this.panel2.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::FourDO.Properties.Resources.FourDO;
            this.pictureBox1.Location = new System.Drawing.Point(23, 20);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 20);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TitleLabel.Location = new System.Drawing.Point(51, 22);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(203, 13);
            this.TitleLabel.TabIndex = 0;
            this.TitleLabel.Text = "Configure various FourDO Settings";
            // 
            // GraphicsPluginLabel
            // 
            this.GraphicsPluginLabel.AutoSize = true;
            this.GraphicsPluginLabel.Location = new System.Drawing.Point(33, 109);
            this.GraphicsPluginLabel.Name = "GraphicsPluginLabel";
            this.GraphicsPluginLabel.Size = new System.Drawing.Size(84, 13);
            this.GraphicsPluginLabel.TabIndex = 2;
            this.GraphicsPluginLabel.Text = "&Graphics Plugin:";
            // 
            // GraphicsPluginComboBox
            // 
            this.GraphicsPluginComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.GraphicsPluginComboBox.Enabled = false;
            this.GraphicsPluginComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GraphicsPluginComboBox.FormattingEnabled = true;
            this.GraphicsPluginComboBox.Items.AddRange(new object[] {
            "(Built-in only)"});
            this.GraphicsPluginComboBox.Location = new System.Drawing.Point(123, 106);
            this.GraphicsPluginComboBox.Name = "GraphicsPluginComboBox";
            this.GraphicsPluginComboBox.Size = new System.Drawing.Size(215, 21);
            this.GraphicsPluginComboBox.TabIndex = 3;
            // 
            // ConfigureGraphicsButton
            // 
            this.ConfigureGraphicsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ConfigureGraphicsButton.Enabled = false;
            this.ConfigureGraphicsButton.Location = new System.Drawing.Point(344, 104);
            this.ConfigureGraphicsButton.Name = "ConfigureGraphicsButton";
            this.ConfigureGraphicsButton.Size = new System.Drawing.Size(75, 23);
            this.ConfigureGraphicsButton.TabIndex = 4;
            this.ConfigureGraphicsButton.Text = "Configure...";
            this.ConfigureGraphicsButton.UseVisualStyleBackColor = true;
            // 
            // ConfigureAudioButton
            // 
            this.ConfigureAudioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ConfigureAudioButton.Enabled = false;
            this.ConfigureAudioButton.Location = new System.Drawing.Point(344, 140);
            this.ConfigureAudioButton.Name = "ConfigureAudioButton";
            this.ConfigureAudioButton.Size = new System.Drawing.Size(75, 23);
            this.ConfigureAudioButton.TabIndex = 7;
            this.ConfigureAudioButton.Text = "Configure...";
            this.ConfigureAudioButton.UseVisualStyleBackColor = true;
            this.ConfigureAudioButton.Click += new System.EventHandler(this.ConfigureAudioButton_Click);
            // 
            // AudioPluginComboBox
            // 
            this.AudioPluginComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.AudioPluginComboBox.Enabled = false;
            this.AudioPluginComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AudioPluginComboBox.FormattingEnabled = true;
            this.AudioPluginComboBox.Items.AddRange(new object[] {
            "(Built-in only)"});
            this.AudioPluginComboBox.Location = new System.Drawing.Point(123, 142);
            this.AudioPluginComboBox.Name = "AudioPluginComboBox";
            this.AudioPluginComboBox.Size = new System.Drawing.Size(215, 21);
            this.AudioPluginComboBox.TabIndex = 6;
            // 
            // AudioPluginLabel
            // 
            this.AudioPluginLabel.AutoSize = true;
            this.AudioPluginLabel.Location = new System.Drawing.Point(33, 145);
            this.AudioPluginLabel.Name = "AudioPluginLabel";
            this.AudioPluginLabel.Size = new System.Drawing.Size(69, 13);
            this.AudioPluginLabel.TabIndex = 5;
            this.AudioPluginLabel.Text = "&Audio Plugin:";
            // 
            // ConfigureInputButton
            // 
            this.ConfigureInputButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ConfigureInputButton.Location = new System.Drawing.Point(344, 176);
            this.ConfigureInputButton.Name = "ConfigureInputButton";
            this.ConfigureInputButton.Size = new System.Drawing.Size(75, 23);
            this.ConfigureInputButton.TabIndex = 10;
            this.ConfigureInputButton.Text = "Configure...";
            this.ConfigureInputButton.UseVisualStyleBackColor = true;
            this.ConfigureInputButton.Click += new System.EventHandler(this.ConfigureInputButton_Click);
            // 
            // InputPluginComboBox
            // 
            this.InputPluginComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.InputPluginComboBox.Enabled = false;
            this.InputPluginComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InputPluginComboBox.FormattingEnabled = true;
            this.InputPluginComboBox.Items.AddRange(new object[] {
            "(Built-in only)"});
            this.InputPluginComboBox.Location = new System.Drawing.Point(123, 178);
            this.InputPluginComboBox.Name = "InputPluginComboBox";
            this.InputPluginComboBox.Size = new System.Drawing.Size(215, 21);
            this.InputPluginComboBox.TabIndex = 9;
            // 
            // InputPluginLabel
            // 
            this.InputPluginLabel.AutoSize = true;
            this.InputPluginLabel.Location = new System.Drawing.Point(33, 181);
            this.InputPluginLabel.Name = "InputPluginLabel";
            this.InputPluginLabel.Size = new System.Drawing.Size(66, 13);
            this.InputPluginLabel.TabIndex = 8;
            this.InputPluginLabel.Text = "&Input Plugin:";
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 439);
            this.Controls.Add(this.ConfigureInputButton);
            this.Controls.Add(this.InputPluginComboBox);
            this.Controls.Add(this.InputPluginLabel);
            this.Controls.Add(this.ConfigureAudioButton);
            this.Controls.Add(this.AudioPluginComboBox);
            this.Controls.Add(this.AudioPluginLabel);
            this.Controls.Add(this.ConfigureGraphicsButton);
            this.Controls.Add(this.GraphicsPluginComboBox);
            this.Controls.Add(this.GraphicsPluginLabel);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.Label GraphicsPluginLabel;
        private System.Windows.Forms.ComboBox GraphicsPluginComboBox;
        private System.Windows.Forms.Button ConfigureGraphicsButton;
        private System.Windows.Forms.Button ConfigureAudioButton;
        private System.Windows.Forms.ComboBox AudioPluginComboBox;
        private System.Windows.Forms.Label AudioPluginLabel;
        private System.Windows.Forms.Button ConfigureInputButton;
        private System.Windows.Forms.ComboBox InputPluginComboBox;
        private System.Windows.Forms.Label InputPluginLabel;
    }
}