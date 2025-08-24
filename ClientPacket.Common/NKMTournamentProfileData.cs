using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class NKMTournamentProfileData : ISerializable
{
	public NKMTournamentCountryCode countryCode;

	public NKMCommonProfile commonProfile = new NKMCommonProfile();

	public NKMGuildSimpleData guildData = new NKMGuildSimpleData();

	public NKMAsyncDeckData deck = new NKMAsyncDeckData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref countryCode);
		stream.PutOrGet(ref commonProfile);
		stream.PutOrGet(ref guildData);
		stream.PutOrGet(ref deck);
	}
}
