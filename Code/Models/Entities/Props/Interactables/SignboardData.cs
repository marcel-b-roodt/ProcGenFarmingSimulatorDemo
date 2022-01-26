using System;

[Serializable]
public class SignboardData : TileEntityData
{
	public override TileEntityType TileEntityType { get { return TileEntityType.Building; } }
}
