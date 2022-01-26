using Godot;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CSharpThread = System.Threading.Thread;

public class GameWorld : Node2D, IDisposable
{
	public const int WorldCellTileSize = 32; //32px x 32px
	public const int WorldCellMapDimension = 50; //50 tiles x 50 tiles
	public const int WorldCellScaleConstant = WorldCellTileSize * WorldCellMapDimension;
	public const int MapCellImageSize = WorldCellMapDimension;

	public static GameWorld Instance { get; private set; }

	public static Dictionary<WorldTile, TileMap> WorldTileDictionary { get { return Instance.worldTileDictionary; } }
	public static TileMap FeatureTileMap { get { return Instance.featureTileMap; } }
	public static Node2D Messages { get { return Instance.messages; } }
	public static RandomNumberGenerator RNG { get { return Instance.rng; } }

	public Camera2D MapViewCamera;
	public Viewport MapViewport;

	public CellCoordinates ActiveCell { get; private set; } = new CellCoordinates(int.MaxValue, int.MaxValue);
	public WorldCell ActiveWorldCell { get { return WorldCells[ActiveCell.ToString()]; } }

	private Dictionary<WorldTile, TileMap> worldTileDictionary = new Dictionary<WorldTile, TileMap>();
	private TileMap featureTileMap;
	private Node2D messages;
	private RandomNumberGenerator rng;

	private ConcurrentDictionary<string, WorldCell> WorldCells = new ConcurrentDictionary<string, WorldCell>();
	private ConcurrentDictionary<string, TextureRect> MapWorldCells = new ConcurrentDictionary<string, TextureRect>();
	private ConcurrentDictionary<string, TextureRect> MapFeatureCells = new ConcurrentDictionary<string, TextureRect>();
	private ConcurrentDictionary<string, TextureRect> MapEntityCells = new ConcurrentDictionary<string, TextureRect>();


	private List<CellCoordinates> activeCells = new List<CellCoordinates>();
	private ConcurrentDictionary<string, WorldCell> cellsNeedingUpdate = new ConcurrentDictionary<string, WorldCell>();
	private CSharpThread cellInitialisationThread;

	#region Player
	private bool playerSpawned = false;
	//public static Vector2? PlayerSpawnPosition = null; //Change this on game save and load it. Otherwise initialise it to (0,0)
	//public bool PlayerHasSpawned;
	#endregion

	#region Entities
	public Dictionary<ulong, ulong> entityWorldCellMappings = new Dictionary<ulong, ulong>();
	#endregion

	public static uint WorldSeed = 0;
	public static int MapGen_Terrain_Scale = 250;
	public static int MapGen_Terrain_Octaves = 4;
	public static float MapGen_Terrain_ValueScale = 0.75f;
	public static float MapGen_Terrain_Persistence = 0.75f;
	public static float MapGen_Terrain_Lacunarity = 2.5f;

	public static int MapGen_Moisture_Scale = 250;
	public static int MapGen_Moisture_Octaves = 4;
	public static float MapGen_Moisture_ValueScale = 0.75f;
	public static float MapGen_Moisture_Persistence = 0.75f;
	public static float MapGen_Moisture_Lacunarity = 2.5f;

	//TODO: Create a catalogue of static Biome classes to their Generation properties
	//TODO: Add these river properties to the biome collection
	//TODO: Randomly generate the number of rivers. 
	//TODO: The number of water tiles as well as neighbouring cells should determine how moist a place is
	//TODO: Heightmap general values should determine whether it's rockier or not
	public static int MapGenMinRiverDistanceLakeSizeRatio = 6; //Higher = chance for larger lakes 
	public static int MapGenMaxRiverDistanceLakeSizeRatio = 3; //Higher = chance for smaller lakes 

	public static int MapGenCliffOpeningRoleChance = Mathf.Clamp(6, 0, 100); //Chance that a cliff opening is created

	private static Vector2 lastTileUpdatePlayerCellPosition = new Vector2(int.MaxValue, int.MaxValue);
	private static int tileUpdateDistance = 5;
	private static int tileCellRenderRange;

