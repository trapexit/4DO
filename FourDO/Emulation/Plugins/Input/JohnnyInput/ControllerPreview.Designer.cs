namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	partial class ControllerPreview
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControllerPreview));
			this.tmrPulsate = new System.Windows.Forms.Timer(this.components);
			this.ButtonLabelPanel = new System.Windows.Forms.Panel();
			this.ButtonLabel = new System.Windows.Forms.Label();
			this.ButtonPositionPanel = new System.Windows.Forms.Panel();
			this.RPanel = new System.Windows.Forms.Panel();
			this.LPanel = new System.Windows.Forms.Panel();
			this.ButtonLabelPanel.SuspendLayout();
			this.ButtonPositionPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// tmrPulsate
			// 
			this.tmrPulsate.Tick += new System.EventHandler(this.tmrPulsate_Tick);
			// 
			// ButtonLabelPanel
			// 
			this.ButtonLabelPanel.BackColor = System.Drawing.Color.Transparent;
			this.ButtonLabelPanel.Controls.Add(this.ButtonLabel);
			this.ButtonLabelPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.ButtonLabelPanel.Location = new System.Drawing.Point(0, 420);
			this.ButtonLabelPanel.Name = "ButtonLabelPanel";
			this.ButtonLabelPanel.Size = new System.Drawing.Size(507, 33);
			this.ButtonLabelPanel.TabIndex = 11;
			// 
			// ButtonLabel
			// 
			this.ButtonLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ButtonLabel.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ButtonLabel.Location = new System.Drawing.Point(0, 0);
			this.ButtonLabel.Name = "ButtonLabel";
			this.ButtonLabel.Size = new System.Drawing.Size(507, 33);
			this.ButtonLabel.TabIndex = 10;
			this.ButtonLabel.Text = "X Button";
			this.ButtonLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ButtonPositionPanel
			// 
			this.ButtonPositionPanel.BackColor = System.Drawing.Color.Transparent;
			this.ButtonPositionPanel.Controls.Add(this.RPanel);
			this.ButtonPositionPanel.Controls.Add(this.LPanel);
			this.ButtonPositionPanel.Location = new System.Drawing.Point(0, 0);
			this.ButtonPositionPanel.Name = "ButtonPositionPanel";
			this.ButtonPositionPanel.Size = new System.Drawing.Size(221, 100);
			this.ButtonPositionPanel.TabIndex = 12;
			// 
			// RPanel
			// 
			this.RPanel.BackColor = System.Drawing.Color.Transparent;
			this.RPanel.Location = new System.Drawing.Point(159, -2);
			this.RPanel.Name = "RPanel";
			this.RPanel.Size = new System.Drawing.Size(46, 21);
			this.RPanel.TabIndex = 16;
			// 
			// LPanel
			// 
			this.LPanel.BackColor = System.Drawing.Color.Transparent;
			this.LPanel.Location = new System.Drawing.Point(17, -2);
			this.LPanel.Name = "LPanel";
			this.LPanel.Size = new System.Drawing.Size(49, 21);
			this.LPanel.TabIndex = 15;
			// 
			// ControllerPreview
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(205)))), ((int)(((byte)(230)))));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.Controls.Add(this.ButtonPositionPanel);
			this.Controls.Add(this.ButtonLabelPanel);
			this.DoubleBuffered = true;
			this.Name = "ControllerPreview";
			this.Size = new System.Drawing.Size(507, 453);
			this.Load += new System.EventHandler(this.ControllerPreview_Load);
			this.Resize += new System.EventHandler(this.ControllerPreview_Resize);
			this.ButtonLabelPanel.ResumeLayout(false);
			this.ButtonPositionPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Timer tmrPulsate;
		private System.Windows.Forms.Panel ButtonLabelPanel;
		private System.Windows.Forms.Label ButtonLabel;
		private System.Windows.Forms.Panel ButtonPositionPanel;
		private System.Windows.Forms.Panel RPanel;
		private System.Windows.Forms.Panel LPanel;
	}
}
