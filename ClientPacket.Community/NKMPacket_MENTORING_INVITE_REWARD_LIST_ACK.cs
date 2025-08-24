using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_MENTORING_INVITE_REWARD_LIST_ACK)]
public sealed class NKMPacket_MENTORING_INVITE_REWARD_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public HashSet<int> rewardHistories = new HashSet<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref rewardHistories);
	}
}
