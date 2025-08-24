using System;
using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Shop;
using Cs.Protocol;
using NKC;
using NKM.Shop;

namespace NKM;

public class NKMShopData : ISerializable
{
	public Dictionary<int, NKMShopPurchaseHistory> histories = new Dictionary<int, NKMShopPurchaseHistory>();

	public NKMShopRandomData randomShop;

	public Dictionary<int, NKMShopSubscriptionData> subscriptions;

	private double totalPaidAmount;

	private List<ShopChainTabNextResetData> m_lstChainTabResetData = new List<ShopChainTabNextResetData>();

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref histories);
		stream.PutOrGet(ref randomShop);
		stream.PutOrGet(ref subscriptions);
	}

	public int GetRealPrice(ShopItemTemplet shopTemplet, int count = 1, bool useSteamPrice = false)
	{
		int num = shopTemplet.m_Price;
		if (useSteamPrice)
		{
			num = ((!NKMOpenTagManager.IsOpened("STEAM_CURRENCY_KRW")) ? shopTemplet.m_PriceSteam : shopTemplet.m_PriceSteamKRW);
		}
		if (shopTemplet.m_DiscountRate > 0f && NKCSynchronizedTime.IsEventTime(shopTemplet.discountIntervalId, shopTemplet.DiscountStartDateUtc, shopTemplet.DiscountEndDateUtc))
		{
			num -= (int)((float)num * (shopTemplet.m_DiscountRate / 100f));
		}
		if (shopTemplet.m_PurchaseEventType == PURCHASE_EVENT_REWARD_TYPE.INCREACE_PRICE_PER_PURCHASE_COUNT)
		{
			int purchasedCount = GetPurchasedCount(shopTemplet);
			int num2 = purchasedCount + count;
			return (num2 * (num2 + 1) - purchasedCount * (purchasedCount + 1)) * num / 2;
		}
		return num * count;
	}

	public int GetPurchasedCount(ShopItemTemplet shopTemplet)
	{
		if (histories.TryGetValue(shopTemplet.m_ProductID, out var value))
		{
			if (shopTemplet.IsCountResetType() && NKCSynchronizedTime.IsFinished(value.nextResetDate))
			{
				return 0;
			}
			return value.purchaseCount;
		}
		return 0;
	}

	public void SetTotalPayment(double totalPaid)
	{
		totalPaidAmount = totalPaid;
	}

	public double GetTotalPayment()
	{
		return totalPaidAmount;
	}

	public void SetChainTabResetData(List<ShopChainTabNextResetData> lstChainTabResetData)
	{
		m_lstChainTabResetData = lstChainTabResetData;
	}

	public DateTime GetChainTabResetTime(string tabType, int subIndex)
	{
		for (int i = 0; i < m_lstChainTabResetData.Count; i++)
		{
			if (m_lstChainTabResetData[i].tabType == tabType && m_lstChainTabResetData[i].subIndex == subIndex)
			{
				return m_lstChainTabResetData[i].nextResetUtc;
			}
		}
		return default(DateTime);
	}
}
