using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_EMOTICON_DATA_REQ)]
public sealed class NKMPacket_EMOTICON_DATA_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
