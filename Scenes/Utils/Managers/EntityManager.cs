using Godot;

public static class EntityManager
{
	public static bool SpawnEntityInPosition<T1, T2>(Vector2 position) where T1 : Node2D, IEntity where T2 : EntityData
	{
		var worldCell = Global.Instance.GameWorld.GetWorldCellFromPosition(position, out TileCoordinates _);
		return worldCell.AddEntity<T1, T2>(position, out _);
	}

	public static bool SpawnEntityInPosition<T1, T2>(Vector2 position, out T1 entity) where T1 : Node2D, IEntity where T2 : EntityData
	{
		var worldCell = Global.Instance.GameWorld.GetWorldCellFromPosition(position, out TileCoordinates _);
		return worldCell.AddEntity<T1, T2>(position, out entity);

	}

	public static bool SpawnTileEntityInPosition<T1, T2>(Vector2 position) where T1 : Node2D, ITileEntity where T2 : TileEntityData
	{
		var worldCell = Global.Instance.GameWorld.GetWorldCellFromPosition(position, out TileCoordinates tileCoordinates);
		return worldCell.AddTileEntity<T1, T2>(tileCoordinates, out _);
	}

	public static bool SpawnTileEntityInPosition<T1, T2>(Vector2 position, out T1 tileEntity) where T1 : Node2D, ITileEntity where T2 : TileEntityData
	{
		var worldCell = Global.Instance.GameWorld.GetWorldCellFromPosition(position, out TileCoordinates tileCoordinates);
		return worldCell.AddTileEntity<T1, T2>(tileCoordinates, out tileEntity);
	}

	public static bool SpawnTileEntityInPosition<T1, T2>(CellCoordinates cellCoordinates, TileCoordinates tileCoordinates) where T1 : Node2D, ITileEntity where T2 : TileEntityData
	{
		var worldCell = Global.Instance.GameWorld.GetWorldCell(cellCoordinates);
		return worldCell.AddTileEntity<T1, T2>(tileCoordinates, out _);
	}

	public static bool SpawnTileEntityInPosition<T1, T2>(CellCoordinates cellCoordinates, TileCoordinates tileCoordinates, out T1 tileEntity) where T1 : Node2D, ITileEntity where T2 : TileEntityData
	{
		var worldCell = Global.Instance.GameWorld.GetWorldCell(cellCoordinates);
		return worldCell.AddTileEntity<T1, T2>(tileCoordinates, out tileEntity);
	}

	public static void ClearTileEntityFromPosition(CellCoordinates cellCoordinates, TileCoordinates tileCoordinates)
	{
		var worldCell = Global.Instance.GameWorld.GetWorldCell(cellCoordinates);
		worldCell.RemoveEntity(tileCoordinates);
	}
}
