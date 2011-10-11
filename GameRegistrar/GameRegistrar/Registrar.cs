////////////////////////////////////////////////////
// GameRegistrar
////////
// I most likely will never use this program again. I used it to determine the minimum amount of 
// sectors necessary to uniquely identify the known 3DO games out there. 

// Interestingly enough, the first 2048 bytes is enough for all but one game: Starblade. Starblade's 
// Japan and US releases had their first difference in byte 2121.
//
// So, as a result, I will be reading the first two sectors (4k) of any loaded game in 4DO to determine
// it's checksum.
// 
// I intend to create a game database (in XML) with this data. I will be coding things to leave it
// extensible to checksum collisions in the future. So, I'll record the full checksum of each game along
// with a "game ID" (which is the first 8 out of 32 characters on the checksum). 
//
// Rather than just use the MD5 immediately, this process will occur to identify a game:
//   * Calculate checksum on first 4k.
//   * Look for a matching checksum value in game database.
//       * If there is a single match, use the game ID in the matching record.
//       * If there is no match, create a game ID using the first 8 characters of the hash.
//       * If there are multiple matches, the database should identify additional sectors
//         of the game that can be read to uniquely identify the game from the others 
//         sharing the checksum. (I won't be coding 4DO with this initially. In fact, it
//         may never come up!).
//   * Use the Game ID for file names in save states.
// 
////////////////////////////////////////////////////
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Text.RegularExpressions;

namespace GameRegistrar
{
	public partial class Registrar : Form
	{
		private const string romDirectory = @"H:\Emulation\3DO\ALL_3DO_ROMS";
		private const string outputFilePath = @"C:\fourdo\doc\FourDO\GameDatabase.xml";

		private const bool skipDifferenceCheck = true;

		public class GameRecord : IComparable
		{
			public GameRecord(string fileName, string gameID, int fileSize)
			{
				this.FileName = fileName;
				this.GameID = gameID;
				this.FileSize = fileSize;
				this.Unique = true;
			}

			public string FileName { get; set; }
			public string GameID { get; set; }
			public int FileSize { get; set; }
			public long FirstUniqueByte { get; set; }
			public bool Unique { get; set; }

			public override string ToString()
			{
				return this.GameID + this.FileName;
			}

			public int CompareTo(object obj)
			{
				GameRecord other = obj as GameRecord;
				if (other == null)
					return 1;

				return string.Compare(this.ToString(), other.ToString());
			}
		}

		private List<GameRecord> records = new List<GameRecord>();
		private ListViewColumnSorter lvwColumnSorter = new ListViewColumnSorter();

		public Registrar()
		{
			InitializeComponent();

			List<string> roms = this.GetAllRoms();
			this.records = this.GetGameRecords(roms);
			this.FindDuplicates();

			this.UpdateUI();
		}

		private void UpdateUI()
		{
			this.GameRecordsListView.ListViewItemSorter = lvwColumnSorter;

			this.GameRecordsListView.Columns.Clear();
			this.GameRecordsListView.Columns.Add("ID");
			this.GameRecordsListView.Columns.Add("Unique");
			this.GameRecordsListView.Columns.Add("UniqueByte");
			this.GameRecordsListView.Columns.Add("FileSize");
			this.GameRecordsListView.Columns.Add("FileName");

			this.GameRecordsListView.Items.Clear();

			foreach (var record in this.records)
			{
				var item = this.GameRecordsListView.Items.Add(record.GameID);
				item.SubItems.Add(record.Unique.ToString());
				item.SubItems.Add(record.FirstUniqueByte.ToString());
				item.SubItems.Add(record.FileSize.ToString());
				item.SubItems.Add(record.FileName);
			}

			this.GameRecordsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
		}

		public List<string> GetAllRoms()
		{
			var roms = new List<string>();

			// Get all dirs
			List<string>subDirs = Directory.GetDirectories(romDirectory).ToList<string>();
			subDirs = subDirs.Where<string>(x => !x.Contains("BIOS")).ToList<string>();

			// Get all zips
			foreach (string subDir in subDirs)
			{
				roms.AddRange(Directory.GetFiles(subDir, "*.zip").ToList<string>());
			}

			return roms;
		}

