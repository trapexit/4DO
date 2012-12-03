using System;
using System.IO;
using System.Xml;
using FourDO.Emulation.FreeDO;

namespace FourDO.Emulation.States
{
	internal static class SaveStateHelper
	{
		public static void SaveState(string fileName)
		{
			BinaryWriter binaryWriter = null;
			try
			{
				//////////////////////////////////
				// Create directory if it doesn't exist
				string saveDirectory = Path.GetDirectoryName(fileName);
				if (saveDirectory == null)
					throw new DirectoryNotFoundException("Could not identify containing folder for: " + fileName);

				if (!Directory.Exists(saveDirectory))
					Directory.CreateDirectory(saveDirectory);

				//////////////////////////////////
				// Save binary data from core emulation.
				binaryWriter = new BinaryWriter(new FileStream(fileName, FileMode.Create));
				var saveData = new byte[FreeDOCore.GetSaveSize()];
				unsafe
				{
					fixed (byte* saveDataPtr = saveData)
					{
						var pointer = new IntPtr(saveDataPtr);
						FreeDOCore.DoSave(pointer);
					}
				}
				binaryWriter.Write(saveData);
				binaryWriter.Close();
			}
			finally
			{
				if (binaryWriter != null)
					binaryWriter.Close();
			}
		}

		public static void LoadState(string fileName)
		{
			BinaryReader reader = null;
			try
			{
				reader = new BinaryReader(new FileStream(fileName, FileMode.Open));
				var saveData = reader.ReadBytes((int)FreeDOCore.GetSaveSize());
				unsafe
				{
					fixed (byte* saveDataPtr = saveData)
					{
						var pointer = new IntPtr(saveDataPtr);
						FreeDOCore.DoLoad(pointer);
					}
				}
				reader.Close();
			}
			finally
			{
				if (reader != null)
					reader.Close();
			}
		}
	}
}
