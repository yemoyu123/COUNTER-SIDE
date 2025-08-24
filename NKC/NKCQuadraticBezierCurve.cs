using UnityEngine;

namespace NKC;

public class NKCQuadraticBezierCurve : IBezierCurve
{
	private Vector3 P0;

	private Vector3 P1;

	private Vector3 P2;

	private NKCQuadraticBezierCurve()
	{
	}

	public NKCQuadraticBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2)
	{
		P0 = p0;
		P1 = p1;
		P2 = p2;
	}

	public Vector3 GetPosition(float t)
	{
		float num = 1f - t;
		return num * num * P0 + 2f * t * num * P1 + t * t * P2;
	}

	public static Vector3 GetPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2)
	{
		float num = 1f - t;
		return num * num * p0 + 2f * t * num * p1 + t * t * p2;
	}
}
