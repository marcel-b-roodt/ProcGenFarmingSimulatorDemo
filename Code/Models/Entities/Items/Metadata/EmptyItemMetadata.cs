public class EmptyItemMetadata : BaseItemMetadata
{
	public EmptyItemMetadata(string iconPath, int stackLimit, float value) : base(iconPath, stackLimit, value)
	{
		Type = InventoryItemType.unset;
	}
}