using TMPro;
using UnityEngine;

namespace NKC.FX;

public class NKC_FXM_TMPRO : NKC_FXM_EVALUATER
{
	public TextMeshPro Target;

	public TextMeshPro[] Targets;

	public bool UseMultiTargets;

	public Gradient Gradient;

	public bool UseIntensity;

	public AnimationCurve Intensity;

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
		if (Gradient != null)
		{
			Gradient = null;
		}
		if (Intensity != null)
		{
			Intensity = null;
		}
	}

	public override void Init()
	{
		if (init)
		{
			return;
		}
		if (Intensity == null || Intensity.length < 1)
		{
			Intensity = InitCurve(1f, 1f);
		}
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
				Debug.LogWarning("TextMeshPro(s) not found -> " + base.transform.name, base.gameObject);
			}
		}
		else if (Target == null)
		{
			TextMeshPro component = GetComponent<TextMeshPro>();
			if (component != null)
			{
				Target = component;
				init = true;
			}
			else
			{
				init = false;
				Debug.LogWarning("TextMeshPro not found -> " + base.transform.name, base.gameObject);
			}
		}
		else
		{
			init = true;
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
			if (string.IsNullOrEmpty(Target.text))
			{
				Target.enabled = false;
			}
			else
			{
				SetTMPro(Target, _render);
			}
			return;
		}
		for (int i = 0; i < Targets.Length; i++)
		{
			if (string.IsNullOrEmpty(Targets[i].text))
			{
				Targets[i].enabled = false;
			}
			else
			{
				SetTMPro(Targets[i], _render);
			}
		}
	}

	private void SetTMPro(TextMeshPro _tmpro, bool _render)
	{
		if (UseIntensity)
		{
			_tmpro.color = Evaluate(Gradient) * Evaluate(Intensity);
		}
		else
		{
			_tmpro.color = Evaluate(Gradient);
		}
		Target.enabled = _render;
	}
}
