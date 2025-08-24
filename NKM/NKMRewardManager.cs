using System.Collections.Generic;
using NKM.Templet;

namespace NKM;

public static class NKMRewardManager
{
	private static Dictionary<int, NKMRewardGroupTemplet> m_RewardData = new Dictionary<int, NKMRewardGroupTemplet>();

	public static NKMRewardGroupTemplet GetRewardGroup(int groupID)
	{
		m_RewardData.TryGetValue(groupID, out var value);
		return value;
	}

	public static bool ContainsKey(int groupId)
	{
		return m_RewardData.ContainsKey(groupId);
	}

	public static bool LoadFromLUA(string fileName)
	{
		using (NKMLua nKMLua = new NKMLua())
		{
			if (!nKMLua.LoadCommonPath("AB_SCRIPT", fileName) || !nKMLua.OpenTable("REWARD_TEMPLET"))
			{
				return false;
			}
			int num = 1;
			while (nKMLua.OpenTable(num))
			{
				if (NKMContentsVersionManager.CheckContentsVersion(nKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMRewardManager.cs", 35))
				{
					int rValue = 0;
					if (!nKMLua.GetData("m_RewardGroupID", ref rValue) || rValue <= 0)
					{
						num++;
						nKMLua.CloseTable();
						continue;
					}
					if (!m_RewardData.ContainsKey(rValue))
					{
						NKMRewardGroupTemplet value = new NKMRewardGroupTemplet(rValue);
						m_RewardData.Add(rValue, value);
					}
					NKMRewardTemplet nKMRewardTemplet = new NKMRewardTemplet();
					nKMRewardTemplet.LoadFromLUA(nKMLua);
					nKMRewardTemplet.m_Ratio = 1;
					nKMRewardTemplet.Validate();
					m_RewardData[rValue].Add(nKMRewardTemplet);
				}
				num++;
				nKMLua.CloseTable();
			}
			nKMLua.CloseTable();
		}
		return true;
	}

	public static void InvokeJoin()
	{
		foreach (NKMRewardGroupTemplet value in m_RewardData.Values)
		{
			foreach (NKMRewardTemplet item in value.List)
			{
				item.Join();
			}
		}
	}

	public static void Clear()
	{
		m_RewardData.Clear();
	}
}
