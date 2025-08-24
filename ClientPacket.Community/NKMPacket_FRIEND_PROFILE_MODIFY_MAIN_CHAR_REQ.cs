using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_FRIEND_PROFILE_MODIFY_MAIN_CHAR_REQ)]
public sealed class NKMPacket_FRIEND_PROFILE_MODIFY_MAIN_CHAR_REQ : ISerializable
{
	public int mainCharId;

	public int mainCharSkinId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref mainCharId);
		stream.PutOrGet(ref mainCharSkinId);
	}
}
