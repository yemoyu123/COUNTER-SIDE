using System.Collections.Generic;
using UnityEngine;

namespace NKC;

public class NKCGenericBezierCurve : IBezierCurve
{
	private List<Vector3> lstPoints;

	private NKCGenericBezierCurve()
	{
	}

	public NKCGenericBezierCurve(params Vector3[] points)
	{
		lstPoints = new List<Vector3>(points);
	}

	public Vector3 GetPosition(float t)
	{
		float num = 1f - t;
		Vector3 zero = Vector3.zero;
		int num2 = lstPoints.Count - 1;
		for (int i = 0; i < lstPoints.Count; i++)
		{
			zero += (float)Binomial(num2, i) * Pow(num, num2 - i) * Pow(t, i) * lstPoints[i];
		}
		return zero;
	}

	private static int Binomial(int n, int k)
	{
		int num = 1;
		for (int i = 1; i <= k; i++)
		{
			num *= n - (k - i);
			num /= i;
		}
		return num;
	}

	private static float Pow(float num, int exp)
	{
		float num2 = 1f;
		while (exp > 0)
		{
			if (exp % 2 == 1)
			{
				num2 *= num;
			}
			exp >>= 1;
			num *= num;
		}
		return num2;
	}

	public static Vector3 GetPosition(float t, params Vector3[] points)
	{
		float num = 1f - t;
		Vector3 zero = Vector3.zero;
		int num2 = points.Length - 1;
		for (int i = 0; i < points.Length; i++)
		{
			zero += (float)Binomial(num2, i) * Pow(num, num2 - i) * Pow(t, i) * points[i];
		}
		return zero;
	}
}
