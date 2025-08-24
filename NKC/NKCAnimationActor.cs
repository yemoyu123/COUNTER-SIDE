using NKC.UI.Component;
using UnityEngine;

namespace NKC;

public class NKCAnimationActor : MonoBehaviour, INKCAnimationActor
{
	public Animator m_Ani;

	public Transform m_trSDParent;

	public NKCUIComCharacterEmotion m_comEmotion;

	private NKCASUIUnitIllust m_UnitIllust;

	public Animator Animator => m_Ani;

	public Transform Transform => base.transform;

	public Transform SDParent => m_trSDParent;

	public void SetSpineIllust(NKCASUIUnitIllust illust, bool bSetParent = false)
	{
		if (bSetParent)
		{
			illust.SetParent(m_trSDParent, worldPositionStays: false);
		}
		m_UnitIllust = illust;
	}

	public NKCASUIUnitIllust GetSpineIllust()
	{
		return m_UnitIllust;
	}

	public Vector3 GetBonePosition(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return base.transform.position;
		}
		if (m_UnitIllust != null)
		{
			return m_UnitIllust.GetBoneWorldPosition(name);
		}
		Transform transform = base.transform.Find(name);
		if (!(transform != null))
		{
			return base.transform.position;
		}
		return transform.position;
	}

	public void PlaySpineAnimation(string name, bool loop, float timeScale)
	{
		if (m_UnitIllust != null)
		{
			m_UnitIllust.SetAnimation(name, loop, 0, bForceRestart: true, 0f, bReturnDefault: false);
			m_UnitIllust.SetTimeScale(timeScale);
		}
	}

	public void PlaySpineAnimation(NKCASUIUnitIllust.eAnimation eAnim, bool loop, float timeScale, bool bDefaultAnim)
	{
		if (m_UnitIllust != null)
		{
			if (bDefaultAnim)
			{
				m_UnitIllust.SetDefaultAnimation(eAnim);
			}
			else
			{
				m_UnitIllust.SetAnimation(eAnim, loop, 0, bForceRestart: true, 0f, bReturnDefault: false);
			}
			m_UnitIllust.SetTimeScale(timeScale);
		}
	}

	public bool IsSpineAnimationFinished(NKCASUIUnitIllust.eAnimation eAnim)
	{
		if (m_UnitIllust == null)
		{
			return true;
		}
		string animationName = NKCASUIUnitIllust.GetAnimationName(eAnim);
		return IsSpineAnimationFinished(animationName);
	}

	public bool IsSpineAnimationFinished(string name)
	{
		if (m_UnitIllust == null)
		{
			return true;
		}
		if (m_UnitIllust.GetCurrentAnimationName() == name && m_UnitIllust.GetAnimationTime(name) > m_UnitIllust.GetCurrentAnimationTime())
		{
			return false;
		}
		return true;
	}

	public void PlayEmotion(string animName, float speed)
	{
		if (m_comEmotion != null)
		{
			m_comEmotion.Play(animName, speed);
		}
	}

	public bool CanPlaySpineAnimation(string name)
	{
		if (m_UnitIllust != null)
		{
			return m_UnitIllust.HasAnimation(name);
		}
		return false;
	}

	public bool CanPlaySpineAnimation(NKCASUIUnitIllust.eAnimation eAnim)
	{
		if (m_UnitIllust != null)
		{
			return m_UnitIllust.HasAnimation(eAnim);
		}
		return false;
	}
}
