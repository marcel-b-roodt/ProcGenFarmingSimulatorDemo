using Godot;

public class ToastMessage : Node2D
{
	public Sprite Sprite
	{
		get { if (sprite == null) { sprite = GetNode<Sprite>("Sprite_Image"); } return sprite; }
	}

	private Sprite sprite;

	public Label Text
	{
		get { if (text == null) { text = GetNode<Label>("Label_Text"); } return text; }
	}

	private Label text;

	private float displayTime = 0;
	private float upGravity = 0;
	private float liveTime = 0;
	private bool initialised = false;

	public override void _Ready()
	{

	}

	public override void _Process(float delta)
	{
		base._Process(delta);

		if (initialised)
		{
			Position += (Vector2.Up * upGravity * delta);

			liveTime -= delta;

			var alphaFraction = Mathf.FloorToInt(255 * (liveTime / displayTime));
			this.Modulate = Color.Color8(255, 255, 255, (byte)alphaFraction);

			if (liveTime <= 0)
				Destroy();
		}
	}

	public void ConfigureMessage(Vector2 position, string icon, float displayTime, float upGravity, string message)
	{
		var texture = ImageStore.Get(icon);
		InitialiseToastMessage(position, texture, displayTime, upGravity, message);
	}

	public void ConfigureMessage(Vector2 position, Texture texture, float displayTime, float upGravity, string message)
	{
		InitialiseToastMessage(position, texture, displayTime, upGravity, message);
	}

	private void InitialiseToastMessage(Vector2 position, Texture texture, float displayTime, float upGravity, string message)
	{
		GlobalPosition = position;
		this.displayTime = displayTime;
		this.upGravity = upGravity;
		this.liveTime = displayTime;
		Text.Text = message;
		SetSpriteImage(texture);

		Show();
		GameWorld.Messages.AddChild(this);
		initialised = true;
	}

	private void Destroy()
	{
		QueueFree();
		//Debug.Print("Destroyed Toast Message");
	}

	private void SetSpriteImage(Texture texture)
	{
		this.Sprite.Texture = texture;
		this.Sprite.ScaleImageToTargetSize(36);
	}
}
