using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Defence;

[PacketId(ClientPacketId.kNKMPacket_DEFENCE_RANK_REWARD_ACK)]
public sealed class NKMPacket_DEFENCE_RANK_REWARD_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMRewardData rewardData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref rewardData);
	}
}
