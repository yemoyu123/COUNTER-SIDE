using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DATA_ACK)]
public sealed class NKMPacket_GUILD_DATA_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long guildUid;

	public NKMGuildData guildData = new NKMGuildData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref guildData);
	}
}
