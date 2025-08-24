using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class RandomItemReward : StageRewardData
{
	private NKM_REWARD_TYPE m_rewardType;

	private int m_itemId;

	private NKCUISlot.OnClick m_onSlotClick;

	public RandomItemReward(Transform slotParent, NKM_REWARD_TYPE rewardType, int itemId, NKCUISlot.OnClick onSlotClick)
		: base(slotParent)
	{
		m_rewardType = rewardType;
		m_itemId = itemId;
		m_onSlotClick = onSlotClick;
	}

	public override void CreateSlotData(NKMStageTempletV2 stageTemplet, NKMUserData cNKMUserData, List<int> listNRGI, List<NKCUISlot> listSlotToShow)
	{
		if (stageTemplet == null || cNKMUserData == null)
		{
			return;
		}
		bool num = NKCUtil.CheckExistRewardType(listNRGI, m_rewardType);
		int maxGradeInRewardGroups = NKCUtil.GetMaxGradeInRewardGroups(listNRGI, m_rewardType);
		bool eventDropMark = NKCUtil.CheckEventDropReward(stageTemplet, listNRGI, m_rewardType);
		if (num)
		{
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeMiscItemData(new NKMItemMiscData(m_itemId, 1L, 0L));
			m_cSlot.SetData(data, bShowName: false, bShowNumber: false, bEnableLayoutElement: true, m_onSlotClick);
			m_cSlot.SetBackGround(maxGradeInRewardGroups);
			m_cSlot.SetEventDropMark(eventDropMark);
			if (m_onSlotClick == null)
			{
				m_cSlot.SetOpenItemBoxOnClick();
			}
			listSlotToShow.Add(m_cSlot);
		}
	}

	public override void CreateSlotData(NKMDefenceTemplet defenceTemplet, NKMUserData cNKMUserData, List<int> listNRGI, List<NKCUISlot> listSlotToShow)
	{
		if (defenceTemplet == null || cNKMUserData == null)
		{
			return;
		}
		bool num = NKCUtil.CheckExistRewardType(listNRGI, m_rewardType);
		int maxGradeInRewardGroups = NKCUtil.GetMaxGradeInRewardGroups(listNRGI, m_rewardType);
		if (num)
		{
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeMiscItemData(new NKMItemMiscData(m_itemId, 1L, 0L));
			m_cSlot.SetData(data, bShowName: false, bShowNumber: false, bEnableLayoutElement: true, m_onSlotClick);
			m_cSlot.SetBackGround(maxGradeInRewardGroups);
			if (m_onSlotClick == null)
			{
				m_cSlot.SetOpenItemBoxOnClick();
			}
			listSlotToShow.Add(m_cSlot);
		}
	}

	public override void Release()
	{
		base.Release();
		m_onSlotClick = null;
	}
}
