using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_MASTER_SPECIFIED_MIGRATION_NOT)]
public sealed class NKMPacket_GUILD_MASTER_SPECIFIED_MIGRATION_NOT : ISerializable
{
	public long guildUid;

	public long oldMasterUserUid;

	public long newMasterUserUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref oldMasterUserUid);
		stream.PutOrGet(ref newMasterUserUid);
	}
}
