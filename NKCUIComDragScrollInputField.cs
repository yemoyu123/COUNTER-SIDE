using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NKCUIComDragScrollInputField : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler, IScrollHandler, IPointerDownHandler, IPointerUpHandler
{
	public InputField inputField;

	private LoopScrollRect scrollRect;

	private bool drag;

	private bool activeInput;

	public LoopScrollRect ScrollRect
	{
		set
		{
			scrollRect = value;
		}
	}

	public bool ActiveInput
	{
		set
		{
			activeInput = value;
		}
	}

	private void Awake()
	{
		inputField.enabled = false;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (scrollRect != null)
		{
			scrollRect.OnBeginDrag(eventData);
		}
		drag = true;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (scrollRect != null)
		{
			scrollRect.OnDrag(eventData);
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (scrollRect != null)
		{
			scrollRect.OnEndDrag(eventData);
		}
		drag = false;
	}

	public void OnScroll(PointerEventData data)
	{
		if (scrollRect != null)
		{
			scrollRect.OnScroll(data);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (!drag && !activeInput)
		{
			activeInput = true;
			inputField.enabled = true;
			inputField.Select();
			inputField.ActivateInputField();
		}
	}
}
