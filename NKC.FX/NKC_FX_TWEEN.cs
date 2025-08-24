using System;
using Cs.Math;
using UnityEngine;

namespace NKC.FX;

public class NKC_FX_TWEEN
{
	public enum Ease
	{
		Linear,
		EaseInQuad,
		EaseOutQuad,
		EaseInOutQuad,
		EaseOutInQuad,
		EaseInCubic,
		EaseOutCubic,
		EaseInOutCubic,
		EaseOutInCubic,
		EaseInQuart,
		EaseOutQuart,
		EaseInOutQuart,
		EaseOutInQuart,
		EaseInQuint,
		EaseOutQuint,
		EaseInOutQuint,
		EaseOutInQuint,
		EaseInSine,
		EaseOutSine,
		EaseInOutSine,
		EaseOutInSine,
		EaseInExpo,
		EaseOutExpo,
		EaseInOutExpo,
		EaseOutInExpo,
		EaseInCirc,
		EaseOutCirc,
		EaseInOutCirc,
		EaseOutInCirc,
		EaseInElastic,
		EaseOutElastic,
		EaseInOutElastic,
		EaseOutInElastic,
		EaseInBack,
		EaseOutBack,
		EaseInOutBack,
		EaseOutInBack,
		EaseInBounce,
		EaseOutBounce,
		EaseInOutBounce,
		EaseOutInBounce
	}

	public class Equations
	{
		private delegate float EaseDelegate(float t, float b, float c, float d);

		private static EaseDelegate[] methods = new EaseDelegate[41]
		{
			EaseNone, EaseInQuad, EaseOutQuad, EaseInOutQuad, EaseOutInQuad, EaseInCubic, EaseOutCubic, EaseInOutCubic, EaseOutInCubic, EaseInQuart,
			EaseOutQuart, EaseInOutQuart, EaseOutInQuart, EaseInQuint, EaseOutQuint, EaseInOutQuint, EaseOutInQuint, EaseInSine, EaseOutSine, EaseInOutSine,
			EaseOutInSine, EaseInExpo, EaseOutExpo, EaseInOutExpo, EaseOutInExpo, EaseInCirc, EaseOutCirc, EaseInOutCirc, EaseOutInCirc, EaseInElastic,
			EaseOutElastic, EaseInOutElastic, EaseOutInElastic, EaseInBack, EaseOutBack, EaseInOutBack, EaseOutInBack, EaseInBounce, EaseOutBounce, EaseInOutBounce,
			EaseOutInBounce
		};

		public static float EaseNone(float t, float b, float c, float d)
		{
			return c * t / d + b;
		}

		public static float EaseInQuad(float t, float b, float c, float d)
		{
			return c * (t /= d) * t + b;
		}

		public static float EaseOutQuad(float t, float b, float c, float d)
		{
			return (0f - c) * (t /= d) * (t - 2f) + b;
		}

		public static float EaseInOutQuad(float t, float b, float c, float d)
		{
			if ((t /= d / 2f) < 1f)
			{
				return c / 2f * t * t + b;
			}
			return (0f - c) / 2f * ((t -= 1f) * (t - 2f) - 1f) + b;
		}

		public static float EaseOutInQuad(float t, float b, float c, float d)
		{
			if (t < d / 2f)
			{
				return EaseOutQuad(t * 2f, b, c / 2f, d);
			}
			return EaseInQuad(t * 2f - d, b + c / 2f, c / 2f, d);
		}

		public static float EaseInCubic(float t, float b, float c, float d)
		{
			return c * (t /= d) * t * t + b;
		}

		public static float EaseOutCubic(float t, float b, float c, float d)
		{
			return c * ((t = t / d - 1f) * t * t + 1f) + b;
		}

		public static float EaseInOutCubic(float t, float b, float c, float d)
		{
			if ((t /= d / 2f) < 1f)
			{
				return c / 2f * t * t * t + b;
			}
			return c / 2f * ((t -= 2f) * t * t + 2f) + b;
		}

		public static float EaseOutInCubic(float t, float b, float c, float d)
		{
			if (t < d / 2f)
			{
				return EaseOutCubic(t * 2f, b, c / 2f, d);
			}
			return EaseInCubic(t * 2f - d, b + c / 2f, c / 2f, d);
		}

		public static float EaseInQuart(float t, float b, float c, float d)
		{
			return c * (t /= d) * t * t * t + b;
		}

		public static float EaseOutQuart(float t, float b, float c, float d)
		{
			return (0f - c) * ((t = t / d - 1f) * t * t * t - 1f) + b;
		}

		public static float EaseInOutQuart(float t, float b, float c, float d)
		{
			if ((t /= d / 2f) < 1f)
			{
				return c / 2f * t * t * t * t + b;
			}
			return (0f - c) / 2f * ((t -= 2f) * t * t * t - 2f) + b;
		}

