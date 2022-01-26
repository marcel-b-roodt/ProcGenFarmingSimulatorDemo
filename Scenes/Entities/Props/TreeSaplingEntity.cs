using Godot;
using System.Collections.Generic;

public class TreeSaplingEntity : StaticBody2D, ITileEntity, IChoppable
{
	public static Dictionary<ItemMetadataID, int> ChopDownDropMap = new Dictionary<ItemMetadataID, int>
	{
		{ ItemMetadataID.seeds_sapling_pine, 100 }
	};

	IEntityData IEntity.Data { get { return Data; } }
	public TreeData Data { get; private set; }
	public ulong ID => Data.ID;
	public new string Name => Data.Name;
	public NameTagHolder NameTagHolder { get; set; }
	public new Vector2 Position => Data.Position;
	public CellCoordinates CellCoordinates => Data.CellCoordinates;
	public TileCoordinates TileCoordinates => Data.TileCoordinates;
	public List<SoundPlayer> SoundPlayers { get; private set; } = new List<SoundPlayer>();

	public int ChopHealth { get { return Data.ChopHealth; } private set { Data.ChopHealth = value; } }

	#region GodotSetup
	public override void _Ready()
	{
		base._Ready();
		NodeHelpers.InitialiseEntity(this, Data);
	}
	#endregion

	public void Initialise(EntityData entityData)
	{
		Data = (TreeData)entityData;
		ChopHealth = 1;
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

	public int TakeChopDamage(int chopDamage)
	{
		ChopHealth = 0;
		ChopDown();
		return 1;
	}

	public void ChopDown()
	{
		ItemStore.SpawnItemFromMap(GlobalPosition, ChopDownDropMap);
		//TODO: Make this better than just disappearing. Play a death animation
		//TODO: Play a chop down sound
		Destroy();
	}

	//TODO: Make Sapling turn into Tree
	//We should use a function that turns an existing Sapling into a Tree, inheriting all the details from the Sapling
}
