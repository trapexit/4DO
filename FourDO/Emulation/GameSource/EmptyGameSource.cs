using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FourDO.Utilities;
using System.Windows.Forms;

namespace FourDO.Emulation.GameSource
{
    internal class EmptyGameSource : IGameSource
    {
        public EmptyGameSource()
        {
        }

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

        public void ReadSector(IntPtr destinationBuffer, int sectorNumber)
        {
            // I'll never talk!
        }

        #endregion // IGameSource Implementation
    }
}