		public static float EaseOutInQuart(float t, float b, float c, float d)
		{
			if (t < d / 2f)
			{
				return EaseOutQuart(t * 2f, b, c / 2f, d);
			}
			return EaseInQuart(t * 2f - d, b + c / 2f, c / 2f, d);
		}

		public static float EaseInQuint(float t, float b, float c, float d)
		{
			return c * (t /= d) * t * t * t * t + b;
		}

		public static float EaseOutQuint(float t, float b, float c, float d)
		{
			return c * ((t = t / d - 1f) * t * t * t * t + 1f) + b;
		}

		public static float EaseInOutQuint(float t, float b, float c, float d)
		{
			if ((t /= d / 2f) < 1f)
			{
				return c / 2f * t * t * t * t * t + b;
			}
			return c / 2f * ((t -= 2f) * t * t * t * t + 2f) + b;
		}

		public static float EaseOutInQuint(float t, float b, float c, float d)
		{
			if (t < d / 2f)
			{
				return EaseOutQuint(t * 2f, b, c / 2f, d);
			}
			return EaseInQuint(t * 2f - d, b + c / 2f, c / 2f, d);
		}

		public static float EaseInSine(float t, float b, float c, float d)
		{
			return (0f - c) * Mathf.Cos(t / d * ((float)Math.PI / 2f)) + c + b;
		}

		public static float EaseOutSine(float t, float b, float c, float d)
		{
			return c * Mathf.Sin(t / d * ((float)Math.PI / 2f)) + b;
		}

		public static float EaseInOutSine(float t, float b, float c, float d)
		{
			return (0f - c) / 2f * (Mathf.Cos((float)Math.PI * t / d) - 1f) + b;
		}

		public static float EaseOutInSine(float t, float b, float c, float d)
		{
			if (t < d / 2f)
			{
				return EaseOutSine(t * 2f, b, c / 2f, d);
			}
			return EaseInSine(t * 2f - d, b + c / 2f, c / 2f, d);
		}

		public static float EaseInExpo(float t, float b, float c, float d)
		{
			if (!t.IsNearlyZero())
			{
				return c * Mathf.Pow(2f, 10f * (t / d - 1f)) + b - c * 0.001f;
			}
			return b;
		}

		public static float EaseOutExpo(float t, float b, float c, float d)
		{
			if (t != d)
			{
				return c * 1.001f * (0f - Mathf.Pow(2f, -10f * t / d) + 1f) + b;
			}
			return b + c;
		}

		public static float EaseInOutExpo(float t, float b, float c, float d)
		{
			if (t.IsNearlyZero())
			{
				return b;
			}
			if (t == d)
			{
				return b + c;
			}
			if ((t /= d / 2f) < 1f)
			{
				return c / 2f * Mathf.Pow(2f, 10f * (t - 1f)) + b - c * 0.0005f;
			}
			return c / 2f * 1.0005f * (0f - Mathf.Pow(2f, -10f * (t -= 1f)) + 2f) + b;
		}

		public static float EaseOutInExpo(float t, float b, float c, float d)
		{
			if (t < d / 2f)
			{
				return EaseOutExpo(t * 2f, b, c / 2f, d);
			}
			return EaseInExpo(t * 2f - d, b + c / 2f, c / 2f, d);
		}

		public static float EaseInCirc(float t, float b, float c, float d)
		{
			return (0f - c) * (Mathf.Sqrt(1f - (t /= d) * t) - 1f) + b;
		}

		public static float EaseOutCirc(float t, float b, float c, float d)
		{
			return c * Mathf.Sqrt(1f - (t = t / d - 1f) * t) + b;
		}

		public static float EaseInOutCirc(float t, float b, float c, float d)
		{
			if ((t /= d / 2f) < 1f)
			{
				return (0f - c) / 2f * (Mathf.Sqrt(1f - t * t) - 1f) + b;
			}
			return c / 2f * (Mathf.Sqrt(1f - (t -= 2f) * t) + 1f) + b;
		}

		public static float EaseOutInCirc(float t, float b, float c, float d)
		{
			if (t < d / 2f)
			{
				return EaseOutCirc(t * 2f, b, c / 2f, d);
			}
			return EaseInCirc(t * 2f - d, b + c / 2f, c / 2f, d);
		}

		public static float EaseInElastic(float t, float b, float c, float d)
		{
			if (t.IsNearlyZero())
			{
				return b;
			}
			if ((t /= d) == 1f)
			{
				return b + c;
			}
			float num = d * 0.3f;
			float num2 = 0f;
			float num3 = 0f;
			if (num3.IsNearlyZero() || num3 < Mathf.Abs(c))
			{
				num3 = c;
				num2 = num / 4f;
			}
			else
			{
				num2 = num / ((float)Math.PI * 2f) * Mathf.Asin(c / num3);
			}
			return 0f - num3 * Mathf.Pow(2f, 10f * (t -= 1f)) * Mathf.Sin((t * d - num2) * ((float)Math.PI * 2f) / num) + b;
		}

