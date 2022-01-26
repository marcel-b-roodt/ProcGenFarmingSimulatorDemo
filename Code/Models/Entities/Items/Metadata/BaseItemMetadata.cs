using Godot;

public class BaseItemMetadata
{
	public ItemMetadataID ID { get; private set; } = ItemMetadataID.meta_empty;
	public string Name { get; private set; }
	public string Description { get; private set; }
	public string IconImageName { get; private set; }
	public Texture IconTexture
	{
		get
		{
			if (iconTexture == null)
				iconTexture = ImageStore.Get(IconImageName);

			return iconTexture;
		}
	}
	private Texture iconTexture;

	public InventoryItemType Type { get; set; } = InventoryItemType.unset;
	public int StackLimit { get; private set; }
	public float Value { get; private set; }
	public bool Stackable { get { return StackLimit > 1; } }

	public BaseItemMetadata(string iconImageName, int stackLimit, float value)
	{
		IconImageName = iconImageName;
		StackLimit = stackLimit;
		Value = value;
	}

	internal void SetID(ItemMetadataID id)
	{
		this.ID = id;
		Name = Strings.UI.ItemName(id);
		Description = Strings.UI.ItemDescription(id);
	}
}

public enum InventoryItemType
{
	buildingKit,
	consumable,
	misc,
	unset,
	seed,
	tool,
}