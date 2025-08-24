using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_CANCEL_REQUEST_NOT)]
public sealed class NKMPacket_GUILD_CANCEL_REQUEST_NOT : ISerializable
{
	public long guildUid;

	public bool isRequest;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref isRequest);
	}
}
