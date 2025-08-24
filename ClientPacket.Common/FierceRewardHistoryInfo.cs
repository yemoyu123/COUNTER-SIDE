using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class FierceRewardHistoryInfo : ISerializable
{
	public FierceRewardType fierceRewardType;

	public int fierceRewardId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref fierceRewardType);
		stream.PutOrGet(ref fierceRewardId);
	}
}
