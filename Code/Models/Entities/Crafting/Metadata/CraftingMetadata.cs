public class CraftingMetadata
{
	public CraftingCategory Category { get; private set; }
	public ItemMetadataID OutputID;
	public BaseCraftingRequirement[] Requirements;
	public string ItemImagePath { get; private set; }
	public string Description { get; private set; }

	public CraftingMetadata(string itemImagePath, params BaseCraftingRequirement[] requirements)
	{
		ItemImagePath = itemImagePath;
		Requirements = requirements;
	}

	public void SetID(ItemMetadataID outputID, CraftingCategory category)
	{
		this.OutputID = outputID;
		this.Description = Strings.UI.CraftingRecipeDescription(outputID, category);
		this.Category = category;
	}
}

public enum CraftingCategory
{
	buildings,
	consumables,
	tools,
}