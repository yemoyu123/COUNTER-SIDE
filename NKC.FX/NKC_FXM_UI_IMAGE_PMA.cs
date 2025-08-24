using UnityEngine;

namespace NKC.FX;

public class NKC_FXM_UI_IMAGE_PMA : NKC_FXM_UI_IMAGE
{
	public float ColorBoost = 0.5f;

	public bool UseBlendCurve;

	public float BlendFactor = 1f;

	public AnimationCurve Blend;

	private void OnDestroy()
	{
		if (Target != null)
		{
			Target = null;
		}
		if (Targets != null)
		{
			Targets = null;
		}
		if (Blend != null)
		{
			Blend = null;
		}
		if (Curve != null)
		{
			Curve = null;
		}
		if (Sprites != null)
		{
			Sprites = null;
		}
		if (shuffleSprites != null)
		{
			shuffleSprites = null;
		}
		if (Frame != null)
		{
			Frame = null;
		}
	}

	protected override void InitAnimationCurve()
	{
		base.InitAnimationCurve();
		if (Blend == null || Blend.length < 1)
		{
			Blend = InitCurve(1f, 1f);
		}
	}

	protected override Color SetOutputColor(Color _in)
	{
		Color result = default(Color);
		if (UseAlphaCurve)
		{
			result.r = _in.r * _in.a * Evaluate(Curve) * AlphaMultiplier * ColorBoost;
			result.g = _in.g * _in.a * Evaluate(Curve) * AlphaMultiplier * ColorBoost;
			result.b = _in.b * _in.a * Evaluate(Curve) * AlphaMultiplier * ColorBoost;
			if (UseBlendCurve)
			{
				result.a = Mathf.Lerp(0f, 1f * _in.a * Evaluate(Curve) * AlphaMultiplier, BlendFactor * Evaluate(Blend));
			}
			else
			{
				result.a = Mathf.Lerp(0f, 1f * _in.a * Evaluate(Curve) * AlphaMultiplier, BlendFactor);
			}
		}
		else
		{
			result.r = _in.r * _in.a * ColorBoost;
			result.g = _in.g * _in.a * ColorBoost;
			result.b = _in.b * _in.a * ColorBoost;
			if (UseBlendCurve)
			{
				result.a = Mathf.Lerp(0f, 1f * _in.a, BlendFactor * Evaluate(Blend));
			}
			else
			{
				result.a = Mathf.Lerp(0f, 1f * _in.a, BlendFactor);
			}
		}
		return result;
	}
}
