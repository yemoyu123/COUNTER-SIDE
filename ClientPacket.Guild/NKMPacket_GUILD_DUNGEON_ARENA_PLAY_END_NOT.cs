using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DUNGEON_ARENA_PLAY_END_NOT)]
public sealed class NKMPacket_GUILD_DUNGEON_ARENA_PLAY_END_NOT : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long playedUserUid;

	public int arenaId;

	public int totalGrade;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref playedUserUid);
		stream.PutOrGet(ref arenaId);
		stream.PutOrGet(ref totalGrade);
	}
}
