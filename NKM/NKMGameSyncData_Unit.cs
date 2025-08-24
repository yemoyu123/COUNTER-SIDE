using Cs.Protocol;

namespace NKM;

public class NKMGameSyncData_Unit : ISerializable
{
	public NKMUnitSyncData m_NKMGameUnitSyncData;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_NKMGameUnitSyncData);
	}
}
