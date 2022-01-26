using System.Collections.Generic;

public static class ItemLookup
{
	private static Dictionary<ItemMetadataID, BaseItemMetadata> Items = new Dictionary<ItemMetadataID, BaseItemMetadata>();

	static ItemLookup()
	{
		AddMeta();
		AddBuildingKits();
		AddConsumables();
		AddMisc();
		AddSeeds();
		AddTools();

		foreach (var kvp in Items)
		{
			Items[kvp.Key].SetID(kvp.Key);
		}
	}

	//TODO: When restoring items from saves, make sure to use the name code to restore the item, as the IDs can move around. We're keeping them in alphabetical order

	public static BaseItemMetadata NoItem() => Items[ItemMetadataID.meta_empty];
	public static BaseItemMetadata Get(ItemMetadataID id)
	{
		if (Items.ContainsKey(id))
			return Items[id];
		else
			return NoItem();
	}

	private static void AddMeta()
	{
		Items.Add(ItemMetadataID.meta_empty, new EmptyItemMetadata(nameof(ImageStore.UI.UI_EmptyCell), stackLimit: 1, value: 0));
	}

	private static void AddBuildingKits()
	{
		Items.Add(ItemMetadataID.building_tent, new BuildingKitMetadata(nameof(ImageStore.Items.Building_Tent), buildingType: BuildingType.Bed, value: 35));
	}

	private static void AddConsumables()
	{
		Items.Add(ItemMetadataID.consumable_crop_artichoke, new FoodConsumableMetadata(nameof(ImageStore.Items.Consumable_Artichoke), stackLimit: 20, value: 5));
		Items.Add(ItemMetadataID.consumable_crop_apple, new FoodConsumableMetadata(nameof(ImageStore.Items.Consumable_Apple), stackLimit: 20, value: 5));
		Items.Add(ItemMetadataID.consumable_crop_carrot, new FoodConsumableMetadata(nameof(ImageStore.Items.Consumable_Carrot), stackLimit: 20, value: 5));
		Items.Add(ItemMetadataID.consumable_crop_corn, new FoodConsumableMetadata(nameof(ImageStore.Items.Consumable_Corn), stackLimit: 20, value: 5));
		Items.Add(ItemMetadataID.consumable_crop_gourd, new FoodConsumableMetadata(nameof(ImageStore.Items.Consumable_Gourd), stackLimit: 20, value: 5));
		Items.Add(ItemMetadataID.consumable_crop_potato, new FoodConsumableMetadata(nameof(ImageStore.Items.Consumable_Potato), stackLimit: 20, value: 5));
		Items.Add(ItemMetadataID.consumable_crop_tomato, new FoodConsumableMetadata(nameof(ImageStore.Items.Consumable_Tomato), stackLimit: 20, value: 5));
		Items.Add(ItemMetadataID.consumable_misc_nametag, new NameTagConsumableMetadata(nameof(ImageStore.Items.Consumable_Nametag), stackLimit: 10, value: 20));
	}

	private static void AddMisc()
	{
		Items.Add(ItemMetadataID.misc_grass, new MiscItemMetadata(nameof(ImageStore.Items.Misc_Grass), stackLimit: 80, value: 1));
		Items.Add(ItemMetadataID.misc_stone, new MiscItemMetadata(nameof(ImageStore.Items.Misc_Stone), stackLimit: 25, value: 8));
		Items.Add(ItemMetadataID.misc_wooden_log, new MiscItemMetadata(nameof(ImageStore.Items.Misc_WoodenLog), stackLimit: 20, value: 5));
		Items.Add(ItemMetadataID.misc_wooden_stick, new MiscItemMetadata(nameof(ImageStore.Items.Misc_WoodenStick), stackLimit: 80, value: 2));
	}

