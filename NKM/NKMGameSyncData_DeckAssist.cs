using Cs.Protocol;

namespace NKM;

public class NKMGameSyncData_DeckAssist : ISerializable
{
	public NKM_TEAM_TYPE m_NKM_TEAM_TYPE;

	public sbyte m_AutoRespawnIndexAssist = -1;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref m_NKM_TEAM_TYPE);
		stream.PutOrGet(ref m_AutoRespawnIndexAssist);
	}
}
