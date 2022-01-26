using Godot;

public class MenuListContainer : Container
{
	[Signal] public delegate void FocusItemChanged(Control item);

	public override void _Ready()
	{
		base._Ready();

		foreach (Control child in GetChildren())
		{
			child.Connect("focus_entered", this, nameof(GotItemFocus), new Godot.Collections.Array { child });
		}
	}

	private void GotItemFocus(Control control)
	{
		EmitSignal(nameof(FocusItemChanged), control);
	}
}