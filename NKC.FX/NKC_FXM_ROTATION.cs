using UnityEngine;

namespace NKC.FX;

public class NKC_FXM_ROTATION : NKC_FXM_TRANSFORM
{
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
					if (Targets[i] != null)
					{
						Targets[i].localEulerAngles = GetValue(RandomValues[i].x, RandomValues[i].y, RandomValues[i].z, Targets[i]);
					}
				}
				return;
			}
			for (int j = 0; j < Targets.Length; j++)
			{
				if (Targets[j] != null)
				{
					Targets[j].localEulerAngles = GetValue(FactorX, FactorY, FactorZ, Targets[j]);
				}
			}
		}
		else if (Target != null)
		{
			Target.localEulerAngles = GetValue(FactorX, FactorY, FactorZ, Target);
		}
	}

	private Vector3 GetValue(float _x, float _y, float _z, Transform _target)
	{
		if (SeparateAxes)
		{
			val.Set(Evaluate(CurveX) * _x, Evaluate(CurveY) * _y, Evaluate(CurveZ) * _z);
		}
		else
		{
			val.Set(_target.localEulerAngles.x, _target.localEulerAngles.y, Evaluate(CurveX) * _x);
		}
		return val;
	}
}
