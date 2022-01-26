using Godot;

//TODO: Remove this class
public class DebugOutputMenu : Control
{
	private RichTextLabel debugOutput;

	private Vector2 lastPlayerPosition;

	public override void _Ready()
	{
		debugOutput = GetNode<RichTextLabel>("VB_Debug/RichTextLabel_DebugOutput");
	}

	public override void _Process(float delta)
	{
		var playerPosition = Global.Instance.PlayerData.Position;

		if (playerPosition != lastPlayerPosition)
		{
			debugOutput.Text = $@"Player Pos:
		X{playerPosition.x}
		Y{playerPosition.y}";

			lastPlayerPosition = playerPosition;
		}

		base._Process(delta);
	}
}
