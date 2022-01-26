public class ToolItemMetadata : BaseItemMetadata
{
	public InventoryToolType ToolType { get; private set; }
	public int MaxCondition { get; private set; }
	public int Damage { get; private set; }

	//TODO: Turn this Tool class into multiple tool type Metadatas. The Metadata should have the methods to do work
	public ToolItemMetadata(string iconPath, InventoryToolType toolType, int maxCondition, int damage, float value) : base(iconPath, 1, value)
	{
		Type = InventoryItemType.tool;
		ToolType = toolType;
		MaxCondition = maxCondition;
		Damage = damage;
	}
}

public enum InventoryToolType
{
	spade = 0,
	hoe = 1,
	pickaxe = 2,
	axe = 3,
	melee = 4,
}