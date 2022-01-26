using Godot;
using static Helpers;

public class ItemQuantityPopup : PopupPanel
{
	private InventoryTab inventoryTab;

	private Label itemQuantityMinLabel;
	private Slider itemQuantitySlider;
	private Label itemQuantityMaxLabel;
	private Label selectedItemQuantityLabel;

	private float sliderScrollDelay = 0.05f;
	private float sliderScrollInitialDelay = 0.2f;
	private float sliderScrollTime;

	public int SliderValue;

	private SceneTree sceneTree;

	public override void _Ready()
	{
		inventoryTab = GetParent<InventoryTab>();

		itemQuantityMinLabel = GetNode<Label>($"Margin/VB/HB_Slider/ItemQuantityMin");
		itemQuantitySlider = GetNode<Slider>($"Margin/VB/HB_Slider/ItemQuantitySlider");
		itemQuantityMaxLabel = GetNode<Label>($"Margin/VB/HB_Slider/ItemQuantityMax");
		selectedItemQuantityLabel = GetNode<Label>($"Margin/VB/SelectedItemQuantityLabel");

		itemQuantitySlider.MinValue = 1;
	}

	public override void _EnterTree()
	{
		base._EnterTree();

		sceneTree = GetTree();
	}

	public override void _Process(float delta)
	{
		base._Process(delta);

		if (Visible)
		{
			if (sliderScrollTime <= 0)
			{
				if (Input.IsActionPressed(PlayerInputCodes.DPad_Left) || Input.IsActionPressed(PlayerInputCodes.Left))
				{
					itemQuantitySlider.Value = Mathf.Max((float)itemQuantitySlider.Value - 1, 0);
					sliderScrollTime = sliderScrollDelay;
				}

				else if (Input.IsActionPressed(PlayerInputCodes.DPad_Right) || Input.IsActionPressed(PlayerInputCodes.Right))
				{
					itemQuantitySlider.Value = Mathf.Min((float)itemQuantitySlider.Value + 1, (float)itemQuantitySlider.MaxValue);
					sliderScrollTime = sliderScrollDelay;
				}

				else if (Input.IsActionJustReleased(PlayerInputCodes.DPad_Left) || Input.IsActionJustReleased(PlayerInputCodes.Left))
				{
					sliderScrollTime = 0;
				}

				else if (Input.IsActionJustReleased(PlayerInputCodes.DPad_Right) || Input.IsActionJustReleased(PlayerInputCodes.Right))
				{
					sliderScrollTime = 0;
				}
			}
			else
			{
				sliderScrollTime -= delta;
			}
		}
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (Visible)
		{
			if ((Input.IsActionJustPressed(PlayerInputCodes.DPad_Left) && @event.IsAction(PlayerInputCodes.DPad_Left) && !sceneTree.IsInputHandled())
				|| (Input.IsActionJustPressed(PlayerInputCodes.Left) && @event.IsAction(PlayerInputCodes.Left) && !sceneTree.IsInputHandled()))
			{
				itemQuantitySlider.Value = Mathf.Max((float)itemQuantitySlider.Value - 1, 0);
				sliderScrollTime = sliderScrollInitialDelay;
				sceneTree.SetInputAsHandled();
			}

			else if ((Input.IsActionJustPressed(PlayerInputCodes.DPad_Right) && @event.IsAction(PlayerInputCodes.DPad_Right) && !sceneTree.IsInputHandled())
				|| (Input.IsActionJustPressed(PlayerInputCodes.Right) && @event.IsAction(PlayerInputCodes.Right) && !sceneTree.IsInputHandled()))
			{
				itemQuantitySlider.Value = Mathf.Min((float)itemQuantitySlider.Value + 1, (float)itemQuantitySlider.MaxValue);
				sliderScrollTime = sliderScrollInitialDelay;
				sceneTree.SetInputAsHandled();
			}
		}
	}

	public void ShowItemQuantityDialogue(int sliderMaxValue)
	{
		Visible = true;
		itemQuantityMinLabel.Text = "1";
		itemQuantitySlider.MaxValue = sliderMaxValue;
		itemQuantitySlider.Value = 1;
		itemQuantityMaxLabel.Text = sliderMaxValue.ToString();
		selectedItemQuantityLabel.Text = ((int)itemQuantitySlider.Value).ToString();
	}

	public void HideItemQuantityDialogue()
	{
		Visible = false;
	}

	private void _on_ItemQuantitySlider_value_changed(float value)
	{
		selectedItemQuantityLabel.Text = ((int)value).ToString();
		SliderValue = (int)value;
	}
}






