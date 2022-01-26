using Godot;

public static class Noise
{
	public static OpenSimplexNoise TerrainNoise
	{
		get
		{
			if (terrainNoise == null)
			{
				terrainNoise = new OpenSimplexNoise();
				terrainNoise.Lacunarity = GameWorld.MapGen_Terrain_Lacunarity;
				terrainNoise.Octaves = GameWorld.MapGen_Terrain_Octaves;
				terrainNoise.Persistence = GameWorld.MapGen_Terrain_Persistence;
				terrainNoise.Period = GameWorld.MapGen_Terrain_Scale;
			}

			terrainNoise.Seed = (int)(GameWorld.WorldSeed - uint.MaxValue);
			return terrainNoise;
		}
	}
	private static OpenSimplexNoise terrainNoise;

	public static OpenSimplexNoise MoistureNoise
	{
		get
		{
			if (moistureNoise == null)
			{
				moistureNoise = new OpenSimplexNoise();
				moistureNoise.Lacunarity = GameWorld.MapGen_Moisture_Lacunarity;
				moistureNoise.Octaves = GameWorld.MapGen_Moisture_Octaves;
				moistureNoise.Persistence = GameWorld.MapGen_Moisture_Persistence;
				moistureNoise.Period = GameWorld.MapGen_Moisture_Scale;
			}

			moistureNoise.Seed = (int)(GameWorld.WorldSeed - uint.MaxValue);
			return moistureNoise;
		}
	}
	private static OpenSimplexNoise moistureNoise;

	private static OpenSimplexNoise genericNoise = new OpenSimplexNoise();

	public static void ResetNoise()
	{
		terrainNoise = new OpenSimplexNoise();
		terrainNoise.Seed = (int)(GameWorld.WorldSeed - uint.MaxValue);
		terrainNoise.Lacunarity = GameWorld.MapGen_Terrain_Lacunarity;
		terrainNoise.Octaves = GameWorld.MapGen_Terrain_Octaves;
		terrainNoise.Persistence = GameWorld.MapGen_Terrain_Persistence;
		terrainNoise.Period = GameWorld.MapGen_Terrain_Scale;
		//Debug.Print($"Noise Seed: {GameWorld.WorldSeed}");
	}

	public enum NormalizeMode { Local, Global };

	//TODO: Create a second set of Noise Map that we can use to overlap Moisture. Then we look up terrain vs moisture and see which biome tile we should be fetching
	//There's that old helpful blog about getting moisture and terrain height in order to calculate which kind of biome we are in

	public static float[,] GenerateNoiseMap(int mapDimension, Vector2 offset, NormalizeMode normalizeMode)
	{
		float[,] noiseMap = new float[mapDimension, mapDimension];

		float maxPossibleHeight = 1;
		float maxLocalNoiseHeight = float.MinValue;
		float minLocalNoiseHeight = float.MaxValue;

		for (int y = 0; y < mapDimension; y++)
		{
			for (int x = 0; x < mapDimension; x++)
			{
				float noiseHeight = TerrainNoise.GetNoise2dv(new Vector2(x + offset.x, y + offset.y)) * 2 - 1;

				if (noiseHeight > maxLocalNoiseHeight)
				{
					maxLocalNoiseHeight = noiseHeight;
				}
				else if (noiseHeight < minLocalNoiseHeight)
				{
					minLocalNoiseHeight = noiseHeight;
				}
				noiseMap[x, y] = noiseHeight;
			}
		}

		for (int y = 0; y < mapDimension; y++)
		{
			for (int x = 0; x < mapDimension; x++)
			{
				if (normalizeMode == NormalizeMode.Local)
				{
					noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
				}
				else
				{
					float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / 0.9f);
					noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
				}
			}
		}

		return noiseMap;
	}

	public static float SampleNoise(Vector2 position, float lacunarity, int octaves, float persistence, int scale)
	{
		genericNoise.Seed = (int)(GameWorld.WorldSeed - uint.MaxValue);
		genericNoise.Lacunarity = lacunarity;
		genericNoise.Octaves = octaves;
		genericNoise.Persistence = persistence;
		genericNoise.Period = scale;

		return genericNoise.GetNoise2dv(position) * 2 - 1; //Return between 0 and 1
	}
}
