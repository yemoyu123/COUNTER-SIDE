using Cs.Protocol;

namespace NKM.Templet;

public class NKMRewardInfo : ISerializable
{
	public NKM_REWARD_TYPE rewardType;

	public NKM_ITEM_PAYMENT_TYPE paymentType;

	public int ID;

	public int Count;

	public string DebugName => $"[{rewardType}] id:{ID} count:{Count} paymentType:{paymentType}";

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref rewardType);
		stream.PutOrGetEnum(ref paymentType);
		stream.PutOrGet(ref ID);
		stream.PutOrGet(ref Count);
	}

	public string GetRewardName()
	{
		return rewardType.ParseName(ID);
	}
}