	public static Vector2 PlayerPositionTileRenderBounds_Start { get { return new Vector2(lastTileUpdatePlayerCellPosition.x - PlayerEntity.TileRenderRange, lastTileUpdatePlayerCellPosition.y + PlayerEntity.TileRenderRange); } }
	public static Vector2 PlayerPositionTileRenderBounds_End { get { return new Vector2(lastTileUpdatePlayerCellPosition.x + PlayerEntity.TileRenderRange, lastTileUpdatePlayerCellPosition.y - PlayerEntity.TileRenderRange); } }

	private Node2D mapWorldCells;
	private Node2D mapFeatureCells;
	private Node2D mapEntityCells;
	private Node2D worldCells;
	private YSort activeEntities;
	private CanvasModulate worldModulate;

	//TODO: Make Global TimeOfDay setting based on the GameTime. Trigger events for when it moves over to Dawn, Noon, Dusk and Midnight. Also probably have a global lighting value.
	private float timeSinceRefreshedColour;
	private float refreshColourTime = 0.1f;

	public static bool RollRNG(float successChance)
	{
		var roll = RNG.RandiRange(0, 100);
		return roll >= (100 - Mathf.Clamp(successChance, 0, 100));
	}

	private static void InitialiseWorldSeed()
	{
		GD.Randomize();
		//WorldSeed = GD.Randi();
		WorldSeed = (2918626301); //Weird Rivers and Lakes world
								  //Nice Seeds for testing
								  //WorldSeed = (uint)(830585070 + uint.MaxValue);
								  //Past Problem Seeds
								  //WorldSeed = (uint)(-186079397 + uint.MaxValue);
								  //WorldSeed = (uint)(-695082111 + uint.MaxValue);

		//WorldSeed = (uint)(-83034975 + uint.MaxValue); //River World
		Noise.ResetNoise();
		Instance.rng = new RandomNumberGenerator();
		Instance.rng.Seed = WorldSeed;
		Instance.rng.Randomize();
		Debug.Print($"Game Started. WorldSeed: {WorldSeed}");
	}

	public override void _Ready()
	{
		base._Ready();

		Instance = this;
		Global.Instance.GameWorld = this;
		InitialiseWorldSeed();

		MapViewCamera = GetNode<Camera2D>("MapViewport/MapView/MapViewCamera");
		MapViewport = GetNode<Viewport>("MapViewport");

		mapWorldCells = GetNode<Node2D>("MapViewport/MapView/MapWorldCells");
		mapFeatureCells = GetNode<Node2D>("MapViewport/MapView/MapFeatureCells");
		mapEntityCells = GetNode<Node2D>("MapViewport/MapView/MapEntityCells");

		WorldTileDictionary.Add(WorldTile.Water, GetNode<TileMap>("Environment/WorldTiles/Water"));
		WorldTileDictionary.Add(WorldTile.Sand, GetNode<TileMap>("Environment/WorldTiles/Sand"));
		WorldTileDictionary.Add(WorldTile.Plains, GetNode<TileMap>("Environment/WorldTiles/Plains"));
		WorldTileDictionary.Add(WorldTile.Grass, GetNode<TileMap>("Environment/WorldTiles/Grass"));
		WorldTileDictionary.Add(WorldTile.RockTier1, GetNode<TileMap>("Environment/WorldTiles/LowRock"));
		WorldTileDictionary.Add(WorldTile.RockTier2, GetNode<TileMap>("Environment/WorldTiles/HighRock"));
		WorldTileDictionary.Add(WorldTile.Cliff, GetNode<TileMap>("Environment/WorldTiles/Cliffs"));
		WorldTileDictionary.Add(WorldTile.Mountain, GetNode<TileMap>("Environment/WorldTiles/Mountains"));
		WorldTileDictionary.Add(WorldTile.River, GetNode<TileMap>("Environment/WorldTiles/Rivers"));

		Instance.featureTileMap = GetNode<TileMap>("Environment/FeatureTiles");
		Instance.messages = GetNode<Node2D>("Messages");
		worldCells = GetNode<Node2D>("WorldCells");
		activeEntities = GetNode<YSort>("ActiveEntities");
		worldModulate = GetNode<CanvasModulate>("WorldModulate");

		EventManager.ListenEvent(Helpers.GlobalEventCodes.World_CellUpdated, GD.FuncRef(this, nameof(UpdateCellMapImagesfromEvent)));

		TriggerPlayerSpawn();

		cellInitialisationThread = new CSharpThread(new ThreadStart(UpdateMapCellWorlds));
		cellInitialisationThread.Start();
	}

