using System;
using ClientPacket.Event;
using NKC.UI.Component;
using NKM;
using NKM.Templet;
using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCPopupEventRaceV2 : NKCUIBase, IScrollHandler, IEventSystemHandler
{
	private const string ASSET_BUNDLE_NAME = "UI_SINGLE_RACE";

	private const string UI_ASSET_NAME = "UI_SINGLE_RACE";

	private static NKCPopupEventRaceV2 m_Instance;

	public Animator m_Ani;

	public Image m_imgReqItem;

	public NKCComTMPUIText m_lbReqItemCount;

	public NKCUIComStateButton m_csbtnExit;

	[Header("Step1")]
	public SkeletonGraphic m_SkeletonUnitBlue;

	public SkeletonGraphic m_SkeletonUnitRed;

	[Header("Center")]
	public NKCComTMPUIText m_lbLeftTime;

	public NKCComTMPUIText m_lbPercentRed;

	public NKCComTMPUIText m_lbPercentBlue;

	public NKCComTMPUIText m_lbValueRed;

	public NKCComTMPUIText m_lbValueBlue;

	[Space]
	public GameObject m_objGaugeRed;

	public GameObject m_ObjGuessWinRed;

	public GameObject m_ObjGuessWinTagRed;

	public RectTransform m_rtGuessWinGaugeRed;

	public GameObject m_objGaugeBlue;

	public GameObject m_ObjGuessWinBlue;

	public GameObject m_ObjGuessWinTagBlue;

	public RectTransform m_rtGuessWinGaugeBlue;

	public float m_fMaxGaugeWidth = 994f;

	[Header("Bottom")]
	public NKCUIComStateButton m_csbtnMission;

	public NKCUIComStateButton m_csbtnShop;

	public NKCUIComStateButton m_csbtnHistory;

	public NKCUIComStateButton m_csbtnReplay;

	public NKCUIComStateButton m_csbtnHelp;

	public NKCUIComStateButton m_csbtnStart;

	public GameObject m_ObjStartBtnLock;

	public GameObject m_ObjStartBtnNormal;

	[Space]
	public GameObject m_objSelectTeamNone;

	public GameObject m_objSelectTeamBlue;

	public GameObject m_objSelectTeamRed;

	public GameObject m_objSelectTeamCount;

	public NKCComTMPUIText m_lbBetCount;

	public NKCComTMPUIText m_lbTodayBetCount;

	public GameObject m_objSelectTeamFXBlue;

	public GameObject m_objSelectTeamFXRed;

	public GameObject m_objMissionRedDot;

	[Header("Step2")]
	public GameObject m_objSelectTeamBlueNormal;

	public GameObject m_objSelectTeamBlueSelect;

	public GameObject m_objSelectTeamBlueLock;

	public GameObject m_objSelectTeamRedNormal;

	public GameObject m_objSelectTeamRedSelect;

	public GameObject m_objSelectTeamRedLock;

	public NKCUIComStateButton m_csbtnSelectTeamRed;

	public NKCUIComStateButton m_csbtnSelectTeamBlue;

	[Space]
	public NKCUIComStateButton m_csbtnNext;

	public NKCUIComStateButton m_csbtnCancel;

	[Header("Step3")]
	public GameObject m_objSelectTeamRed1;

	public GameObject m_objSelectTeamBlue1;

	public NKCUIComStateButton m_csbtnMin;

	public NKCUIComStateButton m_csbtnMinus;

	public NKCUIComStateButton m_csbtnMax;

	public NKCUIComStateButton m_csbtnPlus;

	public NKCComTMPUIText m_lbBetCount1;

	public NKCUIComStateButton m_csbtnCancel1;

	public NKCUIComStateButton m_csbtnConfirm;

	[Header("Setting")]
	public Color m_LoserTeamUnitColor = Color.gray;

	public float m_fBGMPlayDelayTime = 1f;

	public int m_iGaugeDivideValue = 50;

	private string m_AniTrigger_INTRO = "INTRO";

	private string m_AniTrigger_01_IDLE = "01_IDLE";

	private string m_AniTrigger_01_TO_02 = "01_TO_02";

	private string m_AniTrigger_02_TO_01 = "02_TO_01";

	private string m_AniTrigger_02_IDLE = "02_IDLE";

	private string m_AniTrigger_02_TO_03 = "02_TO_03";

	private string m_AniTrigger_03_IDLE = "03_IDLE";

	private string m_AniTrigger_03_TO_02 = "03_TO_02";

	private string m_AniTrigger_01_TO_03 = "01_TO_03";

	private string m_AniTrigger_03_TO_01 = "03_TO_01";

	[Header("Music")]
	public string m_strBGMName;

	public NKCPopupEventRaceV2History m_RaceHistory;

	private bool m_bWaitOpenUI;

	private bool m_bCheckIntroAniComplete;

	private bool m_bIncreaseWinnerGauge;

	private RectTransform m_rtTargetGauge;

	private float m_fBaseWidth;

	private float m_fGoalWidth;

	private float m_fIncreaseValue;

	private NKMEventRaceTemplet m_curEventRaceTemplet;

	private int m_iPreBetCnt;

	private int m_iCurBetCnt;

	private int m_iMaxBetCnt;

	private bool m_bWaitPacket;

	public const float PRESS_GAP_MAX = 0.35f;

	public const float PRESS_GAP_MIN = 0.01f;

	public const float DAMPING = 0.8f;

	private float m_fDelay = 0.5f;

	private float m_fHoldTime;

	private int m_iChangeValue;

	private bool m_bPress;

	private bool m_bWasHold;

	private EventBetTeam m_CurSelectTeam;

	private EventBetTeam m_PrevSelectTeam;

	public static NKCPopupEventRaceV2 Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupEventRaceV2>("UI_SINGLE_RACE", "UI_SINGLE_RACE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupEventRaceV2>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

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

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => "";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public void Open(int iEventID)
	{
		Open(NKMEventRaceTemplet.Find(iEventID));
	}

	public void Open(NKMEventRaceTemplet raceTemplet)
	{
		if (raceTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		m_bWaitOpenUI = false;
		m_bWaitPacket = false;
		m_curEventRaceTemplet = raceTemplet;
		if (!base.gameObject.activeSelf)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		}
		m_RaceHistory.Close();
		CheckMissionReddot();
		ResetUI();
		UpdateSelectUnitUI();
		UpdateBetUI();
		DoOnce();
		if (!m_bWaitOpenUI)
		{
			OpenUIEvent();
		}
		UIOpened();
	}

	private void DoOnce()
	{
		NKMEventBetRecord curRaceBetInfo = NKCScenManager.CurrentUserData().GetRaceData().CurRaceBetInfo;
		if (curRaceBetInfo.winRateTeamA > curRaceBetInfo.winRateTeamB)
		{
			NKCUtil.SetGameobjectActive(m_objGaugeRed, bValue: true);
			NKCUtil.SetGameobjectActive(m_ObjGuessWinRed, bValue: true);
			NKCUtil.SetGameobjectActive(m_ObjGuessWinTagRed, bValue: true);
			NKCUtil.SetGameobjectActive(m_ObjGuessWinTagRed, bValue: true);
			m_rtGuessWinGaugeRed?.SetWidth(m_fMaxGaugeWidth / 2f);
			NKCUtil.SetGameobjectActive(m_objGaugeBlue, bValue: false);
			NKCUtil.SetGameobjectActive(m_ObjGuessWinBlue, bValue: false);
			NKCUtil.SetGameobjectActive(m_ObjGuessWinTagBlue, bValue: false);
			NKCUtil.SetGameobjectActive(m_rtGuessWinGaugeBlue, bValue: false);
			m_SkeletonUnitRed.color = Color.white;
			m_SkeletonUnitBlue.color = m_LoserTeamUnitColor;
		}
		else if (curRaceBetInfo.winRateTeamA == curRaceBetInfo.winRateTeamB)
		{
			NKCUtil.SetGameobjectActive(m_objGaugeRed, bValue: true);
			NKCUtil.SetGameobjectActive(m_ObjGuessWinRed, bValue: true);
			NKCUtil.SetGameobjectActive(m_ObjGuessWinTagRed, bValue: true);
			NKCUtil.SetGameobjectActive(m_ObjGuessWinTagRed, bValue: true);
			m_rtGuessWinGaugeRed?.SetWidth(m_fMaxGaugeWidth / 2f);
			NKCUtil.SetGameobjectActive(m_objGaugeBlue, bValue: false);
			NKCUtil.SetGameobjectActive(m_ObjGuessWinBlue, bValue: false);
			NKCUtil.SetGameobjectActive(m_ObjGuessWinTagBlue, bValue: false);
			NKCUtil.SetGameobjectActive(m_rtGuessWinGaugeBlue, bValue: false);
			m_SkeletonUnitRed.color = Color.white;
			m_SkeletonUnitBlue.color = Color.white;
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objGaugeRed, bValue: false);
			NKCUtil.SetGameobjectActive(m_ObjGuessWinRed, bValue: false);
			NKCUtil.SetGameobjectActive(m_ObjGuessWinTagRed, bValue: false);
			NKCUtil.SetGameobjectActive(m_ObjGuessWinTagRed, bValue: false);
			m_rtGuessWinGaugeBlue?.SetWidth(m_fMaxGaugeWidth / 2f);
			NKCUtil.SetGameobjectActive(m_objGaugeBlue, bValue: true);
			NKCUtil.SetGameobjectActive(m_ObjGuessWinBlue, bValue: true);
			NKCUtil.SetGameobjectActive(m_ObjGuessWinTagBlue, bValue: true);
			NKCUtil.SetGameobjectActive(m_rtGuessWinGaugeBlue, bValue: true);
			m_SkeletonUnitRed.color = m_LoserTeamUnitColor;
			m_SkeletonUnitBlue.color = Color.white;
		}
		NKMEventBetPrivateResult yesterDayRaceData = NKCScenManager.CurrentUserData().GetRaceData().GetYesterDayRaceData();
		if (yesterDayRaceData != null && !yesterDayRaceData.receiveReward)
		{
			m_bWaitOpenUI = true;
			NKCPacketSender.Send_NKMPACKET_EVENT_BET_RESULT_REQ();
		}
	}

	private void CheckMissionReddot()
	{
		NKCUtil.SetGameobjectActive(m_objMissionRedDot, NKCPopupEventRaceMissionV2.Instance.GetMissionRedDotState(m_curEventRaceTemplet.Key));
	}

	private void OpenUIEvent()
	{
		if (!string.IsNullOrEmpty(m_strBGMName))
		{
			NKCSoundManager.StopAllSound();
			NKCSoundManager.StopMusic();
		}
		m_Ani.SetTrigger(m_AniTrigger_INTRO);
		m_bCheckIntroAniComplete = true;
	}

	private void Update()
	{
		if (m_bCheckIntroAniComplete && m_Ani.GetCurrentAnimatorStateInfo(0).IsName(m_AniTrigger_01_IDLE))
		{
			if (!string.IsNullOrEmpty(m_strBGMName))
			{
				NKCSoundManager.PlayMusic(m_strBGMName, bLoop: true);
			}
			if (NKCScenManager.CurrentUserData().GetRaceData().CurRaceBetInfo != null)
			{
				NKMEventBetRecord curRaceBetInfo = NKCScenManager.CurrentUserData().GetRaceData().CurRaceBetInfo;
				m_bIncreaseWinnerGauge = true;
				if (curRaceBetInfo.winRateTeamA > curRaceBetInfo.winRateTeamB)
				{
					m_rtTargetGauge = m_rtGuessWinGaugeRed;
					m_fBaseWidth = m_rtGuessWinGaugeRed.GetWidth();
					m_fGoalWidth = m_fMaxGaugeWidth * ((float)curRaceBetInfo.winRateTeamA * 0.01f);
					m_fIncreaseValue = (m_fGoalWidth - m_fBaseWidth) / (float)m_iGaugeDivideValue;
				}
				else
				{
					m_rtTargetGauge = m_rtGuessWinGaugeBlue;
					m_fBaseWidth = m_rtGuessWinGaugeBlue.GetWidth();
					m_fGoalWidth = m_fMaxGaugeWidth * ((float)curRaceBetInfo.winRateTeamB * 0.01f);
					m_fIncreaseValue = (m_fGoalWidth - m_fBaseWidth) / (float)m_iGaugeDivideValue;
				}
				m_bCheckIntroAniComplete = false;
			}
		}
		if (m_bIncreaseWinnerGauge)
		{
			m_fBaseWidth += m_fIncreaseValue;
			if (m_fBaseWidth >= m_fGoalWidth)
			{
				m_fBaseWidth = m_fGoalWidth;
				m_rtTargetGauge.SetWidth(m_fBaseWidth);
				m_bIncreaseWinnerGauge = false;
			}
			m_rtTargetGauge.SetWidth(m_fBaseWidth);
		}
		OnUpdateButtonHold();
	}

	public void ResetUI()
	{
		NKMEventBetPrivate curUserBetInfo = NKCScenManager.CurrentUserData().GetRaceData().CurUserBetInfo;
		if (curUserBetInfo != null)
		{
			m_PrevSelectTeam = curUserBetInfo.selectTeam;
			m_CurSelectTeam = m_PrevSelectTeam;
			m_iPreBetCnt = (int)curUserBetInfo.currentBetCount;
		}
		else
		{
			m_PrevSelectTeam = EventBetTeam.None;
			m_CurSelectTeam = m_PrevSelectTeam;
			m_iPreBetCnt = 0;
			m_csbtnNext.Lock();
		}
		m_iCurBetCnt = m_iPreBetCnt;
		int num = (int)NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(m_curEventRaceTemplet.RaceItemId);
		m_iMaxBetCnt = Math.Min(m_curEventRaceTemplet.RaceTryItemValue, m_iPreBetCnt + num);
		UpdateUI();
	}

	private void UpdateUI()
	{
		if (m_curEventRaceTemplet != null)
		{
			int num = (int)NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(m_curEventRaceTemplet.RaceItemId);
			int raceTryItemValue = m_curEventRaceTemplet.RaceTryItemValue;
			NKCUtil.SetLabelText(m_lbTodayBetCount, $"{m_iPreBetCnt}/{raceTryItemValue}");
			NKCUtil.SetLabelText(m_lbReqItemCount, num.ToString());
			NKCUtil.SetLabelText(m_lbBetCount, m_iPreBetCnt.ToString());
			NKCUtil.SetLabelText(m_lbLeftTime, $"{m_curEventRaceTemplet.StartDate} ~ {m_curEventRaceTemplet.EndDate}");
			NKCUtil.SetGameobjectActive(m_objSelectTeamRed1, m_CurSelectTeam == EventBetTeam.TeamA);
			NKCUtil.SetGameobjectActive(m_objSelectTeamBlue1, m_CurSelectTeam == EventBetTeam.TeamB);
			if (NKCScenManager.CurrentUserData().GetRaceData().CurRaceBetInfo != null)
			{
				NKMEventBetRecord curRaceBetInfo = NKCScenManager.CurrentUserData().GetRaceData().CurRaceBetInfo;
				NKCUtil.SetLabelText(m_lbPercentRed, $"{curRaceBetInfo.winRateTeamA}%");
				NKCUtil.SetLabelText(m_lbPercentBlue, $"{curRaceBetInfo.winRateTeamB}%");
				NKCUtil.SetLabelText(m_lbValueRed, string.Format(NKCUtilString.GET_STRING_EVENT_RACE_BETTING_RATIO, curRaceBetInfo.dividentRateTeamA));
				NKCUtil.SetLabelText(m_lbValueBlue, string.Format(NKCUtilString.GET_STRING_EVENT_RACE_BETTING_RATIO, curRaceBetInfo.dividentRateTeamB));
			}
			bool flag = m_iPreBetCnt == raceTryItemValue || m_iCurBetCnt == raceTryItemValue || NKCPopupEventRaceUtil.IsLastDay();
			NKCUtil.SetGameobjectActive(m_ObjStartBtnLock, flag);
			NKCUtil.SetGameobjectActive(m_ObjStartBtnNormal, !flag);
			NKMEventBetPrivate curUserBetInfo = NKCScenManager.CurrentUserData().GetRaceData().CurUserBetInfo;
			if (curUserBetInfo != null)
			{
				NKCUtil.SetGameobjectActive(m_objSelectTeamFXBlue, curUserBetInfo.selectTeam == EventBetTeam.TeamB);
				NKCUtil.SetGameobjectActive(m_objSelectTeamFXRed, curUserBetInfo.selectTeam == EventBetTeam.TeamA);
				NKCUtil.SetGameobjectActive(m_objSelectTeamBlue, curUserBetInfo.selectTeam == EventBetTeam.TeamB);
				NKCUtil.SetGameobjectActive(m_objSelectTeamRed, curUserBetInfo.selectTeam == EventBetTeam.TeamA);
				NKCUtil.SetGameobjectActive(m_objSelectTeamNone, curUserBetInfo.selectTeam == EventBetTeam.None);
				NKCUtil.SetLabelText(m_lbBetCount, curUserBetInfo.currentBetCount.ToString());
				NKCUtil.SetGameobjectActive(m_lbBetCount, curUserBetInfo.selectTeam != EventBetTeam.None);
			}
			else
			{
				m_SkeletonUnitRed.color = Color.white;
				m_SkeletonUnitBlue.color = Color.white;
				NKCUtil.SetGameobjectActive(m_objSelectTeamFXBlue, bValue: false);
				NKCUtil.SetGameobjectActive(m_objSelectTeamFXRed, bValue: false);
				NKCUtil.SetGameobjectActive(m_objSelectTeamBlue, bValue: false);
				NKCUtil.SetGameobjectActive(m_objSelectTeamRed, bValue: false);
				NKCUtil.SetGameobjectActive(m_objSelectTeamNone, bValue: true);
				NKCUtil.SetGameobjectActive(m_lbBetCount, bValue: false);
			}
		}
	}

	public void InitUI()
	{
		m_RaceHistory.Init();
		NKCUtil.SetBindFunction(m_csbtnExit, base.Close);
		NKCUtil.SetBindFunction(m_csbtnMission, OnClickMission);
		NKCUtil.SetBindFunction(m_csbtnShop, OnClickShop);
		NKCUtil.SetBindFunction(m_csbtnHistory, OnClickHistory);
		NKCUtil.SetBindFunction(m_csbtnReplay, OnClickReplay);
		NKCUtil.SetBindFunction(m_csbtnHelp, OnClickHelp);
		NKCUtil.SetBindFunction(m_csbtnStart, OnClickStep2);
		NKCUtil.SetHotkey(m_csbtnStart, HotkeyEventType.Confirm);
		NKCUtil.SetBindFunction(m_csbtnNext, OnClickStep3);
		NKCUtil.SetHotkey(m_csbtnNext, HotkeyEventType.Confirm);
		NKCUtil.SetBindFunction(m_csbtnCancel, OnClickBack);
		NKCUtil.SetBindFunction(m_csbtnMin, OnClickMin);
		NKCUtil.SetBindFunction(m_csbtnMinus, OnClickMinus);
		NKCUtil.SetBindFunction(m_csbtnMax, OnClickMax);
		NKCUtil.SetBindFunction(m_csbtnPlus, OnClickPlus);
		NKCUtil.SetBindFunction(m_csbtnCancel1, OnClickBack1);
		NKCUtil.SetBindFunction(m_csbtnConfirm, OnClickConfirm);
		NKCUtil.SetHotkey(m_csbtnConfirm, HotkeyEventType.Confirm);
		m_csbtnMinus?.PointerDown.RemoveAllListeners();
		m_csbtnMinus?.PointerDown.AddListener(OnMinusDown);
		m_csbtnMinus?.PointerUp.RemoveAllListeners();
		m_csbtnMinus?.PointerUp.AddListener(OnButtonUp);
		NKCUtil.SetHotkey(m_csbtnMin, HotkeyEventType.Minus, this, bUpDownEvent: true);
		m_csbtnPlus?.PointerDown.RemoveAllListeners();
		m_csbtnPlus?.PointerDown.AddListener(OnPlusDown);
		m_csbtnPlus?.PointerUp.RemoveAllListeners();
		m_csbtnPlus?.PointerUp.AddListener(OnButtonUp);
		NKCUtil.SetHotkey(m_csbtnPlus, HotkeyEventType.Plus, this, bUpDownEvent: true);
		NKCUtil.SetBindFunction(m_csbtnSelectTeamRed, OnClickSelectTeamRed);
		NKCUtil.SetBindFunction(m_csbtnSelectTeamBlue, OnClickSelectTeamBlue);
	}

	public override void OnBackButton()
	{
		Close();
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		NKCSoundManager.StopAllSound();
		NKCSoundManager.StopMusic();
		m_bWaitOpenUI = false;
		NKCSoundManager.PlayScenMusic(NKCScenManager.GetScenManager().GetNowScenID());
	}

	private void UpdateBetUI()
	{
		NKCUtil.SetLabelText(m_lbBetCount1, m_iCurBetCnt.ToString());
	}

	private void UpdateSelectUnitUI()
	{
		switch (m_CurSelectTeam)
		{
		case EventBetTeam.None:
			NKCUtil.SetGameobjectActive(m_objSelectTeamBlueNormal, bValue: true);
			NKCUtil.SetGameobjectActive(m_objSelectTeamRedNormal, bValue: true);
			NKCUtil.SetGameobjectActive(m_objSelectTeamBlueSelect, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSelectTeamBlueLock, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSelectTeamRedSelect, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSelectTeamRedLock, bValue: false);
			break;
		case EventBetTeam.TeamA:
			NKCUtil.SetGameobjectActive(m_objSelectTeamBlueNormal, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSelectTeamRedNormal, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSelectTeamBlueSelect, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSelectTeamBlueLock, bValue: true);
			NKCUtil.SetGameobjectActive(m_objSelectTeamRedSelect, bValue: true);
			NKCUtil.SetGameobjectActive(m_objSelectTeamRedLock, bValue: false);
			break;
		case EventBetTeam.TeamB:
			NKCUtil.SetGameobjectActive(m_objSelectTeamBlueNormal, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSelectTeamRedNormal, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSelectTeamBlueSelect, bValue: true);
			NKCUtil.SetGameobjectActive(m_objSelectTeamBlueLock, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSelectTeamRedSelect, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSelectTeamRedLock, bValue: true);
			break;
		}
	}

	private void OnClickMission()
	{
		if (!m_bWaitPacket)
		{
			NKCPopupEventRaceMissionV2.Instance.Open(m_curEventRaceTemplet.Key);
		}
	}

	private void OnClickShop()
	{
		if (!string.IsNullOrEmpty(m_curEventRaceTemplet.ShopShortCutParam))
		{
			NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_SHOP, m_curEventRaceTemplet.ShopShortCutParam);
		}
	}

	private void OnClickHistory()
	{
		if (m_bWaitPacket)
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			if (nKMUserData.GetRaceData().UserJoinCount == 0)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_EVENT_RACE_NONE_BETTING_HISTORY);
			}
			else
			{
				m_RaceHistory.Open();
			}
		}
	}

	private void OnClickReplay()
	{
		if (!m_bWaitPacket)
		{
			NKCPacketSender.Send_NKMPACKET_EVENT_BET_RESULT_REQ();
			m_bWaitPacket = true;
		}
	}

	private void OnClickHelp()
	{
		if (!m_bWaitPacket)
		{
			NKCPopupEventHelp.Instance.Open(NKCUtilString.GET_STRING_EVENT_RACE_BETTING_TUTORIAL);
		}
	}

	private void OnClickStep2()
	{
		if (!m_bWaitPacket)
		{
			if (NKCPopupEventRaceUtil.IsLastDay())
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_EVENT_RACE_BETTING_LAST_DAY);
				NKCUtil.SetGameobjectActive(m_ObjStartBtnLock, bValue: true);
				NKCUtil.SetGameobjectActive(m_ObjStartBtnNormal, bValue: false);
			}
			else if (NKCPopupEventRaceUtil.IsMaintenanceTime())
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_EVENT_RACE_BETTING_MAINTENANCE_TIME);
				NKCUtil.SetGameobjectActive(m_ObjStartBtnLock, bValue: true);
				NKCUtil.SetGameobjectActive(m_ObjStartBtnNormal, bValue: false);
			}
			else if ((int)NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(m_curEventRaceTemplet.RaceItemId) <= 0)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_EVENT_RACE_BET_ITEM_NOT_ENOUGH);
			}
			else if (m_iPreBetCnt >= m_curEventRaceTemplet.MaxBetCount)
			{
				NKCPopupMessageManager.AddPopupMessage(string.Format(NKCUtilString.GET_STRING_EVENT_RACE_BET_MAX_DESC, m_curEventRaceTemplet.MaxBetCount));
				NKCUtil.SetGameobjectActive(m_ObjStartBtnLock, bValue: true);
				NKCUtil.SetGameobjectActive(m_ObjStartBtnNormal, bValue: false);
			}
			else if (m_CurSelectTeam != EventBetTeam.None)
			{
				m_Ani.SetTrigger(m_AniTrigger_01_TO_03);
			}
			else
			{
				m_Ani.SetTrigger(m_AniTrigger_01_TO_02);
			}
		}
	}

	private void OnClickStep3()
	{
		if (m_CurSelectTeam == EventBetTeam.None)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_EVENT_RACE_BETTING_NOT_SELECT_TEAM);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objSelectTeamBlue1, m_CurSelectTeam == EventBetTeam.TeamB);
		NKCUtil.SetGameobjectActive(m_objSelectTeamRed1, m_CurSelectTeam == EventBetTeam.TeamA);
		m_Ani.SetTrigger(m_AniTrigger_02_TO_03);
	}

	private void OnClickBack()
	{
		m_Ani.SetTrigger(m_AniTrigger_02_TO_01);
		m_CurSelectTeam = EventBetTeam.None;
	}

	private void OnClickMin()
	{
		m_iCurBetCnt = m_iPreBetCnt;
		UpdateBetUI();
	}

	private void OnClickMinus()
	{
		m_iCurBetCnt--;
		m_iCurBetCnt = Math.Max(m_iPreBetCnt, m_iCurBetCnt);
		UpdateBetUI();
	}

	private void OnClickMax()
	{
		m_iCurBetCnt = m_iMaxBetCnt;
		m_iCurBetCnt = Math.Max(m_iPreBetCnt, m_iCurBetCnt);
		UpdateBetUI();
	}

	private void OnClickPlus()
	{
		m_iCurBetCnt++;
		m_iCurBetCnt = Math.Min(m_iMaxBetCnt, m_iCurBetCnt);
		UpdateBetUI();
	}

	private void OnClickBack1()
	{
		Debug.Log("<color=red>OnClickBack1</color>");
		if (m_PrevSelectTeam != EventBetTeam.None)
		{
			m_Ani.SetTrigger(m_AniTrigger_03_TO_01);
		}
		else
		{
			m_Ani.SetTrigger(m_AniTrigger_03_TO_02);
		}
		UpdateSelectUnitUI();
	}

	private void OnMinusDown(PointerEventData eventData)
	{
		m_iChangeValue = -1;
		m_bPress = true;
		m_fDelay = 0.35f;
		m_fHoldTime = 0f;
		m_bWasHold = false;
	}

	private void OnPlusDown(PointerEventData eventData)
	{
		m_iChangeValue = 1;
		m_bPress = true;
		m_fDelay = 0.35f;
		m_fHoldTime = 0f;
		m_bWasHold = false;
	}

	private void OnButtonUp()
	{
		m_iChangeValue = 0;
		m_fDelay = 0.35f;
		m_bPress = false;
	}

	private void OnUpdateButtonHold()
	{
		if (!m_bPress)
		{
			return;
		}
		m_fHoldTime += Time.deltaTime;
		if (m_fHoldTime >= m_fDelay)
		{
			m_fHoldTime = 0f;
			m_fDelay *= 0.8f;
			int num = ((!(m_fDelay < 0.01f)) ? 1 : 5);
			m_fDelay = Mathf.Clamp(m_fDelay, 0.01f, 0.35f);
			m_iCurBetCnt += m_iChangeValue * num;
			m_bWasHold = true;
			if (m_iChangeValue < 0 && m_iCurBetCnt < m_iPreBetCnt)
			{
				m_iCurBetCnt = m_iPreBetCnt;
				m_bPress = false;
			}
			if (m_iChangeValue > 0 && m_iCurBetCnt > m_iMaxBetCnt)
			{
				m_iCurBetCnt = m_iMaxBetCnt;
				m_bPress = false;
			}
			UpdateBetUI();
		}
	}

	public void OnScroll(PointerEventData eventData)
	{
		if (eventData.scrollDelta.y < 0f)
		{
			if (m_iCurBetCnt > m_iPreBetCnt)
			{
				m_iCurBetCnt--;
			}
		}
		else if (eventData.scrollDelta.y > 0f && m_iCurBetCnt < m_iMaxBetCnt)
		{
			m_iCurBetCnt++;
		}
		if (m_iCurBetCnt == 0)
		{
			m_iCurBetCnt = 1;
		}
		int iCurBetCnt = Mathf.Clamp(m_iCurBetCnt, 1, m_iMaxBetCnt);
		m_iCurBetCnt = iCurBetCnt;
		UpdateBetUI();
	}

	private void OnClickSelectTeamRed()
	{
		m_CurSelectTeam = EventBetTeam.TeamA;
		m_csbtnNext.UnLock();
		UpdateSelectUnitUI();
	}

	private void OnClickSelectTeamBlue()
	{
		m_CurSelectTeam = EventBetTeam.TeamB;
		m_csbtnNext.UnLock();
		UpdateSelectUnitUI();
	}

	private void OnClickConfirm()
	{
		if (m_iCurBetCnt == m_iPreBetCnt)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_EVENT_RACE_BETTING_COUNT_NOT_ENOUGH);
		}
		else if (m_iPreBetCnt > 0)
		{
			NKCPacketSender.Send_NKMPACKET_EVENT_BET_BETTING_REQ(m_iCurBetCnt - m_iPreBetCnt);
			Debug.Log($"<color=red>Send_NKMPACKET_EVENT_BET_BETTING_REQ : {m_iCurBetCnt - m_iPreBetCnt}</color>");
		}
		else
		{
			NKCPacketSender.Send_NKMPACKET_EVENT_BET_SELECT_TEAM_REQ(m_CurSelectTeam, m_iCurBetCnt);
			Debug.Log($"<color=red>Send_NKMPACKET_EVENT_BET_SELECT_TEAM_REQ : {m_CurSelectTeam} - {m_iCurBetCnt}</color>");
		}
	}

	public void OnRecv(NKMPACKET_EVENT_BET_RESULT_ACK sPacket)
	{
		m_bWaitPacket = false;
		if (sPacket != null)
		{
			if (m_bWaitOpenUI)
			{
				OpenUIEvent();
				m_bWaitOpenUI = false;
				NKCPopupEventRacePlay.Instance.Open(sPacket.eventIndex, sPacket.rewardData, sPacket.isWin, sPacket.selectTeam, sPacket.isApplyDividentRate);
			}
			else if (sPacket.eventIndex == -1 || sPacket.selectTeam == EventBetTeam.None)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_EVENT_RACE_NONE_BETTING_HISTORY);
			}
			else
			{
				NKCPopupEventRacePlay.Instance.Open(sPacket.eventIndex, sPacket.rewardData, sPacket.isWin, sPacket.selectTeam, sPacket.isApplyDividentRate);
			}
		}
	}

	public void OnRecv(NKMPACKET_EVENT_BET_SELECT_TEAM_ACK sPacket)
	{
		NKCPopupEventRaceBetFinish.Instance.Open(m_CurSelectTeam, (int)sPacket.eventBetPrivate.currentBetCount, OnBetComplete);
	}

	public void OnRecv(NKMPACKET_EVENT_BET_BETTING_ACK sPacket)
	{
		NKCPopupEventRaceBetFinish.Instance.Open(m_CurSelectTeam, (int)sPacket.eventBetPrivate.currentBetCount, OnBetComplete);
	}

	public void OnBetComplete()
	{
		ResetUI();
		m_Ani.SetTrigger(m_AniTrigger_01_IDLE);
	}

	public void OnRecvPacket()
	{
		m_bWaitPacket = false;
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		if (m_curEventRaceTemplet != null && itemData.ItemID == m_curEventRaceTemplet.RaceItemId)
		{
			ResetUI();
		}
	}

	public override void OnMissionUpdated()
	{
		CheckMissionReddot();
	}
}
