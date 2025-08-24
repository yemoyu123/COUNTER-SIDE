using Cs.Protocol;

namespace NKM;

public class NKMGameSyncData_TC_Combo : ISerializable
{
	public NKM_TEAM_TYPE m_NKM_TEAM_TYPE;

	public int m_TCID;

	public int m_Combo;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref m_NKM_TEAM_TYPE);
		stream.PutOrGet(ref m_TCID);
		stream.PutOrGet(ref m_Combo);
	}
}
