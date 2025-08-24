using System;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.UI.Component;

public class NKCUIComSkeletonAnimation : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	[Serializable]
	public struct EventSoundData
	{
		public string eventKey;

		public string audioClipName;
	}

	[Header("Skeleton Graphic")]
	public SkeletonGraphic m_SkeletonGraphic;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\u05b4ϸ\ufffd\ufffd\u033c\ufffd")]
	public string m_strStartAnimationName;

	[Header("\ufffd⺻ \ufffd\u05b4ϸ\ufffd\ufffd\u033c\ufffd")]
	public bool m_bLoopIdleAnimation = true;

	public List<string> m_lstIdleAnimationName;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public List<EventSoundData> m_lstEventSound;

	[Header("\ufffd\ufffdġ \ufffd\u05b4ϸ\ufffd\ufffd\u033c\ufffd")]
	public bool m_bTouchAni;

	public List<string> m_lstTouchAni;

	public float m_fTouchCheckDistance = 0.1f;

	private Vector2 m_vecTouchStartPosition = Vector2.zero;

	private void Start()
	{
		if (null == m_SkeletonGraphic)
		{
			Debug.LogError("NKCUIComSkeletonAnimation : target is null GameObject[" + base.name + "] StartAnim[" + m_strStartAnimationName + "]");
		}
		else if (m_SkeletonGraphic.AnimationState == null)
		{
			Debug.LogError("NKCUIComSkeletonAnimation : AnimationState is null GameObject[" + base.name + "] StartAnim[" + m_strStartAnimationName + "]");
		}
		else
		{
			m_SkeletonGraphic.AnimationState.Complete += SetNextAni;
			m_SkeletonGraphic.AnimationState.Event += HandleEvent;
			if (!string.IsNullOrEmpty(m_strStartAnimationName))
			{
				m_SkeletonGraphic.AnimationState.SetAnimation(0, m_strStartAnimationName, loop: false);
			}
		}
	}

	private void SetNextAni(TrackEntry trackEntry)
	{
		if (string.Equals(m_strStartAnimationName, trackEntry.Animation.Name) || m_lstTouchAni.Contains(trackEntry.Animation.Name))
		{
			int index = UnityEngine.Random.Range(0, m_lstIdleAnimationName.Count);
			m_SkeletonGraphic.AnimationState.SetAnimation(0, m_lstIdleAnimationName[index].ToString(), m_bLoopIdleAnimation);
		}
	}

	private void HandleEvent(TrackEntry trackEntry, Spine.Event e)
	{
		Debug.Log($"<color=red>Get HandleEvent : {e.String}/{e.Data}</color>");
		foreach (EventSoundData item in m_lstEventSound)
		{
			if (string.Equals(item.eventKey, e.String))
			{
				NKCSoundManager.PlaySound(item.audioClipName, 1f, 0f, 0f);
			}
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		m_vecTouchStartPosition = eventData.position;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (m_bTouchAni && m_lstTouchAni.Count > 0 && Vector2.Distance(m_vecTouchStartPosition, eventData.position) <= m_fTouchCheckDistance)
		{
			int index = UnityEngine.Random.Range(0, m_lstTouchAni.Count);
			m_SkeletonGraphic.AnimationState.SetAnimation(0, m_lstTouchAni[index].ToString(), loop: false);
		}
	}
}
