using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_USER_PROFILE_BY_FRIEND_CODE_REQ)]
public sealed class NKMPacket_USER_PROFILE_BY_FRIEND_CODE_REQ : ISerializable
{
	public long friendCode;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref friendCode);
	}
}
