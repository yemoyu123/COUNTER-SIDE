using Cs.Protocol;
using Protocol;

namespace ClientPacket.Defence;

[PacketId(ClientPacketId.kNKMPacket_DEFENCE_GAME_GIVE_UP_REQ)]
public sealed class NKMPacket_DEFENCE_GAME_GIVE_UP_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
