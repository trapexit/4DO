using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FourDO.FileSystem.Core;
using FourDO.FileSystem.Core.Structs;

namespace FourDO.FileSystem
{
	public class File : IItem
	{
		private UInt32 _firstByte;
		private FileSystem _fileSystem;
		private CoreDirectoryEntry _coreDirectoryEntry;
		private Directory _parentDirectory;

		internal File(UInt32 firstByte, FileSystem fileSystem, CoreDirectoryEntry coreDirectoryEntry, Directory parentDirectory)
		{
			_firstByte = firstByte;
			_fileSystem = fileSystem;
			_coreDirectoryEntry = coreDirectoryEntry;
			_parentDirectory = parentDirectory;
		}

		public uint Flags { get { return _coreDirectoryEntry.flags; }}
		public uint Id { get { return _coreDirectoryEntry.id; }}
		public uint BlockSize { get { return _coreDirectoryEntry.blockSize; }}
		public uint EntryLengthBytes { get { return _coreDirectoryEntry.entryLengthBytes; }}
		public uint EntryLengthBlocks { get { return _coreDirectoryEntry.entryLengthBlocks; }}
		public uint Burst { get { return _coreDirectoryEntry.burst; }}
		public uint Gap { get { return _coreDirectoryEntry.gap; }}

		public string FileName { get { return _coreDirectoryEntry.FileNameString; } }
		public string Extension { get { return _coreDirectoryEntry.ExtString; } }

		public Directory Parent
		{
			get { return _parentDirectory; }
		}

		public ItemType ItemType
		{
			get
			{
				return ItemType.File;
			}
		}

		public string Name
		{
			get
			{
				var returnValue = "";
				returnValue = _coreDirectoryEntry.FileNameString;

				var extension = _coreDirectoryEntry.ExtString.Trim();
				if (extension.Trim().Length > 0)
				{
					if (extension[0] == '*')
					{
						if (extension.Length == 1)
							extension = "";
						else
							extension = "." + extension.Substring(1);
					}
					returnValue += extension;
				}

				return returnValue;
			}
		}

		public string GetFullPath()
		{
			return GetFullPath(this);
		}

		internal static string GetFullPath(IItem item)
		{
			string fullPath = item.Name;
			while (item.Parent != null)
			{
				item = item.Parent;
				fullPath = item.Name + "/" + fullPath;
			}
			return fullPath;
		}
	}
}
