using Godot;

public class CraftingTreeRecipeNode : CraftingTreeNode
{
	private const int itemIndentSize = 16;

	public Texture ItemIcon;
	public ItemMetadataID? ItemMetadataID = null;
	public CraftingTreeCategoryNode CategoryNode;
	public int RecipePosition;

	private Button button;
	private Control itemIndent;
	private TextureRect itemIcon;
	private Label itemLabel;

	private bool mouseOver;

	public override void _Ready()
	{
		base._Ready();

		button = GetNode<Button>("Button");
		itemIndent = GetNode<Control>("Button/HB/Padding");
		itemIcon = GetNode<TextureRect>("Button/HB/Icon");
		itemLabel = GetNode<Label>("Button/HB/Name");

		itemIndent.RectMinSize = new Vector2(itemIndentSize, 0);
		itemIcon.Texture = ItemIcon;
		itemLabel.Text = ItemName;
	}

	public void ConfigureSelectClick(Object caller, string method)
	{
		button.Connect("pressed", caller, method, new Godot.Collections.Array { Index });
	}
}
