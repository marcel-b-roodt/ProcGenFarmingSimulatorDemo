using Godot;

public static class CropManager
{
	public static bool PlaceSoilAtTile(Vector2 position)
	{
		var worldCell = Global.Instance.GameWorld.GetWorldCellFromPosition(position, out TileCoordinates tileCoordinates);
		return worldCell.ToggleSoilAtPosition(tileCoordinates);
	}

	public static bool PlaceCropAtTile<T1, T2>(Vector2 position) where T1 : Node2D, ICrop where T2 : CropData
	{
		var worldCell = Global.Instance.GameWorld.GetWorldCellFromPosition(position, out TileCoordinates tileCoordinates);
		return worldCell.AddCropAtPosition<T1, T2>(tileCoordinates);
	}

	public static bool PlaceCropAtTile(InventoryCropSeedType cropType, Vector2 position)
	{
		var worldCell = Global.Instance.GameWorld.GetWorldCellFromPosition(position, out TileCoordinates tileCoordinates);

		switch (cropType)
		{
			case InventoryCropSeedType.Artichoke: return worldCell.AddCropAtPosition<ArtichokeCropEntity, ArtichokeCropData>(tileCoordinates);
			case InventoryCropSeedType.Carrot: return worldCell.AddCropAtPosition<CarrotCropEntity, CarrotCropData>(tileCoordinates);
			case InventoryCropSeedType.Corn: return worldCell.AddCropAtPosition<CornCropEntity, CornCropData>(tileCoordinates);
			case InventoryCropSeedType.Gourd: return worldCell.AddCropAtPosition<GourdCropEntity, GourdCropData>(tileCoordinates);
			case InventoryCropSeedType.Potato: return worldCell.AddCropAtPosition<PotatoCropEntity, PotatoCropData>(tileCoordinates);
			case InventoryCropSeedType.Tomato: return worldCell.AddCropAtPosition<TomatoCropEntity, TomatoCropData>(tileCoordinates);
		}

		return false;
	}
}
