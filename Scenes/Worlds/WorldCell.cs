using Godot;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

public class WorldCell : Node2D
{
	public WorldCellData Data = new WorldCellData(); //TODO: Replace when we Serialize and Deserialize this

	public CellCoordinates CellCoordinates { get { return Data.CellCoordinates; } }
	private int XOffset { get { return Data.XOffset; } }
	private int YOffset { get { return Data.YOffset; } }

	public bool WorldTilesInitialised { get { return Data.WorldTilesInitialised; } private set { Data.WorldTilesInitialised = value; } }
	public bool FeatureTilesInitialised { get { return Data.FeatureTilesInitialised; } private set { Data.FeatureTilesInitialised = value; } }
	public bool EntityTilesInitialised { get { return Data.EntityTilesInitialised; } private set { Data.EntityTilesInitialised = value; } }

	private float[,] HeightMap { get { return Data.HeightMap; } }
	private WorldCellTileData[,] MapTiles { get { return Data.MapTiles; } }
	//private Dictionary<ulong, EntityData> Entities { get { return Data.Entities; } }

	private bool[,] RenderedTiles = new bool[GameWorld.WorldCellMapDimension, GameWorld.WorldCellMapDimension];

	public bool MapUpdated { get { return Data.MapUpdated; } }

	public ImageTexture MapWorldTilesImage;
	public ImageTexture MapFeatureTilesImage;
	public ImageTexture MapEntityTilesImage;
	private Image mapWorldTileImage;
	private Image mapFeatureTileImage;
	private Image mapEntityTileImage;

	private bool startingCell;

	public static readonly WorldTile[] TraversableTileTypes = new WorldTile[] { WorldTile.Sand, WorldTile.Plains, WorldTile.Grass, WorldTile.RockTier1, WorldTile.RockTier2 };
	public static readonly WorldTile[] ValidSoilTileTypes = new WorldTile[] { WorldTile.Plains, WorldTile.Grass };
	//TODO: Put all of these inside of the Biome information.
	public static readonly Dictionary<TileEntityType, WorldTile[]> ValidSpawnRegions = new Dictionary<TileEntityType, WorldTile[]>
	{
		{ TileEntityType.Building, TraversableTileTypes },
		{ TileEntityType.Crop, ValidSoilTileTypes },
		{ TileEntityType.Misc, TraversableTileTypes },
		{ TileEntityType.Tree, ValidSoilTileTypes },
	};

	private Node2D entities;

	public bool IsActive { get; internal set; }

	public void InitialiseCell(CellCoordinates cellCoordinates, bool startingCell)
	{
		this.startingCell = startingCell;

		Data.CellCoordinates = cellCoordinates;
		Data.SetupRegions();

		//Tile Offsets for the current map
		Data.XOffset = CellCoordinates.X * GameWorld.WorldCellMapDimension;
		Data.YOffset = CellCoordinates.Y * GameWorld.WorldCellMapDimension;

		Data.HeightMap = Noise.GenerateNoiseMap(GameWorld.WorldCellMapDimension, new Vector2(XOffset, YOffset), Noise.NormalizeMode.Global);
		ClassifyWorldTilesFromHeightMap();

		entities = GetNode<Node2D>("CellEntities");
	}

	public void Deactivate()
	{
		Visible = false;
		SetProcess(false);

		foreach (var tile in MapTiles)
		{
			//foreach (var tilemap in GameWorld.WorldTileDictionary.Values)
			//{
			//	tilemap.SetCellv(new Vector2(XOffset + tile.X, YOffset + tile.Y), -1);
			//}

			//for (int j = tile.Y - 1; j < tile.Y + 1; j++)
			//{
			//	for (int i = tile.X - 1; i < tile.X + 1; i++)
			//	{
			//		var xPos = i + XOffset;
			//		var yPos = j + YOffset;
			//		var pos = new Vector2(xPos, yPos);

			//		//Debug.Print($"Drawing portion {pos.ToString()}");
			//		GameWorld.WorldTileDictionary[tile.WorldTile].SetCellv(pos, -1);
			//	}
			//}

			GameWorld.WorldTileDictionary[tile.WorldTile].SetCellv(new Vector2(XOffset + tile.X, YOffset + tile.Y), -1);
			RenderedTiles[tile.X, tile.Y] = false;
			GameWorld.FeatureTileMap.SetCellv(new Vector2(XOffset + tile.X, YOffset + tile.Y), -1);
		}

		foreach (Node2D entity in entities.GetChildren())
		{
			entity.SetProcess(false);
			entity.Visible = false;
		}
	}

	public void Activate()
	{
		Visible = true;
		SetProcess(true);

		foreach (Node2D entity in entities.GetChildren())
		{
			if (entity != null)
			{
				entity.SetProcess(true);
				entity.Visible = false;
			}
		}
	}

	public bool ToggleSoilAtPosition(TileCoordinates tileCoordinates)
	{
		//Debug.Print($"Toggling Soil at {tileCoordinates}");

		var targetTile = MapTiles[tileCoordinates.X, tileCoordinates.Y];

		if (targetTile.HasEntity)
			return false;

		if (targetTile.FeatureTile == FeatureTile.Soil)
		{
			targetTile.FeatureTile = FeatureTile.Unset;
			DrawFeatureTileForCellAndNeighbours(targetTile.X, targetTile.Y);
			return true;
		}
		if (ValidSoilTileTypes.Contains(targetTile.WorldTile))
		{
			targetTile.FeatureTile = FeatureTile.Soil;
			DrawFeatureTileForCellAndNeighbours(targetTile.X, targetTile.Y);
			return true;
		}

		return false;
	}

	public bool AddCropAtPosition<T1, T2>(TileCoordinates tileCoordinates) where T1 : Node2D, ICrop where T2 : CropData
	{
		var targetTile = MapTiles[tileCoordinates.X, tileCoordinates.Y];

		if (targetTile.HasEntity)
			return false;

		if (targetTile.FeatureTile != FeatureTile.Soil)
			return false;

		var success = AddTileEntity<T1, T2>(tileCoordinates, out _);
		return success;
	}

	private bool RemoveFeatureTileAtPosition(TileCoordinates tileCoordinates)
	{
		var targetTile = MapTiles[tileCoordinates.X, tileCoordinates.Y];

		if (targetTile.FeatureTile == FeatureTile.Soil)
		{
			targetTile.FeatureTile = FeatureTile.Unset;
			DrawFeatureTileForCellAndNeighbours(targetTile.X, targetTile.Y);
			return true;
		}

		//TODO: Make this false for any Feature Tiles we cannot remove

		return true;
	}

	public void AddPickup(ItemPickupEntity item)
	{
		entities.AddChild(item);
	}

