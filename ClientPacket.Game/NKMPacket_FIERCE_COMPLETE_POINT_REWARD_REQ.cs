using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_FIERCE_COMPLETE_POINT_REWARD_REQ)]
public sealed class NKMPacket_FIERCE_COMPLETE_POINT_REWARD_REQ : ISerializable
{
	public int fiercePointRewardId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref fiercePointRewardId);
	}
}
