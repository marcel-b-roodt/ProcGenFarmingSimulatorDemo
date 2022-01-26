using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bokujou.Code.Models.World
{
	public class CellRiverData
	{
		public TileCoordinates StartPosition { get; set; }
		public List<TileCoordinates> RiverTiles { get; set; }
	}
}
