using System;
using Cs.Logging;
using NKC.FX;
using NKM;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace NKC;

public class NKCAnimSpine
{
	private GameObject m_SpineObject;

	private SPINE_ANIM_TYPE m_SPINE_ANIM_TYPE;

	private SkeletonAnimation m_SkeletonAnimation;

	private SkeletonGraphic m_SkeletonGraphic;

	private TrackEntry m_TrackEntry;

	private bool m_bObjectActive;

	private string m_AnimName = "";

	private bool m_bLoop;

	private float m_fPlaySpeed = 1f;

	private bool m_bAnimationEnd;

	private bool m_bAnimStartThisFrame;

	private float m_AnimTimeNow;

	private float m_AnimTimeBefore;

	private bool m_bShow = true;

	private bool m_bHalfUpdate;

	private bool m_bUpdateThisFrame;

	private float m_fUpdateDeltaTime;

	private Animator m_Animator;

	private Animator[] m_Animators;

	private NKC_FXM_PLAYER[] m_NKC_FXM_PLAYERs;

	private NKC_FX_DELAY_EXECUTER[] m_NKC_FX_DELAY_EXECUTERs;

	private ParticleSystem[] m_ParticleSystems;

	private float[] m_ParticleSystem_SimulationSpeedOrg;

	private NKM_GAME_SPEED_TYPE m_NKM_GAME_SPEED_TYPE;

	public string GetAnimName()
	{
		return m_AnimName;
	}

	public void Init()
	{
		m_SpineObject = null;
		m_SPINE_ANIM_TYPE = SPINE_ANIM_TYPE.SAT_INVALID;
		m_SkeletonAnimation = null;
		m_SkeletonGraphic = null;
		m_TrackEntry = null;
		m_bObjectActive = false;
		m_AnimName = "";
		m_bLoop = false;
		m_fPlaySpeed = 1f;
		m_bAnimationEnd = false;
		m_bAnimStartThisFrame = false;
		m_AnimTimeNow = 0f;
		m_AnimTimeBefore = 0f;
		m_bShow = true;
		m_bUpdateThisFrame = false;
		m_fUpdateDeltaTime = 0f;
		m_Animator = null;
		m_Animators = null;
		m_NKC_FXM_PLAYERs = null;
		m_NKC_FX_DELAY_EXECUTERs = null;
		m_ParticleSystems = null;
		m_ParticleSystem_SimulationSpeedOrg = null;
		m_NKM_GAME_SPEED_TYPE = NKM_GAME_SPEED_TYPE.NGST_1;
	}

