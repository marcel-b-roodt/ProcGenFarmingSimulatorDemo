using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static Helpers;

[Serializable]
public class PlayerInventory
{
	public const int BACKPACK_LEVEL_SLOT_MULTIPIER = 10;
	public const int BACKPACK_MAX_LEVEL = 3;

	public Dictionary<int, PlayerInventorySlot> ItemSlots { get; private set; }
	[Export] public int BackpackLevel { get; set; }

	public int FinalMaxSlots { get { return BACKPACK_MAX_LEVEL * BACKPACK_LEVEL_SLOT_MULTIPIER; } }
	public int CurrentMaxSlots { get { return (BackpackLevel + 1) * BACKPACK_LEVEL_SLOT_MULTIPIER; } }

	public PlayerInventory()
	{
		BackpackLevel = 1;

		ItemSlots = new Dictionary<int, PlayerInventorySlot>(CurrentMaxSlots);

		foreach (int index in GD.Range(0, CurrentMaxSlots))
		{
			ItemSlots.Add(index, new PlayerInventorySlot());
		}
	}

	public void UpgradeBackpack()
	{
		BackpackLevel++;

		if (BackpackLevel > BACKPACK_MAX_LEVEL)
			BackpackLevel = BACKPACK_MAX_LEVEL;

		foreach (int index in GD.Range(ItemSlots.Count - 1, CurrentMaxSlots))
		{
			ItemSlots.Add(index, new PlayerInventorySlot());
		}

		EventManager.RaiseEvent(GlobalEventCodes.Player_InventoryBackpackUpgraded, null);
	}

	public PlayerInventorySlot GetItemByKey(int ID)
	{
		return ItemSlots[ID];
	}

	public int GetItemKey(PlayerInventorySlot item)
	{
		foreach (var slot in ItemSlots)
		{
			if (slot.Value == item)
				return slot.Key;
		}

		return -1;
	}

	public void UpdateItem(PlayerInventorySlot item)
	{
		var key = GetItemKey(item);
		EventManager.RaiseEvent(GlobalEventCodes.Player_InventoryCellUpdated, key);
	}

	public int GetItemTotalQuantity(ItemMetadataID itemMetadataID)
	{
		var totalQuantity = 0;

		foreach (var item in ItemSlots.Values)
		{
			if (item.Item.Metadata.ID == itemMetadataID)
			{
				totalQuantity += item.Quantity;
			}
		}

		return totalQuantity;
	}

	public void AddItem(InventoryItem item, int quantity, out int unhandledQuantity)
	{
		unhandledQuantity = quantity;

		if (item.Empty)
			return;

		var remainingQuantity = quantity;

		if (!item.Metadata.Stackable)
		{
			var emptySlotIndex = GetEmptySlot();
			if (emptySlotIndex < 0)
				return;

			ItemSlots[emptySlotIndex].SetNewItemData(item, 1);
			EventManager.RaiseEvent(GlobalEventCodes.Player_InventoryCellUpdated, emptySlotIndex);
			unhandledQuantity = 0;
		}
		else
		{
			var matchingSlots = ItemSlots.Where(slot => slot.Value.Item.Metadata.ID == item.Metadata.ID)
				.Select(slot => slot.Key);

			foreach (var key in matchingSlots)
			{
				var preAddQuantity = remainingQuantity;
				remainingQuantity = ItemSlots[key].AddQuantity(remainingQuantity);

				if (preAddQuantity != remainingQuantity)
					EventManager.RaiseEvent(GlobalEventCodes.Player_InventoryCellUpdated, key);
			}

			unhandledQuantity = AssignOverflowQuantityOfItemToNewSlots(item, remainingQuantity);
		}
	}

	public bool ConsumeItem(ItemMetadataID itemID, int quantity)
	{
		var remainingQuantity = quantity;
		if (remainingQuantity > GetItemTotalQuantity(itemID))
			return false;

		if (itemID == ItemMetadataID.meta_empty)
			return false;

		var matchingSlots = ItemSlots.Where(item => item.Value.Item.Metadata.ID == itemID)
				.Select(item => item.Key);

		foreach (var key in matchingSlots.Reverse())
		{
			RemoveItemQuantityAndUpdate(key, remainingQuantity, out remainingQuantity, false);
		}

		return true;
	}

