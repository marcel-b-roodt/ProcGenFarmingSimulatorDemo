using Godot;
using System.Collections.Generic;
using static Helpers;

public class QuickSlotMenu : Panel
{
	public int selectedRowIndex = 0; //0-3
	public int selectedColumnIndex = 0; //0-9
	private int selectedSlot { get { return selectedRowIndex * PlayerInventory.BACKPACK_LEVEL_SLOT_MULTIPIER + selectedColumnIndex; } }  //public int inventoryIndex = rowIndex * 10 + columnIndex;

	private float scrollDelay = 0.1f;
	private float scrollInitialDelay = 0.25f;
	private float scrollTime = 0;

	public bool MenuActive { get; private set; }

	private List<TextureButton> inventoryHeaders = new List<TextureButton>();
	private List<CenterContainer> inventoryRows = new List<CenterContainer>();

	private SceneTree sceneTree;

	public override void _Ready()
	{
		EventManager.ListenEvent(GlobalEventCodes.Player_InventoryBackpackUpgraded, GD.FuncRef(this, nameof(OnBackpackUpgraded)));
		EventManager.ListenEvent(GlobalEventCodes.Player_InventoryCellUpdated, GD.FuncRef(this, nameof(OnCellUpdated)));

		//TODO: Enable the inventory rows on backpack upgrade
		inventoryHeaders.Add(GetNode<TextureButton>("VB/ItemRowHeaders/Row1"));
		inventoryHeaders.Add(GetNode<TextureButton>("VB/ItemRowHeaders/Row2"));
		inventoryHeaders.Add(GetNode<TextureButton>("VB/ItemRowHeaders/Row3"));
		inventoryHeaders.Add(GetNode<TextureButton>("VB/ItemRowHeaders/Row4"));

		inventoryRows.Add(GetNode<CenterContainer>("VB/PC/ItemList/InventoryRow1"));
		inventoryRows.Add(GetNode<CenterContainer>("VB/PC/ItemList/InventoryRow2"));
		inventoryRows.Add(GetNode<CenterContainer>("VB/PC/ItemList/InventoryRow3"));
		inventoryRows.Add(GetNode<CenterContainer>("VB/PC/ItemList/InventoryRow4"));

		for (int i = 0; i < PlayerInventory.BACKPACK_MAX_LEVEL + 1; i++)
		{
			if (i <= Global.Instance.PlayerData.Inventory.BackpackLevel)
				continue;
			else
			{
				inventoryHeaders[i].Visible = false;
				//inventoryRows[i].Visible = false;
			}
		}

		LoadCellInitialItems();

		DeselectInventoryItem(selectedRowIndex, selectedColumnIndex);
		selectedRowIndex = 0;
		selectedColumnIndex = 0;
		SelectInventoryItem(selectedRowIndex, selectedColumnIndex);
		inventoryHeaders[selectedRowIndex].TextureNormal = ImageStore.Get(nameof(ImageStore.UI.UI_SelectedCell));
	}

	public override void _EnterTree()
	{
		base._EnterTree();

		sceneTree = GetTree();
	}

	public override void _Process(float delta)
	{
		base._Process(delta);

		if (MenuActive)
		{
			#region ItemListScroll
			if (scrollTime <= 0)
			{
				if (Input.IsActionPressed(Helpers.PlayerInputCodes.DPad_Up) || Input.IsActionJustPressed(Helpers.PlayerInputCodes.L2))
				{
					ScrollItemList(ScrollDirection.Up);
				}

				else if (Input.IsActionPressed(Helpers.PlayerInputCodes.DPad_Down) || Input.IsActionJustPressed(Helpers.PlayerInputCodes.R2))
				{
					ScrollItemList(ScrollDirection.Down);
				}

				else if (Input.IsActionPressed(Helpers.PlayerInputCodes.DPad_Left) || Input.IsActionJustPressed(Helpers.PlayerInputCodes.L1))
				{
					ScrollItemList(ScrollDirection.Left);
				}

				else if (Input.IsActionPressed(Helpers.PlayerInputCodes.DPad_Right) || Input.IsActionJustPressed(Helpers.PlayerInputCodes.R1))
				{
					ScrollItemList(ScrollDirection.Right);
				}

				else if (Input.IsActionJustReleased(Helpers.PlayerInputCodes.DPad_Up) || Input.IsActionJustReleased(Helpers.PlayerInputCodes.L2))
				{
					scrollTime = 0;
				}

				else if (Input.IsActionJustReleased(Helpers.PlayerInputCodes.DPad_Down) || Input.IsActionJustReleased(Helpers.PlayerInputCodes.R2))
				{
					scrollTime = 0;
				}

				else if (Input.IsActionJustReleased(Helpers.PlayerInputCodes.DPad_Left) || Input.IsActionJustReleased(Helpers.PlayerInputCodes.L1))
				{
					scrollTime = 0;
				}

				else if (Input.IsActionJustReleased(Helpers.PlayerInputCodes.DPad_Right) || Input.IsActionJustReleased(Helpers.PlayerInputCodes.R1))
				{
					scrollTime = 0;
				}
			}
			else
			{
				scrollTime -= delta;
			}
			#endregion
		}
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (MenuActive)
		{
			#region ItemListInitialScroll
			if ((Input.IsActionJustPressed(PlayerInputCodes.DPad_Up) && @event.IsAction(PlayerInputCodes.DPad_Up) && !sceneTree.IsInputHandled())
				|| (Input.IsActionJustPressed(PlayerInputCodes.L2) && @event.IsAction(PlayerInputCodes.L2) && !sceneTree.IsInputHandled()))
			{
				ScrollItemList(ScrollDirection.Up);
				scrollTime = scrollInitialDelay;
			}

			if ((Input.IsActionJustPressed(PlayerInputCodes.DPad_Down) && @event.IsAction(PlayerInputCodes.DPad_Down) && !sceneTree.IsInputHandled())
				|| (Input.IsActionJustPressed(PlayerInputCodes.R2) && @event.IsAction(PlayerInputCodes.R2) && !sceneTree.IsInputHandled()))
			{
				ScrollItemList(ScrollDirection.Down);
				scrollTime = scrollInitialDelay;
			}

			if ((Input.IsActionJustPressed(PlayerInputCodes.DPad_Left) && @event.IsAction(PlayerInputCodes.DPad_Left) && !sceneTree.IsInputHandled())
				|| (Input.IsActionJustPressed(PlayerInputCodes.L1) && @event.IsAction(PlayerInputCodes.L1) && !sceneTree.IsInputHandled()))
			{
				ScrollItemList(ScrollDirection.Left);
				scrollTime = scrollInitialDelay;
			}

			if ((Input.IsActionJustPressed(PlayerInputCodes.DPad_Right) && @event.IsAction(PlayerInputCodes.DPad_Right) && !sceneTree.IsInputHandled())
				|| (Input.IsActionJustPressed(PlayerInputCodes.R1) && @event.IsAction(PlayerInputCodes.R1) && !sceneTree.IsInputHandled()))
			{
				ScrollItemList(ScrollDirection.Right);
				scrollTime = scrollInitialDelay;
			}
			#endregion
		}
	}

