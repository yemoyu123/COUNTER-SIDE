using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NKC;

public class NKCUIComClick : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerClickHandler
{
	public UnityEvent PointerDown;

	public UnityEvent PointerUp;

	public UnityEvent PointerClick;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (PointerDown != null)
		{
			PointerDown.Invoke();
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (PointerUp != null)
		{
			PointerUp.Invoke();
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (PointerClick != null)
		{
			PointerClick.Invoke();
		}
	}
}
