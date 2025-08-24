using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_MASTER_MIGRATION_ACK)]
public sealed class NKMPacket_GUILD_MASTER_MIGRATION_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long guildUid;

	public long oldMasterUserUid;

	public long newMasterUserUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref oldMasterUserUid);
		stream.PutOrGet(ref newMasterUserUid);
	}
}