	public bool AddEntity<T1, T2>(Vector2 position, out T1 node) where T1 : Node2D, IEntity where T2 : EntityData
	{
		//TODO: If able to place an item near this object
		//TOOD: Do a cast of some kind to see if we overlap with some object at this position
		//GetWorld2d().DirectSpaceState.IntersectPoint(position);

		node = null;
		var success = Data.AddEntity<T1, T2>(position, out T1 entity);
		if (!success)
			return false;

		entity.Position = new Vector2((CellCoordinates.X * GameWorld.WorldCellMapDimension + position.x) * GameWorld.WorldCellTileSize + GameWorld.WorldCellTileSize / 2,
									(CellCoordinates.Y * GameWorld.WorldCellMapDimension + position.y) * GameWorld.WorldCellTileSize + GameWorld.WorldCellTileSize / 2);
		entities.AddChild(entity);
		node = (T1)entity;
		return true;
	}

	public bool AddTileEntity<T1, T2>(TileCoordinates tileCoordinates, out T1 node) where T1 : Node2D, ITileEntity where T2 : TileEntityData
	{
		node = null;
		var success = Data.AddTileEntity<T1, T2>(tileCoordinates, out T1 entity);
		if (!success)
			return false;

		//Debug.Print($"Tile Entity spawned. Instance: {entity}");
		entity.Position = new Vector2((CellCoordinates.X * GameWorld.WorldCellMapDimension + tileCoordinates.X) * GameWorld.WorldCellTileSize + GameWorld.WorldCellTileSize / 2,
									(CellCoordinates.Y * GameWorld.WorldCellMapDimension + tileCoordinates.Y) * GameWorld.WorldCellTileSize + GameWorld.WorldCellTileSize / 2);
		entities.AddChild(entity);
		node = (T1)entity;
		//Debug.Print($"Converted to Node: Instance: {node}");
		return true;
	}

	public void RemoveEntity(ulong nodeID)
	{
		Data.RemoveEntity(nodeID);
		//var node = (Node2D)GD.InstanceFromId(nodeID);
		//entities.RemoveChild(node);
	}

	public void RemoveEntity(TileCoordinates tileCoordinates)
	{
		Data.RemoveEntity(tileCoordinates);
		//var node = (Node2D)GD.InstanceFromId(nodeID);
		//entities.RemoveChild(node);
	}

	#region TileSetup
	public void UpdateCellTiles()
	{
		UpdateCellWorldTiles();
		UpdateCellFeatureTiles();
		UpdateCellEntityTiles();
	}

	private bool UpdateMapImages()
	{
		bool updatedWorldTileMap = UpdateWorldTileMapImage();
		bool updatedFeatureTileMap = UpdateFeatureTileMapImage();
		bool updatedEntityTileMap = UpdateEntityTileMapImage();

		foreach (var tile in MapTiles) //Tiles drawn
		{
			tile.WorldTileUpdated = false;
			tile.FeatureTileUpdated = false;
			tile.EntityTileUpdated = false;
			tile.ShroudUpdated = false;
		}

		return updatedWorldTileMap || updatedFeatureTileMap || updatedEntityTileMap;
	}

	public bool UpdateTileMaps()
	{
		foreach (var mapTile in MapTiles)
		{
			var tilePosition = new Vector2(mapTile.X + XOffset, mapTile.Y + YOffset);

			if (GameWorld.MustRenderTileInPosition(tilePosition))
			{
				if (!RenderedTiles[mapTile.X, mapTile.Y])
				{
					DrawWorldTileForCellAndNeighbours(mapTile.X, mapTile.Y);
					DrawFeatureTileForCellAndNeighbours(mapTile.X, mapTile.Y);
					mapTile.Shrouded = false;
				}
			}
			else
			{
				if (RenderedTiles[mapTile.X, mapTile.Y])
				{
					//for (int j = mapTile.Y - 1; j < mapTile.Y + 1; j++)
					//{
					//	for (int i = mapTile.X - 1; i < mapTile.X + 1; i++)
					//	{
					//		var xPos = i + XOffset;
					//		var yPos = j + YOffset;
					//		var pos = new Vector2(xPos, yPos);

					//		//Debug.Print($"Drawing portion {pos.ToString()}");
					//		GameWorld.WorldTileDictionary[mapTile.WorldTile].SetCellv(pos, -1);
					//	}
					//}

					GameWorld.WorldTileDictionary[mapTile.WorldTile].SetCellv(tilePosition, -1);
					RenderedTiles[mapTile.X, mapTile.Y] = false;
					GameWorld.FeatureTileMap.SetCellv(tilePosition, -1);
				}
			}
		}

		return UpdateMapImages();
	}

	private WorldTile UpdateWorldTileAtPosition(int worldTileX, int worldTileY, WorldTile otherTile, bool updateRegionsForTile = true)
	{
		var newTile = MapTiles[worldTileX, worldTileY].WorldTile.GetPriorityTile(otherTile);
		MapTiles[worldTileX, worldTileY].WorldTile = newTile;

		return newTile;
	}

	private FeatureTile UpdateFeatureTileAtPosition(int featureTileX, int featureTileY, FeatureTile otherTile)
	{
		var newTile = MapTiles[featureTileX, featureTileY].FeatureTile.GetPriorityTile(otherTile);
		MapTiles[featureTileX, featureTileY].FeatureTile = newTile;

		return newTile;
	}

	private void UpdateCellWorldTiles()
	{
		GenerateRiver();
		GenerateCliffs();

		Data.WorldTilesInitialised = true;
	}

	private void UpdateCellFeatureTiles()
	{
		GenerateCave();
		//Replace the above methods with the following, which introduce a chance for these things to happen
		//TODO: GenerateRandomNumberOfRivers();
		//TODO: GenerateRandomNumberOfForests();
		//TODO: GenerateRandomNumberOfCaves();

		Data.FeatureTilesInitialised = true;
	}

	private void UpdateCellEntityTiles()
	{
		GenerateForests();
		GenerateTallGrass();
		GenerateRocks();

		Data.EntityTilesInitialised = true;
	}

	private bool UpdateWorldTileMapImage()
	{
		bool mapUpdated = false;

		if (mapWorldTileImage == null)
		{
			mapWorldTileImage = new Image();
			mapWorldTileImage.Create(GameWorld.WorldCellMapDimension, GameWorld.WorldCellMapDimension, true, Image.Format.Rgbaf);
		}

		mapWorldTileImage.Lock();
		foreach (var mapTile in MapTiles)
		{
			var position = new Vector2(mapTile.X, mapTile.Y);

			if (!mapTile.Shrouded && (mapTile.ShroudUpdated || mapTile.WorldTileUpdated))
			{
				switch (mapTile.WorldTile)
				{
					case WorldTile.Water:
					case WorldTile.River:
						mapWorldTileImage.SetPixelv(position, Colors.Blue);
						break;
					case WorldTile.Sand:
						mapWorldTileImage.SetPixelv(position, Colors.Yellow);
						break;
					case WorldTile.Plains:
						mapWorldTileImage.SetPixelv(position, Colors.OliveDrab);
						break;
					case WorldTile.Grass:
						mapWorldTileImage.SetPixelv(position, Colors.DarkGreen);
						break;
					case WorldTile.RockTier1:
						mapWorldTileImage.SetPixelv(position, Colors.LightGray);
						break;
					case WorldTile.Cliff:
						mapWorldTileImage.SetPixelv(position, Colors.DarkSlateGray);
						break;
					case WorldTile.RockTier2:
						mapWorldTileImage.SetPixelv(position, Colors.SlateGray);
						break;
					case WorldTile.Mountain:
						mapWorldTileImage.SetPixelv(position, Colors.WhiteSmoke);
						break;
				}

				mapUpdated = true;
			}
		}

		mapWorldTileImage.Unlock();

		if (mapUpdated)
		{
			MapWorldTilesImage = new ImageTexture();
			MapWorldTilesImage.CreateFromImage(mapWorldTileImage);
			MapWorldTilesImage.Storage = ImageTexture.StorageEnum.CompressLossless;
			MapWorldTilesImage.Flags = 0;
		}

		return mapUpdated;
	}

