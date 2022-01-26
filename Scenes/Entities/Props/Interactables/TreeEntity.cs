using Godot;
using System.Collections.Generic;

public class TreeEntity : StaticBody2D, ITileEntity, IHarvestInteractable, IChoppable
{
	//TODO: Make some fruit tree types that drop particular fruit
	public static Dictionary<ItemMetadataID, int> HarvestDropMap = new Dictionary<ItemMetadataID, int>
	{
		{ ItemMetadataID.misc_wooden_stick, 25 }
	};
	public static Dictionary<ItemMetadataID, int> ChopDropMap = new Dictionary<ItemMetadataID, int>
	{
		{ ItemMetadataID.misc_wooden_log, 100 }
	};
	public static Dictionary<ItemMetadataID, int> ChopDownDropMap = new Dictionary<ItemMetadataID, int>
	{
		{ ItemMetadataID.seeds_sapling_pine, 100 }
	};

	IEntityData IEntity.Data { get { return Data; } }
	public TreeData Data { get; private set; }
	public ulong ID => Data.ID;
	public new string Name => Data.Name;
	public new Vector2 Position => Data.Position;
	public CellCoordinates CellCoordinates => Data.CellCoordinates;
	public TileCoordinates TileCoordinates => Data.TileCoordinates;
	public List<SoundPlayer> SoundPlayers { get; private set; } = new List<SoundPlayer>();
	public NameTagHolder NameTagHolder { get; set; }

	public int ChopHealth { get { return Data.ChopHealth; } private set { Data.ChopHealth = value; } }

	#region GodotSetup
	private Sprite Sprite { get; set; }
	private Tween Tween { get; set; }

	private float transparencyTweenTime = 0.2f;
	private float transparencyMagnitude = 0.5f;

	public override void _Ready()
	{
		base._Ready();
		NodeHelpers.InitialiseEntity(this, Data);

		Sprite = GetNode<Sprite>("Sprite");
		Tween = GetNode<Tween>("Tween");
	}

	private void _on_TransparencyArea_body_entered(object body)
	{
		if (body is PlayerEntity)
		{
			//Debug.Print($"Player entered body. Transparency set");

			Tween.InterpolateProperty(Sprite, "modulate", new Color(1, 1, 1, 1),
				new Color(1, 1, 1, transparencyMagnitude), transparencyTweenTime, Tween.TransitionType.Linear, Tween.EaseType.InOut);
			Tween.Start();
		}

	}

	private void _on_TransparencyArea_body_exited(object body)
	{
		if (body is PlayerEntity)
		{
			//Debug.Print($"Player exited body. Transparency reset");

			Tween.InterpolateProperty(Sprite, "modulate", new Color(1, 1, 1, transparencyMagnitude),
					new Color(1, 1, 1, 1), transparencyTweenTime, Tween.TransitionType.Linear, Tween.EaseType.InOut);
			Tween.Start();
		}
	}
	#endregion

	public void Initialise(EntityData entityData)
	{
		Data = (TreeData)entityData;
		ChopHealth = 5;
		//TODO: Add growth stages that will have the tree have varying levels of health at each stage
		//TODO: Have trees regenerate health as time goes by
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

	public void Harvest()
	{
		ItemStore.SpawnItemFromMap(GlobalPosition, HarvestDropMap);
	}

	public int TakeChopDamage(int chopDamage)
	{
		//Debug.Print($"Chop! Current Health is {ChopHealth}. Damage is {chopDamage}");
		var currentChopDamage = chopDamage;
		while (ChopHealth > 0 && currentChopDamage > 0)
		{
			ItemStore.SpawnItemFromMap(GlobalPosition, ChopDropMap);
			currentChopDamage -= 1;
			ChopHealth -= 1;
		}

		if (ChopHealth <= 0)
			ChopDown();

		//TODO: Play a chop sound

		return chopDamage - currentChopDamage;
	}

	public void ChopDown()
	{
		ItemStore.SpawnItemFromMap(GlobalPosition, ChopDownDropMap);
		//TODO: Make this better than just disappearing. Play a death animation
		//TODO: Play a chop down sound
		Destroy();
	}
}
