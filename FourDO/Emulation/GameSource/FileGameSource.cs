using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		private const string LOG_PREFIX = "GameSource - ";

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
					if (cueSheet.Tracks.Length != 1)
					{
						Trace.WriteLine(LOG_PREFIX + "Cue file found, but the number of tracks within was not 1.");
					}
					else
					{
						var track = cueSheet.Tracks[0];
						var dataType = track.TrackDataType;

						if (track.DataFile.Filetype != FileType.BINARY ||
							(track.TrackDataType != DataType.MODE1_2048 && track.TrackDataType != DataType.MODE1_2352))
						{
							Trace.WriteLine(LOG_PREFIX + "Cue file found, but the track within was not in the right format (should be BINARY and Mode1+2048 or Mode1+2352)");
						}
						else
						{
							// Combine this track's filename with the original file's directory
							string fileToOpen = Path.Combine(Path.GetDirectoryName(this.GameFilePath), track.DataFile.Filename);
							try
							{
								this.gameRomReader = new BinaryReader(new FileStream(fileToOpen, FileMode.Open));
								this.imageDataType = track.TrackDataType;
								identifiedFile = true;
							}
							catch (Exception ex)
							{
								// Crap. Well, just fall back to the original file name.
								Trace.WriteLine(LOG_PREFIX + "Cue file found, but there was an error opening the file it indicated within. Problem encountered was: " + ex.ToString());
							}
						}
					}
				}

				///////////
				// If we still aren't done, then the CUE file route did not work out.
				if (!identifiedFile)
				{
					/////////////////////
					// Try to guess the encoding by looking at the file size.

					// Assume 2048 unless directed otherwise.
					this.imageDataType = DataType.MODE1_2352;

					FileInfo fileInfo = new FileInfo(this.GameFilePath);
					if (fileInfo.Extension.ToUpper() == ".ISO")
					{
						// ISO will typically be 2048.
						this.imageDataType = DataType.MODE1_2048;

						if (fileInfo.Length % 2048 != 0)
						{
							// It's not divisible by 2048? That's odd.
							if (fileInfo.Length % 2352 != 0)
							{
								// Oh well, it's not divisible by 2352 either. It's all sorts of messed up. We'll just assume 2048.
							}
							else
							{
								// Well, it's divisible by 2352. Assume this is a 2352 ISO instead.
								this.imageDataType = DataType.MODE1_2352;
							}
						}
					}
					else
					{
						// Other types (like BIN) we'll assume 2352.
						if (fileInfo.Length % 2352 != 0)
						{
							// Well that's odd. It's not divisible by 2352.
							if (fileInfo.Length % 2048 != 0)
							{
								// It's not divisible by 2048 either. We'll just assume 2352.
							}
							else
							{
								// It's divisible by 2048. Assume this is a 2048 formatted file.
								this.imageDataType = DataType.MODE1_2048;
							}
						}
					}

					Trace.WriteLine(LOG_PREFIX + "No cue file found. After taking a guess on the format based on file type and file size, the format used will be: " + this.imageDataType.ToString());

					// (encoding should be sorted out by now.)

					/////////////////////
					// Open the game.
					this.gameRomReader = new BinaryReader(new FileStream(this.GameFilePath, FileMode.Open));
				}
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
