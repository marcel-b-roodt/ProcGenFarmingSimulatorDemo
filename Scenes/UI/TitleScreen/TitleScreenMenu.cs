using Godot;
using System.Collections.Generic;
using static Helpers;

public class TitleScreenMenu : TitleMenu
{
	private TextureRect background;

	private Button NewGameButton;
	private Button ContinueButton;
	private Button OptionsButton;
	private Button QuitButton;

	private PanelContainer NewGameMenu;
	private List<Label> NewGameMenu_SaveSlotPlayerDetailsLabels = new List<Label>();
	private PopupPanel StartNewGamePanel;
	private int SelectedNewGameSlot { get { return sngs; } set { sngs = Mathf.Clamp(value, 0, 2); } }
	private int sngs = 0;

	private LineEdit PlayerNameEdit;
	private bool nameSelected;

	private SceneTree sceneTree;

	public override void _Ready()
	{
		base._Ready();

		background = GetNode<TextureRect>("Background");

		NewGameButton = GetNode<Button>("PC_MainButtons/VB/NewGameButton");
		ContinueButton = GetNode<Button>("PC_MainButtons/VB/ContinueButton");
		OptionsButton = GetNode<Button>("PC_MainButtons/VB/OptionsButton");
		QuitButton = GetNode<Button>("PC_MainButtons/VB/QuitButton");

		NewGameMenu = GetNode<PanelContainer>("PC_NewGameMenu");
		NewGameMenu_SaveSlotPlayerDetailsLabels.Add(GetNode<Label>("PC_NewGameMenu/MC/VB/HB_PlayerSlots/TextureButton_PlayerSlot1/PlayerDetailsLabel"));
		NewGameMenu_SaveSlotPlayerDetailsLabels.Add(GetNode<Label>("PC_NewGameMenu/MC/VB/HB_PlayerSlots/TextureButton_PlayerSlot2/PlayerDetailsLabel"));
		NewGameMenu_SaveSlotPlayerDetailsLabels.Add(GetNode<Label>("PC_NewGameMenu/MC/VB/HB_PlayerSlots/TextureButton_PlayerSlot3/PlayerDetailsLabel"));
		StartNewGamePanel = GetNode<PopupPanel>("PopupPanel_StartNewGame");
		PlayerNameEdit = GetNode<LineEdit>("PopupPanel_StartNewGame/VB/CC/PlayerNameEdit");

		//if (ReturnFocusItem == null)
		NewGameButton.GrabFocus();
	}

	public override void _EnterTree()
	{
		base._EnterTree();

		sceneTree = GetTree();
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (!InTransition)
		{
			if (StartNewGamePanel.Visible)
			{
				if ((Input.IsActionJustPressed(PlayerInputCodes.UI_Accept) && @event.IsAction(PlayerInputCodes.UI_Accept) && !sceneTree.IsInputHandled())
					|| (Input.IsActionJustPressed(PlayerInputCodes.UI_Select) && @event.IsAction(PlayerInputCodes.UI_Select) && !sceneTree.IsInputHandled()))
				{
					if (nameSelected)
					{
						StartGame();
					}
					else
					{
						nameSelected = true;
						if (PlayerNameEdit.HasFocus())
							PlayerNameEdit.ReleaseFocus();
					}
				}

				if ((Input.IsActionJustPressed(PlayerInputCodes.UI_Select) && @event.IsAction(PlayerInputCodes.UI_Select) && !sceneTree.IsInputHandled()) && !PlayerNameEdit.HasFocus())
				{
					if (nameSelected)
					{
						StartGame();
					}
					else
					{
						PlayerNameEdit.GrabFocus();
					}
				}

				if ((Input.IsActionJustPressed(PlayerInputCodes.UI_Cancel) && @event.IsAction(PlayerInputCodes.UI_Cancel) && !sceneTree.IsInputHandled()))
				{
					HideStartNewGameMenu();
				}
			}

			else if (NewGameMenu.Visible)
			{
				if ((Input.IsActionJustPressed(PlayerInputCodes.UI_Select) && @event.IsAction(PlayerInputCodes.UI_Select) && !sceneTree.IsInputHandled()))
				{
					ShowStartNewGameMenu();
				}

				if ((Input.IsActionJustPressed(PlayerInputCodes.UI_Cancel) && @event.IsAction(PlayerInputCodes.UI_Cancel) && !sceneTree.IsInputHandled()))
				{
					_on_NewGameButton_toggled(false);
				}

				if ((Input.IsActionJustPressed(PlayerInputCodes.UI_Left) && @event.IsAction(PlayerInputCodes.UI_Left) && !sceneTree.IsInputHandled()))
				{
					NewGameMenu_SaveSlotPlayerDetailsLabels[SelectedNewGameSlot].AddColorOverride("font_color", new Color(0, 0, 0));
					SelectedNewGameSlot--;
					NewGameMenu_SaveSlotPlayerDetailsLabels[SelectedNewGameSlot].AddColorOverride("font_color", new Color(1, 1, 1));
				}

				if ((Input.IsActionJustPressed(PlayerInputCodes.UI_Right) && @event.IsAction(PlayerInputCodes.UI_Right) && !sceneTree.IsInputHandled()))
				{
					NewGameMenu_SaveSlotPlayerDetailsLabels[SelectedNewGameSlot].AddColorOverride("font_color", new Color(0, 0, 0));
					SelectedNewGameSlot++;
					NewGameMenu_SaveSlotPlayerDetailsLabels[SelectedNewGameSlot].AddColorOverride("font_color", new Color(1, 1, 1));
				}
			}
		}
	}

