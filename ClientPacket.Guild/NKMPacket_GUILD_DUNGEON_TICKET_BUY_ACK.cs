using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DUNGEON_TICKET_BUY_ACK)]
public sealed class NKMPacket_GUILD_DUNGEON_TICKET_BUY_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int currentTicketBuyCount;

	public NKMItemMiscData costItemData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref currentTicketBuyCount);
		stream.PutOrGet(ref costItemData);
	}
}
