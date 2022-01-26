using Godot;

public static class IEntityExtensions
{
	public static void PlaySound(this IEntity entity, SoundType soundType)
	{
		SoundPlayer currentPlayer = null;

		foreach (var soundPlayer in entity.SoundPlayers)
		{
			if (!soundPlayer.AudioStreamPlayer.Playing)
				currentPlayer = soundPlayer;
		}

		if (currentPlayer == null)
		{
			currentPlayer = SceneStore.Instantiate<SoundPlayer>(); //Make sure we add the SoundPlayer to the Entity's Scene Tree
			var soundPlayersNode = (entity as Node2D).GetNodeOrNull("SoundPlayers");
			if (soundPlayersNode == null)
			{
				var node = new Node2D();
				node.Name = "SoundPlayers";
				(entity as Node2D).AddChild(node);
				soundPlayersNode = node;
			}

			soundPlayersNode.AddChild(currentPlayer);
			entity.SoundPlayers.Add(currentPlayer);
		}

		currentPlayer.PlaySound(soundType);
	}
}

