using System.Collections.Generic;
using System.Linq;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMTrimTemplet : INKMTemplet
{
	public const int TrimDungeonCount = 3;

	public int TrimId;

	private string m_OpenTag;

	public string TirmGroupName;

	public string TirmGroupNameKOR;

	public string TirmGroupDesc;

	public string TirmGroupDescKOR;

	public int[] TrimDungeonIds = new int[3];

	public string TrimGroupCombatPenalty;

	public string TrimGroupBGPrefab;

	public string TrimBGColor;

	public bool ShowInterval;

	public int MaxTrimLevel;

	public int TrimPointGroup;

	public bool m_bActiveBattleSkip;

	public UnlockInfo m_UnlockInfo;

	public int m_StageReqItemID;

	public int m_StageReqItemCount;

	public readonly Dictionary<int, List<NKMTrimDungeonTemplet>> TrimDungeonTemplets = new Dictionary<int, List<NKMTrimDungeonTemplet>>();

	public List<NKMTrimCombatPenaltyTemplet> TrimCombatPenaltyList;

	public int Key => TrimId;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public static IEnumerable<NKMTrimTemplet> Values => NKMTempletContainer<NKMTrimTemplet>.Values;

	public static NKMTrimTemplet Find(int key)
	{
		return NKMTempletContainer<NKMTrimTemplet>.Find((NKMTrimTemplet x) => x.Key == key);
	}

	public static NKMTrimTemplet LoadFromLua(NKMLua cNKMLua)
	{
		NKMTrimTemplet nKMTrimTemplet = new NKMTrimTemplet();
		bool flag = true;
		flag &= cNKMLua.GetData("TrimID", ref nKMTrimTemplet.TrimId);
		for (int i = 0; i < 3; i++)
		{
			flag &= cNKMLua.GetData($"TrimDungeonID_{i + 1}", ref nKMTrimTemplet.TrimDungeonIds[i]);
		}
		flag &= cNKMLua.GetData("TrimPointGroup", ref nKMTrimTemplet.TrimPointGroup);
		flag &= cNKMLua.GetData("m_bActiveBattleSkip", ref nKMTrimTemplet.m_bActiveBattleSkip);
		flag &= cNKMLua.GetData("m_StageReqItemID", ref nKMTrimTemplet.m_StageReqItemID);
		flag &= cNKMLua.GetData("m_StageReqItemCount", ref nKMTrimTemplet.m_StageReqItemCount);
		cNKMLua.GetData("m_OpenTag", ref nKMTrimTemplet.m_OpenTag);
		cNKMLua.GetData("TrimGroupName", ref nKMTrimTemplet.TirmGroupName);
		cNKMLua.GetData("TrimGroupName_KOR", ref nKMTrimTemplet.TirmGroupNameKOR);
		cNKMLua.GetData("TrimGroupDesc", ref nKMTrimTemplet.TirmGroupDesc);
		cNKMLua.GetData("TrimGroupDesc_KOR", ref nKMTrimTemplet.TirmGroupDescKOR);
		cNKMLua.GetData("TrimGroupBGPrefab", ref nKMTrimTemplet.TrimGroupBGPrefab);
		cNKMLua.GetData("TrimBGColor", ref nKMTrimTemplet.TrimBGColor);
		cNKMLua.GetData("m_bShowInterval", ref nKMTrimTemplet.ShowInterval);
		nKMTrimTemplet.m_UnlockInfo = UnlockInfo.LoadFromLua(cNKMLua, nullable: false);
		if (!flag)
		{
			return null;
		}
		return nKMTrimTemplet;
	}

	public void Join()
	{
		int i = 0;
		while (i < 3)
		{
			TrimDungeonTemplets.Add(TrimDungeonIds[i], NKMTrimDungeonTemplet.TrimDungeonList.Where((NKMTrimDungeonTemplet e) => e.TrimDungeonId == TrimDungeonIds[i]).ToList());
			int num = i + 1;
			i = num;
		}
	}

	public void Validate()
	{
		foreach (KeyValuePair<int, List<NKMTrimDungeonTemplet>> trimDungeonTemplet in TrimDungeonTemplets)
		{
			if (trimDungeonTemplet.Value.Count == 0)
			{
				NKMTempletError.Add($"[NKMTrimTemplet] Trim 던전이 존재하지 않음 Trim ID :{TrimId} TrimDungeon ID : {trimDungeonTemplet.Key}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMTrimTemplet.cs", 81);
			}
			foreach (NKMTrimDungeonTemplet item in trimDungeonTemplet.Value)
			{
				MaxTrimLevel = ((MaxTrimLevel > item.TrimLevelHigh) ? MaxTrimLevel : item.TrimLevelHigh);
			}
		}
		foreach (KeyValuePair<int, List<NKMTrimDungeonTemplet>> trimDungeonTemplet2 in TrimDungeonTemplets)
		{
			int num = 1;
			foreach (NKMTrimDungeonTemplet item2 in trimDungeonTemplet2.Value)
			{
				if (item2.IsValidateLevel(num))
				{
					num = item2.TrimLevelHigh + 1;
					continue;
				}
				num = item2.TrimLevelHigh;
				NKMTempletError.Add($"[NKMTrimTemplet] TrimDungeon 레벨 범위가 벗어남 TrimDungeonId :{item2.TrimId}, High Level : {num}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMTrimTemplet.cs", 101);
			}
			TrimCombatPenaltyList = (from e in NKMTrimCombatPenaltyTemplet.PenaltyTempletList
				where e.TrimId == TrimId
				orderby e.LowCombatRate
				select e).ToList();
			if (TrimCombatPenaltyList.Count() == 0)
			{
				NKMTempletError.Add($"[NKMTrimTemplet] Trim에 Combat 패널티가 존재하지 않음 :{TrimId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMTrimTemplet.cs", 110);
			}
		}
	}

	public NKMTrimDungeonTemplet GetTrimDungeonTempletByDungeonID(int dungeonID, int trimLevel)
	{
		foreach (KeyValuePair<int, List<NKMTrimDungeonTemplet>> trimDungeonTemplet in TrimDungeonTemplets)
		{
			List<NKMTrimDungeonTemplet> value = trimDungeonTemplet.Value;
			if (value != null)
			{
				NKMTrimDungeonTemplet nKMTrimDungeonTemplet = value.Find((NKMTrimDungeonTemplet e) => e.DungeonId == dungeonID && e.TrimLevelLow <= trimLevel && e.TrimLevelHigh >= trimLevel);
				if (nKMTrimDungeonTemplet != null)
				{
					return nKMTrimDungeonTemplet;
				}
			}
		}
		return null;
	}

	public NKMTrimDungeonTemplet GetTrimDungeonTemplet(int trimDungeonID, int trimLevel)
	{
		if (!TrimDungeonTemplets.TryGetValue(trimDungeonID, out var value))
		{
			return null;
		}
		return value?.Find((NKMTrimDungeonTemplet e) => e.TrimLevelLow <= trimLevel && e.TrimLevelHigh >= trimLevel);
	}
}
