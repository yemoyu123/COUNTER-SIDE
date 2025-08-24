using System.Collections.Generic;
using NKC.Templet;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI.Trim;

public class NKCUITrimReward : MonoBehaviour
{
	public struct TrimRewardInfo
	{
		public NKCUISlot.SlotData slotData;

		public bool isFirstClearReward;

		public bool isEventDropReward;

		public TrimRewardInfo(NKCUISlot.SlotData _slotData, bool _isFirstClearReward, bool _isEventDropReward)
		{
			slotData = _slotData;
			isFirstClearReward = _isFirstClearReward;
			isEventDropReward = _isEventDropReward;
		}
	}

	public Transform m_rewardParent;

	private List<NKCUISlot> m_rewardSlotList = new List<NKCUISlot>();

	public void Init()
	{
		if (m_rewardParent != null)
		{
			NKCUISlot[] componentsInChildren = m_rewardParent.GetComponentsInChildren<NKCUISlot>(includeInactive: true);
			int num = ((componentsInChildren != null) ? componentsInChildren.Length : 0);
			for (int i = 0; i < num; i++)
			{
				m_rewardSlotList.Add(componentsInChildren[i]);
			}
		}
	}

	public void SetData(int trimId, int trimLevel)
	{
		List<TrimRewardInfo> list = new List<TrimRewardInfo>();
		List<NKCUISlot.SlotData> equipRewardList = new List<NKCUISlot.SlotData>();
		NKCUISlot.SlotData slotData = null;
		NKCTrimRewardTemplet nKCTrimRewardTemplet = NKCTrimRewardTemplet.Find(trimId, trimLevel);
		int num = 0;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			num = nKMUserData.TrimData.GetClearedTrimLevel(trimId);
		}
		bool flag = trimLevel <= num;
		if (nKCTrimRewardTemplet != null)
		{
			if (nKCTrimRewardTemplet.RewardUserExp > 0)
			{
				NKCUISlot.SlotData slotData2 = NKCUISlot.SlotData.MakeRewardTypeData(NKM_REWARD_TYPE.RT_MISC, 501, 1);
				list.Add(new TrimRewardInfo(slotData2, _isFirstClearReward: false, _isEventDropReward: false));
			}
			if (nKCTrimRewardTemplet.FirstClearRewardType != NKM_REWARD_TYPE.RT_NONE)
			{
				NKCUISlot.SlotData slotData3 = NKCUISlot.SlotData.MakeRewardTypeData(nKCTrimRewardTemplet.FirstClearRewardType, nKCTrimRewardTemplet.FirstClearRewardID, 1);
				list.Add(new TrimRewardInfo(slotData3, _isFirstClearReward: true, _isEventDropReward: false));
			}
			int count = nKCTrimRewardTemplet.EventDropIndex.Count;
			for (int i = 0; i < count; i++)
			{
				NKMRewardGroupTemplet groupTemplet = NKMRewardManager.GetRewardGroup(nKCTrimRewardTemplet.EventDropIndex[i]);
				if (groupTemplet == null)
				{
					continue;
				}
				int count2 = groupTemplet.List.Count;
				int j = 0;
				while (j < count2)
				{
					if (groupTemplet.List[j].intervalTemplet.IsValidTime(NKCSynchronizedTime.ServiceTime) && list.FindIndex((TrimRewardInfo e) => e.slotData.ID == groupTemplet.List[j].m_RewardID) < 0)
					{
						NKCUISlot.SlotData slotData4 = NKCUISlot.SlotData.MakeRewardTypeData(groupTemplet.List[j].m_eRewardType, groupTemplet.List[j].m_RewardID, 1);
						list.Add(new TrimRewardInfo(slotData4, _isFirstClearReward: false, _isEventDropReward: true));
					}
					int num2 = j + 1;
					j = num2;
				}
			}
			if (nKCTrimRewardTemplet.FixRewardType != NKM_REWARD_TYPE.RT_NONE)
			{
				NKCUISlot.SlotData slotData5 = NKCUISlot.SlotData.MakeRewardTypeData(nKCTrimRewardTemplet.FixRewardType, nKCTrimRewardTemplet.FixRewardID, 1);
				list.Add(new TrimRewardInfo(slotData5, _isFirstClearReward: false, _isEventDropReward: false));
			}
			int count3 = nKCTrimRewardTemplet.RewardGroupID.Count;
			for (int num3 = 0; num3 < count3; num3++)
			{
				NKMRewardGroupTemplet groupTemplet2 = NKMRewardManager.GetRewardGroup(nKCTrimRewardTemplet.RewardGroupID[num3]);
				if (groupTemplet2 == null)
				{
					continue;
				}
				int count4 = groupTemplet2.List.Count;
				int j2 = 0;
				while (j2 < count4)
				{
					if (groupTemplet2.List[j2].intervalTemplet.IsValidTime(NKCSynchronizedTime.ServiceTime))
					{
						if (groupTemplet2.List[j2].m_eRewardType == NKM_REWARD_TYPE.RT_EQUIP)
						{
							NKCUISlot.SlotData item = NKCUISlot.SlotData.MakeRewardTypeData(groupTemplet2.List[j2].m_eRewardType, groupTemplet2.List[j2].m_RewardID, 1);
							equipRewardList.Add(item);
						}
						else if (list.FindIndex((TrimRewardInfo e) => e.slotData.ID == groupTemplet2.List[j2].m_RewardID) < 0)
						{
							NKCUISlot.SlotData slotData6 = NKCUISlot.SlotData.MakeRewardTypeData(groupTemplet2.List[j2].m_eRewardType, groupTemplet2.List[j2].m_RewardID, 1);
							list.Add(new TrimRewardInfo(slotData6, _isFirstClearReward: false, _isEventDropReward: false));
						}
					}
					int num2 = j2 + 1;
					j2 = num2;
				}
			}
			int count5 = nKCTrimRewardTemplet.RewardUnitExp.Count;
			for (int num4 = 0; num4 < count5; num4++)
			{
				if (nKCTrimRewardTemplet.RewardUnitExp[num4] > 0)
				{
					int expItemId = 0;
					switch (num4)
					{
					case 0:
						expItemId = 1031;
						break;
					case 1:
						expItemId = 1032;
						break;
					case 2:
						expItemId = 1033;
						break;
					}
					if (expItemId != 0 && list.FindIndex((TrimRewardInfo e) => e.slotData.ID == expItemId) < 0)
					{
						NKCUISlot.SlotData slotData7 = NKCUISlot.SlotData.MakeRewardTypeData(NKM_REWARD_TYPE.RT_MISC, expItemId, 1);
						list.Add(new TrimRewardInfo(slotData7, _isFirstClearReward: false, _isEventDropReward: false));
					}
				}
			}
			if (nKCTrimRewardTemplet.RewardCreditMin > 0)
			{
				slotData = NKCUISlot.SlotData.MakeRewardTypeData(NKM_REWARD_TYPE.RT_MISC, 1, 1);
			}
		}
		int num5 = 0;
		int count6 = list.Count;
		int count7 = m_rewardSlotList.Count;
		for (int num6 = 0; num6 < count7; num6++)
		{
			if (count6 <= num6)
			{
				m_rewardSlotList[num6].SetActive(bSet: false);
				continue;
			}
			m_rewardSlotList[num6].SetActive(bSet: true);
			m_rewardSlotList[num6].Init();
			m_rewardSlotList[num6].SetData(list[num6].slotData);
			if (list[num6].isFirstClearReward)
			{
				m_rewardSlotList[num6].SetFirstGetMark(!flag);
				m_rewardSlotList[num6].SetCompleteMark(flag);
			}
			else if (list[num6].isEventDropReward)
			{
				m_rewardSlotList[num6].SetEventDropMark(bValue: true);
			}
			num5++;
		}
		for (int num7 = count7; num7 < count6; num7++)
		{
			NKCUISlot newInstance = NKCUISlot.GetNewInstance(m_rewardParent);
			if (!(newInstance == null))
			{
				newInstance.transform.localScale = Vector3.one;
				newInstance.SetActive(bSet: true);
				newInstance.SetData(list[num7].slotData);
				m_rewardSlotList.Add(newInstance);
				if (list[num7].isFirstClearReward)
				{
					newInstance.SetFirstGetMark(!flag);
					newInstance.SetCompleteMark(flag);
				}
				else if (list[num7].isEventDropReward)
				{
					newInstance.SetEventDropMark(bValue: true);
				}
			}
		}
		NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeMiscItemData(new NKMItemMiscData(902, 1L, 0L));
		if (equipRewardList.Count > 0)
		{
			NKCUISlot nKCUISlot = null;
			if (num5 >= count7)
			{
				nKCUISlot = NKCUISlot.GetNewInstance(m_rewardParent);
				if (nKCUISlot != null)
				{
					nKCUISlot.transform.localScale = Vector3.one;
					m_rewardSlotList.Add(nKCUISlot);
				}
			}
			else
			{
				nKCUISlot = m_rewardSlotList[num5];
				nKCUISlot?.Init();
				num5++;
			}
			if (nKCUISlot != null)
			{
				nKCUISlot.SetActive(bSet: true);
				nKCUISlot.SetData(data, bShowName: false, bShowNumber: false, bEnableLayoutElement: true, delegate
				{
					List<int> lstID = new List<int>();
					equipRewardList.ForEach(delegate(NKCUISlot.SlotData e)
					{
						lstID.Add(e.ID);
					});
					NKCUISlotListViewer.Instance.OpenRewardList(lstID, NKM_REWARD_TYPE.RT_EQUIP, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_TITLE, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_DESC);
				});
				int maxGrade = 0;
				equipRewardList.ForEach(delegate(NKCUISlot.SlotData e)
				{
					int rewardGrade = NKCUtil.GetRewardGrade(e.ID, NKM_REWARD_TYPE.RT_EQUIP);
					if (maxGrade < rewardGrade)
					{
						maxGrade = rewardGrade;
					}
				});
				nKCUISlot.SetBackGround(maxGrade);
			}
		}
		if (slotData == null)
		{
			return;
		}
		NKCUISlot nKCUISlot2 = null;
		if (num5 >= count7)
		{
			nKCUISlot2 = NKCUISlot.GetNewInstance(m_rewardParent);
			if (nKCUISlot2 != null)
			{
				nKCUISlot2.transform.localScale = Vector3.one;
				m_rewardSlotList.Add(nKCUISlot2);
			}
		}
		else
		{
			nKCUISlot2 = m_rewardSlotList[num5];
			nKCUISlot2?.Init();
			num5++;
		}
		if (nKCUISlot2 != null)
		{
			nKCUISlot2.SetActive(bSet: true);
			nKCUISlot2.SetData(slotData);
		}
	}
}
