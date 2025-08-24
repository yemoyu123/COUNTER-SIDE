using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_WECHAT_COUPON_CHECK_ACK)]
public sealed class NKMPacket_WECHAT_COUPON_CHECK_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int zlongInfoCode;

	public WechatCouponData data = new WechatCouponData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref zlongInfoCode);
		stream.PutOrGet(ref data);
	}
}
