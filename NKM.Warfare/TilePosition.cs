using System;

namespace NKM.Warfare;

public readonly struct TilePosition : IEquatable<TilePosition>
{
	public byte X { get; }

	public byte Y { get; }

	public short Index { get; }

	public TilePosition(byte x, byte y, short index)
	{
		X = x;
		Y = y;
		Index = index;
	}

	public static bool operator ==(in TilePosition t1, in TilePosition t2)
	{
		if (t1.X == t2.X)
		{
			return t1.Y == t2.Y;
		}
		return false;
	}

	public static bool operator !=(in TilePosition t1, in TilePosition t2)
	{
		if (t1.X == t2.X)
		{
			return t1.Y != t2.Y;
		}
		return true;
	}

	public static TileVector operator -(in TilePosition lhs, in TilePosition rhs)
	{
		return new TileVector((sbyte)(lhs.X - rhs.X), (sbyte)(lhs.Y - rhs.Y));
	}

	public bool IsNeighbor(in TilePosition rhs)
	{
		if (Math.Abs(X - rhs.X) <= 1)
		{
			return Math.Abs(Y - rhs.Y) <= 1;
		}
		return false;
	}

	public double GetLengthTo(in TilePosition rhs)
	{
		int num = X - rhs.X;
		int num2 = Y - rhs.Y;
		return Math.Sqrt(num * num + num2 * num2);
	}

	public override string ToString()
	{
		return $"coord:({X},{Y}) index:{Index}";
	}

	public override bool Equals(object obj)
	{
		if (obj is TilePosition other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(TilePosition other)
	{
		if (X == other.X && Y == other.Y)
		{
			return Index == other.Index;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Index.GetHashCode();
	}
}
