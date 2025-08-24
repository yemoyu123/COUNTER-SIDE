using UnityEngine;

namespace NKC.FX;

public class NKC_FXM_TWEEN : NKC_FXM_EVALUATER
{
	public Transform Target;

	public NKC_FX_TWEEN.Ease Ease;

	public Transform From;

	public Transform To;

	public Vector3 FromPoint;

	public Vector3 ToPoint;

	private void OnDestroy()
	{
		if (Target != null)
		{
			Target = null;
		}
		if (From != null)
		{
			From = null;
		}
		if (To != null)
		{
			To = null;
		}
	}

	public override void Init()
	{
		if (!init)
		{
			if (Target == null)
			{
				Target = base.transform;
			}
			if (Target != null)
			{
				init = true;
			}
			else
			{
				init = false;
			}
		}
	}

	protected override void OnStart()
	{
		if (From != null)
		{
			FromPoint = From.position;
		}
		if (To != null)
		{
			ToPoint = To.position;
		}
	}

	protected override void OnExecute(bool _render)
	{
		if (base.enabled && (base.IsStarted || base.IsCompleted))
		{
			Target.position = Vector3.LerpUnclamped(FromPoint, ToPoint, NKC_FX_TWEEN.Equations.ChangeFloat(playbackTime, 0f, 1f, Duration, Ease));
		}
	}
}
