using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FourDO.FileSystem.Core.Structs;

namespace FourDO.FileSystem.Core
{
	internal class CoreDirectory
	{
		// 
		// the file system.
		// 
		CoreFileSystem _coreFileSystem;

		// 
		// this will be initialized to the directory found 
		// after openDirectory was called.  basically the current directory
		// 
		CoreDirectoryHeader _coreDirectoryHeader;

		// 
		// the directory hierarchy.  used for searching based on relative paths
		// 
		List<CoreDirectoryEntry> _dirTree;

		// 
		// our current directory path
		// 
		string _path;

		// 
		// used in enumerateDirectory so we know when we've seen the last
		// directory entry in the directory.
		// 
		bool _endOfDir;

		public CoreDirectory(CoreFileSystem coreFileSystem)
		{
			_coreFileSystem = coreFileSystem;
		}

		public bool OpenDirectory(string path)
		{
			bool ret;
			char separator = '/';
			string token;
			CoreVolumeHeader coreVolumeHeader = new CoreVolumeHeader();
			CoreDirectoryEntry dirEntry = new CoreDirectoryEntry();
			CoreDirectoryEntry newEntry;
			CoreDirectoryEntry oldEntry;
			string[] parts = path.Split(separator);

			// 
			// can't do much with an empty path
			// 
			if (path.Length <= 0)
			{
				return false;
			}

			// 
			// if the path isn't absolute, then this isn't going to work
			// 
			if (path.StartsWith("/"))
			{
				return false;
			}

			// 
			// check if our path is somewhat sane
			// 
			if (parts.Length <= 1)
			{
				return false;
			}

			// 
			// seek to the beginning of the filesystem
			// 
			ret = _coreFileSystem.FileReader.SeekToByte(0, false);

			if (!ret)
			{
				return false;
			}

			// clear our dir tree
			_dirTree.Clear();

			// 
			// read the volume info from the filesystem
			// 
			ret = _coreFileSystem.ReadVolumeHeader(ref coreVolumeHeader);

			if (!ret)
			{
				return false;
			}

			// 
			// read the root directory's header
			// 
			ret = _coreFileSystem.ReadDirectoryHeader(ref _coreDirectoryHeader);

			if (!ret)
			{
				return false;
			}

			// 
			// create a directory entry for the root dir and throw it into our dir tree
			// 
			newEntry = new CoreDirectoryEntry();

			unsafe
			{
				newEntry.blockSize = coreVolumeHeader.rootDirBlockSize;
				newEntry.entryLengthBlocks = coreVolumeHeader.rootDirBlocks;
				newEntry.fileName[0] = (byte)'/';
				newEntry.fileName[1] = 0;
				newEntry.lastCopy = coreVolumeHeader.lastRootDirCopy;
				newEntry.copies = coreVolumeHeader.rootDirCopies[0];
			}
			_dirTree.Add(newEntry);
	
			// 
			// our current path
			// 
			_path = "/";

			foreach (string part in parts)
			{
				ret = FindInCurrentDirectory(part, ref dirEntry);
				if (!ret)
				{
					break;
				}
			}

			// enable enumeration
			if (ret)
				_endOfDir = false;

			return true;
		}

		public bool ChangeDirectory(string path)
		{
			CoreDirectoryEntry dirEntry = new CoreDirectoryEntry();
			string pathString = path;
			string token;
			bool ret;

			if (pathString.Length <= 0)
				return false;

			// 
			// if our path start with /, then it's an absolute
			// path and we'll just call openDirectory
			// 
			if (pathString.StartsWith("/"))
			{
				return OpenDirectory(path);
			}

			string[] parts = pathString.Split('/');
			foreach (string part in parts)
			{
				ret = FindInCurrentDirectory(part, ref dirEntry);
				if (!ret)
				{
					return false;
				}
			};

			return true;
		}

		public string GetPath()
		{
			return _path;
		}