	public void SetActive(bool active)
	{ MenuActive = active; }

	public int GetQuickSlotIndex()
	{
		return selectedSlot;
	}

	private void OnBackpackUpgraded()
	{

	}

	private void OnCellUpdated(int index)
	{
		UpdateSlot(index);
	}

	private void LoadCellInitialItems()
	{
		//TODO: Fix the item references to the Inventory Screen
		foreach (int slot in Global.Instance.PlayerData.Inventory.ItemSlots.Keys)
		{
			//var cell = (InventoryCell)emptyCellNode.Instance();
			//uiItemList.AddChild(cell);
			UpdateSlot(slot);

			//Order matters when moving in and out of focus of items
			//cell.Connect("focus_entered", this, nameof(SetSelectedSlot), new Godot.Collections.Array() { cell.Index });
		}
	}

	private void ScrollItemList(ScrollDirection scrollDirection)
	{
		DeselectInventoryItem(selectedRowIndex, selectedColumnIndex);

		switch (scrollDirection)
		{
			case ScrollDirection.Up:
				inventoryRows[selectedRowIndex].Visible = false;
				inventoryHeaders[selectedRowIndex].TextureNormal = ImageStore.Get(nameof(ImageStore.UI.UI_EmptyCell));
				selectedRowIndex -= 1;
				if (selectedRowIndex < 0)
					selectedRowIndex = 0;
				inventoryRows[selectedRowIndex].Visible = true;
				inventoryHeaders[selectedRowIndex].TextureNormal = ImageStore.Get(nameof(ImageStore.UI.UI_SelectedCell));
				break;
			case ScrollDirection.Down:
				inventoryRows[selectedRowIndex].Visible = false;
				inventoryHeaders[selectedRowIndex].TextureNormal = ImageStore.Get(nameof(ImageStore.UI.UI_EmptyCell));
				selectedRowIndex += 1;
				if (selectedRowIndex >= Global.Instance.PlayerData.Inventory.BackpackLevel)
					selectedRowIndex = Global.Instance.PlayerData.Inventory.BackpackLevel;
				inventoryRows[selectedRowIndex].Visible = true;
				inventoryHeaders[selectedRowIndex].TextureNormal = ImageStore.Get(nameof(ImageStore.UI.UI_SelectedCell));
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

	private void DeselectInventoryItem(int rowIndex, int columnIndex)
	{
		inventoryRows[rowIndex].GetChild(0).GetChild<InventoryCell>(columnIndex).TextureNormal = ImageStore.Get(nameof(ImageStore.UI.UI_EmptyCell));
	}

	private void SelectInventoryItem(int rowIndex, int columnIndex)
	{
		inventoryRows[rowIndex].GetChild(0).GetChild<InventoryCell>(columnIndex).TextureNormal = ImageStore.Get(nameof(ImageStore.UI.UI_SelectedCell));
		EventManager.RaiseEvent(GlobalEventCodes.Player_InventoryQuickSlotUpdated, selectedSlot);
	}

	private void UpdateSlot(int slot)
	{
		var inventoryItem = Global.Instance.PlayerData.Inventory.ItemSlots[slot];
		InventoryCell inventoryCell = GetQuickSlotAtIndex(slot);
		inventoryCell.SetData(inventoryItem);

		if (slot == selectedSlot)
			EventManager.RaiseEvent(GlobalEventCodes.Player_InventoryQuickSlotUpdated, slot);
	}

	private InventoryCell GetQuickSlotAtIndex(int slot)
	{
		int rowIndex = slot / PlayerInventory.BACKPACK_LEVEL_SLOT_MULTIPIER;
		int columnIndex = slot % PlayerInventory.BACKPACK_LEVEL_SLOT_MULTIPIER;

		return inventoryRows[rowIndex].GetChild(0).GetChild<InventoryCell>(columnIndex);
	}
	private enum ScrollDirection
	{
		Up, Down, Left, Right
	}
}
