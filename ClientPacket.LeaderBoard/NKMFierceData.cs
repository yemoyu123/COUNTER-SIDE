using ClientPacket.Common;
using Cs.Protocol;

namespace ClientPacket.LeaderBoard;

public sealed class NKMFierceData : ISerializable
{
	public NKMCommonProfile commonProfile = new NKMCommonProfile();

	public long fiercePoint;

	public NKMGuildSimpleData guildData = new NKMGuildSimpleData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref commonProfile);
		stream.PutOrGet(ref fiercePoint);
		stream.PutOrGet(ref guildData);
	}
}
