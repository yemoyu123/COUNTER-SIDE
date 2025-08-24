using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NKC;

public class NKCUIComStateButton : NKCUIComStateButtonBase
{
	public NKCUnityEvent PointerDown = new NKCUnityEvent();

	public UnityEvent PointerUp;

	public UnityEvent PointerClick;

	public NKCUnityEventInt PointerClickWithData;

	protected override void OnPointerDownEvent(PointerEventData eventData)
	{
		if (PointerDown != null)
		{
			PointerDown.Invoke(eventData);
		}
	}

	protected override void OnPointerUpEvent(PointerEventData eventData)
	{
		if (PointerUp != null)
		{
			PointerUp.Invoke();
		}
	}

	protected override void OnPointerClickEvent(PointerEventData eventData)
	{
		if (PointerClick != null)
		{
			PointerClick.Invoke();
		}
		if (PointerClickWithData != null)
		{
			PointerClickWithData.Invoke(m_DataInt);
		}
	}
}
