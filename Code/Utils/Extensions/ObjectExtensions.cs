
using System.Reflection;

public static class ObjectExtensions
{
	public static void CopyProperties<T>(this T targetObject, T sourceObject)
	{
		foreach (PropertyInfo property in typeof(T).GetProperties())
		{
			property.SetValue(targetObject, property.GetValue(sourceObject, null), null);
		}
	}
}
