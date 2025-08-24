using NKC.UI;
using UnityEngine;

namespace NKC;

public class NKCHealthyGame
{
	private bool m_bEnable;

	private float m_fLastTime = float.MinValue;

	private int m_HourCount;

	public void Start()
	{
		m_bEnable = true;
		m_fLastTime = Time.time;
	}

	public void Update()
	{
		if (m_bEnable && m_fLastTime + 3600f <= Time.time)
		{
			m_fLastTime = Time.time;
			m_HourCount++;
			NKCUIPopupMessageServer.Instance.Open(NKCUIPopupMessageServer.eMessageStyle.Slide, NKCStringTable.GetString("SI_DP_PC_HEALTH_WARNING"));
			NKCUIPopupMessageServer.Instance.Open(NKCUIPopupMessageServer.eMessageStyle.Slide, NKCUtilString.GetPlayTimeWarning(m_HourCount));
		}
	}
}
