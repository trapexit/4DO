namespace FourDO.UI.DiscBrowser
{
	partial class Browser
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Browser));
			this.MainStatusStrip = new System.Windows.Forms.StatusStrip();
			this.MainImageList = new System.Windows.Forms.ImageList(this.components);
			this.FileListView = new System.Windows.Forms.ListView();
			this.NameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SizeColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.IDColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ExtensionColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.BrowserContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.ExtractMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.ExtractDirectoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ExtractDiscMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.OpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.DirectoryTextBox = new System.Windows.Forms.TextBox();
			this.DirectoryUpButton = new System.Windows.Forms.Button();
			this.DirectoryNotFoundLabel = new System.Windows.Forms.Label();
			this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.MainStatusStrip.SuspendLayout();
			this.BrowserContextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// MainStatusStrip
			// 
			this.MainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
			this.MainStatusStrip.Location = new System.Drawing.Point(0, 349);
			this.MainStatusStrip.Name = "MainStatusStrip";
			this.MainStatusStrip.Size = new System.Drawing.Size(388, 22);
			this.MainStatusStrip.TabIndex = 1;
			this.MainStatusStrip.Text = "statusStrip1";
			// 
			// MainImageList
			// 
			this.MainImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("MainImageList.ImageStream")));
			this.MainImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.MainImageList.Images.SetKeyName(0, "Directory_Closed");
			this.MainImageList.Images.SetKeyName(1, "Directory_Open");
			this.MainImageList.Images.SetKeyName(2, "Directory_Up");
			this.MainImageList.Images.SetKeyName(3, "File");
			// 
			// FileListView
			// 
			this.FileListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.FileListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameColumn,
            this.SizeColumn,
            this.IDColumn,
            this.ExtensionColumn});
			this.FileListView.ContextMenuStrip = this.BrowserContextMenuStrip;
			this.FileListView.FullRowSelect = true;
			this.FileListView.Location = new System.Drawing.Point(4, 30);
			this.FileListView.Name = "FileListView";
			this.FileListView.Size = new System.Drawing.Size(381, 313);
			this.FileListView.SmallImageList = this.MainImageList;
			this.FileListView.TabIndex = 2;
			this.FileListView.UseCompatibleStateImageBehavior = false;
			this.FileListView.View = System.Windows.Forms.View.Details;
			this.FileListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.FileListView_ItemSelectionChanged);
			this.FileListView.DoubleClick += new System.EventHandler(this.FileListView_DoubleClick);
			this.FileListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FileListView_KeyDown);
			// 
			// NameColumn
			// 
			this.NameColumn.Text = "Name";
			// 
			// SizeColumn
			// 
			this.SizeColumn.Text = "Bytes";
			// 
			// IDColumn
			// 
			this.IDColumn.Text = "ID";
			// 
			// ExtensionColumn
			// 
			this.ExtensionColumn.Text = "Extension";
			// 
			// BrowserContextMenuStrip
			// 
			this.BrowserContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ExtractMenuItem,
            this.toolStripSeparator1,
            this.ExtractDirectoryMenuItem,
            this.ExtractDiscMenuItem,
            this.toolStripMenuItem1,
            this.OpenMenuItem});
			this.BrowserContextMenuStrip.Name = "BrowserContextMenuStrip";
			this.BrowserContextMenuStrip.Size = new System.Drawing.Size(224, 104);
			this.BrowserContextMenuStrip.VisibleChanged += new System.EventHandler(this.ContextMenuStrip_VisibleChanged);
			// 
			// ExtractMenuItem
			// 
			this.ExtractMenuItem.Name = "ExtractMenuItem";
			this.ExtractMenuItem.Size = new System.Drawing.Size(223, 22);
			this.ExtractMenuItem.Text = "&Extract To...";
			this.ExtractMenuItem.Click += new System.EventHandler(this.ExtractMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(220, 6);
			// 
			// ExtractDirectoryMenuItem
			// 
			this.ExtractDirectoryMenuItem.Name = "ExtractDirectoryMenuItem";
			this.ExtractDirectoryMenuItem.Size = new System.Drawing.Size(223, 22);
			this.ExtractDirectoryMenuItem.Text = "Extract &Current Directory To...";
			this.ExtractDirectoryMenuItem.Click += new System.EventHandler(this.ExtractDirectoryMenuItem_Click);
			// 
			// ExtractDiscMenuItem
			// 
			this.ExtractDiscMenuItem.Name = "ExtractDiscMenuItem";
			this.ExtractDiscMenuItem.Size = new System.Drawing.Size(223, 22);
			this.ExtractDiscMenuItem.Text = "Extract Whole &Disc To...";
			this.ExtractDiscMenuItem.Click += new System.EventHandler(this.ExtractDiscMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(220, 6);
			// 
			// OpenMenuItem
			// 
			this.OpenMenuItem.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.OpenMenuItem.Name = "OpenMenuItem";
			this.OpenMenuItem.Size = new System.Drawing.Size(223, 22);
			this.OpenMenuItem.Text = "&Open Directory";
			this.OpenMenuItem.Click += new System.EventHandler(this.OpenMenuItem_Click);
			// 
			// DirectoryTextBox
			// 
			this.DirectoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.DirectoryTextBox.Location = new System.Drawing.Point(68, 3);
			this.DirectoryTextBox.Name = "DirectoryTextBox";
			this.DirectoryTextBox.Size = new System.Drawing.Size(317, 21);
			this.DirectoryTextBox.TabIndex = 3;
			this.DirectoryTextBox.TextChanged += new System.EventHandler(this.DirectoryTextBox_TextChanged);
			// 
			// DirectoryUpButton
			// 
			this.DirectoryUpButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.DirectoryUpButton.ImageKey = "Directory_Up";
			this.DirectoryUpButton.ImageList = this.MainImageList;
			this.DirectoryUpButton.Location = new System.Drawing.Point(7, 3);
			this.DirectoryUpButton.Name = "DirectoryUpButton";
			this.DirectoryUpButton.Size = new System.Drawing.Size(55, 21);
			this.DirectoryUpButton.TabIndex = 5;
			this.DirectoryUpButton.Text = "  Up";
			this.DirectoryUpButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.DirectoryUpButton.UseVisualStyleBackColor = true;
			this.DirectoryUpButton.Click += new System.EventHandler(this.DirectoryUpButton_Click);
			// 
			// DirectoryNotFoundLabel
			// 
			this.DirectoryNotFoundLabel.BackColor = System.Drawing.SystemColors.Window;
			this.DirectoryNotFoundLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.DirectoryNotFoundLabel.Location = new System.Drawing.Point(4, 30);
			this.DirectoryNotFoundLabel.Name = "DirectoryNotFoundLabel";
			this.DirectoryNotFoundLabel.Size = new System.Drawing.Size(584, 246);
			this.DirectoryNotFoundLabel.TabIndex = 6;
			this.DirectoryNotFoundLabel.Text = "(Could not find the specified directory)";
			this.DirectoryNotFoundLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// StatusLabel
			// 
			this.StatusLabel.Name = "StatusLabel";
			this.StatusLabel.Size = new System.Drawing.Size(0, 17);
			// 
			// Browser
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(388, 371);
			this.Controls.Add(this.DirectoryUpButton);
			this.Controls.Add(this.DirectoryTextBox);
			this.Controls.Add(this.FileListView);
			this.Controls.Add(this.MainStatusStrip);
			this.Controls.Add(this.DirectoryNotFoundLabel);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MinimizeBox = false;
			this.Name = "Browser";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Disc Browser";
			this.Load += new System.EventHandler(this.Browser_Load);
			this.MainStatusStrip.ResumeLayout(false);
			this.MainStatusStrip.PerformLayout();
			this.BrowserContextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip MainStatusStrip;
		private System.Windows.Forms.ImageList MainImageList;
		private System.Windows.Forms.ListView FileListView;
		private System.Windows.Forms.TextBox DirectoryTextBox;
		private System.Windows.Forms.Button DirectoryUpButton;
		private System.Windows.Forms.Label DirectoryNotFoundLabel;
		private System.Windows.Forms.ColumnHeader NameColumn;
		private System.Windows.Forms.ColumnHeader SizeColumn;
		private System.Windows.Forms.ColumnHeader IDColumn;
		private System.Windows.Forms.ColumnHeader ExtensionColumn;
		private System.Windows.Forms.ContextMenuStrip BrowserContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem ExtractMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem OpenMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ExtractDiscMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ExtractDirectoryMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
	}
}