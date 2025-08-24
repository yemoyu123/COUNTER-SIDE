using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DUNGEON_MEMBER_INFO_ACK)]
public sealed class NKMPacket_GUILD_DUNGEON_MEMBER_INFO_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<GuildDungeonMemberInfo> memberInfoList = new List<GuildDungeonMemberInfo>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref memberInfoList);
	}
}
