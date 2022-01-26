using Godot;
using System.Collections.Generic;
using System.Text;
using static Helpers;

public class InventoryTab : Panel
{
	private PlayerInventorySlot cursorItem = new PlayerInventorySlot();

	public int selectedRowIndex = 0; //0-3
	public int selectedColumnIndex = 0; //0-9
	private int selectedSlot { get { return selectedRowIndex * PlayerInventory.BACKPACK_LEVEL_SLOT_MULTIPIER + selectedColumnIndex; } }  //public int inventoryIndex = rowIndex * 10 + columnIndex;
	private int fromSlot = -1;

	private float itemQuantityDialogueHoldTime = 0.25f;
	private float scrollDelay = 0.1f;
	private float scrollInitialDelay = 0.25f;
	private float scrollTime = 0;

	private MainMenu mainMenu;
	private VBoxContainer itemList;
	private List<CenterContainer> inventoryRows = new List<CenterContainer>();

	private PopupPanel itemDetailPopup;
	private TextureRect itemDetailIcon;
	private RichTextLabel itemDetailLabel;

	private ItemQuantityPopup itemQuantityPopup;

	private InventoryCell itemCursorCell;

	private double timeSinceEnteringState;
	private InventoryActionState previousActionState;
	private InventoryActionState currentActionState;

	private SceneTree sceneTree;

	private void TransitionToState(InventoryActionState newState)
	{
		previousActionState = currentActionState;
		OnStateExit(previousActionState, newState);
		currentActionState = newState;
		OnStateEnter(newState, previousActionState);
		timeSinceEnteringState = 0;
	}

	private void OnStateEnter(InventoryActionState state, InventoryActionState fromState)
	{
		switch (state)
		{
			case InventoryActionState.SelectingItemForMove:
				fromSlot = selectedSlot;
				break;
			case InventoryActionState.HoldingItem:
				//OpenMenuMoveItems();
				break;
			case InventoryActionState.SelectingItemForDrop:
				fromSlot = selectedSlot;
				break;
			case InventoryActionState.Free:
				HideItemCursor();
				fromSlot = -1;
				break;
		}
	}

	private void OnStateExit(InventoryActionState state, InventoryActionState toState)
	{
		switch (state)
		{

		}
	}

	public override void _Ready()
	{
		base._Ready();

		EventManager.ListenEvent(GlobalEventCodes.Player_InventoryBackpackUpgraded, GD.FuncRef(this, nameof(OnBackpackUpgraded)));
		EventManager.ListenEvent(GlobalEventCodes.Player_InventoryCellUpdated, GD.FuncRef(this, nameof(OnCellUpdated)));

		//TODO: Enable the inventory rows on backpack upgrade
		mainMenu = (MainMenu)FindParent("MainMenu");

		itemList = GetNode<VBoxContainer>("VB_Items/ItemList");
		inventoryRows.Add(GetNode<CenterContainer>("VB_Items/ItemList/InventoryRow1"));
		inventoryRows.Add(GetNode<CenterContainer>("VB_Items/ItemList/InventoryRow2"));
		inventoryRows.Add(GetNode<CenterContainer>("VB_Items/ItemList/InventoryRow3"));
		inventoryRows.Add(GetNode<CenterContainer>("VB_Items/ItemList/InventoryRow4"));

		for (int i = 0; i < PlayerInventory.BACKPACK_MAX_LEVEL + 1; i++)
		{
			if (i <= Global.Instance.PlayerData.Inventory.BackpackLevel)
				continue;
			else
				inventoryRows[i].Visible = false;
		}

		itemDetailPopup = GetNode<PopupPanel>($"ItemDetailPopup");
		itemDetailIcon = GetNode<TextureRect>($"ItemDetailPopup/Margin/VB/ItemDetailIcon");
		itemDetailLabel = GetNode<RichTextLabel>($"ItemDetailPopup/Margin/VB/ItemDetailInfoLabel");

		itemQuantityPopup = GetNode<ItemQuantityPopup>("ItemQuantityPopup");
		itemCursorCell = GetNode<InventoryCell>($"CursorItem_InventoryCell");

		//ManagePersistentPopups();
		HookUpCellSignals();
		LoadCellInitialItems();

		DeselectInventoryItem(selectedRowIndex, selectedColumnIndex);
		selectedRowIndex = 0;
		selectedColumnIndex = 0;
		SelectInventoryItem(selectedRowIndex, selectedColumnIndex);
	}

