using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_MINI_GAME_REWARD_ALL_ACK)]
public sealed class NKMPacket_MINI_GAME_REWARD_ALL_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<int> rewardIds = new List<int>();

	public NKMRewardData rewardData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref rewardIds);
		stream.PutOrGet(ref rewardData);
	}
}
