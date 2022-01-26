using System;

[Serializable]
public class TreeData : TileEntityData
{
	public int ChopHealth;

	public override TileEntityType TileEntityType { get { return TileEntityType.Tree; } }
}
