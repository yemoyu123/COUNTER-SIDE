using System;
using System.Collections.Generic;
using ClientPacket.Warfare;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupRepeatOperation : NKCUIBase
{
	public delegate void dOnClose();

	public EventTrigger m_etBG;

	public NKCUIComStateButton m_csbtnClose;

	[Header("왼쪽 UI")]
	public Text m_lbEPTitle;

	public Text m_lbEPName;

	public static string m_LastEPTitle = "";

	public static string m_LastEPName = "";

	public GameObject m_objRewardNone;

	public GameObject m_objReward;

	public LoopScrollRect m_lsrReward;

	public GridLayoutGroup m_GridLayoutGroup;

	[Header("오른쪽 메뉴")]
	public NKCUIComStateButton m_csbtnStart;

	public NKCUIComStateButton m_csbtnStartDisabled;

	public NKCUIComStateButton m_csbtnCancel;

	public NKCUIComStateButton m_csbtnContinue;

	public NKCUIComStateButton m_csbtnOK;

	public GameObject m_objRepeatCountStop;

	public NKCUIComStateButton m_csbtnRepeatCountStopReset;

	public NKCUIComStateButton m_csbtnRepeatCountStopAdd1;

	public NKCUIComStateButton m_csbtnRepeatCountStopAdd10;

	public NKCUIComStateButton m_csbtnRepeatCountStopAdd100;

	public NKCUIComStateButton m_csbtnRepeatCountStopAddMax;

	public Text m_lbRepeatCount;

	public GameObject m_objRepeatCountProgress;

	public Text m_lbRepeatCountInProgress;

	public Text m_lbRemainRepeatCount;

	public NKCUISlot m_NKCUISlot;

	public Text m_lbCostDesc;

	public Text m_lbCostCount;

	public GameObject m_objHavingCount;

	public Text m_lbHavingCount;

	public GameObject m_objHavingCountNone;

	public GameObject m_objStateNone;

	public GameObject m_objState;

	public Text m_lbState;

	public GameObject m_objStateProgressingIcon;

	public Text m_lbProgressTime;

	public GameObject m_objPowerSaveMode;

	public NKCUIComToggle m_ctPowerSaveMode;

	private dOnClose m_dOnClose;

	private static int m_CostItemCount = 0;

	private static long m_CostHavingCount = 0L;

	private static long m_RepeatCount = 0L;

	private bool m_bFirstOpen = true;

	private List<NKCUISlot.SlotData> m_lstList = new List<NKCUISlot.SlotData>();

	private static float m_fElapsedTime = 0f;

	private bool m_bCheatMode;

	private int MAX_REPEAT_COUNT_FOR_CHEAT = 3000;

	[Header("입장 제한")]
	public GameObject m_OPERATION_REPEAT_EnterLimit;

	public Text m_EnterLimit_TEXT;

	public GameObject m_BonusType;

	public Image m_BonusType_Icon;

	private int m_iCurEpisodeKey;

	private static NKCPopupRepeatOperation m_Instance = null;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "NKCPopupRepeatOperation";

	public static NKCPopupRepeatOperation Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupRepeatOperation>("AB_UI_NKM_UI_OPERATION", "NKM_UI_POPUP_OPERATION REPEAT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupRepeatOperation>();
				m_Instance.Init();
			}
			return m_Instance;
		}
		private set
		{
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

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public void Init()
	{
		m_etBG.triggers.Clear();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			Close();
		});
		m_etBG.triggers.Add(entry);
		m_csbtnClose.PointerClick.RemoveAllListeners();
		m_csbtnClose.PointerClick.AddListener(base.Close);
		m_csbtnStart.PointerClick.RemoveAllListeners();
		m_csbtnStart.PointerClick.AddListener(OnClickStart);
		NKCUtil.SetHotkey(m_csbtnStart, HotkeyEventType.Confirm);
		m_csbtnStartDisabled.PointerClick.RemoveAllListeners();
		m_csbtnStartDisabled.PointerClick.AddListener(OnClickStart);
		m_csbtnCancel.PointerClick.RemoveAllListeners();
		m_csbtnCancel.PointerClick.AddListener(OnClickCancel);
		m_csbtnContinue.PointerClick.RemoveAllListeners();
		m_csbtnContinue.PointerClick.AddListener(delegate
		{
			NKCScenManager.GetScenManager().GetNKCPowerSaveMode().SetEnable(m_ctPowerSaveMode.m_bChecked);
			OnClickClose();
		});
		NKCUtil.SetHotkey(m_csbtnContinue, HotkeyEventType.Confirm);
		m_csbtnOK.PointerClick.RemoveAllListeners();
		m_csbtnOK.PointerClick.AddListener(OnClickClose);
		NKCUtil.SetHotkey(m_csbtnOK, HotkeyEventType.Confirm);
		m_csbtnRepeatCountStopReset.PointerClick.RemoveAllListeners();
		m_csbtnRepeatCountStopReset.PointerClick.AddListener(OnClickResetRepeatCount);
		m_csbtnRepeatCountStopAdd1.PointerClick.RemoveAllListeners();
		m_csbtnRepeatCountStopAdd1.PointerClick.AddListener(delegate
		{
			OnClickRepeatCountAdd(1);
		});
		m_csbtnRepeatCountStopAdd10.PointerClick.RemoveAllListeners();
		m_csbtnRepeatCountStopAdd10.PointerClick.AddListener(delegate
		{
			OnClickRepeatCountAdd(10);
		});
		m_csbtnRepeatCountStopAdd100.PointerClick.RemoveAllListeners();
		m_csbtnRepeatCountStopAdd100.PointerClick.AddListener(delegate
		{
			OnClickRepeatCountAdd(100);
		});
		m_csbtnRepeatCountStopAddMax.PointerClick.RemoveAllListeners();
		m_csbtnRepeatCountStopAddMax.PointerClick.AddListener(OnClickRepeatAddMax);
		m_NKCUISlot.Init();
		m_lsrReward.dOnGetObject += GetRewardSlot;
		m_lsrReward.dOnReturnObject += ReturnRewardSlot;
		m_lsrReward.dOnProvideData += ProvideRewardSlot;
		NKCUtil.SetScrollHotKey(m_lsrReward);
		m_fElapsedTime = 0f;
	}

	public RectTransform GetRewardSlot(int index)
	{
		NKCUISlot newInstance = NKCUISlot.GetNewInstance(null);
		if (newInstance != null)
		{
			return newInstance.GetComponent<RectTransform>();
		}
		return null;
	}

	public void ReturnRewardSlot(Transform tr)
	{
		tr.SetParent(base.transform);
		UnityEngine.Object.Destroy(tr.gameObject);
	}

	public void ProvideRewardSlot(Transform tr, int index)
	{
		NKCUISlot component = tr.GetComponent<NKCUISlot>();
		if (component != null)
		{
			if (m_lstList.Count > index && index >= 0)
			{
				NKCUtil.SetGameobjectActive(component, bValue: true);
				component.SetData(m_lstList[index]);
				SetEquipSlotHaveNoMenu(component);
			}
			else
			{
				NKCUtil.SetGameobjectActive(component, bValue: false);
			}
		}
	}

	private void UpdateRepeatCountUI()
	{
		if (m_RepeatCount >= 1)
		{
			NKCUtil.SetLabelText(m_lbRepeatCount, string.Format(NKCUtilString.GET_STRING_REPEAT_OPERATION_REPEAT_COUNT_ONE_PARAM, m_RepeatCount));
		}
		else
		{
			NKCUtil.SetLabelText(m_lbRepeatCount, string.Format(NKCUtilString.GET_STRING_REPEAT_OPERATION_RED_COLOR_REPEAT_COUNT_ONE_PARAM, m_RepeatCount));
		}
	}

	private void OnClickResetRepeatCount()
	{
		if (m_bCheatMode)
		{
			m_RepeatCount = 1L;
		}
		else if (m_CostHavingCount >= m_CostItemCount)
		{
			m_RepeatCount = 1L;
		}
		else
		{
			m_RepeatCount = 0L;
		}
		UpdateRepeatCountUI();
		UpdateHavingCountUI();
		UpdateCostItemCountUI();
		NKCUtil.SetGameobjectActive(m_csbtnStart, m_RepeatCount == 1);
		NKCUtil.SetGameobjectActive(m_csbtnStartDisabled, m_RepeatCount == 0);
	}

	private void OnClickRepeatCountAdd(int count)
	{
		long repeatCount = m_RepeatCount;
		repeatCount += count;
		long maxRepeatCount = GetMaxRepeatCount();
		if (maxRepeatCount < repeatCount)
		{
			repeatCount = maxRepeatCount;
		}
		m_RepeatCount = repeatCount;
		UpdateRepeatCountUI();
		UpdateHavingCountUI();
		UpdateCostItemCountUI();
	}

	private long GetMaxRepeatCount()
	{
		if (m_bCheatMode)
		{
			return MAX_REPEAT_COUNT_FOR_CHEAT;
		}
		if (m_CostItemCount > 0)
		{
			int num = -1;
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null && m_iCurEpisodeKey != 0)
			{
				NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(m_iCurEpisodeKey);
				if (nKMStageTempletV != null && nKMStageTempletV.EnterLimit > 0)
				{
					num = ((!nKMUserData.IsHaveStatePlayData(m_iCurEpisodeKey)) ? nKMStageTempletV.EnterLimit : (nKMStageTempletV.EnterLimit - nKMUserData.GetStatePlayCnt(m_iCurEpisodeKey)));
				}
			}
			if (num >= 0)
			{
				return Math.Min(num, m_CostHavingCount / m_CostItemCount);
			}
			return m_CostHavingCount / m_CostItemCount;
		}
		return 0L;
	}

	private void OnClickRepeatAddMax()
	{
		m_RepeatCount = GetMaxRepeatCount();
		UpdateRepeatCountUI();
		UpdateHavingCountUI();
		UpdateCostItemCountUI();
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		if (m_dOnClose != null)
		{
			m_dOnClose();
			m_dOnClose = null;
		}
	}

	public void OpenForResult(dOnClose _dOnClose = null)
	{
		m_bCheatMode = false;
		m_dOnClose = _dOnClose;
		UIOpened();
		if (m_bFirstOpen)
		{
			m_lsrReward.PrepareCells();
			m_bFirstOpen = false;
		}
		UpdateEnterLimitData();
		SetUIForResult();
	}

	private void SetUIForResult()
	{
		NKCRepeatOperaion nKCRepeatOperaion = NKCScenManager.GetScenManager().GetNKCRepeatOperaion();
		if (nKCRepeatOperaion != null)
		{
			NKCUtil.SetGameobjectActive(m_objPowerSaveMode, bValue: false);
			NKCUtil.SetLabelText(m_lbEPTitle, nKCRepeatOperaion.GetPrevEPTitle());
			NKCUtil.SetLabelText(m_lbEPName, nKCRepeatOperaion.GetPrevEPName());
			int prevCostItemID = nKCRepeatOperaion.GetPrevCostItemID();
			nKCRepeatOperaion.GetPrevCostItemCount();
			if (prevCostItemID > 0)
			{
				NKCUtil.SetGameobjectActive(m_NKCUISlot, bValue: true);
				m_NKCUISlot.SetData(NKCUISlot.SlotData.MakeMiscItemData(prevCostItemID, 1L));
				NKCUtil.SetLabelText(m_lbCostDesc, NKCUtilString.GET_STRING_REPEAT_OPERATION_COST_COUNT_UNTIL_NOW);
				NKCUtil.SetGameobjectActive(m_objHavingCount, bValue: false);
				NKCUtil.SetGameobjectActive(m_objHavingCountNone, bValue: true);
				long num = 0L;
				num = nKCRepeatOperaion.GetPrevCostIncreaseCount() * m_CostItemCount;
				NKCUtil.SetLabelText(m_lbCostCount, num.ToString());
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_NKCUISlot, bValue: false);
			}
			NKCUtil.SetGameobjectActive(m_objRepeatCountStop, bValue: false);
			NKCUtil.SetGameobjectActive(m_objRepeatCountProgress, bValue: true);
			NKCUtil.SetGameobjectActive(m_objStateNone, bValue: false);
			NKCUtil.SetGameobjectActive(m_objState, bValue: true);
			NKCUtil.SetGameobjectActive(m_objStateProgressingIcon, bValue: false);
			NKCUtil.SetLabelText(m_lbRemainRepeatCount, "");
			NKCUtil.SetLabelText(m_lbRepeatCountInProgress, string.Format(NKCUtilString.GET_STRING_REPEAT_OPERATION_COMPLETE_REPEAT_COUNT_ONE_PARAM, nKCRepeatOperaion.GetPrevRepeatCount().ToString()));
			NKCUtil.SetGameobjectActive(m_csbtnStart, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnStartDisabled, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnCancel, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnContinue, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnOK, bValue: true);
			NKCUtil.SetLabelText(m_lbState, nKCRepeatOperaion.GetStopReason());
			UpdateResultTime();
			List<NKCUISlot.SlotData> list = NKCUISlot.MakeSlotDataListFromReward(nKCRepeatOperaion.GetPrevReward());
			if (list.Count == 0)
			{
				m_lsrReward.TotalCount = 0;
				m_lsrReward.RefreshCells();
				NKCUtil.SetGameobjectActive(m_objRewardNone, bValue: true);
			}
			else
			{
				m_lstList = list;
				m_lsrReward.TotalCount = m_lstList.Count;
				NKCUtil.SetGameobjectActive(m_objRewardNone, bValue: false);
				m_lsrReward.velocity = new Vector2(0f, 0f);
				m_lsrReward.SetIndexPosition(0);
			}
		}
	}

	public void Open(dOnClose _dOnClose = null)
	{
		m_bCheatMode = false;
		m_dOnClose = _dOnClose;
		UIOpened();
		if (m_bFirstOpen)
		{
			m_lsrReward.PrepareCells();
			m_bFirstOpen = false;
		}
		UpdateEnterLimitData();
		SetUIByCurrScen();
	}

	private void UpdateEnterLimitData()
	{
		m_iCurEpisodeKey = 0;
		NKMGameData gameData = NKCScenManager.GetScenManager().GetGameClient().GetGameData();
		NKMStageTempletV2 nKMStageTempletV = null;
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME && NKCScenManager.GetScenManager().WarfareGameData != null)
		{
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(NKCScenManager.GetScenManager().WarfareGameData.warfareTempletID);
			if (nKMWarfareTemplet != null)
			{
				nKMStageTempletV = nKMWarfareTemplet.StageTemplet;
			}
		}
		if (nKMStageTempletV == null && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_DUNGEON_ATK_READY)
		{
			nKMStageTempletV = NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().GetStageTemplet();
		}
		if (nKMStageTempletV == null && gameData != null && gameData.GetGameType() == NKM_GAME_TYPE.NGT_WARFARE)
		{
			nKMStageTempletV = NKMWarfareTemplet.Find(gameData.m_WarfareID).StageTemplet;
		}
		else if (nKMStageTempletV == null && gameData != null)
		{
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(NKMDungeonManager.GetDungeonStrID(gameData.m_DungeonID));
			if (dungeonTempletBase != null)
			{
				nKMStageTempletV = dungeonTempletBase.StageTemplet;
			}
		}
		if (nKMStageTempletV != null)
		{
			string text = "";
			int statePlayCnt = NKCScenManager.CurrentUserData().GetStatePlayCnt(nKMStageTempletV.Key);
			text = nKMStageTempletV.EnterLimitCond switch
			{
				NKMStageTempletV2.RESET_TYPE.DAY => string.Format(NKCUtilString.GET_STRING_POPUP_DUNGEON_ENTER_LIMIT_DESC_DAY_02, nKMStageTempletV.EnterLimit - statePlayCnt, nKMStageTempletV.EnterLimit), 
				NKMStageTempletV2.RESET_TYPE.MONTH => string.Format(NKCUtilString.GET_STRING_POPUP_DUNGEON_ENTER_LIMIT_DESC_MONTH_02, nKMStageTempletV.EnterLimit - statePlayCnt, nKMStageTempletV.EnterLimit), 
				NKMStageTempletV2.RESET_TYPE.WEEK => string.Format(NKCUtilString.GET_STRING_POPUP_DUNGEON_ENTER_LIMIT_DESC_WEEK_02, nKMStageTempletV.EnterLimit - statePlayCnt, nKMStageTempletV.EnterLimit), 
				_ => string.Format(NKCUtilString.GET_STRING_POPUP_DUNGEON_ENTER_LIMIT_DESC_DAY_02, nKMStageTempletV.EnterLimit - statePlayCnt, nKMStageTempletV.EnterLimit), 
			};
			m_iCurEpisodeKey = nKMStageTempletV.Key;
			NKCUtil.SetGameobjectActive(m_OPERATION_REPEAT_EnterLimit, nKMStageTempletV.EnterLimit > 0);
			NKCUtil.SetLabelText(m_EnterLimit_TEXT, text);
			if (nKMStageTempletV.m_BuffType.Equals(RewardTuningType.None))
			{
				NKCUtil.SetGameobjectActive(m_BonusType, bValue: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_BonusType, bValue: true);
				NKCUtil.SetImageSprite(m_BonusType_Icon, NKCUtil.GetBounsTypeIcon(nKMStageTempletV.m_BuffType, big: false));
			}
			if (nKMStageTempletV.EnterLimit - statePlayCnt <= 0)
			{
				NKCUtil.SetLabelTextColor(m_EnterLimit_TEXT, Color.red);
				NKCUtil.SetGameobjectActive(m_csbtnStart, bValue: false);
				NKCUtil.SetGameobjectActive(m_csbtnStartDisabled, bValue: true);
			}
			else
			{
				NKCUtil.SetLabelTextColor(m_EnterLimit_TEXT, Color.white);
				NKCUtil.SetGameobjectActive(m_csbtnStart, bValue: true);
				NKCUtil.SetGameobjectActive(m_csbtnStartDisabled, bValue: false);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_OPERATION_REPEAT_EnterLimit, bValue: false);
		}
	}

	private void SetEP_UI(NKMStageTempletV2 cNKMStageTemplet, string dungeonOrWarfareName)
	{
		if (cNKMStageTemplet != null)
		{
			NKMEpisodeTempletV2 nKMEpisodeTempletV = null;
			nKMEpisodeTempletV = cNKMStageTemplet.EpisodeTemplet;
			if (nKMEpisodeTempletV != null)
			{
				NKCUtil.SetLabelText(m_lbEPTitle, nKMEpisodeTempletV.GetEpisodeTitle());
			}
			if (cNKMStageTemplet.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_PRACTICE)
			{
				NKCUtil.SetLabelText(m_lbEPName, string.Format(NKCUtilString.GET_STRING_EP_TRAINING_NUMBER, cNKMStageTemplet.m_StageUINum) + " " + dungeonOrWarfareName);
				return;
			}
			if (nKMEpisodeTempletV != null && nKMEpisodeTempletV.m_EPCategory == EPISODE_CATEGORY.EC_DAILY)
			{
				NKCUtil.SetLabelText(m_lbEPName, dungeonOrWarfareName + " " + NKCUtilString.GetDailyDungeonLVDesc(cNKMStageTemplet.m_StageUINum));
				return;
			}
			NKCUtil.SetLabelText(m_lbEPName, cNKMStageTemplet.ActId + "-" + cNKMStageTemplet.m_StageUINum + ". " + dungeonOrWarfareName);
		}
	}

	private void SetUIByCurrScen()
	{
		NKCRepeatOperaion nKCRepeatOperaion = NKCScenManager.GetScenManager().GetNKCRepeatOperaion();
		if (nKCRepeatOperaion == null)
		{
			return;
		}
		NKCUtil.SetGameobjectActive(m_objPowerSaveMode, bValue: true);
		m_ctPowerSaveMode.Select(bSelect: false);
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMStageTempletV2 stageTemplet = null;
		string text = "";
		int costItemID = 0;
		int costItemCount = 0;
		if (!NKCRepeatOperaion.GetRepeatOperationType(out var _, out stageTemplet))
		{
			return;
		}
		text = NKCRepeatOperaion.GetEpisodeBattleName();
		NKCRepeatOperaion.GetCostInfo(out costItemID, out costItemCount);
		SetEP_UI(stageTemplet, text);
		if (m_lbEPName != null)
		{
			m_LastEPName = m_lbEPName.text;
		}
		if (m_lbEPTitle != null)
		{
			m_LastEPTitle = m_lbEPTitle.text;
		}
		if (costItemID > 0)
		{
			NKCUtil.SetGameobjectActive(m_NKCUISlot, bValue: true);
			m_NKCUISlot.SetData(NKCUISlot.SlotData.MakeMiscItemData(costItemID, 1L));
			if (nKCRepeatOperaion.GetIsOnGoing())
			{
				NKCUtil.SetLabelText(m_lbCostDesc, NKCUtilString.GET_STRING_REPEAT_OPERATION_COST_COUNT_UNTIL_NOW);
				NKCUtil.SetGameobjectActive(m_objHavingCount, bValue: false);
				NKCUtil.SetGameobjectActive(m_objHavingCountNone, bValue: true);
			}
			else
			{
				NKCUtil.SetLabelText(m_lbCostDesc, NKCUtilString.GET_STRING_REPEAT_OPERATION_COST_COUNT);
				NKCUtil.SetGameobjectActive(m_objHavingCount, bValue: true);
				NKCUtil.SetGameobjectActive(m_objHavingCountNone, bValue: false);
				m_CostItemCount = costItemCount;
				m_CostHavingCount = myUserData.m_InventoryData.GetCountMiscItem(costItemID);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKCUISlot, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objRepeatCountStop, !nKCRepeatOperaion.GetIsOnGoing());
		NKCUtil.SetGameobjectActive(m_objRepeatCountProgress, nKCRepeatOperaion.GetIsOnGoing());
		NKCUtil.SetGameobjectActive(m_objStateNone, !nKCRepeatOperaion.GetIsOnGoing());
		NKCUtil.SetGameobjectActive(m_objState, nKCRepeatOperaion.GetIsOnGoing());
		NKCUtil.SetGameobjectActive(m_objStateProgressingIcon, nKCRepeatOperaion.GetIsOnGoing());
		if (nKCRepeatOperaion.GetIsOnGoing())
		{
			NKCUtil.SetLabelText(m_lbRemainRepeatCount, string.Format(NKCUtilString.GET_STRING_REPEAT_OPERATION_REMAIN_REPEAT_COUNT_ONE_PARAM, (nKCRepeatOperaion.GetMaxRepeatCount() - nKCRepeatOperaion.GetCurrRepeatCount()).ToString()));
			NKCUtil.SetLabelText(m_lbRepeatCountInProgress, string.Format(NKCUtilString.GET_STRING_REPEAT_OPERATION_COMPLETE_REPEAT_COUNT_ONE_PARAM, nKCRepeatOperaion.GetCurrRepeatCount().ToString()));
			NKCUtil.SetGameobjectActive(m_csbtnStart, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnStartDisabled, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnCancel, bValue: true);
			NKCUtil.SetGameobjectActive(m_csbtnContinue, bValue: true);
			NKCUtil.SetGameobjectActive(m_csbtnOK, bValue: false);
			NKCUtil.SetLabelText(m_lbState, NKCUtilString.GET_STRING_REPEAT_OPERATION_IS_ON_GOING);
			UpdateProgressTime();
		}
		else
		{
			OnClickResetRepeatCount();
			NKCUtil.SetGameobjectActive(m_csbtnCancel, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnContinue, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnOK, bValue: false);
		}
		UpdateCostItemCountUI();
		UpdateHavingCountUI();
		List<NKCUISlot.SlotData> list = NKCUISlot.MakeSlotDataListFromReward(nKCRepeatOperaion.GetReward());
		if (list.Count == 0)
		{
			m_lsrReward.TotalCount = 0;
			m_lsrReward.RefreshCells();
			NKCUtil.SetGameobjectActive(m_objRewardNone, bValue: true);
		}
		else
		{
			m_lstList = list;
			m_lsrReward.TotalCount = m_lstList.Count;
			NKCUtil.SetGameobjectActive(m_objRewardNone, bValue: false);
			m_lsrReward.velocity = new Vector2(0f, 0f);
			m_lsrReward.SetIndexPosition(0);
		}
	}

	private void SetEquipSlotHaveNoMenu(NKCUISlot cNKCUISlot)
	{
		if (cNKCUISlot.GetSlotData() != null && (cNKCUISlot.GetSlotData().eType == NKCUISlot.eSlotMode.Equip || cNKCUISlot.GetSlotData().eType == NKCUISlot.eSlotMode.EquipCount))
		{
			cNKCUISlot.Set_EQUIP_BOX_BOTTOM_MENU_TYPE(NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
		}
	}

	private void UpdateCostItemCountUI()
	{
		if (m_bCheatMode)
		{
			NKCUtil.SetLabelText(m_lbCostCount, "");
			return;
		}
		NKCRepeatOperaion nKCRepeatOperaion = NKCScenManager.GetScenManager().GetNKCRepeatOperaion();
		if (nKCRepeatOperaion != null)
		{
			if (nKCRepeatOperaion.GetIsOnGoing())
			{
				long num = 0L;
				num = nKCRepeatOperaion.GetCostIncreaseCount() * m_CostItemCount;
				NKCUtil.SetLabelText(m_lbCostCount, num.ToString());
			}
			else if (m_RepeatCount >= 1)
			{
				NKCUtil.SetLabelText(m_lbCostCount, (m_CostItemCount * m_RepeatCount).ToString());
			}
			else
			{
				NKCUtil.SetLabelText(m_lbCostCount, m_CostItemCount.ToString());
			}
		}
	}

	private void UpdateHavingCountUI()
	{
		if (m_bCheatMode)
		{
			NKCUtil.SetLabelText(m_lbHavingCount, "");
		}
		else if (m_RepeatCount >= 1)
		{
			NKCUtil.SetLabelText(m_lbHavingCount, m_CostHavingCount.ToString());
		}
		else
		{
			NKCUtil.SetLabelText(m_lbHavingCount, "<color=red>" + m_CostHavingCount + "</color>");
		}
	}

	private void UpdateProgressTime()
	{
		NKCRepeatOperaion nKCRepeatOperaion = NKCScenManager.GetScenManager().GetNKCRepeatOperaion();
		if (nKCRepeatOperaion != null && nKCRepeatOperaion.GetIsOnGoing())
		{
			TimeSpan timeSpan = NKCSynchronizedTime.GetServerUTCTime() - nKCRepeatOperaion.GetStartTime();
			NKCUtil.SetLabelText(m_lbProgressTime, NKCUtilString.GetTimeSpanString(timeSpan));
		}
	}

	private void UpdateResultTime()
	{
		NKCRepeatOperaion nKCRepeatOperaion = NKCScenManager.GetScenManager().GetNKCRepeatOperaion();
		if (nKCRepeatOperaion != null)
		{
			TimeSpan prevProgressDuration = nKCRepeatOperaion.GetPrevProgressDuration();
			NKCUtil.SetLabelText(m_lbProgressTime, string.Format(NKCUtilString.GET_STRING_REPEAT_OPERATION_RESULT_TOTAL_TIME, NKCUtilString.GetTimeSpanString(prevProgressDuration)));
		}
	}

	private void Update()
	{
		if (m_fElapsedTime + 1f < Time.time)
		{
			m_fElapsedTime = Time.time;
			UpdateProgressTime();
		}
	}

	private void OnClickStart()
	{
		if (m_bCheatMode)
		{
			return;
		}
		if (m_NKCUISlot.gameObject.activeSelf && m_CostHavingCount < m_CostItemCount)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_REPEAT_OPERATION_COST_MORE_REQUIRED);
			return;
		}
		switch (NKCScenManager.GetScenManager().GetNowScenID())
		{
		case NKM_SCEN_ID.NSI_WARFARE_GAME:
		{
			WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
			if (warfareGameData == null)
			{
				break;
			}
			if (warfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_STOP)
			{
				if (NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame()
					.OnClickGameStart(bRepeatOperation: true))
				{
					CloseAndStartWithCurrOption();
				}
				break;
			}
			CloseAndStartWithCurrOption();
			if (!NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame()
				.IsAutoWarfare())
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame()
					.m_NKCWarfareGameHUD.SendAutoReq(bAuto: true);
			}
			break;
		}
		case NKM_SCEN_ID.NSI_GAME:
		{
			NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
			if (gameClient == null)
			{
				break;
			}
			NKMGameRuntimeTeamData myRunTimeTeamData = gameClient.GetMyRunTimeTeamData();
			if (myRunTimeTeamData != null)
			{
				CloseAndStartWithCurrOption();
				if (!myRunTimeTeamData.m_bAutoRespawn)
				{
					gameClient.Send_Packet_GAME_AUTO_RESPAWN_REQ(bAutoRespawn: true);
				}
			}
			break;
		}
		case NKM_SCEN_ID.NSI_DUNGEON_ATK_READY:
			CloseAndStartWithCurrOption();
			NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().StartByRepeatOperation();
			break;
		}
	}

	public void CloseAndStartWithCurrOption()
	{
		if (base.IsOpen)
		{
			NKCScenManager.GetScenManager().GetNKCRepeatOperaion().SetIsOnGoing(bSet: true);
			NKCScenManager.GetScenManager().GetNKCRepeatOperaion().SetNeedToSaveRewardData(bSet: true);
			NKCScenManager.GetScenManager().GetNKCRepeatOperaion().SetStartTime(NKCSynchronizedTime.GetServerUTCTime());
			NKCScenManager.GetScenManager().GetNKCRepeatOperaion().SetCurrRepeatCount(0L);
			NKCScenManager.GetScenManager().GetNKCRepeatOperaion().SetMaxRepeatCount(m_RepeatCount);
			NKCScenManager.GetScenManager().GetNKCRepeatOperaion().ResetReward();
			NKCScenManager.GetScenManager().GetNKCRepeatOperaion().UpdateRepeatOperationGameHudUI();
			NKCScenManager.GetScenManager().GetNKCPowerSaveMode().SetEnable(m_ctPowerSaveMode.m_bChecked);
			Close();
		}
	}

	private void OnClickCancel()
	{
		NKCScenManager.GetScenManager().GetNKCRepeatOperaion().OnClickCancel();
		Close();
	}

	private void OnClickClose()
	{
		Close();
	}
}
