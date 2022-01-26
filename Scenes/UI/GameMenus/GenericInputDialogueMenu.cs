using Godot;
using System;
using static Helpers;

public class GenericInputDialogueMenu : Panel
{
	private Label titleLabel;
	private Label textLabel;
	private LineEdit inputLineEdit;

	private Action<string, dynamic[]> confirmAction;
	private Action cancelAction;
	private dynamic[] inputArgs;

	private SceneTree sceneTree;

	public static GenericInputDialogueMenu Instance { get; private set; }

	public GenericInputDialogueMenu()
	{
		Instance = this;
	}

	public override void _Ready()
	{
		titleLabel = GetNode<Label>("Margin/VB/DialogueTitle");
		textLabel = GetNode<Label>("Margin/VB/VB_Body/DialogueText");
		inputLineEdit = GetNode<LineEdit>("Margin/VB/VB_Body/CC/InputText");
	}

	public override void _EnterTree()
	{
		base._EnterTree();

		sceneTree = GetTree();
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (Visible)
		{
			//TODO: Fix the UI of the dialogue
			//TODO: Fix the input. Right now, when the dialogue is closed, then the next NameTag opens the dialogue again
			if (!inputLineEdit.HasFocus() && @event.IsActionPressed(PlayerInputCodes.Start) && !sceneTree.IsInputHandled())
			{
				inputLineEdit.FocusMode = FocusModeEnum.Click;
				inputLineEdit.GrabFocus();

				sceneTree.SetInputAsHandled();
			}
			else if (inputLineEdit.HasFocus() && @event.IsActionPressed(PlayerInputCodes.Start) && !sceneTree.IsInputHandled())
			{
				inputLineEdit.FocusMode = FocusModeEnum.None;
				inputLineEdit.FocusMode = FocusModeEnum.Click;

				inputLineEdit.Text = inputLineEdit.Text
										.ReplaceN(" ", "_")
										//.ReplaceN("", "")
										.Trim();

				sceneTree.SetInputAsHandled();
			}

			if (!inputLineEdit.HasFocus() && @event.IsActionPressed(PlayerInputCodes.A) && !sceneTree.IsInputHandled())
			{
				ConfirmDialogue();
				sceneTree.SetInputAsHandled();
			}

			if (!inputLineEdit.HasFocus() && @event.IsActionPressed(PlayerInputCodes.B) && !sceneTree.IsInputHandled())
			{
				CancelDialogue();
				GetTree().SetInputAsHandled();
			}
		}
	}

	public static void WaitForDialogue(string title, string text, string inputText, Action<string, dynamic[]> confirmAction, Action cancelAction, params dynamic[] inputArgs)
	{
		Instance.titleLabel.Text = title;
		Instance.textLabel.Text = text;
		Instance.inputLineEdit.Text = string.Empty;
		Instance.confirmAction = confirmAction;
		Instance.cancelAction = cancelAction;
		Instance.inputArgs = inputArgs;
		Instance.Show();
	}

	public void ConfirmDialogue()
	{
		var input = inputLineEdit.Text
			//.ReplaceN(" ", "_")
			//.ReplaceN("", "")
			.Trim();

		if (string.IsNullOrWhiteSpace(input))
			return;

		this.confirmAction(input, inputArgs);
		this.Hide();

		this.confirmAction = null;
		this.cancelAction = null;
		this.inputArgs = null;
	}

	public void CancelDialogue()
	{
		this.cancelAction();
		this.Hide();

		this.confirmAction = null;
		this.cancelAction = null;
		this.inputArgs = null;
	}

	public void ShowGenericDialogue(DialoguePurpose purpose)
	{
		this.Show();
		titleLabel.Text = Strings.UI.GenericDialogueTitle(purpose);
		textLabel.Text = Strings.UI.GenericDialogueText(purpose);
	}
}

public enum DialoguePurpose
{
	ChooseName
}
