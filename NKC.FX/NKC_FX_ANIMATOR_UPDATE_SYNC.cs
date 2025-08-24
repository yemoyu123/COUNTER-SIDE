using System;
using UnityEngine;

namespace NKC.FX;

public class NKC_FX_ANIMATOR_UPDATE_SYNC : MonoBehaviour
{
	[Serializable]
	public class AnimatorSyncGroup
	{
		public string GroupName = string.Empty;

		public bool MultiplyAnimatorSpeed;

		public Animator[] m_Animators;

		public NKC_FX_DELAY_EXECUTER[] m_Executers;

		public NKC_FXM_PLAYER[] m_Players;

		public ParticleSystem[] m_ParticleSystems;
	}

	[SerializeField]
	public AnimatorSyncGroup[] SyncGroups;

	private void OnDestroy()
	{
		if (SyncGroups != null)
		{
			SyncGroups = null;
		}
	}

	private void OnDisable()
	{
		if (SyncGroups == null || SyncGroups.Length == 0)
		{
			return;
		}
		for (int i = 0; i < SyncGroups.Length; i++)
		{
			if (SyncGroups[i].m_Executers != null && SyncGroups[i].m_Executers.Length != 0)
			{
				for (int j = 0; j < SyncGroups[i].m_Executers.Length; j++)
				{
					if (SyncGroups[i].m_Executers[j] != null)
					{
						SyncGroups[i].m_Executers[j].Stop();
					}
				}
			}
			if (SyncGroups[i].m_Players != null && SyncGroups[i].m_Players.Length != 0)
			{
				for (int k = 0; k < SyncGroups[i].m_Players.Length; k++)
				{
					if (SyncGroups[i].m_Players[k] != null)
					{
						SyncGroups[i].m_Players[k].Stop();
					}
				}
			}
			if (SyncGroups[i].m_ParticleSystems == null || SyncGroups[i].m_ParticleSystems.Length == 0)
			{
				continue;
			}
			for (int l = 0; l < SyncGroups[i].m_ParticleSystems.Length; l++)
			{
				if (SyncGroups[i].m_ParticleSystems[l] != null)
				{
					SyncGroups[i].m_ParticleSystems[l].Stop(withChildren: true);
				}
			}
		}
	}
}
