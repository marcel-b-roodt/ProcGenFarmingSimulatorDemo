using System;

[Serializable]
public class PlayerInventorySlot
{
	//TODO: Separate out the PlayerInventorySlot with InventoryItem. 
	//We need to differentiate them so that we can maintain the stats of a used tool/item, i.e. quality
	public InventoryItem Item = new InventoryItem();
	public int Quantity { get; private set; }
	public int AvailableQuantityInStack { get { return Item.Metadata.StackLimit - Quantity; } }
	public string DisplayQuantity
	{
		get
		{
			if (Item.Empty || !Item.Metadata.Stackable)
				return "";
			else
				return Quantity.ToString();
		}
	}

	public PlayerInventorySlot()
	{
		Item = new InventoryItem();
		Quantity = 0;
	}

	public PlayerInventorySlot(PlayerInventorySlot copy)
	{
		Item = copy.Item;
		Quantity = copy.Quantity;
	}

	//TODO: Add Player Slot Updates here to trigger updates on the Inventory System
	public void TakeDamage(int damage)
	{
		Item.TakeDamage(damage);
		Global.Instance.PlayerData.Inventory.UpdateItem(this);
	}

	public void ConsumeItem()
	{
		Item.Consume();
		RemoveQuantity(1);
		Global.Instance.PlayerData.Inventory.UpdateItem(this);
	}

	public int AddQuantity(int quantity)
	{
		var stackLimit = Item.Metadata.StackLimit;
		Quantity += quantity;

		if (Quantity > stackLimit)
		{
			var remainder = Quantity - stackLimit;
			Quantity = stackLimit;
			return remainder;
		}

		return 0;
	}

	public int RemoveQuantity(int quantity)
	{
		if (Quantity < quantity)
			quantity = Quantity;

		Quantity -= quantity;
		if (Quantity == 0)
		{
			Item = new InventoryItem();
			return 0;
		}
		else
		{
			return Quantity; //RemainingQuantity
		}
	}

	public int SetNewItemData(InventoryItem item, int quantity)
	{
		this.Item = item;
		return AddQuantity(quantity);
	}

	public void CopyItemProperties(PlayerInventorySlot other)
	{
		this.Item = other.Item;
		this.Quantity = other.Quantity;
	}

	public bool CanMergeWith(PlayerInventorySlot toItem)
	{
		return Item.Metadata.ID == toItem.Item.Metadata.ID && Item.Metadata.Stackable;
	}
}