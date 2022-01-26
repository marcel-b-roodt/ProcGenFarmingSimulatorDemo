using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bokujou.Code.Models.World
{
	public class CellLakeData
	{
		public TileCoordinates StartPosition { get; set; }
		public float LakeMaxHeight { get; set; }
	}
}
