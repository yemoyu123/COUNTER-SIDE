using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_MISSION_COMPLETE_ALL_ACK)]
public sealed class NKMPacket_MISSION_COMPLETE_ALL_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<int> missionIDList = new List<int>();

	public NKMRewardData rewardDate;

	public NKMAdditionalReward additionalReward = new NKMAdditionalReward();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref missionIDList);
		stream.PutOrGet(ref rewardDate);
		stream.PutOrGet(ref additionalReward);
	}
}
