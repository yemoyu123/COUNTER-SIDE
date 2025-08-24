using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIComRectScreen : MonoBehaviour
{
	[Flags]
	public enum ScreenExpand
	{
		None = 0,
		Left = 1,
		Right = 2,
		Up = 4,
		Down = 8
	}

	private ScreenExpand m_flagScreenExpand;

	public RectTransform m_rtCenter;

	public Image m_imgCenter;

	public EventTrigger m_etCenter;

	public RectTransform m_rtLeft;

	public RectTransform m_rtRight;

	public RectTransform m_rtTop;

	public RectTransform m_rtBottom;

	public RectOffset m_offsetCenter;

	private RectTransform m_rtTarget;

	private Renderer m_RendererTarget;

	private bool m_bIsFromMidCanvas;

	private bool m_bAttach;

	private bool CheckExpandFlag(ScreenExpand flag)
	{
		return (flag & m_flagScreenExpand) == flag;
	}

	public void SetScreen(Renderer targetRenderer, bool bAttach, ScreenExpand expandFlag = ScreenExpand.None)
	{
		if (targetRenderer == null)
		{
			m_rtCenter.pivot = Vector2.zero;
			m_rtCenter.localPosition = Vector3.zero;
			m_rtCenter.SetSize(Vector2.zero);
			RepositionRectByCenter();
			return;
		}
		m_bAttach = bAttach;
		m_flagScreenExpand = expandFlag;
		Bounds bounds = targetRenderer.bounds;
		Vector3 position = bounds.center - bounds.extents;
		Vector3 position2 = bounds.center + bounds.extents;
		Camera camera = NKCCamera.GetCamera();
		Camera subUICamera = NKCCamera.GetSubUICamera();
		Vector3 vector = subUICamera.ScreenToWorldPoint(camera.WorldToScreenPoint(position));
		vector.z = 0f;
		Vector3 vector2 = subUICamera.ScreenToWorldPoint(camera.WorldToScreenPoint(position2));
		vector2.z = 0f;
		vector.x -= m_offsetCenter.left;
		vector.y -= m_offsetCenter.bottom;
		vector2.x += m_offsetCenter.right;
		vector2.y += m_offsetCenter.top;
		if (CheckExpandFlag(ScreenExpand.Left))
		{
			vector.x = (float)(-Screen.width) * 0.5f;
		}
		if (CheckExpandFlag(ScreenExpand.Right))
		{
			vector2.x = (float)Screen.width * 0.5f;
		}
		if (CheckExpandFlag(ScreenExpand.Up))
		{
			vector.y = (float)Screen.height * 0.5f;
		}
		if (CheckExpandFlag(ScreenExpand.Down))
		{
			vector2.y = (float)(-Screen.height) * 0.5f;
		}
		m_rtCenter.pivot = Vector2.zero;
		m_rtCenter.position = vector;
		m_rtCenter.SetSize(vector2 - vector);
		RepositionRectByCenter();
		m_RendererTarget = (bAttach ? targetRenderer : null);
		m_rtTarget = null;
	}

	public void SetScreen(RectTransform target, bool bIsFromMidCanvas, bool bAttach)
	{
		if (target == null)
		{
			m_rtCenter.pivot = Vector2.zero;
			m_rtCenter.localPosition = Vector3.zero;
			m_rtCenter.SetSize(Vector2.zero);
			RepositionRectByCenter();
			return;
		}
		m_bAttach = bAttach;
		m_rtCenter.pivot = target.pivot;
		Vector2 vector;
		if (bIsFromMidCanvas)
		{
			Camera camera = NKCCamera.GetCamera();
			vector = NKCCamera.GetSubUICamera().ScreenToWorldPoint(camera.WorldToScreenPoint(target.position));
		}
		else
		{
			vector = target.position;
		}
		Vector2 size = target.GetSize();
		Vector2 vector2 = vector + size * (Vector2.one - target.pivot) * target.lossyScale;
		Vector2 vector3 = vector - size * target.pivot * target.lossyScale;
		Vector2 vector4 = (vector2 + vector3) * 0.5f;
		Vector2 newSize = vector2 - vector3;
		newSize.x = Mathf.Abs(newSize.x);
		newSize.y = Mathf.Abs(newSize.y);
		m_rtCenter.pivot = Vector2.one * 0.5f;
		m_rtCenter.position = vector4;
		newSize.x += m_offsetCenter.left + m_offsetCenter.right;
		newSize.y += m_offsetCenter.bottom + m_offsetCenter.top;
		m_rtCenter.SetSize(newSize);
		Vector3 vector5 = new Vector3((target.lossyScale.x != 0f) ? target.lossyScale.x : 1f, (target.lossyScale.y != 0f) ? target.lossyScale.y : 1f, (target.lossyScale.z != 0f) ? target.lossyScale.z : 1f);
		m_rtCenter.localScale = new Vector3(1f / vector5.x, 1f / vector5.y, 1f / vector5.z);
		RepositionRectByCenter();
		m_bIsFromMidCanvas = bIsFromMidCanvas;
		m_rtTarget = (bAttach ? target : null);
		m_RendererTarget = null;
	}

	private void ResetTouchSteal()
	{
		m_imgCenter.raycastTarget = false;
		m_etCenter.triggers.Clear();
	}

	public void SetTouchSteal(UnityAction<BaseEventData> onTouch)
	{
		m_imgCenter.raycastTarget = true;
		if (onTouch != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerDown;
			entry.callback.AddListener(onTouch);
			m_etCenter.triggers.Clear();
			m_etCenter.triggers.Add(entry);
		}
	}

	public void RepositionRectByCenter()
	{
		Vector2 size = GetComponent<RectTransform>().GetSize();
		Vector2 vector = m_rtCenter.anchorMin * size + m_rtCenter.anchoredPosition;
		float num = vector.x - m_rtCenter.pivot.x * m_rtCenter.GetWidth();
		float num2 = vector.x + (1f - m_rtCenter.pivot.x) * m_rtCenter.GetWidth();
		float newSize = vector.y - m_rtCenter.pivot.y * m_rtCenter.GetHeight();
		float num3 = vector.y + (1f - m_rtCenter.pivot.y) * m_rtCenter.GetHeight();
		m_rtLeft.SetWidth(num);
		m_rtRight.SetWidth(size.x - num2);
		m_rtTop.SetHeight(size.y - num3);
		m_rtTop.offsetMin = new Vector2(num, m_rtTop.offsetMin.y);
		m_rtTop.offsetMax = new Vector2(num2 - size.x, m_rtTop.offsetMax.y);
		m_rtBottom.SetHeight(newSize);
		m_rtBottom.offsetMin = new Vector2(num, m_rtBottom.offsetMin.y);
		m_rtBottom.offsetMax = new Vector2(num2 - size.x, m_rtBottom.offsetMax.y);
	}

	public void CleanUp()
	{
		m_rtTarget = null;
		m_RendererTarget = null;
		ResetTouchSteal();
	}

	private void Update()
	{
		if (m_bAttach)
		{
			if (m_rtTarget != null)
			{
				SetScreen(m_rtTarget, m_bIsFromMidCanvas, bAttach: true);
			}
			else if (m_RendererTarget != null)
			{
				SetScreen(m_RendererTarget, bAttach: true, m_flagScreenExpand);
			}
		}
	}

	public void SetAlpha(float a)
	{
		Color color = new Color(0f, 0f, 0f, a);
		NKCUtil.SetImageColor(m_imgCenter, color);
		NKCUtil.SetImageColor(m_rtBottom.GetComponent<Image>(), color);
		NKCUtil.SetImageColor(m_rtLeft.GetComponent<Image>(), color);
		NKCUtil.SetImageColor(m_rtRight.GetComponent<Image>(), color);
		NKCUtil.SetImageColor(m_rtTop.GetComponent<Image>(), color);
	}
}
