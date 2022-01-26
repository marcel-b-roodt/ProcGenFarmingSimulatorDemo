using Godot;

public class PlayerCameraPivot : Position2D
{
	private PlayerEntity parent;
	private Sprite activeItemSprite;

	public override void _Ready()
	{
		parent = GetParent().GetParent<PlayerEntity>();
		activeItemSprite = GetNode<Sprite>("ActiveItemSprite");
	}

	public override void _PhysicsProcess(float delta)
	{
		base._PhysicsProcess(delta);
		UpdatePivotAngle();
	}

	private void UpdatePivotAngle()
	{
		Rotation = parent.LookDirection.Angle();
		activeItemSprite.GlobalRotation = 0;
	}
}
