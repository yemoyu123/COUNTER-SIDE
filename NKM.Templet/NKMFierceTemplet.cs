using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMFierceTemplet : INKMTemplet, INKMTempletEx
{
	private string gameIntervalId;

	private string rewardIntervalId;

	public int FierceID;

	public int PointRewardGroupID;

	public int RankRewardGroupID;

	public int DailyEnterLimit = 3;

	public List<int> FierceBossGroupIdList = new List<int>();

	public List<NKMRewardInfo> DailyRewards = new List<NKMRewardInfo>();

	private string m_OpenTag;

	public int Key => FierceID;

	public NKMIntervalTemplet GameInterval { get; private set; } = NKMIntervalTemplet.Invalid;

	public NKMIntervalTemplet RewardInterval { get; private set; } = NKMIntervalTemplet.Invalid;

	public DateTime FierceGameStart => GameInterval.StartDate;

	public DateTime FierceGameEnd => GameInterval.EndDate;

	public DateTime FierceRewardPeriodStart => RewardInterval.StartDate;

	public DateTime FierceRewardPeriodEnd => RewardInterval.EndDate;

	internal bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public static IEnumerable<NKMFierceTemplet> Values => NKMTempletContainer<NKMFierceTemplet>.Values;

	public static NKMFierceTemplet Find(int key)
	{
		return NKMTempletContainer<NKMFierceTemplet>.Find((NKMFierceTemplet x) => x.FierceID == key);
	}

	public static NKMFierceTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceTemplet.cs", 44))
		{
			return null;
		}
		NKMFierceTemplet nKMFierceTemplet = new NKMFierceTemplet();
		bool flag = true;
		flag &= lua.GetData("FierceID", ref nKMFierceTemplet.FierceID);
		flag &= lua.GetData("m_GameDateStrID", ref nKMFierceTemplet.gameIntervalId);
		flag &= lua.GetData("m_RewardDateStrID", ref nKMFierceTemplet.rewardIntervalId);
		flag &= lua.GetData("PointRewardGroupID", ref nKMFierceTemplet.PointRewardGroupID);
		flag &= lua.GetData("RankRewardGroupID", ref nKMFierceTemplet.RankRewardGroupID);
		flag &= lua.GetData("DailyEnterLimit", ref nKMFierceTemplet.DailyEnterLimit);
		flag &= lua.GetData("m_OpenTag", ref nKMFierceTemplet.m_OpenTag);
		if (!flag)
		{
			Log.Error($"[FierceTemplet] ID[{nKMFierceTemplet.FierceID}] - no OpenTag", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceTemplet.cs", 59);
		}
		int rValue = 0;
		lua.GetData("FierceBossGroupID_1", ref rValue);
		if (rValue > 0)
		{
			nKMFierceTemplet.FierceBossGroupIdList.Add(rValue);
		}
		for (int i = 1; i <= 2; i++)
		{
			NKM_REWARD_TYPE result = NKM_REWARD_TYPE.RT_NONE;
			lua.GetData($"DailyRewardType_{i}", ref result);
			if (result != NKM_REWARD_TYPE.RT_NONE)
			{
				int rValue2 = 0;
				flag &= lua.GetData($"DailyRewardID_{i}", ref rValue2);
				int rValue3 = 0;
				flag &= lua.GetData($"DailyRewardCount_{i}", ref rValue3);
				NKMRewardInfo item = new NKMRewardInfo
				{
					rewardType = result,
					ID = rValue2,
					Count = rValue3,
					paymentType = NKM_ITEM_PAYMENT_TYPE.NIPT_FREE
				};
				nKMFierceTemplet.DailyRewards.Add(item);
			}
		}
		if (!flag)
		{
			NKMTempletError.Add($"[FierceTemplet] data is invalid, fierce id: {nKMFierceTemplet.FierceID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceTemplet.cs", 111);
			return null;
		}
		return nKMFierceTemplet;
	}

	public void Join()
	{
		if (NKMUtil.IsServer)
		{
			JoinIntervalTemplet();
		}
	}

	public void JoinIntervalTemplet()
	{
		GameInterval = NKMIntervalTemplet.Find(gameIntervalId);
		if (GameInterval == null)
		{
			GameInterval = NKMIntervalTemplet.Invalid;
			NKMTempletError.Add($"[NKMFierceTemplet:{Key}] game interval templet이 존재하지 않음. gameIntervalId:{gameIntervalId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceTemplet.cs", 132);
		}
		else if (GameInterval.IsRepeatDate)
		{
			NKMTempletError.Add($"[NKMFierceTemplet:{Key}] game Interval templet에 반복 기간 설정이 적용됨. gameIntervalId:{gameIntervalId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceTemplet.cs", 136);
		}
		RewardInterval = NKMIntervalTemplet.Find(rewardIntervalId);
		if (RewardInterval == null)
		{
			RewardInterval = NKMIntervalTemplet.Invalid;
			NKMTempletError.Add($"[NKMFierceTemplet:{Key}] reward interval templet이 존재하지 않음. rewardIntervalId:{rewardIntervalId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceTemplet.cs", 143);
		}
		else if (RewardInterval.IsRepeatDate)
		{
			NKMTempletError.Add($"[NKMFierceTemplet:{Key}] reward interval templet에 반복 기간 설정이 적용됨. rewardIntervalId:{rewardIntervalId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceTemplet.cs", 147);
		}
	}

	public void Validate()
	{
		NKMFierceTemplet prevFierceTemplet = GetPrevFierceTemplet(FierceID);
		if (prevFierceTemplet != null)
		{
			TimeSpan timeSpan = TimeSpan.FromMinutes(10.0);
			if (FierceGameStart - prevFierceTemplet.FierceRewardPeriodEnd < timeSpan)
			{
				NKMTempletError.Add($"[NKMFierceTemplet:{Key}] 격전지원 간의 시즌 초기화 작업 시간이 부여되지 않았습니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceTemplet.cs", 161);
				return;
			}
		}
		if (!NKMFiercePointRewardTemplet.Groups.ContainsKey(PointRewardGroupID))
		{
			NKMTempletError.Add($"[NKMFierceTemplet:{Key}] Point Reward Templet을 찾을 수 없습니다. PointRewardGroupId:{PointRewardGroupID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceTemplet.cs", 168);
			return;
		}
		if (!NKMFierceRankRewardTemplet.Groups.ContainsKey(RankRewardGroupID))
		{
			NKMTempletError.Add($"[NKMFierceTemplet:{Key}] Rank Reward Templet을 찾을 수 없습니다. RankRewardGroupId:{RankRewardGroupID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceTemplet.cs", 174);
			return;
		}
		foreach (int fierceBossGroupId in FierceBossGroupIdList)
		{
			if (!NKMFierceBossGroupTemplet.Groups.ContainsKey(fierceBossGroupId))
			{
				NKMTempletError.Add($"[NKMFierceTemplet:{Key}] Boss Group Templet을 찾을 수 없습니다. bossGroupId:{fierceBossGroupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceTemplet.cs", 182);
				return;
			}
		}
		if (FierceGameStart > FierceGameEnd)
		{
			NKMTempletError.Add($"[NKMFierceTemplet:{Key}] 플레이 시작시간이 종료시간보다 뒤에 있습니다.  GameStart:{FierceGameStart} GameEnd:{FierceGameEnd}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceTemplet.cs", 189);
		}
		else if (FierceRewardPeriodStart > FierceRewardPeriodEnd)
		{
			NKMTempletError.Add($"[NKMFierceTemplet:{Key}] 보상 수령 시작 시간이 종료시간보다 뒤에 있습니다. RewardStart:{FierceRewardPeriodStart} RewardEnd:{FierceRewardPeriodEnd}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceTemplet.cs", 195);
		}
		else if (FierceGameEnd > FierceRewardPeriodEnd)
		{
			NKMTempletError.Add($"[NKMFierceTemplet:{Key}] 게임 종료 시간이 보상 종료시간보다 뒤에 있습니다. GameEnd:{FierceGameEnd} rewardEnd:{FierceRewardPeriodEnd}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceTemplet.cs", 201);
		}
	}

	public bool IsGamePeriodOut(DateTime current)
	{
		if (!EnableByTag)
		{
			return false;
		}
		if (current < FierceGameStart || current > FierceGameEnd)
		{
			return true;
		}
		return false;
	}

	public bool IsSeasonOut(DateTime current)
	{
		if (!EnableByTag)
		{
			return false;
		}
		if (current < FierceGameStart || current > FierceRewardPeriodEnd)
		{
			return true;
		}
		return false;
	}

	public bool IsRewardPeriodOut(DateTime current)
	{
		if (!EnableByTag)
		{
			return false;
		}
		if (current < FierceRewardPeriodStart || current > FierceRewardPeriodEnd)
		{
			return true;
		}
		return false;
	}

	public static NKMFierceTemplet GetCurrentFierceTemplet(DateTime current)
	{
		foreach (NKMFierceTemplet value in Values)
		{
			if (value.EnableByTag && current < value.FierceRewardPeriodEnd)
			{
				return value;
			}
		}
		return null;
	}

	public static NKMFierceTemplet GetNextFierceTemplet(int fierceId)
	{
		return Values.Where((NKMFierceTemplet e) => e.EnableByTag).FirstOrDefault((NKMFierceTemplet e) => e.FierceID > fierceId);
	}

	private static NKMFierceTemplet GetPrevFierceTemplet(int fierceId)
	{
		return Values.Where((NKMFierceTemplet e) => e.EnableByTag).LastOrDefault((NKMFierceTemplet e) => e.FierceID < fierceId);
	}

	public void PostJoin()
	{
		JoinIntervalTemplet();
	}
}
