namespace FourDO.UI
{
	partial class GameInformation
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
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.OKButton = new System.Windows.Forms.Button();
			this.GameNameLabel = new System.Windows.Forms.Label();
			this.GameNameTextBox = new System.Windows.Forms.TextBox();
			this.GameIdTextBox = new System.Windows.Forms.TextBox();
			this.GameIdLabel = new System.Windows.Forms.Label();
			this.ChecksumTextBox = new System.Windows.Forms.TextBox();
			this.ChecksumLabel = new System.Windows.Forms.Label();
			this.ReleaseYearTextBox = new System.Windows.Forms.TextBox();
			this.ReleaseYearLabel = new System.Windows.Forms.Label();
			this.PublisherTextBox = new System.Windows.Forms.TextBox();
			this.PublisherLabel = new System.Windows.Forms.Label();
			this.RegionsTextBox = new System.Windows.Forms.TextBox();
			this.RegionLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::FourDO.Properties.Images.CD;
			this.pictureBox1.Location = new System.Drawing.Point(30, 32);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(176, 156);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// OKButton
			// 
			this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.OKButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.OKButton.Location = new System.Drawing.Point(526, 220);
			this.OKButton.Name = "OKButton";
			this.OKButton.Size = new System.Drawing.Size(75, 23);
			this.OKButton.TabIndex = 12;
			this.OKButton.Text = "&OK";
			this.OKButton.UseVisualStyleBackColor = true;
			this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
			// 
			// GameNameLabel
			// 
			this.GameNameLabel.AutoSize = true;
			this.GameNameLabel.Location = new System.Drawing.Point(230, 63);
			this.GameNameLabel.Name = "GameNameLabel";
			this.GameNameLabel.Size = new System.Drawing.Size(69, 13);
			this.GameNameLabel.TabIndex = 0;
			this.GameNameLabel.Text = "Game Name:";
			// 
			// GameNameTextBox
			// 
			this.GameNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.GameNameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GameNameTextBox.Location = new System.Drawing.Point(310, 60);
			this.GameNameTextBox.Name = "GameNameTextBox";
			this.GameNameTextBox.ReadOnly = true;
			this.GameNameTextBox.Size = new System.Drawing.Size(291, 20);
			this.GameNameTextBox.TabIndex = 1;
			this.GameNameTextBox.Text = "Game Name";
			// 
			// GameIdTextBox
			// 
			this.GameIdTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.GameIdTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GameIdTextBox.Location = new System.Drawing.Point(310, 79);
			this.GameIdTextBox.Name = "GameIdTextBox";
			this.GameIdTextBox.ReadOnly = true;
			this.GameIdTextBox.Size = new System.Drawing.Size(291, 20);
			this.GameIdTextBox.TabIndex = 3;
			this.GameIdTextBox.Text = "Game Id";
			// 
			// GameIdLabel
			// 
			this.GameIdLabel.AutoSize = true;
			this.GameIdLabel.Location = new System.Drawing.Point(230, 82);
			this.GameIdLabel.Name = "GameIdLabel";
			this.GameIdLabel.Size = new System.Drawing.Size(50, 13);
			this.GameIdLabel.TabIndex = 2;
			this.GameIdLabel.Text = "Game Id:";
			// 
			// ChecksumTextBox
			// 
			this.ChecksumTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ChecksumTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ChecksumTextBox.Location = new System.Drawing.Point(351, 181);
			this.ChecksumTextBox.Name = "ChecksumTextBox";
			this.ChecksumTextBox.ReadOnly = true;
			this.ChecksumTextBox.Size = new System.Drawing.Size(250, 20);
			this.ChecksumTextBox.TabIndex = 11;
			this.ChecksumTextBox.Text = "Checksum";
			// 
			// ChecksumLabel
			// 
			this.ChecksumLabel.AutoSize = true;
			this.ChecksumLabel.Location = new System.Drawing.Point(230, 181);
			this.ChecksumLabel.Name = "ChecksumLabel";
			this.ChecksumLabel.Size = new System.Drawing.Size(115, 13);
			this.ChecksumLabel.TabIndex = 10;
			this.ChecksumLabel.Text = "Sector 0+1 Checksum:";
			// 
			// ReleaseYearTextBox
			// 
			this.ReleaseYearTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ReleaseYearTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ReleaseYearTextBox.Location = new System.Drawing.Point(310, 98);
			this.ReleaseYearTextBox.Name = "ReleaseYearTextBox";
			this.ReleaseYearTextBox.ReadOnly = true;
			this.ReleaseYearTextBox.Size = new System.Drawing.Size(291, 20);
			this.ReleaseYearTextBox.TabIndex = 5;
			this.ReleaseYearTextBox.Text = "Release Year";
			// 
			// ReleaseYearLabel
			// 
			this.ReleaseYearLabel.AutoSize = true;
			this.ReleaseYearLabel.Location = new System.Drawing.Point(230, 101);
			this.ReleaseYearLabel.Name = "ReleaseYearLabel";
			this.ReleaseYearLabel.Size = new System.Drawing.Size(74, 13);
			this.ReleaseYearLabel.TabIndex = 4;
			this.ReleaseYearLabel.Text = "Release Year:";
			// 
			// PublisherTextBox
			// 
			this.PublisherTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PublisherTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.PublisherTextBox.Location = new System.Drawing.Point(310, 117);
			this.PublisherTextBox.Name = "PublisherTextBox";
			this.PublisherTextBox.ReadOnly = true;
			this.PublisherTextBox.Size = new System.Drawing.Size(291, 20);
			this.PublisherTextBox.TabIndex = 7;
			this.PublisherTextBox.Text = "Publisher";
			// 
			// PublisherLabel
			// 
			this.PublisherLabel.AutoSize = true;
			this.PublisherLabel.Location = new System.Drawing.Point(230, 120);
			this.PublisherLabel.Name = "PublisherLabel";
			this.PublisherLabel.Size = new System.Drawing.Size(53, 13);
			this.PublisherLabel.TabIndex = 6;
			this.PublisherLabel.Text = "Publisher:";
			// 
			// RegionsTextBox
			// 
			this.RegionsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.RegionsTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.RegionsTextBox.Location = new System.Drawing.Point(310, 136);
			this.RegionsTextBox.Name = "RegionsTextBox";
			this.RegionsTextBox.ReadOnly = true;
			this.RegionsTextBox.Size = new System.Drawing.Size(291, 20);
			this.RegionsTextBox.TabIndex = 9;
			this.RegionsTextBox.Text = "Regions";
			// 
			// RegionLabel
			// 
			this.RegionLabel.AutoSize = true;
			this.RegionLabel.Location = new System.Drawing.Point(230, 139);
			this.RegionLabel.Name = "RegionLabel";
			this.RegionLabel.Size = new System.Drawing.Size(55, 13);
			this.RegionLabel.TabIndex = 8;
			this.RegionLabel.Text = "Region(s):";
			// 
			// GameInformation
			// 
			this.AcceptButton = this.OKButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.OKButton;
			this.ClientSize = new System.Drawing.Size(613, 255);
			this.Controls.Add(this.RegionsTextBox);
			this.Controls.Add(this.RegionLabel);
			this.Controls.Add(this.PublisherTextBox);
			this.Controls.Add(this.PublisherLabel);
			this.Controls.Add(this.ReleaseYearTextBox);
			this.Controls.Add(this.ReleaseYearLabel);
			this.Controls.Add(this.ChecksumTextBox);
			this.Controls.Add(this.ChecksumLabel);
			this.Controls.Add(this.GameIdTextBox);
			this.Controls.Add(this.GameIdLabel);
			this.Controls.Add(this.GameNameTextBox);
			this.Controls.Add(this.GameNameLabel);
			this.Controls.Add(this.OKButton);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GameInformation";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Game Information";
			this.Load += new System.EventHandler(this.GameInformation_Load);
			this.Shown += new System.EventHandler(this.GameInformation_Shown);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button OKButton;
		private System.Windows.Forms.Label GameNameLabel;
		private System.Windows.Forms.TextBox GameNameTextBox;
		private System.Windows.Forms.TextBox GameIdTextBox;
		private System.Windows.Forms.Label GameIdLabel;
		private System.Windows.Forms.TextBox ChecksumTextBox;
		private System.Windows.Forms.Label ChecksumLabel;
		private System.Windows.Forms.TextBox ReleaseYearTextBox;
		private System.Windows.Forms.Label ReleaseYearLabel;
		private System.Windows.Forms.TextBox PublisherTextBox;
		private System.Windows.Forms.Label PublisherLabel;
		private System.Windows.Forms.TextBox RegionsTextBox;
		private System.Windows.Forms.Label RegionLabel;
	}
}