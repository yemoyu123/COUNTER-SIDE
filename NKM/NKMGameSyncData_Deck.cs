using Cs.Protocol;

namespace NKM;

public class NKMGameSyncData_Deck : ISerializable
{
	public NKM_TEAM_TYPE m_NKM_TEAM_TYPE;

	public sbyte m_UnitDeckIndex = -1;

	public long m_UnitDeckUID = -1L;

	public long m_DeckUsedAddUnitUID = -1L;

	public sbyte m_DeckUsedRemoveIndex = -1;

	public long m_DeckTombAddUnitUID = -1L;

	public sbyte m_AutoRespawnIndex = -1;

	public long m_NextDeckUnitUID = -1L;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref m_NKM_TEAM_TYPE);
		stream.PutOrGet(ref m_UnitDeckIndex);
		stream.PutOrGet(ref m_UnitDeckUID);
		stream.PutOrGet(ref m_DeckUsedAddUnitUID);
		stream.PutOrGet(ref m_DeckUsedRemoveIndex);
		stream.PutOrGet(ref m_DeckTombAddUnitUID);
		stream.PutOrGet(ref m_AutoRespawnIndex);
		stream.PutOrGet(ref m_NextDeckUnitUID);
	}
}
