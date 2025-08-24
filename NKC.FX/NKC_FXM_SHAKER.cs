using UnityEngine;

namespace NKC.FX;

public class NKC_FXM_SHAKER : NKC_FXM_EVALUATER
{
	public Transform Target;

	public Transform[] Targets;

	public bool UseMultiTargets;

	public Vector3 PositionOrigin = Vector3.zero;

	public bool Dimension3D;

	[Range(0f, 100f)]
	public float Magnitude = 1f;

	[Range(0f, 30f)]
	public float Frequency = 10f;

	public AnimationCurve Curve;

	public AnimationCurve CurveB;

	private float noiseScroll;

	private float noiseA;

	private float noiseB;

	private float noiseC;

	private float noiseD;

	private float noiseE;

	private float noiseF;

	private Vector3 value;

	private Vector2 offset;

	private void Awake()
	{
		if (Curve == null || Curve.length < 1)
		{
			Curve = InitCurve(1f, 1f);
		}
		if (CurveB == null || CurveB.length < 1)
		{
			CurveB = InitCurve(1f, 1f);
		}
		if (Target == null)
		{
			Target = base.transform;
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
		if (CurveB != null)
		{
			CurveB = null;
		}
	}

	public override void Init()
	{
		noiseScroll = 0f;
		offset.x = GetRandomValue();
		offset.y = GetRandomValue();
		if (Target != null)
		{
			init = true;
			return;
		}
		init = false;
		Debug.LogWarning("Null Target.", base.gameObject);
	}

	public override void SetRandomValue(bool _resimulate)
	{
		if (!base.IsStarted)
		{
			SetNoiseRandom();
		}
		else if (_resimulate)
		{
			SetNoiseRandom();
		}
	}

	protected override void OnExecute(bool _render)
	{
		if (!base.enabled)
		{
			return;
		}
		if (_render)
		{
			if (Magnitude > 0f)
			{
				noiseScroll += 2f * deltaTime * Frequency * Evaluate(CurveB);
				if (Dimension3D)
				{
					value.Set(PositionOrigin.x + GetMagnitude(Magnitude, noiseScroll, noiseA, noiseB), PositionOrigin.y + GetMagnitude(Magnitude, noiseScroll, noiseC, noiseD), PositionOrigin.z + GetMagnitude(Magnitude, noiseScroll, noiseE, noiseF));
				}
				else
				{
					value.Set(PositionOrigin.x + GetMagnitude(Magnitude, noiseScroll, noiseA, noiseB), PositionOrigin.y + GetMagnitude(Magnitude, noiseScroll, noiseC, noiseD), PositionOrigin.z + 0f);
				}
				Target.localPosition = value;
			}
		}
		else
		{
			Target.localPosition = PositionOrigin;
		}
	}

	private float GetRandomValue()
	{
		return Random.Range(-10f, 10f);
	}

	private void SetNoiseRandom()
	{
		noiseA = GetRandomValue();
		noiseB = GetRandomValue();
		noiseC = GetRandomValue();
		noiseD = GetRandomValue();
		noiseE = GetRandomValue();
		noiseF = GetRandomValue();
	}

	private float GetMagnitude(float _power, float _scroll, float _x, float _y)
	{
		_power *= Evaluate(Curve);
		return Mathf.Lerp(0f - _power, _power, Mathf.PerlinNoise(_scroll + _x + offset.x, _scroll + _y + offset.y));
	}
}
