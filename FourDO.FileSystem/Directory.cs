using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FourDO.FileSystem.Core;
using FourDO.FileSystem.Core.Structs;

namespace FourDO.FileSystem
{
	public class Directory : IItem
	{
		private UInt32 _firstByte;
		private FileSystem _fileSystem;
		private CoreDirectoryHeader _coreDirectoryHeader;
		private CoreDirectoryEntry? _coreDirectoryEntry;
		private Directory _parentDirectory;

		private bool _itemsFetched = false;
		private List<IItem> _items;

		internal CoreDirectoryHeader CoreDirectoryHeader
		{
			get { return _coreDirectoryHeader; }
		}

		internal CoreDirectoryEntry? CoreDirectoryEntry
		{
			get { return _coreDirectoryEntry; }
		}

		internal Directory(UInt32 firstByte, FileSystem fileSystem, CoreDirectoryHeader coreDirectoryHeader, CoreDirectoryEntry? coreDirectoryEntry, Directory parentDirectory)
		{
			_firstByte = firstByte;
			_coreDirectoryHeader = coreDirectoryHeader;
			_coreDirectoryEntry = coreDirectoryEntry;
			_fileSystem = fileSystem;
			_parentDirectory = parentDirectory;
		}

		public uint? Id
		{
			get
			{
				return _coreDirectoryEntry.HasValue ? _coreDirectoryEntry.Value.id : (uint?)null;
			}
		}

		public uint? BlockSize
		{
			get { return _coreDirectoryEntry.HasValue ? _coreDirectoryEntry.Value.blockSize : (uint?)null; }
		}

		public IItem FindItem(string itemName)
		{
			itemName = itemName.ToLower();
			foreach (var item in this.Children)
			{
				if (itemName == item.Name.ToLower())
					return item;
			}
			return null;
		}

		public Directory Parent
		{
			get { return _parentDirectory; }
		}

		public string GetFullPath()
		{
			return File.GetFullPath(this);
		}

		public ItemType ItemType
		{
			get
			{
				return ItemType.Directory;
			}
		}

		public string Name
		{
			get
			{
				return _coreDirectoryEntry.HasValue ? _coreDirectoryEntry.Value.FileNameString : "";
			}
		}

		public string Extension
		{
			get
			{
				return _coreDirectoryEntry.HasValue ? _coreDirectoryEntry.Value.ExtString : "";
			}
		}

		public List<IItem> Children 
		{
			get
			{
				if (!_itemsFetched)
				{
					_itemsFetched = true;
					_items = FetchItems();
				}
				var listCopy = new List<IItem>(_items);
				return listCopy;
			}
		}

		private List<IItem> FetchItems()
		{
			var returnValue = new List<IItem>();

			var coreFileSystem = _fileSystem.CoreFileSystem;
			var coreDirectoryEntry = new CoreDirectoryEntry();
			var coreDirectory = new CoreDirectory(coreFileSystem);
			coreDirectory.SetCoreDirectoryHeader(_coreDirectoryHeader);

			coreFileSystem.FileReader.SeekToByte(_firstByte, false);
			coreFileSystem.FileReader.SeekToByte(_coreDirectoryHeader.directoryOffset, true);

			var currentByte = coreFileSystem.FileReader.CurrentByte;
			while (coreDirectory.EnumerateDirectory(ref coreDirectoryEntry))
			{
				IItem newItem;
				UInt32 itemType = (coreDirectoryEntry.flags & CoreDirectoryEntryConsts.DirectoryEntryTypeMask);

				if (itemType == CoreDirectoryEntryConsts.DirectoryEntryTypeFolder)
				{
					//////////////////////////
					// Seek to and read the folder header now!
					var returnByte = coreFileSystem.FileReader.CurrentByte;
					coreFileSystem.SeekToBlock(coreDirectoryEntry.FirstCopy, false);
					var referencedByte = coreFileSystem.FileReader.CurrentByte;

					var coreDirectoryHeader = new CoreDirectoryHeader();
					coreFileSystem.ReadDirectoryHeader(ref coreDirectoryHeader);

					// Go back into the enumeration position.
					coreFileSystem.SeekToByte(returnByte, false);

					//////////////////////////
					// Now we're ready to create the directory entry.
					newItem = new Directory(referencedByte, _fileSystem, coreDirectoryHeader, coreDirectoryEntry, this);
				}
				else
				{
					newItem = new File(currentByte, _fileSystem, coreDirectoryEntry, this);
				}

				returnValue.Add(newItem);
				currentByte = coreFileSystem.FileReader.CurrentByte;
			}

			return returnValue;
		}
	}
}
