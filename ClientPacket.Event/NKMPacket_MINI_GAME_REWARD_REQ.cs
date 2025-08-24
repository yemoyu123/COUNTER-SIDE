using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_MINI_GAME_REWARD_REQ)]
public sealed class NKMPacket_MINI_GAME_REWARD_REQ : ISerializable
{
	public int templetId;

	public int rewardId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref templetId);
		stream.PutOrGet(ref rewardId);
	}
}
