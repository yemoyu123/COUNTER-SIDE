using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_CHECK_DIE_UNIT_REQ)]
public sealed class NKMPacket_GAME_CHECK_DIE_UNIT_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
