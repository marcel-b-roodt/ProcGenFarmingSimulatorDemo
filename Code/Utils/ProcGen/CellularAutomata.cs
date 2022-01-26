
//using System.Collections.Generic;
//using System.Linq;

//public static class CellularAutomata
//{
//	static WorldTile[,] map;
//	static int mapSize = GameWorld.WorldCellMapDimension;

//	//private static void PopulateMap(WorldTile[,] map, int mapSize)
//	//{
//	//	System.Random rand = new System.Random();
//	//	for (int x = 0; x < mapSize; x++)
//	//	{
//	//		for (int y = 0; y < mapSize; y++)
//	//		{
//	//			map[x, y] = rand.Next(5); // Set a random type between 1 - 4
//	//		}
//	//	}
//	//}

//	//private static int GetTile(int x, int y)
//	//{
//	//	// Get the tile type or -1 if outside of bounds
//	//	return (x > map.GetUpperBound(0) || y > map.GetUpperBound(1) || x < 0 || y < 0) ? -1 : map[x, y];
//	//}

//	private static int GetTile(int x, int y)
//	{
//		// Get the tile type or -1 if outside of bounds
//		return (x > map.GetUpperBound(0) || y > map.GetUpperBound(1) || x < 0 || y < 0) ? -1 : map[x, y];
//	}

//	private static IEnumerable<int> GetNeighbours(int x, int y)
//	{
//		// Each tile has 8 neighbors
//		var neighbours = new int[8];

//		// Get each neighbor based on the tile position x, y
//		neighbours[0] = GetTile(x - 1, y);
//		neighbours[1] = GetTile(x, y - 1);
//		neighbours[2] = GetTile(x - 1, y - 1);
//		neighbours[3] = GetTile(x + 1, y);
//		neighbours[4] = GetTile(x, y + 1);
//		neighbours[5] = GetTile(x + 1, y + 1);
//		neighbours[6] = GetTile(x - 1, y + 1);
//		neighbours[7] = GetTile(x + 1, y - 1);

//		// Exclude -1 tiles because those are unexisting ones returned by GetTile
//		return neighbours.Where(f => f != -1);
//	}

//	//private static void ApplyCellularAutomationMin(WorldTile tile, WorldTile replacementTile, int minAlive)
//	//{
//	//	// Perform twice to cleanup residue of automation
//	//	for (int i = 0; i < 2; i++)
//	//	{
//	//		for (int x = 0; x < map.GetLength(0); x++)
//	//		{
//	//			for (int y = 0; y < map.GetLength(1); y++)
//	//			{
//	//				// Get only the neighbors where the type is the given tile.
//	//				var neighbours = GetNeighbours(x, y).Where(f => f == tile);
//	//				// If there are not enough alive, then replace the tile.
//	//				if (neighbours.Count() < minAlive)
//	//				{
//	//					map[x, y] = replacementTile;
//	//				}
//	//			}
//	//		}
//	//	}
//	//}

//	private static void ApplyCellularAutomationMin(WorldCellRegion region, WorldTile replacementTile, int minAlive)
//	{
//		// Perform twice to cleanup residue of automation
//		for (int i = 0; i < 2; i++)
//		{
//			for (int x = 0; x < map.GetLength(0); x++)
//			{
//				for (int y = 0; y < map.GetLength(1); y++)
//				{
//					// Get only the neighbors where the type is the given tile.
//					var neighbours = GetNeighbours(x, y).Where(f => f == tile);
//					// If there are not enough alive, then replace the tile.
//					if (neighbours.Count() < minAlive)
//					{
//						map[x, y] = replacementTile;
//					}
//				}
//			}
//		}
//	}
//}