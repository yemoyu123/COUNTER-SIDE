using UnityEngine;

namespace NKC.FX;

public class NKC_FXM_TRAIL_RENDERER : NKC_FXM_EVALUATER
{
	public float Width = 1f;

	public AnimationCurve Curve;

	private TrailRenderer rend;

	private void OnDestroy()
	{
		if (Curve != null)
		{
			Curve = null;
		}
		if (rend != null)
		{
			rend = null;
		}
	}

	public override void Init()
	{
		if (!init)
		{
			if (rend == null)
			{
				rend = GetComponent<TrailRenderer>();
			}
			if (Curve == null || Curve.length < 1)
			{
				Curve = InitCurve(1f, 1f);
			}
			init = true;
		}
	}

	protected override void OnExecute(bool _render)
	{
		if (base.enabled)
		{
			rend.widthMultiplier = Evaluate(Curve) * Width;
			rend.emitting = _render;
		}
	}
}
