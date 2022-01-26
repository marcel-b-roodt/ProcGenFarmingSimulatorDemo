using Godot;

public class MapView : Node2D
{
	private Node2D mapWorldCells;
	private Node2D mapFeatureCells;
	private Node2D mapEntityCells;
	private Node2D mapShroudCells;
	private Node2D playerMarker;
	private Label playerName;

	//private Dictionary<string, Node2D> TownMarkers = new Dictionary<string, Node2D>();

	public override void _Ready()
	{
		mapWorldCells = GetNode<Node2D>("MapWorldCells");
		mapFeatureCells = GetNode<Node2D>("MapFeatureCells");
		mapEntityCells = GetNode<Node2D>("MapEntityCells");
		mapShroudCells = GetNode<Node2D>("MapShroudCells");
		playerMarker = GetNode<Node2D>("Markers/PlayerMarker");
		playerName = GetNode<Label>("Markers/PlayerMarker/PlayerName");

		mapWorldCells.Position = new Vector2(GameWorld.WorldCellMapDimension / 2, GameWorld.WorldCellMapDimension / 2);
		mapFeatureCells.Position = new Vector2(GameWorld.WorldCellMapDimension / 2, GameWorld.WorldCellMapDimension / 2);
		mapEntityCells.Position = new Vector2(GameWorld.WorldCellMapDimension / 2, GameWorld.WorldCellMapDimension / 2);
		mapShroudCells.Position = new Vector2(GameWorld.WorldCellMapDimension / 2, GameWorld.WorldCellMapDimension / 2);
		playerName.Text = Global.Instance.PlayerData.Name;
	}

	public override void _Process(float delta)
	{
		if (Visible)
		{
			var currentPlayerPosition = Global.Instance.PlayerData.Position;
			var playerMapPosition = currentPlayerPosition / GameWorld.WorldCellTileSize;
			playerMarker.Position = playerMapPosition;
		}
	}
}
