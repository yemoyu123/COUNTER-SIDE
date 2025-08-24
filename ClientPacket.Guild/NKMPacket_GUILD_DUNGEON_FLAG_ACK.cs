using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DUNGEON_FLAG_ACK)]
public sealed class NKMPacket_GUILD_DUNGEON_FLAG_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int arenaIndex;

	public int flagIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref arenaIndex);
		stream.PutOrGet(ref flagIndex);
	}
}
