public class BuildingKitMetadata : BaseItemMetadata
{
	public BuildingType BuildingType { get; private set; }

	public BuildingKitMetadata(string iconPath, BuildingType buildingType, float value) : base(iconPath, stackLimit: 1, value)
	{
		Type = InventoryItemType.buildingKit;
		BuildingType = buildingType;
	}
}

public enum BuildingType
{
	Bed,
}