using Godot;
using static Helpers;

public class PlayerUI : CanvasLayer
{
	private MainMenu mainMenu;
	private DialogueMenu dialogueMenu;
	private QuickSlotMenu quickSlotMenu;
	private GenericInputDialogueMenu genericDialogueMenu;
	private DebugMenu debugMenu;

	public bool MainMenuActive { get { return mainMenu.Visible; } }
	public bool DialogueActive { get { return dialogueMenu.Visible; } }
	public bool QuickSlotsActive { get { return quickSlotMenu.MenuActive; } }
	public bool GenericDialogueActive { get { return genericDialogueMenu.Visible; } }
	public bool MenuOpen { get { return MainMenuActive || DialogueActive || GenericDialogueActive; } }

	public bool InventoryActive { get { return mainMenu.InventoryActive; } }
	public bool CraftingActive { get { return mainMenu.CraftingActive; } }
	public bool MapActive { get { return mainMenu.MapActive; } }
	public bool StatsActive { get { return mainMenu.StatsActive; } }
	public bool OptionsActive { get { return mainMenu.OptionsActive; } }

	public bool DebugActive { get { return Global.Instance.DebugFlag; } }

	private SceneTree sceneTree;

	public override void _Ready()
	{
		dialogueMenu = GetNode<DialogueMenu>("DialogueMenu");
		mainMenu = GetNode<MainMenu>("MainMenu");
		quickSlotMenu = GetNode<QuickSlotMenu>("QuickSlotMenu");
		genericDialogueMenu = GetNode<GenericInputDialogueMenu>("GenericDialogueMenu");
		debugMenu = GetNode<DebugMenu>("DebugMenu");

		Global.Instance.PlayerUI = this;
		quickSlotMenu.SetActive(true);

		EventManager.ListenEvent(nameof(GlobalEventCodes.Dialogue_Complete), GD.FuncRef(this, nameof(DialogueComplete)));
	}

	public override void _EnterTree()
	{
		base._EnterTree();

		sceneTree = GetTree();
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (!DialogueActive
			&& !GenericDialogueActive
			&& @event.IsActionPressed(Helpers.PlayerInputCodes.Start) && !sceneTree.IsInputHandled())
		{
			ToggleMainMenu();
			sceneTree.SetInputAsHandled();
		}
	}

	private void ToggleMainMenu()
	{
		if (mainMenu.Visible)
		{
			mainMenu.Visible = false;
			quickSlotMenu.SetActive(true);
			debugMenu.Visible = DebugActive;
		}
		else
		{
			mainMenu.Visible = true;
			mainMenu.ShowCurrentTab();
			quickSlotMenu.SetActive(false);
			debugMenu.Visible = false;
		}
	}

	public void ShowDialogue(string[] dialogueText)
	{
		quickSlotMenu.SetActive(false);
		quickSlotMenu.Visible = false;
		dialogueMenu.ShowDialogue(dialogueText);
	}

	//public void ShowGenericDialogue(DialoguePurpose purpose)
	//{
	//	genericDialogueMenu.ShowGenericDialogue(purpose);
	//}

	//public static void DisplayDialogue(DialoguePurpose purpose)
	//{
	//	Global.Instance.PlayerUI.ShowGenericDialogue(purpose);
	//}

	public int GetQuickSlotIndex()
	{
		return quickSlotMenu.GetQuickSlotIndex();
	}

	private void DialogueComplete()
	{
		quickSlotMenu.SetActive(true);
		quickSlotMenu.Visible = true;
	}
}
