using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class NKMAdditionalReward : ISerializable
{
	public long guildExpDelta;

	public long unionPointDelta;

	public long eventPassExpDelta;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildExpDelta);
		stream.PutOrGet(ref unionPointDelta);
		stream.PutOrGet(ref eventPassExpDelta);
	}
}
