using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FourDO.Utilities;
using FourDO.Utilities.CueSharp;
using System.Windows.Forms;

namespace FourDO.Emulation.GameSource
{
	internal class FileGameSource : GameSourceBase
	{
		private FourDO.Utilities.CueSharp.DataType imageDataType;
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
				bool identifiedFile = false;

				// TODO: Currently I require the CUE file for anything other than 2048.
				// We could attempt to figure out the image format in a few ways.
				// We could see if the image size is divisible by 2048, or we could read
				// the first two sectors and attempt to match it to the game database.
				// Neither of these is perfect, and I don't feel like implementing them.
				// Most people keep a .cue file around anyway.
				this.imageDataType = Utilities.CueSharp.DataType.MODE1_2048;

				////////////////////
				// We will identify the correct format of this file if there is a matching cue file.
				// (Also, if they opened a cue file, we open the cue file here).
				//
				// NOTE: If they open a .bin file, but the .cue file is for a .iso in the same directory, 
				//       then we end up opening the .iso file. But I don't care, so shut up.
				string cueFile = Path.ChangeExtension(this.GameFilePath, ".cue");
				if (File.Exists(cueFile))
				{
					var cueSheet = new CueSheet(cueFile);
					if (cueSheet.Tracks.Length == 1)
					{
						var track = cueSheet.Tracks[0];
						var dataType = track.TrackDataType;

						if (track.DataFile.Filetype == FileType.BINARY && 
							(track.TrackDataType == DataType.MODE1_2048 || track.TrackDataType == DataType.MODE1_2352))
						{
							// Combine this track's filename with the original file's directory
							string fileToOpen = Path.Combine(Path.GetDirectoryName(this.GameFilePath), track.DataFile.Filename);
							try
							{
								this.gameRomReader = new BinaryReader(new FileStream(fileToOpen, FileMode.Open));
								this.imageDataType = track.TrackDataType;
								identifiedFile = true;
							}
							catch 
							{
								// Crap. Well, just fall back to the original file name.
							}
						}
					}
				}
				
				///////////
				// If necessary, just rely on MODE1+2048 format on the actual requested file.
				if (!identifiedFile)
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

			if (this.imageDataType == DataType.MODE1_2352)
				this.gameRomReader.BaseStream.Position = 2352 * sectorNumber + 0x10;
			else
				this.gameRomReader.BaseStream.Position = 2048 * sectorNumber;

			// Read data!
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
