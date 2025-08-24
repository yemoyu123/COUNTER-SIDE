using Cs.Protocol;

namespace NKM;

public class NpcPvpData : ISerializable
{
	public int MaxTierCount;

	public int MaxOpenedTier;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref MaxTierCount);
		stream.PutOrGet(ref MaxOpenedTier);
	}
}
