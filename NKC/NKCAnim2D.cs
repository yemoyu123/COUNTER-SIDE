using System.Collections.Generic;
using UnityEngine;

namespace NKC;

public class NKCAnim2D
{
	private GameObject m_Sprite;

	private Animator m_Animator;

	private AnimatorStateInfo m_AnimatorStateInfo;

	private AnimatorClipInfo[] m_AnimatorClipInfos;

	private bool m_bSpriteActive;

	private string m_AnimName = "";

	private bool m_bLoop;

	private float m_fPlaySpeed = 1f;

	private bool m_bAnimationEnd;

	private bool m_bAnimStartThisFrame;

	private float m_AnimTimeNow;

	private float m_AnimTimeBefore;

	private float m_AnimTimeNormal;

	private bool m_bShow = true;

	private bool m_bHalfUpdate;

	private bool m_bUpdateThisFrame;

	private float m_fUpdateDeltaTime;

	private Dictionary<string, AnimAutoChange> m_dicAnimAutoChange = new Dictionary<string, AnimAutoChange>();

	public string GetAnimName()
	{
		return m_AnimName;
	}

	public void Init()
	{
		m_Sprite = null;
		m_Animator = null;
		m_AnimatorStateInfo = default(AnimatorStateInfo);
		m_AnimatorClipInfos = null;
		m_bSpriteActive = false;
		m_AnimName = "";
		m_bLoop = false;
		m_fPlaySpeed = 1f;
		m_bAnimationEnd = false;
		m_bAnimStartThisFrame = false;
		m_AnimTimeNow = 0f;
		m_AnimTimeBefore = 0f;
		m_dicAnimAutoChange.Clear();
		m_bShow = true;
		m_bUpdateThisFrame = false;
		m_fUpdateDeltaTime = 0f;
	}

	public bool IsClipLoop()
	{
		for (int i = 0; i < m_AnimatorClipInfos.Length; i++)
		{
			AnimatorClipInfo animatorClipInfo = m_AnimatorClipInfos[i];
			if (animatorClipInfo.clip.isLooping)
			{
				return true;
			}
		}
		return false;
	}

	public void SetAnimObj(GameObject sprite)
	{
		Init();
		m_Sprite = sprite;
		m_Animator = m_Sprite.GetComponentInChildren<Animator>(includeInactive: true);
		if (m_Animator == null)
		{
			Init();
			return;
		}
		if (m_Animator.enabled)
		{
			m_Animator.enabled = false;
		}
		SetSpriteActive();
	}

	private void SetSpriteActive()
	{
		if (!m_Animator.gameObject.activeSelf || !m_Animator.gameObject.activeInHierarchy)
		{
			m_bSpriteActive = false;
		}
		else
		{
			m_bSpriteActive = true;
		}
	}

	public AnimationClip GetAnimClipByName(string animClipName)
	{
		if (m_Animator == null)
		{
			Debug.LogError("Animator Null!");
			return null;
		}
		if (m_Animator.runtimeAnimatorController == null)
		{
			Debug.LogError(m_Animator.gameObject.name + " has no animation controller!!");
			return null;
		}
		AnimationClip[] animationClips = m_Animator.runtimeAnimatorController.animationClips;
		foreach (AnimationClip animationClip in animationClips)
		{
			if (animationClip == null)
			{
				Debug.LogError("Animation null : " + m_Animator.gameObject.name);
			}
			else if (animClipName.CompareTo(animationClip.name) == 0)
			{
				return animationClip;
			}
		}
		return null;
	}

