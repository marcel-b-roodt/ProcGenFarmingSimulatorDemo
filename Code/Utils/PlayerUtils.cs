using Godot;

public static class PlayerUtils
{
	public static Vector2 GetRandomPointNearPosition(Vector2 position, float distance = 20, bool onPerimeter = false)
	{
		//var playerPos = Global.Instance.PlayerData.Position;
		var offsetDirection = new Vector2((float)GD.Randf() * 2 - 1, (float)GD.Randf() * 2 - 1).Normalized();
		var distanceOffset = onPerimeter ? distance : (float)(GD.Randf() * distance);
		var offset = offsetDirection * distanceOffset;
		var newPosition = position + offset;

		return newPosition;
	}

	public static Vector2 GetRandomPointNearPlayer(float distance = 20)
	{
		var playerPos = Global.Instance.PlayerData.Position;
		return GetRandomPointNearPosition(playerPos, distance);
	}
}