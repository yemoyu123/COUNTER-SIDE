using System.Collections.Generic;
using ClientPacket.User;
using NKC.UI.Result;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionTeamUp : MonoBehaviour
{
	public enum eTeamUpRewardState
	{
		RS_NONE = -1,
		RS_NOT_READY,
		RS_READY,
		RS_COMPLETE
	}

	public class TeamUpSlotData
	{
		public readonly int m_TeamID;

		public readonly string m_TeamName;

		public readonly int m_RewardCriteria;

		public readonly int m_RewardID;

		public readonly int m_RewardValue;

		public readonly NKM_REWARD_TYPE m_RewardType;

		public eTeamUpRewardState m_RewardState = eTeamUpRewardState.RS_NONE;

		public int m_HasUnitCount;

		public List<int> m_lstUnit;

		public TeamUpSlotData(int TeamID, string Name, int unitCnt, int RewardCriteria, int RewardID, int RewardVal, NKM_REWARD_TYPE RewardType, List<int> lstUnit, eTeamUpRewardState RewardState)
		{
			m_TeamID = TeamID;
			m_TeamName = Name;
			m_RewardCriteria = RewardCriteria;
			m_RewardID = RewardID;
			m_RewardValue = RewardVal;
			m_RewardType = RewardType;
			m_lstUnit = lstUnit;
			m_HasUnitCount = unitCnt;
			m_RewardState = RewardState;
		}
	}

	public NKCUICollectionTeamUpSlot m_pfTeamUpSlot;

	public LoopVerticalScrollFlexibleRect m_LoopVerticalScrollFlexibleRect;

	public RectTransform m_rtTeamUpSlotPool;

	private Stack<RectTransform> m_stkTeamUpPool = new Stack<RectTransform>();

	private NKCUICollection.OnSyncCollectingData dOnSyncCollectingData;

	public NKCUISlot m_pfUISlot;

	public RectTransform m_rtTeamUpSlotIconPool;

	public NKCUICollectionRate m_CollectionRate;

	private Stack<RectTransform> m_stkTeamUpIconPool = new Stack<RectTransform>();

	private NKCUICollection.OnNotify dOnNotify;

	private List<NKCUICollectionTeamUpSlot> m_lstTeamUpSlot = new List<NKCUICollectionTeamUpSlot>();

	private List<TeamUpSlotData> m_list_TeamUp = new List<TeamUpSlotData>();

	private int m_iTotalTeamUpCount;

	private int m_iHasTeamUpCount;

	private bool m_bPrepareTeamUpSlot;

	private List<NKMUnitData> m_lstCurUnitData = new List<NKMUnitData>();

	private List<long> m_listSelectedUnit = new List<long>();

	public void Init(NKCUICollection.OnSyncCollectingData callback, NKCUICollection.OnNotify notify)
	{
		if (null != m_LoopVerticalScrollFlexibleRect)
		{
			m_LoopVerticalScrollFlexibleRect.dOnGetObject += MakeTeamUpSlot;
			m_LoopVerticalScrollFlexibleRect.dOnReturnObject += ReturnTeamUpSlot;
			m_LoopVerticalScrollFlexibleRect.dOnProvideData += ProvideTeamUpSlotData;
			NKCUtil.SetScrollHotKey(m_LoopVerticalScrollFlexibleRect);
		}
		dOnSyncCollectingData = callback;
		dOnNotify = notify;
	}

	public void Open()
	{
		bool bNotify = OpenTeamUpData();
		if (!NKCUnitMissionManager.GetOpenTagCollectionTeamUp())
		{
			bNotify = false;
		}
		dOnNotify?.Invoke(bNotify);
	}

	public void Clear()
	{
		NKCUICollectionUnitInfo.CheckInstanceAndClose();
	}

	private RectTransform MakeTeamUpSlot(int index)
	{
		if (m_stkTeamUpPool.Count > 0)
		{
			RectTransform rectTransform = m_stkTeamUpPool.Pop();
			NKCUtil.SetGameobjectActive(rectTransform, bValue: true);
			return rectTransform;
		}
		NKCUICollectionTeamUpSlot nKCUICollectionTeamUpSlot = Object.Instantiate(m_pfTeamUpSlot);
		nKCUICollectionTeamUpSlot.Init();
		nKCUICollectionTeamUpSlot.transform.localPosition = Vector3.zero;
		nKCUICollectionTeamUpSlot.transform.localScale = Vector3.one;
		return nKCUICollectionTeamUpSlot.GetComponent<RectTransform>();
	}

	private void ReturnTeamUpSlot(Transform go)
	{
		NKCUICollectionTeamUpSlot component = go.GetComponent<NKCUICollectionTeamUpSlot>();
		List<RectTransform> rentalSlot = component.GetRentalSlot();
		for (int i = 0; i < rentalSlot.Count; i++)
		{
			rentalSlot[i].SetParent(m_rtTeamUpSlotIconPool);
			m_stkTeamUpIconPool.Push(rentalSlot[i]);
		}
		component.ClearRentalList();
		NKCUtil.SetGameobjectActive(go, bValue: false);
		go.SetParent(m_rtTeamUpSlotPool);
		m_stkTeamUpPool.Push(go.GetComponent<RectTransform>());
	}

	private List<RectTransform> GetUISlot(int iCnt)
	{
		List<RectTransform> list = new List<RectTransform>();
		for (int i = 0; i < iCnt; i++)
		{
			if (m_stkTeamUpIconPool.Count > 0)
			{
				RectTransform item = m_stkTeamUpIconPool.Pop();
				list.Add(item);
				continue;
			}
			NKCUISlot nKCUISlot = Object.Instantiate(m_pfUISlot);
			nKCUISlot.Init();
			nKCUISlot.transform.localPosition = Vector3.zero;
			nKCUISlot.transform.localScale = Vector3.one;
			RectTransform component = nKCUISlot.GetComponent<RectTransform>();
			list.Add(component);
		}
		return list;
	}

	private void ProvideTeamUpSlotData(Transform tr, int idx)
	{
		NKCUICollectionTeamUpSlot component = tr.GetComponent<NKCUICollectionTeamUpSlot>();
		if (!(component == null))
		{
			List<RectTransform> uISlot = GetUISlot(m_list_TeamUp[idx].m_lstUnit.Count);
			component.SetData(m_list_TeamUp[idx], TryGetReward, bSetUnitList: true, TryUnitInfoOpen, uISlot);
			m_lstTeamUpSlot.Add(component);
		}
	}

	public void TryGetReward(int teamID)
	{
		if (NKCUnitMissionManager.GetOpenTagCollectionTeamUp())
		{
			NKCPacketSender.Send_NKMPacket_TEAM_COLLECTION_REWARD_REQ(teamID);
		}
	}

	public static List<TeamUpSlotData> UpdateTeamUpList(ref int hasTeamUpCount, ref int totalTeamUpCount, NKMArmyData armyData, bool getTeamUpList, out bool bNotify)
	{
		List<TeamUpSlotData> list = null;
		if (getTeamUpList)
		{
			list = new List<TeamUpSlotData>();
		}
		bNotify = false;
		if (armyData == null)
		{
			return null;
		}
		foreach (NKMCollectionTeamUpGroupTemplet value in NKMTempletContainer<NKMCollectionTeamUpGroupTemplet>.Values)
		{
			if (value == null)
			{
				continue;
			}
			List<int> list2 = new List<int>();
			if (value.UnitIDList != null)
			{
				int count = value.UnitIDList.Count;
				for (int i = 0; i < count; i++)
				{
					if (NKMUnitManager.GetUnitTempletBase(value.UnitIDList[i]).PickupEnableByTag)
					{
						list2.Add(value.UnitIDList[i]);
					}
				}
			}
			if (list2.Count <= 0)
			{
				continue;
			}
			int unitCollectCount = armyData.GetUnitCollectCount(list2);
			if (list2.Count <= unitCollectCount)
			{
				hasTeamUpCount++;
			}
			totalTeamUpCount++;
			if (!getTeamUpList)
			{
				continue;
			}
			eTeamUpRewardState eTeamUpRewardState = eTeamUpRewardState.RS_NONE;
			NKMTeamCollectionData teamCollectionData = armyData.GetTeamCollectionData(value.TeamID);
			if (teamCollectionData != null && teamCollectionData.IsRewardComplete())
			{
				eTeamUpRewardState = eTeamUpRewardState.RS_COMPLETE;
			}
			if (eTeamUpRewardState == eTeamUpRewardState.RS_NONE)
			{
				if (value.RewardCriteria <= unitCollectCount)
				{
					bNotify = true;
					eTeamUpRewardState = eTeamUpRewardState.RS_READY;
				}
				else
				{
					eTeamUpRewardState = eTeamUpRewardState.RS_NOT_READY;
				}
			}
			TeamUpSlotData item = new TeamUpSlotData(value.TeamID, NKCStringTable.GetString(value.TeamName), unitCollectCount, list2.Count, value.RewardID, value.RewardValue, value.RewardType, list2, eTeamUpRewardState);
			list.Add(item);
		}
		return list;
	}

	private bool OpenTeamUpData()
	{
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		if (armyData == null)
		{
			return false;
		}
		m_list_TeamUp.Clear();
		m_iHasTeamUpCount = 0;
		m_iTotalTeamUpCount = 0;
		m_list_TeamUp = UpdateTeamUpList(ref m_iHasTeamUpCount, ref m_iTotalTeamUpCount, armyData, getTeamUpList: true, out var bNotify);
		if (!m_bPrepareTeamUpSlot)
		{
			m_lstTeamUpSlot.Clear();
			m_bPrepareTeamUpSlot = true;
			m_LoopVerticalScrollFlexibleRect.TotalCount = m_list_TeamUp.Count;
			m_LoopVerticalScrollFlexibleRect.PrepareCells();
			m_LoopVerticalScrollFlexibleRect.velocity = new Vector2(0f, 0f);
			m_LoopVerticalScrollFlexibleRect.SetIndexPosition(0);
		}
		SyncCollectingUnitData();
		return bNotify;
	}

	private void UpdateTeamUpData()
	{
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		if (armyData == null)
		{
			return;
		}
		bool flag = false;
		foreach (NKMCollectionTeamUpGroupTemplet value in NKMTempletContainer<NKMCollectionTeamUpGroupTemplet>.Values)
		{
			if (value == null)
			{
				continue;
			}
			NKMTeamCollectionData collectionData = armyData.GetTeamCollectionData(value.TeamID);
			if (collectionData != null)
			{
				NKCUICollectionTeamUpSlot nKCUICollectionTeamUpSlot = m_lstTeamUpSlot.Find((NKCUICollectionTeamUpSlot x) => x.GetTeamID() == collectionData.TeamID);
				TeamUpSlotData slotData = m_list_TeamUp.Find((TeamUpSlotData x) => x.m_TeamID == collectionData.TeamID);
				nKCUICollectionTeamUpSlot?.SetData(slotData, null, bSetUnitList: false);
			}
			else if (!flag)
			{
				int unitCollectCount = armyData.GetUnitCollectCount(value.UnitIDList);
				if (value.RewardCriteria <= unitCollectCount)
				{
					flag = true;
				}
			}
		}
		if (!NKCUnitMissionManager.GetOpenTagCollectionTeamUp())
		{
			flag = false;
		}
		dOnNotify?.Invoke(flag);
	}

	private void SyncCollectingUnitData()
	{
		if (dOnSyncCollectingData != null)
		{
			dOnSyncCollectingData(NKCUICollectionGeneral.CollectionType.CT_TEAM_UP, m_iHasTeamUpCount, m_iTotalTeamUpCount);
		}
		m_CollectionRate?.SetData(NKCUICollectionGeneral.CollectionType.CT_TEAM_UP, m_iHasTeamUpCount, m_iTotalTeamUpCount);
	}

	public void OnRecvTeamCollectionRewardAck(NKMPacket_TEAM_COLLECTION_REWARD_ACK sPacket)
	{
		if (sPacket.rewardData.MiscItemDataList.Count > 0)
		{
			NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.AddItemMisc(sPacket.rewardData.MiscItemDataList);
			NKCUIResult.Instance.OpenItemGain(sPacket.rewardData.MiscItemDataList, NKCUtilString.GET_STRING_ITEM_GAIN, NKCUtilString.GET_STRING_CONGRATULATION);
		}
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		if (armyData != null)
		{
			eTeamUpRewardState state = eTeamUpRewardState.RS_READY;
			if (sPacket.teamCollectionData.IsRewardComplete())
			{
				state = eTeamUpRewardState.RS_COMPLETE;
			}
			SetSlotDataState(sPacket.teamCollectionData.TeamID, state);
			armyData.AddTeamCollectionData(sPacket.teamCollectionData);
			UpdateTeamUpData();
		}
	}

	public void TryUnitInfoOpen(NKCUISlot.SlotData slotData, bool bLocked)
	{
		m_lstCurUnitData.Clear();
		NKMCollectionTeamUpGroupTemplet nKMCollectionTeamUpGroupTemplet = NKMCollectionTeamUpGroupTemplet.Find(slotData.GroupID);
		if (nKMCollectionTeamUpGroupTemplet == null)
		{
			return;
		}
		int num = -1;
		for (int i = 0; i < nKMCollectionTeamUpGroupTemplet.UnitIDList.Count; i++)
		{
			if (NKMUnitTempletBase.Find(nKMCollectionTeamUpGroupTemplet.UnitIDList[i]).PickupEnableByTag)
			{
				if (nKMCollectionTeamUpGroupTemplet.UnitIDList[i] == slotData.ID)
				{
					num = m_lstCurUnitData.Count;
				}
				m_lstCurUnitData.Add(NKCUtil.MakeDummyUnit(nKMCollectionTeamUpGroupTemplet.UnitIDList[i], 100, 3));
			}
		}
		if (num >= 0 && m_lstCurUnitData.Count > 0)
		{
			NKCUIUnitInfo.OpenOption openOption = new NKCUIUnitInfo.OpenOption(m_lstCurUnitData, num);
			NKCUICollectionUnitInfo.CheckInstanceAndOpen(m_lstCurUnitData[num], openOption);
		}
	}

	private void SetSlotDataState(int teamID, eTeamUpRewardState state)
	{
		TeamUpSlotData teamUpSlotData = m_list_TeamUp.Find((TeamUpSlotData x) => x.m_TeamID == teamID);
		if (teamUpSlotData != null)
		{
			teamUpSlotData.m_RewardState = state;
		}
	}
}
