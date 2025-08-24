using Cs.Protocol;

namespace ClientPacket.Event;

public sealed class NKMEventPassMissionInfo : ISerializable
{
	public int missionId;

	public int slotIndex;

	public int retryCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref missionId);
		stream.PutOrGet(ref slotIndex);
		stream.PutOrGet(ref retryCount);
	}
}
