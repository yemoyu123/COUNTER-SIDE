using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_FIERCE_PROFILE_ACK)]
public sealed class NKMPacket_FIERCE_PROFILE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMCommonProfile commonProfile = new NKMCommonProfile();

	public NKMGuildSimpleData guildData = new NKMGuildSimpleData();

	public string friendIntro;

	public NKMFierceProfileData profileData = new NKMFierceProfileData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref commonProfile);
		stream.PutOrGet(ref guildData);
		stream.PutOrGet(ref friendIntro);
		stream.PutOrGet(ref profileData);
	}
}
