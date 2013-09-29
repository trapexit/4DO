using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FourDO.Emulation.GameSource;
using FourDO.FileSystem;
using FourDO.FileSystem.Core;
using FourDO.FileSystem.Core.Structs;
using System.Threading;
using FourDO.Resources;
using FourDO.Utilities;

namespace FourDO.UI.DiscBrowser
{
	public partial class Browser : Form
	{
		private class WorkerArgs
		{
			public BackgroundWorker worker;
			public IEnumerable<IItem> items;
			public string path;
		}

		public IGameSource GameSource { get; set; }

		private BackgroundWorker _backgroundWorker = null;

		private FileSystem.FileSystem _fileSystem;
		private IFileReader _fileReader;
		private string _extractPath;
		private Cursor _originalCursor;

		private Directory _currentDirectory;

		public Browser()
		{
			InitializeComponent();
		}

		private void Browser_Load(object sender, System.EventArgs e)
		{
			if (this.GameSource == null || this.GameSource is BiosOnlyGameSource)
			{
				this.Close();
				return;
			}

			_fileReader = new DiscFileReader(GameSource);
			_fileSystem = new FileSystem.FileSystem(_fileReader);

			_extractPath = System.IO.Directory.GetCurrentDirectory();

			DirectoryNotFoundLabel.Location = FileListView.Location;
			DirectoryNotFoundLabel.Size = FileListView.Size;

			this.DirectoryTextBox.Text = @"/";
			this.UpdateCurrentDirectory();

			this.Localize();

			this.UpdateUI();
		}

		private void UpdateUI()
		{
			using (new FormDrawLocker(this))
			{
				// Determine current directory.
				this.UpdateCurrentDirectory();

				if (_currentDirectory == null)
				{
					DirectoryNotFoundLabel.Visible = true;
					FileListView.Visible = false;
					return;
				}

				DirectoryNotFoundLabel.Visible = false;
				FileListView.Visible = true;

				FileListView.Items.Clear();
				foreach (var item in _currentDirectory.Children)
				{
					var newItem = new ListViewItem();
					newItem.Text = item.Name;
					newItem.Tag = item;
					newItem.ImageKey = (item is Directory) ? @"Directory_Closed" : @"File";
					if (item is File)
					{
						var itemFile = (File) item;
						newItem.SubItems.Add(itemFile.EntryLengthBytes.ToString());
						newItem.SubItems.Add(itemFile.Id.ToString());
						newItem.SubItems.Add(itemFile.Extension.ToString());
					}
					else if (item is Directory)
					{
						var itemDir = (Directory) item;
						newItem.SubItems.Add("");
						newItem.SubItems.Add(itemDir.Id.Value.ToString());
						newItem.SubItems.Add(itemDir.Extension.ToString());
					}

					FileListView.Items.Add(newItem);
				}

				if (FileListView.Items.Count > 0)
					FileListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
				else
					FileListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

				FileListView.ListViewItemSorter = new ListViewSorter();
			}
		}

		private void UpdateCurrentDirectory()
		{
			var currentDirectory = DirectoryTextBox.Text;
			if (!currentDirectory.StartsWith(@"/"))
			{
				_currentDirectory = null;
				return;
			}

			// Remove first slash.
			currentDirectory = currentDirectory.Substring(1);

			// Remove final slash
			if (currentDirectory.EndsWith(@"/") && currentDirectory.Length > 1)
				currentDirectory = currentDirectory.Substring(0, currentDirectory.Length - 1);

			if (currentDirectory == @"")
			{
				_currentDirectory = _fileSystem.RootDirectory;
				return;
			}

			string[] parts = currentDirectory.Split('/');
			Directory loopDirectory = _fileSystem.RootDirectory;
			foreach(var part in parts)
			{
				var foundItem = loopDirectory.FindItem(part);
				if (foundItem == null)
				{
					_currentDirectory = null;
					return;
				}

				if (!(foundItem is Directory))
				{
					_currentDirectory = null;
					return;
				}

				loopDirectory = (Directory)foundItem;
			}
			_currentDirectory = loopDirectory;
		}

		private void DirectoryUpButton_Click(object sender, System.EventArgs e)
		{
			this.HandleUpDirectory();
		}

