using Godot;
using System.Collections.Generic;

public class TallGrassEntity : Area2D, ITileEntity, IThreshable
{
	public static Dictionary<ItemMetadataID, int> ThreshMiscDropMap = new Dictionary<ItemMetadataID, int>
	{
		{ ItemMetadataID.misc_grass, 80 },
	};

	public static Dictionary<ItemMetadataID, int> ThreshSeedDropMap = new Dictionary<ItemMetadataID, int>
	{
		{ ItemMetadataID.seeds_corn, 25 },
		{ ItemMetadataID.seeds_tomato, 25 },
	};

	IEntityData IEntity.Data { get { return Data; } }
	public TallGrassData Data { get; private set; }
	public ulong ID => Data.ID;
	public new string Name => Data.Name;
	public new Vector2 Position => Data.Position;
	public CellCoordinates CellCoordinates => Data.CellCoordinates;
	public TileCoordinates TileCoordinates => Data.TileCoordinates;
	public List<SoundPlayer> SoundPlayers { get; private set; } = new List<SoundPlayer>();
	public NameTagHolder NameTagHolder { get; set; }

	#region GodotSetup
	public override void _Ready()
	{
		base._Ready();
		NodeHelpers.InitialiseEntity(this, Data);
	}
	#endregion

	public void Initialise(EntityData entityData)
	{
		Data = (TallGrassData)entityData;
	}

	public void Create()
	{ }

	public void ActivateEntity()
	{ }

	public void DeactivateEntity()
	{ }

	public void Destroy()
	{
		EntityManager.ClearTileEntityFromPosition(CellCoordinates, TileCoordinates);
	}

	public void Thresh()
	{
		ItemStore.SpawnAllItemsFromMap(GlobalPosition, ThreshMiscDropMap);
		ItemStore.SpawnItemFromMap(GlobalPosition, ThreshSeedDropMap);
		//TODO: Play a thresh sound
		Destroy(); //TODO: Gracefully thresh and delete this object
	}
}
