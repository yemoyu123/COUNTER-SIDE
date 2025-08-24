using NKM;
using UnityEngine;

namespace NKC;

public class NKCASUnitViewerSpineSprite : NKCASUnitSpineSprite
{
	private NKCComSpineSkeletonAnimationEvent m_NKCComSpineSkeletonAnimationEvent;

	public NKCASUnitViewerSpineSprite(string bundleName, string name, bool bAsync = false)
		: base(bundleName, name, bAsync)
	{
		m_NKM_OBJECT_POOL_TYPE = NKM_OBJECT_POOL_TYPE.NOPT_NKCASUnitViewerSpineSprite;
	}

	public override bool LoadComplete()
	{
		if (!base.LoadComplete())
		{
			return false;
		}
		if (m_UnitSpineSpriteInstant != null && m_UnitSpineSpriteInstant.m_Instant != null)
		{
			NKCComSpineSkeletonAnimationEvent componentInChildren = m_UnitSpineSpriteInstant.m_Instant.GetComponentInChildren<NKCComSpineSkeletonAnimationEvent>();
			if (componentInChildren != null)
			{
				Object.Destroy(componentInChildren);
			}
			Transform transform = m_UnitSpineSpriteInstant.m_Instant.transform.Find("VFX");
			if (transform != null)
			{
				Object.Destroy(transform.gameObject);
			}
			Transform transform2 = m_UnitSpineSpriteInstant.m_Instant.transform.Find("VFX_STATIC");
			if (transform2 != null)
			{
				Object.Destroy(transform2.gameObject);
			}
		}
		else
		{
			Debug.LogError("NKCASUnitSpineSprite 로드 실패. ObjectPoolBundlename : " + m_ObjectPoolBundleName + " / ObjectPoolName : " + m_ObjectPoolName);
		}
		if (m_SPINE_SkeletonAnimation != null)
		{
			m_NKCComSpineSkeletonAnimationEvent = m_SPINE_SkeletonAnimation.GetComponent<NKCComSpineSkeletonAnimationEvent>();
			if (m_NKCComSpineSkeletonAnimationEvent != null)
			{
				m_NKCComSpineSkeletonAnimationEvent.SetActiveEvent(bActiveEvent: false);
			}
			else
			{
				Debug.LogError("NKCComSpineSkeletonAnimationEvent를 찾지 못했습니다. ObjectPoolBundlename : " + m_ObjectPoolBundleName + " / ObjectPoolName : " + m_ObjectPoolName);
			}
		}
		return true;
	}

	public override void Unload()
	{
		m_NKCComSpineSkeletonAnimationEvent = null;
		base.Unload();
	}
}
