using Godot;
using static Helpers;

public class DialogueMenu : Panel
{
	private bool openedThisFrame = false;
	private bool isInteracting = false;
	private float timer = 0;
	private string[] textToPrint = { };

	private int currentChar;
	private int currentText;

	private float timeEnteredState;
	private PrintingState previousPrintingState;
	private PrintingState currentPrintingState;

	private const float WRITE_SPEED = 0.05f;

	private RichTextLabel dialogueText;
	private UIInput skipButton;
	private UIInput okButton;
	private UIInput leaveButton;

	private SceneTree sceneTree;

	private void TransitionToState(PrintingState newState)
	{
		previousPrintingState = currentPrintingState;
		OnStateExit(previousPrintingState, newState);
		currentPrintingState = newState;
		//TODO: Add time here
		OnStateEnter(newState, previousPrintingState);
	}

	private void OnStateEnter(PrintingState state, PrintingState fromState)
	{
		switch (state)
		{
			case PrintingState.Printing:
				dialogueText.BbcodeText = "";
				currentChar = 0;
				timer = 0;
				Debug.Print($"Printing {textToPrint[currentText]}");

				skipButton.Visible = true;
				okButton.Visible = false;
				leaveButton.Visible = false;
				break;
			case PrintingState.Paused:
				dialogueText.BbcodeText = textToPrint[currentText];
				Debug.Print($"Paused after printing {textToPrint[currentText]}");
				currentText += 1;

				skipButton.Visible = false;
				okButton.Visible = true;
				leaveButton.Visible = false;
				break;
			case PrintingState.Done:
				Debug.Print("Done!");

				skipButton.Visible = false;
				okButton.Visible = false;
				leaveButton.Visible = true;
				break;
		}
	}

	private void OnStateExit(PrintingState state, PrintingState toState)
	{

	}

	public override void _Ready()
	{
		base._Ready();

		dialogueText = GetNode<RichTextLabel>("Margin/VB/DialogueText");
		skipButton = GetNode<UIInput>("Margin/VB/HB/UIInput_Skip");
		okButton = GetNode<UIInput>("Margin/VB/HB/UIInput_OK");
		leaveButton = GetNode<UIInput>("Margin/VB/HB/UIInput_Leave");

		currentPrintingState = PrintingState.Done;
		//TransitionToState(PrintingState.Done);
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
			if (@event.IsActionPressed(PlayerInputCodes.A) && !sceneTree.IsInputHandled())
			{
				isInteracting = true;
				sceneTree.SetInputAsHandled();
			}
		}
	}

	public override void _Process(float delta)
	{
		base._Process(delta);

		if (Visible)
		{
			//TODO: Upgrade this logic to signals
			//Create new events that 
			switch (currentPrintingState)
			{
				case PrintingState.Printing:
					timer += delta;
					if (timer > WRITE_SPEED)
					{
						timer = 0;
						var currentDialogueText = dialogueText.BbcodeText;
						dialogueText.BbcodeText = $"{currentDialogueText}{textToPrint[currentText][currentChar]}";
						currentChar += 1;
					}

					if (currentChar >= textToPrint[currentText].Length)
						TransitionToState(PrintingState.Paused);

					if (!openedThisFrame && isInteracting)
					{
						dialogueText.BbcodeText = textToPrint[currentText];
						TransitionToState(PrintingState.Paused);
					}

					break;

				case PrintingState.Paused:
					if (currentText >= textToPrint.Length)
						TransitionToState(PrintingState.Done);

					else if (!openedThisFrame && isInteracting)
						TransitionToState(PrintingState.Printing);
					break;

				case PrintingState.Done:
					if (isInteracting)
						HideDialogue();

					break;
			}

			openedThisFrame = false;
			isInteracting = false;
		}
	}

	public void HideDialogue()
	{
		EventManager.RaiseEvent(GlobalEventCodes.Dialogue_Complete);

		this.Hide();
		dialogueText.BbcodeText = "";
	}

	public void ShowDialogue(string[] text)
	{
		this.Show();
		openedThisFrame = true;
		textToPrint = text;
		currentText = 0;
		TransitionToState(PrintingState.Printing);
	}
}

public enum PrintingState
{
	Printing,
	Paused,
	Done
}
