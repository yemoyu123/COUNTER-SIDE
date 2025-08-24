using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_FRIEND_PROFILE_MODIFY_MAIN_CHAR_ACK)]
public sealed class NKMPacket_FRIEND_PROFILE_MODIFY_MAIN_CHAR_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int mainCharId;

	public int mainCharSkinId;

	public int mainUnitTacticLevel;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref mainCharId);
		stream.PutOrGet(ref mainCharSkinId);
		stream.PutOrGet(ref mainUnitTacticLevel);
	}
}
