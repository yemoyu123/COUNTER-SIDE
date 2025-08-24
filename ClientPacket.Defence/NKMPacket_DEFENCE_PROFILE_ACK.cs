using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Defence;

[PacketId(ClientPacketId.kNKMPacket_DEFENCE_PROFILE_ACK)]
public sealed class NKMPacket_DEFENCE_PROFILE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMCommonProfile commonProfile = new NKMCommonProfile();

	public NKMGuildSimpleData guildData = new NKMGuildSimpleData();

	public string friendIntro;

	public NKMDefenceProfileData profileData = new NKMDefenceProfileData();

	public int rank;

	public int rankPercent;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref commonProfile);
		stream.PutOrGet(ref guildData);
		stream.PutOrGet(ref friendIntro);
		stream.PutOrGet(ref profileData);
		stream.PutOrGet(ref rank);
		stream.PutOrGet(ref rankPercent);
	}
}