	public int AddItem(ref PlayerInventorySlot slot)
	{
		var startingItemQuantity = slot.Quantity;
		AddItem(slot.Item, slot.Quantity, out int unhandledQuantity);
		var consumedQuantity = startingItemQuantity - unhandledQuantity;
		slot.RemoveQuantity(consumedQuantity);
		return consumedQuantity;
	}

	public int TakeItemQuantity(int key, int targetQuantity)
	{
		RemoveItemQuantityAndUpdate(key, targetQuantity, out int removedQuantity, false);
		return removedQuantity;
	}

	public int DropItemQuantity(int key, int targetQuantity)
	{
		return RemoveItemQuantityAndUpdate(key, targetQuantity, out _, true);
	}

	public void MoveItemToSlot(ref PlayerInventorySlot fromItem, int toSlot)
	{
		var toItem = ItemSlots[toSlot];

		if (fromItem.Item.Metadata.ID == toItem.Item.Metadata.ID && fromItem.Item.Metadata.Stackable)
		{
			SplitQuantityFromItem(ref fromItem, toSlot);
		}
		else
		{
			SwitchItems(ref fromItem, toSlot);
		}

		EventManager.RaiseEvent(GlobalEventCodes.Player_InventoryCellUpdated, toSlot);
	}

	public void SplitQuantityFromItem(ref PlayerInventorySlot fromItem, int toSlot)
	{
		var toItem = ItemSlots[toSlot];
		var startingItemQuantity = fromItem.Quantity;

		if (toItem.Item.Empty)
			SwitchItems(ref fromItem, toSlot);
		else
		{
			var remainingQuantity = toItem.AddQuantity(fromItem.Quantity);
			fromItem.RemoveQuantity(startingItemQuantity - remainingQuantity);
		}
	}

	private void SwitchItems(ref PlayerInventorySlot fromItem, int toSlot)
	{
		var temp = new PlayerInventorySlot(fromItem);
		fromItem.CopyItemProperties(ItemSlots[toSlot]);
		ItemSlots[toSlot].CopyItemProperties(temp);
	}

	private int GetEmptySlot()
	{
		foreach (int key in ItemSlots.Keys)
		{
			var currentItem = ItemSlots[key];

			if (currentItem.Item.Empty)
				return key;
		}

		//Debug.Print($"Inventory is full. No free slots available");
		return -1;
	}

	private int AssignOverflowQuantityOfItemToNewSlots(InventoryItem item, int remainingQuantity)
	{
		while (remainingQuantity > 0)
		{
			var emptySlotIndex = GetEmptySlot();

			if (emptySlotIndex >= 0)
			{
				var newSlot = ItemSlots[emptySlotIndex];
				//Debug.Print($"Allocated new slot to {emptySlotIndex}");
				remainingQuantity = newSlot.SetNewItemData(item, remainingQuantity);
				EventManager.RaiseEvent(GlobalEventCodes.Player_InventoryCellUpdated, emptySlotIndex);
			}
			else
			{
				//Debug.Print($"Inventory full! Cannot add the item");
				break;
			}
		}

		return remainingQuantity;
	}

	private int RemoveItemQuantityAndUpdate(int key, int quantity, out int removedQuantity, bool dropItem = true)
	{
		var item = ItemSlots[key];
		var initialQuantity = item.Quantity;
		var initialItem = item.Item;

		var remainingQuantity = item.RemoveQuantity(quantity);
		EventManager.RaiseEvent(GlobalEventCodes.Player_InventoryCellUpdated, key);
		removedQuantity = initialQuantity - remainingQuantity;

		if (dropItem)
		{
			Global.Instance.GameWorld.SpawnPickup(PlayerUtils.GetRandomPointNearPlayer(), initialItem, removedQuantity);
		}

		return remainingQuantity;
	}
}