using System;
using System.Runtime.InteropServices;
using FourDO.FileSystem.Core.Structs;

namespace FourDO.FileSystem.Core
{
	internal class CoreFile
	{
		private CoreFileSystem _coreFileSystem;

		// 
		// the directory entry for the opened file
		// 
		private CoreDirectoryEntry _dirEntry;

		// 
		// end of file indicator
		// 
		private bool _endOfFile;

		// 
		// currently read bytes
		// 
		private UInt32 currBytes;

		public CoreFile(CoreFileSystem coreFileSystem)
		{
			_coreFileSystem = coreFileSystem;

			// 
			// seems like a decent thing to do. 
			// 
			_endOfFile = true;
		}

		public bool OpenFile(string path)
		{
			bool ret;
			string fileName;
			string filePath;
			
			// TODO: make separator a member of the filesystem?
			char separator = '/';
			CoreDirectory dir = new CoreDirectory(_coreFileSystem);

			filePath = path;

			for (;;)
			{
				//
				// find the last / in the path.  we will split the string based
				// on this.  we will then have '/path/to/file/dir' and 'file_name'
				// 
				string[] parts = filePath.Split(separator);

				if (parts.Length <= 1)
				{
					ret = false;
					break;
				}
				fileName = parts[parts.Length-1];

				// 
				// open the directory that contains our file
				// 
				ret = dir.OpenDirectory(filePath);

				if (!ret)
				{
					break;
				}

				// 
				// enumerate through the directory contents to find our file
				// 
				while (dir.EnumerateDirectory(ref _dirEntry))
				{
					// 
					// we found the file we're looking for, seek to the file
					// location in the filesystem and then we should be ready
					// to start reading from the file
					// 
					unsafe
					{
						string dirEntryFileName = "";
						fixed (byte* fileNameFixed = _dirEntry.fileName)
						{
							dirEntryFileName = Marshal.PtrToStringAnsi(new IntPtr(fileNameFixed));
						}

						if (dirEntryFileName == fileName)
						{
							ret = _coreFileSystem.SeekToBlock(_dirEntry.copies, false);

							if (!ret)
							{
								break;
							}

							// 
							// we're ready to read from the file
							// 
							_endOfFile = false;
							break;
						}
					}
				}

				break;
			}
	
			return ret;
		}

		public bool CloseFile()
		{
			return false;
		}

		public bool Read(IntPtr buf, UInt32 bufLength, ref UInt32 bytesRead)
		{
			UInt32 bytesToRead = bufLength;

			if (_endOfFile)
			{
				return false;
			}

			// 
			// we don't want to read past the end of the file entry
			// 
			if (bytesToRead > (_dirEntry.entryLengthBytes - currBytes))
				bytesToRead = (_dirEntry.entryLengthBytes - currBytes);

			return _coreFileSystem.FileReader.Read(buf, bytesToRead, ref bytesRead);
		}

		public UInt32 GetFileSize()
		{
			return _dirEntry.entryLengthBytes;
		}

		public UInt32 GetFileType()
		{
			return (_dirEntry.flags & CoreDirectoryEntryConsts.DirectoryEntryTypeMask);
		}
	}
}
