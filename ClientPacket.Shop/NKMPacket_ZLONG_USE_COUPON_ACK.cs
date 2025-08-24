using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_ZLONG_USE_COUPON_ACK)]
public sealed class NKMPacket_ZLONG_USE_COUPON_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int zlongInfoCode;

	public NKMRewardData rewardData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref zlongInfoCode);
		stream.PutOrGet(ref rewardData);
	}
}
