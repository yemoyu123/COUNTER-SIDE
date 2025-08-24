using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_TOURNAMENT_REWARD_INFO_REQ)]
public sealed class NKMPacket_TOURNAMENT_REWARD_INFO_REQ : ISerializable
{
	public int templetId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref templetId);
	}
}
