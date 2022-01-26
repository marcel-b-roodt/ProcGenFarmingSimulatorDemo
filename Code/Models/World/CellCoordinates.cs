using System;

[Serializable]
public struct CellCoordinates
{
	public int X;
	public int Y;

	public CellCoordinates(int x, int y)
	{
		this.X = x;
		this.Y = y;
	}

	public override string ToString()
	{
		return $"({X},{Y})";
	}

	public override bool Equals(object obj)
	{
		if (!(obj is CellCoordinates))
		{
			return false;
		}

		var coordinates = (CellCoordinates)obj;
		return X.Equals(coordinates.X) &&
			   Y.Equals(coordinates.Y);
	}

	public override int GetHashCode()
	{
		return X.GetHashCode() ^ Y.GetHashCode();
	}

	public static bool operator == (CellCoordinates c1, CellCoordinates c2)
	{
		return c1.X == c2.X && c1.Y == c2.Y;
	}

	public static bool operator !=(CellCoordinates c1, CellCoordinates c2)
	{
		return c1.X != c2.X || c1.Y != c2.Y;
	}
}
