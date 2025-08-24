using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMLeaguePvpRankGroupTemplet : INKMTemplet
{
	private readonly List<NKMLeaguePvpRankTemplet> list = new List<NKMLeaguePvpRankTemplet>();

	public int Key { get; }

	public IReadOnlyList<NKMLeaguePvpRankTemplet> List => list;

	public IReadOnlyList<NKMLeaguePvpRankTemplet> ScoreAdjustTargets => list.Where((NKMLeaguePvpRankTemplet e) => e.ScoreRelegation > 0).ToList();

	public bool EnableScoreAdjust => list.Sum((NKMLeaguePvpRankTemplet e) => e.ScoreRelegation) > 0;

	private NKMLeaguePvpRankGroupTemplet(int groupId)
	{
		Key = groupId;
	}

	public static void LoadFile()
	{
		string bundleName = "AB_SCRIPT";
		string text = "LUA_PVP_LEAGUE";
		string text2 = "PVP_LEAGUE";
		Dictionary<int, NKMLeaguePvpRankGroupTemplet> dictionary = new Dictionary<int, NKMLeaguePvpRankGroupTemplet>();
		using (NKMLua nKMLua = new NKMLua())
		{
			if (!nKMLua.LoadCommonPath(bundleName, text) || !nKMLua.OpenTable(text2))
			{
				Log.ErrorAndExit("[LeaguePvpRank] loading file failed. fileName:" + text + " tablName:" + text2, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankGroupTemplet.cs", 35);
			}
			int num = 1;
			while (nKMLua.OpenTable(num))
			{
				if (!NKMContentsVersionManager.CheckContentsVersion(nKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankGroupTemplet.cs", 41))
				{
					num++;
					nKMLua.CloseTable();
					continue;
				}
				if (!nKMLua.GetData("m_RankGroup", out var rValue, 0))
				{
					Log.ErrorAndExit($"[LeaguePvpRank] loading key failed. groupId:{rValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankGroupTemplet.cs", 50);
				}
				if (!dictionary.TryGetValue(rValue, out var value))
				{
					value = new NKMLeaguePvpRankGroupTemplet(rValue);
					dictionary.Add(rValue, value);
				}
				NKMLeaguePvpRankTemplet nKMLeaguePvpRankTemplet = NKMLeaguePvpRankTemplet.LoadFromLUA(nKMLua, rValue);
				if (nKMLeaguePvpRankTemplet == null)
				{
					Log.ErrorAndExit($"[LeaguePvpRank] loading failed. groupId:{rValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankGroupTemplet.cs", 62);
				}
				value.list.Add(nKMLeaguePvpRankTemplet);
				num++;
				nKMLua.CloseTable();
			}
		}
		NKMTempletContainer<NKMLeaguePvpRankGroupTemplet>.Load(dictionary.Values, null);
	}

	public static NKMLeaguePvpRankGroupTemplet Find(int key)
	{
		return NKMTempletContainer<NKMLeaguePvpRankGroupTemplet>.Find(key);
	}

	public void Join()
	{
		list.Sort((NKMLeaguePvpRankTemplet a, NKMLeaguePvpRankTemplet b) => a.LeagueTier.CompareTo(b.LeagueTier));
		if (list[0].LeaguePointReq != 0)
		{
			NKMTempletError.Add($"[LeaguePvpRank:{Key}] RankGroup의 입문 승점은 0으로 시작해야 함 leaguePointReq:{list[0].LeaguePointReq}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankGroupTemplet.cs", 82);
		}
		for (int num = 0; num < list.Count; num++)
		{
			NKMLeaguePvpRankTemplet nKMLeaguePvpRankTemplet = list[num];
			nKMLeaguePvpRankTemplet.Join();
			if (num > 0 && list[num - 1].LeaguePointReq >= nKMLeaguePvpRankTemplet.LeaguePointReq)
			{
				NKMTempletError.Add($"[LeaguePvpRank:{Key}] 티어가 높아지는데 입문 승점이 오르지 않음. tier:{nKMLeaguePvpRankTemplet.LeagueTier} 이전티어점수:{list[num - 1].LeaguePointReq} 현재티어점수:{nKMLeaguePvpRankTemplet.LeaguePointReq}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankGroupTemplet.cs", 92);
			}
			int num2 = num + 1;
			if (nKMLeaguePvpRankTemplet.LeagueTier != num2)
			{
				NKMTempletError.Add($"[LeaguePvpRank:{Key}] tier값이 없음. index:{num} expected:{num2} actual:{nKMLeaguePvpRankTemplet.LeagueTier}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankGroupTemplet.cs", 98);
			}
		}
	}

	public void Validate()
	{
		foreach (NKMLeaguePvpRankTemplet item in list)
		{
			item.Validate();
		}
	}

	public NKMLeaguePvpRankTemplet GetByTier(int tierId)
	{
		int num = Math.Max(tierId - 1, 0);
		if (num >= list.Count)
		{
			return list[list.Count - 1];
		}
		return list[num];
	}

	public NKMLeaguePvpRankTemplet GetByScore(int score)
	{
		for (int i = 1; i < list.Count; i++)
		{
			if (list[i].LeaguePointReq > score)
			{
				return list[i - 1];
			}
		}
		return list[list.Count - 1];
	}
}
