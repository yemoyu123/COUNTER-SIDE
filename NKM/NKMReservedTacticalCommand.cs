namespace NKM;

public class NKMReservedTacticalCommand
{
	private float m_fReservedTime;

	private NKMTacticalCommandTemplet m_cNKMTacticalCommandTemplet;

	private NKMTacticalCommandData m_cNKMTacticalCommandData;

	private NKM_TEAM_TYPE m_NKM_TEAM_TYPE;

	public NKMTacticalCommandTemplet GetNKMTacticalCommandTemplet()
	{
		return m_cNKMTacticalCommandTemplet;
	}

	public NKMTacticalCommandData GetNKMTacticalCommandData()
	{
		return m_cNKMTacticalCommandData;
	}

	public NKM_TEAM_TYPE Get_NKM_TEAM_TYPE()
	{
		return m_NKM_TEAM_TYPE;
	}

	public void Invalidate()
	{
		m_fReservedTime = 0f;
		m_cNKMTacticalCommandTemplet = null;
		m_cNKMTacticalCommandData = null;
		m_NKM_TEAM_TYPE = NKM_TEAM_TYPE.NTT_INVALID;
	}

	public void SetNewData(float fReservedTime, NKMTacticalCommandTemplet cNKMTacticalCommandTemplet, NKMTacticalCommandData cNKMTacticalCommandData, NKM_TEAM_TYPE eNKM_TEAM_TYPE)
	{
		if (!(m_fReservedTime > 0f))
		{
			m_fReservedTime = fReservedTime;
			m_cNKMTacticalCommandTemplet = cNKMTacticalCommandTemplet;
			m_cNKMTacticalCommandData = cNKMTacticalCommandData;
			m_NKM_TEAM_TYPE = eNKM_TEAM_TYPE;
		}
	}

	public void Update(float fDeltaTime)
	{
		m_fReservedTime -= fDeltaTime;
		if (m_fReservedTime <= 0f)
		{
			m_fReservedTime = 0f;
		}
	}

	public bool CheckApplyTiming()
	{
		if (m_cNKMTacticalCommandData == null || m_cNKMTacticalCommandTemplet == null)
		{
			return false;
		}
		if (m_NKM_TEAM_TYPE == NKM_TEAM_TYPE.NTT_INVALID)
		{
			return false;
		}
		if (m_fReservedTime <= 0f)
		{
			return true;
		}
		return false;
	}
}
