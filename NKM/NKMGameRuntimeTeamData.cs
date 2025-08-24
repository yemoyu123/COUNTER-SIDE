using Cs.Protocol;

namespace NKM;

public class NKMGameRuntimeTeamData : ISerializable
{
	public long m_UserUID;

	public bool m_bAutoRespawn = true;

	public bool m_bAIDisable;

	public bool m_bGodMode;

	public float m_fRespawnCost = 3f;

	public float m_fRespawnCostAssist;

	public float m_fUsedRespawnCost;

	public int m_respawn_count;

	public NKM_GAME_AUTO_SKILL_TYPE m_NKM_GAME_AUTO_SKILL_TYPE = NKM_GAME_AUTO_SKILL_TYPE.NGST_OFF_HYPER;

	public virtual void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_UserUID);
		stream.PutOrGet(ref m_bAutoRespawn);
		stream.PutOrGet(ref m_bAIDisable);
		stream.PutOrGet(ref m_bGodMode);
		stream.PutOrGet(ref m_fRespawnCost);
		stream.PutOrGet(ref m_fRespawnCostAssist);
		stream.PutOrGet(ref m_fUsedRespawnCost);
		stream.PutOrGet(ref m_respawn_count);
		stream.PutOrGetEnum(ref m_NKM_GAME_AUTO_SKILL_TYPE);
	}

	public void DeepCopyFromSource(NKMGameRuntimeTeamData source)
	{
		m_bAutoRespawn = source.m_bAutoRespawn;
		m_bAIDisable = source.m_bAIDisable;
		m_bGodMode = source.m_bGodMode;
		m_fRespawnCost = source.m_fRespawnCost;
		m_fRespawnCostAssist = source.m_fRespawnCostAssist;
		m_fUsedRespawnCost = source.m_fUsedRespawnCost;
		m_respawn_count = source.m_respawn_count;
		m_NKM_GAME_AUTO_SKILL_TYPE = source.m_NKM_GAME_AUTO_SKILL_TYPE;
	}
}
