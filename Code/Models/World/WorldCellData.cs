using Godot;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class WorldCellData
{
	public CellCoordinates CellCoordinates;
	public int XOffset;
	public int YOffset;

	public bool WorldTilesInitialised;
	public bool FeatureTilesInitialised;
	public bool EntityTilesInitialised;

	public float[,] HeightMap = new float[GameWorld.WorldCellMapDimension, GameWorld.WorldCellMapDimension];
	public WorldCellTileData[,] MapTiles;
	public Dictionary<ulong, EntityData> Entities = new Dictionary<ulong, EntityData>(); //TODO: When restoring entities, we need to update their IDs
	public ConcurrentDictionary<int, WorldCellRegion> Regions = new ConcurrentDictionary<int, WorldCellRegion>();

	public bool MapUpdated { get { return MapTilesNeedUpdate(); } }

	private bool MapTilesNeedUpdate()
	{
		foreach (var tile in MapTiles)
		{
			if (tile.Updated)
				return true;
		}

		return false;
	}

	#region Initialisation
	public WorldCellData()
	{
		MapTiles = InitialiseWorldCellTileData();
	}

	private WorldCellTileData[,] InitialiseWorldCellTileData()
	{
		var data = new WorldCellTileData[GameWorld.WorldCellMapDimension, GameWorld.WorldCellMapDimension];

		for (int y = 0; y < GameWorld.WorldCellMapDimension; y++)
		{
			for (int x = 0; x < GameWorld.WorldCellMapDimension; x++)
			{
				data[x, y] = new WorldCellTileData()
				{
					X = x,
					Y = y,
					WorldTile = WorldTile.Unset,
					FeatureTile = FeatureTile.Unset,
					Shrouded = true,
					WorldTileUpdated = false,
					FeatureTileUpdated = false,
					ShroudUpdated = false,
					//ShroudUpdated = true,
				};
			}
		}

		return data;
	}
	#endregion

	#region WorldCellRegions
	public void SetupRegions()
	{
		Regions.Clear();

		//Setup
		bool[,] tilesVisited = new bool[GameWorld.WorldCellMapDimension, GameWorld.WorldCellMapDimension];
		TileCoordinates[] tilesAddedLastRound = new TileCoordinates[] { };
		List<TileCoordinates> tilesAddedThisRound = new List<TileCoordinates>();
		bool continueSearchingForRegionTiles = false;

		var regionID = 0;

		tilesAddedThisRound.Add(new TileCoordinates(0, 0));

		for (int y = 0; y < GameWorld.WorldCellMapDimension; y++)
		{
			for (int x = 0; x < GameWorld.WorldCellMapDimension; x++)
			{
				if (tilesVisited[x, y])
					continue;

				tilesVisited[x, y] = true;
				var currentMapTile = MapTiles[x, y];
				WorldTile currentWorldTile = currentMapTile.WorldTile;

				regionID++;
				var newRegion = new WorldCellRegion { ID = regionID, WorldTile = currentWorldTile };
				newRegion.Tiles.Add(new TileCoordinates(x, y));
				Regions.TryAdd(regionID, newRegion);

				tilesAddedThisRound.Add(new TileCoordinates(x, y));
				continueSearchingForRegionTiles = true;

				while (continueSearchingForRegionTiles)
				{
					tilesAddedLastRound = tilesAddedThisRound.ToArray();
					tilesAddedThisRound.Clear();

					foreach (var currentTile in tilesAddedLastRound)
					{
						var northTilePosition = new TileCoordinates(currentTile.X, currentTile.Y - 1);
						var southTilePosition = new TileCoordinates(currentTile.X, currentTile.Y + 1);
						var eastTilePosition = new TileCoordinates(currentTile.X + 1, currentTile.Y);
						var westTilePosition = new TileCoordinates(currentTile.X - 1, currentTile.Y);

						if (Regions_IsValidMapPosition(northTilePosition, GameWorld.WorldCellMapDimension) &&
							!tilesVisited[northTilePosition.X, northTilePosition.Y]
							&& MapTiles[northTilePosition.X, northTilePosition.Y].WorldTile == currentWorldTile)
						{
							newRegion.Tiles.Add(northTilePosition);
							tilesVisited[northTilePosition.X, northTilePosition.Y] = true;
							tilesAddedThisRound.Add(northTilePosition);
						}

						if (Regions_IsValidMapPosition(southTilePosition, GameWorld.WorldCellMapDimension)
							&& !tilesVisited[southTilePosition.X, southTilePosition.Y]
							&& MapTiles[southTilePosition.X, southTilePosition.Y].WorldTile == currentWorldTile)
						{
							newRegion.Tiles.Add(southTilePosition);
							tilesVisited[southTilePosition.X, southTilePosition.Y] = true;
							tilesAddedThisRound.Add(southTilePosition);
						}

						if (Regions_IsValidMapPosition(eastTilePosition, GameWorld.WorldCellMapDimension)
							&& !tilesVisited[eastTilePosition.X, eastTilePosition.Y]
							&& MapTiles[eastTilePosition.X, eastTilePosition.Y].WorldTile == currentWorldTile)
						{
							newRegion.Tiles.Add(eastTilePosition);
							tilesVisited[eastTilePosition.X, eastTilePosition.Y] = true;
							tilesAddedThisRound.Add(eastTilePosition);
						}

						if (Regions_IsValidMapPosition(westTilePosition, GameWorld.WorldCellMapDimension)
							&& !tilesVisited[westTilePosition.X, westTilePosition.Y]
							&& MapTiles[westTilePosition.X, westTilePosition.Y].WorldTile == currentWorldTile)
						{
							newRegion.Tiles.Add(westTilePosition);
							tilesVisited[westTilePosition.X, westTilePosition.Y] = true;
							tilesAddedThisRound.Add(westTilePosition);
						}
					}

					continueSearchingForRegionTiles = tilesAddedThisRound.Count > 0;
				}
			}
		}

		//int tileCount = Regions.Select(item => item.Tiles.Count).Sum();
		//string regionList = string.Concat(Regions.Select(item => item.WorldTile + System.Environment.NewLine)).TrimEnd();
		//Debug.Print($"Cell has {Regions.Count} regions, totalling {tileCount} tiles.{System.Environment.NewLine}Cell Regions:{System.Environment.NewLine}{regionList}");
	}

	public int Regions_GetTileCountOfType(WorldTile worldTile)
	{
		var count = 0;

		foreach (var region in Regions.Values)
		{
			if (region.WorldTile != worldTile)
				continue;

			count += region.Tiles.Count;
		}

		return count;
	}

	public List<WorldCellRegion> Regions_GetAllRegionsOfTypes(params WorldTile[] worldTiles)
	{
		var regions = new List<WorldCellRegion>();

		foreach (var region in Regions.Values)
		{
			if (!worldTiles.Contains(region.WorldTile))
				continue;

			regions.Add(region);
		}

		return regions;
	}

	public WorldCellRegion Regions_GetRegionAtPosition(TileCoordinates position, out WorldTile worldTileType)
	{
		var targetRegion = Regions.Values.FirstOrDefault(region => region.Tiles.Contains(position));
		if (targetRegion != null)
		{
			worldTileType = targetRegion.WorldTile;
			return targetRegion;
		}
		else
		{
			worldTileType = WorldTile.Unset;
			return null;
		}
	}

	public int Regions_GetRegionIDAtPosition(TileCoordinates position, out WorldTile worldTileType)
	{
		var region = Regions_GetRegionAtPosition(position, out worldTileType);
		return region != null ? region.ID : -1;
	}

	public List<TileCoordinates> Regions_GetAllTilesOfTypes(bool emptyTiles, params WorldTile[] worldTiles)
	{
		List<TileCoordinates> result = new List<TileCoordinates>();

		foreach (var region in Regions)
		{
			if (!worldTiles.Contains(region.Value.WorldTile))
				continue;

			if (emptyTiles)
			{
				foreach (var tile in region.Value.Tiles)
				{
					if (MapTiles[tile.X, tile.Y].EntityTile == EntityTile.Unset)
						result.Add(tile);
				}
			}
			else
			{
				result.AddRange(region.Value.Tiles);
			}
		}

		return result;
	}

	private bool Regions_IsValidMapPosition(TileCoordinates tilePosition, int mapDimension)
	{
		return (tilePosition.X >= 0 && tilePosition.X < mapDimension) && (tilePosition.Y >= 0 && tilePosition.Y < mapDimension);
	}
	#endregion

	#region Entities
	public bool AddEntity<T1, T2>(Vector2 position, out T1 node) where T1 : Node2D, IEntity where T2 : EntityData
	{
		node = null;

		if (typeof(T1) is ITileEntity)
		{
			Debug.Print("Can't add a Tile Entity with the non-Tile Entity method.");
			return false;
		}

		var entityData = (T2)Activator.CreateInstance(typeof(T2));
		return AddEntity<T1>(position, entityData, out node);
	}

	public bool AddEntity<T>(Vector2 position, EntityData entityData, out T node) where T : Node2D, IEntity
	{
		node = null;

		if (typeof(T) is ITileEntity)
		{
			Debug.Print("Can't add a Tile Entity with the non-Tile Entity method.");
			return false;
		}

		var entity = SceneStore.Instantiate<T>();

		entityData.ID = entity.GetInstanceId();
		entityData.Name = entity.Name;
		entityData.Position = position;
		entity.Initialise(entityData);

		Entities.Add(entity.GetInstanceId(), entityData);

		node = (T)entity;
		return true;
	}

	public bool AddTileEntity<T1, T2>(TileCoordinates tileCoordinates, out T1 node) where T1 : Node2D, ITileEntity where T2 : TileEntityData
	{
		var entityData = (T2)Activator.CreateInstance(typeof(T2));
		return AddTileEntity<T1>(tileCoordinates, entityData, out node);
	}

	public bool AddTileEntity<T>(TileCoordinates tileCoordinates, TileEntityData entityData, out T node) where T : Node2D, ITileEntity
	{
		node = null;
		var targetTile = MapTiles[tileCoordinates.X, tileCoordinates.Y];

		if (!WorldCell.ValidSpawnRegions[entityData.TileEntityType].Contains(targetTile.WorldTile))
		{
			Debug.Print($"Couldn't create a tile entity. Tried to plant on {targetTile.WorldTile} but it was not in ({string.Join(",", WorldCell.ValidSpawnRegions[entityData.TileEntityType])})");
			return false;
		}

		if (MapTiles[tileCoordinates.X, tileCoordinates.Y].HasEntity)
		{
			Debug.Print($"Couldn't create a tile entity. Tile already has an entity.");
			return false;
		}

		var entity = SceneStore.Instantiate<T>();

		entityData.ID = entity.GetInstanceId();
		entityData.Name = entity.Name;
		entityData.Position = GameWorld.GetPositionFromCoordinates(CellCoordinates, tileCoordinates);
		entityData.CellCoordinates = CellCoordinates;
		entityData.TileCoordinates = tileCoordinates;

		entity.Initialise(entityData);

		MapTiles[tileCoordinates.X, tileCoordinates.Y].UpdateEntity(entity.GetInstanceId());
		Entities.Add(entity.GetInstanceId(), entityData);

		node = entity;
		return true;
	}

	public void RestoreEntity()
	{
		//TODO: Find a way to restore entity from Data and instantiate it.
		//It must probably be done from the World Cell, when restoring a serialised WorldCellData from there
	}

	public void RemoveEntity(ulong ID)
	{
		if (Entities.ContainsKey(ID))
			Entities.Remove(ID);

		var item = GD.InstanceFromId(ID);
		if (item != null && !item.IsQueuedForDeletion())
		{
			((Node2D)item).QueueFree();
		}
	}

	public void RemoveEntity(TileCoordinates tileCoordinates)
	{
		var tile = MapTiles[tileCoordinates.X, tileCoordinates.Y];
		var ID = tile.TileEntityID;
		if (tile.HasEntity)
		{
			Entities.Remove(ID);
			var item = GD.InstanceFromId(ID);
			if (item != null && !item.IsQueuedForDeletion())
			{
				((Node2D)item).QueueFree();
			}
		}

		tile.ClearEntity();
	}
	#endregion
}