	public override void _Process(float delta)
	{
		base._Process(delta);

		if (Global.Instance.GameInProgress && !Global.Instance.GamePaused && playerSpawned)
		{
			UpdateGameTime(delta);

			var playerPosition = Global.Instance.PlayerData.Position;
			RenderWorldCellTiles(playerPosition);

			var targetCell = GetCoordinatesFromPosition(playerPosition, out _);
			MarkActiveCell(targetCell, false);
		}
	}

	public static bool MustRenderTileInPosition(Vector2 position)
	{
		return (position.DistanceTo(lastTileUpdatePlayerCellPosition) <= tileCellRenderRange);
	}

	public static bool TileOrSurroundsNotDrawn(WorldTile worldTile, Vector2 tilePosition)
	{
		int xPos = (int)tilePosition.x;
		int yPos = (int)tilePosition.y;

		for (int i = yPos - 1; i < yPos + 1; i++)
		{
			for (int j = xPos - 1; i < xPos + 1; j++)
			{
				if (WorldTileDictionary[worldTile].GetCellv(new Vector2(xPos, yPos)) < 0)
					return true;
			}
		}

		return false;
	}

	public static bool TileOrSurroundsIsDrawn(WorldTile worldTile, Vector2 tilePosition)
	{
		int xPos = (int)tilePosition.x;
		int yPos = (int)tilePosition.y;

		for (int i = yPos - 1; i < yPos + 1; i++)
		{
			for (int j = xPos - 1; i < xPos + 1; j++)
			{
				if (WorldTileDictionary[worldTile].GetCellv(new Vector2(xPos, yPos)) >= 0)
					return true;
			}
		}

		return false;
	}

	#region Positioning
	public static CellCoordinates GetCoordinatesFromPosition(Vector2 position, out TileCoordinates tileCoordinates)
	{
		float cellPosX = Mathf.Floor(position.x / WorldCellScaleConstant);
		float cellPosY = Mathf.Floor(position.y / WorldCellScaleConstant);
		float tilePosX = (position.x / WorldCellTileSize) % WorldCellMapDimension;
		float tilePosY = (position.y / WorldCellTileSize) % WorldCellMapDimension;

		//TODO: Make sure edge cases work fine with this method
		if (tilePosX >= WorldCellMapDimension)
			tilePosX -= WorldCellMapDimension;
		if (tilePosX < 0)
			tilePosX += WorldCellMapDimension;

		if (tilePosY >= WorldCellMapDimension)
			tilePosY -= WorldCellMapDimension;
		if (tilePosY < 0)
			tilePosY += WorldCellMapDimension;

		//Debug.Print($"CellX {cellPosX} CellY {cellPosY} TileX {tilePosX} TileY {tilePosY}");
		tileCoordinates = new TileCoordinates((int)tilePosX, (int)tilePosY);
		return new CellCoordinates((int)cellPosX, (int)cellPosY);
	}

	public static Vector2 GetPositionFromCoordinates(CellCoordinates cellCoordinates, TileCoordinates tileCoordinates)
	{
		var posX = cellCoordinates.X * WorldCellScaleConstant + tileCoordinates.X * WorldCellTileSize;
		var posY = cellCoordinates.Y * WorldCellScaleConstant + tileCoordinates.Y * WorldCellTileSize;

		return new Vector2(posX, posY);
	}

	public static Vector2 GetPositionAsCellPosition(Vector2 position)
	{
		var tilePosition = new Vector2((int)(position.x / WorldCellTileSize), (int)(position.y / WorldCellTileSize));
		var globalPosition = tilePosition * WorldCellTileSize + new Vector2(WorldCellTileSize / 2, (Mathf.Sign(position.y) * WorldCellTileSize / 2));
		return globalPosition;
	}
	#endregion

