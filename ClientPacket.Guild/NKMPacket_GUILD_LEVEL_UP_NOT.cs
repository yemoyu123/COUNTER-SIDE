using System;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_LEVEL_UP_NOT)]
public sealed class NKMPacket_GUILD_LEVEL_UP_NOT : ISerializable
{
	public long guildUid;

	public int guildLevel;

	public long guildLevelExp;

	public long guildTotalExp;

	public DateTime levelUpTime;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref guildLevel);
		stream.PutOrGet(ref guildLevelExp);
		stream.PutOrGet(ref guildTotalExp);
		stream.PutOrGet(ref levelUpTime);
	}
}
