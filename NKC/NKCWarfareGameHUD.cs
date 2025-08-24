using System;
using System.Collections;
using System.Collections.Generic;
using ClientPacket.Warfare;
using DG.Tweening;
using NKC.UI;
using NKC.UI.Warfare;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC;

public class NKCWarfareGameHUD : MonoBehaviour
{
	public delegate void OnStartUserPhase();

	public delegate void OnStartEnemyPhase();

	private GameObject m_NUF_WARFARE;

	private string m_WarfareStrID = "";

	public GameObject m_NUF_WARFARE_SHIP_NUM;

	public Text m_NUF_WARFARE_SHIP_NUM_TEXT;

	public GameObject m_NUF_WARFARE_SHIP_NUM_SUPPORT;

	public Text m_NUF_WARFARE_SHIP_NUM_SUPPORT_TEXT;

	public GameObject m_NUF_WARFARE_OPERATION_TITLE;

	public Text m_OPERATION_TITLE_EP_TEXT;

	public Text m_OPERATION_TITLE_TEXT;

	public Image m_OPERATION_TITLE_BOUNS;

	public Text m_OPERATION_TITLE_EP_TEXT_FOR_PLAY;

	public Text m_OPERATION_TITLE_TEXT_FOR_PLAY;

	[Header("우측 하단")]
	public GameObject m_NUF_WARFARE_SUB_MENU_OPERATION_SUPPORT_TEXT;

	public GameObject m_NUF_WARFARE_SUB_MENU_OPERATION_TEXT;

	public GameObject m_NUF_WARFARE_SUB_MENU_OPERATION_BUTTON;

	public Animator m_NUF_WARFARE_SUB_MENU_OPERATION_BUTTON_FX;

	public GameObject m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT;

	public NKCUIComButton m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_BUTTON;

	public NKCUIComButton m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_BUTTON_RED;

	public Text m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_TEXT;

	public Text m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_TURN_COUNT;

	public Animator m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_FX;

	public GameObject m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_TURN;

	public Text m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_TEXT_RED;

	public Text m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_TURN_COUNT_RED;

	public Animator m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_FX_RED;

	public GameObject m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_TURN_RED;

	public GameObject m_NUF_WARFARE_SUB_MENU_OPERATION_RECOVERY;

	public NKCUIComStateButton m_NUF_WARFARE_SUB_MENU_OPERATION_RECOVERY_btn;

	public GameObject m_NUF_WARFARE_SUB_MENU_OPERATION_RECOVERY_fx;

	public Text m_NUF_WARFARE_SUB_MENU_OPERATION_RECOVERY_text;

	[Header("중첩작전")]
	public GameObject m_objReadyMultiply;

	public NKCUIComToggle m_tgReadyMultiply;

	public NKCUIOperationMultiply m_NKCUIOperationMultiply;

	public GameObject m_objPlayingMultiply;

	public Text m_txtPlayingMultiply;

	private int m_CurrMultiplyRewardCount = 1;

	[Space]
	public GameObject m_ReadyTitle;

	public GameObject m_PlayTitle;

	[Header("페이즈")]
	public GameObject m_NUF_WARFARE_PHASE;

	public GameObject m_NUF_WARFARE_PHASE_USER;

	public GameObject m_NUF_WARFARE_PHASE_ENEMY;

	public Text m_NUF_WARFARE_PHASE_NUM_TEXT;

	[Header("우측 상단")]
	public RectTransform m_rtUpperRightMenuRoot;

	public Text m_NUF_WARFARE_PAUSE_GAME_TIME_Text;

	public GameObject m_NUF_WARFARE_AUTO;

	public GameObject m_NUF_WARFARE_AUTO_ON;

	public NKCUIComButton m_NUF_WARFARE_AUTO_ON_btn;

	public GameObject m_NUF_WARFARE_AUTO_OFF;

	public NKCUIComButton m_NUF_WARFARE_AUTO_OFF_Btn;

	public Text m_NUF_WARFARE_AUTO_OFF_Txt;

	public Image m_NUF_WARFARE_AUTO_OFF_Img;

	public GameObject m_NUF_WARFARE_REPEAT;

	public NKCUIComButton m_NUF_WARFARE_REPEAT_ON_btn;

	public NKCUIComButton m_NUF_WARFARE_REPEAT_OFF_Btn;

	public GameObject m_NUF_WARFARE_AUTO_SUPPLY;

	public NKCUIComButton m_NUF_WARFARE_AUTO_SUPPLY_OFF_btn;

	public NKCUIComButton m_NUF_WARFARE_AUTO_SUPPLY_ON_btn;

	[Space]
	public GameObject m_NUF_WARFARE_DECO;

	public GameObject m_NUF_WARFARE_PAUSE;

	public NKCUIComButton m_NUF_WARFARE_PAUSE_BUTTON;

	public GameObject m_NKM_UI_POPUP_WARFARE_PHASE_PLAYER;

	public GameObject m_NKM_UI_POPUP_WARFARE_PHASE_ENEMY;

	public GameObject m_NKM_UI_POPUP_WARFARE_PHASE_COMPLETE;

	public GameObject m_NKM_UI_POPUP_WARFARE_PHASE_FAIL;

	public NKCUIComResourceButton m_ResourceButton;

	public NKCUIComButton m_NUF_WARFARE_INFO_WARINFO;

	public GameObject m_NUF_WARFARE_INFO_MEDALINFO_DETAIL;

	public DOTweenAnimation m_NUF_WARFARE_INFO_MEDALINFO_DETAIL_DTA;

	public Text m_MEDALINFO_DETAIL_SLOT1_TEXT;

	public Text m_MEDALINFO_DETAIL_SLOT2_TEXT;

	public Text m_MEDALINFO_DETAIL_SLOT3_TEXT;

	public GameObject m_MEDALINFO_DETAIL_SLOT1_line;

	public GameObject m_MEDALINFO_DETAIL_SLOT2_line;

	public NKCUIComToggle m_NUF_WARFARE_INFO_MEDALINFO_toggle;

	[Header("승리패배 조건")]
	public GameObject m_NUF_WARFARE_INFO_VICTORY_1_ICON_TARGET;

	public GameObject m_NUF_WARFARE_INFO_VICTORY_1_ICON_WC_ENTER;

	public GameObject m_NUF_WARFARE_INFO_VICTORY_1_ICON_WC_HOLD;

	public Text m_NUF_WARFARE_INFO_VICTORY_1_TEXT;

	public GameObject m_NUF_WARFARE_INFO_VICTORY_1_ICON_WC_DEFENSE;

	public Text m_NUF_WARFARE_INFO_DEFEAT_TEXT;

	[Header("선택된 스쿼드")]
	public GameObject m_NUF_WARFARE_SELECTED_SQUAD;

	public Text m_NUF_WARFARE_SELECTED_SQUAD_Text1;

	public Text m_NUF_WARFARE_SELECTED_SQUAD_Text2;

	public NKCUIComStateButton m_NUF_WARFARE_SELECTED_SUPPLY;

	public NKCUIComStateButton m_NUF_WARFARE_SELECTED_REPAIR;

	public NKCUIComStateButton m_NUF_WARFARE_SELECTED_DETAIL;

	private NKMDeckIndex m_NKMDeckIndexSelected;

	public GameObject m_NUF_WARFARE_WAIT_BOX_Panel;

	public GameObject m_NUF_WARFARE_CONTAINER;

	public GameObject m_obj_container;

	public Text m_NUF_WARFARE_CONTAINER_COUNT;

	public GameObject m_NUF_WARFARE_CONTAINER_FX;

	private List<GameObject> m_moverPoolList = new List<GameObject>();

	private Coroutine m_moverCoroutine;

	private const int MOVER_POOL_COUNT = 2;

	private const float MOVER_TIME = 0.5f;

	private float m_fPrevUpdateTime;

	private OnStartUserPhase dOnStartUserPhase;

	private OnStartEnemyPhase dOnStartEnemyPhase;

