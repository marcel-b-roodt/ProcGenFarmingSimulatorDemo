using System.Collections.Generic;

class CraftingLookup
{
	//public static string CraftingImageResourceBasePath = @"res://Resources/HighRes/";
	public static string CraftingImageResourceBasePath = @"res://Resources/Icons/";

	public static Dictionary<CraftingCategory, Dictionary<ItemMetadataID, CraftingMetadata>> Recipes = new Dictionary<CraftingCategory, Dictionary<ItemMetadataID, CraftingMetadata>>();
	private static Dictionary<ItemMetadataID, CraftingMetadata> Buildings = new Dictionary<ItemMetadataID, CraftingMetadata>();
	private static Dictionary<ItemMetadataID, CraftingMetadata> Tools = new Dictionary<ItemMetadataID, CraftingMetadata>();

	static CraftingLookup()
	{
		AddBuildingKits();
		AddConsumables();
		AddMisc();
		AddTools();

		Recipes.Add(CraftingCategory.buildings, Buildings);
		Recipes.Add(CraftingCategory.tools, Tools);

		foreach (var categoryDictionaryKvp in Recipes)
		{
			foreach (var kvp in categoryDictionaryKvp.Value)
			{
				Recipes[categoryDictionaryKvp.Key][kvp.Key].SetID(kvp.Key, categoryDictionaryKvp.Key);
			}
		}
	}

	public static CraftingMetadata GetItemByMetadata(ItemMetadataID metadataID)
	{
		foreach (var category in Recipes.Values)
		{
			if (category.ContainsKey(metadataID))
				return (category[metadataID]);
		}

		return null;
	}

	private static string CraftingImageResourcePath(string imageName)
	{
		return $"{CraftingImageResourceBasePath}{imageName}";
	}

	private static void AddBuildingKits()
	{
		Buildings.Add(ItemMetadataID.building_tent, new CraftingMetadata(CraftingImageResourcePath("mc_wooden_spade.png"),
			new ResourceCraftingRequirement(ItemMetadataID.misc_wooden_stick, 12), new ResourceCraftingRequirement(ItemMetadataID.misc_grass, 12)));
	}

	private static void AddConsumables()
	{

	}

	private static void AddMisc()
	{

	}

	private static void AddTools()
	{
		#region StoneTools
		Tools.Add(ItemMetadataID.tool_stone_axe, new CraftingMetadata(CraftingImageResourcePath("mc_stone_axe.png"),
			new ResourceCraftingRequirement(ItemMetadataID.misc_wooden_stick, 6), new ResourceCraftingRequirement(ItemMetadataID.misc_stone, 3)));
		Tools.Add(ItemMetadataID.tool_stone_hoe, new CraftingMetadata(CraftingImageResourcePath("mc_stone_hoe.png"),
			new ResourceCraftingRequirement(ItemMetadataID.misc_wooden_stick, 4), new ResourceCraftingRequirement(ItemMetadataID.misc_stone, 2)));
		Tools.Add(ItemMetadataID.tool_stone_pickaxe, new CraftingMetadata(CraftingImageResourcePath("mc_stone_pickaxe.png"),
			new ResourceCraftingRequirement(ItemMetadataID.misc_wooden_stick, 6), new ResourceCraftingRequirement(ItemMetadataID.misc_stone, 3)));
		Tools.Add(ItemMetadataID.tool_stone_spade, new CraftingMetadata(CraftingImageResourcePath("mc_stone_spade.png"),
			new ResourceCraftingRequirement(ItemMetadataID.misc_wooden_stick, 5), new ResourceCraftingRequirement(ItemMetadataID.misc_stone, 2)));
		#endregion

		#region WoodTools
		Tools.Add(ItemMetadataID.tool_wood_axe, new CraftingMetadata(CraftingImageResourcePath("mc_wooden_axe.png"),
			new ResourceCraftingRequirement(ItemMetadataID.misc_wooden_stick, 12)));
		Tools.Add(ItemMetadataID.tool_wood_hoe, new CraftingMetadata(CraftingImageResourcePath("mc_wodden_hoe.png"),
			new ResourceCraftingRequirement(ItemMetadataID.misc_wooden_stick, 8)));
		Tools.Add(ItemMetadataID.tool_wood_pickaxe, new CraftingMetadata(CraftingImageResourcePath("mc_wooden_pickaxe.png"),
			new ResourceCraftingRequirement(ItemMetadataID.misc_wooden_stick, 10)));
		Tools.Add(ItemMetadataID.tool_wood_spade, new CraftingMetadata(CraftingImageResourcePath("mc_wooden_spade.png"),
			new ResourceCraftingRequirement(ItemMetadataID.misc_wooden_stick, 9)));
		#endregion
	}
}