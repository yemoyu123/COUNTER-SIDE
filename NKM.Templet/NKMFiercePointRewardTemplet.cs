using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMFiercePointRewardTemplet : INKMTemplet
{
	private static Dictionary<int, List<NKMFiercePointRewardTemplet>> PointRewardGroups;

	public int FiercePointRewardID;

	public int FiercePointRewardGroupID;

	public int Step;

	public string PointDescStrID;

	public int Point;

	public List<NKM_REWARD_DATA> Rewards = new List<NKM_REWARD_DATA>();

	public int Key => FiercePointRewardID;

	public static IEnumerable<NKMFiercePointRewardTemplet> Values => NKMTempletContainer<NKMFiercePointRewardTemplet>.Values;

	public static IReadOnlyDictionary<int, List<NKMFiercePointRewardTemplet>> Groups => PointRewardGroups;

	public static NKMFiercePointRewardTemplet Find(int fiercePointRewardId)
	{
		return NKMTempletContainer<NKMFiercePointRewardTemplet>.Find(fiercePointRewardId);
	}

	public static NKMFiercePointRewardTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFiercePointRewardTemplet.cs", 29))
		{
			return null;
		}
		NKMFiercePointRewardTemplet nKMFiercePointRewardTemplet = new NKMFiercePointRewardTemplet();
		bool flag = true;
		flag &= lua.GetData("FiercePointRewardID", ref nKMFiercePointRewardTemplet.FiercePointRewardID);
		flag &= lua.GetData("FiercePointRewardGroupID", ref nKMFiercePointRewardTemplet.FiercePointRewardGroupID);
		flag &= lua.GetData("Step", ref nKMFiercePointRewardTemplet.Step);
		flag &= lua.GetData("PointDescStrID", ref nKMFiercePointRewardTemplet.PointDescStrID);
		flag &= lua.GetData("Point", ref nKMFiercePointRewardTemplet.Point);
		for (int i = 1; i <= 3; i++)
		{
			NKM_REWARD_DATA nKM_REWARD_DATA = new NKM_REWARD_DATA();
			flag &= lua.GetData($"PointRewardType_{i}", ref nKM_REWARD_DATA.RewardType);
			flag &= lua.GetData($"PointRewardID_{i}", ref nKM_REWARD_DATA.RewardID);
			flag &= lua.GetData($"PointRewardQuantity_{i}", ref nKM_REWARD_DATA.RewardQuantity);
			if (nKM_REWARD_DATA.RewardID != 0)
			{
				nKMFiercePointRewardTemplet.Rewards.Add(nKM_REWARD_DATA);
			}
		}
		if (!flag)
		{
			Log.ErrorAndExit($"[FiercePointRewardTemplet] data is invalid, fiercePointRewardId: {nKMFiercePointRewardTemplet.FiercePointRewardID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFiercePointRewardTemplet.cs", 56);
			return null;
		}
		return nKMFiercePointRewardTemplet;
	}

	public void Join()
	{
		PointRewardGroups = (from e in Values
			group e by e.FiercePointRewardGroupID).ToDictionary((IGrouping<int, NKMFiercePointRewardTemplet> e) => e.Key, (IGrouping<int, NKMFiercePointRewardTemplet> e) => e.ToList());
	}

	public void Validate()
	{
	}
}
