namespace NKM.Warfare;

public readonly struct TileVector
{
	public sbyte X { get; }

	public sbyte Y { get; }

	public TileVector(sbyte x, sbyte y)
	{
		X = x;
		Y = y;
	}

	public static TileVector operator +(in TileVector lhs, in TileVector rhs)
	{
		return new TileVector((sbyte)(lhs.X + rhs.X), (sbyte)(lhs.Y + rhs.Y));
	}

	public static TileVector operator -(in TileVector lhs, in TileVector rhs)
	{
		return new TileVector((sbyte)(lhs.X - rhs.X), (sbyte)(lhs.Y - rhs.Y));
	}

	public override string ToString()
	{
		return $"vector:({X},{Y})";
	}
}