	public override void _EnterTree()
	{
		base._EnterTree();

		sceneTree = GetTree();
	}

	public override void _Process(float delta)
	{
		base._Process(delta);
		timeSinceEnteringState += delta;

		if (mainMenu.InventoryActive)
		{
			if (!Visible)
				Show();

			switch (currentActionState)
			{
				case InventoryActionState.SelectingItemForMove:
					if (timeSinceEnteringState >= itemQuantityDialogueHoldTime && !itemQuantityPopup.Visible)
						OpenItemQuantityDialogue();

					if (Input.IsActionJustReleased(PlayerInputCodes.A))
					{
						if (itemQuantityPopup.Visible)
							SwitchItemQuantityWithCursorItem(itemQuantityPopup.SliderValue);
						else
							SwitchItemsWithCursorItem();
					}

					break;
				case InventoryActionState.HoldingItem:
					#region ItemListScroll
					if (scrollTime <= 0)
					{
						if (Input.IsActionPressed(PlayerInputCodes.UI_Up))
						{
							ScrollItemList(ScrollDirection.Up);
						}

						else if (Input.IsActionPressed(PlayerInputCodes.UI_Down))
						{
							ScrollItemList(ScrollDirection.Down);
						}

						else if (Input.IsActionPressed(PlayerInputCodes.UI_Left))
						{
							ScrollItemList(ScrollDirection.Left);
						}

						else if (Input.IsActionPressed(PlayerInputCodes.UI_Right))
						{
							ScrollItemList(ScrollDirection.Right);
						}

						else if (Input.IsActionJustReleased(PlayerInputCodes.UI_Up))
						{
							scrollTime = 0;
						}

						else if (Input.IsActionJustReleased(PlayerInputCodes.UI_Down))
						{
							scrollTime = 0;
						}

						else if (Input.IsActionJustReleased(PlayerInputCodes.UI_Left))
						{
							scrollTime = 0;
						}

						else if (Input.IsActionJustReleased(PlayerInputCodes.UI_Right))
						{
							scrollTime = 0;
						}
					}
					else
					{
						scrollTime -= delta;
					}
					#endregion

					if (Input.IsActionJustReleased(PlayerInputCodes.A))
						MoveItemToSlot();

					else if (Input.IsActionJustReleased(PlayerInputCodes.B))
						DropSingleItemFromCursorItem();

					break;
				case InventoryActionState.SelectingItemForDrop:
					if (timeSinceEnteringState >= itemQuantityDialogueHoldTime && !itemQuantityPopup.Visible)
						OpenItemQuantityDialogue();

					if (Input.IsActionJustReleased(PlayerInputCodes.B))
					{
						if (itemQuantityPopup.Visible)
							DropItemQuantity(itemQuantityPopup.SliderValue);
						else
							DropSingleItem();
					}

					break;
				case InventoryActionState.Free:
					#region ItemListScroll
					if (scrollTime <= 0)
					{
						if (Input.IsActionPressed(PlayerInputCodes.UI_Up))
						{
							ScrollItemList(ScrollDirection.Up);
						}

						else if (Input.IsActionPressed(PlayerInputCodes.UI_Down))
						{
							ScrollItemList(ScrollDirection.Down);
						}

						else if (Input.IsActionPressed(PlayerInputCodes.UI_Left))
						{
							ScrollItemList(ScrollDirection.Left);
						}

						else if (Input.IsActionPressed(PlayerInputCodes.UI_Right))
						{
							ScrollItemList(ScrollDirection.Right);
						}

						else if (Input.IsActionJustReleased(PlayerInputCodes.UI_Up))
						{
							scrollTime = 0;
						}

						else if (Input.IsActionJustReleased(PlayerInputCodes.UI_Down))
						{
							scrollTime = 0;
						}

						else if (Input.IsActionJustReleased(PlayerInputCodes.UI_Left))
						{
							scrollTime = 0;
						}

						else if (Input.IsActionJustReleased(PlayerInputCodes.UI_Right))
						{
							scrollTime = 0;
						}
					}
					else
					{
						scrollTime -= delta;
					}
					#endregion

					if (Input.IsActionJustPressed(PlayerInputCodes.A))
					{
						StartSelectingItemForMove();
					}

					else if (Input.IsActionJustPressed(PlayerInputCodes.B))
					{
						DropItem();
					}

					else if (Input.IsActionJustPressed(PlayerInputCodes.X))
					{
						//ConsumeItem();
					}

					else if (Input.IsActionJustPressed(PlayerInputCodes.Y))
					{
						OpenItemInfo();
					}
					break;
			}
		}
		else
		{
			if (Visible)
				Hide();
		}
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (mainMenu.InventoryActive)
		{
			if (!Visible)
				Show();

			if (currentActionState == InventoryActionState.Free || currentActionState == InventoryActionState.HoldingItem)
			{
				#region ItemListInitialScroll
				if (Input.IsActionJustPressed(PlayerInputCodes.UI_Up) && @event.IsAction(PlayerInputCodes.UI_Up) && !sceneTree.IsInputHandled())
				{
					ScrollItemList(ScrollDirection.Up);
					scrollTime = scrollInitialDelay;
					sceneTree.SetInputAsHandled();
				}

				if (Input.IsActionJustPressed(PlayerInputCodes.UI_Down) && @event.IsAction(PlayerInputCodes.UI_Down) && !sceneTree.IsInputHandled())
				{
					ScrollItemList(ScrollDirection.Down);
					scrollTime = scrollInitialDelay;
					sceneTree.SetInputAsHandled();
				}

				if (Input.IsActionJustPressed(PlayerInputCodes.UI_Left) && @event.IsAction(PlayerInputCodes.UI_Left) && !sceneTree.IsInputHandled())
				{
					ScrollItemList(ScrollDirection.Left);
					scrollTime = scrollInitialDelay;
					sceneTree.SetInputAsHandled();
				}

				if (Input.IsActionJustPressed(PlayerInputCodes.UI_Right) && @event.IsAction(PlayerInputCodes.UI_Right) && !sceneTree.IsInputHandled())
				{
					ScrollItemList(ScrollDirection.Right);
					scrollTime = scrollInitialDelay;
					sceneTree.SetInputAsHandled();
				}
				#endregion
			}
		}
		else
		{
			if (Visible)
				Hide();
		}
	}

