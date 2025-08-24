using System.Collections.Generic;
using System.Linq;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCItemDropInfoTemplet : INKMTemplet
{
	private int index;

	private readonly string itemId;

	private readonly List<ItemDropInfo> m_DropInfoList;

	public int Key => index;

	public List<ItemDropInfo> ItemDropInfoList
	{
		get
		{
			if (m_DropInfoList != null)
			{
				return m_DropInfoList;
			}
			return new List<ItemDropInfo>();
		}
	}

	public static NKCItemDropInfoTemplet Find(string key)
	{
		return NKMTempletContainer<NKCItemDropInfoTemplet>.Find(key);
	}

	private NKCItemDropInfoTemplet(IGrouping<string, ItemDropInfo> group, int index)
	{
		itemId = group.Key;
		m_DropInfoList = group.ToList();
		this.index = index;
	}

	public static void LoadFromLua()
	{
		int index = 0;
		NKMTempletContainer<NKCItemDropInfoTemplet>.AddRange(from e in NKMTempletLoader.LoadCommonPath("AB_SCRIPT", "LUA_ITEM_DROP_LIST", "ItemDropList", ItemDropInfo.LoadFromLUA)
			group e by e.ItemID into e
			select new NKCItemDropInfoTemplet(e, index++), (NKCItemDropInfoTemplet e) => e.itemId);
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
