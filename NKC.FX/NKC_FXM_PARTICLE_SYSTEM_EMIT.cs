using System;
using UnityEngine;

namespace NKC.FX;

public class NKC_FXM_PARTICLE_SYSTEM_EMIT : NKC_FXM_EVALUATER
{
	[Serializable]
	public class BurstInfo
	{
		public float StartTime;

		public int BurstCount;

		public int BurstCountMin;

		public int BurstCountMax;

		public int Count;

		public int Cycles;

		public float Interval;

		public float Probability;

		public float TotalTime()
		{
			return StartTime + Interval * (float)Cycles;
		}
	}

	public ParticleSystem[] Targets;

	public BurstInfo[] BurstSequence;

	public int BurstCount = 1;

	public float RateOverTime;

	public float RateOverDistance;

	public bool UseTransformAsEmitter = true;

	public bool ApplyShapeToPosition = true;

	public bool EnableAngularVelocity;

	public float AngularVelocity;

	public bool EnableAngularVelocity3D;

	public Vector3 AngularVelocity3D;

	public bool EnableAxisOfRotation;

	public Vector3 AxisOfRotation;

	public bool EnableRandomSeed;

	public uint RandomSeed;

	public bool EnableRotation;

	public float Rotation;

	public bool EnableRotation3D;

	public Vector3 Rotation3D;

	public bool EnableStartColor;

	public Color StartColor = Color.white;

	public bool EnableStartLifetime;

	public float StartLifetime;

	public bool EnableStartSize;

	public float StartSize;

	public bool EnableStartSize3D;

	public Vector3 StartSize3D;

	public bool EnableVelocity;

	public Vector3 Velocity;

	private ParticleSystem.EmitParams emitParams;

	private ParticleSystem.ShapeModule shapeModule;

	private float rate;

	private float emissionTime;

	private int emitCount;

	private readonly float unit = 1f;

	private float mileage;

	private int step;

	private Vector3 previousPos;

	private void OnDestroy()
	{
		if (Targets != null)
		{
			Targets = null;
		}
		if (BurstSequence != null)
		{
			BurstSequence = null;
		}
	}

	public override void Init()
	{
		if (Targets != null)
		{
			previousPos = base.transform.position;
			init = true;
		}
		else
		{
			init = false;
		}
	}

	public override void SetRandomValue(bool _resimulate)
	{
		if (!_resimulate)
		{
			return;
		}
		int seed = (int)(UnityEngine.Random.value * 2.1474836E+09f);
		for (int i = 0; i < Targets.Length; i++)
		{
			ParticleSystem.MainModule main = Targets[i].main;
			main.stopAction = ParticleSystemStopAction.Callback;
			NKC_FX_PARTICLE_SYSTEM_STOP_ACTION nKC_FX_PARTICLE_SYSTEM_STOP_ACTION = Targets[i].GetComponent<NKC_FX_PARTICLE_SYSTEM_STOP_ACTION>();
			if (nKC_FX_PARTICLE_SYSTEM_STOP_ACTION == null)
			{
				nKC_FX_PARTICLE_SYSTEM_STOP_ACTION = Targets[i].gameObject.AddComponent<NKC_FX_PARTICLE_SYSTEM_STOP_ACTION>();
			}
			nKC_FX_PARTICLE_SYSTEM_STOP_ACTION.EnableReseed = true;
			nKC_FX_PARTICLE_SYSTEM_STOP_ACTION.Seed = seed;
		}
	}

	protected override void OnStart()
	{
		previousPos = base.transform.position;
		if (BurstSequence != null && BurstSequence.Length != 0)
		{
			for (int i = 0; i < BurstSequence.Length; i++)
			{
				BurstSequence[i].Count = 0;
			}
		}
	}

	protected override void OnExecute(bool _render)
	{
		if (base.isActiveAndEnabled && _render)
		{
			if (BurstSequence != null && BurstSequence.Length != 0)
			{
				EmitBurst();
			}
			if (playbackTime > 0f)
			{
				EmitRateOverTime();
				EmitRateOverDistance();
			}
		}
	}

	private void EmitBurst()
	{
		for (int i = 0; i < BurstSequence.Length; i++)
		{
			if (BurstSequence[i].Count > BurstSequence[i].Cycles || !(BurstSequence[i].StartTime + BurstSequence[i].Interval * (float)BurstSequence[i].Count <= playbackTime))
			{
				continue;
			}
			BurstSequence[i].Count++;
			if (!(UnityEngine.Random.value <= BurstSequence[i].Probability))
			{
				continue;
			}
			BurstSequence[i].BurstCount = UnityEngine.Random.Range(BurstSequence[i].BurstCountMin, BurstSequence[i].BurstCountMax);
			for (int j = 0; j < Targets.Length; j++)
			{
				if (Targets[j] != null)
				{
					Emit(Targets[j], BurstSequence[i].BurstCount, base.transform.position);
				}
			}
		}
	}

