using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_WECHAT_COUPON_CHECK_REQ)]
public sealed class NKMPacket_WECHAT_COUPON_CHECK_REQ : ISerializable
{
	public int templetId;

	public int zlongServerId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref templetId);
		stream.PutOrGet(ref zlongServerId);
	}
}
