using UnityEngine;

namespace NKC.FX;

public class NKC_FXM_SPRITE : NKC_FXM_EVALUATER
{
	public SpriteRenderer Target;

	public SpriteRenderer[] Targets;

	public bool UseMultiTargets;

	public float Intensity = 1f;

	public ParticleSystem.MinMaxGradient MinMaxGradient = new ParticleSystem.MinMaxGradient(Color.white);

	public bool UseAlphaCurve;

	public AnimationCurve Alpha;

	public bool UseSheetAnimation;

	public bool RandomSprite;

	public bool ShuffleStart;

	public bool ShuffleArray;

	public bool RandomFlipX;

	public bool RandomFlipY;

	public Sprite[] Sprites;

	private Sprite[] shuffleSprites;

	public AnimationCurve Frame;

	private Color color;

	private float colorSeed = 0.5f;

	private int currentIndex;

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
		if (Alpha != null)
		{
			Alpha = null;
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

	private void Awake()
	{
		if (Alpha == null || Alpha.length < 1)
		{
			Alpha = InitCurve(1f, 1f);
		}
		if (Frame == null || Frame.length < 1)
		{
			Frame = InitCurveLinear(0f, 1f);
		}
		if (Target == null)
		{
			Target = GetComponent<SpriteRenderer>();
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

	private bool ValidTarget(SpriteRenderer _t)
	{
		bool flag = false;
		if (_t != null)
		{
			if (_t.sprite != null)
			{
				if (_t.sharedMaterial != null)
				{
					flag = true;
				}
				else
				{
					flag = false;
					Debug.LogWarning("Sprite Renderer Material not found -> " + _t.transform.name + " :: " + _t.transform.root, _t.gameObject);
				}
			}
			else
			{
				flag = false;
				Debug.LogWarning("Sprite Renderer Sprite not found -> " + _t.transform.name + " :: " + _t.transform.root, _t.gameObject);
			}
		}
		else
		{
			flag = false;
			Debug.LogWarning("Sprite Renderer not found : null renderer from " + base.gameObject.name);
		}
		return flag;
	}

	protected override void OnExecute(bool _render)
	{
		if (!base.enabled)
		{
			return;
		}
		if (Sprites != null && Sprites.Length != 0)
		{
			if (RandomSprite)
			{
				if (!UseMultiTargets)
				{
					Target.sprite = Sprites[currentIndex];
				}
				else
				{
					for (int i = 0; i < Targets.Length; i++)
					{
						Targets[i].sprite = Sprites[currentIndex];
					}
				}
			}
			else
			{
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
				}
				else
				{
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
			}
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
		for (int k = 0; k < Targets.Length; k++)
		{
			if (Targets[k] != null)
			{
				Targets[k].color = color;
				Targets[k].enabled = _render;
			}
		}
	}

	private Color SetOutputColor(Color _in)
	{
		Color result = default(Color);
		if (UseAlphaCurve)
		{
			result.r = _in.r * Intensity;
			result.g = _in.g * Intensity;
			result.b = _in.b * Intensity;
			result.a = _in.a * Intensity * Evaluate(Alpha);
			return result;
		}
		return _in * Intensity;
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
		if (RandomFlipX || RandomFlipY)
		{
			SetRandomFlip();
		}
	}

	private void SetRandomFlip()
	{
		if (!base.enabled || !init)
		{
			return;
		}
		if (!UseMultiTargets)
		{
			if (RandomFlipX)
			{
				Target.flipX = Random.value > 0.5f;
			}
			if (RandomFlipY)
			{
				Target.flipY = Random.value > 0.5f;
			}
			return;
		}
		for (int i = 0; i < Targets.Length; i++)
		{
			if (RandomFlipX)
			{
				Targets[i].flipX = Random.value > 0.5f;
			}
			if (RandomFlipY)
			{
				Targets[i].flipY = Random.value > 0.5f;
			}
		}
	}

	public void SetForceFlip()
	{
		if (base.enabled && init)
		{
			if (!UseMultiTargets)
			{
				Target.flipX = RandomFlipX;
				Target.flipY = RandomFlipY;
				return;
			}
			for (int i = 0; i < Targets.Length; i++)
			{
				Targets[i].flipX = RandomFlipX;
				Targets[i].flipY = RandomFlipY;
			}
		}
		else
		{
			Debug.LogWarning("Invalid Initialized -> " + base.transform.name, base.gameObject);
		}
	}
}
