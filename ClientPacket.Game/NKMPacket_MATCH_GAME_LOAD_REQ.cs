using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_MATCH_GAME_LOAD_REQ)]
public sealed class NKMPacket_MATCH_GAME_LOAD_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
