namespace NKC.UI.HUD;

public class NKCGameHudAlarmCommonControl : IGameHudAlert
{
	private NKCGameHudAlertCommon m_objAlarm;

	private string m_title;

	private string m_desc;

	private string m_iconName;

	public NKCGameHudAlarmCommonControl(NKCGameHudAlertCommon objAlarm, string _title, string _desc, string _iconName = null)
	{
		m_objAlarm = objAlarm;
		m_title = _title;
		m_desc = _desc;
		m_iconName = _iconName;
	}

	public void OnStart()
	{
		if (!(m_objAlarm == null))
		{
			m_objAlarm.SetData(m_title, m_desc, m_iconName);
			NKCUtil.SetGameobjectActive(m_objAlarm, bValue: true);
		}
	}

	public void OnUpdate()
	{
	}

	public bool IsFinished()
	{
		if (m_objAlarm == null)
		{
			return true;
		}
		return m_objAlarm.IsFinished();
	}

	public void OnCleanup()
	{
		NKCUtil.SetGameobjectActive(m_objAlarm, bValue: false);
	}
}
