using System;
using Cs.Core.Util;
using Cs.Logging;
using NKC;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMPvpRankSeasonTemplet : INKMTemplet, INKMTempletEx
{
	private int seasonID;

	private string seasonStrID;

	private string seasonIcon;

	private string seasonLobbyName;

	private int rankGroup;

	private DateTime rankGroupDateEnd;

	private bool isSeasonLobbyPrefab;

	private string seasonBattleCondition;

	private string seasonDateStrId;

	private string rankGroupDateStrId;

	private int seasonRewardGroupId;

	public int Key => seasonID;

	public int SeasonID => seasonID;

	public string SeasonStrID => seasonStrID;

	public string Name => $"[{seasonID}] {SeasonStrID}";

	public DateTime EndDate => ServiceTime.ToUtcTime(Interval.EndDate);

	public int RankGroup => rankGroup;

	public NKMIntervalTemplet Interval { get; private set; }

	public string SeasonBattleCondition => seasonBattleCondition;

	public NKMBattleConditionTemplet BattleConditiaon { get; private set; }

	public string SeasonIcon => seasonIcon;

	public int SeasonRewardGroupId => seasonRewardGroupId;

	public string SeasonLobbyName => seasonLobbyName;

	public bool IsSeasonLobbyPrefab => isSeasonLobbyPrefab;

	public DateTime StartDate => ServiceTime.ToUtcTime(Interval.StartDate);

	public DateTime RankGroupDateEndUTC => ServiceTime.ToUtcTime(rankGroupDateEnd);

	public static NKMPvpRankSeasonTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPvpRankSeasonTemplet.cs", 36))
		{
			return null;
		}
		NKMPvpRankSeasonTemplet nKMPvpRankSeasonTemplet = new NKMPvpRankSeasonTemplet();
		int num = (int)(1u & (lua.GetData("m_Season", ref nKMPvpRankSeasonTemplet.seasonID) ? 1u : 0u)) & (lua.GetData("m_SeasonName", ref nKMPvpRankSeasonTemplet.seasonStrID) ? 1 : 0);
		lua.GetData("m_SeasonIcon", ref nKMPvpRankSeasonTemplet.seasonIcon);
		int num2 = (int)((uint)num & (lua.GetData("m_SeasonLobbyName", ref nKMPvpRankSeasonTemplet.seasonLobbyName) ? 1u : 0u) & (lua.GetData("m_RankGroup", ref nKMPvpRankSeasonTemplet.rankGroup) ? 1u : 0u) & (lua.GetData("m_bSeasonLobbyPrefab", ref nKMPvpRankSeasonTemplet.isSeasonLobbyPrefab) ? 1u : 0u) & (lua.GetData("m_SeasonDateStrID", ref nKMPvpRankSeasonTemplet.seasonDateStrId) ? 1u : 0u)) & (lua.GetData("m_RankGroupDateStrID", ref nKMPvpRankSeasonTemplet.rankGroupDateStrId) ? 1 : 0);
		lua.GetData("m_SeasonBattleCondition", ref nKMPvpRankSeasonTemplet.seasonBattleCondition);
		lua.GetData("m_RankSeasonRewardGroup", ref nKMPvpRankSeasonTemplet.seasonRewardGroupId);
		if (num2 == 0)
		{
			Log.Error($"NKMPvpRankSeasonTemplet Load Fail - {nKMPvpRankSeasonTemplet.seasonID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPvpRankSeasonTemplet.cs", 56);
			return null;
		}
		return nKMPvpRankSeasonTemplet;
	}

	public bool IsInSeason(DateTime currentSvc)
	{
		if (Interval.StartDate <= currentSvc)
		{
			return currentSvc < rankGroupDateEnd;
		}
		return false;
	}

	public void Join()
	{
		if (NKMUtil.IsServer)
		{
			JoinIntervalTemplet();
		}
		if (!string.IsNullOrEmpty(seasonBattleCondition))
		{
			BattleConditiaon = NKMBattleConditionManager.GetTempletByStrID(seasonBattleCondition);
			if (BattleConditiaon == null)
			{
				NKMTempletError.Add("[PvpSeasonTemplet:" + Name + "] invalid battleConditionId:" + seasonBattleCondition, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPvpRankSeasonTemplet.cs", 80);
			}
		}
	}

	public void JoinIntervalTemplet()
	{
		Interval = NKMIntervalTemplet.Find(seasonDateStrId);
		if (Interval == null)
		{
			Interval = NKMIntervalTemplet.Invalid;
			NKMTempletError.Add("[PvpSeasonTemplet:" + Name + "] invalid seasonDateStrId:" + seasonDateStrId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPvpRankSeasonTemplet.cs", 91);
		}
		NKMIntervalTemplet nKMIntervalTemplet = NKMIntervalTemplet.Find(rankGroupDateStrId);
		if (nKMIntervalTemplet == null)
		{
			NKMTempletError.Add("[PvpSeasonTemplet:" + Name + "] invalid rankGroupDateStrId:" + rankGroupDateStrId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPvpRankSeasonTemplet.cs", 97);
			return;
		}
		if (nKMIntervalTemplet.IsRepeatDate)
		{
			NKMTempletError.Add("[PvpSeasonTemplet:" + Name + "] 반복 기간설정 사용 불가. rankGroupDateStrId:" + rankGroupDateStrId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPvpRankSeasonTemplet.cs", 103);
		}
		rankGroupDateEnd = nKMIntervalTemplet.EndDate;
	}

	public void Validate()
	{
		if (Interval != null)
		{
			if (Interval.IsRepeatDate)
			{
				NKMTempletError.Add("[PvpSeasonTemplet:" + Name + "] 반복 기간설정 사용 불가. rankGroupDateStrId:" + rankGroupDateStrId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPvpRankSeasonTemplet.cs", 116);
			}
			if (Interval.StartDate.DayOfWeek != DayOfWeek.Monday)
			{
				Log.ErrorAndExit("[NKMPvpRankSeasonTemplet:" + Name + "] IntervalStrStrKey:" + Interval.StrKey + " 시즌 시작일은 월요일이어야 합니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPvpRankSeasonTemplet.cs", 121);
			}
		}
	}

	public string GetSeasonStrID()
	{
		return NKCStringTable.GetString(SeasonStrID);
	}

	public bool CheckMySeason(DateTime now)
	{
		if (StartDate <= now && now <= EndDate)
		{
			return true;
		}
		return false;
	}

	public bool CheckSeasonForRank(DateTime now)
	{
		if (StartDate <= now && now <= RankGroupDateEndUTC)
		{
			return true;
		}
		return false;
	}

	public void PostJoin()
	{
		JoinIntervalTemplet();
	}
}
