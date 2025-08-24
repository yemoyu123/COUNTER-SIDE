using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_ACCEPT_JOIN_NOT)]
public sealed class NKMPacket_GUILD_ACCEPT_JOIN_NOT : ISerializable
{
	public bool isAllow;

	public long guildUid;

	public string guildName;

	public PrivateGuildData privateGuildData = new PrivateGuildData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref isAllow);
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref guildName);
		stream.PutOrGet(ref privateGuildData);
	}
}
