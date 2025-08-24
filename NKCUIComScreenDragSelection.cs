using NKC;
using UnityEngine;

public class NKCUIComScreenDragSelection : MonoBehaviour
{
	public delegate void OnDragging(Vector2 minPos, Vector2 maxPos, bool bDragEnd);

	public RectTransform selectionBox;

	public RectTransform targetRect;

	private Vector2 m_vStartPos;

	private Vector2 m_vEndPos;

	private Vector2 m_vMinPos;

	private Vector2 m_vMaxPos;

	private OnDragging m_dOnDragging;

	public void Init(OnDragging dOnDrag)
	{
		m_dOnDragging = dOnDrag;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRect, Input.mousePosition, NKCCamera.GetSubUICamera(), out m_vStartPos);
			m_vEndPos = m_vStartPos;
		}
		if (Input.GetMouseButton(0))
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRect, Input.mousePosition, NKCCamera.GetSubUICamera(), out m_vEndPos);
			DrawSelectionBox();
		}
		if (Input.GetMouseButtonUp(0))
		{
			selectionBox.gameObject.SetActive(value: false);
			if (m_dOnDragging != null)
			{
				m_dOnDragging(m_vMinPos, m_vMaxPos, bDragEnd: true);
			}
		}
	}

	private void DrawSelectionBox()
	{
		if (!selectionBox.gameObject.activeSelf)
		{
			selectionBox.gameObject.SetActive(value: true);
		}
		m_vMinPos = Vector2.Min(m_vStartPos, m_vEndPos);
		m_vMaxPos = Vector2.Max(m_vStartPos, m_vEndPos);
		Vector2 vector = m_vEndPos - m_vStartPos;
		selectionBox.anchoredPosition = m_vStartPos + vector / 2f;
		selectionBox.sizeDelta = new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
		SelectObjects();
	}

	private void SelectObjects()
	{
		if (m_dOnDragging != null)
		{
			m_dOnDragging(m_vMinPos, m_vMaxPos, bDragEnd: false);
		}
	}
}
