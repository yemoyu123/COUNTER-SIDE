using UnityEngine;

public class NKCUIMoveText : MonoBehaviour
{
	public RectTransform m_rtText;

	public float m_speed = 1f;

	private Vector2 m_oriPos;

	private void Start()
	{
		if (m_rtText != null)
		{
			m_oriPos = m_rtText.anchoredPosition;
		}
	}

	private void Update()
	{
		if (m_rtText == null)
		{
			return;
		}
		RectTransform component = GetComponent<RectTransform>();
		if (component == null)
		{
			return;
		}
		float width = component.rect.width;
		float width2 = m_rtText.rect.width;
		if (width >= width2)
		{
			m_rtText.anchoredPosition = m_oriPos;
			return;
		}
		Vector3 vector = m_rtText.anchoredPosition;
		vector.x -= m_speed;
		if (vector.x < 0f && Mathf.Abs(vector.x) >= width2)
		{
			vector.x = width;
		}
		m_rtText.anchoredPosition = vector;
	}
}
