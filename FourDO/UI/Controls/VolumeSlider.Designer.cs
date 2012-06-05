namespace FourDO.UI.Controls
{
	partial class VolumeSlider
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.volumeTrackBar = new System.Windows.Forms.TrackBar();
			this.volumeLabel = new System.Windows.Forms.Label();
			this.VolumeMessageLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.volumeTrackBar)).BeginInit();
			this.SuspendLayout();
			// 
			// volumeTrackBar
			// 
			this.volumeTrackBar.BackColor = System.Drawing.Color.White;
			this.volumeTrackBar.LargeChange = 25;
			this.volumeTrackBar.Location = new System.Drawing.Point(93, 3);
			this.volumeTrackBar.Maximum = 100;
			this.volumeTrackBar.Name = "volumeTrackBar";
			this.volumeTrackBar.Size = new System.Drawing.Size(100, 42);
			this.volumeTrackBar.TabIndex = 0;
			this.volumeTrackBar.TickFrequency = 25;
			this.volumeTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
			this.volumeTrackBar.Value = 100;
			this.volumeTrackBar.Scroll += new System.EventHandler(this.volumeTrackBar_Scroll);
			this.volumeTrackBar.ValueChanged += new System.EventHandler(this.volumeTrackBar_ValueChanged);
			// 
			// volumeLabel
			// 
			this.volumeLabel.AutoSize = true;
			this.volumeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.volumeLabel.Location = new System.Drawing.Point(59, 7);
			this.volumeLabel.Name = "volumeLabel";
			this.volumeLabel.Size = new System.Drawing.Size(37, 13);
			this.volumeLabel.TabIndex = 5;
			this.volumeLabel.Text = "100%";
			// 
			// VolumeMessageLabel
			// 
			this.VolumeMessageLabel.AutoSize = true;
			this.VolumeMessageLabel.Location = new System.Drawing.Point(3, 6);
			this.VolumeMessageLabel.Name = "VolumeMessageLabel";
			this.VolumeMessageLabel.Size = new System.Drawing.Size(45, 13);
			this.VolumeMessageLabel.TabIndex = 4;
			this.VolumeMessageLabel.Text = "Volume:";
			// 
			// VolumeSlider
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.volumeLabel);
			this.Controls.Add(this.VolumeMessageLabel);
			this.Controls.Add(this.volumeTrackBar);
			this.Name = "VolumeSlider";
			this.Size = new System.Drawing.Size(195, 29);
			this.Load += new System.EventHandler(this.VolumeSlider_Load);
			((System.ComponentModel.ISupportInitialize)(this.volumeTrackBar)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TrackBar volumeTrackBar;
		private System.Windows.Forms.Label volumeLabel;
		private System.Windows.Forms.Label VolumeMessageLabel;
	}
}
