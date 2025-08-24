using UnityEngine;

namespace NKC;

public class NKCUIFollowCamera : MonoBehaviour
{
	[Header("해당 (화면 비율)값 미만인 경우 동작합니다.")]
	public float m_fScreenRateActiveValue = 2f;

	[Header("카메라와의 거리 Z축")]
	public float m_fDistanceToCameraZ = 1100f;

	[Header("scale이 적용될 타겟")]
	public RectTransform m_rtBackground;

	[Header("scale이 적용될 타겟에 추가 사이즈")]
	public Vector2 m_vecAddValue;

	private Vector3 m_distanceToCamera;

	private bool m_bFollowCam;

	public void Init(RectTransform rt)
	{
		float num = (float)Screen.width / (float)Screen.height;
		if (m_fScreenRateActiveValue > num)
		{
			m_rtBackground.sizeDelta = rt.sizeDelta + m_vecAddValue;
			m_rtBackground.localPosition = rt.localPosition;
			NKCCamera.RescaleRectToCameraFrustrum(m_rtBackground, NKCCamera.GetCamera(), Vector2.zero, 0f - m_fDistanceToCameraZ, NKCCamera.FitMode.FitToWidth);
		}
		Debug.Log("check ScreenRatio : " + num);
		m_bFollowCam = NKCCamera.GetTrackingPos().GetPause();
		m_distanceToCamera = new Vector3(0f, 0f, m_fDistanceToCameraZ);
	}

	private void Update()
	{
		if (m_bFollowCam)
		{
			base.transform.position = NKCCamera.GetCamera().transform.position + m_distanceToCamera;
		}
	}
}
