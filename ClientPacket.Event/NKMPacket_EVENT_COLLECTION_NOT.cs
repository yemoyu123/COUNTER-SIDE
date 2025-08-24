using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_COLLECTION_NOT)]
public sealed class NKMPacket_EVENT_COLLECTION_NOT : ISerializable
{
	public NKMEventCollectionInfo eventCollectionInfo = new NKMEventCollectionInfo();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref eventCollectionInfo);
	}
}
