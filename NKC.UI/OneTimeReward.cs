using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class OneTimeReward : StageRewardData
{
	private int m_index;

	public OneTimeReward(Transform slotParent, int index)
		: base(slotParent)
	{
		m_index = index;
	}

	public override void CreateSlotData(NKMStageTempletV2 stageTemplet, NKMUserData cNKMUserData, List<int> listNRGI, List<NKCUISlot> listSlotToShow)
	{
		if (stageTemplet == null || cNKMUserData == null)
		{
			return;
		}
		bool flag = false;
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
			flag = cNKMUserData.CheckWarfareOneTimeReward(warfareTemplet.m_WarfareID, m_index);
			break;
		}
		case STAGE_TYPE.ST_DUNGEON:
		{
			NKMDungeonTempletBase dungeonTempletBase = stageTemplet.DungeonTempletBase;
			if (dungeonTempletBase == null)
			{
				return;
			}
			flag = cNKMUserData.CheckDungeonOneTimeReward(dungeonTempletBase.m_DungeonID, m_index);
			break;
		}
		case STAGE_TYPE.ST_PHASE:
			flag = NKCPhaseManager.CheckOneTimeReward(stageTemplet, m_index);
			break;
		}
		NKMRewardInfo nKMRewardInfo = stageTemplet.GetOneTimeReward(m_index)?.GetRewardInfo();
		if (nKMRewardInfo != null)
		{
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(nKMRewardInfo.rewardType, nKMRewardInfo.ID, nKMRewardInfo.Count);
			m_cSlot.SetData(data);
			m_cSlot.SetCompleteMark(flag);
			m_cSlot.SetOnetimeMark(bValue: true);
			listSlotToShow.Add(m_cSlot);
		}
	}

	public override void CreateSlotData(NKMDefenceTemplet defenceTemplet, NKMUserData cNKMUserData, List<int> listNRGI, List<NKCUISlot> listSlotToShow)
	{
	}
}
