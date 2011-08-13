using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private static UInt32 ReverseBytes(UInt32 value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }
    }
}
