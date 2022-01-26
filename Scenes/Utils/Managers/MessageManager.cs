using Godot;

public static class MessageManager
{
	public static void WriteMessage(Vector2 position, string icon, ToastLength toastLength, string message)
	{
		ToastMessage toastMessageInstance = SetupToast(toastLength, out float displayTime, out float upGravity);
		toastMessageInstance.ConfigureMessage(position, icon, displayTime, upGravity, message);
	}

	public static void WriteMessage(Vector2 position, Texture texture, ToastLength toastLength, string message)
	{
		ToastMessage toastMessageInstance = SetupToast(toastLength, out float displayTime, out float upGravity);
		toastMessageInstance.ConfigureMessage(position, texture, displayTime, upGravity, message);
	}

	private static ToastMessage SetupToast(ToastLength toastLength, out float displayTime, out float upGravity)
	{
		displayTime = 0f;
		upGravity = 0f;
		switch (toastLength)
		{
			case ToastLength.Short:
				upGravity = 50f;
				displayTime = 1.5f;
				break;
			case ToastLength.Long:
				upGravity = 30f;
				displayTime = 4f;
				break;
		}

		return SceneStore.Instantiate<ToastMessage>();
	}
}

public enum ToastLength
{
	Short,
	Long
}
