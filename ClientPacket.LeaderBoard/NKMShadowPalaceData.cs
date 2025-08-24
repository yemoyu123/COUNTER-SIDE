using ClientPacket.Common;
using Cs.Protocol;

namespace ClientPacket.LeaderBoard;

public sealed class NKMShadowPalaceData : ISerializable
{
	public NKMCommonProfile commonProfile = new NKMCommonProfile();

	public int bestTime;

	public NKMGuildSimpleData guildData = new NKMGuildSimpleData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref commonProfile);
		stream.PutOrGet(ref bestTime);
		stream.PutOrGet(ref guildData);
	}
}