	private bool UpdateFeatureTileMapImage()
	{
		bool mapUpdated = false;

		if (mapFeatureTileImage == null)
		{
			mapFeatureTileImage = new Image();
			mapFeatureTileImage.Create(GameWorld.WorldCellMapDimension, GameWorld.WorldCellMapDimension, true, Image.Format.Rgbaf);
		}

		mapFeatureTileImage.Lock();
		foreach (var mapTile in MapTiles)
		{
			var position = new Vector2(mapTile.X, mapTile.Y);

			if (!mapTile.Shrouded && (mapTile.ShroudUpdated || mapTile.FeatureTileUpdated))
			{
				switch (mapTile.FeatureTile)
				{
					case FeatureTile.Soil:
						mapFeatureTileImage.SetPixelv(position, Colors.SaddleBrown);
						break;
					default:
						mapFeatureTileImage.SetPixelv(position, Colors.Transparent);
						break;
				}

				mapUpdated = true;
			}
		}

		mapFeatureTileImage.Unlock();

		if (mapUpdated)
		{
			MapFeatureTilesImage = new ImageTexture();
			MapFeatureTilesImage.CreateFromImage(mapFeatureTileImage);
			MapFeatureTilesImage.Storage = ImageTexture.StorageEnum.CompressLossless;
			MapFeatureTilesImage.Flags = 0;
		}

		return mapUpdated;
	}

	private bool UpdateEntityTileMapImage()
	{
		bool mapUpdated = false;

		if (mapEntityTileImage == null)
		{
			mapEntityTileImage = new Image();
			mapEntityTileImage.Create(GameWorld.WorldCellMapDimension, GameWorld.WorldCellMapDimension, true, Image.Format.Rgbaf);
		}

		mapEntityTileImage.Lock();

		foreach (var mapTile in MapTiles)
		{
			var position = new Vector2(mapTile.X, mapTile.Y);

			if (!mapTile.Shrouded && (mapTile.ShroudUpdated || mapTile.EntityTileUpdated))
			{
				switch (mapTile.EntityTile)
				{
					case EntityTile.Tree:
						mapEntityTileImage.SetPixelv(position, Colors.SaddleBrown);
						break;
					default:
						mapEntityTileImage.SetPixelv(position, Colors.Transparent);
						break;
				}

				mapUpdated = true;
			}
		}

		mapEntityTileImage.Unlock();

		if (mapUpdated)
		{
			MapEntityTilesImage = new ImageTexture();
			MapEntityTilesImage.CreateFromImage(mapEntityTileImage);
			MapEntityTilesImage.Storage = ImageTexture.StorageEnum.CompressLossless;
			MapEntityTilesImage.Flags = 0;
		}

		return mapUpdated;
	}

	private void ClassifyWorldTilesFromHeightMap()
	{
		foreach (var mapTile in MapTiles)
		{
			if (mapTile.WorldTile != WorldTile.Unset)
			{
				//Debug.Print($"{CellCoordinates} Tile ({x},{y}) is not Unset");
				continue;
			}

			//Debug.Print($"Updating {CellCoordinates} Tile ({x},{y})");
			var X = mapTile.X;
			var Y = mapTile.Y;
			var currentValue = HeightMap[mapTile.X, mapTile.Y];
			UpdateWorldTileAtPosition(X, Y, WorldGen.GetWorldTileFromHeight(currentValue), false);
		}

		Data.SetupRegions();
	}

	private void DrawWorldTileForCellAndNeighbours(int x, int y)
	{
		var mapTile = MapTiles[x, y];
		WorldTile targetTileType = mapTile.WorldTile;

		if (targetTileType == WorldTile.Unset)
			return;

		if (targetTileType.IsMultiTile())
		{
			try
			{
				//Debug.Print($"Drawing tile Cell ({CellCoordinates.X},{CellCoordinates.Y}) ({x},{y}) Tile Type {targetTileType.ToString()}");

				//for (int j = y - 1; j < y + 1; j++)
				//{
				//	for (int i = x - 1; i < x + 1; i++)
				//	{
				var xPos = x + XOffset;
				var yPos = y + YOffset;
				var pos = new Vector2(xPos, yPos);

				//Debug.Print($"Drawing portion {pos.ToString()}");
				GameWorld.WorldTileDictionary[mapTile.WorldTile].SetCellv(pos, (int)targetTileType);
				GameWorld.WorldTileDictionary[mapTile.WorldTile].UpdateBitmaskArea(pos);
				//	}
				//}

				//var topLeftRegion = new Vector2(x + XOffset - 1, y + YOffset - 1);
				//var bottomRightRegion = new Vector2(x + XOffset + 1, y + YOffset + 1);
				//var middlePos = new Vector2(x + XOffset, y + YOffset);
				//GameWorld.WorldTileDictionary[mapTile.WorldTile].SetCellv(middlePos, (int)targetTileType);
				//GameWorld.WorldTileDictionary[mapTile.WorldTile].UpdateBitmaskArea(middlePos);
				//GameWorld.WorldTileDictionary[mapTile.WorldTile].UpdateBitmaskRegion(topLeftRegion, bottomRightRegion);
				RenderedTiles[mapTile.X, mapTile.Y] = true;
			}
			catch (Exception e)
			{
				Debug.Print($"Something went super wrong. Here's the message:", e.Message, e.StackTrace);
				throw e;
			}
		}
		else
		{
			var pos = new Vector2(x + XOffset, y + YOffset);

			GameWorld.WorldTileDictionary[mapTile.WorldTile].SetCellv(pos, (int)targetTileType);
			GameWorld.WorldTileDictionary[mapTile.WorldTile].UpdateBitmaskArea(pos);
		}

		//mapTile.WorldTileUpdated = false;
	}

	private void DrawFeatureTileForCellAndNeighbours(int x, int y)
	{
		var mapTile = MapTiles[x, y];
		FeatureTile targetTileType = mapTile.FeatureTile;

		var pos = new Vector2(x + XOffset, y + YOffset);

		if (targetTileType == FeatureTile.Unset)
		{
			GameWorld.FeatureTileMap.SetCellv(pos, -1);
			GameWorld.FeatureTileMap.UpdateBitmaskArea(pos);
		}
		else
		{
			GameWorld.FeatureTileMap.SetCellv(pos, (int)targetTileType);
			GameWorld.FeatureTileMap.UpdateBitmaskArea(pos);
		}

		//mapTile.FeatureTileUpdated = false;
	}
	#endregion

	#region Caves
	private void GenerateCave()
	{

	}
	#endregion

