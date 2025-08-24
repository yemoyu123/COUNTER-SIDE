using Cs.Protocol;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_ZLONG_USE_COUPON_REQ2)]
public sealed class NKMPacket_ZLONG_USE_COUPON_REQ2 : ISerializable
{
	public string couponCode;

	public int zlongServerId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref couponCode);
		stream.PutOrGet(ref zlongServerId);
	}
}
