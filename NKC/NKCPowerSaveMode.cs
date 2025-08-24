namespace NKC;

public class NKCPowerSaveMode
{
	public delegate void DoAtEvent();

	private float TIME_UNTIL_MAX_POWER_SAVE_MODE = 5f;

	private bool m_bEnable;

	private bool m_bJukeBoxMode;

	private float m_fLastKeyInputTime;

	private bool m_bPowerSaveModeCallback;

	private bool m_bMaxPowerSaveModeCallback;

	private bool m_bMaxPowerSaveModeNextFrameCallback;

	private bool m_bTurnOffPowerSaveModeCallback = true;

	private DoAtEvent m_dgPowerSaveMode;

	private DoAtEvent m_dgMaxPowerSaveMode;

	private DoAtEvent m_dgMaxPowerSaveModeNextFrame;

	private DoAtEvent m_dgTurnOffPowerSaveMode;

	public bool IsJukeBoxMode => m_bJukeBoxMode;

	public void SetEnable(bool bEnable)
	{
		m_bEnable = bEnable;
	}

	public bool GetEnable()
	{
		return m_bEnable;
	}

	public void SetJukeBoxMode(bool bEnable)
	{
		m_bJukeBoxMode = bEnable;
	}

	public void SetLastKeyInputTime(float fInputTime)
	{
		m_fLastKeyInputTime = fInputTime;
	}

	public void SetTurnOnEvent(DoAtEvent dgDoAtEvent)
	{
		m_dgPowerSaveMode = dgDoAtEvent;
	}

	public void SetTurnOffEvent(DoAtEvent dgDoAtEvent)
	{
		m_dgTurnOffPowerSaveMode = dgDoAtEvent;
	}

	public void SetMaxModeEvent(DoAtEvent dgDoAtEvent)
	{
		m_dgMaxPowerSaveMode = dgDoAtEvent;
	}

	public void SetMaxModeNextFrameEvent(DoAtEvent dgDoAtEvent)
	{
		m_dgMaxPowerSaveModeNextFrame = dgDoAtEvent;
	}

	public void Update(float fTime)
	{
		if (GetEnable())
		{
			if (m_fLastKeyInputTime + TIME_UNTIL_MAX_POWER_SAVE_MODE < fTime)
			{
				m_bPowerSaveModeCallback = false;
				m_bTurnOffPowerSaveModeCallback = false;
				if (!m_bMaxPowerSaveModeCallback)
				{
					m_bMaxPowerSaveModeCallback = true;
					if (m_dgMaxPowerSaveMode != null)
					{
						m_dgMaxPowerSaveMode();
					}
				}
				else if (!m_bMaxPowerSaveModeNextFrameCallback)
				{
					m_bMaxPowerSaveModeNextFrameCallback = true;
					if (m_dgMaxPowerSaveModeNextFrame != null)
					{
						m_dgMaxPowerSaveModeNextFrame();
					}
				}
				return;
			}
			m_bMaxPowerSaveModeCallback = false;
			m_bMaxPowerSaveModeNextFrameCallback = false;
			m_bTurnOffPowerSaveModeCallback = false;
			if (!m_bPowerSaveModeCallback)
			{
				m_bPowerSaveModeCallback = true;
				if (m_dgPowerSaveMode != null)
				{
					m_dgPowerSaveMode();
				}
			}
			return;
		}
		m_bMaxPowerSaveModeNextFrameCallback = false;
		m_bPowerSaveModeCallback = false;
		if (!m_bTurnOffPowerSaveModeCallback)
		{
			m_bTurnOffPowerSaveModeCallback = true;
			if (m_dgTurnOffPowerSaveMode != null)
			{
				m_dgTurnOffPowerSaveMode();
			}
		}
	}
}
