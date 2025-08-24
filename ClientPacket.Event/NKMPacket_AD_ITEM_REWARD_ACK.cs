using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_AD_ITEM_REWARD_ACK)]
public sealed class NKMPacket_AD_ITEM_REWARD_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMADItemRewardInfo itemRewardInfo = new NKMADItemRewardInfo();

	public NKMRewardData rewardData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref itemRewardInfo);
		stream.PutOrGet(ref rewardData);
	}
}
