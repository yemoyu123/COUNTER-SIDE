using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DATA_UPDATED_NOT)]
public sealed class NKMPacket_GUILD_DATA_UPDATED_NOT : ISerializable
{
	public NKMGuildData guildData = new NKMGuildData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildData);
	}
}
