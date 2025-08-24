using System;
using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.UI.NPC;

public abstract class NKCUINPCSpine : NKCUINPCBase
{
	private enum AnimationType
	{
		NOANI,
		IDLE,
		DESPAIR,
		HATE,
		LAUGH,
		PRIDE,
		SERIOUS,
		START,
		SURPRISE,
		TOUCH,
		TOUCH2,
		THANKS
	}

	public SkeletonGraphic m_spUnitIllust;

	public bool m_bUseTouch = true;

	public bool m_bUseDrag;

	private Transform m_trTalk_L;

	private Transform m_trTalk_R;

	private NKCComUITalkBox m_talkBox;

	private NKCAssetInstanceData m_talkInstance;

	protected bool m_bUseIdleAni;

	protected float m_fIdleIntervalTime;

	protected float m_fIdleWaitTime;

	protected void ResetIdleWaitTime()
	{
		m_fIdleWaitTime = 0f;
	}

	public override void Init(bool bUseIdleAnimation = true)
	{
		if (m_bUseTouch && m_spUnitIllust != null)
		{
			m_spUnitIllust.raycastTarget = false;
			EventTrigger eventTrigger = base.gameObject.GetComponent<EventTrigger>();
			if (eventTrigger == null)
			{
				eventTrigger = base.gameObject.AddComponent<EventTrigger>();
			}
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerDown;
			entry.callback.AddListener(delegate
			{
				TouchIllust();
			});
			eventTrigger.triggers.Clear();
			eventTrigger.triggers.Add(entry);
		}
		if (m_bUseDrag && m_spUnitIllust != null)
		{
			DragEvent();
		}
		SetIdleWaitTime();
	}

	public void TouchIllust()
	{
		if (m_spUnitIllust != null)
		{
			if (HasAction(NPC_ACTION_TYPE.TOUCH))
			{
				PlayAni(NPC_ACTION_TYPE.TOUCH);
			}
			else
			{
				PlayAni(NPC_ACTION_TYPE.NONE);
			}
		}
	}

	private bool HasAnimation(string animName)
	{
		if (m_spUnitIllust == null)
		{
			return false;
		}
		if (m_spUnitIllust.SkeletonData == null)
		{
			return false;
		}
		if (m_spUnitIllust.SkeletonData.FindAnimation(animName) == null)
		{
			return false;
		}
		return true;
	}

	public void PlayAni(string animationName)
	{
		if (string.IsNullOrEmpty(animationName) || m_spUnitIllust == null || m_spUnitIllust.AnimationState == null)
		{
			return;
		}
		if (Enum.Parse(typeof(AnimationType), animationName) != null)
		{
			AnimationType animationType = (AnimationType)Enum.Parse(typeof(AnimationType), animationName);
			if (animationType == AnimationType.NOANI)
			{
				m_spUnitIllust.AnimationState.SetAnimation(0, "IDLE", loop: true);
			}
			else
			{
				m_spUnitIllust.AnimationState.SetAnimation(0, animationType.ToString(), loop: false);
				m_spUnitIllust.AnimationState.AddAnimation(0, "IDLE", loop: true, 0f);
			}
		}
		ResetIdleWaitTime();
	}

	public override void PlayAni(NPC_ACTION_TYPE actionType, bool bMute = false)
	{
		NKCNPCTemplet nPCTemplet = GetNPCTemplet(actionType);
		if (nPCTemplet != null)
		{
			PlayAni(nPCTemplet.m_AnimationName);
			if (!bMute)
			{
				bool bShowCaption = actionType == NPC_ACTION_TYPE.TOUCH || actionType == NPC_ACTION_TYPE.START || actionType == NPC_ACTION_TYPE.ENTER_BASE;
				NKCUINPCBase.PlayVoice(NPCType, nPCTemplet, bStopCurrentSound: true, bIgnoreCoolTime: false, bShowCaption);
			}
			else
			{
				StopVoice();
			}
		}
	}

	private bool NeedAnimationBlock(AnimationType type)
	{
		if ((uint)type <= 1u || type == AnimationType.START)
		{
			return false;
		}
		return true;
	}

	public override void PlayAni(eEmotion emotion)
	{
	}

	public override void OpenTalk(bool bLeft, NKC_UI_TALK_BOX_DIR dir, string talk, float fadeTime = 0f)
	{
		if (m_talkBox == null)
		{
			NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("ab_ui_talk_box", "AB_UI_TALK_BOX");
			m_talkBox = nKCAssetInstanceData.m_Instant.GetComponent<NKCComUITalkBox>();
			if (m_talkBox == null)
			{
				NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
				return;
			}
			m_talkInstance = nKCAssetInstanceData;
		}
		if (m_trTalk_L == null && m_trTalk_R == null && base.transform.parent != null)
		{
			m_trTalk_L = base.transform.parent.Find("Root_Speach_Bubble_L");
			m_trTalk_R = base.transform.parent.Find("Root_Speach_Bubble_R");
		}
		Transform transform = (bLeft ? m_trTalk_L : m_trTalk_R);
		if (transform == null)
		{
			CloseTalk();
			return;
		}
		m_talkBox.transform.SetParent(transform);
		m_talkBox.transform.localPosition = Vector3.zero;
		NKCUtil.SetGameobjectActive(m_talkBox, bValue: true);
		m_talkBox.SetDir(dir);
		m_talkBox.SetText(talk, fadeTime);
	}

	public override void CloseTalk()
	{
		if (m_talkInstance != null)
		{
			NKCAssetResourceManager.CloseInstance(m_talkInstance);
			m_talkInstance = null;
			m_talkBox = null;
		}
	}

	public void Update()
	{
		if (m_bUseIdleAni && !NKCSoundManager.IsPlayingVoice())
		{
			m_fIdleWaitTime += Time.deltaTime;
			if (m_fIdleWaitTime > m_fIdleIntervalTime)
			{
				m_fIdleWaitTime = 0f;
				PlayAni(NPC_ACTION_TYPE.IDLE);
			}
		}
	}

	protected void SetIdleWaitTime()
	{
		if (!m_bUseIdleAni)
		{
			return;
		}
		NKCNPCTemplet nPCTemplet = GetNPCTemplet(NPC_ACTION_TYPE.IDLE);
		if (nPCTemplet != null)
		{
			m_fIdleIntervalTime = nPCTemplet.m_ConditionValue;
			if (m_fIdleIntervalTime <= 0f)
			{
				m_bUseIdleAni = false;
			}
		}
	}
}
