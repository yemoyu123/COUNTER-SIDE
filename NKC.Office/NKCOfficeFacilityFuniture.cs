using NKC.UI;
using NKC.UI.Component.Office;
using UnityEngine;

namespace NKC.Office;

public class NKCOfficeFacilityFuniture : NKCOfficeFuniture
{
	[Header("시설 가구 관련")]
	public NKCUIComOfficeFacilityNametag m_comNameTag;

	public RectTransform m_rtTutorialHighlightArea;

	private bool m_bLock;

	public void SetLock(bool value)
	{
		m_bLock = value;
		if (m_comNameTag != null)
		{
			m_comNameTag.SetLock(value);
		}
	}

	public void SetAlarm(NKCAlarmManager.ALARM_TYPE type)
	{
		if (m_comNameTag != null)
		{
			m_comNameTag.SetAlarm(type);
		}
	}

	public void SetAlarm(bool value)
	{
		if (m_comNameTag != null)
		{
			m_comNameTag.SetAlarm(value);
		}
	}

	public override RectTransform MakeHighlightRect()
	{
		if (m_rtTutorialHighlightArea != null)
		{
			return m_rtTutorialHighlightArea;
		}
		if (m_comNameTag != null && (bool)m_comNameTag.m_objOpen)
		{
			return m_comNameTag.m_objOpen.GetComponent<RectTransform>();
		}
		return base.MakeHighlightRect();
	}
}
