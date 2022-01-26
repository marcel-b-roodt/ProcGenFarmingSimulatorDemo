using System;

[Serializable]
public class BuildingData : TileEntityData
{
	public override TileEntityType TileEntityType { get { return TileEntityType.Building; } }
}
