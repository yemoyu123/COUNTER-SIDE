using Cs.Protocol;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_ZLONG_USE_COUPON_REQ)]
public sealed class NKMPacket_ZLONG_USE_COUPON_REQ : ISerializable
{
	public string couponCode;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref couponCode);
	}
}
