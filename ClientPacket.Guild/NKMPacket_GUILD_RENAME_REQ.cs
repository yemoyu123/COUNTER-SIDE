using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_RENAME_REQ)]
public sealed class NKMPacket_GUILD_RENAME_REQ : ISerializable
{
	public string newName;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref newName);
	}
}
