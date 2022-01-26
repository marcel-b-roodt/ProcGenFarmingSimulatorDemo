using Godot;
using static Helpers;

public class DebugMenu : Control
{
	//private bool debugPopupOpen = false;
	private bool debugMode = false;
	private bool debugMenuOpen = false;

	private PlayerUI playerUI;
	private PanelContainer addItemPanelContainer;
	private SpinBox itemIDSpinBox;
	private TextureRect itemDetailIcon;
	private Label itemDetailName;
	private RichTextLabel debugOutput;

	private Vector2 lastPlayerPosition;

	private float scrollTime;
	private float scrollInitialDelay = 0.2f;
	private float scrollDelay = 0.1f;


	public override void _Ready()
	{
		base._Ready();

		EventManager.ListenEvent(GlobalEventCodes.Debug_Toggle, GD.FuncRef(this, nameof(OnDebugToggle)));

		playerUI = GetParent<PlayerUI>();
		addItemPanelContainer = GetNode<PanelContainer>($"AddItemPanelContainer");
		itemIDSpinBox = GetNode<SpinBox>($"AddItemPanelContainer/VB/VB_ItemID/HB/ItemIDSpinBox");
		itemDetailIcon = GetNode<TextureRect>($"AddItemPanelContainer/VB/ItemDetailIcon");
		itemDetailName = GetNode<Label>($"AddItemPanelContainer/VB/VB_ItemName/ItemDetailName");

		debugOutput = GetNode<RichTextLabel>("VB/DebugOutputText");

		addItemPanelContainer.Hide();
	}

