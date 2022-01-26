
using Godot;

public static class SpriteExtensions
{
	public static void ScaleImageToTargetSize(this Sprite sprite, int targetSize)
	{
		var imageSize = sprite.Texture.GetSize();

		var targetSpriteSquareSize = targetSize;
		var xScale = targetSpriteSquareSize / imageSize.x / 2;
		var yScale = targetSpriteSquareSize / imageSize.y / 2;
		var targetScale = Mathf.Min(xScale, yScale);

		var scale = new Vector2(targetScale, targetScale);
		sprite.Scale = scale;
	}
}
