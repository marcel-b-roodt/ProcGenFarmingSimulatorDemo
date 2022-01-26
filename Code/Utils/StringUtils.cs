public class StringUtils
{
	public static string JoinLineBreak(params string[] values)
	{
		return string.Join(System.Environment.NewLine, values);
	}

	public static string JoinComma(params string[] values)
	{
		return string.Join(",", values);
	}

	public static string[] Array(params string[] values)
	{
		return values;
	}
}