		public bool EnumerateDirectory(ref CoreDirectoryEntry de)
		{
			bool ret;

			// 
			// this is only set if enumerateDirectory has previously seen
			// a directory entry with with a DirectoryEntryPosLastInDir mask
			// 
			if (_endOfDir)
			{
				return false;
			}

			ret = _coreFileSystem.ReadDirectoryEntry(ref de);
			if (!ret)
			{
				return false;
			}
			if ((de.flags & CoreDirectoryEntryConsts.DirectoryEntryPosMask) == CoreDirectoryEntryConsts.DirectoryEntryPosLastInBlock)
			{
				// 
				// move to the beginning of the next directory header
				// 
				ret = _coreFileSystem.FileReader.SeekToByte(
					_coreFileSystem.GetBlockSize() - _coreDirectoryHeader.unusedOffset, 
					true);

				if (!ret)
				{
					return false;
				}

				ret = _coreFileSystem.ReadDirectoryHeader(ref _coreDirectoryHeader);

				if (!ret)
				{
					return false;
				}

				return true;
			}
			if ((de.flags & CoreDirectoryEntryConsts.DirectoryEntryPosMask) == CoreDirectoryEntryConsts.DirectoryEntryPosLastInDir)
			{
				_endOfDir = true;
			}

			return true;
		}

		public bool FindInCurrentDirectory(string item, ref CoreDirectoryEntry dirEntry)
		{
			bool ret;
			bool found = false;
			CoreDirectoryEntry newEntry;
			CoreDirectoryEntry currentEntry;

			// 
			// can't do much of anything if we don't have a current directory
			// 
			if (_dirTree.Count <= 0)
			{
				return false;
			}

			// 
			// if the desired directory is '..', then we want to go up a dir.
			// get the parent dir and adjust the current path
			// 

			// TODO string comp
			if (item == "..")
			{
				currentEntry = _dirTree[0];
				_dirTree.RemoveAt(0);
				newEntry = _dirTree[0];

				ret = _coreFileSystem.SeekToBlock(newEntry.copies, false);

				if (!ret)
				{
					_dirTree.Insert(0, currentEntry);
					return false;
				}

				ret = _coreFileSystem.ReadDirectoryHeader(ref _coreDirectoryHeader);

				if (!ret)
				{
					_dirTree.Insert(0, currentEntry);
					return false;
				}

				// remove the rightmost path element from our current path
				unsafe
				{
					IntPtr stringPointer = new IntPtr((int) currentEntry.fileName);
					string currentEntryFileName = Marshal.PtrToStringAnsi(stringPointer);
					_path = _path.Substring(0, _path.Length - currentEntryFileName.Length - 1);
				}

				_endOfDir = false;

				return true;
			}

			// 
			// move the filesystem fp to the beginning of the dir and reset
			// the end of directory indicator so we can enumerate
			// 

			currentEntry = _dirTree[0];
			ret = _coreFileSystem.SeekToBlock(currentEntry.copies, false);

			if (!ret)
			{
				return false;
			}

			ret = _coreFileSystem.ReadDirectoryHeader(ref _coreDirectoryHeader);

			if (!ret)
			{
				return false;
			}

			_endOfDir = false;

			while (EnumerateDirectory(ref dirEntry))
			{
				string dirEntryFileName = "";
				unsafe
				{
					fixed (byte* fileNameFixed = dirEntry.fileName)
					{
						dirEntryFileName = Marshal.PtrToStringAnsi(new IntPtr(fileNameFixed));
					}
				}

				if (dirEntryFileName == item)
				{
					// 
					// we found what we wanted.
					// move to the beginning of the directory and read the header
					// 

					ret = _coreFileSystem.SeekToBlock(dirEntry.copies, false);

					if (!ret)
					{
						break;
					}

					ret = _coreFileSystem.ReadDirectoryHeader(ref _coreDirectoryHeader);

					if (!ret)
					{
						break;
					}

					// update the directory tree
					newEntry = dirEntry;

					_dirTree.Insert(0, newEntry);

					// update our current path
					_path = _path + item + "/";

					found = true;
					break;
				}
			}

			if (found)
				_endOfDir = false;

			return found;
		}
	}
}
