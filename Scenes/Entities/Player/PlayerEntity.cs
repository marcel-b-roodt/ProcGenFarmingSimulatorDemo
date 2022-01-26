using Godot;
using System.Collections.Generic;
using static Helpers;

public class PlayerEntity : KinematicBody2D, IPlayerActor
{
	IEntityData IEntity.Data { get { return Data; } }
	public PlayerData Data { get; private set; }
	public ulong ID => Data.ID;
	public new string Name => Data.Name;
	public new Vector2 Position => Data.Position;
	public List<SoundPlayer> SoundPlayers { get; private set; } = new List<SoundPlayer>();
	public NameTagHolder NameTagHolder { get; set; }

	public int Speed { get; private set; } = 200;
	public Vector2 LookDirection { get { return lookDirection; } }
	public Vector2 MoveDirection { get { return moveDirection; } }
	public Vector2 TargetPosition { get { return GlobalPosition + LookDirection * GameWorld.WorldCellTileSize; } }


	public static float ItemCollectionRange;
	public static float TileRenderRange;

	private Vector2 lookDirection = new Vector2(1, 0);
	private Vector2 moveDirection = new Vector2();

	private Camera2D playerCamera;
	private AnimationPlayer playerSpriteAnimator;
	private AnimatedSprite placeItemPreviewSprite;
	private Area2D interactArea;
	private Sprite activeItemSprite;
	private Area2D entityRangeArea;
	private Area2D tileRangeArea;
	private PlayerUI playerUI;

	private SceneTree sceneTree;

	//TODO: Store all the InGameMenus in a list and then cycle through them if they are open and you press R1 or L1
	//Would only be necessary if we need to cycle through multiple menus

	#region GodotEvents
	public override void _Ready()
	{
		NodeHelpers.InitialiseEntity(this, Data);

		EventManager.ListenEvent(GlobalEventCodes.Player_InventoryQuickSlotUpdated, GD.FuncRef(this, nameof(UpdateQuickSlotSelection)));
		sceneTree = GetTree();

		playerCamera = GetNode<Camera2D>("YSort/LookDirectionPivot/CameraOffset/PlayerCamera");
		playerCamera.MakeCurrent();

		playerSpriteAnimator = GetNode<AnimationPlayer>("YSort/PlayerSpriteAnimator");
		placeItemPreviewSprite = GetNode<AnimatedSprite>("YSort/PlaceItemPreviewSprite");
		interactArea = GetNode<Area2D>("YSort/LookDirectionPivot/InteractionRangeArea");
		activeItemSprite = GetNode<Sprite>("YSort/LookDirectionPivot/ActiveItemSprite");
		entityRangeArea = GetNode<Area2D>("EntityRangeArea");
		tileRangeArea = GetNode<Area2D>("TileRangeArea");
		playerUI = GetNode<PlayerUI>("PlayerUI");

		Data = Global.Instance.PlayerData;

		Global.Instance.Player = this;
		Global.Instance.PlayerUI = playerUI;
		ItemCollectionRange = ((CircleShape2D)(GetNode<CollisionShape2D>($"ItemCollectionRange/ItemCollectionRangeShape").Shape)).Radius;
		TileRenderRange = ((CircleShape2D)(GetNode<CollisionShape2D>($"TileRangeArea/TileRangeShape").Shape)).Radius;
	}

	public override void _EnterTree()
	{
		base._EnterTree();

		sceneTree = GetTree();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		base._UnhandledInput(@event);

		if (!Global.Instance.GamePaused && !Global.Instance.DebugMenuFlag)
		{
			if (Input.IsActionJustPressed(PlayerInputCodes.X) && @event.IsAction(PlayerInputCodes.X) && !sceneTree.IsInputHandled())
			{
				InteractWithObject();

				sceneTree.SetInputAsHandled();
			}

			if (Input.IsActionJustPressed(PlayerInputCodes.A) && @event.IsAction(PlayerInputCodes.A) && !sceneTree.IsInputHandled())
			{
				UseActiveItem();

				sceneTree.SetInputAsHandled();
			}
		}
	}

