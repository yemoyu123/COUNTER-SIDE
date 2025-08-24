using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropUnit : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IEndDragHandler, IDragHandler
{
	[Range(0f, 1f)]
	public float MinScale = 0.3f;

	[Range(1f, 3f)]
	public float MaxScale = 2f;

	[Range(0f, 0.5f)]
	public float WheelFactor = 0.05f;

	private Canvas canvas;

	private RectTransform rectTransform;

	private CanvasGroup canvasGroup;

	private float scaleFactor = 1f;

	private Vector3 vec3;

	private void Awake()
	{
		canvas = GameObject.Find("NKM_SCEN_UI_MID_Canvas").GetComponent<Canvas>();
		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();
	}

	private void Update()
	{
		_ = Input.mouseScrollDelta;
		scaleFactor = rectTransform.localScale.y + Input.mouseScrollDelta.y * WheelFactor;
		if (scaleFactor > MinScale && scaleFactor < MaxScale)
		{
			vec3.Set(scaleFactor, scaleFactor, 1f);
			rectTransform.localScale = vec3;
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		canvasGroup.blocksRaycasts = false;
	}

	public void OnDrag(PointerEventData eventData)
	{
		rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		canvasGroup.blocksRaycasts = true;
	}
}
