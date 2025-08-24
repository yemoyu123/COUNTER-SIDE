using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_FRIEND_ACCEPT_REQ)]
public sealed class NKMPacket_FRIEND_ACCEPT_REQ : ISerializable
{
	public long friendCode;

	public bool isAllow;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref friendCode);
		stream.PutOrGet(ref isAllow);
	}
}