	#region Entities
	#region Forests
	private void GenerateForests()
	{
		var worldTiles = Data.Regions_GetAllTilesOfTypes(true, ValidSpawnRegions[TileEntityType.Tree]);
		//TODO: GetBiomeTreeGenerationDictionary(); Return the dictionary for the biome we are currently in

		var poissonNoiseTiles = PoissonDiscNoise.GeneratePointsForWorldTiles(TemperateBiome.MapGen_TreeRadius, new Vector2(GameWorld.WorldCellMapDimension, GameWorld.WorldCellMapDimension));

		foreach (var tile in worldTiles)
		{
			var tileType = MapTiles[tile.X, tile.Y].WorldTile;
			if (poissonNoiseTiles[tileType].Contains(tile))
				AddTree(tile.X, tile.Y);
		}
	}

	private void AddTree(int x, int y)
	{
		MapTiles[x, y].QueueEntityForSpawn(EntityTile.Tree);
	}
	#endregion

	#region TallGrass
	private void GenerateTallGrass()
	{
		var worldTiles = Data.Regions_GetAllTilesOfTypes(true, ValidSpawnRegions[TileEntityType.Crop]);
		//TODO: GetBiomeTreeGenerationDictionary(); Return the dictionary for the biome we are currently in

		var poissonNoiseTiles = PoissonDiscNoise.GeneratePointsForWorldTiles(TemperateBiome.MapGen_TallGrassRadius, new Vector2(GameWorld.WorldCellMapDimension, GameWorld.WorldCellMapDimension));

		foreach (var tile in worldTiles)
		{
			var tileType = MapTiles[tile.X, tile.Y].WorldTile;
			if (poissonNoiseTiles[tileType].Contains(tile))
				AddTallGrass(tile.X, tile.Y);
		}
	}

	private void AddTallGrass(int x, int y)
	{
		int randomRadius = GameWorld.RNG.RandiRange(1, 4);
		int radiusSquared = (int)Mathf.Pow(randomRadius, 2);
		int grassSpawnCap = GameWorld.RNG.RandiRange(radiusSquared / 4, radiusSquared / 2);

		int startX = Mathf.Max(0, x - randomRadius);
		int endX = Mathf.Min(x + randomRadius, GameWorld.WorldCellMapDimension - 1);
		int startY = Mathf.Max(0, y - randomRadius);
		int endY = Mathf.Min(y + randomRadius, GameWorld.WorldCellMapDimension - 1);

		var currentGrassCounter = grassSpawnCap;
		while (currentGrassCounter > 0)
		{
			var xPos = GameWorld.RNG.RandiRange(startX, endX);
			var yPos = GameWorld.RNG.RandiRange(startY, endY);
			var mapTile = MapTiles[xPos, yPos];

			if (ValidSpawnRegions[TileEntityType.Crop].Contains(mapTile.WorldTile))
				MapTiles[xPos, yPos].QueueEntityForSpawn(EntityTile.TallGrass);

			currentGrassCounter--;
		}
	}
	#endregion

	#region Rocks
	private void GenerateRocks()
	{
		var worldTiles = Data.Regions_GetAllTilesOfTypes(true, ValidSpawnRegions[TileEntityType.Misc]);
		//TODO: GetBiomeTreeGenerationDictionary(); Return the dictionary for the biome we are currently in

		var poissonNoiseTiles = PoissonDiscNoise.GeneratePointsForWorldTiles(TemperateBiome.MapGen_RockRadius, new Vector2(GameWorld.WorldCellMapDimension, GameWorld.WorldCellMapDimension));

		foreach (var tile in worldTiles)
		{
			var tileType = MapTiles[tile.X, tile.Y].WorldTile;
			if (poissonNoiseTiles[tileType].Contains(tile))
				AddRock(tile.X, tile.Y);
		}
	}

	private void AddRock(int x, int y)
	{
		MapTiles[x, y].QueueEntityForSpawn(EntityTile.Rock);
	}
	#endregion

	public void PlaceTileEntities()
	{
		foreach (var mapTile in MapTiles)
		{
			var X = mapTile.X;
			var Y = mapTile.Y;
			//WorldCellRegions.GetRegionIDAtPosition(new Vector2Int(x, y), out WorldTile worldTileType);
			if (!mapTile.EntityTileUpdated)
				continue;

			if (!mapTile.EntityQueued)
				continue;

			switch (mapTile.EntityTile)
			{
				case EntityTile.Tree:
					{
						if (!ValidSpawnRegions[TileEntityType.Tree].Contains(mapTile.WorldTile))
							RemoveEntity(new TileCoordinates(X, Y));
						//mapTile.ClearEntity(); //TODO: Go to river and lake generation and remove entities with every tile we place. It's not 100%

						InitialiseTileEntity<TreeEntity, TreeData>(mapTile);
						break;
					}
				case EntityTile.TallGrass:
					{
						if (!ValidSpawnRegions[TileEntityType.Crop].Contains(mapTile.WorldTile))
							RemoveEntity(new TileCoordinates(X, Y));

						InitialiseTileEntity<TallGrassEntity, TallGrassData>(mapTile);
						break;
					}
				case EntityTile.Rock:
					{
						if (!ValidSpawnRegions[TileEntityType.Misc].Contains(mapTile.WorldTile))
							RemoveEntity(new TileCoordinates(X, Y));

						InitialiseTileEntity<RockEntity, RockData>(mapTile);
						break;
					}
				default:
					mapTile.ClearEntity();
					break;
			}
		}
	}

	private void InitialiseTileEntity<T1, T2>(WorldCellTileData mapTile) where T1 : Node2D, ITileEntity where T2 : TileEntityData
	{
		if (mapTile.TileEntityID > 0)
		{
			//Debug.Print($"An item already exists at {CellCoordinates}{mapTile}. Cannot spawn entity");
			return;
		}

		var tileCoordinates = new TileCoordinates(mapTile.X, mapTile.Y);

		if (!RemoveFeatureTileAtPosition(tileCoordinates))
		{
			return;
		}

		var success = AddTileEntity<T1, T2>(tileCoordinates, out T1 entity);
		if (!success)
			return;

		entity.Visible = false;
		//Debug.Print($"Spawned {nameof(T1)} at ({entity.Position.x},{entity.Position.y})");
	}
	#endregion

	#region Rivers
	private void GenerateRiver()
	{
		//TODO: Consider width of the river. Start with a single tile and then consider expanding it on a second pass or so
		var mountainCells = Data.Regions_GetAllTilesOfTypes(true, WorldTile.Mountain);

		if (mountainCells.Count == 0)
		{
			//Debug.Print($"No mountains on cell ({CellCoordinates.X},{CellCoordinates.Y}). We cannot start a river.");
			return;
		}

		int targetMountainCellIndex = GameWorld.RNG.RandiRange(0, mountainCells.Count - 1);
		var randomMountainCellPosition = mountainCells[targetMountainCellIndex];
		//Debug.Print($"Mountain cell is ({randomMountainCellPosition.X},{randomMountainCellPosition.Y})");
		TileCoordinates localTilesPosition = new TileCoordinates(randomMountainCellPosition.X, randomMountainCellPosition.Y); //Remove the offset to get back to WorldTiles coordinates
		TileCoordinates riverStartPosition = localTilesPosition;

		//Debug.Print($"Set position for new river in cell ({CellPosition.x},{CellPosition.y}) to {localTilesPosition}");
		var riverTiles = new List<TileCoordinates>();
		GenerateRiverTiles(riverStartPosition, ref riverTiles);
	}

