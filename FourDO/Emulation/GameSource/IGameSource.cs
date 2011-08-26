using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FourDO.Emulation.GameSource
{
	internal interface IGameSource
	{
		void Open();
		void Close();

		int GetSectorCount();
		void ReadSector(IntPtr destinationBuffer, int sectorNumber);
	}
}
