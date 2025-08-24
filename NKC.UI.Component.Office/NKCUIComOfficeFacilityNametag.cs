using UnityEngine;

namespace NKC.UI.Component.Office;

public class NKCUIComOfficeFacilityNametag : MonoBehaviour
{
	public GameObject m_objOpen;

	public GameObject m_objLock;

	public GameObject m_objReddot;

	private bool m_bLock;

	public void SetLock(bool value)
	{
		m_bLock = value;
		NKCUtil.SetGameobjectActive(m_objOpen, !m_bLock);
		NKCUtil.SetGameobjectActive(m_objLock, m_bLock);
	}

	public void SetAlarm(NKCAlarmManager.ALARM_TYPE type)
	{
		NKCUtil.SetGameobjectActive(m_objReddot, NKCAlarmManager.CheckNotify(NKCScenManager.CurrentUserData(), type));
	}

	public void SetAlarm(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objReddot, value);
	}
}
