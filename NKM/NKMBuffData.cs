namespace NKM;

public class NKMBuffData : NKMObjectPoolData
{
	public NKMBuffSyncData m_BuffSyncData = new NKMBuffSyncData();

	public float m_fLifeTime = -1f;

	public float m_fBarrierHP = -1f;

	public float m_fBuffPosX;

	public bool m_StateEndRemove;

	public bool m_StateEndCheck;

	public NKMBuffTemplet m_NKMBuffTemplet;

	public NKMDamageInst m_DamageInstBuff = new NKMDamageInst();

	public NKMBuffData()
	{
		m_NKM_OBJECT_POOL_TYPE = NKM_OBJECT_POOL_TYPE.NOPT_NKMBuffData;
		Init();
	}

	public void Init()
	{
		m_BuffSyncData.Init();
		m_fLifeTime = -1f;
		m_fBarrierHP = -1f;
		m_fBuffPosX = 0f;
		m_StateEndRemove = false;
		m_StateEndCheck = false;
		m_NKMBuffTemplet = null;
		m_DamageInstBuff.Init();
	}

	public override void Close()
	{
		Init();
	}

	public float GetLifeTimeMax()
	{
		if (m_NKMBuffTemplet == null)
		{
			return -1f;
		}
		return m_NKMBuffTemplet.GetLifeTimeMax(m_BuffSyncData.m_BuffTimeLevel);
	}
}
