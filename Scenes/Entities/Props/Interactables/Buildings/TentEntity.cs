using Godot;
using System.Collections.Generic;

public class TentEntity : StaticBody2D, ITileEntity, IBuildingInteractable
{
	IEntityData IEntity.Data { get { return Data; } }
	public TentData Data { get; private set; }
	public ulong ID => Data.ID;
	public new string Name => Data.Name;
	public new Vector2 Position => Data.Position;
	public CellCoordinates CellCoordinates => Data.CellCoordinates;
	public TileCoordinates TileCoordinates => Data.TileCoordinates;
	public List<SoundPlayer> SoundPlayers { get; private set; } = new List<SoundPlayer>();
	public NameTagHolder NameTagHolder { get; set; }

	#region GodotSetup
	private Sprite Sprite { get; set; }
	private Tween Tween { get; set; }
	private CollisionShape2D CollisionShape { get; set; }
	private float transparencyTweenTime = 0.2f;
	private float transparencyMagnitude = 0.5f;

	public override void _Ready()
	{
		base._Ready();
		NodeHelpers.InitialiseEntity(this, Data);

		Sprite = GetNode<Sprite>("Sprite");
		Tween = GetNode<Tween>("Tween");
		CollisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		//CollisionShape.Shape.GEt
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
		Data = (TentData)entityData;
	}

	public void Create()
	{ }

	public void ActivateEntity()
	{ }

	public void DeactivateEntity()
	{ }

	public void Destroy()
	{ }
}
