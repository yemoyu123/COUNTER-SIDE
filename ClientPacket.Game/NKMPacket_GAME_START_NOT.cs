using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_START_NOT)]
public sealed class NKMPacket_GAME_START_NOT : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
