using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_BINGO_REWARD_ACK)]
public sealed class NKMPacket_EVENT_BINGO_REWARD_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int eventId;

	public int rewardIndex;

	public NKMRewardData rewardData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref eventId);
		stream.PutOrGet(ref rewardIndex);
		stream.PutOrGet(ref rewardData);
	}
}
