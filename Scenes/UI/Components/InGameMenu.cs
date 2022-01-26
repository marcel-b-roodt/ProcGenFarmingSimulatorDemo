using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class InGameMenu : Panel
{
	public Dictionary<string, PopupPanel> PopupMenus = new Dictionary<string, PopupPanel>();
	public Dictionary<string, PopupPanel> ExcludedPopupMenus = new Dictionary<string, PopupPanel>();

	public Control ReturnFocusItem;

	private Dictionary<string, bool> PopupMenuVisibilities = new Dictionary<string, bool>();

	public override void _Ready()
	{
		base._Ready();
	}

	#region MenuConfiguration
	//Use this if you want to come back to the same control all the time.
	//Or if you want to remember what control you had focused before leaving the menu
	public void SetReturnFocusItem(Control control)
	{
		ReturnFocusItem = control;
	}
	#endregion

	public void ManagePersistentPopups(params PopupPanel[] popups)
	{
		foreach (var popup in popups)
		{
			PopupMenus.Add(popup.Name, popup);
			PopupMenuVisibilities.Add(popup.Name, popup.Visible);
		}

		var fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
		var properties = GetType().GetProperties();

		foreach (var field in fields)
		{
			if (field.FieldType == typeof(PopupPanel))
			{
				PopupPanel popup = field.GetValue(this) as PopupPanel;
				if (popup != null)
				{
					if (!PopupMenus.Keys.Contains(popup.Name))
					{
						ExcludedPopupMenus.Add(popup.Name, popup);
					}
				}
			}
		}
	}

	public new void Show()
	{
		foreach (var menu in PopupMenus.Values)
		{
			if (PopupMenuVisibilities[menu.Name] == true)
				menu.Show();
		}

		foreach (var menu in ExcludedPopupMenus.Values)
		{
			menu.Hide();
		}

		base.Show();
		GrabReturnFocusItem();
	}

	public new void Hide()
	{
		foreach (var menu in PopupMenus.Values)
		{
			PopupMenuVisibilities[menu.Name] = menu.Visible;
			menu.Hide();
		}

		foreach (var menu in ExcludedPopupMenus.Values)
		{
			menu.Hide();
		}

		base.Hide();
	}

	public void GrabReturnFocusItem()
	{
		ReturnFocusItem?.GrabFocus();
	}
}
