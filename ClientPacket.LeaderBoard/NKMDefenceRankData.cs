using ClientPacket.Common;
using Cs.Protocol;

namespace ClientPacket.LeaderBoard;

public sealed class NKMDefenceRankData : ISerializable
{
	public NKMCommonProfile commonProfile = new NKMCommonProfile();

	public int bestScore;

	public NKMGuildSimpleData guildData = new NKMGuildSimpleData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref commonProfile);
		stream.PutOrGet(ref bestScore);
		stream.PutOrGet(ref guildData);
	}
}
