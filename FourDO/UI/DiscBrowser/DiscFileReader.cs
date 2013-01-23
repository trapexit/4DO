using System;
using FourDO.Emulation.GameSource;
using FourDO.FileSystem;
using FourDO.Utilities;

namespace FourDO.UI.DiscBrowser
{
	internal class DiscFileReader : IFileReader
	{
		private IGameSource _gameSource;
		private uint _currentByte;

		public DiscFileReader(IGameSource gameSource)
		{
			_gameSource = gameSource;
		}

		public bool Read(IntPtr buf, uint bufLength, ref uint bytesRead)
		{
			unsafe
			{
				int sectorNumber = (int)(_currentByte / 2048);
				int offset = (int)(_currentByte % 2048);
				IntPtr bufWriteIntPtr = buf;
				int bytesCopied = 0;

				fixed (byte* sectorBytesPointer = _cachedSectorBytes)
				{
					while (bytesCopied < bufLength)
					{
						IntPtr sectorBytesIntPtr = new IntPtr((int) sectorBytesPointer);
						this.UpdateCachedSector(new IntPtr((int) sectorBytesPointer), sectorNumber);
						sectorBytesIntPtr = new IntPtr((int) sectorBytesPointer + offset);

						int bytesToCopy = Math.Min((int) (2048 - offset), (int) (bufLength - bytesCopied));
						Memory.CopyMemory(bufWriteIntPtr, sectorBytesIntPtr, bytesToCopy);
						bytesCopied += bytesToCopy;
						bufWriteIntPtr = new IntPtr(buf.ToInt32() + bytesCopied);

						offset = 0;
						sectorNumber++;
					}
				}
				_currentByte += (uint)bytesCopied;
				bytesRead = (uint)bytesCopied;
			}
			return true;
		}

		private byte[] _cachedSectorBytes = new byte[2048];
		private int _cachedSectorNumber = -1;
		private void UpdateCachedSector(IntPtr sectorBytesIntPointer, int sectorNumber)
		{
			if (sectorNumber == _cachedSectorNumber)
				return;
			_gameSource.ReadSector(sectorBytesIntPointer, sectorNumber);
			_cachedSectorNumber = sectorNumber;
		}

		public bool SeekToByte(uint byteNumber, bool relative)
		{
			if (!relative)
				_currentByte = (uint) byteNumber;
			else
				_currentByte += (uint) byteNumber;
			return true;
		}

		public uint CurrentByte
		{
			get { return _currentByte; }
		}
	}
}
