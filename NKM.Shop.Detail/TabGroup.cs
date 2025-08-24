using System.Collections.Generic;
using System.Linq;
using Cs.Core.Util;
using Cs.Logging;

namespace NKM.Shop.Detail;

internal sealed class TabGroup
{
	private readonly string tabType;

	private readonly List<ShopTabTemplet> list = new List<ShopTabTemplet>(10);

	public IEnumerable<ShopTabTemplet> List => list.Where((ShopTabTemplet e) => e != null);

	public TabGroup(string tabType)
	{
		this.tabType = tabType;
	}

	internal void Add(ShopTabTemplet templet)
	{
		int subIndex = templet.SubIndex;
		GuraranteeListSize(subIndex + 1);
		if (list[subIndex] != null)
		{
			Log.ErrorAndExit("[ShopTabGroup] subIndex duplicated. tabType:" + tabType, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/Detail/TabGroup.cs", 26);
		}
		list[subIndex] = templet;
	}

	internal void Join()
	{
		foreach (ShopTabTemplet item in list)
		{
			item?.Join();
		}
		if (NKMUtil.IsServer)
		{
			UnionTabIntervals();
		}
	}

	internal void UnionTabIntervals()
	{
		for (int i = 0; i < list.Count; i++)
		{
			list[i]?.UnionItemIntervals(ServiceTime.Recent);
		}
		if (list.Count > 1)
		{
			list[0]?.UnionTabIntervals(list, ServiceTime.Recent);
		}
	}

	internal ShopTabTemplet Get(int subIndex)
	{
		if (subIndex < 0 || subIndex >= list.Count)
		{
			return null;
		}
		return list[subIndex];
	}

	private void GuraranteeListSize(int size)
	{
		while (list.Count < size)
		{
			list.Add(null);
		}
	}

	internal void PostJoin()
	{
		if (!NKMUtil.IsServer)
		{
			UnionTabIntervals();
		}
	}
}
