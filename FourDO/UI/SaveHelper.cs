using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FourDO.Emulation.GameSource;

namespace FourDO.UI
{
	internal class SaveHelper
	{
		private const string SAVE_SUBFOLDER = "Saves";
		private const string SAVE_STATE_EXTENSION = ".4dosav";

		private const string SCREENSHOT_SUBFOLDER = "Screenshots";

		private const int MAXIMUM_FRIENDLY_NAME_LENGTH = 32;

		public static string GetSaveStateFileName(IGameSource gameSource, int saveStateSlot)
		{
			//////////////
			// Get the filename we'll use for this game and save slot.
			string preferredFileName = null;
			string fileNamePrefix = null;
			if (gameSource is BiosOnlyGameSource || gameSource == null)
			{
				// TODO: Come up with separate "buckets" for different BIOS's?
				//       Currently, any BIOS will save and load from the same block of save states.
				fileNamePrefix = "No_Game_Loaded";
			}
			else
			{
				string friendlyGameName = SaveHelper.GetFriendlyGameName(gameSource.GetGameName());
				if (friendlyGameName != null)
				{
					// If the "preferred" save filename exists, go ahead and use it.
					string preferredFileNamePrefix = string.Format("{0}_{1}", gameSource.GetGameId(), friendlyGameName);
					preferredFileName = SaveHelper.GetFullSaveStateFilePath(preferredFileNamePrefix, saveStateSlot);
					if (File.Exists(preferredFileName))
						return preferredFileName;
				}
				
				// Otherwise, we'll try to find the closest match by the game Id.
				// NOTE: This is because a game may have previously saved under a different "friendly name" somewhere in the filename.
				//       This makes the code capable of dealing with changes to the "friendly names" inside 4DO.
				fileNamePrefix = gameSource.GetGameId();
			}
			
			//////////////
			// We didn't find an obvious pick, so let's try finding one by gameId.
			
			// Come up with a search pattern.
			string fullSearchFilePath = SaveHelper.GetFullSaveStateFilePath(fileNamePrefix + "*", saveStateSlot);
			string searchDirectory = Path.GetDirectoryName(fullSearchFilePath);
			string searchFilePattern = Path.GetFileName(fullSearchFilePath);
			string[] candidateFiles = null;

			// Find candidates!
			if (Directory.Exists(searchDirectory))
				candidateFiles = Directory.GetFiles(searchDirectory, searchFilePattern);

			//////////////
			// Return the first match if there is one.
			if (candidateFiles != null && candidateFiles.Length > 0)
				return candidateFiles[0];

			// If we have a preferred file name (when the game has a name), use that as the expected prefix.
			if (preferredFileName != null)
				return preferredFileName; 
			
			// Well crap. Just return the most basic filename we can, using gameId as the prefix.
			return SaveHelper.GetFullSaveStateFilePath(fileNamePrefix, saveStateSlot);
		}

		public static string GetNvramFilePath()
		{
			string appFolder = Path.GetDirectoryName(Application.ExecutablePath);
			return Path.Combine(Path.Combine(appFolder, SAVE_SUBFOLDER), "NVRAM_SaveData.ram");
		}

		public static string GetScreenshotFilePath(IGameSource gameSource)
		{
			string gameName = null;
			if (gameSource is BiosOnlyGameSource || gameSource == null)
				gameName = "(No Game)";
			else
				gameName = gameSource.GetGameName();

			string currentTime = DateTime.Now.ToString("u");
			currentTime = currentTime.Replace("Z", "");
			currentTime = currentTime.Replace("-", ".");
			currentTime = currentTime.Replace(":", ".");
			currentTime = currentTime.Replace(" ", "_");

			string fileName = string.Format("{0} - {1}",
				currentTime,
				GetFriendlyGameName(gameName));

			string screenshotFolder = GetScreenshotFilePath();

			string fullFilePath = Path.Combine(screenshotFolder, fileName);

			return fullFilePath;
		}

		private static string GetFriendlyGameName(string gameName)
		{
			if (gameName == null)
				return null;

			StringBuilder friendlyName = new StringBuilder();
			char[] nameChars = gameName.ToCharArray();
			for (int x = 0; x < nameChars.Length; x++)
			{
				if (char.IsLetter(nameChars[x]) || char.IsNumber(nameChars[x]))
				{
					friendlyName.Append(nameChars[x].ToString());
					if (friendlyName.Length >= MAXIMUM_FRIENDLY_NAME_LENGTH)
						break;
				}
			}

			if (friendlyName.Length == 0)
				return null;

			return friendlyName.ToString();
		}

		private static string GetFullSaveStateFilePath(string fileNamePrefix, int saveStateSlot)
		{
			string saveStateFolder = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), SAVE_SUBFOLDER);
			string saveFileName = string.Format("{0}.{1}{2}", fileNamePrefix, saveStateSlot, SAVE_STATE_EXTENSION);
			return Path.Combine(saveStateFolder, saveFileName);
		}

		private static string GetScreenshotFilePath()
		{
			return Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), SCREENSHOT_SUBFOLDER);
		}
	}
}
