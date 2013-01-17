using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using FourDO.Emulation.GameSource;
using FourDO.FileSystem;
using FourDO.FileSystem.Core;
using FourDO.FileSystem.Core.Structs;

namespace FourDO.UI.DiscBrowser
{
	public partial class Browser : Form
	{
		public IGameSource GameSource { get; set; }

		private FileSystem.FileSystem _fileSystem;
		private IFileReader _fileReader;

		private Directory _currentDirectory;

		public Browser()
		{
			InitializeComponent();
		}

		private void Browser_Load(object sender, System.EventArgs e)
		{
			_fileReader = new DiscFileReader(GameSource);
			_fileSystem = new FileSystem.FileSystem(_fileReader);

			DirectoryNotFoundLabel.Location = FileListView.Location;
			DirectoryNotFoundLabel.Size = FileListView.Size;

			this.DirectoryTextBox.Text = @"/";
			this.UpdateCurrentDirectory();

			this.UpdateUI();
		}

		private void UpdateUI()
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
					var itemFile = (File)item;
					newItem.SubItems.Add(itemFile.EntryLengthBytes.ToString());
					newItem.SubItems.Add(itemFile.Id.ToString());
				}
				else if (item is Directory)
				{
					var itemDir = (Directory)item;
					newItem.SubItems.Add("");
					newItem.SubItems.Add(itemDir.Id.Value.ToString());
				}

				FileListView.Items.Add(newItem);
			}
			
			if (FileListView.Items.Count > 0)
				FileListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			else
				FileListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

			FileListView.ListViewItemSorter = new ListViewSorter();
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
			this.UpdateCurrentDirectory();
			if (_currentDirectory == null)
				DirectoryTextBox.Text = @"/";
			else if (_currentDirectory.Parent == null)
				DirectoryTextBox.Text = @"/";
			else if (_currentDirectory.Parent == _fileSystem.RootDirectory)
				DirectoryTextBox.Text = @"/";
			else
				DirectoryTextBox.Text = _currentDirectory.Parent.GetFullPath();
			UpdateUI();
		}

		private class ListViewSorter : IComparer
		{
			public int Compare(object x, object y)
			{
				x = ((ListViewItem)x).Tag;
				y = ((ListViewItem)y).Tag;

				if (x is Directory && y is Directory)
				{
					return ((Directory) x).Name.CompareTo(((Directory) y).Name);
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
			this.OpenItem(listItem);
		}

		private void FileListView_KeyDown(object sender, KeyEventArgs e)
		{
			if (FileListView.SelectedItems.Count != 1)
				return;

			this.OpenItem(FileListView.SelectedItems[1]);
		}

		private void OpenItem(ListViewItem listItem)
		{
			if (listItem == null)
				return;

			var item = listItem.Tag;
			if (!(item is Directory))
				return;

			var itemDir = (Directory)item;
			DirectoryTextBox.Text = itemDir.GetFullPath();
			this.UpdateCurrentDirectory();
			this.UpdateUI();
		}
	}
}
