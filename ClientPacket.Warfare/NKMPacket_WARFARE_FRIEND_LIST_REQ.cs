using Cs.Protocol;
using Protocol;

namespace ClientPacket.Warfare;

[PacketId(ClientPacketId.kNKMPacket_WARFARE_FRIEND_LIST_REQ)]
public sealed class NKMPacket_WARFARE_FRIEND_LIST_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
