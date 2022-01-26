using System;
using System.Collections.Generic;

[Serializable]
public class WorldCellRegion
{
	public int ID { get; set; }
	public WorldTile WorldTile { get; set; }
	public List<TileCoordinates> Tiles = new List<TileCoordinates>();
}