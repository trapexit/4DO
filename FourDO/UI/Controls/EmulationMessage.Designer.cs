namespace FourDO.UI.Controls
{
	partial class EmulationMessage
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
			this.HideTimer = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// HideTimer
			// 
			this.HideTimer.Interval = 1500;
			this.HideTimer.Tick += new System.EventHandler(this.HideTimer_Tick);
			// 
			// EmulationMessage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "EmulationMessage";
			this.Size = new System.Drawing.Size(644, 150);
			this.Load += new System.EventHandler(this.EmulationMessage_Load);
			this.Resize += new System.EventHandler(this.EmulationMessage_Resize);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Timer HideTimer;

	}
}
