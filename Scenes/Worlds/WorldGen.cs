public static class WorldGen
{
	public static float OceanHeightValue = 0.08f;
	public static float ShallowWaterValue = 0.13f;
	public static float BeachWithWaterValue = 0.14f;
	public static float BeachValue = 0.22f;
	public static float PlainsValue = 0.32f;
	public static float GrasslandsValue = 0.65f;
	public static float LowRocklandsValue = 0.78f;
	public static float HighRocklandsValue = 0.90f;
	public static float MountainsValue = 1f;

	public static WorldTile GetWorldTileFromHeight(float height)
	{
		if (height <= OceanHeightValue)
			return WorldTile.Water; //TODO: Ocean;
		else if (height <= ShallowWaterValue)
			return WorldTile.Water; //TODO: Shallow Water -- This needs to be a +1 tile-size because it has no collision block
		else if (height <= BeachWithWaterValue)
			return WorldTile.Water; //TODO: Beach with Water -- This needs to be +1
		else if (height <= BeachValue)
			return WorldTile.Sand; //+1 size
		else if (height <= PlainsValue)
			return WorldTile.Plains; //+1 size
		else if (height <= GrasslandsValue)
			return WorldTile.Grass; //+1 size
		else if (height <= LowRocklandsValue)
			return WorldTile.RockTier1; //+1 size
		else if (height <= HighRocklandsValue)
			return WorldTile.RockTier2; //+1 size
		else
			return WorldTile.Mountain;
	}
}