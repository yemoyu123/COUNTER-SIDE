using System;

namespace Cs.Math;

public static class FloatExt
{
	public static bool IsNearlyEqual(this float self, float operand, float epsilon = 1E-05f)
	{
		return System.Math.Abs(self - operand) < epsilon;
	}

	public static bool IsNearlyZero(this float self, float epsilon = 1E-05f)
	{
		return System.Math.Abs(self) < epsilon;
	}

	public static bool IsNearlyEqual(this double self, double operand, double epsilon = 1E-05)
	{
		return System.Math.Abs(self - operand) < epsilon;
	}

	public static bool IsNearlyZero(this double self, double epsilon = 1E-05)
	{
		return System.Math.Abs(self) < epsilon;
	}

	public static float Clamp(this float value, float min, float max)
	{
		if (value < min)
		{
			return min;
		}
		if (value > max)
		{
			return max;
		}
		return value;
	}
}
