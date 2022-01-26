using System;

[Serializable]
public class MiscData : TileEntityData
{
	public override TileEntityType TileEntityType { get { return TileEntityType.Misc; } }
}
