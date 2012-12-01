namespace FourDO.UI.Controls
{
    partial class SizeBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SizeBox));
            this.WidthPixLabel = new System.Windows.Forms.Label();
            this.HideTimer = new System.Windows.Forms.Timer(this.components);
            this.HorizontalBox = new System.Windows.Forms.PictureBox();
            this.VerticalBox = new System.Windows.Forms.PictureBox();
            this.WidthPercentLabel = new System.Windows.Forms.Label();
            this.HeightPercentLabel = new System.Windows.Forms.Label();
            this.HeightPixLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.HorizontalBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.VerticalBox)).BeginInit();
            this.SuspendLayout();
            // 
            // WidthPixLabel
            // 
            this.WidthPixLabel.AutoSize = true;
            this.WidthPixLabel.Location = new System.Drawing.Point(69, 5);
            this.WidthPixLabel.Name = "WidthPixLabel";
            this.WidthPixLabel.Size = new System.Drawing.Size(34, 13);
            this.WidthPixLabel.TabIndex = 0;
            this.WidthPixLabel.Text = "1920 ";
            this.WidthPixLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // HideTimer
            // 
            this.HideTimer.Interval = 3000;
            this.HideTimer.Tick += new System.EventHandler(this.HideTimer_Tick);
            // 
            // HorizontalBox
            // 
            this.HorizontalBox.Image = ((System.Drawing.Image)(resources.GetObject("HorizontalBox.Image")));
            this.HorizontalBox.InitialImage = null;
            this.HorizontalBox.Location = new System.Drawing.Point(4, 4);
            this.HorizontalBox.Name = "HorizontalBox";
            this.HorizontalBox.Size = new System.Drawing.Size(16, 16);
            this.HorizontalBox.TabIndex = 1;
            this.HorizontalBox.TabStop = false;
            // 
            // VerticalBox
            // 
            this.VerticalBox.Image = ((System.Drawing.Image)(resources.GetObject("VerticalBox.Image")));
            this.VerticalBox.InitialImage = null;
            this.VerticalBox.Location = new System.Drawing.Point(124, 4);
            this.VerticalBox.Name = "VerticalBox";
            this.VerticalBox.Size = new System.Drawing.Size(16, 16);
            this.VerticalBox.TabIndex = 2;
            this.VerticalBox.TabStop = false;
            // 
            // WidthPercentLabel
            // 
            this.WidthPercentLabel.AutoSize = true;
            this.WidthPercentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WidthPercentLabel.Location = new System.Drawing.Point(26, 5);
            this.WidthPercentLabel.Name = "WidthPercentLabel";
            this.WidthPercentLabel.Size = new System.Drawing.Size(37, 13);
            this.WidthPercentLabel.TabIndex = 3;
            this.WidthPercentLabel.Text = "800%";
            this.WidthPercentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // HeightPercentLabel
            // 
            this.HeightPercentLabel.AutoSize = true;
            this.HeightPercentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HeightPercentLabel.Location = new System.Drawing.Point(146, 5);
            this.HeightPercentLabel.Name = "HeightPercentLabel";
            this.HeightPercentLabel.Size = new System.Drawing.Size(37, 13);
            this.HeightPercentLabel.TabIndex = 5;
            this.HeightPercentLabel.Text = "800%";
            this.HeightPercentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // HeightPixLabel
            // 
            this.HeightPixLabel.AutoSize = true;
            this.HeightPixLabel.Location = new System.Drawing.Point(189, 5);
            this.HeightPixLabel.Name = "HeightPixLabel";
            this.HeightPixLabel.Size = new System.Drawing.Size(34, 13);
            this.HeightPixLabel.TabIndex = 4;
            this.HeightPixLabel.Text = "1920 ";
            this.HeightPixLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SizeBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.Controls.Add(this.HeightPercentLabel);
            this.Controls.Add(this.HeightPixLabel);
            this.Controls.Add(this.WidthPercentLabel);
            this.Controls.Add(this.VerticalBox);
            this.Controls.Add(this.HorizontalBox);
            this.Controls.Add(this.WidthPixLabel);
            this.DoubleBuffered = true;
            this.Name = "SizeBox";
            this.Size = new System.Drawing.Size(237, 24);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.SizeBox_Paint);
            this.Resize += new System.EventHandler(this.SizeBox_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.HorizontalBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.VerticalBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label WidthPixLabel;
        private System.Windows.Forms.Timer HideTimer;
        private System.Windows.Forms.PictureBox HorizontalBox;
        private System.Windows.Forms.PictureBox VerticalBox;
        private System.Windows.Forms.Label WidthPercentLabel;
        private System.Windows.Forms.Label HeightPercentLabel;
        private System.Windows.Forms.Label HeightPixLabel;

    }
}
