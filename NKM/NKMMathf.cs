using System;

namespace NKM;

public static class NKMMathf
{
	public const float DEG2RAD = (float)Math.PI / 180f;

	public static float Sin(float degree)
	{
		return (float)Math.Sin(degree * ((float)Math.PI / 180f));
	}

	public static float Cos(float degree)
	{
		return (float)Math.Cos(degree * ((float)Math.PI / 180f));
	}

	public static float SinRad(float rad)
	{
		return (float)Math.Sin(rad);
	}

	public static float CosRad(float rad)
	{
		return (float)Math.Cos(rad);
	}

	public static void RotateVector2(float x, float y, float degree, out float xOut, out float yOut)
	{
		float rad = degree * ((float)Math.PI / 180f);
		float num = CosRad(rad);
		float num2 = SinRad(rad);
		xOut = x * num - y * num2;
		yOut = x * num2 + y * num;
	}

	public static int Ceiling(float x)
	{
		return (int)Math.Ceiling(x);
	}

	public static float Max(float a, float b)
	{
		if (!(a > b))
		{
			return b;
		}
		return a;
	}

	public static float Min(float a, float b)
	{
		if (!(a > b))
		{
			return a;
		}
		return b;
	}
}
