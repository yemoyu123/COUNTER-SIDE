using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.UI.Component;

public class NKCUIComDraggableListSlot : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
{
	public delegate void OnDragEvent(NKCUIComDraggableListSlot slot, PointerEventData eventData);

	public Transform m_trDragMovePart;

	public OnDragEvent dOnBeginDrag;

	public OnDragEvent dOnDrag;

	public OnDragEvent dOnEndDrag;

	public int Index { get; set; }

	public void ReturnToPos(bool bAnim)
	{
		m_trDragMovePart.DOKill();
		m_trDragMovePart.SetParent(base.transform, worldPositionStays: true);
		m_trDragMovePart.localScale = Vector3.one;
		if (bAnim)
		{
			m_trDragMovePart.DOLocalMove(Vector3.zero, 0.4f).SetEase(Ease.OutCubic);
		}
		else
		{
			m_trDragMovePart.localPosition = Vector3.zero;
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		dOnBeginDrag?.Invoke(this, eventData);
	}

	public void OnDrag(PointerEventData eventData)
	{
		dOnDrag?.Invoke(this, eventData);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		dOnEndDrag?.Invoke(this, eventData);
	}
}
