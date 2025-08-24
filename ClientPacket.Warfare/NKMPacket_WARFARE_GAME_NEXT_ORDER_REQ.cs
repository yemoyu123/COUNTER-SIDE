using Cs.Protocol;
using Protocol;

namespace ClientPacket.Warfare;

[PacketId(ClientPacketId.kNKMPacket_WARFARE_GAME_NEXT_ORDER_REQ)]
public sealed class NKMPacket_WARFARE_GAME_NEXT_ORDER_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
