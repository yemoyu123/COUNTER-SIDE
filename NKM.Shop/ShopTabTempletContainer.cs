using System.Collections.Generic;
using System.Linq;
using NKM.Shop.Detail;
using NKM.Templet.Base;

namespace NKM.Shop;

public static class ShopTabTempletContainer
{
	private static readonly Dictionary<TabId, ShopTabTemplet> templets = new Dictionary<TabId, ShopTabTemplet>();

	private static readonly Dictionary<string, TabGroup> groups = new Dictionary<string, TabGroup>();

	internal static IEnumerable<ShopTabTemplet> Values => groups.SelectMany((KeyValuePair<string, TabGroup> e) => e.Value.List);

	public static void Load()
	{
		string[] array = new string[2] { "LUA_SHOP_TAB_TEMPLET_01", "LUA_SHOP_TAB_TEMPLET_02" };
		for (int i = 0; i < array.Length; i++)
		{
			LoadTemplet(array[i]);
		}
	}

	public static void Drop()
	{
		templets.Clear();
		groups.Clear();
	}

	private static void LoadTemplet(string luaFileName)
	{
		foreach (ShopTabTemplet item in NKMTempletLoader.LoadCommonPath("AB_SCRIPT", luaFileName, "m_ShopTabTable", ShopTabTemplet.LoadFromLUA))
		{
			if (!groups.TryGetValue(item.TabType, out var value))
			{
				value = new TabGroup(item.TabType);
				groups.Add(item.TabType, value);
			}
			value.Add(item);
			templets.Add(item.TabId, item);
		}
	}

	public static void Join()
	{
		foreach (TabGroup value in groups.Values)
		{
			value.Join();
		}
	}

	public static ShopTabTemplet Find(string tab, int subIndex)
	{
		if (!groups.TryGetValue(tab, out var value))
		{
			return null;
		}
		return value.Get(subIndex);
	}

	public static IEnumerable<ShopTabTemplet> GetAllSubtabs(string tab)
	{
		if (!groups.TryGetValue(tab, out var value))
		{
			return null;
		}
		return value.List;
	}

	public static ShopTabTemplet Find(TabId tabId)
	{
		if (!templets.TryGetValue(tabId, out var value))
		{
			return null;
		}
		return value;
	}

	public static void PostJoin()
	{
		foreach (TabGroup value in groups.Values)
		{
			value.PostJoin();
		}
	}
}
