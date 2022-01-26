using System;

[Serializable]
public class WorldCellTileData
{
	public int X { get; set; }
	public int Y { get; set; }

	public WorldTile WorldTile { get { return worldTile; } set { worldTile = value; WorldTileUpdated = true; } }
	private WorldTile worldTile;
	public bool WorldTileUpdated { get; set; }

	public FeatureTile FeatureTile { get { return featureTile; } set { featureTile = value; FeatureTileUpdated = true; } }
	private FeatureTile featureTile;
	public bool FeatureTileUpdated { get; set; }

	public EntityTile EntityTile { get { return entityTile; } set { entityTile = value; EntityTileUpdated = true; } }
	private EntityTile entityTile;
	public ulong TileEntityID { get { return tileEntityID; } private set { tileEntityID = value; EntityTileUpdated = true; } }
	private ulong tileEntityID;
	public bool HasEntity { get { return tileEntityID > 0; } }
	public bool EntityQueued { get { return EntityTile != EntityTile.Unset && !HasEntity; } }
	public bool EntityTileUpdated { get; set; }

	public bool Shrouded { get { return shrouded; } set { shrouded = value; ShroudUpdated = true; } }
	private bool shrouded;
	public bool ShroudUpdated { get; set; }

	public bool Updated { get { return WorldTileUpdated || FeatureTileUpdated || EntityTileUpdated || ShroudUpdated; } } //TODO: Use this to track in CallDeferred whether cells need to be redrawn, as well as which ones

	public void QueueEntityForSpawn(EntityTile entityTile)
	{
		if (EntityTile == EntityTile.Unset)
			EntityTile = entityTile;
	}

	public void UpdateEntity(ulong ID)
	{
		TileEntityID = ID;
		EntityTileUpdated = false;
	}

	public void ClearEntity()
	{
		EntityTile = EntityTile.Unset;
		TileEntityID = 0;
	}
}
