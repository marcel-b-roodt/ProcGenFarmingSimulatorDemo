using Godot;

public static class Themes
{
	public static Theme GameTheme = ResourceLoader.Load<Theme>("res://Resources/Themes/game_theme.tres");

	public static class Styleboxes
	{

		public static class Flat
		{
			public static StyleBox Highlighted = ResourceLoader.Load<StyleBox>("res://Resources/Themes/Styles/panel_stylebox_flat_highlighted.tres");
			public static StyleBox Unhighlighted = ResourceLoader.Load<StyleBox>("res://Resources/Themes/Styles/panel_stylebox_flat_unhighlighted.tres");
		}
	}
}
