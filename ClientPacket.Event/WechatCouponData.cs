using Cs.Protocol;

namespace ClientPacket.Event;

public sealed class WechatCouponData : ISerializable
{
	public int templetId;

	public WechatCouponState state;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref templetId);
		stream.PutOrGetEnum(ref state);
	}
}
