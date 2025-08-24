using UnityEngine;

namespace NKC.FX;

[RequireComponent(typeof(ParticleSystem))]
public class NKC_FX_PARTICLE_SYSTEM_STOP_ACTION : MonoBehaviour
{
	public bool EnableReseed;

	public bool IsRandomSeed;

	private int seed;

	private ParticleSystem ps;

	public int Seed
	{
		get
		{
			return seed;
		}
		set
		{
			seed = value;
		}
	}

	private void Start()
	{
		ps = GetComponent<ParticleSystem>();
	}

	private void OnParticleSystemStopped()
	{
		if (EnableReseed)
		{
			if (IsRandomSeed)
			{
				Reseed();
			}
			else
			{
				Reseed(seed);
			}
		}
	}

	private void Reseed()
	{
		ps.randomSeed = (uint)(Random.value * 4.2949673E+09f);
	}

	private void Reseed(int _seed)
	{
		ps.randomSeed = (uint)_seed;
	}
}
