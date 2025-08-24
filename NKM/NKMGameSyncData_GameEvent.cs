using Cs.Protocol;

namespace NKM;

public class NKMGameSyncData_GameEvent : ISerializable
{
	public NKM_GAME_EVENT_TYPE m_NKM_GAME_EVENT_TYPE;

	public NKM_TEAM_TYPE m_NKM_TEAM_TYPE;

	public int m_EventID;

	public float m_fValue;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref m_NKM_GAME_EVENT_TYPE);
		stream.PutOrGetEnum(ref m_NKM_TEAM_TYPE);
		stream.PutOrGet(ref m_EventID);
		stream.PutOrGet(ref m_fValue);
	}
}