		public List<GameRecord> GetGameRecords(List<string> roms)
		{
			var gameRecords = new List<GameRecord>();

			foreach (string rom in roms)
			{
				int size;

				string gameId = this.CalculateID(rom, out size);

				if (!string.IsNullOrEmpty(gameId))
					gameRecords.Add(new GameRecord(rom, gameId, size));
			}

			return gameRecords;
		}

		public string CalculateID(string rom, out int size)
		{
			using (ZipFile file = new ZipFile(rom))
			{
				size = 0;
				ZipEntry isoEntry = this.GetIsoEntry(file);
				if (isoEntry == null)
					return null;

				const int readLength = 2048 * 2;

				////////////
				var inputData = new byte[readLength];

				Stream inputStream = file.GetInputStream(isoEntry);
				inputStream.Read(inputData, 0, readLength);

				string hash = this.GetHash(inputData);
				//size = (int)((ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream)inputStream).;
				size = (int)isoEntry.Size;

				return hash;

			}
		}

		public ZipEntry GetIsoEntry(ZipFile file)
		{
			foreach (ZipEntry entry in file)
			{
				if (entry.Name.EndsWith(".iso", true, System.Globalization.CultureInfo.CurrentCulture))
				{
					return entry;
				}
			}
			return null;
		}

		public string GetHash(byte[] input)
		{
			var md5 = System.Security.Cryptography.MD5.Create();
			byte[] hash = md5.ComputeHash(input);

			// step 2, convert byte array to hex string
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("X2"));
			}
			return sb.ToString();
		}

		private void GameRecordsListView_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			// Determine if clicked column is already the column that is being sorted.
			if (e.Column == lvwColumnSorter.SortColumn)
			{
				// Reverse the current sort direction for this column.
				if (lvwColumnSorter.Order == SortOrder.Ascending)
				{
					lvwColumnSorter.Order = SortOrder.Descending;
				}
				else
				{
					lvwColumnSorter.Order = SortOrder.Ascending;
				}
			}
			else
			{
				// Set the column number that is to be sorted; default to ascending.
				lvwColumnSorter.SortColumn = e.Column;
				lvwColumnSorter.Order = SortOrder.Ascending;
			}

			// Perform the sort with these new sort options.
			this.GameRecordsListView.Sort();
		}

		private long FindFirstDifferentByte(Stream stream1, Stream stream2)
		{
			const int blockSize = 2048 * 10;

			var buffer1 = new byte[blockSize];
			var buffer2 = new byte[blockSize];

			int blockNumber = -1;
			int compareCount;
			int readCount1;
			int readCount2;
			do
			{
				blockNumber++;
				readCount1 = stream1.Read(buffer1, 0, buffer1.Length);
				readCount2 = stream2.Read(buffer2, 0, buffer2.Length);

				compareCount = Math.Min(readCount1, readCount2);
				for (int x = 0; x < compareCount; x++)
				{
					if(buffer1[x] != buffer2[x])
					{
						return (blockNumber * blockSize) + x;
					}
				}

				if (readCount1 != readCount2)
				{
					// NOTE: this means one game is "longer" than the other. I don't know if I want to count this.

					if (readCount1 > readCount2)
					{
						for (int x = readCount2; x < readCount1; x++)
						{
							if (buffer1[x] != 0)
							{
								return (blockNumber * blockSize) + x;
							}
						}

						// We compared these bytes too.
						compareCount = readCount1;
					}
					else
					{
						for (int x = readCount1; x < readCount2; x++)
						{
							if (buffer2[x] != 0)
							{
								return (blockNumber * blockSize) + x;
							}
						}

						// We compared these bytes too.
						compareCount = readCount2;
					}
				}
			}while (compareCount == buffer1.Length);

			return -1;
		}

		private void FindDuplicates()
		{
			this.records.Sort();

			GameRecord lastRecord = null;

			List<string> nonUniqueGames = new List<string>(){
				//"0B0F27B610B77A03F7A06C179217D7CF", //	False	-1	671703040	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Road Rash (1994)(Electronic Arts)(EU).zip
				//"0B0F27B610B77A03F7A06C179217D7CF", //	False	-1	672010240	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Road Rash (1994)(Electronic Arts)(US)[!].zip
				//"0E183E7E0BA4635BA73E16451D6EC73B", //	False	-1	105476096	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Shanghai - Triple-Threat (1994)(Activision)(US)[!][02500-2.4].zip
				//"0E183E7E0BA4635BA73E16451D6EC73B", //	False	-1	105472000	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Shanghai - Triple-Threat (1994)(Activision)(US)[!][TRIPLETHREAT R1J].zip
				//"12AAFBBBE32E5DB296DCC7CC8AEAE7F0", //	False	-1	441018368	H:\Emulation\3DO\ALL_3DO_ROMS\Games\BattleSport (1995)(Studio 3DO)(EU)[!].zip
				//"12AAFBBBE32E5DB296DCC7CC8AEAE7F0", //	False	-1	441323520	H:\Emulation\3DO\ALL_3DO_ROMS\Games\BattleSport (1995)(Studio 3DO)(US)[!].zip
				"1F925AFD64594F299C9492DA1994C188", //	False	-1	655974400	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Psychic Detective (1995)(Electronic Arts)(EU)(Disc 1 of 3)[!].zip
				"1F925AFD64594F299C9492DA1994C188", //	False	-1	655978496	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Psychic Detective (1995)(Electronic Arts)(US)(Disc 1 of 3)[!].zip
				"217024DB8EAFA597615263AC4A0C9586", //	False	-1	676456448	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Captain Quazar (1995)(Studio 3DO)(EU)[!].zip
				"217024DB8EAFA597615263AC4A0C9586", //	False	-1	676458496	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Captain Quazar (1996)(Studio 3DO)(US)[!].zip
				"28EC609CAC993A37042F74B97C3B06A6", //	False	-1	676454400	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Immercenary (1995)(Electronic Arts)(EU)[!].zip
				"28EC609CAC993A37042F74B97C3B06A6", //	False	-1	676761600	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Immercenary (1995)(Electronic Arts)(US)[!].zip
				//"2A3CB139FDCB1A7B931EAF88029376D0", //	False	-1	650733568	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Shock Wave (1994)(Electronic Arts)(EU)[!].zip
				//"2A3CB139FDCB1A7B931EAF88029376D0", //	False	-1	650735616	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Shock Wave (1994)(Electronic Arts)(US)[!][722207-2 RE2].zip
				"2F5E0CD016C86BF657A33F20F1FA0214", //	False	-1	650731520	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Psychic Detective (1995)(Electronic Arts)(EU)(Disc 3 of 3)[!].zip
				"2F5E0CD016C86BF657A33F20F1FA0214", //	False	-1	650735616	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Psychic Detective (1995)(Electronic Arts)(US)(Disc 3 of 3)[!].zip
				//"35E0A893EF29FBD564C2FB70C1C845DF", //	False	-1	263067648	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Flashback - The Quest for Identity (1994)(U.S. Gold)(EU)(en-fr).zip
				//"35E0A893EF29FBD564C2FB70C1C845DF", //	False	-1	262758400	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Flashback - The Quest for Identity (1994)(U.S. Gold)(US)(en-fr)[!].zip
				//"3D010BAC3FC82E00A96B0FC26C6C4690", //	False	-1	629760000	H:\Emulation\3DO\ALL_3DO_ROMS\Games\PGA Tour 96 (1995)(EA Sports)(EU)[!].zip
				//"3D010BAC3FC82E00A96B0FC26C6C4690", //	False	-1	629764096	H:\Emulation\3DO\ALL_3DO_ROMS\Games\PGA Tour 96 (1995)(EA Sports)(US)[!].zip
				//"407B1A68AB9FAB37A83EC6089B1A8D10", //	False	-1	420352000	H:\Emulation\3DO\ALL_3DO_ROMS\Games\John Madden Football (1994)(EA Sports)(US)[!][722007-2 70].zip
				//"407B1A68AB9FAB37A83EC6089B1A8D10", //	False	-1	420044800	H:\Emulation\3DO\ALL_3DO_ROMS\Games\John Madden Football (1994)(EA Sports)(US)[!][722007-2 R73].zip
				//"4250DE2A2BDD3FF276F5303CA0BCC631", //	False	-1	655974400	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Shockwave 2 - Beyond the Gate (1995)(Electronic Arts)(EU)(Disc 2 of 2)[!].zip
				//"4250DE2A2BDD3FF276F5303CA0BCC631", //	False	-1	655978496	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Shockwave 2 - Beyond the Gate (1995)(Electronic Arts)(US)(Disc 2 of 2)[!].zip
				//"45862BC094A0BDD1C841291EA96E5213", //	False	-1	315187200	H:\Emulation\3DO\ALL_3DO_ROMS\Games\BladeForce (1995)(Studio 3DO)(EU)[!].zip
				//"45862BC094A0BDD1C841291EA96E5213", //	False	-1	315494400	H:\Emulation\3DO\ALL_3DO_ROMS\Games\BladeForce (1995)(Studio 3DO)(US)[!].zip
				//"4EB073B4BCA514699CA463AFC45C21F4", //	False	-1	629649408	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Crash 'n Burn (1993)(Crystal Dynamics)(US)[!][3DRM-1239120 1].zip
				//"4EB073B4BCA514699CA463AFC45C21F4", //	False	-1	629452800	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Crash 'n Burn (1993)(Crystal Dynamics)(US)[!][DFJN5002ZAZ].zip
				//"510DF1D6955ECC4B4932B1B9B75F4FB0", //	False	-1	671709184	H:\Emulation\3DO\ALL_3DO_ROMS\Games\World Cup Golf - Hyatt Dorado Beach (1994)(U.S. Gold)(EU)[!].zip
				//"510DF1D6955ECC4B4932B1B9B75F4FB0", //	False	-1	671399936	H:\Emulation\3DO\ALL_3DO_ROMS\Games\World Cup Golf - Hyatt Dorado Beach (1994)(U.S. Gold)(US)[!].zip
				//"540990AB7BD755E6DE23435EE1A70A61", //	False	-1	524902400	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Burning Soldier (1994)(Panasonic)(CA).zip
				//"540990AB7BD755E6DE23435EE1A70A61", //	False	-1	525209600	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Burning Soldier (1994)(Panasonic)(US)[!].zip
				//"546161DF4FF6242B521C2E5D25835E38", //	False	-1	619274240	H:\Emulation\3DO\ALL_3DO_ROMS\Games\FIFA International Soccer (1994)(EA Sports)(EU)[!].zip
				//"546161DF4FF6242B521C2E5D25835E38", //	False	-1	619581440	H:\Emulation\3DO\ALL_3DO_ROMS\Games\FIFA International Soccer (1994)(EA Sports)(US)[!].zip
				"5C93AAD6E0DA441344476EB4CC4C5CC1", //	False	-1	310689792	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Super Street Fighter II Turbo (1994)(Panasonic)(US)[!][FZSM3851].zip
				"5C93AAD6E0DA441344476EB4CC4C5CC1", //	False	-1	310996992	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Super Street Fighter II Turbo (1994)(Panasonic)(US)[SM3851-2 RE2].zip
				//"6189250AECDB3282B3C0F340C870A806", //	False	-1	629762048	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Phoenix 3 (1995)(Studio 3DO)(EU)[!].zip
				//"6189250AECDB3282B3C0F340C870A806", //	False	-1	629456896	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Phoenix 3 (1995)(Studio 3DO)(US)[!].zip
				//"6233C7D6E5499C0E1FB7099F46528E6B", //	False	-1	467230720	H:\Emulation\3DO\ALL_3DO_ROMS\Coverdiscs\3DO Magazine 01 (1994)(Paragon Publishing)(GB)[!].zip
				//"6233C7D6E5499C0E1FB7099F46528E6B", //	False	-1	467537920	H:\Emulation\3DO\ALL_3DO_ROMS\Samplers\3DO Interactive Sampler CD, The (1994)(3DO Company)(US)[!].zip
				"6B3DFF1EBCA7E61DA1C944BEBF13DFDB", //	False	-1	675594240	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Killing Time (1995)(Studio 3DO)(EU)[!].zip
				"6B3DFF1EBCA7E61DA1C944BEBF13DFDB", //	False	-1	675901440	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Killing Time v3.5 (1995)(Studio 3DO)(US)[!].zip
				//"72B385F88F27108C9A0FB2F01440988C", //	False	-1	650428416	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Twisted - The Game Show (1993)(Electronic Arts)(US)[!][730807-2 R73].zip
				//"72B385F88F27108C9A0FB2F01440988C", //	False	-1	650735616	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Twisted - The Game Show (1993)(Electronic Arts)(US)[730807-2 70].zip
				//"79B222ECC211AB4FD4629065BB4443F8", //	False	-1	105164800	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Slam 'N Jam '95 (1994)(Crystal Dynamics)(EU-US)[!].zip
				//"79B222ECC211AB4FD4629065BB4443F8", //	False	-1	105168896	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Slam 'N Jam '95 (1995)(Crystal Dynamics)(US)[!].zip
				//"815330BA96102E52087B31911346CC84", //	False	-1	671703040	H:\Emulation\3DO\ALL_3DO_ROMS\Games\MegaRace (1994)(Mindscape)(EU-US)(M5)[!].zip
				//"815330BA96102E52087B31911346CC84", //	False	-1	672010240	H:\Emulation\3DO\ALL_3DO_ROMS\Games\MegaRace (1994)(Mindscape)(US)(M5).zip
				"850A8A945176D53F64D069840226CBC2", //	False	-1	420044800	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Space Hulk - Vengeance of the Blood Angels (1995)(Electronic Arts)(EU)[!].zip
				"850A8A945176D53F64D069840226CBC2", //	False	-1	420048896	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Space Hulk - Vengeance of the Blood Angels (1995)(Electronic Arts)(US)[!].zip
				//"85A236FB411D32EC37AF06B9AEC2CED6", //	False	-1	210333696	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Dragon's Lair (1993)(ReadySoft)(US)[!][DH 65003].zip
				//"85A236FB411D32EC37AF06B9AEC2CED6", //	False	-1	210636800	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Dragon's Lair (1993)(ReadySoft)(US)[!][DH 65003-2].zip
				"901C3D156AED31080D756EDC2CEA51C1", //	False	-1	630067200	H:\Emulation\3DO\ALL_3DO_ROMS\Samplers\3DO Interactive Sampler CD #3, The (1995)(3DO Company)(US)[!].zip
				"901C3D156AED31080D756EDC2CEA51C1", //	False	-1	629456896	H:\Emulation\3DO\ALL_3DO_ROMS\Samplers\3DO Multi Game Sampler Number 3, The (1995)(3DO Company)(EU)[!].zip
				//"97FC47C1E65F7A5544DD1B10EDE926A3", //	False	-1	577331200	H:\Emulation\3DO\ALL_3DO_ROMS\Games\StarBlade (1994)(Namco)(JP)(en)[!].zip
				//"97FC47C1E65F7A5544DD1B10EDE926A3", //	False	-1	577331200	H:\Emulation\3DO\ALL_3DO_ROMS\Games\StarBlade (1994)(Panasonic)(CA)[!].zip
				//"97FC47C1E65F7A5544DD1B10EDE926A3", //	False	-1	577638400	H:\Emulation\3DO\ALL_3DO_ROMS\Games\StarBlade (1994)(Panasonic)(US)[!].zip
				//"9A992FE1763D4576A95363ABD609C363", //	False	-1	112812032	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Syndicate (1995)(Electronic Arts)(EU)[!].zip
				//"9A992FE1763D4576A95363ABD609C363", //	False	-1	113119232	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Syndicate (1995)(Electronic Arts)(US)[!].zip
				//"9C337398BB475B7939FD06F8287A4E5A", //	False	-1	310251520	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Slayer (1994)(Mindscape)(EU)[!].zip
				//"9C337398BB475B7939FD06F8287A4E5A", //	False	-1	309948416	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Slayer (1994)(SSi)(US)[!][02500-21].zip
				//"9C337398BB475B7939FD06F8287A4E5A", //	False	-1	309944320	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Slayer (1994)(SSi)(US)[!][SLAYER R1J].zip
				//"A558E6CD3082CDD77B3BEFC02016AC2B", //	False	-1	336465920	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Total Eclipse (1993)(Crystal Dynamics)(US)[!][DFJN5015ZAZ].zip
				//"A558E6CD3082CDD77B3BEFC02016AC2B", //	False	-1	336158720	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Total Eclipse (1993)(Crystal Dynamics)(US)[61203].zip
				//"AEC65A2DD814B4C9CDACBF0D2498003A", //	False	-1	84197376	H:\Emulation\3DO\ALL_3DO_ROMS\Educational\Putt-Putt Joins the Parade (1993)(Humongous Entertainment)(US)[!][CQ 6922-2].zip
				//"AEC65A2DD814B4C9CDACBF0D2498003A", //	False	-1	84504576	H:\Emulation\3DO\ALL_3DO_ROMS\Educational\Putt-Putt Joins the Parade (1993)(Humongous Entertainment)(US)[CQ PARADE].zip
				//"B406EAE99020CAB7EBF8793E2BA11E80", //	False	-1	655974400	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Shockwave 2 - Beyond the Gate (1995)(Electronic Arts)(EU)(Disc 1 of 2)[!].zip
				//"B406EAE99020CAB7EBF8793E2BA11E80", //	False	-1	655978496	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Shockwave 2 - Beyond the Gate (1995)(Electronic Arts)(US)(Disc 1 of 2)[!].zip
				//"B4D064DF6C1DADBAF595EC9326A276EE", //	False	-1	197746688	H:\Emulation\3DO\ALL_3DO_ROMS\Coverdiscs\3DO Magazine 03 (1995-03-30)(Paragon Publishing)(GB)[!].zip
				//"B4D064DF6C1DADBAF595EC9326A276EE", //	False	-1	197746688	H:\Emulation\3DO\ALL_3DO_ROMS\Coverdiscs\3DO Magazine 11 (1996)(Paragon Publishing)(GB)[!][Jun 1996].zip
				"B6B55A453AA6D0ADC533E8460A466C9B", //	False	-1	629760000	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Psychic Detective (1995)(Electronic Arts)(EU)(Disc 2 of 3)[!].zip
				"B6B55A453AA6D0ADC533E8460A466C9B", //	False	-1	629764096	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Psychic Detective (1995)(Electronic Arts)(US)(Disc 2 of 3)[!].zip
				//"B86E02DAD74D477184B90C11B0447487", //	False	-1	629456896	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Flying Nightmares (1995)(Domark)(EU)[!].zip
				//"B86E02DAD74D477184B90C11B0447487", //	False	-1	629764096	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Flying Nightmares (1995)(Domark)(US)[!].zip
				//"B8723F20C3D7900CEB98A36E37077BFD", //	False	-1	660172800	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Snow Job Starring Tracy Scoggins (1995)(Studio 3DO)(US)(Disc 2 of 2)[!].zip
				//"B8723F20C3D7900CEB98A36E37077BFD", //	False	-1	660170752	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Snowjob (1995)(Studio 3DO)(EU)(Disc 2 of 2).zip
				"D0EA0BBDD21AD72DFE8129669725B40C", //	False	-1	378101760	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Pebble Beach Golf Links (1993)(Panasonic)(CA).zip
				"D0EA0BBDD21AD72DFE8129669725B40C", //	False	-1	378105856	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Pebble Beach Golf Links (1993)(Panasonic)(US)[!].zip
				//"D6C5D374E7BA9C6D51FBCDE610FE2479", //	False	-1	50946048	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Real Pinball (1994)(Panasonic)(CA).zip
				//"D6C5D374E7BA9C6D51FBCDE610FE2479", //	False	-1	51251200	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Real Pinball (1994)(Panasonic)(US)[!].zip
				//"D6DF0C7E8A65E9E8A5B7C83869F0FCBE", //	False	-1	231301120	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Guardian War (1994)(Panasonic)(CA).zip
				//"D6DF0C7E8A65E9E8A5B7C83869F0FCBE", //	False	-1	231608320	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Guardian War (1994)(Panasonic)(US)[!].zip
				//"E4392792861D92356FE597A582FD71FC", //	False	-1	629762048	H:\Emulation\3DO\ALL_3DO_ROMS\Coverdiscs\3DO Magazine 09 (1996)(Paragon Publishing)(GB)[!][Apr 1996].zip
				//"E4392792861D92356FE597A582FD71FC", //	False	-1	629764096	H:\Emulation\3DO\ALL_3DO_ROMS\Samplers\3DO Interactive Sampler CD 4, The (1995)(3DO Company)(US)[!].zip
				//"EDB7EB579985AE619CF8C5A3CD6286AE", //	False	-1	660172800	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Snow Job Starring Tracy Scoggins (1995)(Studio 3DO)(US)(Disc 1 of 2)[!].zip
				//"EDB7EB579985AE619CF8C5A3CD6286AE", //	False	-1	660170752	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Snowjob (1995)(Studio 3DO)(EU)(Disc 1 of 2).zip
				//"EE4D244FD43C60C37A0EA7F0A942C2BD", //	False	-1	117006336	H:\Emulation\3DO\ALL_3DO_ROMS\Coverdiscs\3DO Magazine 04 (1995)(Paragon Publishing)(GB)[!].zip
				//"EE4D244FD43C60C37A0EA7F0A942C2BD", //	False	-1	117006336	H:\Emulation\3DO\ALL_3DO_ROMS\Coverdiscs\3DO Magazine 12 (1996)(Paragon Publishing)(GB)[!][Jul 1996].zip
				//"F7C0BD4E0D24C017976E6249D7CD18A3", //	False	-1	197746688	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Theme Park (1994)(Electronic Arts)(EU)(en-fr)[!].zip
				//"F7C0BD4E0D24C017976E6249D7CD18A3", //	False	-1	198053888	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Theme Park (1994)(Electronic Arts)(US)(en-fr)[!].zip
				//"F929539D0A52F95158398D0C7EA064F6", //	False	-1	122249216	H:\Emulation\3DO\ALL_3DO_ROMS\Coverdiscs\3DO Magazine 02 (1995)(Paragon Publishing)(GB)[!].zip
				//"F929539D0A52F95158398D0C7EA064F6", //	False	-1	122251264	H:\Emulation\3DO\ALL_3DO_ROMS\Coverdiscs\3DO Magazine 10 (1996)(Paragon Publishing)(GB)[!][May 1996].zip
				//"F929539D0A52F95158398D0C7EA064F6", //	False	-1	122249216	H:\Emulation\3DO\ALL_3DO_ROMS\Samplers\Super Street Fighter II X - Grand Master Challenge (demo) (1994)(Panasonic)(JP)[!].zip
				//"FFF68FE9AD16178A3697B9264D40829C", //	False	-1	336465920	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Out of this World (1993)(Interplay)(US)[!][45097-1].zip
				//"FFF68FE9AD16178A3697B9264D40829C", //	False	-1	335855616	H:\Emulation\3DO\ALL_3DO_ROMS\Games\Out of this World (1993)(Interplay)(US)[!][CD3DO0230RE].zip

			};

			foreach (GameRecord record in this.records)
			{
				if (lastRecord != null && record.GameID == lastRecord.GameID)
				{
					long firstDifferentByte;

					if (nonUniqueGames.Contains(lastRecord.GameID) || skipDifferenceCheck)
					{
						firstDifferentByte = -1;
					}
					else
					{
						var lastZipFile = new ZipFile(lastRecord.FileName);
						var thisZipFile = new ZipFile(record.FileName);

						var lastZipEntry = this.GetIsoEntry(lastZipFile);
						var thisZipEntry = this.GetIsoEntry(thisZipFile);

						var lastZipStream = lastZipFile.GetInputStream(lastZipEntry);
						var thisZipStream = thisZipFile.GetInputStream(thisZipEntry);

						firstDifferentByte = this.FindFirstDifferentByte(lastZipStream, thisZipStream);

						lastZipFile.Close();
						thisZipFile.Close();
					}

					lastRecord.FirstUniqueByte = firstDifferentByte;
					record.FirstUniqueByte = firstDifferentByte;
					
					lastRecord.Unique = false;
					record.Unique = false;
				}
				lastRecord = record;
			}
		}

		private void GameRecordsListView_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.C)
			{
				var builder = new StringBuilder();

				foreach (ListViewItem item in this.GameRecordsListView.SelectedItems)
				{
					bool firstInLine = true;
					foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
					{
						if (!firstInLine)
							builder.Append("\t");

						builder.Append(subItem.Text);
						firstInLine = false;
					}
					builder.Append("\r\n");
				}

				Clipboard.Clear();
				Clipboard.SetText(builder.ToString());
			}
		}

		private void CloseButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void SaveButton_Click(object sender, EventArgs e)
		{
			var doc = new XmlDocument();
			
			XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", null, null);
			doc.AppendChild(declaration);

			XmlElement root = doc.CreateElement("database");
			doc.AppendChild(root);
			
			foreach (var record in records)
			{
				XmlElement gameRecord = doc.CreateElement("game");
				
				XmlElement gameIdElement = doc.CreateElement("id");
				gameIdElement.InnerText = record.GameID.Substring(0, 8);

				XmlElement checkSumElement = doc.CreateElement("checkSum");
				checkSumElement.InnerText = record.GameID;

				string fileName = Path.GetFileNameWithoutExtension(record.FileName);
				Regex gameNameRegex = new Regex(@"(?<name>.*)\s+\((?<year>19[0-9][0-9])[^\)]*\)\((?<publisher>[^\)]*)\)");
				Regex discRegex = new Regex(@"\((?<disc>Disc[^\)]*)\)");
				Regex regionRegex = new Regex(@"(\((?<regions>(GB|US|EU|JP|CA|EU\-US|EU\-JP|CA\-US|JP\-US|FR))\))");
				Match m = gameNameRegex.Match(fileName);
				Match d = discRegex.Match(fileName);

				string gameName = m.Groups["name"].Value;

				if (!string.IsNullOrEmpty(d.Groups["disc"].Value))
					gameName += " (" + d.Groups["disc"] + ")";
	
				XmlElement gameNameElement = doc.CreateElement("name");
				gameNameElement.InnerText = gameName;

				XmlElement yearElement = doc.CreateElement("releaseYear");
				yearElement.InnerText = m.Groups["year"].Value;

				XmlElement publisherElement = doc.CreateElement("publisher");
				publisherElement.InnerText = m.Groups["publisher"].Value;

				m = regionRegex.Match(fileName);
				XmlElement regionsElement = doc.CreateElement("regions");
				regionsElement.InnerText = m.Groups["regions"].Value;

				gameRecord.AppendChild(gameIdElement);
				gameRecord.AppendChild(checkSumElement);
				gameRecord.AppendChild(gameNameElement);
				gameRecord.AppendChild(yearElement);
				gameRecord.AppendChild(publisherElement);
				gameRecord.AppendChild(regionsElement);

				root.AppendChild(gameRecord);
			}

			doc.Save(outputFilePath);
		}
	}

	#region listviewsort

	/// <summary>
	/// This class is an implementation of the 'IComparer' interface.
	/// </summary>
	public class ListViewColumnSorter : IComparer
	{
		/// <summary>
		/// Specifies the column to be sorted
		/// </summary>
		private int ColumnToSort;
		/// <summary>
		/// Specifies the order in which to sort (i.e. 'Ascending').
		/// </summary>
		private SortOrder OrderOfSort;
		/// <summary>
		/// Case insensitive comparer object
		/// </summary>
		private CaseInsensitiveComparer ObjectCompare;

		/// <summary>
		/// Class constructor.  Initializes various elements
		/// </summary>
		public ListViewColumnSorter()
		{
			// Initialize the column to '0'
			ColumnToSort = 0;

			// Initialize the sort order to 'none'
			OrderOfSort = SortOrder.None;

			// Initialize the CaseInsensitiveComparer object
			ObjectCompare = new CaseInsensitiveComparer();
		}

		/// <summary>
		/// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
		/// </summary>
		/// <param name="x">First object to be compared</param>
		/// <param name="y">Second object to be compared</param>
		/// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
		public int Compare(object x, object y)
		{
			int compareResult;
			ListViewItem listviewX, listviewY;

			// Cast the objects to be compared to ListViewItem objects
			listviewX = (ListViewItem)x;
			listviewY = (ListViewItem)y;

			// Compare the two items
			compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);

			// Calculate correct return value based on object comparison
			if (OrderOfSort == SortOrder.Ascending)
			{
				// Ascending sort is selected, return normal result of compare operation
				return compareResult;
			}
			else if (OrderOfSort == SortOrder.Descending)
			{
				// Descending sort is selected, return negative result of compare operation
				return (-compareResult);
			}
			else
			{
				// Return '0' to indicate they are equal
				return 0;
			}
		}

		/// <summary>
		/// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
		/// </summary>
		public int SortColumn
		{
			set
			{
				ColumnToSort = value;
			}
			get
			{
				return ColumnToSort;
			}
		}

		/// <summary>
		/// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
		/// </summary>
		public SortOrder Order
		{
			set
			{
				OrderOfSort = value;
			}
			get
			{
				return OrderOfSort;
			}
		}

	}

	#endregion
}
