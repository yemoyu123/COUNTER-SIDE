using Cs.Protocol;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_FIRST_CASH_PURCHASE_NOT)]
public sealed class NKMPacket_FIRST_CASH_PURCHASE_NOT : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
