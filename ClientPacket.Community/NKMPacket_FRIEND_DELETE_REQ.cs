using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_FRIEND_DELETE_REQ)]
public sealed class NKMPacket_FRIEND_DELETE_REQ : ISerializable
{
	public long friendCode;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref friendCode);
	}
}
