using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DUNGEON_NOTICE_UPDATE_ACK)]
public sealed class NKMPacket_GUILD_DUNGEON_NOTICE_UPDATE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long guildUid;

	public string noticeBefore;

	public string notice;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref noticeBefore);
		stream.PutOrGet(ref notice);
	}
}
