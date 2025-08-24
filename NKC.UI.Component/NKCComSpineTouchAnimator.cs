using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.UI.Component;

public class NKCComSpineTouchAnimator : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	[Header("기본 애니 이름. 반드시 존재해야 함")]
	public string ANIM_BASE = "BASE";

	[Header("터치 애니 이름. 여러 개의 애니가 존재하는 경우 TOUCH, TOUCH1, TOUCH2.. 등으로 애니를 만들어 넣으면 랜덤하게 하나를 재생")]
	public string ANIM_TOUCH = "TOUCH";

	private SkeletonGraphic[] m_aSkeletonGraphics;

	public void OnPointerClick(PointerEventData eventData)
	{
		if (m_aSkeletonGraphics == null)
		{
			m_aSkeletonGraphics = GetComponentsInChildren<SkeletonGraphic>();
		}
		List<string> list = new List<string>();
		if (HasAnimation(m_aSkeletonGraphics, ANIM_TOUCH))
		{
			list.Add(ANIM_TOUCH);
		}
		int num = 1;
		while (HasAnimation(m_aSkeletonGraphics, ANIM_TOUCH + num))
		{
			list.Add(ANIM_TOUCH + num);
			num++;
		}
		if (list.Count > 0)
		{
			string animName = list[Random.Range(0, list.Count)];
			SetAnimation(m_aSkeletonGraphics, animName, loop: false);
			AddAnimation(m_aSkeletonGraphics, ANIM_BASE, loop: true);
		}
	}

	protected void SetAnimation(SkeletonGraphic[] aTarget, string AnimName, bool loop, float timeScale = 1f)
	{
		if (aTarget == null)
		{
			return;
		}
		foreach (SkeletonGraphic skeletonGraphic in aTarget)
		{
			if (HasAnimation(skeletonGraphic, AnimName))
			{
				skeletonGraphic.Skeleton?.SetToSetupPose();
				skeletonGraphic.AnimationState.SetAnimation(0, AnimName, loop).TimeScale = timeScale;
			}
		}
	}

	protected bool HasAnimation(SkeletonGraphic target, string AnimName)
	{
		if (target == null || target.SkeletonData == null)
		{
			return false;
		}
		return target.SkeletonData.FindAnimation(AnimName) != null;
	}

	protected bool HasAnimation(SkeletonGraphic[] aTarget, string AnimName)
	{
		if (aTarget == null)
		{
			return false;
		}
		foreach (SkeletonGraphic skeletonGraphic in aTarget)
		{
			if (skeletonGraphic.SkeletonData != null && skeletonGraphic.SkeletonData.FindAnimation(AnimName) != null)
			{
				return true;
			}
		}
		return false;
	}

	protected void AddAnimation(SkeletonGraphic target, string AnimName, bool loop)
	{
		if (HasAnimation(target, AnimName))
		{
			target.AnimationState.AddAnimation(0, AnimName, loop, 0f);
		}
	}

	protected void AddAnimation(SkeletonGraphic[] aTarget, string AnimName, bool loop)
	{
		if (aTarget == null)
		{
			return;
		}
		foreach (SkeletonGraphic skeletonGraphic in aTarget)
		{
			if (HasAnimation(skeletonGraphic, AnimName))
			{
				skeletonGraphic.AnimationState.AddAnimation(0, AnimName, loop, 0f);
			}
		}
	}
}
