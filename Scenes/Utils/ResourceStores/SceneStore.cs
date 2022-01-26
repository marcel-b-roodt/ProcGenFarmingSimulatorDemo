using Godot;
using Godot.Collections;

public static class SceneStore
{
	public static class Scenes
	{
		public static class Entities
		{
			public const string ArtichokeCropEntity = "res://Scenes/Entities/Props/Crops/ArtichokeCropEntity.tscn";
			public const string CarrotCropEntity = "res://Scenes/Entities/Props/Crops/CarrotCropEntity.tscn";
			public const string CornCropEntity = "res://Scenes/Entities/Props/Crops/CornCropEntity.tscn";
			public const string GourdCropEntity = "res://Scenes/Entities/Props/Crops/GourdCropEntity.tscn";
			public const string ItemPickupEntity = "res://Scenes/Entities/Items/ItemPickupEntity.tscn";
			public const string PlayerEntity = "res://Scenes/Entities/Player/PlayerEntity.tscn";
			public const string PotatoCropEntity = "res://Scenes/Entities/Props/Crops/PotatoCropEntity.tscn";
			public const string RockEntity = "res://Scenes/Entities/Props/RockEntity.tscn";
			public const string SignboardEntity = "res://Scenes/Entities/Props/Interactables/SignboardEntity.tscn";
			public const string TallGrassEntity = "res://Scenes/Entities/Props/TallGrassEntity.tscn";
			public const string TentEntity = "res://Scenes/Entities/Props/Interactables/Buildings/TentEntity.tscn";
			public const string TomatoCropEntity = "res://Scenes/Entities/Props/Crops/TomatoCropEntity.tscn";
			public const string TreeEntity = "res://Scenes/Entities/Props/Interactables/TreeEntity.tscn";
			public const string TreeSaplingEntity = "res://Scenes/Entities/Props/TreeSaplingEntity.tscn";
		}

		public static class Meta
		{
			public const string NameTag = "res://Scenes/Entities/Meta/NameTag.tscn";
			public const string SoundPlayer = "res://Scenes/Entities/Meta/SoundPlayer.tscn";
			public const string ToastMessage = "res://Scenes/Entities/Meta/ToastMessage.tscn";
			public const string WorldCell = "res://Scenes/Worlds/WorldCell.tscn";
		}

		public static class UI
		{
			public const string CraftingRequirementNode = "res://Scenes/UI/GameMenus/MenuComponents/CraftingTab/CraftingRequirementNode.tscn";
			public const string CraftingTreeCategoryNode = "res://Scenes/UI/GameMenus/MenuComponents/CraftingTab/CraftingTreeCategoryNode.tscn";
			public const string CraftingTreeRecipeNode = "res://Scenes/UI/GameMenus/MenuComponents/CraftingTab/CraftingTreeRecipeNode.tscn";
			public const string InventoryCell = "res://Scenes/UI/GameMenus/MenuComponents/InventoryTab/InventoryCell.tscn";
			public const string TownMarker = "res://Scenes/UI/GameMenus/MenuComponents/MapTab/MapMarkers/TownMarker.tscn";
			public const string TransitionControl = "res://Scenes/UI/TransitionControl/TransitionControl.tscn";
		}
	}

	private static Dictionary<string, PackedScene> resources = new Dictionary<string, PackedScene>
	{
		#region Entities
		{ nameof(Scenes.Entities.ArtichokeCropEntity),       ResourceLoader.Load<PackedScene>(Scenes.Entities.ArtichokeCropEntity)},
		{ nameof(Scenes.Entities.CornCropEntity),            ResourceLoader.Load<PackedScene>(Scenes.Entities.CornCropEntity)},
		{ nameof(Scenes.Entities.CarrotCropEntity),          ResourceLoader.Load<PackedScene>(Scenes.Entities.CarrotCropEntity)},
		{ nameof(Scenes.Entities.GourdCropEntity),           ResourceLoader.Load<PackedScene>(Scenes.Entities.GourdCropEntity)},
		{ nameof(Scenes.Entities.ItemPickupEntity),          ResourceLoader.Load<PackedScene>(Scenes.Entities.ItemPickupEntity)},
		{ nameof(Scenes.Entities.PlayerEntity),              ResourceLoader.Load<PackedScene>(Scenes.Entities.PlayerEntity)},
		{ nameof(Scenes.Entities.PotatoCropEntity),          ResourceLoader.Load<PackedScene>(Scenes.Entities.PotatoCropEntity)},
		{ nameof(Scenes.Entities.RockEntity),                ResourceLoader.Load<PackedScene>(Scenes.Entities.RockEntity)},
		{ nameof(Scenes.Entities.SignboardEntity),           ResourceLoader.Load<PackedScene>(Scenes.Entities.SignboardEntity)},
		{ nameof(Scenes.Entities.TallGrassEntity),           ResourceLoader.Load<PackedScene>(Scenes.Entities.TallGrassEntity)},
		{ nameof(Scenes.Entities.TentEntity),                ResourceLoader.Load<PackedScene>(Scenes.Entities.TentEntity)},
		{ nameof(Scenes.Entities.TomatoCropEntity),          ResourceLoader.Load<PackedScene>(Scenes.Entities.TomatoCropEntity)},
		{ nameof(Scenes.Entities.TreeEntity),                ResourceLoader.Load<PackedScene>(Scenes.Entities.TreeEntity)},
		{ nameof(Scenes.Entities.TreeSaplingEntity),         ResourceLoader.Load<PackedScene>(Scenes.Entities.TreeSaplingEntity)},
		#endregion

		#region Meta
		{ nameof(Scenes.Meta.NameTag),              ResourceLoader.Load<PackedScene>(Scenes.Meta.NameTag)},
		{ nameof(Scenes.Meta.SoundPlayer),               ResourceLoader.Load<PackedScene>(Scenes.Meta.SoundPlayer)},
		{ nameof(Scenes.Meta.ToastMessage),              ResourceLoader.Load<PackedScene>(Scenes.Meta.ToastMessage)},
		{ nameof(Scenes.Meta.WorldCell),                 ResourceLoader.Load<PackedScene>(Scenes.Meta.WorldCell)},
		#endregion

		#region UI
		{ nameof(Scenes.UI.CraftingRequirementNode),   ResourceLoader.Load<PackedScene>(Scenes.UI.CraftingRequirementNode)},
		{ nameof(Scenes.UI.CraftingTreeCategoryNode),          ResourceLoader.Load<PackedScene>(Scenes.UI.CraftingTreeCategoryNode)},
		{ nameof(Scenes.UI.CraftingTreeRecipeNode),          ResourceLoader.Load<PackedScene>(Scenes.UI.CraftingTreeRecipeNode)},
		{ nameof(Scenes.UI.InventoryCell),             ResourceLoader.Load<PackedScene>(Scenes.UI.InventoryCell)},
		{ nameof(Scenes.UI.TownMarker),                ResourceLoader.Load<PackedScene>(Scenes.UI.TownMarker)},
		{ nameof(Scenes.UI.TransitionControl),         ResourceLoader.Load<PackedScene>(Scenes.UI.TransitionControl)},
		#endregion
		
	};

	public static T Instantiate<T>() where T : Node
	{
		var targetResource = typeof(T).Name;
		var success = resources.TryGetValue(targetResource, out PackedScene targetScene);
		if (success)
		{
			var instance = targetScene.Instance();
			return (T)instance;
		}
		else
		{
			return default(T);
		}
	}
}
