using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC;

public class NKCUIComDrag : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
{
	public NKCUnityEvent BeginDrag = new NKCUnityEvent();

	public NKCUnityEvent Drag = new NKCUnityEvent();

	public NKCUnityEvent EndDrag = new NKCUnityEvent();

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (BeginDrag != null)
		{
			BeginDrag.Invoke(eventData);
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (Drag != null)
		{
			Drag.Invoke(eventData);
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (EndDrag != null)
		{
			EndDrag.Invoke(eventData);
		}
	}
}