	private void QuitGame()
	{
		GetTree().Quit();
	}

	private void StartGame()
	{
		if (PlayerNameEdit.Text.Length > 0)
		{
			SetPlayerInformation();
			StartNewGamePanel.Visible = false;
			StartSceneTransition(LevelScenes.GameWorld, false);
		}
	}

	private void SetPlayerInformation()
	{
		var playerName = PlayerNameEdit.Text;

		Global.Instance.CurrentSaveSlot = (SaveSlot)(SelectedNewGameSlot + 1);
		Global.Instance.PlayerData.InitializeNewPlayerVars(playerName);
		Global.Instance.SetupNewGameData();
	}

	private void ShowNewGameMenu()
	{
		ToggleMainMenuButtonFocus(false);
		NewGameMenu.Visible = true;
		SelectedNewGameSlot = 0;
		NewGameMenu_SaveSlotPlayerDetailsLabels[SelectedNewGameSlot].AddColorOverride("font_color", new Color(1, 1, 1));
		//SaveSlot1PlayerDetailsLabel.Text = "";
		//SaveSlot2PlayerDetailsLabel.Text = "";
		//SaveSlot3PlayerDetailsLabel.Text = "";

	}

	private void ToggleMainMenuButtonFocus(bool enabled)
	{
		if (enabled)
		{
			NewGameButton.FocusMode = FocusModeEnum.All;
			ContinueButton.FocusMode = FocusModeEnum.All;
			OptionsButton.FocusMode = FocusModeEnum.All;
			QuitButton.FocusMode = FocusModeEnum.All;
		}
		else
		{
			NewGameButton.FocusMode = FocusModeEnum.None;
			ContinueButton.FocusMode = FocusModeEnum.None;
			OptionsButton.FocusMode = FocusModeEnum.None;
			QuitButton.FocusMode = FocusModeEnum.None;
		}
	}

	private void ShowStartNewGameMenu()
	{
		StartNewGamePanel.Visible = true;
		nameSelected = false;
	}

	private void HideStartNewGameMenu()
	{
		StartNewGamePanel.Visible = false;
		nameSelected = false;
	}

	#region Signals
	private void _on_NewGameButton_toggled(bool button_pressed)
	{
		NewGameButton.Pressed = button_pressed;

		if (button_pressed)
		{
			ToggleMainMenuButtonFocus(false);
			HideStartNewGameMenu();
			ShowNewGameMenu();
		}
		else
		{
			NewGameMenu.Visible = false;
			ToggleMainMenuButtonFocus(true);
			NewGameButton.GrabFocus();
		}
	}

	private void _on_ContinueButton_toggled(bool button_pressed)
	{
		// Replace with function body.
	}

	private void _on_OptionsButton_toggled(bool button_pressed)
	{
		// Replace with function body.
	}

	private void _on_QuitButton_pressed()
	{
		QuitGame();
	}

	#endregion
}














