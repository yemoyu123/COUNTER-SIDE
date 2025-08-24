using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_POST_RECEIVE_ACK)]
public sealed class NKMPacket_POST_RECEIVE_ACK : ISerializable
{
	public long postIndex;

	public NKMRewardData rewardDate;

	public int postCount;

	public NKM_ERROR_CODE errorCode;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref postIndex);
		stream.PutOrGet(ref rewardDate);
		stream.PutOrGet(ref postCount);
		stream.PutOrGetEnum(ref errorCode);
	}
}
