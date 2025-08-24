using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet;

namespace NKM;

public class NKMRandomGradeManager
{
	public static Dictionary<int, Dictionary<int, RatioData>> m_dicRandomGrade = new Dictionary<int, Dictionary<int, RatioData>>();

	public static Dictionary<int, NKMRandomGradeTemplet> m_dicRandomGradeByID = new Dictionary<int, NKMRandomGradeTemplet>();

	public static Dictionary<string, NKMRandomGradeTemplet> m_dicRandomGradeByStrID = new Dictionary<string, NKMRandomGradeTemplet>();

	public static Dictionary<string, NKMRandomGradeTemplet> Get_dicRandomGradeByStrID()
	{
		return m_dicRandomGradeByStrID;
	}

	public static bool LoadFromLUA(string fileName)
	{
		NKMLua nKMLua = new NKMLua();
		if (nKMLua.LoadCommonPath("AB_SCRIPT", fileName) && nKMLua.OpenTable("RANDOM_GRADE"))
		{
			int num = 1;
			while (nKMLua.OpenTable(num))
			{
				NKMRandomGradeTemplet nKMRandomGradeTemplet = new NKMRandomGradeTemplet();
				nKMRandomGradeTemplet.LoadFromLUA(nKMLua);
				if (m_dicRandomGradeByID.ContainsKey(nKMRandomGradeTemplet.m_RandomGradeID))
				{
					m_dicRandomGradeByID[nKMRandomGradeTemplet.m_RandomGradeID].MergeData(nKMRandomGradeTemplet.m_iMaxSalaryLevel, nKMRandomGradeTemplet.GetLastData());
				}
				else
				{
					m_dicRandomGradeByID.Add(nKMRandomGradeTemplet.m_RandomGradeID, nKMRandomGradeTemplet);
				}
				if (m_dicRandomGradeByStrID.ContainsKey(nKMRandomGradeTemplet.m_RandomGradeStrID))
				{
					m_dicRandomGradeByStrID[nKMRandomGradeTemplet.m_RandomGradeStrID].MergeData(nKMRandomGradeTemplet.m_iMaxSalaryLevel, nKMRandomGradeTemplet.GetLastData());
				}
				else
				{
					m_dicRandomGradeByStrID.Add(nKMRandomGradeTemplet.m_RandomGradeStrID, nKMRandomGradeTemplet);
				}
				num++;
				nKMLua.CloseTable();
			}
			nKMLua.CloseTable();
		}
		nKMLua.LuaClose();
		return true;
	}

	public static NKMRandomGradeTemplet GetRandomGradeTemplet(int randomGradeID)
	{
		if (!m_dicRandomGradeByID.ContainsKey(randomGradeID))
		{
			Log.Error($"GetRandomGrade m_dicRandomGradeByID InvalidID! randomGradeID: [{randomGradeID}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMRandomGradeManager.cs", 159);
			return null;
		}
		NKMRandomGradeTemplet nKMRandomGradeTemplet = m_dicRandomGradeByID[randomGradeID];
		if (nKMRandomGradeTemplet == null)
		{
			Log.Error($"GetRandomGrade m_dicRandomGradeByID InvalidID! randomGradeID: [{randomGradeID}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMRandomGradeManager.cs", 166);
			return null;
		}
		return nKMRandomGradeTemplet;
	}

	public static NKMRandomGradeTemplet GetRandomGradeTemplet(string randomGradeStrID)
	{
		if (!m_dicRandomGradeByStrID.ContainsKey(randomGradeStrID))
		{
			Log.Error("GetRandomGrade m_dicRandomGradeByStrID InvalidID! randomGradeStrID: [" + randomGradeStrID + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMRandomGradeManager.cs", 177);
			return null;
		}
		NKMRandomGradeTemplet nKMRandomGradeTemplet = m_dicRandomGradeByStrID[randomGradeStrID];
		if (nKMRandomGradeTemplet == null)
		{
			Log.Error("GetRandomGrade m_dicRandomStarGradeByStrID InvalidID! randomGradeStrID: [" + randomGradeStrID + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMRandomGradeManager.cs", 184);
			return null;
		}
		return nKMRandomGradeTemplet;
	}

	public static NKM_UNIT_GRADE GetRandomGrade(int randomGradeID)
	{
		return GetRandomGradeTemplet(randomGradeID)?.GetRandomGrade() ?? NKM_UNIT_GRADE.NUG_N;
	}

	public static NKM_UNIT_GRADE GetRandomGrade(string randomGradeStrID)
	{
		NKMRandomGradeTemplet randomGradeTemplet = GetRandomGradeTemplet(randomGradeStrID);
		if (randomGradeTemplet == null)
		{
			Log.Error("GetRandomGrade m_dicRandomGradeByStrID InvalidID! randomGradeStrID: [" + randomGradeStrID + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMRandomGradeManager.cs", 207);
			return NKM_UNIT_GRADE.NUG_N;
		}
		return randomGradeTemplet.GetRandomGrade();
	}
}
