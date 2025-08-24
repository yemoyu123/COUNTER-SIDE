namespace NKM;

public class NKMGameUnitRespawnData : NKMObjectPoolData
{
	public long m_UnitUID;

	public float m_fRespawnCoolTime;

	public float m_fRespawnPosX;

	public NKM_TEAM_TYPE m_eNKM_TEAM_TYPE;

	public float m_fRollbackTime;

	public NKMGameUnitRespawnData()
	{
		m_NKM_OBJECT_POOL_TYPE = NKM_OBJECT_POOL_TYPE.NOPT_NKMGameUnitRespawnData;
	}

	public override void Close()
	{
		Init();
	}

	public void Init()
	{
		m_UnitUID = 0L;
		m_fRespawnCoolTime = 0f;
		m_fRespawnPosX = -1f;
		m_eNKM_TEAM_TYPE = NKM_TEAM_TYPE.NTT_INVALID;
		m_fRollbackTime = 0f;
	}

	public void DeepCopyFromSource(NKMGameUnitRespawnData source)
	{
		m_UnitUID = source.m_UnitUID;
		m_fRespawnCoolTime = source.m_fRespawnCoolTime;
		m_fRespawnPosX = source.m_fRespawnPosX;
		m_eNKM_TEAM_TYPE = source.m_eNKM_TEAM_TYPE;
		m_fRollbackTime = source.m_fRollbackTime;
	}
}