	private NKCWarfareGame m_NKCWarfareGame;

	[Header("입장 제한")]
	public GameObject m_OPERATION_EnterLimit;

	public Text m_EnterLimit_TEXT;

	public Image m_OPERATION_START_ICON;

	public Text m_OPERATION_START_TEXT;

	public GameObject m_OPERATION_TITLE_BONUS;

	public Image m_BONUS_ICON;

	private int m_AttackCostItemCount;

	public GameObject m_NUF_DECK_WARFARE_OPERATION_TEXT_OPERATION_TITLE_BONUS;

	public Image m_NUF_DECK_WARFARE_OPERATION_TEXT_BONUS_ICON;

	private int m_iCurrentBatchedShipCnt;

	private bool m_bBatchedSupportShip;

	private List<NKCAssetInstanceData> m_listContainerMover = new List<NKCAssetInstanceData>();

	public int GetCurrMultiplyRewardCount()
	{
		return m_CurrMultiplyRewardCount;
	}

	public void InitUI(NKCWarfareGame cNKCWarfareGame, OnStartUserPhase onStartUserPhase, OnStartEnemyPhase onStartEnemyPhase)
	{
		m_NKCWarfareGame = cNKCWarfareGame;
		m_NUF_WARFARE = base.gameObject;
		m_NUF_WARFARE.SetActive(value: false);
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_INFO_MEDALINFO_DETAIL, bValue: true);
		m_NUF_WARFARE_INFO_MEDALINFO_DETAIL.transform.localScale = new Vector3(1f, 1f, 1f);
		if (m_ResourceButton != null)
		{
			m_ResourceButton.PointerClick.RemoveAllListeners();
			m_ResourceButton.PointerClick.AddListener(delegate
			{
				cNKCWarfareGame.OnClickGameStart();
			});
		}
		m_NUF_WARFARE_PAUSE_BUTTON.PointerClick.RemoveAllListeners();
		m_NUF_WARFARE_PAUSE_BUTTON.PointerClick.AddListener(cNKCWarfareGame.OnClickPause);
		m_NUF_WARFARE_AUTO_ON_btn.PointerClick.RemoveAllListeners();
		m_NUF_WARFARE_AUTO_ON_btn.PointerClick.AddListener(delegate
		{
			SendAutoReq(bAuto: false);
		});
		m_NUF_WARFARE_AUTO_OFF_Btn.PointerClick.RemoveAllListeners();
		m_NUF_WARFARE_AUTO_OFF_Btn.PointerClick.AddListener(delegate
		{
			SendAutoReq(bAuto: true);
		});
		m_NUF_WARFARE_REPEAT_ON_btn.PointerClick.RemoveAllListeners();
		m_NUF_WARFARE_REPEAT_ON_btn.PointerClick.AddListener(OnClickOperationRepeat);
		m_NUF_WARFARE_REPEAT_OFF_Btn.PointerClick.RemoveAllListeners();
		m_NUF_WARFARE_REPEAT_OFF_Btn.PointerClick.AddListener(OnClickOperationRepeat);
		m_NUF_WARFARE_AUTO_SUPPLY_OFF_btn.PointerClick.RemoveAllListeners();
		m_NUF_WARFARE_AUTO_SUPPLY_OFF_btn.PointerClick.AddListener(delegate
		{
			SendAutoSupplyReq(bAutoSupply: true);
		});
		m_NUF_WARFARE_AUTO_SUPPLY_ON_btn.PointerClick.RemoveAllListeners();
		m_NUF_WARFARE_AUTO_SUPPLY_ON_btn.PointerClick.AddListener(delegate
		{
			SendAutoSupplyReq(bAutoSupply: false);
		});
		m_NUF_WARFARE_INFO_WARINFO.PointerClick.RemoveAllListeners();
		m_NUF_WARFARE_INFO_WARINFO.PointerClick.AddListener(OpenWarfareInfoPopup);
		m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_BUTTON.PointerClick.RemoveAllListeners();
		m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_BUTTON.PointerClick.AddListener(cNKCWarfareGame.OnClickNextTurn);
		m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_BUTTON_RED.PointerClick.RemoveAllListeners();
		m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_BUTTON_RED.PointerClick.AddListener(cNKCWarfareGame.OnClickNextTurn);
		m_NUF_WARFARE_SELECTED_DETAIL.PointerClick.RemoveAllListeners();
		m_NUF_WARFARE_SELECTED_DETAIL.PointerClick.AddListener(cNKCWarfareGame.OnClickSquadInfo);
		m_NUF_WARFARE_SELECTED_SUPPLY.PointerClick.RemoveAllListeners();
		m_NUF_WARFARE_SELECTED_SUPPLY.PointerClick.AddListener(cNKCWarfareGame.UseSupplyItem);
		m_NUF_WARFARE_SELECTED_REPAIR.PointerClick.RemoveAllListeners();
		m_NUF_WARFARE_SELECTED_REPAIR.PointerClick.AddListener(cNKCWarfareGame.UseRepairItem);
		m_NUF_WARFARE_INFO_MEDALINFO_toggle.OnValueChanged.RemoveAllListeners();
		m_NUF_WARFARE_INFO_MEDALINFO_toggle.OnValueChanged.AddListener(OnClickMedalInfo);
		m_NUF_WARFARE_SUB_MENU_OPERATION_RECOVERY_btn.PointerClick.RemoveAllListeners();
		m_NUF_WARFARE_SUB_MENU_OPERATION_RECOVERY_btn.PointerClick.AddListener(cNKCWarfareGame.OnTouchRecoveryBtn);
		SetActiveRecovery(active: false);
		m_tgReadyMultiply.OnValueChanged.RemoveAllListeners();
		m_tgReadyMultiply.OnValueChanged.AddListener(OnClickMultiply);
		InitAniEvent(m_NKM_UI_POPUP_WARFARE_PHASE_PLAYER, OnCompleteUserPhaseAni);
		InitAniEvent(m_NKM_UI_POPUP_WARFARE_PHASE_ENEMY, OnCompleteEnemyPhaseAni);
		InitAniEvent(m_NKM_UI_POPUP_WARFARE_PHASE_COMPLETE, OnCompleteCompleteOrFailPhaseAni);
		InitAniEvent(m_NKM_UI_POPUP_WARFARE_PHASE_FAIL, OnCompleteCompleteOrFailPhaseAni);
		dOnStartUserPhase = onStartUserPhase;
		dOnStartEnemyPhase = onStartEnemyPhase;
		m_CurrMultiplyRewardCount = 1;
		m_NKCUIOperationMultiply.Init(OnOperationMultiplyUpdated, OnClickMultiplyRewardClose);
		NKCUtil.SetGameobjectActive(m_NKCUIOperationMultiply, bValue: false);
		NKCUtil.SetGameobjectActive(m_objReadyMultiply, bValue: false);
		NKCUtil.SetGameobjectActive(m_objPlayingMultiply, bValue: false);
	}

	public void TurnOffOperationMultiplyUI()
	{
		NKCUtil.SetGameobjectActive(m_NKCUIOperationMultiply, bValue: false);
	}

	private void OnClickOperationRepeat()
	{
		if (!NKCRepeatOperaion.CheckPossibleForWarfare(NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareStrID()) || NKCScenManager.GetScenManager().GetMyUserData() == null)
		{
			return;
		}
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData == null)
		{
			return;
		}
		if (warfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_STOP)
		{
			if (IsCanStartEterniumStage(bCallLackPopup: true))
			{
				m_tgReadyMultiply.Select(bSelect: false);
				if (m_NKCWarfareGame.GetNKCWarfareGameUnitMgr().GetCurrentUserUnit() == 0)
				{
					NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_WARFARE_CANNOT_START_BECAUSE_NO_USER_UNIT);
				}
				else
				{
					NKCPopupRepeatOperation.Instance.Open();
				}
			}
			return;
		}
		if (warfareGameData.rewardMultiply > 1)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString("SI_DP_DOUBLE_OPERATION_CANNOT_REPEAT"));
			return;
		}
		if (NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetRetryData() == null)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_WARFARE_CANNOT_FIND_RETRY_DATA);
			return;
		}
		if (!m_NKCWarfareGame.CheckEnablePause())
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_WARFARE_CANNOT_PAUSE);
			return;
		}
		m_NKCWarfareGame.SetPause(bSet: true);
		NKCPopupRepeatOperation.Instance.Open(delegate
		{
			if (m_NKCWarfareGame != null)
			{
				m_NKCWarfareGame.SetPause(bSet: false);
			}
		});
	}

	private bool IsCanStartEterniumStage(bool bCallLackPopup)
	{
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData != null && warfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_STOP)
		{
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(warfareGameData.warfareTempletID);
			if (nKMWarfareTemplet != null && nKMWarfareTemplet.StageTemplet != null)
			{
				return NKCUtil.IsCanStartEterniumStage(nKMWarfareTemplet.StageTemplet, bCallLackPopup);
			}
		}
		return true;
	}

	private void InitAniEvent(GameObject obj, UnityAction callback)
	{
		NKCUIComAniEventHandler component = obj.GetComponent<NKCUIComAniEventHandler>();
		if (component != null)
		{
			component.m_NKCUIComAniEvent.RemoveAllListeners();
			component.m_NKCUIComAniEvent.AddListener(callback);
		}
		EventTrigger component2 = obj.GetComponent<EventTrigger>();
		if (component2 != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(delegate
			{
				callback?.Invoke();
			});
			component2.triggers.Add(entry);
		}
	}

	public bool IsOpenSelectedSquadUI()
	{
		return m_NUF_WARFARE_SELECTED_SQUAD.activeSelf;
	}

	public NKMDeckIndex GetNKMDeckIndexSelected()
	{
		return m_NKMDeckIndexSelected;
	}

	public void OpenSelectedSquadUI(NKMDeckIndex sNKMDeckIndex, bool bRepair = false, bool bResupply = false)
	{
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_SELECTED_SQUAD, bValue: true);
		m_NKMDeckIndexSelected = sNKMDeckIndex;
		UpdateSelectedSquadUI(bRepair, bResupply);
		NKCOperatorUtil.PlayVoice(m_NKMDeckIndexSelected, VOICE_TYPE.VT_SHIP_SELECT);
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_OPERATION_TITLE, bValue: false);
	}

	public void UpdateSelectedSquadUI(bool bRepair = false, bool bResupply = false)
	{
		if (IsOpenSelectedSquadUI())
		{
			m_NUF_WARFARE_SELECTED_SQUAD_Text1.text = string.Format(NKCUtilString.GET_STRING_SQUAD_ONE_PARAM, NKCUtilString.GetDeckNumberString(m_NKMDeckIndexSelected));
			int num = m_NKMDeckIndexSelected.m_iIndex + 1;
			m_NUF_WARFARE_SELECTED_SQUAD_Text2.text = string.Format(NKCUtilString.GET_STRING_SQUAD_TWO_PARAM, num, NKCUtilString.GetRankNumber(num).ToUpper());
			NKCUtil.SetGameobjectActive(m_NUF_WARFARE_SELECTED_REPAIR, bRepair);
			NKCUtil.SetGameobjectActive(m_NUF_WARFARE_SELECTED_SUPPLY, bResupply);
		}
	}

	public void CloseSelectedSquadUI()
	{
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_SELECTED_SQUAD, bValue: false);
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_OPERATION_TITLE, bValue: true);
	}

	public void UpdateMedalInfo()
	{
		if (!m_NUF_WARFARE_INFO_MEDALINFO_DETAIL.activeSelf)
		{
			return;
		}
		m_MEDALINFO_DETAIL_SLOT1_TEXT.text = "";
		m_MEDALINFO_DETAIL_SLOT2_TEXT.text = "";
		m_MEDALINFO_DETAIL_SLOT3_TEXT.text = NKCUtilString.GetWFMissionText(WARFARE_GAME_MISSION_TYPE.WFMT_CLEAR, 0);
		if (NKCScenManager.GetScenManager().GetMyUserData() == null)
		{
			return;
		}
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData == null)
		{
			return;
		}
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(warfareGameData.warfareTempletID);
		if (nKMWarfareTemplet != null)
		{
			m_MEDALINFO_DETAIL_SLOT2_TEXT.text = NKCUtilString.GetWFMissionText(nKMWarfareTemplet.m_WFMissionType_1, nKMWarfareTemplet.m_WFMissionValue_1);
			m_MEDALINFO_DETAIL_SLOT1_TEXT.text = NKCUtilString.GetWFMissionText(nKMWarfareTemplet.m_WFMissionType_2, nKMWarfareTemplet.m_WFMissionValue_2);
			if (warfareGameData.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_STOP)
			{
				m_MEDALINFO_DETAIL_SLOT2_TEXT.text += GetCurrentStateOfWFMission(nKMWarfareTemplet.m_WFMissionType_1, nKMWarfareTemplet.m_WFMissionValue_1);
				m_MEDALINFO_DETAIL_SLOT1_TEXT.text += GetCurrentStateOfWFMission(nKMWarfareTemplet.m_WFMissionType_2, nKMWarfareTemplet.m_WFMissionValue_2);
				NKCUtil.SetGameobjectActive(m_MEDALINFO_DETAIL_SLOT2_line, IsFailWFMission(nKMWarfareTemplet.m_WFMissionType_1, nKMWarfareTemplet.m_WFMissionValue_1));
				NKCUtil.SetGameobjectActive(m_MEDALINFO_DETAIL_SLOT1_line, IsFailWFMission(nKMWarfareTemplet.m_WFMissionType_2, nKMWarfareTemplet.m_WFMissionValue_2));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_MEDALINFO_DETAIL_SLOT2_line, bValue: false);
				NKCUtil.SetGameobjectActive(m_MEDALINFO_DETAIL_SLOT1_line, bValue: false);
			}
		}
	}

	private string GetCurrentStateOfWFMission(WARFARE_GAME_MISSION_TYPE missionType, int value)
	{
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData == null)
		{
			return "";
		}
		return missionType switch
		{
			WARFARE_GAME_MISSION_TYPE.WFMT_PHASE => NKCUtilString.GetCurrentProgress(warfareGameData.turnCount, value), 
			WARFARE_GAME_MISSION_TYPE.WFMT_KILL => NKCUtilString.GetCurrentProgress(warfareGameData.enemiesKillCount, value), 
			WARFARE_GAME_MISSION_TYPE.WFMT_FIRST_ATTACK => NKCUtilString.GetCurrentProgress(warfareGameData.firstAttackCount, value), 
			WARFARE_GAME_MISSION_TYPE.WFMT_ASSIST => NKCUtilString.GetCurrentProgress(warfareGameData.assistCount, value), 
			WARFARE_GAME_MISSION_TYPE.WFMT_CONTAINER => NKCUtilString.GetCurrentProgress(warfareGameData.containerCount, value), 
			_ => "", 
		};
	}

	private bool IsFailWFMission(WARFARE_GAME_MISSION_TYPE missionType, int value)
	{
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData == null)
		{
			return false;
		}
		switch (missionType)
		{
		case WARFARE_GAME_MISSION_TYPE.WFMT_PHASE:
			if (warfareGameData.turnCount <= value)
			{
				return warfareGameData.alliesKillCount > 0;
			}
			return true;
		case WARFARE_GAME_MISSION_TYPE.WFMT_ALLKILL:
			return warfareGameData.alliesKillCount > 0;
		case WARFARE_GAME_MISSION_TYPE.WFMT_NO_SHIPWRECK:
			return warfareGameData.alliesKillCount > 0;
		case WARFARE_GAME_MISSION_TYPE.WFMT_NOSUPPLY_WIN:
		case WARFARE_GAME_MISSION_TYPE.WFMT_NOSUPPLY_ALLKILL:
			return warfareGameData.supplyUseCount > 0;
		default:
			return false;
		}
	}

	public void UpdateWinCondition()
	{
		if (!m_NUF_WARFARE_INFO_MEDALINFO_DETAIL.activeSelf || NKCScenManager.GetScenManager().GetMyUserData() == null)
		{
			return;
		}
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData == null)
		{
			return;
		}
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(warfareGameData.warfareTempletID);
		if (nKMWarfareTemplet != null)
		{
			m_NUF_WARFARE_INFO_VICTORY_1_TEXT.text = NKCUtilString.GetWFWinContionText(nKMWarfareTemplet.m_WFWinCondition);
			m_NUF_WARFARE_INFO_DEFEAT_TEXT.text = NKCUtilString.GetWFLoseConditionText(nKMWarfareTemplet.m_WFLoseCondition);
			NKCUtil.SetGameobjectActive(m_NUF_WARFARE_INFO_VICTORY_1_ICON_TARGET, nKMWarfareTemplet.m_WFWinCondition == WARFARE_GAME_CONDITION.WFC_KILL_TARGET);
			NKCUtil.SetGameobjectActive(m_NUF_WARFARE_INFO_VICTORY_1_ICON_WC_ENTER, nKMWarfareTemplet.m_WFWinCondition == WARFARE_GAME_CONDITION.WFC_TILE_ENTER);
			NKCUtil.SetGameobjectActive(m_NUF_WARFARE_INFO_VICTORY_1_ICON_WC_HOLD, nKMWarfareTemplet.m_WFWinCondition == WARFARE_GAME_CONDITION.WFC_PHASE_TILE_HOLD);
			NKCUtil.SetGameobjectActive(m_NUF_WARFARE_INFO_VICTORY_1_ICON_WC_DEFENSE, nKMWarfareTemplet.m_WFLoseCondition == WARFARE_GAME_CONDITION.WFC_TILE_ENTER);
			bool bPlay = warfareGameData.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_STOP;
			m_NUF_WARFARE_INFO_VICTORY_1_TEXT.text += GetCurrentWinCondition(bPlay, nKMWarfareTemplet.m_WFWinCondition, nKMWarfareTemplet.m_WFWinValue);
			m_NUF_WARFARE_INFO_DEFEAT_TEXT.text += GetCurrentLoseCondition(bPlay, nKMWarfareTemplet.m_WFLoseCondition, nKMWarfareTemplet.m_WFLoseValue);
			bool activeTurnFinishWarningBtn = false;
			if (nKMWarfareTemplet.m_WFLoseCondition == WARFARE_GAME_CONDITION.WFC_PHASE && warfareGameData.turnCount >= nKMWarfareTemplet.m_WFLoseValue)
			{
				activeTurnFinishWarningBtn = true;
			}
			SetActiveTurnFinishWarningBtn(activeTurnFinishWarningBtn);
		}
	}

	private string GetCurrentWinCondition(bool bPlay, WARFARE_GAME_CONDITION winCondition, int value)
	{
		if (!bPlay)
		{
			return "";
		}
		if (NKCScenManager.GetScenManager().GetMyUserData() == null)
		{
			return "";
		}
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData == null)
		{
			return "";
		}
		switch (winCondition)
		{
		case WARFARE_GAME_CONDITION.WFC_KILL_TARGET:
			return NKCUtilString.GetCurrentProgress(warfareGameData.targetKillCount, value);
		case WARFARE_GAME_CONDITION.WFC_PHASE_TILE_HOLD:
		{
			int holdCount = warfareGameData.holdCount;
			return NKCUtilString.GetCurrentProgress(value - holdCount, value);
		}
		default:
			return "";
		}
	}

	private string GetCurrentLoseCondition(bool bPlay, WARFARE_GAME_CONDITION loseCondition, int value)
	{
		if (NKCScenManager.GetScenManager().GetMyUserData() == null)
		{
			return "";
		}
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData == null)
		{
			return "";
		}
		if (loseCondition == WARFARE_GAME_CONDITION.WFC_PHASE)
		{
			if (bPlay)
			{
				return NKCUtilString.GetCurrentProgress(warfareGameData.turnCount, value);
			}
			return NKCUtilString.GetCurrentProgress(0, value);
		}
		if (bPlay && loseCondition == WARFARE_GAME_CONDITION.WFC_KILL_COUNT)
		{
			return NKCUtilString.GetCurrentProgress(warfareGameData.alliesKillCount, value);
		}
		return "";
	}

	public void OnClickMedalInfo(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_INFO_MEDALINFO_DETAIL, bSet);
		if (bSet)
		{
			m_NUF_WARFARE_INFO_MEDALINFO_DETAIL_DTA.DORestart();
			UpdateMedalInfo();
		}
	}

	public void OpenWarfareInfoPopup()
	{
		NKCPopupWarfareInfo.Instance.Open();
	}

	public void SetUpperRightMenuPosition(bool bPlaying)
	{
		if (m_rtUpperRightMenuRoot != null)
		{
			m_rtUpperRightMenuRoot.anchoredPosition = new Vector2(m_rtUpperRightMenuRoot.anchoredPosition.x, bPlaying ? 0f : (-85.88f));
		}
	}

	public void SetPauseState(bool bSet)
	{
		if (m_NKM_UI_POPUP_WARFARE_PHASE_PLAYER != null)
		{
			m_NKM_UI_POPUP_WARFARE_PHASE_PLAYER.GetComponent<Animator>().enabled = !bSet;
		}
		if (m_NKM_UI_POPUP_WARFARE_PHASE_ENEMY != null)
		{
			m_NKM_UI_POPUP_WARFARE_PHASE_ENEMY.GetComponent<Animator>().enabled = !bSet;
		}
		if (m_NKM_UI_POPUP_WARFARE_PHASE_COMPLETE != null)
		{
			m_NKM_UI_POPUP_WARFARE_PHASE_COMPLETE.GetComponent<Animator>().enabled = !bSet;
		}
		if (m_NKM_UI_POPUP_WARFARE_PHASE_FAIL != null)
		{
			m_NKM_UI_POPUP_WARFARE_PHASE_FAIL.GetComponent<Animator>().enabled = !bSet;
		}
	}

	public void SetTurnCount(int count)
	{
		m_NUF_WARFARE_PHASE_NUM_TEXT.text = $"{count:00}";
	}

	public void SetAttackCost(int itemID, int itemCount)
	{
		m_AttackCostItemCount = itemCount;
		if (m_ResourceButton != null)
		{
			m_ResourceButton.SetData(itemID, m_CurrMultiplyRewardCount * m_AttackCostItemCount);
		}
	}

	private void UpdateAttckCostUI()
	{
		if (m_ResourceButton != null && m_ResourceButton.GetItemID() != 0)
		{
			m_ResourceButton.SetData(m_ResourceButton.GetItemID(), m_CurrMultiplyRewardCount * m_AttackCostItemCount);
		}
	}

	public void SetPhaseUserType(bool bUser)
	{
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_PHASE_USER, bUser);
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_PHASE_ENEMY, !bUser);
	}

	public void DeActivateAllTriggerUI()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_WARFARE_PHASE_ENEMY, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_WARFARE_PHASE_PLAYER, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_WARFARE_PHASE_COMPLETE, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_WARFARE_PHASE_FAIL, bValue: false);
	}

	public void TriggerPlayerTurnUI()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_WARFARE_PHASE_ENEMY, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_WARFARE_PHASE_PLAYER, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_WARFARE_PHASE_PLAYER, bValue: true);
	}

	public void TriggerEnemyTurnUI()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_WARFARE_PHASE_PLAYER, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_WARFARE_PHASE_ENEMY, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_WARFARE_PHASE_ENEMY, bValue: true);
	}

	public void TriggerCompleteUI()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_WARFARE_PHASE_COMPLETE, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_WARFARE_PHASE_COMPLETE, bValue: true);
	}

	public void TriggerFailUI()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_WARFARE_PHASE_FAIL, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_WARFARE_PHASE_FAIL, bValue: true);
	}

	public bool CheckVisibleWarfareStateEffectUI()
	{
		if (m_NKM_UI_POPUP_WARFARE_PHASE_ENEMY.activeSelf || m_NKM_UI_POPUP_WARFARE_PHASE_PLAYER.activeSelf || m_NKM_UI_POPUP_WARFARE_PHASE_COMPLETE.activeSelf || m_NKM_UI_POPUP_WARFARE_PHASE_FAIL.activeSelf)
		{
			return true;
		}
		return false;
	}

	public void SetActiveTurnFinishBtn(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT, bSet);
	}

	private void SetActiveTurnFinishWarningBtn(bool bWarning)
	{
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_TURN, !bWarning);
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_TURN_RED, bWarning);
	}

	public void SetRemainTurnOnUnitCount(int count)
	{
		SetRemainTurnOnUnitCountNormal(count);
		SetRemainTurnOnUnitCountRed(count);
	}

	private void SetRemainTurnOnUnitCountNormal(int count)
	{
		if (m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_TURN_COUNT != null)
		{
			m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_TURN_COUNT.text = count.ToString();
		}
		if (!(m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_TEXT != null))
		{
			return;
		}
		if (count <= 0)
		{
			m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_TEXT.text = NKCUtilString.GET_STRING_WARFARE_PHASE_FINISH;
			if (m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT.activeSelf)
			{
				if (NKCScenManager.GetScenManager().WarfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_PLAYING)
				{
					m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_FX.Play("BASE_READY");
				}
				else
				{
					m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_FX.Play("BASE");
				}
			}
		}
		else
		{
			m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_TEXT.text = NKCUtilString.GET_STRING_WARFARE_PHASE_FINISH;
			if (m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT.activeSelf)
			{
				m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_FX.Play("BASE");
			}
		}
	}

	public void SetRemainTurnOnUnitCountRed(int count)
	{
		if (!m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_TURN_RED.activeSelf)
		{
			return;
		}
		if (m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_TURN_COUNT_RED != null)
		{
			m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_TURN_COUNT_RED.text = count.ToString();
		}
		if (!(m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_TEXT_RED != null))
		{
			return;
		}
		if (count <= 0)
		{
			m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_TEXT_RED.text = NKCUtilString.GET_STRING_WARFARE_PHASE_FINISH;
			if (m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT.activeSelf)
			{
				if (NKCScenManager.GetScenManager().WarfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_PLAYING)
				{
					m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_FX_RED.Play("BASE_READY");
				}
				else
				{
					m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_FX_RED.Play("BASE");
				}
			}
		}
		else
		{
			m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_TEXT_RED.text = NKCUtilString.GET_STRING_WARFARE_PHASE_FINISH;
			if (m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT.activeSelf)
			{
				m_NUF_WARFARE_SUB_MENU_OPERATION_NEXT_FX_RED.Play("BASE");
			}
		}
	}

	public void SetActiveBatchCountUI(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_SHIP_NUM, bSet);
		SetActiveSupportBatchCountUI(bSet);
	}

	private void SetActiveSupportBatchCountUI(bool bSet)
	{
		bool bValue = false;
		if (bSet)
		{
			bValue = IsCanSpawnSupportShip();
		}
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_SHIP_NUM_SUPPORT, bValue);
	}

	public void SetActiveTitle(bool bReadyTitle)
	{
		NKCUtil.SetGameobjectActive(m_ReadyTitle, bReadyTitle);
		NKCUtil.SetGameobjectActive(m_PlayTitle, !bReadyTitle);
	}

	public void SetActiveBatchGuideText(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_SUB_MENU_OPERATION_TEXT, bSet);
	}

	public void SetActiveBatchSupportGuideText(bool bSet)
	{
		bool bValue = false;
		if (bSet)
		{
			bValue = IsCanSpawnSupportShip();
		}
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_SUB_MENU_OPERATION_SUPPORT_TEXT, bValue);
	}

	private bool IsCanSpawnSupportShip()
	{
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData == null)
		{
			return false;
		}
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(warfareGameData.warfareTempletID);
		if (nKMWarfareTemplet == null)
		{
			return false;
		}
		NKMWarfareMapTemplet mapTemplet = nKMWarfareTemplet.MapTemplet;
		if (mapTemplet == null)
		{
			return false;
		}
		int spawnPointCountByType = mapTemplet.GetSpawnPointCountByType(NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_DIVE);
		int spawnPointCountByType2 = mapTemplet.GetSpawnPointCountByType(NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_ASSAULT);
		if (nKMWarfareTemplet.m_bFriendSummon)
		{
			return spawnPointCountByType + spawnPointCountByType2 > 1;
		}
		return false;
	}

	public void SetActiveOperationBtn(bool bSet)
	{
		if (NKCScenManager.CurrentUserData() != null)
		{
			WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
			if (warfareGameData != null && NKMWarfareTemplet.Find(warfareGameData.warfareTempletID) != null)
			{
				NKCUtil.SetGameobjectActive(m_NUF_WARFARE_SUB_MENU_OPERATION_BUTTON, bSet);
			}
		}
	}

	public void SetActiveDeco(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_DECO, bSet);
	}

	public void SetActiveAuto(bool bVisible, bool bUsable)
	{
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_AUTO, bVisible);
		if (bVisible)
		{
			string text = "#FFFFFF";
			string text2 = "#7B7B7B";
			m_NUF_WARFARE_AUTO_OFF_Txt.color = NKCUtil.GetColor(bUsable ? text : text2);
			m_NUF_WARFARE_AUTO_OFF_Img.color = NKCUtil.GetColor(bUsable ? text : text2);
		}
	}

	public void SetActiveAutoOnOff(bool bOn, bool bAutoSupply)
	{
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_AUTO_ON, bOn);
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_AUTO_OFF, !bOn);
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_AUTO_SUPPLY, bOn);
		if (bOn)
		{
			NKCUtil.SetGameobjectActive(m_NUF_WARFARE_AUTO_SUPPLY_ON_btn, bAutoSupply);
			NKCUtil.SetGameobjectActive(m_NUF_WARFARE_AUTO_SUPPLY_OFF_btn, !bAutoSupply);
		}
	}

	public void SetActiveRepeatOperation(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_REPEAT, bSet);
	}

	public void SetActiveRepeatOperationOnOff(bool bOn)
	{
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_REPEAT_ON_btn, bOn);
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_REPEAT_OFF_Btn, !bOn);
	}

	public void SendAutoReq(bool bAuto)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMPacket_WARFARE_GAME_AUTO_REQ(bAuto, myUserData.m_UserOption.m_bAutoWarfareRepair);
	}

	public void SendAutoSupplyReq(bool bAutoSupply)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (!myUserData.m_UserOption.m_bAutoWarfare)
		{
			Debug.LogError("warfare - auto가 아닌데 autoSupply가 들어옴");
		}
		else
		{
			NKMPacket_WARFARE_GAME_AUTO_REQ(myUserData.m_UserOption.m_bAutoWarfare, bAutoSupply);
		}
	}

	public void NKMPacket_WARFARE_GAME_AUTO_REQ(bool bAuto, bool bAutoSupply)
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.WARFARE_AUTO_MOVE))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.WARFARE_AUTO_MOVE);
		}
		else if (!NKCGameEventManager.IsWaiting())
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			NKMWarfareTemplet.Find(m_WarfareStrID);
			if (NKCWarfareManager.CheckPossibleAuto(myUserData, bAuto, bAutoSupply) == NKM_ERROR_CODE.NEC_OK && !NKMPopUpBox.IsOpenedWaitBox())
			{
				Debug.Log("NKMPacket_WARFARE_GAME_AUTO_REQ - NKCWarfareGameHUD");
				m_NKCWarfareGame.SetPause(bSet: true);
				m_NKCWarfareGame.WaitAutoPacket = true;
				NKCPacketSender.Send_NKMPacket_WARFARE_GAME_AUTO_REQ(bAuto, bAutoSupply);
			}
		}
	}

	public void SetActivePhase(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_PHASE, bSet);
	}

	public void SetBatchedShipCount(int count)
	{
		int num = -1;
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet != null)
		{
			num = nKMWarfareTemplet.m_UserTeamCount;
		}
		m_iCurrentBatchedShipCnt = count;
		if (m_iCurrentBatchedShipCnt <= 0 && m_tgReadyMultiply.m_bSelect)
		{
			m_tgReadyMultiply.Select(bSelect: false);
		}
		SetActiveOperationBtn(m_iCurrentBatchedShipCnt > 0);
		m_NUF_WARFARE_SHIP_NUM_TEXT.text = $"{m_iCurrentBatchedShipCnt}/{num}";
		if (m_iCurrentBatchedShipCnt >= num)
		{
			if (m_NUF_WARFARE_SUB_MENU_OPERATION_BUTTON.activeSelf)
			{
				NKCUtil.SetGameobjectActive(m_NUF_WARFARE_SUB_MENU_OPERATION_BUTTON_FX.gameObject, bValue: true);
				m_NUF_WARFARE_SUB_MENU_OPERATION_BUTTON_FX.Play("BASE_READY");
			}
		}
		else
		{
			m_NUF_WARFARE_SHIP_NUM_TEXT.color = new Color(1f, 1f, 1f);
			if (m_NUF_WARFARE_SUB_MENU_OPERATION_BUTTON_FX.gameObject.activeInHierarchy)
			{
				NKCUtil.SetGameobjectActive(m_NUF_WARFARE_SUB_MENU_OPERATION_BUTTON_FX.gameObject, bValue: false);
			}
		}
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_SUB_MENU_OPERATION_TEXT, count > 0 && count < num);
		NKCUtil.SetLabelTextColor(m_EnterLimit_TEXT, Color.white);
		if (count > 0)
		{
			bool bValue = false;
			if (nKMWarfareTemplet != null && nKMWarfareTemplet.StageTemplet != null)
			{
				bValue = nKMWarfareTemplet.StageTemplet.EnterLimit > 0;
			}
			UpdateStagePlayState();
			NKCUtil.SetGameobjectActive(m_OPERATION_EnterLimit, bValue);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_OPERATION_EnterLimit, bValue: false);
		}
	}

	public void UpdateSupportShipTile(List<int> userUnitTileIndex)
	{
		if (!IsCanSpawnSupportShip() || userUnitTileIndex.Count <= 0 || m_bBatchedSupportShip)
		{
			return;
		}
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData == null)
		{
			return;
		}
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(warfareGameData.warfareTempletID);
		if (nKMWarfareTemplet == null)
		{
			return;
		}
		NKMWarfareMapTemplet mapTemplet = nKMWarfareTemplet.MapTemplet;
		if (mapTemplet == null)
		{
			return;
		}
		int divePointTileCount = mapTemplet.GetDivePointTileCount();
		bool activeBatchSupportGuideText = false;
		for (int i = 0; i < divePointTileCount; i++)
		{
			int divePointTileIndex = mapTemplet.GetDivePointTileIndex(i);
			if (divePointTileIndex >= 0 && !userUnitTileIndex.Contains(divePointTileIndex))
			{
				activeBatchSupportGuideText = true;
				break;
			}
		}
		SetActiveBatchSupportGuideText(activeBatchSupportGuideText);
	}

	public void SetBatchedSupportShipCount(bool bSet)
	{
		if (bSet)
		{
			NKCUtil.SetLabelText(m_NUF_WARFARE_SHIP_NUM_SUPPORT_TEXT, "1/1");
		}
		else
		{
			NKCUtil.SetLabelText(m_NUF_WARFARE_SHIP_NUM_SUPPORT_TEXT, "0/1");
		}
		m_bBatchedSupportShip = bSet;
		SetActiveBatchSupportGuideText(!bSet);
	}

	private void ConfirmResetStagePlayCnt()
	{
		NKMWarfareTemplet cNKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (cNKMWarfareTemplet == null || cNKMWarfareTemplet.StageTemplet == null)
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		int num = 0;
		if (nKMUserData != null)
		{
			num = nKMUserData.GetStageRestoreCnt(cNKMWarfareTemplet.StageTemplet.Key);
		}
		if (!cNKMWarfareTemplet.StageTemplet.Restorable)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_ENTER_LIMIT_OVER);
			return;
		}
		if (num >= cNKMWarfareTemplet.StageTemplet.RestoreLimit)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_WARFARE_GAEM_HUD_RESTORE_LIMIT_OVER_DESC);
			return;
		}
		NKCPopupResourceWithdraw.Instance.OpenForRestoreEnterLimit(cNKMWarfareTemplet.StageTemplet, delegate
		{
			NKCPacketSender.Send_NKMPacket_RESET_STAGE_PLAY_COUNT_REQ(cNKMWarfareTemplet.StageTemplet.Key);
		}, num);
	}

	public void SetActivePause(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_PAUSE, bSet);
	}

	public void OnCompleteCompleteOrFailPhaseAni()
	{
		DeActivateAllTriggerUI();
		if (NKCScenManager.GetScenManager().GetMyUserData() != null)
		{
			WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
			if (warfareGameData != null && warfareGameData.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_STOP)
			{
				SendGetNextOrderREQ();
			}
		}
	}

	public void OnCompleteUserPhaseAni()
	{
		DeActivateAllTriggerUI();
		dOnStartUserPhase?.Invoke();
	}

	public void OnCompleteEnemyPhaseAni()
	{
		DeActivateAllTriggerUI();
		dOnStartEnemyPhase?.Invoke();
	}

	private void SendGetNextOrderREQ()
	{
		if (NKCWarfareManager.CheckGetNextOrderCond(NKCScenManager.GetScenManager().GetMyUserData()) == NKM_ERROR_CODE.NEC_OK)
		{
			m_NKCWarfareGame.SetPause(bSet: true);
			NKCPacketSender.Send_NKMPacket_WARFARE_GAME_NEXT_ORDER_REQ();
		}
	}

	public void SetWarfareStrID(string warfareStrID)
	{
		m_WarfareStrID = warfareStrID;
	}

	public void Open()
	{
		m_CurrMultiplyRewardCount = 1;
		if (!m_NUF_WARFARE.activeSelf)
		{
			SetWaitBox(bSet: false);
			m_NUF_WARFARE.SetActive(value: true);
			ResetUI();
		}
	}

	private void ResetUI()
	{
		if (m_WarfareStrID == "")
		{
			return;
		}
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet == null)
		{
			return;
		}
		NKMStageTempletV2 nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(m_WarfareStrID);
		if (nKMStageTempletV != null)
		{
			NKMEpisodeTempletV2 episodeTemplet = nKMStageTempletV.EpisodeTemplet;
			if (episodeTemplet != null)
			{
				m_OPERATION_TITLE_EP_TEXT.text = episodeTemplet.GetEpisodeTitle();
				m_OPERATION_TITLE_EP_TEXT_FOR_PLAY.text = episodeTemplet.GetEpisodeTitle();
			}
			m_OPERATION_TITLE_TEXT.text = $"{nKMStageTempletV.ActId}-{nKMStageTempletV.m_StageUINum} {nKMWarfareTemplet.GetWarfareName()}";
			m_OPERATION_TITLE_TEXT_FOR_PLAY.text = $"{nKMStageTempletV.ActId}-{nKMStageTempletV.m_StageUINum} {nKMWarfareTemplet.GetWarfareName()}";
			if (nKMStageTempletV.m_BuffType.Equals(RewardTuningType.None))
			{
				NKCUtil.SetGameobjectActive(m_OPERATION_TITLE_BONUS, bValue: false);
				NKCUtil.SetGameobjectActive(m_NUF_DECK_WARFARE_OPERATION_TEXT_OPERATION_TITLE_BONUS, bValue: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_OPERATION_TITLE_BONUS, bValue: true);
				NKCUtil.SetImageSprite(m_BONUS_ICON, NKCUtil.GetBounsTypeIcon(nKMStageTempletV.m_BuffType, big: false));
				NKCUtil.SetGameobjectActive(m_NUF_DECK_WARFARE_OPERATION_TEXT_OPERATION_TITLE_BONUS, bValue: true);
				NKCUtil.SetImageSprite(m_NUF_DECK_WARFARE_OPERATION_TEXT_BONUS_ICON, NKCUtil.GetBounsTypeIcon(nKMStageTempletV.m_BuffType, big: false));
			}
			UpdateStagePlayState();
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_OPERATION_TITLE_BOUNS.gameObject, bValue: false);
		}
		SetActiveRepeatOperationOnOff(NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing());
		InitContainerWhenOpen();
		UpdateMultiplyUI();
		SetOperationMultiplyData();
		NKCUtil.SetGameobjectActive(m_NKCUIOperationMultiply, bValue: false);
	}

	public void UpdateStagePlayState()
	{
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet == null)
		{
			return;
		}
		NKMStageTempletV2 stageTemplet = nKMWarfareTemplet.StageTemplet;
		if (stageTemplet == null || stageTemplet.EnterLimit <= 0)
		{
			return;
		}
		int statePlayCnt = NKCScenManager.CurrentUserData().GetStatePlayCnt(stageTemplet.Key);
		string text = "";
		NKCUtil.SetLabelText(msg: stageTemplet.EnterLimitCond switch
		{
			NKMStageTempletV2.RESET_TYPE.DAY => string.Format(NKCUtilString.GET_STRING_POPUP_DUNGEON_ENTER_LIMIT_DESC_DAY_02, stageTemplet.EnterLimit - statePlayCnt, stageTemplet.EnterLimit), 
			NKMStageTempletV2.RESET_TYPE.MONTH => string.Format(NKCUtilString.GET_STRING_POPUP_DUNGEON_ENTER_LIMIT_DESC_MONTH_02, stageTemplet.EnterLimit - statePlayCnt, stageTemplet.EnterLimit), 
			NKMStageTempletV2.RESET_TYPE.WEEK => string.Format(NKCUtilString.GET_STRING_POPUP_DUNGEON_ENTER_LIMIT_DESC_WEEK_02, stageTemplet.EnterLimit - statePlayCnt, stageTemplet.EnterLimit), 
			_ => string.Format(NKCUtilString.GET_STRING_POPUP_DUNGEON_ENTER_LIMIT_DESC_DAY_02, stageTemplet.EnterLimit - statePlayCnt, stageTemplet.EnterLimit), 
		}, label: m_EnterLimit_TEXT);
		if (stageTemplet.EnterLimit - statePlayCnt <= 0)
		{
			NKCUtil.SetLabelTextColor(m_EnterLimit_TEXT, Color.red);
			NKCUtil.SetBindFunction(m_ResourceButton, ConfirmResetStagePlayCnt);
			NKCUtil.SetLabelText(m_OPERATION_START_TEXT, NKCUtilString.GET_STRING_WARFARE_GAME_HUD_OPERATION_RESTORE);
			NKCUtil.SetImageSprite(m_OPERATION_START_ICON, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_ICON_ENTERLIMIT_RECOVER_SMALL"));
			return;
		}
		NKCUtil.SetLabelTextColor(m_EnterLimit_TEXT, Color.white);
		NKCUtil.SetLabelText(m_OPERATION_START_TEXT, NKCUtilString.GET_STRING_WARFARE_GAME_HUD_OPERATION_START);
		NKCUtil.SetBindFunction(m_ResourceButton, delegate
		{
			m_NKCWarfareGame.OnClickGameStart();
		});
		NKCUtil.SetImageSprite(m_OPERATION_START_ICON, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_ICON_GAUNTLET"));
	}

	public void SelfUpdateAttackCost()
	{
		int itemID = 0;
		int itemCount = 0;
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet.StageTemplet != null && nKMWarfareTemplet.StageTemplet.EnterLimit > 0)
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				int statePlayCnt = nKMUserData.GetStatePlayCnt(nKMWarfareTemplet.StageTemplet.Key);
				int enterLimit = nKMWarfareTemplet.StageTemplet.EnterLimit;
				if (statePlayCnt >= enterLimit && nKMWarfareTemplet.StageTemplet.RestoreLimit > 0)
				{
					itemID = nKMWarfareTemplet.StageTemplet.RestoreReqItem.ItemId;
					itemCount = nKMWarfareTemplet.StageTemplet.RestoreReqItem.Count32;
				}
			}
		}
		if (itemID == 0 || itemCount == 0)
		{
			NKCWarfareManager.GetCurrWarfareAttackCost(out itemID, out itemCount);
		}
		SetAttackCost(itemID, itemCount);
	}

	public void SetWaitBox(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_WAIT_BOX_Panel, bSet);
	}

	public void SetActiveContainer(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_obj_container, bSet);
	}

	public void SetContainerCount(int count)
	{
		if (count <= 0)
		{
			if (m_obj_container.activeSelf)
			{
				SetActiveContainer(bSet: false);
			}
			return;
		}
		if (!m_obj_container.activeSelf)
		{
			SetActiveContainer(bSet: true);
		}
		m_NUF_WARFARE_CONTAINER_COUNT.text = string.Format(NKCUtilString.GET_STRING_COUNTING_ONE_PARAM, count.ToString());
	}

	public void InitContainerWhenOpen()
	{
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_CONTAINER_FX, bValue: false);
		for (int i = 0; i < 2; i++)
		{
			NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_WARFARE", "NUM_WARFARE_CONTAINER_MOVER");
			GameObject instant = nKCAssetInstanceData.m_Instant;
			m_listContainerMover.Add(nKCAssetInstanceData);
			if (instant != null)
			{
				instant.transform.SetParent(m_NUF_WARFARE_CONTAINER.transform);
				instant.SetActive(value: false);
				m_moverPoolList.Add(instant);
			}
		}
	}

	public void PlayGetContainer(Vector3 itemPos, int count)
	{
		if (m_moverPoolList.Count == 0)
		{
			return;
		}
		GameObject gameObject = m_moverPoolList[0];
		for (int i = 0; i < m_moverPoolList.Count; i++)
		{
			GameObject gameObject2 = m_moverPoolList[i];
			if (!gameObject2.activeSelf)
			{
				gameObject = gameObject2;
				break;
			}
		}
		gameObject.transform.position = new Vector3(itemPos.x, itemPos.y, 0f);
		m_moverCoroutine = StartCoroutine(moveContainer(gameObject, count));
	}

	private IEnumerator moveContainer(GameObject obj, int count)
	{
		obj.SetActive(value: true);
		Vector3 beginPos = obj.transform.localPosition;
		Vector3 endPos = m_obj_container.transform.localPosition;
		float deltaTime = 0f;
		for (deltaTime += Time.deltaTime; deltaTime < 0.5f; deltaTime += Time.deltaTime)
		{
			float progress = NKMTrackingFloat.TrackRatio(TRACKING_DATA_TYPE.TDT_SLOWER, deltaTime, 0.5f);
			obj.transform.localPosition = NKCUtil.Lerp(beginPos, endPos, progress);
			yield return null;
		}
		obj.transform.localPosition = endPos;
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_CONTAINER_FX, bValue: false);
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_CONTAINER_FX, bValue: true);
		SetContainerCount(count);
		obj.SetActive(value: false);
	}

	public void SetActiveRecovery(bool active)
	{
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_SUB_MENU_OPERATION_RECOVERY, active);
		if (!active)
		{
			SetRecoveryFx(active: false);
		}
	}

	public void SetRecoveryFx(bool active)
	{
		NKCUtil.SetGameobjectActive(m_NUF_WARFARE_SUB_MENU_OPERATION_RECOVERY_fx, active);
	}

	public void SetRecoveryCount(int count)
	{
		string msg = string.Format(NKCUtilString.GET_STRING_WARFARE_RECOVERY_COUNT_ONE_PARAM, count);
		NKCUtil.SetLabelText(m_NUF_WARFARE_SUB_MENU_OPERATION_RECOVERY_text, msg);
	}

	public void Close()
	{
		SetWaitBox(bSet: false);
		if (m_NUF_WARFARE.activeSelf)
		{
			m_NUF_WARFARE.SetActive(value: false);
		}
		if (m_moverCoroutine != null)
		{
			StopCoroutine(m_moverCoroutine);
			m_moverCoroutine = null;
		}
		m_moverPoolList.Clear();
		for (int i = 0; i < m_listContainerMover.Count; i++)
		{
			NKCAssetResourceManager.CloseInstance(m_listContainerMover[i]);
		}
		m_listContainerMover.Clear();
		NKCPopupWarfareInfo.CheckInstanceAndClose();
	}

	private void Update()
	{
		if (!(m_fPrevUpdateTime < Time.time + 1f))
		{
			return;
		}
		m_fPrevUpdateTime = Time.time;
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData != null)
		{
			if (warfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_STOP)
			{
				TimeSpan timeSpan = new TimeSpan(12, 0, 0);
				m_NUF_WARFARE_PAUSE_GAME_TIME_Text.text = NKCUtilString.GetTimeSpanString(timeSpan);
			}
			else
			{
				DateTime serverUTCTime = NKCSynchronizedTime.GetServerUTCTime();
				TimeSpan timeSpan2 = new DateTime(warfareGameData.expireTimeTick) - serverUTCTime;
				m_NUF_WARFARE_PAUSE_GAME_TIME_Text.text = NKCUtilString.GetTimeSpanString(timeSpan2);
			}
		}
	}

	public void HideOperationEnterLimit()
	{
		NKCUtil.SetGameobjectActive(m_OPERATION_EnterLimit, bValue: false);
	}

	private void OnOperationMultiplyUpdated(int newCount)
	{
		m_CurrMultiplyRewardCount = newCount;
		UpdateAttckCostUI();
	}

	private void OnClickMultiplyRewardClose()
	{
		m_tgReadyMultiply.Select(bSelect: false);
	}

	private void SetOperationMultiplyData()
	{
		NKMRewardMultiplyTemplet.RewardMultiplyItem costItem = NKMRewardMultiplyTemplet.GetCostItem(NKMRewardMultiplyTemplet.ScopeType.General);
		int num = 99;
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		NKMWarfareTemplet nKMWarfareTemplet = ((warfareGameData != null) ? NKMWarfareTemplet.Find(warfareGameData.warfareTempletID) : null);
		NKMStageTempletV2 nKMStageTempletV = ((nKMWarfareTemplet != null) ? NKMEpisodeMgr.FindStageTempletByBattleStrID(nKMWarfareTemplet.m_WarfareStrID) : null);
		if (nKMStageTempletV != null && nKMStageTempletV.EnterLimit > 0)
		{
			int statePlayCnt = NKCScenManager.CurrentUserData().GetStatePlayCnt(nKMStageTempletV.Key);
			num = nKMStageTempletV.EnterLimit - statePlayCnt;
		}
		if (nKMWarfareTemplet != null && nKMWarfareTemplet.m_RewardMultiplyMax != 0)
		{
			num = Mathf.Min(num, nKMWarfareTemplet.m_RewardMultiplyMax);
		}
		m_NKCUIOperationMultiply.SetData(costItem.MiscItemId, costItem.MiscItemCount, m_ResourceButton.GetItemID(), m_AttackCostItemCount, m_CurrMultiplyRewardCount, 2, num);
	}

	private void OnClickMultiply(bool bSet)
	{
		if (bSet)
		{
			if (m_iCurrentBatchedShipCnt <= 0)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_WARFARE_CANNOT_START_BECAUSE_NO_USER_UNIT);
				m_tgReadyMultiply.Select(bSelect: false);
				return;
			}
			if (!CheckMultiply(bMsg: true))
			{
				m_tgReadyMultiply.Select(bSelect: false);
				return;
			}
			if (m_CurrMultiplyRewardCount == 1)
			{
				m_CurrMultiplyRewardCount = 2;
				UpdateAttckCostUI();
				SetOperationMultiplyData();
			}
		}
		NKCUtil.SetGameobjectActive(m_NKCUIOperationMultiply, bSet);
		if (!bSet)
		{
			m_CurrMultiplyRewardCount = 1;
			UpdateAttckCostUI();
			SetOperationMultiplyData();
		}
	}

	private bool CheckMultiply(bool bMsg)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return false;
		}
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData == null)
		{
			return false;
		}
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(warfareGameData.warfareTempletID);
		if (nKMWarfareTemplet == null)
		{
			return false;
		}
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.OPERATION_MULTIPLY))
		{
			return false;
		}
		if (!IsCanStartEterniumStage(bCallLackPopup: true))
		{
			return false;
		}
		if (!nKMUserData.IsSuperUser())
		{
			if (!nKMUserData.CheckWarfareClear(warfareGameData.warfareTempletID))
			{
				if (bMsg)
				{
					NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_MULTIPLY_OPERATION_MEDAL_COND);
				}
				return false;
			}
			NKMWarfareClearData warfareClearData = nKMUserData.GetWarfareClearData(warfareGameData.warfareTempletID);
			if (warfareClearData == null || !warfareClearData.m_mission_result_1 || !warfareClearData.m_mission_result_2)
			{
				if (bMsg)
				{
					NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_MULTIPLY_OPERATION_MEDAL_COND);
				}
				return false;
			}
		}
		if (nKMWarfareTemplet.m_RewardMultiplyMax <= 1)
		{
			return false;
		}
		NKMStageTempletV2 nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(nKMWarfareTemplet.m_WarfareStrID);
		if (nKMStageTempletV.EnterLimit > 0)
		{
			int statePlayCnt = NKCScenManager.CurrentUserData().GetStatePlayCnt(nKMStageTempletV.Key);
			if (nKMStageTempletV.EnterLimit - statePlayCnt <= 0)
			{
				return false;
			}
		}
		return true;
	}

	public void UpdateMultiplyUI()
	{
		if (NKCScenManager.CurrentUserData() == null)
		{
			return;
		}
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData == null)
		{
			return;
		}
		bool flag = true;
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet != null && nKMWarfareTemplet.StageTemplet != null)
		{
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(nKMWarfareTemplet.StageTemplet.m_StageBattleStrID);
			if (dungeonTempletBase != null)
			{
				flag = dungeonTempletBase.m_RewardMultiplyMax > 1;
			}
		}
		bool flag2 = NKCContentManager.IsContentsUnlocked(ContentsType.OPERATION_MULTIPLY);
		if (!flag2 || !flag)
		{
			NKCUtil.SetGameobjectActive(m_objReadyMultiply, bValue: false);
			NKCUtil.SetGameobjectActive(m_objPlayingMultiply, bValue: false);
			return;
		}
		m_NKCUIOperationMultiply.SetLockUI(!flag2);
		if (warfareGameData.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_STOP)
		{
			NKCUtil.SetGameobjectActive(m_NKCUIOperationMultiply, bValue: false);
			bool flag3 = warfareGameData.rewardMultiply > 1;
			NKCUtil.SetGameobjectActive(m_objReadyMultiply, bValue: false);
			NKCUtil.SetGameobjectActive(m_objPlayingMultiply, flag3);
			if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.WARFARE_REPEAT))
			{
				SetActiveRepeatOperation(bSet: false);
			}
			else
			{
				SetActiveRepeatOperation(!flag3);
			}
			NKCUtil.SetLabelText(m_txtPlayingMultiply, NKCUtilString.GET_MULTIPLY_REWARD_ONE_PARAM, warfareGameData.rewardMultiply);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objReadyMultiply, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.WARFARE_REPEAT));
			NKCUtil.SetGameobjectActive(m_objPlayingMultiply, bValue: false);
		}
	}
}
