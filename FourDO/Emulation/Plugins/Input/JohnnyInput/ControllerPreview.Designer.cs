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
			this.CPanel = new System.Windows.Forms.Panel();
			this.BPanel = new System.Windows.Forms.Panel();
			this.APanel = new System.Windows.Forms.Panel();
			this.PPanel = new System.Windows.Forms.Panel();
			this.XPanel = new System.Windows.Forms.Panel();
			this.LeftPanel = new System.Windows.Forms.Panel();
			this.RightPanel = new System.Windows.Forms.Panel();
			this.DownPanel = new System.Windows.Forms.Panel();
			this.UpPanel = new System.Windows.Forms.Panel();
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
			this.ButtonPositionPanel.Controls.Add(this.CPanel);
			this.ButtonPositionPanel.Controls.Add(this.BPanel);
			this.ButtonPositionPanel.Controls.Add(this.APanel);
			this.ButtonPositionPanel.Controls.Add(this.PPanel);
			this.ButtonPositionPanel.Controls.Add(this.XPanel);
			this.ButtonPositionPanel.Controls.Add(this.LeftPanel);
			this.ButtonPositionPanel.Controls.Add(this.RightPanel);
			this.ButtonPositionPanel.Controls.Add(this.DownPanel);
			this.ButtonPositionPanel.Controls.Add(this.UpPanel);
			this.ButtonPositionPanel.Controls.Add(this.RPanel);
			this.ButtonPositionPanel.Controls.Add(this.LPanel);
			this.ButtonPositionPanel.Location = new System.Drawing.Point(0, 0);
			this.ButtonPositionPanel.Name = "ButtonPositionPanel";
			this.ButtonPositionPanel.Size = new System.Drawing.Size(221, 100);
			this.ButtonPositionPanel.TabIndex = 12;
			// 
			// CPanel
			// 
			this.CPanel.BackColor = System.Drawing.Color.Transparent;
			this.CPanel.Location = new System.Drawing.Point(185, 31);
			this.CPanel.Name = "CPanel";
			this.CPanel.Size = new System.Drawing.Size(22, 22);
			this.CPanel.TabIndex = 26;
			this.CPanel.Tag = "C Button";
			// 
			// BPanel
			// 
			this.BPanel.BackColor = System.Drawing.Color.Transparent;
			this.BPanel.Location = new System.Drawing.Point(161, 36);
			this.BPanel.Name = "BPanel";
			this.BPanel.Size = new System.Drawing.Size(23, 22);
			this.BPanel.TabIndex = 25;
			this.BPanel.Tag = "B Button";
			// 
			// APanel
			// 
			this.APanel.BackColor = System.Drawing.Color.Transparent;
			this.APanel.Location = new System.Drawing.Point(141, 49);
			this.APanel.Name = "APanel";
			this.APanel.Size = new System.Drawing.Size(22, 22);
			this.APanel.TabIndex = 23;
			this.APanel.Tag = "A Button";
			// 
			// PPanel
			// 
			this.PPanel.BackColor = System.Drawing.Color.Transparent;
			this.PPanel.Location = new System.Drawing.Point(116, 65);
			this.PPanel.Name = "PPanel";
			this.PPanel.Size = new System.Drawing.Size(9, 9);
			this.PPanel.TabIndex = 24;
			this.PPanel.Tag = "P Button (\"Play/Pause\")";
			// 
			// XPanel
			// 
			this.XPanel.BackColor = System.Drawing.Color.Transparent;
			this.XPanel.Location = new System.Drawing.Point(97, 65);
			this.XPanel.Name = "XPanel";
			this.XPanel.Size = new System.Drawing.Size(9, 9);
			this.XPanel.TabIndex = 22;
			this.XPanel.Tag = "X Button (\"Stop\")";
			// 
			// LeftPanel
			// 
			this.LeftPanel.BackColor = System.Drawing.Color.Transparent;
			this.LeftPanel.Location = new System.Drawing.Point(27, 48);
			this.LeftPanel.Name = "LeftPanel";
			this.LeftPanel.Size = new System.Drawing.Size(15, 10);
			this.LeftPanel.TabIndex = 21;
			this.LeftPanel.Tag = "Left (\"Previous Track\")";
			// 
			// RightPanel
			// 
			this.RightPanel.BackColor = System.Drawing.Color.Transparent;
			this.RightPanel.Location = new System.Drawing.Point(52, 48);
			this.RightPanel.Name = "RightPanel";
			this.RightPanel.Size = new System.Drawing.Size(15, 10);
			this.RightPanel.TabIndex = 19;
			this.RightPanel.Tag = "Right (\"Next Track\")";
			// 
			// DownPanel
			// 
			this.DownPanel.BackColor = System.Drawing.Color.Transparent;
			this.DownPanel.Location = new System.Drawing.Point(42, 57);
			this.DownPanel.Name = "DownPanel";
			this.DownPanel.Size = new System.Drawing.Size(10, 16);
			this.DownPanel.TabIndex = 18;
			this.DownPanel.Tag = "Down (\"Seek Backwards\")";
			// 
			// UpPanel
			// 
			this.UpPanel.BackColor = System.Drawing.Color.Transparent;
			this.UpPanel.Location = new System.Drawing.Point(42, 33);
			this.UpPanel.Name = "UpPanel";
			this.UpPanel.Size = new System.Drawing.Size(10, 16);
			this.UpPanel.TabIndex = 17;
			this.UpPanel.Tag = "Up (\"Seek Forward\")";
			// 
			// RPanel
			// 
			this.RPanel.BackColor = System.Drawing.Color.Transparent;
			this.RPanel.Location = new System.Drawing.Point(160, 0);
			this.RPanel.Name = "RPanel";
			this.RPanel.Size = new System.Drawing.Size(44, 20);
			this.RPanel.TabIndex = 16;
			this.RPanel.Tag = "R (Right Shoulder)";
			// 
			// LPanel
			// 
			this.LPanel.BackColor = System.Drawing.Color.Transparent;
			this.LPanel.Location = new System.Drawing.Point(18, -1);
			this.LPanel.Name = "LPanel";
			this.LPanel.Size = new System.Drawing.Size(45, 20);
			this.LPanel.TabIndex = 15;
			this.LPanel.Tag = "L (Left Shoulder)";
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
			this.MouseLeave += new System.EventHandler(this.ControllerPreview_MouseLeave);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ControllerPreview_MouseMove);
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
        private System.Windows.Forms.Panel CPanel;
        private System.Windows.Forms.Panel BPanel;
        private System.Windows.Forms.Panel APanel;
        private System.Windows.Forms.Panel PPanel;
        private System.Windows.Forms.Panel XPanel;
        private System.Windows.Forms.Panel LeftPanel;
        private System.Windows.Forms.Panel RightPanel;
        private System.Windows.Forms.Panel DownPanel;
        private System.Windows.Forms.Panel UpPanel;
	}
}
