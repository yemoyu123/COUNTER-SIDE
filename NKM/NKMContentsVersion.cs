using System;
using System.Text.RegularExpressions;

namespace NKM;

public sealed class NKMContentsVersion : IComparable<NKMContentsVersion>
{
	public static NKMContentsVersion MinValue { get; } = new NKMContentsVersion(int.MinValue, int.MinValue, 'a');

	public static NKMContentsVersion MaxValue { get; } = new NKMContentsVersion(int.MaxValue, int.MaxValue, 'z');

	public int First { get; }

	public int Second { get; }

	public char Third { get; }

	public string Literal { get; }

	private NKMContentsVersion(int first, int second, char third)
	{
		First = first;
		Second = second;
		Third = third;
		Literal = $"{first}.{second}.{third}";
	}

	public static NKMContentsVersion Create(string literal)
	{
		Match match = Regex.Match(literal, "\\b([\\d]).([\\d]).([a-z])\\b");
		if (!match.Success)
		{
			return null;
		}
		return new NKMContentsVersion(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), match.Groups[3].Value[0]);
	}

	public static bool TryCreate(string literal, out NKMContentsVersion version)
	{
		Match match = Regex.Match(literal, "\\b([\\d]).([\\d]).([a-z])\\b");
		if (!match.Success)
		{
			version = null;
			return false;
		}
		version = new NKMContentsVersion(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), match.Groups[3].Value[0]);
		return true;
	}

	public static bool operator ==(NKMContentsVersion left, NKMContentsVersion right)
	{
		return left?.Equals(right) ?? ((object)right == null);
	}

	public static bool operator !=(NKMContentsVersion left, NKMContentsVersion right)
	{
		return !(left == right);
	}

	public static bool operator <(NKMContentsVersion left, NKMContentsVersion right)
	{
		if ((object)left != null)
		{
			return left.CompareTo(right) < 0;
		}
		return (object)right != null;
	}

	public static bool operator <=(NKMContentsVersion left, NKMContentsVersion right)
	{
		if ((object)left != null)
		{
			return left.CompareTo(right) <= 0;
		}
		return true;
	}

	public static bool operator >(NKMContentsVersion left, NKMContentsVersion right)
	{
		if ((object)left != null)
		{
			return left.CompareTo(right) > 0;
		}
		return false;
	}

	public static bool operator >=(NKMContentsVersion left, NKMContentsVersion right)
	{
		if ((object)left != null)
		{
			return left.CompareTo(right) >= 0;
		}
		return (object)right == null;
	}

	public override bool Equals(object other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (other is NKMContentsVersion other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(NKMContentsVersion other)
	{
		if (other != null && First == other.First && Second == other.Second && Third == other.Third)
		{
			return Literal == other.Literal;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Literal.GetHashCode();
	}

	public int CompareTo(NKMContentsVersion other)
	{
		if (First != other.First)
		{
			return First.CompareTo(other.First);
		}
		if (Second != other.Second)
		{
			return Second.CompareTo(other.Second);
		}
		return Third.CompareTo(other.Third);
	}

	public override string ToString()
	{
		return Literal;
	}
}
