using System;

namespace NKM;

public readonly struct TabId : IEquatable<TabId>
{
	public readonly string Type;

	public readonly int SubIndex;

	public TabId(string type, int subIndex)
	{
		Type = type;
		SubIndex = subIndex;
	}

	public static bool operator ==(TabId left, TabId right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(TabId left, TabId right)
	{
		return !(left == right);
	}

	public override bool Equals(object other)
	{
		if (other == null)
		{
			return false;
		}
		if (other is TabId other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(TabId other)
	{
		if (Type == other.Type)
		{
			return SubIndex == other.SubIndex;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (Type, SubIndex).GetHashCode();
	}

	public override string ToString()
	{
		return $"{Type}[{SubIndex}]";
	}
}