	public WorldCell GetWorldCell(CellCoordinates cellCoordinates, bool startingCell = false)
	{
		if (!WorldCells.ContainsKey(cellCoordinates.ToString()))
		{
			//Debug.Print($"Creating new cell for coordinates {cellCoordinatesString}");
			AddWorldCell(cellCoordinates, startingCell);
		}

		return WorldCells[cellCoordinates.ToString()];
	}

	public WorldCell GetWorldCellFromPosition(Vector2 position, out TileCoordinates tileCoordinates)
	{
		var cellCoordinates = GetCoordinatesFromPosition(position, out tileCoordinates);
		return GetWorldCell(cellCoordinates);
	}

	private void AddWorldCell(CellCoordinates cellCoordinates, bool startingCell)
	{
		var cellCoordinatesString = cellCoordinates.ToString();

		var worldCell = SceneStore.Instantiate<WorldCell>();
		//Debug.Print($"{cellCoordinatesString}: Instance Created");

		var mapCellPosition = new Vector2(
			cellCoordinates.X * MapCellImageSize - MapCellImageSize / 2,
			cellCoordinates.Y * MapCellImageSize - MapCellImageSize / 2
			);

		var mapCellWorldImage = new TextureRect();
		mapCellWorldImage.Expand = true;
		mapCellWorldImage.SetSize(new Vector2(MapCellImageSize, MapCellImageSize));
		mapCellWorldImage.RectGlobalPosition = mapCellPosition;
		mapCellWorldImage.SetProcess(false);

		var mapWorldCellFeatureImage = new TextureRect();
		mapWorldCellFeatureImage.Expand = true;
		mapWorldCellFeatureImage.SetSize(new Vector2(MapCellImageSize, MapCellImageSize));
		mapWorldCellFeatureImage.RectGlobalPosition = mapCellPosition;
		mapWorldCellFeatureImage.SetProcess(false);

		var mapWorldCellEntityImage = new TextureRect();
		mapWorldCellEntityImage.Expand = true;
		mapWorldCellEntityImage.SetSize(new Vector2(MapCellImageSize, MapCellImageSize));
		mapWorldCellEntityImage.RectGlobalPosition = mapCellPosition;
		mapWorldCellEntityImage.SetProcess(false);

		WorldCells.TryAdd(cellCoordinatesString, worldCell);
		MapWorldCells.TryAdd(cellCoordinatesString, mapCellWorldImage);
		MapFeatureCells.TryAdd(cellCoordinatesString, mapWorldCellFeatureImage);
		MapEntityCells.TryAdd(cellCoordinatesString, mapWorldCellEntityImage);

		//Debug.Print($"{cellCoordinatesString}: Preparing to Initialise Cell");
		worldCell.InitialiseCell(cellCoordinates, startingCell);

		if (startingCell)
		{
			worldCell.UpdateCellTiles();
			worldCell.PlaceTileEntities();
		}
		else
			cellsNeedingUpdate.GetOrAdd(cellCoordinatesString, worldCell);

		mapWorldCells.AddChild(mapCellWorldImage);
		mapFeatureCells.AddChild(mapWorldCellFeatureImage);
		mapEntityCells.AddChild(mapWorldCellEntityImage);
		worldCells.AddChild(worldCell);
	}

	private void UpdateMapCellWorlds()
	{
		List<CellCoordinates> cellsToRedrawMap = new List<CellCoordinates>();

		while (true)
		{
			cellsToRedrawMap.Clear();
			while (cellsNeedingUpdate.Count > 0)
			{
				foreach (var cellCoordinates in cellsNeedingUpdate.Keys)
				{
					var worldCell = WorldCells[cellCoordinates];

					if (!(worldCell.WorldTilesInitialised && worldCell.FeatureTilesInitialised && worldCell.EntityTilesInitialised))
					{
						worldCell.UpdateCellTiles();
						CallDeferred(nameof(UpdateCellMapImages), cellCoordinates);
						//worldCell.CallDeferred(nameof(worldCell.PlaceTileEntities));
					}
				}
			}

			CSharpThread.Sleep(500);
		}
	}

