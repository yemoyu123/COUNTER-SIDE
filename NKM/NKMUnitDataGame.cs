namespace NKM;

public class NKMUnitDataGame
{
	public long m_UnitUID;

	public short m_MasterGameUnitUID;

	public short m_GameUnitUID;

	public bool m_bSummonUnit;

	public bool m_bChangeUnit;

	public NKM_TEAM_TYPE m_NKM_TEAM_TYPE_ORG;

	public NKM_TEAM_TYPE m_NKM_TEAM_TYPE;

	public float m_RespawnPosX;

	public float m_RespawnPosZ;

	public float m_RespawnJumpYPos;

	public float m_fTargetNearRange;

	public NKMUnitDataGame()
	{
		Init();
	}

	public void Init()
	{
		m_UnitUID = 0L;
		m_MasterGameUnitUID = 0;
		m_GameUnitUID = 0;
		m_bChangeUnit = false;
		m_NKM_TEAM_TYPE = NKM_TEAM_TYPE.NTT_A1;
		m_RespawnPosX = 0f;
		m_RespawnPosZ = 0f;
		m_RespawnJumpYPos = 0f;
		m_fTargetNearRange = 0f;
	}

	public void DeepCopyFromSource(NKMUnitDataGame source)
	{
		m_UnitUID = source.m_UnitUID;
		m_MasterGameUnitUID = source.m_MasterGameUnitUID;
		m_GameUnitUID = source.m_GameUnitUID;
		m_bChangeUnit = source.m_bChangeUnit;
		m_NKM_TEAM_TYPE_ORG = source.m_NKM_TEAM_TYPE_ORG;
		m_NKM_TEAM_TYPE = source.m_NKM_TEAM_TYPE;
		m_RespawnPosX = source.m_RespawnPosX;
		m_RespawnPosZ = source.m_RespawnPosZ;
		m_RespawnJumpYPos = source.m_RespawnJumpYPos;
		m_fTargetNearRange = source.m_fTargetNearRange;
	}
}
