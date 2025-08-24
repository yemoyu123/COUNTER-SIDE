using System;
using System.Collections.Generic;
using NKC.Templet;
using NKM;
using NKM.Event;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCUIEventSubUIMission : NKCUIEventSubUIBase
{
	[Header("일괄 완료 버튼")]
	public NKCUIComStateButton m_csbtnCompleteAll;

	[Header("이벤트 상점 이동 버튼")]
	public NKCUIComStateButton m_csbtnEventShop;

	[Header("카데고리/탭 프리팹")]
	public NKCUIEventSubUIMissionTab m_pfbCategory;

	[Header("카데고리 버튼 스크롤렉트")]
	public ScrollRect m_srCategory;

	[Header("카데고리 버튼 토글 그룹")]
	public NKCUIComToggleGroup m_tgCategory;

	[Header("미션 슬롯 프리팹")]
	public NKCUIMissionAchieveSlot m_pfbMissionSlot;

	[Header("미션 스크롤렉트")]
	public LoopScrollRect m_srMission;

	[Header("스페셜 미션 슬롯(프리팹 아님)")]
	public NKCUIMissionAchieveSlot m_SpecialMissionSlot;

	[Header("스페셜 미션 첫번째 보상 이름(필요한 경우)")]
	public Text m_lbSpecialMissionRewardName;

	[Header("초기화가 되는 경우 남은시간")]
	public GameObject m_objResetRemainTime;

	public Text m_lbResetRemainTime;

	private NKCEventMissionTemplet m_EventMissionTemplet;

	private DateTime m_NextResetTime;

	private RectTransform m_rtSlotPool;

	private int m_SelectedTabID;

	private Dictionary<int, NKCUIEventSubUIMissionTab> m_dicTab = new Dictionary<int, NKCUIEventSubUIMissionTab>();

	private List<NKMMissionTemplet> m_lstCurrentList = new List<NKMMissionTemplet>();

	private Dictionary<int, List<NKMMissionTemplet>> m_dicNKMMissionTemplet = new Dictionary<int, List<NKMMissionTemplet>>();

	private Stack<NKCUIMissionAchieveSlot> m_stkSlot = new Stack<NKCUIMissionAchieveSlot>();

	private float m_fDeltaTime;

	private RectTransform SlotPool
	{
		get
		{
			if (m_rtSlotPool == null)
			{
				GameObject gameObject = new GameObject("Slotpool", typeof(RectTransform));
				gameObject.transform.SetParent(base.transform);
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localScale = Vector3.one;
				m_rtSlotPool = gameObject.GetComponent<RectTransform>();
				gameObject.SetActive(value: false);
			}
			return m_rtSlotPool;
		}
	}

	public override void Init()
	{
		base.Init();
		RectTransform component = GetComponent<RectTransform>();
		if (component != null)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(component);
		}
		m_srMission.dOnGetObject += GetMissionSlot;
		m_srMission.dOnReturnObject += ReturnMissionSlot;
		m_srMission.dOnProvideData += ProvideData;
		m_srMission.ContentConstraintCount = 1;
		m_srMission.PrepareCells();
		NKCUtil.SetScrollHotKey(m_srMission);
		NKCUtil.SetGameobjectActive(m_csbtnCompleteAll, bValue: true);
		NKCUtil.SetButtonClickDelegate(m_csbtnEventShop, OnMovetoShop);
		NKCUtil.SetButtonClickDelegate(m_csbtnEventShortcut, base.OnMoveShortcut);
		NKCUtil.SetButtonClickDelegate(m_csbtnCompleteAll, OnCompleteAll);
		m_SpecialMissionSlot?.Init();
	}

	public override void UnHide()
	{
		base.UnHide();
		Refresh();
	}

	public override void Open(NKMEventTabTemplet tabTemplet)
	{
		m_tabTemplet = tabTemplet;
		if (m_tabTemplet != null)
		{
			m_EventMissionTemplet = NKCEventMissionTemplet.Find(m_tabTemplet.Key);
			if (m_EventMissionTemplet != null)
			{
				SetDateLimit();
				NKCUtil.SetGameobjectActive(m_csbtnEventShop, m_EventMissionTemplet.m_ShortcutType != NKM_SHORTCUT_TYPE.SHORTCUT_NONE);
				PrepareMissionTab(m_EventMissionTemplet);
				Refresh();
			}
		}
	}

	public override void Refresh()
	{
		RefreshMissionTab();
		SelectTab(m_SelectedTabID);
		if (m_EventMissionTemplet != null)
		{
			SetSpecialMissionData(m_EventMissionTemplet.m_SpecialMissionTab);
		}
	}

	private void SelectTab(int tabID)
	{
		if (m_dicTab.TryGetValue(tabID, out var value))
		{
			value.m_tglButton.Select(bSelect: true, bForce: true);
		}
		m_SelectedTabID = tabID;
		BuildAllMissionTempletListByTab(tabID);
		m_srMission.TotalCount = m_lstCurrentList.Count;
		m_srMission.StopMovement();
		m_srMission.SetIndexPosition(0);
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		NKMUserMissionData missionData = NKCScenManager.GetScenManager().GetMyUserData().m_MissionData;
		if (missionData != null)
		{
			if (!missionData.CheckCompletableMission(myUserData, tabID))
			{
				m_csbtnCompleteAll?.Lock();
			}
			else
			{
				m_csbtnCompleteAll?.UnLock();
			}
		}
	}

	private void OnMovetoShop()
	{
		NKCContentManager.MoveToShortCut(m_EventMissionTemplet.m_ShortcutType, m_EventMissionTemplet.m_ShortcutParam);
	}

	private void OnCompleteAll()
	{
		NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_ALL_REQ(m_SelectedTabID);
	}

	public RectTransform GetMissionSlot(int index)
	{
		RectTransform parent = ((m_srMission != null) ? m_srMission.content : null);
		NKCUIMissionAchieveSlot nKCUIMissionAchieveSlot;
		if (m_stkSlot.Count > 0)
		{
			nKCUIMissionAchieveSlot = m_stkSlot.Pop();
			nKCUIMissionAchieveSlot.transform.SetParent(parent);
		}
		else
		{
			nKCUIMissionAchieveSlot = UnityEngine.Object.Instantiate(m_pfbMissionSlot, parent);
			nKCUIMissionAchieveSlot.Init();
		}
		return nKCUIMissionAchieveSlot.GetComponent<RectTransform>();
	}

	public void ReturnMissionSlot(Transform tr)
	{
		tr.gameObject.SetActive(value: false);
		NKCUIMissionAchieveSlot component = tr.GetComponent<NKCUIMissionAchieveSlot>();
		if (component != null)
		{
			m_stkSlot.Push(component);
		}
		tr.SetParent(SlotPool);
	}

	public void ProvideData(Transform tr, int index)
	{
		NKCUIMissionAchieveSlot component = tr.GetComponent<NKCUIMissionAchieveSlot>();
		if (component != null)
		{
			NKMMissionTemplet cNKMMissionTemplet = m_lstCurrentList[index];
			component.SetData(cNKMMissionTemplet, OnClickMove, OnClickComplete);
		}
	}

	public void OnClickMove(NKCUIMissionAchieveSlot cNKCUIMissionAchieveSlot)
	{
		if (cNKCUIMissionAchieveSlot == null)
		{
			return;
		}
		NKMMissionTemplet nKMMissionTemplet = cNKCUIMissionAchieveSlot.GetNKMMissionTemplet();
		if (nKMMissionTemplet == null)
		{
			return;
		}
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(nKMMissionTemplet.m_MissionTabId);
		if (missionTabTemplet != null)
		{
			if (NKMMissionManager.IsMissionTabExpired(missionTabTemplet, NKCScenManager.CurrentUserData()))
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_MISSION_EXPIRED, Refresh, NKCUtilString.GET_STRING_CONFIRM);
			}
			else
			{
				NKCContentManager.MoveToShortCut(nKMMissionTemplet.m_ShortCutType, nKMMissionTemplet.m_ShortCut);
			}
		}
	}

	public void OnClickComplete(NKCUIMissionAchieveSlot cNKCUIMissionAchieveSlot)
	{
		if (cNKCUIMissionAchieveSlot == null)
		{
			return;
		}
		NKMMissionTemplet nKMMissionTemplet = cNKCUIMissionAchieveSlot.GetNKMMissionTemplet();
		if (nKMMissionTemplet == null)
		{
			return;
		}
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(nKMMissionTemplet.m_MissionTabId);
		if (missionTabTemplet == null)
		{
			return;
		}
		if (m_tabTemplet.m_EventType == NKM_EVENT_TYPE.ONTIME)
		{
			NKMMissionData missionData = NKMMissionManager.GetMissionData(nKMMissionTemplet);
			if (missionData != null && NKMMissionManager.CheckCanReset(nKMMissionTemplet.m_ResetInterval, missionData))
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_MISSION_EXPIRED, Refresh, NKCUtilString.GET_STRING_CONFIRM);
				return;
			}
		}
		if (NKMMissionManager.IsMissionTabExpired(missionTabTemplet, NKCScenManager.CurrentUserData()))
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_MISSION_EXPIRED, Refresh, NKCUtilString.GET_STRING_CONFIRM);
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_REQ(nKMMissionTemplet);
		}
	}

	private void BuildAllMissionTempletListByTab(int tabID)
	{
		if (!m_dicNKMMissionTemplet.ContainsKey(tabID))
		{
			m_dicNKMMissionTemplet.Add(tabID, NKMMissionManager.GetMissionTempletListByType(tabID));
		}
		m_lstCurrentList = m_dicNKMMissionTemplet[tabID];
		if (m_tabTemplet.m_EventType == NKM_EVENT_TYPE.MISSION)
		{
			m_lstCurrentList.Sort(NKMMissionManager.Comparer);
		}
	}

	private void BuildMissionTempletListByTab(int tabID)
	{
		m_lstCurrentList.Clear();
		if (!m_dicNKMMissionTemplet.ContainsKey(tabID))
		{
			m_dicNKMMissionTemplet.Add(tabID, NKMMissionManager.GetMissionTempletListByType(tabID));
		}
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

	private void PrepareMissionTab(NKCEventMissionTemplet eventMissionTemplet)
	{
		if (eventMissionTemplet.m_lstMissionTab.Count == 1)
		{
			NKCUtil.SetGameobjectActive(m_srCategory, bValue: false);
			m_SelectedTabID = eventMissionTemplet.m_lstMissionTab[0];
			return;
		}
		if (m_srCategory == null || m_pfbCategory == null)
		{
			Debug.LogError($"Event {eventMissionTemplet.m_EventID} require Category, but prefab don't have it");
			return;
		}
		NKCUtil.SetGameobjectActive(m_srCategory, bValue: true);
		List<int> list = new List<int>(eventMissionTemplet.m_lstMissionTab);
		list.Sort();
		foreach (int item in list)
		{
			if (!m_dicTab.TryGetValue(item, out var value))
			{
				value = UnityEngine.Object.Instantiate(m_pfbCategory, m_srCategory.content);
				m_dicTab.Add(item, value);
			}
			value.transform.SetAsLastSibling();
			NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(item);
			if (missionTabTemplet != null)
			{
				value.SetData(missionTabTemplet, m_tgCategory, OnSelectTab);
			}
		}
		if (m_SelectedTabID != 0)
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			foreach (int item2 in list)
			{
				NKMMissionManager.GetMissionTabTemplet(item2);
				if (m_dicTab.TryGetValue(item2, out var value2) && !value2.Locked && !value2.Completed)
				{
					if (nKMUserData.m_MissionData.CheckCompletableMission(nKMUserData, item2))
					{
						m_SelectedTabID = item2;
						break;
					}
					if (m_SelectedTabID == 0)
					{
						m_SelectedTabID = item2;
					}
				}
			}
		}
		if (m_SelectedTabID == 0)
		{
			m_SelectedTabID = eventMissionTemplet.m_lstMissionTab[0];
		}
	}

	private void RefreshMissionTab()
	{
		foreach (KeyValuePair<int, NKCUIEventSubUIMissionTab> item in m_dicTab)
		{
			NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(item.Key);
			item.Value.SetData(missionTabTemplet, m_tgCategory, OnSelectTab);
		}
	}

	private void SetSpecialMissionData(int specialMissionTabID)
	{
		if (m_SpecialMissionSlot == null)
		{
			return;
		}
		if (specialMissionTabID <= 0)
		{
			NKCUtil.SetGameobjectActive(m_SpecialMissionSlot, bValue: false);
			return;
		}
		List<NKMMissionTemplet> missionTempletListByType = NKMMissionManager.GetMissionTempletListByType(specialMissionTabID);
		NKMMissionTemplet nKMMissionTemplet = null;
		NKMMissionTemplet nKMMissionTemplet2 = null;
		NKMMissionTemplet nKMMissionTemplet3 = null;
		bool flag = false;
		foreach (NKMMissionTemplet item in missionTempletListByType)
		{
			switch (NKMMissionManager.GetMissionStateData(item).state)
			{
			case NKMMissionManager.MissionState.CAN_COMPLETE:
			case NKMMissionManager.MissionState.REPEAT_CAN_COMPLETE:
			case NKMMissionManager.MissionState.ONGOING:
				nKMMissionTemplet = item;
				flag = true;
				break;
			case NKMMissionManager.MissionState.REPEAT_COMPLETED:
			case NKMMissionManager.MissionState.COMPLETED:
				nKMMissionTemplet2 = item;
				break;
			case NKMMissionManager.MissionState.LOCKED:
				if (nKMMissionTemplet3 == null)
				{
					nKMMissionTemplet3 = item;
				}
				break;
			}
			if (flag)
			{
				break;
			}
		}
		if (nKMMissionTemplet == null)
		{
			if (nKMMissionTemplet2 != null)
			{
				nKMMissionTemplet = nKMMissionTemplet2;
			}
			else
			{
				if (nKMMissionTemplet3 == null)
				{
					NKCUtil.SetGameobjectActive(m_SpecialMissionSlot, bValue: false);
					return;
				}
				nKMMissionTemplet = nKMMissionTemplet3;
			}
		}
		m_NextResetTime = DateTime.MaxValue;
		if (m_objResetRemainTime != null)
		{
			NKCUtil.SetGameobjectActive(m_objResetRemainTime, m_tabTemplet.m_EventType == NKM_EVENT_TYPE.ONTIME && nKMMissionTemplet.m_ResetInterval != NKM_MISSION_RESET_INTERVAL.NONE);
			if (m_objResetRemainTime.activeSelf)
			{
				m_NextResetTime = NKMTime.GetNextResetTime(NKCSynchronizedTime.GetServerUTCTime(), nKMMissionTemplet.m_ResetInterval);
				SetRemainTime();
			}
		}
		m_SpecialMissionSlot.SetData(nKMMissionTemplet, OnClickMove, OnClickComplete);
		NKCUtil.SetGameobjectActive(m_SpecialMissionSlot, bValue: true);
		if (m_lbSpecialMissionRewardName != null && nKMMissionTemplet != null && nKMMissionTemplet.m_MissionReward.Count > 0)
		{
			NKMItemMiscTemplet nKMItemMiscTemplet = NKMItemMiscTemplet.Find(nKMMissionTemplet.m_MissionReward[0].reward_id);
			if (nKMItemMiscTemplet != null)
			{
				NKCUtil.SetLabelText(m_lbSpecialMissionRewardName, nKMItemMiscTemplet.GetItemName());
			}
		}
	}

	private void SetRemainTime()
	{
		if (!NKCSynchronizedTime.IsFinished(m_NextResetTime))
		{
			NKCUtil.SetLabelText(m_lbResetRemainTime, NKCUtilString.GetRemainTimeStringEx(m_NextResetTime));
		}
		else
		{
			Refresh();
		}
	}

	private void OnSelectTab(int tabID, bool bSet)
	{
		if (bSet)
		{
			SelectTab(tabID);
		}
	}

	private void Update()
	{
		if (!(m_objResetRemainTime == null) && m_objResetRemainTime.activeSelf)
		{
			m_fDeltaTime += Time.deltaTime;
			if (m_fDeltaTime > 1f)
			{
				m_fDeltaTime -= 1f;
				SetRemainTime();
			}
		}
	}
}
