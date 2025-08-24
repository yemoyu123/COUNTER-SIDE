using System;
using System.Collections.Generic;
using System.Text;
using ClientPacket.Common;
using ClientPacket.Event;
using DG.Tweening;
using NKC.UI.Collection;
using NKC.UI.Result;
using NKC.UI.Shop;
using NKM;
using NKM.EventPass;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIEventPass : NKCUIBase
{
	public enum ExpFXType
	{
		NONE,
		DIRECT,
		TRANS
	}

	public enum ExpGainType
	{
		LevelUp,
		Gain
	}

	private delegate void SendMissionRequest(EventPassMissionType missionType);

	public delegate void OnResetMissionTime();

	public const string UI_ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_EVENT_PASS";

	public const string UI_ASSET_NAME = "NKM_UI_EVENT_PASS";

	private static NKCUIEventPass m_Instance;

	public Animator m_aniEventPass;

	[Header("레벨, 경험치 정보")]
	public Text m_lbPassLevel;

	public Text m_lbPassExp;

	public Color m_passExpColor;

	public Image m_imgPassExpGauge;

	public float m_fExpGaugeTimeMultiplier;

	public int m_iExpFontSize;

	public GameObject m_objExpRoot;

	public GameObject m_objExpMaxLevel;

	[Header("공통 오브젝트")]
	public Text m_lbPassTime;

	public Text m_lbPassTitle;

	public NKCUICharacterView m_characterView;

	public GameObject m_objEndNotice;

	public int m_iEndWarningRemainDays;

	public Text m_lbEndTimeRemain;

	public Color m_colEndTimeText;

	public GameObject m_RewardSubMenuRedDot;

	public GameObject m_MissionSubMenuRedDot;

	public NKCUIComToggle m_tglRewardSubMenu;

	public NKCUIComToggle m_tglMissionSubMenu;

	public NKCUIComStateButton m_csbtnPassLevelUp;

	public NKCUIComStateButton m_csbtnPurchaseCorePass;

	[Header("이벤트 스테이지 버튼")]
	public NKCUIComStateButton m_csbtnEventStage;

	public Text m_lbEventStage;

	[Header("장비 패스 정보")]
	public NKCUIComEventPassEquip m_eventPassEquip;

	public GameObject m_objEquipRoot;

	[Header("보상 패널")]
	public GameObject m_objRewardListPanel;

	public GameObject m_objRewardRedDot;

	public LoopScrollRect m_LoopRewardScrollRect;

	[Header("최대 레벨 보상")]
	public Animator m_aniFinalReward;

	public NKCUISlot m_finalNormalRewardSlot;

	public NKCUISlot m_finalCoreRewardSlot;

	public Text m_lbFinalRewardLevel;

	public Color m_colFinalRewardFontColor;

	public int m_iFinalRewardFontSize;

	[Header("미션 패널")]
	public GameObject m_objMissionListPanel;

	public LoopScrollRect m_LoopMissionScrollRect;

	public RectTransform m_rtToggleBar;

	public Vector2 m_vToggleDailyMissionPos;

	public Vector2 m_vToggleWeeklyMissionPos;

	public float m_fToggleMissionTime = 0.4f;

	public NKCUIComStateButton m_csbtnDailyMission;

	public NKCUIComStateButton m_csbtnWeeklyMission;

	public NKCUIComStateButton m_csbtnCompleteAllMission;

	public GameObject m_objMissionRedDot;

	public GameObject m_objDailyMissionRedDot;

	public GameObject m_objWeeklyMissionRedDot;

	public GameObject m_objPassMissionComplete;

	public Text m_lbPassMissionCompleteText;

	public Text m_lbPassMissionCompleteExText;

	public GameObject m_objLevelFullMissionDisable;

	public Text m_lbWeekCount;

	public Color m_colDailyCountColor;

	public Color m_colWeeklyCountColor;

	public Text m_lbRefreshTimeRemain;

	public Text m_lbDailyTabText;

	public Text m_lbWeeklyTabText;

	public Color m_colActivatedTabColor;

	public Color m_colDeactivatedTabColor;

	public GameObject m_objMissionResetting;

	public Text m_lbMissionResetMsg;

	[Header("미션 달성도")]
	public GameObject m_objDailyAchieveBG;

	public GameObject m_objWeeklyAchieveBG;

	public GameObject m_objAchieveSlotEffect;

	public NKCUISlot m_achieveSlot;

	public Text m_lbAchieveTitle;

	public Text m_lbAchieveTimeRemain;

	public Image m_imgAchieveGauge;

	public Text m_lbAchieveCount;

	private static bool m_bOpenUIStandby;

	private static bool m_bRewardDot;

	private static bool m_bDailyMissionDot;

	private static bool m_bWeeklyMissionDot;

	private List<int> m_listUpsideMenuResource = new List<int> { 1, 2, 101, 102 };

	private Dictionary<EventPassMissionType, List<NKMEventPassMissionInfo>> m_listMissionInfo = new Dictionary<EventPassMissionType, List<NKMEventPassMissionInfo>>();

	private Dictionary<EventPassMissionType, List<NKMMissionTemplet>> m_missionTempletInfo = new Dictionary<EventPassMissionType, List<NKMMissionTemplet>>();

	private Dictionary<EventPassMissionType, DateTime> m_resetMissionTime = new Dictionary<EventPassMissionType, DateTime>();

	private Dictionary<EventPassMissionType, bool> m_finalMissionCompleted = new Dictionary<EventPassMissionType, bool>();

	private EventPassMissionType m_currentMissionType;

	private Tweener m_tweenerExpGauge;

	private static NKCEventPassDataManager m_NKCEventPassDataManager;

	private int m_iUserPassLevel;

	private int m_iEventPassId;

	private const int MissionResetTime = 5;

	private float m_fUpdateTime;

	private const float UpdateInterval = 1f;

	private bool m_bDailyMissionCompleteAchieved;

	private bool m_bMissionTabClick;

	private bool m_bRefreshScrollRectStandby;

	private SendMissionRequest m_dSendMissionRequest;

	public static OnResetMissionTime m_dOnMissionUpdate;

	public static NKCUIEventPass Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIEventPass>("AB_UI_NKM_UI_EVENT_PASS", "NKM_UI_EVENT_PASS", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanUpInstance).GetInstance<NKCUIEventPass>();
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

	public override string MenuName => NKCUtilString.GET_STRING_EVENTPASS_EVENT_PASS_MENU_TITLE;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override List<int> UpsideMenuShowResourceList => m_listUpsideMenuResource;

	public static bool OpenUIStandby
	{
		get
		{
			return m_bOpenUIStandby;
		}
		set
		{
			m_bOpenUIStandby = value;
		}
	}

	public static NKCEventPassDataManager EventPassDataManager
	{
		set
		{
			m_NKCEventPassDataManager = value;
		}
	}

	public int UserPassLevel => m_iUserPassLevel;

	public static bool RewardRedDot
	{
		get
		{
			return m_bRewardDot;
		}
		set
		{
			m_bRewardDot = value;
			if (IsInstanceOpen)
			{
				m_Instance.SetRewardRedDot(value);
			}
		}
	}

	public static bool DailyMissionRedDot
	{
		get
		{
			return m_bDailyMissionDot;
		}
		set
		{
			m_bDailyMissionDot = value;
			if (IsInstanceOpen)
			{
				m_Instance.SetDailyMissionRedDot(value);
			}
		}
	}

	public static bool WeeklyMissionRedDot
	{
		get
		{
			return m_bWeeklyMissionDot;
		}
		set
		{
			m_bWeeklyMissionDot = value;
			if (IsInstanceOpen)
			{
				m_Instance.SetWeeklyMissionRedDot(value);
			}
		}
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
		m_LoopRewardScrollRect.dOnGetObject += GetRewardSlot;
		m_LoopRewardScrollRect.dOnReturnObject += ReturnRewardSlot;
		m_LoopRewardScrollRect.dOnProvideData += ProvideRewardData;
		m_LoopRewardScrollRect.ContentConstraintCount = 1;
		NKCUtil.SetScrollHotKey(m_LoopRewardScrollRect);
		m_LoopMissionScrollRect.dOnGetObject += GetMissionSlot;
		m_LoopMissionScrollRect.dOnReturnObject += ReturnMissionSlot;
		m_LoopMissionScrollRect.dOnProvideData += ProvideMissionData;
		m_LoopMissionScrollRect.ContentConstraintCount = 1;
		NKCUtil.SetScrollHotKey(m_LoopMissionScrollRect);
		NKCUtil.SetGameobjectActive(m_objRewardListPanel, bValue: true);
		NKCUtil.SetGameobjectActive(m_objMissionListPanel, bValue: true);
		m_LoopRewardScrollRect.PrepareCells();
		m_LoopRewardScrollRect.TotalCount = 0;
		m_LoopRewardScrollRect.RefreshCells();
		m_LoopMissionScrollRect.PrepareCells();
		m_LoopMissionScrollRect.TotalCount = 0;
		m_LoopMissionScrollRect.RefreshCells();
		base.gameObject.SetActive(value: false);
		NKCUtil.SetButtonClickDelegate(m_csbtnPassLevelUp, OnClickPassLevelUp);
		NKCUtil.SetButtonClickDelegate(m_csbtnPurchaseCorePass, OnClickPurchaseCorePass);
		NKCUtil.SetToggleValueChangedDelegate(m_tglRewardSubMenu, OnToggleRewardPanel);
		NKCUtil.SetToggleValueChangedDelegate(m_tglMissionSubMenu, OnToggleMissionPanel);
		NKCUtil.SetButtonClickDelegate(m_csbtnDailyMission, (UnityAction)delegate
		{
			OnClickDailyMission(bAnim: true);
		});
		NKCUtil.SetButtonClickDelegate(m_csbtnWeeklyMission, (UnityAction)delegate
		{
			OnClickWeeklyMission(bAnim: true);
		});
		NKCUtil.SetButtonClickDelegate(m_csbtnCompleteAllMission, OnClickMissionCompleteAll);
		NKCUtil.SetButtonClickDelegate(m_csbtnEventStage, OnClickEventStage);
		if (m_csbtnEventStage != null)
		{
			m_csbtnEventStage.m_bGetCallbackWhileLocked = true;
		}
		m_LoopRewardScrollRect.onValueChanged.AddListener(OnRewardScrollValueChanged);
		m_listMissionInfo.Clear();
		m_listMissionInfo.Add(EventPassMissionType.Daily, null);
		m_listMissionInfo.Add(EventPassMissionType.Weekly, null);
		m_missionTempletInfo.Clear();
		m_missionTempletInfo.Add(EventPassMissionType.Daily, new List<NKMMissionTemplet>());
		m_missionTempletInfo.Add(EventPassMissionType.Weekly, new List<NKMMissionTemplet>());
		m_resetMissionTime.Clear();
		m_resetMissionTime.Add(EventPassMissionType.Daily, GetMissionResetTime(EventPassMissionType.Daily));
		m_resetMissionTime.Add(EventPassMissionType.Weekly, GetMissionResetTime(EventPassMissionType.Weekly));
		m_finalMissionCompleted.Clear();
		m_finalMissionCompleted.Add(EventPassMissionType.Daily, value: false);
		m_finalMissionCompleted.Add(EventPassMissionType.Weekly, value: false);
		m_dSendMissionRequest = null;
		m_currentMissionType = EventPassMissionType.Daily;
		m_finalCoreRewardSlot.Init();
		m_finalNormalRewardSlot.Init();
		m_achieveSlot.Init();
		m_characterView.Init();
	}

	public override void CloseInternal()
	{
		m_aniEventPass.keepAnimatorControllerStateOnDisable = false;
		base.gameObject.SetActive(value: false);
		m_characterView.CleanUp();
	}

	public void Open(NKCEventPassDataManager eventPassDataManager)
	{
		if (m_NKCEventPassDataManager == null)
		{
			if (eventPassDataManager == null)
			{
				return;
			}
			m_NKCEventPassDataManager = eventPassDataManager;
			m_iEventPassId = eventPassDataManager.EventPassId;
			if (m_listMissionInfo[EventPassMissionType.Daily] != null)
			{
				m_listMissionInfo[EventPassMissionType.Daily].Clear();
				m_listMissionInfo[EventPassMissionType.Daily] = null;
			}
			if (m_listMissionInfo[EventPassMissionType.Weekly] != null)
			{
				m_listMissionInfo[EventPassMissionType.Weekly].Clear();
				m_listMissionInfo[EventPassMissionType.Weekly] = null;
			}
			m_missionTempletInfo[EventPassMissionType.Daily].Clear();
			m_missionTempletInfo[EventPassMissionType.Weekly].Clear();
			m_dSendMissionRequest = null;
		}
		base.gameObject.SetActive(value: true);
		m_fUpdateTime = 1f;
		SetCorePassPurchaseState(m_NKCEventPassDataManager.CorePassPurchased);
		NKCUtil.SetGameobjectActive(m_objPassMissionComplete, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLevelFullMissionDisable, bValue: false);
		NKCUtil.SetGameobjectActive(m_objAchieveSlotEffect, bValue: false);
		NKMEventPassTemplet nKMEventPassTemplet = NKMTempletContainer<NKMEventPassTemplet>.Find(m_iEventPassId);
		if (nKMEventPassTemplet != null)
		{
			NKCUtil.SetLabelText(m_lbPassTitle, NKCStringTable.GetString(nKMEventPassTemplet.EventPassTitleStrId));
			NKCUtil.SetLabelText(m_lbPassTime, NKCUtilString.GetTimeIntervalString(nKMEventPassTemplet.EventPassStartDate, nKMEventPassTemplet.EventPassEndDate, NKMTime.INTERVAL_FROM_UTC));
			SetPassLevelExp(m_NKCEventPassDataManager.TotalExp, nKMEventPassTemplet.PassLevelUpExp, nKMEventPassTemplet.PassMaxLevel, ExpFXType.NONE);
			SetMaxLevelRewardFloatingUI(nKMEventPassTemplet);
			SetMaxLevelMainRewardImage(nKMEventPassTemplet, m_characterView, m_eventPassEquip, m_objEquipRoot);
			SetEndNotice(nKMEventPassTemplet);
			SetRewardRedDot(m_bRewardDot);
			SetMissionRedDot(m_bDailyMissionDot, m_bWeeklyMissionDot);
			SetEventMissionButtonState(nKMEventPassTemplet);
			m_tglRewardSubMenu?.Select(bSelect: false, bForce: true);
			m_tglRewardSubMenu?.Select(bSelect: true);
			m_aniEventPass.keepAnimatorControllerStateOnDisable = true;
			m_bRefreshScrollRectStandby = false;
			UIOpened();
		}
	}

	public void OnRecv(NKMPacket_EVENT_PASS_LEVEL_COMPLETE_ACK cNKMPacket_EVENT_PASS_LEVEL_COMPLETE_ACK)
	{
		m_NKCEventPassDataManager.NormalRewardLevel = cNKMPacket_EVENT_PASS_LEVEL_COMPLETE_ACK.rewardNormalLevel;
		m_NKCEventPassDataManager.CoreRewardLevel = cNKMPacket_EVENT_PASS_LEVEL_COMPLETE_ACK.rewardCoreLevel;
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			RefreshScrollRect(initScrollPosition: false);
			NKCUIResult.Instance.OpenRewardGain(myUserData.m_ArmyData, cNKMPacket_EVENT_PASS_LEVEL_COMPLETE_ACK.rewardData, null, NKCUtilString.GET_STRING_RESULT_MISSION, "", null);
		}
	}

	public void OnRecv(NKMPacket_EVENT_PASS_MISSION_ACK cNKMPacket_EVENT_PASS_MISSION_ACK)
	{
		EventPassMissionType missionType = cNKMPacket_EVENT_PASS_MISSION_ACK.missionType;
		if (m_listMissionInfo[missionType] != null)
		{
			m_listMissionInfo[missionType].Clear();
			m_listMissionInfo[missionType] = null;
		}
		m_listMissionInfo[missionType] = cNKMPacket_EVENT_PASS_MISSION_ACK.missionInfoList;
		m_listMissionInfo[missionType].Sort(delegate(NKMEventPassMissionInfo e1, NKMEventPassMissionInfo e2)
		{
			if (e1.slotIndex > e2.slotIndex)
			{
				return 1;
			}
			return (e1.slotIndex < e2.slotIndex) ? (-1) : 0;
		});
		m_resetMissionTime[missionType] = NKCSynchronizedTime.ToUtcTime(cNKMPacket_EVENT_PASS_MISSION_ACK.nextResetDate);
		m_finalMissionCompleted[missionType] = cNKMPacket_EVENT_PASS_MISSION_ACK.isFinalMissionCompleted;
		m_dSendMissionRequest = NKCPacketSender.Send_NKMPacket_EVENT_PASS_MISSION_REQ;
		bool flag = true;
		if (m_bMissionTabClick)
		{
			m_bMissionTabClick = false;
			if (missionType == EventPassMissionType.Daily && IsDailyMissionCompleted(m_listMissionInfo[missionType]))
			{
				OnClickWeeklyMission(bAnim: false);
				flag = false;
			}
		}
		if (missionType == EventPassMissionType.Daily && m_currentMissionType == EventPassMissionType.Weekly && m_objMissionListPanel.activeSelf && (m_listMissionInfo[EventPassMissionType.Weekly] == null || NKCSynchronizedTime.IsFinished(m_resetMissionTime[EventPassMissionType.Weekly])))
		{
			m_dSendMissionRequest = null;
			NKCPacketSender.Send_NKMPacket_EVENT_PASS_MISSION_REQ(EventPassMissionType.Weekly);
			flag = false;
		}
		if (IsInstanceOpen && flag)
		{
			RefreshScrollRect();
		}
	}

	public void RefreshPurchaseCorePass(bool corePassPurchased, int totalExp = -1)
	{
		SetCorePassPurchaseState(corePassPurchased);
		NKMEventPassTemplet nKMEventPassTemplet = NKMTempletContainer<NKMEventPassTemplet>.Find(m_iEventPassId);
		if (totalExp >= 0 && nKMEventPassTemplet != null)
		{
			SetPassLevelExp(totalExp, nKMEventPassTemplet.PassLevelUpExp, nKMEventPassTemplet.PassMaxLevel, ExpFXType.DIRECT);
		}
		if (m_objRewardListPanel.activeSelf)
		{
			RefreshScrollRectCounterPassReward(initScrollPosition: true);
		}
		else
		{
			UpdateRewardRedDot();
			SetMissionRedDot(m_bDailyMissionDot, m_bWeeklyMissionDot);
		}
		NKCPopupEventPassUnlock.Instance.Open();
	}

	public void RefreshFinalMissionCompleted(NKMPacket_EVENT_PASS_FINAL_MISSION_COMPLETE_ACK cPacket)
	{
		m_finalMissionCompleted[cPacket.missionType] = true;
		int num = cPacket.totalExp - m_NKCEventPassDataManager.TotalExp;
		RefreshPassTotalExpRelatedInfo(cPacket.totalExp, initScrollPosition: true, ExpFXType.TRANS);
		if (NKCScenManager.GetScenManager().GetMyUserData() != null)
		{
			m_tweenerExpGauge?.Pause();
			NKMAdditionalReward nKMAdditionalReward = new NKMAdditionalReward();
			nKMAdditionalReward.eventPassExpDelta = num;
			NKCPopupMessageToastSimple.Instance.Open(new NKMRewardData(), nKMAdditionalReward, delegate
			{
				PlayExpDoTween();
			});
		}
	}

	public void RefreshPassTotalExpRelatedInfo(int totalExp, bool initScrollPosition = true, ExpFXType fxType = ExpFXType.DIRECT)
	{
		if (totalExp >= 0)
		{
			NKMEventPassTemplet nKMEventPassTemplet = NKMTempletContainer<NKMEventPassTemplet>.Find(m_iEventPassId);
			if (nKMEventPassTemplet != null)
			{
				SetPassLevelExp(totalExp, nKMEventPassTemplet.PassLevelUpExp, nKMEventPassTemplet.PassMaxLevel, fxType);
			}
		}
		RefreshScrollRect(initScrollPosition);
	}

	public void RefreshPassAdditionalExpRelatedInfo(long addExp)
	{
		if (addExp > 0)
		{
			NKMEventPassTemplet nKMEventPassTemplet = NKMTempletContainer<NKMEventPassTemplet>.Find(m_iEventPassId);
			if (nKMEventPassTemplet != null)
			{
				SetPassLevelExp(m_NKCEventPassDataManager.TotalExp + (int)addExp, nKMEventPassTemplet.PassLevelUpExp, nKMEventPassTemplet.PassMaxLevel, ExpFXType.DIRECT);
			}
		}
		m_tweenerExpGauge?.Pause();
		RefreshScrollRect();
	}

	public void RefreshSelectedDailyMissionSlot(NKMEventPassMissionInfo missionInfo)
	{
		int num = missionInfo.slotIndex - 1;
		if (m_listMissionInfo[m_currentMissionType].Count > num)
		{
			m_listMissionInfo[m_currentMissionType][num] = missionInfo;
		}
		RefreshScrollRect(initScrollPosition: false);
	}

	public static void RefreshMissionState(HashSet<NKMMissionData> missionUpdateList)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		bool flag = false;
		bool flag2 = false;
		foreach (NKMMissionData missionUpdate in missionUpdateList)
		{
			NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(missionUpdate.tabId);
			if (missionTabTemplet == null || missionTabTemplet.m_MissionType != NKM_MISSION_TYPE.EVENT_PASS)
			{
				continue;
			}
			NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(missionUpdate.mission_id);
			if (missionTemplet == null)
			{
				continue;
			}
			if (NKMMissionManager.CanComplete(missionTemplet, myUserData, missionUpdate) == NKM_ERROR_CODE.NEC_OK)
			{
				if (missionTemplet.m_ResetInterval == NKM_MISSION_RESET_INTERVAL.DAILY)
				{
					flag = true;
				}
				if (missionTemplet.m_ResetInterval == NKM_MISSION_RESET_INTERVAL.WEEKLY)
				{
					flag2 = true;
				}
			}
			if (flag && flag2)
			{
				break;
			}
		}
		if (m_dOnMissionUpdate != null)
		{
			m_dOnMissionUpdate();
		}
		if (flag)
		{
			DailyMissionRedDot = true;
		}
		if (flag2)
		{
			WeeklyMissionRedDot = true;
		}
		NKCEventPassDataManager eventPassDataManager = NKCScenManager.GetScenManager().GetEventPassDataManager();
		if (eventPassDataManager != null)
		{
			int passLevel = eventPassDataManager.GetPassLevel();
			NKMEventPassTemplet nKMEventPassTemplet = NKMEventPassTemplet.Find(eventPassDataManager.EventPassId);
			if (nKMEventPassTemplet != null && passLevel >= nKMEventPassTemplet.PassMaxLevel)
			{
				DailyMissionRedDot = false;
				WeeklyMissionRedDot = false;
			}
		}
		if (IsInstanceOpen)
		{
			Instance.RefreshScrollRect(initScrollPosition: false);
		}
	}

	public void SetRewardRedDot(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objRewardRedDot, value);
	}

	public void SetDailyMissionRedDot(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objDailyMissionRedDot, value);
	}

	public void SetWeeklyMissionRedDot(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objWeeklyMissionRedDot, value);
	}

	public void SetPassMissionCompleteText(EventPassMissionType missionType)
	{
		switch (missionType)
		{
		case EventPassMissionType.Daily:
			NKCUtil.SetLabelText(m_lbPassMissionCompleteText, NKCUtilString.GET_STRING_EVENTPASS_MISSION_COMPLETE_DAILY_ALL);
			NKCUtil.SetLabelText(m_lbPassMissionCompleteExText, NKCUtilString.GET_STRING_EVENTPASS_MISSION_COMPLETE_DAILY_ALL_EX);
			break;
		case EventPassMissionType.Weekly:
			NKCUtil.SetLabelText(m_lbPassMissionCompleteText, NKCUtilString.GET_STRING_EVENTPASS_MISSION_COMPLETE_WEEKLY_ALL);
			NKCUtil.SetLabelText(m_lbPassMissionCompleteExText, NKCUtilString.GET_STRING_EVENTPASS_MISSION_COMPLETE_WEEKLY_ALL_EX);
			break;
		}
	}

	public static void SetMaxLevelMainRewardImage(NKMEventPassTemplet eventPassTemplet, NKCUICharacterView characterView, NKCUIComEventPassEquip eventPassEquip, GameObject equipRoot)
	{
		if (eventPassTemplet != null)
		{
			switch (eventPassTemplet.EventPassMainRewardType)
			{
			case NKM_REWARD_TYPE.RT_UNIT:
			case NKM_REWARD_TYPE.RT_SHIP:
			case NKM_REWARD_TYPE.RT_OPERATOR:
				NKCUtil.SetGameobjectActive(characterView, bValue: true);
				NKCUtil.SetGameobjectActive(equipRoot, bValue: false);
				characterView.SetCharacterIllust(eventPassTemplet.EventPassMainReward);
				break;
			case NKM_REWARD_TYPE.RT_SKIN:
			{
				NKCUtil.SetGameobjectActive(characterView, bValue: true);
				NKCUtil.SetGameobjectActive(equipRoot, bValue: false);
				NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(eventPassTemplet.EventPassMainReward);
				characterView.SetCharacterIllust(skinTemplet);
				break;
			}
			case NKM_REWARD_TYPE.RT_MISC:
			case NKM_REWARD_TYPE.RT_EQUIP:
			case NKM_REWARD_TYPE.RT_MOLD:
				NKCUtil.SetGameobjectActive(characterView, bValue: false);
				NKCUtil.SetGameobjectActive(equipRoot, bValue: true);
				eventPassEquip.SetData(eventPassTemplet);
				break;
			case NKM_REWARD_TYPE.RT_USER_EXP:
			case NKM_REWARD_TYPE.RT_BUFF:
			case NKM_REWARD_TYPE.RT_EMOTICON:
			case NKM_REWARD_TYPE.RT_MISSION_POINT:
			case NKM_REWARD_TYPE.RT_BINGO_TILE:
			case NKM_REWARD_TYPE.RT_PASS_EXP:
				break;
			}
		}
	}

	public bool IsExpOverflowed(NKMEventPassTemplet eventPassTemplet, int addExp)
	{
		return m_NKCEventPassDataManager.TotalExp + addExp > eventPassTemplet.PassMaxExp;
	}

	public void PlayExpDoTween()
	{
		m_tweenerExpGauge?.Play();
	}

	public static bool IsEventTime(bool activateAlarm = true)
	{
		NKCEventPassDataManager eventPassDataManager = NKCScenManager.GetScenManager().GetEventPassDataManager();
		if (eventPassDataManager == null)
		{
			return false;
		}
		NKMEventPassTemplet nKMEventPassTemplet = NKMTempletContainer<NKMEventPassTemplet>.Find(eventPassDataManager.EventPassId);
		if (nKMEventPassTemplet == null)
		{
			return false;
		}
		if (NKMContentsVersionManager.HasDFChangeTagType(DataFormatChangeTagType.OPEN_TAG_EVENTPASS) && !nKMEventPassTemplet.EnableByTag)
		{
			return false;
		}
		DateTime startTimeUTC = NKCSynchronizedTime.ToUtcTime(nKMEventPassTemplet.EventPassStartDate);
		DateTime finishTimeUTC = NKCSynchronizedTime.ToUtcTime(nKMEventPassTemplet.EventPassEndDate);
		bool num = NKCSynchronizedTime.IsEventTime(startTimeUTC, finishTimeUTC);
		if (!num && activateAlarm)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_EVENTPASS_END, CheckInstanceAndClose);
		}
		if (!num)
		{
			eventPassDataManager.EventPassDataReceived = false;
		}
		return num;
	}

	public static DateTime GetMissionResetTime(EventPassMissionType missionType)
	{
		DateTime serverUTCTime = NKCSynchronizedTime.GetServerUTCTime();
		return missionType switch
		{
			EventPassMissionType.Daily => NKMTime.GetNextResetTime(serverUTCTime, NKMTime.TimePeriod.Day), 
			EventPassMissionType.Weekly => NKMTime.GetNextResetTime(serverUTCTime, NKMTime.TimePeriod.Week), 
			_ => default(DateTime), 
		};
	}

	private void Update()
	{
		if (m_bRefreshScrollRectStandby)
		{
			RefreshScrollRect(initScrollPosition: false);
		}
		m_fUpdateTime -= Time.deltaTime;
		if (!(m_fUpdateTime < 0f))
		{
			return;
		}
		if (m_objMissionListPanel.activeSelf)
		{
			UpdateMissionResetTime(m_resetMissionTime[m_currentMissionType]);
		}
		if (m_objMissionResetting.activeSelf)
		{
			OnOffMissionResettingPanel(m_currentMissionType);
		}
		if (m_dSendMissionRequest != null)
		{
			if (!IsEventTime(activateAlarm: false))
			{
				m_dSendMissionRequest = null;
				return;
			}
			if (NKCSynchronizedTime.IsFinished(m_resetMissionTime[EventPassMissionType.Daily]))
			{
				m_dSendMissionRequest(EventPassMissionType.Daily);
				m_dSendMissionRequest = null;
				if (m_dOnMissionUpdate != null)
				{
					m_dOnMissionUpdate();
				}
			}
		}
		m_fUpdateTime = 1f;
	}

	private void SetCorePassPurchaseState(bool corePassPurchased)
	{
		m_csbtnPurchaseCorePass.SetLock(corePassPurchased);
	}

	private void SetPassLevelExp(int totalExp, int passLevelUpExp, int passMaxLevel, ExpFXType fxType)
	{
		int totalExp2 = m_NKCEventPassDataManager.TotalExp;
		m_NKCEventPassDataManager.TotalExp = totalExp;
		int iUserPassLevel = m_iUserPassLevel;
		m_iUserPassLevel = Mathf.Min(passMaxLevel, totalExp / passLevelUpExp + 1);
		int currentExp = totalExp % passLevelUpExp;
		string expTextColor = ColorUtility.ToHtmlStringRGB(m_passExpColor);
		int num = Mathf.Min(totalExp, (passMaxLevel - 1) * passLevelUpExp);
		int startExp = Mathf.Max(0, ((num - totalExp2) / passLevelUpExp < 10) ? totalExp2 : (num - passLevelUpExp * 10));
		float duration = ((float)Mathf.Max(0, num - startExp) / (float)passLevelUpExp + 1f) * m_fExpGaugeTimeMultiplier;
		if (m_tweenerExpGauge != null && m_tweenerExpGauge.IsActive())
		{
			m_tweenerExpGauge.Kill(complete: true);
		}
		m_tweenerExpGauge = DOTween.To(() => startExp, delegate(int x)
		{
			int num2 = Mathf.Min(passMaxLevel, x / passLevelUpExp + 1);
			int num3 = x % passLevelUpExp;
			if (num2 >= passMaxLevel)
			{
				num3 = passLevelUpExp;
			}
			NKCUtil.SetLabelText(m_lbPassExp, string.Format(NKCUtilString.GET_STRING_EVENTPASS_EXP, m_iExpFontSize, expTextColor, num3, passLevelUpExp));
			NKCUtil.SetLabelText(m_lbPassLevel, num2.ToString());
			NKCUtil.SetImageFillAmount(m_imgPassExpGauge, (float)num3 / (float)passLevelUpExp);
		}, num, duration).SetEase(Ease.OutCubic).OnComplete(delegate
		{
			CheckPassLevelAchievedMax(currentExp, passLevelUpExp, passMaxLevel);
		});
		NKCUtil.SetGameobjectActive(m_csbtnPassLevelUp, m_iUserPassLevel < passMaxLevel);
		ExpGainType gainType = ExpGainType.Gain;
		if (iUserPassLevel < m_iUserPassLevel)
		{
			gainType = ExpGainType.LevelUp;
		}
		else if (totalExp2 < totalExp)
		{
			gainType = ExpGainType.Gain;
		}
		TriggerExpFx(gainType, fxType);
	}

	private void CheckPassLevelAchievedMax(int currentExp, int passLevelUpExp, int passMaxLevel)
	{
		if (m_iUserPassLevel >= passMaxLevel)
		{
			currentExp = passLevelUpExp;
			NKCUtil.SetGameobjectActive(m_objExpRoot, bValue: false);
			NKCUtil.SetGameobjectActive(m_objExpMaxLevel, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objExpRoot, bValue: true);
			NKCUtil.SetGameobjectActive(m_objExpMaxLevel, bValue: false);
		}
		string text = ColorUtility.ToHtmlStringRGB(m_passExpColor);
		NKCUtil.SetLabelText(m_lbPassExp, string.Format(NKCUtilString.GET_STRING_EVENTPASS_EXP, m_iExpFontSize, text, currentExp, passLevelUpExp));
		NKCUtil.SetLabelText(m_lbPassLevel, m_iUserPassLevel.ToString());
		NKCUtil.SetImageFillAmount(m_imgPassExpGauge, (float)currentExp / (float)passLevelUpExp);
	}

	private void SetMaxLevelRewardFloatingUI(NKMEventPassTemplet eventPassTemplet)
	{
		NKMEventPassRewardTemplet rewardTemplet = NKMEventPassRewardTemplet.GetRewardTemplet(eventPassTemplet.PassRewardGroupId, eventPassTemplet.PassMaxLevel);
		if (rewardTemplet != null)
		{
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(rewardTemplet.NormalRewardItemType, rewardTemplet.NormalRewardItemId, rewardTemplet.NormalRewardItemCount);
			m_finalNormalRewardSlot?.SetData(data);
			NKCUISlot.SlotData data2 = NKCUISlot.SlotData.MakeRewardTypeData(rewardTemplet.CoreRewardItemType, rewardTemplet.CoreRewardItemId, rewardTemplet.CoreRewardItemCount);
			m_finalCoreRewardSlot?.SetData(data2, bEnableLayoutElement: true, OnClickSlot);
			string arg = ColorUtility.ToHtmlStringRGB(m_colFinalRewardFontColor);
			NKCUtil.SetLabelText(m_lbFinalRewardLevel, string.Format(NKCUtilString.GET_STRING_EVENTPASS_MAX_PASS_LEVEL_FINAL_REWARD, m_iFinalRewardFontSize, arg, eventPassTemplet.PassMaxLevel));
		}
	}

	private void SetEndNotice(NKMEventPassTemplet eventPassTemplet)
	{
		if (eventPassTemplet == null)
		{
			return;
		}
		TimeSpan timeLeft = NKCSynchronizedTime.GetTimeLeft(NKCSynchronizedTime.ToUtcTime(eventPassTemplet.EventPassEndDate));
		string empty = string.Empty;
		empty = ((timeLeft.Days > 0) ? string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_DAYS"), timeLeft.Days) : ((timeLeft.Hours > 0) ? string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_HOURS"), timeLeft.Hours) : ((timeLeft.Minutes <= 0) ? string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_MINUTES"), 1) : string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_MINUTES"), timeLeft.Minutes))));
		if (timeLeft.Days <= m_iEndWarningRemainDays)
		{
			NKCUtil.SetGameobjectActive(m_objEndNotice, bValue: true);
			string arg = ColorUtility.ToHtmlStringRGB(m_colEndTimeText);
			if (timeLeft.Days > 0)
			{
				NKCUtil.SetLabelText(m_lbEndTimeRemain, string.Format(NKCUtilString.GET_STRING_EVENTPASS_END_TIME_REMAIN, arg, empty));
			}
			else
			{
				NKCUtil.SetLabelText(m_lbEndTimeRemain, string.Format(NKCUtilString.GET_STRING_EVENTPASS_END_TIME_ALMOST_END, arg, empty));
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objEndNotice, bValue: false);
		}
	}

	private void TriggerExpFx(ExpGainType gainType, ExpFXType fxType)
	{
		switch (fxType)
		{
		case ExpFXType.DIRECT:
			switch (gainType)
			{
			case ExpGainType.LevelUp:
				m_aniEventPass.ResetTrigger("DIRECT_LEVEL_UP");
				m_aniEventPass.SetTrigger("DIRECT_LEVEL_UP");
				break;
			case ExpGainType.Gain:
				m_aniEventPass.ResetTrigger("DIRECT_GAIN");
				m_aniEventPass.SetTrigger("DIRECT_GAIN");
				break;
			}
			break;
		case ExpFXType.TRANS:
			switch (gainType)
			{
			case ExpGainType.LevelUp:
				m_aniEventPass.ResetTrigger("TRANS_LEVEL_UP");
				m_aniEventPass.SetTrigger("TRANS_LEVEL_UP");
				break;
			case ExpGainType.Gain:
				m_aniEventPass.ResetTrigger("TRANS_GAIN");
				m_aniEventPass.SetTrigger("TRANS_GAIN");
				break;
			}
			break;
		}
	}

	private void FindMissionTemplet(EventPassMissionType missionType)
	{
		if (m_listMissionInfo[missionType] != null)
		{
			m_missionTempletInfo[missionType].Clear();
			int count = m_listMissionInfo[missionType].Count;
			for (int i = 0; i < count; i++)
			{
				NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(m_listMissionInfo[missionType][i].missionId);
				m_missionTempletInfo[missionType].Add(missionTemplet);
			}
			m_missionTempletInfo[missionType].Sort(Comparer);
		}
	}

	private int Comparer(NKMMissionTemplet x, NKMMissionTemplet y)
	{
		NKMMissionManager.MissionStateData missionStateData = NKMMissionManager.GetMissionStateData(x);
		NKMMissionManager.MissionStateData missionStateData2 = NKMMissionManager.GetMissionStateData(y);
		if (missionStateData.state != missionStateData2.state)
		{
			return missionStateData.state.CompareTo(missionStateData2.state);
		}
		return 0;
	}

	private void RefreshScrollRect(bool initScrollPosition = true)
	{
		if (!base.gameObject.activeSelf)
		{
			m_bRefreshScrollRectStandby = true;
			return;
		}
		m_bRefreshScrollRectStandby = false;
		if (m_objRewardListPanel.activeSelf)
		{
			RefreshScrollRectCounterPassReward(initScrollPosition);
		}
		if (!m_objMissionListPanel.activeSelf)
		{
			return;
		}
		NKMEventPassTemplet nKMEventPassTemplet = NKMTempletContainer<NKMEventPassTemplet>.Find(m_iEventPassId);
		if (nKMEventPassTemplet == null)
		{
			return;
		}
		if (m_iUserPassLevel >= nKMEventPassTemplet.PassMaxLevel)
		{
			NKCUtil.SetGameobjectActive(m_objLevelFullMissionDisable, bValue: true);
			NKCUtil.SetGameobjectActive(m_objMissionListPanel, bValue: false);
			NKCUtil.SetGameobjectActive(m_objMissionResetting, bValue: false);
			SetMissionRedDot(existCompletableDailyMission: false, existCompletableWeeklyMission: false);
			UpdateRewardRedDot();
			return;
		}
		int weekSinceEventStart = nKMEventPassTemplet.GetWeekSinceEventStart(NKCSynchronizedTime.ServiceTime);
		FindMissionTemplet(m_currentMissionType);
		bool flag = m_finalMissionCompleted[m_currentMissionType];
		NKCUISlot.SlotData slotData = null;
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		m_achieveSlot.Init();
		long completedMissionCount = GetCompletedMissionCount(myUserData, EventPassMissionType.Daily);
		long completedMissionCount2 = GetCompletedMissionCount(myUserData, EventPassMissionType.Weekly);
		bool flag2 = completedMissionCount >= nKMEventPassTemplet.DailyMissionClearCount;
		bool flag3 = completedMissionCount2 >= nKMEventPassTemplet.WeeklyMissionClearCount;
		bool flag4 = flag2 && !m_finalMissionCompleted[EventPassMissionType.Daily];
		bool flag5 = flag3 && !m_finalMissionCompleted[EventPassMissionType.Weekly];
		bool flag6 = false;
		bool flag7 = false;
		bool flag8 = DailyMissionRedDot;
		bool flag9 = WeeklyMissionRedDot;
		if (m_currentMissionType == EventPassMissionType.Daily)
		{
			NKCUtil.SetGameobjectActive(m_objDailyAchieveBG, bValue: true);
			NKCUtil.SetGameobjectActive(m_objWeeklyAchieveBG, bValue: false);
			NKCUtil.SetLabelText(m_lbAchieveTitle, NKCUtilString.GET_STRING_EVENTPASS_DAILY_MISSION_ACHIEVE);
			string arg = ColorUtility.ToHtmlStringRGB(m_colDailyCountColor);
			NKCUtil.SetLabelText(m_lbWeekCount, string.Format(NKCUtilString.GET_STRING_EVENTPASS_ELAPSED_WEEK, arg, weekSinceEventStart));
			flag8 = completedMissionCount < nKMEventPassTemplet.DailyMissionClearCount && ExistCompletableMission(myUserData, m_missionTempletInfo[EventPassMissionType.Daily]);
			flag6 = flag4;
			flag7 = flag2;
			m_csbtnCompleteAllMission.SetLock(!flag8);
			m_bDailyMissionCompleteAchieved = flag2;
			NKCUtil.SetLabelText(m_lbAchieveCount, $"{Mathf.Min(completedMissionCount, nKMEventPassTemplet.DailyMissionClearCount)}/{nKMEventPassTemplet.DailyMissionClearCount}");
			NKCUtil.SetImageFillAmount(m_imgAchieveGauge, (float)completedMissionCount / (float)nKMEventPassTemplet.DailyMissionClearCount);
			slotData = NKCUISlot.SlotData.MakeRewardTypeData(NKM_REWARD_TYPE.RT_PASS_EXP, 504, nKMEventPassTemplet.DailyMissionClearRewardExp);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objWeeklyAchieveBG, bValue: true);
			NKCUtil.SetGameobjectActive(m_objDailyAchieveBG, bValue: false);
			NKCUtil.SetLabelText(m_lbAchieveTitle, NKCUtilString.GET_STRING_EVENTPASS_WEEKLY_MISSION_ACHIEVE);
			string arg2 = ColorUtility.ToHtmlStringRGB(m_colWeeklyCountColor);
			NKCUtil.SetLabelText(m_lbWeekCount, string.Format(NKCUtilString.GET_STRING_EVENTPASS_ELAPSED_WEEK, arg2, weekSinceEventStart));
			flag9 = completedMissionCount2 < nKMEventPassTemplet.WeeklyMissionClearCount && ExistCompletableMission(myUserData, m_missionTempletInfo[EventPassMissionType.Weekly]);
			flag6 = flag5;
			flag7 = flag3;
			m_csbtnCompleteAllMission.SetLock(!flag9);
			NKCUtil.SetLabelText(m_lbAchieveCount, $"{completedMissionCount2}/{nKMEventPassTemplet.WeeklyMissionClearCount}");
			NKCUtil.SetImageFillAmount(m_imgAchieveGauge, (float)completedMissionCount2 / (float)nKMEventPassTemplet.WeeklyMissionClearCount);
			slotData = NKCUISlot.SlotData.MakeRewardTypeData(NKM_REWARD_TYPE.RT_PASS_EXP, 504, nKMEventPassTemplet.WeeklyMissionClearRewardExp);
		}
		if (!flag6)
		{
			if (slotData != null)
			{
				m_achieveSlot.SetData(slotData);
			}
			NKCUtil.SetGameobjectActive(m_objAchieveSlotEffect, bValue: false);
		}
		else
		{
			if (slotData != null)
			{
				m_achieveSlot.SetData(slotData, bEnableLayoutElement: true, OnClickCompleteFinalMission);
			}
			NKCUtil.SetGameobjectActive(m_objAchieveSlotEffect, bValue: true);
		}
		NKCUtil.SetGameobjectActive(m_objPassMissionComplete, flag7);
		SetPassMissionCompleteText(m_currentMissionType);
		UpdateMissionResetTime(m_resetMissionTime[m_currentMissionType]);
		m_achieveSlot.SetDisable(flag);
		m_achieveSlot.SetCompleteMark(flag);
		SetMissionRedDot(flag8 || flag4, flag9 || flag5);
		UpdateRewardRedDot();
		OnOffMissionResettingPanel(m_currentMissionType);
		if (m_missionTempletInfo[m_currentMissionType] != null)
		{
			m_LoopMissionScrollRect.TotalCount = m_missionTempletInfo[m_currentMissionType].Count;
			m_LoopMissionScrollRect.StopMovement();
			if (initScrollPosition)
			{
				m_LoopMissionScrollRect.SetIndexPosition(0);
			}
			else
			{
				m_LoopMissionScrollRect.RefreshCells();
			}
		}
	}

	private void RefreshScrollRectCounterPassReward(bool initScrollPosition)
	{
		NKCUtil.SetGameobjectActive(m_objLevelFullMissionDisable, bValue: false);
		NKMEventPassTemplet nKMEventPassTemplet = NKMTempletContainer<NKMEventPassTemplet>.Find(m_iEventPassId);
		if (nKMEventPassTemplet == null)
		{
			return;
		}
		List<NKMEventPassRewardTemplet> rewardGroupTemplet = NKMEventPassRewardTemplet.GetRewardGroupTemplet(nKMEventPassTemplet.PassRewardGroupId);
		if (rewardGroupTemplet != null)
		{
			m_LoopRewardScrollRect.TotalCount = rewardGroupTemplet.Count;
			m_LoopRewardScrollRect.StopMovement();
			if (initScrollPosition)
			{
				int value = Mathf.Max(m_NKCEventPassDataManager.NormalRewardLevel - 1, m_NKCEventPassDataManager.CoreRewardLevel - 1, m_iUserPassLevel - 1);
				value = Mathf.Clamp(value, 0, rewardGroupTemplet.Count - 1);
				m_LoopRewardScrollRect.SetIndexPosition(value);
			}
			else
			{
				m_LoopRewardScrollRect.RefreshCells();
			}
			UpdateRewardRedDot();
			SetMissionRedDot(m_bDailyMissionDot, m_bWeeklyMissionDot);
		}
	}

	private void UpdateRewardRedDot()
	{
		bool num = m_NKCEventPassDataManager.NormalRewardLevel < m_iUserPassLevel;
		bool flag = m_NKCEventPassDataManager.CorePassPurchased && m_NKCEventPassDataManager.CoreRewardLevel < m_iUserPassLevel;
		RewardRedDot = num || flag;
	}

	private void SetMissionRedDot(bool existCompletableDailyMission, bool existCompletableWeeklyMission)
	{
		NKMEventPassTemplet nKMEventPassTemplet = NKMEventPassTemplet.Find(m_iEventPassId);
		if (nKMEventPassTemplet != null && m_iUserPassLevel >= nKMEventPassTemplet.PassMaxLevel)
		{
			existCompletableDailyMission = false;
			existCompletableWeeklyMission = false;
		}
		NKCUtil.SetGameobjectActive(m_objMissionRedDot, existCompletableDailyMission || existCompletableWeeklyMission);
		DailyMissionRedDot = existCompletableDailyMission;
		WeeklyMissionRedDot = existCompletableWeeklyMission;
	}

	private long GetCompletedMissionCount(NKMUserData cNKMUserData, EventPassMissionType missionType)
	{
		int num = 0;
		if (m_listMissionInfo.ContainsKey(missionType) && m_listMissionInfo[missionType] != null)
		{
			int count = m_listMissionInfo[missionType].Count;
			for (int i = 0; i < count; i++)
			{
				int missionId = m_listMissionInfo[missionType][i].missionId;
				NKMMissionData missionDataByMissionId = cNKMUserData.m_MissionData.GetMissionDataByMissionId(missionId);
				if (missionDataByMissionId != null && missionDataByMissionId.IsComplete)
				{
					num++;
				}
			}
		}
		return num;
	}

	private bool ExistCompletableMission(NKMUserData cNKMUserData, List<NKMMissionTemplet> listMissionTemplet)
	{
		if (listMissionTemplet == null)
		{
			return false;
		}
		int count = listMissionTemplet.Count;
		for (int i = 0; i < count; i++)
		{
			NKMMissionTemplet nKMMissionTemplet = listMissionTemplet[i];
			NKMMissionData missionData = NKMMissionManager.GetMissionData(nKMMissionTemplet);
			if (missionData != null && !missionData.IsComplete && NKMMissionManager.CanComplete(nKMMissionTemplet, cNKMUserData, missionData) == NKM_ERROR_CODE.NEC_OK)
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckMissionRetryEnable(EventPassMissionType missionType, int destMissionID)
	{
		NKMEventPassTemplet nKMEventPassTemplet = NKMTempletContainer<NKMEventPassTemplet>.Find(m_iEventPassId);
		if (nKMEventPassTemplet == null)
		{
			return false;
		}
		List<NKMEventPassMissionGroupTemplet> list = null;
		list = ((missionType != EventPassMissionType.Daily) ? NKMEventPassMissionGroupTemplet.GetMissionGroupList(missionType, nKMEventPassTemplet.WeeklyMissionGroupId, nKMEventPassTemplet.GetWeekSinceEventStart(NKCSynchronizedTime.ServiceTime)) : NKMEventPassMissionGroupTemplet.GetMissionGroupList(missionType, nKMEventPassTemplet.DailyMissionGroupId, nKMEventPassTemplet.GetWeekSinceEventStart(NKCSynchronizedTime.ServiceTime)));
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			if (list[i].MissionIds.Find((int e) => e == destMissionID) > 0)
			{
				return list[i].IsRetryEnable;
			}
		}
		return false;
	}

	private void UpdateMissionResetTime(DateTime resetTime)
	{
		string remainTimeStringEx = NKCUtilString.GetRemainTimeStringEx(resetTime);
		string remainTimeString = NKCUtilString.GetRemainTimeString(resetTime, 2);
		NKCUtil.SetLabelText(m_lbAchieveTimeRemain, remainTimeStringEx);
		NKCUtil.SetLabelText(m_lbRefreshTimeRemain, string.Format(NKCUtilString.GET_STRING_EVENTPASS_UPDATE_TIME_LEFT, remainTimeString));
	}

	private void OnOffMissionResettingPanel(EventPassMissionType missionType)
	{
		DateTime dateTime = m_resetMissionTime[missionType];
		string msg = string.Empty;
		switch (missionType)
		{
		case EventPassMissionType.Daily:
			dateTime = dateTime.AddDays(-1.0);
			msg = NKCUtilString.GET_STRING_EVENTPASS_MISSION_UPDATING_DAILY;
			break;
		case EventPassMissionType.Weekly:
			dateTime = dateTime.AddDays(-7.0);
			msg = NKCUtilString.GET_STRING_EVENTPASS_MISSION_UPDATING_WEEKLY;
			break;
		}
		TimeSpan timeSpan = NKCSynchronizedTime.GetServerUTCTime() - dateTime;
		if (timeSpan.Ticks >= 0 && timeSpan.TotalMinutes < 5.0)
		{
			NKCUtil.SetLabelText(m_lbMissionResetMsg, msg);
			NKCUtil.SetGameobjectActive(m_objMissionResetting, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objMissionResetting, bValue: false);
		}
	}

	private bool IsEventMissionOpened(NKMEventPassTemplet eventPassTemplet)
	{
		if (eventPassTemplet == null)
		{
			return false;
		}
		bool result = true;
		if (eventPassTemplet.m_ShortCutType == NKM_SHORTCUT_TYPE.SHORTCUT_NONE)
		{
			result = false;
		}
		else if (string.IsNullOrEmpty(eventPassTemplet.m_ShortCut))
		{
			result = false;
		}
		else
		{
			string[] array = eventPassTemplet.m_ShortCut.Split('@');
			EPISODE_CATEGORY result2;
			if (array == null || array.Length <= 1)
			{
				result = false;
			}
			else if (Enum.TryParse<EPISODE_CATEGORY>(array[0], out result2))
			{
				int episodeID;
				if (!NKCContentManager.IsContentsUnlocked(NKCContentManager.GetContentsType(result2)))
				{
					result = false;
				}
				else if (int.TryParse(array[1], out episodeID))
				{
					if (NKMEpisodeMgr.GetListNKMEpisodeTempletByCategory(result2, bOnlyOpen: true).Find((NKMEpisodeTempletV2 e) => e.m_EpisodeID == episodeID) == null)
					{
						result = false;
					}
				}
				else
				{
					result = false;
				}
			}
			else
			{
				result = false;
			}
		}
		return result;
	}

	private void SetEventMissionButtonState(NKMEventPassTemplet eventPassTemplet)
	{
		if (eventPassTemplet == null)
		{
			NKCUtil.SetLabelText(m_lbEventStage, "");
		}
		else if (IsEventMissionOpened(eventPassTemplet))
		{
			NKCUtil.SetLabelText(m_lbEventStage, NKCStringTable.GetString("SI_PF_EVENT_PASS_EVENT_STAGE_SHORTCUT_BUTTON"));
		}
		else
		{
			NKCUtil.SetLabelText(m_lbEventStage, NKCStringTable.GetString("SI_LOBBY_RIGHT_MENU_2_ALBUM_TEXT"));
		}
	}

	private RectTransform GetRewardSlot(int index)
	{
		return NKCUIEventPassRewardSlot.GetNewInstance(null)?.GetComponent<RectTransform>();
	}

	private void ReturnRewardSlot(Transform tr)
	{
		NKCUIEventPassRewardSlot component = tr.GetComponent<NKCUIEventPassRewardSlot>();
		tr.SetParent(null);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			UnityEngine.Object.Destroy(tr.gameObject);
		}
	}

	private void ProvideRewardData(Transform tr, int index)
	{
		NKCUIEventPassRewardSlot component = tr.GetComponent<NKCUIEventPassRewardSlot>();
		if (component != null)
		{
			int passLevel = index + 1;
			NKMEventPassTemplet nKMEventPassTemplet = NKMTempletContainer<NKMEventPassTemplet>.Find(m_iEventPassId);
			if (nKMEventPassTemplet != null)
			{
				NKMEventPassRewardTemplet rewardTemplet = NKMEventPassRewardTemplet.GetRewardTemplet(nKMEventPassTemplet.PassRewardGroupId, passLevel);
				int passMaxLevel = nKMEventPassTemplet.PassMaxLevel;
				component.SetData(rewardTemplet, m_iUserPassLevel, passMaxLevel, m_NKCEventPassDataManager.CorePassPurchased, m_NKCEventPassDataManager.NormalRewardLevel, m_NKCEventPassDataManager.CoreRewardLevel, OnClickGetPassLevelReward);
			}
		}
	}

	private RectTransform GetMissionSlot(int index)
	{
		return NKCUIMissionAchieveSlot.GetNewInstance(null, "AB_UI_NKM_UI_EVENT_PASS", "NKM_UI_EVENT_PASS_MISSION_SLOT")?.GetComponent<RectTransform>();
	}

	private void ReturnMissionSlot(Transform tr)
	{
		NKCUIMissionAchieveSlot component = tr.GetComponent<NKCUIMissionAchieveSlot>();
		tr.SetParent(null);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			UnityEngine.Object.Destroy(tr.gameObject);
		}
	}

	private void ProvideMissionData(Transform tr, int index)
	{
		NKCUIMissionAchieveSlot component = tr.GetComponent<NKCUIMissionAchieveSlot>();
		if (component == null || m_missionTempletInfo[m_currentMissionType] == null || m_missionTempletInfo[m_currentMissionType].Count <= index)
		{
			return;
		}
		NKMMissionTemplet nKMMissionTemplet = m_missionTempletInfo[m_currentMissionType][index];
		if (m_currentMissionType == EventPassMissionType.Daily)
		{
			component.SetData(nKMMissionTemplet, OnClickMissionMove, OnClickMissionComplete, OnClickRefreshDailyMission);
			if (m_bDailyMissionCompleteAchieved)
			{
				component.SetForceMissionDisabled();
			}
		}
		else
		{
			component.SetData(nKMMissionTemplet, OnClickMissionMove, OnClickMissionComplete);
		}
		if (CheckMissionRetryEnable(m_currentMissionType, nKMMissionTemplet.m_MissionID))
		{
			component.SetForceActivateEventPassRefreshButton();
		}
	}

	private int GetMissionTabId(EventPassMissionType missionType)
	{
		if (!m_missionTempletInfo.ContainsKey(missionType) || m_missionTempletInfo[missionType] == null || m_missionTempletInfo[missionType].Count <= 0)
		{
			return -1;
		}
		return m_missionTempletInfo[missionType][0].m_MissionTabId;
	}

	private void OnRewardScrollValueChanged(Vector2 value)
	{
		float num = 1f - 1f / (float)(m_LoopRewardScrollRect.TotalCount - m_LoopRewardScrollRect.content.transform.childCount);
		if (value.y > num)
		{
			m_aniFinalReward?.ResetTrigger("in");
			m_aniFinalReward?.SetTrigger("out");
		}
		else
		{
			m_aniFinalReward?.ResetTrigger("out");
			m_aniFinalReward?.SetTrigger("in");
		}
	}

	private bool IsDailyMissionCompleted(List<NKMEventPassMissionInfo> dailyMissionInfoList)
	{
		if (dailyMissionInfoList == null)
		{
			return false;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return false;
		}
		NKMEventPassTemplet nKMEventPassTemplet = NKMEventPassTemplet.Find(m_iEventPassId);
		if (nKMEventPassTemplet == null)
		{
			return false;
		}
		if (GetCompletedMissionCount(myUserData, EventPassMissionType.Daily) >= nKMEventPassTemplet.DailyMissionClearCount)
		{
			return m_finalMissionCompleted[EventPassMissionType.Daily];
		}
		return false;
	}

	private void Release()
	{
		m_NKCEventPassDataManager = null;
		if (m_listMissionInfo != null)
		{
			if (m_listMissionInfo.ContainsKey(EventPassMissionType.Daily) && m_listMissionInfo[EventPassMissionType.Daily] != null)
			{
				m_listMissionInfo[EventPassMissionType.Daily].Clear();
				m_listMissionInfo[EventPassMissionType.Daily] = null;
			}
			if (m_listMissionInfo.ContainsKey(EventPassMissionType.Weekly) && m_listMissionInfo[EventPassMissionType.Weekly] != null)
			{
				m_listMissionInfo[EventPassMissionType.Weekly].Clear();
				m_listMissionInfo[EventPassMissionType.Weekly] = null;
			}
			m_listMissionInfo.Clear();
		}
		if (m_missionTempletInfo != null)
		{
			foreach (KeyValuePair<EventPassMissionType, List<NKMMissionTemplet>> item in m_missionTempletInfo)
			{
				if (m_missionTempletInfo.Values != null)
				{
					item.Value.Clear();
				}
			}
			m_missionTempletInfo.Clear();
		}
		if (m_tweenerExpGauge != null)
		{
			m_tweenerExpGauge.Kill();
			m_tweenerExpGauge = null;
		}
	}

	private static void CleanUpInstance()
	{
		m_Instance.Release();
		m_Instance = null;
	}

	private int GetTempletDataCount<T>(IEnumerable<T> container)
	{
		int num = 0;
		foreach (T item in container)
		{
			_ = item;
			num++;
		}
		return num;
	}

	private void OnClickMissionMove(NKCUIMissionAchieveSlot cNKCUIMissionAchieveSlot)
	{
		if (IsEventTime() && !(cNKCUIMissionAchieveSlot == null))
		{
			NKMMissionTemplet nKMMissionTemplet = cNKCUIMissionAchieveSlot.GetNKMMissionTemplet();
			if (nKMMissionTemplet != null)
			{
				NKCContentManager.MoveToShortCut(nKMMissionTemplet.m_ShortCutType, nKMMissionTemplet.m_ShortCut);
			}
		}
	}

	private void OnClickMissionComplete(NKCUIMissionAchieveSlot cNKCUIMissionAchieveSlot)
	{
		if (IsEventTime() && !(cNKCUIMissionAchieveSlot == null) && !m_objMissionResetting.activeSelf)
		{
			NKMMissionTemplet nKMMissionTemplet = cNKCUIMissionAchieveSlot.GetNKMMissionTemplet();
			if (nKMMissionTemplet != null)
			{
				NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_REQ(nKMMissionTemplet);
			}
		}
	}

	private void OnClickMissionCompleteAll()
	{
		if (IsEventTime() && !m_objMissionResetting.activeSelf)
		{
			NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_ALL_REQ(GetMissionTabId(m_currentMissionType));
		}
	}

	private void OnClickRefreshDailyMission(NKCUIMissionAchieveSlot cNKCUIMissionAchieveSlot)
	{
		if (!IsEventTime() || m_objMissionResetting.activeSelf)
		{
			return;
		}
		NKMMissionTemplet missionTemplet = cNKCUIMissionAchieveSlot.GetNKMMissionTemplet();
		if (missionTemplet == null)
		{
			return;
		}
		NKMEventPassMissionInfo nKMEventPassMissionInfo = m_listMissionInfo[m_currentMissionType].Find((NKMEventPassMissionInfo e) => e.missionId == missionTemplet.m_MissionID);
		if (nKMEventPassMissionInfo == null)
		{
			return;
		}
		int totalMissionRerollCount = NKMEventPassConst.TotalMissionRerollCount;
		StringBuilder stringBuilder = new StringBuilder();
		if (nKMEventPassMissionInfo.retryCount < totalMissionRerollCount)
		{
			stringBuilder.Append(NKCUtilString.GET_STRING_EVENTPASS_MISSION_REFRESH_DESC);
			if (NKMMissionManager.GetMissionStateData(missionTemplet).progressCount > 0)
			{
				stringBuilder.Append("\n");
				stringBuilder.Append(NKCUtilString.GET_STRING_EVENTPASS_MISSION_REFRESH_WARNING_DESC);
			}
			if (nKMEventPassMissionInfo.retryCount < NKMEventPassConst.FreeMissionRerollCount)
			{
				stringBuilder.Append("\n");
				stringBuilder.AppendFormat(NKCUtilString.GET_STRING_EVENTPASS_MISSION_REFRESH_FREECOUNT, nKMEventPassMissionInfo.retryCount, NKMEventPassConst.FreeMissionRerollCount);
				NKCPopupResourceConfirmBox.Instance.OpenForConfirm(NKCUtilString.GET_STRING_EVENTPASS_MISSION_REFRESH, stringBuilder.ToString(), delegate
				{
					NKCPacketSender.Send_cNKMPacket_EVENT_PASS_DAILY_MISSION_RETRY_REQ(missionTemplet.m_MissionID);
				});
			}
			else
			{
				stringBuilder.Append("\n");
				stringBuilder.AppendFormat(NKCUtilString.GET_STRING_EVENTPASS_MISSION_REFRESH_COUNT, nKMEventPassMissionInfo.retryCount - NKMEventPassConst.FreeMissionRerollCount, NKMEventPassConst.PayMissionRerollCount);
				NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_EVENTPASS_MISSION_REFRESH, stringBuilder.ToString(), 1, 20000, delegate
				{
					NKCPacketSender.Send_cNKMPacket_EVENT_PASS_DAILY_MISSION_RETRY_REQ(missionTemplet.m_MissionID);
				}, null, showResource: true);
			}
		}
		else
		{
			stringBuilder.Append(NKCUtilString.GET_STRING_EVENTPASS_MISSION_REFRESH_MAX_DESC);
			stringBuilder.Append("\n");
			stringBuilder.AppendFormat(NKCUtilString.GET_STRING_EVENTPASS_MISSION_REFRESH_COUNT, nKMEventPassMissionInfo.retryCount, totalMissionRerollCount);
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_EVENTPASS_MISSION_REFRESH, stringBuilder.ToString());
		}
	}

	private void OnClickGetPassLevelReward()
	{
		if (IsEventTime())
		{
			NKCPacketSender.Send_NKMPacket_EVENT_PASS_LEVEL_COMPLETE_REQ();
		}
	}

	private void OnClickPassLevelUp()
	{
		if (!IsEventTime())
		{
			return;
		}
		NKMEventPassTemplet nKMEventPassTemplet = NKMTempletContainer<NKMEventPassTemplet>.Find(m_iEventPassId);
		if (nKMEventPassTemplet == null)
		{
			return;
		}
		string contentText = NKCUtilString.GET_STRING_EVENTPASS_PASS_LEVEL_UP_DESC.Split('\n')[0];
		NKCPopupInventoryAdd.SliderInfo sliderInfo = new NKCPopupInventoryAdd.SliderInfo
		{
			increaseCount = 1,
			maxCount = nKMEventPassTemplet.PassMaxLevel,
			currentCount = m_iUserPassLevel,
			inventoryType = NKM_INVENTORY_EXPAND_TYPE.NIET_NONE
		};
		NKCPopupInventoryAdd.Instance.Open(NKCUtilString.GET_STRING_EVENTPASS_PASS_LEVEL_UP_NOTICE, contentText, sliderInfo, nKMEventPassTemplet.PassLevelUpMiscCount, nKMEventPassTemplet.PassLevelUpMiscId, delegate(int value)
		{
			if (IsEventTime())
			{
				NKCPacketSender.Send_NKMPacket_EVENT_PASS_LEVEL_UP_REQ(value);
			}
		});
	}

	private void OnClickPurchaseCorePass()
	{
		if (IsEventTime())
		{
			NKMEventPassTemplet nKMEventPassTemplet = NKMEventPassTemplet.Find(m_iEventPassId);
			if (nKMEventPassTemplet != null)
			{
				bool endTimeNotice = NKCSynchronizedTime.GetTimeLeft(NKCSynchronizedTime.ToUtcTime(nKMEventPassTemplet.EventPassEndDate)).Days <= m_iEndWarningRemainDays;
				NKCPopupEventPassPurchase.Instance.Open(endTimeNotice, IsEventTime);
			}
		}
	}

	private void OnToggleRewardPanel(bool value)
	{
		if (value)
		{
			NKCUtil.SetGameobjectActive(m_objRewardListPanel, value);
			NKCUtil.SetGameobjectActive(m_objMissionListPanel, !value);
			if (IsEventTime())
			{
				RefreshScrollRect();
			}
		}
	}

	private void OnToggleMissionPanel(bool value)
	{
		if (value)
		{
			NKCUtil.SetGameobjectActive(m_objRewardListPanel, !value);
			NKCUtil.SetGameobjectActive(m_objMissionListPanel, value);
			if (IsEventTime())
			{
				m_bMissionTabClick = true;
				OnClickDailyMission(bAnim: false);
			}
		}
	}

	private void OnClickDailyMission(bool bAnim)
	{
		if (bAnim)
		{
			m_rtToggleBar?.DOAnchorPos(m_vToggleDailyMissionPos, m_fToggleMissionTime).SetEase(Ease.OutCubic);
		}
		else if (m_rtToggleBar != null)
		{
			m_rtToggleBar.anchoredPosition = m_vToggleDailyMissionPos;
		}
		m_currentMissionType = EventPassMissionType.Daily;
		NKCUtil.SetLabelTextColor(m_lbDailyTabText, m_colActivatedTabColor);
		NKCUtil.SetLabelTextColor(m_lbWeeklyTabText, m_colDeactivatedTabColor);
		if (!IsEventTime())
		{
			return;
		}
		if (m_listMissionInfo[m_currentMissionType] == null || NKCSynchronizedTime.IsFinished(m_resetMissionTime[m_currentMissionType]))
		{
			m_dSendMissionRequest = null;
			NKCPacketSender.Send_NKMPacket_EVENT_PASS_MISSION_REQ(EventPassMissionType.Daily);
			return;
		}
		if (m_bMissionTabClick)
		{
			m_bMissionTabClick = false;
			if (IsDailyMissionCompleted(m_listMissionInfo[EventPassMissionType.Daily]))
			{
				OnClickWeeklyMission(bAnim: false);
				return;
			}
		}
		RefreshScrollRect();
	}

	private void OnClickWeeklyMission(bool bAnim)
	{
		if (bAnim)
		{
			m_rtToggleBar?.DOAnchorPos(m_vToggleWeeklyMissionPos, m_fToggleMissionTime).SetEase(Ease.OutCubic);
		}
		else if (m_rtToggleBar != null)
		{
			m_rtToggleBar.anchoredPosition = m_vToggleWeeklyMissionPos;
		}
		m_currentMissionType = EventPassMissionType.Weekly;
		NKCUtil.SetLabelTextColor(m_lbDailyTabText, m_colDeactivatedTabColor);
		NKCUtil.SetLabelTextColor(m_lbWeeklyTabText, m_colActivatedTabColor);
		if (IsEventTime())
		{
			if (m_listMissionInfo[m_currentMissionType] == null || NKCSynchronizedTime.IsFinished(m_resetMissionTime[m_currentMissionType]))
			{
				m_dSendMissionRequest = null;
				NKCPacketSender.Send_NKMPacket_EVENT_PASS_MISSION_REQ(EventPassMissionType.Weekly);
			}
			else
			{
				RefreshScrollRect();
			}
		}
	}

	private void OnClickCompleteFinalMission(NKCUISlot.SlotData slotData, bool bLocked)
	{
		if (IsEventTime() && !m_objMissionResetting.activeSelf)
		{
			NKCPacketSender.Send_NKMPacket_EVENT_PASS_FINAL_MISSION_COMPLETE_REQ(m_currentMissionType);
		}
	}

	private void OnClickSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		NKMEventPassTemplet nKMEventPassTemplet = NKMEventPassTemplet.Find(m_iEventPassId);
		if (nKMEventPassTemplet != null)
		{
			NKM_REWARD_TYPE eventPassMainRewardType = nKMEventPassTemplet.EventPassMainRewardType;
			if (eventPassMainRewardType == NKM_REWARD_TYPE.RT_UNIT || eventPassMainRewardType == NKM_REWARD_TYPE.RT_SKIN)
			{
				OnClickEventStage();
			}
		}
	}

	private void OnClickEventStage()
	{
		NKMEventPassTemplet nKMEventPassTemplet = NKMEventPassTemplet.Find(m_iEventPassId);
		if (nKMEventPassTemplet == null)
		{
			return;
		}
		if (IsEventMissionOpened(nKMEventPassTemplet))
		{
			NKCContentManager.MoveToShortCut(nKMEventPassTemplet.m_ShortCutType, nKMEventPassTemplet.m_ShortCut);
			return;
		}
		if (nKMEventPassTemplet.EventPassMainRewardType == NKM_REWARD_TYPE.RT_SKIN)
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(nKMEventPassTemplet.EventPassMainReward);
			if (skinTemplet != null)
			{
				NKCUIShopSkinPopup.Instance.OpenForSkinInfo(skinTemplet, 0);
				return;
			}
		}
		NKMUnitData nKMUnitData = NKCUtil.MakeDummyUnit(nKMEventPassTemplet.EventPassMainReward, 100, 3);
		if (nKMUnitData != null)
		{
			NKCUICollectionUnitInfo.CheckInstanceAndOpen(nKMUnitData, null, null, NKCUICollectionUnitInfo.eCollectionState.CS_PROFILE, isGauntlet: false, NKCUIUpsideMenu.eMode.BackButtonOnly);
		}
	}
}