	//Menu preparation
	public new void Show()
	{
		base.Show();

		//Debug.Print("Showing Inventory Screen");
		DeselectInventoryItem(selectedRowIndex, selectedColumnIndex);
		selectedRowIndex = 0;
		selectedColumnIndex = 0;
		SelectInventoryItem(selectedRowIndex, selectedColumnIndex);
	}

	//Menu cleanup
	public new void Hide()
	{
		if (!cursorItem.Item.Empty)
		{
			Global.Instance.PlayerData.Inventory.AddItem(ref cursorItem);

			if (!cursorItem.Item.Empty)
			{
				var initialItem = cursorItem.Item;
				var initialQuantity = cursorItem.Quantity;
				cursorItem.RemoveQuantity(cursorItem.Quantity);
				Global.Instance.GameWorld.SpawnPickup(PlayerUtils.GetRandomPointNearPlayer(), initialItem, initialQuantity);
			}

		}

		HideItemInfo();
		itemQuantityPopup.HideItemQuantityDialogue();
		TransitionToState(InventoryActionState.Free);

		base.Hide();
	}

	public void SwitchItemQuantityWithCursorItem(int sliderQuantity)
	{
		var selectedItem = Global.Instance.PlayerData.Inventory.ItemSlots[selectedSlot];
		if (sliderQuantity == selectedItem.Quantity)
		{
			SwitchItemsWithCursorItem();
		}
		else
		{
			var quantity = Global.Instance.PlayerData.Inventory.TakeItemQuantity(selectedSlot, sliderQuantity);
			cursorItem.SetNewItemData(selectedItem.Item, quantity);
		}

		UpdateItemCursor();
		TransitionToState(InventoryActionState.HoldingItem);
		itemQuantityPopup.HideItemQuantityDialogue();
	}

	public void DropItemQuantity(int sliderQuantity)
	{
		DropManyItems(sliderQuantity);
		itemQuantityPopup.HideItemQuantityDialogue();
	}

	private void OnBackpackUpgraded()
	{
		//TODO: Implement backpack upgrades
	}

	private void OnCellUpdated(int index)
	{
		UpdateSlot(index);
	}