		private void HandleUpDirectory()
		{
			this.UpdateCurrentDirectory();

			// Get the last full path.
			string lastFullPath = null;
			if (_currentDirectory != null)
				lastFullPath = _currentDirectory.GetFullPath();

			// Determine the next directory.
			if (_currentDirectory == null)
				DirectoryTextBox.Text = @"/";
			else if (_currentDirectory.Parent == null)
				DirectoryTextBox.Text = @"/";
			else if (_currentDirectory.Parent == _fileSystem.RootDirectory)
				DirectoryTextBox.Text = @"/";
			else
				DirectoryTextBox.Text = _currentDirectory.Parent.GetFullPath();

			// This will populate the list with items.
			UpdateUI();

			// See if we came from a directory that we can select now.
			if (!string.IsNullOrEmpty(lastFullPath))
			{
				var parts = lastFullPath.Split('/');
				var lastPart = parts[parts.Length - 1];
				foreach (var item in FileListView.Items)
				{
					ListViewItem listItem = item as ListViewItem;
					if (listItem == null)
						continue;

					Directory directoryItem = listItem.Tag as Directory;
					if (directoryItem == null)
						continue;

					if (directoryItem.Name == lastPart)
					{
						listItem.Selected = true;
						listItem.EnsureVisible();
						break;
					}
				}
			}

			// Set focus back to the listview.
			this.FileListView.Focus();
		}

		private void DirectoryTextBox_TextChanged(object sender, System.EventArgs e)
		{
			var oldDir = _currentDirectory;
			this.UpdateCurrentDirectory();

			if (oldDir != _currentDirectory)
				this.UpdateUI();
		}

		private void FileListView_DoubleClick(object sender, System.EventArgs e)
		{
			var mousePosition = Cursor.Position;
			Point localPoint = FileListView.PointToClient(mousePosition);
			var listItem = FileListView.GetItemAt(localPoint.X, localPoint.Y);
			if (!(listItem.Tag is IItem))
				return;

			this.OpenItem((IItem)listItem.Tag);
		}

