using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKCStatInfoTemplet : INKMTemplet
{
	private static Dictionary<int, List<NKCStatInfoTemplet>> StatInfoGroups;

	public int index;

	public int Category_Order_List;

	public NKM_STAT_TYPE StatType;

	public STAT_TYPE Category_Type;

	public string Stat_Category_Name;

	public string Stat_ID;

	public string Stat_Name;

	public string Stat_Negative_Name;

	public string Stat_Desc;

	public string Stat_Negative_DESC;

	public string Filter_Name;

	public string Filter_Desc;

	public bool UseFilter;

	public bool m_bShowGuide = true;

	public List<string> m_lstSearchKeyword;

	public int Key => index;

	public static IEnumerable<NKCStatInfoTemplet> Values => NKMTempletContainer<NKCStatInfoTemplet>.Values;

	public static IReadOnlyDictionary<int, List<NKCStatInfoTemplet>> Groups => StatInfoGroups;

	public bool HasNegativeName => !string.IsNullOrEmpty(Stat_Negative_Name);

	public static NKCStatInfoTemplet Find(NKM_STAT_TYPE statType)
	{
		return NKMTempletContainer<NKCStatInfoTemplet>.Find((NKCStatInfoTemplet x) => x.StatType == statType);
	}

	public static NKCStatInfoTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCStatInfoTemplet.cs", 50))
		{
			return null;
		}
		NKCStatInfoTemplet nKCStatInfoTemplet = new NKCStatInfoTemplet();
		int num = (int)(1u & (lua.GetData("index", ref nKCStatInfoTemplet.index) ? 1u : 0u) & (lua.GetData("Category_Order_List", ref nKCStatInfoTemplet.Category_Order_List) ? 1u : 0u) & (lua.GetData("Category_Type", ref nKCStatInfoTemplet.Category_Type) ? 1u : 0u) & (lua.GetData("Stat_Category_Name", ref nKCStatInfoTemplet.Stat_Category_Name) ? 1u : 0u)) & (lua.GetData("Stat_ID", ref nKCStatInfoTemplet.Stat_ID) ? 1 : 0);
		nKCStatInfoTemplet.StatType = (NKM_STAT_TYPE)Enum.Parse(typeof(NKM_STAT_TYPE), nKCStatInfoTemplet.Stat_ID);
		int num2 = num & (lua.GetData("Stat_Name", ref nKCStatInfoTemplet.Stat_Name) ? 1 : 0);
		lua.GetData("Stat_Negative_Name", ref nKCStatInfoTemplet.Stat_Negative_Name);
		int num3 = num2 & (lua.GetData("Stat_Desc", ref nKCStatInfoTemplet.Stat_Desc) ? 1 : 0);
		lua.GetData("Stat_Negative_DESC", ref nKCStatInfoTemplet.Stat_Negative_DESC);
		lua.GetData("UseFilter", ref nKCStatInfoTemplet.UseFilter);
		lua.GetData("m_bShowGuide", ref nKCStatInfoTemplet.m_bShowGuide);
		lua.GetData("Filter_Name", ref nKCStatInfoTemplet.Filter_Name);
		lua.GetData("Filter_Desc", ref nKCStatInfoTemplet.Filter_Desc);
		lua.GetDataList("m_lstSearchKeyword", out nKCStatInfoTemplet.m_lstSearchKeyword, nullIfEmpty: true);
		if (num3 == 0)
		{
			Log.ErrorAndExit($"[NKCStatInfoTemplet] data is invalid, Category_Order_List: {nKCStatInfoTemplet.Category_Order_List}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCStatInfoTemplet.cs", 76);
			return null;
		}
		return nKCStatInfoTemplet;
	}

	public static void MakeGroups()
	{
		StatInfoGroups = (from e in Values
			group e by e.Category_Order_List).ToDictionary((IGrouping<int, NKCStatInfoTemplet> e) => e.Key, (IGrouping<int, NKCStatInfoTemplet> e) => e.ToList());
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
