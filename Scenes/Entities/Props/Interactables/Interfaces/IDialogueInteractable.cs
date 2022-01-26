public interface IDialogueInteractable : IInteractable
{
	string[] DialogueText { get; }

	void InitialiseText(string[] dialogueText);
}
