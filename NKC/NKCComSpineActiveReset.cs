using Spine;
using Spine.Unity;
using UnityEngine;

namespace NKC;

[DisallowMultipleComponent]
public class NKCComSpineActiveReset : MonoBehaviour
{
	public string m_AnimName = "BASE";

	public bool m_bLoop;

	private SkeletonAnimation m_SkeletonAnimation;

	private SkeletonGraphic m_SkeletonGraphic;

	private TrackEntry m_TrackEntry;

	private void Awake()
	{
		m_SkeletonAnimation = base.gameObject.GetComponent<SkeletonAnimation>();
		m_SkeletonGraphic = base.gameObject.GetComponent<SkeletonGraphic>();
		if (m_SkeletonAnimation != null)
		{
			m_SkeletonAnimation.Awake();
			m_SkeletonAnimation.enabled = false;
		}
		if (m_SkeletonGraphic != null)
		{
			m_SkeletonGraphic.enabled = false;
		}
	}

	private void OnEnable()
	{
		if (m_SkeletonAnimation != null)
		{
			m_SkeletonAnimation.AnimationState.SetAnimation(0, m_AnimName, m_bLoop);
			m_TrackEntry = m_SkeletonAnimation.AnimationState.GetCurrent(0);
			m_SkeletonAnimation.Update(0f);
			m_SkeletonAnimation.enabled = true;
		}
		if (m_SkeletonGraphic != null)
		{
			m_SkeletonGraphic.AnimationState.SetAnimation(0, m_AnimName, m_bLoop);
			m_TrackEntry = m_SkeletonGraphic.AnimationState.GetCurrent(0);
			m_SkeletonGraphic._Update(0f);
			m_SkeletonGraphic.enabled = true;
		}
	}

	private void OnDisable()
	{
		if (m_TrackEntry != null)
		{
			m_TrackEntry.TrackTime = 0f;
		}
	}
}
