using System;
using FourDO.Emulation.FreeDO;

namespace FourDO.Emulation.GameSource
{
	internal class BiosOnlyGameSource : IGameSource
	{
		#region IGameSource Implementation

		public void Open()
		{
			// Pfftt. I ain't doin nothing.
		}

		public void Close()
		{
			// Yeah right, buddy. Get real.
		}

		public int GetSectorCount()
		{
			// Nothing's loaded!
			return 0;
		}

		public string GetGameId()
		{
			return null;
		}

		public string GetGameName()
		{
			return null;
		}

		public void ReadSector(IntPtr destinationBuffer, int sectorNumber)
		{
            FreeDOCore.SetAnvilFix(2);
			// Set fix for anvil bios
		}

		#endregion // IGameSource Implementation

		public object Clone()
		{
			return new BiosOnlyGameSource();
		}
	}
}
