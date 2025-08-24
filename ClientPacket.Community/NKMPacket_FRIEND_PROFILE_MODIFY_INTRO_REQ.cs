using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_FRIEND_PROFILE_MODIFY_INTRO_REQ)]
public sealed class NKMPacket_FRIEND_PROFILE_MODIFY_INTRO_REQ : ISerializable
{
	public string intro;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref intro);
	}
}
