using ClientPacket.Common;
using Cs.Protocol;

namespace ClientPacket.LeaderBoard;

public sealed class NKMAchieveData : ISerializable
{
	public NKMCommonProfile commonProfile = new NKMCommonProfile();

	public long achievePoint;

	public NKMGuildSimpleData guildData = new NKMGuildSimpleData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref commonProfile);
		stream.PutOrGet(ref achievePoint);
		stream.PutOrGet(ref guildData);
	}
}
