using Godot;
using System.Collections.Generic;

public class PotatoCropEntity : StaticBody2D, ICrop
{
	public static Dictionary<ItemMetadataID, int> HarvestNotReadyDropMap = new Dictionary<ItemMetadataID, int>
	{
		{ ItemMetadataID.seeds_corn, 33 }
	};
	public static Dictionary<ItemMetadataID, int> HarvestReadyDropMap = new Dictionary<ItemMetadataID, int>
	{
		{ ItemMetadataID.consumable_crop_potato, 100 },
		{ ItemMetadataID.seeds_potato, 100 }
	};

	IEntityData IEntity.Data { get { return Data; } }
	public PotatoCropData Data { get; private set; }
	public ulong ID => Data.ID;
	public new string Name => Data.Name;
	public new Vector2 Position => Data.Position;
	public CellCoordinates CellCoordinates => Data.CellCoordinates;
	public TileCoordinates TileCoordinates => Data.TileCoordinates;
	public InventoryCropSeedType CropType => InventoryCropSeedType.Corn;
	public List<SoundPlayer> SoundPlayers { get; private set; } = new List<SoundPlayer>();
	public NameTagHolder NameTagHolder { get; set; }

	public int MaturityFromNewDays => Data.MaturityFromNewDays;
	public int MaturityFromPickedDays => Data.MaturityFromPickedDays;
	public int MaxPhase => Data.MaxPhase;
	public int MaxYield => Data.MaxYield;
	public bool Pickable => Data.Pickable;
	public bool DestroyOnPick => Data.DestroyOnPick;
	public GameDateTime TimePlanted { get { return Data.TimePlanted; } set { Data.TimePlanted = value; } }
	public GameDateTime TimePlantedOrHarvested { get { return Data.TimePlantedOrHarvested; } set { Data.TimePlantedOrHarvested = value; } }
	public GameDateTime TimeReadyForHarvest { get { return Data.TimeReadyForHarvest; } set { Data.TimeReadyForHarvest = value; } }
	public int CurrentPhase { get { return Data.CurrentPhase; } set { Data.CurrentPhase = value; AnimatedSprite.Frame = CurrentPhase; } }
	public bool ReadyToHarvest => (CurrentPhase == MaxPhase);

	#region GodotSetup
	private AnimatedSprite AnimatedSprite;
	private AnimatedSprite ReadyAnimatedSprite;

	public override void _Ready()
	{
		base._Ready();
		NodeHelpers.InitialiseEntity(this, Data);

		AnimatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		ReadyAnimatedSprite = GetNode<AnimatedSprite>("ReadyAnimatedSprite");
		ReadyAnimatedSprite.Visible = false;
	}

	public override void _Process(float delta)
	{
		base._Process(delta);

		if (Data.ReadyForNextPhase())
		{
			CurrentPhase = Mathf.Clamp(CurrentPhase + 1, 0, MaxPhase);
			//CurrentPhase++;
		}

		if (ReadyToHarvest && !ReadyAnimatedSprite.Playing)
		{
			Debug.Print($"ReadyToHarvest true: {Global.Instance.GameTime} >= {TimeReadyForHarvest}");
			ReadyAnimatedSprite.Visible = true;
			ReadyAnimatedSprite.Play();
		}
		else if (!ReadyToHarvest && ReadyAnimatedSprite.Playing)
		{
			Debug.Print($"ReadyToHarvest false: Global Game Time {Global.Instance.GameTime}. Time for Harvest {TimeReadyForHarvest}");
			ReadyAnimatedSprite.Visible = false;
			ReadyAnimatedSprite.Stop();
		}
	}
	#endregion

	public void Create()
	{
	}

	public void Initialise(EntityData entityData)
	{
		Data = (PotatoCropData)entityData;
		TimePlanted = Global.Instance.GameTime;
		TimePlantedOrHarvested = Global.Instance.GameTime;
		TimeReadyForHarvest = Global.Instance.GameTime.AddDays(MaturityFromNewDays);
		Debug.Print($"Crop Initialised: TimePlanted {TimePlanted} - TimeReadyForHarvest {TimeReadyForHarvest}");
	}

	public void ActivateEntity()
	{
		//EventManager.ListenEvent(nameof(Helpers.GlobalEventCodes.Global_DayUpdated), GD.FuncRef(this, nameof(UpdateCrop)));
	}

	public void DeactivateEntity()
	{
		//EventManager.IgnoreEvent(nameof(Helpers.GlobalEventCodes.Global_DayUpdated), GD.FuncRef(this, nameof(UpdateCrop)));
	}

	public void Destroy()
	{
		EntityManager.ClearTileEntityFromPosition(CellCoordinates, TileCoordinates);
	}

	public void BreakCrop()
	{
		if (ReadyToHarvest)
		{
			var currentYield = GameWorld.RNG.RandiRange(1, MaxYield);

			for (int i = 0; i < currentYield; i++)
			{
				ItemStore.SpawnAllItemsFromMap(GlobalPosition, HarvestReadyDropMap);
			}

			//TODO: Initialise Toast for Harvested Crops Message
			//ToastManager.TriggerMessage(GlobalPosition, $"+{currentYield}");
		}
		else
		{
			ItemStore.SpawnAllItemsFromMap(GlobalPosition, HarvestNotReadyDropMap);
		}

		Destroy();
	}

	public void PickCrop()
	{
		if (ReadyToHarvest)
		{
			var currentYield = GameWorld.RNG.RandiRange(1, MaxYield);

			for (int i = 0; i < currentYield; i++)
			{
				ItemStore.SpawnAllItemsFromMap(GlobalPosition, HarvestReadyDropMap);
			}

			CurrentPhase--;
			TimePlantedOrHarvested = Global.Instance.GameTime;
			TimeReadyForHarvest = Global.Instance.GameTime.AddDays(MaturityFromPickedDays);
		}

		if (DestroyOnPick)
		{
			Destroy();
		}
	}

	public void Harvest()
	{
		PickCrop();
	}
}
