namespace FourDO.Emulation.GameSource
{
	internal class GameRecord
	{
		public GameRecord (
			string id,
			string checkSum,
			string name,
			string releaseYear,
			string publisher,
			string regions)
		{
			Id = id;
			CheckSum = checkSum;
			Name = name;
			ReleaseYear = releaseYear;
			Publisher = publisher;
			Regions = regions;
		}

		public string Id { get; private set; }
		public string CheckSum { get; private set; }
		public string Name { get; private set; }
		public string ReleaseYear { get; private set; }
		public string Publisher { get; private set; }
		public string Regions { get; private set; }
	}
}
