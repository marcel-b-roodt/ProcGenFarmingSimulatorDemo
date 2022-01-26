using Godot;

public class TransitionControl : ColorRect
{
	//TODO: Add new signals here for animations of different kinds
	//Or turn this control into something that takes in a Control which gets animated by these anims
	//This will let you pick the menu component you want animated at some point
	//Let it be a generic control, such as a container, which can then slide on or off screen
	[Signal] public delegate void FadeOutFinished();

	private AnimationPlayer animationPlayer;

	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public void Reset()
	{
		animationPlayer.Stop();
	}

	public void FadeIn()
	{
		Show();
		animationPlayer.Play("fade_in");
	}

	public void FadeOut()
	{
		Show();
		animationPlayer.Play("fade_out");
	}

	public void FadeOutLong()
	{
		Show();
		animationPlayer.Play("fade_out_long");
	}

	public void _on_AnimationPlayer_animation_finished(string anim_name)
	{
		if (anim_name == "fade_in")
			Hide();

		else
			EmitSignal(nameof(FadeOutFinished));
	}
}
