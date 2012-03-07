using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace FourDO.Emulation.GameSource
{
	internal static class GameRegistrar
	{
		// We will read two sectors for the first checksum lookup. 
		//
		// This must NEVER CHANGE, or you may lose everyone's save data! We lookup the 
		// game Id (some arbitrary number) based off the checksum, and we use the game Id
		// later for saved games' file names.
		//
		// And I probably will never do any more than the initial lookup anyway, unless
		// homebrew 3DO really picks up and the collisions are frightful. Although I
		// suppose it's possible that the game collection I queried was incomplete.
		private const int INITIAL_SECTORS_TO_READ = 2;

		// I want two databases, one by Id and one from CheckSum.
		// This allows me to look up by either "index" quickly.
		private static Dictionary<string, GameRecord> gameDatabaseById = new Dictionary<string,GameRecord>();
		private static Dictionary<string, GameRecord> gameDatabaseByCheckSum = new Dictionary<string, GameRecord>();

		static GameRegistrar()
		{
			// Load XML document of the game database.
			var assembly = Assembly.GetExecutingAssembly();
			var stream = assembly.GetManifestResourceStream("FourDO.Emulation.GameSource.GameDatabase.xml");
			XmlDocument doc = new XmlDocument();
			doc.Load(stream);
			
			var nodes = doc.SelectNodes(@"/database/game");
			foreach (var node in nodes)
			{
				var gameNode = (XmlNode)node;

				var gameRecord = new GameRecord(
					gameNode.SelectSingleNode("id").InnerText,
					gameNode.SelectSingleNode("checkSum").InnerText,
					gameNode.SelectSingleNode("name").InnerText,
					gameNode.SelectSingleNode("releaseYear").InnerText,
					gameNode.SelectSingleNode("publisher").InnerText,
					gameNode.SelectSingleNode("regions").InnerText);

				GameRegistrar.gameDatabaseById.Add(gameRecord.Id, gameRecord);
				GameRegistrar.gameDatabaseByCheckSum.Add(gameRecord.CheckSum, gameRecord);
			}
		}

		public static string CalculateGameChecksum(GameSourceBase gameSource)
		{
			//////////////////////
			// Come up with MD5 hash of the first block of CD data.
			byte[] checkSumSourceData = new byte[2048 * INITIAL_SECTORS_TO_READ];
			for (int x = 0; x < INITIAL_SECTORS_TO_READ; x++)
			{
				unsafe
				{
					fixed (byte* dataPtr = &checkSumSourceData[x * 2048])
					{
						gameSource.ReadSector(new IntPtr((int)dataPtr), x);
					}
				}
			}
			return GetHash(checkSumSourceData);
		}

		public static string LookUpGameId(GameSourceBase gameSource)
		{
			string hash = CalculateGameChecksum(gameSource);

			// Look it up!
			GameRecord gameRecord;
			GameRegistrar.gameDatabaseByCheckSum.TryGetValue(hash, out gameRecord);

			// If we found a record, we're done!
			if (gameRecord != null)
				return gameRecord.Id;

			// (if we didn't, it gets messy...)

			//////////////////////
			// Try desperately to come up with a game ID to fill in for this unknown, rogue game.

			// TODO: If at some point there's a nice (non-intrusive) way to let the user know
			// that this game is unknown, then we should do it here. This code will only be able 
			// to come up with an Id that doesn't exist in the database. If there are two 
			// games out there that don't exist in the database, there's a chance that they'll
			// use the same GameId, and therefore share save states. This isn't too likely, and
			// if it happens, most users won't notice unless they play a lot of both offending
			// games. That's why it would be good to notify users and hopefully get feedback
			// early.
			// But, of course, I really doubt this will ever be a problem.

			// Start with the first 8 characters of the hash.
			// (that's how the other items in the intial game database were made anyway).
			string candidateId = null;
			bool unique = false;
			bool firstTry = true;
			
			// As long as this game
			do
			{
				if (firstTry)
					candidateId = hash.Substring(0, 8);
				else
					candidateId = GetNextId(candidateId);
				firstTry = false;

				GameRecord existingRecord = null;
				GameRegistrar.gameDatabaseById.TryGetValue(candidateId, out existingRecord);

				unique = (existingRecord == null);
			} while (!unique);

			// Well, this key oughta do for now.
			return candidateId;
		}

		public static string GetGameNameById(string gameId)
		{
			GameRecord gameRecord = GameRegistrar.GetGameRecordById(gameId);
			if (gameRecord == null)
				return null;
			return gameRecord.Name;
		}

		public static GameRecord GetGameRecordById(string gameId)
		{
			if (gameId == null)
				return null;

			GameRecord gameRecord;
			GameRegistrar.gameDatabaseById.TryGetValue(gameId, out gameRecord);
			return gameRecord;
		}

		private static string GetHash(byte[] input)
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

		private static string GetNextId(string previousId)
		{
			uint previousIdInt = Convert.ToUInt32(previousId, 16);
			if (previousIdInt == UInt32.MaxValue)
				return String.Format("{0:X8}", UInt32.MinValue);
			else
				return String.Format("{0:X8}", previousIdInt + 1);
		}
	}
}
