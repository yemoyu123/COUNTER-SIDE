using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_EVENT_PVP_REWARD_ACK)]
public sealed class NKMPacket_EVENT_PVP_REWARD_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMRewardData> rewardDatas = new List<NKMRewardData>();

	public List<EventPvpReward> eventPvpRewardInfos = new List<EventPvpReward>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref rewardDatas);
		stream.PutOrGet(ref eventPvpRewardInfos);
	}
}
