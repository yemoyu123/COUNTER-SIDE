namespace NKM;

public class SkillStatData
{
	public NKM_STAT_TYPE m_NKM_STAT_TYPE;

	public float m_fStatValue;

	public SkillStatData(NKM_STAT_TYPE eType, float value)
	{
		m_NKM_STAT_TYPE = eType;
		m_fStatValue = value;
	}
}
