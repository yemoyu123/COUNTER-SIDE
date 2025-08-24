using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_FRIEND_CANCEL_REQUEST_NOT)]
public sealed class NKMPacket_FRIEND_CANCEL_REQUEST_NOT : ISerializable
{
	public long userUid;

	public long friendCode;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref userUid);
		stream.PutOrGet(ref friendCode);
	}
}
