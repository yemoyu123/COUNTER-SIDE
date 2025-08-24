namespace NKM;

public class NKMAStarSearchLocation
{
	public readonly int x;

	public readonly int y;

	public NKMAStarSearchLocation(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public NKMAStarSearchLocation(float x, float y)
	{
		this.x = (int)x;
		this.y = (int)y;
	}

	public override bool Equals(object obj)
	{
		NKMAStarSearchLocation nKMAStarSearchLocation = obj as NKMAStarSearchLocation;
		if (x == nKMAStarSearchLocation.x)
		{
			return y == nKMAStarSearchLocation.y;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (x * 597) ^ (y * 1173);
	}
}
