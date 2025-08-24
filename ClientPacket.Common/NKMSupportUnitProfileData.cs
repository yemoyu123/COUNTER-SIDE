using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class NKMSupportUnitProfileData : ISerializable
{
	public NKMCommonProfile commonProfile = new NKMCommonProfile();

	public NKMGuildSimpleData guildData = new NKMGuildSimpleData();

	public NKMSupportUnitData supportUnitData = new NKMSupportUnitData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref commonProfile);
		stream.PutOrGet(ref guildData);
		stream.PutOrGet(ref supportUnitData);
	}
}
