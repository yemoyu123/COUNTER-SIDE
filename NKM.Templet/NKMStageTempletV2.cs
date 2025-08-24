using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using Cs.Shared.Time;
using NKC;
using NKM.Contract2;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMStageTempletV2 : IComparable<NKMStageTempletV2>, INKMTemplet
{
	public enum RESET_TYPE
	{
		NONE,
		DAY,
		WEEK,
		MONTH
	}

	private enum DAY_OF_WEEK
	{
		SUN,
		MON,
		TUE,
		WED,
		THU,
		FRI,
		SAT
	}

	public sealed class MainRewardTemplet
	{
		private const int ProbabilityMax = 10000;

		public readonly NKM_REWARD_TYPE rewardType;

		public int ID;

		public int MinValue;

		public int MaxValue;

		public int Probability;

		private MainRewardTemplet(NKM_REWARD_TYPE rewardType)
		{
			this.rewardType = rewardType;
		}

		public static MainRewardTemplet Create(NKMLua lua)
		{
			if (!lua.GetData("m_MainRewardType", out var result, NKM_REWARD_TYPE.RT_NONE) || result == NKM_REWARD_TYPE.RT_NONE)
			{
				return null;
			}
			MainRewardTemplet mainRewardTemplet = new MainRewardTemplet(result);
			lua.GetData("m_MainRewardID", ref mainRewardTemplet.ID);
			lua.GetData("m_MainRewardMin", ref mainRewardTemplet.MinValue);
			lua.GetData("m_MainRewardMax", ref mainRewardTemplet.MaxValue);
			lua.GetData("m_MainRewardProbability", ref mainRewardTemplet.Probability);
			return mainRewardTemplet;
		}

		public void Validate()
		{
			if (!NKMRewardTemplet.IsValidReward(rewardType, ID))
			{
				NKMTempletError.Add($"[Stage] 스테이지 메인보상이 올바르지 않음. rewardType:{rewardType} id:{ID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 846);
			}
			if (MinValue <= 0 || MaxValue <= 0 || MinValue > MaxValue)
			{
				NKMTempletError.Add($"[Stage] 스테이지 메인보상 수치 이상. rewardType:{rewardType} id:{ID} minValue:{MinValue} maxValue:{MaxValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 851);
			}
		}

		public void ValidateServerOnly()
		{
			if (Probability <= 0 || Probability > 10000)
			{
				NKMTempletError.Add($"[Stage] 스테이지 확률 수치 이상. rewardType:{rewardType} id:{ID} probarility:{Probability}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 859);
			}
		}

		public bool Decide(out NKMRewardInfo rewardInfo)
		{
			if (!NKMRewardTemplet.IsOpenedReward(rewardType, ID, useRandomContract: false))
			{
				rewardInfo = null;
				return false;
			}
			if (PerThreadRandom.Instance.Next(10000) >= Probability)
			{
				rewardInfo = null;
				return false;
			}
			rewardInfo = new NKMRewardInfo
			{
				rewardType = rewardType,
				ID = ID,
				Count = PerThreadRandom.Instance.Next(MinValue, MaxValue + 1),
				paymentType = NKM_ITEM_PAYMENT_TYPE.NIPT_FREE
			};
			return true;
		}
	}

	private const int EventDropColCount = 3;

	private int episodeId;

	private int actId;

	private int id;

	private string m_OpenTag;

	private List<int> companyBuffDropIds = new List<int>();

	private int companyBuffDropIndex;

	public string StrId;

	public STAGE_TYPE m_STAGE_TYPE;

	public STAGE_SUB_TYPE m_STAGE_SUB_TYPE;

	public int m_StageIndex;

	public int m_StageUINum;

	public string m_StageBattleStrID = "";

	private string m_StageDesc = "";

	public string m_StageCharStr = "";

	public string m_StageCharStrFace = "UNIT_IDLE";

	public STAGE_BASIC_UNLOCK_TYPE m_StageBasicUnlockType;

	public int m_StageReqItemID;

	public int m_StageReqItemCount;

	public bool m_bSupportUnit;

	public string m_ACT_BG_Image = "";

	public RewardTuningType m_BuffType;

	public int m_BuffRatio;

	public EPISODE_DIFFICULTY m_Difficulty;

	public string m_ShopShortcut = "TAB_NONE";

	public int m_ShopShortcutResourceID;

	public string m_ShopShortcutBgName;

	public UnlockInfo m_UnlockInfo;

	private FirstRewardData firstRewardData;

	public MainRewardTemplet MainRewardData;

	private readonly NKMRewardInfo missionReward = new NKMRewardInfo();

	private NKMOnetimeRewardTemplet[] onetimeRewards = new NKMOnetimeRewardTemplet[3];

	private readonly HashSet<DayOfWeek> enterableDayOfWeeks = new HashSet<DayOfWeek>();

	private int m_EnterLimit;

	private RESET_TYPE m_EnterLimitCond;

	private int m_RestoreLimit;

	private int m_RestoreLimitEnterCount;

	public bool m_bNoAutoRepeat;

	public bool m_bActiveBattleSkip;

	private bool m_bHaveEventDrop;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public int Key => id;

	public NKMEpisodeTempletV2 EpisodeTemplet { get; private set; }

	public int EpisodeId => EpisodeTemplet.m_EpisodeID;

	public int ActId => actId;

	public string DebugName => $"[{id}] {episodeId}-{actId}";

	public NKMRewardInfo MissionReward => missionReward;

	public IReadOnlyList<NKMOnetimeRewardTemplet> OnetimeRewards => onetimeRewards;

	public EPISODE_CATEGORY EpisodeCategory => EpisodeTemplet.m_EPCategory;

	public NKMDungeonTempletBase DungeonTempletBase { get; private set; }

	public NKMWarfareTemplet WarfareTemplet { get; private set; }

	public NKMPhaseTemplet PhaseTemplet { get; private set; }

	public NKMKillCountTemplet KillCountTemplet { get; internal set; }

	public IEnumerable<DayOfWeek> EnterableDays => enterableDayOfWeeks;

	public int EnterLimit => m_EnterLimit;

	public RESET_TYPE EnterLimitCond => m_EnterLimitCond;

	public bool Restorable => RestoreLimit > 0;

	public int RestoreLimit => m_RestoreLimit;

	public int RestoreLimitEnterCount => m_RestoreLimitEnterCount;

	public bool IsClearRequireItem
	{
		get
		{
			if (EpisodeCategory == EPISODE_CATEGORY.EC_DAILY)
			{
				return m_STAGE_TYPE == STAGE_TYPE.ST_DUNGEON;
			}
			return false;
		}
	}

	public MiscItemUnit RestoreReqItem { get; private set; }

	public MiscItemUnit UnlockReqItem { get; private set; }

	public bool NeedToUnlock => UnlockReqItem != null;

	public List<int> CompanyBuffDropIds => companyBuffDropIds;

	public int CompanyBuffDropIndex => companyBuffDropIndex;

	public List<(int rewardGroupId, int prob)> m_EventDrop { get; private set; } = new List<(int, int)>();

	public static IEnumerable<NKMStageTempletV2> Values => NKMTempletContainer<NKMStageTempletV2>.Values;

	public static int EventDropGroupCount => 3;

	public void SetOwner(NKMEpisodeTempletV2 episodeTemplet)
	{
		EpisodeTemplet = episodeTemplet;
	}

	public int CompareTo(NKMStageTempletV2 other)
	{
		return m_StageIndex.CompareTo(other.m_StageIndex);
	}

	public static NKMStageTempletV2 LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 295))
		{
			return null;
		}
		NKMStageTempletV2 nKMStageTempletV = new NKMStageTempletV2();
		lua.GetData("m_EpisodeID", ref nKMStageTempletV.episodeId);
		lua.GetData("m_ActID", ref nKMStageTempletV.actId);
		lua.GetData("m_StageID", ref nKMStageTempletV.id);
		lua.GetData("m_StageStrID", ref nKMStageTempletV.StrId);
		lua.GetData("m_OpenTag", ref nKMStageTempletV.m_OpenTag);
		if (!lua.GetData("m_StageType", ref nKMStageTempletV.m_STAGE_TYPE))
		{
			Log.ErrorAndExit("NKMEpisodeTemplet invalid EpisodeMissionType", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 308);
			return null;
		}
		lua.GetData("m_StageSubType", ref nKMStageTempletV.m_STAGE_SUB_TYPE);
		lua.GetData("m_StageIndex", ref nKMStageTempletV.m_StageIndex);
		lua.GetData("m_StageUINum", ref nKMStageTempletV.m_StageUINum);
		lua.GetData("m_StageBattleStrID", ref nKMStageTempletV.m_StageBattleStrID);
		lua.GetData("m_StageDesc", ref nKMStageTempletV.m_StageDesc);
		lua.GetData("m_StageCharStr", ref nKMStageTempletV.m_StageCharStr);
		lua.GetData("m_StageCharStrFace", ref nKMStageTempletV.m_StageCharStrFace);
		lua.GetData("m_Difficulty", ref nKMStageTempletV.m_Difficulty);
		if (!lua.GetData("m_StageBasicUnlockType", ref nKMStageTempletV.m_StageBasicUnlockType))
		{
			Log.ErrorAndExit("NKMEpisodeTemplet invalid m_StageBasicUnlockType", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 325);
			return null;
		}
		lua.GetData("m_bNoAutoRepeat", ref nKMStageTempletV.m_bNoAutoRepeat);
		lua.GetData("m_StageReqItemID", ref nKMStageTempletV.m_StageReqItemID);
		lua.GetData("m_StageReqItemCount", ref nKMStageTempletV.m_StageReqItemCount);
		lua.GetData("m_ACT_BG_Image", ref nKMStageTempletV.m_ACT_BG_Image);
		lua.GetData("RewardTuningType", ref nKMStageTempletV.m_BuffType);
		lua.GetData("m_BuffRatio", ref nKMStageTempletV.m_BuffRatio);
		nKMStageTempletV.firstRewardData = FirstRewardData.Load(lua);
		for (int i = 0; i < 3; i++)
		{
			NKMOnetimeRewardTemplet nKMOnetimeRewardTemplet = NKMOnetimeRewardTemplet.Load(lua, i);
			if (nKMOnetimeRewardTemplet != null)
			{
				nKMStageTempletV.onetimeRewards[i] = nKMOnetimeRewardTemplet;
			}
		}
		lua.GetData("m_allStarRewardType", ref nKMStageTempletV.missionReward.rewardType);
		lua.GetData("m_allStarRewardID", ref nKMStageTempletV.missionReward.ID);
		lua.GetData("m_allStarRewardValue", ref nKMStageTempletV.missionReward.Count);
		nKMStageTempletV.MainRewardData = MainRewardTemplet.Create(lua);
		lua.GetData("m_EnterLimit", ref nKMStageTempletV.m_EnterLimit);
		lua.GetData("m_EnterLimitCond", ref nKMStageTempletV.m_EnterLimitCond);
		if (lua.GetDataList("m_EnterLimitDays", out List<string> result, nullIfEmpty: false))
		{
			foreach (string item2 in result)
			{
				if (!Enum.TryParse<DAY_OF_WEEK>(item2, ignoreCase: true, out var result2))
				{
					NKMTempletError.Add("[" + nKMStageTempletV.DebugName + "] 요일 입력이 올바르지 않음: " + item2, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 367);
					continue;
				}
				DayOfWeek item = (DayOfWeek)result2;
				if (nKMStageTempletV.enterableDayOfWeeks.Contains(item))
				{
					NKMTempletError.Add("[" + nKMStageTempletV.DebugName + "] 요일 중복 설정: " + item2, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 374);
				}
				else
				{
					nKMStageTempletV.enterableDayOfWeeks.Add(item);
				}
			}
		}
		lua.GetData("m_RestoreLimit", ref nKMStageTempletV.m_RestoreLimit);
		lua.GetData("m_RestoreLimitEnterCount", ref nKMStageTempletV.m_RestoreLimitEnterCount);
		lua.GetData("m_RestoreLimitReqItemID", out var rValue, 0);
		lua.GetData("m_RestoreLimitReqItemCount", out var rValue2, 0);
		if (rValue > 0)
		{
			nKMStageTempletV.RestoreReqItem = new MiscItemUnit(rValue, rValue2);
		}
		lua.GetData("m_UnlockItemID", out rValue, 0);
		lua.GetData("m_UnlockItemPrice", out rValue2, 0);
		if (rValue > 0)
		{
			nKMStageTempletV.UnlockReqItem = new MiscItemUnit(rValue, rValue2);
		}
		lua.GetData("m_bActiveBattleSkip", ref nKMStageTempletV.m_bActiveBattleSkip);
		for (int j = 1; j <= 3; j++)
		{
			int rValue3 = 0;
			int rValue4 = 0;
			lua.GetData($"m_EventDropIndex_{j}", ref rValue3);
			lua.GetData($"m_EventDropProbability_{j}", ref rValue4);
			if (rValue3 != 0 && NKMRewardManager.ContainsKey(rValue3))
			{
				nKMStageTempletV.m_EventDrop.Add((rValue3, rValue4));
			}
		}
		lua.GetData("m_ShopShortcut", ref nKMStageTempletV.m_ShopShortcut);
		lua.GetData("m_ShortcutResourceID", ref nKMStageTempletV.m_ShopShortcutResourceID);
		lua.GetData("m_ShopShortcutBgName", ref nKMStageTempletV.m_ShopShortcutBgName);
		lua.GetData("m_bSupportUnit", ref nKMStageTempletV.m_bSupportUnit);
		lua.GetDataList("m_CompanyBuffDropID", out nKMStageTempletV.companyBuffDropIds, nullIfEmpty: false);
		lua.GetData("m_CompanyBuffDropIndex", ref nKMStageTempletV.companyBuffDropIndex);
		nKMStageTempletV.m_UnlockInfo = UnlockInfo.LoadFromLua(lua, nullable: false);
		return nKMStageTempletV;
	}

	public static NKMStageTempletV2 Find(int key)
	{
		return NKMTempletContainer<NKMStageTempletV2>.Find(key);
	}

	public static NKMStageTempletV2 Find(string key)
	{
		return NKMTempletContainer<NKMStageTempletV2>.Find(key);
	}

	public bool CanUseRewardMultiply()
	{
		if (EpisodeCategory == EPISODE_CATEGORY.EC_COUNTERCASE)
		{
			return false;
		}
		return true;
	}

	public void Join()
	{
		NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(episodeId, m_Difficulty);
		if (nKMEpisodeTempletV != null)
		{
			EpisodeTemplet = nKMEpisodeTempletV;
			SetOwner(nKMEpisodeTempletV);
			nKMEpisodeTempletV.AddStageTemplet(this);
			switch (m_STAGE_TYPE)
			{
			case STAGE_TYPE.ST_WARFARE:
				WarfareTemplet = NKMWarfareTemplet.Find(m_StageBattleStrID);
				if (WarfareTemplet == null)
				{
					NKMTempletError.Add($"[StageTemplet] 전역 정보가 존재하지 않음 m_EpisodeID:{episodeId} m_ActID:{actId} m_StageIndex:{m_StageIndex} m_StageBattleStrID:{m_StageBattleStrID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 476);
					return;
				}
				WarfareTemplet.StageTemplet = this;
				break;
			case STAGE_TYPE.ST_DUNGEON:
				DungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(m_StageBattleStrID);
				if (DungeonTempletBase == null)
				{
					NKMTempletError.Add($"[StageTemplet] 던전 정보가 존재하지 않음 m_EpisodeID:{episodeId} m_ActID:{actId} m_StageIndex:{m_StageIndex} m_StageBattleStrID:{m_StageBattleStrID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 488);
					return;
				}
				DungeonTempletBase.StageTemplet = this;
				break;
			case STAGE_TYPE.ST_PHASE:
				PhaseTemplet = NKMPhaseTemplet.Find(m_StageBattleStrID);
				if (PhaseTemplet == null)
				{
					NKMTempletError.Add($"[StageTemplet] 페이즈 정보가 존재하지 않음 m_EpisodeID:{episodeId} m_ActID:{actId} m_StageIndex:{m_StageIndex} m_StageBattleStrID:{m_StageBattleStrID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 499);
					return;
				}
				PhaseTemplet.StageTemplet = this;
				break;
			default:
				NKMTempletError.Add($"episode에서 지원하지 않는 stage type: {m_STAGE_TYPE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 507);
				break;
			}
			RestoreReqItem?.Join("/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 511);
			UnlockReqItem?.Join("/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 512);
		}
		else
		{
			NKMTempletError.Add($"EpisodeTemplet is null - StageID : {Key}, EpisodeID : {episodeId}, Difficulty : {m_Difficulty}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 466);
		}
	}

	public void Validate()
	{
		if (!NKMContentUnlockManager.IsValidMissionUnlockType(m_UnlockInfo))
		{
			NKMTempletError.Add($"[NKMStageTemplet] 해제조건이 유효하지 않음 m_EpisodeID:{episodeId} m_ActID:{actId} m_StageIndex:{m_StageIndex} m_UnlockReqType:{m_UnlockInfo.eReqType} m_UnlockReqValue:{m_UnlockInfo.reqValue} m_UnlockReqValueStr:{m_UnlockInfo.reqValueStr} m_UnlockDateTime:{m_UnlockInfo.reqDateTime}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 519);
		}
		if (m_STAGE_TYPE == STAGE_TYPE.ST_WARFARE)
		{
			if (m_StageReqItemID <= 0)
			{
				NKMTempletError.Add($"[NKMStageTemplet] m_StageReqItemId is empty, episodeId: {episodeId}, value: {m_StageReqItemID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 526);
			}
			if (m_StageReqItemCount <= 0)
			{
				NKMTempletError.Add($"[NKMStageTemplet] m_StageReqItemCount is empty, episodeId: {episodeId}, value: {m_StageReqItemCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 531);
			}
		}
		if (missionReward.rewardType != NKM_REWARD_TYPE.RT_NONE && !NKMRewardTemplet.IsValidReward(missionReward.rewardType, missionReward.ID))
		{
			NKMTempletError.Add($"[NKMStageTemplet] 도전과제 보상이 유효하지 않음 m_EpisodeID:{episodeId} rewardType:{missionReward.rewardType} ID:{missionReward.ID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 543);
		}
		NKMOnetimeRewardTemplet[] array = onetimeRewards;
		foreach (NKMOnetimeRewardTemplet nKMOnetimeRewardTemplet in array)
		{
			if (nKMOnetimeRewardTemplet != null && !nKMOnetimeRewardTemplet.IsValid())
			{
				NKMTempletError.Add($"[NKMStageTemplet] 일회성 보상 데이터가 유효하지 않음 m_EpisodeID:{episodeId} m_ActID:{actId} m_StageIndex:{m_StageIndex}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 550);
			}
		}
		if (m_EnterLimit > 0 && m_EnterLimitCond == RESET_TYPE.NONE)
		{
			NKMTempletError.Add($"[{DebugName}] 입장 횟수 제한이 설정되었으나 리셋 주기 값이 지정되지 않음. m_EnterLimitCond:{m_EnterLimitCond}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 556);
		}
		if (Restorable)
		{
			if (m_EnterLimit <= 0)
			{
				NKMTempletError.Add($"[{DebugName}] 입장 횟수 제한이 없으나 작전 복원 횟수 제한이 설정됨. m_EnterLimit:{m_EnterLimit} m_RestoreLimit:{m_RestoreLimit}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 563);
			}
			if (m_RestoreLimitEnterCount <= 0)
			{
				NKMTempletError.Add($"[{DebugName}] 작전 복원 횟수 제한이 설정되었으나 복원 횟수 값이 지정되지 않음. m_RestoreLimitEnterCount:{m_RestoreLimitEnterCount} m_RestoreLimit:{m_RestoreLimit}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 568);
			}
			else if (m_RestoreLimitEnterCount > m_EnterLimit)
			{
				NKMTempletError.Add($"[{DebugName}] 작전 복원 횟수가 입장 횟수 제한보다 큼. m_RestoreLimitEnterCount:{m_RestoreLimitEnterCount} m_EnterLimit:{m_EnterLimit}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 572);
			}
			if (RestoreReqItem == null)
			{
				NKMTempletError.Add($"[{DebugName}] 작전 복원 횟수 제한이 설정되었으나 복원 관련 값이 지정되지 않음. m_RestoreLimit:{m_RestoreLimit}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 577);
			}
		}
		else
		{
			if (RestoreReqItem != null)
			{
				NKMTempletError.Add($"[{DebugName}] 작전 복원 횟수 제한이 설정되지 않았으나 복원 아이템이 지정됨. m_RestoreLimit:{m_RestoreLimit} m_RestoreLimitReqItem:{RestoreReqItem}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 584);
			}
			if (m_RestoreLimitEnterCount > 0)
			{
				NKMTempletError.Add($"[{DebugName}] 작전 복원 횟수 제한이 설정되지 않았으나 복원 횟수가 지정됨. m_RestoreLimit:{m_RestoreLimit} m_RestoreLimitEnterCount:{m_RestoreLimitEnterCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 589);
			}
		}
		firstRewardData.Validate();
		MainRewardData?.Validate();
		if (EpisodeCategory == EPISODE_CATEGORY.EC_COUNTERCASE && UnlockReqItem != null && UnlockReqItem.ItemId != 3)
		{
			NKMTempletError.Add(DebugName + " 카운터케이스에 언락은 '정보'로만 설정 가능", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 605);
		}
		if (m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_TIMEATTACK)
		{
			bool flag = NKMDungeonManager.IsTutorialDungeon(DungeonTempletBase.m_DungeonID);
			if (m_STAGE_TYPE != STAGE_TYPE.ST_DUNGEON || flag)
			{
				NKMTempletError.Add($"[StageTemplet] 타임어택은 던전타입에서만 셋팅 가능. m_STAGE_TYPE: {m_STAGE_TYPE} isTutorialDungeon: {flag}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 615);
			}
		}
		if (m_StageReqItemID == 0 && m_StageReqItemCount == 0)
		{
			if (MainRewardData != null)
			{
				NKMTempletError.Add($"[StageTemplet:{Key}] 입장 재화가 없지만 MainReward 정보가 존재함. reqItemId:{m_StageReqItemID} reqItemCount:{m_StageReqItemCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 623);
			}
			if (DungeonTempletBase != null && (DungeonTempletBase.m_RewardUserEXP > 0 || DungeonTempletBase.m_RewardCredit_Min > 0))
			{
				NKMTempletError.Add($"[StageTemplet:{Key}] 입장 재화가 없지만 던전 보상이 존재함. DungeonId:{DungeonTempletBase.Key} m_RewardUserEXP:{DungeonTempletBase.m_RewardUserEXP} m_RewardCredit_Min:{DungeonTempletBase.m_RewardCredit_Min}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 630);
			}
		}
		if (m_bActiveBattleSkip && (m_StageReqItemID == 0 || m_StageReqItemCount < 0))
		{
			NKMTempletError.Add($"[StageTemplet:{Key}] 스킵 가능한 던전은 입장재화가 있어야합니다. DungeonId:{DungeonTempletBase.Key} m_StageReqItemID:{m_StageReqItemID} m_StageReqItemCount:{m_StageReqItemCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 637);
		}
	}

	public void ValidateServerOnly()
	{
		MainRewardData?.ValidateServerOnly();
		NKMOnetimeRewardTemplet[] array = onetimeRewards;
		for (int i = 0; i < array.Length; i++)
		{
			array[i]?.ValidateServerOnly();
		}
		foreach (var item in m_EventDrop)
		{
			if (item.prob <= 0)
			{
				NKMTempletError.Add($"[Stage] 이벤트드랍 확률 수치 이상 rewardGroupId:{item.rewardGroupId} prob:{item.prob}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMStageTempletV2.cs", 654);
			}
		}
	}

	public FirstRewardData GetFirstRewardData()
	{
		if (firstRewardData == null)
		{
			return FirstRewardData.Empty;
		}
		if (!NKMRewardTemplet.IsOpenedReward(firstRewardData.Type, firstRewardData.RewardId, useRandomContract: false))
		{
			return FirstRewardData.Empty;
		}
		return firstRewardData;
	}

	public NKMOnetimeRewardTemplet GetOneTimeReward(int index)
	{
		if (index < 0 || index >= onetimeRewards.Length)
		{
			return null;
		}
		return onetimeRewards[index];
	}

	public NKMDungeonEventDeckTemplet GetEventDeckTemplet()
	{
		return m_STAGE_TYPE switch
		{
			STAGE_TYPE.ST_PHASE => PhaseTemplet?.EventDeckTemplet, 
			STAGE_TYPE.ST_DUNGEON => DungeonTempletBase?.EventDeckTemplet, 
			_ => null, 
		};
	}

	public NKMDeckCondition GetDeckCondition()
	{
		return m_STAGE_TYPE switch
		{
			STAGE_TYPE.ST_PHASE => PhaseTemplet?.DeckCondition, 
			STAGE_TYPE.ST_DUNGEON => DungeonTempletBase?.m_DeckCondition, 
			_ => null, 
		};
	}

	public List<NKMBattleConditionTemplet> GetBattleConditions()
	{
		switch (m_STAGE_TYPE)
		{
		case STAGE_TYPE.ST_DUNGEON:
			return DungeonTempletBase?.BattleConditions;
		case STAGE_TYPE.ST_PHASE:
		{
			List<NKMBattleConditionTemplet> list = new List<NKMBattleConditionTemplet>();
			if (PhaseTemplet != null)
			{
				foreach (NKMPhaseOrderTemplet item in PhaseTemplet.PhaseList.List)
				{
					if (item.Dungeon == null || item.Dungeon.BattleConditions == null || item.Dungeon.BattleConditions.Count <= 0)
					{
						continue;
					}
					foreach (NKMBattleConditionTemplet battleConditionTemplet in item.Dungeon.BattleConditions)
					{
						if (list.FindIndex((NKMBattleConditionTemplet e) => e == battleConditionTemplet) == -1)
						{
							list.Add(battleConditionTemplet);
						}
					}
				}
			}
			return list;
		}
		default:
			return null;
		}
	}

	public List<int> GetPreConditionList()
	{
		switch (m_STAGE_TYPE)
		{
		case STAGE_TYPE.ST_DUNGEON:
			return DungeonTempletBase?.m_BCPreconditionGroups;
		case STAGE_TYPE.ST_PHASE:
		{
			HashSet<int> hashSet = new HashSet<int>();
			if (PhaseTemplet != null)
			{
				foreach (NKMPhaseOrderTemplet item in PhaseTemplet.PhaseList.List)
				{
					if (item.Dungeon != null && item.Dungeon.m_BCPreconditionGroups != null)
					{
						hashSet.UnionWith(item.Dungeon.m_BCPreconditionGroups);
					}
				}
			}
			return hashSet.ToList();
		}
		default:
			return null;
		}
	}

	public bool IsUsingEventDeck()
	{
		return GetEventDeckID() != 0;
	}

	public int GetEventDeckID()
	{
		if (m_STAGE_TYPE == STAGE_TYPE.ST_PHASE)
		{
			NKMPhaseTemplet phaseTemplet = PhaseTemplet;
			if (phaseTemplet != null)
			{
				return phaseTemplet.m_UseEventDeck;
			}
		}
		else
		{
			NKMDungeonTempletBase dungeonTempletBase = DungeonTempletBase;
			if (dungeonTempletBase != null)
			{
				return dungeonTempletBase.m_UseEventDeck;
			}
		}
		return 0;
	}

	public string GetDungeonName()
	{
		switch (m_STAGE_TYPE)
		{
		case STAGE_TYPE.ST_WARFARE:
			if (WarfareTemplet != null)
			{
				return WarfareTemplet.GetWarfareName();
			}
			break;
		case STAGE_TYPE.ST_PHASE:
			if (PhaseTemplet != null)
			{
				return PhaseTemplet.GetName();
			}
			break;
		default:
			if (DungeonTempletBase != null)
			{
				return DungeonTempletBase.GetDungeonName();
			}
			break;
		}
		return "";
	}

	public string GetDungeonDesc()
	{
		if (m_STAGE_TYPE == STAGE_TYPE.ST_PHASE)
		{
			if (PhaseTemplet != null)
			{
				return PhaseTemplet.GetDesc();
			}
		}
		else if (DungeonTempletBase != null)
		{
			return DungeonTempletBase.GetDungeonDesc();
		}
		return "";
	}

	public string GetStageDesc()
	{
		return NKCStringTable.GetString(m_StageDesc);
	}

	public int GetEventDropRewardGroupID(int index)
	{
		if (m_EventDrop == null || index >= m_EventDrop.Count || index < 0)
		{
			return -1;
		}
		return m_EventDrop[index].rewardGroupId;
	}

	public int GetBuffDropRewardGroupID()
	{
		if (CompanyBuffDropIndex <= 0 || CompanyBuffDropIds.Count <= 0)
		{
			return -1;
		}
		return CompanyBuffDropIndex;
	}

	public (NKM_UNIT_SOURCE_TYPE, NKM_UNIT_SOURCE_TYPE) GetSourceTypes()
	{
		switch (m_STAGE_TYPE)
		{
		case STAGE_TYPE.ST_PHASE:
		{
			if (PhaseTemplet == null || PhaseTemplet.PhaseList == null || PhaseTemplet.PhaseList.List == null)
			{
				break;
			}
			for (int i = 0; i < PhaseTemplet.PhaseList.List.Count; i++)
			{
				if (PhaseTemplet.PhaseList.List[i].Dungeon != null)
				{
					if (PhaseTemplet.PhaseList.List[i].Dungeon.m_StageSourceTypeMain != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
					{
						return (PhaseTemplet.PhaseList.List[i].Dungeon.m_StageSourceTypeMain, PhaseTemplet.PhaseList.List[i].Dungeon.m_StageSourceTypeSub);
					}
					if (PhaseTemplet.PhaseList.List[i].Dungeon.m_GuideSourceTypeMain != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
					{
						return (PhaseTemplet.PhaseList.List[i].Dungeon.m_GuideSourceTypeMain, PhaseTemplet.PhaseList.List[i].Dungeon.m_GuideSourceTypeSub);
					}
				}
			}
			break;
		}
		case STAGE_TYPE.ST_DUNGEON:
			if (DungeonTempletBase != null)
			{
				if (DungeonTempletBase.m_StageSourceTypeMain != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
				{
					return (DungeonTempletBase.m_StageSourceTypeMain, DungeonTempletBase.m_StageSourceTypeSub);
				}
				if (DungeonTempletBase.m_GuideSourceTypeMain != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
				{
					return (DungeonTempletBase.m_GuideSourceTypeMain, DungeonTempletBase.m_GuideSourceTypeSub);
				}
			}
			break;
		}
		return (NKM_UNIT_SOURCE_TYPE.NUST_NONE, NKM_UNIT_SOURCE_TYPE.NUST_NONE);
	}

	public NKCCutScenTemplet GetStageBeforeCutscen()
	{
		switch (m_STAGE_TYPE)
		{
		case STAGE_TYPE.ST_DUNGEON:
			if (DungeonTempletBase != null)
			{
				return NKCCutScenManager.GetCutScenTemple(DungeonTempletBase.m_CutScenStrIDBefore);
			}
			break;
		case STAGE_TYPE.ST_WARFARE:
			if (WarfareTemplet != null)
			{
				return NKCCutScenManager.GetCutScenTemple(WarfareTemplet.m_CutScenStrIDBefore);
			}
			break;
		case STAGE_TYPE.ST_PHASE:
			if (PhaseTemplet != null)
			{
				return NKCCutScenManager.GetCutScenTemple(PhaseTemplet.m_CutScenStrIDBefore);
			}
			break;
		}
		return null;
	}

	public NKCCutScenTemplet GetStageAfterCutscen()
	{
		switch (m_STAGE_TYPE)
		{
		case STAGE_TYPE.ST_DUNGEON:
			if (DungeonTempletBase != null)
			{
				return NKCCutScenManager.GetCutScenTemple(DungeonTempletBase.m_CutScenStrIDAfter);
			}
			break;
		case STAGE_TYPE.ST_WARFARE:
			if (WarfareTemplet != null)
			{
				return NKCCutScenManager.GetCutScenTemple(WarfareTemplet.m_CutScenStrIDAfter);
			}
			break;
		case STAGE_TYPE.ST_PHASE:
			if (PhaseTemplet != null)
			{
				return NKCCutScenManager.GetCutScenTemple(PhaseTemplet.m_CutScenStrIDAfter);
			}
			break;
		}
		return null;
	}

	public string GetStageBeforeCutscenStrID()
	{
		switch (m_STAGE_TYPE)
		{
		case STAGE_TYPE.ST_DUNGEON:
			if (DungeonTempletBase != null)
			{
				return DungeonTempletBase.m_CutScenStrIDBefore;
			}
			break;
		case STAGE_TYPE.ST_WARFARE:
			if (WarfareTemplet != null)
			{
				return WarfareTemplet.m_CutScenStrIDBefore;
			}
			break;
		case STAGE_TYPE.ST_PHASE:
			if (PhaseTemplet != null)
			{
				return PhaseTemplet.m_CutScenStrIDBefore;
			}
			break;
		}
		return null;
	}

	public string GetStageAfterCutscenStrID()
	{
		switch (m_STAGE_TYPE)
		{
		case STAGE_TYPE.ST_DUNGEON:
			if (DungeonTempletBase != null)
			{
				return DungeonTempletBase.m_CutScenStrIDAfter;
			}
			break;
		case STAGE_TYPE.ST_WARFARE:
			if (WarfareTemplet != null)
			{
				return WarfareTemplet.m_CutScenStrIDAfter;
			}
			break;
		case STAGE_TYPE.ST_PHASE:
			if (PhaseTemplet != null)
			{
				return PhaseTemplet.m_CutScenStrIDAfter;
			}
			break;
		}
		return null;
	}

	public bool IsOpenedDayOfWeek()
	{
		if (enterableDayOfWeeks.Count <= 0)
		{
			return true;
		}
		DateTime dateTime = DailyReset.CalcLastReset(NKCSynchronizedTime.ServiceTime);
		return enterableDayOfWeeks.Contains(dateTime.DayOfWeek);
	}

	public NKMStageTempletV2 GetPossibleNextOperation()
	{
		NKMEpisodeTempletV2 episodeTemplet = EpisodeTemplet;
		if (episodeTemplet != null && episodeTemplet.m_DicStage.ContainsKey(ActId))
		{
			bool flag = false;
			for (int i = 0; i < episodeTemplet.m_DicStage[ActId].Count; i++)
			{
				if (flag)
				{
					NKMStageTempletV2 nKMStageTempletV = episodeTemplet.m_DicStage[ActId][i];
					if (NKMEpisodeMgr.CheckEpisodeMission(NKCScenManager.CurrentUserData(), nKMStageTempletV))
					{
						return nKMStageTempletV;
					}
					return null;
				}
				if (episodeTemplet.m_DicStage[ActId][i].Key == Key)
				{
					flag = true;
				}
			}
		}
		return null;
	}

	public int GetStageLevel()
	{
		if (m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_TUTORIAL || m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_PRACTICE)
		{
			return 0;
		}
		switch (m_STAGE_TYPE)
		{
		case STAGE_TYPE.ST_DUNGEON:
			if (DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_CUTSCENE)
			{
				return 0;
			}
			return DungeonTempletBase.m_DungeonLevel;
		case STAGE_TYPE.ST_PHASE:
			return PhaseTemplet.PhaseLevel;
		case STAGE_TYPE.ST_WARFARE:
			return WarfareTemplet.m_WarfareLevel;
		default:
			return 0;
		}
	}
}
