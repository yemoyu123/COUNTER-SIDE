using UnityEngine;

namespace NKC.FX;

public class NKC_FX_UPDATE_SYNC_SMB : StateMachineBehaviour
{
	public string SyncGroupName = string.Empty;

	private NKC_FX_ANIMATOR_UPDATE_SYNC timeSync;

	private NKC_FX_ANIMATOR_UPDATE_SYNC.AnimatorSyncGroup[] groups;

	private Animator[] syncAnimators;

	private NKC_FX_DELAY_EXECUTER[] syncExecuters;

	private NKC_FXM_PLAYER[] syncPlayers;

	private ParticleSystem[] syncParticleSystems;

	private ParticleSystem.MainModule[] syncMainModules;

	private float[] syncSimulateSpeed;

	private NKC_FX_PARTICLE_SYSTEM_SYNC_FLAG[] syncFlags;

	private bool isMultiplyAnimatorSpeed;

	private float sinceTime;

	private float deltatime;

	private float current;

	private float estimatedDeltatime;

	private float adjustTimeScale = 1f;

	private NKCGameOptionData gameOptionData;

	private NKCGameOptionDataSt.GraphicOptionAnimationQuality quality;

	private NKCGameOptionDataSt.GraphicOptionGameFrameLimit limitFrame;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		NKCGameOptionData nKCGameOptionData = NKCScenManager.GetScenManager()?.GetGameOptionData();
		if (nKCGameOptionData != null)
		{
			quality = nKCGameOptionData.AnimationQuality;
			limitFrame = nKCGameOptionData.GameFrameLimit;
			adjustTimeScale = 1f;
			estimatedDeltatime = Time.deltaTime;
			if (quality == NKCGameOptionDataSt.GraphicOptionAnimationQuality.Normal || limitFrame == NKCGameOptionDataSt.GraphicOptionGameFrameLimit.Thirty)
			{
				adjustTimeScale *= 2f;
				estimatedDeltatime = Time.deltaTime * adjustTimeScale;
			}
		}
		current = stateInfo.normalizedTime;
		timeSync = animator.GetComponent<NKC_FX_ANIMATOR_UPDATE_SYNC>();
		if (!timeSync)
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
			if (SyncGroupName.Equals(groups[i].GroupName))
			{
				isMultiplyAnimatorSpeed = groups[i].MultiplyAnimatorSpeed;
				syncAnimators = groups[i].m_Animators;
				syncExecuters = groups[i].m_Executers;
				syncPlayers = groups[i].m_Players;
				syncParticleSystems = groups[i].m_ParticleSystems;
				break;
			}
			syncAnimators = null;
			syncExecuters = null;
			syncPlayers = null;
			syncParticleSystems = null;
		}
		if (syncAnimators != null && syncAnimators.Length != 0)
		{
			for (int j = 0; j < syncAnimators.Length; j++)
			{
				if (syncAnimators[j] != null)
				{
					syncAnimators[j].enabled = false;
					if (syncAnimators[j].gameObject.activeSelf)
					{
						syncAnimators[j].Update(current);
					}
				}
			}
		}
		if (syncExecuters != null && syncExecuters.Length != 0)
		{
			for (int k = 0; k < syncExecuters.Length; k++)
			{
				if (syncExecuters[k] != null)
				{
					syncExecuters[k].SetUseGameUpdate(bUseGameUpdate: true);
					syncExecuters[k].UpdateInternal(current);
				}
			}
		}
		if (syncPlayers != null && syncPlayers.Length != 0)
		{
			for (int l = 0; l < syncPlayers.Length; l++)
			{
				if (syncPlayers[l] != null)
				{
					syncPlayers[l].SetUseGameUpdate(bUseGameUpdate: true);
					syncPlayers[l].UpdateInternal(current);
				}
			}
		}
		if (syncParticleSystems == null || syncParticleSystems.Length == 0)
		{
			return;
		}
		if (syncMainModules == null)
		{
			syncMainModules = new ParticleSystem.MainModule[syncParticleSystems.Length];
		}
		if (syncSimulateSpeed == null)
		{
			syncSimulateSpeed = new float[syncParticleSystems.Length];
		}
		if (syncFlags == null)
		{
			syncFlags = new NKC_FX_PARTICLE_SYSTEM_SYNC_FLAG[syncParticleSystems.Length];
		}
		for (int m = 0; m < syncParticleSystems.Length; m++)
		{
			if (syncParticleSystems[m] != null)
			{
				syncMainModules[m] = syncParticleSystems[m].main;
				syncSimulateSpeed[m] = syncMainModules[m].simulationSpeed;
				syncFlags[m] = syncParticleSystems[m].GetComponent<NKC_FX_PARTICLE_SYSTEM_SYNC_FLAG>();
				if (syncFlags[m] != null)
				{
					syncFlags[m].IsDriven = true;
				}
			}
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (timeSync == null)
		{
			return;
		}
		sinceTime = stateInfo.length * stateInfo.normalizedTime;
		estimatedDeltatime = Time.deltaTime * adjustTimeScale;
		if (isMultiplyAnimatorSpeed)
		{
			deltatime = (sinceTime - current) * animator.speed;
		}
		else
		{
			deltatime = sinceTime - current;
		}
		if (Mathf.Abs(estimatedDeltatime - deltatime) > Time.deltaTime)
		{
			deltatime = estimatedDeltatime;
		}
		current = sinceTime;
		if (syncAnimators != null && syncAnimators.Length != 0)
		{
			for (int i = 0; i < syncAnimators.Length; i++)
			{
				if (syncAnimators[i] != null)
				{
					syncAnimators[i].enabled = false;
					if (syncAnimators[i].gameObject.activeSelf)
					{
						syncAnimators[i].Update(deltatime);
					}
				}
			}
		}
		if (syncExecuters != null && syncExecuters.Length != 0)
		{
			for (int j = 0; j < syncExecuters.Length; j++)
			{
				if (syncExecuters[j] != null)
				{
					syncExecuters[j].SetUseGameUpdate(bUseGameUpdate: true);
					if (syncExecuters[j].gameObject.activeSelf)
					{
						syncExecuters[j].UpdateInternal(deltatime);
					}
				}
			}
		}
		if (syncPlayers != null && syncPlayers.Length != 0)
		{
			for (int k = 0; k < syncPlayers.Length; k++)
			{
				if (syncPlayers[k] != null)
				{
					syncPlayers[k].SetUseGameUpdate(bUseGameUpdate: true);
					if (syncPlayers[k].gameObject.activeSelf)
					{
						syncPlayers[k].UpdateInternal(deltatime);
					}
				}
			}
		}
		if (syncParticleSystems == null || syncParticleSystems.Length == 0)
		{
			return;
		}
		for (int l = 0; l < syncParticleSystems.Length; l++)
		{
			if (!(syncParticleSystems[l] != null))
			{
				continue;
			}
			if (isMultiplyAnimatorSpeed)
			{
				if (quality == NKCGameOptionDataSt.GraphicOptionAnimationQuality.Normal || limitFrame == NKCGameOptionDataSt.GraphicOptionGameFrameLimit.Thirty)
				{
					syncFlags[l].Sync(syncSimulateSpeed[l] * animator.speed * adjustTimeScale);
				}
				else
				{
					syncFlags[l].Sync(syncSimulateSpeed[l] * animator.speed);
				}
			}
			else if (quality == NKCGameOptionDataSt.GraphicOptionAnimationQuality.Normal || limitFrame == NKCGameOptionDataSt.GraphicOptionGameFrameLimit.Thirty)
			{
				syncFlags[l].Sync(syncSimulateSpeed[l] * adjustTimeScale);
			}
			else
			{
				syncFlags[l].Sync(syncSimulateSpeed[l]);
			}
		}
	}

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
		if (syncMainModules != null)
		{
			syncMainModules = null;
		}
		if (syncSimulateSpeed != null)
		{
			syncSimulateSpeed = null;
		}
		if (syncFlags != null)
		{
			syncFlags = null;
		}
	}
}
