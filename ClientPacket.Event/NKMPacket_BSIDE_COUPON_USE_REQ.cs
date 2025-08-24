using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_BSIDE_COUPON_USE_REQ)]
public sealed class NKMPacket_BSIDE_COUPON_USE_REQ : ISerializable
{
	public string couponCode;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref couponCode);
	}
}