	private static void AddSeeds()
	{
		Items.Add(ItemMetadataID.seeds_artichoke, new CropSeedItemMetadata(nameof(ImageStore.Items.Seeds_Apple), cropType: InventoryCropSeedType.Artichoke, stackLimit: 20, value: 10));
		Items.Add(ItemMetadataID.seeds_carrot, new CropSeedItemMetadata(nameof(ImageStore.Items.Seeds_Carrot), cropType: InventoryCropSeedType.Carrot, stackLimit: 20, value: 10));
		Items.Add(ItemMetadataID.seeds_corn, new CropSeedItemMetadata(nameof(ImageStore.Items.Seeds_Corn), cropType: InventoryCropSeedType.Corn, stackLimit: 20, value: 10));
		Items.Add(ItemMetadataID.seeds_gourd, new CropSeedItemMetadata(nameof(ImageStore.Items.Seeds_Gourd), cropType: InventoryCropSeedType.Gourd, stackLimit: 20, value: 10));
		Items.Add(ItemMetadataID.seeds_potato, new CropSeedItemMetadata(nameof(ImageStore.Items.Seeds_Potato), cropType: InventoryCropSeedType.Potato, stackLimit: 20, value: 10));
		Items.Add(ItemMetadataID.seeds_sapling_pine, new CropSeedItemMetadata(nameof(ImageStore.Items.Sapling_Pine), cropType: InventoryCropSeedType.Tree_Pine, stackLimit: 8, value: 10));
		Items.Add(ItemMetadataID.seeds_sapling_oak, new CropSeedItemMetadata(nameof(ImageStore.Items.Sapling_Oak), cropType: InventoryCropSeedType.Tree_Oak, stackLimit: 8, value: 10));
		Items.Add(ItemMetadataID.seeds_sapling_apple, new CropSeedItemMetadata(nameof(ImageStore.Items.Sapling_Apple), cropType: InventoryCropSeedType.Tree_Apple, stackLimit: 8, value: 10));
		Items.Add(ItemMetadataID.seeds_tomato, new CropSeedItemMetadata(nameof(ImageStore.Items.Seeds_Tomato), cropType: InventoryCropSeedType.Tomato, stackLimit: 20, value: 10));
	}

	private static void AddTools()
	{
		Items.Add(ItemMetadataID.tool_stone_axe, new ToolItemMetadata(nameof(ImageStore.Items.Tool_StoneAxe), toolType: InventoryToolType.axe, maxCondition: 120, damage: 1, value: 55));
		Items.Add(ItemMetadataID.tool_wood_axe, new ToolItemMetadata(nameof(ImageStore.Items.Tool_WoodenAxe), toolType: InventoryToolType.axe, maxCondition: 40, damage: 1, value: 24));
		Items.Add(ItemMetadataID.tool_stone_hoe, new ToolItemMetadata(nameof(ImageStore.Items.Tool_StoneHoe), toolType: InventoryToolType.hoe, maxCondition: 120, damage: 1, value: 6));
		Items.Add(ItemMetadataID.tool_wood_hoe, new ToolItemMetadata(nameof(ImageStore.Items.Tool_WoodenHoe), toolType: InventoryToolType.hoe, maxCondition: 40, damage: 1, value: 6));
		Items.Add(ItemMetadataID.tool_stone_pickaxe, new ToolItemMetadata(nameof(ImageStore.Items.Tool_StonePickaxe), toolType: InventoryToolType.pickaxe, maxCondition: 120, damage: 1, value: 25));
		Items.Add(ItemMetadataID.tool_wood_pickaxe, new ToolItemMetadata(nameof(ImageStore.Items.Tool_WoodenPickaxe), toolType: InventoryToolType.pickaxe, maxCondition: 40, damage: 1, value: 25));
		Items.Add(ItemMetadataID.tool_stone_spade, new ToolItemMetadata(nameof(ImageStore.Items.Tool_StoneSpade), toolType: InventoryToolType.spade, maxCondition: 120, damage: 1, value: 45));
		Items.Add(ItemMetadataID.tool_wood_spade, new ToolItemMetadata(nameof(ImageStore.Items.Tool_WoodenSpade), toolType: InventoryToolType.spade, maxCondition: 40, damage: 1, value: 45));
	}
}

public enum ItemMetadataID
{
	meta_empty,
	building_tent,
	consumable_crop_apple,
	consumable_crop_artichoke,
	consumable_crop_carrot,
	consumable_crop_corn,
	consumable_crop_gourd,
	consumable_crop_potato,
	consumable_crop_tomato,
	consumable_misc_nametag,
	misc_stone,
	misc_grass,
	misc_wooden_log,
	misc_wooden_stick,
	seeds_artichoke,
	seeds_carrot,
	seeds_corn,
	seeds_gourd,
	seeds_potato,
	seeds_sapling_pine,
	seeds_sapling_oak,
	seeds_sapling_apple,
	seeds_tomato,
	tool_stone_axe,
	tool_stone_hoe,
	tool_stone_pickaxe,
	tool_stone_spade,
	tool_wood_axe,
	tool_wood_hoe,
	tool_wood_pickaxe,
	tool_wood_spade,
}

