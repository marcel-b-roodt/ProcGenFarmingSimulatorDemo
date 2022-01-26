using Godot;

public class NameTagHolder : Node2D
{
	private NameTag nameTag;
	private bool detectedPlayer;

	public override void _Ready()
	{
		nameTag = GetNode<NameTag>("NameTag");

		nameTag.SetProcess(false);
	}

	public void SetupName(string name, bool startProcessing)
	{
		nameTag.SetupName(name);
		nameTag.Visible = true;
		nameTag.SetProcess(startProcessing);
	}

	public void HideName()
	{
		nameTag.SetProcess(false);
		nameTag.Visible = false;
	}

	private void _on_PlayerDetector_area_entered(object area)
	{
		if (!Global.Instance.GameInProgress)
			return;

		if (area is IPlayerActor && !detectedPlayer)
		{
			detectedPlayer = true;
			nameTag.SetProcess(true);
			Debug.Print("Name Tag Holder: Player Entered Area");
		}
	}

	private void _on_PlayerDetector_area_exited(object area)
	{
		if (!Global.Instance.GameInProgress)
			return;

		if (area is IPlayerActor && detectedPlayer)
		{
			detectedPlayer = false;
			nameTag.SetProcess(false);
			Debug.Print("Name Tag Holder: Player Exited Area");
		}
	}
}



