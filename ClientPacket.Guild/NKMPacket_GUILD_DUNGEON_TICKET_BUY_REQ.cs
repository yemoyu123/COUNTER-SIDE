using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DUNGEON_TICKET_BUY_REQ)]
public sealed class NKMPacket_GUILD_DUNGEON_TICKET_BUY_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
