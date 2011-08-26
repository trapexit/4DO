using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FourDO.Utilities;
using System.Windows.Forms;
using CDLib;

namespace FourDO.Emulation.GameSource
{
	internal class DiscGameSource : GameSourceBase
	{
		private CDDrive drive;

		public DiscGameSource(char driveLetter)
		{
			this.DriveLetter = driveLetter;
		}

		public char DriveLetter { get; private set; }

		protected override void OnOpen()
		{
			try
			{
				this.drive = new CDDrive();
				this.drive.Open(this.DriveLetter);

				if (this.drive.IsCDReady() == false)
					throw new Exception("No CD in drive " + this.DriveLetter + ".");

				if (this.drive.Refresh() == false)
					throw new Exception("Failed to read the drive table of contents of drive " + this.DriveLetter + ".");

				if (this.drive.GetNumTracks() != 1)
					throw new Exception("3DO Games have only one track!");
			}
			catch
			{
				// Cleanup
				try
				{
					this.drive.Close();
					this.drive = null;
				}
				catch { }

				// Rethrow
				throw;
			}
		}

		protected override void OnClose()
		{
			this.drive.Close();
			this.drive = null;
		}

		protected override void OnReadSector(IntPtr destinationBuffer, int sectorNumber)
		{
			if (this.drive == null || (this.drive.IsOpened == false))
				return;

			this.drive.ReadSector(sectorNumber, destinationBuffer);
		}
	}
}
