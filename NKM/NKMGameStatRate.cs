namespace NKM;

public class NKMGameStatRate
{
	public float m_MainStatFactorRate = 1f;

	public float m_SubStatValueRate = 1f;

	public float m_EquipStatRate = 1f;

	public float m_HPRate = 1f;

	public float m_ATKRate = 1f;

	public float m_DEFRate = 1f;

	public float m_CRITRate = 1f;

	public float m_HITRate = 1f;

	public float m_EvadeRate = 1f;

	public float GetStatValueRate(NKM_STAT_TYPE eStat)
	{
		return eStat switch
		{
			NKM_STAT_TYPE.NST_HP => m_HPRate, 
			NKM_STAT_TYPE.NST_ATK => m_ATKRate, 
			NKM_STAT_TYPE.NST_DEF => m_DEFRate, 
			NKM_STAT_TYPE.NST_CRITICAL => m_CRITRate, 
			NKM_STAT_TYPE.NST_HIT => m_HITRate, 
			NKM_STAT_TYPE.NST_EVADE => m_EvadeRate, 
			_ => m_SubStatValueRate, 
		};
	}

	public float GetStatFactorRate(NKM_STAT_TYPE eStat)
	{
		if (NKMUnitStatManager.IsMainStat(eStat))
		{
			return m_MainStatFactorRate;
		}
		return 1f;
	}

	public float GetEquipStatRate()
	{
		return m_EquipStatRate;
	}
}
