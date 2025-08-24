using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_WECHAT_COUPON_REWARD_REQ)]
public sealed class NKMPacket_WECHAT_COUPON_REWARD_REQ : ISerializable
{
	public int templetId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref templetId);
	}
}
