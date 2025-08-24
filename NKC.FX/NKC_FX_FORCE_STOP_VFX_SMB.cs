using UnityEngine;

namespace NKC.FX;

public class NKC_FX_FORCE_STOP_VFX_SMB : StateMachineBehaviour
{
	private float nTime;

	private NKC_FX_ANIMATOR_UPDATE_SYNC timeSync;

	private NKC_FX_ANIMATOR_UPDATE_SYNC.AnimatorSyncGroup[] groups;

	private Animator[] syncAnimators;

	private NKC_FX_DELAY_EXECUTER[] syncExecuters;

	private NKC_FXM_PLAYER[] syncPlayers;

	private ParticleSystem[] syncParticleSystems;

	private void OnDestroy()
	{
		if (timeSync != null)
		{
			timeSync = null;
		}
		if (groups != null)
		{
			groups = null;
		}
		if (syncAnimators != null)
		{
			syncAnimators = null;
		}
		if (syncExecuters != null)
		{
			syncExecuters = null;
		}
		if (syncPlayers != null)
		{
			syncPlayers = null;
		}
		if (syncParticleSystems != null)
		{
			syncParticleSystems = null;
		}
	}

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (timeSync == null)
		{
			timeSync = animator.GetComponent<NKC_FX_ANIMATOR_UPDATE_SYNC>();
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		nTime = Mathf.Clamp01(stateInfo.normalizedTime);
		if (nTime == 1f)
		{
			ForceStopVFX(animator);
			nTime = 0f;
		}
	}

	private void ForceStopVFX(Animator animator)
	{
		if (!(timeSync != null))
		{
			return;
		}
		groups = timeSync.SyncGroups;
		if (groups == null || groups.Length == 0)
		{
			return;
		}
		for (int i = 0; i < groups.Length; i++)
		{
			syncExecuters = groups[i].m_Executers;
			if (syncExecuters != null && syncExecuters.Length != 0)
			{
				for (int j = 0; j < syncExecuters.Length; j++)
				{
					if (syncExecuters[j] != null)
					{
						syncExecuters[j].Stop();
					}
				}
			}
			syncPlayers = groups[i].m_Players;
			if (syncPlayers != null && syncPlayers.Length != 0)
			{
				for (int k = 0; k < syncPlayers.Length; k++)
				{
					if (syncPlayers[k] != null && !syncPlayers[k].IsStopped)
					{
						syncPlayers[k].Stop();
						if (Debug.isDebugBuild)
						{
							Debug.LogWarning("Force Stop VFX(<color=cyan><b>Player</b></color>) in <color=yellow>" + animator.gameObject.name + "</color> : " + syncPlayers[k].name, syncPlayers[k].gameObject);
						}
					}
				}
			}
			syncParticleSystems = groups[i].m_ParticleSystems;
			if (syncParticleSystems == null || syncParticleSystems.Length == 0)
			{
				continue;
			}
			for (int l = 0; l < syncParticleSystems.Length; l++)
			{
				if (syncParticleSystems[l] != null && syncParticleSystems[l].IsAlive(withChildren: true))
				{
					syncParticleSystems[l].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
					if (Debug.isDebugBuild)
					{
						Debug.LogWarning("Force Stop VFX(<color=red><b>ParticleSystem</b></color>) in <color=yellow>" + animator.gameObject.name + "</color> : " + syncParticleSystems[l].name, syncParticleSystems[l].gameObject);
					}
				}
			}
		}
	}
}
