using UnityEngine;
using UnityEngine.UI;

namespace NKC.FX;

public class NKC_FXM_UI_TEXT : NKC_FXM_EVALUATER
{
	public Text Target;

	public Text[] Targets;

	public bool UseMultiTargets;

	public ParticleSystem.MinMaxGradient MinMaxGradient = new ParticleSystem.MinMaxGradient(Color.white);

	public bool UseAlphaCurve;

	public float AlphaMultiplier = 1f;

	public AnimationCurve Curve;

	private Color color;

	private float colorSeed = 0.5f;

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
		if (Curve == null || Curve.length < 1)
		{
			Curve = InitCurve(1f, 1f);
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
				Debug.LogWarning("TextMeshPro(s) not found. -> " + base.transform.name, base.gameObject);
			}
		}
		else if (Target == null)
		{
			Text component = GetComponent<Text>();
			if (component != null)
			{
				Target = component;
				init = true;
			}
			else
			{
				init = false;
				Debug.LogWarning("Text(UI) not found -> " + base.transform.name, base.gameObject);
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
		switch (MinMaxGradient.mode)
		{
		case ParticleSystemGradientMode.Color:
			color = SetOutputColor(MinMaxGradient.color);
			break;
		case ParticleSystemGradientMode.Gradient:
			color = SetOutputColor(Evaluate(MinMaxGradient.gradient));
			break;
		case ParticleSystemGradientMode.TwoColors:
			color = SetOutputColor(Color.Lerp(MinMaxGradient.colorMin, MinMaxGradient.colorMax, colorSeed));
			break;
		case ParticleSystemGradientMode.TwoGradients:
			color = SetOutputColor(Color.Lerp(Evaluate(MinMaxGradient.gradientMin), Evaluate(MinMaxGradient.gradientMax), colorSeed));
			break;
		case ParticleSystemGradientMode.RandomColor:
			color = SetOutputColor(MinMaxGradient.gradient.Evaluate(colorSeed));
			break;
		}
		if (!UseMultiTargets)
		{
			Target.color = color;
			Target.enabled = _render;
			return;
		}
		for (int i = 0; i < Targets.Length; i++)
		{
			Targets[i].color = color;
			Targets[i].enabled = _render;
		}
	}

	private Color SetOutputColor(Color _in)
	{
		Color result = _in;
		if (UseAlphaCurve)
		{
			result.a *= Evaluate(Curve) * AlphaMultiplier;
		}
		else
		{
			result.a *= AlphaMultiplier;
		}
		return result;
	}
}
