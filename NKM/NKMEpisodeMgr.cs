using System.Collections.Generic;
using System.Linq;
using ClientPacket.Common;
using Cs.Core.Util;
using Cs.Logging;
using NKC;
using NKC.Templet;
using NKC.UI;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKM;

public static class NKMEpisodeMgr
{
	private static readonly Dictionary<int, List<NKMStageTempletV2>> CounterCaseTemplets = new Dictionary<int, List<NKMStageTempletV2>>();

	private static Dictionary<EPISODE_CATEGORY, List<int>> m_dicEpisodeTabResource = new Dictionary<EPISODE_CATEGORY, List<int>>();

	private static Dictionary<EPISODE_CATEGORY, List<NKMEpisodeTempletV2>> m_dicListEPTempletByCategory = new Dictionary<EPISODE_CATEGORY, List<NKMEpisodeTempletV2>>();

	private static Dictionary<string, NKMStageTempletV2> m_dicStageTempletByBattleStrID = new Dictionary<string, NKMStageTempletV2>();

	private static List<int> m_lstUnlockedStageIds = new List<int>();

	private static Dictionary<int, NKMStageTempletV2> m_dicFavoriteStages = new Dictionary<int, NKMStageTempletV2>();

	private static Dictionary<int, NKMStageTempletV2> m_dicFavoriteStagesForEdit;

	public static IReadOnlyList<NKMEpisodeTempletV2> EpisodeTemplets { get; private set; }

	public static void LoadFromLUA(string fileName)
	{
		NKMTempletContainer<NKMStageTempletV2>.Load("AB_SCRIPT", fileName, "STAGE_TEMPLET", NKMStageTempletV2.LoadFromLUA, (NKMStageTempletV2 e) => e.StrId);
	}

	public static void Initialize()
	{
		foreach (NKMEpisodeTempletV2 value in NKMEpisodeTempletV2.Values)
		{
			AddCounterCaseTemplets(value);
		}
	}

