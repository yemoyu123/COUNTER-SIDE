using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DUNGEON_BOSS_ORDER_ACK)]
public sealed class NKMPacket_GUILD_DUNGEON_BOSS_ORDER_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public short orderIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref orderIndex);
	}
}
