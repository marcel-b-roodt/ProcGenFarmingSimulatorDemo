using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using static Helpers;

public class Global : Node2D
{
	public static Global Instance { get; private set; }
	public PlayerData PlayerData { get; private set; }
	public GameData GameData { get; set; }

	public PlayerEntity Player { get; set; }
	public PlayerUI PlayerUI { get; set; }
	public GameWorld GameWorld { get; set; }

	public SaveSlot CurrentSaveSlot { get; set; }
	public bool DebugFlag { get; set; }
	public bool DebugMenuFlag { get; set; }

	public bool GameInProgress { get { return CurrentSaveSlot != SaveSlot.Unset && Player != null && GameWorld != null && !GamePaused; } } //In-game instances not active and Player Save Slot not set
	public bool GamePaused { get { return PlayerUI?.MenuOpen ?? true; } }

	public GamepadType ActiveInputType { get; private set; }

	private Dictionary<int, GamepadType> connectedGamepads = new Dictionary<int, GamepadType>();
	private SceneTree sceneTree;

	public GameDateTime GameTime
	{
		get { return GameData.GameTime; }
		set { GameData.GameTime = value; }
	}

	public Global()
	{
		Instance = this;
		CurrentSaveSlot = SaveSlot.Unset;
		PlayerData = new PlayerData();
		GameData = new GameData();
	}

	public override void _Ready()
	{
		base._Ready();

		InitialiseInputListening();

		EventManager.ListenEvent(GlobalEventCodes.Debug_ToggleMenu, GD.FuncRef(this, nameof(DebugToggleMenuChanged)));
	}

	public override void _EnterTree()
	{
		base._EnterTree();

		sceneTree = GetTree();
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (@event.IsPressed())
		{
			bool shouldRaiseEvent = false;

			if (@event is InputEventMouseButton || @event is InputEventKey)
			{
				if (ActiveInputType != GamepadType.PC)
				{
					ActiveInputType = GamepadType.PC;
					shouldRaiseEvent = true;
				}
			}
			else
			{
				//TODO: Fix this. We need to make it robust
				//TODO: Fix PS controllers with DS4 Tool turned on
				//Debug.Print($"Joypad ID: {@event.Device}: Connected Gamepad IDs: ({string.Join(",", connectedGamepads.Keys)})");
				var inputType = connectedGamepads[@event.Device];

				if (ActiveInputType != inputType)
				{
					ActiveInputType = inputType;
					shouldRaiseEvent = true;
				}
			}

			if (shouldRaiseEvent)
			{
				//var joyName = ActiveInputType == GamepadType.PC ? "Mouse + Keyboard" : Input.GetJoyName(@event.Device);
				//Debug.Print($"Raising Input Change Event. Type: {ActiveInputType}; Device: {joyName}; ID: {@event.Device}");
				EventManager.RaiseEvent(GlobalEventCodes.Global_PlayerInputUpdated);
			}
		}

		if (@event.IsActionPressed("debug") && !sceneTree.IsInputHandled())
		{
			DebugFlag = !DebugFlag;
			EventManager.RaiseEvent(GlobalEventCodes.Debug_Toggle, DebugFlag);
			Debug.Print($"Debug is now turned to {DebugFlag}");

			sceneTree.SetInputAsHandled();
		}
	}

	public void SetupNewGameData()
	{
		GameTime = new GameDateTime(1, GameData.TotalTimeInDaySeconds / 4, GameData.TotalTimeInDaySeconds);
	}

	public void ResetToGameTitle()
	{
		//TODO: Move data to a container class
		//TODO: Serialize that class
		//TODO: Load it back and manually reset stuff
		//SaveGame();

		//Reset in-game instances to null
		Player = null;
		PlayerUI = null;
		GameWorld = null;

		var tree = GetTree();
		tree.Paused = false;
		tree.ChangeScene(Helpers.LevelScenes.TitleScreen);
		EventManager.ClearEvents();
		ResetGlobalPropertiesToDefault();
	}

	//All objects in Global must be marked as Serializable
	public void SaveGame()
	{
		if (CurrentSaveSlot == SaveSlot.Unset)
			Debug.Print("The current game cannot be saved because a save slot has not been selected");

		using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);

			var file = new File();
			file.OpenCompressed(SaveSlots.SavePath(CurrentSaveSlot), File.ModeFlags.Write, File.CompressionMode.Gzip);
			file.StoreLine(Convert.ToBase64String(memoryStream.ToArray()));
			file.Close();
		}
	}

	public void LoadGame()
	{
		if (CurrentSaveSlot == SaveSlot.Unset)
			Debug.Print("The current game cannot be saved because a save slot has not been selected");

		using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
		{
			var file = new File();
			file.OpenCompressed(SaveSlots.SavePath(CurrentSaveSlot), File.ModeFlags.Read, File.CompressionMode.Gzip);
			var data = file.GetLine();
			var buffer = Convert.FromBase64String(data);

			BinaryFormatter binaryFormatter = new BinaryFormatter();
			memoryStream.Write(buffer, 0, buffer.Length);
			memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
			Global savedObject = (Global)binaryFormatter.Deserialize(memoryStream);
			this.CopyProperties(savedObject);
			Instance = this;

			file.Close();
		}
	}

	private void InitialiseInputListening()
	{

		var joypads = Input.GetConnectedJoypads();

		foreach (var deviceID in joypads)
		{
			var joyName = Input.GetJoyName((int)deviceID);

			if (joyName.ToLower().Contains("ps"))
				connectedGamepads.Add((int)deviceID, GamepadType.Playstation);
			else if (joyName.ToLower().Contains("switch"))
				connectedGamepads.Add((int)deviceID, GamepadType.Switch);
			else
				connectedGamepads.Add((int)deviceID, GamepadType.Xbox);

			//Debug.Print($"Device Connected - ID: {(int)deviceID}; Name: {Input.GetJoyName((int)deviceID)}");
		}

		//Debug.Print($"Connected Gamepad IDs: ({string.Join(",", connectedGamepads.Keys)})");
		Input.Singleton.Connect("joy_connection_changed", this, nameof(OnJoyConnectionChanged));
	}

	private void OnJoyConnectionChanged(int device, bool connected)
	{
		var joyName = Input.GetJoyName(device);

		Debug.Print($"Device: {device}; Name: {joyName}; Connected: {connected};");// Name: {name}; GUID: {guid}");

		if (connected)
		{
			//var joyName = Input.GetJoyName(device);

			if (joyName.ToLower().Contains("ps"))
				connectedGamepads.Add(device, GamepadType.Playstation);
			else if (joyName.ToLower().Contains("switch"))
				connectedGamepads.Add(device, GamepadType.Switch);
			else
				connectedGamepads.Add(device, GamepadType.Xbox);
		}
		else
		{
			if (connectedGamepads.ContainsKey(device))
				connectedGamepads.Remove(device);
		}
	}

	private void DebugToggleMenuChanged(bool menuVisibility)
	{
		DebugMenuFlag = menuVisibility;
	}

	private void ResetGlobalPropertiesToDefault()
	{
		CurrentSaveSlot = SaveSlot.Unset;
		PlayerData = new PlayerData();
		GameData = new GameData();
	}
}

public enum SaveSlot
{
	Unset = 0,
	Slot1 = 1,
	Slot2 = 2,
	Slot3 = 3,
}
