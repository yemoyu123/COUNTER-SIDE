using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_SURRENDER_NOT)]
public sealed class NKMPacket_GAME_SURRENDER_NOT : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
