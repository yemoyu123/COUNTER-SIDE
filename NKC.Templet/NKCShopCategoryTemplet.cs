using System.Collections.Generic;
using NKM;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCShopCategoryTemplet : INKMTemplet
{
	public NKCShopManager.ShopTabCategory m_eCategory;

	public string m_TabCategoryName;

	public int m_OrderList;

	public List<string> m_UseTabID;

	public string m_ThumbnailImg;

	public HashSet<int> m_UnusedResourceID;

	int INKMTemplet.Key => (int)m_eCategory;

	public static NKCShopCategoryTemplet Find(NKCShopManager.ShopTabCategory category)
	{
		return NKMTempletContainer<NKCShopCategoryTemplet>.Find((int)category);
	}

	public static void Load()
	{
		NKMTempletLoader.Load("AB_SCRIPT", "SHOP_CATEGORY_TEMPLET", LoadFromLUA, "LUA_SHOP_CATEGORY_TEMPLET_01", "LUA_SHOP_CATEGORY_TEMPLET_02");
	}

	public static NKCShopCategoryTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCShopCategoryTemplet.cs", 31))
		{
			return null;
		}
		NKCShopCategoryTemplet nKCShopCategoryTemplet = new NKCShopCategoryTemplet();
		bool flag = true;
		flag &= lua.GetData("m_ShopTabCategory", ref nKCShopCategoryTemplet.m_eCategory);
		flag &= lua.GetData("m_TabCategoryName", ref nKCShopCategoryTemplet.m_TabCategoryName);
		flag &= lua.GetData("m_OrderList", ref nKCShopCategoryTemplet.m_OrderList);
		flag &= lua.GetData("m_ThumbnailImg", ref nKCShopCategoryTemplet.m_ThumbnailImg);
		if (lua.OpenTable("m_UseTabID"))
		{
			nKCShopCategoryTemplet.m_UseTabID = new List<string>();
			int i = 1;
			for (string rValue = ""; lua.GetData(i, ref rValue); i++)
			{
				nKCShopCategoryTemplet.m_UseTabID.Add(rValue);
			}
			lua.CloseTable();
		}
		else
		{
			flag = false;
		}
		if (lua.OpenTable("m_UnusedResourceID"))
		{
			nKCShopCategoryTemplet.m_UnusedResourceID = new HashSet<int>();
			int j = 1;
			for (int rValue2 = 0; lua.GetData(j, ref rValue2); j++)
			{
				nKCShopCategoryTemplet.m_UnusedResourceID.Add(rValue2);
			}
			lua.CloseTable();
		}
		if (!flag)
		{
			return null;
		}
		return nKCShopCategoryTemplet;
	}

	public bool HasTab(string tabType)
	{
		return m_UseTabID.Contains(tabType);
	}

	void INKMTemplet.Join()
	{
	}

	void INKMTemplet.Validate()
	{
	}
}