		private void FileListView_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Back)
			{
				this.HandleUpDirectory();
			}
			else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
			{
				var items = GetSelectedItems();
				if (items.Count != 1)
					return;

				this.OpenItem(items[0]);
			}
			else if (e.KeyCode == Keys.A && e.Control)
			{
				foreach (var item in FileListView.Items)
					((ListViewItem)item).Selected = true;
			}
		}

		private void OpenItem(IItem item)
		{
			if (!(item is Directory))
				return;

			var itemDir = (Directory)item;
			DirectoryTextBox.Text = itemDir.GetFullPath();
			this.UpdateCurrentDirectory();
			this.UpdateUI();
		}

		private string AskForFolder(string currentFolder)
		{
			if (Ookii.Dialogs.VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
			{
				var dialog = new Ookii.Dialogs.VistaFolderBrowserDialog();
				dialog.SelectedPath = currentFolder;
				dialog.ShowNewFolderButton = true;
				var result = dialog.ShowDialog();
				if (result == DialogResult.OK)
					return dialog.SelectedPath;
			}
			else
			{
				var dialog = new FolderBrowserDialog();
				dialog.SelectedPath = currentFolder;
				dialog.ShowNewFolderButton = true;
				var result = dialog.ShowDialog();
				if (result == DialogResult.OK)
					return dialog.SelectedPath;
			}
			return null;
		}

		private void ExtractMenuItem_Click(object sender, System.EventArgs e)
		{
			var items = GetSelectedItems();
			if (items.Count <= 0)
				return;

			AskAndBeginExtract(items);
		}

		private void ExtractDirectoryMenuItem_Click(object sender, System.EventArgs e)
		{
			if (_currentDirectory == null)
				return;

			if (_currentDirectory.Children == null)
				return;

			if (_currentDirectory.Children.Count <= 0)
				return;

			var items = _currentDirectory.Children.ToList();
			AskAndBeginExtract(items);
		}

		private void ExtractDiscMenuItem_Click(object sender, System.EventArgs e)
		{
			if (_fileSystem == null)
				return;

			if (_fileSystem.RootDirectory == null)
				return;

			if (_fileSystem.RootDirectory.Children == null)
				return;

			if (_fileSystem.RootDirectory.Children.Count <= 0)
				return;

			var items = _fileSystem.RootDirectory.Children.ToList();
			AskAndBeginExtract(items);
		}

		private void AskAndBeginExtract(IEnumerable<IItem> items)
		{
			// Find extraction folder
			var newFolder = this.AskForFolder(_extractPath);
			if (string.IsNullOrEmpty(newFolder))
				return;

			_extractPath = newFolder;

			// Disable UI
			this.SetFormWorkMode(true);
			this.Refresh();

			/////////////
			// Start worker
			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.WorkerReportsProgress = true;
			_backgroundWorker.WorkerSupportsCancellation = true;
			_backgroundWorker.DoWork += new DoWorkEventHandler(worker_DoWork);
			_backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
			_backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

			WorkerArgs workerArgs = new WorkerArgs();
			workerArgs.worker = _backgroundWorker;
			workerArgs.items = items;
			workerArgs.path = _extractPath;

			_backgroundWorker.RunWorkerAsync(workerArgs);
		}

		void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			WorkerArgs args = (WorkerArgs)e.Argument;
			ExtractToDirectory(args.items, args.path, args.worker);
		}

		void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			ExtractingLabel.Text = "\n" +
					Strings.BrowserExtracting + "\n" +
					"\n" +
					(string)e.UserState;
			this.Refresh();
		}

		void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.SetFormWorkMode(false);
			_backgroundWorker = null;
		}

		private void SetFormWorkMode(bool workingMode)
		{
			if (workingMode)
			{
				/////////////
				// Disable UI
				this.ControlBox = false;
				this.MainStatusStrip.Visible = false;

				foreach (Control control in this.Controls)
					control.Enabled = false;

				TransferPanel.Dock = DockStyle.Fill;
				ExtractingLabel.BringToFront();
				TransferPanel.Enabled = true;
				ExtractingLabel.Visible = true;
				TransferPanel.Visible = true;

				_originalCursor = Cursor.Current;
				Cursor.Current = Cursors.WaitCursor;
			}
			else
			{
				/////////////
				// Re-enable UI
				foreach (Control control in this.Controls)
					control.Enabled = true;
				TransferPanel.Visible = false;

				this.ControlBox = true;
				this.MainStatusStrip.Visible = true;

				Cursor.Current = _originalCursor;
			}
		}

		private void ExtractToDirectory(IEnumerable<IItem> items, string path, BackgroundWorker worker)
		{
			foreach (var item in items)
			{
				if (worker.CancellationPending)
					return;

				if (item is File)
				{
					var fileItem = item as File;
					var bytes = fileItem.ReadBytes();
					var outputPath = System.IO.Path.Combine(path, fileItem.Name);
					worker.ReportProgress(0, fileItem.GetFullPath());
					System.IO.File.WriteAllBytes(outputPath, bytes);
				}
				else if (item is Directory)
				{
					var dirItem = item as Directory;
					var outputPath = System.IO.Path.Combine(path, item.Name);
					if (!System.IO.Directory.Exists(outputPath))
						System.IO.Directory.CreateDirectory(outputPath);
					ExtractToDirectory(dirItem.Children, outputPath, worker);
				}
			}
		}

		private void OpenMenuItem_Click(object sender, System.EventArgs e)
		{
			var items = GetSelectedItems();
			if (items.Count != 1)
				return;

			var item = items[0];
			if (!(item is Directory))
				return;

			this.OpenItem(item);
		}

		private void ContextMenuStrip_VisibleChanged(object sender, System.EventArgs e)
		{
			if (!this.Visible)
				return;

			var items = GetSelectedItems();
			OpenMenuItem.Enabled = (items.Count == 1) && (items[0] is Directory);
			ExtractDirectoryMenuItem.Enabled = (_currentDirectory != null && _currentDirectory.Children.Count > 0);
			ExtractDiscMenuItem.Enabled = (_fileSystem != null && _fileSystem.RootDirectory != null && _fileSystem.RootDirectory.Children.Count > 0);
			ExtractMenuItem.Enabled = (items.Count > 0);
		}

		private List<IItem> GetSelectedItems()
		{
			var returnValue = new List<IItem>();
			foreach (var selectedItem in FileListView.SelectedItems)
			{
				var listItem = (ListViewItem)selectedItem;
				if (listItem.Tag is IItem)
					returnValue.Add((IItem)listItem.Tag);
			}
			return returnValue;
		}

		private class ListViewSorter : IComparer
		{
			public int Compare(object x, object y)
			{
				x = ((ListViewItem)x).Tag;
				y = ((ListViewItem)y).Tag;

				if (x is Directory && y is Directory)
				{
					return ((Directory)x).Name.CompareTo(((Directory)y).Name);
				}
				else if (x is Directory)
					return -1;
				else if (y is Directory)
					return 1;
				else
				{
					return ((File)x).Name.CompareTo(((File)y).Name);
				}
			}
		}

		private void FileListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			var items = GetSelectedItems();
			if (items.Count == 0)
			{
				StatusLabel.Text = "";
				return;
			}

			StatusLabel.Text = items.Count + " item(s) selected.";
		}

		private void CancelButton_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (_backgroundWorker != null)
				_backgroundWorker.CancelAsync();
		}

		private void TransferPanel_Resize(object sender, System.EventArgs e)
		{
			ExtractingLabel.Left = 0;
			ExtractingLabel.Top = TransferPanel.ClientSize.Height / 2;
			ExtractingLabel.Height = TransferPanel.ClientSize.Height - ExtractingLabel.Top;
			ExtractingLabel.Width = TransferPanel.ClientSize.Width;

			CancelButton.Left = 0;
			CancelButton.Top = ExtractingLabel.Top - CancelButton.Height;
			CancelButton.Width = TransferPanel.Width;
		}

		private void SelectAllToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			foreach (var item in FileListView.Items)
				((ListViewItem)item).Selected = true;
		}
	}
}
