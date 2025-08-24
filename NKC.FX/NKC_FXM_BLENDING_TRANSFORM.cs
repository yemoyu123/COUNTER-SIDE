using UnityEngine;

namespace NKC.FX;

public class NKC_FXM_BLENDING_TRANSFORM : NKC_FXM_EVALUATER
{
	public Transform Source;

	public Transform Destination;

	public bool EnablePosition;

	public bool EnableRotation;

	public bool IsLinearBlendPosition;

	public bool IsLinearBlendRotation;

	[Range(0f, 1f)]
	public float BlendPosition;

	[Range(0f, 1f)]
	public float BlendRotation;

	public AnimationCurve CurvePosition;

	public AnimationCurve CurveRotation;

	private void OnDestroy()
	{
		if (Source != null)
		{
			Source = null;
		}
		if (Destination != null)
		{
			Destination = null;
		}
		if (CurvePosition != null)
		{
			CurvePosition = null;
		}
		if (CurveRotation != null)
		{
			CurveRotation = null;
		}
	}

	public override void Init()
	{
		if (!init)
		{
			if (CurvePosition == null || CurvePosition.length < 1)
			{
				CurvePosition = InitCurve(0f, 0f);
			}
			if (CurveRotation == null || CurveRotation.length < 1)
			{
				CurveRotation = InitCurve(0f, 0f);
			}
			if (Source != null && Destination != null)
			{
				init = true;
			}
			else
			{
				init = false;
			}
		}
	}

	protected override void OnExecute(bool _render)
	{
		if (!base.enabled || !_render)
		{
			return;
		}
		if (EnablePosition)
		{
			if (IsLinearBlendPosition)
			{
				base.transform.position = Vector3.Lerp(Source.position, Destination.position, BlendPosition * Evaluate(CurvePosition));
			}
			else
			{
				base.transform.position = Vector3.Slerp(Source.position, Destination.position, BlendPosition * Evaluate(CurvePosition));
			}
		}
		if (EnableRotation)
		{
			if (IsLinearBlendRotation)
			{
				base.transform.rotation = Quaternion.Lerp(Source.rotation, Destination.rotation, BlendRotation * Evaluate(CurveRotation));
			}
			else
			{
				base.transform.rotation = Quaternion.Slerp(Source.rotation, Destination.rotation, BlendRotation * Evaluate(CurveRotation));
			}
		}
	}
}