	public override void _Process(float delta)
	{
		var playerPosition = Global.Instance.PlayerData.Position;
		var activeCell = Global.Instance.GameWorld.ActiveWorldCell;
		var timeOfDay = Global.Instance.GameTime;

		var tileX = playerPosition.x / GameWorld.WorldCellTileSize;
		var tileY = playerPosition.y / GameWorld.WorldCellTileSize;
		int localTileX = (int)(playerPosition.x / GameWorld.WorldCellTileSize) % GameWorld.WorldCellMapDimension;
		int localTileY = (int)(playerPosition.y / GameWorld.WorldCellTileSize) % GameWorld.WorldCellMapDimension;

		if (localTileX >= GameWorld.WorldCellMapDimension)
			localTileX -= GameWorld.WorldCellMapDimension;
		if (localTileX < 0)
			localTileX += GameWorld.WorldCellMapDimension;

		if (localTileY >= GameWorld.WorldCellMapDimension)
			localTileY -= GameWorld.WorldCellMapDimension;
		if (localTileY < 0)
			localTileY += GameWorld.WorldCellMapDimension;

		//TODO: Fix rendering issue with tiles that are meant to be drawn at the correct level. Sand is drawn over the Grass, etc. This is probably due to not using correct AutoTiles

		//var localTileX = Mathf.FloorToInt((int)Mathf.Clamp(tileX - activeCell.Data.XOffset, 0, GameWorld.WorldCellMapDimension));
		//var localTileY = Mathf.FloorToInt((int)Mathf.Clamp(tileY - Mathf.Sign(activeCell.Data.YOffset) * activeCell.Data.YOffset, 0, GameWorld.WorldCellMapDimension));
		//var something = $"TileType{ activeCell.Data.MapTiles[localTileX, localTileY].WorldTile}";

		if (debugMode)
		{
			debugOutput.Text = $@"Player Pos:
		X{playerPosition.x}
		Y{playerPosition.y}
		TileX{tileX}
		TileY{tileY}
Current Cell:{activeCell.Data.CellCoordinates}
		XOffset{activeCell.Data.XOffset}
		YOffset{activeCell.Data.YOffset}
		LocalTileX{localTileX}
		LocalTileY{localTileY}
		TileType{activeCell.Data.MapTiles[localTileX, localTileY].WorldTile}
Time:{timeOfDay}";

			lastPlayerPosition = playerPosition;
		}

		if (scrollTime <= 0)
		{
			if (addItemPanelContainer.Visible)
			{
				if (Input.IsKeyPressed((int)KeyList.A))
				{
					itemIDSpinBox.Value--;
					var id = (ItemMetadataID)itemIDSpinBox.Value;
					SetDebugItemInfo(id);
					scrollTime = scrollDelay;
				}

				else if (Input.IsKeyPressed((int)KeyList.D))
				{
					itemIDSpinBox.Value++;
					var id = (ItemMetadataID)itemIDSpinBox.Value;
					SetDebugItemInfo(id);
					scrollTime = scrollDelay;
				}
			}
		}
		else
		{
			scrollTime -= delta;
		}

		if (!Input.IsKeyPressed((int)KeyList.A) && !Input.IsKeyPressed((int)KeyList.D))
		{
			scrollTime = 0;
		}

		base._Process(delta);
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if ((!Global.Instance.GamePaused || this.Visible) && Global.Instance.DebugFlag)
		{
			var keyEvent = (@event as InputEventKey);
			if (keyEvent != null && keyEvent.IsPressed() && !(keyEvent.Echo))
			{
				if (keyEvent.Scancode == (int)KeyList.F2)
				{
					if (addItemPanelContainer.Visible)
					{
						HideMenus();
					}
					else
					{
						HideMenus();
						EventManager.RaiseEvent(Helpers.GlobalEventCodes.Debug_ToggleMenu, true);
						addItemPanelContainer.Show();
						debugMenuOpen = true;
					}
				}

				if (addItemPanelContainer.Visible)
				{
					if (keyEvent.Scancode == (int)KeyList.A)
					{
						itemIDSpinBox.Value--;
						var id = (ItemMetadataID)itemIDSpinBox.Value;
						SetDebugItemInfo(id);
						scrollTime = scrollInitialDelay;
					}

					if (keyEvent.Scancode == (int)KeyList.D)
					{
						itemIDSpinBox.Value++;
						var id = (ItemMetadataID)itemIDSpinBox.Value;
						SetDebugItemInfo(id);
						scrollTime = scrollInitialDelay;
					}

					if (keyEvent.Scancode == (int)KeyList.F)
					{
						AddCurrentItemToInventory();
					}

					if (keyEvent.Scancode == (int)KeyList.G)
					{
						SpawnCurrentItemAsPickup();
					}
				}
			}
		}
	}

	private void OnDebugToggle(bool isDebug)
	{
		debugMode = isDebug;

		if (isDebug && !playerUI.MenuOpen)
			Show();
		else
		{
			HideMenus();
			Hide();
		}
	}

	private void HideMenus()
	{
		addItemPanelContainer.Hide();
		debugMenuOpen = false;
		EventManager.RaiseEvent(Helpers.GlobalEventCodes.Debug_ToggleMenu, false);
	}

	private void SetDebugItemInfo(ItemMetadataID id)
	{
		var itemMetadata = ItemLookup.Get(id);
		itemDetailIcon.Texture = itemMetadata.IconTexture;
		itemDetailName.Text = itemMetadata.Name;
	}

	private void AddCurrentItemToInventory()
	{
		var metadataID = (ItemMetadataID)itemIDSpinBox.Value;
		if (metadataID == ItemMetadataID.meta_empty)
			return;

		InventoryItem item = new InventoryItem(metadataID);
		Global.Instance.PlayerData.Inventory.AddItem(item, 1, out _);
	}

	private void SpawnCurrentItemAsPickup()
	{
		var metadataID = (ItemMetadataID)itemIDSpinBox.Value;
		if (metadataID == ItemMetadataID.meta_empty)
			return;

		InventoryItem item = new InventoryItem(metadataID);
		Global.Instance.GameWorld.SpawnPickup(PlayerUtils.GetRandomPointNearPlayer(100), item, 1);
	}
}
