using Godot;
using System;

public class DropDownMenu : PopupMenu
{
	private Container menuContainer;

	private Control returnFocusControl;

	public override void _Ready()
	{
		menuContainer = GetChild<Container>(1);

		//foreach (Button button in menuContainer.GetChildren())
		//{
		//	button.Connect("pressed", this, nameof(_on_MenuButton_pressed), new Godot.Collections.Array() { });
		//}
	}

	public void SetReturnFocusControl(Control targetControl)
	{
		returnFocusControl = targetControl;
	}

	public new void Show()
	{
		if (returnFocusControl == null)
			throw new InvalidOperationException("When showing a dropdown, a return focus item needs to be specified.");

		base.Show();
		var firstControl = (Control)menuContainer.GetChild(0);
		firstControl.GrabFocus();
	}

	public new void Hide()
	{
		base.Hide();

		returnFocusControl.GrabFocus();
		returnFocusControl = null;
	}

	//private void _on_MenuButton_pressed()
	//{
	//	Hide();
	//}
}
