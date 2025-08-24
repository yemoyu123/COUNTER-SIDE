using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Defence;
using Cs.Protocol;
using NKC.UI;
using NKC.UI.Result;
using NKM;
using NKM.Templet;

namespace NKC.Util;

public static class NKCBattleResultUtility
{
	public static void MakeDefenceBattleResult(ref NKCUIResult.BattleResultData retVal, NKMGameEndData gameEndData, NKMDefenceClearData defenceClearData)
	{
		retVal.m_NKM_GAME_TYPE = NKM_GAME_TYPE.NGT_PVE_DEFENCE;
		NKMDefenceTemplet nKMDefenceTemplet = NKMDefenceTemplet.Find(defenceClearData.defenceTempletId);
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(nKMDefenceTemplet.m_DungeonID);
		if (defenceClearData.gameScore >= nKMDefenceTemplet.m_ClearScore)
		{
			retVal.m_BATTLE_RESULT_TYPE = BATTLE_RESULT_TYPE.BRT_WIN;
		}
		else
		{
			retVal.m_BATTLE_RESULT_TYPE = BATTLE_RESULT_TYPE.BRT_LOSE;
		}
		retVal.m_stageID = nKMDefenceTemplet.m_DungeonID;
		if (gameEndData.dungeonClearData.rewardData != null)
		{
			retVal.m_RewardData = gameEndData.dungeonClearData.rewardData.DeepCopy();
		}
		else
		{
			retVal.m_RewardData = null;
		}
		retVal.m_OnetimeRewardData = gameEndData.dungeonClearData.oneTimeRewards;
		retVal.m_firstRewardData = NKCUIResult.GetRewardItemAfterFilter(ref retVal.m_RewardData, dungeonTempletBase.GetFirstRewardData().Type, dungeonTempletBase.GetFirstRewardData().RewardId, dungeonTempletBase.GetFirstRewardData().RewardQuantity);
		NKMRewardData rewardData = null;
		bool num = NKCDefenceDungeonManager.m_DefenceTempletId == defenceClearData.defenceTempletId && NKCDefenceDungeonManager.m_BestClearScore < nKMDefenceTemplet.m_ClearScore;
		if (gameEndData.dungeonClearData.missionReward != null)
		{
			rewardData = gameEndData.dungeonClearData.missionReward.DeepCopy();
		}
		if (num && rewardData != null)
		{
			retVal.m_firstAllClearData = NKCUIResult.GetRewardItemAfterFilter(ref rewardData, nKMDefenceTemplet.m_DungeonMissionReward.rewardType, nKMDefenceTemplet.m_DungeonMissionReward.ID, nKMDefenceTemplet.m_DungeonMissionReward.Count);
		}
		retVal.m_KillCountGain = defenceClearData.gameScore;
		retVal.m_KillCountStageRecord = defenceClearData.bestScore;
		retVal.m_lstMissionData = new List<NKCUIResultSubUIDungeon.MissionData>();
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVE_DEFENCE_MEDAL_REWARD))
		{
			NKCUIResultSubUIDungeon.MissionData missionData = new NKCUIResultSubUIDungeon.MissionData();
			missionData.bSuccess = retVal.IsWin;
			missionData.eMissionType = DUNGEON_GAME_MISSION_TYPE.DGMT_TEAM_A_KILL_COUNT;
			missionData.iMissionValue = nKMDefenceTemplet.m_ClearScore;
			retVal.m_lstMissionData.Add(missionData);
			NKCUIResultSubUIDungeon.MissionData missionData2 = new NKCUIResultSubUIDungeon.MissionData();
			missionData2.bSuccess = defenceClearData != null && defenceClearData.gameScore >= dungeonTempletBase.m_DGMissionValue_1;
			missionData2.eMissionType = dungeonTempletBase.m_DGMissionType_1;
			missionData2.iMissionValue = dungeonTempletBase.m_DGMissionValue_1;
			retVal.m_lstMissionData.Add(missionData2);
			NKCUIResultSubUIDungeon.MissionData missionData3 = new NKCUIResultSubUIDungeon.MissionData();
			missionData3.bSuccess = defenceClearData != null && defenceClearData.gameScore >= dungeonTempletBase.m_DGMissionValue_2;
			missionData3.eMissionType = dungeonTempletBase.m_DGMissionType_2;
			missionData3.iMissionValue = dungeonTempletBase.m_DGMissionValue_2;
			retVal.m_lstMissionData.Add(missionData3);
		}
	}

	public static NKCUIResult.BattleResultData MakeCoreBattleResultData(NKMGameEndData gameEndData, bool bWin, int stageID, int dungeonID, NKCUIBattleStatistics.BattleData battleData)
	{
		NKCUIResult.BattleResultData battleResultData = new NKCUIResult.BattleResultData();
		if (bWin)
		{
			battleResultData.m_BATTLE_RESULT_TYPE = BATTLE_RESULT_TYPE.BRT_WIN;
		}
		else
		{
			battleResultData.m_BATTLE_RESULT_TYPE = BATTLE_RESULT_TYPE.BRT_LOSE;
		}
		if (gameEndData.dungeonClearData != null)
		{
			battleResultData.m_iUnitExp = gameEndData.dungeonClearData.unitExp;
		}
		if (dungeonID > 0 && stageID == 0)
		{
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(dungeonID);
			if (dungeonTempletBase != null && dungeonTempletBase.StageTemplet != null)
			{
				stageID = dungeonTempletBase.StageTemplet.Key;
			}
		}
		if (stageID > 0)
		{
			NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(stageID);
			if (nKMStageTempletV != null)
			{
				if (nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_KILLCOUNT && gameEndData.killCountData != null && gameEndData.win)
				{
					battleResultData.m_KillCountGain = gameEndData.killCountDelta;
					battleResultData.m_KillCountTotal = gameEndData.killCountData.killCount;
					battleResultData.m_KillCountStageRecord = NKCScenManager.CurrentUserData().GetStageKillCountBest(stageID);
				}
				if (nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_TIMEATTACK)
				{
					battleResultData.m_ShadowCurrClearTime = (int)gameEndData.totalPlayTime;
					battleResultData.m_ShadowBestClearTime = NKCScenManager.CurrentUserData().GetStageBestClearSec(stageID);
				}
			}
		}
		battleResultData.m_battleData = battleData;
		return battleResultData;
	}
}
