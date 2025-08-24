namespace NKM;

public class NKMDamageTempletBase
{
	public string m_DamageTempletName = "";

	public int m_DamageTempletIndex;

	public NKM_STAT_TYPE m_AtkFactorStat = NKM_STAT_TYPE.NST_ATK;

	public float m_fAtkFactor;

	public float m_fAtkMaxHPRateFactor;

	public float m_fAtkHPRateFactor;

	public float m_fAtkFactorPVP;

	public float m_fAtkMaxHPRateFactorPVP;

	public float m_fAtkHPRateFactorPVP;

	private bool m_bZeroDamageCache;

	private bool m_bZeroDamagePvPCache;

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		cNKMLua.GetData("m_DamageTempletName", ref m_DamageTempletName);
		cNKMLua.GetData("m_AtkFactorStat", ref m_AtkFactorStat);
		cNKMLua.GetData("m_fAtkFactor", ref m_fAtkFactor);
		cNKMLua.GetData("m_fAtkMaxHPRateFactor", ref m_fAtkMaxHPRateFactor);
		cNKMLua.GetData("m_fAtkHPRateFactor", ref m_fAtkHPRateFactor);
		cNKMLua.GetData("m_fAtkFactorPVP", ref m_fAtkFactorPVP);
		cNKMLua.GetData("m_fAtkMaxHPRateFactorPVP", ref m_fAtkMaxHPRateFactorPVP);
		cNKMLua.GetData("m_fAtkHPRateFactorPVP", ref m_fAtkHPRateFactorPVP);
		MakeZeroDamageCache();
		return true;
	}

	public void DeepCopyFromSource(NKMDamageTempletBase source)
	{
		m_DamageTempletName = source.m_DamageTempletName;
		m_DamageTempletIndex = source.m_DamageTempletIndex;
		m_AtkFactorStat = source.m_AtkFactorStat;
		m_fAtkFactor = source.m_fAtkFactor;
		m_fAtkMaxHPRateFactor = source.m_fAtkMaxHPRateFactor;
		m_fAtkHPRateFactor = source.m_fAtkHPRateFactor;
		m_fAtkFactorPVP = source.m_fAtkFactorPVP;
		m_fAtkMaxHPRateFactorPVP = source.m_fAtkMaxHPRateFactorPVP;
		m_fAtkHPRateFactorPVP = source.m_fAtkHPRateFactorPVP;
		MakeZeroDamageCache();
	}

	private void MakeZeroDamageCache()
	{
		m_bZeroDamageCache = m_fAtkFactor == 0f && m_fAtkMaxHPRateFactor == 0f && m_fAtkHPRateFactor == 0f;
		m_bZeroDamagePvPCache = m_fAtkFactor == 0f && m_fAtkMaxHPRateFactor == 0f && m_fAtkHPRateFactor == 0f && m_fAtkFactorPVP == 0f && m_fAtkMaxHPRateFactorPVP == 0f && m_fAtkHPRateFactorPVP == 0f;
	}

	public bool IsZeroDamage()
	{
		return m_bZeroDamagePvPCache;
	}

	public bool IsZeroDamage(bool isPvP)
	{
		if (isPvP)
		{
			return m_bZeroDamagePvPCache;
		}
		return m_bZeroDamageCache;
	}
}
