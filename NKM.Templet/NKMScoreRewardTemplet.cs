using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMScoreRewardTemplet : INKMTemplet
{
	private static Dictionary<int, List<NKMScoreRewardTemplet>> ScoreRewardGroups = new Dictionary<int, List<NKMScoreRewardTemplet>>();

	public int m_ScoreRewardGroupID;

	public int m_ScoreRewardID;

	public int m_Step;

	public string m_ScoreDescStrID;

	public int m_Score;

	public List<NKMRewardInfo> m_ScoreReward = new List<NKMRewardInfo>();

	public int Key => m_ScoreRewardID;

	public static IEnumerable<NKMScoreRewardTemplet> Values => NKMTempletContainer<NKMScoreRewardTemplet>.Values;

	public static IReadOnlyDictionary<int, List<NKMScoreRewardTemplet>> Groups => ScoreRewardGroups;

	public IReadOnlyList<NKMRewardInfo> Rewards => m_ScoreReward;

	public static NKMScoreRewardTemplet Find(int key)
	{
		return NKMTempletContainer<NKMScoreRewardTemplet>.Values.Where((NKMScoreRewardTemplet e) => e.Key == key).FirstOrDefault();
	}

	public static NKMScoreRewardTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		NKMScoreRewardTemplet nKMScoreRewardTemplet = new NKMScoreRewardTemplet();
		bool flag = true;
		flag &= cNKMLua.GetData("m_ScoreRewardGroupID", ref nKMScoreRewardTemplet.m_ScoreRewardGroupID);
		flag &= cNKMLua.GetData("m_ScoreRewardID", ref nKMScoreRewardTemplet.m_ScoreRewardID);
		cNKMLua.GetData("m_Step", ref nKMScoreRewardTemplet.m_Step);
		cNKMLua.GetData("m_ScoreDescStrID", ref nKMScoreRewardTemplet.m_ScoreDescStrID);
		flag &= cNKMLua.GetData("m_Score", ref nKMScoreRewardTemplet.m_Score);
		for (int i = 1; i <= 3; i++)
		{
			NKM_REWARD_TYPE result = NKM_REWARD_TYPE.RT_NONE;
			int rValue = 0;
			int rValue2 = 0;
			cNKMLua.GetData($"m_ScoreRewardType_{i}", ref result);
			cNKMLua.GetData($"m_ScoreRewardID_{i}", ref rValue);
			cNKMLua.GetData($"m_ScoreRewardQuantity_{i}", ref rValue2);
			if (rValue > 0)
			{
				nKMScoreRewardTemplet.m_ScoreReward.Add(new NKMRewardInfo
				{
					rewardType = result,
					ID = rValue,
					Count = rValue2,
					paymentType = NKM_ITEM_PAYMENT_TYPE.NIPT_FREE
				});
			}
		}
		if (!flag)
		{
			return null;
		}
		return nKMScoreRewardTemplet;
	}

	public void Join()
	{
		if (!ScoreRewardGroups.ContainsKey(m_ScoreRewardGroupID))
		{
			ScoreRewardGroups.Add(m_ScoreRewardGroupID, new List<NKMScoreRewardTemplet>());
			if (ScoreRewardGroups[m_ScoreRewardGroupID].Find((NKMScoreRewardTemplet x) => x.m_ScoreRewardID == m_ScoreRewardID) == null)
			{
				ScoreRewardGroups[m_ScoreRewardGroupID].Add(this);
			}
			else
			{
				Log.Error($"NKMScoreRewardTemplet.m_ScoreRewardID is duplecated - {m_ScoreRewardID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMScoreRewardTemplet.cs", 73);
			}
		}
		else if (ScoreRewardGroups[m_ScoreRewardGroupID].Find((NKMScoreRewardTemplet x) => x.m_ScoreRewardID == m_ScoreRewardID) == null)
		{
			ScoreRewardGroups[m_ScoreRewardGroupID].Add(this);
		}
		else
		{
			Log.Error($"NKMScoreRewardTemplet.m_ScoreRewardID is duplecated - {m_ScoreRewardID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMScoreRewardTemplet.cs", 84);
		}
	}

	public void Validate()
	{
		if (m_ScoreReward.Count == 0)
		{
			NKMTempletError.Add($"NKMScoreRewardTemplet(m_ScoreRewardGroupID {m_ScoreRewardGroupID}, m_ScoreRewardID {m_ScoreRewardID}) : \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMScoreRewardTemplet.cs", 93);
		}
		if (m_Score < 0)
		{
			NKMTempletError.Add($"[NKMScoreRewardTemplet:{Key}] \ufffd\ufffd\ufffd\ufffd ȹ\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd. m_ScoreRewardGroupID:{m_ScoreRewardGroupID} m_Score:{m_Score}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMScoreRewardTemplet.cs", 98);
		}
		foreach (NKMRewardInfo item in m_ScoreReward)
		{
			if (!NKMRewardTemplet.IsValidReward(item.rewardType, item.ID))
			{
				NKMTempletError.Add($"[NKMScoreRewardTemplet:{Key}] \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdʽ\ufffd\ufffdϴ\ufffd. m_ScoreRewardGroupID:{m_ScoreRewardGroupID} type:{item.rewardType} id:{item.ID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMScoreRewardTemplet.cs", 106);
			}
			if (item.Count <= 0)
			{
				NKMTempletError.Add($"[NKMScoreRewardTemplet:{Key}] \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd 0 \ufffd\ufffd\ufffd\ufffd. m_ScoreRewardGroupID:{m_ScoreRewardGroupID} type:{item.rewardType} id:{item.ID} count:{item.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMScoreRewardTemplet.cs", 112);
			}
		}
	}
}
