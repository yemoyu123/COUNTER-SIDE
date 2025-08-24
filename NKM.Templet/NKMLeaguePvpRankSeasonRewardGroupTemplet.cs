using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMLeaguePvpRankSeasonRewardGroupTemplet : INKMTemplet
{
	private readonly List<NKMLeaguePvpRankSeasonRewardTemplet> list = new List<NKMLeaguePvpRankSeasonRewardTemplet>();

	private int groupId;

	public int Key => groupId;

	public NKMLeaguePvpRankSeasonRewardGroupTemplet(int groupId, List<NKMLeaguePvpRankSeasonRewardTemplet> list)
	{
		if (list == null || list.Count == 0)
		{
			Log.ErrorAndExit($"invalid list data. GroupID : {groupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankSeasonRewardGroupTemplet.cs", 17);
			return;
		}
		this.groupId = groupId;
		this.list.AddRange(list);
	}

	public static NKMLeaguePvpRankSeasonRewardGroupTemplet Find(int key)
	{
		return NKMTempletContainer<NKMLeaguePvpRankSeasonRewardGroupTemplet>.Find((NKMLeaguePvpRankSeasonRewardGroupTemplet e) => e.groupId == key);
	}

	public void Join()
	{
		list.OrderBy((NKMLeaguePvpRankSeasonRewardTemplet e) => e.MinRank);
	}

	public void Validate()
	{
		int num = list.Max((NKMLeaguePvpRankSeasonRewardTemplet e) => e.MaxRank);
		int num2 = list.Select((NKMLeaguePvpRankSeasonRewardTemplet e) => e.MaxRank - e.MinRank + 1).Sum();
		if (num != num2)
		{
			NKMTempletError.Add($"[NKMLeaguePvpRankSeasonRewardGroupTemplet:{Key}] 보상 범위가 잘못설정 되어있음.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankSeasonRewardGroupTemplet.cs", 40);
		}
	}

	public NKMLeaguePvpRankSeasonRewardTemplet GetRewardTemplet(long rank)
	{
		if (rank == 0L)
		{
			return null;
		}
		foreach (NKMLeaguePvpRankSeasonRewardTemplet item in list)
		{
			if (item.MinRank <= rank && item.MaxRank >= rank)
			{
				return item;
			}
		}
		return null;
	}

	public List<NKMLeaguePvpRankSeasonRewardTemplet> GetRewardTempletList()
	{
		return list;
	}
}
