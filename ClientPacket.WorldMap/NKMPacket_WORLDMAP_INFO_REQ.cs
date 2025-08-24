using Cs.Protocol;
using Protocol;

namespace ClientPacket.WorldMap;

[PacketId(ClientPacketId.kNKMPacket_WORLDMAP_INFO_REQ)]
public sealed class NKMPacket_WORLDMAP_INFO_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
