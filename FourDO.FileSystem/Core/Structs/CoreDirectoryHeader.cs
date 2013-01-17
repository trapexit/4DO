using System;

namespace FourDO.FileSystem.Core.Structs
{
	public static class CoreDirectoryHeaderConsts
	{
		public const byte DirectoryHeaderSize = 20;

		public const UInt32 DirectoryHeaderLastBlock  = 0xFFFFFFFF;
		public const UInt32 DirectoryHeaderFirstBlock = 0xFFFFFFFF;
	}

	public struct CoreDirectoryHeader        // 20 bytes
	{
		public Int32 nextBlock;        // 4 bytes
		public Int32 prevBlock;        // 4 bytes
		public UInt32 flags;           // 4 bytes
		public UInt32 unusedOffset;    // 4 bytes
		public UInt32 directoryOffset; // 4 bytes
	};
}
