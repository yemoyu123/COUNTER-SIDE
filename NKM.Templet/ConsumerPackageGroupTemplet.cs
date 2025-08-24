using System.Collections.Generic;
using System.Linq;
using NKM.Shop;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class ConsumerPackageGroupTemplet : INKMTemplet
{
	private readonly int productId;

	private readonly List<ConsumerPackageGroupData> datas = new List<ConsumerPackageGroupData>();

	private readonly long maxLevelRequireValue;

	private readonly int requireItemId;

	private ShopItemTemplet shopTemplet;

	public int Key => productId;

	public int RequireItemId => requireItemId;

	public long MaxLevel => datas.Count();

	public long MaxLevelRequireValue => maxLevelRequireValue;

	public ShopItemTemplet ShopTemplet => shopTemplet;

	public ConsumerPackageGroupTemplet(int packageId, List<ConsumerPackageGroupData> datas)
	{
		if (datas == null || datas.Count == 0)
		{
			NKMTempletError.Add($"invalid data list. packageId:{packageId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ConsumerPackageGroupTemplet.cs", 21);
			return;
		}
		productId = packageId;
		this.datas.AddRange(datas.OrderBy((ConsumerPackageGroupData e) => e.ConsumeRequireItemValue));
		requireItemId = this.datas.Last().ConsumeRequireItemId;
		maxLevelRequireValue = this.datas.Last().ConsumeRequireItemValue;
	}

	public static ConsumerPackageGroupTemplet Find(int key)
	{
		return NKMTempletContainer<ConsumerPackageGroupTemplet>.Find(key);
	}

	public static void ValidateServerOnly()
	{
		foreach (ShopItemTemplet value in NKMTempletContainer<ShopItemTemplet>.Values)
		{
			if (value.m_PurchaseEventType == PURCHASE_EVENT_REWARD_TYPE.CONSUMER_PACKAGE && Find(value.m_PurchaseEventValue) == null)
			{
				NKMTempletError.Add($"[ShopTemplet] 소비자 패키지 정보가 존재하지 않음 m_ProductId:{value.m_ProductID}, m_PurchaseEventValue:{value.m_PurchaseEventValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ConsumerPackageGroupTemplet.cs", 47);
			}
		}
	}

	public ConsumerPackageGroupData GetRewardData(int level)
	{
		int num = level - 1;
		if (num < 0)
		{
			return null;
		}
		return datas[num];
	}

	public int RewardLevel(long consumeValue)
	{
		return datas.Where((ConsumerPackageGroupData e) => e.ConsumeRequireItemValue <= consumeValue).Count();
	}

	public void Join()
	{
		foreach (ConsumerPackageGroupData data in datas)
		{
			data.Join();
		}
		IEnumerable<ShopItemTemplet> source = NKMTempletContainer<ShopItemTemplet>.Values.Where((ShopItemTemplet e) => e.m_bEnabled && e.m_PurchaseEventType == PURCHASE_EVENT_REWARD_TYPE.CONSUMER_PACKAGE && e.m_PurchaseEventValue == productId);
		if (!source.Any())
		{
			NKMTempletError.Add($"[ConsumerPackageTemplet] 해당 패키지 Id와 연결된 상품이 존재하지 않음 m_PackageId:{productId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ConsumerPackageGroupTemplet.cs", 82);
		}
		if (source.Count() > 1)
		{
			NKMTempletError.Add(string.Format("[ConsumerPackageTemplet] 동일한 패키지 Id를 가지는 상품이 두개 이상 존재함 m_PackageId:{0}, ProductIds:{1}", productId, string.Join(",", source.Select((ShopItemTemplet e) => e.m_ProductID))), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ConsumerPackageGroupTemplet.cs", 87);
		}
		shopTemplet = source.FirstOrDefault();
	}

	public void Validate()
	{
		foreach (ConsumerPackageGroupData data in datas)
		{
			data.Validate();
		}
		long[] array = (from e in datas
			where e.ConsumeRequireItemId != requireItemId
			select e.ConsumeRequireItemValue).ToArray();
		if (array.Any())
		{
			string arg = string.Join(", ", array);
			NKMTempletError.Add($"[ConsumerPackageTemplet] 동일한 packageId 정보 중 itemId가 맞지 않는 항목이 존재. m_PackageId:{productId} Require:{arg}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ConsumerPackageGroupTemplet.cs", 108);
		}
		if (shopTemplet.EventIntervalTemplet == null)
		{
			NKMTempletError.Add($"[ConsumerPackageTemplet] 패키지 상품의 EventIntervalTemplet 정보가 없음. m_PackageId:{productId} productId:{shopTemplet.Key}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ConsumerPackageGroupTemplet.cs", 113);
		}
		if (shopTemplet.resetType != SHOP_RESET_TYPE.FIXED)
		{
			NKMTempletError.Add($"[ConsumerPackageTemplet] 패키지 상품의 리셋 타입이 FIXED가 아님. m_PackageId:{productId}, resetType:{shopTemplet.resetType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ConsumerPackageGroupTemplet.cs", 118);
		}
	}
}
