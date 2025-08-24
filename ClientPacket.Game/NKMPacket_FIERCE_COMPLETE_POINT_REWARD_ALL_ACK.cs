using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_FIERCE_COMPLETE_POINT_REWARD_ALL_ACK)]
public sealed class NKMPacket_FIERCE_COMPLETE_POINT_REWARD_ALL_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<int> pointRewardIds = new List<int>();

	public NKMRewardData rewardData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref pointRewardIds);
		stream.PutOrGet(ref rewardData);
	}
}
