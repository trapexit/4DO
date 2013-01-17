using System;
using System.Runtime.InteropServices;
using FourDO.FileSystem.Core.Structs;

namespace FourDO.FileSystem.Core
{
	internal class CoreFileSystem
	{
		private IFileReader _fileReader;
		private CoreVolumeHeader _coreVolumeHeader;
		private UInt32 _blockSize;

		public IFileReader FileReader
		{
			get { return _fileReader; }
			set { _fileReader = value; }
		}

		public CoreFileSystem(IFileReader fileReader)
		{
			_fileReader = fileReader;
		}

		// 
		// ReadVolumeHeader: 
		//     reads the filesystem volume header.  this header should
		//     be at the beginning of every 3do iso/disc.  it contains 
		//     information about the filesystem on the disc, including
		//     block size, total disc size, root directory location, etc.
		// 
		// arguments:
		//     1) CoreVolumeHeader *vh (OUT): a pointer to an allocated CoreVolumeHeader object.
		//         this will be initialized with the data read from the disc
		// return value:
		//     true on success, false otherwise
		// 
		public bool ReadVolumeHeader(ref CoreVolumeHeader vh)
		{
			UInt32 bytesRead = 0;
			bool ret;

			byte[] buffer = new byte[CoreVolumeHeaderConsts.VolumeHeaderSize];
			GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			ret = _fileReader.Read(handle.AddrOfPinnedObject(), CoreVolumeHeaderConsts.VolumeHeaderSize, ref bytesRead);
			if (!ret)
			{
				return false;
			}
			vh = (CoreVolumeHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof (CoreVolumeHeader));
			handle.Free();

			// 
			// make all relevant fields little endian
			// 
			EndianSwap(ref vh.id);
			EndianSwap(ref vh.blockSize);
			EndianSwap(ref vh.blockCount);
			EndianSwap(ref vh.rootDirId);
			EndianSwap(ref vh.rootDirBlocks);
			EndianSwap(ref vh.rootDirBlockSize);
			EndianSwap(ref vh.lastRootDirCopy);
			unsafe
			{
				for (int i = 0; i < 8; i++)
				{
					fixed (uint* copies = vh.rootDirCopies)
					{
						EndianSwap(ref copies[i]);
					}
				}
			}

			// 
			// keep a local copy for ourselves
			// 
			_coreVolumeHeader = vh;
	
			// 
			// go ahead and move the fp forward to the root dir location
			// 
			unsafe
			{
				fixed (uint* copies = vh.rootDirCopies)
				{
					ret = SeekToBlock(copies[0], false);
				}
			}

			if (!ret)
				return false;

			return true;
		}

		// 
		// ReadDirectoryHeader: 
		//     reads a directory header from the filesystem.  a directory
		//     header contains information about a directory.  mostly this 
		//     is just information about the next and previous blocks in this
		//     directory
		// 
		// arguments:
		//     1) CoreDirectoryHeader *de (OUT): a pointer to an allocated CoreDirectoryHeader
		//         object.  this will be initialized with the data read from the disc
		// 
		// return value:
		//     true on success, false otherwise
		// 
		public bool ReadDirectoryHeader(ref CoreDirectoryHeader dh)
		{
			UInt32 bytesRead = 0;
			bool ret;

			byte[] buffer = new byte[CoreDirectoryHeaderConsts.DirectoryHeaderSize];
			GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			ret = _fileReader.Read(handle.AddrOfPinnedObject(), CoreDirectoryHeaderConsts.DirectoryHeaderSize, ref bytesRead);
			if (!ret)
			{
				return false;
			}
			dh = (CoreDirectoryHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof (CoreDirectoryHeader));
			handle.Free();

			// 
			// make all relevant fields little endian
			// 

			EndianSwap(ref dh.nextBlock);
			EndianSwap(ref dh.prevBlock);
			EndianSwap(ref dh.flags);
			EndianSwap(ref dh.unusedOffset);
			EndianSwap(ref dh.directoryOffset);

			return true;
		}

		// 
		// ReadDirectoryEntry: 
		//     reads a directory entry from the filesystem, initializing
		//     the passed in object with the data read.  a directory entry
		//     is basically a pointer to some object residing in a directory.
		//     this object can be either another directory, or a file.
		// 
		// arguments:
		//     1) CoreDirectoryEntry *de (OUT): a pointer to an allocated CoreDirectoryEntry 
		//         object that will be initialized with the data read from the disc
		// 
		// return value:
		//     true on success, false otherwise
		// 
		public bool ReadDirectoryEntry(ref CoreDirectoryEntry de)
		{
			UInt32 bytesRead = 0;
			bool ret;

			byte[] buffer = new byte[CoreDirectoryEntryConsts.DirectoryEntrySize];
			GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			ret = _fileReader.Read(handle.AddrOfPinnedObject(), CoreDirectoryEntryConsts.DirectoryEntrySize, ref bytesRead);
			if (!ret)
			{
				return false;
			}
			de = (CoreDirectoryEntry)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(CoreDirectoryEntry));
			handle.Free();

			// 
			// make all relevant fields little endian
			//

			EndianSwap(ref de.flags);
			EndianSwap(ref de.id);
			EndianSwap(ref de.blockSize);
			EndianSwap(ref de.entryLengthBytes);
			EndianSwap(ref de.entryLengthBlocks);
			EndianSwap(ref de.burst);
			EndianSwap(ref de.gap);
			EndianSwap(ref de.lastCopy);
			EndianSwap(ref de.copies);

			// 
			// we may need to move the file pointer a little further along.
			// this is due to the fact that the copies field is actually of 
			// variable length, but we always only read in 4 bytes of it
			// because we don't really need more than one copy of anything.
			// 

			if (de.lastCopy > 0)
			{
				UInt32 pos = de.lastCopy * 4;

				ret = _fileReader.SeekToByte(pos, true);

				if (!ret)
					return false;
			}

			return true;
		}

		public bool SeekToBlock(UInt32 block, bool relative)
		{
			bool ret;
			UInt32 pos = (GetBlockSize() * block);

			ret =_fileReader.SeekToByte(pos, relative);

			if (!ret)
			{
				return false;
			}

			return true;
		}

		public bool SeekToByte(UInt32 byteNumber, bool relative)
		{
			return _fileReader.SeekToByte(byteNumber, relative);
		}

		public UInt32 GetBlockSize()
		{
			// XXX: is this always true?
			return _coreVolumeHeader.rootDirBlockSize;
		}

		private void EndianSwap(ref UInt16 x)
		{
			x = (UInt16)((x >> 8) | (x << 8));
		}

		private void EndianSwap(ref UInt32 x)
		{
			x = (x >> 24) | 
				((x >> 8) & 0x0000FF00) | 
				((x << 8) & 0x00FF0000) | 
				(x << 24);
		}

		private void EndianSwap(ref Int32 x)
		{
			x = (x >> 24) | 
				((x >> 8) & 0x0000FF00) | 
				((x << 8) & 0x00FF0000) | 
				(x << 24);
		}
	}
}
