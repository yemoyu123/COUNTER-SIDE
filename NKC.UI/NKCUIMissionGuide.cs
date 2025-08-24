using System;
using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.User;
using NKC.UI.Result;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIMissionGuide : NKCUIBase
{
	public const string UI_ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_mission_guide";

	public const string UI_ASSET_NAME = "NKM_UI_MISSION_GUIDE";

	private static NKCUIMissionGuide m_Instance;

	public Image m_BannerImg;

	public NKCUIComStateButton m_csbtnClose;

	public NKCUIComToggleGroup m_ctgTab;

	public LoopVerticalScrollRect m_lvsrTab;

	public LoopVerticalScrollRect m_lvsrMission;

	public GameObject m_objReceive;

	public NKCUISlot[] m_lstReceiveSlots;

	public NKCUIComStateButton m_csbtnQuick;

	public NKCUIComStateButton m_csbtnReceive;

	[Header("All Clear")]
	public GameObject m_objBottom;

	public Slider m_imgReceiveGauge;

	public Text m_lbReceiveGauge;

	private bool m_bRefreshReserved;

	private bool m_bFirstOpen = true;

	private int m_NKM_MISSION_TAB_ID;

	private List<NKMMissionTabTemplet> m_lstMissionTabTemplet = new List<NKMMissionTabTemplet>();

	private Dictionary<int, List<NKMMissionTemplet>> m_dicNKMMissionTemplet = new Dictionary<int, List<NKMMissionTemplet>>();

	private List<NKMMissionTemplet> m_lstGrowthMissionList = new List<NKMMissionTemplet>();

	private Dictionary<int, NKCUIMissionAchieveTab> m_dicMissionTab = new Dictionary<int, NKCUIMissionAchieveTab>();

	private List<NKMMissionTemplet> m_lstCurrentList = new List<NKMMissionTemplet>();

	public static NKCUIMissionGuide Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIMissionGuide>("ab_ui_nkm_ui_mission_guide", "NKM_UI_MISSION_GUIDE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIMissionGuide>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public static bool HasInstance => m_Instance != null;

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override string MenuName => NKCUtilString.GET_STRING_MISSION;

	public override eMenutype eUIType => eMenutype.FullScreen;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	public override void UnHide()
	{
		base.UnHide();
		if (m_bRefreshReserved)
		{
			SetUIByCurrTab();
			m_bRefreshReserved = false;
		}
	}

	public override void CloseInternal()
	{
		foreach (KeyValuePair<int, NKCUIMissionAchieveTab> item in m_dicMissionTab)
		{
			item.Value.GetToggle().Select(bSelect: false);
		}
		m_bRefreshReserved = false;
		m_bFirstOpen = true;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void InitUI()
	{
		NKCUtil.SetBindFunction(m_csbtnClose, base.Close);
		NKCUtil.SetBindFunction(m_csbtnQuick, OnClickGrowthMoveToCenter);
		NKCUtil.SetBindFunction(m_csbtnReceive, OnClickReceive);
		if (null != m_lvsrTab)
		{
			m_lvsrTab.dOnGetObject += GetMissionTab;
			m_lvsrTab.dOnReturnObject += ReturnMissionTab;
			m_lvsrTab.dOnProvideData += ProvideTabData;
			m_lvsrTab.ContentConstraintCount = 1;
			NKCUtil.SetScrollHotKey(m_lvsrTab);
		}
		if (null != m_lvsrMission)
		{
			m_lvsrMission.dOnGetObject += GetGrowthMissionSlot;
			m_lvsrMission.dOnReturnObject += ReturnGrowthMissionSlot;
			m_lvsrMission.dOnProvideData += ProvideGrowthData;
			m_lvsrMission.ContentConstraintCount = 1;
			NKCUtil.SetScrollHotKey(m_lvsrMission);
		}
		NKCUISlot[] lstReceiveSlots = m_lstReceiveSlots;
		for (int i = 0; i < lstReceiveSlots.Length; i++)
		{
			lstReceiveSlots[i]?.Init();
		}
	}

	public void Open(int missionTabID = 0)
	{
		m_bRefreshReserved = false;
		m_NKM_MISSION_TAB_ID = missionTabID;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (m_bFirstOpen)
		{
			ResetMissionTabData();
			m_lvsrTab.PrepareCells();
			m_lvsrMission.PrepareCells();
			m_bFirstOpen = false;
		}
		ResetTabUI();
		OnClickTab(m_NKM_MISSION_TAB_ID, bSet: true);
		SetCompletableMissionAlarm();
		UIOpened();
	}

	private void ResetMissionTabData()
	{
		m_lstMissionTabTemplet.Clear();
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		foreach (NKMMissionTabTemplet value in NKMMissionManager.DicMissionTab.Values)
		{
			if (value == null || !value.EnableByTag || value.m_MissionType != NKM_MISSION_TYPE.COMBINE_GUIDE_MISSION)
			{
				continue;
			}
			if (!NKMMissionManager.CheckMissionTabUnlocked(value.m_tabID, nKMUserData))
			{
				if (value.m_VisibleWhenLocked && !NKMMissionManager.IsMissionTabExpired(value, nKMUserData))
				{
					if (value.m_UnlockInfo.Count <= 1)
					{
						m_lstMissionTabTemplet.Add(value);
					}
					else if (NKMContentUnlockManager.IsContentUnlocked(nKMUserData, value.m_UnlockInfo[0]))
					{
						m_lstMissionTabTemplet.Add(value);
					}
				}
			}
			else if (NKMContentUnlockManager.IsContentUnlocked(nKMUserData, in value.m_UnlockInfo))
			{
				m_lstMissionTabTemplet.Add(value);
			}
		}
		m_lstMissionTabTemplet.Sort(CompTabSort);
		NKMMissionTabTemplet nKMMissionTabTemplet = m_lstMissionTabTemplet.Find((NKMMissionTabTemplet e) => e.m_tabID == m_NKM_MISSION_TAB_ID);
		if (nKMMissionTabTemplet != null)
		{
			return;
		}
		foreach (NKMMissionTabTemplet item in m_lstMissionTabTemplet)
		{
			NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(item.m_completeMissionID);
			if (missionTemplet != null && NKMMissionManager.GetMissionStateData(missionTemplet.m_MissionID).IsMissionCanClear)
			{
				m_NKM_MISSION_TAB_ID = item.m_tabID;
				return;
			}
		}
		foreach (NKMMissionTabTemplet item2 in m_lstMissionTabTemplet)
		{
			NKMMissionTemplet missionTemplet2 = NKMMissionManager.GetMissionTemplet(item2.m_completeMissionID);
			if (missionTemplet2 != null && !NKMMissionManager.GetMissionStateData(missionTemplet2.m_MissionID).IsMissionCompleted)
			{
				m_NKM_MISSION_TAB_ID = item2.m_tabID;
				return;
			}
		}
		m_NKM_MISSION_TAB_ID = m_lstMissionTabTemplet[0].m_tabID;
	}

	private void ResetTabUI()
	{
		if (m_lstMissionTabTemplet.Count <= 0)
		{
			Debug.LogError("NKCUIMissionGuide::ResetMissionTabData - tab data is zero");
			return;
		}
		m_lvsrTab.TotalCount = m_lstMissionTabTemplet.Count;
		m_lvsrTab.RefreshCells();
		m_dicMissionTab[m_NKM_MISSION_TAB_ID].GetToggle().Select(bSelect: true);
	}

	private int CompTabSort(NKMMissionTabTemplet lItem, NKMMissionTabTemplet rItem)
	{
		bool flag = false;
		bool value = false;
		NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(lItem.m_completeMissionID);
		if (missionTemplet != null)
		{
			NKMMissionData missionData = NKCScenManager.CurrentUserData().m_MissionData.GetMissionData(missionTemplet);
			if (missionData != null && missionData.IsComplete)
			{
				flag = true;
			}
		}
		NKMMissionTemplet missionTemplet2 = NKMMissionManager.GetMissionTemplet(rItem.m_completeMissionID);
		if (missionTemplet2 != null)
		{
			NKMMissionData missionData2 = NKCScenManager.CurrentUserData().m_MissionData.GetMissionData(missionTemplet2);
			if (missionData2 != null && missionData2.IsComplete)
			{
				value = true;
			}
		}
		if (flag.CompareTo(value) == 0)
		{
			if (lItem.m_OrderList.CompareTo(rItem.m_OrderList) == 0)
			{
				return lItem.m_tabID.CompareTo(rItem.m_tabID);
			}
			return lItem.m_OrderList.CompareTo(rItem.m_OrderList);
		}
		return flag.CompareTo(value);
	}

	private void OnClickGrowthMoveToCenter()
	{
		int indexPosition = m_lvsrMission.TotalCount - 1;
		bool bCompleteAll;
		NKMMissionTemplet growthMissionIngTempletByTab = NKMMissionManager.GetGrowthMissionIngTempletByTab(m_NKM_MISSION_TAB_ID, out bCompleteAll);
		for (int i = 0; i < m_lstGrowthMissionList.Count; i++)
		{
			if (m_lstGrowthMissionList[i] == growthMissionIngTempletByTab)
			{
				indexPosition = i;
				break;
			}
		}
		m_lvsrMission.StopMovement();
		m_lvsrMission.SetIndexPosition(indexPosition);
	}

	private void OnClickReceive()
	{
		NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(NKMMissionManager.GetMissionTabTemplet(m_NKM_MISSION_TAB_ID).m_completeMissionID);
		if (missionTemplet == null || NKMMissionManager.GetMissionTabTemplet(missionTemplet.m_MissionTabId) == null)
		{
			return;
		}
		NKMMissionManager.MissionStateData missionStateData = NKMMissionManager.GetMissionStateData(missionTemplet);
		if (!missionStateData.IsMissionCompleted)
		{
			if (!missionStateData.IsMissionCanClear)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_MISSION_NEED_GROWTH_ALL_COMPLETE);
			}
			else
			{
				NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_REQ(missionTemplet);
			}
		}
	}

	public void SetUIByCurrTab()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			m_bRefreshReserved = true;
			return;
		}
		ResetTabUI();
		SetUIByTab(m_NKM_MISSION_TAB_ID);
		SetCompletableMissionAlarm();
	}

	private void SetUIByTab(int _NKM_MISSION_TAB_ID)
	{
		if (NKCScenManager.GetScenManager().GetMyUserData() == null)
		{
			return;
		}
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(_NKM_MISSION_TAB_ID);
		if (missionTabTemplet == null)
		{
			return;
		}
		if (!string.IsNullOrEmpty(missionTabTemplet.m_MissionBannerImage))
		{
			NKCUtil.SetImageSprite(m_BannerImg, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("UI_MISSION_GUIDE_BANNER", missionTabTemplet.m_MissionBannerImage));
		}
		if (!m_dicNKMMissionTemplet.ContainsKey(_NKM_MISSION_TAB_ID))
		{
			m_dicNKMMissionTemplet.Add(_NKM_MISSION_TAB_ID, NKMMissionManager.GetMissionTempletListByType(_NKM_MISSION_TAB_ID));
		}
		switch (missionTabTemplet.m_MissionType)
		{
		default:
			BuildMissionTempletListByTab(_NKM_MISSION_TAB_ID);
			break;
		case NKM_MISSION_TYPE.GROWTH:
		case NKM_MISSION_TYPE.GROWTH_UNIT:
		case NKM_MISSION_TYPE.COMBINE_GUIDE_MISSION:
		{
			BuildGrowthMissionTempletByTab(_NKM_MISSION_TAB_ID);
			int indexPosition = 0;
			bool bCompleteAll;
			NKMMissionTemplet templet = NKMMissionManager.GetGrowthMissionIngTempletByTab(m_NKM_MISSION_TAB_ID, out bCompleteAll);
			if (m_lstGrowthMissionList.Contains(templet))
			{
				indexPosition = m_lstGrowthMissionList.FindIndex((NKMMissionTemplet x) => x == templet);
			}
			m_lvsrMission.TotalCount = m_lstGrowthMissionList.Count;
			m_lvsrMission.StopMovement();
			m_lvsrMission.SetIndexPosition(indexPosition);
			break;
		}
		}
		SetAllClearMissionBottomUI();
	}

	public void SetCompletableMissionAlarm()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		foreach (KeyValuePair<int, NKCUIMissionAchieveTab> item in m_dicMissionTab)
		{
			item.Value.SetNewObject(!item.Value.GetLocked() && !item.Value.GetCompleted() && myUserData.m_MissionData.CheckCompletableMission(myUserData, item.Key));
		}
	}

	private void BuildMissionTempletListByTab(int tabID)
	{
		m_lstCurrentList.Clear();
		List<NKMMissionTemplet> list = m_dicNKMMissionTemplet[tabID];
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null || NKMMissionManager.GetMissionTabTemplet(tabID) == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			NKMMissionTemplet nKMMissionTemplet = list[i];
			if (nKMMissionTemplet == null)
			{
				continue;
			}
			if (nKMMissionTemplet.m_MissionRequire != 0)
			{
				NKMMissionData missionData = myUserData.m_MissionData.GetMissionData(nKMMissionTemplet);
				if (missionData == null)
				{
					continue;
				}
				if (missionData.mission_id == nKMMissionTemplet.m_MissionID)
				{
					m_lstCurrentList.Add(nKMMissionTemplet);
					continue;
				}
				missionData = myUserData.m_MissionData.GetMissionDataByMissionId(nKMMissionTemplet.m_MissionRequire);
				if (missionData == null)
				{
					continue;
				}
				if (missionData.IsComplete && missionData.mission_id == nKMMissionTemplet.m_MissionRequire)
				{
					m_lstCurrentList.Add(nKMMissionTemplet);
					continue;
				}
				if (missionData.mission_id <= nKMMissionTemplet.m_MissionRequire)
				{
					continue;
				}
			}
			m_lstCurrentList.Add(nKMMissionTemplet);
		}
		m_lstCurrentList.Sort(NKMMissionManager.Comparer);
	}

	private void SetAllClearMissionBottomUI()
	{
		List<NKMMissionTemplet> missionTempletListByType = NKMMissionManager.GetMissionTempletListByType(m_NKM_MISSION_TAB_ID);
		NKMMissionTemplet lastCompletedMissionTemplet = NKMMissionManager.GetLastCompletedMissionTempletByTab(m_NKM_MISSION_TAB_ID);
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(m_NKM_MISSION_TAB_ID);
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (missionTabTemplet == null || nKMUserData == null)
		{
			return;
		}
		bool flag = false;
		NKM_MISSION_TYPE missionType = missionTabTemplet.m_MissionType;
		flag = (((uint)(missionType - 6) <= 1u || missionType == NKM_MISSION_TYPE.GROWTH_UNIT || missionType == NKM_MISSION_TYPE.COMBINE_GUIDE_MISSION) ? true : false);
		NKCUtil.SetGameobjectActive(m_objBottom, flag);
		NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(missionTabTemplet.m_completeMissionID);
		if (missionTemplet == null)
		{
			return;
		}
		_ = missionTempletListByType.Count;
		int num = 0;
		if (lastCompletedMissionTemplet != null)
		{
			num = missionTempletListByType.FindIndex((NKMMissionTemplet x) => x == lastCompletedMissionTemplet) + 1;
		}
		if (flag)
		{
			m_lbReceiveGauge.text = $"{NKCStringTable.GetString(missionTemplet.m_MissionDesc)} {num} / {missionTempletListByType.Count}";
			m_imgReceiveGauge.value = (float)num / (float)missionTempletListByType.Count;
		}
		else
		{
			NKMMissionManager.MissionStateData missionStateData = NKMMissionManager.GetMissionStateData(missionTemplet);
			m_lbReceiveGauge.text = $"{NKCStringTable.GetString(missionTemplet.m_MissionDesc)} {missionStateData.progressCount} / {missionTemplet.m_Times}";
			m_imgReceiveGauge.value = (float)missionStateData.progressCount / (float)missionTemplet.m_Times;
		}
		for (int num2 = 0; num2 < missionTemplet.m_MissionReward.Count; num2++)
		{
			MissionReward missionReward = missionTemplet.m_MissionReward[num2];
			m_lstReceiveSlots[num2].SetData(NKCUISlot.SlotData.MakeRewardTypeData(missionReward.reward_type, missionReward.reward_id, missionReward.reward_value));
			m_lstReceiveSlots[num2].SetActive(bSet: true);
		}
		for (int num3 = missionTemplet.m_MissionReward.Count; num3 < m_lstReceiveSlots.Length; num3++)
		{
			m_lstReceiveSlots[num3].SetActive(bSet: false);
		}
		NKMMissionManager.MissionStateData missionStateData2 = NKMMissionManager.GetMissionStateData(missionTemplet.m_MissionID);
		m_csbtnReceive?.SetLock(!missionStateData2.IsMissionCanClear);
	}

	public RectTransform GetGrowthMissionSlot(int index)
	{
		NKCUIMissionAchieveSlotGrowth newInstance = NKCUIMissionAchieveSlotGrowth.GetNewInstance(m_lvsrMission.content, "ab_ui_nkm_ui_mission_guide", "NKM_UI_MISSION_GUIDE_SLOT", OnClickMove, OnClickComplete);
		if (newInstance == null)
		{
			return null;
		}
		return newInstance.GetComponent<RectTransform>();
	}

	public void ReturnGrowthMissionSlot(Transform tr)
	{
		NKCUIMissionAchieveSlotGrowth component = tr.GetComponent<NKCUIMissionAchieveSlotGrowth>();
		tr.SetParent(base.transform);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			UnityEngine.Object.Destroy(tr.gameObject);
		}
	}

	public void ProvideGrowthData(Transform tr, int index)
	{
		NKCUIMissionAchieveSlotGrowth component = tr.GetComponent<NKCUIMissionAchieveSlotGrowth>();
		if (component != null)
		{
			NKMMissionTemplet data = m_lstGrowthMissionList[index];
			component.SetData(data);
		}
	}

	private void BuildGrowthMissionTempletByTab(int tabID)
	{
		m_lstGrowthMissionList = m_dicNKMMissionTemplet[tabID];
		m_lstGrowthMissionList.Sort(CompareByID);
	}

	private int CompareByID(NKMMissionTemplet x, NKMMissionTemplet y)
	{
		return x.m_MissionID.CompareTo(y.m_MissionID);
	}

	public void OnClickMove(NKCUIMissionAchieveSlotGrowth cNKCUIMissionAchieveSlot)
	{
		if (!(cNKCUIMissionAchieveSlot == null))
		{
			NKMMissionTemplet nKMMissionTemplet = cNKCUIMissionAchieveSlot.GetNKMMissionTemplet();
			if (nKMMissionTemplet != null)
			{
				NKCContentManager.MoveToShortCut(nKMMissionTemplet.m_ShortCutType, nKMMissionTemplet.m_ShortCut);
			}
		}
	}

	public void OnClickComplete(NKCUIMissionAchieveSlotGrowth cNKCUIMissionAchieveSlot)
	{
		if (!(cNKCUIMissionAchieveSlot == null))
		{
			NKMMissionTemplet nKMMissionTemplet = cNKCUIMissionAchieveSlot.GetNKMMissionTemplet();
			if (nKMMissionTemplet != null)
			{
				NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_REQ(nKMMissionTemplet);
			}
		}
	}

	public RectTransform GetMissionTab(int index)
	{
		NKCUIMissionAchieveTab newInstance = NKCUIMissionAchieveTab.GetNewInstance(m_lvsrTab.content.transform, "ab_ui_nkm_ui_mission_guide", "NKM_UI_MISSION_GUIDE_TAB");
		if (newInstance == null)
		{
			return null;
		}
		return newInstance.GetComponent<RectTransform>();
	}

	public void ReturnMissionTab(Transform tr)
	{
		NKCUIMissionAchieveTab component = tr.GetComponent<NKCUIMissionAchieveTab>();
		tr.SetParent(base.transform);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			UnityEngine.Object.Destroy(tr.gameObject);
		}
	}

	public void ProvideTabData(Transform tr, int index)
	{
		NKCUIMissionAchieveTab component = tr.GetComponent<NKCUIMissionAchieveTab>();
		if (!m_lstMissionTabTemplet[index].EnableByTag)
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
			return;
		}
		if (!m_lstMissionTabTemplet[index].m_Visible)
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(component, bValue: true);
		component.SetData(m_lstMissionTabTemplet[index], m_ctgTab, OnClickTab);
		component.SetCompleteObject(component.GetCompleted());
		component.SetLockObject(bSkipCompletLock: true);
		component.SetSelected(m_lstMissionTabTemplet[index].m_tabID == m_NKM_MISSION_TAB_ID);
		component.gameObject.name = m_lstMissionTabTemplet[index].m_tabID.ToString("D3");
		if (!m_dicMissionTab.ContainsKey(m_lstMissionTabTemplet[index].m_tabID))
		{
			m_dicMissionTab.Add(m_lstMissionTabTemplet[index].m_tabID, component);
		}
		else
		{
			m_dicMissionTab[m_lstMissionTabTemplet[index].m_tabID] = component;
		}
	}

	public void OnClickTab(int tabID, bool bSet)
	{
		if (bSet)
		{
			m_NKM_MISSION_TAB_ID = tabID;
			SetUIByTab(m_NKM_MISSION_TAB_ID);
		}
	}

	public void ReservedRefresh(NKMPacket_MISSION_UPDATE_NOT cNKMPacket_MISSION_UPDATE_NOT)
	{
		foreach (NKMMissionData missionData in cNKMPacket_MISSION_UPDATE_NOT.missionDataList)
		{
			if (NKMMissionManager.GetMissionTemplet(missionData.mission_id)?.m_MissionTabId == m_NKM_MISSION_TAB_ID)
			{
				m_bRefreshReserved = true;
			}
		}
	}

	public void OnRecv(NKMPacket_MISSION_COMPLETE_ALL_ACK cNKMPacket_MISSION_COMPLETE_ALL_ACK)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKCUIResult.OnClose a = null;
		if (NKCGameEventManager.IsWaiting())
		{
			a = NKCGameEventManager.WaitFinished;
		}
		a = (NKCUIResult.OnClose)Delegate.Combine(a, new NKCUIResult.OnClose(SetUIByCurrTab));
		NKCUIResult.Instance.OpenRewardGain(myUserData.m_ArmyData, cNKMPacket_MISSION_COMPLETE_ALL_ACK.rewardDate, cNKMPacket_MISSION_COMPLETE_ALL_ACK.additionalReward, NKCUtilString.GET_STRING_RESULT_MISSION, "", a);
	}

	public void OnMissionComplete(int missionID, NKMRewardData rewardData, NKMAdditionalReward additionalReward)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(missionID);
		if (missionTemplet != null && NKMMissionManager.GetMissionTabTemplet(missionTemplet.m_MissionTabId) != null)
		{
			NKCUIResult.OnClose a = null;
			if (NKCGameEventManager.IsWaiting())
			{
				a = NKCGameEventManager.WaitFinished;
			}
			ResetMissionTabData();
			a = (NKCUIResult.OnClose)Delegate.Combine(a, new NKCUIResult.OnClose(SetUIByCurrTab));
			NKCUIResult.Instance.OpenRewardGain(myUserData.m_ArmyData, rewardData, additionalReward, NKCUtilString.GET_STRING_RESULT_MISSION, "", a);
		}
	}
}
