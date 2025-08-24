using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Defence;

[PacketId(ClientPacketId.kNKMPacket_DEFENCE_SCORE_REWARD_ALL_ACK)]
public sealed class NKMPacket_DEFENCE_SCORE_REWARD_ALL_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<int> scoreRewardIds = new List<int>();

	public NKMRewardData rewardData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref scoreRewardIds);
		stream.PutOrGet(ref rewardData);
	}
}
