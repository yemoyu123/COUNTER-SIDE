using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Core.Util;
using Cs.Logging;
using NKC;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMLeaguePvpRankSeasonTemplet : INKMTemplet, INKMTempletEx
{
	private int seasonId;

	private string seasonStrId;

	private int dungeonId;

	private string seasonIcon;

	private string seasonLobbyName;

	private int rankGroupId;

	private DateTime rankGroupDateEnd;

	private bool isSeasonLobbyPrefab;

	private string seasonDateStrId;

	private string rankGroupDateStrId;

	private string seasonBattleCondition;

	private int rankSeasonRewardGroup;

	private string openTag;

	private string seasonRule;

	private NKM_GAME_TYPE gameType;

	private List<int> missionIdList = new List<int>();

	private bool unitMaxLevel;

	private bool forcedBanEquip;

	private readonly List<string> battleConditionIds = new List<string>();

	public List<NKMBattleConditionTemplet> BattleConditionTemplets = new List<NKMBattleConditionTemplet>();

	public static IEnumerable<NKMLeaguePvpRankSeasonTemplet> Values => NKMTempletContainer<NKMLeaguePvpRankSeasonTemplet>.Values;

	public int Key => seasonId;

	public int SeasonId => seasonId;

	public string SeasonStrId => seasonStrId;

	public string Name => $"[{seasonId}] {SeasonStrId}";

	public string SeasonLobbyName => seasonLobbyName;

	public bool IsSeasonLobbyPrefab => isSeasonLobbyPrefab;

	public string SeasonIcon => seasonIcon;

	public int RankSeasonRewardGroup => rankSeasonRewardGroup;

	public bool UnitMaxLevel => unitMaxLevel;

	public bool ForceBanEquip => forcedBanEquip;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(openTag);

	public bool HasRankReward => rankSeasonRewardGroup > 0;

	public DateTime RankGroupDateEnd => rankGroupDateEnd;

	public NKM_GAME_TYPE GameType => gameType;

	public int DungeonId => dungeonId;

	public List<int> MissionIdList => missionIdList;

	public NKMIntervalTemplet Interval { get; private set; }

	public NKMIntervalTemplet SeasonRewardInterval { get; private set; }

	public NKMLeaguePvpRankGroupTemplet RankGroup { get; private set; }

	public NKMBattleConditionTemplet BattleConditiaon { get; private set; }

	public DateTime StartDateUTC => ServiceTime.ToUtcTime(Interval.StartDate);

	public DateTime EndDateUTC => ServiceTime.ToUtcTime(Interval.EndDate);

	public DateTime RankGroupDateEndUTC => ServiceTime.ToUtcTime(rankGroupDateEnd);

	public static NKMLeaguePvpRankSeasonTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankSeasonTemplet.cs", 61))
		{
			return null;
		}
		NKMLeaguePvpRankSeasonTemplet nKMLeaguePvpRankSeasonTemplet = new NKMLeaguePvpRankSeasonTemplet();
		int num = (int)(1u & (lua.GetData("m_Season", ref nKMLeaguePvpRankSeasonTemplet.seasonId) ? 1u : 0u)) & (lua.GetData("m_SeasonName", ref nKMLeaguePvpRankSeasonTemplet.seasonStrId) ? 1 : 0);
		lua.GetData("m_SeasonIcon", ref nKMLeaguePvpRankSeasonTemplet.seasonIcon);
		int num2 = (int)((uint)num & (lua.GetData("m_SeasonLobbyName", ref nKMLeaguePvpRankSeasonTemplet.seasonLobbyName) ? 1u : 0u) & (lua.GetData("m_RankGroup", ref nKMLeaguePvpRankSeasonTemplet.rankGroupId) ? 1u : 0u) & (lua.GetData("m_bSeasonLobbyPrefab", ref nKMLeaguePvpRankSeasonTemplet.isSeasonLobbyPrefab) ? 1u : 0u) & (lua.GetData("m_SeasonDateStrID", ref nKMLeaguePvpRankSeasonTemplet.seasonDateStrId) ? 1u : 0u) & (lua.GetData("OpenTag", ref nKMLeaguePvpRankSeasonTemplet.openTag) ? 1u : 0u) & (lua.GetData("m_RankGroupDateStrID", ref nKMLeaguePvpRankSeasonTemplet.rankGroupDateStrId) ? 1u : 0u) & (lua.GetData("GameType", ref nKMLeaguePvpRankSeasonTemplet.gameType) ? 1u : 0u) & (lua.GetData("UnitMaxLevel", ref nKMLeaguePvpRankSeasonTemplet.unitMaxLevel) ? 1u : 0u)) & (lua.GetData("bForcedBanEquip", ref nKMLeaguePvpRankSeasonTemplet.forcedBanEquip) ? 1 : 0);
		lua.GetData("SeasonRule", ref nKMLeaguePvpRankSeasonTemplet.seasonRule);
		lua.GetData("m_SeasonBattleCondition", ref nKMLeaguePvpRankSeasonTemplet.seasonBattleCondition);
		lua.GetData("m_RankSeasonRewardGroup", ref nKMLeaguePvpRankSeasonTemplet.rankSeasonRewardGroup);
		lua.GetDataList("LeagueMissionID", out nKMLeaguePvpRankSeasonTemplet.missionIdList, nullIfEmpty: false);
		lua.GetDataList("BattleConditionID", out List<string> result, nullIfEmpty: false);
		lua.GetData("DungeonId", ref nKMLeaguePvpRankSeasonTemplet.dungeonId);
		if (result != null)
		{
			nKMLeaguePvpRankSeasonTemplet.battleConditionIds.AddRange(result);
		}
		if (num2 == 0)
		{
			Log.Error($"NKMPvpRankSeasonTemplet Load Fail - {nKMLeaguePvpRankSeasonTemplet.seasonId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankSeasonTemplet.cs", 96);
			return null;
		}
		return nKMLeaguePvpRankSeasonTemplet;
	}

	public static NKMLeaguePvpRankSeasonTemplet Find(int key)
	{
		return NKMTempletContainer<NKMLeaguePvpRankSeasonTemplet>.Find(key);
	}

	public static NKMLeaguePvpRankSeasonTemplet Find(DateTime currentSvc)
	{
		return Values.FirstOrDefault((NKMLeaguePvpRankSeasonTemplet e) => e.SeasonEnable(currentSvc));
	}

	public void Join()
	{
		if (NKMUtil.IsServer)
		{
			JoinIntervalTemplet();
		}
		RankGroup = NKMLeaguePvpRankGroupTemplet.Find(rankGroupId);
		if (RankGroup == null)
		{
			NKMTempletError.Add($"[LeaguePvpSeason:{Name}] invalid groupId:{rankGroupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankSeasonTemplet.cs", 117);
		}
		if (!string.IsNullOrEmpty(seasonBattleCondition))
		{
			BattleConditiaon = NKMBattleConditionManager.GetTempletByStrID(seasonBattleCondition);
			if (BattleConditiaon == null)
			{
				NKMTempletError.Add("[LeaguePvpSeason:" + Name + "] invalid battleConditionId:" + seasonBattleCondition, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankSeasonTemplet.cs", 125);
			}
		}
		foreach (string battleConditionId in battleConditionIds)
		{
			NKMBattleConditionTemplet templetByStrID = NKMBattleConditionManager.GetTempletByStrID(battleConditionId);
			if (templetByStrID == null)
			{
				NKMTempletError.Add("[LeaguePvpSeason:" + Name + "] invalid battleConditionId:" + battleConditionId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankSeasonTemplet.cs", 134);
			}
			else
			{
				BattleConditionTemplets.Add(templetByStrID);
			}
		}
	}

	public void JoinIntervalTemplet()
	{
		Interval = NKMIntervalTemplet.Find(seasonDateStrId);
		if (Interval == null)
		{
			Interval = NKMIntervalTemplet.Invalid;
			NKMTempletError.Add("[LeaguePvpSeasonTemplet:" + Name + "] invalid seasonDateStrId:" + seasonDateStrId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankSeasonTemplet.cs", 148);
		}
		NKMIntervalTemplet nKMIntervalTemplet = NKMIntervalTemplet.Find(rankGroupDateStrId);
		if (nKMIntervalTemplet == null)
		{
			NKMTempletError.Add("[PvpSeasonTemplet:" + Name + "] invalid rankGroupDateStrId:" + rankGroupDateStrId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankSeasonTemplet.cs", 154);
			return;
		}
		if (nKMIntervalTemplet.IsRepeatDate)
		{
			NKMTempletError.Add("[PvpSeasonTemplet:" + Name + "] 반복 기간설정 사용 불가. rankGroupDateStrId:" + rankGroupDateStrId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankSeasonTemplet.cs", 160);
		}
		rankGroupDateEnd = nKMIntervalTemplet.EndDate;
		SeasonRewardInterval = nKMIntervalTemplet;
	}

	public void Validate()
	{
		if (Interval != null && Interval.IsRepeatDate)
		{
			NKMTempletError.Add("[PvpSeasonTemplet:" + Name + "] 반복 기간설정 사용 불가. rankGroupDateStrId:" + rankGroupDateStrId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankSeasonTemplet.cs", 175);
		}
		if (RankSeasonRewardGroup > 0 && NKMLeaguePvpRankSeasonRewardGroupTemplet.Find(RankSeasonRewardGroup) == null)
		{
			NKMTempletError.Add($"[PvpSeasonTemplet:{Name}] 랭크 보상 데이터가 존재하지 않음. RankSeasonRewardGroup:{RankSeasonRewardGroup}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankSeasonTemplet.cs", 184);
		}
		if (dungeonId > 0 && NKMDungeonManager.GetDungeonTemplet(DungeonId) == null)
		{
			NKMTempletError.Add($"[PvpSeasonTemplet:{Name}] 던전 템플릿 정보가 존재하지 않음. DungeonId:{dungeonId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankSeasonTemplet.cs", 193);
		}
		NKM_GAME_TYPE nKM_GAME_TYPE = gameType;
		if (nKM_GAME_TYPE != NKM_GAME_TYPE.NGT_PVP_LEAGUE && nKM_GAME_TYPE != NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
		{
			NKMTempletError.Add($"[PvpSeasonTemplet:{Name}] 게임 타입이 올바르지 않음. GameType:{GameType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankSeasonTemplet.cs", 203);
		}
	}

	public bool SeasonGameEnable(DateTime dateTime)
	{
		if (!EnableByTag)
		{
			return false;
		}
		if (Interval.IsValidTime(dateTime))
		{
			return true;
		}
		return false;
	}

	public bool SeasonRewardEnable(DateTime dateTime)
	{
		if (!EnableByTag)
		{
			return false;
		}
		if (SeasonRewardInterval.IsValidTime(dateTime))
		{
			return true;
		}
		return false;
	}

	public bool SeasonEnable(DateTime dateTime)
	{
		if (!EnableByTag)
		{
			return false;
		}
		if (SeasonGameEnable(dateTime) || SeasonRewardEnable(dateTime))
		{
			return true;
		}
		return false;
	}

	public void PostJoin()
	{
		JoinIntervalTemplet();
	}

	public bool CheckMySeason(DateTime nowUTC)
	{
		if (StartDateUTC <= nowUTC && nowUTC <= EndDateUTC)
		{
			return true;
		}
		return false;
	}

	public bool CheckSeasonForRank(DateTime nowUTC)
	{
		if (StartDateUTC <= nowUTC && nowUTC <= RankGroupDateEndUTC)
		{
			return true;
		}
		return false;
	}

	public string GetSeasonStrId()
	{
		return NKCStringTable.GetString(SeasonStrId);
	}

	public string GetSeasonRule()
	{
		return NKCStringTable.GetString(seasonRule);
	}
}
