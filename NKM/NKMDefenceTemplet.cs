using System;
using System.Collections.Generic;
using System.Linq;
using NKC;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public sealed class NKMDefenceTemplet : INKMTemplet, INKMTempletEx
{
	public enum REWARD_TYPE
	{
		RANK,
		SCORE
	}

	private static NKMDefenceTemplet activeDefenceTemplet;

	private const int EventDropColCount = 3;

	private int m_Id;

	private string m_IntervalStr = string.Empty;

	private string m_RewardIntervalStrID = string.Empty;

	private string m_OpenTag = string.Empty;

	public int m_DungeonID;

	public int m_RequiredItemId;

	public int m_RequiredItemCount;

	public int m_ClearScore;

	public string m_SeasonName;

	public string EventDeckBG;

	public int m_RankRewardGroupID;

	public int m_ScoreGroupID;

	public int m_ScoreRewardGroupID;

	public string m_BannerDescKey;

	public NKMStageTempletV2.MainRewardTemplet m_MainRewardData;

	public FirstRewardData m_FirstRewardData;

	public NKMRewardInfo m_DungeonMissionReward = new NKMRewardInfo();

	public bool m_PrivateRankInfo;

	public IEnumerable<NKMDefenceRankRewardTemplet> numberickRewardTemplets { get; private set; }

	public IEnumerable<NKMDefenceRankRewardTemplet> percentRewardTemplets { get; private set; }

	public List<(int rewardGroupId, int prob)> m_EventDrop { get; private set; } = new List<(int, int)>();

	public int Key => m_Id;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public bool EnableScoreGroup => m_ScoreGroupID > 0;

	public bool EnableRankRewardGroup => m_RankRewardGroupID > 0;

	public bool EnableScoreRewardGroup => m_ScoreRewardGroupID > 0;

	public NKMIntervalTemplet IntervalTemplet { get; private set; } = NKMIntervalTemplet.Invalid;

	public NKMIntervalTemplet RewardIntervalTemplet { get; private set; } = NKMIntervalTemplet.Invalid;

	public DateTime ExpireDate
	{
		get
		{
			if (!EnableByTag || !EnableRankRewardGroup)
			{
				return IntervalTemplet.EndDate;
			}
			return RewardIntervalTemplet.EndDate;
		}
	}

	public static IEnumerable<NKMDefenceTemplet> Values => NKMTempletContainer<NKMDefenceTemplet>.Values;

	public bool UsePayBack => false;

	public static NKMDefenceTemplet Find(int id)
	{
		return NKMTempletContainer<NKMDefenceTemplet>.Find(id);
	}

	private static bool SelecetDefenceTemplet(DateTime currentTime)
	{
		NKM_ERROR_CODE resultCode = NKM_ERROR_CODE.NEC_OK;
		if (activeDefenceTemplet == null)
		{
			foreach (NKMDefenceTemplet value in Values)
			{
				if (value.EnableByTag && value.CheckEnable(currentTime, out resultCode))
				{
					activeDefenceTemplet = value;
					break;
				}
			}
			if (activeDefenceTemplet == null)
			{
				return false;
			}
		}
		if (!activeDefenceTemplet.CheckEnable(currentTime, out resultCode))
		{
			foreach (NKMDefenceTemplet value2 in Values)
			{
				if (value2.EnableByTag && value2.CheckEnable(currentTime, out resultCode))
				{
					activeDefenceTemplet = value2;
					break;
				}
			}
			if (!activeDefenceTemplet.CheckEnable(currentTime, out resultCode))
			{
				return false;
			}
			return true;
		}
		return true;
	}

	public static NKMDefenceTemplet GetCurrentDefenceDungeonTemplet(DateTime current)
	{
		if (!SelecetDefenceTemplet(current))
		{
			return null;
		}
		return activeDefenceTemplet;
	}

	public static NKMDefenceTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		NKMDefenceTemplet nKMDefenceTemplet = new NKMDefenceTemplet();
		cNKMLua.GetData("m_Id", ref nKMDefenceTemplet.m_Id);
		cNKMLua.GetData("m_OpenTag", ref nKMDefenceTemplet.m_OpenTag);
		cNKMLua.GetData("m_DateStrID", ref nKMDefenceTemplet.m_IntervalStr);
		cNKMLua.GetData("m_RewardDateStrID", ref nKMDefenceTemplet.m_RewardIntervalStrID);
		cNKMLua.GetData("m_DungeonID", ref nKMDefenceTemplet.m_DungeonID);
		cNKMLua.GetData("m_RequredItemID", ref nKMDefenceTemplet.m_RequiredItemId);
		cNKMLua.GetData("m_RequredItemCount", ref nKMDefenceTemplet.m_RequiredItemCount);
		cNKMLua.GetData("m_ClearScore", ref nKMDefenceTemplet.m_ClearScore);
		cNKMLua.GetData("m_RankRewardGroupID", ref nKMDefenceTemplet.m_RankRewardGroupID);
		cNKMLua.GetData("m_SeasonName", ref nKMDefenceTemplet.m_SeasonName);
		cNKMLua.GetData("m_ClearScoreGroup", ref nKMDefenceTemplet.m_ScoreGroupID);
		cNKMLua.GetData("DefenceScoreRewardGroupID", ref nKMDefenceTemplet.m_ScoreRewardGroupID);
		cNKMLua.GetData("EventDeckBG", ref nKMDefenceTemplet.EventDeckBG);
		cNKMLua.GetData("BannerDesc", ref nKMDefenceTemplet.m_BannerDescKey);
		nKMDefenceTemplet.m_FirstRewardData = FirstRewardData.Load(cNKMLua);
		nKMDefenceTemplet.m_MainRewardData = NKMStageTempletV2.MainRewardTemplet.Create(cNKMLua);
		cNKMLua.GetData("m_allStarRewardType", ref nKMDefenceTemplet.m_DungeonMissionReward.rewardType);
		cNKMLua.GetData("m_allStarRewardID", ref nKMDefenceTemplet.m_DungeonMissionReward.ID);
		cNKMLua.GetData("m_allStarRewardValue", ref nKMDefenceTemplet.m_DungeonMissionReward.Count);
		cNKMLua.GetData("m_PrivateRankInfo", ref nKMDefenceTemplet.m_PrivateRankInfo);
		for (int i = 1; i <= 3; i++)
		{
			int rValue = 0;
			int rValue2 = 0;
			cNKMLua.GetData($"m_EventDropIndex_{i}", ref rValue);
			cNKMLua.GetData($"m_EventDropProbability_{i}", ref rValue2);
			if (rValue != 0 && NKMRewardManager.ContainsKey(rValue))
			{
				nKMDefenceTemplet.m_EventDrop.Add((rValue, rValue2));
			}
		}
		return nKMDefenceTemplet;
	}

	public void Join()
	{
		if (EnableByTag && NKMUtil.IsServer)
		{
			JoinIntervalTemplet();
			if (m_RankRewardGroupID != 0)
			{
				JoinRewardTemplet();
			}
		}
	}

	public void Validate()
	{
		if (!EnableByTag)
		{
			return;
		}
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(m_DungeonID);
		if (dungeonTempletBase == null)
		{
			NKMTempletError.Add("[NKMDefenceTemplet] 연결 된 템플릿이 존재하지 않음", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDefenceTemplet.cs", 219);
		}
		m_FirstRewardData.Validate();
		if (m_DungeonMissionReward.rewardType != NKM_REWARD_TYPE.RT_NONE && !NKMRewardTemplet.IsValidReward(m_DungeonMissionReward.rewardType, m_DungeonMissionReward.ID))
		{
			NKMTempletError.Add($"[NKMDefenceTemplet] 도전과제 보상이 유효하지 않음 Defence_ID :{Key} rewardType:{m_DungeonMissionReward.rewardType} ID:{m_DungeonMissionReward.ID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDefenceTemplet.cs", 226);
		}
		if (m_RequiredItemId == 0 && m_RequiredItemCount == 0 && (dungeonTempletBase.m_RewardUserEXP > 0 || dungeonTempletBase.m_RewardCredit_Min > 0 || dungeonTempletBase.m_RewardCredit_Max > 0))
		{
			NKMTempletError.Add($"[NKMDefenceTemplet : {Key}] 입장 재화가 없지만 던전 보상이 존재함." + $" DungeonId:{dungeonTempletBase.Key} m_RewardUserEXP:{dungeonTempletBase.m_RewardUserEXP} m_RewardCredit_Min:{dungeonTempletBase.m_RewardCredit_Min}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDefenceTemplet.cs", 233);
		}
		if (EnableRankRewardGroup)
		{
			if (percentRewardTemplets.Count() == 0)
			{
				NKMTempletError.Add($"[NKMDefenceTemplet : {Key}] 순위(비) 보상이 존재하지 않음.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDefenceTemplet.cs", 244);
			}
			else
			{
				foreach (NKMDefenceRankRewardTemplet rewardTemplet in percentRewardTemplets)
				{
					if (rewardTemplet.RankValue > 100)
					{
						NKMTempletError.Add($"[NKMDefenceTemplet : {Key}] [NKMDefenceRankRewardTemplet : {rewardTemplet.Key}] 순위 보상의 값이 100%를 초과함. RankValue : {rewardTemplet.RankValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDefenceTemplet.cs", 253);
					}
					if (rewardTemplet.RankValue <= 0)
					{
						NKMTempletError.Add($"[NKMDefenceTemplet : {Key}] [NKMDefenceRankRewardTemplet : {rewardTemplet.Key}] 순위 보상의 값이 0% 보다 작거나 같음. RankValue : {rewardTemplet.RankValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDefenceTemplet.cs", 257);
					}
					if (percentRewardTemplets.Where((NKMDefenceRankRewardTemplet e) => e.RankValue == rewardTemplet.RankValue).Count() > 1)
					{
						NKMTempletError.Add($"[NKMDefenceTemplet : {Key}] [NKMDefenceRankRewardTemplet : {rewardTemplet.Key}] 중복 된 RankValue가 존재함. RankValue : {rewardTemplet.RankValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDefenceTemplet.cs", 263);
					}
				}
			}
			if (numberickRewardTemplets.Count() == 0)
			{
				NKMTempletError.Add($"[NKMDefenceTemplet : {Key}] 순위(값) 보상이 존재하지 않음.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDefenceTemplet.cs", 270);
			}
			else
			{
				foreach (NKMDefenceRankRewardTemplet rewardTemplet2 in numberickRewardTemplets)
				{
					if (rewardTemplet2.RankValue <= 0)
					{
						NKMTempletError.Add($"[NKMDefenceTemplet : {Key}] [NKMDefenceRankRewardTemplet : {rewardTemplet2.Key}] 순위 보상의 값이 0 보다 작거나 같음. RankValue : {rewardTemplet2.RankValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDefenceTemplet.cs", 279);
					}
					if (numberickRewardTemplets.Where((NKMDefenceRankRewardTemplet e) => e.RankValue == rewardTemplet2.RankValue).Count() > 1)
					{
						NKMTempletError.Add($"[NKMDefenceTemplet : {Key}] [NKMDefenceRankRewardTemplet : {rewardTemplet2.Key}] 중복 된 RankValue가 존재함. RankValue : {rewardTemplet2.RankValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDefenceTemplet.cs", 285);
					}
				}
			}
		}
		if (EnableScoreRewardGroup)
		{
			if (!NKMDefenceScoreRewardTemplet.Groups.ContainsKey(m_ScoreRewardGroupID))
			{
				NKMTempletError.Add($"[NKMDefenceTemplet : {Key}] 점수 보상이 존재하지 않음.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDefenceTemplet.cs", 297);
			}
			List<NKMDefenceScoreRewardTemplet> list = NKMDefenceScoreRewardTemplet.Groups[m_ScoreRewardGroupID];
			if (list == null)
			{
				NKMTempletError.Add($"[NKMDefenceTemplet : {Key}] 보상 리스트가 존재하지 않음. GroupID : {m_ScoreRewardGroupID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDefenceTemplet.cs", 303);
			}
			foreach (NKMDefenceScoreRewardTemplet scoreRewardTemplet in list)
			{
				if (list.Where((NKMDefenceScoreRewardTemplet e) => e.Step > scoreRewardTemplet.Step && e.Score <= scoreRewardTemplet.Score).ToList().Count() > 0)
				{
					NKMTempletError.Add($"[NKMDefenceScoreRewardTemplet : {scoreRewardTemplet.Key}] 현재 템플릿 보다 높은 스텝에 있는 점수의 값이 작습니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDefenceTemplet.cs", 311);
				}
			}
		}
		if (EnableScoreGroup)
		{
			if (!NKMDefenceMonsterScoreTemplet.Groups.ContainsKey(m_ScoreGroupID))
			{
				NKMTempletError.Add($"[NKMDefenceTemplet : {Key}] 연결 된 템플릿 그룹이 존재하지 않음." + $" ScoreGroupID:{m_ScoreGroupID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDefenceTemplet.cs", 322);
			}
			List<NKMDefenceMonsterScoreTemplet> list2 = NKMDefenceMonsterScoreTemplet.Groups[m_ScoreGroupID];
			if (list2 == null || list2.Count() <= 0)
			{
				NKMTempletError.Add($"[NKMDefenceTemplet : {Key}] 연결 된 템플릿 그룹이 존재하지 않음." + $" ScoreGroupID:{m_ScoreGroupID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDefenceTemplet.cs", 328);
			}
		}
		foreach (NKMDefenceTemplet value in Values)
		{
			if (value.Key == Key || !value.EnableByTag)
			{
				continue;
			}
			if (value.IntervalTemplet.IsValidTime(IntervalTemplet.StartDate) || value.IntervalTemplet.IsValidTime(IntervalTemplet.EndDate))
			{
				NKMTempletError.Add($"[NKMDefenceTemplet:{Key}] 이벤트 기간이 중복된 경우가 존재. 중복된 템플릿 ID:{value.Key}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDefenceTemplet.cs", 348);
				break;
			}
			if (EnableRankRewardGroup && value.EnableRankRewardGroup)
			{
				if (value.RewardIntervalTemplet.IsValidTime(IntervalTemplet.StartDate) || value.RewardIntervalTemplet.IsValidTime(IntervalTemplet.EndDate))
				{
					NKMTempletError.Add($"[NKMDefenceTemplet:{Key}] 현재 이벤트 기간과 중복된 보상 수령 기간이 존재. 중복된 템플릿 ID:{value.Key}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDefenceTemplet.cs", 357);
					break;
				}
				if (value.RewardIntervalTemplet.IsValidTime(RewardIntervalTemplet.StartDate) || value.RewardIntervalTemplet.IsValidTime(RewardIntervalTemplet.EndDate))
				{
					NKMTempletError.Add($"[NKMDefenceTemplet:{Key}] 현재 보상 수령 기간과 중복된 보상 수령 기간이 기간이 존재. 중첩된 템플릿 ID:{value.Key}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDefenceTemplet.cs", 364);
					break;
				}
			}
		}
	}

	public void JoinIntervalTemplet()
	{
		if (m_IntervalStr == string.Empty)
		{
			NKMTempletError.Add($"[NKMDefenceTemplet:{Key}] interval 스트링이 없음", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDefenceTemplet.cs", 376);
		}
		IntervalTemplet = NKMIntervalTemplet.Find(m_IntervalStr);
		if (IntervalTemplet == null)
		{
			NKMTempletError.Add($"[NKMDefenceTemplet:{Key}] IntervalTemplt이 존재하지 않음. m_IntervalStr:{m_IntervalStr}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDefenceTemplet.cs", 382);
			IntervalTemplet = NKMIntervalTemplet.Unuseable;
		}
		else if (IntervalTemplet.IsRepeatDate)
		{
			NKMTempletError.Add($"[NKMDefenceTemplet:{Key}] 반복 기간이 설정된 IntervalTemplet은 사용 불가. m_IntervalStr:{m_IntervalStr}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDefenceTemplet.cs", 387);
		}
	}

	public void JoinRewardTemplet()
	{
		IEnumerable<NKMDefenceRankRewardTemplet> source = NKMDefenceRankRewardTemplet.Values.Where((NKMDefenceRankRewardTemplet e) => e.DefenceRankRewardGroupID == m_RankRewardGroupID);
		IEnumerable<NKMDefenceRankRewardTemplet> source2 = source.Where((NKMDefenceRankRewardTemplet e) => e.PercentCheck);
		IEnumerable<NKMDefenceRankRewardTemplet> source3 = source.Where((NKMDefenceRankRewardTemplet e) => !e.PercentCheck);
		percentRewardTemplets = source2.OrderBy((NKMDefenceRankRewardTemplet e) => e.RankValue);
		numberickRewardTemplets = source3.OrderBy((NKMDefenceRankRewardTemplet e) => e.RankValue);
		RewardIntervalTemplet = NKMIntervalTemplet.Find(m_RewardIntervalStrID);
		if (RewardIntervalTemplet == null)
		{
			NKMTempletError.Add($"[NKMDefenceTemplet:{Key}] reward IntervalTemplet이 존재하지 않음. m_RewardIntervalStrID:{m_RewardIntervalStrID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDefenceTemplet.cs", 405);
			RewardIntervalTemplet = NKMIntervalTemplet.Unuseable;
		}
		else if (RewardIntervalTemplet.IsRepeatDate)
		{
			NKMTempletError.Add($"[NKMDefenceTemplet:{Key}] 반복 기간이 설정된 IntervalTemplet은 사용 불가. m_RewardIntervalStrID:{m_RewardIntervalStrID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDefenceTemplet.cs", 410);
		}
	}

	public bool CheckEnable(DateTime current, out NKM_ERROR_CODE resultCode)
	{
		resultCode = NKM_ERROR_CODE.NEC_OK;
		bool flag = false;
		if (!EnableByTag)
		{
			resultCode = NKM_ERROR_CODE.NEC_FAIL_DEFENCE_DUNGEON_NOT_OPENED;
			return flag;
		}
		flag |= CheckGameEnable(current, out resultCode);
		if (m_RankRewardGroupID != 0)
		{
			flag |= CheckRewardEnable(current, out resultCode);
		}
		if (!flag)
		{
			resultCode = NKM_ERROR_CODE.NEC_FAIL_DEFENCE_DUNGEON_NOT_OPENED;
			return flag;
		}
		resultCode = NKM_ERROR_CODE.NEC_OK;
		return flag;
	}

	public bool CheckGameEnable(DateTime current, out NKM_ERROR_CODE resultCode)
	{
		resultCode = NKM_ERROR_CODE.NEC_OK;
		if (!EnableByTag)
		{
			resultCode = NKM_ERROR_CODE.NEC_FAIL_DEFENCE_DUNGEON_NOT_OPENED;
			return false;
		}
		if (!IntervalTemplet.IsValidTime(current))
		{
			resultCode = NKM_ERROR_CODE.NEC_FAIL_DEFENCE_DUNGEON_INVALID_INTERVAL;
			return false;
		}
		if (NKMDungeonManager.GetDungeonTemplet(m_DungeonID) == null)
		{
			resultCode = NKM_ERROR_CODE.NEC_FAIL_DEFENCE_DUNGEON_INVALID_DUNGEON_TEMPLET;
			return false;
		}
		return true;
	}

	public bool CheckRewardEnable(DateTime current, out NKM_ERROR_CODE resultCode)
	{
		resultCode = NKM_ERROR_CODE.NEC_OK;
		if (!EnableByTag)
		{
			resultCode = NKM_ERROR_CODE.NEC_FAIL_DEFENCE_DUNGEON_NOT_OPENED;
			return false;
		}
		if (!RewardIntervalTemplet.IsValidTime(current))
		{
			resultCode = NKM_ERROR_CODE.NEC_FAIL_DEFENCE_DUNGEON_INVALID_INTERVAL;
			return false;
		}
		if (numberickRewardTemplets == null || percentRewardTemplets == null)
		{
			resultCode = NKM_ERROR_CODE.NEC_FAIL_DEFENCE_DUNGEON_RANK_HAVE_NOT_REWARD_TEMPLET;
			return false;
		}
		if (numberickRewardTemplets.Count() == 0 || percentRewardTemplets.Count() == 0)
		{
			resultCode = NKM_ERROR_CODE.NEC_FAIL_DEFENCE_DUNGEON_RANK_HAVE_NOT_REWARD_TEMPLET;
			return false;
		}
		return true;
	}

	public FirstRewardData GetFirstRewardData()
	{
		if (m_FirstRewardData == null)
		{
			return FirstRewardData.Empty;
		}
		if (!NKMRewardTemplet.IsOpenedReward(m_FirstRewardData.Type, m_FirstRewardData.RewardId, useRandomContract: false))
		{
			return FirstRewardData.Empty;
		}
		return m_FirstRewardData;
	}

	public NKMDefenceRankRewardTemplet GetRankRewardTemplet(int rank, int percent)
	{
		if (rank == 0)
		{
			return null;
		}
		foreach (NKMDefenceRankRewardTemplet numberickRewardTemplet in numberickRewardTemplets)
		{
			if (rank <= numberickRewardTemplet.RankValue)
			{
				return numberickRewardTemplet;
			}
		}
		foreach (NKMDefenceRankRewardTemplet percentRewardTemplet in percentRewardTemplets)
		{
			if (percent <= percentRewardTemplet.RankValue)
			{
				return percentRewardTemplet;
			}
		}
		return null;
	}

	public NKMDefenceScoreRewardTemplet GetScoreRewardTemplet(int rewardTempletID, int score)
	{
		if (!EnableScoreRewardGroup)
		{
			return null;
		}
		if (!NKMDefenceScoreRewardTemplet.Groups.ContainsKey(m_ScoreRewardGroupID))
		{
			return null;
		}
		return NKMDefenceScoreRewardTemplet.Groups[m_ScoreRewardGroupID].Where((NKMDefenceScoreRewardTemplet e) => e.Score <= score && rewardTempletID == e.Key).FirstOrDefault();
	}

	public List<NKMDefenceScoreRewardTemplet> GetScoreRewardTempletListByScore(int score)
	{
		List<NKMDefenceScoreRewardTemplet> list = new List<NKMDefenceScoreRewardTemplet>();
		if (!EnableScoreRewardGroup)
		{
			return list;
		}
		if (!NKMDefenceScoreRewardTemplet.Groups.ContainsKey(m_ScoreRewardGroupID))
		{
			return list;
		}
		list.AddRange((from e in NKMDefenceScoreRewardTemplet.Groups[m_ScoreRewardGroupID]
			where e.Score <= score
			orderby e.Step
			select e).ToList());
		return list;
	}

	public string GetSeasonName()
	{
		return NKCStringTable.GetString(m_SeasonName);
	}

	public void PostJoin()
	{
		JoinIntervalTemplet();
		if (m_RankRewardGroupID != 0)
		{
			JoinRewardTemplet();
		}
	}
}
