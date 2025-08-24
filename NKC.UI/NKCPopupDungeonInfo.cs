using System.Collections.Generic;
using ClientPacket.User;
using NKC.Publisher;
using NKC.UI.NPC;
using NKC.UI.Tooltip;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupDungeonInfo : NKCUIBase
{
	private struct BattleCondition
	{
		public Image Img;

		public NKCUIComStateButton Btn;

		public BattleCondition(Image _img, NKCUIComStateButton _btn)
		{
			Img = _img;
			Btn = _btn;
		}
	}

	public delegate void OnButton(NKMStageTempletV2 stageTemplet);

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_operation";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_DUNGEON";

	private static NKCPopupDungeonInfo m_Instance;

	public Vector2 DEFAULT_CHAR_POS = new Vector2(-9.97f, -104.7f);

	public GameObject m_objDungeonBoss;

	public Text m_lbDungeonName;

	public Text m_lbDungeonDesc;

	public Text m_lbEpisodeTitle;

	[Header("훈련")]
	public GameObject m_NKM_UI_OPERATION_POPUP_TRAINING_ICON;

	[Header("컷씬")]
	public GameObject m_NKM_UI_OPERATION_POPUP_CUT_SCEN;

	public GameObject m_NKM_UI_OPERATION_POPUP_CHARACTER;

	public Image m_NKM_UI_OPERATION_POPUP_CUT_SCEN_BTN_ON;

	public NKCUIComToggle m_Tgl_CUT_SCEN_CHECK;

	public NKCUINPCSpineIllust m_NKCUINPCSpineIllust;

	[Header("등장하는 적 표시")]
	public NKCUIComEnemyList m_NKCUIComEnemyList;

	[Header("던전 달성 목표")]
	public NKCUIComDungeonMission m_NKCUIComDungeonMission;

	[Header("입장 제한")]
	public GameObject m_NKM_UI_OPERATION_POPUP_EnterLimit;

	public Text m_EnterLimit_TEXT;

	[Header("전투환경")]
	public GameObject m_NKM_UI_OPERATION_POPUP_SCENARIO_BC;

	public GameObject m_NKM_UI_OPERATION_POPUP_SCENARIO_BC_LAYOUT;

	public GameObject m_NKM_UI_OPERATION_POPUP_SCENARIO_BC_ICON;

	[Header("던전 보너스")]
	public GameObject m_BonusType;

	public Image m_BonusType_Icon;

	[Header("던전 입장 요구 아이템")]
	public GameObject m_NKM_UI_OPERATION_POPUP_ETERNIUM;

	public Image m_NKM_UI_OPERATION_POPUP_ETERNIUM_ICON;

	public Text m_NKM_UI_OPERATION_POPUP_ETERNIUM_TEXT;

	[Header("던전 보상 리스트 컴포넌트")]
	public NKCUIComDungeonRewardList m_NKCUIComDungeonRewardList;

	[Header("페이즈 관련")]
	public GameObject m_objPhase;

	public Text m_lbPhase;

	[Header("Etc")]
	public Text m_NKM_UI_OPERATION_POPUP_BOTTOM_OK_TEXT;

	[Header("이벤트 트리거")]
	public EventTrigger m_eventTriggerBG;

	[Header("버튼")]
	public NKCUIComButton m_btnCancel;

	public NKCUIComButton m_btnNext;

	[Header("치트")]
	public EventTrigger m_ETDungeonClearReward;

	[Header("팀업")]
	public GameObject m_objTeamUP;

	public RectTransform m_rtTeamUpParent;

	public GameObject m_objTeamUpBox;

	[Header("버프 적용 유닛 노출 ID 범위")]
	public int m_iMinDisplayUnitID = 1001;

	public int m_iMaxDisplayUnitID = 10000;

	private List<BattleCondition> m_listBattleConditionSlot = new List<BattleCondition>();

	private const string DEFAULT_CHAR_STR = "NKM_NPC_OPERATOR_LENA";

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	private NKCASUISpineIllust m_unitIllust;

	private NKMStageTempletV2 m_StageTemplet;

	private OnButton dOnOKButton;

	private List<GameObject> m_lstTeamUpBox = new List<GameObject>();

	private List<NKCUISlot> lstTeamUpUnits = new List<NKCUISlot>();

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.DEFAULT;

	public static NKCPopupDungeonInfo Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupDungeonInfo>("ab_ui_nkm_ui_operation", "NKM_UI_POPUP_DUNGEON", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupDungeonInfo>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCUtilString.GET_STRING_MENU_NAME_DUNGEON_POPUP;

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

	public static void PreloadInstance()
	{
		if (m_Instance == null)
		{
			NKCUtil.SetGameobjectActive(Instance, bValue: false);
		}
	}

	public void InitUI()
	{
		if (m_NKCUIOpenAnimator == null)
		{
			m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		}
		m_NKCUIComDungeonRewardList.InitUI();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			Close();
		});
		m_eventTriggerBG.triggers.Add(entry);
		m_btnCancel.PointerClick.RemoveAllListeners();
		m_btnCancel.PointerClick.AddListener(base.Close);
		m_btnNext.PointerClick.RemoveAllListeners();
		m_btnNext.PointerClick.AddListener(OnOK);
		NKCUtil.SetHotkey(m_btnNext, HotkeyEventType.Confirm);
		m_Tgl_CUT_SCEN_CHECK.OnValueChanged.RemoveAllListeners();
		m_Tgl_CUT_SCEN_CHECK.OnValueChanged.AddListener(OnChangedCutscenCheck);
		m_NKCUIComEnemyList.InitUI();
		base.gameObject.SetActive(value: false);
	}

	public void Open(NKMStageTempletV2 stageTemplet, OnButton onOkButton = null, bool isPlaying = false)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null || stageTemplet == null)
		{
			return;
		}
		dOnOKButton = onOkButton;
		base.gameObject.SetActive(value: true);
		m_StageTemplet = stageTemplet;
		m_NKCUIComDungeonRewardList.CreateRewardSlotDataList(myUserData, stageTemplet, stageTemplet.m_StageBattleStrID);
		NKCUtil.SetGameobjectActive(m_objDungeonBoss, bValue: false);
		NKMEpisodeTempletV2 episodeTemplet = stageTemplet.EpisodeTemplet;
		SetEpisodeTitle(m_lbEpisodeTitle, episodeTemplet);
		NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_POPUP_TRAINING_ICON, stageTemplet.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_PRACTICE);
		string text = "";
		bool bCutscenDungeon = false;
		Color successTextColor = new Color(1f, 1f, 1f, 1f);
		Color failTextColor = new Color(0.4392157f, 41f / 85f, 44f / 85f, 1f);
		switch (stageTemplet.m_STAGE_TYPE)
		{
		default:
			return;
		case STAGE_TYPE.ST_WARFARE:
		{
			NKMWarfareTemplet warfareTemplet = m_StageTemplet.WarfareTemplet;
			if (warfareTemplet == null)
			{
				return;
			}
			text = warfareTemplet.GetWarfareName();
			SetUIByStageWarfareData(myUserData, warfareTemplet, successTextColor, failTextColor);
			break;
		}
		case STAGE_TYPE.ST_DUNGEON:
		{
			NKMDungeonTempletBase dungeonTempletBase = m_StageTemplet.DungeonTempletBase;
			if (dungeonTempletBase == null)
			{
				return;
			}
			bCutscenDungeon = dungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_CUTSCENE;
			text = dungeonTempletBase.GetDungeonName();
			SetUIByStageDungeonData(myUserData, stageTemplet, dungeonTempletBase, episodeTemplet, successTextColor, failTextColor);
			break;
		}
		case STAGE_TYPE.ST_PHASE:
		{
			NKMPhaseTemplet phaseTemplet = m_StageTemplet.PhaseTemplet;
			if (phaseTemplet == null)
			{
				return;
			}
			bCutscenDungeon = false;
			text = phaseTemplet.GetName();
			SetUIByStagePhaseData(myUserData, stageTemplet, phaseTemplet, episodeTemplet, successTextColor, failTextColor);
			break;
		}
		}
		bool bCheckDailyDungeon = IsDailyDungeon(episodeTemplet);
		SetDungeonName(stageTemplet, m_lbDungeonName, text, bCutscenDungeon, bCheckDailyDungeon);
		SetDungeonDesc(m_lbDungeonDesc, stageTemplet.GetStageDesc());
		if (m_NKCUIComEnemyList != null && m_NKCUIComEnemyList.gameObject.activeSelf)
		{
			m_NKCUIComEnemyList.SetData(stageTemplet);
		}
		bool flag = IsEnterConditionLimited(stageTemplet);
		NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_POPUP_EnterLimit, flag);
		if (flag)
		{
			SetEnterLimitCondition(m_EnterLimit_TEXT, stageTemplet);
		}
		SetDungeonBonus(m_BonusType, m_BonusType_Icon, stageTemplet);
		SetStageRequiredItem(m_NKM_UI_OPERATION_POPUP_ETERNIUM, m_NKM_UI_OPERATION_POPUP_ETERNIUM_ICON, m_NKM_UI_OPERATION_POPUP_ETERNIUM_TEXT, stageTemplet);
		if (string.Equals(stageTemplet.m_StageBattleStrID, "NKM_WARFARE_EP1_1_1"))
		{
			NKCPublisherModule.Notice.OpenPromotionalBanner(NKCPublisherModule.NKCPMNotice.eOptionalBannerPlaces.EP1Start, null);
		}
		m_NKCUIOpenAnimator.PlayOpenAni();
		if (stageTemplet.m_STAGE_TYPE == STAGE_TYPE.ST_PHASE && stageTemplet.PhaseTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_objPhase, bValue: true);
			NKCUtil.SetLabelText(m_lbPhase, NKCStringTable.GetString("SI_DP_STAGE_PHASE_COUNT", stageTemplet.PhaseTemplet.GetPhaseCount()));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objPhase, bValue: false);
		}
		SetOKButtonText(isPlaying);
		UIOpened();
	}

	public void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator.Update();
			m_NKCUIComDungeonRewardList.ShowRewardListUpdate();
		}
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
		if (m_unitIllust != null)
		{
			m_unitIllust.Unload();
			m_unitIllust = null;
		}
		ClearTeamUPData();
	}

	public void OnOK()
	{
		CloseWithCallback();
	}

	public void CloseWithCallback()
	{
		Close();
		if (dOnOKButton != null)
		{
			dOnOKButton(m_StageTemplet);
		}
	}

	public void OnRecv(NKMPacket_GAME_OPTION_PLAY_CUTSCENE_ACK cNKMPacket_GAME_OPTION_PLAY_CUTSCENE_ACK)
	{
		if (!cNKMPacket_GAME_OPTION_PLAY_CUTSCENE_ACK.isPlayCutscene)
		{
			m_Tgl_CUT_SCEN_CHECK.Select(bSelect: false, bForce: true);
		}
		else
		{
			m_Tgl_CUT_SCEN_CHECK.Select(bSelect: true, bForce: true);
		}
	}

	private void OnChangedCutscenCheck(bool bCheck)
	{
		NKMPacket_GAME_OPTION_PLAY_CUTSCENE_REQ nKMPacket_GAME_OPTION_PLAY_CUTSCENE_REQ = new NKMPacket_GAME_OPTION_PLAY_CUTSCENE_REQ();
		nKMPacket_GAME_OPTION_PLAY_CUTSCENE_REQ.isPlayCutscene = m_Tgl_CUT_SCEN_CHECK.m_bChecked;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GAME_OPTION_PLAY_CUTSCENE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	private bool IsEnterConditionLimited(NKMStageTempletV2 stageTemplet)
	{
		if (stageTemplet == null)
		{
			return false;
		}
		return stageTemplet.EnterLimit > 0;
	}

	private void SetEpisodeTitle(Text episodeTitleText, NKMEpisodeTempletV2 cNKMEpisodeTemplet)
	{
		if (cNKMEpisodeTemplet != null)
		{
			NKCUtil.SetLabelText(m_lbEpisodeTitle, cNKMEpisodeTemplet.GetEpisodeTitle());
		}
	}

	private void SetUIByStageWarfareData(NKMUserData cNKMUserData, NKMWarfareTemplet cNKMWarfareTemplet, Color successTextColor, Color failTextColor)
	{
		if (cNKMUserData != null && cNKMWarfareTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_NKCUIComDungeonMission, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_POPUP_CHARACTER, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKCUIComEnemyList, bValue: true);
			SetCutscenBtnUI(HasCutScen(cNKMWarfareTemplet));
			m_NKCUIComDungeonMission.SetData(cNKMWarfareTemplet);
			SetBattleConditionUI(cNKMWarfareTemplet);
		}
	}

	private void SetUIByStageDungeonData(NKMUserData cNKMUserData, NKMStageTempletV2 stageTemplet, NKMDungeonTempletBase cNKMDungeonTempletBase, NKMEpisodeTempletV2 cNKMEpisodeTemplet, Color successTextColor, Color failTextColor)
	{
		if (cNKMUserData == null || stageTemplet == null || cNKMDungeonTempletBase == null || cNKMEpisodeTemplet == null)
		{
			return;
		}
		if (cNKMEpisodeTemplet.m_EPCategory == EPISODE_CATEGORY.EC_DAILY)
		{
			NKCUtil.SetGameobjectActive(m_NKCUIComDungeonMission, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_POPUP_CHARACTER, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKCUIComEnemyList, bValue: true);
			SetCutscenBtnUI(cNKMDungeonTempletBase.HasCutscen());
		}
		else if (cNKMDungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_CUTSCENE || stageTemplet.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_PRACTICE || stageTemplet.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_TUTORIAL)
		{
			if (cNKMDungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_CUTSCENE || stageTemplet.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_TUTORIAL)
			{
				if (string.IsNullOrEmpty(stageTemplet.m_StageCharStr))
				{
					m_unitIllust = AddSpineIllustration("NKM_NPC_OPERATOR_LENA");
				}
				else
				{
					m_unitIllust = AddSpineIllustration(stageTemplet.m_StageCharStr);
				}
				if (m_unitIllust != null && m_NKCUINPCSpineIllust != null)
				{
					m_NKCUINPCSpineIllust.m_spUnitIllust = m_unitIllust.m_SpineIllustInstant_SkeletonGraphic;
					m_unitIllust.SetParent(m_NKCUINPCSpineIllust.transform, worldPositionStays: false);
					m_unitIllust.SetAnchoredPosition(DEFAULT_CHAR_POS);
					if (m_unitIllust.HasAnimation(NKCASUIUnitIllust.eAnimation.UNIT_IDLE))
					{
						m_unitIllust.SetAnimation(NKCASUIUnitIllust.eAnimation.UNIT_IDLE, loop: true);
					}
				}
				NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_POPUP_CHARACTER, bValue: true);
				if (stageTemplet.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_TUTORIAL)
				{
					SetCutscenBtnUI(cNKMDungeonTempletBase.HasCutscen());
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_POPUP_CUT_SCEN, bValue: false);
				}
			}
			else if (stageTemplet.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_PRACTICE)
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_POPUP_CHARACTER, bValue: false);
				SetCutscenBtnUI(cNKMDungeonTempletBase.HasCutscen());
			}
			NKCUtil.SetGameobjectActive(m_NKCUIComDungeonMission, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKCUIComEnemyList, bValue: false);
		}
		else
		{
			SetCutscenBtnUI(cNKMDungeonTempletBase.HasCutscen());
			NKCUtil.SetGameobjectActive(m_NKCUIComDungeonMission, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_POPUP_CHARACTER, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKCUIComEnemyList, bValue: true);
		}
		if (m_NKCUIComDungeonMission.gameObject.activeSelf)
		{
			m_NKCUIComDungeonMission.SetData(cNKMDungeonTempletBase);
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_POPUP_SCENARIO_BC, bValue: false);
		SetBattleConditionUI(cNKMDungeonTempletBase);
	}

	private void SetUIByStagePhaseData(NKMUserData cNKMUserData, NKMStageTempletV2 stageTemplet, NKMPhaseTemplet phaseTemplet, NKMEpisodeTempletV2 cNKMEpisodeTemplet, Color successTextColor, Color failTextColor)
	{
		if (cNKMUserData != null && stageTemplet != null && phaseTemplet != null && cNKMEpisodeTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_NKCUIComDungeonMission, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_POPUP_CHARACTER, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKCUIComEnemyList, bValue: true);
			SetCutscenBtnUI(HasCutScen(phaseTemplet));
			m_NKCUIComDungeonMission.SetData(phaseTemplet);
			SetBattleConditionUI(phaseTemplet);
		}
	}

	private bool IsDailyDungeon(NKMEpisodeTempletV2 cNKMEpisodeTemplet)
	{
		if (cNKMEpisodeTemplet == null)
		{
			return false;
		}
		return cNKMEpisodeTemplet.m_EPCategory == EPISODE_CATEGORY.EC_DAILY;
	}

	private void SetDungeonName(NKMStageTempletV2 stageTemplet, Text lbStage, string stageName, bool bCutscenDungeon, bool bCheckDailyDungeon)
	{
		if (stageTemplet == null || lbStage == null)
		{
			return;
		}
		if (stageTemplet.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_PRACTICE)
		{
			lbStage.text = string.Format(NKCUtilString.GET_STRING_EP_TRAINING_NUMBER, stageTemplet.m_StageUINum) + " " + stageName;
			lbStage.color = NKCUtil.GetColor("#4EC2F3");
			return;
		}
		lbStage.color = NKCUtil.GetColor("#FFFFFF");
		if (bCutscenDungeon)
		{
			lbStage.text = string.Format(NKCUtilString.GET_STRING_EP_CUTSCEN_NUMBER, stageTemplet.m_StageUINum) + " " + stageName;
		}
		else if (bCheckDailyDungeon)
		{
			lbStage.text = $"{stageName} {NKCUtilString.GetDailyDungeonLVDesc(stageTemplet.m_StageUINum)}";
		}
		else
		{
			lbStage.text = $"{stageTemplet.ActId}-{stageTemplet.m_StageUINum}. {stageName}";
		}
	}

	private void SetDungeonDesc(Text dungeonDesc, string desc)
	{
		NKCUtil.SetLabelText(dungeonDesc, desc);
	}

	private void SetEnterLimitCondition(Text enterLimitText, NKMStageTempletV2 stageTemplet)
	{
		if (stageTemplet != null)
		{
			int statePlayCnt = NKCScenManager.CurrentUserData().GetStatePlayCnt(stageTemplet.Key);
			string text = "";
			NKCUtil.SetLabelText(enterLimitText, stageTemplet.EnterLimitCond switch
			{
				NKMStageTempletV2.RESET_TYPE.DAY => string.Format(NKCUtilString.GET_STRING_POPUP_DUNGEON_ENTER_LIMIT_DESC_DAY_02, stageTemplet.EnterLimit - statePlayCnt, stageTemplet.EnterLimit), 
				NKMStageTempletV2.RESET_TYPE.MONTH => string.Format(NKCUtilString.GET_STRING_POPUP_DUNGEON_ENTER_LIMIT_DESC_MONTH_02, stageTemplet.EnterLimit - statePlayCnt, stageTemplet.EnterLimit), 
				NKMStageTempletV2.RESET_TYPE.WEEK => string.Format(NKCUtilString.GET_STRING_POPUP_DUNGEON_ENTER_LIMIT_DESC_WEEK_02, stageTemplet.EnterLimit - statePlayCnt, stageTemplet.EnterLimit), 
				_ => string.Format(NKCUtilString.GET_STRING_POPUP_DUNGEON_ENTER_LIMIT_DESC_DAY_02, stageTemplet.EnterLimit - statePlayCnt, stageTemplet.EnterLimit), 
			});
			if (stageTemplet.EnterLimit - statePlayCnt <= 0)
			{
				NKCUtil.SetLabelTextColor(enterLimitText, Color.red);
			}
			else
			{
				NKCUtil.SetLabelTextColor(enterLimitText, Color.white);
			}
		}
	}

	private void SetDungeonBonus(GameObject bonusType, Image bonusTypeIcon, NKMStageTempletV2 stageTemplet)
	{
		if (stageTemplet != null)
		{
			if (stageTemplet.m_BuffType.Equals(RewardTuningType.None))
			{
				NKCUtil.SetGameobjectActive(bonusType, bValue: false);
				return;
			}
			NKCUtil.SetGameobjectActive(bonusType, bValue: true);
			NKCUtil.SetImageSprite(bonusTypeIcon, NKCUtil.GetBounsTypeIcon(stageTemplet.m_BuffType, big: false));
		}
	}

	private void SetStageRequiredItem(GameObject itemObject, Image itemIcon, Text itemCount, NKMStageTempletV2 stageTemplet)
	{
		if (stageTemplet == null)
		{
			return;
		}
		NKCUtil.SetGameobjectActive(itemObject, stageTemplet.m_StageReqItemID != 0 && stageTemplet.m_StageReqItemCount > 0);
		int eternium = stageTemplet.m_StageReqItemCount;
		if (stageTemplet.m_StageReqItemID == 2)
		{
			if (stageTemplet.WarfareTemplet != null)
			{
				NKCCompanyBuff.SetDiscountOfEterniumInEnteringWarfare(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref eternium);
			}
			else if (stageTemplet.DungeonTempletBase != null)
			{
				NKCCompanyBuff.SetDiscountOfEterniumInEnteringDungeon(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref eternium);
			}
			else if (stageTemplet.PhaseTemplet != null)
			{
				NKCCompanyBuff.SetDiscountOfEterniumInEnteringDungeon(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref eternium);
			}
		}
		NKCUtil.SetLabelText(itemCount, eternium.ToString());
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(stageTemplet.m_StageReqItemID);
		if (itemMiscTempletByID != null)
		{
			Sprite orLoadMiscItemSmallIcon = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(itemMiscTempletByID);
			NKCUtil.SetImageSprite(itemIcon, orLoadMiscItemSmallIcon);
		}
	}

	private void SetOKButtonText(bool isPlaying)
	{
		m_NKM_UI_OPERATION_POPUP_BOTTOM_OK_TEXT.text = (isPlaying ? NKCUtilString.GET_STRING_OPERATION_POPUP_BUTTON_PLAYING : NKCUtilString.GET_STRING_OPERATION_POPUP_BUTTON);
	}

	private void SetCutscenBtnUI(bool bExistCutscen)
	{
		if (!bExistCutscen)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_POPUP_CUT_SCEN, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_POPUP_CUT_SCEN, bValue: true);
		bool bPlayCutscene = NKCScenManager.GetScenManager().GetMyUserData().m_UserOption.m_bPlayCutscene;
		m_Tgl_CUT_SCEN_CHECK.Select(bPlayCutscene, bForce: true);
	}

	private void SetBattleConditionUI(NKMWarfareTemplet warfareTemplet)
	{
		List<NKMBattleConditionTemplet> list = new List<NKMBattleConditionTemplet>();
		if (warfareTemplet != null && warfareTemplet.MapTemplet != null)
		{
			for (int i = 0; i < warfareTemplet.MapTemplet.TileCount; i++)
			{
				NKMWarfareTileTemplet tile = warfareTemplet.MapTemplet.GetTile(i);
				if (tile != null && tile.BattleCondition != null && !list.Contains(tile.BattleCondition))
				{
					list.Add(tile.BattleCondition);
				}
			}
			foreach (string dungeonStrID in warfareTemplet.MapTemplet.GetDungeonStrIDList())
			{
				NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(dungeonStrID);
				if (dungeonTempletBase == null)
				{
					continue;
				}
				foreach (NKMBattleConditionTemplet battleCondition in dungeonTempletBase.BattleConditions)
				{
					list.Add(battleCondition);
				}
			}
		}
		UpdateBattleConditionUI(list);
	}

	private void SetBattleConditionUI(NKMDungeonTempletBase dungeonTempletBase)
	{
		if (dungeonTempletBase != null && dungeonTempletBase.BattleConditions != null)
		{
			UpdateBattleConditionUI(dungeonTempletBase.BattleConditions);
		}
	}

	private void SetBattleConditionUI(NKMPhaseTemplet phaseTemplet)
	{
		List<NKMBattleConditionTemplet> list = new List<NKMBattleConditionTemplet>();
		if (phaseTemplet != null)
		{
			foreach (NKMPhaseOrderTemplet item in phaseTemplet.PhaseList.List)
			{
				if (item.Dungeon == null)
				{
					continue;
				}
				foreach (NKMBattleConditionTemplet battleConditionTemplet in item.Dungeon.BattleConditions)
				{
					if (list.FindIndex((NKMBattleConditionTemplet e) => e == battleConditionTemplet) == -1)
					{
						list.Add(battleConditionTemplet);
					}
				}
			}
		}
		UpdateBattleConditionUI(list);
	}

	private void UpdateBattleConditionUI(List<NKMBattleConditionTemplet> listBattleConditionTemplet)
	{
		listBattleConditionTemplet?.RemoveAll((NKMBattleConditionTemplet x) => x.m_bHide);
		if (listBattleConditionTemplet == null || listBattleConditionTemplet.Count == 0)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_POPUP_SCENARIO_BC, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_POPUP_SCENARIO_BC, bValue: true);
		int count = listBattleConditionTemplet.Count;
		int num = count - m_listBattleConditionSlot.Count;
		for (int num2 = 0; num2 < num; num2++)
		{
			GameObject obj = Object.Instantiate(m_NKM_UI_OPERATION_POPUP_SCENARIO_BC_ICON, m_NKM_UI_OPERATION_POPUP_SCENARIO_BC_LAYOUT.transform);
			Image component = obj.GetComponent<Image>();
			NKCUIComStateButton component2 = obj.GetComponent<NKCUIComStateButton>();
			if (component != null && component2 != null)
			{
				m_listBattleConditionSlot.Add(new BattleCondition(component, component2));
			}
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_POPUP_SCENARIO_BC_ICON, bValue: false);
		for (int num3 = 0; num3 < m_listBattleConditionSlot.Count; num3++)
		{
			Image img = m_listBattleConditionSlot[num3].Img;
			if (num3 < count)
			{
				NKMBattleConditionTemplet cNKMBattleConditionTemplet = listBattleConditionTemplet[num3];
				if (cNKMBattleConditionTemplet != null)
				{
					Sprite spriteBattleConditionICon = NKCUtil.GetSpriteBattleConditionICon(cNKMBattleConditionTemplet);
					if (spriteBattleConditionICon != null)
					{
						NKCUtil.SetImageSprite(img, spriteBattleConditionICon);
						NKCUtil.SetGameobjectActive(img.gameObject, bValue: true);
					}
					NKCUIComStateButton btn = m_listBattleConditionSlot[num3].Btn;
					if (btn != null)
					{
						btn.PointerDown.RemoveAllListeners();
						btn.PointerDown.AddListener(delegate(PointerEventData e)
						{
							NKCUITooltip.Instance.Open(NKCUISlot.eSlotMode.Etc, cNKMBattleConditionTemplet.BattleCondName_Translated, cNKMBattleConditionTemplet.BattleCondDesc_Translated, e.position);
						});
					}
					continue;
				}
			}
			NKCUtil.SetGameobjectActive(img.gameObject, bValue: false);
		}
		UpdateBattleConditionTeamUpUI(listBattleConditionTemplet);
	}

	private void UpdateBattleConditionTeamUpUI(List<NKMBattleConditionTemplet> listBattleConditionTemplet)
	{
		ClearTeamUPData();
		List<int> list = new List<int>();
		if (listBattleConditionTemplet != null && listBattleConditionTemplet.Count > 0)
		{
			List<string> list2 = new List<string>();
			foreach (NKMBattleConditionTemplet item in listBattleConditionTemplet)
			{
				if (item == null)
				{
					continue;
				}
				foreach (string item2 in item.AffectTeamUpID)
				{
					if (!list2.Contains(item2))
					{
						list2.Add(item2);
					}
				}
				foreach (int item3 in item.hashAffectUnitID)
				{
					if (!list.Contains(item3))
					{
						list.Add(item3);
					}
				}
			}
			List<NKMUnitTempletBase> list3 = new List<NKMUnitTempletBase>();
			if (list2.Count > 0)
			{
				foreach (string item4 in list2)
				{
					foreach (NKMUnitTempletBase item5 in NKMUnitManager.GetListTeamUPUnitTempletBase(item4))
					{
						if (!list3.Contains(item5))
						{
							list3.Add(item5);
						}
					}
				}
			}
			if (list3.Count == 0 && list.Count > 0)
			{
				foreach (int item6 in list)
				{
					NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(item6);
					if (unitTempletBase != null && !list3.Contains(unitTempletBase))
					{
						list3.Add(unitTempletBase);
					}
				}
			}
			foreach (NKMUnitTempletBase item7 in list3)
			{
				if (!item7.PickupEnableByTag || item7.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_NORMAL || item7.m_UnitID < m_iMinDisplayUnitID || item7.m_UnitID > m_iMaxDisplayUnitID || (item7.m_ShipGroupID != 0 && item7.m_ShipGroupID != item7.m_UnitID))
				{
					continue;
				}
				GameObject gameObject = Object.Instantiate(m_objTeamUpBox);
				if (!(null == gameObject))
				{
					gameObject.transform.localScale = Vector3.one;
					NKCUtil.SetGameobjectActive(gameObject, bValue: true);
					gameObject.transform.SetParent(m_rtTeamUpParent, worldPositionStays: false);
					m_lstTeamUpBox.Add(gameObject);
					NKCUISlot newInstance = NKCUISlot.GetNewInstance(gameObject.transform);
					if (null != newInstance)
					{
						newInstance.transform.localPosition = Vector3.zero;
						newInstance.transform.localScale = Vector3.one;
						NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(NKM_REWARD_TYPE.RT_UNIT, item7.m_UnitID, 1);
						NKCUtil.SetGameobjectActive(newInstance.gameObject, bValue: true);
						newInstance.SetData(data);
						lstTeamUpUnits.Add(newInstance);
					}
				}
			}
		}
		NKCUtil.SetGameobjectActive(m_objTeamUP, m_lstTeamUpBox.Count > 0);
	}

	private void ClearTeamUPData()
	{
		for (int i = 0; i < m_lstTeamUpBox.Count; i++)
		{
			Object.Destroy(m_lstTeamUpBox[i]);
			m_lstTeamUpBox[i] = null;
		}
		m_lstTeamUpBox.Clear();
	}

	private NKCASUISpineIllust AddSpineIllustration(string prefabStrID)
	{
		return (NKCASUISpineIllust)NKCResourceUtility.OpenSpineIllustWithManualNaming(prefabStrID);
	}

	private static bool HasCutScen(NKMWarfareTemplet templet)
	{
		if (templet.m_CutScenStrIDAfter.Length > 0 || templet.m_CutScenStrIDBefore.Length > 0)
		{
			return true;
		}
		foreach (string dungeonStrID in templet.MapTemplet.GetDungeonStrIDList())
		{
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(dungeonStrID);
			if (dungeonTempletBase != null && dungeonTempletBase.HasCutscen())
			{
				return true;
			}
		}
		return false;
	}

	private static bool HasCutScen(NKMPhaseTemplet templet)
	{
		if (templet.m_CutScenStrIDAfter.Length > 0 || templet.m_CutScenStrIDBefore.Length > 0)
		{
			return true;
		}
		foreach (NKMPhaseOrderTemplet item in templet.PhaseList.List)
		{
			if (item.Dungeon != null && item.Dungeon.HasCutscen())
			{
				return true;
			}
		}
		return false;
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}
}
