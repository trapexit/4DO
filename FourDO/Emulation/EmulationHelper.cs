using System;
using FourDO.FileSystem.Core.Structs;

namespace FourDO.Emulation
{
	public unsafe static class EmulationHelper
	{
		public static int GetSectorCount(IntPtr sectorZeroByteZero)
		{
			var sectorZeroStruct = (CoreVolumeHeader*)sectorZeroByteZero.ToPointer();
			return (int)ReverseBytes(sectorZeroStruct->blockCount);
		}

		public static void InitializeNvram(IntPtr nvramByteZero)
		{
			var nvramStruct = (CoreVolumeHeader*)nvramByteZero.ToPointer();

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
			int w = sizeof(CoreVolumeHeader) / 4;

			var nvramData = (UInt32*)nvramByteZero.ToPointer();
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
