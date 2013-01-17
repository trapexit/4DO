namespace FourDO.FileSystem
{
	public interface IItem
	{
		ItemType ItemType { get; }
		Directory Parent { get; }
		string Name { get; }
		string GetFullPath();
	}
}
