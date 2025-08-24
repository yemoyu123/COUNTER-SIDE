using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_MINI_GAME_REWARD_ALL_REQ)]
public sealed class NKMPacket_MINI_GAME_REWARD_ALL_REQ : ISerializable
{
	public int templetId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref templetId);
	}
}