	private void UpdateCellMapImagesfromEvent(string cellCoordinates)
	{
		Debug.Print($"Updating cell {cellCoordinates} from event");
		WorldCells.TryGetValue(cellCoordinates, out var cell);
		cellsNeedingUpdate.TryAdd(cellCoordinates, cell);
	}

	private void UpdateCellMapImages(string cellCoordinates)
	{
		WorldCells[cellCoordinates].PlaceTileEntities();
		cellsNeedingUpdate.TryRemove(cellCoordinates, out _);
		//Debug.Print($"Cell {cellCoordinates} was updated and removed from Cells Needing Initialisation");
	}

	//TODO: Move this and all tile renders to a Render queue thread
	private void RenderWorldCellTiles(Vector2 playerPosition)
	{
		var playerCellPosition = new Vector2(playerPosition.x / WorldCellTileSize, playerPosition.y / WorldCellTileSize);

		if (playerCellPosition.DistanceTo(lastTileUpdatePlayerCellPosition) >= tileUpdateDistance)
		{
			lastTileUpdatePlayerCellPosition = playerCellPosition;

			foreach (var activeCell in activeCells)
			{
				var worldCell = WorldCells[activeCell.ToString()];
				UpdateWorldCellMaps(worldCell);
			}
		}
	}

	private void UpdateGameTime(float delta)
	{
		if (Global.Instance.GameInProgress)
		{
			//Debug.Print($"Updating time");

			var currentDay = Global.Instance.GameTime.Day;
			//Debug.Print($"GameTime {Global.Instance.GameTime}. Time passed: {delta}");
			Global.Instance.GameTime = Global.Instance.GameTime.AddTime(delta);

			if (Global.Instance.GameTime.Day > currentDay)
			{
				EventManager.RaiseEvent(nameof(Helpers.GlobalEventCodes.Game_DayUpdated));
			}

			if (timeSinceRefreshedColour >= refreshColourTime)
			{
				timeSinceRefreshedColour -= refreshColourTime;
				//Debug.Print($"Refreshing colour: Current {worldModulate.Color}");
				worldModulate.Color = Global.Instance.GameTime.GetTimeOfDayColour();
				//Debug.Print($"Refreshed colour: New {worldModulate.Color}");
			}

			timeSinceRefreshedColour += delta;
			//TODO: Speed up transition time so that we have more time at the correct lighting. This is what we fixed in Archetype. e.g. hit midnight darkness before midnight, and stay there for a bit.
			//Up the lerp multiplier a bit so that we hit 1 for some time
			//TODO: Add an Advance Time Debug method
			//TODO: Make the player place the crops
		}
	}

	private void MarkActiveCell(CellCoordinates cellCoordinates, bool startingCell)
	{
		if (cellCoordinates == ActiveCell)
		{
			//Debug.Print("Current Active Cell. We are not doing anything");
			return;
		}

		var playerPosition = Global.Instance.PlayerData.Position;
		var playerCellPosition = new Vector2(Mathf.RoundToInt((playerPosition.x / WorldCellScaleConstant) - 0.5f), Mathf.RoundToInt((playerPosition.y / WorldCellScaleConstant) - 0.5f));
		Debug.Print($"Marking active cell ({cellCoordinates.X},{cellCoordinates.Y}). ",
			$"Player position is ({playerPosition.x},{playerPosition.y}). ",
			$"Player cell position is ({playerCellPosition.x},{playerCellPosition.y})",
			$"Player local cell position is ({playerCellPosition.x / cellCoordinates.X},{playerCellPosition.y / cellCoordinates.Y})");

		ActiveCell = cellCoordinates;
		List<CellCoordinates> newActiveCells = new List<CellCoordinates>();

		for (int y = cellCoordinates.Y - 1; y <= cellCoordinates.Y + 1; y++)
		{
			for (int x = cellCoordinates.X - 1; x <= cellCoordinates.X + 1; x++)
			{
				newActiveCells.Add(new CellCoordinates(x, y));
			}
		}

		if (startingCell)
		{
			foreach (var newActiveCell in newActiveCells)
			{
				//Debug.Print($"Activating Starting Cell {newActiveCell}");
				GetWorldCell(newActiveCell, true);
				WorldCells[newActiveCell.ToString()].Activate();
				activeCells.Add(newActiveCell);
			}
		}
		else
		{
			List<CellCoordinates> previousActiveCells = new List<CellCoordinates>();

			foreach (var activeCell in activeCells)
			{
				previousActiveCells.Add(activeCell);
			}

			foreach (var cell in newActiveCells)
			{
				if (!activeCells.Contains(cell))
				{
					//Debug.Print($"Activating Cell {cell}");
					GetWorldCell(cell, false);
					WorldCells[cell.ToString()].Activate();
					activeCells.Add(cell);
				}
			}

			foreach (var cell in previousActiveCells)
			{
				if (!newActiveCells.Contains(cell))
				{
					//Debug.Print($"Deactivating Cell {cell}");
					GetWorldCell(cell, false);
					WorldCells[cell.ToString()].Deactivate();
					activeCells.Remove(cell);
				}
			}
		}
	}

