using System.Collections.Generic;
using NKM;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCShopCustomTabTemplet
{
	public const string CashTabName = "TAB_CASH";

	private TabId tabId;

	public string listContentsTagAllow;

	public string listContentsTagIgnore;

	public int m_OrderList;

	public string m_UsePrefabName;

	public List<int> m_UseProductID;

	private static Dictionary<TabId, List<NKCShopCustomTabTemplet>> groups;

	private static bool bLoaded;

	public TabId TabId => tabId;

	public string TabType => tabId.Type;

	public int SubIndex => tabId.SubIndex;

	public static List<NKCShopCustomTabTemplet> Find(TabId tabID)
	{
		if (!bLoaded)
		{
			bLoaded = true;
			groups = new Dictionary<TabId, List<NKCShopCustomTabTemplet>>();
			foreach (NKCShopCustomTabTemplet item in NKMTempletLoader.LoadCommonPath("AB_SCRIPT", "LUA_SHOP_TAB_CUSTOM_TEMPLET", "SHOP_TAB_CUSTOM_TEMPLET", LoadFromLUA))
			{
				if (!groups.TryGetValue(item.TabId, out var value))
				{
					value = new List<NKCShopCustomTabTemplet>();
					groups.Add(item.TabId, value);
				}
				value.Add(item);
			}
			foreach (KeyValuePair<TabId, List<NKCShopCustomTabTemplet>> group in groups)
			{
				group.Value.Sort((NKCShopCustomTabTemplet a, NKCShopCustomTabTemplet b) => a.m_OrderList.CompareTo(b.m_OrderList));
			}
		}
		if (groups.TryGetValue(tabID, out var value2))
		{
			return value2;
		}
		return null;
	}

	public static NKCShopCustomTabTemplet Find(TabId tabID, int index)
	{
		List<NKCShopCustomTabTemplet> list = Find(tabID);
		if (list == null)
		{
			return null;
		}
		if (index < 0)
		{
			return null;
		}
		if (index < list.Count)
		{
			return list[index];
		}
		return null;
	}

	public static NKCShopCustomTabTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCShopCustomTabTemplet.cs", 77))
		{
			return null;
		}
		NKCShopCustomTabTemplet nKCShopCustomTabTemplet = new NKCShopCustomTabTemplet();
		string rValue;
		int rValue2;
		int num = (int)(1u & (cNKMLua.GetData("m_TabID", out rValue, "TAB_CASH") ? 1u : 0u) & (cNKMLua.GetData("m_TabSubIndex", out rValue2, 0) ? 1u : 0u) & (cNKMLua.GetData("m_OrderList", ref nKCShopCustomTabTemplet.m_OrderList) ? 1u : 0u) & (cNKMLua.GetData("m_UsePrefabName", ref nKCShopCustomTabTemplet.m_UsePrefabName) ? 1u : 0u)) & (cNKMLua.GetDataList("m_UseProductID", out nKCShopCustomTabTemplet.m_UseProductID, nullIfEmpty: false) ? 1 : 0);
		nKCShopCustomTabTemplet.tabId = new TabId(rValue, rValue2);
		if (num == 0)
		{
			return null;
		}
		return nKCShopCustomTabTemplet;
	}

	public static void Drop()
	{
		groups = null;
		bLoaded = false;
	}
}
