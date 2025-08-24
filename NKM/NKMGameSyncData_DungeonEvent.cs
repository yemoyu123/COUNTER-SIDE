using Cs.Protocol;

namespace NKM;

public class NKMGameSyncData_DungeonEvent : ISerializable
{
	public NKM_EVENT_ACTION_TYPE m_eEventActionType;

	public int m_EventID;

	public int m_iEventActionValue;

	public string m_strEventActionValue;

	public bool m_bPause;

	public NKM_TEAM_TYPE m_eTeam;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref m_eEventActionType);
		stream.PutOrGet(ref m_EventID);
		stream.PutOrGet(ref m_iEventActionValue);
		stream.PutOrGet(ref m_strEventActionValue);
		stream.PutOrGet(ref m_bPause);
		stream.PutOrGetEnum(ref m_eTeam);
	}
}
