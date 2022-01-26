using Godot;

public class OptionsTab : Panel
{
	public Button saveButton;
	public Button loadButton;
	public Button optionsButton;
	public Button saveAndQuitButton;

	public override void _Ready()
	{
		base._Ready();

		saveButton = GetNode<Button>("Margin/HB/VB_Buttons/SaveButton");
		loadButton = GetNode<Button>("Margin/HB/VB_Buttons/LoadButton");
		optionsButton = GetNode<Button>("Margin/HB/VB_Buttons/OptionsButton");
		saveAndQuitButton = GetNode<Button>("Margin/HB/VB_Buttons/SaveAndQuitButton");
		saveButton.GrabFocus();
	}

	public new void Show()
	{
		base.Show();
		saveButton.GrabFocus();
	}

	public new void Hide()
	{
		base.Hide();
	}

	private void _on_SaveButton_pressed()
	{
		// Replace with function body.
	}

	private void _on_LoadButton_pressed()
	{
		// Replace with function body.
	}

	private void _on_OptionsButton_pressed()
	{
		// Replace with function body.
	}

	private void _on_SaveAndQuitButton_pressed()
	{
		Global.Instance.ResetToGameTitle();
	}
}
