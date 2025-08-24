using System;
using UnityEngine;

namespace NKC.UI.Office;

public class NKCUIOfficeMinimap : MonoBehaviour
{
	public float m_fBgScrollRange;

	public Transform m_background;

	public RectTransform m_rtBgImage;

	public NKCUIOfficeMinimapFacility m_NKCUIMinimapFacility;

	public NKCUIOfficeMinimapRoom m_NKCUIMinimapRoom;

	private Rect m_rtCamBoundRect;

	public void Init()
	{
		m_background?.SetParent(NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIMidCanvas));
		CalcCamMoveBoundRect();
		NKCUIOfficeMinimapFacility nKCUIMinimapFacility = m_NKCUIMinimapFacility;
		nKCUIMinimapFacility.m_dOnScrollCamMove = (NKCUIOfficeMinimapFacility.OnScroll)Delegate.Combine(nKCUIMinimapFacility.m_dOnScrollCamMove, new NKCUIOfficeMinimapFacility.OnScroll(MoveCameraByScroll));
		NKCUIOfficeMinimapRoom nKCUIMinimapRoom = m_NKCUIMinimapRoom;
		nKCUIMinimapRoom.m_dOnScrollCamMove = (NKCUIOfficeMinimapRoom.OnScroll)Delegate.Combine(nKCUIMinimapRoom.m_dOnScrollCamMove, new NKCUIOfficeMinimapRoom.OnScroll(MoveCameraByScroll));
		m_NKCUIMinimapFacility.Init();
		m_NKCUIMinimapRoom.Init();
	}

	public void SetAcive(bool value)
	{
		base.gameObject.SetActive(value);
		NKCUtil.SetGameobjectActive(m_background.gameObject, value);
		if (value)
		{
			NKCCamera.GetCamera().orthographic = false;
			if (m_NKCUIMinimapFacility.gameObject.activeSelf)
			{
				m_NKCUIMinimapFacility.UpdateRedDotAll();
			}
			if (m_NKCUIMinimapRoom.gameObject.activeSelf)
			{
				m_NKCUIMinimapRoom.UpdateRedDotAll();
			}
		}
	}

	public void SetActiveFacility(bool value)
	{
		m_NKCUIMinimapFacility.SetActive(value);
		m_NKCUIMinimapRoom.SetActive(!value);
	}

	public void SetActiveRoom(bool value)
	{
		m_NKCUIMinimapFacility.SetActive(!value);
		m_NKCUIMinimapRoom.SetActive(value);
	}

	public void CalcCamMoveBoundRect()
	{
		NKCCamera.RescaleRectToCameraFrustrum(m_rtBgImage, NKCCamera.GetCamera(), new Vector2(m_fBgScrollRange, 0f), NKCCamera.GetCamera().transform.position.z, NKCCamera.FitMode.FitAuto, NKCCamera.ScaleMode.RectSize);
		m_rtCamBoundRect = NKCCamera.GetCameraBoundRect(m_rtBgImage, NKCCamera.GetCamera().transform.position.z);
	}

	public void Release()
	{
		m_background.SetParent(base.transform);
		NKCUIOfficeMinimapFacility nKCUIMinimapFacility = m_NKCUIMinimapFacility;
		nKCUIMinimapFacility.m_dOnScrollCamMove = (NKCUIOfficeMinimapFacility.OnScroll)Delegate.Remove(nKCUIMinimapFacility.m_dOnScrollCamMove, new NKCUIOfficeMinimapFacility.OnScroll(MoveCameraByScroll));
		NKCUIOfficeMinimapRoom nKCUIMinimapRoom = m_NKCUIMinimapRoom;
		nKCUIMinimapRoom.m_dOnScrollCamMove = (NKCUIOfficeMinimapRoom.OnScroll)Delegate.Remove(nKCUIMinimapRoom.m_dOnScrollCamMove, new NKCUIOfficeMinimapRoom.OnScroll(MoveCameraByScroll));
	}

	private void MoveCameraByScroll(Vector2 scrollNormalizedPosition)
	{
		float x = Mathf.Lerp(m_rtCamBoundRect.xMin, m_rtCamBoundRect.xMax, scrollNormalizedPosition.x);
		Vector3 position = NKCCamera.GetCamera().transform.position;
		position.x = x;
		NKCCamera.SetPos(position, bTrackingStop: true, bForce: true);
	}
}
