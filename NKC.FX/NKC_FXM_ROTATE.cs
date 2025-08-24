using UnityEngine;

namespace NKC.FX;

public class NKC_FXM_ROTATE : NKC_FXM_TRANSFORM_EXT
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
					if (playbackTime > 0f)
					{
						if (Targets[i] != null)
						{
							Targets[i].Rotate(GetValue(RandomValues[i].x, RandomValues[i].y, RandomValues[i].z, Targets[i]), Space);
						}
					}
					else if (Targets[i] != null)
					{
						Targets[i].localEulerAngles = Vector3.zero;
					}
				}
				return;
			}
			for (int j = 0; j < Targets.Length; j++)
			{
				if (playbackTime > 0f)
				{
					if (Targets[j] != null)
					{
						Targets[j].Rotate(GetValue(FactorX, FactorY, FactorZ, Targets[j]), Space);
					}
				}
				else if (Targets[j] != null)
				{
					Targets[j].localEulerAngles = Vector3.zero;
				}
			}
		}
		else if (playbackTime > 0f)
		{
			if (Target != null)
			{
				Target.Rotate(GetValue(FactorX, FactorY, FactorZ, Target), Space);
			}
		}
		else if (Target != null)
		{
			Target.localEulerAngles = Vector3.zero;
		}
	}

	private Vector3 GetValue(float _x, float _y, float _z, Transform _target)
	{
		float num = deltaTime * Speed;
		if (SeparateAxes)
		{
			val.Set(Evaluate(CurveX) * _x * num, Evaluate(CurveY) * _y * num, Evaluate(CurveZ) * _z * num);
		}
		else
		{
			val.Set(_target.localEulerAngles.x, _target.localEulerAngles.y, Evaluate(CurveX) * _x * num);
		}
		return val;
	}
}
