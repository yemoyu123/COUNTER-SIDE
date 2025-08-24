using Cs.Protocol;
using Protocol;

namespace ClientPacket.WorldMap;

[PacketId(ClientPacketId.kNKMPacket_WORLDMAP_EVENT_CANCEL_REQ)]
public sealed class NKMPacket_WORLDMAP_EVENT_CANCEL_REQ : ISerializable
{
	public int cityID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref cityID);
	}
}
