using UnityEngine;

namespace NKC.FX;

public class NKC_FX_PARTICLE_SYSTEM_SYNC_FLAG : MonoBehaviour
{
	private ParticleSystem ps;

	private ParticleSystem.MainModule main;

	private float syncSimulationSpeed;

	private bool isDriven;

	private bool sync;

	public bool IsDriven
	{
		get
		{
			return isDriven;
		}
		set
		{
			isDriven = value;
		}
	}

	public void Sync()
	{
		sync = true;
	}

	public void Sync(float _syncSimulationSpeed)
	{
		sync = true;
		syncSimulationSpeed = _syncSimulationSpeed;
	}

	private void Start()
	{
		if (ps == null)
		{
			ps = GetComponent<ParticleSystem>();
			main = ps.main;
		}
	}

	private void Update()
	{
		if (isDriven && ps != null && ps.IsAlive(withChildren: true))
		{
			if (sync)
			{
				sync = false;
				main.simulationSpeed = syncSimulationSpeed;
			}
			else
			{
				main.simulationSpeed = 0f;
			}
		}
	}
}