	private void TriggerPlayerSpawn()
	{
		List<CellCoordinates> checkedCells = new List<CellCoordinates>();
		CellCoordinates currentCell = new CellCoordinates(0, 0);
		MarkActiveCell(currentCell, true);

		var validPlayerSpawnTiles = new List<TileCoordinates>();
		WorldCell cell = null;

		while (validPlayerSpawnTiles.Count == 0)
		{
			currentCell = activeCells.First(cellCoords => !checkedCells.Contains(cellCoords));
			checkedCells.Add(currentCell);
			cell = GetWorldCell(currentCell, true);
			validPlayerSpawnTiles = cell.Data.Regions_GetAllTilesOfTypes(true, WorldCell.TraversableTileTypes);
		}

		MarkActiveCell(currentCell, true);
		UpdateWorldCellMaps(cell);

		int targetSpawnTileIndex = RNG.RandiRange(0, validPlayerSpawnTiles.Count - 1);

		var playerTile = validPlayerSpawnTiles[targetSpawnTileIndex];
		var playerXPos = (cell.CellCoordinates.X * WorldCellMapDimension + playerTile.X) * WorldCellTileSize;
		var playerYPos = (cell.CellCoordinates.Y * WorldCellMapDimension + playerTile.Y) * WorldCellTileSize;

		TileCoordinates signboardTile = new TileCoordinates();
		for (int y = -3; y < 3; y++)
		{
			if (y == 0) continue;

			for (int x = -3; x < 3; x++)
			{
				if (x == 0) continue;

				TileCoordinates neighbouringTile = new TileCoordinates(playerTile.X + x, playerTile.Y + y);
				if (validPlayerSpawnTiles.Contains(neighbouringTile))
				{
					Debug.Print($"Found a neighbouring tile for the Signboard. Placing it here: ({signboardTile})");
					signboardTile = neighbouringTile;
				}
			}
		}

		DeactivateAllEntityNodes();

		EntityManager.SpawnEntityInPosition<PlayerEntity, PlayerData>(new Vector2(playerXPos, playerYPos), out PlayerEntity playerInstance);
		playerInstance.GlobalPosition = GetPositionAsCellPosition(new Vector2(playerXPos, playerYPos));
		playerInstance.GetParent().RemoveChild(playerInstance);
		activeEntities.AddChild(playerInstance);

		tileCellRenderRange = (int)(PlayerEntity.TileRenderRange / WorldCellTileSize);

		EntityManager.SpawnTileEntityInPosition<SignboardEntity, SignboardData>(currentCell, signboardTile, out SignboardEntity signboardInstance);
		signboardInstance.InitialiseText(Strings.Dialogue.HelpDialogueText);
		//signboardInstance.GlobalPosition = new Vector2(currentCell, signboardTile);
		signboardInstance.GetParent().RemoveChild(signboardInstance);
		activeEntities.AddChild(signboardInstance);

		ActivateAllEntityNodesNearPlayer();

		playerSpawned = true;
	}

	private void UpdateWorldCellMaps(WorldCell worldCell)
	{
		var cellCoordinates = worldCell.CellCoordinates.ToString();
		worldCell.UpdateTileMaps();
		MapWorldCells[cellCoordinates].Texture = worldCell.MapWorldTilesImage;
		MapFeatureCells[cellCoordinates].Texture = worldCell.MapFeatureTilesImage;
		MapEntityCells[cellCoordinates].Texture = worldCell.MapEntityTilesImage;
	}