	private void GenerateRiverFromPosition(TileCoordinates startPosition, ref List<TileCoordinates> riverTiles)
	{
		//Debug.Print($"Set position to continue river generation in cell {CellCoordinates} to {startPosition}");
		if (!Data.WorldTilesInitialised)
		{
			ClassifyWorldTilesFromHeightMap();
		}

		GenerateRiverTiles(startPosition, ref riverTiles);
	}

	private void GenerateRiverTiles(TileCoordinates riverStartPosition, ref List<TileCoordinates> riverTiles)
	{
		TileCoordinates currentCellPosition = riverStartPosition;

		while (true)
		{
			bool currentCellHasWater = TileExistsAtLocalPosition(WorldTile.Water, currentCellPosition);
			if (currentCellHasWater)
				break;

			var currentCellPosX = currentCellPosition.X;
			var currentCellPosY = currentCellPosition.Y;
			var offsetPosX = currentCellPosX + XOffset;
			var offsetPosY = currentCellPosY + YOffset;
			//Debug.Print($"Current river position in cell ({CellCoordinates.X},{CellCoordinates.Y}) is ({currentCellPosX},{currentCellPosY})");

			if (IsPositionInNeighbouringCell(currentCellPosX, currentCellPosY))
			{
				GenerateRiverInNeighbouringMap(currentCellPosition, ref riverTiles);
				break;
			}
			else
			{
				UpdateWorldTileAtPosition(currentCellPosX, currentCellPosY, WorldTile.River);

				riverTiles.Add(currentCellPosition);

				var northHeight = GetHeightMapValueForPosition(currentCellPosX, currentCellPosY - 1);
				var southHeight = GetHeightMapValueForPosition(currentCellPosX, currentCellPosY + 1);
				var eastHeight = GetHeightMapValueForPosition(currentCellPosX + 1, currentCellPosY);
				var westHeight = GetHeightMapValueForPosition(currentCellPosX - 1, currentCellPosY);

				var currentHeight = HeightMap[currentCellPosX, currentCellPosY];
				var minHeight = MathfExtensions.Min(currentHeight, northHeight, southHeight, eastHeight, westHeight);

				if (minHeight == northHeight) //Up
					currentCellPosY = currentCellPosY - 1;

				else if (minHeight == southHeight) //Down
					currentCellPosY = currentCellPosY + 1;

				else if (minHeight == westHeight) // Left
					currentCellPosX = currentCellPosX - 1;

				else if (minHeight == eastHeight) //Right
					currentCellPosX = currentCellPosX + 1;

				//We reached a local low. We should stop generating rivers
				if (minHeight == currentHeight)
				{
					var cellsToGenerateLakesIn = new CellLakeDictionary();
					GenerateLake(new CellLakeTarget(CellCoordinates, currentCellPosition), ref riverTiles, ref cellsToGenerateLakesIn);

					break;
				}

				//Update variables for next pass
				var newCellPosition = new TileCoordinates(currentCellPosX, currentCellPosY);
				currentCellPosition = newCellPosition;
			}
		}

		Data.SetupRegions();
		EventManager.RaiseEvent(Helpers.GlobalEventCodes.World_CellUpdated, this.CellCoordinates.ToString());
	}
	#endregion

	#region Lakes
	private void GenerateLake(CellLakeTarget cellLakeTarget, ref List<TileCoordinates> riverTiles, ref CellLakeDictionary cellsToGenerateLakesIn)
	{
		int lakeBacktrackDistance = GameWorld.RNG.RandiRange(riverTiles.Count / GameWorld.MapGenMinRiverDistanceLakeSizeRatio,
			riverTiles.Count / GameWorld.MapGenMaxRiverDistanceLakeSizeRatio);

		var lakeBacktrackTilePosition = riverTiles[riverTiles.Count - lakeBacktrackDistance - 1];
		var heightAtBacktrackPosition = GetHeightMapValueForPosition(lakeBacktrackTilePosition.X, lakeBacktrackTilePosition.Y);
		Debug.Print($"River Length is {riverTiles.Count}",
			$"Lake Backtrack Distance is {lakeBacktrackDistance}",
			$"Lake Backtrack Position is ({lakeBacktrackTilePosition.X},{lakeBacktrackTilePosition.Y})",
			$"Height at Lake Backtrack Position is {heightAtBacktrackPosition}");

		for (int i = riverTiles.Count - 1; i > riverTiles.Count - lakeBacktrackDistance; i--)
		{
			var currentRiverTile = riverTiles[i];
			MapTiles[currentRiverTile.X, currentRiverTile.Y].WorldTile = WorldTile.Water;
			RemoveEntity(new TileCoordinates(currentRiverTile.X, currentRiverTile.Y));
			//WorldCellRegions.UpdateRegionsForTile(MapTiles[riverTiles[i].X, riverTiles[i].Y]);//RedrawMaps = true;
		}

		cellsToGenerateLakesIn.AddCell(cellLakeTarget);

		GenerateLakeTiles(heightAtBacktrackPosition, lakeBacktrackDistance, cellLakeTarget, ref cellsToGenerateLakesIn);
	}