	private void ScrollItemList(ScrollDirection scrollDirection)
	{
		DeselectInventoryItem(selectedRowIndex, selectedColumnIndex);

		switch (scrollDirection)
		{
			case ScrollDirection.Up:
				selectedRowIndex -= 1;
				if (selectedRowIndex < 0)
					selectedRowIndex = 0;
				break;
			case ScrollDirection.Down:
				selectedRowIndex += 1;
				if (selectedRowIndex >= Global.Instance.PlayerData.Inventory.BackpackLevel)
					selectedRowIndex = Global.Instance.PlayerData.Inventory.BackpackLevel;
				break;
			case ScrollDirection.Left:
				selectedColumnIndex -= 1;
				if (selectedColumnIndex < 0)
					selectedColumnIndex = 0;
				break;
			case ScrollDirection.Right:
				selectedColumnIndex += 1;
				if (selectedColumnIndex >= PlayerInventory.BACKPACK_LEVEL_SLOT_MULTIPIER - 1)
					selectedColumnIndex = PlayerInventory.BACKPACK_LEVEL_SLOT_MULTIPIER - 1;
				break;
		}

		SelectInventoryItem(selectedRowIndex, selectedColumnIndex);
		scrollTime = scrollDelay;
	}

	private void SelectCell(int rowIndex, int columnIndex)
	{
		Debug.Print($"Cell ({rowIndex},{columnIndex}): Click!");
		DeselectInventoryItem(selectedRowIndex, selectedColumnIndex);
		selectedRowIndex = rowIndex;
		selectedColumnIndex = columnIndex;
		SelectInventoryItem(selectedRowIndex, selectedColumnIndex);
	}

	private void HookUpCellSignals()
	{
		var rowIndex = 0;
		foreach (var row in inventoryRows)
		{
			var columnIndex = 0;
			foreach (var cell in row.GetChild(0).GetChildren())
			{
				var inventoryCell = cell as InventoryCell;
				inventoryCell.Connect("pressed", this, nameof(SelectCell), new Godot.Collections.Array { rowIndex, columnIndex });

				//TODO: Update left click to pick up item stack. Wait timer: Quick click picks up whole stack. Hold + delay opens quantity menu
				//TODO: Move the Item Quantity Dialogue to be right underneath the selected cell. Do the same for Dropping


				//TODO: Right click for the info screen. Mouse_Exit will close the info for this item
				//TODO: Middle mouse for drop item. Hold middle mouse for 

				//Debug.Print($"{row.Name}: {inventoryCell.Name}: Connected signal with indexes ({rowIndex},{columnIndex})");

				columnIndex++;
			}

			rowIndex++;
		}
	}

	private void LoadCellInitialItems()
	{
		foreach (int slot in Global.Instance.PlayerData.Inventory.ItemSlots.Keys)
		{
			UpdateSlot(slot);
		}
	}

	private void DeselectInventoryItem(int rowIndex, int columnIndex)
	{
		inventoryRows[rowIndex].GetChild(0).GetChild<InventoryCell>(columnIndex).TextureNormal = ImageStore.Get(nameof(ImageStore.UI.UI_EmptyCell));
	}

	private void SelectInventoryItem(int rowIndex, int columnIndex)
	{
		inventoryRows[rowIndex].GetChild(0).GetChild<InventoryCell>(columnIndex).TextureNormal = ImageStore.Get(nameof(ImageStore.UI.UI_SelectedCell));
		MoveItemCursor();
		HideItemInfo();
	}

	private void UpdateSlot(int slot)
	{
		var inventoryItem = Global.Instance.PlayerData.Inventory.ItemSlots[slot];
		InventoryCell inventoryCell = GetInventorySlotAtIndex(slot);
		inventoryCell.SetData(inventoryItem);
	}

	private InventoryCell GetInventorySlotAtIndex(int slot)
	{
		int rowIndex = slot / PlayerInventory.BACKPACK_LEVEL_SLOT_MULTIPIER;
		int columnIndex = slot % PlayerInventory.BACKPACK_LEVEL_SLOT_MULTIPIER;

		return inventoryRows[rowIndex].GetChild(0).GetChild<InventoryCell>(columnIndex);
	}

	private InventoryCell GetSelectedInventorySlot()
	{
		return inventoryRows[selectedRowIndex].GetChild(0).GetChild<InventoryCell>(selectedColumnIndex);
	}

	private void UpdateItemCursor()
	{
		MoveItemCursor();
		itemCursorCell.SetData(cursorItem);
		itemCursorCell.Visible = true;
	}

	private void HideItemCursor()
	{
		itemCursorCell.Visible = false;
	}

	private void MoveItemCursor()
	{
		InventoryCell selectedItem = GetSelectedInventorySlot();
		Vector2 position = selectedItem.RectPosition;
		position.x += selectedItem.RectSize.x;
		position.y += selectedItem.RectSize.y;

		itemCursorCell.SetPosition(position);
	}

