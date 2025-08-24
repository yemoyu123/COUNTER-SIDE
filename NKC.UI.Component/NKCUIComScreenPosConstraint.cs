using UnityEngine;

namespace NKC.UI.Component;

public class NKCUIComScreenPosConstraint : MonoBehaviour
{
	public Vector2 relativePosition;

	public Vector2 offset;

	private void LateUpdate()
	{
		SetPosition();
	}

	private void SetPosition()
	{
		Vector2 screenPoint = default(Vector2);
		screenPoint.x = (float)Screen.width * relativePosition.x + offset.x;
		screenPoint.y = (float)Screen.height * relativePosition.y + offset.y;
		RectTransformUtility.ScreenPointToWorldPointInRectangle(NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCanvas), screenPoint, NKCCamera.GetSubUICamera(), out var worldPoint);
		base.transform.position = worldPoint;
	}
}
