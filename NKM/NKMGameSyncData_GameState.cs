using Cs.Protocol;

namespace NKM;

public class NKMGameSyncData_GameState : ISerializable
{
	public NKM_GAME_STATE m_NKM_GAME_STATE;

	public NKM_TEAM_TYPE m_WinTeam;

	public int m_WaveID;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref m_NKM_GAME_STATE);
		stream.PutOrGetEnum(ref m_WinTeam);
		stream.PutOrGet(ref m_WaveID);
	}
}
