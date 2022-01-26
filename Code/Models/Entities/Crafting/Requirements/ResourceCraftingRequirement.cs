public class ResourceCraftingRequirement : BaseCraftingRequirement
{
	public ItemMetadataID ItemID;
	public int Quantity;

	public ResourceCraftingRequirement(ItemMetadataID itemID, int quantity)
	{
		ItemID = itemID;
		Quantity = quantity;
	}

	public override bool ValidateRequirement()
	{
		var playerQuantity = Global.Instance.PlayerData.Inventory.GetItemTotalQuantity(ItemID);
		return playerQuantity >= Quantity;
	}
}