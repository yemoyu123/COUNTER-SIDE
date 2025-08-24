using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKC;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public class NKMDungeonTempletBase : INKMTemplet, INKMTempletEx
{
	public enum NKM_WARFARE_GAME_UNIT_DIE_TYPE
	{
		NWGUDT_EXPLODE,
		NWGUDT_RUNAWAY
	}

	private const int MAX_SOURCE_TYPE_COUNT = 2;

	private readonly HashSet<NKMPhaseGroupTemplet> phaseReferences = new HashSet<NKMPhaseGroupTemplet>();

	public int m_DungeonID;

	public string m_DungeonStrID = "";

	public string m_DungeonTempletFileName = "";

	public string m_DungeonMapStrID = "";

	private List<string> m_BattleConditionStrIDs;

	public string m_MusicName = "";

	public float m_fGameTime = 180f;

	public float m_fDoubleCostTime = 60f;

	public NKM_DUNGEON_TYPE m_DungeonType = NKM_DUNGEON_TYPE.NDT_BOSS_KILL;

	public bool m_bNoShuffleDeck;

	public bool m_bDeckReuse = true;

	public int m_RespawnCountMaxSameTime;

	public int m_UseEventDeck;

	private bool m_bBonus_Resource;

	private string m_DungeonName = "";

	private string m_DungeonDesc = "";

	public string m_DungeonIcon = "";

	public int m_DungeonLevel = 1;

	public int m_DGRecommendFightPower;

	public int m_DGLimitUserLevel;

	public string m_CutScenStrIDBefore = "";

	public string m_CutScenStrIDAfter = "";

	public DUNGEON_GAME_MISSION_TYPE m_DGMissionType_1;

	public int m_DGMissionValue_1;

	public DUNGEON_GAME_MISSION_TYPE m_DGMissionType_2;

	public int m_DGMissionValue_2;

	public int m_RewardUserEXP;

	public int m_RewardUnitEXP;

	public int m_RewardCredit_Min;

	public int m_RewardCredit_Max;

	public int m_RewardEternium_Min;

	public int m_RewardEternium_Max;

	public int m_RewardInformation_Min;

	public int m_RewardInformation_Max;

	public int m_RewardUnitExp1Tier;

	public int m_RewardUnitExp2Tier;

	public int m_RewardUnitExp3Tier;

	public int m_RewardMultiplyMax = 1;

	public string m_Intro = "";

	public string m_Outro = "";

	public string m_EventRewardRateDateStrID;

	public List<DungeonReward> m_listDungeonReward = new List<DungeonReward>();

	public List<DungeonReward> m_EventListDungeonReward = new List<DungeonReward>();

	public List<int> m_BCPreconditionGroups = new List<int>();

	public NKM_UNIT_SOURCE_TYPE m_StageSourceTypeMain;

	public NKM_UNIT_SOURCE_TYPE m_StageSourceTypeSub;

	public NKM_UNIT_SOURCE_TYPE m_GuideSourceTypeMain;

	public NKM_UNIT_SOURCE_TYPE m_GuideSourceTypeSub;

	private List<NKM_UNIT_SOURCE_TYPE> m_list_Stage_Source_Type = new List<NKM_UNIT_SOURCE_TYPE>();

	private List<NKM_UNIT_SOURCE_TYPE> m_list_Guide_Source_Type = new List<NKM_UNIT_SOURCE_TYPE>();

	public NKMDeckCondition m_DeckCondition;

	public NKMIntervalTemplet m_EventRewardRateDate = NKMIntervalTemplet.Invalid;

	public NKM_WARFARE_GAME_UNIT_DIE_TYPE m_NKM_WARFARE_GAME_UNIT_DIE_TYPE;

	public List<NKMBattleConditionTemplet> BattleConditions = new List<NKMBattleConditionTemplet>();

	public List<int> ViewEventTagRewardGroup;

	public int Key => m_DungeonID;

	public string DebugName => $"[{m_DungeonID}] {m_DungeonStrID}";

	public NKMStageTempletV2 StageTemplet { get; internal set; }

	public string DungeonName => m_DungeonName;

	public bool BonusResult => m_bBonus_Resource;

	public bool IsPhaseDungeon => phaseReferences.Any();

	public NKMDungeonEventDeckTemplet EventDeckTemplet { get; private set; }

	public bool HasCutscen()
	{
		if (m_CutScenStrIDBefore.Length <= 0)
		{
			return m_CutScenStrIDAfter.Length > 0;
		}
		return true;
	}

	public bool IsUsingEventDeck()
	{
		return m_UseEventDeck != 0;
	}

	public static NKMDungeonTempletBase LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDungeonTempletBase.cs", 121))
		{
			return null;
		}
		NKMDungeonTempletBase nKMDungeonTempletBase = new NKMDungeonTempletBase();
		cNKMLua.GetData("m_DungeonID", ref nKMDungeonTempletBase.m_DungeonID);
		cNKMLua.GetData("m_DungeonStrID", ref nKMDungeonTempletBase.m_DungeonStrID);
		cNKMLua.GetData("m_DungeonTempletFileName", ref nKMDungeonTempletBase.m_DungeonTempletFileName);
		cNKMLua.GetData("m_DungeonMapStrID", ref nKMDungeonTempletBase.m_DungeonMapStrID);
		cNKMLua.GetDataList("m_BattleConditionStrID", out nKMDungeonTempletBase.m_BattleConditionStrIDs, nullIfEmpty: false);
		cNKMLua.GetData("EventRateDateStrID", ref nKMDungeonTempletBase.m_EventRewardRateDateStrID);
		cNKMLua.GetData("m_MusicAssetName", ref nKMDungeonTempletBase.m_MusicName);
		cNKMLua.GetData("m_fGameTime", ref nKMDungeonTempletBase.m_fGameTime);
		cNKMLua.GetData("m_fDoubleCostTime", ref nKMDungeonTempletBase.m_fDoubleCostTime);
		cNKMLua.GetData("m_DungeonType", ref nKMDungeonTempletBase.m_DungeonType);
		cNKMLua.GetData("m_bNoShuffleDeck", ref nKMDungeonTempletBase.m_bNoShuffleDeck);
		cNKMLua.GetData("m_bDeckReuse", ref nKMDungeonTempletBase.m_bDeckReuse);
		cNKMLua.GetData("m_RespawnCountMaxSameTime", ref nKMDungeonTempletBase.m_RespawnCountMaxSameTime);
		cNKMLua.GetData("m_UseEventDeck", ref nKMDungeonTempletBase.m_UseEventDeck);
		cNKMLua.GetData("m_bBonus_Resource", ref nKMDungeonTempletBase.m_bBonus_Resource);
		cNKMLua.GetData("m_DungeonName", ref nKMDungeonTempletBase.m_DungeonName);
		cNKMLua.GetData("m_DungeonDesc", ref nKMDungeonTempletBase.m_DungeonDesc);
		cNKMLua.GetData("m_DungeonIcon", ref nKMDungeonTempletBase.m_DungeonIcon);
		cNKMLua.GetData("m_DungeonLevel", ref nKMDungeonTempletBase.m_DungeonLevel);
		cNKMLua.GetData("m_DGRecommendFightPower", ref nKMDungeonTempletBase.m_DGRecommendFightPower);
		cNKMLua.GetData("m_DGLimitUserLevel", ref nKMDungeonTempletBase.m_DGLimitUserLevel);
		cNKMLua.GetData("m_CutScenStrIDBefore", ref nKMDungeonTempletBase.m_CutScenStrIDBefore);
		cNKMLua.GetData("m_CutScenStrIDAfter", ref nKMDungeonTempletBase.m_CutScenStrIDAfter);
		cNKMLua.GetData("m_DGMissionType_1", ref nKMDungeonTempletBase.m_DGMissionType_1);
		cNKMLua.GetData("m_DGMissionValue_1", ref nKMDungeonTempletBase.m_DGMissionValue_1);
		cNKMLua.GetData("m_DGMissionType_2", ref nKMDungeonTempletBase.m_DGMissionType_2);
		cNKMLua.GetData("m_DGMissionValue_2", ref nKMDungeonTempletBase.m_DGMissionValue_2);
		cNKMLua.GetData("m_RewardUserEXP", ref nKMDungeonTempletBase.m_RewardUserEXP);
		cNKMLua.GetData("m_RewardUnitEXP", ref nKMDungeonTempletBase.m_RewardUnitEXP);
		cNKMLua.GetData("m_RewardCredit_Min", ref nKMDungeonTempletBase.m_RewardCredit_Min);
		cNKMLua.GetData("m_RewardCredit_Max", ref nKMDungeonTempletBase.m_RewardCredit_Max);
		cNKMLua.GetData("m_RewardEternium_Min", ref nKMDungeonTempletBase.m_RewardEternium_Min);
		cNKMLua.GetData("m_RewardEternium_Max", ref nKMDungeonTempletBase.m_RewardEternium_Max);
		cNKMLua.GetData("m_RewardInformation_Min", ref nKMDungeonTempletBase.m_RewardInformation_Min);
		cNKMLua.GetData("m_RewardInformation_Max", ref nKMDungeonTempletBase.m_RewardInformation_Max);
		cNKMLua.GetData("m_RewardUnitExp1Tier", ref nKMDungeonTempletBase.m_RewardUnitExp1Tier);
		cNKMLua.GetData("m_RewardUnitExp2Tier", ref nKMDungeonTempletBase.m_RewardUnitExp2Tier);
		cNKMLua.GetData("m_RewardUnitExp3Tier", ref nKMDungeonTempletBase.m_RewardUnitExp3Tier);
		cNKMLua.GetData("m_NKM_WARFARE_GAME_UNIT_DIE_TYPE", ref nKMDungeonTempletBase.m_NKM_WARFARE_GAME_UNIT_DIE_TYPE);
		cNKMLua.GetData("m_RewardMultiplyMax", ref nKMDungeonTempletBase.m_RewardMultiplyMax);
		cNKMLua.GetData("m_Intro", ref nKMDungeonTempletBase.m_Intro);
		cNKMLua.GetData("m_Outro", ref nKMDungeonTempletBase.m_Outro);
		cNKMLua.GetDataListEnum("m_Stage_Source_Type", nKMDungeonTempletBase.m_list_Stage_Source_Type);
		if (nKMDungeonTempletBase.m_list_Stage_Source_Type.Count > 0)
		{
			nKMDungeonTempletBase.m_StageSourceTypeMain = nKMDungeonTempletBase.m_list_Stage_Source_Type[0];
		}
		if (nKMDungeonTempletBase.m_list_Stage_Source_Type.Count > 1)
		{
			nKMDungeonTempletBase.m_StageSourceTypeSub = nKMDungeonTempletBase.m_list_Stage_Source_Type[1];
		}
		cNKMLua.GetDataListEnum("m_Guide_Source_Type", nKMDungeonTempletBase.m_list_Guide_Source_Type);
		if (nKMDungeonTempletBase.m_list_Guide_Source_Type.Count > 0)
		{
			nKMDungeonTempletBase.m_GuideSourceTypeMain = nKMDungeonTempletBase.m_list_Guide_Source_Type[0];
		}
		if (nKMDungeonTempletBase.m_list_Guide_Source_Type.Count > 1)
		{
			nKMDungeonTempletBase.m_GuideSourceTypeSub = nKMDungeonTempletBase.m_list_Guide_Source_Type[1];
		}
		nKMDungeonTempletBase.m_listDungeonReward.Clear();
		nKMDungeonTempletBase.m_EventListDungeonReward.Clear();
		for (int i = 0; i < DungeonReward.MAX_REWARD_COUNT; i++)
		{
			DungeonReward dungeonReward = new DungeonReward();
			DungeonReward dungeonReward2 = dungeonReward;
			int rValue = 0;
			if (!cNKMLua.GetData($"m_RewardGroupID_{i + 1}", ref dungeonReward.m_RewardGroupID))
			{
				break;
			}
			if (cNKMLua.GetData($"m_fRewardEventRate_{i + 1}", ref rValue))
			{
				dungeonReward2 = new DungeonReward();
				dungeonReward2.m_RewardGroupID = dungeonReward.m_RewardGroupID;
				dungeonReward2.m_RewardRate = rValue;
			}
			cNKMLua.GetData($"m_fRewardRate_{i + 1}", ref dungeonReward.m_RewardRate);
			nKMDungeonTempletBase.m_listDungeonReward.Add(dungeonReward);
			nKMDungeonTempletBase.m_EventListDungeonReward.Add(dungeonReward2);
			if (dungeonReward.m_RewardRate > dungeonReward2.m_RewardRate)
			{
				NKMTempletError.Add($"[DungeonTempletBase:{nKMDungeonTempletBase.Key}] 이벤트로 확장된 이벤트 Rate가. 기존 값 보다 작음 기본 : {dungeonReward.m_RewardRate}, 확장 : {dungeonReward2.m_RewardRate}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDungeonTempletBase.cs", 231);
				break;
			}
		}
		cNKMLua.GetDataList("ViewEventTagRewardGroup", out nKMDungeonTempletBase.ViewEventTagRewardGroup, nullIfEmpty: false);
		cNKMLua.GetDataList("m_PreConditionGroup", out nKMDungeonTempletBase.m_BCPreconditionGroups, nullIfEmpty: false);
		NKMDeckCondition.LoadFromLua(cNKMLua, "DECK_CONDITION", out nKMDungeonTempletBase.m_DeckCondition, $"NKMDungeonDeckTempletBase DeckCondition Parse Fail. dungeonID : {nKMDungeonTempletBase.m_DungeonID}");
		nKMDungeonTempletBase.CheckValidation();
		return nKMDungeonTempletBase;
	}

	public FirstRewardData GetFirstRewardData()
	{
		return StageTemplet?.GetFirstRewardData() ?? FirstRewardData.Empty;
	}

	public List<DungeonReward> GetCurrentDungeonRewardList(DateTime currnet, bool useEventReward = false)
	{
		if (m_EventRewardRateDate == NKMIntervalTemplet.Invalid)
		{
			return m_listDungeonReward;
		}
		if (m_EventRewardRateDate.IsValidTime(currnet) || useEventReward)
		{
			return m_EventListDungeonReward;
		}
		return m_listDungeonReward;
	}

	public void JoinIntervalTemplet()
	{
		if (!string.IsNullOrEmpty(m_EventRewardRateDateStrID))
		{
			m_EventRewardRateDate = NKMIntervalTemplet.Find(m_EventRewardRateDateStrID);
			if (m_EventRewardRateDate == null)
			{
				NKMTempletError.Add($"[DungeonTempletBase : {Key}] 인터벌을 찾을 수 없습니다. m_DateStrID:{m_EventRewardRateDateStrID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDungeonTempletBase.cs", 279);
			}
		}
	}

	public void Join()
	{
		if (m_BattleConditionStrIDs.Count > 0)
		{
			foreach (string battleConditionStrID in m_BattleConditionStrIDs)
			{
				NKMBattleConditionTemplet templetByStrID = NKMBattleConditionManager.GetTempletByStrID(battleConditionStrID);
				if (templetByStrID == null)
				{
					Log.ErrorAndExit($"[{m_DungeonID}]{m_DungeonStrID} BattleCondition 값이 올바르지 않음: {templetByStrID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDungeonTempletBase.cs", 292);
				}
				else
				{
					BattleConditions.Add(templetByStrID);
				}
			}
		}
		if (m_UseEventDeck > 0)
		{
			EventDeckTemplet = NKMDungeonManager.GetEventDeckTemplet(m_UseEventDeck);
			if (EventDeckTemplet == null)
			{
				NKMTempletError.Add($"{DebugName} invalid eventDeckId:{m_UseEventDeck}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDungeonTempletBase.cs", 306);
			}
		}
		if (NKMUtil.IsServer)
		{
			JoinIntervalTemplet();
		}
	}

	public void Validate()
	{
		if (m_RewardCredit_Min < 0 || m_RewardCredit_Max < 0 || m_RewardCredit_Min > m_RewardCredit_Max)
		{
			NKMTempletError.Add($"[DungeonTempletBase] invalid reward credit range. [{m_RewardCredit_Min},{m_RewardCredit_Max}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDungeonTempletBase.cs", 321);
		}
		if (m_RewardEternium_Min < 0 || m_RewardEternium_Max < 0 || m_RewardEternium_Min > m_RewardEternium_Max)
		{
			NKMTempletError.Add($"[DungeonTempletBase] invalid reward eternium range. [{m_RewardEternium_Min}, {m_RewardEternium_Max}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDungeonTempletBase.cs", 327);
		}
		if (m_RewardInformation_Min < 0 || m_RewardInformation_Max < 0 || m_RewardInformation_Min > m_RewardInformation_Max)
		{
			NKMTempletError.Add($"[DungeonTempletBase] invalid reward infomation range. [{m_RewardInformation_Min}, {m_RewardInformation_Max}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDungeonTempletBase.cs", 333);
		}
		if (m_DungeonType == NKM_DUNGEON_TYPE.NDT_CUTSCENE && BonusResult)
		{
			NKMTempletError.Add($"[DungeonTempletBase] bonusResult[{BonusResult}] is invalid, CutScene dungeon must be bonusResult = false", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDungeonTempletBase.cs", 338);
		}
		if (m_RewardMultiplyMax <= 0)
		{
			NKMTempletError.Add($"[{Key}] {m_DungeonStrID} 보상 배수의 맥스치가 0이하 입니다. value: {m_RewardMultiplyMax}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDungeonTempletBase.cs", 343);
		}
		if (IsPhaseDungeon)
		{
			if (m_RewardCredit_Min > 0 || m_RewardCredit_Max > 0)
			{
				NKMTempletError.Add($"{DebugName} 페이즈 던전에 크레딧 보상이 설정되어 있음: {m_RewardCredit_Min} ~ {m_RewardCredit_Max}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDungeonTempletBase.cs", 350);
			}
			if (m_RewardEternium_Min > 0 || m_RewardEternium_Max > 0)
			{
				NKMTempletError.Add($"{DebugName} 페이즈 던전에 이터니움 보상이 설정되어 있음: {m_RewardEternium_Min} ~ {m_RewardEternium_Max}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDungeonTempletBase.cs", 355);
			}
			if (m_RewardInformation_Min > 0 || m_RewardInformation_Max > 0)
			{
				NKMTempletError.Add($"{DebugName} 페이즈 던전에 정보 보상이 설정되어 있음: {m_RewardInformation_Min} ~ {m_RewardInformation_Max}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDungeonTempletBase.cs", 360);
			}
			if (BonusResult)
			{
				NKMTempletError.Add(DebugName + " 페이즈 던전에 메달 보너스 비율이 설정되어 있음", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDungeonTempletBase.cs", 365);
			}
			if (m_RewardUserEXP > 0)
			{
				NKMTempletError.Add($"{DebugName} 페이즈 던전에 유저 경험치가 설정되어 있음:{m_RewardUserEXP}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDungeonTempletBase.cs", 370);
			}
			if (m_RewardUnitEXP > 0)
			{
				NKMTempletError.Add($"{DebugName} 페이즈 던전에 유닛 경험치가 설정되어 있음:{m_RewardUserEXP}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDungeonTempletBase.cs", 375);
			}
			if (m_RewardMultiplyMax > 1)
			{
				NKMTempletError.Add($"{DebugName} 페이즈 던전에 중첩작전 배율이 설정되어 있음:{m_RewardMultiplyMax}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDungeonTempletBase.cs", 380);
			}
			if (m_DGMissionType_1 != DUNGEON_GAME_MISSION_TYPE.DGMT_NONE || m_DGMissionType_2 != DUNGEON_GAME_MISSION_TYPE.DGMT_NONE)
			{
				NKMTempletError.Add($"{DebugName} 페이즈 던전에 중첩작전 미션이 설정되어 있음:{m_DGMissionType_1} {m_DGMissionType_2}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDungeonTempletBase.cs", 386);
			}
		}
		if (m_list_Stage_Source_Type != null && m_list_Stage_Source_Type.Count > 2)
		{
			NKMTempletError.Add($"{DebugName} m_Stage_Source_Type 근원성이 {m_list_Stage_Source_Type.Count}개 부여되어 있음", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMDungeonTempletBase.cs", 392);
		}
	}

	public void AddReference(NKMPhaseGroupTemplet phaseGroupTemplet)
	{
		phaseReferences.Add(phaseGroupTemplet);
	}

	public int DecideRewardCredit()
	{
		return PerThreadRandom.Instance.Next(m_RewardCredit_Min, m_RewardCredit_Max + 1);
	}

	public int DecideRewardEternium()
	{
		return PerThreadRandom.Instance.Next(m_RewardEternium_Min, m_RewardEternium_Max + 1);
	}

	public int DecideRewardInfomation()
	{
		return PerThreadRandom.Instance.Next(m_RewardInformation_Min, m_RewardInformation_Max + 1);
	}

	public string GetDungeonName()
	{
		if (!string.IsNullOrWhiteSpace(m_DungeonName))
		{
			return NKCStringTable.GetString(m_DungeonName);
		}
		return "";
	}

	public string GetDungeonDesc()
	{
		return NKCStringTable.GetString(m_DungeonDesc);
	}

	private void CheckValidation()
	{
		if (m_UseEventDeck > 0 && NKMDungeonManager.GetEventDeckTemplet(m_UseEventDeck) == null)
		{
			Log.ErrorAndExit($"[DungeonTempletBase] 이벤트 덱 정보가 존재하지 않음 m_DungeonID : {m_DungeonID}, useEventDeck : {m_UseEventDeck}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/Templet/NKMDungeonTempletBase.cs", 29);
		}
		if (m_DungeonType == NKM_DUNGEON_TYPE.NDT_CUTSCENE && m_CutScenStrIDAfter == "" && m_CutScenStrIDBefore == "")
		{
			Log.ErrorAndExit($"[DungeonTempletBase] 컷씬 던전인데 컷씬 정보가 입력되어 있지 않음 m_DungeonID : {m_DungeonID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/Templet/NKMDungeonTempletBase.cs", 37);
		}
		if (m_DungeonMapStrID != "" && NKMMapManager.GetMapTempletByStrID(m_DungeonMapStrID) == null)
		{
			Log.ErrorAndExit($"[DungeonTempletBase] 맵 정보가 존재하지 않음 m_DungeonID : {m_DungeonID}, m_DungeonMapStrID : {m_DungeonMapStrID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/Templet/NKMDungeonTempletBase.cs", 46);
		}
	}

	public void PostJoin()
	{
		JoinIntervalTemplet();
	}
}
