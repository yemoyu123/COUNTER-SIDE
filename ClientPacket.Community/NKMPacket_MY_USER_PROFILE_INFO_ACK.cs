using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_MY_USER_PROFILE_INFO_ACK)]
public sealed class NKMPacket_MY_USER_PROFILE_INFO_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMUserProfileData userProfileData = new NKMUserProfileData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref userProfileData);
	}
}
