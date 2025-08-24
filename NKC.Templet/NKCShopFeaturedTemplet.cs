using NKM;
using NKM.Shop;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCShopFeaturedTemplet : INKMTemplet
{
	public enum DisplayCond
	{
		None,
		HAVE_ITEM_UNDER,
		PAID_AMOUNT_UNDER,
		PAID_AMOUNT_OVER
	}

	public int m_PackageID;

	public string m_PackageGroupID;

	public int m_OrderList;

	public DisplayCond m_DisplayCond;

	public int m_DisplayCondValue1;

	public int m_DisplayCondValue2;

	public string m_FeaturedImage;

	public bool m_ReddotRequired;

	int INKMTemplet.Key => m_PackageID;

	public static NKCShopFeaturedTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCShopFeaturedTemplet.cs", 36))
		{
			return null;
		}
		NKCShopFeaturedTemplet nKCShopFeaturedTemplet = new NKCShopFeaturedTemplet();
		bool flag = true;
		flag &= lua.GetData("m_PackageID", ref nKCShopFeaturedTemplet.m_PackageID);
		flag &= lua.GetData("m_PackageGroupID", ref nKCShopFeaturedTemplet.m_PackageGroupID);
		flag &= lua.GetData("m_OrderList", ref nKCShopFeaturedTemplet.m_OrderList);
		lua.GetData("m_DisplayCond", ref nKCShopFeaturedTemplet.m_DisplayCond);
		if (nKCShopFeaturedTemplet.m_DisplayCond != DisplayCond.None)
		{
			flag &= lua.GetData("m_DisplayCondValue1", ref nKCShopFeaturedTemplet.m_DisplayCondValue1);
			lua.GetData("m_DisplayCondValue2", ref nKCShopFeaturedTemplet.m_DisplayCondValue2);
		}
		lua.GetData("m_FeaturedImage", ref nKCShopFeaturedTemplet.m_FeaturedImage);
		lua.GetData("m_ReddotRequired", ref nKCShopFeaturedTemplet.m_ReddotRequired);
		if (!flag)
		{
			return null;
		}
		return nKCShopFeaturedTemplet;
	}

	public static void Load()
	{
		NKMTempletContainer<NKCShopFeaturedTemplet>.Load("AB_SCRIPT", "LUA_SHOP_FEATURED_TEMPLET", "SHOP_FEATURED_TEMPLET", LoadFromLUA);
	}

	void INKMTemplet.Join()
	{
	}

	void INKMTemplet.Validate()
	{
	}

	public bool CheckCondition(NKMUserData userData)
	{
		return m_DisplayCond switch
		{
			DisplayCond.HAVE_ITEM_UNDER => userData.m_InventoryData.GetCountMiscItem(m_DisplayCondValue1) < m_DisplayCondValue2, 
			DisplayCond.PAID_AMOUNT_OVER => userData.m_ShopData.GetTotalPayment() >= (double)m_DisplayCondValue1, 
			DisplayCond.PAID_AMOUNT_UNDER => userData.m_ShopData.GetTotalPayment() < (double)m_DisplayCondValue1, 
			_ => true, 
		};
	}

	public static int CompareHighPriceFirst(NKCShopFeaturedTemplet lhs, NKCShopFeaturedTemplet rhs)
	{
		if (lhs.m_OrderList != rhs.m_OrderList)
		{
			return lhs.m_OrderList.CompareTo(rhs.m_OrderList);
		}
		ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(lhs.m_PackageID);
		ShopItemTemplet shopItemTemplet2 = ShopItemTemplet.Find(rhs.m_PackageID);
		int priceItemOrder = GetPriceItemOrder(shopItemTemplet.m_PriceItemID);
		int priceItemOrder2 = GetPriceItemOrder(shopItemTemplet2.m_PriceItemID);
		if (priceItemOrder != priceItemOrder2)
		{
			return priceItemOrder.CompareTo(priceItemOrder2);
		}
		if (shopItemTemplet.m_PriceItemID != shopItemTemplet2.m_PriceItemID)
		{
			return shopItemTemplet.m_PriceItemID.CompareTo(shopItemTemplet2.m_PriceItemID);
		}
		return shopItemTemplet2.m_Price.CompareTo(shopItemTemplet.m_Price);
	}

	public static int CompareLowPriceFirst(NKCShopFeaturedTemplet lhs, NKCShopFeaturedTemplet rhs)
	{
		if (lhs.m_OrderList != rhs.m_OrderList)
		{
			return lhs.m_OrderList.CompareTo(rhs.m_OrderList);
		}
		ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(lhs.m_PackageID);
		ShopItemTemplet shopItemTemplet2 = ShopItemTemplet.Find(rhs.m_PackageID);
		int priceItemOrder = GetPriceItemOrder(shopItemTemplet.m_PriceItemID);
		int priceItemOrder2 = GetPriceItemOrder(shopItemTemplet2.m_PriceItemID);
		if (priceItemOrder != priceItemOrder2)
		{
			return priceItemOrder.CompareTo(priceItemOrder2);
		}
		if (shopItemTemplet.m_PriceItemID != shopItemTemplet2.m_PriceItemID)
		{
			return shopItemTemplet.m_PriceItemID.CompareTo(shopItemTemplet2.m_PriceItemID);
		}
		return shopItemTemplet.m_Price.CompareTo(shopItemTemplet2.m_Price);
	}

	private static int GetPriceItemOrder(int priceItemID)
	{
		return priceItemID switch
		{
			0 => 0, 
			102 => 1, 
			101 => 2, 
			_ => 3, 
		};
	}
}
