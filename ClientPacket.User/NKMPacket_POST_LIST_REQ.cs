using Cs.Protocol;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_POST_LIST_REQ)]
public sealed class NKMPacket_POST_LIST_REQ : ISerializable
{
	public long lastPostIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref lastPostIndex);
	}
}
