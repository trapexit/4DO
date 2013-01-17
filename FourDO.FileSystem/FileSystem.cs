using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FourDO.FileSystem.Core;
using FourDO.FileSystem.Core.Structs;

namespace FourDO.FileSystem
{
	public class FileSystem
	{
		public IFileReader _fileReader;

		private CoreFileSystem _coreFileSystem;

		private VolumeHeader _rootVolumeHeader;
		private Directory _rootDirectory;


		internal CoreFileSystem CoreFileSystem
		{
			get { return _coreFileSystem; }
		}

		public VolumeHeader VolumeHeader
		{
			get { return _rootVolumeHeader; }
		}

		public Directory RootDirectory
		{
			get { return _rootDirectory; }
		}

		public FileSystem(IFileReader fileReader)
		{
			_fileReader = fileReader;
			_fileReader.SeekToByte(0, false);

			_coreFileSystem = new CoreFileSystem(_fileReader);

			var coreVolumeHeader = new CoreVolumeHeader();
			var coreDirectoryHeader = new CoreDirectoryHeader();

			_coreFileSystem.ReadVolumeHeader(ref coreVolumeHeader);

			_coreFileSystem.SeekToBlock(coreVolumeHeader.FirstCopy, false);
			uint directoryHeaderStart = _coreFileSystem.FileReader.CurrentByte;
			_coreFileSystem.ReadDirectoryHeader(ref coreDirectoryHeader);

			_rootVolumeHeader = new VolumeHeader(coreVolumeHeader);
			_rootDirectory = new Directory(directoryHeaderStart, this, coreDirectoryHeader, null, null);
		}
	}
}
