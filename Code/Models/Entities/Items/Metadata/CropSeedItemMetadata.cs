public class CropSeedItemMetadata : BaseItemMetadata
{
	public InventoryCropSeedType CropType { get; private set; }

	public CropSeedItemMetadata(string iconPath, InventoryCropSeedType cropType, int stackLimit, float value) : base(iconPath, stackLimit, value)
	{
		Type = InventoryItemType.seed;
		CropType = cropType;
	}
}

public enum InventoryCropSeedType
{
	Artichoke,
	Carrot,
	Corn,
	Gourd,
	Potato,
	Tomato,
	Tree_Pine,
	Tree_Oak,
	Tree_Apple,
}