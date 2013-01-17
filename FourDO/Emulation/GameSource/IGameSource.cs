using System;

namespace FourDO.Emulation.GameSource
{
	public interface IGameSource
	{
		void Open();
		void Close();

		int GetSectorCount();
		string GetGameId();
		string GetGameName();

		void ReadSector(IntPtr destinationBuffer, int sectorNumber);
	}
}
