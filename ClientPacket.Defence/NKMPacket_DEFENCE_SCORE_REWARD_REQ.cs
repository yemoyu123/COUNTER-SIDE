using Cs.Protocol;
using Protocol;

namespace ClientPacket.Defence;

[PacketId(ClientPacketId.kNKMPacket_DEFENCE_SCORE_REWARD_REQ)]
public sealed class NKMPacket_DEFENCE_SCORE_REWARD_REQ : ISerializable
{
	public int scoreRewardId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref scoreRewardId);
	}
}
