using Godot;

public class InventoryCell : TextureButton
{
	private TextureRect icon;
	private Label quantity;
	private ProgressBar health;

	public override void _Ready()
	{
		base._Ready();

		icon = GetNode<TextureRect>($"TextureRect_ItemIcon");
		quantity = GetNode<Label>($"TextLabel_ItemQuantity");
		health = GetNode<ProgressBar>("ProgressBar_ItemHealth");
	}

	public void SetData(PlayerInventorySlot slot)
	{
		if (!slot.Item.Empty)
			icon.Texture = slot.Item.Metadata.IconTexture;
		else
			icon.Texture = null;

		if (slot.Item.Metadata.Stackable)
		{
			quantity.Visible = true;
			quantity.Text = slot.DisplayQuantity;
		}
		else
		{
			quantity.Visible = false;
		}

		if (slot.Item.Metadata.Type == InventoryItemType.tool)
		{
			health.Visible = true;
			ToolItemMetadata itemMetadata = (ToolItemMetadata)slot.Item.Metadata;
			health.MinValue = 0;
			health.MaxValue = itemMetadata.MaxCondition;
			health.Value = slot.Item.CurrentCondition;
		}
		else
		{
			health.Visible = false;
		}
	}
}