	public void Update(float deltaTime)
	{
		if (m_Sprite == null || m_Animator == null)
		{
			return;
		}
		if (!m_bSpriteActive)
		{
			SetSpriteActive();
			if (m_bSpriteActive)
			{
				Play(m_AnimName, m_bLoop, m_AnimTimeNormal);
			}
		}
		else
		{
			SetSpriteActive();
		}
		if (!m_bSpriteActive)
		{
			return;
		}
		m_bAnimStartThisFrame = false;
		m_AnimTimeBefore = m_AnimTimeNow;
		m_AnimTimeNow = GetAnimTimeNow(bNormalTime: false);
		m_AnimTimeNormal = GetAnimTimeNow(bNormalTime: true);
		m_AnimatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
		if (!m_AnimatorStateInfo.IsName(m_AnimName))
		{
			UpdateAnimator(deltaTime);
		}
		else if (IsAnimationEnd())
		{
			if (m_dicAnimAutoChange.ContainsKey(m_AnimName))
			{
				AnimAutoChange animAutoChange = m_dicAnimAutoChange[m_AnimName];
				SetPlaySpeed(animAutoChange.m_fAnimSpeed);
				Play(animAutoChange.m_AnimName, animAutoChange.m_bLoop);
			}
			else if (m_bLoop && m_bAnimationEnd)
			{
				Play(m_AnimName, m_bLoop);
			}
			m_bAnimationEnd = true;
		}
		else
		{
			UpdateAnimator(deltaTime);
		}
	}

	public void UpdateAnimator(float deltaTime)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null && gameOptionData.AnimationQuality == NKCGameOptionDataSt.GraphicOptionAnimationQuality.Normal)
		{
			m_bHalfUpdate = true;
		}
		if (m_bHalfUpdate)
		{
			m_fUpdateDeltaTime += deltaTime;
			m_bUpdateThisFrame = !m_bUpdateThisFrame;
			if (m_bUpdateThisFrame)
			{
				m_Animator.Update(m_fUpdateDeltaTime);
				m_fUpdateDeltaTime = 0f;
			}
		}
		else
		{
			m_Animator.Update(deltaTime);
		}
	}

	public void SetAnimAutoChange(string animName, string nextAnimName, bool bLoop, float fAnimSpeed)
	{
		AnimAutoChange animAutoChange = null;
		if (!m_dicAnimAutoChange.ContainsKey(animName))
		{
			animAutoChange = new AnimAutoChange();
			m_dicAnimAutoChange.Add(animName, animAutoChange);
		}
		else
		{
			animAutoChange = m_dicAnimAutoChange[animName];
		}
		animAutoChange.m_AnimName = nextAnimName;
		animAutoChange.m_bLoop = bLoop;
		animAutoChange.m_fAnimSpeed = fAnimSpeed;
	}

	public void Play(string animName, bool bLoop, float fNormalTime = 0f)
	{
		if (!(m_Animator == null))
		{
			m_AnimName = animName;
			m_bLoop = bLoop;
			m_Animator.speed = m_fPlaySpeed;
			m_bAnimationEnd = false;
			m_bAnimStartThisFrame = true;
			m_AnimTimeNow = 0f;
			m_AnimTimeBefore = 0f;
			m_AnimTimeNormal = fNormalTime;
			if (!m_bSpriteActive)
			{
				SetSpriteActive();
			}
			if (m_bSpriteActive)
			{
				m_Animator.Play(m_AnimName, -1, m_AnimTimeNormal);
				m_Animator.Update(0.001f);
				m_AnimatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
				m_AnimatorClipInfos = m_Animator.GetCurrentAnimatorClipInfo(0);
			}
		}
	}

	public float GetAnimTimeNow(bool bNormalTime)
	{
		if (bNormalTime)
		{
			return m_AnimatorStateInfo.normalizedTime;
		}
		return m_AnimatorStateInfo.length * m_AnimatorStateInfo.normalizedTime;
	}

	public bool IsAnimationEnd()
	{
		if (m_AnimatorStateInfo.normalizedTime >= 1f)
		{
			return true;
		}
		return false;
	}

	public void SetShow(bool bShow)
	{
		if (m_bShow != bShow)
		{
			if (m_Sprite != null)
			{
				m_Sprite.SetActive(bShow);
			}
			m_bShow = bShow;
		}
	}

	public void SetPlaySpeed(float fSpeed)
	{
		m_fPlaySpeed = fSpeed;
		m_Animator.speed = m_fPlaySpeed;
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
