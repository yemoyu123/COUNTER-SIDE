using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_ZLONG_CBT_PAYMENT_REWARD_REQ)]
public sealed class NKMPacket_ZLONG_CBT_PAYMENT_REWARD_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
