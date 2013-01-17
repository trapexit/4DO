using System;
using System.Runtime.InteropServices;

namespace FourDO.FileSystem.Core.Structs
{
	public static class CoreDirectoryEntryConsts
	{
		public const int DirectoryEntrySize = 72;

		public const UInt32 DirectoryEntryTypeFile       = 0x00000002;
		public const UInt32 DirectoryEntryTypeSpecial    = 0x00000006;
		public const UInt32 DirectoryEntryTypeFolder     = 0x00000007;
		public const UInt32 DirectoryEntryTypeMask       = 0x0000000F;
		public const UInt32 DirectoryEntryPosLastInBlock = 0x40000000;
		public const UInt32 DirectoryEntryPosLastInDir   = 0xC0000000;
		public const UInt32 DirectoryEntryPosMask        = 0xF0000000;
	}

	unsafe public struct CoreDirectoryEntry           // 72 bytes
	{
		public UInt32 flags;             // 4 bytes
		public UInt32 id;                // 4 bytes
		public fixed byte ext[4];            // 4 bytes
		public UInt32 blockSize;         // 4 bytes
		public UInt32 entryLengthBytes;  // 4 bytes
		public UInt32 entryLengthBlocks; // 4 bytes
		public UInt32 burst;             // 4 bytes
		public UInt32 gap;               // 4 bytes
		public fixed byte fileName[32];      // 32 bytes
		public UInt32 lastCopy;          // 4 bytes

		// 
		// note that this field is actually of variable length.
		// specifically, (lastCopy + 1) * 4 bytes.  we don't really
		// care about any other copies though, we only really need 
		// one copy.
		// 
		public UInt32 copies;            // 4 bytes

		public UInt32 FirstCopy
		{
			get { return copies; }
		}

		public string FileNameString
		{
			get
			{
				string stringValue = "";
				fixed (void* fileNameFixed = this.fileName)
				{
					stringValue = Marshal.PtrToStringAnsi(new IntPtr(fileNameFixed));
				}
				return stringValue;
			}
		}

		public string ExtString
		{
			get
			{
				string stringValue = "";
				fixed (void* extFixed = this.ext)
				{
					stringValue = Marshal.PtrToStringAnsi(new IntPtr(extFixed));
				}
				return stringValue;
			}
		}
	};
}
