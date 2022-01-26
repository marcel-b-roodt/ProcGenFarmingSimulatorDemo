using Godot;

public class CraftingTreeCategoryNode : CraftingTreeNode
{
	public Texture ItemIcon;

	//[Signal] public delegate void CellSelected();

	private Button button;
	private TextureButton itemExpanderButton;
	private Label itemLabel;

	private bool mouseOver;
	private bool categoryOpen;

	public override void _Ready()
	{
		base._Ready();

		button = GetNode<Button>("Button");
		itemExpanderButton = GetNode<TextureButton>("Button/HB/Expander");
		itemLabel = GetNode<Label>("Button/HB/Name");

		categoryOpen = true;
		itemExpanderButton.TextureNormal = ImageStore.Get(nameof(ImageStore.UI.UI_TreeNodeArrow_Open_Normal));
		itemExpanderButton.TextureHover = ImageStore.Get(nameof(ImageStore.UI.UI_TreeNodeArrow_Open_Hover));

		itemLabel.Text = ItemName;
	}

	public void ConfigureExpanderClick(Object caller, string method)
	{
		itemExpanderButton.Connect("pressed", caller, method, new Godot.Collections.Array { Index });
	}

	public void ConfigureSelectClick(Object caller, string method)
	{
		button.Connect("pressed", caller, method, new Godot.Collections.Array { Index });
	}

	public void ToggleCategoryAccordion(bool open)
	{
		categoryOpen = open;

		if (categoryOpen)
		{
			itemExpanderButton.TextureNormal = ImageStore.Get(nameof(ImageStore.UI.UI_TreeNodeArrow_Open_Normal));
			itemExpanderButton.TextureHover = ImageStore.Get(nameof(ImageStore.UI.UI_TreeNodeArrow_Open_Hover));
		}
		else
		{
			itemExpanderButton.TextureNormal = ImageStore.Get(nameof(ImageStore.UI.UI_TreeNodeArrow_Closed_Normal));
			itemExpanderButton.TextureHover = ImageStore.Get(nameof(ImageStore.UI.UI_TreeNodeArrow_Closed_Hover));
		}
	}
}