	private void OpenItemInfo()
	{
		InventoryCell selectedItem = GetSelectedInventorySlot();
		var inventorySlot = Global.Instance.PlayerData.Inventory.ItemSlots[selectedSlot];
		if (inventorySlot.Item.Empty)
			return;

		StringBuilder itemInfoText = new StringBuilder();
		Vector2 position = selectedItem.RectGlobalPosition;
		position.x += selectedItem.RectSize.x;
		position.y += selectedItem.RectSize.y;

		itemDetailPopup.RectPosition = position;
		itemDetailIcon.Texture = inventorySlot.Item.Metadata.IconTexture;

		itemInfoText.AppendLine($"Name: [color={Colours.UI.Inventory_ItemPopupName}]{inventorySlot.Item.Metadata.Name}[/color]");
		itemInfoText.AppendLine($"Type: [color={Colours.UI.Inventory_ItemPopupType}]{inventorySlot.Item.Metadata.Type}[/color]");
		itemInfoText.AppendLine($"[color={Colours.UI.Inventory_ItemPopupDescription}]{inventorySlot.Item.Metadata.Description}[/color]");

		itemDetailLabel.BbcodeText = itemInfoText.ToString();
		itemDetailPopup.Show();
	}

	private void HideItemInfo()
	{
		itemDetailPopup.Visible = false;
	}

	private void DropItem()
	{
		var selectedItem = GetInventorySlotAtIndex(selectedSlot);
		var inventorySlot = Global.Instance.PlayerData.Inventory.ItemSlots[selectedSlot];
		if (inventorySlot.Item.Empty)
			return;

		if (inventorySlot.Quantity <= 5)
			DropSingleItem();
		else
			TransitionToState(InventoryActionState.SelectingItemForDrop);
	}

	private void DropSingleItem()
	{
		var newAmount = Global.Instance.PlayerData.Inventory.DropItemQuantity(selectedSlot, 1);
		if (newAmount <= 0)
			HideItemInfo();

		TransitionToState(InventoryActionState.Free);
	}

	private void DropSingleItemFromCursorItem()
	{
		var initialItem = cursorItem.Item;
		var removedQuantity = cursorItem.RemoveQuantity(1);
		Global.Instance.GameWorld.SpawnPickup(PlayerUtils.GetRandomPointNearPlayer(), cursorItem.Item, 1);

		if (cursorItem.Item.Empty)
		{
			TransitionToState(InventoryActionState.Free);
			HideItemCursor();
		}
		else
		{
			UpdateItemCursor();
		}

	}

	private void DropManyItems(int selectedQuantity)
	{
		var newAmount = Global.Instance.PlayerData.Inventory.DropItemQuantity(selectedSlot, selectedQuantity);
		if (newAmount <= 0)
			HideItemInfo();

		GetSelectedInventorySlot().GrabFocus();
		TransitionToState(InventoryActionState.Free);
	}

	private void StartSelectingItemForMove()
	{
		var inventorySlot = Global.Instance.PlayerData.Inventory.ItemSlots[selectedSlot];
		if (inventorySlot.Item.Empty)
			return;

		TransitionToState(InventoryActionState.SelectingItemForMove);
	}

	private void SwitchItemsWithCursorItem()
	{
		Global.Instance.PlayerData.Inventory.MoveItemToSlot(ref cursorItem, selectedSlot);
		UpdateItemCursor();

		if (cursorItem.Item.Empty)
			TransitionToState(InventoryActionState.Free);
		else
			TransitionToState(InventoryActionState.HoldingItem);
	}

	private void MoveItemToSlot()
	{
		Global.Instance.PlayerData.Inventory.MoveItemToSlot(ref cursorItem, selectedSlot);
		UpdateItemCursor();

		if (cursorItem.Quantity == 0 || cursorItem.Item.Empty)
			TransitionToState(InventoryActionState.Free);
	}

	private void OpenItemQuantityDialogue()
	{
		var inventoryItem = Global.Instance.PlayerData.Inventory.ItemSlots[selectedSlot];
		if (inventoryItem.Item.Empty)
			return;

		itemQuantityPopup.ShowItemQuantityDialogue(inventoryItem.Quantity);
	}

	private enum InventoryActionState
	{
		Free,
		SelectingItemForMove,
		HoldingItem,
		SelectingItemForDrop
	}

	private enum ScrollDirection
	{
		Up, Down, Left, Right
	}
}