	public void SetAnimObj(GameObject spineObject, GameObject animatorObject = null, bool bPreload = false)
	{
		Init();
		m_SpineObject = spineObject;
		m_SkeletonAnimation = m_SpineObject.GetComponentInChildren<SkeletonAnimation>(includeInactive: true);
		if (m_SkeletonAnimation != null)
		{
			m_SPINE_ANIM_TYPE = SPINE_ANIM_TYPE.SAT_SPINE;
			if (bPreload)
			{
				MeshRenderer componentInChildren = m_SkeletonAnimation.GetComponentInChildren<MeshRenderer>(includeInactive: true);
				NKCScenManager.GetScenManager().ForceRender(componentInChildren);
			}
		}
		m_SkeletonGraphic = m_SpineObject.GetComponentInChildren<SkeletonGraphic>(includeInactive: true);
		if (m_SkeletonGraphic != null)
		{
			m_SPINE_ANIM_TYPE = SPINE_ANIM_TYPE.SAT_SPINE_UI;
			if (bPreload)
			{
				NKCScenManager.GetScenManager().ForceRender(m_SkeletonGraphic.mainTexture);
			}
		}
		if (m_SPINE_ANIM_TYPE == SPINE_ANIM_TYPE.SAT_INVALID)
		{
			Init();
			return;
		}
		ComponentEnable(bEnable: false);
		if (animatorObject == null)
		{
			animatorObject = spineObject;
		}
		m_Animator = animatorObject.GetComponent<Animator>();
		if (m_Animator != null && m_Animator.enabled)
		{
			m_Animator.enabled = false;
		}
		m_Animators = animatorObject.GetComponentsInChildren<Animator>(includeInactive: true);
		for (int i = 0; i < m_Animators.Length; i++)
		{
			if (m_Animators[i].enabled)
			{
				m_Animators[i].enabled = false;
			}
		}
		m_NKC_FXM_PLAYERs = animatorObject.GetComponentsInChildren<NKC_FXM_PLAYER>(includeInactive: true);
		for (int j = 0; j < m_NKC_FXM_PLAYERs.Length; j++)
		{
			m_NKC_FXM_PLAYERs[j].SetUseGameUpdate(bUseGameUpdate: true);
		}
		m_NKC_FX_DELAY_EXECUTERs = animatorObject.GetComponentsInChildren<NKC_FX_DELAY_EXECUTER>(includeInactive: true);
		for (int k = 0; k < m_NKC_FX_DELAY_EXECUTERs.Length; k++)
		{
			m_NKC_FX_DELAY_EXECUTERs[k].SetUseGameUpdate(bUseGameUpdate: true);
		}
		m_ParticleSystems = animatorObject.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
		m_ParticleSystem_SimulationSpeedOrg = new float[m_ParticleSystems.Length];
		for (int l = 0; l < m_ParticleSystems.Length; l++)
		{
			ParticleSystem particleSystem = m_ParticleSystems[l];
			if (!(particleSystem == null))
			{
				ParticleSystem.MainModule main = particleSystem.main;
				m_ParticleSystem_SimulationSpeedOrg[l] = main.simulationSpeed;
				if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME && NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_STATE() == NKC_SCEN_STATE.NSS_START)
				{
					main.simulationSpeed = m_ParticleSystem_SimulationSpeedOrg[l] * 1.1f;
				}
				else
				{
					main.simulationSpeed = m_ParticleSystem_SimulationSpeedOrg[l];
				}
			}
		}
		SetSpriteActive();
	}

	public void ResetParticleSimulSpeedOrg()
	{
		if (m_ParticleSystems == null || m_ParticleSystem_SimulationSpeedOrg == null)
		{
			return;
		}
		for (int i = 0; i < m_ParticleSystems.Length; i++)
		{
			ParticleSystem particleSystem = m_ParticleSystems[i];
			if (!(particleSystem == null))
			{
				ParticleSystem.MainModule main = particleSystem.main;
				main.simulationSpeed = m_ParticleSystem_SimulationSpeedOrg[i];
			}
		}
		m_NKM_GAME_SPEED_TYPE = NKM_GAME_SPEED_TYPE.NGST_1;
	}

	public void ComponentEnable(bool bEnable)
	{
		if (m_SPINE_ANIM_TYPE == SPINE_ANIM_TYPE.SAT_SPINE && m_SkeletonAnimation.enabled == !bEnable)
		{
			m_SkeletonAnimation.enabled = bEnable;
		}
	}

	public Spine.Animation GetAnimByName(string animName)
	{
		return m_SPINE_ANIM_TYPE switch
		{
			SPINE_ANIM_TYPE.SAT_SPINE => m_SkeletonAnimation.state.Data.SkeletonData.FindAnimation(animName), 
			SPINE_ANIM_TYPE.SAT_SPINE_UI => m_SkeletonGraphic.AnimationState.Data.SkeletonData.FindAnimation(animName), 
			_ => null, 
		};
	}

	private void SetSpriteActive()
	{
		switch (m_SPINE_ANIM_TYPE)
		{
		case SPINE_ANIM_TYPE.SAT_SPINE:
			if (!m_SkeletonAnimation.gameObject.activeSelf || !m_SkeletonAnimation.gameObject.activeInHierarchy)
			{
				m_bObjectActive = false;
			}
			else
			{
				m_bObjectActive = true;
			}
			break;
		case SPINE_ANIM_TYPE.SAT_SPINE_UI:
			if (!m_SkeletonGraphic.gameObject.activeSelf || !m_SkeletonGraphic.gameObject.activeInHierarchy)
			{
				m_bObjectActive = false;
			}
			else
			{
				m_bObjectActive = true;
			}
			break;
		}
	}

	public void Update(float deltaTime)
	{
		if (m_SpineObject == null)
		{
			return;
		}
		switch (m_SPINE_ANIM_TYPE)
		{
		default:
			return;
		case SPINE_ANIM_TYPE.SAT_SPINE:
			if (m_SkeletonAnimation == null)
			{
				return;
			}
			break;
		case SPINE_ANIM_TYPE.SAT_SPINE_UI:
			if (m_SkeletonGraphic == null)
			{
				return;
			}
			break;
		}
		if (!m_bObjectActive)
		{
			SetSpriteActive();
			if (m_bObjectActive)
			{
				Play(m_AnimName, m_bLoop);
			}
		}
		else
		{
			SetSpriteActive();
		}
		if (!m_bObjectActive)
		{
			return;
		}
		m_bAnimStartThisFrame = false;
		m_AnimTimeBefore = m_AnimTimeNow;
		m_AnimTimeNow = GetAnimTimeNow();
		switch (m_SPINE_ANIM_TYPE)
		{
		case SPINE_ANIM_TYPE.SAT_SPINE:
			m_TrackEntry = m_SkeletonAnimation.AnimationState.GetCurrent(0);
			break;
		case SPINE_ANIM_TYPE.SAT_SPINE_UI:
			m_TrackEntry = m_SkeletonGraphic.AnimationState.GetCurrent(0);
			break;
		}
		if (IsAnimationEnd())
		{
			if (m_bLoop && m_bAnimationEnd)
			{
				Play(m_AnimName, m_bLoop);
			}
			m_bAnimationEnd = true;
		}
		else
		{
			NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
			if (gameOptionData != null)
			{
				if (gameOptionData.AnimationQuality == NKCGameOptionDataSt.GraphicOptionAnimationQuality.Normal)
				{
					m_bHalfUpdate = true;
				}
				else
				{
					m_bHalfUpdate = false;
				}
				if (gameOptionData.GameFrameLimit == NKCGameOptionDataSt.GraphicOptionGameFrameLimit.Thirty)
				{
					m_bHalfUpdate = false;
				}
			}
			if (m_bHalfUpdate)
			{
				m_fUpdateDeltaTime += deltaTime;
				m_bUpdateThisFrame = !m_bUpdateThisFrame;
				if (m_bUpdateThisFrame)
				{
					UpdateSpine(m_fUpdateDeltaTime);
					m_fUpdateDeltaTime = 0f;
				}
			}
			else
			{
				UpdateSpine(deltaTime);
			}
		}
		UpdateEffect(deltaTime);
	}

	public void UpdateSpine(float deltaTime)
	{
		if (m_SPINE_ANIM_TYPE == SPINE_ANIM_TYPE.SAT_SPINE)
		{
			m_SkeletonAnimation.Update(deltaTime);
			m_SkeletonAnimation.LateUpdate();
		}
		for (int i = 0; i < m_Animators.Length; i++)
		{
			Animator animator = m_Animators[i];
			if (animator != null && animator.gameObject.activeInHierarchy)
			{
				animator.Update(deltaTime);
			}
		}
	}

	public void UpdateEffect(float deltaTime)
	{
		try
		{
			float num = deltaTime * m_fPlaySpeed;
			for (int i = 0; i < m_NKC_FXM_PLAYERs.Length; i++)
			{
				NKC_FXM_PLAYER nKC_FXM_PLAYER = m_NKC_FXM_PLAYERs[i];
				if (nKC_FXM_PLAYER != null && nKC_FXM_PLAYER.gameObject.activeInHierarchy)
				{
					nKC_FXM_PLAYER.UpdateInternal(num);
				}
			}
			for (int j = 0; j < m_NKC_FX_DELAY_EXECUTERs.Length; j++)
			{
				NKC_FX_DELAY_EXECUTER nKC_FX_DELAY_EXECUTER = m_NKC_FX_DELAY_EXECUTERs[j];
				if (nKC_FX_DELAY_EXECUTER != null && nKC_FX_DELAY_EXECUTER.gameObject.activeInHierarchy)
				{
					nKC_FX_DELAY_EXECUTER.UpdateInternal(num);
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogError("Effect update failed. m_SpineObject: " + m_SpineObject.name);
			Debug.LogException(exception);
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_GAME || NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_STATE() != NKC_SCEN_STATE.NSS_START || m_NKM_GAME_SPEED_TYPE == NKCScenManager.GetScenManager().GetGameClient().GetGameRuntimeData()
			.m_NKM_GAME_SPEED_TYPE)
		{
			return;
		}
		m_NKM_GAME_SPEED_TYPE = NKCScenManager.GetScenManager().GetGameClient().GetGameRuntimeData()
			.m_NKM_GAME_SPEED_TYPE;
		for (int k = 0; k < m_ParticleSystems.Length; k++)
		{
			ParticleSystem particleSystem = m_ParticleSystems[k];
			if (!(particleSystem == null))
			{
				ParticleSystem.MainModule main = particleSystem.main;
				switch (m_NKM_GAME_SPEED_TYPE)
				{
				case NKM_GAME_SPEED_TYPE.NGST_1:
				case NKM_GAME_SPEED_TYPE.NGST_3:
				case NKM_GAME_SPEED_TYPE.NGST_10:
					main.simulationSpeed = m_ParticleSystem_SimulationSpeedOrg[k] * 1.1f;
					break;
				case NKM_GAME_SPEED_TYPE.NGST_2:
					main.simulationSpeed = m_ParticleSystem_SimulationSpeedOrg[k] * 1.5f;
					break;
				case NKM_GAME_SPEED_TYPE.NGST_05:
					main.simulationSpeed = m_ParticleSystem_SimulationSpeedOrg[k] * 0.6f;
					break;
				}
			}
		}
	}

	public void PlayAnimator()
	{
		if (!(m_Animator == null))
		{
			if (m_TrackEntry.TrackTime > 0f)
			{
				m_Animator.Play(m_AnimName, -1, m_TrackEntry.AnimationTime / m_TrackEntry.AnimationEnd);
			}
			else
			{
				m_Animator.Play(m_AnimName, -1, 0f);
			}
			m_Animator.Update(0.001f);
		}
	}

	public void Play(string animName, bool bLoop, float fStartTime = 0f)
	{
		switch (m_SPINE_ANIM_TYPE)
		{
		default:
			return;
		case SPINE_ANIM_TYPE.SAT_SPINE:
			if (m_SkeletonAnimation == null)
			{
				return;
			}
			break;
		case SPINE_ANIM_TYPE.SAT_SPINE_UI:
			if (m_SkeletonGraphic == null)
			{
				return;
			}
			break;
		}
		if (!m_bObjectActive)
		{
			SetSpriteActive();
		}
		if (!m_bObjectActive)
		{
			return;
		}
		Spine.Animation spineAnim = GetSpineAnim(animName);
		if (spineAnim == null)
		{
			return;
		}
		m_AnimName = animName;
		m_bLoop = bLoop;
		m_bAnimationEnd = false;
		m_bAnimStartThisFrame = true;
		m_AnimTimeNow = 0f;
		m_AnimTimeBefore = 0f;
		if (fStartTime > 0f)
		{
			m_AnimTimeNow = fStartTime;
			m_AnimTimeBefore = fStartTime;
		}
		switch (m_SPINE_ANIM_TYPE)
		{
		case SPINE_ANIM_TYPE.SAT_SPINE:
			m_SkeletonAnimation.AnimationState.TimeScale = m_fPlaySpeed;
			if (m_SkeletonAnimation.Skeleton != null)
			{
				m_SkeletonAnimation.Skeleton.SetToSetupPose();
			}
			m_SkeletonAnimation.AnimationState.SetAnimation(0, spineAnim, loop: false);
			m_TrackEntry = m_SkeletonAnimation.AnimationState.GetCurrent(0);
			if (fStartTime > 0f)
			{
				m_TrackEntry.AnimationLast = fStartTime;
			}
			else
			{
				m_TrackEntry.AnimationLast = -1f;
			}
			m_TrackEntry.TrackTime = fStartTime;
			m_SkeletonAnimation.Update(0.001f);
			m_SkeletonAnimation.LateUpdate();
			break;
		case SPINE_ANIM_TYPE.SAT_SPINE_UI:
			m_SkeletonGraphic.AnimationState.TimeScale = m_fPlaySpeed;
			if (m_SkeletonGraphic.Skeleton != null)
			{
				m_SkeletonGraphic.Skeleton.SetToSetupPose();
			}
			m_SkeletonGraphic.AnimationState.SetAnimation(0, spineAnim, loop: false);
			m_TrackEntry = m_SkeletonGraphic.AnimationState.GetCurrent(0);
			if (fStartTime > 0f)
			{
				m_TrackEntry.AnimationLast = fStartTime;
			}
			else
			{
				m_TrackEntry.AnimationLast = -1f;
			}
			m_TrackEntry.TrackTime = fStartTime;
			m_SkeletonGraphic._Update(0.001f);
			m_SkeletonGraphic.LateUpdate();
			break;
		}
		PlayAnimator();
	}

	private Spine.Animation GetSpineAnim(string animName)
	{
		Spine.Animation animation = null;
		switch (m_SPINE_ANIM_TYPE)
		{
		case SPINE_ANIM_TYPE.SAT_SPINE:
			if (m_SkeletonAnimation != null && m_SkeletonAnimation.AnimationState != null)
			{
				animation = m_SkeletonAnimation.AnimationState.Data.SkeletonData.FindAnimation(animName);
				if (animation == null && m_SkeletonAnimation.SkeletonDataAsset != null)
				{
					Debug.LogErrorFormat("NKCAnimSpine NoExistAnim {0}, {1}", m_SkeletonAnimation.SkeletonDataAsset.name, animName);
				}
			}
			else
			{
				Debug.LogErrorFormat("NKCAnimSpine NoExistAnim {0}", animName);
			}
			break;
		case SPINE_ANIM_TYPE.SAT_SPINE_UI:
			if (m_SkeletonGraphic != null && m_SkeletonGraphic.AnimationState != null)
			{
				animation = m_SkeletonGraphic.AnimationState.Data.SkeletonData.FindAnimation(animName);
				if (animation == null && m_SkeletonGraphic.SkeletonDataAsset != null)
				{
					Debug.LogErrorFormat("NKCAnimSpine NoExistAnim {0}, {1}", m_SkeletonGraphic.SkeletonDataAsset.name, animName);
				}
			}
			else
			{
				Debug.LogErrorFormat("NKCAnimSpine NoExistAnim {0}", animName);
			}
			break;
		}
		return animation;
	}

	public float GetAnimTimeNow()
	{
		if (m_TrackEntry == null)
		{
			return 0f;
		}
		return m_TrackEntry.AnimationTime;
	}

	public bool IsAnimationEnd()
	{
		if (m_TrackEntry != null && m_TrackEntry.TrackTime >= m_TrackEntry.AnimationEnd)
		{
			return true;
		}
		return false;
	}

	public void SetShow(bool bShow)
	{
		if (m_bShow != bShow)
		{
			if (m_SpineObject != null)
			{
				m_SpineObject.SetActive(bShow);
			}
			m_bShow = bShow;
		}
	}

	public void SetPlaySpeed(float fSpeed)
	{
		m_fPlaySpeed = fSpeed;
		switch (m_SPINE_ANIM_TYPE)
		{
		case SPINE_ANIM_TYPE.SAT_SPINE:
			if (m_SkeletonAnimation != null && m_SkeletonAnimation.AnimationState != null)
			{
				m_SkeletonAnimation.AnimationState.TimeScale = m_fPlaySpeed;
			}
			else
			{
				Log.Error("m_SkeletonAnimation null, m_SpineObject: " + m_SpineObject.name, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCAnimSpine.cs", 629);
			}
			break;
		case SPINE_ANIM_TYPE.SAT_SPINE_UI:
			if (m_SkeletonGraphic != null && m_SkeletonGraphic.AnimationState != null)
			{
				m_SkeletonGraphic.AnimationState.TimeScale = m_fPlaySpeed;
			}
			else
			{
				Log.Error("m_SkeletonGraphic null, m_SpineObject: " + m_SpineObject.name, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCAnimSpine.cs", 637);
			}
			break;
		}
		if (m_Animator != null)
		{
			m_Animator.speed = m_fPlaySpeed;
		}
	}

	public bool EventTimer(float fTime)
	{
		if (fTime == 0f && m_bAnimStartThisFrame)
		{
			return true;
		}
		if (fTime > m_AnimTimeBefore && fTime <= m_AnimTimeNow)
		{
			return true;
		}
		return false;
	}
}
