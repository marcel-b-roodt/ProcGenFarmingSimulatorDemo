public class MiscItemMetadata : BaseItemMetadata
{
	public MiscItemMetadata(string iconPath, int stackLimit, float value) : base(iconPath, stackLimit, value)
	{
		Type = InventoryItemType.misc;
	}
}