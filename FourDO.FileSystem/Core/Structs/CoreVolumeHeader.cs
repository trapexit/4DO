using System;
using System.Runtime.InteropServices;

namespace FourDO.FileSystem.Core.Structs
{
	public static class CoreVolumeHeaderConsts
	{
		public const byte VolumeHeaderSize = 132;
	}

	[StructLayout(LayoutKind.Sequential)]
	unsafe public struct CoreVolumeHeader             // 132 bytes
	{
		public byte recordType;               // 1 byte
		public fixed byte syncBytes[5];       // 5 bytes
		public byte recordVersion;            // 1 byte
		public byte flags;                    // 1 byte
		public fixed byte comment[32];        // 32 bytes
		public fixed byte label[32];          // 32 bytes
		public UInt32 id;                     // 4 bytes
		public UInt32 blockSize;              // 4 bytes
		public UInt32 blockCount;             // 4 bytes
		public UInt32 rootDirId;              // 4 bytes
		public UInt32 rootDirBlocks;          // 4 bytes
		public UInt32 rootDirBlockSize;       // 4 bytes
		public UInt32 lastRootDirCopy;        // 4 bytes
		public fixed UInt32 rootDirCopies[8]; // 32 bytes

		public UInt32 FirstCopy
		{
			get
			{
				fixed (uint* copies = this.rootDirCopies)
				{
					return copies[0];
				}
			}
		}

		public string CommentString
		{
			get
			{
				string stringValue = "";
				fixed (void* fileNameFixed = this.comment)
				{
					stringValue = Marshal.PtrToStringAnsi(new IntPtr(fileNameFixed));
				}
				return stringValue;
			}
		}

		public string LabelString
		{
			get
			{
				string stringValue = "";
				fixed (void* fileNameFixed = this.label)
				{
					stringValue = Marshal.PtrToStringAnsi(new IntPtr(fileNameFixed));
				}
				return stringValue;
			}
		}
	};
}