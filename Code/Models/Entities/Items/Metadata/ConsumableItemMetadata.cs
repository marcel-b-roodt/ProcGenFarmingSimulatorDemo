public class ConsumableItemMetadata : BaseItemMetadata
{
	public InventoryConsumableType ConsumableType { get; private set; }

	public ConsumableItemMetadata(string iconPath, InventoryConsumableType consumableType, int stackLimit, float value) : base(iconPath, stackLimit, value)
	{
		Type = InventoryItemType.consumable;
		ConsumableType = consumableType;
	}
}

public enum InventoryConsumableType
{
	Food,
	Nametag,
	Potion,
}