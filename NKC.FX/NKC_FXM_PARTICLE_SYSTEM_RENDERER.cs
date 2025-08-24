using UnityEngine;

namespace NKC.FX;

public class NKC_FXM_PARTICLE_SYSTEM_RENDERER : NKC_FXM_EVALUATER
{
	public ActionMode ActionType;

	public SeedMode SeedType;

	public int[] Seed;

	public ParticleSystem[] Targets;

	private void OnDestroy()
	{
		if (Targets != null)
		{
			Targets = null;
		}
	}

	public override void Init()
	{
		if (Targets != null)
		{
			for (int i = 0; i < Targets.Length; i++)
			{
				if (Targets[i] != null)
				{
					Targets[i].Stop(withChildren: true);
					if (Targets[i].isStopped)
					{
						Targets[i].useAutoRandomSeed = false;
						ParticleSystem.MainModule main = Targets[i].main;
						main.playOnAwake = false;
					}
					init = true;
					continue;
				}
				init = false;
				Debug.LogWarning("Particle System(s) not found -> " + base.transform.name + " -> " + base.transform.root.name, base.gameObject);
				break;
			}
		}
		else
		{
			init = false;
		}
	}

	public override void SetRandomValue(bool _resimulate)
	{
		if (!_resimulate || !init)
		{
			return;
		}
		uint randomSeed = (uint)GetRandomSeed();
		int index = GetIndex();
		switch (SeedType)
		{
		case SeedMode.Random:
		{
			if (ActionType != ActionMode.Play)
			{
				break;
			}
			for (int k = 0; k < Targets.Length; k++)
			{
				Targets[k].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
				if (Targets[k].isStopped)
				{
					Targets[k].randomSeed = randomSeed;
				}
			}
			break;
		}
		case SeedMode.RandomEach:
		{
			if (ActionType != ActionMode.Play)
			{
				break;
			}
			for (int j = 0; j < Targets.Length; j++)
			{
				Targets[j].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
				if (Targets[j].isStopped)
				{
					Targets[j].randomSeed = (uint)GetRandomSeed();
				}
			}
			break;
		}
		case SeedMode.Custom:
		{
			if (ActionType != ActionMode.Play)
			{
				break;
			}
			for (int l = 0; l < Targets.Length; l++)
			{
				Targets[l].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
				if (Targets[l].isStopped)
				{
					if (Seed.Length != 0)
					{
						Targets[l].randomSeed = (uint)Seed[index];
						continue;
					}
					Targets[l].randomSeed = (uint)GetRandomSeed();
					Debug.LogWarning("Seed List is Empty, Force Set RandomSeed -> " + base.transform.name, base.gameObject);
				}
			}
			break;
		}
		case SeedMode.CustomEach:
		{
			if (ActionType != ActionMode.Play)
			{
				break;
			}
			for (int i = 0; i < Targets.Length; i++)
			{
				Targets[i].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
				if (Targets[i].isStopped)
				{
					if (Seed.Length != 0)
					{
						index = GetIndex();
						Targets[i].randomSeed = (uint)Seed[index];
					}
					else
					{
						Targets[i].randomSeed = (uint)GetRandomSeed();
						Debug.LogWarning("Seed List is Empty, Force Set RandomSeed -> " + base.transform.name, base.gameObject);
					}
				}
			}
			break;
		}
		case SeedMode.None:
			break;
		}
	}

	protected override void OnStart()
	{
		if (!init)
		{
			return;
		}
		switch (ActionType)
		{
		case ActionMode.Play:
			if (!Application.isPlaying)
			{
				for (int m = 0; m < Targets.Length; m++)
				{
					Targets[m].Simulate(0f, withChildren: true, restart: true);
				}
			}
			else
			{
				for (int n = 0; n < Targets.Length; n++)
				{
					Targets[n].Play(withChildren: true);
				}
			}
			break;
		case ActionMode.Stop:
		{
			for (int j = 0; j < Targets.Length; j++)
			{
				Targets[j].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
			}
			break;
		}
		case ActionMode.PlayContinue:
		{
			if (!Application.isPlaying)
			{
				for (int k = 0; k < Targets.Length; k++)
				{
					Targets[k].Simulate(0f, withChildren: true, restart: true);
				}
				break;
			}
			for (int l = 0; l < Targets.Length; l++)
			{
				if (!Targets[l].isEmitting)
				{
					Targets[l].Play(withChildren: true);
				}
			}
			break;
		}
		case ActionMode.StopContinue:
		{
			for (int i = 0; i < Targets.Length; i++)
			{
				if (!Targets[i].isStopped)
				{
					Targets[i].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
				}
			}
			break;
		}
		}
	}

	protected override void OnExecute(bool _render)
	{
		if (!base.enabled || !init || ActionType != ActionMode.Play || Application.isPlaying)
		{
			return;
		}
		for (int i = 0; i < Targets.Length; i++)
		{
			if (Targets[i] != null)
			{
				Targets[i].time = playbackTime * Targets[i].main.simulationSpeed;
				Targets[i].Simulate(Targets[i].time, withChildren: true, restart: true);
			}
		}
	}

	public void GenerateSeed(int _index)
	{
		Seed[_index] = GetRandomSeed();
	}

	private int GetRandomSeed()
	{
		return Random.Range(-2147483647, int.MaxValue);
	}

	private int GetIndex()
	{
		return (int)Mathf.Lerp(0f, Seed.Length, Random.value);
	}
}
