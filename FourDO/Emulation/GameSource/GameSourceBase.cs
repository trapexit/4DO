using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FourDO.Emulation.GameSource
{
	/// <summary>
	/// Game source base class to help by reading sector 0 metadata.
	/// </summary>
	internal abstract class GameSourceBase : IGameSource
	{
		private int sectorCount = 0;
		private string gameId = null;
		private string gameName = null;

		private bool isOpen = false;

		protected virtual void OnOpen() { }

		public virtual void Open()
		{
			if (!this.isOpen)
			{
				this.isOpen = true;
				
				this.OnOpen();
				
				// Get some metadata
				this.ReadSectorCount();
				this.gameId = GameRegistrar.LookUpGameId(this);
				this.gameName = GameRegistrar.GetGameNameById(this.gameId);
			}
		}

		protected virtual void OnClose() { }

		public virtual void Close()
		{
			if (this.isOpen)
			{
				this.isOpen = false;

				this.OnClose();
			}
		}

		protected virtual void OnReadSector(IntPtr destinationBuffer, int sectorNumber) {}

		public virtual void ReadSector(IntPtr destinationBuffer, int sectorNumber)
		{
			if (this.isOpen)
				this.OnReadSector(destinationBuffer, sectorNumber);
		}

		public int GetSectorCount()
		{
			if (this.isOpen)
				return this.sectorCount;
			else
				return 0;
		}

		public string GetGameId()
		{
			if (this.isOpen)
				return this.gameId;
			else
				return null;
		}

		public string GetGameName()
		{
			if (this.isOpen)
				return this.gameName;
			else
				return null;
		}

		private unsafe void ReadSectorCount()
		{
			byte[] sectorZero = new byte[2048];
			fixed (byte* sectorBytePointer = sectorZero)
			{
				IntPtr sectorPointer = new IntPtr((int)sectorBytePointer);
				this.ReadSector(sectorPointer, 0);
				this.sectorCount = EmulationHelper.GetSectorCount(sectorPointer);
			}
		}
	}
}
