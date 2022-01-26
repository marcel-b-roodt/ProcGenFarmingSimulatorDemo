using Godot;

public static class Debug 
{
	public static void Print(params string[] text)
	{
#if DEBUG
		var fullText = string.Join(System.Environment.NewLine, text);
		GD.Print(fullText);
#endif
	}
}
