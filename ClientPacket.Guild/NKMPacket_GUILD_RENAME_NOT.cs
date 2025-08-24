using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_RENAME_NOT)]
public sealed class NKMPacket_GUILD_RENAME_NOT : ISerializable
{
	public long guildUid;

	public string newName;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref newName);
	}
}