	private void GenerateLakeTiles(float heightAtBacktrackPosition, int lakeBacktrackDistance, CellLakeTarget lakeSource, ref CellLakeDictionary cellsToGenerateLakesIn)
	{
		ConcurrentDictionary<CellCoordinates, CellLakeTarget> cellLakeTargetsToAdd = new ConcurrentDictionary<CellCoordinates, CellLakeTarget>();
		var uncheckedWorldCell = cellsToGenerateLakesIn.GetCell(CellCoordinates);

		//Debug.Print($"Checking unchecked cell {uncheckedCell.CellCoordinates}", 
		//	$"Unchecked cell start position {uncheckedCell.CellStartPosition}");

		//if (uncheckedCell.CellStartPosition.X < 0 || uncheckedCell.CellStartPosition.X >= GameWorld.WorldCellMapDimension
		//	|| uncheckedCell.CellStartPosition.Y < 0 || uncheckedCell.CellStartPosition.Y >= GameWorld.WorldCellMapDimension)
		//	Debug.Print($"Something is wrong. There's an overflow");

		uncheckedWorldCell.Checked = true;

		//Setup
		bool[,] tilesToPaintWithWater = new bool[GameWorld.WorldCellMapDimension, GameWorld.WorldCellMapDimension];
		TileCoordinates[] tilesAddedLastRound = new TileCoordinates[] { };
		List<TileCoordinates> tilesAddedThisRound = new List<TileCoordinates>();
		bool continueSearchingForTiles = false;

		//First Cell
		var uncheckedCellHasWater = TileExistsAtLocalPosition(WorldTile.Water, uncheckedWorldCell.CellStartPosition);
		var uncheckedCellHeight = GetHeightMapValueForPosition(uncheckedWorldCell.CellStartPosition);
		var uncheckedCellBelowBacktrackPosition = uncheckedCellHeight <= heightAtBacktrackPosition;

		//Debug.Print($"Safety Check. Unchecked Cell StartPos = {uncheckedWorldCell.CellStartPosition}; Below BacktrackPosition = {uncheckedCellBelowBacktrackPosition}; Has Water = {uncheckedCellHasWater}");
		if (uncheckedCellHeight <= heightAtBacktrackPosition)
		{
			continueSearchingForTiles = true;
			tilesToPaintWithWater[uncheckedWorldCell.CellStartPosition.X, uncheckedWorldCell.CellStartPosition.Y] = true;
			tilesAddedThisRound.Add(uncheckedWorldCell.CellStartPosition);
		}

		//Check all neighbouring cells recursively
		while (continueSearchingForTiles)
		{
			tilesAddedLastRound = tilesAddedThisRound.ToArray();
			tilesAddedThisRound.Clear();

			foreach (var currentTile in tilesAddedLastRound)
			{
				var northTilePosition = new TileCoordinates(currentTile.X, currentTile.Y - 1);
				var southTilePosition = new TileCoordinates(currentTile.X, currentTile.Y + 1);
				var eastTilePosition = new TileCoordinates(currentTile.X + 1, currentTile.Y);
				var westTilePosition = new TileCoordinates(currentTile.X - 1, currentTile.Y);

				var northHeight = GetHeightMapValueForPosition(northTilePosition);
				var southHeight = GetHeightMapValueForPosition(southTilePosition);
				var eastHeight = GetHeightMapValueForPosition(eastTilePosition);
				var westHeight = GetHeightMapValueForPosition(westTilePosition);

				if (northHeight <= heightAtBacktrackPosition)
				{
					var tileAlreadyHasWater = TileExistsAtLocalPosition(WorldTile.Water, northTilePosition);
					var northTileInNeighbouringCell = IsPositionInNeighbouringCell(northTilePosition);

					if (!tileAlreadyHasWater)
					{
						if (northTileInNeighbouringCell)
						{
							var targetCell = GetNeighbouringCellCoordinates(CellNeighbour.North);
							var newLocalCellTilePosition = new TileCoordinates(northTilePosition.X, northTilePosition.Y + GameWorld.WorldCellMapDimension);
							var newCellLakeTarget = new CellLakeTarget(targetCell, newLocalCellTilePosition);
							var distanceFromLakeSource = GetTileDistanceBetweenPoints(targetCell, newLocalCellTilePosition, lakeSource.CellCoordinates, lakeSource.CellStartPosition);

							if (distanceFromLakeSource < lakeBacktrackDistance)
								cellLakeTargetsToAdd.TryAdd(newCellLakeTarget.CellCoordinates, newCellLakeTarget);
						}

						else if (tilesToPaintWithWater[northTilePosition.X, northTilePosition.Y] != true)
						{
							var distanceFromLakeSource = GetTileDistanceBetweenPoints(CellCoordinates, northTilePosition, lakeSource.CellCoordinates, lakeSource.CellStartPosition);
							if (distanceFromLakeSource < lakeBacktrackDistance)
							{
								tilesAddedThisRound.Add(northTilePosition);
								tilesToPaintWithWater[northTilePosition.X, northTilePosition.Y] = true;
							}
						}
					}
				}

				if (southHeight <= heightAtBacktrackPosition)
				{
					var tileAlreadyHasWater = TileExistsAtLocalPosition(WorldTile.Water, southTilePosition);
					var southTileInNeighbouringCell = IsPositionInNeighbouringCell(southTilePosition);

					if (!tileAlreadyHasWater)
					{
						if (southTileInNeighbouringCell)
						{
							var targetCell = GetNeighbouringCellCoordinates(CellNeighbour.South);
							var newLocalCellTilePosition = new TileCoordinates(southTilePosition.X, southTilePosition.Y - GameWorld.WorldCellMapDimension);
							var newCellLakeTarget = new CellLakeTarget(targetCell, newLocalCellTilePosition);
							var distanceFromLakeSource = GetTileDistanceBetweenPoints(targetCell, newLocalCellTilePosition, lakeSource.CellCoordinates, lakeSource.CellStartPosition);

							if (distanceFromLakeSource < lakeBacktrackDistance)
								cellLakeTargetsToAdd.TryAdd(newCellLakeTarget.CellCoordinates, newCellLakeTarget);
						}

						else if (tilesToPaintWithWater[southTilePosition.X, southTilePosition.Y] != true)
						{
							var distanceFromLakeSource = GetTileDistanceBetweenPoints(CellCoordinates, southTilePosition, lakeSource.CellCoordinates, lakeSource.CellStartPosition);
							if (distanceFromLakeSource < lakeBacktrackDistance)
							{
								tilesAddedThisRound.Add(southTilePosition);
								tilesToPaintWithWater[southTilePosition.X, southTilePosition.Y] = true;
							}
						}
					}
				}

				if (eastHeight <= heightAtBacktrackPosition)
				{
					var tileAlreadyHasWater = TileExistsAtLocalPosition(WorldTile.Water, eastTilePosition);
					var eastTileInNeighbouringCell = IsPositionInNeighbouringCell(eastTilePosition);

					if (!tileAlreadyHasWater)
					{
						if (eastTileInNeighbouringCell)
						{
							var targetCell = GetNeighbouringCellCoordinates(CellNeighbour.East);
							var newLocalCellTilePosition = new TileCoordinates(eastTilePosition.X - GameWorld.WorldCellMapDimension, eastTilePosition.Y);
							var newCellLakeTarget = new CellLakeTarget(targetCell, newLocalCellTilePosition);
							var distanceFromLakeSource = GetTileDistanceBetweenPoints(targetCell, newLocalCellTilePosition, lakeSource.CellCoordinates, lakeSource.CellStartPosition);

							if (distanceFromLakeSource < lakeBacktrackDistance)
								cellLakeTargetsToAdd.TryAdd(newCellLakeTarget.CellCoordinates, newCellLakeTarget);
						}

						else if (tilesToPaintWithWater[eastTilePosition.X, eastTilePosition.Y] != true)
						{
							var distanceFromLakeSource = GetTileDistanceBetweenPoints(CellCoordinates, eastTilePosition, lakeSource.CellCoordinates, lakeSource.CellStartPosition);
							if (distanceFromLakeSource < lakeBacktrackDistance)
							{
								tilesAddedThisRound.Add(eastTilePosition);
								tilesToPaintWithWater[eastTilePosition.X, eastTilePosition.Y] = true;
							}
						}
					}
				}

				if (westHeight <= heightAtBacktrackPosition)
				{
					var tileAlreadyHasWater = TileExistsAtLocalPosition(WorldTile.Water, westTilePosition);
					var westTileInNeighbouringCell = IsPositionInNeighbouringCell(westTilePosition);

					if (!tileAlreadyHasWater)
					{
						if (westTileInNeighbouringCell)
						{
							var targetCell = GetNeighbouringCellCoordinates(CellNeighbour.West);
							var newLocalCellTilePosition = new TileCoordinates(westTilePosition.X + GameWorld.WorldCellMapDimension, westTilePosition.Y);
							var newCellLakeTarget = new CellLakeTarget(targetCell, newLocalCellTilePosition);
							var distanceFromLakeSource = GetTileDistanceBetweenPoints(targetCell, newLocalCellTilePosition, lakeSource.CellCoordinates, lakeSource.CellStartPosition);

							if (distanceFromLakeSource < lakeBacktrackDistance)
								cellLakeTargetsToAdd.TryAdd(newCellLakeTarget.CellCoordinates, newCellLakeTarget);
						}

						else if (tilesToPaintWithWater[westTilePosition.X, westTilePosition.Y] != true)
						{
							var distanceFromLakeSource = GetTileDistanceBetweenPoints(CellCoordinates, westTilePosition, lakeSource.CellCoordinates, lakeSource.CellStartPosition);
							if (distanceFromLakeSource < lakeBacktrackDistance)
							{
								tilesAddedThisRound.Add(westTilePosition);
								tilesToPaintWithWater[westTilePosition.X, westTilePosition.Y] = true;
							}
						}
					}
				}
			}

			continueSearchingForTiles = tilesAddedThisRound.Count > 0;
		}

		foreach (var mapTile in MapTiles)
		{
			var x = mapTile.X;
			var y = mapTile.Y;

			var shouldPaintCell = tilesToPaintWithWater[x, y];

			if (shouldPaintCell)
			{
				MapTiles[x, y].WorldTile = WorldTile.Water;
				RemoveEntity(new TileCoordinates(x, y));
				//WorldCellRegions.UpdateRegionsForTile(MapTiles[x, y]);//RedrawMaps = true;
			}
		}

		cellsToGenerateLakesIn.AddCells(cellLakeTargetsToAdd.Values.ToArray());

		foreach (var newUncheckedCell in cellsToGenerateLakesIn.GetAllUncheckedCells())
		{
			Global.Instance.GameWorld.GetWorldCell(newUncheckedCell.CellCoordinates, startingCell)
				.GenerateLakeTiles(heightAtBacktrackPosition, lakeBacktrackDistance, lakeSource, ref cellsToGenerateLakesIn);
		}

		Data.SetupRegions();
		EventManager.RaiseEvent(Helpers.GlobalEventCodes.World_CellUpdated, this.CellCoordinates.ToString());
	}
	#endregion

