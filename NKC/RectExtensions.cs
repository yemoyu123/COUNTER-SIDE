using UnityEngine;

namespace NKC;

public static class RectExtensions
{
	public static Rect Union(this Rect RA, Rect RB)
	{
		return new Rect
		{
			min = Vector2.Min(RA.min, RB.min),
			max = Vector2.Max(RA.max, RB.max)
		};
	}
}
