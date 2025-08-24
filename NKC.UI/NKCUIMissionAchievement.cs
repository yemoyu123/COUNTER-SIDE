using System;
using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.User;
using NKC.UI.NPC;
using NKC.UI.Result;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIMissionAchievement : NKCUIBase
{
	public const string UI_ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_mission";

	public const string UI_ASSET_NAME = "NKM_UI_MISSION";

	private static NKCUIMissionAchievement m_Instance;

	private const string REPEAT_MISSION_BG_ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_mission_sprite";

	private const string DAILY_REPEAT_MISSION_BG_NAME = "AB_UI_NKM_UI_MISSION_REPEAT_TOP_BG_DAILY";

	private const string WEEKLY_REPEAT_MISSION_BG_NAME = "AB_UI_NKM_UI_MISSION_REPEAT_TOP_BG_WEEKLY";

	public Vector2 DEFAULT_CHAR_POS = new Vector2(-9.97f, -104.7f);

	public LoopScrollRect m_LoopScrollRectTab;

	public Transform m_trTabParent;

	public NKCUIComToggleGroup m_tglGroup;

	public GameObject m_NKM_UI_MISSION_LIST_ACHIEVE;

	public GameObject m_NKM_UI_MISSION_LIST_ACHIEVE_POINT;

	public GameObject m_NKM_UI_MISSION_LIST_REPEAT_POINT;

	public LoopScrollRect m_LoopScrollRect;

	public Text m_NKM_UI_MISSION_ACHIEVE_POINT;

	[Header("반복미션")]
	public Text m_MISSION_REPEAT_TYPE_TITLE;

	public Text m_MISSION_REPEAT_SCORE;

	public Image m_MISSION_REPEAT_BG;

	public Slider m_NKM_UI_MISSION_REPEAT_POINT_SLIDER;

	public List<NKCUIMissionAchieveRepeatBox> m_lstRepeatBox = new List<NKCUIMissionAchieveRepeatBox>();

	public Text m_MISSION_REPEAT_TIME_TEXT;

	[Header("하단 올클리어 미션")]
	public GameObject m_NKM_UI_BOTTOM_ALLCLEAR_MISSION;

	public NKCUIComStateButton m_GROWTH_BOTTOM_BUTTON_QUICK;

	public NKCUIComStateButton m_GROWTH_BOTTOM_BUTTON_RECEIVE;

	public Slider m_GROWTH_BOTTOM_SLIDER;

	public Text m_GROWTH_BOTTOM_SLIDER_TEXT;

	public NKCUIComStateButton m_NKM_UI_ALLCLEAR_MISSION_BOTTOM_BUTTON_ALL;

	[Header("하단 전체받기만 있는 버전")]
	public GameObject m_objCompleteAll;

	public NKCUIComStateButton m_csbtnCompleteAll;

	[Header("반복미션 전체받기 버튼")]
	public NKCUIComStateButton m_REPEAT_BOTTOM_BUTTON_ALL;

	public NKCUIComStateButton m_REPEAT_BOTTOM_BUTTON_ALL_DISABLE;

	[Header("성장미션")]
	public GameObject m_NKM_UI_MISSION_GROWTH;

	public GameObject m_NKM_UI_MISSION_GROWTH_BANNER;

	public Text m_lbGrowthBannerTitle;

	public NKCUINPCSpineIllust m_NKCUINPCSpineIllust;

	public LoopScrollRect m_LoopScrollRectGrowth;

	public List<NKCUISlot> m_lstRewardSlot = new List<NKCUISlot>();

	private bool m_bFirstOpen = true;

	private int m_NKM_MISSION_TAB_ID;

	private List<NKMMissionTabTemplet> m_lstMissionTabTemplet = new List<NKMMissionTabTemplet>();

	private Dictionary<int, List<NKMMissionTemplet>> m_dicNKMMissionTemplet = new Dictionary<int, List<NKMMissionTemplet>>();

	private List<NKMMissionTemplet> m_lstDailyRepeatMissionTemplet = new List<NKMMissionTemplet>();

	private List<NKMMissionTemplet> m_lstWeeklyRepeatMissionTemplet = new List<NKMMissionTemplet>();

	private int m_iDailyIndex;

	private int m_iWeeklyIndex;

	private int m_iMonthlyIndex;

	private float m_fLastUIUpdateTime;

	private NKCASUISpineIllust m_unitIllust;

	private bool m_bRefreshReserved;

	private bool m_bBlockRepeatBox;

	private const int DEFAULT_MISSION_TAB_ID = 2;

	private Dictionary<int, NKCUIMissionAchieveTab> m_dicMissionTab = new Dictionary<int, NKCUIMissionAchieveTab>();

	private List<NKMMissionTemplet> m_lstCurrentList = new List<NKMMissionTemplet>();

	private List<NKMMissionTemplet> m_lstGrowthMissionList = new List<NKMMissionTemplet>();

	public static NKCUIMissionAchievement Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIMissionAchievement>("ab_ui_nkm_ui_mission", "NKM_UI_MISSION", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIMissionAchievement>();
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

	public override string GuideTempletID => "ARTICLE_SYSTEM_MISSION";

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

	public void InitUI()
	{
		base.gameObject.SetActive(value: false);
		m_LoopScrollRect.dOnGetObject += GetMissionSlot;
		m_LoopScrollRect.dOnReturnObject += ReturnMissionSlot;
		m_LoopScrollRect.dOnProvideData += ProvideData;
		m_LoopScrollRect.ContentConstraintCount = 1;
		NKCUtil.SetScrollHotKey(m_LoopScrollRect);
		m_LoopScrollRectGrowth.dOnGetObject += GetGrowthMissionSlot;
		m_LoopScrollRectGrowth.dOnReturnObject += ReturnGrowthMissionSlot;
		m_LoopScrollRectGrowth.dOnProvideData += ProvideGrowthData;
		m_LoopScrollRectGrowth.ContentConstraintCount = 1;
		NKCUtil.SetScrollHotKey(m_LoopScrollRectGrowth);
		m_LoopScrollRectTab.dOnGetObject += GetMissionTab;
		m_LoopScrollRectTab.dOnReturnObject += ReturnMissionTab;
		m_LoopScrollRectTab.dOnProvideData += ProvideTabData;
		m_LoopScrollRectTab.ContentConstraintCount = 1;
		m_tglGroup.m_bCallbackOnUnSelect = true;
		m_GROWTH_BOTTOM_BUTTON_QUICK.PointerClick.RemoveAllListeners();
		m_GROWTH_BOTTOM_BUTTON_QUICK.PointerClick.AddListener(OnClickGrowthMoveToCenter);
		m_GROWTH_BOTTOM_BUTTON_RECEIVE.PointerClick.RemoveAllListeners();
		m_GROWTH_BOTTOM_BUTTON_RECEIVE.PointerClick.AddListener(OnClickGrowthComplete);
		NKCUtil.SetButtonClickDelegate(m_csbtnCompleteAll, OnClickAchievementBundleFinish);
		NKCUtil.SetButtonClickDelegate(m_NKM_UI_ALLCLEAR_MISSION_BOTTOM_BUTTON_ALL, OnClickAchievementBundleFinish);
		m_REPEAT_BOTTOM_BUTTON_ALL.PointerClick.RemoveAllListeners();
		m_REPEAT_BOTTOM_BUTTON_ALL.PointerClick.AddListener(OnClickAchievementBundleFinish);
		for (int i = 0; i < m_lstRewardSlot.Count; i++)
		{
			m_lstRewardSlot[i].Init();
		}
	}

	private void RefreshTabTempletList()
	{
		m_lstMissionTabTemplet = new List<NKMMissionTabTemplet>();
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		foreach (NKMMissionTabTemplet value in NKMMissionManager.DicMissionTab.Values)
		{
			if (!value.EnableByTag || !value.m_Visible || value.m_MissionType == NKM_MISSION_TYPE.GROWTH_COMPLETE || value.m_MissionType == NKM_MISSION_TYPE.COMBINE_GUIDE_MISSION)
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
			else
			{
				if (!NKMContentUnlockManager.IsContentUnlocked(nKMUserData, in value.m_UnlockInfo))
				{
					continue;
				}
				if (value.m_completeMissionID > 0)
				{
					NKMMissionData missionDataByMissionId = nKMUserData.m_MissionData.GetMissionDataByMissionId(value.m_completeMissionID);
					if (missionDataByMissionId != null && missionDataByMissionId.IsComplete)
					{
						continue;
					}
				}
				if (value.m_firstMissionID > 0)
				{
					NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(value.m_firstMissionID);
					if (missionTemplet != null && missionTemplet.m_MissionRequire > 0)
					{
						NKMMissionData missionDataByMissionId2 = nKMUserData.m_MissionData.GetMissionDataByMissionId(missionTemplet.m_MissionRequire);
						if (missionDataByMissionId2 == null || !missionDataByMissionId2.IsComplete)
						{
							continue;
						}
					}
				}
				m_lstMissionTabTemplet.Add(value);
			}
		}
		m_lstMissionTabTemplet.Sort(CompTabSort);
		m_LoopScrollRectTab.TotalCount = m_lstMissionTabTemplet.Count;
		m_LoopScrollRectTab.RefreshCells();
	}

	private int CompTabSort(NKMMissionTabTemplet lItem, NKMMissionTabTemplet rItem)
	{
		if (lItem.m_OrderList.CompareTo(rItem.m_OrderList) == 0)
		{
			return lItem.m_tabID.CompareTo(rItem.m_tabID);
		}
		return lItem.m_OrderList.CompareTo(rItem.m_OrderList);
	}

	public RectTransform GetMissionTab(int index)
	{
		NKCUIMissionAchieveTab newInstance = NKCUIMissionAchieveTab.GetNewInstance(null, "AB_UI_NKM_UI_MISSION", "NKM_UI_MISSION_TAB");
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
		component.SetData(m_lstMissionTabTemplet[index], m_tglGroup, OnClickTab);
		component.SetCompleteObject(component.GetCompleted());
		component.SetLockObject();
		component.SetSelected(m_NKM_MISSION_TAB_ID == m_lstMissionTabTemplet[index].m_tabID);
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

	public RectTransform GetMissionSlot(int index)
	{
		return NKCUIMissionAchieveSlot.GetNewInstance(null, "AB_UI_NKM_UI_MISSION", "NKM_UI_MISSION_LIST_SLOT")?.GetComponent<RectTransform>();
	}

	public void ReturnMissionSlot(Transform tr)
	{
		NKCUIMissionAchieveSlot component = tr.GetComponent<NKCUIMissionAchieveSlot>();
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

	public void ProvideData(Transform tr, int index)
	{
		NKCUIMissionAchieveSlot component = tr.GetComponent<NKCUIMissionAchieveSlot>();
		if (component != null)
		{
			if (0 <= index && index < m_lstCurrentList.Count)
			{
				NKMMissionTemplet cNKMMissionTemplet = m_lstCurrentList[index];
				component.SetData(cNKMMissionTemplet, OnClickMove, OnClickComplete);
			}
			else
			{
				NKCUtil.SetGameobjectActive(component, bValue: false);
			}
		}
	}

	private void BuildMissionTempletListByTab(int tabID)
	{
		m_lstCurrentList.Clear();
		List<NKMMissionTemplet> list = m_dicNKMMissionTemplet[tabID];
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(tabID);
		if (missionTabTemplet == null)
		{
			return;
		}
		if (missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.REPEAT_DAILY)
		{
			m_lstDailyRepeatMissionTemplet.Clear();
		}
		if (missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.REPEAT_WEEKLY)
		{
			m_lstWeeklyRepeatMissionTemplet.Clear();
		}
		for (int i = 0; i < list.Count; i++)
		{
			NKMMissionTemplet nKMMissionTemplet = list[i];
			if (nKMMissionTemplet == null || (missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.REPEAT_DAILY && nKMMissionTemplet.m_ResetInterval != NKM_MISSION_RESET_INTERVAL.DAILY) || (missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.REPEAT_WEEKLY && nKMMissionTemplet.m_ResetInterval != NKM_MISSION_RESET_INTERVAL.WEEKLY))
			{
				continue;
			}
			if (missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.REPEAT_DAILY && nKMMissionTemplet.m_MissionCond.mission_cond == NKM_MISSION_COND.HAVE_DAILY_POINT)
			{
				m_lstDailyRepeatMissionTemplet.Add(nKMMissionTemplet);
				continue;
			}
			if (missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.REPEAT_WEEKLY && nKMMissionTemplet.m_MissionCond.mission_cond == NKM_MISSION_COND.HAVE_WEEKLY_POINT)
			{
				m_lstWeeklyRepeatMissionTemplet.Add(nKMMissionTemplet);
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
		m_iDailyIndex = -1;
		m_iWeeklyIndex = -1;
		m_iMonthlyIndex = -1;
		for (int j = 0; j < m_lstCurrentList.Count; j++)
		{
			NKMMissionManager.MissionStateData missionStateData = NKMMissionManager.GetMissionStateData(m_lstCurrentList[j]);
			if (!missionStateData.IsMissionCompleted && missionStateData.progressCount < m_lstCurrentList[j].m_Times)
			{
				if (m_lstCurrentList[j].m_ResetInterval == NKM_MISSION_RESET_INTERVAL.DAILY && m_iDailyIndex < 0)
				{
					m_iDailyIndex = j;
				}
				else if (m_lstCurrentList[j].m_ResetInterval == NKM_MISSION_RESET_INTERVAL.WEEKLY && m_iWeeklyIndex < 0)
				{
					m_iWeeklyIndex = j;
				}
				else if (m_lstCurrentList[j].m_ResetInterval == NKM_MISSION_RESET_INTERVAL.MONTHLY && m_iMonthlyIndex < 0)
				{
					m_iMonthlyIndex = j;
				}
			}
		}
		m_lstDailyRepeatMissionTemplet.Sort(CompRepeatSort);
		m_lstWeeklyRepeatMissionTemplet.Sort(CompRepeatSort);
	}

	private int CompRepeatSort(NKMMissionTemplet x, NKMMissionTemplet y)
	{
		return x.m_Times.CompareTo(y.m_Times);
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
				m_NKM_MISSION_TAB_ID = 2;
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_MISSION_EXPIRED, RefreshMissionUI, NKCUtilString.GET_STRING_CONFIRM);
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
		if (missionTabTemplet != null)
		{
			if (NKMMissionManager.IsMissionTabExpired(missionTabTemplet, NKCScenManager.CurrentUserData()))
			{
				m_NKM_MISSION_TAB_ID = 2;
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_MISSION_EXPIRED, RefreshMissionUI, NKCUtilString.GET_STRING_CONFIRM);
			}
			else
			{
				NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_REQ(nKMMissionTemplet);
			}
		}
	}

	public RectTransform GetGrowthMissionSlot(int index)
	{
		NKCUIMissionAchieveSlotGrowth newInstance = NKCUIMissionAchieveSlotGrowth.GetNewInstance(null, OnClickMove, OnClickComplete);
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

	public void Open(int reservedTabID = 0)
	{
		m_bRefreshReserved = false;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (m_bFirstOpen)
		{
			m_LoopScrollRect.PrepareCells();
			m_LoopScrollRectGrowth.PrepareCells();
			m_LoopScrollRectTab.PrepareCells();
			m_bFirstOpen = false;
		}
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(reservedTabID);
		if (missionTabTemplet != null && missionTabTemplet.EnableByTag && missionTabTemplet.m_Visible)
		{
			m_NKM_MISSION_TAB_ID = reservedTabID;
		}
		else
		{
			m_NKM_MISSION_TAB_ID = 2;
		}
		RefreshTabTempletList();
		m_dicMissionTab[m_NKM_MISSION_TAB_ID].GetToggle().Select(bSelect: true);
		SetUIByTab(m_NKM_MISSION_TAB_ID);
		SetCompletableMissionAlarm();
		UIOpened();
		CheckTutorial();
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

	public void SelectNextTab()
	{
		RefreshTabTempletList();
		NKMMissionTabTemplet nextMissionTabTemplet = NKMMissionManager.GetNextMissionTabTemplet(m_NKM_MISSION_TAB_ID);
		if (nextMissionTabTemplet == null)
		{
			m_dicMissionTab[2].GetToggle().Select(bSelect: true, bForce: false, bImmediate: true);
			m_NKM_MISSION_TAB_ID = 2;
		}
		else if (m_dicMissionTab.ContainsKey(nextMissionTabTemplet.m_tabID))
		{
			m_dicMissionTab[nextMissionTabTemplet.m_tabID].GetToggle().Select(bSelect: true, bForce: false, bImmediate: true);
			m_NKM_MISSION_TAB_ID = nextMissionTabTemplet.m_tabID;
		}
	}

	private void OnClickDailyBox(NKMMissionTemplet missionTemplet)
	{
		if (missionTemplet == null || m_bBlockRepeatBox)
		{
			return;
		}
		NKMMissionData missionData = NKCScenManager.CurrentUserData().m_MissionData.GetMissionData(missionTemplet);
		if (missionData != null && !NKMMissionManager.CheckCanReset(missionTemplet.m_ResetInterval, missionData))
		{
			NKMMissionTemplet missionTemplet2 = NKMMissionManager.GetMissionTemplet(missionData.mission_id);
			if (missionTemplet2 != null && !missionData.IsComplete && missionData.times >= missionTemplet.m_Times && missionData.mission_id <= missionTemplet.m_MissionID)
			{
				m_bBlockRepeatBox = true;
				NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_REQ(missionTemplet2);
				return;
			}
		}
		if (missionTemplet.m_MissionReward.Count > 0)
		{
			NKCUISlotListViewer.Instance.OpenMissionRewardList(missionTemplet.m_MissionReward);
		}
	}

	public void SetUIByCurrTab()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			m_bRefreshReserved = true;
			return;
		}
		SetUIByTab(m_NKM_MISSION_TAB_ID);
		SetCompletableMissionAlarm();
	}

	private void SetUIByTab(int _NKM_MISSION_TAB_ID)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(_NKM_MISSION_TAB_ID);
		if (missionTabTemplet == null)
		{
			return;
		}
		m_bBlockRepeatBox = false;
		NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_ACHIEVE, missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.ACHIEVE || missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.REPEAT_DAILY || missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.REPEAT_WEEKLY || missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.EMBLEM);
		NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_ACHIEVE_POINT, missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.ACHIEVE || missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.TITLE || missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.TROPHY);
		NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_REPEAT_POINT, missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.REPEAT_DAILY || missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.REPEAT_WEEKLY);
		NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_GROWTH_BANNER, missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.GROWTH_UNIT);
		if (!m_dicNKMMissionTemplet.ContainsKey(_NKM_MISSION_TAB_ID))
		{
			m_dicNKMMissionTemplet.Add(_NKM_MISSION_TAB_ID, NKMMissionManager.GetMissionTempletListByType(_NKM_MISSION_TAB_ID));
		}
		switch (missionTabTemplet.m_MissionType)
		{
		default:
			NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_ACHIEVE, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_GROWTH, bValue: false);
			BuildMissionTempletListByTab(_NKM_MISSION_TAB_ID);
			m_LoopScrollRect.TotalCount = m_lstCurrentList.Count;
			m_LoopScrollRect.StopMovement();
			m_LoopScrollRect.SetIndexPosition(0);
			if (missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.REPEAT_DAILY || missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.REPEAT_WEEKLY)
			{
				m_LoopScrollRect.GetComponent<RectTransform>().offsetMax = new Vector2(0f, -144f);
				m_LoopScrollRect.GetComponent<RectTransform>().offsetMin = new Vector2(0f, -33f);
			}
			else
			{
				m_LoopScrollRect.GetComponent<RectTransform>().offsetMax = new Vector2(0f, 0f);
				m_LoopScrollRect.GetComponent<RectTransform>().offsetMin = new Vector2(0f, 4.95f);
			}
			break;
		case NKM_MISSION_TYPE.GROWTH:
		case NKM_MISSION_TYPE.GROWTH_UNIT:
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_ACHIEVE, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_GROWTH, bValue: true);
			BuildGrowthMissionTempletByTab(_NKM_MISSION_TAB_ID);
			int indexPosition = 0;
			bool bCompleteAll;
			NKMMissionTemplet templet = NKMMissionManager.GetGrowthMissionIngTempletByTab(m_NKM_MISSION_TAB_ID, out bCompleteAll);
			if (m_lstGrowthMissionList.Contains(templet))
			{
				indexPosition = m_lstGrowthMissionList.FindIndex((NKMMissionTemplet x) => x == templet);
			}
			m_LoopScrollRectGrowth.TotalCount = m_lstGrowthMissionList.Count;
			m_LoopScrollRectGrowth.StopMovement();
			m_LoopScrollRectGrowth.SetIndexPosition(indexPosition);
			if (missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.GROWTH_UNIT)
			{
				m_LoopScrollRectGrowth.GetComponent<RectTransform>().offsetMin = new Vector2(420f, 159f);
			}
			else
			{
				m_LoopScrollRectGrowth.GetComponent<RectTransform>().offsetMin = new Vector2(0f, 159f);
			}
			break;
		}
		}
		if (missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.REPEAT_DAILY || missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.REPEAT_WEEKLY)
		{
			List<NKMMissionTemplet> repeatMissionTempletByTab = GetRepeatMissionTempletByTab(missionTabTemplet.m_MissionType);
			List<float> list = CalcRepeatBoxPosition(repeatMissionTempletByTab);
			long num = 0L;
			Sprite sprite = null;
			if (missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.REPEAT_DAILY)
			{
				NKCUtil.SetLabelText(m_MISSION_REPEAT_TYPE_TITLE, NKMItemManager.GetItemMiscTempletByID(203).GetItemName());
				sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_mission_sprite", "AB_UI_NKM_UI_MISSION_REPEAT_TOP_BG_DAILY");
			}
			else
			{
				NKCUtil.SetLabelText(m_MISSION_REPEAT_TYPE_TITLE, NKMItemManager.GetItemMiscTempletByID(204).GetItemName());
				sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_mission_sprite", "AB_UI_NKM_UI_MISSION_REPEAT_TOP_BG_WEEKLY");
			}
			if (repeatMissionTempletByTab.Count > 0)
			{
				NKMMissionData missionData = myUserData.m_MissionData.GetMissionData(repeatMissionTempletByTab[0]);
				if (missionData != null && !NKMMissionManager.CheckCanReset(repeatMissionTempletByTab[0].m_ResetInterval, missionData))
				{
					num = missionData.times;
				}
			}
			NKCUtil.SetLabelText(m_MISSION_REPEAT_SCORE, num.ToString());
			NKCUtil.SetImageSprite(m_MISSION_REPEAT_BG, sprite);
			for (int num2 = 0; num2 < m_lstRepeatBox.Count; num2++)
			{
				if (num2 < repeatMissionTempletByTab.Count)
				{
					NKCUtil.SetGameobjectActive(m_lstRepeatBox[num2], bValue: true);
					m_lstRepeatBox[num2].SetData(repeatMissionTempletByTab[num2], OnClickDailyBox);
					m_lstRepeatBox[num2].GetComponent<RectTransform>().anchoredPosition = new Vector2(list[num2], m_lstRepeatBox[num2].GetComponent<RectTransform>().anchoredPosition.y);
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_lstRepeatBox[num2], bValue: false);
				}
			}
			if (repeatMissionTempletByTab.Count > 0)
			{
				float num3 = (float)num / (float)repeatMissionTempletByTab[repeatMissionTempletByTab.Count - 1].m_Times;
				if (num3 <= 0f)
				{
					num3 = 0f;
				}
				if (num3 >= 1f)
				{
					num3 = 1f;
				}
				m_NKM_UI_MISSION_REPEAT_POINT_SLIDER.value = num3;
			}
			bool flag = myUserData.m_MissionData.CheckCompletableMission(myUserData, _NKM_MISSION_TAB_ID);
			NKCUtil.SetGameobjectActive(m_REPEAT_BOTTOM_BUTTON_ALL, flag);
			NKCUtil.SetGameobjectActive(m_REPEAT_BOTTOM_BUTTON_ALL_DISABLE, !flag);
			NKMTime.TimePeriod timePeriod = ((missionTabTemplet.m_MissionType != NKM_MISSION_TYPE.REPEAT_DAILY) ? NKMTime.TimePeriod.Week : NKMTime.TimePeriod.Day);
			NKCUtil.SetLabelText(m_MISSION_REPEAT_TIME_TEXT, NKCUtilString.GetResetTimeString(NKCSynchronizedTime.GetServerUTCTime(), timePeriod, 3));
		}
		else if (missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.GROWTH_UNIT)
		{
			if (!string.IsNullOrEmpty(missionTabTemplet.m_MainUnitStrID))
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(missionTabTemplet.m_MainUnitStrID);
				if (unitTempletBase != null)
				{
					NKCUtil.SetLabelText(m_lbGrowthBannerTitle, unitTempletBase.GetUnitName());
					m_unitIllust = AddSpineIllustration(unitTempletBase.m_UnitStrID);
				}
				if (m_unitIllust != null && m_NKCUINPCSpineIllust != null)
				{
					m_NKCUINPCSpineIllust.m_spUnitIllust = m_unitIllust.m_SpineIllustInstant_SkeletonGraphic;
					m_unitIllust.SetParent(m_NKCUINPCSpineIllust.transform, worldPositionStays: false);
					m_unitIllust.SetAnchoredPosition(DEFAULT_CHAR_POS);
				}
			}
		}
		else
		{
			m_NKM_UI_MISSION_ACHIEVE_POINT.text = myUserData.GetMissionAchievePoint().ToString();
			bool flag2 = myUserData.m_MissionData.CheckCompletableMission(myUserData, _NKM_MISSION_TAB_ID);
			m_csbtnCompleteAll?.SetLock(!flag2);
			m_NKM_UI_ALLCLEAR_MISSION_BOTTOM_BUTTON_ALL?.SetLock(!flag2);
		}
		SetAllClearMissionBottomUI();
	}

	private bool IsShowCompleteAllButton(NKM_MISSION_TYPE tabType)
	{
		switch (tabType)
		{
		default:
			return true;
		case NKM_MISSION_TYPE.TUTORIAL:
		case NKM_MISSION_TYPE.REPEAT_DAILY:
		case NKM_MISSION_TYPE.REPEAT_WEEKLY:
		case NKM_MISSION_TYPE.GROWTH:
		case NKM_MISSION_TYPE.GROWTH_COMPLETE:
		case NKM_MISSION_TYPE.GROWTH_UNIT:
		case NKM_MISSION_TYPE.MAX:
			return false;
		}
	}

	private NKCASUISpineIllust AddSpineIllustration(string prefabStrID)
	{
		return (NKCASUISpineIllust)NKCResourceUtility.OpenSpineIllustWithManualNaming(prefabStrID);
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
		NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(missionTabTemplet.m_completeMissionID);
		_ = missionTempletListByType.Count;
		int num = 0;
		if (lastCompletedMissionTemplet != null)
		{
			num = missionTempletListByType.FindIndex((NKMMissionTemplet x) => x == lastCompletedMissionTemplet) + 1;
		}
		if (missionTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_BOTTOM_ALLCLEAR_MISSION, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_ALLCLEAR_MISSION_BOTTOM_BUTTON_ALL, IsShowCompleteAllButton(missionTabTemplet.m_MissionType));
			NKCUtil.SetGameobjectActive(m_objCompleteAll, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_BOTTOM_ALLCLEAR_MISSION, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_ALLCLEAR_MISSION_BOTTOM_BUTTON_ALL, bValue: false);
			NKCUtil.SetGameobjectActive(m_objCompleteAll, IsShowCompleteAllButton(missionTabTemplet.m_MissionType));
		}
		bool flag = false;
		NKM_MISSION_TYPE missionType = missionTabTemplet.m_MissionType;
		flag = (((uint)(missionType - 6) <= 1u || missionType == NKM_MISSION_TYPE.GROWTH_UNIT) ? true : false);
		NKCUtil.SetGameobjectActive(m_GROWTH_BOTTOM_BUTTON_QUICK, flag);
		if (missionTemplet != null)
		{
			if (flag)
			{
				m_GROWTH_BOTTOM_SLIDER_TEXT.text = $"{NKCStringTable.GetString(missionTemplet.m_MissionDesc)} {num} / {missionTempletListByType.Count}";
				m_GROWTH_BOTTOM_SLIDER.value = (float)num / (float)missionTempletListByType.Count;
			}
			else
			{
				NKMMissionManager.MissionStateData missionStateData = NKMMissionManager.GetMissionStateData(missionTemplet);
				m_GROWTH_BOTTOM_SLIDER_TEXT.text = $"{NKCStringTable.GetString(missionTemplet.m_MissionDesc)} {missionStateData.progressCount} / {missionTemplet.m_Times}";
				m_GROWTH_BOTTOM_SLIDER.value = (float)missionStateData.progressCount / (float)missionTemplet.m_Times;
			}
			for (int num2 = 0; num2 < missionTemplet.m_MissionReward.Count; num2++)
			{
				MissionReward missionReward = missionTemplet.m_MissionReward[num2];
				m_lstRewardSlot[num2].SetData(NKCUISlot.SlotData.MakeRewardTypeData(missionReward.reward_type, missionReward.reward_id, missionReward.reward_value));
				m_lstRewardSlot[num2].SetActive(bSet: true);
			}
			for (int num3 = missionTemplet.m_MissionReward.Count; num3 < m_lstRewardSlot.Count; num3++)
			{
				m_lstRewardSlot[num3].SetActive(bSet: false);
			}
			NKMMissionManager.MissionStateData missionStateData2 = NKMMissionManager.GetMissionStateData(missionTemplet.m_MissionID);
			m_GROWTH_BOTTOM_BUTTON_RECEIVE?.SetLock(!missionStateData2.IsMissionCanClear);
		}
	}

	public void RefreshMissionUI()
	{
		m_NKM_MISSION_TAB_ID = 2;
		Open();
	}

	public void OnClickTab(int tabID, bool bSet)
	{
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(tabID);
		if (missionTabTemplet != null && NKMMissionManager.IsMissionTabExpired(missionTabTemplet, NKCScenManager.CurrentUserData()))
		{
			m_NKM_MISSION_TAB_ID = 2;
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_MISSION_EXPIRED, RefreshMissionUI, NKCUtilString.GET_STRING_CONFIRM);
		}
		else if (bSet)
		{
			m_NKM_MISSION_TAB_ID = tabID;
			SetUIByTab(m_NKM_MISSION_TAB_ID);
		}
	}

	public void OnClickAchievementBundleFinish()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			NKMUserMissionData missionData = NKCScenManager.GetScenManager().GetMyUserData().m_MissionData;
			if (missionData != null && missionData.CheckCompletableMission(myUserData, m_NKM_MISSION_TAB_ID))
			{
				NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_ALL_REQ(m_NKM_MISSION_TAB_ID);
			}
		}
	}

	public void OnClickGrowthComplete()
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

	public void OnClickGrowthMoveToCenter()
	{
		int indexPosition = m_LoopScrollRectGrowth.TotalCount - 1;
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
		m_LoopScrollRectGrowth.StopMovement();
		m_LoopScrollRectGrowth.SetIndexPosition(indexPosition);
	}

	public void OnClickMoveToDaily()
	{
		if (m_iDailyIndex >= 0)
		{
			m_LoopScrollRect.SetIndexPosition(m_iDailyIndex);
		}
	}

	public void OnClickMoveToWeekly()
	{
		if (m_iWeeklyIndex >= 0)
		{
			m_LoopScrollRect.SetIndexPosition(m_iWeeklyIndex);
		}
	}

	public void OnClickMoveToMonthly()
	{
		if (m_iMonthlyIndex >= 0)
		{
			m_LoopScrollRect.SetIndexPosition(m_iMonthlyIndex);
		}
	}

	private void Update()
	{
		if (!base.gameObject.activeSelf || !(m_fLastUIUpdateTime + 1f < Time.time))
		{
			return;
		}
		m_fLastUIUpdateTime = Time.time;
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(m_NKM_MISSION_TAB_ID);
		if (missionTabTemplet == null)
		{
			return;
		}
		if (missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.REPEAT_DAILY)
		{
			NKCUtil.SetLabelText(m_MISSION_REPEAT_TIME_TEXT, NKCUtilString.GetResetTimeString(NKCSynchronizedTime.GetServerUTCTime(), NKMTime.TimePeriod.Day, 3));
		}
		else if (missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.REPEAT_WEEKLY)
		{
			NKCUtil.SetLabelText(m_MISSION_REPEAT_TIME_TEXT, NKCUtilString.GetResetTimeString(NKCSynchronizedTime.GetServerUTCTime(), NKMTime.TimePeriod.Week, 3));
		}
		if (!myUserData.m_MissionData.HasAlreadyCompleteMission(missionTabTemplet.m_tabID))
		{
			return;
		}
		bool flag = false;
		foreach (KeyValuePair<int, NKMMissionData> item in myUserData.m_MissionData.GetAlreadyCompleteMission(missionTabTemplet.m_tabID))
		{
			NKMMissionData value = item.Value;
			if (value != null)
			{
				NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(value.mission_id);
				if (missionTemplet != null && (missionTemplet.m_ResetInterval == NKM_MISSION_RESET_INTERVAL.DAILY || missionTemplet.m_ResetInterval == NKM_MISSION_RESET_INTERVAL.MONTHLY || missionTemplet.m_ResetInterval == NKM_MISSION_RESET_INTERVAL.WEEKLY) && NKMMissionManager.CheckCanReset(missionTemplet.m_ResetInterval, value))
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			SetUIByCurrTab();
		}
	}

	private void OnDestroyImpl()
	{
	}

	private void OnDestroy()
	{
		OnDestroyImpl();
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		foreach (KeyValuePair<int, NKCUIMissionAchieveTab> item in m_dicMissionTab)
		{
			item.Value.GetToggle().Select(bSelect: false);
		}
		OnDestroyImpl();
		m_bRefreshReserved = false;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void OnMissionComplete(int missionID, NKMRewardData rewardData, NKMAdditionalReward additionalReward)
	{
		m_bBlockRepeatBox = false;
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(missionID);
		if (missionTemplet == null)
		{
			return;
		}
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(missionTemplet.m_MissionTabId);
		if (missionTabTemplet != null)
		{
			if (missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.GROWTH_COMPLETE)
			{
				SelectNextTab();
			}
			NKCUIResult.OnClose a = null;
			if (NKCGameEventManager.IsWaiting())
			{
				a = NKCGameEventManager.WaitFinished;
			}
			a = (NKCUIResult.OnClose)Delegate.Combine(a, new NKCUIResult.OnClose(SetUIByCurrTab));
			NKCUIResult.Instance.OpenRewardGain(myUserData.m_ArmyData, rewardData, additionalReward, NKCUtilString.GET_STRING_RESULT_MISSION, "", a);
		}
	}

	public void OnRecv(NKMPacket_MISSION_COMPLETE_ALL_ACK cNKMPacket_MISSION_COMPLETE_ALL_ACK)
	{
		m_bBlockRepeatBox = false;
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKCUIResult.OnClose a = null;
		if (NKCGameEventManager.IsWaiting())
		{
			a = NKCGameEventManager.WaitFinished;
		}
		a = (NKCUIResult.OnClose)Delegate.Combine(a, new NKCUIResult.OnClose(SetUIByCurrTab));
		NKCUIResult.Instance.OpenRewardGain(myUserData.m_ArmyData, cNKMPacket_MISSION_COMPLETE_ALL_ACK.rewardDate, cNKMPacket_MISSION_COMPLETE_ALL_ACK.additionalReward, NKCUtilString.GET_STRING_RESULT_MISSION, "", a);
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

	private List<NKMMissionTemplet> GetRepeatMissionTempletByTab(NKM_MISSION_TYPE missionType)
	{
		return missionType switch
		{
			NKM_MISSION_TYPE.REPEAT_DAILY => m_lstDailyRepeatMissionTemplet, 
			NKM_MISSION_TYPE.REPEAT_WEEKLY => m_lstWeeklyRepeatMissionTemplet, 
			_ => new List<NKMMissionTemplet>(), 
		};
	}

	private List<float> CalcRepeatBoxPosition(List<NKMMissionTemplet> lstRepeatTemplet)
	{
		List<float> list = new List<float>();
		if (lstRepeatTemplet.Count == 0)
		{
			return list;
		}
		long times = lstRepeatTemplet[lstRepeatTemplet.Count - 1].m_Times;
		for (int i = 0; i < lstRepeatTemplet.Count; i++)
		{
			list.Add((float)lstRepeatTemplet[i].m_Times / (float)times * 1000f);
		}
		return list;
	}

	public void CheckTutorial()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.Achieventment);
	}

	public RectTransform GetRectTransformSlot(int missionID)
	{
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(m_NKM_MISSION_TAB_ID);
		if (missionTabTemplet == null)
		{
			return null;
		}
		switch (missionTabTemplet.m_MissionType)
		{
		case NKM_MISSION_TYPE.ACHIEVE:
		case NKM_MISSION_TYPE.REPEAT_DAILY:
		case NKM_MISSION_TYPE.REPEAT_WEEKLY:
		{
			int num3 = m_lstCurrentList.FindIndex((NKMMissionTemplet v) => v.Key == missionID);
			if (num3 < 0)
			{
				return null;
			}
			m_LoopScrollRectGrowth.StopMovement();
			m_LoopScrollRect.SetIndexPosition(num3);
			NKCUIMissionAchieveSlot[] componentsInChildren2 = m_LoopScrollRect.content.GetComponentsInChildren<NKCUIMissionAchieveSlot>();
			for (int num4 = 0; num4 < componentsInChildren2.Length; num4++)
			{
				NKMMissionTemplet nKMMissionTemplet2 = componentsInChildren2[num4].GetNKMMissionTemplet();
				if (nKMMissionTemplet2 != null && nKMMissionTemplet2.Key == missionID)
				{
					return componentsInChildren2[num4].GetComponent<RectTransform>();
				}
			}
			break;
		}
		case NKM_MISSION_TYPE.GROWTH:
		case NKM_MISSION_TYPE.GROWTH_UNIT:
		{
			int num = m_lstGrowthMissionList.FindIndex((NKMMissionTemplet v) => v.Key == missionID);
			if (num < 0)
			{
				return null;
			}
			m_LoopScrollRectGrowth.StopMovement();
			m_LoopScrollRectGrowth.SetIndexPosition(num);
			NKCUIMissionAchieveSlotGrowth[] componentsInChildren = m_LoopScrollRectGrowth.content.GetComponentsInChildren<NKCUIMissionAchieveSlotGrowth>();
			for (int num2 = 0; num2 < componentsInChildren.Length; num2++)
			{
				NKMMissionTemplet nKMMissionTemplet = componentsInChildren[num2].GetNKMMissionTemplet();
				if (nKMMissionTemplet != null && nKMMissionTemplet.Key == missionID)
				{
					return componentsInChildren[num2].GetComponent<RectTransform>();
				}
			}
			break;
		}
		}
		return null;
	}
}
