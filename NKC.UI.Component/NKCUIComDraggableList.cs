using System.Collections.Generic;
using DG.Tweening;
using NKC.Util;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.UI.Component;

public class NKCUIComDraggableList : MonoBehaviour
{
	public delegate void OnSlotSwapped(int oldIndex, int newIndex);

	public List<NKCUIComDraggableListSlot> m_lstDraggableSlot;

	public OnSlotSwapped dOnSlotSwapped;

	public RectTransform m_rtRootDragging;

	private Transform m_trCurrentDragging;

	public bool m_bUseSwapAnimation;

	public bool m_bResetIndexOnAwake = true;

	private Canvas m_myCanvas;

	private NKCUIComDraggableListSlot m_currentSwapTarget;

	private bool m_bInit;

	private void Awake()
	{
		Init(m_bResetIndexOnAwake);
	}

	public void Init(bool bResetIndex)
	{
		if (m_bInit)
		{
			return;
		}
		m_myCanvas = NKCUIUtility.FindCanvas(base.transform);
		for (int i = 0; i < m_lstDraggableSlot.Count; i++)
		{
			NKCUIComDraggableListSlot nKCUIComDraggableListSlot = m_lstDraggableSlot[i];
			if (bResetIndex)
			{
				nKCUIComDraggableListSlot.Index = i;
			}
			nKCUIComDraggableListSlot.dOnBeginDrag = OnBeginDragSlot;
			nKCUIComDraggableListSlot.dOnDrag = OnDragSlot;
			nKCUIComDraggableListSlot.dOnEndDrag = OnEndDragSlot;
		}
		m_bInit = true;
	}

	private void OnDisable()
	{
		ResetPosition();
	}

	public void ResetPosition(bool bAnim = false)
	{
		foreach (NKCUIComDraggableListSlot item in m_lstDraggableSlot)
		{
			item.ReturnToPos(bAnim);
		}
	}

	private void Swap(int oldIndex, int newIndex)
	{
		Debug.Log($"Swap {oldIndex} {newIndex}");
		dOnSlotSwapped?.Invoke(oldIndex, newIndex);
	}

	private void OnBeginDragSlot(NKCUIComDraggableListSlot slot, PointerEventData eventData)
	{
		m_trCurrentDragging = slot.m_trDragMovePart;
		m_trCurrentDragging.SetParent(m_rtRootDragging, worldPositionStays: true);
		m_trCurrentDragging.localScale = Vector3.one;
	}

	private void OnDragSlot(NKCUIComDraggableListSlot slot, PointerEventData eventData)
	{
		if (!(m_trCurrentDragging != null))
		{
			return;
		}
		RectTransformUtility.ScreenPointToLocalPointInRectangle(m_rtRootDragging, eventData.position, m_myCanvas.worldCamera, out var localPoint);
		m_trCurrentDragging.localPosition = localPoint;
		if (!m_bUseSwapAnimation)
		{
			return;
		}
		NKCUIComDraggableListSlot nKCUIComDraggableListSlot = GetSlotFromPoint(eventData);
		if (nKCUIComDraggableListSlot == slot)
		{
			nKCUIComDraggableListSlot = null;
		}
		if (nKCUIComDraggableListSlot != m_currentSwapTarget)
		{
			if (m_currentSwapTarget != null)
			{
				m_currentSwapTarget.ReturnToPos(bAnim: true);
				m_currentSwapTarget = null;
			}
			if (nKCUIComDraggableListSlot != null)
			{
				m_currentSwapTarget = nKCUIComDraggableListSlot;
				m_currentSwapTarget.m_trDragMovePart.SetParent(m_rtRootDragging, worldPositionStays: true);
				m_currentSwapTarget.m_trDragMovePart.localScale = Vector3.one;
				m_currentSwapTarget.m_trDragMovePart.DOKill();
				m_currentSwapTarget.m_trDragMovePart.DOMove(slot.transform.position, 0.4f).SetEase(Ease.OutCubic);
			}
		}
	}

	private void OnEndDragSlot(NKCUIComDraggableListSlot slot, PointerEventData eventData)
	{
		NKCUIComDraggableListSlot slotFromPoint = GetSlotFromPoint(eventData);
		if (slotFromPoint != null && slotFromPoint != slot)
		{
			if (m_currentSwapTarget != null)
			{
				m_currentSwapTarget.ReturnToPos(bAnim: false);
			}
			slot.ReturnToPos(bAnim: false);
			Swap(slot.Index, slotFromPoint.Index);
		}
		else
		{
			if (m_currentSwapTarget != null)
			{
				m_currentSwapTarget.ReturnToPos(bAnim: true);
			}
			slot.ReturnToPos(bAnim: true);
		}
		m_currentSwapTarget = null;
		m_trCurrentDragging = null;
	}

	private NKCUIComDraggableListSlot GetSlotFromPoint(PointerEventData eventData)
	{
		if (eventData.pointerCurrentRaycast.gameObject != null)
		{
			NKCUIComDraggableListSlot component = base.gameObject.GetComponent<NKCUIComDraggableListSlot>();
			if (component != null)
			{
				return component;
			}
		}
		foreach (NKCUIComDraggableListSlot item in m_lstDraggableSlot)
		{
			RectTransform component2 = item.GetComponent<RectTransform>();
			if (!(component2 == null) && RectTransformUtility.RectangleContainsScreenPoint(component2, eventData.position, m_myCanvas.worldCamera))
			{
				return item;
			}
		}
		return null;
	}
}
