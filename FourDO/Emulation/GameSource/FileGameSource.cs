using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FourDO.Utilities;
using System.Windows.Forms;

namespace FourDO.Emulation.GameSource
{
    internal class FileGameSource : GameSourceBase
    {
        private BinaryReader gameRomReader = null;

        public FileGameSource(string gameFilePath)
        {
            this.GameFilePath = gameFilePath;
        }

        public string GameFilePath { get; private set; }

        #region IGameSource Implementation

        protected override void OnOpen()
        {
            try
            {
                this.gameRomReader = new BinaryReader(new FileStream(this.GameFilePath, FileMode.Open));
            }
            catch
            {
                // This source type has no specific actions. The console will throw a BadGameRomException.
                throw;
            }
        }

        protected override void OnClose()
        {
            if (this.gameRomReader != null)
                this.gameRomReader.Close();
        }

        protected override void OnReadSector(IntPtr destinationBuffer, int sectorNumber)
        {
            if (this.gameRomReader == null)
                return; // No game loaded.

            this.gameRomReader.BaseStream.Position = 2048 * sectorNumber;
            byte[] bytesToCopy = this.gameRomReader.ReadBytes(2048);

            // Now copy!
            unsafe
            {
                fixed (byte* sourceRomBytesPointer = bytesToCopy)
                {
                    Memory.CopyMemory(destinationBuffer, new IntPtr((int)sourceRomBytesPointer), 2048);
                }
            }
        }

        #endregion // IGameSource Implementation
    }
}
