using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_SEARCH_REQ)]
public sealed class NKMPacket_GUILD_SEARCH_REQ : ISerializable
{
	public string keyword;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref keyword);
	}
}
