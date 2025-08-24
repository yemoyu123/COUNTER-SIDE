using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.UI.NPC;

public class NKCUINPCSpineIllust : MonoBehaviour
{
	public delegate void OnTouch();

	public SkeletonGraphic m_spUnitIllust;

	public bool m_bUseTouch = true;

	public bool m_bResetOnEnable = true;

	public OnTouch m_dOnTouch;

	private string defaultAnimation = "IDLE";

	private void Awake()
	{
		if (m_bUseTouch && m_spUnitIllust != null)
		{
			m_spUnitIllust.raycastTarget = false;
			EventTrigger eventTrigger = m_spUnitIllust.gameObject.GetComponent<EventTrigger>();
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
	}

	private void OnEnable()
	{
		if (m_spUnitIllust != null && m_spUnitIllust.IsValid && m_bResetOnEnable)
		{
			if (HasAnimation("START"))
			{
				m_spUnitIllust.AnimationState.SetAnimation(0, "START", loop: false);
				m_spUnitIllust.AnimationState.AddAnimation(0, defaultAnimation, loop: true, 0f);
			}
			else
			{
				m_spUnitIllust.AnimationState.SetAnimation(0, defaultAnimation, loop: true);
			}
		}
	}

	public void TouchIllust()
	{
		if (!(m_spUnitIllust != null))
		{
			return;
		}
		if (HasAnimation("TOUCH"))
		{
			m_spUnitIllust.AnimationState.SetAnimation(0, "TOUCH", loop: false);
			m_spUnitIllust.AnimationState.AddAnimation(0, defaultAnimation, loop: true, 0f);
			if (m_dOnTouch != null)
			{
				m_dOnTouch();
			}
		}
		else
		{
			m_spUnitIllust.AnimationState.SetAnimation(0, defaultAnimation, loop: true);
		}
	}

	public void SetAnimation(string animName)
	{
		if (!(m_spUnitIllust == null))
		{
			if (HasAnimation(animName))
			{
				m_spUnitIllust.AnimationState.SetAnimation(0, animName, loop: false);
				m_spUnitIllust.AnimationState.AddAnimation(0, defaultAnimation, loop: true, 0f);
			}
			else
			{
				m_spUnitIllust.AnimationState.SetAnimation(0, defaultAnimation, loop: true);
			}
		}
	}

	public void SetDefaultAnimation(string animName)
	{
		if (!(m_spUnitIllust == null))
		{
			if (HasAnimation(animName))
			{
				m_spUnitIllust.Skeleton?.SetToSetupPose();
				m_spUnitIllust.AnimationState.SetAnimation(0, animName, loop: true);
				defaultAnimation = animName;
			}
			else
			{
				m_spUnitIllust.Skeleton?.SetToSetupPose();
				m_spUnitIllust.AnimationState.SetAnimation(0, "IDLE", loop: true);
				defaultAnimation = "IDLE";
			}
		}
	}

	public string GetCurrentAnimationName()
	{
		return m_spUnitIllust.AnimationState.GetCurrent(0).Animation.Name;
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
}
