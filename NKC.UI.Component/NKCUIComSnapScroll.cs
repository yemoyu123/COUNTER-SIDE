using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Component;

[RequireComponent(typeof(ScrollRect))]
public class NKCUIComSnapScroll : MonoBehaviour, IEndDragHandler, IEventSystemHandler, IScrollHandler
{
	public delegate void OnCurrentSlotChanged(RectTransform rectTransform);

	private ScrollRect m_scrollRect;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd Snap\ufffd\ufffdų\ufffd\ufffd")]
	public float m_fSnapPivot = 0.5f;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffdð\ufffd")]
	public float m_fSnapTime = 0.4f;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffdν\ufffd \ufffdӵ\ufffd")]
	public float m_fMinSnapVelocity = 170f;

	[Header("\ufffd\ufffdǥ \ufffdε\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdٲ\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\u07bd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd")]
	public bool m_bGetMessageForEveryTargetSlotChange;

	[Header("\ufffd\ufffd\ufffd콺 \ufffd\ufffd \ufffd\u07be\ufffd\ufffd\ufffd \ufffd\ufffd \ufffd\ufffdũ\ufffd\ufffd \ufffdӵ\ufffd")]
	public float m_fScrollTime = 0.1f;

	public OnCurrentSlotChanged dOnCurrentSlotChanged;

	private int m_CurrentTargetSlotIndex = -1;

	private RectTransform m_CurrentTargetSlot;

	private bool m_bCanSnap;

	private ScrollRect ScrollRect
	{
		get
		{
			if (m_scrollRect == null)
			{
				m_scrollRect = GetComponent<ScrollRect>();
			}
			return m_scrollRect;
		}
	}

	private RectTransform ViewPort => ScrollRect.viewport;

	private RectTransform Content => ScrollRect.content;

	private bool bVertical => ScrollRect.vertical;

	private float ViewPortSize
	{
		get
		{
			if (!bVertical)
			{
				return ViewPort.GetWidth();
			}
			return ViewPort.GetHeight();
		}
	}

	public int CurrentTargetedSlotIndex => m_CurrentTargetSlotIndex;

	public RectTransform CurrentTargetSlot => m_CurrentTargetSlot;

	private void Start()
	{
		ScrollRect.scrollSensitivity = 0f;
	}

	private float GetContentDefaultPosition()
	{
		float num = (bVertical ? (Content.GetHeight() * (Content.pivot.y - 1f)) : (Content.GetWidth() * (1f - Content.pivot.x)));
		float num2 = (bVertical ? (ViewPortSize * (Content.pivot.y - 1f)) : (ViewPortSize * Content.pivot.x));
		return num - num2;
	}

	private float GetSlotSnapContentPosition(RectTransform targetSlot)
	{
		float num = (bVertical ? (targetSlot.anchoredPosition.y + targetSlot.GetHeight() * (1f - targetSlot.pivot.y)) : (targetSlot.anchoredPosition.x + targetSlot.GetWidth() * targetSlot.pivot.x));
		float num2 = (bVertical ? (targetSlot.GetHeight() * (1f - m_fSnapPivot)) : (targetSlot.GetWidth() * m_fSnapPivot));
		return num - num2;
	}

	private float GetSlotSnapPosition(RectTransform targetSlot)
	{
		float contentDefaultPosition = GetContentDefaultPosition();
		float slotSnapContentPosition = GetSlotSnapContentPosition(targetSlot);
		float num = (bVertical ? (ViewPortSize * (1f - m_fSnapPivot)) : (ViewPortSize * m_fSnapPivot));
		return contentDefaultPosition - slotSnapContentPosition - num;
	}

	public void ScrollToSnapIndex(int index, float time)
	{
		if (index >= 0 && index < Content.childCount)
		{
			RectTransform targetSlot = Content.GetChild(index) as RectTransform;
			ScrollToSnapPos(targetSlot, time);
		}
	}

	public void ScrollToSnapPos(RectTransform targetSlot, float time)
	{
		if (targetSlot == null)
		{
			return;
		}
		float slotSnapPosition = GetSlotSnapPosition(targetSlot);
		m_bCanSnap = false;
		ScrollRect.velocity = Vector2.zero;
		if (time <= 0f)
		{
			Content.anchoredPosition = (bVertical ? new Vector2(Content.anchoredPosition.x, slotSnapPosition) : new Vector2(slotSnapPosition, Content.anchoredPosition.y));
		}
		else
		{
			Content.DOKill();
			if (bVertical)
			{
				Content.DOAnchorPosY(slotSnapPosition, time);
			}
			else
			{
				Content.DOAnchorPosX(slotSnapPosition, time);
			}
		}
		dOnCurrentSlotChanged?.Invoke(targetSlot);
	}

	private (int, RectTransform) FindSnapCandidate()
	{
		if (Content.childCount == 0)
		{
			return (-1, null);
		}
		RectTransform rectTransform = Content.GetChild(0) as RectTransform;
		if (rectTransform == null)
		{
			return (-1, null);
		}
		int item = 0;
		float num = (bVertical ? Content.anchoredPosition.y : Content.anchoredPosition.x);
		float num2 = Mathf.Abs(num - GetSlotSnapPosition(rectTransform));
		for (int i = 1; i < Content.childCount; i++)
		{
			RectTransform rectTransform2 = Content.GetChild(i) as RectTransform;
			if (!(rectTransform2 == null))
			{
				float num3 = Mathf.Abs(num - GetSlotSnapPosition(rectTransform2));
				if (num3 < num2)
				{
					num2 = num3;
					item = i;
					rectTransform = rectTransform2;
				}
			}
		}
		return (item, rectTransform);
	}

	private void Update()
	{
		var (num, rectTransform) = FindSnapCandidate();
		if (m_CurrentTargetSlotIndex != num)
		{
			m_CurrentTargetSlotIndex = num;
			m_CurrentTargetSlot = rectTransform;
			if (m_bGetMessageForEveryTargetSlotChange)
			{
				dOnCurrentSlotChanged?.Invoke(rectTransform);
			}
		}
		if (m_bCanSnap && Mathf.Abs(ScrollRect.velocity.x + ScrollRect.velocity.y) <= m_fMinSnapVelocity)
		{
			ScrollToSnapPos(rectTransform, m_fSnapTime);
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		m_bCanSnap = true;
	}

	public void OnScroll(PointerEventData eventData)
	{
		if (eventData.scrollDelta.y < 0f)
		{
			ScrollToSnapIndex(m_CurrentTargetSlotIndex + 1, m_fScrollTime);
		}
		else if (eventData.scrollDelta.y > 0f)
		{
			ScrollToSnapIndex(m_CurrentTargetSlotIndex - 1, m_fScrollTime);
		}
		eventData.Use();
	}
}
