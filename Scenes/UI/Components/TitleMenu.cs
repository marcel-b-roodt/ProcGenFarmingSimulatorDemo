using Godot;

public class TitleMenu : Control
{
	public bool InTransition;
	public string ScenePathToLoad { get; set; }
	public Control ReturnFocusItem;
	public TransitionControl TransitionControl;

	public override void _Ready()
	{
		base._Ready();

		TransitionControl = SceneStore.Instantiate<TransitionControl>();
		AddChild(TransitionControl);

		TransitionControl.Connect(nameof(TransitionControl.FadeOutFinished), this, nameof(SceneTransitionFinished));

		TransitionControl.Reset();
		TransitionControl.FadeIn();
	}

	//TODO: Change this boolean to an enum to control the animation type
	public void StartSceneTransition(string scenePathToLoad, bool shortTransition = true)
	{
		InTransition = true;
		ScenePathToLoad = scenePathToLoad;
		if (shortTransition)
			TransitionControl.FadeOut();
		else
			TransitionControl.FadeOutLong();
	}

	public virtual void SceneTransitionFinished()
	{
		InTransition = false;
		GetTree().ChangeScene(ScenePathToLoad);
	}

	#region MenuConfiguration
	//Use this if you want to come back to the same control all the time.
	//Or if you want to remember what control you had focused before leaving the menu
	//public void SetReturnFocusItem(Control control)
	//{
	//	ReturnFocusItem = control;
	//}

	//public void GrabReturnFocusItem()
	//{
	//	ReturnFocusItem?.GrabFocus();
	//}

	//public void FocusChanged(Control control)
	//{
	//	SetReturnFocusItem(control);
	//}
	#endregion
}