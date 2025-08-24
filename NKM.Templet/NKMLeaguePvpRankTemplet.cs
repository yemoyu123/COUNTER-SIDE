using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMLeaguePvpRankTemplet
{
	private readonly List<NKMRewardInfo> rewardWeekly = new List<NKMRewardInfo>();

	private readonly List<NKMRewardInfo> rewardSeason = new List<NKMRewardInfo>();

	private int leagueTier;

	private string leagueName;

	private LEAGUE_TIER_ICON leagueTierIcon;

	private int leagueTierIconNumber;

	private LEAGUE_TYPE leagueType;

	private bool leagueDemote;

	private int leaguePointReq;

	private string gameStatRateId;

	private int loseScoreDeduction;

	private int scoreRelegation;

	private bool enableLoseScore;

	public string DebugName => $"[{LeagueTier}]{LeagueTierIcon}";

	public int Key => leagueTier;

	public int GroupId { get; }

	public int LeagueTier => leagueTier;

	public string LeagueName => leagueName;

	public LEAGUE_TIER_ICON LeagueTierIcon => leagueTierIcon;

	public int LeagueTierIconNumber => leagueTierIconNumber;

	public LEAGUE_TYPE LeagueType => leagueType;

	public bool LeagueDemote => leagueDemote;

	public int LeaguePointReq => leaguePointReq;

	public int LoseScoreDeduction => loseScoreDeduction;

	public int ScoreRelegation => scoreRelegation;

	public bool EnableLoseScore => enableLoseScore;

	public List<NKMRewardInfo> RewardWeekly => rewardWeekly;

	public List<NKMRewardInfo> RewardSeason => rewardSeason;

	public NKMGameStatRateTemplet GameStatRateTemplet { get; private set; }

	public NKMLeaguePvpRankTemplet(int groupId)
	{
		GroupId = groupId;
	}

	public static NKMLeaguePvpRankTemplet LoadFromLUA(NKMLua lua, int groupId)
	{
		NKMLeaguePvpRankTemplet nKMLeaguePvpRankTemplet = new NKMLeaguePvpRankTemplet(groupId);
		bool flag = true;
		flag &= lua.GetData("m_LeagueTier", ref nKMLeaguePvpRankTemplet.leagueTier);
		flag &= lua.GetData("m_LeagueName", ref nKMLeaguePvpRankTemplet.leagueName);
		flag &= lua.GetData("m_LeagueTierIcon", ref nKMLeaguePvpRankTemplet.leagueTierIcon);
		flag &= lua.GetData("m_LeagueTierIconNumber", ref nKMLeaguePvpRankTemplet.leagueTierIconNumber);
		flag &= lua.GetData("m_LeagueType", ref nKMLeaguePvpRankTemplet.leagueType);
		flag &= lua.GetData("m_bLeagueDemote", ref nKMLeaguePvpRankTemplet.leagueDemote);
		flag &= lua.GetData("m_LeaguePointReq", ref nKMLeaguePvpRankTemplet.leaguePointReq);
		flag &= lua.GetData("m_GameStatRateID", ref nKMLeaguePvpRankTemplet.gameStatRateId);
		flag &= lua.GetData("m_LoseScoreDeduction", ref nKMLeaguePvpRankTemplet.loseScoreDeduction);
		flag &= lua.GetData("ScoreRelegation", ref nKMLeaguePvpRankTemplet.scoreRelegation);
		flag &= lua.GetData("m_bLoseScore", ref nKMLeaguePvpRankTemplet.enableLoseScore);
		int rValue = 0;
		int rValue2 = 0;
		lua.GetData("m_RewardCashWeekly", ref rValue);
		lua.GetData("m_RewardPVPPointWeekly", ref rValue2);
		if (rValue != 0)
		{
			nKMLeaguePvpRankTemplet.rewardWeekly.Add(new NKMRewardInfo
			{
				rewardType = NKM_REWARD_TYPE.RT_MISC,
				ID = 101,
				Count = rValue
			});
		}
		if (rValue2 != 0)
		{
			nKMLeaguePvpRankTemplet.rewardWeekly.Add(new NKMRewardInfo
			{
				rewardType = NKM_REWARD_TYPE.RT_MISC,
				ID = 5,
				Count = rValue2
			});
		}
		for (int i = 0; i < 3; i++)
		{
			NKMRewardInfo nKMRewardInfo = new NKMRewardInfo();
			int num = i + 1;
			if (!lua.GetData($"m_RewardTypeWeekly_{num}", ref nKMRewardInfo.rewardType))
			{
				break;
			}
			flag &= lua.GetData($"m_RewardIDWeekly_{num}", ref nKMRewardInfo.ID);
			flag &= lua.GetData($"m_RewardValueWeekly_{num}", ref nKMRewardInfo.Count);
			nKMLeaguePvpRankTemplet.rewardWeekly.Add(nKMRewardInfo);
		}
		int rValue3 = 0;
		int rValue4 = 0;
		lua.GetData("m_RewardCashSeason", ref rValue3);
		lua.GetData("m_RewardPVPPointSeason", ref rValue4);
		if (rValue3 != 0)
		{
			nKMLeaguePvpRankTemplet.rewardSeason.Add(new NKMRewardInfo
			{
				rewardType = NKM_REWARD_TYPE.RT_MISC,
				ID = 101,
				Count = rValue3
			});
		}
		if (rValue4 != 0)
		{
			nKMLeaguePvpRankTemplet.rewardSeason.Add(new NKMRewardInfo
			{
				rewardType = NKM_REWARD_TYPE.RT_MISC,
				ID = 5,
				Count = rValue4
			});
		}
		for (int j = 0; j < 3; j++)
		{
			NKMRewardInfo nKMRewardInfo2 = new NKMRewardInfo();
			int num2 = j + 1;
			if (!lua.GetData($"m_RewardTypeSeason_{num2}", ref nKMRewardInfo2.rewardType))
			{
				break;
			}
			flag &= lua.GetData($"m_RewardIDSeason_{num2}", ref nKMRewardInfo2.ID);
			flag &= lua.GetData($"m_RewardValueSeason_{num2}", ref nKMRewardInfo2.Count);
			nKMLeaguePvpRankTemplet.rewardSeason.Add(nKMRewardInfo2);
		}
		if (nKMLeaguePvpRankTemplet.rewardSeason.Count == 0)
		{
			Log.ErrorAndExit("PVP Season RewardCount is zero.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankTemplet.cs", 150);
			return null;
		}
		if (!flag)
		{
			return null;
		}
		return nKMLeaguePvpRankTemplet;
	}

	public static bool FindByTier(int seasonId, int leagueTierId, out NKMLeaguePvpRankTemplet templet)
	{
		NKMLeaguePvpRankSeasonTemplet nKMLeaguePvpRankSeasonTemplet = NKMLeaguePvpRankSeasonTemplet.Find(seasonId);
		if (nKMLeaguePvpRankSeasonTemplet == null)
		{
			templet = null;
			return false;
		}
		templet = nKMLeaguePvpRankSeasonTemplet.RankGroup.GetByTier(leagueTierId);
		return templet != null;
	}

	public static bool FindByScore(int seasonId, int score, out NKMLeaguePvpRankTemplet templet)
	{
		NKMLeaguePvpRankSeasonTemplet nKMLeaguePvpRankSeasonTemplet = NKMLeaguePvpRankSeasonTemplet.Find(seasonId);
		if (nKMLeaguePvpRankSeasonTemplet == null)
		{
			templet = null;
			return false;
		}
		templet = nKMLeaguePvpRankSeasonTemplet.RankGroup.GetByScore(score);
		return templet != null;
	}

	public void Join()
	{
		GameStatRateTemplet = NKMGameStatRateTemplet.Find(gameStatRateId);
		if (GameStatRateTemplet == null)
		{
			NKMTempletError.Add($"[LeaguePvpRank/group:{GroupId} tier:{LeagueTier}] invalid StatRateId:{gameStatRateId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankTemplet.cs", 188);
		}
	}

	public void Validate()
	{
		if (leagueType != LEAGUE_TYPE.LEAGUE_TYPE_NORMAL)
		{
			NKMTempletError.Add($"[LeaguePvpRank/group:{GroupId} tier:{LeagueTier}] 리그전은 LEAGUE_TYPE_NORMAL 타입만 사용. league:{leagueDemote} leagueType:{LeagueType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankTemplet.cs", 197);
		}
	}
}
