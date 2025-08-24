using System.Collections;
using System.Collections.Generic;
using NKC.UI.Tooltip;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperatorInfoPopupSkill : NKCUIBase
{
	private enum TAB_TYPE
	{
		OPER,
		TOKEN
	}

	private enum SLOT_ANI_TYPE
	{
		NONE,
		ENHANCE_LOOP,
		ENHANCE_OUT,
		SKILL_ON,
		SKILL_ON_OUT,
		ISLOCK,
		ISLOCK_TOKEN
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_operator_info";

	private const string UI_ASSET_NAME = "NKM_UI_OPERATOR_INFO_POPUP_SKILL";

	private static NKCUIOperatorInfoPopupSkill m_Instance;

	private readonly List<int> RESOURCE_LIST = new List<int> { 1, 3, 101 };

	public NKCUIOperatorSelectListSlot m_BaseSlot;

	public NKCUIOperatorSelectListSlot m_ResourceSlot;

	public GameObject m_objResourceSlot;

	public NKCUISlot m_ResourceSubSkillTokenSlot;

	public NKCUIComStateButton m_BUTTON_CANCEL;

	public NKCUIOperatorSkill m_BaseMainSkill;

	public NKCUIOperatorSkill m_BaseSubSkill;

	public NKCUIOperatorSkill m_MatMainSkill;

	public NKCUIOperatorSkill m_MatSubSkill;

	public GameObject m_objMatMainSkillNone;

	public Text m_lbMainSkillSuccessfulRate;

	public Text m_lbMainSubSuccessfulRate;

	public GameObject m_objMainSkillUpgradeLabel;

	public Text m_lbMainSkillLevelBefore;

	public Text m_lbMainSkillLevelAfter;

	public GameObject m_SkillPanel;

	public GameObject m_SkillPanelNone;

	public NKCComText m_lbNoneTitle;

	public NKCComText m_lbNoneDesc;

	public List<Image> m_lstMainSkillGrow;

	public List<Image> m_lstSubSkillGrow;

	private TAB_TYPE m_curTab;

	[Header("tab")]
	public NKCUIComToggle m_ctglOper;

	public NKCUIComToggle m_ctglToken;

	[Header("button")]
	public NKCUIComToggle m_SKILL_IMPLANT_TOGGLE_CHECK;

	public NKCUIComStateButton m_MERGE_BUTTON;

	[Header("Resource")]
	public Image m_RESOURCE_ICON;

	public Text m_RESOURCE_NUMBER_TEXT;

	[Header("Sort & filter")]
	public NKCUIComUnitSortOptions m_SortUI;

	public LoopScrollRect m_LoopScrollRect;

	public LoopScrollRect m_LoopScrollRect_ForToken;

	[Header("animaitor")]
	public Animator m_SKILL_ANI;

	public Animator m_SubSkillAni;

	public Animator m_ResourceInvenAni;

	public GameObject m_BaseCardMergeEffect;

	public GameObject m_ResourceCardMergeEffect;

	public Animator m_MainSkillSlotAni;

	public Animator m_SubSkillSlotAni;

	[Header("block text")]
	public Text m_lbMainSkillDesc;

	public Text m_lbSubSkillDesc;

	[Header("event")]
	public GameObject m_objEventSubSkill;

	public Text m_lbEventSubSkill;

	public GameObject m_objEvent;

	public Text m_lbEventDesc;

	private NKMOperator m_BaseOperator;

	private NKMOperator m_ResourceOperator;

	private NKCOperatorPassiveToken m_ResourcePassiveToken;

	private bool m_bEnhanceSubSkill;

	private bool m_bEnhanceMainSkill;

	private bool m_bCanTrasferPassiveSkill;

	private const int SUB_SKILL_TOKEN_TRANSFER_LEVEL = 1;

	private bool m_bEnoughCostItem;

	private int m_iLackItemID;

	private int m_iLackItemCnt;

	[Header("오퍼레이터 스킬 버튼")]
	public NKCUIComStateButton m_BaseMainSkillBtn;

	public NKCUIComStateButton m_BaseSubSkillBtn;

	public NKCUIComStateButton m_ResourceMainSkillBtn;

	public NKCUIComStateButton m_ResourceSubSkillBtn;

	[Header("애니메이션 딜레이 갭")]
	public float m_fDelayGap;

	private const string ANI_IDLE = "IDLE";

	private const string ANI_IDLE2 = "IDLE2";

	private const string ANI_SWITCH_01_TO_02 = "01_TO_02";

	private const string ANI_SWITCH_02_TO_01 = "02_TO_01";

	private bool m_bWaitPacketResult;

	private NKCOperatorSortSystem m_OperatorSortSystem;

	private Stack<NKCUIOperatorDeckSelectSlot> m_stkOperatorSlotPool = new Stack<NKCUIOperatorDeckSelectSlot>();

	private List<NKCUIOperatorDeckSelectSlot> m_lstVisibleSlot = new List<NKCUIOperatorDeckSelectSlot>();

	private NKCOperatorSortSystem.OperatorListOptions m_operatorSortOptions;

	public RectTransform m_rectSlotPoolRect;

	private NKCOperatorTokenSortSystem m_OperatorTokenSortSystem;

	private NKCOperatorTokenSortSystem.OperatorTokenListOptions m_OperTokenSortOption;

	private Stack<NKCUISlot> m_stkTokenSlotPool = new Stack<NKCUISlot>();

	private List<NKCUISlot> m_lstVisibleTokenSlot = new List<NKCUISlot>();

	public RectTransform m_rectSlotPoolRect_ForToken;

	public static NKCUIOperatorInfoPopupSkill Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIOperatorInfoPopupSkill>("ab_ui_nkm_ui_operator_info", "NKM_UI_OPERATOR_INFO_POPUP_SKILL", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIOperatorInfoPopupSkill>();
				m_Instance.Init();
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

	public override string MenuName => NKCUtilString.GET_STRING_OPERATOR_SKILL_TRANSFER;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string GuideTempletID => "ARTICLE_OPERATOR_COMPOSE";

	public override List<int> UpsideMenuShowResourceList => RESOURCE_LIST;

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

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public override void OnCloseInstance()
	{
		base.OnCloseInstance();
		Clear();
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
		Cleanup();
		m_ResourceSubSkillTokenSlot.SetEmpty();
		m_ResourceSubSkillTokenSlot.SetBGVisible(bSet: false);
	}

	public override void UnHide()
	{
		base.UnHide();
		m_SKILL_ANI.SetTrigger((m_curTab == TAB_TYPE.OPER) ? "IDLE" : "IDLE2");
		NKCUtil.SetGameobjectActive(m_BaseCardMergeEffect, bValue: false);
		NKCUtil.SetGameobjectActive(m_ResourceCardMergeEffect, bValue: false);
		if (m_ResourceOperator != null)
		{
			m_SubSkillAni.SetTrigger("idle");
		}
		if (m_ResourcePassiveToken != null)
		{
			m_ResourceInvenAni.SetTrigger("idle");
		}
	}

	public void Init()
	{
		if (m_LoopScrollRect != null)
		{
			m_LoopScrollRect.dOnGetObject += GetSlot;
			m_LoopScrollRect.dOnReturnObject += ReturnSlot;
			m_LoopScrollRect.dOnProvideData += ProvideSlotData;
			m_LoopScrollRect.ContentConstraintCount = 3;
			m_LoopScrollRect.PrepareCells();
			NKCUtil.SetScrollHotKey(m_LoopScrollRect);
		}
		if (m_LoopScrollRect_ForToken != null)
		{
			m_LoopScrollRect_ForToken.dOnGetObject += GetSlotToken;
			m_LoopScrollRect_ForToken.dOnReturnObject += ReturnSlotToken;
			m_LoopScrollRect_ForToken.dOnProvideData += ProvideSlotDataToken;
			m_LoopScrollRect_ForToken.ContentConstraintCount = 4;
			m_LoopScrollRect_ForToken.PrepareCells();
		}
		if (m_SortUI != null)
		{
			m_SortUI.Init(OnSortChanged, bIsCollection: false);
		}
		m_operatorSortOptions.setFilterOption = new HashSet<NKCOperatorSortSystem.eFilterOption>();
		m_operatorSortOptions.SetBuildOption(true, BUILD_OPTIONS.DESCENDING, BUILD_OPTIONS.EXCLUDE_LOCKED_UNIT, BUILD_OPTIONS.EXCLUDE_DECKED_UNIT);
		m_operatorSortOptions.lstSortOption = NKCOperatorSortSystem.GetDefaultSortOptions(bIsCollection: false);
		m_operatorSortOptions.eDeckType = NKM_DECK_TYPE.NDT_NORMAL;
		m_OperTokenSortOption.setFilterOption = new HashSet<NKCOperatorTokenSortSystem.eFilterOption>();
		m_OperTokenSortOption.SetBuildOption(true, BUILD_OPTIONS.DESCENDING);
		m_OperTokenSortOption.lstSortOption = NKCOperatorTokenSortSystem.GetDefaultSortOptions();
		NKCUtil.SetBindFunction(m_BUTTON_CANCEL, OnClickMatUnitCancel);
		NKCUtil.SetBindFunction(m_MERGE_BUTTON, OnClickUnitTransfer);
		NKCUtil.SetHotkey(m_MERGE_BUTTON, HotkeyEventType.Confirm);
		if (m_SKILL_IMPLANT_TOGGLE_CHECK != null)
		{
			m_SKILL_IMPLANT_TOGGLE_CHECK.OnValueChanged.RemoveAllListeners();
			m_SKILL_IMPLANT_TOGGLE_CHECK.OnValueChanged.AddListener(OnClickPassiveSkillTransfer);
		}
		if (m_BaseMainSkillBtn != null)
		{
			m_BaseMainSkillBtn.PointerDown.RemoveAllListeners();
			m_BaseMainSkillBtn.PointerDown.AddListener(OnPointDownMainBaseSkill);
		}
		if (m_BaseSubSkillBtn != null)
		{
			m_BaseSubSkillBtn.PointerDown.RemoveAllListeners();
			m_BaseSubSkillBtn.PointerDown.AddListener(OnPointDownMainSubSkill);
		}
		if (m_ResourceMainSkillBtn != null)
		{
			m_ResourceMainSkillBtn.PointerDown.RemoveAllListeners();
			m_ResourceMainSkillBtn.PointerDown.AddListener(OnPointDownResourceBaseSkill);
		}
		if (m_ResourceSubSkillBtn != null)
		{
			m_ResourceSubSkillBtn.PointerDown.RemoveAllListeners();
			m_ResourceSubSkillBtn.PointerDown.AddListener(OnPointDownResourceSubSkill);
		}
		NKCUtil.SetToggleValueChangedDelegate(m_ctglOper, delegate(bool b)
		{
			if (b)
			{
				OnSwitchTab(TAB_TYPE.OPER);
			}
		});
		NKCUtil.SetToggleValueChangedDelegate(m_ctglToken, delegate(bool b)
		{
			if (b)
			{
				OnSwitchTab(TAB_TYPE.TOKEN);
			}
		});
		if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.OPERATOR_EXTRACT))
		{
			m_ctglToken.SetLock(value: true, bForce: true);
		}
	}

	public void Cleanup()
	{
		m_OperatorSortSystem = null;
		m_OperatorTokenSortSystem = null;
		m_ResourceOperator = null;
		m_ResourcePassiveToken = null;
		m_bWaitPacketResult = false;
		m_SortUI?.ResetUI();
	}

	public void Open(NKMOperator operatorData)
	{
		if (operatorData != null)
		{
			m_BaseOperator = operatorData;
			m_curTab = TAB_TYPE.OPER;
			m_ctglOper.Select(bSelect: true, bForce: true, bImmediate: true);
			NKCUtil.SetGameobjectActive(m_LoopScrollRect_ForToken.gameObject, bValue: false);
			NKCUtil.SetGameobjectActive(m_objMatMainSkillNone, bValue: false);
			Cleanup();
			UpdateUI(bForceUpdate: true);
			UpdateSortUIData();
			if (m_ResourceOperator == null && m_objEventSubSkill.activeSelf)
			{
				NKCUtil.SetGameobjectActive(m_objEventSubSkill, bValue: false);
			}
			NKCUtil.SetGameobjectActive(m_BaseCardMergeEffect, bValue: false);
			NKCUtil.SetGameobjectActive(m_ResourceCardMergeEffect, bValue: false);
			NKCUtil.SetGameobjectActive(m_objMainSkillUpgradeLabel, bValue: false);
			UIOpened();
		}
	}

	private void OnSwitchTab(TAB_TYPE newTab)
	{
		if (m_curTab != newTab)
		{
			m_curTab = newTab;
			switch (m_curTab)
			{
			case TAB_TYPE.OPER:
				NKCUtil.SetGameobjectActive(m_objMatMainSkillNone, bValue: false);
				m_SKILL_ANI.SetTrigger("02_TO_01");
				m_operatorSortOptions.setFilterOption = new HashSet<NKCOperatorSortSystem.eFilterOption>();
				break;
			case TAB_TYPE.TOKEN:
				m_SKILL_ANI.SetTrigger("01_TO_02");
				m_OperTokenSortOption.setFilterOption = new HashSet<NKCOperatorTokenSortSystem.eFilterOption>();
				break;
			}
			OnClickMatUnitCancel();
			UpdateUI(bForceUpdate: true);
			UpdateSortUIData();
		}
	}

	private void UpdateSortUIData()
	{
		switch (m_curTab)
		{
		case TAB_TYPE.OPER:
			if (m_OperatorSortSystem == null)
			{
				UpdateSortDataNScrollSetting();
			}
			m_SortUI.RegisterOperatorSort(m_OperatorSortSystem);
			m_SortUI.ResetUI(bUseFavorite: false, bClearFilterSet: true);
			m_SortUI.RegisterCategories(NKCOperatorSortSystem.MakeDefaultFilterCategory(NKCOperatorSortSystem.FILTER_OPEN_TYPE.NORMAL), NKCPopupSort.MakeDefaultOprSortSet(NKM_UNIT_TYPE.NUT_OPERATOR, bIsCollection: false), bFavoriteFilterActive: false);
			if (m_SortUI.m_NKCPopupSort != null)
			{
				m_SortUI.m_NKCPopupSort.m_bUseDefaultSortAdd = false;
			}
			break;
		case TAB_TYPE.TOKEN:
			if (m_OperatorTokenSortSystem == null)
			{
				UpdateSortDataNScrollSetting();
			}
			m_SortUI.RegisterOperatorTokenSort(m_OperatorTokenSortSystem);
			m_SortUI.ResetUI(bUseFavorite: false, bClearFilterSet: true);
			m_SortUI.RegisterCategories(NKCOperatorTokenSortSystem.setDefaultOperatorFilterCategory, NKCOperatorTokenSortSystem.setDefaultOperatorSortCategory, bFavoriteFilterActive: false);
			if (m_SortUI.m_NKCPopupSort != null)
			{
				m_SortUI.m_NKCPopupSort.m_bUseDefaultSortAdd = false;
			}
			break;
		}
	}

	private NKMDeckIndex GetDeckIndex(long unitUID)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null && nKMUserData.m_ArmyData != null)
		{
			return nKMUserData.m_ArmyData.GetOperatorDeckIndex(NKM_DECK_TYPE.NDT_NORMAL, unitUID);
		}
		return NKMDeckIndex.None;
	}

	private void UpdateLeftUI()
	{
		m_bCanTrasferPassiveSkill = false;
		UpdateBaseUnitUI();
		if (m_ResourceOperator != null)
		{
			m_ResourceSlot.SetData(m_ResourceOperator, GetDeckIndex(m_ResourceOperator.uid), bEnableLayoutElement: true, null);
		}
		else if (m_ResourcePassiveToken != null)
		{
			NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeMiscItemData(m_ResourcePassiveToken.ItemID, m_ResourcePassiveToken.ItemCount);
			m_ResourceSubSkillTokenSlot.SetMiscItemData(slotData, bShowName: true, bShowCount: true, bEnableLayoutElement: true, null);
		}
		UpdateSkillTransferUI();
		UpdateCostItem();
		if (!m_bCanTrasferPassiveSkill)
		{
			m_SKILL_IMPLANT_TOGGLE_CHECK.Select(bSelect: false, bForce: true);
		}
		if (m_curTab == TAB_TYPE.OPER)
		{
			NKCUtil.SetLabelText(m_lbNoneTitle, NKCUtilString.GET_STRING_OPERATOR_SKILL_TRANSFER_NONE_TITLE);
			NKCUtil.SetLabelText(m_lbNoneDesc, NKCUtilString.GET_STRING_OPERATOR_SKILL_TRANSFER_NONE_DESC);
			NKCUtil.SetGameobjectActive(m_SkillPanelNone, m_ResourceOperator == null);
			NKCUtil.SetGameobjectActive(m_BUTTON_CANCEL, m_ResourceOperator != null);
			NKCUtil.SetGameobjectActive(m_objResourceSlot, m_ResourceOperator != null);
		}
		else if (m_curTab == TAB_TYPE.TOKEN)
		{
			NKCUtil.SetLabelText(m_lbNoneTitle, NKCUtilString.GET_STRING_OPERATOR_TOKEN_TRANSFER_NONE_TITLE);
			NKCUtil.SetLabelText(m_lbNoneDesc, NKCUtilString.GET_STRING_OPERATOR_TOKEN_TRANSFER_NONE_DESC);
			NKCUtil.SetGameobjectActive(m_SkillPanelNone, m_ResourcePassiveToken == null);
			NKCUtil.SetGameobjectActive(m_BUTTON_CANCEL, m_ResourcePassiveToken != null);
			if (m_ResourcePassiveToken == null)
			{
				m_ResourceSubSkillTokenSlot.SetEmpty();
				m_ResourceSubSkillTokenSlot.SetBGVisible(bSet: false);
			}
		}
	}

	private void UpdateBaseUnitUI()
	{
		if (m_BaseOperator != null)
		{
			m_BaseSlot.SetData(m_BaseOperator, GetDeckIndex(m_BaseOperator.uid), bEnableLayoutElement: true, null);
			NKMOperator operatorData = NKCOperatorUtil.GetOperatorData(m_BaseOperator.uid);
			if (operatorData != null)
			{
				m_BaseMainSkill.SetData(operatorData.mainSkill.id, operatorData.mainSkill.level);
				m_BaseSubSkill.SetData(operatorData.subSkill.id, operatorData.subSkill.level);
			}
		}
	}

	private void UpdateSkillTransferUI()
	{
		if (m_BaseOperator != null || m_ResourceOperator != null)
		{
			if (m_ResourceOperator != null)
			{
				UpdateSkillTransferUI(m_ResourceOperator);
			}
			else if (m_ResourcePassiveToken != null)
			{
				UpdateSkillTransferUI(m_ResourcePassiveToken);
			}
		}
	}

	private void UpdateSkillTransferUI(NKMOperator ResourceOperator)
	{
		m_bEnhanceMainSkill = NKCOperatorUtil.IsCanEnhanceMainSkill(m_BaseOperator, ResourceOperator);
		m_bEnhanceSubSkill = NKCOperatorUtil.IsCanEnhanceSubSkill(m_BaseOperator, ResourceOperator);
		NKCUtil.SetGameobjectActive(m_MatMainSkill.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_objMatMainSkillNone, bValue: false);
		m_MatMainSkill.SetData(ResourceOperator.mainSkill.id, ResourceOperator.mainSkill.level);
		m_MatSubSkill.SetData(ResourceOperator.subSkill.id, ResourceOperator.subSkill.level);
		NKCUtil.SetGameobjectActive(m_objMainSkillUpgradeLabel, m_bEnhanceMainSkill);
		if (m_bEnhanceMainSkill)
		{
			int enhanceSuccessfulRate = NKCOperatorUtil.GetEnhanceSuccessfulRate(ResourceOperator, m_BaseOperator.id == ResourceOperator.id);
			NKCUtil.SetLabelText(m_lbMainSkillSuccessfulRate, $"{enhanceSuccessfulRate}%");
			Color enhanceSuccessfulRateColor = GetEnhanceSuccessfulRateColor(enhanceSuccessfulRate);
			NKCUtil.SetLabelTextColor(m_lbMainSkillSuccessfulRate, enhanceSuccessfulRateColor);
			foreach (Image item in m_lstMainSkillGrow)
			{
				NKCUtil.SetImageColor(item, enhanceSuccessfulRateColor);
			}
			NKCUtil.SetLabelText(m_lbMainSkillLevelBefore, m_BaseOperator.mainSkill.level.ToString());
			NKCUtil.SetLabelText(m_lbMainSkillLevelAfter, (m_BaseOperator.mainSkill.level + 1).ToString());
		}
		else if (NKCOperatorUtil.IsMaximumSkillLevel(m_BaseOperator.mainSkill.id, m_BaseOperator.mainSkill.level))
		{
			NKCUtil.SetLabelText(m_lbMainSkillDesc, NKCUtilString.GET_STRING_OPERATOR_POPUP_SKILL_ENHACE_REJECT_DESC_MAIN_SKILL_MAX_LEVEL);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbMainSkillDesc, NKCUtilString.GET_STRING_OPERATOR_POPUP_SKILL_ENHACE_REJECT_DESC_MAIN_SKILL_NOT_MATCH);
		}
		if (m_bEnhanceSubSkill)
		{
			float skillEnhanceSuccessfulRate = GetSkillEnhanceSuccessfulRate();
			NKCUtil.SetLabelText(m_lbMainSubSuccessfulRate, $"{skillEnhanceSuccessfulRate}%");
			Color enhanceSuccessfulRateColor2 = GetEnhanceSuccessfulRateColor(skillEnhanceSuccessfulRate);
			NKCUtil.SetLabelTextColor(m_lbMainSubSuccessfulRate, enhanceSuccessfulRateColor2);
			foreach (Image item2 in m_lstSubSkillGrow)
			{
				NKCUtil.SetImageColor(item2, enhanceSuccessfulRateColor2);
			}
		}
		else
		{
			if (NKCOperatorUtil.IsMaximumSkillLevel(m_BaseOperator.subSkill.id, m_BaseOperator.subSkill.level))
			{
				NKCUtil.SetLabelText(m_lbSubSkillDesc, NKCUtilString.GET_STRING_OPERATOR_POPUP_SKILL_ENHACE_REJECT_DESC_SUB_SKILL_MAX_LEVEL);
			}
			else
			{
				NKCUtil.SetLabelText(m_lbSubSkillDesc, NKCUtilString.GET_STRING_OPERATOR_POPUP_SKILL_ENHACE_REJECT_DESC_SUB_SKILL_NOT_MATCH);
			}
			NKCUtil.SetGameobjectActive(m_objEventSubSkill, bValue: false);
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_BaseOperator.id);
		NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(ResourceOperator.id);
		if (unitTempletBase != null && unitTempletBase2 != null)
		{
			m_bCanTrasferPassiveSkill = m_BaseOperator.subSkill.id != ResourceOperator.subSkill.id && unitTempletBase.m_NKM_UNIT_GRADE <= unitTempletBase2.m_NKM_UNIT_GRADE;
		}
	}

	private void UpdateSkillTransferUI(NKCOperatorPassiveToken operatorPassiveToen)
	{
		m_bEnhanceMainSkill = false;
		int operatorSkill = NKCOperatorUtil.GetOperatorSkill(m_ResourcePassiveToken);
		if (operatorSkill == 0)
		{
			return;
		}
		m_bEnhanceSubSkill = NKCOperatorUtil.IsCanEnhanceSubSkill(m_BaseOperator, operatorSkill);
		NKCUtil.SetGameobjectActive(m_MatMainSkill.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_objMatMainSkillNone, operatorPassiveToen != null);
		m_MatSubSkill.SetData(operatorSkill, 1);
		NKCUtil.SetLabelText(m_lbMainSkillDesc, NKCUtilString.GET_STRING_OPERATOR_TOKEN_TRANSFER_MAIN_SKILL_NOT_TOKEN);
		if (m_bEnhanceSubSkill)
		{
			float skillEnhanceSuccessfulRate = GetSkillEnhanceSuccessfulRate();
			NKCUtil.SetLabelText(m_lbMainSubSuccessfulRate, $"{skillEnhanceSuccessfulRate}%");
			Color enhanceSuccessfulRateColor = GetEnhanceSuccessfulRateColor(skillEnhanceSuccessfulRate);
			NKCUtil.SetLabelTextColor(m_lbMainSubSuccessfulRate, enhanceSuccessfulRateColor);
			foreach (Image item in m_lstSubSkillGrow)
			{
				NKCUtil.SetImageColor(item, enhanceSuccessfulRateColor);
			}
		}
		else
		{
			if (NKCOperatorUtil.IsMaximumSkillLevel(m_BaseOperator.subSkill.id, m_BaseOperator.subSkill.level))
			{
				NKCUtil.SetLabelText(m_lbSubSkillDesc, NKCUtilString.GET_STRING_OPERATOR_POPUP_SKILL_ENHACE_REJECT_DESC_SUB_SKILL_MAX_LEVEL);
			}
			else
			{
				NKCUtil.SetLabelText(m_lbSubSkillDesc, NKCUtilString.GET_STRING_OPERATOR_POPUP_SKILL_ENHACE_REJECT_DESC_SUB_SKILL_NOT_MATCH);
			}
			NKCUtil.SetGameobjectActive(m_objEventSubSkill, bValue: false);
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_BaseOperator.id);
		if (unitTempletBase != null)
		{
			m_bCanTrasferPassiveSkill = m_BaseOperator.subSkill.id != operatorSkill && NKCOperatorUtil.IsCanTransferGrade(unitTempletBase.m_NKM_UNIT_GRADE, operatorPassiveToen.m_itemGrade);
		}
	}

	private float GetSkillEnhanceSuccessfulRate()
	{
		float changeRatio = 0f;
		float bonusRatio = 0f;
		if (m_curTab == TAB_TYPE.OPER && m_ResourceOperator != null)
		{
			changeRatio = NKCOperatorUtil.GetEnhanceSuccessfulRate(m_ResourceOperator);
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_ResourceOperator.id);
			if (unitTempletBase != null && !m_bCanTrasferPassiveSkill)
			{
				NKCCompanyBuff.IncreaseChargeOperatorSkillEnhanceRatio(NKCScenManager.GetScenManager().GetMyUserData().m_companyBuffDataList, unitTempletBase.m_NKM_UNIT_GRADE, ref changeRatio, ref bonusRatio);
			}
		}
		else if (m_curTab == TAB_TYPE.TOKEN && m_ResourcePassiveToken != null)
		{
			changeRatio = NKCOperatorUtil.GetEnhanceSuccessfulRate(m_ResourcePassiveToken.ItemGrade);
			if (!m_bCanTrasferPassiveSkill)
			{
				NKCCompanyBuff.IncreaseChargeOperatorSkillEnhanceRatio(NKCScenManager.GetScenManager().GetMyUserData().m_companyBuffDataList, m_ResourcePassiveToken.m_itemGrade, ref changeRatio, ref bonusRatio);
			}
		}
		NKCUtil.SetGameobjectActive(m_objEventSubSkill, 0f != bonusRatio);
		NKCUtil.SetLabelText(m_lbEventSubSkill, string.Format(NKCUtilString.GET_EVENT_BUFF_TYPE_OPERATOR_SKILL_INCRESE_RATIO_DESC, bonusRatio));
		return changeRatio;
	}

	private void UpdateSkillTransferInfo()
	{
		if (!m_bCanTrasferPassiveSkill)
		{
			return;
		}
		int num = 0;
		m_BaseSubSkill.SetData(m_BaseOperator.subSkill.id, m_BaseOperator.subSkill.level);
		if (m_curTab == TAB_TYPE.OPER && m_ResourceOperator != null)
		{
			m_MatSubSkill.SetData(m_ResourceOperator.subSkill.id, m_ResourceOperator.subSkill.level);
			num = NKCOperatorUtil.GetTransferSuccessfulRate(m_ResourceOperator);
		}
		else if (m_curTab == TAB_TYPE.TOKEN && m_ResourcePassiveToken != null)
		{
			int operatorSkill = NKCOperatorUtil.GetOperatorSkill(m_ResourcePassiveToken);
			m_MatSubSkill.SetData(operatorSkill, 1);
			num = NKCOperatorUtil.GetTransferSuccessfulRate(m_ResourcePassiveToken.ItemGrade);
		}
		NKCUtil.SetLabelText(m_lbMainSubSuccessfulRate, $"{num}%");
		Color enhanceSuccessfulRateColor = GetEnhanceSuccessfulRateColor(num);
		NKCUtil.SetLabelTextColor(m_lbMainSubSuccessfulRate, enhanceSuccessfulRateColor);
		foreach (Image item in m_lstSubSkillGrow)
		{
			NKCUtil.SetImageColor(item, enhanceSuccessfulRateColor);
		}
	}

	public static Color GetEnhanceSuccessfulRateColor(float successfulRate)
	{
		if (successfulRate >= 100f)
		{
			return NKCUtil.GetColor("#2467F0");
		}
		if (successfulRate >= 70f)
		{
			return NKCUtil.GetColor("#4ABA24");
		}
		if (successfulRate >= 30f)
		{
			return NKCUtil.GetColor("#FF9800");
		}
		return NKCUtil.GetColor("#D40000");
	}

	private void UpdateCostItem()
	{
		m_bEnoughCostItem = false;
		m_iLackItemCnt = 0;
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_BaseOperator.id);
		if (unitTempletBase != null)
		{
			NKMOperatorConstTemplet.HostUnit hostUnit = NKMCommonConst.OperatorConstTemplet.hostUnits.Find((NKMOperatorConstTemplet.HostUnit e) => e.m_NKM_UNIT_GRADE == unitTempletBase.m_NKM_UNIT_GRADE);
			if (hostUnit != null)
			{
				NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(hostUnit.itemId);
				if (itemMiscTempletByID != null)
				{
					NKCUtil.SetImageSprite(m_RESOURCE_ICON, NKCResourceUtility.GetOrLoadMiscItemSmallIcon(itemMiscTempletByID));
				}
				NKMItemMiscData itemMisc = NKCScenManager.CurrentUserData().m_InventoryData.GetItemMisc(hostUnit.itemId);
				int info = hostUnit.itemCount;
				int bounsRatio = 0;
				NKCCompanyBuff.SetDiscountOfOperatorSkillEnhanceCost(NKCScenManager.GetScenManager().GetMyUserData().m_companyBuffDataList, ref info, ref bounsRatio);
				if (itemMisc != null)
				{
					m_bEnoughCostItem = itemMisc.TotalCount >= info;
					m_iLackItemID = hostUnit.itemId;
					m_iLackItemCnt = info - (int)itemMisc.TotalCount;
				}
				else
				{
					m_bEnoughCostItem = false;
					m_iLackItemID = hostUnit.itemId;
					m_iLackItemCnt = info;
				}
				if (m_bEnoughCostItem)
				{
					NKCUtil.SetLabelText(m_RESOURCE_NUMBER_TEXT, info.ToString("#,##0"));
				}
				else
				{
					NKCUtil.SetLabelText(m_RESOURCE_NUMBER_TEXT, string.Format("<color=#ff0000ff>{0}</color>", info.ToString("#,##0")));
				}
				NKCUtil.SetGameobjectActive(m_objEvent, bounsRatio != 0);
				NKCUtil.SetLabelText(m_lbEventDesc, string.Format(NKCUtilString.GET_EVENT_BUFF_TYPE_OPERATOR_SKILL_ENHANCE_DESC, bounsRatio));
				return;
			}
		}
		NKCUtil.SetLabelText(m_RESOURCE_NUMBER_TEXT, "0");
		NKCUtil.SetGameobjectActive(m_objEvent, bValue: false);
	}

	private HashSet<long> GetExclueOperatorUID()
	{
		HashSet<long> hashSet = new HashSet<long> { m_BaseOperator.uid };
		NKMOperator operatorData = NKCOperatorUtil.GetOperatorData(m_BaseOperator.uid);
		if (operatorData != null)
		{
			Dictionary<long, NKMOperator> dicMyOperator = NKCScenManager.CurrentUserData().m_ArmyData.m_dicMyOperator;
			if (NKCOperatorUtil.IsMaximumSkillLevel(operatorData.mainSkill.id, operatorData.mainSkill.level) && NKCOperatorUtil.IsMaximumSkillLevel(operatorData.subSkill.id, operatorData.subSkill.level))
			{
				foreach (KeyValuePair<long, NKMOperator> item in dicMyOperator)
				{
					if (item.Value.id == operatorData.id && item.Value.subSkill.id == operatorData.subSkill.id)
					{
						hashSet.Add(item.Key);
					}
				}
			}
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_BaseOperator.id);
			if (unitTempletBase != null)
			{
				if (NKCOperatorUtil.IsMaximumSkillLevel(operatorData.subSkill.id, operatorData.subSkill.level))
				{
					foreach (KeyValuePair<long, NKMOperator> item2 in dicMyOperator)
					{
						NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(item2.Value.id);
						if (unitTempletBase2 != null && unitTempletBase.m_NKM_UNIT_GRADE > unitTempletBase2.m_NKM_UNIT_GRADE)
						{
							hashSet.Add(item2.Key);
						}
					}
				}
				else
				{
					foreach (KeyValuePair<long, NKMOperator> item3 in dicMyOperator)
					{
						NKMUnitTempletBase unitTempletBase3 = NKMUnitManager.GetUnitTempletBase(item3.Value.id);
						if (unitTempletBase3 != null && unitTempletBase.m_NKM_UNIT_GRADE > unitTempletBase3.m_NKM_UNIT_GRADE && operatorData.subSkill.id != item3.Value.subSkill.id)
						{
							hashSet.Add(item3.Key);
						}
					}
				}
			}
		}
		return hashSet;
	}

	private int SortBySameOperator(NKMUnitData lhs, NKMUnitData rhs)
	{
		if (m_BaseOperator != null && (m_BaseOperator.id == lhs.m_UnitID || m_BaseOperator.id == rhs.m_UnitID))
		{
			if (m_BaseOperator.id == lhs.m_UnitID)
			{
				return -1;
			}
			return 1;
		}
		return 0;
	}

	private int SortBySameSubSkill(NKMOperator lhs, NKMOperator rhs)
	{
		if (m_BaseOperator != null)
		{
			NKMOperator operatorData = NKCOperatorUtil.GetOperatorData(m_BaseOperator.uid);
			if (lhs != null && rhs != null && operatorData != null && (operatorData.subSkill.id == lhs.subSkill.id || operatorData.subSkill.id == rhs.subSkill.id))
			{
				if (operatorData.subSkill.id == lhs.subSkill.id)
				{
					return -1;
				}
				return 1;
			}
		}
		return 0;
	}

	private void UpdateUI(bool bForceUpdate = false)
	{
		if (bForceUpdate)
		{
			Cleanup();
		}
		UpdateSortDataNScrollSetting(bForceUpdate);
		UpdateSlotAni();
		UpdateLeftUI();
		NKCUtil.SetGameobjectActive(m_LoopScrollRect.gameObject, m_curTab == TAB_TYPE.OPER);
		NKCUtil.SetGameobjectActive(m_LoopScrollRect_ForToken.gameObject, m_curTab == TAB_TYPE.TOKEN);
	}

	private void UpdateSortDataNScrollSetting(bool bForceUpdate = false)
	{
		if (m_curTab == TAB_TYPE.OPER && (m_OperatorSortSystem == null || bForceUpdate))
		{
			m_operatorSortOptions.setExcludeOperatorUID = GetExclueOperatorUID();
			m_OperatorSortSystem = new NKCOperatorSort(NKCScenManager.CurrentUserData(), m_operatorSortOptions);
			if (m_SortUI != null)
			{
				m_SortUI.RegisterOperatorSort(m_OperatorSortSystem);
			}
			m_LoopScrollRect.TotalCount = m_OperatorSortSystem.SortedOperatorList.Count;
			m_LoopScrollRect.SetIndexPosition(0);
			m_LoopScrollRect.RefreshCells(bForce: true);
		}
		if (m_curTab == TAB_TYPE.TOKEN && (m_OperatorTokenSortSystem == null || bForceUpdate))
		{
			m_OperTokenSortOption.setExcludeOperatorTokenID = GetExclueOperatorTokenID();
			m_OperatorTokenSortSystem = new NKCOperatorTokenSortSystem(m_OperTokenSortOption);
			if (m_SortUI != null)
			{
				m_SortUI.RegisterOperatorTokenSort(m_OperatorTokenSortSystem);
			}
			m_LoopScrollRect_ForToken.TotalCount = m_OperatorTokenSortSystem.SortedOperatorTokenList.Count;
			m_LoopScrollRect_ForToken.SetIndexPosition(0);
			m_LoopScrollRect_ForToken.RefreshCells(bForce: true);
		}
	}

	private void OnClickMatUnitCancel()
	{
		if ((m_curTab == TAB_TYPE.OPER && m_ResourceOperator == null) || (m_curTab == TAB_TYPE.TOKEN && m_ResourcePassiveToken == null))
		{
			return;
		}
		if (m_curTab == TAB_TYPE.OPER && m_ResourceOperator != null)
		{
			m_SubSkillAni.SetTrigger("unselect");
		}
		if (m_curTab == TAB_TYPE.TOKEN && m_ResourcePassiveToken != null)
		{
			m_ResourceInvenAni.SetTrigger("unselect");
		}
		NKCUtil.SetGameobjectActive(m_BUTTON_CANCEL, bValue: false);
		NKCUtil.SetGameobjectActive(m_ResourceSlot, bValue: false);
		m_ResourceOperator = null;
		m_ResourcePassiveToken = null;
		NKCUtil.SetGameobjectActive(m_objEventSubSkill, bValue: false);
		NKCUtil.SetGameobjectActive(m_objMatMainSkillNone, bValue: false);
		UpdateSlotAni();
		UpdateLeftUI();
		foreach (NKCUIOperatorDeckSelectSlot item in m_lstVisibleSlot)
		{
			item.SetSlotSelectState(NKCUIUnitSelectList.eUnitSlotSelectState.NONE);
		}
		foreach (NKCUISlot item2 in m_lstVisibleTokenSlot)
		{
			item2.SetSelected(bSelected: false);
		}
	}

	private void OnClickPassiveSkillTransfer(bool bSet)
	{
		if (bSet)
		{
			if (!m_bCanTrasferPassiveSkill)
			{
				m_SKILL_IMPLANT_TOGGLE_CHECK.Select(bSelect: false, bForce: true);
				NKCPopupMessageManager.AddPopupMessage((m_curTab == TAB_TYPE.OPER) ? NKCUtilString.GET_STRING_OPERATOR_PASSIVE_SKILL_TRANSFER_BLOCK : NKCUtilString.GET_STRING_OPERATOR_TOKEN_PASSIVE_SKILL_TRANSFER_BLOCK);
				return;
			}
			UpdateSkillTransferInfo();
		}
		UpdateSlotAni();
	}

	private void OnClickUnitTransfer()
	{
		if (m_curTab == TAB_TYPE.OPER && m_ResourceOperator != null)
		{
			if (m_ResourceOperator == null)
			{
				return;
			}
			if (NKCOperatorUtil.GetOperatorData(m_ResourceOperator.uid).bLock)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_OPERATOR_IMPLANT_BLOCK_LOCK_UNIT);
				return;
			}
		}
		if (!m_bEnoughCostItem)
		{
			NKCShopManager.OpenItemLackPopup(m_iLackItemID, m_iLackItemCnt);
			return;
		}
		if (!m_bEnhanceMainSkill && !m_bEnhanceSubSkill)
		{
			if (!m_bCanTrasferPassiveSkill)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_OPERATOR_IMPLANT_BLOCK_NOT_POSSIBLE);
				return;
			}
			if (m_bCanTrasferPassiveSkill && !m_SKILL_IMPLANT_TOGGLE_CHECK.m_bSelect)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_OPERATOR_IMPLANT_TRY_NOTHING);
				return;
			}
		}
		if (!m_bWaitPacketResult)
		{
			if (m_curTab == TAB_TYPE.OPER)
			{
				NKCUIOperatorPopupConfirm.Instance.Open(m_ResourceOperator.uid, m_BaseOperator.id, OnConfirm);
			}
			else if (m_curTab == TAB_TYPE.TOKEN)
			{
				NKCUIOperatorPopupConfirm.Instance.OpenForToken(m_ResourcePassiveToken.ItemID, m_BaseOperator.id, OnConfirm);
			}
		}
	}

	private void OnConfirm()
	{
		m_bWaitPacketResult = true;
		if (m_curTab == TAB_TYPE.OPER)
		{
			NKCPacketSender.Send_NKMPacket_OPERATOR_ENHANCE_REQ(m_BaseOperator.uid, m_ResourceOperator.uid, 0, m_SKILL_IMPLANT_TOGGLE_CHECK.m_bSelect);
		}
		else if (m_curTab == TAB_TYPE.TOKEN)
		{
			NKCPacketSender.Send_NKMPacket_OPERATOR_ENHANCE_REQ(m_BaseOperator.uid, 0L, m_ResourcePassiveToken.ItemID, m_SKILL_IMPLANT_TOGGLE_CHECK.m_bSelect);
		}
	}

	private void OnPointDownMainBaseSkill(PointerEventData eventData)
	{
		if (m_BaseOperator != null)
		{
			NKMOperator operatorData = NKCOperatorUtil.GetOperatorData(m_BaseOperator.uid);
			if (operatorData != null)
			{
				NKCUITooltip.Instance.Open(operatorData.mainSkill.id, operatorData.mainSkill.level, eventData.position);
			}
		}
	}

	private void OnPointDownMainSubSkill(PointerEventData eventData)
	{
		if (m_BaseOperator != null)
		{
			NKMOperator operatorData = NKCOperatorUtil.GetOperatorData(m_BaseOperator.uid);
			if (operatorData != null)
			{
				NKCUITooltip.Instance.Open(operatorData.subSkill.id, operatorData.subSkill.level, eventData.position);
			}
		}
	}

	private void OnPointDownResourceBaseSkill(PointerEventData eventData)
	{
		if (m_ResourceOperator != null)
		{
			NKCUITooltip.Instance.Open(m_ResourceOperator.mainSkill.id, m_ResourceOperator.mainSkill.level, eventData.position);
		}
	}

	private void OnPointDownResourceSubSkill(PointerEventData eventData)
	{
		if (m_ResourceOperator != null)
		{
			NKCUITooltip.Instance.Open(m_ResourceOperator.subSkill.id, m_ResourceOperator.subSkill.level, eventData.position);
		}
	}

	private RectTransform GetSlot(int index)
	{
		if (m_stkOperatorSlotPool.Count > 0)
		{
			NKCUIOperatorDeckSelectSlot nKCUIOperatorDeckSelectSlot = m_stkOperatorSlotPool.Pop();
			NKCUtil.SetGameobjectActive(nKCUIOperatorDeckSelectSlot, bValue: true);
			nKCUIOperatorDeckSelectSlot.transform.localScale = Vector3.one;
			m_lstVisibleSlot.Add(nKCUIOperatorDeckSelectSlot);
			return nKCUIOperatorDeckSelectSlot.GetComponent<RectTransform>();
		}
		NKCUIOperatorDeckSelectSlot newInstance = NKCUIOperatorDeckSelectSlot.GetNewInstance(m_LoopScrollRect.content);
		newInstance.Init();
		NKCUtil.SetGameobjectActive(newInstance, bValue: true);
		newInstance.transform.localScale = Vector3.one;
		m_lstVisibleSlot.Add(newInstance);
		return newInstance.GetComponent<RectTransform>();
	}

	private void ReturnSlot(Transform go)
	{
		NKCUIOperatorDeckSelectSlot component = go.GetComponent<NKCUIOperatorDeckSelectSlot>();
		m_lstVisibleSlot.Remove(component);
		NKCUtil.SetGameobjectActive(go, bValue: false);
		go.SetParent(m_rectSlotPoolRect);
		m_stkOperatorSlotPool.Push(component);
	}

	private void ProvideSlotData(Transform tr, int idx)
	{
		if (m_OperatorSortSystem != null)
		{
			NKCUIOperatorDeckSelectSlot component = tr.GetComponent<NKCUIOperatorDeckSelectSlot>();
			if (!(component == null) && m_OperatorSortSystem.SortedOperatorList.Count > idx)
			{
				NKMOperator curOperatorData = m_OperatorSortSystem.SortedOperatorList[idx];
				component.SetData(m_BaseOperator, curOperatorData, OnSlotSelected);
			}
		}
	}

	private void OnSlotSelected(NKMOperator selectedOperator, NKMUnitTempletBase unitTempletBase, NKMDeckIndex selectedUnitDeckIndex, NKCUnitSortSystem.eUnitState unitSlotState, NKCUIUnitSelectList.eUnitSlotSelectState unitSlotSelectState)
	{
		if (selectedOperator == null)
		{
			return;
		}
		m_ResourceOperator = selectedOperator;
		NKCUtil.SetGameobjectActive(m_SubSkillAni.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_SubSkillAni.gameObject, bValue: true);
		m_SubSkillAni.SetTrigger("select");
		UpdateSlotAni();
		UpdateLeftUI();
		if (m_bCanTrasferPassiveSkill && m_SKILL_IMPLANT_TOGGLE_CHECK.m_bSelect)
		{
			UpdateSkillTransferInfo();
		}
		foreach (NKCUIOperatorDeckSelectSlot item in m_lstVisibleSlot)
		{
			if (item.OperatorUID == m_ResourceOperator.uid)
			{
				item.SetSlotSelectState(NKCUIUnitSelectList.eUnitSlotSelectState.SELECTED);
			}
			else
			{
				item.SetSlotSelectState(NKCUIUnitSelectList.eUnitSlotSelectState.NONE);
			}
		}
	}

	private void OnSortChanged(bool bResetScroll)
	{
		if (m_curTab == TAB_TYPE.OPER && m_OperatorSortSystem != null)
		{
			m_operatorSortOptions = m_OperatorSortSystem.Options;
			m_LoopScrollRect.TotalCount = m_OperatorSortSystem.SortedOperatorList.Count;
			if (bResetScroll)
			{
				m_LoopScrollRect.SetIndexPosition(0);
			}
			else
			{
				m_LoopScrollRect.RefreshCells();
			}
		}
		else if (m_curTab == TAB_TYPE.TOKEN && m_OperatorTokenSortSystem != null)
		{
			m_OperTokenSortOption = m_OperatorTokenSortSystem.Options;
			m_LoopScrollRect_ForToken.TotalCount = m_OperatorTokenSortSystem.SortedOperatorTokenList.Count;
			if (bResetScroll)
			{
				m_LoopScrollRect_ForToken.SetIndexPosition(0);
			}
			else
			{
				m_LoopScrollRect_ForToken.RefreshCells();
			}
		}
	}

	private void UpdateSlotAni()
	{
		if (m_curTab == TAB_TYPE.OPER)
		{
			UpdateSlotAni(m_ResourceOperator);
		}
		else if (m_curTab == TAB_TYPE.TOKEN)
		{
			UpdateSlotAni(m_ResourcePassiveToken);
		}
	}

	private void UpdateSlotAni(NKMOperator resourceOperator)
	{
		NKCUtil.SetGameobjectActive(m_MainSkillSlotAni.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_MainSkillSlotAni.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_SubSkillSlotAni.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_SubSkillSlotAni.gameObject, bValue: true);
		if (resourceOperator == null)
		{
			if (m_bEnhanceMainSkill)
			{
				m_MainSkillSlotAni.SetTrigger(SLOT_ANI_TYPE.ENHANCE_OUT.ToString());
				Debug.Log("<color=red>1. 재료 유닛 x, 이전에 강화가 가능했었음 ==> ENHANCE_OUT </color>");
			}
			if (m_bEnhanceSubSkill)
			{
				m_SubSkillSlotAni.SetTrigger(SLOT_ANI_TYPE.ENHANCE_OUT.ToString());
				Debug.Log("<color=red>2. 재료 유닛 x, 이전에 보조 강화가 가능했었음 ==> ENHANCE_OUT </color>");
			}
			else if (m_bCanTrasferPassiveSkill)
			{
				m_SubSkillSlotAni.SetTrigger(SLOT_ANI_TYPE.SKILL_ON_OUT.ToString());
				Debug.Log("<color=red>3. 재료 유닛 x, 이전에 보조 이식이 가능했었음 ==> SKILL_ON_OUT </color>");
			}
			m_MainSkillSlotAni.SetTrigger(SLOT_ANI_TYPE.NONE.ToString());
			m_SubSkillSlotAni.SetTrigger(SLOT_ANI_TYPE.NONE.ToString());
			m_MainSkillSlotAni.SetBool(SLOT_ANI_TYPE.ISLOCK.ToString(), value: false);
			m_SubSkillSlotAni.SetBool(SLOT_ANI_TYPE.ISLOCK.ToString(), value: false);
			NKCUtil.SetGameobjectActive(m_objMainSkillUpgradeLabel, bValue: false);
		}
		else
		{
			if (m_BaseOperator == null || resourceOperator == null)
			{
				return;
			}
			if (NKCOperatorUtil.IsCanEnhanceMainSkill(m_BaseOperator, resourceOperator))
			{
				m_MainSkillSlotAni.SetBool(SLOT_ANI_TYPE.ISLOCK.ToString(), value: false);
				m_MainSkillSlotAni.SetTrigger(SLOT_ANI_TYPE.ENHANCE_LOOP.ToString());
				Debug.Log("<color=red>4. 재료 유닛 o, 주 스킬 강화가 가능 ==> ENHANCE_LOOP </color>");
			}
			else
			{
				if (m_bEnhanceMainSkill)
				{
					m_MainSkillSlotAni.SetTrigger(SLOT_ANI_TYPE.ENHANCE_OUT.ToString());
					Debug.Log("<color=red>5. 재료 유닛 o, 주 스킬 강화가 가능했었음 지금은 아님 ==> ENHANCE_OUT </color>");
				}
				m_MainSkillSlotAni.SetBool(SLOT_ANI_TYPE.ISLOCK.ToString(), value: true);
			}
			if (NKCOperatorUtil.IsCanEnhanceSubSkill(m_BaseOperator, resourceOperator))
			{
				m_SubSkillSlotAni.SetBool(SLOT_ANI_TYPE.ISLOCK.ToString(), value: false);
				m_SubSkillSlotAni.SetTrigger(SLOT_ANI_TYPE.ENHANCE_LOOP.ToString());
				Debug.Log("<color=red>6. 재료 유닛 o, 보조 스킬 강화가 가능 ==> ENHANCE_LOOP </color>");
				return;
			}
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_BaseOperator.id);
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(resourceOperator.id);
			if (unitTempletBase == null || unitTempletBase2 == null)
			{
				return;
			}
			if (m_BaseOperator.subSkill.id != resourceOperator.subSkill.id && unitTempletBase.m_NKM_UNIT_GRADE <= unitTempletBase2.m_NKM_UNIT_GRADE)
			{
				if (!m_SKILL_IMPLANT_TOGGLE_CHECK.m_bSelect)
				{
					if (m_bCanTrasferPassiveSkill)
					{
						m_SubSkillSlotAni.SetTrigger(SLOT_ANI_TYPE.SKILL_ON_OUT.ToString());
					}
					else
					{
						m_SubSkillSlotAni.SetTrigger(SLOT_ANI_TYPE.NONE.ToString());
					}
					m_SubSkillSlotAni.SetBool(SLOT_ANI_TYPE.ISLOCK.ToString(), value: true);
					Debug.Log("<color=red>7. 재료 유닛 o, 보조 스킬 이식 가능 상태이지만, 이식 버튼 off ==> SKILL_ON_OUT </color>");
				}
				else
				{
					m_SubSkillSlotAni.SetBool(SLOT_ANI_TYPE.ISLOCK.ToString(), value: false);
					m_SubSkillSlotAni.SetTrigger(SLOT_ANI_TYPE.SKILL_ON.ToString());
					Debug.Log("<color=red>8. 재료 유닛 o, 보조 스킬 이식 가능 ==> SKILL_ON </color>");
				}
			}
			else
			{
				if (m_bCanTrasferPassiveSkill)
				{
					m_SubSkillSlotAni.SetTrigger(SLOT_ANI_TYPE.SKILL_ON_OUT.ToString());
					Debug.Log("<color=red>9. 재료 유닛 o, 보조 스킬 이식 가능 했었는데 지금은 아님 ==> SKILL_ON_OUT </color>");
				}
				m_SubSkillSlotAni.SetBool(SLOT_ANI_TYPE.ISLOCK.ToString(), value: true);
			}
		}
	}

	private void UpdateSlotAni(NKCOperatorPassiveToken passiveToken)
	{
		NKCUtil.SetGameobjectActive(m_MainSkillSlotAni.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_MainSkillSlotAni.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_SubSkillSlotAni.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_SubSkillSlotAni.gameObject, bValue: true);
		if (passiveToken == null)
		{
			if (m_bEnhanceSubSkill)
			{
				m_SubSkillSlotAni.SetTrigger(SLOT_ANI_TYPE.ENHANCE_OUT.ToString());
				Debug.Log("<color=red>2. 재료 토큰 x, 이전에 보조 강화가 가능했었음 ==> ENHANCE_OUT </color>");
			}
			else if (m_bCanTrasferPassiveSkill)
			{
				m_SubSkillSlotAni.SetTrigger(SLOT_ANI_TYPE.SKILL_ON_OUT.ToString());
				Debug.Log("<color=red>3. 재료 토큰 x, 이전에 보조 이식이 가능했었음 ==> SKILL_ON_OUT </color>");
			}
			m_MainSkillSlotAni.SetTrigger(SLOT_ANI_TYPE.NONE.ToString());
			m_SubSkillSlotAni.SetTrigger(SLOT_ANI_TYPE.NONE.ToString());
			m_MainSkillSlotAni.SetBool(SLOT_ANI_TYPE.ISLOCK_TOKEN.ToString(), value: false);
			m_SubSkillSlotAni.SetBool(SLOT_ANI_TYPE.ISLOCK.ToString(), value: false);
		}
		else
		{
			if (m_BaseOperator == null || passiveToken == null)
			{
				return;
			}
			m_MainSkillSlotAni.SetBool(SLOT_ANI_TYPE.ISLOCK_TOKEN.ToString(), value: true);
			int operatorSkill = NKCOperatorUtil.GetOperatorSkill(m_ResourcePassiveToken);
			if (operatorSkill == 0)
			{
				return;
			}
			if (NKCOperatorUtil.IsCanEnhanceSubSkill(m_BaseOperator, operatorSkill))
			{
				m_SubSkillSlotAni.SetBool(SLOT_ANI_TYPE.ISLOCK.ToString(), value: false);
				m_SubSkillSlotAni.SetTrigger(SLOT_ANI_TYPE.ENHANCE_LOOP.ToString());
				Debug.Log("<color=red>6. 재료 토큰 o, 보조 스킬 강화가 가능 ==> ENHANCE_LOOP </color>");
				return;
			}
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_BaseOperator.id);
			if (unitTempletBase == null)
			{
				return;
			}
			if (m_BaseOperator.subSkill.id != operatorSkill && NKCOperatorUtil.IsCanTransferGrade(unitTempletBase.m_NKM_UNIT_GRADE, m_ResourcePassiveToken.ItemGrade))
			{
				if (!m_SKILL_IMPLANT_TOGGLE_CHECK.m_bSelect)
				{
					if (m_bCanTrasferPassiveSkill)
					{
						m_SubSkillSlotAni.SetTrigger(SLOT_ANI_TYPE.SKILL_ON_OUT.ToString());
					}
					else
					{
						m_SubSkillSlotAni.SetTrigger(SLOT_ANI_TYPE.NONE.ToString());
					}
					m_SubSkillSlotAni.SetBool(SLOT_ANI_TYPE.ISLOCK.ToString(), value: true);
					Debug.Log("<color=red>7. 재료 토큰 o, 보조 스킬 이식 가능 상태이지만, 이식 버튼 off ==> SKILL_ON_OUT </color>");
				}
				else
				{
					m_SubSkillSlotAni.SetBool(SLOT_ANI_TYPE.ISLOCK.ToString(), value: false);
					m_SubSkillSlotAni.SetTrigger(SLOT_ANI_TYPE.SKILL_ON.ToString());
					Debug.Log("<color=red>8. 재료 토큰 o, 보조 스킬 이식 가능 ==> SKILL_ON </color>");
				}
			}
			else
			{
				if (m_bCanTrasferPassiveSkill)
				{
					m_SubSkillSlotAni.SetTrigger(SLOT_ANI_TYPE.SKILL_ON_OUT.ToString());
					Debug.Log("<color=red>9. 재료 토큰 o, 보조 스킬 이식 가능 했었는데 지금은 아님 ==> SKILL_ON_OUT </color>");
				}
				m_SubSkillSlotAni.SetBool(SLOT_ANI_TYPE.ISLOCK.ToString(), value: true);
			}
		}
	}

	public void OnRecv(bool bTryMainSkill, bool bMainSkillLvUp, bool bTrySubskill, bool bSubskillLvUp, bool bTryImplantSubSkill, bool bImplantSubskill, int oldSubSkillID, int oldSubSkillLv, int PassiveTokenID)
	{
		Debug.Log($"메인강화 시도:{bTryMainSkill}, 결과 {bMainSkillLvUp}\n서브강화 시도:{bTrySubskill}, 결과:{bSubskillLvUp}\n이식시도:{bTryImplantSubSkill},결과:{bImplantSubskill}");
		m_BaseOperator = NKCOperatorUtil.GetOperatorData(m_BaseOperator.uid);
		NKCUtil.SetGameobjectActive(m_objMatMainSkillNone, bValue: false);
		UpdateUI(bForceUpdate: true);
		float fWaitTime = 1f;
		NKCUtil.SetGameobjectActive(m_BaseCardMergeEffect, bValue: false);
		NKCUtil.SetGameobjectActive(m_BaseCardMergeEffect, bValue: true);
		NKCUtil.SetGameobjectActive(m_ResourceCardMergeEffect, bValue: false);
		NKCUtil.SetGameobjectActive(m_ResourceCardMergeEffect, bValue: true);
		NKCUtil.SetGameobjectActive(m_objEventSubSkill, bValue: false);
		StartCoroutine(OnWaitResultUI(fWaitTime, bTryMainSkill, bMainSkillLvUp, bTrySubskill, bSubskillLvUp, bTryImplantSubSkill, bImplantSubskill, oldSubSkillID, oldSubSkillLv));
	}

	private IEnumerator OnWaitResultUI(float fWaitTime, bool bTryMainSkill, bool bMainSkillLvUp, bool bTrySubskill, bool bSubskillLvUp, bool bTryImplantSubSkill, bool bImplantSubskill, int oldSubSkillID, int oldSubSkillLv)
	{
		yield return new WaitForSeconds(fWaitTime);
		NKCOperatorInfoPopupSkillResult.Instance.Open(m_BaseOperator.uid, bTryMainSkill, bMainSkillLvUp, bTrySubskill, bSubskillLvUp, bTryImplantSubSkill, bImplantSubskill, oldSubSkillID, oldSubSkillLv);
	}

	private void Clear()
	{
		foreach (NKCUIOperatorDeckSelectSlot item in m_lstVisibleSlot)
		{
			item.DestoryInstance();
		}
		m_lstVisibleSlot.Clear();
		while (m_stkOperatorSlotPool.Count > 0)
		{
			NKCUIOperatorDeckSelectSlot nKCUIOperatorDeckSelectSlot = m_stkOperatorSlotPool.Pop();
			if (nKCUIOperatorDeckSelectSlot != null)
			{
				nKCUIOperatorDeckSelectSlot.DestoryInstance();
			}
		}
	}

	private RectTransform GetSlotToken(int index)
	{
		if (m_stkTokenSlotPool.Count > 0)
		{
			NKCUISlot nKCUISlot = m_stkTokenSlotPool.Pop();
			NKCUtil.SetGameobjectActive(nKCUISlot, bValue: true);
			nKCUISlot.transform.localScale = Vector3.one;
			m_lstVisibleTokenSlot.Add(nKCUISlot);
			return nKCUISlot.GetComponent<RectTransform>();
		}
		NKCUISlot newInstance = NKCUISlot.GetNewInstance(m_LoopScrollRect_ForToken.content);
		newInstance.Init();
		NKCUtil.SetGameobjectActive(newInstance, bValue: true);
		newInstance.transform.localScale = Vector3.one;
		m_lstVisibleTokenSlot.Add(newInstance);
		return newInstance.GetComponent<RectTransform>();
	}

	private void ReturnSlotToken(Transform go)
	{
		NKCUISlot component = go.GetComponent<NKCUISlot>();
		m_lstVisibleTokenSlot.Remove(component);
		NKCUtil.SetGameobjectActive(go, bValue: false);
		go.SetParent(m_rectSlotPoolRect_ForToken);
		m_stkTokenSlotPool.Push(component);
	}

	private void ProvideSlotDataToken(Transform tr, int idx)
	{
		if (m_OperatorTokenSortSystem != null)
		{
			NKCUISlot component = tr.GetComponent<NKCUISlot>();
			if (!(component == null) && m_OperatorTokenSortSystem.SortedOperatorTokenList.Count > idx)
			{
				NKCOperatorPassiveToken nKCOperatorPassiveToken = m_OperatorTokenSortSystem.SortedOperatorTokenList[idx];
				NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeMiscItemData(nKCOperatorPassiveToken.ItemID, nKCOperatorPassiveToken.ItemCount);
				component.SetMiscItemData(slotData, bShowName: true, bShowCount: true, bEnableLayoutElement: true, OnClickPassiveToken);
				bool tokenArrow = m_BaseOperator.subSkill.id == NKCOperatorUtil.GetOperatorSkill(nKCOperatorPassiveToken.ItemID);
				component.SetTokenArrow(tokenArrow);
			}
		}
	}

	public void OnClickPassiveToken(NKCUISlot.SlotData slotData, bool bLocked)
	{
		if (slotData == null || (m_ResourcePassiveToken != null && m_ResourcePassiveToken.ItemID == slotData.ID))
		{
			return;
		}
		m_ResourcePassiveToken = m_OperatorTokenSortSystem.SortedOperatorTokenList.Find((NKCOperatorPassiveToken x) => x.ItemID == slotData.ID);
		if (m_ResourcePassiveToken == null)
		{
			return;
		}
		NKCUtil.SetGameobjectActive(m_ResourceInvenAni.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_ResourceInvenAni.gameObject, bValue: true);
		m_ResourceInvenAni.SetTrigger("select");
		UpdateSlotAni();
		UpdateLeftUI();
		if (m_bCanTrasferPassiveSkill && m_SKILL_IMPLANT_TOGGLE_CHECK.m_bSelect)
		{
			UpdateSkillTransferInfo();
		}
		foreach (NKCUISlot item in m_lstVisibleTokenSlot)
		{
			if (item.GetSlotData() != null)
			{
				item.SetSelected(item.GetSlotData().ID == m_ResourcePassiveToken.ItemID);
			}
		}
	}

	private HashSet<int> GetExclueOperatorTokenID()
	{
		HashSet<int> hashSet = new HashSet<int>();
		NKMOperator operatorData = NKCOperatorUtil.GetOperatorData(m_BaseOperator.uid);
		if (operatorData != null)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_BaseOperator.id);
			if (unitTempletBase != null)
			{
				if (NKCOperatorUtil.IsMaximumSkillLevel(operatorData.subSkill.id, operatorData.subSkill.level))
				{
					NKM_ITEM_GRADE nKM_ITEM_GRADE = NKCUtil.ConvertUnitGradeToItemGrade(unitTempletBase.m_NKM_UNIT_GRADE);
					NKMOperatorConstTemplet.PassiveToken[] listPassiveToken = NKMCommonConst.OperatorConstTemplet.listPassiveToken;
					foreach (NKMOperatorConstTemplet.PassiveToken passiveToken in listPassiveToken)
					{
						if (passiveToken.m_NKM_ITEM_GRADE >= nKM_ITEM_GRADE)
						{
							continue;
						}
						foreach (int item in passiveToken.ItemID)
						{
							hashSet.Add(item);
						}
					}
				}
				else
				{
					NKM_ITEM_GRADE nKM_ITEM_GRADE2 = NKCUtil.ConvertUnitGradeToItemGrade(unitTempletBase.m_NKM_UNIT_GRADE);
					NKMOperatorConstTemplet.PassiveToken[] listPassiveToken = NKMCommonConst.OperatorConstTemplet.listPassiveToken;
					foreach (NKMOperatorConstTemplet.PassiveToken passiveToken2 in listPassiveToken)
					{
						if (passiveToken2.m_NKM_ITEM_GRADE >= nKM_ITEM_GRADE2)
						{
							continue;
						}
						foreach (int item2 in passiveToken2.ItemID)
						{
							if (operatorData.subSkill.id != NKCOperatorUtil.GetOperatorSkill(item2))
							{
								hashSet.Add(item2);
							}
						}
					}
				}
			}
		}
		return hashSet;
	}
}
