using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_FRIEND_PROFILE_MODIFY_INTRO_ACK)]
public sealed class NKMPacket_FRIEND_PROFILE_MODIFY_INTRO_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public string intro;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref intro);
	}
}
