using Godot;

public class NameTag : Node2D
{
	private CircleShape2D playerDetectorShape;
	private Label text;

	private const float minimumRangeThreshold = 0.25f;
	private float detectorRange = 0;
	private bool detectedPlayer = false;
	private bool customName = false;

	public override void _Ready()
	{
		playerDetectorShape = ((CircleShape2D)GetNode<CollisionShape2D>("PlayerDetector/PlayerDetectorShape").Shape);
		text = GetNode<Label>("NameTagLabel");

		detectorRange = playerDetectorShape.Radius;
	}

	public void SetupName(string name)
	{
		customName = true;
		text.Text = name;
	}

	public override void _Process(float delta)
	{
		base._Process(delta);

		if (customName && detectedPlayer)
		{
			var playerPosition = Global.Instance.Player.Position;
			var distanceToPlayer = Position.DistanceTo(playerPosition);
			var distanceLerpFactor = minimumRangeThreshold + ((1 - (distanceToPlayer / detectorRange)) * 0.75f);
			var alphaFraction = Mathf.FloorToInt(255 * distanceLerpFactor);
			this.Modulate = Color.Color8(255, 255, 255, (byte)alphaFraction);
		}
	}

	private void _on_Area2D_PlayerDetector_area_entered(object area)
	{
		if (!Global.Instance.GameInProgress)
			return;

		//Debug.Print("Name Tag: Something Entered Area");

		if (Visible && area is IPlayerActor && !detectedPlayer)
		{
			detectedPlayer = true;
			Debug.Print("Name Tag: Player Entered Area");
		}
	}

	private void _on_Area2D_PlayerDetector_area_exited(object area)
	{
		if (!Global.Instance.GameInProgress)
			return;

		//Debug.Print("Name Tag: Something Exited Area");

		if (Visible && area is IPlayerActor && detectedPlayer)
		{
			detectedPlayer = false;
			Debug.Print("Name Tag: Player Exited Area");
		}
	}
}
