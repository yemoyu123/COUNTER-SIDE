using System;
using NKM;
using NKM.Shop;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKC;

public class NKCShopBannerTemplet : INKMTemplet
{
	public string m_ContentsVersionStart = "";

	public string m_ContentsVersionEnd = "";

	public int IDX;

	public bool m_Enable;

	public string m_ShopHome_BannerImage;

	public string m_ShopHome_BannerPrefab;

	public string m_TabID;

	public int m_TabSubIndex;

	public int m_ProductID;

	public SHOP_RECOMMEND_COND m_DisplayCond;

	public string m_DisplayCondValue;

	public string m_DateStrID;

	public string m_OpenTag;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public DateTime m_EventDateStartUTC => NKCSynchronizedTime.GetIntervalStartUtc(m_DateStrID);

	public DateTime m_EventDateEndUTC => NKCSynchronizedTime.GetIntervalEndUtc(m_DateStrID);

	public int Key => IDX;

	public static NKCShopBannerTemplet Find(int idx)
	{
		return NKMTempletContainer<NKCShopBannerTemplet>.Find(idx);
	}

	public static NKCShopBannerTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCShopBannerTemplet.cs", 55))
		{
			return null;
		}
		NKCShopBannerTemplet nKCShopBannerTemplet = new NKCShopBannerTemplet();
		lua.GetData("m_ContentsVersionStart", ref nKCShopBannerTemplet.m_ContentsVersionStart);
		lua.GetData("m_ContentsVersionEnd", ref nKCShopBannerTemplet.m_ContentsVersionEnd);
		int num = (int)(1u & (lua.GetData("IDX", ref nKCShopBannerTemplet.IDX) ? 1u : 0u)) & (lua.GetData("m_Enable", ref nKCShopBannerTemplet.m_Enable) ? 1 : 0);
		lua.GetData("m_ShopHome_BannerImage", ref nKCShopBannerTemplet.m_ShopHome_BannerImage);
		lua.GetData("m_ShopHome_BannerPrefab", ref nKCShopBannerTemplet.m_ShopHome_BannerPrefab);
		int num2 = (int)((uint)num & ((!string.IsNullOrEmpty(nKCShopBannerTemplet.m_ShopHome_BannerImage) || !string.IsNullOrEmpty(nKCShopBannerTemplet.m_ShopHome_BannerPrefab)) ? 1u : 0u) & (lua.GetData("m_TabID", ref nKCShopBannerTemplet.m_TabID) ? 1u : 0u)) & (lua.GetData("m_TabSubIndex", ref nKCShopBannerTemplet.m_TabSubIndex) ? 1 : 0);
		lua.GetData("m_DateStrID", ref nKCShopBannerTemplet.m_DateStrID);
		lua.GetData("m_ProductID", ref nKCShopBannerTemplet.m_ProductID);
		lua.GetData("m_DisplayCond", ref nKCShopBannerTemplet.m_DisplayCond);
		lua.GetData("m_DisplayCondValue", ref nKCShopBannerTemplet.m_DisplayCondValue);
		lua.GetData("m_OpenTag", ref nKCShopBannerTemplet.m_OpenTag);
		if (num2 == 0)
		{
			return null;
		}
		return nKCShopBannerTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (ShopTabTemplet.Find(m_TabID, m_TabSubIndex) == null)
		{
			NKMTempletError.Add($"[BannerTemplet] tabID / subIndex 에 해당하는 ShopTabTemplet 이 없음. tabId:{m_TabID} subIndex:{m_TabSubIndex}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCShopBannerTemplet.cs", 96);
		}
		if (m_ProductID > 0)
		{
			if (ShopItemTemplet.Find(m_ProductID) == null)
			{
				NKMTempletError.Add($"[BannerTemplet] m_ProductID 에 해당하는 상품이 없음. m_ProductID :{m_ProductID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCShopBannerTemplet.cs", 103);
			}
			if (ShopItemTemplet.Find(m_ProductID).m_TabID != m_TabID || ShopItemTemplet.Find(m_ProductID).m_TabSubIndex != m_TabSubIndex)
			{
				NKMTempletError.Add($"[BannerTemplet] 상품의 탭 정보와 일치하지 않음. m_ProductID :{m_ProductID} tabId:{m_TabID} subIndex:{m_TabSubIndex}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCShopBannerTemplet.cs", 108);
			}
		}
		if (NKMIntervalTemplet.Find(m_DateStrID) == null)
		{
			NKMTempletError.Add($"NKCShopRecommendTemplet(IDX {IDX}) : Interval Templet {m_DateStrID} 존재하지 않음", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCShopBannerTemplet.cs", 114);
		}
	}
}
