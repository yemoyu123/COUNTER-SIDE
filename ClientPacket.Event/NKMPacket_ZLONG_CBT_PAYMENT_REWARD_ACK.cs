using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_ZLONG_CBT_PAYMENT_REWARD_ACK)]
public sealed class NKMPacket_ZLONG_CBT_PAYMENT_REWARD_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMRewardData rewardData;

	public ZlongCbtPaymentData paymentData = new ZlongCbtPaymentData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref paymentData);
	}
}
