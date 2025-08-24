using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_LOG_LIST_NOT)]
public sealed class NKMPacket_GUILD_LOG_LIST_NOT : ISerializable
{
	public long guildUid;

	public List<NKMGuildLogMessageData> logs = new List<NKMGuildLogMessageData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref logs);
	}
}
