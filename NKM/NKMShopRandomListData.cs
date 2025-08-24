using Cs.Protocol;
using NKM.Templet;

namespace NKM;

public class NKMShopRandomListData : ISerializable
{
	public int itemId;

	public NKM_REWARD_TYPE itemType;

	public int itemCount;

	public int priceItemId;

	public int price;

	public bool isBuy;

	public int discountRatio;

	public int GetPrice()
	{
		if (discountRatio > 0)
		{
			return price * (100 - discountRatio) / 100;
		}
		return price;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref itemId);
		stream.PutOrGetEnum(ref itemType);
		stream.PutOrGet(ref itemCount);
		stream.PutOrGet(ref priceItemId);
		stream.PutOrGet(ref price);
		stream.PutOrGet(ref isBuy);
		stream.PutOrGet(ref discountRatio);
	}
}
