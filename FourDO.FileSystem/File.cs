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
				return _coreDirectoryEntry.FileNameString;
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

		public byte[] ReadBytes()
		{
			var bytes = new byte[_coreDirectoryEntry.entryLengthBytes];
			_fileSystem.CoreFileSystem.SeekToBlock(_coreDirectoryEntry.FirstCopy, false);
			unsafe
			{
				fixed (byte* bytesPtr = bytes)
				{
					var bytesIntPtr = new IntPtr((int)bytesPtr);
					uint bytesRead = 0;
					_fileSystem.CoreFileSystem.FileReader.Read(bytesIntPtr, (uint)bytes.Length, ref bytesRead);
				}
			}
			return bytes;
		}
	}
}
