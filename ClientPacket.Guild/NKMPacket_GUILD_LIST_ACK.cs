using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_LIST_ACK)]
public sealed class NKMPacket_GUILD_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<GuildListData> list = new List<GuildListData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref list);
	}
}
