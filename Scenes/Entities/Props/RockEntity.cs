using Godot;
using System.Collections.Generic;

public class RockEntity : StaticBody2D, ITileEntity, IPickable
{
	public static Dictionary<ItemMetadataID, int> PickDropMap = new Dictionary<ItemMetadataID, int>
	{
		{ ItemMetadataID.misc_stone, 40 }
	};
	//TODO: Add other materials here like a small chance of minerals/iron ore
	public static Dictionary<ItemMetadataID, int> PickBreakDropMap = new Dictionary<ItemMetadataID, int>
	{
		{ ItemMetadataID.misc_stone, 67 }
	};

	IEntityData IEntity.Data { get { return Data; } }
	public RockData Data { get; private set; }
	public ulong ID => Data.ID;
	public new string Name => Data.Name;
	public new Vector2 Position => Data.Position;
	public CellCoordinates CellCoordinates => Data.CellCoordinates;
	public TileCoordinates TileCoordinates => Data.TileCoordinates;
	//TODO: Have rocks regenerate health as time goes by
	public List<SoundPlayer> SoundPlayers { get; private set; } = new List<SoundPlayer>();
	public NameTagHolder NameTagHolder { get; set; }

	public int PickHealth { get { return Data.PickHealth; } private set { Data.PickHealth = value; } }

	#region GodotSetup
	public override void _Ready()
	{
		base._Ready();
		NodeHelpers.InitialiseEntity(this, Data);
	}
	#endregion

	public void Initialise(EntityData entityData)
	{
		Data = (RockData)entityData;
		PickHealth = 8;
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

	public int TakePickDamage(int pickDamage)
	{
		//Debug.Print($"Chop! Current Health is {ChopHealth}. Damage is {chopDamage}");
		var currentPickDamage = pickDamage;
		while (PickHealth > 0 && currentPickDamage > 0)
		{
			ItemStore.SpawnItemFromMap(GlobalPosition, PickDropMap);
			currentPickDamage -= 1;
			PickHealth -= 1;
		}

		if (PickHealth <= 0)
			Break();

		return pickDamage - currentPickDamage;
	}

	public void Break()
	{
		ItemStore.SpawnItemFromMap(GlobalPosition, PickBreakDropMap);
		//TODO: Make this better than just disappearing. Play a death animation
		//TODO: Play a break sound
		Destroy();
	}
}
