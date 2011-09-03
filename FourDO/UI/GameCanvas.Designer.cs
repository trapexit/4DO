namespace FourDO.UI
{
    partial class GameCanvas
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
			this.slimDXCanvas = new FourDO.UI.DX.SlimDXCanvas();
			this.SuspendLayout();
			// 
			// slimDXCanvas
			// 
			this.slimDXCanvas.Location = new System.Drawing.Point(139, 0);
			this.slimDXCanvas.Name = "slimDXCanvas";
			this.slimDXCanvas.Size = new System.Drawing.Size(147, 238);
			this.slimDXCanvas.TabIndex = 0;
			// 
			// GameCanvas
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.BackgroundImage = global::FourDO.Properties.Resources.VoidImage;
			this.Controls.Add(this.slimDXCanvas);
			this.DoubleBuffered = true;
			this.Name = "GameCanvas";
			this.Size = new System.Drawing.Size(286, 238);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.GameCanvas_Paint);
			this.Resize += new System.EventHandler(this.GameCanvas_Resize);
			this.ResumeLayout(false);

        }

        #endregion

		private DX.SlimDXCanvas slimDXCanvas;
    }
}
