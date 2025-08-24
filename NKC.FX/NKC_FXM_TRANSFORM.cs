using UnityEngine;

namespace NKC.FX;

public class NKC_FXM_TRANSFORM : NKC_FXM_EVALUATER
{
	public Transform Target;

	public Transform[] Targets;

	[HideInInspector]
	public bool UseMultiTargets;

	[HideInInspector]
	public bool UseMultiRandom;

	[HideInInspector]
	public bool SeparateAxes;

	[HideInInspector]
	public bool ExpandRange;

	[HideInInspector]
	public float FactorX = 1f;

	[HideInInspector]
	public float FactorY = 1f;

	[HideInInspector]
	public float FactorZ = 1f;

	[HideInInspector]
	public float minX;

	[HideInInspector]
	public float minY;

	[HideInInspector]
	public float minZ;

	[HideInInspector]
	public float maxX = 1f;

	[HideInInspector]
	public float maxY = 1f;

	[HideInInspector]
	public float maxZ = 1f;

	[HideInInspector]
	public AnimationCurve CurveX;

	[HideInInspector]
	public AnimationCurve CurveY;

	[HideInInspector]
	public AnimationCurve CurveZ;

	[HideInInspector]
	public Vector3[] RandomValues;

	[HideInInspector]
	public DimensionMode Dimension;

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
		if (CurveX != null)
		{
			CurveX = null;
		}
		if (CurveY != null)
		{
			CurveY = null;
		}
		if (CurveZ != null)
		{
			CurveZ = null;
		}
		if (RandomValues != null)
		{
			RandomValues = null;
		}
	}

	private void Awake()
	{
		if (CurveX == null || CurveX.length < 1)
		{
			CurveX = InitCurve(1f, 1f);
		}
		if (CurveY == null || CurveY.length < 1)
		{
			CurveY = InitCurve(1f, 1f);
		}
		if (CurveZ == null || CurveZ.length < 1)
		{
			CurveZ = InitCurve(1f, 1f);
		}
		if (Target == null)
		{
			Target = base.transform;
		}
	}

	public override void Init()
	{
		if (UseMultiTargets)
		{
			if (Targets == null)
			{
				return;
			}
			if (Targets.Length != 0)
			{
				if (RandomValue && UseMultiRandom && RandomValues == null && RandomValues.Length != Targets.Length)
				{
					RandomValues = new Vector3[Targets.Length];
				}
				for (int i = 0; i < Targets.Length; i++)
				{
					if (Targets[i] != null)
					{
						init = true;
						continue;
					}
					init = false;
					Debug.LogWarning("Null Target. Index : " + i, base.gameObject);
					break;
				}
			}
			else
			{
				init = false;
				Debug.LogWarning("Empty Targets.", base.gameObject);
			}
		}
		else if (Target != null)
		{
			init = true;
		}
		else
		{
			init = false;
			Debug.LogWarning("Null Target.", base.gameObject);
		}
	}
}
