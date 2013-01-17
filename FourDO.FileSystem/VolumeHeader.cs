using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FourDO.FileSystem.Core.Structs;

namespace FourDO.FileSystem
{
	public class VolumeHeader
	{
		private CoreVolumeHeader _coreVolumeHeader;

		internal VolumeHeader(CoreVolumeHeader coreVolumeHeader)
		{
			_coreVolumeHeader = coreVolumeHeader;
		}

		public byte RecordType { get { return _coreVolumeHeader.recordType; } }
		public byte RecordVersion { get { return _coreVolumeHeader.recordVersion; } }
		public byte Flags { get { return _coreVolumeHeader.flags; } }
		public uint Id { get { return _coreVolumeHeader.id; } }
		public uint BlockSize { get { return _coreVolumeHeader.blockSize; } }
		public uint BlockCount { get { return _coreVolumeHeader.blockCount; } }
		public uint RootDirId { get { return _coreVolumeHeader.rootDirId; } }
		public uint RootDirBlocks { get { return _coreVolumeHeader.rootDirBlocks; } }
		public uint RootDirBlockSize { get { return _coreVolumeHeader.rootDirBlockSize; } }
		public uint LastRootDirCopy { get { return _coreVolumeHeader.lastRootDirCopy; } }

		public string Comment { get { return _coreVolumeHeader.CommentString; } }
		public string Label { get { return _coreVolumeHeader.LabelString; } }
		
		// NOTE: I didn't expose these. Who cares.
		//public fixed UInt32 rootDirCopies[8]; // 32 bytes
		//public fixed byte syncBytes[5];       // 5 bytes

	}
}
