using UnityEngine;
using UnityEngine.UI;

namespace NKC.FX;

public class NKC_FXM_UI_IMAGE : NKC_FXM_EVALUATER
{
	public Image Target;

	public Image[] Targets;

	public bool UseMultiTargets;

	public ParticleSystem.MinMaxGradient MinMaxGradient = new ParticleSystem.MinMaxGradient(Color.white);

	public bool UseAlphaCurve;

	public float AlphaMultiplier = 1f;

	public AnimationCurve Curve;

	public bool UseSheetAnimation;

	public bool RandomSprite;

	public bool ShuffleStart;

	public bool ShuffleArray;

	public Sprite[] Sprites;

	protected Sprite[] shuffleSprites;

	public AnimationCurve Frame;

	protected Color color;

	protected float colorSeed = 0.5f;

	protected int currentIndex;

	private void Awake()
	{
		InitAnimationCurve();
		if (Target == null)
		{
			Target = GetComponent<Image>();
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

	protected virtual void InitAnimationCurve()
	{
		if (Curve == null || Curve.length < 1)
		{
			Curve = InitCurve(1f, 1f);
		}
		if (Frame == null || Frame.length < 1)
		{
			Frame = InitCurveLinear(0f, 1f);
		}
	}

	public override void Init()
	{
		if (UseMultiTargets)
		{
			Target = null;
			if (Targets.Length != 0)
			{
				for (int i = 0; i < Targets.Length; i++)
				{
					init = ValidTarget(Targets[i]);
				}
			}
			else
			{
				init = false;
			}
		}
		else
		{
			init = ValidTarget(Target);
		}
		if (Sprites != null && Sprites.Length != 0 && ShuffleArray)
		{
			shuffleSprites = (Sprite[])Sprites.Clone();
		}
	}

	private bool ValidTarget(Image _t)
	{
		bool flag = false;
		if (_t != null)
		{
			_t.raycastTarget = false;
			_t.canvasRenderer.cullTransparentMesh = false;
			if (_t.sprite != null)
			{
				_t.useSpriteMesh = true;
				flag = true;
			}
			else
			{
				flag = false;
				Debug.LogWarning("Image Sprite not found -> " + _t.transform.name + " :: " + _t.transform.root.name, _t.gameObject);
			}
		}
		else
		{
			flag = false;
			Debug.LogWarning("Image not found : null image from  -> " + base.transform.name + " :: " + base.transform.root.name, base.gameObject);
		}
		return flag;
	}

	protected override void OnExecute(bool _render)
	{
		if (base.enabled)
		{
			ExecuteCurrentSprite();
			ExecuteCurrentColor(_render);
		}
	}

	private void ExecuteCurrentSprite()
	{
		if (Sprites == null || Sprites.Length == 0)
		{
			return;
		}
		if (RandomSprite)
		{
			if (!UseMultiTargets)
			{
				Target.sprite = Sprites[currentIndex];
				return;
			}
			for (int i = 0; i < Targets.Length; i++)
			{
				Targets[i].sprite = Sprites[currentIndex];
			}
			return;
		}
		int num = 0;
		num = ((!ShuffleStart) ? ((int)Evaluate(Frame)) : ((int)Mathf.Repeat(Evaluate(Frame) + (float)currentIndex, Sprites.Length)));
		num = Mathf.Clamp(num, 0, Sprites.Length - 1);
		if (!UseMultiTargets)
		{
			if (ShuffleArray)
			{
				Target.sprite = shuffleSprites[num];
			}
			else
			{
				Target.sprite = Sprites[num];
			}
			return;
		}
		for (int j = 0; j < Targets.Length; j++)
		{
			if (ShuffleArray)
			{
				Targets[j].sprite = shuffleSprites[num];
			}
			else
			{
				Targets[j].sprite = Sprites[num];
			}
		}
	}

	private void ExecuteCurrentColor(bool _render)
	{
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
			Target.enabled = true;
			if (_render)
			{
				Target.color = color;
			}
			else
			{
				Target.color = Color.clear;
			}
			return;
		}
		for (int i = 0; i < Targets.Length; i++)
		{
			Targets[i].enabled = true;
			if (_render)
			{
				Targets[i].color = color;
			}
			else
			{
				Targets[i].color = Color.clear;
			}
		}
	}

	protected virtual Color SetOutputColor(Color _in)
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

	public override void SetRandomValue(bool _resimulate)
	{
		if (!_resimulate || !base.enabled || !init)
		{
			return;
		}
		colorSeed = Random.value;
		if (Sprites != null && Sprites.Length != 0)
		{
			currentIndex = Random.Range(0, Sprites.Length);
			if (ShuffleArray)
			{
				Shuffle(shuffleSprites);
			}
		}
	}
}
