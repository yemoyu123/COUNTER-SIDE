using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_MY_USER_PROFILE_INFO_REQ)]
public sealed class NKMPacket_MY_USER_PROFILE_INFO_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
