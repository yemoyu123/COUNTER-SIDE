using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class FirstClearData : StageRewardData
{
	public FirstClearData(Transform slotParent)
		: base(slotParent)
	{
	}

	public override void CreateSlotData(NKMStageTempletV2 stageTemplet, NKMUserData cNKMUserData, List<int> listNRGI, List<NKCUISlot> listSlotToShow)
	{
		if (stageTemplet != null && cNKMUserData != null)
		{
			FirstRewardData firstRewardData = stageTemplet.GetFirstRewardData();
			bool completeMark = NKMEpisodeMgr.CheckClear(cNKMUserData, stageTemplet);
			if (firstRewardData != null && firstRewardData.Type != NKM_REWARD_TYPE.RT_NONE && firstRewardData.RewardId != 0)
			{
				NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(firstRewardData.Type, firstRewardData.RewardId, firstRewardData.RewardQuantity);
				m_cSlot.SetData(data);
				m_cSlot.SetCompleteMark(completeMark);
				m_cSlot.SetFirstGetMark(bValue: true);
				listSlotToShow.Add(m_cSlot);
			}
		}
	}

	public override void CreateSlotData(NKMDefenceTemplet defenceTemplet, NKMUserData cNKMUserData, List<int> listNRGI, List<NKCUISlot> listSlotToShow)
	{
		if (defenceTemplet != null && cNKMUserData != null)
		{
			FirstRewardData firstRewardData = defenceTemplet.GetFirstRewardData();
			bool completeMark = NKCDefenceDungeonManager.m_DefenceTempletId == defenceTemplet.Key && NKCDefenceDungeonManager.m_BestClearScore >= defenceTemplet.m_ClearScore;
			if (firstRewardData != null && firstRewardData.Type != NKM_REWARD_TYPE.RT_NONE && firstRewardData.RewardId != 0)
			{
				NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(firstRewardData.Type, firstRewardData.RewardId, firstRewardData.RewardQuantity);
				m_cSlot.SetData(data);
				m_cSlot.SetCompleteMark(completeMark);
				m_cSlot.SetFirstGetMark(bValue: true);
				listSlotToShow.Add(m_cSlot);
			}
		}
	}
}
