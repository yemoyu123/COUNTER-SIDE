using UnityEngine;

namespace NKC;

[RequireComponent(typeof(RectTransform))]
public class NKCUIFitToCamera : MonoBehaviour
{
	private RectTransform m_rect;

	private void Awake()
	{
		m_rect = GetComponent<RectTransform>();
	}

	private void Update()
	{
		NKCCamera.FitRectToCamera(m_rect);
	}
}
