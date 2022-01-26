using Godot;

public class SoundPlayer : Node2D
{
	public AudioStreamPlayer2D AudioStreamPlayer
	{
		get { if (audioStreamPlayer == null) { audioStreamPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer"); } return audioStreamPlayer; }
	}

	private AudioStreamPlayer2D audioStreamPlayer;

	public void PlaySound(SoundType soundType)
	{
		var sound = SoundStore.Get(soundType);
		AudioStreamPlayer.Stream = sound; //This may break because the Audio Stream Player might not be part of the SceneTree

		var pitch = GD.RandRange(0.9f, 1.1f);
		AudioStreamPlayer.PitchScale = (float)pitch;
		AudioStreamPlayer.Play();
	}

	private void Destroy()
	{
		QueueFree();
	}
}
