using UnityEngine;

namespace NKC.FX;

public class NKC_FXM_LINE_RENDERER : NKC_FXM_EVALUATER
{
	public LineRenderer Target;

	public LineRenderer[] Targets;

	public bool UseMultiTargets;

	public float Width = 1f;

	public AnimationCurve Curve;

	public bool AssignRender;

	private void Awake()
	{
		if (Curve == null || Curve.length < 1)
		{
			Curve = InitCurve(1f, 1f);
		}
		if (Target == null)
		{
			Target = GetComponent<LineRenderer>();
		}
	}

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
		if (Curve != null)
		{
			Curve = null;
		}
	}

	public override void Init()
	{
		if (UseMultiTargets)
		{
			if (Targets.Length != 0)
			{
				if (Target == null)
				{
					Target = Targets[0];
				}
				init = true;
			}
			else
			{
				init = false;
				Debug.LogWarning("Line Renderer(s) not found -> " + base.transform.name, base.gameObject);
			}
		}
		else if (Target != null)
		{
			init = true;
		}
		else
		{
			init = false;
			Debug.LogWarning("Line Renderer not found -> " + base.transform.name, base.gameObject);
		}
	}

	protected override void OnExecute(bool _render)
	{
		if (!base.enabled)
		{
			return;
		}
		if (!UseMultiTargets)
		{
			Target.widthMultiplier = Evaluate(Curve) * Width;
			Target.enabled = _render;
			return;
		}
		for (int i = 0; i < Targets.Length; i++)
		{
			Targets[i].widthMultiplier = Evaluate(Curve) * Width;
			Targets[i].enabled = _render;
		}
	}
}