	#region Cliffs
	private void GenerateCliffs()
	{
		var rockCells = Data.Regions_GetAllTilesOfTypes(true, WorldTile.RockTier2);
		List<TileCoordinates> cliffPositions = new List<TileCoordinates>();

		foreach (var rockTileCoordinates in rockCells)
		{
			//Try and ignore whether the position is in a neighbouring cell.
			//Check if the tile that exists at a local position is rock or a mountain. We don't need cliffs adjacent to mountains

			bool rockNeighbourNorth = IsPositionInNeighbouringCell(rockTileCoordinates.X, rockTileCoordinates.Y - 1) || AnyTileExistsAtLocalPosition(rockTileCoordinates.X, rockTileCoordinates.Y - 1, WorldTile.RockTier2, WorldTile.Mountain);
			bool rockNeighbourSouth = IsPositionInNeighbouringCell(rockTileCoordinates.X, rockTileCoordinates.Y + 1) || AnyTileExistsAtLocalPosition(rockTileCoordinates.X, rockTileCoordinates.Y + 1, WorldTile.RockTier2, WorldTile.Mountain);
			bool rockNeighbourEast = IsPositionInNeighbouringCell(rockTileCoordinates.X + 1, rockTileCoordinates.Y) || AnyTileExistsAtLocalPosition(rockTileCoordinates.X + 1, rockTileCoordinates.Y, WorldTile.RockTier2, WorldTile.Mountain);
			bool rockNeighbourWest = IsPositionInNeighbouringCell(rockTileCoordinates.X - 1, rockTileCoordinates.Y) || AnyTileExistsAtLocalPosition(rockTileCoordinates.X - 1, rockTileCoordinates.Y, WorldTile.RockTier2, WorldTile.Mountain);

			bool missingRockNeighbour = !rockNeighbourNorth || !rockNeighbourSouth || !rockNeighbourEast || !rockNeighbourWest;

			if (missingRockNeighbour)
				cliffPositions.Add(rockTileCoordinates);
		}

		foreach (var cliffPosition in cliffPositions)
		{
			if (GameWorld.RollRNG(GameWorld.MapGenCliffOpeningRoleChance))
				continue;

			MapTiles[cliffPosition.X, cliffPosition.Y].WorldTile = WorldTile.Mountain;
			RemoveEntity(cliffPosition);
		}

		Data.SetupRegions();
	}

	#endregion

	private int GetTileDistanceBetweenPoints(CellCoordinates cellA, TileCoordinates positionA, CellCoordinates cellB, TileCoordinates positionB)
	{
		var cellAOffsetX = cellA.X * GameWorld.WorldCellMapDimension;
		var cellAOffsetY = cellA.Y * GameWorld.WorldCellMapDimension;
		var cellBOffsetX = cellB.X * GameWorld.WorldCellMapDimension;
		var cellBOffsetY = cellB.Y * GameWorld.WorldCellMapDimension;

		var vectorA = new Vector2(positionA.X + cellAOffsetX, positionA.Y + cellAOffsetY);
		var vectorB = new Vector2(positionB.X + cellBOffsetX, positionB.Y + cellBOffsetY);
		var resultVector = vectorA.DistanceTo(vectorB);
		var roundedResult = Mathf.RoundToInt(resultVector);

		return roundedResult;
	}

	private float GetHeightMapValueForPosition(int currentCellPosX, int currentCellPosY)
	{
		//Debug.Print($"Getting height map value for ({currentCellPosX},{currentCellPosY}) on cell ({CellPosition.x},{CellPosition.y}) in seed {GameWorld.WorldSeed}");

		if (currentCellPosX < 0)
		{
			var targetCell = GetNeighbouringCell(CellNeighbour.West);
			return targetCell.GetHeightMapValueForPosition(currentCellPosX + GameWorld.WorldCellMapDimension, currentCellPosY);
		}
		else if (currentCellPosX >= GameWorld.WorldCellMapDimension)
		{
			var targetCell = GetNeighbouringCell(CellNeighbour.East);
			return targetCell.GetHeightMapValueForPosition(currentCellPosX - GameWorld.WorldCellMapDimension, currentCellPosY);
		}

		else if (currentCellPosY < 0)
		{
			var targetCell = GetNeighbouringCell(CellNeighbour.North);
			return targetCell.GetHeightMapValueForPosition(currentCellPosX, currentCellPosY + GameWorld.WorldCellMapDimension);
		}

		else if (currentCellPosY >= GameWorld.WorldCellMapDimension)
		{
			var targetCell = GetNeighbouringCell(CellNeighbour.South);
			return targetCell.GetHeightMapValueForPosition(currentCellPosX, currentCellPosY - GameWorld.WorldCellMapDimension);
		}
		else
		{
			return HeightMap[currentCellPosX, currentCellPosY];
		}
	}

