using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_RESTART_REQ)]
public sealed class NKMPacket_GAME_RESTART_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
