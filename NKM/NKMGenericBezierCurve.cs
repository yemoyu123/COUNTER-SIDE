using System;
using System.Collections.Generic;

namespace NKM;

public class NKMGenericBezierCurve : IBezierCurve
{
	private List<NKMVector3> lstPoints;

	private NKMGenericBezierCurve()
	{
	}

	public NKMGenericBezierCurve(params NKMVector3[] points)
	{
		lstPoints = new List<NKMVector3>(points);
	}

	public NKMVector3 GetPosition(float t)
	{
		float num = 1f - t;
		NKMVector3 result = new NKMVector3(0f, 0f, 0f);
		int num2 = lstPoints.Count - 1;
		for (int i = 0; i < lstPoints.Count; i++)
		{
			result += (float)Binomial(num2, i) * Pow(num, num2 - i) * Pow(t, i) * lstPoints[i];
		}
		return result;
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

	public static NKMVector3 GetPosition(float t, params NKMVector3[] points)
	{
		float num = 1f - t;
		NKMVector3 result = new NKMVector3(0f, 0f, 0f);
		int num2 = points.Length - 1;
		for (int i = 0; i < points.Length; i++)
		{
			result += (float)Binomial(num2, i) * (float)Math.Pow(num, num2 - i) * (float)Math.Pow(t, i) * points[i];
		}
		return result;
	}
}
