using System;

public static class WorldTileExtensions
{
	public static WorldTile GetPriorityTile(this WorldTile thisTile, WorldTile otherTile)
	{
		//if (thisTile == 0)
		//	thisTile = WorldTile.Water;

		TilePriority thisTilePriority = (TilePriority)Enum.Parse(typeof(TilePriority), thisTile.ToString(), true);
		TilePriority otherTilePriority = (TilePriority)Enum.Parse(typeof(TilePriority), otherTile.ToString(), true);

		if ((int)thisTilePriority > (int)otherTilePriority)
			return thisTile;
		else
		{
			thisTile = otherTile;
			return otherTile;
		}
	}

	public static bool IsMultiTile(this WorldTile thisTile)
	{
		switch (thisTile)
		{
			case WorldTile.Cliff:
			case WorldTile.River:
				//case WorldTile.Water:
				return false;
			default:
				return true;
		}
	}

	public static FeatureTile GetPriorityTile(this FeatureTile thisTile, FeatureTile otherTile)
	{
		//if (thisTile == 0)
		//	thisTile = FeatureTile.Unset;

		TilePriority thisTilePriority = (TilePriority)Enum.Parse(typeof(TilePriority), thisTile.ToString(), true);
		TilePriority otherTilePriority = (TilePriority)Enum.Parse(typeof(TilePriority), otherTile.ToString(), true);

		if ((int)thisTilePriority > (int)otherTilePriority)
			return thisTile;
		else
		{
			thisTile = otherTile;
			return otherTile;
		}
	}

	private enum TilePriority //This should handle depth of the cells
	{
		Unset = -1,

		//World Tiles
		Water,

		Sand,

		Plains,

		Grass,

		RockTier1,

		RockTier2,

		Cliff,

		Mountain,

		River,

		//Feature Tiles
		Soil,
	}
}