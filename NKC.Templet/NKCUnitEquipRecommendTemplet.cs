using System.Collections.Generic;
using NKM;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCUnitEquipRecommendTemplet : INKMTemplet
{
	public int m_UnitID;

	public List<int> m_lstSetOptionID = new List<int>();

	public Dictionary<int, List<int>> m_dicRecommendList = new Dictionary<int, List<int>>();

	public List<NKM_STAT_TYPE> m_lstRecommendStatType = new List<NKM_STAT_TYPE>();

	private const int COL_COUNT = 3;

	public int Key => m_UnitID;

	public static NKCUnitEquipRecommendTemplet LoadFromLua(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCUnitEquipRecommendTemplet.cs", 22))
		{
			return null;
		}
		NKCUnitEquipRecommendTemplet nKCUnitEquipRecommendTemplet = new NKCUnitEquipRecommendTemplet();
		cNKMLua.GetData("m_UnitID", ref nKCUnitEquipRecommendTemplet.m_UnitID);
		nKCUnitEquipRecommendTemplet.m_dicRecommendList = new Dictionary<int, List<int>>();
		nKCUnitEquipRecommendTemplet.m_lstSetOptionID = new List<int>();
		for (int i = 0; i < 3; i++)
		{
			int rValue = 0;
			List<int> result = new List<int>();
			cNKMLua.GetData($"EquipSetID_{i + 1}", ref rValue);
			cNKMLua.GetDataList($"EquipRecommendGroup_{i + 1}", out result, nullIfEmpty: false);
			if (rValue <= 0)
			{
				break;
			}
			if (!nKCUnitEquipRecommendTemplet.m_lstSetOptionID.Contains(rValue))
			{
				nKCUnitEquipRecommendTemplet.m_lstSetOptionID.Add(rValue);
			}
			if (!nKCUnitEquipRecommendTemplet.m_dicRecommendList.ContainsKey(rValue))
			{
				nKCUnitEquipRecommendTemplet.m_dicRecommendList.Add(rValue, result);
			}
		}
		cNKMLua.GetDataListEnum("EquipRecommendStat", nKCUnitEquipRecommendTemplet.m_lstRecommendStatType);
		if (nKCUnitEquipRecommendTemplet.m_lstSetOptionID.Count > 0)
		{
			nKCUnitEquipRecommendTemplet.m_lstSetOptionID.Sort();
		}
		return nKCUnitEquipRecommendTemplet;
	}

	public static NKCUnitEquipRecommendTemplet Find(int unitID)
	{
		return NKMTempletContainer<NKCUnitEquipRecommendTemplet>.Find(unitID);
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (m_UnitID == 0)
		{
			NKMTempletError.Add("NKCUnitEquipRecommendTemplet Error - m_UnitID == 0", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCUnitEquipRecommendTemplet.cs", 68);
		}
		else if (m_dicRecommendList.Count <= 0)
		{
			NKMTempletError.Add($"NKCUnitEquipRecommendTemplet Error - data in null : {m_UnitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCUnitEquipRecommendTemplet.cs", 74);
		}
	}
}