	private void EmitRateOverTime()
	{
		if (!(RateOverTime > 0f))
		{
			return;
		}
		rate = 1f / RateOverTime;
		if (rate < deltaTime)
		{
			emitCount = Mathf.CeilToInt(deltaTime / rate);
		}
		else
		{
			emitCount = 1;
		}
		if (emissionTime > rate)
		{
			emissionTime -= rate;
			for (int i = 0; i < Targets.Length; i++)
			{
				if (Targets[i] != null)
				{
					Emit(Targets[i], emitCount, base.transform.position);
				}
			}
		}
		else
		{
			emissionTime += deltaTime;
		}
	}

	private void EmitRateOverDistance()
	{
		if (!(base.transform.position != previousPos))
		{
			return;
		}
		mileage += Vector3.Distance(base.transform.position, previousPos) * RateOverDistance;
		if (unit < mileage)
		{
			for (int i = 0; i < Targets.Length; i++)
			{
				if (Targets[i] != null)
				{
					Emit(Targets[i], 1, base.transform.position);
				}
			}
			rate = unit / RateOverDistance;
			step = Mathf.CeilToInt(mileage / rate);
			if (step > 1)
			{
				for (int j = 1; j < step; j++)
				{
					float t = (float)j / (float)step;
					for (int k = 0; k < Targets.Length; k++)
					{
						if (Targets[k] != null)
						{
							Emit(Targets[k], 1, Vector3.Lerp(base.transform.position, previousPos, t));
						}
					}
				}
			}
			mileage = 0f;
		}
		previousPos = base.transform.position;
	}

	private void Emit(ParticleSystem _ps, int _count, Vector3 _position)
	{
		if (UseTransformAsEmitter)
		{
			emitParams.position = ConvertWorldPointToSimulationSpace(_position, _ps);
			shapeModule = _ps.shape;
			shapeModule.rotation = base.transform.localEulerAngles;
		}
		else
		{
			emitParams.ResetPosition();
		}
		emitParams = GetEmitParams(_ps);
		_ps.Emit(emitParams, _count);
	}

	public ParticleSystem.EmitParams GetEmitParams(ParticleSystem _ps)
	{
		emitParams.applyShapeToPosition = ApplyShapeToPosition;
		if (EnableAngularVelocity)
		{
			if (EnableAngularVelocity3D)
			{
				emitParams.angularVelocity3D = AngularVelocity3D;
			}
			else
			{
				emitParams.angularVelocity = AngularVelocity;
			}
		}
		else
		{
			emitParams.ResetAngularVelocity();
		}
		if (EnableAxisOfRotation)
		{
			emitParams.axisOfRotation = AxisOfRotation;
		}
		else
		{
			emitParams.ResetAxisOfRotation();
		}
		if (EnableRandomSeed)
		{
			emitParams.randomSeed = RandomSeed;
		}
		else
		{
			emitParams.ResetRandomSeed();
		}
		if (EnableRotation)
		{
			if (EnableRotation3D)
			{
				emitParams.rotation3D = Rotation3D;
			}
			else
			{
				emitParams.rotation = Rotation;
			}
		}
		else
		{
			emitParams.ResetRotation();
		}
		if (EnableStartColor)
		{
			emitParams.startColor = StartColor;
		}
		else
		{
			emitParams.ResetStartColor();
		}
		if (EnableStartLifetime)
		{
			emitParams.startLifetime = StartLifetime;
		}
		else
		{
			emitParams.ResetStartLifetime();
		}
		if (EnableStartSize)
		{
			if (EnableStartSize3D)
			{
				emitParams.startSize3D = StartSize3D;
			}
			else
			{
				emitParams.startSize = StartSize;
			}
		}
		else
		{
			emitParams.ResetStartSize();
		}
		if (EnableVelocity)
		{
			emitParams.velocity = Velocity;
		}
		else
		{
			emitParams.ResetVelocity();
		}
		return emitParams;
	}

	private Vector3 ConvertWorldPointToSimulationSpace(Vector3 _worldPos, ParticleSystem _ps)
	{
		ParticleSystem.MainModule main = _ps.main;
		return main.simulationSpace switch
		{
			ParticleSystemSimulationSpace.World => _worldPos, 
			ParticleSystemSimulationSpace.Custom => main.customSimulationSpace.InverseTransformPoint(_worldPos), 
			_ => _ps.transform.InverseTransformPoint(_worldPos), 
		};
	}
}
