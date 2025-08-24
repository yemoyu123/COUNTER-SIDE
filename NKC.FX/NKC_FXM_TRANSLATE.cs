using UnityEngine;

namespace NKC.FX;

public class NKC_FXM_TRANSLATE : NKC_FXM_TRANSFORM_EXT
{
	public bool Absolutely;

	private Vector3 factor;

	private Vector3 val;

	public override void SetRandomValue(bool _resimulate)
	{
		if (!RandomValue)
		{
			return;
		}
		if (UseMultiTargets && UseMultiRandom)
		{
			for (int i = 0; i < RandomValues.Length; i++)
			{
				if (_resimulate)
				{
					RandomValues[i].Set(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
				}
				else
				{
					RandomValues[i].Set(Mathf.Lerp(minX, maxX, 0.5f), Mathf.Lerp(minY, maxY, 0.5f), Mathf.Lerp(minZ, maxZ, 0.5f));
				}
			}
		}
		else if (_resimulate)
		{
			FactorX = Random.Range(minX, maxX);
			FactorY = Random.Range(minY, maxY);
			FactorZ = Random.Range(minZ, maxZ);
		}
		else
		{
			FactorX = Mathf.Lerp(minX, maxX, 0.5f);
			FactorY = Mathf.Lerp(minY, maxY, 0.5f);
			FactorZ = Mathf.Lerp(minZ, maxZ, 0.5f);
		}
	}

	protected override void OnExecute(bool _render)
	{
		if (!base.enabled)
		{
			return;
		}
		if (UseMultiTargets)
		{
			if (UseMultiRandom)
			{
				for (int i = 0; i < Targets.Length; i++)
				{
					if (playbackTime > 0f)
					{
						Targets[i].Translate(GetValue(RandomValues[i].x, RandomValues[i].y, RandomValues[i].z, Targets[i]), Space);
					}
					else
					{
						Targets[i].localPosition = Vector3.zero;
					}
				}
				return;
			}
			for (int j = 0; j < Targets.Length; j++)
			{
				if (playbackTime > 0f)
				{
					Targets[j].Translate(GetValue(FactorX, FactorY, FactorZ, Targets[j]), Space);
				}
				else
				{
					Targets[j].localPosition = Vector3.zero;
				}
			}
		}
		else if (playbackTime > 0f)
		{
			Target.Translate(GetValue(FactorX, FactorY, FactorZ, Target), Space);
		}
		else
		{
			Target.localPosition = Vector3.zero;
		}
	}

	private Vector3 GetValue(float _x, float _y, float _z, Transform _target)
	{
		float num = deltaTime * Speed;
		if (!Absolutely)
		{
			factor.Set(Evaluate(CurveX) * _x * num * _target.lossyScale.x, Evaluate(CurveY) * _y * num * _target.lossyScale.y, Evaluate(CurveZ) * _z * num * _target.lossyScale.z);
		}
		else
		{
			factor.Set(Evaluate(CurveX) * _x * num, Evaluate(CurveY) * _y * num, Evaluate(CurveZ) * _z * num);
		}
		if (Dimension == DimensionMode.ThreeDimension)
		{
			if (SeparateAxes)
			{
				val.Set(factor.x, factor.y, factor.z);
			}
			else
			{
				val.Set(factor.x * _target.forward.x, factor.x * _target.forward.y, factor.x);
			}
		}
		else if (SeparateAxes)
		{
			val.Set(factor.x, factor.y, 0f);
		}
		else
		{
			val.Set(factor.x, factor.x * _target.right.y, 0f);
		}
		return val;
	}
}