	public override void _Process(float delta)
	{
		base._Process(delta);

		if (placeItemPreviewSprite.Visible)
		{
			var targetPosition = GameWorld.GetPositionAsCellPosition(TargetPosition); //TODO: Fix this. Find which TileCoordinate the player stands in, and choose the neighbouring one
			placeItemPreviewSprite.GlobalPosition = targetPosition;
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		UpdateMovementInput();
		UpdateLookDirection();
		UpdatePlayerSpriteAnimation();

		var velocity = moveDirection.Normalized() * Speed;
		MoveAndSlide(velocity);
		Data.Position = GlobalPosition;

		base._PhysicsProcess(delta);
	}
	#endregion

	public void Initialise(EntityData entityData)
	{
		//Data = (PlayerData)entityData;
		//TODO: Sort this out when the player gets spawned. They need to get the data from the save game
	}

	public void Create()
	{ }

	public void ActivateEntity()
	{ }

	public void DeactivateEntity()
	{ }

	public void Destroy()
	{ }

	#region Movement
	private void UpdateMovementInput()
	{
		moveDirection.x = 0;
		moveDirection.y = 0;

		if (!Global.Instance.GamePaused && !Global.Instance.DebugMenuFlag)
		{
			if (Input.IsActionPressed(PlayerInputCodes.Right))
				moveDirection.x += 1;
			if (Input.IsActionPressed(PlayerInputCodes.Left))
				moveDirection.x -= 1;
			if (Input.IsActionPressed(PlayerInputCodes.Down))
				moveDirection.y += 1;
			if (Input.IsActionPressed(PlayerInputCodes.Up))
				moveDirection.y += -1;
		}
	}

	private void UpdateLookDirection()
	{
		if (moveDirection.Length() > 0)
		{
			lookDirection = moveDirection.Normalized();
		}
	}

	private void UpdatePlayerSpriteAnimation()
	{
		if (moveDirection.Length() > 0)
		{
			if (moveDirection.x > 0)
				playerSpriteAnimator.Play("walk_right");
			else if (moveDirection.x < 0)
				playerSpriteAnimator.Play("walk_left");
			else if (moveDirection.y > 0)
				playerSpriteAnimator.Play("walk_down");
			else if (moveDirection.y < 0)
				playerSpriteAnimator.Play("walk_up");
		}
		else
		{
			if (lookDirection.x > 0)
				playerSpriteAnimator.Play("idle_right");
			else if (lookDirection.x < 0)
				playerSpriteAnimator.Play("idle_left");
			else if (lookDirection.y > 0)
				playerSpriteAnimator.Play("idle_down");
			else if (lookDirection.y < 0)
				playerSpriteAnimator.Play("idle_up");
		}
	}
	#endregion

	#region Actions
	private void InteractWithObject()
	{
		var bodies = interactArea.GetOverlappingBodies();
		var closestBody = NodeUtils.GetClosestNode<IInteractable>(Position, bodies);

		if (closestBody == null)
			return;

		if (closestBody is IDialogueInteractable)
		{
			var dialogue = (IDialogueInteractable)closestBody;
			playerUI.ShowDialogue(dialogue.DialogueText);
		}
		else if (closestBody is IHarvestInteractable)
		{
			var harvestable = (IHarvestInteractable)closestBody;
			harvestable.Harvest();
		}
		else if (closestBody is ICrop)
		{
			var crop = (ICrop)closestBody;
			crop.PickCrop();
		}
		else
		{
			throw new System.InvalidCastException($"A case was not available for the interactable with name {closestBody.Name} and type {closestBody.GetType()}");
		}
	}

	private void UseActiveItem()
	{
		var inventoryIndex = playerUI.GetQuickSlotIndex();
		var activeItem = Global.Instance.PlayerData.Inventory.ItemSlots[inventoryIndex];
		if (activeItem.Item.Metadata is ToolItemMetadata)
		{
			UseTool(activeItem);
		}
		else if (activeItem.Item.Metadata is CropSeedItemMetadata)
		{
			UseCrop(activeItem);
		}
		else if (activeItem.Item.Metadata is ConsumableItemMetadata)
		{
			UseConsumable(activeItem);
		}
	}

	#region ActiveItems
	private void UseTool(PlayerInventorySlot activeItem)
	{
		var toolMetadata = (ToolItemMetadata)activeItem.Item.Metadata;
		var areas = interactArea.GetOverlappingAreas();
		var bodies = interactArea.GetOverlappingBodies();
		var areasAndBodies = new List<object>();

		foreach (var area in areas)
		{ areasAndBodies.Add(area); }
		foreach (var body in bodies)
		{ areasAndBodies.Add(body); }

		UseActiveTool(activeItem, toolMetadata, areasAndBodies);
	}

	//TODO: Move to a separate Tool Logic class / Static class for each Tool type
	private void UseActiveTool(PlayerInventorySlot activeItem, ToolItemMetadata toolMetadata, List<object> areasAndBodies)
	{
		switch (toolMetadata.ToolType)
		{
			case InventoryToolType.axe:
				var choppable = NodeUtils.GetClosestNode<IChoppable>(Position, areasAndBodies) as IChoppable;
				if (choppable == null)
					return;

				var damageToChoppable = toolMetadata.Damage;
				var damageToAxe = choppable.TakeChopDamage(damageToChoppable);
				activeItem.TakeDamage(damageToAxe);
				return;

			case InventoryToolType.hoe:
				foreach (var body in areasAndBodies)
				{
					if (body is IThreshable)
					{
						var threshableBody = (IThreshable)body;
						threshableBody.Thresh();
						activeItem.TakeDamage(1);
					}
					if (body is ICrop)
					{
						var cropBody = (ICrop)body;
						cropBody.BreakCrop();
					}
				}
				return;
			case InventoryToolType.melee:
				return;
			case InventoryToolType.pickaxe:
				var pickable = NodeUtils.GetClosestNode<IPickable>(Position, areasAndBodies) as IPickable;
				if (pickable == null)
					return;

				var damageToPickable = toolMetadata.Damage;
				var damageToPickaxe = pickable.TakePickDamage(damageToPickable);
				activeItem.TakeDamage(damageToPickaxe);
				return;
			case InventoryToolType.spade:
				//var targetItem = bodies.FirstOrDefault();
				//TODO: Get the cell that we are targeting. We need to make sure we can create soil there
				//Then we create a soil patch and save that in the game world.
				//If we hit a soil patch, cover it up. If that soil patch contains a crop, then delete the crop
				var success = CropManager.PlaceSoilAtTile(TargetPosition);
				if (success)
				{
					activeItem.TakeDamage(1); //TODO: Play animation. Do work. Play sound
				}
				return;
		}
	}

	private void UseCrop(PlayerInventorySlot activeItem)
	{
		var cropMetadata = (CropSeedItemMetadata)activeItem.Item.Metadata;

		var targetPosition = GameWorld.GetPositionAsCellPosition(TargetPosition); //TODO: Fix this. Find which TileCoordinate the player stands in, and choose the neighbouring one

		//TODO: Refactor into Crop Management
		UseActiveCrop(activeItem, cropMetadata, targetPosition);
	}

	//TODO: Move to a separate Crop Logic class / Static class for each Tool type
	private static void UseActiveCrop(PlayerInventorySlot activeItem, CropSeedItemMetadata cropMetadata, Vector2 targetPosition)
	{
		if (cropMetadata.CropType == InventoryCropSeedType.Tree_Pine)
		{
			var success = EntityManager.SpawnTileEntityInPosition<TreeSaplingEntity, TreeData>(targetPosition);
			if (success)
			{
				activeItem.ConsumeItem();
			}
		}
		else
		{
			var success = CropManager.PlaceCropAtTile(cropMetadata.CropType, targetPosition);
			if (success)
			{
				activeItem.ConsumeItem();
			}
		}
	}

	private void UseConsumable(PlayerInventorySlot activeItem)
	{
		var consumableMetadata = (ConsumableItemMetadata)activeItem.Item.Metadata;
		var areas = interactArea.GetOverlappingAreas();
		var bodies = interactArea.GetOverlappingBodies();
		var areasAndBodies = new List<object>();

		foreach (var area in areas)
		{ areasAndBodies.Add(area); }
		foreach (var body in bodies)
		{ areasAndBodies.Add(body); }

		Debug.Print($"Using consumable item {activeItem.Item.Metadata.Name}. Found {areasAndBodies.Count} entities.");
		ConsumableManager.UseConsumableItem(this, activeItem, areasAndBodies);
	}
	#endregion

	#endregion

	#region Helpers
	public List<IEntity> GetAllEntityNodesInRange()
	{
		var areas = entityRangeArea.GetOverlappingBodies();
		var results = new List<IEntity>();
		for (int i = 0; i < areas.Count; i++)
		{
			var area = areas[i];

			if (area is IEntity)
				results.Add(area as IEntity);
		}

		return results;
	}

	private void UpdateQuickSlotSelection(int selectedSlot)
	{
		UpdateActiveItem(selectedSlot);
	}

	private void UpdateActiveItem(int selectedSlot)
	{
		if (Global.Instance.PlayerData.Inventory.ItemSlots[selectedSlot].Item.Empty)
		{
			activeItemSprite.Texture = null;
			placeItemPreviewSprite.Visible = false;
		}
		else
		{
			var itemSlot = Global.Instance.PlayerData.Inventory.ItemSlots[selectedSlot];
			var itemMetadata = itemSlot.Item.Metadata;
			SetSpriteImage(itemMetadata.IconTexture);

			if (itemMetadata is ToolItemMetadata || itemMetadata is CropSeedItemMetadata || itemMetadata is BuildingKitMetadata)
			{
				placeItemPreviewSprite.Visible = true;
			}
			else
			{
				placeItemPreviewSprite.Visible = false;
			}
		}
	}

	private void SetSpriteImage(Texture texture)
	{
		var imageSize = texture.GetSize();

		var targetSpriteSquareSize = 48;
		var xScale = targetSpriteSquareSize / imageSize.x / 2;
		var yScale = targetSpriteSquareSize / imageSize.y / 2;
		var targetScale = Mathf.Min(xScale, yScale);

		var scale = new Vector2(targetScale, targetScale);

		activeItemSprite.Texture = texture;
		activeItemSprite.Scale = scale;
	}
	#endregion

	#region Signals
	private void _on_EntityRangeArea_area_entered(object area)
	{
		if (!Global.Instance.GameInProgress)
			return;

		if (area is IEntity)
		{
			Global.Instance.GameWorld.ActivateEntityNode((Node2D)area);
		}
		else
		{
			Debug.Print($"Player Area Entered: Found an area that is not an IEntity<EntityData> type");
		}
	}


	private void _on_EntityRangeArea_area_exited(object area)
	{
		if (!Global.Instance.GameInProgress)
			return;

		if (area is IEntity)
		{
			Global.Instance.GameWorld.DeactivateEntityNode((Node2D)area);
		}
		else
		{
			Debug.Print($"Player Area Exited: Found an area that is not an IEntity<EntityData> type");
		}
	}

	private void _on_EntityRangeArea_body_entered(object body)
	{
		if (!Global.Instance.GameInProgress)
			return;

		if (body is IEntity)
		{
			Global.Instance.GameWorld.ActivateEntityNode((Node2D)body);
		}
		else
		{
			Debug.Print($"Player Area Entered: Found an area that is not an IEntity<EntityData> type");
		}
	}

	private void _on_EntityRangeArea_body_exited(object body)
	{
		if (!Global.Instance.GameInProgress)
			return;

		if (body is IEntity)
		{
			Global.Instance.GameWorld.DeactivateEntityNode((Node2D)body);
		}
		else
		{
			Debug.Print($"Player Area Exited: Found an area that is not an IEntity<EntityData> type");
		}
	}

	private void _on_ItemCollectionRange_area_entered(object area)
	{
		if (!Global.Instance.GameInProgress)
			return;

		if (area is IPickupEntity)
		{
			var item = (IPickupEntity)area;
			//PickupManager.ActorPickupItem(this, item);
			if (item.PickUp())
			{
				(this as IEntity).PlaySound(SoundType.ItemPickup);
			}
		}
	}

	private void _on_ItemCollectionRange_area_exited(object area)
	{
		if (!Global.Instance.GameInProgress)
			return;

		if (area is IPickupEntity)
		{
			var item = (IPickupEntity)area;
			//PickupManager.ActorLeaveItem(this, item);
			item.PickedUp = false;
		}
	}
	#endregion
}


