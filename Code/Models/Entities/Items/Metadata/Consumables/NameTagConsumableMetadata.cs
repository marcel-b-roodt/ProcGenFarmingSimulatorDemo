public class NameTagConsumableMetadata : ConsumableItemMetadata
{
	public NameTagConsumableMetadata(string iconPath, int stackLimit, float value)
		: base(iconPath, InventoryConsumableType.Nametag, stackLimit, value)
	{ }
}