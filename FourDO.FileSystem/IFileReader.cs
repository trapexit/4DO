using System;

namespace FourDO.FileSystem
{
	public interface IFileReader
	{
		bool Read(IntPtr buf, UInt32 bufLength, ref UInt32 bytesRead);
		bool SeekToByte(UInt32 byteNumber, bool relative);
		UInt32 CurrentByte { get; }
	}
}
