using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class MainReward : StageRewardData
{
	public MainReward(Transform slotParent)
		: base(slotParent)
	{
	}

	public override void CreateSlotData(NKMStageTempletV2 stageTemplet, NKMUserData cNKMUserData, List<int> listNRGI, List<NKCUISlot> listSlotToShow)
	{
		if (stageTemplet != null && cNKMUserData != null && stageTemplet.MainRewardData != null && NKCUtil.IsValidReward(stageTemplet.MainRewardData.rewardType, stageTemplet.MainRewardData.ID) && NKMRewardTemplet.IsOpenedReward(stageTemplet.MainRewardData.rewardType, stageTemplet.MainRewardData.ID, useRandomContract: false) && stageTemplet.MainRewardData.ID != 0 && stageTemplet.MainRewardData.MaxValue != 0)
		{
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(stageTemplet.MainRewardData.rewardType, stageTemplet.MainRewardData.ID, stageTemplet.MainRewardData.MinValue);
			m_cSlot.SetData(data);
			m_cSlot.SetMainRewardMark(bValue: true);
			listSlotToShow.Add(m_cSlot);
		}
	}

	public override void CreateSlotData(NKMDefenceTemplet defenceTemplet, NKMUserData cNKMUserData, List<int> listNRGI, List<NKCUISlot> listSlotToShow)
	{
		if (defenceTemplet != null && cNKMUserData != null && defenceTemplet.m_MainRewardData != null && NKCUtil.IsValidReward(defenceTemplet.m_MainRewardData.rewardType, defenceTemplet.m_MainRewardData.ID) && NKMRewardTemplet.IsOpenedReward(defenceTemplet.m_MainRewardData.rewardType, defenceTemplet.m_MainRewardData.ID, useRandomContract: false) && defenceTemplet.m_MainRewardData.ID != 0 && defenceTemplet.m_MainRewardData.MaxValue != 0)
		{
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(defenceTemplet.m_MainRewardData.rewardType, defenceTemplet.m_MainRewardData.ID, defenceTemplet.m_MainRewardData.MinValue);
			m_cSlot.SetData(data);
			m_cSlot.SetMainRewardMark(bValue: true);
			listSlotToShow.Add(m_cSlot);
		}
	}
}
