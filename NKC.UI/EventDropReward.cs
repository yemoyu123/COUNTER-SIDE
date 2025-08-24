using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class EventDropReward : StageRewardData
{
	private int m_iEventDropRewardIndex;

	private int m_iRewardGroupId;

	public EventDropReward(Transform slotParent, int eventDroprewardIndex)
		: base(slotParent)
	{
		m_iEventDropRewardIndex = eventDroprewardIndex;
	}

	public override void CreateSlotData(NKMStageTempletV2 stageTemplet, NKMUserData cNKMUserData, List<int> listNRGI, List<NKCUISlot> listSlotToShow)
	{
		if (stageTemplet == null || !NKMEpisodeMgr.CheckStageHasEventDrop(stageTemplet))
		{
			return;
		}
		m_iRewardGroupId = stageTemplet.GetEventDropRewardGroupID(m_iEventDropRewardIndex);
		NKMRewardGroupTemplet rewardGroup = NKMRewardManager.GetRewardGroup(m_iRewardGroupId);
		List<NKMRewardTemplet> list = new List<NKMRewardTemplet>();
		if (rewardGroup != null)
		{
			int count = rewardGroup.List.Count;
			for (int i = 0; i < count; i++)
			{
				if (rewardGroup.List[i].intervalTemplet.IsValidTime(NKCSynchronizedTime.ServiceTime))
				{
					list.Add(rewardGroup.List[i]);
				}
			}
		}
		int count2 = list.Count;
		if (count2 <= 0)
		{
			return;
		}
		if (count2 > 1)
		{
			int num = -1;
			for (int j = 0; j < list.Count; j++)
			{
				int rewardGrade = NKCUtil.GetRewardGrade(list[j].m_RewardID, list[j].m_eRewardType);
				if (num < rewardGrade)
				{
					num = rewardGrade;
				}
			}
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeMiscItemData(new NKMItemMiscData(903, 1L, 0L));
			m_cSlot.SetData(data, bShowName: false, bShowNumber: false, bEnableLayoutElement: true, OnClickEventDropSlot);
			m_cSlot.SetBackGround(num);
			m_cSlot.SetEventDropMark(bValue: true);
			listSlotToShow.Add(m_cSlot);
		}
		else if (count2 == 1)
		{
			NKMRewardTemplet nKMRewardTemplet = list[0];
			NKCUISlot.SlotData data2 = NKCUISlot.SlotData.MakeRewardTypeData(nKMRewardTemplet.m_eRewardType, nKMRewardTemplet.m_RewardID, 1);
			m_cSlot.SetData(data2, bShowName: false, bShowNumber: false, bEnableLayoutElement: true, null);
			m_cSlot.SetOpenItemBoxOnClick();
			m_cSlot.SetEventDropMark(bValue: true);
			listSlotToShow.Add(m_cSlot);
		}
	}

	public override void CreateSlotData(NKMDefenceTemplet defenceTemplet, NKMUserData cNKMUserData, List<int> listNRGI, List<NKCUISlot> listSlotToShow)
	{
	}

	private void OnClickEventDropSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		NKMRewardGroupTemplet rewardGroup = NKMRewardManager.GetRewardGroup(m_iRewardGroupId);
		if (rewardGroup == null)
		{
			return;
		}
		HashSet<int> hashSet = new HashSet<int>();
		HashSet<int> hashSet2 = new HashSet<int>();
		HashSet<int> hashSet3 = new HashSet<int>();
		HashSet<int> hashSet4 = new HashSet<int>();
		HashSet<int> hashSet5 = new HashSet<int>();
		int count = rewardGroup.List.Count;
		List<NKMRewardTemplet> list = new List<NKMRewardTemplet>();
		for (int i = 0; i < count; i++)
		{
			if (rewardGroup.List[i].intervalTemplet.IsValidTime(NKCSynchronizedTime.ServiceTime))
			{
				list.Add(rewardGroup.List[i]);
			}
		}
		int count2 = list.Count;
		if (count2 <= 0)
		{
			return;
		}
		for (int j = 0; j < count2; j++)
		{
			switch (list[j].m_eRewardType)
			{
			case NKM_REWARD_TYPE.RT_UNIT:
				hashSet.Add(list[j].m_RewardID);
				break;
			case NKM_REWARD_TYPE.RT_OPERATOR:
				hashSet2.Add(list[j].m_RewardID);
				break;
			case NKM_REWARD_TYPE.RT_EQUIP:
				hashSet3.Add(list[j].m_RewardID);
				break;
			case NKM_REWARD_TYPE.RT_MOLD:
				hashSet4.Add(list[j].m_RewardID);
				break;
			case NKM_REWARD_TYPE.RT_MISC:
				hashSet5.Add(list[j].m_RewardID);
				break;
			}
		}
		Dictionary<NKM_REWARD_TYPE, List<int>> dictionary = new Dictionary<NKM_REWARD_TYPE, List<int>>();
		dictionary.Add(NKM_REWARD_TYPE.RT_UNIT, NKCUIComDungeonRewardList.GetUnitRewardIdList(hashSet, hashSet2));
		dictionary.Add(NKM_REWARD_TYPE.RT_EQUIP, NKCUIComDungeonRewardList.GetEquipRewardIdList(hashSet3));
		dictionary.Add(NKM_REWARD_TYPE.RT_MOLD, NKCUIComDungeonRewardList.GetMoldRewardIdList(hashSet4));
		dictionary.Add(NKM_REWARD_TYPE.RT_MISC, NKCUIComDungeonRewardList.GetMiscRewardIdList(hashSet5));
		NKCUISlotListViewer.Instance.OpenRewardList(dictionary, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_TITLE, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_DESC);
	}
}