	private void DeactivateAllEntityNodes()
	{
		foreach (Node2D entityNode in activeEntities.GetChildren())
		{
			if (entityNode is ITileEntity)
			{
				DeactivateEntityNode(entityNode);
			}
		}
	}

	private void ActivateAllEntityNodesNearPlayer()
	{
		var entityNodesNearPlayer = Global.Instance.Player.GetAllEntityNodesInRange();
		foreach (Node2D entityNode in entityNodesNearPlayer)
		{
			ActivateEntityNode(entityNode);
		}
	}

	public void ActivateEntityNode(Node2D entityNode)
	{
		if (entityNode is PlayerEntity)
			return;

		IEntity entity = (IEntity)entityNode;
		entity.ActivateEntity();

		var parent = entityNode.GetParent();
		var parentNodeID = parent.GetInstanceId();
		var entityNodeID = entityNode.GetInstanceId();
		if (!entityWorldCellMappings.ContainsKey(entityNodeID) && parent != activeEntities)
		{
			entityWorldCellMappings.Add(entityNodeID, parentNodeID);
			CallDeferred(nameof(ReparentEntityToActiveEntities), entityNode);
			//Debug.Print($"Activating entity ID:{entityNodeID}; Parent Name: {entityNode.GetParent().Name}");
		}
	}

	public void DeactivateEntityNode(Node2D entityNode)
	{
		if (entityNode is PlayerEntity)
			return;

		IEntity entity = (IEntity)entityNode;
		entity.DeactivateEntity();

		var parent = entityNode.GetParent();
		var entityNodeID = entityNode.GetInstanceId();
		if (entityWorldCellMappings.ContainsKey(entityNodeID) && parent == activeEntities)
		{
			ulong parentNodeID = entityWorldCellMappings[entityNodeID];
			entityWorldCellMappings.Remove(entityNodeID);
			CallDeferred(nameof(ReparentEntityToOriginalParent), parentNodeID, entityNode);
			//Debug.Print($"Deactivating entity ID:{entityNodeID}; Parent Name: {entityNode.GetParent().Name}");
		}
	}

	private void ReparentEntityToActiveEntities(Node2D entityNode)
	{
		if (!Global.Instance.GameInProgress)
			return;

		var parent = entityNode.GetParent();
		parent.RemoveChild(entityNode);
		activeEntities.AddChild(entityNode);
		entityNode.Visible = true;
		entityNode.SetProcess(true);
	}

	private void ReparentEntityToOriginalParent(ulong parentNodeID, Node2D entityNode)
	{
		if (!Global.Instance.GameInProgress)
			return;

		if (entityNode == null)
		{
			Debug.Print($"Cannot reparent Entity {parentNodeID}. It is null.");
			return;
		}

		activeEntities.RemoveChild(entityNode);
		var parentNode = (Node2D)GD.InstanceFromId(parentNodeID);
		parentNode.AddChild(entityNode);
		entityNode.Visible = false;
		entityNode.SetProcess(false);
	}

	//public void SpawnEntity(Vector2 position, IEntity entity)
	//{
	//	var worldCell = GetWorldCellByPosition(position);
	//	worldCell.AddEntity(entity);
	//}

	public void SpawnPickup(Vector2 position, InventoryItem item, int quantity)
	{
		if (item.Empty)
			return;

		var pickup = SceneStore.Instantiate<ItemPickupEntity>();
		pickup.SetItem(position, item, quantity);

		if ((Global.Instance.PlayerData.Position - position).Length() <= PlayerEntity.ItemCollectionRange)
			pickup.PickedUp = true;

		var worldCell = GetWorldCellFromPosition(position, out _);
		worldCell.AddPickup(pickup);
		ActivateEntityNode(pickup);
	}

	#region Signals
	#endregion

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);

		if (cellInitialisationThread != null && cellInitialisationThread.IsAlive)
			cellInitialisationThread.Abort();
	}
}
