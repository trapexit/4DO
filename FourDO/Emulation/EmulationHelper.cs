using System;
using System.Runtime.InteropServices;

namespace FourDO.Emulation
{
	public unsafe static class EmulationHelper
	{
		[StructLayout(LayoutKind.Sequential)]
		private struct VolumeHeader             // 132 bytes
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
		};

		public static int GetSectorCount(IntPtr sectorZeroByteZero)
		{
			VolumeHeader* sectorZeroStruct = (VolumeHeader*)sectorZeroByteZero.ToPointer();
			return (int)ReverseBytes(sectorZeroStruct->blockCount);
		}

		public static void InitializeNvram(IntPtr nvramByteZero)
		{
			VolumeHeader* nvramStruct = (VolumeHeader*)nvramByteZero.ToPointer();

			////////////////
			// Fill out the volume header.
			nvramStruct->recordType = 0x01;
			for (int x = 0; x < 5; x++) nvramStruct->syncBytes[x] = (byte)'Z';
			nvramStruct->recordVersion = 0x02;
			nvramStruct->flags = 0x00;
			for (int x = 0; x < 32; x++) nvramStruct->comment[x] = 0;

			nvramStruct->label[0] = (byte)'n';
			nvramStruct->label[1] = (byte)'v';
			nvramStruct->label[2] = (byte)'r';
			nvramStruct->label[3] = (byte)'a';
			nvramStruct->label[4] = (byte)'m';
			for (int x = 5; x < 32; x++) nvramStruct->label[x] = 0;

			nvramStruct->id         = ReverseBytes(0xFFFFFFFF);
			nvramStruct->blockSize  = ReverseBytes(0x00000001); // Yep, one byte per block.
			nvramStruct->blockCount = ReverseBytes(0x00008000); // 32K worth of NVRAM data.

			nvramStruct->rootDirId        = ReverseBytes(0xFFFFFFFE);
			nvramStruct->rootDirBlocks    = ReverseBytes(0x00000000);
			nvramStruct->rootDirBlockSize = ReverseBytes(0x00000001);
			nvramStruct->lastRootDirCopy  = ReverseBytes(0x00000000);

			nvramStruct->rootDirCopies[0] = ReverseBytes(0x00000084);
			for (int x = 1; x < 8; x++) nvramStruct->rootDirCopies[x] = 0;

			////////////////
			// After this point, I could not find the proper structure for the data.
			int w = sizeof(VolumeHeader) / 4;

			UInt32* nvramData = (UInt32*)nvramByteZero.ToPointer();
			nvramData[w++] = ReverseBytes(0x855A02B6);
			nvramData[w++] = ReverseBytes(0x00000098);
			nvramData[w++] = ReverseBytes(0x00000098);
			nvramData[w++] = ReverseBytes(0x00000014);
			nvramData[w++] = ReverseBytes(0x00000014);
			nvramData[w++] = ReverseBytes(0x7AA565BD);
			nvramData[w++] = ReverseBytes(0x00000084);
			nvramData[w++] = ReverseBytes(0x00000084);
			nvramData[w++] = ReverseBytes(0x00007668); // This is blocks remaining.
			nvramData[w++] = ReverseBytes(0x00000014);
		}

		private static UInt32 ReverseBytes(UInt32 value)
		{
			return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
				   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
		}
	}
}
