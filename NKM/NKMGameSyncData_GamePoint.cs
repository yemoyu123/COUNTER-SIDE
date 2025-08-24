using Cs.Protocol;

namespace NKM;

public class NKMGameSyncData_GamePoint : ISerializable
{
	public int m_fGamePoint;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_fGamePoint);
	}
}
