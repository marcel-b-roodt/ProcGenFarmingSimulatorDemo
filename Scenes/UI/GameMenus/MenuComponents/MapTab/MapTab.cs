using Godot;
using static Helpers;

public class MapTab : InGameMenu
{
	public int CameraSpeed { get; private set; } = 50;

	public Vector2[] ZoomLevels = new Vector2[] { new Vector2(0.3f, 0.3f),
													new Vector2(0.38f, 0.38f),
						new Vector2(0.48f, 0.48f),
	new Vector2(0.6f, 0.6f),
	new Vector2(0.7f, 0.7f), };
	public int ZoomIndex { get { return zoomLevel; } private set { zoomLevel = Mathf.Clamp(value, 0, 4); } }
	private int zoomLevel = 4;

	public Vector2 CameraMoveDirection { get { return cameraMoveDirection; } }

	private Vector2 cameraMoveDirection = new Vector2();
	private bool cameraChasePlayer = true;

	private SceneTree sceneTree;

	private Camera2D MapViewCamera
	{
		get
		{
			if (mapViewCamera == null)
			{
				mapViewCamera = Global.Instance.GameWorld.MapViewCamera;
			}

			return mapViewCamera;
		}
	}
	private Camera2D mapViewCamera;

	private Viewport MapViewport
	{
		get
		{
			if (mapViewport == null)
			{
				mapViewport = Global.Instance.GameWorld.MapViewport;
				mapViewport.Size = mapRenderBox.RectSize;
			}

			return mapViewport;
		}
	}

	private MainMenu mainMenu;
	private Viewport mapViewport;
	private TextureRect mapRenderBox;

	public override void _Ready()
	{
		mainMenu = (MainMenu)FindParent("MainMenu");
		mapRenderBox = GetNode<TextureRect>("VB/BackgroundRect/MapRenderBox");

		MapViewCamera.Zoom = ZoomLevels[ZoomIndex];
	}

	public override void _EnterTree()
	{
		base._EnterTree();

		sceneTree = GetTree();
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (mainMenu.MapActive)
		{
			if (Input.IsActionJustPressed(PlayerInputCodes.A) && @event.IsAction(PlayerInputCodes.A) && !sceneTree.IsInputHandled())
			{
				ZoomIndex--;
				MapViewCamera.Zoom = ZoomLevels[ZoomIndex];
			}

			if (Input.IsActionJustPressed(PlayerInputCodes.B) && @event.IsAction(PlayerInputCodes.B) && !sceneTree.IsInputHandled())
			{
				ZoomIndex++;
				MapViewCamera.Zoom = ZoomLevels[ZoomIndex];
			}

			if (Input.IsActionJustPressed(PlayerInputCodes.X) && @event.IsAction(PlayerInputCodes.X) && !sceneTree.IsInputHandled())
			{
				cameraChasePlayer = true;
			}
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		if (mainMenu.MapActive)
		{
			UpdateMovementInput();

			if (cameraChasePlayer)
			{
				PositionCamera();
			}
			else
			{
				var velocity = cameraMoveDirection.Normalized() * CameraSpeed;
				MapViewCamera.Position = MapViewCamera.Position + velocity * delta;
			}
		}

		base._PhysicsProcess(delta);
	}

	public override void _Process(float delta)
	{
		if (mainMenu.MapActive)
		{
			mapRenderBox.Texture = MapViewport.GetTexture();
		}

		base._Process(delta);
	}

	public new void Show()
	{
		ResetScreen();
	}

	public new void Hide() { }

	private void UpdateMovementInput()
	{
		cameraMoveDirection.x = 0;
		cameraMoveDirection.y = 0;

		if (mainMenu.MapActive)
		{
			if (Input.IsActionPressed(PlayerInputCodes.Up))
			{
				cameraMoveDirection.y += -1;
				cameraChasePlayer = false;
			}

			if (Input.IsActionPressed(PlayerInputCodes.Down))
			{
				cameraMoveDirection.y += 1;
				cameraChasePlayer = false;
			}

			if (Input.IsActionPressed(PlayerInputCodes.Right))
			{
				cameraMoveDirection.x += 1;
				cameraChasePlayer = false;
			}

			if (Input.IsActionPressed(PlayerInputCodes.Left))
			{
				cameraMoveDirection.x -= 1;
				cameraChasePlayer = false;
			}
		}
	}

	private void ResetScreen()
	{
		PositionCamera();
		cameraChasePlayer = true;
		//TODO: Reset Zoom Level
	}

	private void PositionCamera()
	{
		float scaleConstant = 1f / (float)GameWorld.WorldCellTileSize;

		var playerCellPosX = Global.Instance.PlayerData.Position.x * scaleConstant;
		var playerCellPosY = Global.Instance.PlayerData.Position.y * scaleConstant;

		//https://godotengine.org/qa/6815/scaling-the-viewport-manually-2d
		MapViewCamera.Position = new Vector2(playerCellPosX, playerCellPosY);
	}
}
