using System.Collections.Generic;
using Cs.Logging;
using NKC;

namespace NKM.Templet;

public sealed class NKMPvpRankTemplet
{
	private int rankGroup;

	private int leagueTier = 1;

	private string leagueName;

	private LEAGUE_TIER_ICON leagueTierIcon;

	private int leagueTierIconNumber;

	private LEAGUE_TYPE leagueType;

	private bool leagueDemote;

	private int leagueDemotePoint;

	private int leaguePointReq;

	private int blindQuestionMark;

	private bool enableLoseScore;

	private readonly List<NKMRewardInfo> rewardWeekly = new List<NKMRewardInfo>();

	private readonly List<NKMRewardInfo> rewardSeason = new List<NKMRewardInfo>();

	public string m_GameStatRateID;

	public int Key => leagueTier;

	public int RankGroup => rankGroup;

	public int LeagueTier => leagueTier;

	public string LeagueName => leagueName;

	public LEAGUE_TIER_ICON LeagueTierIcon => leagueTierIcon;

	public int LeagueTierIconNumber => leagueTierIconNumber;

	public LEAGUE_TYPE LeagueType => leagueType;

	public bool LeagueDemote => leagueDemote;

	public int LeaguePointReq => leaguePointReq;

	public int BlindQuestionMark => blindQuestionMark;

	public int LeagueDemotePoint => leagueDemotePoint;

	public bool EnableLoseScore => enableLoseScore;

	public List<NKMRewardInfo> RewardWeekly => rewardWeekly;

	public List<NKMRewardInfo> RewardSeason => rewardSeason;

	public bool LoadFromLUA(NKMLua lua)
	{
		bool flag = true;
		flag &= lua.GetData("m_RankGroup", ref rankGroup);
		flag &= lua.GetData("m_LeagueTier", ref leagueTier);
		flag &= lua.GetData("m_LeagueName", ref leagueName);
		flag &= lua.GetData("m_LeagueTierIcon", ref leagueTierIcon);
		flag &= lua.GetData("m_LeagueTierIconNumber", ref leagueTierIconNumber);
		flag &= lua.GetData("m_LeagueType", ref leagueType);
		flag &= lua.GetData("m_bLeagueDemote", ref leagueDemote);
		flag &= lua.GetData("m_LeaguePointReq", ref leaguePointReq);
		flag &= lua.GetData("m_GameStatRateID", ref m_GameStatRateID);
		flag &= lua.GetData("LeagueDemotePoint", ref leagueDemotePoint);
		flag &= lua.GetData("m_bLoseScore", ref enableLoseScore);
		lua.GetData("m_BlindQuestionMark", ref blindQuestionMark);
		int rValue = 0;
		int rValue2 = 0;
		lua.GetData("m_RewardCashWeekly", ref rValue);
		lua.GetData("m_RewardPVPPointWeekly", ref rValue2);
		if (rValue != 0)
		{
			rewardWeekly.Add(new NKMRewardInfo
			{
				rewardType = NKM_REWARD_TYPE.RT_MISC,
				ID = 101,
				Count = rValue
			});
		}
		if (rValue2 != 0)
		{
			rewardWeekly.Add(new NKMRewardInfo
			{
				rewardType = NKM_REWARD_TYPE.RT_MISC,
				ID = 5,
				Count = rValue2
			});
		}
		for (int i = 1; i <= 3; i++)
		{
			NKMRewardInfo nKMRewardInfo = new NKMRewardInfo();
			if (!lua.GetData($"m_RewardTypeWeekly_{i}", ref nKMRewardInfo.rewardType))
			{
				break;
			}
			flag &= lua.GetData($"m_RewardIDWeekly_{i}", ref nKMRewardInfo.ID);
			flag &= lua.GetData($"m_RewardValueWeekly_{i}", ref nKMRewardInfo.Count);
			rewardWeekly.Add(nKMRewardInfo);
		}
		int rValue3 = 0;
		int rValue4 = 0;
		lua.GetData("m_RewardCashSeason", ref rValue3);
		lua.GetData("m_RewardPVPPointSeason", ref rValue4);
		if (rValue3 != 0)
		{
			rewardSeason.Add(new NKMRewardInfo
			{
				rewardType = NKM_REWARD_TYPE.RT_MISC,
				ID = 101,
				Count = rValue3
			});
		}
		if (rValue4 != 0)
		{
			rewardSeason.Add(new NKMRewardInfo
			{
				rewardType = NKM_REWARD_TYPE.RT_MISC,
				ID = 5,
				Count = rValue4
			});
		}
		for (int j = 1; j <= 3; j++)
		{
			NKMRewardInfo nKMRewardInfo2 = new NKMRewardInfo();
			if (!lua.GetData($"m_RewardTypeSeason_{j}", ref nKMRewardInfo2.rewardType))
			{
				break;
			}
			flag &= lua.GetData($"m_RewardIDSeason_{j}", ref nKMRewardInfo2.ID);
			flag &= lua.GetData($"m_RewardValueSeason_{j}", ref nKMRewardInfo2.Count);
			rewardSeason.Add(nKMRewardInfo2);
		}
		if (rewardSeason.Count == 0)
		{
			Log.ErrorAndExit("PVP Season RewardCount is zero.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPvpRankTemplet.cs", 144);
			return false;
		}
		return flag;
	}

	public static bool CompareHighScore(NKMPvpRankTemplet left, NKMPvpRankTemplet right)
	{
		if (left.LeagueTierIcon == LEAGUE_TIER_ICON.LTI_CHALLENGER && right.leagueTierIcon == LEAGUE_TIER_ICON.LTI_CHALLENGER)
		{
			return true;
		}
		return false;
	}

	public string GetLeagueName()
	{
		return NKCStringTable.GetString(LeagueName);
	}
}