		public static float EaseOutElastic(float t, float b, float c, float d)
		{
			if (t.IsNearlyZero())
			{
				return b;
			}
			if ((t /= d) == 1f)
			{
				return b + c;
			}
			float num = d * 0.3f;
			float num2 = 0f;
			float num3 = 0f;
			if (num3.IsNearlyZero() || num3 < Mathf.Abs(c))
			{
				num3 = c;
				num2 = num / 4f;
			}
			else
			{
				num2 = num / ((float)Math.PI * 2f) * Mathf.Asin(c / num3);
			}
			return num3 * Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * d - num2) * ((float)Math.PI * 2f) / num) + c + b;
		}

		public static float EaseInOutElastic(float t, float b, float c, float d)
		{
			if (t.IsNearlyZero())
			{
				return b;
			}
			if ((t /= d / 2f) == 2f)
			{
				return b + c;
			}
			float num = d * 0.45000002f;
			float num2 = 0f;
			float num3 = 0f;
			if (num3.IsNearlyZero() || num3 < Mathf.Abs(c))
			{
				num3 = c;
				num2 = num / 4f;
			}
			else
			{
				num2 = num / ((float)Math.PI * 2f) * Mathf.Asin(c / num3);
			}
			if (t < 1f)
			{
				return -0.5f * (num3 * Mathf.Pow(2f, 10f * (t -= 1f)) * Mathf.Sin((t * d - num2) * ((float)Math.PI * 2f) / num)) + b;
			}
			return num3 * Mathf.Pow(2f, -10f * (t -= 1f)) * Mathf.Sin((t * d - num2) * ((float)Math.PI * 2f) / num) * 0.5f + c + b;
		}

		public static float EaseOutInElastic(float t, float b, float c, float d)
		{
			if (t < d / 2f)
			{
				return EaseOutElastic(t * 2f, b, c / 2f, d);
			}
			return EaseInElastic(t * 2f - d, b + c / 2f, c / 2f, d);
		}

		public static float EaseInBack(float t, float b, float c, float d)
		{
			float num = 1.70158f;
			return c * (t /= d) * t * ((num + 1f) * t - num) + b;
		}

		public static float EaseOutBack(float t, float b, float c, float d)
		{
			float num = 1.70158f;
			return c * ((t = t / d - 1f) * t * ((num + 1f) * t + num) + 1f) + b;
		}

		public static float EaseInOutBack(float t, float b, float c, float d)
		{
			float num = 1.70158f;
			if ((t /= d / 2f) < 1f)
			{
				return c / 2f * (t * t * (((num *= 1.525f) + 1f) * t - num)) + b;
			}
			return c / 2f * ((t -= 2f) * t * (((num *= 1.525f) + 1f) * t + num) + 2f) + b;
		}

		public static float EaseOutInBack(float t, float b, float c, float d)
		{
			if (t < d / 2f)
			{
				return EaseOutBack(t * 2f, b, c / 2f, d);
			}
			return EaseInBack(t * 2f - d, b + c / 2f, c / 2f, d);
		}

		public static float EaseInBounce(float t, float b, float c, float d)
		{
			return c - EaseOutBounce(d - t, 0f, c, d) + b;
		}

		public static float EaseOutBounce(float t, float b, float c, float d)
		{
			if ((t /= d) < 0.36363637f)
			{
				return c * (7.5625f * t * t) + b;
			}
			if (t < 0.72727275f)
			{
				return c * (7.5625f * (t -= 0.54545456f) * t + 0.75f) + b;
			}
			if (t < 0.90909094f)
			{
				return c * (7.5625f * (t -= 0.8181818f) * t + 0.9375f) + b;
			}
			return c * (7.5625f * (t -= 21f / 22f) * t + 63f / 64f) + b;
		}

		public static float EaseInOutBounce(float t, float b, float c, float d)
		{
			if (t < d / 2f)
			{
				return EaseInBounce(t * 2f, 0f, c, d) * 0.5f + b;
			}
			return EaseOutBounce(t * 2f - d, 0f, c, d) * 0.5f + c * 0.5f + b;
		}

		public static float EaseOutInBounce(float t, float b, float c, float d)
		{
			if (t < d / 2f)
			{
				return EaseOutBounce(t * 2f, b, c / 2f, d);
			}
			return EaseInBounce(t * 2f - d, b + c / 2f, c / 2f, d);
		}

		public static Vector3 ChangeVector(float t, Vector3 b, Vector3 c, float d, Ease ease)
		{
			if (d < 0f || Mathf.Approximately(d, 0f))
			{
				d = 1E-07f;
			}
			float x = methods[(int)ease](t, b.x, c.x, d);
			float y = methods[(int)ease](t, b.y, c.y, d);
			float z = methods[(int)ease](t, b.z, c.z, d);
			return new Vector3(x, y, z);
		}

		public static float ChangeFloat(float t, float b, float c, float d, Ease ease)
		{
			if (d < 0f || Mathf.Approximately(d, 0f))
			{
				d = 1E-07f;
			}
			return methods[(int)ease](t, b, c, d);
		}
	}
}
