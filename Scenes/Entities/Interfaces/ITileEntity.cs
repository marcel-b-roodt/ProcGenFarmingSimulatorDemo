public interface ITileEntity : IEntity
{
	CellCoordinates CellCoordinates { get; }
	TileCoordinates TileCoordinates { get; }
}

public enum TileEntityType
{
	Unset = 0,
	Building,
	Crop,
	Misc,
	Tree,
}

