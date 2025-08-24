using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_GIVEUP_REQ)]
public sealed class NKMPacket_GAME_GIVEUP_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
