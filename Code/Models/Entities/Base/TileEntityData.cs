using System;

[Serializable]
public abstract class TileEntityData : EntityData
{
	public CellCoordinates CellCoordinates;
	public TileCoordinates TileCoordinates;
	public abstract TileEntityType TileEntityType { get; }
}
