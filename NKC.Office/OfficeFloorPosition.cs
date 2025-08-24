using System;

namespace NKC.Office;

public struct OfficeFloorPosition : IEquatable<OfficeFloorPosition>
{
	public int x;

	public int y;

	public (int, int) ToPair => (x, y);

	public OfficeFloorPosition(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public OfficeFloorPosition((int, int) pos)
	{
		(x, y) = pos;
	}

	public static OfficeFloorPosition operator +(OfficeFloorPosition a)
	{
		return a;
	}

	public static OfficeFloorPosition operator -(OfficeFloorPosition a)
	{
		return new OfficeFloorPosition(-a.x, -a.y);
	}

	public static OfficeFloorPosition operator +(OfficeFloorPosition a, OfficeFloorPosition b)
	{
		return new OfficeFloorPosition(a.x + b.x, a.y + b.y);
	}

	public static OfficeFloorPosition operator -(OfficeFloorPosition a, OfficeFloorPosition b)
	{
		return new OfficeFloorPosition(a.x - b.x, a.y - b.y);
	}

	bool IEquatable<OfficeFloorPosition>.Equals(OfficeFloorPosition other)
	{
		if (x == other.x)
		{
			return y == other.y;
		}
		return false;
	}

	public override string ToString()
	{
		return $"({x}, {y})";
	}
}