	public static NKM_ERROR_CODE CanGetEpisodeCompleteReward(NKMUserData cNKMUserData, int episodeID)
	{
		NKM_ERROR_CODE nKM_ERROR_CODE = NKM_ERROR_CODE.NEC_OK;
		for (int i = 0; i <= 1; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				nKM_ERROR_CODE = CanGetEpisodeCompleteReward(cNKMUserData, episodeID, (EPISODE_DIFFICULTY)i, j);
				if (nKM_ERROR_CODE == NKM_ERROR_CODE.NEC_OK)
				{
					return nKM_ERROR_CODE;
				}
			}
		}
		return nKM_ERROR_CODE;
	}

	public static NKM_ERROR_CODE CanGetEpisodeCompleteReward(NKMUserData cNKMUserData, int episodeID, EPISODE_DIFFICULTY episodeDifficulty, int rewardIndex)
	{
		NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(episodeID, episodeDifficulty);
		if (nKMEpisodeTempletV == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_INVALID_EPISODE_ID;
		}
		if (rewardIndex < 0 || rewardIndex >= 3)
		{
			return NKM_ERROR_CODE.NEC_FAIL_EPISODE_COMPLETE_REWARD_INVALID_REWARD;
		}
		CompletionReward completionReward = nKMEpisodeTempletV.m_CompletionReward[rewardIndex];
		if (completionReward == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_EPISODE_COMPLETE_REWARD_INVALID_REWARD;
		}
		NKMEpisodeCompleteData episodeCompleteData = cNKMUserData.GetEpisodeCompleteData(episodeID, episodeDifficulty);
		if (episodeCompleteData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_EPISODE_COMPLETE_REWARD_NOT_ENOUGH_COUNT;
		}
		if (episodeCompleteData.m_bRewards[rewardIndex])
		{
			return NKM_ERROR_CODE.NEC_FAIL_EPISODE_COMPLETE_REWARD_ALREADY_GIVEN;
		}
		float num = (float)(GetTotalMedalCount(nKMEpisodeTempletV) * completionReward.m_CompleteRate) * 0.01f;
		if (episodeCompleteData.m_EpisodeCompleteCount < (int)num)
		{
			return NKM_ERROR_CODE.NEC_FAIL_EPISODE_COMPLETE_REWARD_NOT_ENOUGH_COUNT;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static List<NKMStageTempletV2> GetCounterCaseTemplets(int unitID)
	{
		CounterCaseTemplets.TryGetValue(unitID, out var value);
		return value;
	}

	public static List<NKMStageTempletV2> GetAllCounterCaseTemplets()
	{
		return CounterCaseTemplets.Values.SelectMany((List<NKMStageTempletV2> e) => e).ToList();
	}

	public static int GetTotalMedalCount(int episodeID)
	{
		int num = 0;
		for (int i = 0; i < 1; i++)
		{
			NKMEpisodeTempletV2 episodeTemplet = NKMEpisodeTempletV2.Find(episodeID, (EPISODE_DIFFICULTY)i);
			num += GetTotalMedalCount(episodeTemplet);
		}
		return num;
	}

	public static int GetTotalMedalCount(NKMEpisodeTempletV2 episodeTemplet)
	{
		int result = 0;
		if (episodeTemplet != null)
		{
			result = ((!episodeTemplet.m_HasCompleteReward) ? GetTotalClearCount(episodeTemplet) : GetTotalMedalCountByStageType(episodeTemplet));
		}
		return result;
	}

	private static int GetTotalMedalCountByStageType(NKMEpisodeTempletV2 episodeTemplet)
	{
		if (episodeTemplet == null)
		{
			return 0;
		}
		int num = 0;
		foreach (KeyValuePair<int, List<NKMStageTempletV2>> item in episodeTemplet.m_DicStage)
		{
			for (int i = 0; i < item.Value.Count; i++)
			{
				NKMStageTempletV2 nKMStageTempletV = item.Value[i];
				if (nKMStageTempletV.EnableByTag)
				{
					switch (nKMStageTempletV.m_STAGE_TYPE)
					{
					case STAGE_TYPE.ST_WARFARE:
						num += 3;
						break;
					case STAGE_TYPE.ST_DUNGEON:
					{
						NKMDungeonTempletBase dungeonTempletBase = nKMStageTempletV.DungeonTempletBase;
						num = ((dungeonTempletBase.m_DGMissionType_1 != DUNGEON_GAME_MISSION_TYPE.DGMT_NONE || dungeonTempletBase.m_DGMissionValue_1 != 0 || dungeonTempletBase.m_DGMissionType_2 != DUNGEON_GAME_MISSION_TYPE.DGMT_NONE || dungeonTempletBase.m_DGMissionValue_2 != 0) ? (num + 3) : (num + 1));
						break;
					}
					case STAGE_TYPE.ST_PHASE:
					{
						NKMPhaseTemplet phaseTemplet = nKMStageTempletV.PhaseTemplet;
						num = ((phaseTemplet.m_DGMissionType_1 != DUNGEON_GAME_MISSION_TYPE.DGMT_NONE || phaseTemplet.m_DGMissionValue_1 != 0 || phaseTemplet.m_DGMissionType_2 != DUNGEON_GAME_MISSION_TYPE.DGMT_NONE || phaseTemplet.m_DGMissionValue_2 != 0) ? (num + 3) : (num + 1));
						break;
					}
					default:
						Log.Warn($"Invalid STAGE_TYPE, StageID : {nKMStageTempletV.Key}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEpisodeMgr.cs", 207);
						break;
					}
				}
			}
		}
		return num;
	}

	private static int GetTotalClearCount(NKMEpisodeTempletV2 episodeTemplet)
	{
		int num = 0;
		foreach (KeyValuePair<int, List<NKMStageTempletV2>> item in episodeTemplet.m_DicStage)
		{
			for (int i = 0; i < item.Value.Count; i++)
			{
				if (item.Value[i].EnableByTag)
				{
					num++;
				}
			}
		}
		return num;
	}

	private static bool IsCounterCaseUnlockType(STAGE_UNLOCK_REQ_TYPE type)
	{
		if ((uint)(type - 2) <= 9u)
		{
			return true;
		}
		return false;
	}

	private static void AddCounterCaseTemplets(NKMEpisodeTempletV2 episodeTemplet)
	{
		if (episodeTemplet.m_EPCategory != EPISODE_CATEGORY.EC_COUNTERCASE)
		{
			return;
		}
		foreach (KeyValuePair<int, List<NKMStageTempletV2>> item in episodeTemplet.m_DicStage)
		{
			for (int i = 0; i < item.Value.Count; i++)
			{
				NKMStageTempletV2 nKMStageTempletV = item.Value[i];
				if (!IsCounterCaseUnlockType(nKMStageTempletV.m_UnlockInfo.eReqType))
				{
					continue;
				}
				int reqValue = nKMStageTempletV.m_UnlockInfo.reqValue;
				if (reqValue > 0)
				{
					if (!CounterCaseTemplets.TryGetValue(reqValue, out var value))
					{
						value = new List<NKMStageTempletV2>();
						CounterCaseTemplets.Add(reqValue, value);
					}
					value.Add(nKMStageTempletV);
				}
			}
		}
	}

	public static void LoadClientOnlyData()
	{
		m_dicListEPTempletByCategory = (from episode in NKMEpisodeTempletV2.Values
			group episode by episode.m_EPCategory).ToDictionary((IGrouping<EPISODE_CATEGORY, NKMEpisodeTempletV2> group) => group.Key, (IGrouping<EPISODE_CATEGORY, NKMEpisodeTempletV2> group) => group.ToList());
		m_dicStageTempletByBattleStrID = NKMTempletContainer<NKMStageTempletV2>.Values.ToDictionary((NKMStageTempletV2 stage) => stage.m_StageBattleStrID);
		EpisodeTemplets = NKMEpisodeTempletV2.Values.ToList();
		UpdateUnlockInfoDateToIntervalTime();
	}

	public static void SortEpisodeTemplets()
	{
		foreach (KeyValuePair<EPISODE_CATEGORY, List<NKMEpisodeTempletV2>> item in m_dicListEPTempletByCategory)
		{
			item.Value.Sort(EpisodeComp);
		}
		static int EpisodeComp(NKMEpisodeTempletV2 a, NKMEpisodeTempletV2 b)
		{
			int num = a.m_SortIndex.CompareTo(b.m_SortIndex);
			if (num != 0)
			{
				return num;
			}
			return a.m_EpisodeID.CompareTo(b.m_EpisodeID);
		}
	}

	public static void UpdateUnlockInfoDateToIntervalTime()
	{
		foreach (NKMStageTempletV2 value in NKMTempletContainer<NKMStageTempletV2>.Values)
		{
			if (IsIntervalType(value.m_UnlockInfo.eReqType))
			{
				NKMIntervalTemplet nKMIntervalTemplet = NKMIntervalTemplet.Find(value.m_UnlockInfo.reqValueStr);
				if (nKMIntervalTemplet != null)
				{
					value.m_UnlockInfo = new UnlockInfo(value.m_UnlockInfo.eReqType, value.m_UnlockInfo.reqValue, value.m_UnlockInfo.reqValueStr, nKMIntervalTemplet.GetStartDateUtc());
				}
			}
		}
	}

	private static bool IsIntervalType(STAGE_UNLOCK_REQ_TYPE type)
	{
		if ((uint)(type - 41) <= 3u)
		{
			return true;
		}
		return false;
	}

	public static List<NKMEpisodeTempletV2> GetListNKMEpisodeTempletByCategory(EPISODE_CATEGORY category, bool bOnlyOpen = false, EPISODE_DIFFICULTY maxDifficulty = EPISODE_DIFFICULTY.NORMAL)
	{
		if (m_dicListEPTempletByCategory.TryGetValue(category, out var value))
		{
			if (bOnlyOpen)
			{
				return value.FindAll((NKMEpisodeTempletV2 v) => v.IsOpen && v.IsOpenedDayOfWeek() && v.m_Difficulty <= maxDifficulty);
			}
			return value.FindAll((NKMEpisodeTempletV2 v) => v.EnableByTag && v.m_Difficulty <= maxDifficulty);
		}
		return new List<NKMEpisodeTempletV2>();
	}

	public static float GetEPProgressPercent(NKMUserData cNKMUserData, NKMEpisodeTempletV2 episodeTemplet)
	{
		int ePProgressTotalCount = GetEPProgressTotalCount(episodeTemplet);
		int ePProgressClearCount = GetEPProgressClearCount(cNKMUserData, episodeTemplet);
		if (ePProgressTotalCount <= 0)
		{
			return 0f;
		}
		float num = ePProgressClearCount;
		for (int i = 0; i < 3 && i < episodeTemplet.m_CompletionReward.Length && episodeTemplet.m_CompletionReward[i] != null; i++)
		{
			float num2 = (float)ePProgressTotalCount * ((float)episodeTemplet.m_CompletionReward[i].m_CompleteRate * 0.01f);
			if (ePProgressClearCount < (int)num2)
			{
				break;
			}
			num = num2;
		}
		if ((float)ePProgressClearCount < num)
		{
			return num / (float)ePProgressTotalCount;
		}
		return (float)ePProgressClearCount / (float)ePProgressTotalCount;
	}

	public static bool IsPossibleEpisode(NKMUserData cNKMUserData, NKMEpisodeTempletV2 episodeTemplet)
	{
		if (cNKMUserData == null || episodeTemplet == null)
		{
			return false;
		}
		if (!episodeTemplet.EnableByTag)
		{
			return false;
		}
		if (!episodeTemplet.IsOpen)
		{
			return false;
		}
		if (episodeTemplet.m_EPCategory == EPISODE_CATEGORY.EC_COUNTERCASE)
		{
			if (episodeTemplet.m_EpisodeStrID == "NKM_DUNGEON_CC_SECRET")
			{
				return false;
			}
			return true;
		}
		NKMStageTempletV2 firstStage = episodeTemplet.GetFirstStage(1);
		if (firstStage == null)
		{
			return false;
		}
		if (!firstStage.EnableByTag)
		{
			return false;
		}
		switch (episodeTemplet.m_EPCategory)
		{
		case EPISODE_CATEGORY.EC_MAINSTREAM:
		case EPISODE_CATEGORY.EC_DAILY:
		case EPISODE_CATEGORY.EC_SIDESTORY:
		case EPISODE_CATEGORY.EC_FIELD:
		case EPISODE_CATEGORY.EC_EVENT:
		case EPISODE_CATEGORY.EC_SUPPLY:
		case EPISODE_CATEGORY.EC_CHALLENGE:
		case EPISODE_CATEGORY.EC_TIMEATTACK:
		case EPISODE_CATEGORY.EC_SEASONAL:
			return CheckEpisodeMission(cNKMUserData, firstStage);
		default:
			return false;
		}
	}

	public static bool IsPossibleEpisode(NKMUserData cNKMUserData, int episodeID, EPISODE_DIFFICULTY difficulty)
	{
		NKMEpisodeTempletV2 episodeTemplet = NKMEpisodeTempletV2.Find(episodeID, difficulty);
		return IsPossibleEpisode(cNKMUserData, episodeTemplet);
	}

	public static bool CheckLockCounterCase(NKMUserData cNKMUserData, NKMEpisodeTempletV2 episodeTemplet, int actID)
	{
		if (episodeTemplet == null)
		{
			return true;
		}
		if (cNKMUserData == null)
		{
			return true;
		}
		for (int i = 0; i < episodeTemplet.m_DicStage[actID].Count; i++)
		{
			if (CheckEpisodeMission(cNKMUserData, episodeTemplet.m_DicStage[actID][i]))
			{
				return false;
			}
		}
		return true;
	}

	public static bool CheckPossibleAct(NKMUserData cNKMUserData, int episodeID, int actID, EPISODE_DIFFICULTY difficulty = EPISODE_DIFFICULTY.NORMAL)
	{
		if (cNKMUserData == null)
		{
			return false;
		}
		NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(episodeID, difficulty);
		if (nKMEpisodeTempletV == null)
		{
			return false;
		}
		if (nKMEpisodeTempletV.GetFirstStage(actID) != null)
		{
			return CheckEpisodeMission(cNKMUserData, nKMEpisodeTempletV.GetFirstStage(actID));
		}
		return false;
	}

	public static bool CheckOpenedAct(int episodeID, int actID)
	{
		NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(episodeID, EPISODE_DIFFICULTY.NORMAL);
		if (nKMEpisodeTempletV == null)
		{
			return false;
		}
		if (nKMEpisodeTempletV.m_DicStage.ContainsKey(actID) && nKMEpisodeTempletV.m_DicStage[actID] != null && nKMEpisodeTempletV.m_DicStage[actID].Count >= 1)
		{
			return nKMEpisodeTempletV.m_DicStage[actID][0].EnableByTag;
		}
		return false;
	}

	public static List<int> GetEPTabLstID(EPISODE_CATEGORY epCate)
	{
		List<int> result = new List<int>();
		if (m_dicEpisodeTabResource.ContainsKey(epCate))
		{
			result = m_dicEpisodeTabResource[epCate];
		}
		return result;
	}

	public static int GetUnitID(NKMEpisodeTempletV2 episodeTemplet, int actID)
	{
		if (episodeTemplet == null)
		{
			return 0;
		}
		if (episodeTemplet.m_DicStage != null && episodeTemplet.m_DicStage[actID].Count > 0)
		{
			return episodeTemplet.m_DicStage[actID][0].m_UnlockInfo.reqValue;
		}
		return 0;
	}

	public static bool CheckClear(NKMUserData cNKMUserData, NKMEpisodeTempletV2 epTemplet)
	{
		foreach (KeyValuePair<int, List<NKMStageTempletV2>> item in epTemplet.m_DicStage)
		{
			for (int i = 0; i < item.Value.Count; i++)
			{
				if (!CheckClear(cNKMUserData, item.Value[i]))
				{
					return false;
				}
			}
		}
		return true;
	}

	public static bool CheckClear(NKMUserData cNKMUserData, NKMStageTempletV2 stageTemplet)
	{
		if (stageTemplet == null || cNKMUserData == null)
		{
			return false;
		}
		return stageTemplet.m_STAGE_TYPE switch
		{
			STAGE_TYPE.ST_WARFARE => cNKMUserData.CheckWarfareClear(stageTemplet.m_StageBattleStrID), 
			STAGE_TYPE.ST_DUNGEON => cNKMUserData.CheckDungeonClear(stageTemplet.m_StageBattleStrID), 
			STAGE_TYPE.ST_PHASE => NKCPhaseManager.CheckPhaseStageClear(stageTemplet), 
			_ => false, 
		};
	}

	public static bool GetMedalAllClear(NKMUserData userData, NKMStageTempletV2 stageTemplet)
	{
		switch (stageTemplet.m_STAGE_TYPE)
		{
		case STAGE_TYPE.ST_WARFARE:
		{
			if (stageTemplet.WarfareTemplet == null)
			{
				return false;
			}
			NKMWarfareClearData warfareClearData = userData.GetWarfareClearData(stageTemplet.WarfareTemplet.m_WarfareID);
			if (warfareClearData != null)
			{
				if (warfareClearData.m_mission_result_1 && warfareClearData.m_mission_result_2)
				{
					return warfareClearData.m_MissionRewardResult;
				}
				return false;
			}
			break;
		}
		case STAGE_TYPE.ST_DUNGEON:
		{
			if (stageTemplet.DungeonTempletBase == null)
			{
				return false;
			}
			NKMDungeonClearData dungeonClearData = userData.GetDungeonClearData(stageTemplet.DungeonTempletBase.m_DungeonID);
			if (dungeonClearData != null)
			{
				bool flag2 = true;
				if (stageTemplet.DungeonTempletBase.m_DGMissionType_1 != DUNGEON_GAME_MISSION_TYPE.DGMT_NONE)
				{
					flag2 &= dungeonClearData.missionResult1;
				}
				if (stageTemplet.DungeonTempletBase.m_DGMissionType_2 != DUNGEON_GAME_MISSION_TYPE.DGMT_NONE)
				{
					flag2 &= dungeonClearData.missionResult2;
				}
				return flag2;
			}
			break;
		}
		case STAGE_TYPE.ST_PHASE:
		{
			if (stageTemplet.PhaseTemplet == null)
			{
				return false;
			}
			NKMPhaseClearData phaseClearData = NKCPhaseManager.GetPhaseClearData(stageTemplet.PhaseTemplet);
			if (phaseClearData != null)
			{
				bool flag = true;
				if (stageTemplet.PhaseTemplet.m_DGMissionType_1 != DUNGEON_GAME_MISSION_TYPE.DGMT_NONE)
				{
					flag &= phaseClearData.missionResult1;
				}
				if (stageTemplet.PhaseTemplet.m_DGMissionType_2 != DUNGEON_GAME_MISSION_TYPE.DGMT_NONE)
				{
					flag &= phaseClearData.missionResult2;
				}
				return flag;
			}
			break;
		}
		}
		return false;
	}

	public static int GetEPProgressTotalCount(NKMEpisodeTempletV2 episodeTemplet)
	{
		if (episodeTemplet == null)
		{
			return 0;
		}
		return GetTotalMedalCount(episodeTemplet);
	}

	public static int GetEPProgressClearCount(int episodeID)
	{
		int num = 0;
		NKMUserData cNKMUserData = NKCScenManager.CurrentUserData();
		for (int i = 0; i <= 1; i++)
		{
			NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(episodeID, (EPISODE_DIFFICULTY)i);
			if (nKMEpisodeTempletV != null)
			{
				num += GetEPProgressClearCount(cNKMUserData, nKMEpisodeTempletV);
			}
		}
		return num;
	}

	public static int GetEPProgressClearCount(NKMUserData cNKMUserData, NKMEpisodeTempletV2 episodeTemplet)
	{
		int result = 0;
		NKMEpisodeCompleteData episodeCompleteData = cNKMUserData.GetEpisodeCompleteData(episodeTemplet.m_EpisodeID, episodeTemplet.m_Difficulty);
		if (episodeCompleteData != null)
		{
			result = episodeCompleteData.m_EpisodeCompleteCount;
		}
		return result;
	}

	public static NKMStageTempletV2 FindStageTemplet(int episodeID, int actID, int stageIndex, EPISODE_DIFFICULTY episodeDifficulty = EPISODE_DIFFICULTY.NORMAL)
	{
		NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(episodeID, episodeDifficulty);
		if (nKMEpisodeTempletV == null)
		{
			return null;
		}
		if (nKMEpisodeTempletV.m_DicStage.Count <= actID)
		{
			return null;
		}
		if (stageIndex <= 0)
		{
			return null;
		}
		if (stageIndex <= nKMEpisodeTempletV.m_DicStage[actID].Count)
		{
			return nKMEpisodeTempletV.m_DicStage[actID][stageIndex - 1];
		}
		return null;
	}

	public static NKMStageTempletV2 FindStageTempletByBattleStrID(string missionStrID)
	{
		m_dicStageTempletByBattleStrID.TryGetValue(missionStrID, out var value);
		return value;
	}

	public static string GetEpisodeBattleName(string battleStrID)
	{
		return GetEpisodeBattleName(FindStageTempletByBattleStrID(battleStrID));
	}

	public static string GetEpisodeBattleName(int stageID)
	{
		return GetEpisodeBattleName(NKMStageTempletV2.Find(stageID));
	}

	public static string GetEpisodeBattleName(NKMStageTempletV2 cNKMStageTemplet)
	{
		string result = "";
		if (cNKMStageTemplet == null)
		{
			return result;
		}
		switch (cNKMStageTemplet.m_STAGE_TYPE)
		{
		case STAGE_TYPE.ST_WARFARE:
		{
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(cNKMStageTemplet.m_StageBattleStrID);
			if (nKMWarfareTemplet != null)
			{
				result = nKMWarfareTemplet.GetWarfareName();
			}
			break;
		}
		case STAGE_TYPE.ST_DUNGEON:
		{
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(cNKMStageTemplet.m_StageBattleStrID);
			if (dungeonTempletBase != null)
			{
				result = dungeonTempletBase.GetDungeonName();
			}
			break;
		}
		case STAGE_TYPE.ST_PHASE:
		{
			NKMPhaseTemplet nKMPhaseTemplet = NKMPhaseTemplet.Find(cNKMStageTemplet.m_StageBattleStrID);
			if (nKMPhaseTemplet != null)
			{
				result = nKMPhaseTemplet.GetName();
			}
			break;
		}
		}
		return result;
	}

	public static int GetBattleID(NKMStageTempletV2 cNKMStageTemplet)
	{
		if (cNKMStageTemplet == null)
		{
			return 0;
		}
		switch (cNKMStageTemplet.m_STAGE_TYPE)
		{
		case STAGE_TYPE.ST_WARFARE:
		{
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(cNKMStageTemplet.m_StageBattleStrID);
			if (nKMWarfareTemplet != null)
			{
				return nKMWarfareTemplet.Key;
			}
			break;
		}
		case STAGE_TYPE.ST_DUNGEON:
		{
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(cNKMStageTemplet.m_StageBattleStrID);
			if (dungeonTempletBase != null)
			{
				return dungeonTempletBase.Key;
			}
			break;
		}
		case STAGE_TYPE.ST_PHASE:
		{
			NKMPhaseTemplet nKMPhaseTemplet = NKMPhaseTemplet.Find(cNKMStageTemplet.m_StageBattleStrID);
			if (nKMPhaseTemplet != null)
			{
				return nKMPhaseTemplet.Key;
			}
			break;
		}
		}
		return 0;
	}

	public static int GetDailyMissionTicketID(int episodeID)
	{
		return episodeID switch
		{
			101 => 15, 
			103 => 16, 
			102 => 17, 
			_ => -1, 
		};
	}

	public static bool CheckEpisodeMission(NKMUserData cNKMUserData, NKMStageTempletV2 stageTemplet)
	{
		if (!stageTemplet.EnableByTag)
		{
			return false;
		}
		return NKMContentUnlockManager.IsContentUnlocked(cNKMUserData, in stageTemplet.m_UnlockInfo);
	}

	public static bool CheckEpisodeHasEventDrop(NKMEpisodeTempletV2 episodeTemplet)
	{
		return episodeTemplet?.HaveEventDrop ?? false;
	}

	public static bool CheckStageHasEventDrop(NKMStageTempletV2 stageTemplet)
	{
		if (stageTemplet == null)
		{
			return false;
		}
		bool result = false;
		if (stageTemplet.m_EventDrop != null)
		{
			int count = stageTemplet.m_EventDrop.Count;
			for (int i = 0; i < count; i++)
			{
				NKMRewardGroupTemplet rewardGroup = NKMRewardManager.GetRewardGroup(stageTemplet.m_EventDrop[i].rewardGroupId);
				if (rewardGroup == null)
				{
					continue;
				}
				int count2 = rewardGroup.List.Count;
				for (int j = 0; j < count2; j++)
				{
					if (rewardGroup.List[j].intervalTemplet.IsValidTime(NKCSynchronizedTime.ServiceTime))
					{
						result = true;
					}
				}
			}
		}
		return result;
	}

	public static bool CheckEpisodeHasBuffDrop(NKMEpisodeTempletV2 episodeTemplet)
	{
		return episodeTemplet?.HaveBuffDrop ?? false;
	}

	public static bool CheckStageHasBuffDrop(NKMStageTempletV2 stageTemplet)
	{
		if (stageTemplet == null)
		{
			return false;
		}
		bool result = false;
		if (stageTemplet.CompanyBuffDropIndex > 0 && stageTemplet.CompanyBuffDropIds.Count > 0)
		{
			NKMRewardGroupTemplet rewardGroup = NKMRewardManager.GetRewardGroup(stageTemplet.CompanyBuffDropIndex);
			if (rewardGroup != null && NKCCompanyBuffManager.IsCurrentApplyBuff(stageTemplet.CompanyBuffDropIds))
			{
				for (int i = 0; i < rewardGroup.List.Count; i++)
				{
					if (rewardGroup.List[i].intervalTemplet.IsValidTime(NKCSynchronizedTime.ServiceTime))
					{
						result = true;
						break;
					}
				}
			}
		}
		return result;
	}

	public static void SetUnlockedStage(int unlockedStageId)
	{
		if (!m_lstUnlockedStageIds.Contains(unlockedStageId))
		{
			m_lstUnlockedStageIds.Add(unlockedStageId);
		}
	}

	public static void SetUnlockedStage(List<int> lstStageIds)
	{
		m_lstUnlockedStageIds = lstStageIds;
	}

	public static List<int> GetUnlockedStageIds()
	{
		return m_lstUnlockedStageIds;
	}

	public static bool IsUnlockedStage(NKMStageTempletV2 stageTemplet)
	{
		if (stageTemplet.NeedToUnlock)
		{
			return m_lstUnlockedStageIds.Contains(stageTemplet.Key);
		}
		return true;
	}

	public static void DoAfterLogOut()
	{
		m_lstUnlockedStageIds.Clear();
	}

	public static bool HasHardDifficulty(int episodeID)
	{
		return NKMEpisodeTempletV2.Find(episodeID, EPISODE_DIFFICULTY.HARD) != null;
	}

	public static bool HasEnoughResource(NKMStageTempletV2 stageTemplet, int m_LastMultiplyRewardCount = 1)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		int costItemID = 0;
		int num = 0;
		costItemID = stageTemplet.m_StageReqItemID;
		num = stageTemplet.m_StageReqItemCount;
		if (stageTemplet.m_StageReqItemID == 2)
		{
			NKCCompanyBuff.SetDiscountOfEterniumInEnteringDungeon(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref num);
		}
		int realCostCount = num * m_LastMultiplyRewardCount;
		if (!myUserData.CheckPrice(realCostCount, costItemID))
		{
			if (!NKCAdManager.IsAdRewardItem(costItemID))
			{
				NKCShopManager.OpenItemLackPopup(costItemID, realCostCount);
			}
			else
			{
				NKCPopupItemLack.Instance.OpenItemLackAdRewardPopup(costItemID, delegate
				{
					NKCShopManager.OpenItemLackPopup(costItemID, realCostCount);
				});
			}
			return false;
		}
		return true;
	}

	public static bool IsClearedEpisode(NKMEpisodeTempletV2 epTemplet)
	{
		if (epTemplet == null)
		{
			return false;
		}
		if (!epTemplet.IsOpen)
		{
			return false;
		}
		if (epTemplet.m_DicStage == null || epTemplet.m_DicStage.Count == 0)
		{
			return false;
		}
		foreach (KeyValuePair<int, List<NKMStageTempletV2>> item in epTemplet.m_DicStage)
		{
			for (int i = 0; i < item.Value.Count; i++)
			{
				if (item.Value[i].EnableByTag && !NKCScenManager.CurrentUserData().CheckStageCleared(item.Value[i]))
				{
					return false;
				}
			}
		}
		return true;
	}

	public static bool HasReddot(EPISODE_GROUP epGroup)
	{
		List<NKMEpisodeGroupTemplet> list = new List<NKMEpisodeGroupTemplet>();
		foreach (NKMEpisodeGroupTemplet value in NKMTempletContainer<NKMEpisodeGroupTemplet>.Values)
		{
			if (value.GroupCategory == epGroup)
			{
				list.Add(value);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			for (int j = 0; j < list[i].lstEpisodeTemplet.Count; j++)
			{
				if (HasReddot(list[i].lstEpisodeTemplet[j].m_EpisodeID))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool HasReddot(int episodeID, EPISODE_DIFFICULTY difficulty, int actID = 0)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		foreach (KeyValuePair<int, List<NKMStageTempletV2>> item in NKMEpisodeTempletV2.Find(episodeID, difficulty).m_DicStage)
		{
			if (actID > 0 && item.Key != actID)
			{
				continue;
			}
			for (int i = 0; i < item.Value.Count && NKMContentUnlockManager.IsContentUnlocked(nKMUserData, in item.Value[i].m_UnlockInfo); i++)
			{
				if (!PlayerPrefs.HasKey($"{nKMUserData.m_UserUID}_{item.Value[i].m_StageBattleStrID}") && !nKMUserData.CheckStageCleared(item.Value[i]))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool HasReddot(int episodeID)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		for (int i = 0; i <= 1; i++)
		{
			NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(episodeID, (EPISODE_DIFFICULTY)i);
			if (nKMEpisodeTempletV == null || !IsPossibleEpisode(nKMUserData, nKMEpisodeTempletV.m_EpisodeID, EPISODE_DIFFICULTY.NORMAL) || !NKMContentUnlockManager.IsContentUnlocked(nKMUserData, nKMEpisodeTempletV.GetUnlockInfo()))
			{
				continue;
			}
			if (CanGetEpisodeCompleteReward(nKMUserData, nKMEpisodeTempletV.m_EpisodeID) == NKM_ERROR_CODE.NEC_OK)
			{
				return true;
			}
			foreach (KeyValuePair<int, List<NKMStageTempletV2>> item in nKMEpisodeTempletV.m_DicStage)
			{
				for (int j = 0; j < item.Value.Count; j++)
				{
					NKMStageTempletV2 nKMStageTempletV = item.Value[j];
					if (!NKMContentUnlockManager.IsContentUnlocked(nKMUserData, in nKMStageTempletV.m_UnlockInfo))
					{
						break;
					}
					if (!PlayerPrefs.HasKey($"{nKMUserData.m_UserUID}_{nKMStageTempletV.m_StageBattleStrID}") && !nKMUserData.CheckStageCleared(nKMStageTempletV))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public static bool IsChangedFavoriteStage()
	{
		Dictionary<int, NKMStageTempletV2> favoriteStageListForEdit = GetFavoriteStageListForEdit();
		if (favoriteStageListForEdit == null)
		{
			return false;
		}
		Dictionary<int, NKMStageTempletV2> favoriteStageList = GetFavoriteStageList();
		if (favoriteStageList == null)
		{
			return false;
		}
		List<NKMStageTempletV2> list = favoriteStageListForEdit.Values.ToList();
		List<NKMStageTempletV2> list2 = favoriteStageList.Values.ToList();
		for (int i = 0; i < list.Count; i++)
		{
			NKMStageTempletV2 nKMStageTempletV = list2[i];
			NKMStageTempletV2 nKMStageTempletV2 = list[i];
			if (nKMStageTempletV.Key != nKMStageTempletV2.Key)
			{
				return true;
			}
		}
		return false;
	}

	public static void Send_NKMPacket_FAVORITES_STAGE_UPDATE_REQ()
	{
		if (!IsChangedFavoriteStage())
		{
			return;
		}
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		foreach (KeyValuePair<int, NKMStageTempletV2> item in GetFavoriteStageListForEdit())
		{
			dictionary.Add(item.Key, item.Value.Key);
		}
		NKCPacketSender.Send_NKMPacket_FAVORITES_STAGE_UPDATE_REQ(dictionary);
	}

	public static Dictionary<int, NKMStageTempletV2> GetFavoriteStageListForEdit()
	{
		return m_dicFavoriteStagesForEdit;
	}

	public static void CleanUpFavoriteStageEditList()
	{
		if (m_dicFavoriteStagesForEdit != null)
		{
			m_dicFavoriteStagesForEdit.Clear();
			m_dicFavoriteStagesForEdit = null;
		}
	}

	public static void SetFavDicForEdit()
	{
		CleanUpFavoriteStageEditList();
		m_dicFavoriteStagesForEdit = new Dictionary<int, NKMStageTempletV2>(m_dicFavoriteStages);
	}

	public static Dictionary<int, NKMStageTempletV2> GetFavoriteStageList()
	{
		return m_dicFavoriteStages;
	}

	public static void ClearFavoriteStage()
	{
		m_dicFavoriteStages.Clear();
	}

	public static void SetFavoriteStage(Dictionary<int, int> dicFavoritesStage)
	{
		m_dicFavoriteStages.Clear();
		CleanUpFavoriteStageEditList();
		if (dicFavoritesStage == null)
		{
			return;
		}
		foreach (KeyValuePair<int, int> item in dicFavoritesStage)
		{
			NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(item.Value);
			if (nKMStageTempletV == null)
			{
				Log.Error($"StageTemplet is null - Key : {item.Value}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMEpisodeMgrEx.cs", 949);
			}
			else if (!m_dicFavoriteStages.ContainsKey(item.Key))
			{
				m_dicFavoriteStages.Add(item.Key, nKMStageTempletV);
			}
			else
			{
				m_dicFavoriteStages[item.Key] = nKMStageTempletV;
			}
		}
		if (NKCUIOperationNodeViewer.isOpen())
		{
			NKCUIOperationNodeViewer.Instance.RefreshFavoriteInfo();
		}
		if (NKCPopupFavorite.isOpen())
		{
			SetFavDicForEdit();
			NKCPopupFavorite.Instance.RefreshList();
		}
	}

	public static NKCEpisodeSummaryTemplet GetMainSummaryTemplet()
	{
		List<NKCEpisodeSummaryTemplet> list = new List<NKCEpisodeSummaryTemplet>();
		foreach (NKCEpisodeSummaryTemplet value in NKMTempletContainer<NKCEpisodeSummaryTemplet>.Values)
		{
			if (value.CheckEnable(ServiceTime.Recent) && value.EpisodeTemplet != null && value.EpisodeTemplet.IsOpen && value.ShowInPVE_01())
			{
				list.Add(value);
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		list.Sort(SortBySortIndex);
		for (int i = 0; i < list.Count; i++)
		{
			if (!string.IsNullOrEmpty(list[i].m_LobbyResourceID))
			{
				return list[i];
			}
		}
		return null;
	}

	public static void BuildSummaryTemplet(out NKCEpisodeSummaryTemplet mainTemplet, out List<NKCEpisodeSummaryTemplet> lstTempletPVE_01, out List<NKCEpisodeSummaryTemplet> lstTempletPVE_02)
	{
		mainTemplet = null;
		lstTempletPVE_01 = new List<NKCEpisodeSummaryTemplet>();
		lstTempletPVE_02 = new List<NKCEpisodeSummaryTemplet>();
		foreach (NKCEpisodeSummaryTemplet value in NKMTempletContainer<NKCEpisodeSummaryTemplet>.Values)
		{
			if (value.CheckEnable(ServiceTime.Recent) && value.m_EPCategory != EPISODE_CATEGORY.EC_FIERCE && value.EpisodeTemplet != null && value.EpisodeTemplet.IsOpen && value.CheckEpisodeEnable(ServiceTime.Recent))
			{
				if (value.ShowInPVE_01())
				{
					lstTempletPVE_01.Add(value);
				}
				else if (value.ShowInPVE_02())
				{
					lstTempletPVE_02.Add(value);
				}
			}
		}
		lstTempletPVE_01.Sort(SortBySortIndex);
		lstTempletPVE_02.Sort(SortBySortIndex);
		for (int i = 0; i < lstTempletPVE_01.Count; i++)
		{
			if (!string.IsNullOrEmpty(lstTempletPVE_01[i].m_BigResourceID))
			{
				mainTemplet = lstTempletPVE_01[i];
				break;
			}
		}
		if (mainTemplet != null)
		{
			lstTempletPVE_01.Remove(mainTemplet);
		}
	}

	private static int SortBySortIndex(NKCEpisodeSummaryTemplet lItem, NKCEpisodeSummaryTemplet rItem)
	{
		return lItem.m_SortIndex.CompareTo(rItem.m_SortIndex);
	}
}
