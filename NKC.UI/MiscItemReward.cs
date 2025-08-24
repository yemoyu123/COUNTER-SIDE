using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class MiscItemReward : StageRewardData
{
	private int m_miscItemId;

	public MiscItemReward(Transform slotParent, int miscItemId)
		: base(slotParent)
	{
		m_miscItemId = miscItemId;
	}

	public override void CreateSlotData(NKMStageTempletV2 stageTemplet, NKMUserData cNKMUserData, List<int> listNRGI, List<NKCUISlot> listSlotToShow)
	{
		if (stageTemplet == null || cNKMUserData == null)
		{
			return;
		}
		int num = 0;
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
			switch (m_miscItemId)
			{
			case 1:
				num = warfareTemplet.m_RewardCredit_Min;
				break;
			case 2:
				num = warfareTemplet.m_RewardEternium_Min;
				break;
			case 501:
				num = warfareTemplet.m_RewardUserEXP;
				break;
			}
			break;
		}
		case STAGE_TYPE.ST_DUNGEON:
		{
			NKMDungeonTempletBase dungeonTempletBase = stageTemplet.DungeonTempletBase;
			if (dungeonTempletBase == null)
			{
				return;
			}
			switch (m_miscItemId)
			{
			case 501:
				num = dungeonTempletBase.m_RewardUserEXP;
				break;
			case 1:
				num = dungeonTempletBase.m_RewardCredit_Min;
				break;
			case 2:
				num = dungeonTempletBase.m_RewardEternium_Min;
				break;
			case 3:
				num = dungeonTempletBase.m_RewardInformation_Min;
				break;
			}
			break;
		}
		case STAGE_TYPE.ST_PHASE:
			if (stageTemplet.PhaseTemplet == null)
			{
				return;
			}
			switch (m_miscItemId)
			{
			case 501:
				num = stageTemplet.PhaseTemplet.m_RewardUserEXP;
				break;
			case 1:
				num = stageTemplet.PhaseTemplet.m_RewardCredit_Min;
				break;
			}
			break;
		}
		if (num > 0)
		{
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeMiscItemData(new NKMItemMiscData(m_miscItemId, num, 0L));
			m_cSlot.SetData(data, bShowName: false, bShowNumber: false, bEnableLayoutElement: true, null);
			m_cSlot.SetOpenItemBoxOnClick();
			listSlotToShow.Add(m_cSlot);
		}
	}

	public override void CreateSlotData(NKMDefenceTemplet defenceTemplet, NKMUserData cNKMUserData, List<int> listNRGI, List<NKCUISlot> listSlotToShow)
	{
		if (defenceTemplet == null || cNKMUserData == null)
		{
			return;
		}
		int num = 0;
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(defenceTemplet.m_DungeonID);
		if (dungeonTempletBase != null)
		{
			switch (m_miscItemId)
			{
			case 501:
				num = dungeonTempletBase.m_RewardUserEXP;
				break;
			case 1:
				num = dungeonTempletBase.m_RewardCredit_Min;
				break;
			case 2:
				num = dungeonTempletBase.m_RewardEternium_Min;
				break;
			case 3:
				num = dungeonTempletBase.m_RewardInformation_Min;
				break;
			}
			if (num > 0)
			{
				NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeMiscItemData(new NKMItemMiscData(m_miscItemId, num, 0L));
				m_cSlot.SetData(data, bShowName: false, bShowNumber: false, bEnableLayoutElement: true, null);
				m_cSlot.SetOpenItemBoxOnClick();
				listSlotToShow.Add(m_cSlot);
			}
		}
	}
}
