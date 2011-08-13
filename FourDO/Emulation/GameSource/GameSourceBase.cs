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

        protected virtual void OnOpen() { }

        public virtual void Open()
        {
            this.OnOpen();
            this.ReadSectorCount();
        }

        protected virtual void OnClose() { }

        public virtual void Close()
        {
            this.OnClose();
        }

        protected virtual void OnReadSector(IntPtr destinationBuffer, int sectorNumber) {}

        public virtual void ReadSector(IntPtr destinationBuffer, int sectorNumber)
        {
            this.OnReadSector(destinationBuffer, sectorNumber);
        }

        public int GetSectorCount()
        {
            return this.sectorCount;
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
