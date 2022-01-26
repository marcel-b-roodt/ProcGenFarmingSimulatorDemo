using Godot;
using System.Collections.Generic;
using System.Linq;

public static class PoissonDiscNoise
{
	public static Dictionary<WorldTile, IEnumerable<TileCoordinates>> GeneratePointsForWorldTiles(Dictionary<WorldTile, int> treeRadiusDictionary, Vector2 sampleRegionSize, int numSamplesBeforeRejection = 30)
	{
		var dictionary = new Dictionary<WorldTile, IEnumerable<TileCoordinates>>();
		foreach (var kvp in treeRadiusDictionary)
		{
			dictionary.Add(kvp.Key, GeneratePoints(kvp.Value, sampleRegionSize, numSamplesBeforeRejection)
				.Select(item => new TileCoordinates((int)item.x, (int)item.y)));
		}

		return dictionary;
	}

	public static List<Vector2> GeneratePoints(float radius, Vector2 sampleRegionSize, int numSamplesBeforeRejection = 30)
	{
		float cellSize = radius / Mathf.Sqrt(2);

		int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x / cellSize), Mathf.CeilToInt(sampleRegionSize.y / cellSize)];
		List<Vector2> points = new List<Vector2>();
		List<Vector2> spawnPoints = new List<Vector2>();

		spawnPoints.Add(sampleRegionSize / 2);
		while (spawnPoints.Count > 0)
		{
			int spawnIndex = GameWorld.RNG.RandiRange(0, spawnPoints.Count - 1);
			Vector2 spawnCentre = spawnPoints[spawnIndex];
			bool candidateAccepted = false;

			for (int i = 0; i < numSamplesBeforeRejection; i++)
			{
				float angle = GameWorld.RNG.Randf() * Mathf.Pi * 2;
				Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
				Vector2 candidate = spawnCentre + dir * GameWorld.RNG.RandfRange(radius, 2 * radius);
				if (IsValid(candidate, sampleRegionSize, cellSize, radius, points, grid))
				{
					points.Add(candidate);
					spawnPoints.Add(candidate);
					grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
					candidateAccepted = true;
					break;
				}
			}

			if (!candidateAccepted)
			{
				spawnPoints.RemoveAt(spawnIndex);
			}
		}

		return points;
	}

	static bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid)
	{
		if (candidate.x >= 0 && candidate.x < sampleRegionSize.x && candidate.y >= 0 && candidate.y < sampleRegionSize.y)
		{
			int cellX = (int)(candidate.x / cellSize);
			int cellY = (int)(candidate.y / cellSize);
			int searchStartX = Mathf.Max(0, cellX - 2);
			int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
			int searchStartY = Mathf.Max(0, cellY - 2);
			int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

			for (int x = searchStartX; x <= searchEndX; x++)
			{
				for (int y = searchStartY; y <= searchEndY; y++)
				{
					int pointIndex = grid[x, y] - 1;
					if (pointIndex != -1)
					{
						float sqrDst = (candidate - points[pointIndex]).LengthSquared();
						if (sqrDst < radius * radius)
						{
							return false;
						}
					}
				}
			}
			return true;
		}
		return false;
	}
}