using Cs.Protocol;

namespace ClientPacket.Shop;

public sealed class NKMConsumerPackageData : ISerializable
{
	public int productId;

	public int rewardedLevel;

	public long spendCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref productId);
		stream.PutOrGet(ref rewardedLevel);
		stream.PutOrGet(ref spendCount);
	}
}
