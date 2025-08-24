using System.Collections.Generic;
using ClientPacket.Common;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class FirstAllClearData : StageRewardData
{
	public FirstAllClearData(Transform slotParent)
		: base(slotParent)
	{
	}

	public override void CreateSlotData(NKMStageTempletV2 stageTemplet, NKMUserData cNKMUserData, List<int> listNRGI, List<NKCUISlot> listSlotToShow)
	{
		if (stageTemplet == null || cNKMUserData == null)
		{
			return;
		}
		bool completeMark = false;
		switch (stageTemplet.m_STAGE_TYPE)
		{
		default:
			return;
		case STAGE_TYPE.ST_WARFARE:
		{
			NKMWarfareTemplet warfareTemplet = stageTemplet.WarfareTemplet;
			if (warfareTemplet == null)
			{
				return;
			}
			NKMWarfareClearData warfareClearData = cNKMUserData.GetWarfareClearData(warfareTemplet.m_WarfareID);
			if (warfareClearData != null)
			{
				completeMark = warfareClearData.m_mission_result_1 && warfareClearData.m_mission_result_2 && warfareClearData.m_MissionRewardResult;
			}
			break;
		}
		case STAGE_TYPE.ST_DUNGEON:
		{
			if (stageTemplet.DungeonTempletBase == null)
			{
				return;
			}
			NKMDungeonClearData dungeonClearData = cNKMUserData.GetDungeonClearData(stageTemplet.DungeonTempletBase.m_DungeonID);
			if (dungeonClearData != null)
			{
				completeMark = dungeonClearData.missionResult1 && dungeonClearData.missionResult2;
			}
			break;
		}
		case STAGE_TYPE.ST_PHASE:
		{
			if (stageTemplet.PhaseTemplet == null)
			{
				return;
			}
			NKMPhaseClearData phaseClearData = NKCPhaseManager.GetPhaseClearData(stageTemplet.PhaseTemplet);
			if (phaseClearData != null)
			{
				completeMark = phaseClearData.missionResult1 && phaseClearData.missionResult2;
			}
			break;
		}
		}
		bool flag = true;
		if (stageTemplet.MissionReward == null)
		{
			flag = false;
		}
		else
		{
			if (stageTemplet.MissionReward.rewardType == NKM_REWARD_TYPE.RT_NONE || stageTemplet.MissionReward.ID == 0)
			{
				flag = false;
			}
			if (!NKMRewardTemplet.IsOpenedReward(stageTemplet.MissionReward.rewardType, stageTemplet.MissionReward.ID, useRandomContract: false))
			{
				flag = false;
			}
		}
		if (flag)
		{
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(stageTemplet.MissionReward.rewardType, stageTemplet.MissionReward.ID, stageTemplet.MissionReward.Count);
			m_cSlot.SetData(data);
			m_cSlot.SetFirstAllClearMark(bValue: true);
			m_cSlot.SetCompleteMark(completeMark);
			listSlotToShow.Add(m_cSlot);
		}
	}

	public override void CreateSlotData(NKMDefenceTemplet defenceTemplet, NKMUserData cNKMUserData, List<int> listNRGI, List<NKCUISlot> listSlotToShow)
	{
		if (defenceTemplet == null || cNKMUserData == null)
		{
			return;
		}
		bool completeMark = false;
		if (NKCDefenceDungeonManager.m_DefenceTempletId == defenceTemplet.Key)
		{
			completeMark = NKCDefenceDungeonManager.m_bMissionResult1 && NKCDefenceDungeonManager.m_bMissionResult2;
		}
		bool flag = true;
		if (defenceTemplet.m_DungeonMissionReward == null)
		{
			flag = false;
		}
		else
		{
			if (defenceTemplet.m_DungeonMissionReward.rewardType == NKM_REWARD_TYPE.RT_NONE || defenceTemplet.m_DungeonMissionReward.ID == 0)
			{
				flag = false;
			}
			if (!NKMRewardTemplet.IsOpenedReward(defenceTemplet.m_DungeonMissionReward.rewardType, defenceTemplet.m_DungeonMissionReward.ID, useRandomContract: false))
			{
				flag = false;
			}
		}
		if (flag)
		{
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(defenceTemplet.m_DungeonMissionReward.rewardType, defenceTemplet.m_DungeonMissionReward.ID, defenceTemplet.m_DungeonMissionReward.Count);
			m_cSlot.SetData(data);
			m_cSlot.SetFirstAllClearMark(bValue: true);
			m_cSlot.SetCompleteMark(completeMark);
			listSlotToShow.Add(m_cSlot);
		}
	}
}
