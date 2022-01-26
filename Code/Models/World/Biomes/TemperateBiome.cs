using System.Collections.Generic;

public static class TemperateBiome
{
	public static BiomeType BiomeType { get { return BiomeType.Temperate; } }
	public static Dictionary<WorldTile, int> MapGen_TreeRadius
	{
		get
		{
			return new Dictionary<WorldTile, int>()
			{
				{ WorldTile.Grass, 4 },
				{ WorldTile.Plains, 10 },
			};
		}
	}

	public static Dictionary<WorldTile, int> MapGen_TallGrassRadius
	{
		get
		{
			return new Dictionary<WorldTile, int>()
			{
				{ WorldTile.Grass, 6 },
				{ WorldTile.Plains, 9 },
			};
		}
	}

	public static Dictionary<WorldTile, int> MapGen_RockRadius
	{
		get
		{
			return new Dictionary<WorldTile, int>()
			{
				{ WorldTile.Sand, 12 },
				{ WorldTile.Grass, 14 },
				{ WorldTile.Plains, 15 },
				{ WorldTile.RockTier1, 10 },
				{ WorldTile.RockTier2, 8 },
			};
		}
	}
}

