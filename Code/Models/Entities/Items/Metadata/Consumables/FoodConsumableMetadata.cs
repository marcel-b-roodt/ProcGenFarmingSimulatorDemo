public class FoodConsumableMetadata : ConsumableItemMetadata
{
	public float HungerRestored;
	public float SleepRestored;
	public float ThirstRestored;

	public FoodConsumableMetadata(string iconPath, int stackLimit, float value)
		: base(iconPath, InventoryConsumableType.Food, stackLimit, value)
	{ }
}