	private float GetHeightMapValueForPosition(TileCoordinates currentCell)
	{
		return GetHeightMapValueForPosition(currentCell.X, currentCell.Y);
	}

	private bool TileExistsAtLocalPosition(WorldTile targetTile, int currentCellPosX, int currentCellPosY)
	{
		if (currentCellPosX < 0)
		{
			var targetCell = GetNeighbouringCell(CellNeighbour.West);
			return targetCell.TileExistsAtLocalPosition(targetTile, currentCellPosX + GameWorld.WorldCellMapDimension, currentCellPosY);
		}
		else if (currentCellPosX >= GameWorld.WorldCellMapDimension)
		{
			var targetCell = GetNeighbouringCell(CellNeighbour.East);
			return targetCell.TileExistsAtLocalPosition(targetTile, currentCellPosX - GameWorld.WorldCellMapDimension, currentCellPosY);
		}

		else if (currentCellPosY < 0)
		{
			var targetCell = GetNeighbouringCell(CellNeighbour.North);
			return targetCell.TileExistsAtLocalPosition(targetTile, currentCellPosX, currentCellPosY + GameWorld.WorldCellMapDimension);
		}

		else if (currentCellPosY >= GameWorld.WorldCellMapDimension)
		{
			var targetCell = GetNeighbouringCell(CellNeighbour.South);
			return targetCell.TileExistsAtLocalPosition(targetTile, currentCellPosX, currentCellPosY - GameWorld.WorldCellMapDimension);
		}
		else
		{
			return MapTiles[currentCellPosX, currentCellPosY].WorldTile == targetTile;
		}
	}

	private bool TileExistsAtLocalPosition(WorldTile targetTile, TileCoordinates position)
	{
		return TileExistsAtLocalPosition(targetTile, position.X, position.Y);
	}

	private bool AnyTileExistsAtLocalPosition(int currentCellPosX, int currentCellPosY, params WorldTile[] targetTiles)
	{
		foreach (var worldTile in targetTiles)
		{
			var worldTileExists = TileExistsAtLocalPosition(worldTile, currentCellPosX, currentCellPosY);
			if (worldTileExists)
				return true;
		}

		return false;
	}

	private void GenerateRiverInNeighbouringMap(TileCoordinates startPos, ref List<TileCoordinates> riverPositions)
	{
		if (startPos.X < 0)
		{
			var targetCell = GetNeighbouringCell(CellNeighbour.West);
			targetCell.GenerateRiverFromPosition(new TileCoordinates(startPos.X + GameWorld.WorldCellMapDimension, startPos.Y), ref riverPositions);
		}

		else if (startPos.X >= GameWorld.WorldCellMapDimension)
		{
			var targetCell = GetNeighbouringCell(CellNeighbour.East);
			targetCell.GenerateRiverFromPosition(new TileCoordinates(startPos.X - GameWorld.WorldCellMapDimension, startPos.Y), ref riverPositions);
		}

		else if (startPos.Y < 0)
		{
			var targetCell = GetNeighbouringCell(CellNeighbour.North);
			targetCell.GenerateRiverFromPosition(new TileCoordinates(startPos.X, startPos.Y + GameWorld.WorldCellMapDimension), ref riverPositions);
		}

		else if (startPos.Y >= GameWorld.WorldCellMapDimension)
		{
			var targetCell = GetNeighbouringCell(CellNeighbour.South);
			targetCell.GenerateRiverFromPosition(new TileCoordinates(startPos.X, startPos.Y - GameWorld.WorldCellMapDimension), ref riverPositions);
		}

		else
			throw new GameStateException("Couldn't decide on a neighbour to move to for World River Generation");
	}

	private bool IsPositionInNeighbouringCell(int currentCellPosX, int currentCellPosY)
	{
		if (currentCellPosX < 0 || currentCellPosX >= GameWorld.WorldCellMapDimension)
			return true;
		else if (currentCellPosY < 0 || currentCellPosY >= GameWorld.WorldCellMapDimension)
			return true;

		return false;
	}

	private bool IsPositionInNeighbouringCell(TileCoordinates currentCell)
	{
		return IsPositionInNeighbouringCell(currentCell.X, currentCell.Y);
	}

	private WorldCell GetNeighbouringCell(CellNeighbour cellNeighbour)
	{
		return Global.Instance.GameWorld.GetWorldCell(GetNeighbouringCellCoordinates(cellNeighbour), startingCell);
	}

	private CellCoordinates GetNeighbouringCellCoordinates(CellNeighbour cellNeighbour)
	{
		switch (cellNeighbour)
		{
			case CellNeighbour.North:
				return new CellCoordinates(CellCoordinates.X, CellCoordinates.Y - 1);
			case CellNeighbour.South:
				return new CellCoordinates(CellCoordinates.X, CellCoordinates.Y + 1);
			case CellNeighbour.East:
				return new CellCoordinates(CellCoordinates.X + 1, CellCoordinates.Y);
			case CellNeighbour.West:
				return new CellCoordinates(CellCoordinates.X - 1, CellCoordinates.Y);
			default:
				throw new GameStateException($"Couldn't get a neighbouring cell");
		}
	}

	internal class CellLakeDictionary
	{
		public ConcurrentDictionary<CellCoordinates, CellLakeTarget> CellsToGenerateLakesIn = new ConcurrentDictionary<CellCoordinates, CellLakeTarget>();

		public bool HasUncheckedCells()
		{
			foreach (var cell in CellsToGenerateLakesIn.Values)
			{
				if (!cell.Checked)
					return true;
			}

			return false;
		}

		public CellLakeTarget[] GetAllUncheckedCells()
		{
			var cells = new List<CellLakeTarget>();
			foreach (var cell in CellsToGenerateLakesIn.Values)
			{
				if (!cell.Checked)
					cells.Add(cell);
			}

			return cells.ToArray();
		}

		public CellLakeTarget GetCell(CellCoordinates cellCoordinates)
		{
			var success = CellsToGenerateLakesIn.TryGetValue(cellCoordinates, out var cell);

			if (success)
				return cell;
			else
				return null;
		}

		//Add a new target if it isn't in already.
		public void AddCell(CellLakeTarget cellLakeTarget)
		{
			CellsToGenerateLakesIn.TryAdd(cellLakeTarget.CellCoordinates, cellLakeTarget);
		}

		public void AddCells(CellLakeTarget[] cellLakeTargets)
		{
			foreach (var item in cellLakeTargets)
			{
				AddCell(item);
			}
		}
	}

	internal class CellLakeTarget
	{
		public bool Checked;
		public CellCoordinates CellCoordinates;
		public TileCoordinates CellStartPosition;

		public CellLakeTarget(CellCoordinates cellCoordinates, TileCoordinates cellStartPosition)
		{
			Checked = false;
			CellCoordinates = cellCoordinates;
			CellStartPosition = cellStartPosition;
		}
	}
}

public enum CellNeighbour
{
	North,
	South,
	East,
	West
}
