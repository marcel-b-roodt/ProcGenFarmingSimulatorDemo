
public class InventoryItem
{
	public BaseItemMetadata Metadata = ItemLookup.NoItem();
	public int CurrentCondition { get; private set; }
	public bool Empty { get { return Metadata.ID == ItemMetadataID.meta_empty; } }

	public InventoryItem()
	{
		Metadata = ItemLookup.NoItem();
	}

	public InventoryItem(ItemMetadataID id, int condition = -1)
	{
		Metadata = ItemLookup.Get(id);

		if (Metadata is ToolItemMetadata && condition < 0)
			CurrentCondition = ((ToolItemMetadata)Metadata).MaxCondition;
		else
			CurrentCondition = condition;
	}

	//TODO: Add Item Actions here to update what happens in the item
	public void TakeDamage(int damage)
	{
		if (Metadata is ToolItemMetadata)
		{
			CurrentCondition -= damage;
			if (CurrentCondition <= 0)
			{
				BreakItem();
			}
		}
	}

	public void Consume()
	{
		if (Metadata is CropSeedItemMetadata)
		{
			//Nothing really happens here
		}
		else if (Metadata is ConsumableItemMetadata)
		{
			//TODO: Get Benefit of Consumable
		}
	}

	public void BreakItem()
	{
		//TODO: Play Sound
		Metadata = ItemLookup.NoItem();
	}
}
