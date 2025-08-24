using UnityEngine;

namespace NKC;

public class NKCCubicBezierCurve : IBezierCurve
{
	private Vector3 P0;

	private Vector3 P1;

	private Vector3 P2;

	private Vector3 P3;

	private NKCCubicBezierCurve()
	{
	}

	public NKCCubicBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		P0 = p0;
		P1 = p1;
		P2 = p2;
		P3 = p3;
	}

	public Vector3 GetPosition(float t)
	{
		float num = 1f - t;
		return num * num * num * P0 + 3f * num * num * t * P1 + 3f * num * t * t * P2 + t * t * t * P3;
	}

	public static Vector3 GetPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		float num = 1f - t;
		return num * num * num * p0 + 3f * num * num * t * p1 + 3f * num * t * t * p2 + t * t * t * p3;
	}
}
