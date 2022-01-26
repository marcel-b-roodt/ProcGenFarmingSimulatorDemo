using System;

[Serializable]
public struct TileCoordinates
{
	public int X { get; set; }
	public int Y { get; set; }

	public TileCoordinates(int x, int y)
	{
		X = x;
		Y = y;
	}

	public override string ToString()
	{
		return $"({X},{Y})";
	}

	public override bool Equals(object obj)
	{
		if (!(obj is TileCoordinates))
		{
			return false;
		}

		var coordinates = (TileCoordinates)obj;
		return X.Equals(coordinates.X) &&
			   Y.Equals(coordinates.Y);
	}

	public override int GetHashCode()
	{
		return X.GetHashCode() ^ Y.GetHashCode();
	}

	public static bool operator ==(TileCoordinates c1, TileCoordinates c2)
	{
		return c1.X == c2.X && c1.Y == c2.Y;
	}

	public static bool operator !=(TileCoordinates c1, TileCoordinates c2)
	{
		return c1.X != c2.X || c1.Y != c2.Y;
	}
}
