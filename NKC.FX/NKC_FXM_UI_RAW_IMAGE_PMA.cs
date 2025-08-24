using UnityEngine;
using UnityEngine.UI;

namespace NKC.FX;

public class NKC_FXM_UI_RAW_IMAGE_PMA : NKC_FXM_EVALUATER
{
	public RawImage Target;

	public RawImage[] Targets;

	public bool UseMultiTargets;

	public float ColorBoost = 0.5f;

	public bool UseBlendCurve;

	public float BlendFactor = 1f;

	public AnimationCurve Blend;

	public ParticleSystem.MinMaxGradient MinMaxGradient = new ParticleSystem.MinMaxGradient(Color.white);

	public bool UseAlphaCurve;

	public float AlphaMultiplier = 1f;

	public AnimationCurve Curve;

	public bool UseColorAnimation = true;

	public bool UseUvAnimation;

	public UVAnimation UvMode;

	public float UVRectX;

	public float UVRectY;

	public float UVRectWidth = 1f;

	public float UVRectHeight = 1f;

	public float UVRectScrollX;

	public float UVRectScrollY;

	public int StartFrame;

	public int EndFrame = 1;

	public AnimationCurve Frame;

	private Color color;

	private float colorSeed = 0.5f;

	private int currentIndex;

	private Rect rect;

	private Vector4 vec4Animate;

	private void Awake()
	{
		if (Blend == null || Blend.length < 1)
		{
			Blend = InitCurve(1f, 1f);
		}
		if (Curve == null || Curve.length < 1)
		{
			Curve = InitCurve(1f, 1f);
		}
		if (Frame == null || Frame.length < 1)
		{
			Frame = InitCurveLinear(0f, 1f);
		}
		if (Target == null)
		{
			Target = GetComponent<RawImage>();
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
		if (Blend != null)
		{
			Blend = null;
		}
		if (Curve != null)
		{
			Curve = null;
		}
		if (Frame != null)
		{
			Frame = null;
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
	}

	private bool ValidTarget(RawImage _t)
	{
		bool flag = false;
		if (_t != null)
		{
			_t.raycastTarget = false;
			_t.canvasRenderer.cullTransparentMesh = false;
			if (_t.texture != null)
			{
				if (_t.material != null)
				{
					flag = true;
				}
				else if (_t.defaultMaterial != null)
				{
					flag = true;
				}
				else
				{
					flag = false;
					Debug.LogWarning("RawImage Material not found -> " + _t.transform.name + " :: " + _t.transform.root, _t.gameObject);
				}
			}
			else
			{
				flag = false;
				Debug.LogWarning("RawImage Texture not found -> " + _t.transform.name + " :: " + _t.transform.root, _t.gameObject);
			}
		}
		else
		{
			flag = false;
			Debug.LogWarning("RawImage not found : null reference from " + base.gameObject.name);
		}
		return flag;
	}

	protected override void OnExecute(bool _render)
	{
		if (!base.enabled)
		{
			return;
		}
		if (UseUvAnimation)
		{
			switch (UvMode)
			{
			case UVAnimation.ScrollAnimation:
				if (_render)
				{
					rect.x += UVRectScrollX * deltaTime;
					rect.y += UVRectScrollY * deltaTime;
					rect.width = UVRectWidth;
					rect.height = UVRectHeight;
					Target.uvRect = rect;
				}
				else
				{
					rect.x = UVRectX;
					rect.y = UVRectY;
					rect.width = UVRectWidth;
					rect.height = UVRectHeight;
					Target.uvRect = rect;
				}
				break;
			case UVAnimation.TextureSheetIndex:
				vec4Animate = AnimateTextureSheet(StartFrame, (int)UVRectWidth, (int)UVRectHeight);
				rect.x = vec4Animate.z;
				rect.y = vec4Animate.w;
				rect.width = vec4Animate.x;
				rect.height = vec4Animate.y;
				Target.uvRect = rect;
				break;
			case UVAnimation.TextureSheetAnimation:
				if (_render)
				{
					currentIndex = (int)Mathf.Lerp(StartFrame, Mathf.Clamp(EndFrame, 0f, UVRectWidth * UVRectHeight), Evaluate(Frame));
					vec4Animate = AnimateTextureSheet(currentIndex, (int)UVRectWidth, (int)UVRectHeight);
				}
				else
				{
					vec4Animate = AnimateTextureSheet(StartFrame, (int)UVRectWidth, (int)UVRectHeight);
				}
				rect.x = vec4Animate.z;
				rect.y = vec4Animate.w;
				rect.width = vec4Animate.x;
				rect.height = vec4Animate.y;
				Target.uvRect = rect;
				break;
			}
		}
		if (!UseColorAnimation)
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

	private Vector4 AnimateTextureSheet(int _count, int _scaleX, int _scaleY)
	{
		return new Vector4
		{
			x = 1f / (float)_scaleX,
			y = 1f / (float)_scaleY,
			z = (float)_count / (float)_scaleX - (float)(_count / _scaleX),
			w = 1f - 1f / (float)_scaleY - (float)(_count / _scaleX) / (float)_scaleY
		};
	}

	private Color SetOutputColor(Color _in)
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

	public override void SetRandomValue(bool _resimulate)
	{
		if (_resimulate && base.enabled && init)
		{
			colorSeed = Random.value;
		}
	}
}
