using System.Collections.Generic;
using ClientPacket.User;
using ClientPacket.WorldMap;
using NKC.UI.Collection;
using NKC.UI.NPC;
using NKC.UI.Shop;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIUnitSelectList : NKCUIBase
{
	public enum TargetTabType
	{
		Unit,
		Ship,
		Operator,
		Trophy
	}

	public enum eUnitSlotSelectState
	{
		NONE,
		SELECTED,
		DISABLE,
		DELETE
	}

	public enum eUnitSelectListMode
	{
		Normal,
		CUSTOM_LIST,
		ALLUNIT_DEV,
		ALLSKIN_DEV
	}

	public struct UnitSelectListOptions
	{
		public delegate void OnSlotSetData(NKCUIUnitSelectListSlotBase cUnitSlot, NKMUnitData cNKMUnitData, NKMDeckIndex deckIndex);

		public delegate void OnSlotOperatorSetData(NKCUIUnitSelectListSlotBase cUnitSlot, NKMOperator operatorData, NKMDeckIndex deckIndex);

		public delegate bool OnAutoSelectFilter(NKMUnitData unitData);

		public delegate bool OnSelectedUnitWarning(long unitUID, List<long> selectedUnitList, out string msg);

		public delegate void OnClose();

		public eUnitSelectListMode m_UnitSelectListMode;

		public NKCUnitSortSystem.UnitListOptions m_SortOptions;

		public NKCOperatorSortSystem.OperatorListOptions m_OperatorSortOptions;

		public TargetTabType eTargetUnitType;

		public bool bShowUnitShipChangeMenu;

		public bool m_bHideUnitCount;

		public bool bShowRemoveSlot;

		public bool bMultipleSelect;

		public int iMaxMultipleSelect;

		public string MaxUnitNotSelectMsg;

		public string MaxUnitNotSelectMsg2;

		public OnAutoSelectFilter dOnAutoSelectFilter;

		public bool bUseRemoveSmartAutoSelect;

		public bool bEnableLockUnitSystem;

		public bool bShowHideDeckedUnitMenu;

		public bool bEnableRemoveUnitSystem;

		public string ShopShortcutTargetTab;

		public bool bShowRemoveItem;

		public bool bShowBanMsg;

		public OnSelectedUnitWarning dOnSelectedUnitWarning;

		public bool bCanSelectUnitInMission;

		public bool bOpenedAtRearmExtract;

		public bool bShowFromContractUnit;

		public string strUpsideMenuName;

		public string strEmptyMessage;

		public string strGuideTempletID;

		public NKCUIUpsideMenu.eMode eUpsideMenuMode;

		public OnClose dOnClose;

		public NKMDeckIndex beforeUnitDeckIndex;

		public NKMUnitData beforeUnit;

		public NKMOperator beforeOperator;

		public HashSet<long> setSelectedUnitUID;

		public HashSet<long> setDisabledUnitUID;

		public HashSet<NKCUnitSortSystem.eFilterCategory> setUnitFilterCategory;

		public HashSet<NKCUnitSortSystem.eSortCategory> setUnitSortCategory;

		public HashSet<NKCUnitSortSystem.eFilterCategory> setShipFilterCategory;

		public HashSet<NKCUnitSortSystem.eSortCategory> setShipSortCategory;

		public HashSet<NKCOperatorSortSystem.eFilterCategory> setOperatorFilterCategory;

		public HashSet<NKCOperatorSortSystem.eSortCategory> setOperatorSortCategory;

		public OnSlotSetData dOnSlotSetData;

		public OnSlotOperatorSetData dOnSlotOperatorSetData;

		public long m_IncludeUnitUID;

		public string m_strCachingUIName;

		public bool m_bUseFavorite;

		public bool m_bShowShipBuildShortcut;

		public bool bEnableExtractOperatorSystem;

		public bool bHideUnitMissionStatus;

		public bool bTouchHoldEventToCollection;

		public NKM_DECK_TYPE eDeckType
		{
			get
			{
				return m_SortOptions.eDeckType;
			}
			set
			{
				m_SortOptions.eDeckType = value;
			}
		}

		public HashSet<NKCUnitSortSystem.eFilterOption> setFilterOption
		{
			get
			{
				return m_SortOptions.setFilterOption;
			}
			set
			{
				m_SortOptions.setFilterOption = value;
			}
		}

		public HashSet<NKCOperatorSortSystem.eFilterOption> setOperatorFilterOption
		{
			get
			{
				return m_OperatorSortOptions.setFilterOption;
			}
			set
			{
				m_OperatorSortOptions.setFilterOption = value;
			}
		}

		public List<NKCUnitSortSystem.eSortOption> lstSortOption
		{
			get
			{
				return m_SortOptions.lstSortOption;
			}
			set
			{
				m_SortOptions.lstSortOption = value;
			}
		}

		public List<NKCOperatorSortSystem.eSortOption> lstOperatorSortOption
		{
			get
			{
				return m_OperatorSortOptions.lstSortOption;
			}
			set
			{
				m_OperatorSortOptions.lstSortOption = value;
			}
		}

		public bool bDescending
		{
			get
			{
				return m_SortOptions.bDescending;
			}
			set
			{
				m_SortOptions.bDescending = value;
			}
		}

		public bool bPushBackUnselectable
		{
			get
			{
				return m_SortOptions.bPushBackUnselectable;
			}
			set
			{
				m_SortOptions.bPushBackUnselectable = value;
			}
		}

		public bool bHideDeckedUnit
		{
			get
			{
				return m_SortOptions.bHideDeckedUnit;
			}
			set
			{
				m_SortOptions.bHideDeckedUnit = value;
			}
		}

		public bool bExcludeLockedUnit
		{
			get
			{
				return m_SortOptions.bExcludeLockedUnit;
			}
			set
			{
				m_SortOptions.bExcludeLockedUnit = value;
			}
		}

		public bool bExcludeDeckedUnit
		{
			get
			{
				return m_SortOptions.bExcludeDeckedUnit;
			}
			set
			{
				m_SortOptions.bExcludeDeckedUnit = value;
			}
		}

		public HashSet<long> setExcludeUnitUID
		{
			get
			{
				return m_SortOptions.setExcludeUnitUID;
			}
			set
			{
				m_SortOptions.setExcludeUnitUID = value;
			}
		}

		public HashSet<long> setExcludeOperatorUID
		{
			get
			{
				return m_OperatorSortOptions.setExcludeOperatorUID;
			}
			set
			{
				m_OperatorSortOptions.setExcludeOperatorUID = value;
			}
		}

		public HashSet<int> setExcludeUnitID
		{
			get
			{
				return m_SortOptions.setExcludeUnitID;
			}
			set
			{
				m_SortOptions.setExcludeUnitID = value;
			}
		}

		public HashSet<int> setExcludeUnitBaseID
		{
			get
			{
				return m_SortOptions.setExcludeUnitBaseID;
			}
			set
			{
				m_SortOptions.setExcludeUnitBaseID = value;
			}
		}

		public HashSet<int> setOnlyIncludeUnitID
		{
			get
			{
				return m_SortOptions.setOnlyIncludeUnitID;
			}
			set
			{
				m_SortOptions.setOnlyIncludeUnitID = value;
			}
		}

		public HashSet<int> setOnlyIncludeUnitBaseID
		{
			get
			{
				return m_SortOptions.setOnlyIncludeUnitBaseID;
			}
			set
			{
				m_SortOptions.setOnlyIncludeUnitBaseID = value;
			}
		}

		public HashSet<int> setOnlyIncludeOperatorID
		{
			get
			{
				return m_OperatorSortOptions.setOnlyIncludeOperatorID;
			}
			set
			{
				m_OperatorSortOptions.setOnlyIncludeOperatorID = value;
			}
		}

		public HashSet<int> setDuplicateUnitID
		{
			get
			{
				return m_SortOptions.setDuplicateUnitID;
			}
			set
			{
				m_SortOptions.setDuplicateUnitID = value;
			}
		}

		public bool bIncludeUndeckableUnit
		{
			get
			{
				return m_SortOptions.bIncludeUndeckableUnit;
			}
			set
			{
				m_SortOptions.bIncludeUndeckableUnit = value;
			}
		}

		public UnitSelectListOptions(NKM_UNIT_TYPE unitType, bool _bMultipleSelect, NKM_DECK_TYPE _eDeckType, eUnitSelectListMode UIMode = eUnitSelectListMode.Normal, bool bUseDefaultString = true)
			: this(ConvertTabType(unitType), _bMultipleSelect, _eDeckType, UIMode, bUseDefaultString)
		{
		}

		public UnitSelectListOptions(TargetTabType targetUnitType, bool _bMultipleSelect, NKM_DECK_TYPE _eDeckType, eUnitSelectListMode UIMode = eUnitSelectListMode.Normal, bool bUseDefaultString = true)
		{
			m_UnitSelectListMode = UIMode;
			eTargetUnitType = targetUnitType;
			bShowUnitShipChangeMenu = false;
			m_bHideUnitCount = false;
			bShowRemoveSlot = false;
			bMultipleSelect = _bMultipleSelect;
			iMaxMultipleSelect = 8;
			MaxUnitNotSelectMsg = "";
			MaxUnitNotSelectMsg2 = "";
			dOnAutoSelectFilter = null;
			bUseRemoveSmartAutoSelect = false;
			bEnableLockUnitSystem = false;
			bEnableRemoveUnitSystem = false;
			ShopShortcutTargetTab = null;
			bShowHideDeckedUnitMenu = true;
			bShowRemoveItem = false;
			bShowBanMsg = false;
			m_bUseFavorite = false;
			bHideUnitMissionStatus = false;
			bTouchHoldEventToCollection = false;
			dOnSelectedUnitWarning = null;
			bCanSelectUnitInMission = false;
			bOpenedAtRearmExtract = false;
			bShowFromContractUnit = false;
			bEnableExtractOperatorSystem = false;
			if (bUseDefaultString)
			{
				strUpsideMenuName = NKCUtilString.GET_STRING_UNIT_SELECT;
				strEmptyMessage = NKCUtilString.GET_STRING_UNIT_SELECT_UNIT_NO_EXIST;
			}
			else
			{
				strUpsideMenuName = "";
				strEmptyMessage = "";
			}
			strGuideTempletID = "";
			eUpsideMenuMode = NKCUIUpsideMenu.eMode.Invalid;
			dOnClose = null;
			setSelectedUnitUID = null;
			setDisabledUnitUID = null;
			setUnitFilterCategory = new HashSet<NKCUnitSortSystem.eFilterCategory>();
			setUnitSortCategory = new HashSet<NKCUnitSortSystem.eSortCategory>();
			setShipFilterCategory = new HashSet<NKCUnitSortSystem.eFilterCategory>();
			setShipSortCategory = new HashSet<NKCUnitSortSystem.eSortCategory>();
			setOperatorFilterCategory = new HashSet<NKCOperatorSortSystem.eFilterCategory>();
			setOperatorSortCategory = new HashSet<NKCOperatorSortSystem.eSortCategory>();
			beforeUnitDeckIndex = NKMDeckIndex.None;
			beforeUnit = null;
			beforeOperator = null;
			dOnSlotSetData = null;
			dOnSlotOperatorSetData = null;
			m_SortOptions.eDeckType = _eDeckType;
			m_SortOptions.lstDeckTypeOrder = null;
			m_SortOptions.bHideDeckedUnit = false;
			m_SortOptions.bDescending = true;
			m_SortOptions.bPushBackUnselectable = true;
			List<NKCUnitSortSystem.eSortOption> sortOptions = new List<NKCUnitSortSystem.eSortOption>();
			switch (targetUnitType)
			{
			default:
				m_SortOptions.lstSortOption = NKCUnitSortSystem.AddDefaultSortOptions(sortOptions, NKM_UNIT_TYPE.NUT_NORMAL, bIsCollection: false);
				break;
			case TargetTabType.Operator:
				m_SortOptions.lstSortOption = NKCUnitSortSystem.AddDefaultSortOptions(sortOptions, NKM_UNIT_TYPE.NUT_OPERATOR, bIsCollection: false);
				break;
			case TargetTabType.Ship:
				m_SortOptions.lstSortOption = NKCUnitSortSystem.AddDefaultSortOptions(sortOptions, NKM_UNIT_TYPE.NUT_SHIP, bIsCollection: false);
				break;
			}
			m_SortOptions.lstForceSortOption = null;
			m_SortOptions.lstDefaultSortOption = null;
			m_SortOptions.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
			m_SortOptions.lstCustomSortFunc = new Dictionary<NKCUnitSortSystem.eSortCategory, KeyValuePair<string, NKCUnitSortSystem.NKCDataComparerer<NKMUnitData>.CompareFunc>>();
			m_SortOptions.setOnlyIncludeFilterOption = null;
			m_SortOptions.PreemptiveSortFunc = null;
			m_SortOptions.AdditionalExcludeFilterFunc = null;
			m_SortOptions.bExcludeLockedUnit = false;
			m_SortOptions.bExcludeDeckedUnit = false;
			m_SortOptions.setExcludeUnitUID = null;
			m_SortOptions.setExcludeUnitID = null;
			m_SortOptions.setExcludeUnitBaseID = null;
			m_SortOptions.setOnlyIncludeUnitID = null;
			m_SortOptions.setOnlyIncludeUnitBaseID = null;
			m_SortOptions.setDuplicateUnitID = null;
			m_SortOptions.bIncludeUndeckableUnit = true;
			m_SortOptions.bIncludeSeizure = false;
			m_SortOptions.bUseDeckedState = false;
			m_SortOptions.bUseLockedState = false;
			m_SortOptions.bUseLobbyState = false;
			m_SortOptions.bUseDormInState = false;
			m_SortOptions.bIgnoreCityState = false;
			m_SortOptions.bIgnoreWorldMapLeader = false;
			m_SortOptions.AdditionalUnitStateFunc = null;
			m_SortOptions.bIgnoreMissionState = false;
			m_SortOptions.bUseBanData = false;
			m_SortOptions.bUseUpData = false;
			m_SortOptions.bHideTokenFiltering = false;
			m_IncludeUnitUID = 0L;
			m_strCachingUIName = "";
			m_OperatorSortOptions = default(NKCOperatorSortSystem.OperatorListOptions);
			m_OperatorSortOptions.eDeckType = _eDeckType;
			m_OperatorSortOptions.lstSortOption = new List<NKCOperatorSortSystem.eSortOption>();
			m_OperatorSortOptions.lstSortOption = NKCOperatorSortSystem.AddDefaultSortOptions(m_OperatorSortOptions.lstSortOption, bIsCollection: false);
			m_OperatorSortOptions.lstDefaultSortOption = null;
			m_OperatorSortOptions.setFilterOption = new HashSet<NKCOperatorSortSystem.eFilterOption>();
			m_OperatorSortOptions.lstCustomSortFunc = new Dictionary<NKCOperatorSortSystem.eSortCategory, KeyValuePair<string, NKCUnitSortSystem.NKCDataComparerer<NKMOperator>.CompareFunc>>();
			m_OperatorSortOptions.SetBuildOption(true, BUILD_OPTIONS.DESCENDING, BUILD_OPTIONS.PUSHBACK_UNSELECTABLE, BUILD_OPTIONS.INCLUDE_UNDECKABLE_UNIT);
			m_bShowShipBuildShortcut = false;
		}
	}

	public delegate void OnUnitSelectCommand(List<long> unitUID);

	public delegate void OnUnitSortList(long unitUID, List<NKMUnitData> unitUIDList);

	public delegate void OnOperatorSortList(long unitUID, List<NKMOperator> operatorUIDList);

	public delegate void OnUnitSortOption(NKCUnitSortSystem.UnitListOptions unitOption);

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_unit_select_list";

	private const string UI_ASSET_NAME = "NKM_UI_UNIT_SELECT_LIST";

	private static NKCUIUnitSelectList m_Instance;

	private List<int> RESOURCE_LIST = new List<int>();

	private bool m_bWillCloseUnderPopupOnOpen = true;

	[Header("유닛 슬롯 프리팹 & 사이즈 설정")]
	public NKCUIUnitSelectListSlot m_pfbUnitSlot;

	public Vector2 m_vUnitSlotSize;

	public Vector2 m_vUnitSlotSpacing;

	[Space]
	public NKCUIShipSelectListSlot m_pfbShipSlot;

	public Vector2 m_vShipSlotSize;

	public Vector2 m_vShipSlotSpacing;

	[Space]
	public NKCUIOperatorSelectListSlot m_pfbOperatorSlot;

	public Vector2 m_vOperatorSlotSize;

	public Vector2 m_vOperatorSlotSpacing;

	[Space]
	public NKCUIUnitSelectListSlot m_pfbUnitSlotForCastingBan;

	public Vector2 m_vUnitCastingSlotSize;

	public Vector2 m_vUnitCastingSlotSpacing;

	[Space]
	public NKCUIOperatorSelectListSlot m_pfbOperSlotForCastingBan;

	public Vector2 m_vOperCastingSlotSize;

	public Vector2 m_vOperCastingSlotSpacing;

	[Header("UI Components")]
	public RectTransform m_rectContentRect;

	public RectTransform m_rectSlotPoolRect;

	public LoopScrollRect m_LoopScrollRect;

	public GridLayoutGroup m_GridLayoutGroup;

	public NKCUIComSafeArea m_SafeArea;

	[Header("보유 유닛 카운트")]
	public GameObject m_objRootUnitCount;

	public Text m_lbUnitCountDesc;

	public Text m_lbUnitCount;

	public NKCUIComStateButton m_NKM_UI_UNIT_SELECT_LIST_POSSESS_ADD;

	public GameObject m_objUnitListAddEffect;

	[Header("편성 유닛 감추기 토글")]
	public NKCUIComToggle m_ctgHideDeckedUnit;

	[Header("다중 선택용 확인창")]
	public GameObject m_objMultipleSelectRoot;

	public NKCUIComButton m_cbtnMultipleSelectOK;

	public Text m_lbMultipleSelectCount;

	public Text m_lbMultipleSelectText;

	public GameObject m_objMultipleSelectGetItem;

	public Transform m_trMultipleSelectGetItemListContent;

	public NKCUIComButton m_btnMultiCancel;

	public NKCUIComButton m_btnMultiAuto;

	public NKCUIComButton m_btnMultiAutoN;

	public NKCUIComButton m_btnMultiAutoR;

	public NKCUIUnitSelectListRemovePopup m_popupSmartSelect;

	public Image m_cbtnMultipleSelectOK_img;

	public Text m_cbtnMultipleSelectOK_text;

	public Sprite m_spriteButton_01;

	public Sprite m_spriteButton_02;

	public Sprite m_spriteButton_03;

	[Header("필터/정렬 통합UI")]
	public NKCUIComUnitSortOptions m_SortUI;

	[Header("잠금/해고 설정 버튼")]
	public NKCUIComToggle m_ctgLockUnit;

	public NKCUIComStateButton m_btnRemoveUnit;

	public CanvasGroup m_canvasRemoveUnit;

	public GameObject m_objLockMsg;

	public GameObject m_objRemoveMsg;

	public GameObject m_objBanMsg;

	public Text m_txtRemoveMsg;

	public NKCUIComStateButton m_btnOperatorExtract;

	public GameObject m_objOperatorExtractCost;

	public Image m_imgOperatorExtractCostMiscIcon;

	public NKCComText m_lbOperatorExtractCostCnt;

	[Header("상점 숏컷")]
	public NKCUIComStateButton m_btnShopShortcut;

	public NKCUIComStateButton m_btnShipBuildShortcut;

	[Header("유닛/함선 선택 버튼")]
	public NKCUIComToggle m_tglSelectModeUnit;

	public NKCUIComToggle m_tglSelectModeShip;

	public NKCUIComToggle m_tglSelectModeOperator;

	public NKCUIComToggle m_tglSelectModeTrophy;

	[Header("목록이 비었을 때")]
	public GameObject m_objEmpty;

	public Text m_lbEmptyMessage;

	[Header("오퍼레이터")]
	private NKCOperatorSortSystem m_OperatorSortSystem;

	private List<NKCUIUnitSelectListSlotBase> m_lstVisibleSlot = new List<NKCUIUnitSelectListSlotBase>();

	private Stack<NKCUIUnitSelectListSlotBase> m_stkUnitSlotPool = new Stack<NKCUIUnitSelectListSlotBase>();

	private Stack<NKCUIUnitSelectListSlotBase> m_stkUnitCastingBanSlotPool = new Stack<NKCUIUnitSelectListSlotBase>();

	private Stack<NKCUIUnitSelectListSlotBase> m_stkShipSlotPool = new Stack<NKCUIUnitSelectListSlotBase>();

	private Stack<NKCUIUnitSelectListSlotBase> m_stkOperSlotPool = new Stack<NKCUIUnitSelectListSlotBase>();

	private Stack<NKCUIUnitSelectListSlotBase> m_stkOperCastingBanSlotPool = new Stack<NKCUIUnitSelectListSlotBase>();

	private HashSet<NKCUnitSortSystem.eFilterCategory> m_hsFilterCategory = new HashSet<NKCUnitSortSystem.eFilterCategory>();

	private HashSet<NKCUnitSortSystem.eSortCategory> m_hsSortCategory = new HashSet<NKCUnitSortSystem.eSortCategory>();

	private NKCUnitSortSystem m_ssActive;

	private Dictionary<TargetTabType, NKCUnitSortSystem> m_dicUnitSortSystem = new Dictionary<TargetTabType, NKCUnitSortSystem>();

	private long m_lastSelectedUnitUID;

	private bool m_bKeepSortFilterOptions;

	private NKMUnitData m_BeforeUnit;

	private NKMOperator m_BeforeOperator;

	private NKMDeckIndex m_BeforeUnitDeckIndex;

	private UnitSelectListOptions m_currentOption;

	private List<long> m_listSelectedUnit = new List<long>();

	private List<NKCUISlot> m_listBotSlot = new List<NKCUISlot>();

	private NKCUIRectMove m_NKM_UI_UNIT_SELECT_LIST_UNIT;

	private OnUnitSelectCommand m_dOnUnitSelectCommand;

	private OnUnitSortList m_dOnUnitSortList;

	private OnOperatorSortList m_dOnOperatorSortList;

	private OnUnitSortOption m_dOnUnitSortOption;

	private TargetTabType m_currentTargetUnitType;

	private bool m_bLockModeEnabled;

	private bool m_bCellPrepared;

	private bool m_bPrevMultiple;

	private bool m_bIsReturnCastingBanSlot;

	private bool m_bDataValid;

	private bool m_bInit;

	private const int REMOVE_UNIT_MAX = 1000;

	private const int REMOVE_SHIP_MAX = 100;

	[Header("오픈애니메이터")]
	public Animator m_Animator;

	private bool m_bOpenOperatorExtractMode;

	private List<NKMUnitData> m_currentUnitList = new List<NKMUnitData>();

	private int m_iOperatorExtractCostCount;

	private UnitSelectListOptions m_prevOption = new UnitSelectListOptions(NKM_UNIT_TYPE.NUT_NORMAL, _bMultipleSelect: false, NKM_DECK_TYPE.NDT_NORMAL, eUnitSelectListMode.Normal, bUseDefaultString: false);

	private OnUnitSelectCommand m_prevOnUnitSelectCommand;

	public static NKCUIUnitSelectList Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIUnitSelectList>("ab_ui_nkm_ui_unit_select_list", "NKM_UI_UNIT_SELECT_LIST", NKCUIManager.eUIBaseRect.UIFrontCommon, OnCleanupInstance).GetInstance<NKCUIUnitSelectList>();
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

	public static bool IsInstanceLoaded => m_Instance != null;

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.ON_PLAY_GAME;

	public override string GuideTempletID
	{
		get
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_UNIT_LIST)
			{
				return "ARTICLE_SYSTEM_UNIT_LIST";
			}
			return "";
		}
	}

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode
	{
		get
		{
			if (m_currentOption.eUpsideMenuMode != NKCUIUpsideMenu.eMode.Invalid)
			{
				return m_currentOption.eUpsideMenuMode;
			}
			eUnitSelectListMode unitSelectListMode = m_currentOption.m_UnitSelectListMode;
			if ((uint)(unitSelectListMode - 2) <= 1u)
			{
				return NKCUIUpsideMenu.eMode.Disable;
			}
			return NKCUIUpsideMenu.eMode.Normal;
		}
	}

	public override string MenuName
	{
		get
		{
			if (string.IsNullOrEmpty(m_currentOption.strUpsideMenuName))
			{
				return NKCUtilString.GET_STRING_UNIT_SELECT;
			}
			return m_currentOption.strUpsideMenuName;
		}
	}

	public override List<int> UpsideMenuShowResourceList
	{
		get
		{
			if (IsRemoveMode && m_currentTargetUnitType == TargetTabType.Unit)
			{
				return new List<int> { 1022, 1, 101 };
			}
			if (IsExtractOperatorMode && m_currentTargetUnitType == TargetTabType.Operator)
			{
				return new List<int> { 1, 3, 101 };
			}
			return RESOURCE_LIST;
		}
	}

	public override bool WillCloseUnderPopupOnOpen => m_bWillCloseUnderPopupOnOpen;

	private NKMUserData UserData => NKCScenManager.CurrentUserData();

	private bool IsLockMode
	{
		get
		{
			if (m_currentOption.bEnableLockUnitSystem)
			{
				return m_bLockModeEnabled;
			}
			return false;
		}
	}

	private bool IsRemoveMode
	{
		get
		{
			if (m_currentOption.bEnableRemoveUnitSystem)
			{
				return m_currentOption.bShowRemoveItem;
			}
			return false;
		}
	}

	private bool IsExtractOperatorMode
	{
		get
		{
			if (m_currentOption.bEnableExtractOperatorSystem)
			{
				return m_currentOption.bShowRemoveItem;
			}
			return false;
		}
	}

	private bool IsBanMode => m_currentOption.bShowBanMsg;

	private bool IsOpendAtRearmExtract => m_currentOption.bOpenedAtRearmExtract;

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	public static NKCUIUnitSelectList OpenNewInstance(bool bWillCloseUnderPopupOnOpen = true)
	{
		NKCUIUnitSelectList instance = NKCUIManager.OpenNewInstance<NKCUIUnitSelectList>("ab_ui_nkm_ui_unit_select_list", "NKM_UI_UNIT_SELECT_LIST", NKCUIManager.eUIBaseRect.UIFrontCommon, null).GetInstance<NKCUIUnitSelectList>();
		if (instance != null)
		{
			instance.InitUI();
		}
		instance.m_bWillCloseUnderPopupOnOpen = bWillCloseUnderPopupOnOpen;
		return instance;
	}

	public static void OnCleanupInstance()
	{
		m_Instance = null;
	}

	private static TargetTabType ConvertTabType(NKM_UNIT_TYPE unitType)
	{
		return unitType switch
		{
			NKM_UNIT_TYPE.NUT_OPERATOR => TargetTabType.Operator, 
			NKM_UNIT_TYPE.NUT_SHIP => TargetTabType.Ship, 
			_ => TargetTabType.Unit, 
		};
	}

	private static NKM_UNIT_TYPE GetUnitType(TargetTabType tabType)
	{
		return tabType switch
		{
			TargetTabType.Operator => NKM_UNIT_TYPE.NUT_OPERATOR, 
			TargetTabType.Ship => NKM_UNIT_TYPE.NUT_SHIP, 
			_ => NKM_UNIT_TYPE.NUT_NORMAL, 
		};
	}

	private NKCUnitSortSystem GetUnitSortSystem(TargetTabType type)
	{
		if (m_dicUnitSortSystem.ContainsKey(type) && m_dicUnitSortSystem[type] != null)
		{
			NKCUnitSortSystem nKCUnitSortSystem = m_dicUnitSortSystem[type];
			nKCUnitSortSystem.BuildFilterAndSortedList(m_currentOption.setFilterOption, m_currentOption.lstSortOption, m_currentOption.bHideDeckedUnit);
			switch (type)
			{
			case TargetTabType.Ship:
				m_SortUI.RegisterCategories(m_currentOption.setShipFilterCategory, m_currentOption.setShipSortCategory, m_currentOption.setFilterOption.Contains(NKCUnitSortSystem.eFilterOption.Favorite));
				break;
			default:
				m_SortUI.RegisterCategories(m_currentOption.setUnitFilterCategory, m_currentOption.setUnitSortCategory, m_currentOption.setFilterOption.Contains(NKCUnitSortSystem.eFilterOption.Favorite));
				break;
			case TargetTabType.Trophy:
				m_SortUI.RegisterCategories(NKCUnitSortSystem.setDefaultTrophyFilterCategory, NKCUnitSortSystem.setDefaultTrophySortCategory, bFavoriteFilterActive: false);
				break;
			}
			m_SortUI.RegisterUnitSort(nKCUnitSortSystem);
			m_SortUI.ResetUI(m_currentOption.m_bUseFavorite && (type == TargetTabType.Unit || type == TargetTabType.Trophy));
			return nKCUnitSortSystem;
		}
		NKCUnitSortSystem nKCUnitSortSystem2;
		switch (m_currentOption.m_UnitSelectListMode)
		{
		default:
			switch (type)
			{
			default:
				nKCUnitSortSystem2 = new NKCUnitSort(NKCScenManager.CurrentUserData(), m_currentOption.m_SortOptions);
				break;
			case TargetTabType.Ship:
				nKCUnitSortSystem2 = new NKCShipSort(NKCScenManager.CurrentUserData(), m_currentOption.m_SortOptions);
				break;
			case TargetTabType.Trophy:
				nKCUnitSortSystem2 = new NKCGenericUnitSort(NKCScenManager.CurrentUserData(), m_currentOption.m_SortOptions, NKCScenManager.CurrentUserData().m_ArmyData.m_dicMyTrophy.Values);
				nKCUnitSortSystem2.lstSortOption = new List<NKCUnitSortSystem.eSortOption>
				{
					NKCUnitSortSystem.eSortOption.Rarity_High,
					NKCUnitSortSystem.eSortOption.UID_First
				};
				break;
			}
			break;
		case eUnitSelectListMode.CUSTOM_LIST:
		{
			List<NKMUnitData> list3 = new List<NKMUnitData>();
			foreach (int item in m_currentOption.setOnlyIncludeUnitID)
			{
				list3.Add(NKCUnitSortSystem.MakeTempUnitData(item, 1, 0));
			}
			nKCUnitSortSystem2 = new NKCGenericUnitSort(NKCScenManager.CurrentUserData(), m_currentOption.m_SortOptions, list3);
			break;
		}
		case eUnitSelectListMode.ALLUNIT_DEV:
			switch (type)
			{
			default:
				nKCUnitSortSystem2 = new NKCAllUnitSort(NKCScenManager.CurrentUserData(), m_currentOption.m_SortOptions);
				break;
			case TargetTabType.Ship:
				nKCUnitSortSystem2 = new NKCAllShipSort(NKCScenManager.CurrentUserData(), m_currentOption.m_SortOptions);
				break;
			case TargetTabType.Trophy:
			{
				List<NKMUnitData> list2 = new List<NKMUnitData>();
				foreach (NKMUnitTempletBase value in NKMTempletContainer<NKMUnitTempletBase>.Values)
				{
					if (value.IsTrophy)
					{
						list2.Add(NKCUnitSortSystem.MakeTempUnitData(value.m_UnitID, 1, 0));
					}
				}
				nKCUnitSortSystem2 = new NKCGenericUnitSort(NKCScenManager.CurrentUserData(), m_currentOption.m_SortOptions, list2);
				break;
			}
			}
			break;
		case eUnitSelectListMode.ALLSKIN_DEV:
			switch (type)
			{
			default:
			{
				List<NKMUnitData> list = new List<NKMUnitData>();
				foreach (NKMSkinTemplet value2 in NKMSkinManager.m_dicSkinTemplet.Values)
				{
					list.Add(NKCUnitSortSystem.MakeTempUnitData(value2, 1, 0));
				}
				nKCUnitSortSystem2 = new NKCGenericUnitSort(NKCScenManager.CurrentUserData(), m_currentOption.m_SortOptions, list);
				break;
			}
			case TargetTabType.Ship:
				nKCUnitSortSystem2 = new NKCAllShipSort(NKCScenManager.CurrentUserData(), m_currentOption.m_SortOptions);
				break;
			case TargetTabType.Trophy:
				nKCUnitSortSystem2 = new NKCGenericUnitSort(NKCScenManager.CurrentUserData(), m_currentOption.m_SortOptions, new List<NKMUnitData>());
				break;
			}
			break;
		}
		if (m_bKeepSortFilterOptions)
		{
			List<NKCUnitSortSystem.eSortOption> unitSortOption = m_SortUI.GetUnitSortOption();
			if (unitSortOption != null)
			{
				nKCUnitSortSystem2.lstSortOption = unitSortOption;
			}
			HashSet<NKCUnitSortSystem.eFilterOption> unitFilterOption = m_SortUI.GetUnitFilterOption();
			if (unitFilterOption != null)
			{
				nKCUnitSortSystem2.FilterSet = unitFilterOption;
			}
		}
		m_dicUnitSortSystem[type] = nKCUnitSortSystem2;
		switch (type)
		{
		default:
			m_SortUI.RegisterCategories(m_currentOption.setUnitFilterCategory, m_currentOption.setUnitSortCategory, m_currentOption.setFilterOption.Contains(NKCUnitSortSystem.eFilterOption.Favorite));
			break;
		case TargetTabType.Ship:
			m_SortUI.RegisterCategories(m_currentOption.setShipFilterCategory, m_currentOption.setShipSortCategory, m_currentOption.setFilterOption.Contains(NKCUnitSortSystem.eFilterOption.Favorite));
			break;
		case TargetTabType.Trophy:
			m_SortUI.RegisterCategories(NKCUnitSortSystem.setDefaultTrophyFilterCategory, NKCUnitSortSystem.setDefaultTrophySortCategory, bFavoriteFilterActive: false);
			break;
		}
		m_SortUI.RegisterUnitSort(nKCUnitSortSystem2);
		m_SortUI.ResetUI(m_currentOption.m_bUseFavorite && (type == TargetTabType.Unit || type == TargetTabType.Trophy));
		return nKCUnitSortSystem2;
	}

	private NKCOperatorSortSystem GetOperatorSortSystem()
	{
		if (m_OperatorSortSystem != null)
		{
			m_OperatorSortSystem.BuildFilterAndSortedList(m_OperatorSortSystem.FilterSet, m_OperatorSortSystem.lstSortOption, m_currentOption.bHideDeckedUnit);
			m_SortUI.RegisterCategories(m_currentOption.setOperatorFilterCategory, m_currentOption.setOperatorSortCategory, m_currentOption.setFilterOption.Contains(NKCUnitSortSystem.eFilterOption.Favorite));
			m_SortUI.RegisterOperatorSort(m_OperatorSortSystem);
			m_SortUI.ResetUI();
			return m_OperatorSortSystem;
		}
		NKCOperatorSortSystem nKCOperatorSortSystem;
		switch (m_currentOption.m_UnitSelectListMode)
		{
		default:
			nKCOperatorSortSystem = new NKCOperatorSort(NKCScenManager.CurrentUserData(), m_currentOption.m_OperatorSortOptions);
			break;
		case eUnitSelectListMode.CUSTOM_LIST:
		{
			List<NKMOperator> list = new List<NKMOperator>();
			foreach (int item in m_currentOption.setOnlyIncludeUnitID)
			{
				list.Add(NKCOperatorUtil.GetDummyOperator(item));
			}
			nKCOperatorSortSystem = new NKCGenericOperatorSort(NKCScenManager.CurrentUserData(), m_currentOption.m_OperatorSortOptions, list);
			break;
		}
		case eUnitSelectListMode.ALLUNIT_DEV:
			nKCOperatorSortSystem = new NKCAllOperatorSort(NKCScenManager.CurrentUserData(), m_currentOption.m_OperatorSortOptions);
			break;
		case eUnitSelectListMode.ALLSKIN_DEV:
			nKCOperatorSortSystem = new NKCAllOperatorSort(NKCScenManager.CurrentUserData(), m_currentOption.m_OperatorSortOptions);
			break;
		}
		if (m_bKeepSortFilterOptions)
		{
			List<NKCOperatorSortSystem.eSortOption> operatorSortOption = m_SortUI.GetOperatorSortOption();
			if (operatorSortOption != null)
			{
				nKCOperatorSortSystem.lstSortOption = operatorSortOption;
			}
			HashSet<NKCOperatorSortSystem.eFilterOption> operatorFilterOption = m_SortUI.GetOperatorFilterOption();
			if (operatorFilterOption != null)
			{
				nKCOperatorSortSystem.FilterSet = operatorFilterOption;
			}
		}
		m_OperatorSortSystem = nKCOperatorSortSystem;
		m_SortUI.RegisterCategories(m_currentOption.setOperatorFilterCategory, m_currentOption.setOperatorSortCategory, m_currentOption.setFilterOption.Contains(NKCUnitSortSystem.eFilterOption.Favorite));
		m_SortUI.RegisterOperatorSort(nKCOperatorSortSystem);
		m_SortUI.ResetUI();
		return nKCOperatorSortSystem;
	}

	private RectTransform GetSlot(int index)
	{
		Stack<NKCUIUnitSelectListSlotBase> stack;
		NKCUIUnitSelectListSlotBase original;
		switch (m_currentTargetUnitType)
		{
		case TargetTabType.Ship:
			stack = m_stkShipSlotPool;
			original = m_pfbShipSlot;
			break;
		case TargetTabType.Unit:
		case TargetTabType.Trophy:
			if (m_currentOption.bShowBanMsg)
			{
				original = m_pfbUnitSlotForCastingBan;
				stack = m_stkUnitCastingBanSlotPool;
			}
			else
			{
				original = m_pfbUnitSlot;
				stack = m_stkUnitSlotPool;
			}
			break;
		case TargetTabType.Operator:
			if (m_currentOption.bShowBanMsg)
			{
				original = m_pfbOperSlotForCastingBan;
				stack = m_stkOperCastingBanSlotPool;
			}
			else
			{
				stack = m_stkOperSlotPool;
				original = m_pfbOperatorSlot;
			}
			break;
		default:
			return null;
		}
		if (stack.Count > 0)
		{
			NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase = stack.Pop();
			NKCUtil.SetGameobjectActive(nKCUIUnitSelectListSlotBase, bValue: true);
			nKCUIUnitSelectListSlotBase.transform.localScale = Vector3.one;
			m_lstVisibleSlot.Add(nKCUIUnitSelectListSlotBase);
			return nKCUIUnitSelectListSlotBase.GetComponent<RectTransform>();
		}
		NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase2 = Object.Instantiate(original);
		nKCUIUnitSelectListSlotBase2.Init();
		NKCUtil.SetGameobjectActive(nKCUIUnitSelectListSlotBase2, bValue: true);
		nKCUIUnitSelectListSlotBase2.transform.localScale = Vector3.one;
		m_lstVisibleSlot.Add(nKCUIUnitSelectListSlotBase2);
		return nKCUIUnitSelectListSlotBase2.GetComponent<RectTransform>();
	}

	private void ReturnSlot(Transform go)
	{
		NKCUIUnitSelectListSlotBase component = go.GetComponent<NKCUIUnitSelectListSlotBase>();
		m_lstVisibleSlot.Remove(component);
		go.SetParent(m_rectSlotPoolRect);
		if (component is NKCUIUnitSelectListSlot)
		{
			if (m_bIsReturnCastingBanSlot)
			{
				m_stkUnitCastingBanSlotPool.Push(component);
			}
			else
			{
				m_stkUnitSlotPool.Push(component);
			}
		}
		else if (component is NKCUIShipSelectListSlot)
		{
			m_stkShipSlotPool.Push(component);
		}
		else if (component is NKCUIOperatorSelectListSlot)
		{
			if (m_bIsReturnCastingBanSlot)
			{
				m_stkOperCastingBanSlotPool.Push(component);
			}
			else
			{
				m_stkOperSlotPool.Push(component);
			}
		}
	}

	private void ProvideSlotData(Transform tr, int idx)
	{
		if (m_currentTargetUnitType == TargetTabType.Operator)
		{
			if (m_OperatorSortSystem == null)
			{
				Debug.LogError("Slot Operator Sort System Null!!");
				return;
			}
		}
		else if (m_ssActive == null)
		{
			Debug.LogError("Slot Sort System Null!!");
			return;
		}
		NKCUIUnitSelectListSlotBase component = tr.GetComponent<NKCUIUnitSelectListSlotBase>();
		if (component == null)
		{
			return;
		}
		if (m_currentOption.bShowRemoveSlot)
		{
			if (idx == 0)
			{
				if (m_currentTargetUnitType == TargetTabType.Operator)
				{
					component.SetEmpty(bEnableLayoutElement: true, null, OnOperatorSlotSelected);
				}
				else
				{
					component.SetEmpty(bEnableLayoutElement: true, OnSlotSelected);
				}
				return;
			}
			idx--;
		}
		if (m_currentTargetUnitType == TargetTabType.Operator)
		{
			if (m_OperatorSortSystem.SortedOperatorList.Count > idx)
			{
				NKMOperator nKMOperator = m_OperatorSortSystem.SortedOperatorList[idx];
				component.SetEnableShowCastingBanSelectedObject(m_currentOption.bShowBanMsg);
				SetSlotData(component, nKMOperator);
				if (IsRemoveMode)
				{
					component.SetContractedUnitMark(nKMOperator?.fromContract ?? false);
				}
			}
		}
		else
		{
			if (m_ssActive.SortedUnitList.Count <= idx)
			{
				return;
			}
			NKMUnitData nKMUnitData = m_ssActive.SortedUnitList[idx];
			component.SetEnableShowCastingBanSelectedObject(m_currentOption.bShowBanMsg);
			component.SetEnableShowBan(m_currentOption.m_SortOptions.bUseBanData);
			component.SetEnableShowUpUnit(m_currentOption.m_SortOptions.bUseUpData);
			SetSlotData(component, nKMUnitData);
			bool flag = m_SortUI.IsLimitBreakState();
			bool flag2 = m_SortUI.IsTacticUpdateState();
			if ((m_currentTargetUnitType == TargetTabType.Unit && flag) || flag2)
			{
				NKCUIUnitSelectListSlot nKCUIUnitSelectListSlot = component as NKCUIUnitSelectListSlot;
				if (null != nKCUIUnitSelectListSlot)
				{
					if (flag)
					{
						int limitBreakCache = m_ssActive.GetLimitBreakCache(nKMUnitData.m_UnitUID);
						nKCUIUnitSelectListSlot.SetLimitPossibleMark(limitBreakCache >= 0, NKMUnitLimitBreakManager.IsMaxLimitBreak(nKMUnitData, 0));
					}
					if (flag2)
					{
						int tacticUpdateCache = m_ssActive.GetTacticUpdateCache(nKMUnitData.m_UnitUID);
						nKCUIUnitSelectListSlot.SetTacticPossibleMark(tacticUpdateCache);
					}
				}
			}
			if (IsRemoveMode || IsOpendAtRearmExtract || m_currentOption.bShowFromContractUnit)
			{
				component.SetContractedUnitMark(nKMUnitData?.FromContract ?? false);
			}
		}
	}

	private void SetSlotData(NKCUIUnitSelectListSlotBase slot, NKMUnitData unitData)
	{
		long unitUID = unitData.m_UnitUID;
		NKMDeckIndex deckIndexCacheByOption = m_ssActive.GetDeckIndexCacheByOption(unitUID, !m_currentOption.m_SortOptions.bUseDeckedState);
		int unitOfficeIDCacheByOption = m_ssActive.GetUnitOfficeIDCacheByOption(unitUID);
		slot.SetData(unitData, deckIndexCacheByOption, bEnableLayoutElement: true, OnSlotSelected, unitOfficeIDCacheByOption);
		if (NKCScenManager.CurrentUserData() != null)
		{
			slot.SetRecall(NKCRecallManager.IsRecallTargetUnit(unitData, NKCSynchronizedTime.GetServerUTCTime()));
		}
		slot.SetLock(unitData.m_bLock, m_bLockModeEnabled);
		slot.SetFavorite(unitData);
		if (m_ssActive.lstSortOption.Count > 0)
		{
			switch (m_ssActive.lstSortOption[0])
			{
			case NKCUnitSortSystem.eSortOption.Power_Low:
			case NKCUnitSortSystem.eSortOption.Power_High:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Power_High, m_ssActive.GetUnitPowerCache(unitUID), "N0");
				break;
			case NKCUnitSortSystem.eSortOption.Attack_Low:
			case NKCUnitSortSystem.eSortOption.Attack_High:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Attack_High, m_ssActive.GetUnitAttackCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Health_Low:
			case NKCUnitSortSystem.eSortOption.Health_High:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Health_High, m_ssActive.GetUnitHPCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Unit_Defense_Low:
			case NKCUnitSortSystem.eSortOption.Unit_Defense_High:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Unit_Defense_High, m_ssActive.GetUnitDefCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Unit_Crit_Low:
			case NKCUnitSortSystem.eSortOption.Unit_Crit_High:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Unit_Crit_High, m_ssActive.GetUnitCritCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Unit_Hit_Low:
			case NKCUnitSortSystem.eSortOption.Unit_Hit_High:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Unit_Hit_High, m_ssActive.GetUnitHitCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Unit_Evade_Low:
			case NKCUnitSortSystem.eSortOption.Unit_Evade_High:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Unit_Evade_High, m_ssActive.GetUnitEvadeCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Unit_ReduceSkillCool_Low:
			case NKCUnitSortSystem.eSortOption.Unit_ReduceSkillCool_High:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Unit_ReduceSkillCool_High, m_ssActive.GetUnitSkillCoolCache(unitUID));
				break;
			case NKCUnitSortSystem.eSortOption.Unit_Loyalty_High:
			case NKCUnitSortSystem.eSortOption.Unit_Loyalty_Low:
				slot.SetSortingTypeValue(bSet: true, NKCUnitSortSystem.eSortOption.Unit_Loyalty_High, m_ssActive.GetLoyaltyCache(unitUID));
				break;
			default:
				slot.SetSortingTypeValue(bSet: false);
				break;
			}
		}
		else
		{
			slot.SetSortingTypeValue(bSet: false);
		}
		bool flag = m_SortUI.IsTacticUpdateState();
		if (m_currentTargetUnitType == TargetTabType.Unit && flag)
		{
			NKCUIUnitSelectListSlot nKCUIUnitSelectListSlot = slot as NKCUIUnitSelectListSlot;
			if (null != nKCUIUnitSelectListSlot)
			{
				int tacticUpdateCache = m_ssActive.GetTacticUpdateCache(unitData.m_UnitUID);
				nKCUIUnitSelectListSlot.SetTacticPossibleMark(tacticUpdateCache);
			}
		}
		slot.SetSlotState(m_ssActive.GetUnitSlotState(unitData.m_UnitUID));
		eUnitSlotSelectState eUnitSlotSelectState = eUnitSlotSelectState.NONE;
		eUnitSlotSelectState = (m_currentOption.bMultipleSelect ? (m_listSelectedUnit.Contains(unitUID) ? ((!m_currentOption.bShowRemoveItem) ? eUnitSlotSelectState.SELECTED : eUnitSlotSelectState.DELETE) : ((m_currentOption.iMaxMultipleSelect <= m_listSelectedUnit.Count) ? eUnitSlotSelectState.DISABLE : eUnitSlotSelectState.NONE)) : eUnitSlotSelectState.NONE);
		slot.SetSlotSelectState(eUnitSlotSelectState);
		NKMWorldMapManager.WorldMapLeaderState cityStateCache = m_ssActive.GetCityStateCache(unitUID);
		if (m_currentOption.bHideUnitMissionStatus)
		{
			slot.SetCityMissionStatus(value: false);
			slot.SetCityLeaderMark(value: false);
		}
		else
		{
			slot.SetCityLeaderMark(!m_currentOption.bOpenedAtRearmExtract && cityStateCache != NKMWorldMapManager.WorldMapLeaderState.None);
		}
		if (m_currentOption.bTouchHoldEventToCollection)
		{
			slot.SetTouchHoldEvent(OpenUnitCollcetionInfo);
		}
		else
		{
			slot.ClearTouchHoldEvent();
		}
		m_currentOption.dOnSlotSetData?.Invoke(slot, unitData, deckIndexCacheByOption);
	}

	private void SetSlotData(NKCUIUnitSelectListSlotBase slot, NKMOperator operatorData)
	{
		long uid = operatorData.uid;
		NKMDeckIndex deckIndexCache = m_OperatorSortSystem.GetDeckIndexCache(uid, !m_currentOption.m_OperatorSortOptions.IsHasBuildOption(BUILD_OPTIONS.USE_DECKED_STATE));
		if (slot is NKCUIOperatorSelectListSlot && m_currentOption.bShowBanMsg)
		{
			slot.SetDataForBan(operatorData, bEnableLayoutElement: true, OnOperatorSlotSelected);
		}
		else
		{
			slot.SetData(operatorData, deckIndexCache, bEnableLayoutElement: true, OnOperatorSlotSelected);
		}
		slot.SetLock(operatorData.bLock, m_bLockModeEnabled);
		slot.SetFavorite(operatorData);
		if (m_OperatorSortSystem.lstSortOption.Count > 0)
		{
			switch (m_OperatorSortSystem.lstSortOption[0])
			{
			case NKCOperatorSortSystem.eSortOption.Power_Low:
			case NKCOperatorSortSystem.eSortOption.Power_High:
				slot.SetSortingTypeValue(bSet: true, NKCOperatorSortSystem.eSortOption.Power_High, m_OperatorSortSystem.GetUnitPowerCache(uid), "N0");
				break;
			case NKCOperatorSortSystem.eSortOption.Attack_Low:
			case NKCOperatorSortSystem.eSortOption.Attack_High:
				slot.SetSortingTypeValue(bSet: true, NKCOperatorSortSystem.eSortOption.Attack_High, m_OperatorSortSystem.GetUnitAttackCache(uid));
				break;
			case NKCOperatorSortSystem.eSortOption.Health_Low:
			case NKCOperatorSortSystem.eSortOption.Health_High:
				slot.SetSortingTypeValue(bSet: true, NKCOperatorSortSystem.eSortOption.Health_High, m_OperatorSortSystem.GetUnitHPCache(uid));
				break;
			case NKCOperatorSortSystem.eSortOption.Unit_Defense_Low:
			case NKCOperatorSortSystem.eSortOption.Unit_Defense_High:
				slot.SetSortingTypeValue(bSet: true, NKCOperatorSortSystem.eSortOption.Unit_Defense_High, m_OperatorSortSystem.GetUnitDefCache(uid));
				break;
			case NKCOperatorSortSystem.eSortOption.Unit_ReduceSkillCool_Low:
			case NKCOperatorSortSystem.eSortOption.Unit_ReduceSkillCool_High:
				slot.SetSortingTypeValue(bSet: true, NKCOperatorSortSystem.eSortOption.Unit_ReduceSkillCool_High, m_OperatorSortSystem.GetUnitSkillCoolCache(uid));
				break;
			default:
				slot.SetSortingTypeValue(bSet: false);
				break;
			}
		}
		else
		{
			slot.SetSortingTypeValue(bSet: false);
		}
		slot.SetSlotState(m_OperatorSortSystem.GetUnitSlotState(operatorData.uid));
		eUnitSlotSelectState eUnitSlotSelectState = eUnitSlotSelectState.NONE;
		eUnitSlotSelectState = (m_currentOption.bMultipleSelect ? (m_listSelectedUnit.Contains(uid) ? ((!m_currentOption.bShowRemoveItem) ? eUnitSlotSelectState.SELECTED : eUnitSlotSelectState.DELETE) : ((m_currentOption.iMaxMultipleSelect <= m_listSelectedUnit.Count) ? eUnitSlotSelectState.DISABLE : eUnitSlotSelectState.NONE)) : eUnitSlotSelectState.NONE);
		slot.SetSlotSelectState(eUnitSlotSelectState);
		slot.SetCityLeaderMark(value: false);
		m_currentOption.dOnSlotOperatorSetData?.Invoke(slot, operatorData, deckIndexCache);
	}

	private void OpenUnitCollcetionInfo(NKMUnitData unitData)
	{
		if (unitData != null)
		{
			List<NKMEquipItemData> list = new List<NKMEquipItemData>();
			NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
			if (inventoryData != null)
			{
				list.Add(inventoryData.GetItemEquip(unitData.GetEquipItemWeaponUid()));
				list.Add(inventoryData.GetItemEquip(unitData.GetEquipItemDefenceUid()));
				list.Add(inventoryData.GetItemEquip(unitData.GetEquipItemAccessoryUid()));
				list.Add(inventoryData.GetItemEquip(unitData.GetEquipItemAccessory2Uid()));
			}
			NKCUICollectionUnitInfoV2.CheckInstanceAndOpen(unitData, null, list, NKCUICollectionUnitInfoV2.eCollectionState.CS_STATUS, isGauntlet: false, NKCUIUpsideMenu.eMode.BackButtonOnly);
		}
	}

	public void InitUI()
	{
		if (!m_bInit)
		{
			m_bInit = true;
			m_LoopScrollRect.dOnGetObject += GetSlot;
			m_LoopScrollRect.dOnReturnObject += ReturnSlot;
			m_LoopScrollRect.dOnProvideData += ProvideSlotData;
			m_LoopScrollRect.dOnRepopulate += CalculateContentRectSize;
			NKCUtil.SetScrollHotKey(m_LoopScrollRect);
			m_SortUI.Init(OnSortChanged, bIsCollection: false);
			NKCUtil.SetToggleValueChangedDelegate(m_tglSelectModeUnit, OnSelectUnitMode);
			NKCUtil.SetToggleValueChangedDelegate(m_tglSelectModeShip, OnSelectShipMode);
			NKCUtil.SetToggleValueChangedDelegate(m_tglSelectModeOperator, OnSelectOperatorMode);
			NKCUtil.SetToggleValueChangedDelegate(m_tglSelectModeTrophy, OnSelectTrophyMode);
			NKCUtil.SetGameobjectActive(m_tglSelectModeOperator, !NKCOperatorUtil.IsHide());
			m_cbtnMultipleSelectOK.PointerClick.RemoveAllListeners();
			m_cbtnMultipleSelectOK.PointerClick.AddListener(OnUnitSelectCompleteInMulti);
			if (m_NKM_UI_UNIT_SELECT_LIST_POSSESS_ADD != null)
			{
				m_NKM_UI_UNIT_SELECT_LIST_POSSESS_ADD.PointerClick.RemoveAllListeners();
				m_NKM_UI_UNIT_SELECT_LIST_POSSESS_ADD.PointerClick.AddListener(OnExpandInventoryPopup);
			}
			m_ctgLockUnit.OnValueChanged.RemoveAllListeners();
			m_ctgLockUnit.OnValueChanged.AddListener(OnLockModeButton);
			m_btnRemoveUnit.PointerClick.RemoveAllListeners();
			m_btnRemoveUnit.PointerClick.AddListener(delegate
			{
				OnRemoveMode(bValue: true);
			});
			NKCUtil.SetButtonClickDelegate(m_btnShopShortcut, OnShopShortcut);
			NKCUtil.SetButtonClickDelegate(m_btnShipBuildShortcut, OnShipBuildShortcut);
			m_btnMultiCancel.PointerClick.RemoveAllListeners();
			m_btnMultiCancel.PointerClick.AddListener(OnTouchMultiCancel);
			m_btnMultiAuto.PointerClick.RemoveAllListeners();
			m_btnMultiAuto.PointerClick.AddListener(OnTouchAutoSelect);
			m_btnMultiAutoN.PointerClick.RemoveAllListeners();
			m_btnMultiAutoN.PointerClick.AddListener(delegate
			{
				OnAutoSelectByGrade(NKM_UNIT_GRADE.NUG_N);
			});
			m_btnMultiAutoR.PointerClick.RemoveAllListeners();
			m_btnMultiAutoR.PointerClick.AddListener(delegate
			{
				OnAutoSelectByGrade(NKM_UNIT_GRADE.NUG_R);
			});
			NKCUtil.SetButtonClickDelegate(m_btnOperatorExtract, (UnityAction)delegate
			{
				OnExtractOperatorMode(bValue: true);
			});
			m_bOpenOperatorExtractMode = NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.OPERATOR_EXTRACT);
			m_NKM_UI_UNIT_SELECT_LIST_UNIT = base.transform.Find("NKM_UI_UNIT_SELECT_LIST_UNIT").GetComponent<NKCUIRectMove>();
			NKCUtil.SetGameobjectActive(m_popupSmartSelect, bValue: false);
			m_NKM_UI_UNIT_SELECT_LIST_UNIT = base.transform.Find("NKM_UI_UNIT_SELECT_LIST_UNIT").GetComponent<NKCUIRectMove>();
			base.gameObject.SetActive(value: false);
		}
	}

	private void CalculateContentRectSize()
	{
		int minColumn = 0;
		Vector2 cellSize = Vector2.zero;
		Vector2 spacing = Vector2.zero;
		switch (m_currentTargetUnitType)
		{
		case TargetTabType.Unit:
		case TargetTabType.Trophy:
			minColumn = 5;
			if (m_currentOption.bShowBanMsg)
			{
				cellSize = m_vUnitCastingSlotSize;
				spacing = m_vUnitCastingSlotSpacing;
			}
			else
			{
				cellSize = m_vUnitSlotSize;
				spacing = m_vUnitSlotSpacing;
			}
			break;
		case TargetTabType.Ship:
			minColumn = 3;
			cellSize = m_vShipSlotSize;
			spacing = m_vShipSlotSpacing;
			break;
		case TargetTabType.Operator:
			minColumn = 5;
			if (m_currentOption.bShowBanMsg)
			{
				cellSize = m_vOperCastingSlotSize;
				spacing = m_vOperCastingSlotSpacing;
			}
			else
			{
				cellSize = m_vOperatorSlotSize;
				spacing = m_vOperatorSlotSpacing;
			}
			break;
		}
		if (m_SafeArea != null)
		{
			m_SafeArea.SetSafeAreaBase();
		}
		NKCUtil.CalculateContentRectSize(m_LoopScrollRect, m_GridLayoutGroup, minColumn, cellSize, spacing, m_currentTargetUnitType == TargetTabType.Ship);
	}

	public void Open(UnitSelectListOptions options, OnUnitSelectCommand onUnitSelectCommand, OnUnitSortList OnUnitSortList = null, OnOperatorSortList OnOperatorSortList = null, OnUnitSortOption OnUnitSortOption = null, List<int> lstUpsideMenuResource = null)
	{
		SetUnitListAddEffect(bActive: false);
		m_listSelectedUnit.Clear();
		m_ssActive = null;
		m_OperatorSortSystem = null;
		m_dicUnitSortSystem.Clear();
		m_bKeepSortFilterOptions = !string.IsNullOrEmpty(m_currentOption.m_strCachingUIName) && !string.IsNullOrEmpty(options.m_strCachingUIName) && string.Equals(m_currentOption.m_strCachingUIName, options.m_strCachingUIName) && m_currentOption.eTargetUnitType == options.eTargetUnitType;
		m_bIsReturnCastingBanSlot = false;
		if (m_currentOption.eTargetUnitType == options.eTargetUnitType && m_currentOption.bShowBanMsg != options.bShowBanMsg)
		{
			m_bIsReturnCastingBanSlot = m_currentOption.bShowBanMsg;
			m_bCellPrepared = false;
		}
		m_currentOption = options;
		m_BeforeUnit = options.beforeUnit;
		m_BeforeOperator = options.beforeOperator;
		m_BeforeUnitDeckIndex = options.beforeUnitDeckIndex;
		m_dOnUnitSelectCommand = onUnitSelectCommand;
		m_dOnUnitSortList = OnUnitSortList;
		m_dOnOperatorSortList = OnOperatorSortList;
		m_dOnUnitSortOption = OnUnitSortOption;
		if (lstUpsideMenuResource != null)
		{
			RESOURCE_LIST = lstUpsideMenuResource;
		}
		else
		{
			RESOURCE_LIST = base.UpsideMenuShowResourceList;
		}
		NKCUIManager.UpdateUpsideMenu();
		m_bLockModeEnabled = false;
		if (m_currentOption.setSelectedUnitUID != null)
		{
			foreach (long item in m_currentOption.setSelectedUnitUID)
			{
				if (item > 0)
				{
					m_listSelectedUnit.Add(item);
				}
			}
		}
		ChangeUI();
		ProcessByType(m_currentOption.eTargetUnitType);
		SetUnitCount(m_currentOption.eTargetUnitType);
		UpdateMultipleSelectCountUI();
		UpdateDisableUIOnMultipleSelect();
		UpdateMultiSelectGetItemResult();
		NKCUtil.SetGameobjectActive(m_objRootUnitCount, !options.m_bHideUnitCount);
		NKCUtil.SetGameobjectActive(m_popupSmartSelect, bValue: false);
		NKCUtil.SetGameobjectActive(m_btnShipBuildShortcut, options.m_bShowShipBuildShortcut);
		UIOpened();
	}

	private void ChangeUI()
	{
		NKCUtil.SetGameobjectActive(m_objMultipleSelectRoot, m_currentOption.bMultipleSelect);
		m_ctgLockUnit.Select(IsLockMode);
		if (m_NKM_UI_UNIT_SELECT_LIST_UNIT != null)
		{
			if (m_currentOption.bMultipleSelect)
			{
				m_NKM_UI_UNIT_SELECT_LIST_UNIT.Set("SELECT");
			}
			else
			{
				m_NKM_UI_UNIT_SELECT_LIST_UNIT.Set("BASE");
			}
			if (m_bPrevMultiple != m_currentOption.bMultipleSelect)
			{
				m_bCellPrepared = false;
				m_bPrevMultiple = m_currentOption.bMultipleSelect;
			}
		}
		NKCUtil.SetGameobjectActive(m_objMultipleSelectGetItem, m_currentOption.bShowRemoveItem);
		NKCUtil.SetGameobjectActive(m_ctgHideDeckedUnit, m_currentOption.bShowHideDeckedUnitMenu);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_tglSelectModeShip, m_currentOption.bShowUnitShipChangeMenu);
		NKCUtil.SetGameobjectActive(m_tglSelectModeUnit, m_currentOption.bShowUnitShipChangeMenu);
		NKCUtil.SetGameobjectActive(m_tglSelectModeTrophy, m_currentOption.bShowUnitShipChangeMenu);
		if (!NKCOperatorUtil.IsHide())
		{
			NKCUtil.SetGameobjectActive(m_tglSelectModeOperator, m_currentOption.bShowUnitShipChangeMenu);
		}
		NKCUtil.SetGameobjectActive(m_ctgLockUnit, m_currentOption.bEnableLockUnitSystem);
		NKCUtil.SetGameobjectActive(m_btnRemoveUnit, m_currentOption.bEnableRemoveUnitSystem);
		NKCUtil.SetGameobjectActive(m_btnShopShortcut, !string.IsNullOrEmpty(m_currentOption.ShopShortcutTargetTab));
		NKCUtil.SetGameobjectActive(m_btnMultiAuto, m_currentOption.bUseRemoveSmartAutoSelect || m_currentOption.dOnAutoSelectFilter != null);
		NKCUtil.SetGameobjectActive(m_btnMultiAutoN, bValue: false);
		NKCUtil.SetGameobjectActive(m_btnMultiAutoR, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLockMsg, IsLockMode);
		NKCUtil.SetGameobjectActive(m_objRemoveMsg, m_currentOption.bShowRemoveItem);
		NKCUtil.SetGameobjectActive(m_objBanMsg, m_currentOption.bShowBanMsg);
		NKCUtil.SetGameobjectActive(m_objOperatorExtractCost, m_currentTargetUnitType == TargetTabType.Operator && m_currentOption.bEnableExtractOperatorSystem && IsExtractOperatorMode);
		if (m_currentOption.bMultipleSelect)
		{
			TargetTabType eTargetUnitType = m_currentOption.eTargetUnitType;
			if (eTargetUnitType == TargetTabType.Unit || eTargetUnitType != TargetTabType.Ship)
			{
				m_lbMultipleSelectText.text = NKCUtilString.GET_STRING_UNIT_SELECT_UNIT_COUNT;
			}
			else
			{
				m_lbMultipleSelectText.text = NKCUtilString.GET_STRING_UNIT_SELECT_SHIP_COUNT;
			}
		}
		UpdateMultiOkButton();
	}

	public void ClearMultipleSelect()
	{
		m_listSelectedUnit.Clear();
		UpdateMultipleSelectCountUI();
		UpdateDisableUIOnMultipleSelect();
		UpdateMultiSelectGetItemResult();
		UpdateUnitCount();
	}

	public void ClearCachOption()
	{
		m_currentOption.m_strCachingUIName = "";
	}

	private void SetSortAndFilterButtons(TargetTabType targetType)
	{
		switch (targetType)
		{
		case TargetTabType.Unit:
			m_tglSelectModeUnit.Select(bSelect: true, bForce: true);
			break;
		case TargetTabType.Ship:
			m_tglSelectModeShip.Select(bSelect: true, bForce: true);
			break;
		case TargetTabType.Operator:
			m_tglSelectModeOperator.Select(bSelect: true, bForce: true);
			break;
		case TargetTabType.Trophy:
			m_tglSelectModeTrophy.Select(bSelect: true, bForce: true);
			break;
		}
		if (m_currentOption.bShowUnitShipChangeMenu)
		{
			if (!NKCOperatorUtil.IsHide() && NKCOperatorUtil.IsActive())
			{
				m_tglSelectModeOperator.UnLock();
			}
			else
			{
				m_tglSelectModeOperator.Lock();
			}
		}
		m_SortUI.ResetUI(m_currentOption.m_bUseFavorite && (targetType == TargetTabType.Unit || targetType == TargetTabType.Trophy));
	}

	private void ProcessByType(TargetTabType targetType, bool bForceRebuildList = false)
	{
		if (targetType == TargetTabType.Operator)
		{
			if (m_currentOption.lstOperatorSortOption.Count == 0)
			{
				m_currentOption.lstOperatorSortOption = NKCOperatorSortSystem.GetDefaultSortOptions(bIsCollection: false);
				m_currentOption.m_OperatorSortOptions.SetBuildOption(true, BUILD_OPTIONS.DESCENDING);
			}
		}
		else if (m_dicUnitSortSystem.ContainsKey(targetType))
		{
			m_currentOption.setFilterOption = m_dicUnitSortSystem[targetType].FilterSet;
			m_currentOption.lstSortOption = m_dicUnitSortSystem[targetType].lstSortOption;
			m_currentOption.bDescending = m_dicUnitSortSystem[targetType].Descending;
		}
		else
		{
			if (m_currentOption.lstSortOption.Count == 0)
			{
				m_currentOption.lstSortOption = NKCUnitSortSystem.GetDefaultSortOptions(GetUnitType(targetType), bIsCollection: false);
			}
			m_currentOption.bDescending = true;
		}
		if (!m_bCellPrepared || m_currentTargetUnitType != targetType)
		{
			m_bCellPrepared = true;
			m_currentTargetUnitType = targetType;
			CalculateContentRectSize();
			m_LoopScrollRect.PrepareCells();
		}
		SetSortAndFilterButtons(targetType);
		if (bForceRebuildList)
		{
			if (targetType == TargetTabType.Operator)
			{
				m_currentOption.m_OperatorSortOptions.setFilterOption.Clear();
				m_OperatorSortSystem = null;
			}
			else
			{
				m_dicUnitSortSystem.Remove(targetType);
			}
		}
		if (targetType == TargetTabType.Operator)
		{
			m_OperatorSortSystem = GetOperatorSortSystem();
		}
		else
		{
			m_ssActive = GetUnitSortSystem(targetType);
		}
		NKCUtil.SetGameobjectActive(m_btnOperatorExtract.gameObject, m_bOpenOperatorExtractMode && targetType == TargetTabType.Operator);
		m_bDataValid = true;
		OnSortChanged(bResetScroll: true);
	}

	public void OnUnitSelectCompleteInMulti()
	{
		OnUnitSelectComplete(m_listSelectedUnit);
	}

	private void OnButtonRemoveConfirm()
	{
		OnUnitSelectComplete(m_listSelectedUnit);
	}

	private void OnUnitSelectComplete(List<long> unitUID)
	{
		if (m_currentOption.m_IncludeUnitUID != 0L)
		{
			m_dicUnitSortSystem.Clear();
			if (m_currentOption.setExcludeUnitUID.Contains(m_currentOption.m_IncludeUnitUID))
			{
				m_currentOption.setExcludeUnitUID.Remove(m_currentOption.m_IncludeUnitUID);
			}
			m_ssActive = GetUnitSortSystem(m_currentTargetUnitType);
			_ = m_ssActive.SortedUnitList.Count;
		}
		if (m_currentOption.bShowBanMsg && m_currentOption.iMaxMultipleSelect > unitUID.Count)
		{
			if (!string.IsNullOrEmpty(m_currentOption.MaxUnitNotSelectMsg))
			{
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, m_currentOption.MaxUnitNotSelectMsg, delegate
				{
					m_dOnUnitSelectCommand?.Invoke(new List<long>(unitUID));
				});
				return;
			}
			if (!string.IsNullOrEmpty(m_currentOption.MaxUnitNotSelectMsg2))
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, m_currentOption.MaxUnitNotSelectMsg2);
				return;
			}
		}
		if (m_dOnUnitSortList != null)
		{
			m_currentUnitList = m_ssActive.GetCurrentUnitList();
			if (m_currentUnitList.Count >= 1 && unitUID.Count > 0)
			{
				m_dOnUnitSortList?.Invoke(unitUID[0], m_currentUnitList);
			}
		}
		if (m_dOnUnitSortOption != null)
		{
			m_dOnUnitSortOption(m_currentOption.m_SortOptions);
		}
		if (m_dOnUnitSelectCommand != null)
		{
			m_dOnUnitSelectCommand(new List<long>(unitUID));
			if (unitUID.Count > 0)
			{
				m_lastSelectedUnitUID = unitUID[0];
			}
		}
	}

	private void OnOperatorSelectComplete(List<long> unitUID)
	{
		if (m_currentOption.m_IncludeUnitUID != 0L)
		{
			if (m_currentOption.setExcludeUnitUID.Contains(m_currentOption.m_IncludeUnitUID))
			{
				m_currentOption.setExcludeUnitUID.Remove(m_currentOption.m_IncludeUnitUID);
			}
			m_OperatorSortSystem = GetOperatorSortSystem();
		}
		List<NKMOperator> currentOperatorList = m_OperatorSortSystem.GetCurrentOperatorList();
		if (currentOperatorList.Count >= 1 && unitUID.Count > 0)
		{
			m_dOnOperatorSortList?.Invoke(unitUID[0], currentOperatorList);
		}
		if (m_dOnUnitSelectCommand != null)
		{
			m_dOnUnitSelectCommand(new List<long>(unitUID));
		}
	}

	private void OnRemoveSlot()
	{
		OnSlotSelected(null, null, new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE), NKCUnitSortSystem.eUnitState.NONE, eUnitSlotSelectState.NONE);
	}

	private void UpdateMultipleSelectCountUI()
	{
		if (m_currentOption.bMultipleSelect)
		{
			m_lbMultipleSelectCount.text = m_listSelectedUnit.Count + " / " + m_currentOption.iMaxMultipleSelect;
		}
	}

	private void UpdateDisableUIOnMultipleSelect()
	{
		if (!m_currentOption.bMultipleSelect)
		{
			return;
		}
		if (m_currentOption.iMaxMultipleSelect <= m_listSelectedUnit.Count)
		{
			foreach (NKCUIUnitSelectListSlotBase item in m_lstVisibleSlot)
			{
				if (item.NKMUnitData != null && !m_listSelectedUnit.Contains(item.NKMUnitData.m_UnitUID))
				{
					item.SetSlotSelectState(eUnitSlotSelectState.DISABLE);
				}
			}
			return;
		}
		foreach (NKCUIUnitSelectListSlotBase item2 in m_lstVisibleSlot)
		{
			if (item2.NKMUnitData != null && !m_listSelectedUnit.Contains(item2.NKMUnitData.m_UnitUID))
			{
				item2.SetSlotSelectState(eUnitSlotSelectState.NONE);
			}
		}
	}

	private void UpdateMultiSelectGetItemResult()
	{
		if (!m_currentOption.bShowRemoveItem)
		{
			return;
		}
		List<NKCUISlot.SlotData> list = MakeRemoveUnitItemGainList(m_listSelectedUnit, m_currentTargetUnitType);
		int num = list.Count - m_listBotSlot.Count;
		for (int i = 0; i < num; i++)
		{
			NKCUISlot newInstance = NKCUISlot.GetNewInstance(m_trMultipleSelectGetItemListContent);
			if (newInstance != null)
			{
				m_listBotSlot.Add(newInstance);
			}
		}
		for (int j = 0; j < m_listBotSlot.Count; j++)
		{
			NKCUISlot nKCUISlot = m_listBotSlot[j];
			if (j < list.Count)
			{
				nKCUISlot.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
				nKCUISlot.m_cbtnButton.UpdateOrgSize();
				NKCUtil.SetGameobjectActive(nKCUISlot, bValue: true);
				nKCUISlot.SetData(list[j], bShowName: false, bShowNumber: true, bEnableLayoutElement: true, null);
				nKCUISlot.SetOpenItemBoxOnClick();
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_listBotSlot[j], bValue: false);
			}
		}
	}

	private List<NKCUISlot.SlotData> MakeRemoveUnitItemGainList(List<long> lstUnitUID, TargetTabType unitType)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		if (UserData == null)
		{
			return new List<NKCUISlot.SlotData>();
		}
		NKMArmyData armyData = UserData.m_ArmyData;
		NKMUnitData nKMUnitData = null;
		NKMOperator nKMOperator = null;
		m_iOperatorExtractCostCount = 0;
		for (int i = 0; i < lstUnitUID.Count; i++)
		{
			switch (unitType)
			{
			case TargetTabType.Ship:
				nKMUnitData = armyData.GetShipFromUID(lstUnitUID[i]);
				break;
			case TargetTabType.Operator:
				nKMOperator = NKCOperatorUtil.GetOperatorData(lstUnitUID[i]);
				break;
			default:
				nKMUnitData = armyData.GetUnitFromUID(lstUnitUID[i]);
				break;
			case TargetTabType.Trophy:
				nKMUnitData = armyData.GetTrophyFromUID(lstUnitUID[i]);
				break;
			}
			if ((nKMUnitData == null && nKMOperator == null) || (nKMUnitData != null && nKMUnitData.IsSeized))
			{
				continue;
			}
			int unitID = nKMUnitData?.m_UnitID ?? nKMOperator.id;
			bool flag = nKMUnitData?.FromContract ?? nKMOperator.fromContract;
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
			if (unitTempletBase == null)
			{
				continue;
			}
			for (int j = 0; j < unitTempletBase.RemoveRewards.Count; j++)
			{
				int iD = unitTempletBase.RemoveRewards[j].ID;
				int count = unitTempletBase.RemoveRewards[j].Count;
				if (dictionary.ContainsKey(iD))
				{
					dictionary[iD] += count;
				}
				else
				{
					dictionary.Add(iD, count);
				}
			}
			if (flag && unitTempletBase.RemoveRewardFromContract != null)
			{
				int iD2 = unitTempletBase.RemoveRewardFromContract.ID;
				int count2 = unitTempletBase.RemoveRewardFromContract.Count;
				if (dictionary.ContainsKey(iD2))
				{
					dictionary[iD2] += count2;
				}
				else
				{
					dictionary.Add(iD2, count2);
				}
			}
			if (m_currentOption.bEnableExtractOperatorSystem)
			{
				_ = 3;
				int extractItemID = NKCOperatorUtil.GetExtractItemID(unitTempletBase.m_OprPassiveGroupID, nKMOperator.subSkill.id, unitTempletBase.m_NKM_UNIT_GRADE);
				if (dictionary.ContainsKey(extractItemID))
				{
					dictionary[extractItemID]++;
				}
				else
				{
					dictionary.Add(extractItemID, 1);
				}
				NKCOperatorUtil.GetExtractPriceItem(unitTempletBase.m_NKM_UNIT_GRADE, out var _, out var value);
				m_iOperatorExtractCostCount += value;
			}
		}
		Color col = Color.white;
		if (m_iOperatorExtractCostCount > (int)UserData.m_InventoryData.GetCountMiscItem(3))
		{
			col = Color.red;
		}
		NKCUtil.SetLabelText(m_lbOperatorExtractCostCnt, m_iOperatorExtractCostCount.ToString());
		NKCUtil.SetLabelTextColor(m_lbOperatorExtractCostCnt, col);
		List<NKCUISlot.SlotData> list = new List<NKCUISlot.SlotData>();
		foreach (KeyValuePair<int, int> item in dictionary)
		{
			list.Add(NKCUISlot.SlotData.MakeMiscItemData(item.Key, item.Value));
		}
		return list;
	}

	private void OnSlotSelectedInMuiltiSelect(NKMUnitData selectedUnit, NKMDeckIndex selectedUnitDeckIndex)
	{
		int beforeSelectedCount = m_listSelectedUnit.Count;
		if (m_listSelectedUnit.Contains(selectedUnit.m_UnitUID))
		{
			NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase = FindSlotFromCurrentList(selectedUnit.m_UnitUID);
			if (nKCUIUnitSelectListSlotBase != null)
			{
				nKCUIUnitSelectListSlotBase.SetSlotSelectState(eUnitSlotSelectState.NONE);
			}
			m_listSelectedUnit.Remove(selectedUnit.m_UnitUID);
		}
		else if (m_currentOption.iMaxMultipleSelect > m_listSelectedUnit.Count)
		{
			if (m_currentOption.dOnSelectedUnitWarning != null && m_currentOption.dOnSelectedUnitWarning(selectedUnit.m_UnitUID, m_listSelectedUnit, out var msg))
			{
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, msg, delegate
				{
					UpdateMuiltiSelectUIForDismissible(selectedUnit.m_UnitUID, beforeSelectedCount);
				});
			}
			else if (CanRemoveBasicShip(selectedUnit))
			{
				UpdateMuiltiSelectUIForDismissible(selectedUnit.m_UnitUID, beforeSelectedCount);
			}
			return;
		}
		UpdateMuiltiSelectUI(beforeSelectedCount, m_listSelectedUnit.Count);
	}

	private bool CanRemoveBasicShip(NKMUnitData selectedUnit)
	{
		if (m_currentTargetUnitType == TargetTabType.Ship)
		{
			int groupID = selectedUnit.GetShipGroupId();
			if (NKMOpenTagManager.IsOpened("TAG_DELETE_BASIC_SHIP") && NKMConst.Ship.BaseShipGroupIds.Contains(groupID))
			{
				int shipCountByGroupID = NKMShipManager.GetShipCountByGroupID(groupID);
				int count = m_listSelectedUnit.FindAll((long x) => NKMShipManager.GetShipGroupID(x) == groupID).Count;
				if (shipCountByGroupID == count + 1 && !m_listSelectedUnit.Contains(selectedUnit.m_UnitUID))
				{
					NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_REMOVE_SHIP_REFUSE_TEXT, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
					return false;
				}
			}
		}
		return true;
	}

	private void OnSlotSelectedInMuiltiSelect(NKMOperator selectedOperator, NKMDeckIndex selectedUnitDeckIndex)
	{
		int beforeSelectedCount = m_listSelectedUnit.Count;
		if (m_listSelectedUnit.Contains(selectedOperator.uid))
		{
			NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase = FindSlotFromCurrentList(selectedOperator.uid);
			if (nKCUIUnitSelectListSlotBase != null)
			{
				nKCUIUnitSelectListSlotBase.SetSlotSelectState(eUnitSlotSelectState.NONE);
			}
			m_listSelectedUnit.Remove(selectedOperator.uid);
		}
		else if (m_currentOption.iMaxMultipleSelect > m_listSelectedUnit.Count)
		{
			if (m_currentOption.dOnSelectedUnitWarning != null && m_currentOption.dOnSelectedUnitWarning(selectedOperator.uid, m_listSelectedUnit, out var msg))
			{
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, msg, delegate
				{
					UpdateMuiltiSelectUIForDismissible(selectedOperator.uid, beforeSelectedCount);
				});
			}
			else
			{
				UpdateMuiltiSelectUIForDismissible(selectedOperator.uid, beforeSelectedCount);
			}
			return;
		}
		UpdateMuiltiSelectUI(beforeSelectedCount, m_listSelectedUnit.Count);
	}

	private void UpdateMuiltiSelectUIForDismissible(long unitUid, int beforeSelectedCount)
	{
		m_listSelectedUnit.Add(unitUid);
		bool bShowRemoveItem = m_currentOption.bShowRemoveItem;
		if (bShowRemoveItem && m_currentTargetUnitType == TargetTabType.Unit)
		{
			NKCUINPCMachineGap.PlayVoice(NPC_TYPE.MACHINE_GAP, NPC_ACTION_TYPE.DISMISSAL_SELECT, bStopCurrentSound: false);
		}
		NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase = FindSlotFromCurrentList(unitUid);
		if (nKCUIUnitSelectListSlotBase != null)
		{
			nKCUIUnitSelectListSlotBase.SetSlotSelectState((!bShowRemoveItem) ? eUnitSlotSelectState.SELECTED : eUnitSlotSelectState.DELETE);
		}
		UpdateMuiltiSelectUI(beforeSelectedCount, m_listSelectedUnit.Count);
	}

	private void UpdateMuiltiSelectUI(int beforeSelectedCount, int afterSelectedCount)
	{
		if (afterSelectedCount != beforeSelectedCount && (afterSelectedCount == m_currentOption.iMaxMultipleSelect || afterSelectedCount == m_currentOption.iMaxMultipleSelect - 1))
		{
			UpdateDisableUIOnMultipleSelect();
		}
		UpdateMultipleSelectCountUI();
		UpdateMultiSelectGetItemResult();
	}

	private void OnSlotSelected(NKMUnitData selectedUnit, NKMUnitTempletBase unitTempletBase, NKMDeckIndex selectedUnitDeckIndex, NKCUnitSortSystem.eUnitState unitSlotState, eUnitSlotSelectState unitSlotSelectState)
	{
		if (unitSlotSelectState != eUnitSlotSelectState.SELECTED && unitSlotSelectState == eUnitSlotSelectState.DISABLE)
		{
			return;
		}
		switch (unitSlotState)
		{
		case NKCUnitSortSystem.eUnitState.DUPLICATE:
		case NKCUnitSortSystem.eUnitState.DECKED:
		case NKCUnitSortSystem.eUnitState.MAINUNIT:
		case NKCUnitSortSystem.eUnitState.LOCKED:
		case NKCUnitSortSystem.eUnitState.DUNGEON_RESTRICTED:
		case NKCUnitSortSystem.eUnitState.LEAGUE_BANNED:
		case NKCUnitSortSystem.eUnitState.LEAGUE_DECKED_LEFT:
		case NKCUnitSortSystem.eUnitState.LEAGUE_DECKED_RIGHT:
		case NKCUnitSortSystem.eUnitState.OFFICE_DORM_IN:
			return;
		case NKCUnitSortSystem.eUnitState.CITY_MISSION:
		case NKCUnitSortSystem.eUnitState.WARFARE_BATCH:
		case NKCUnitSortSystem.eUnitState.DIVE_BATCH:
			if (!m_currentOption.bCanSelectUnitInMission)
			{
				return;
			}
			break;
		case NKCUnitSortSystem.eUnitState.SEIZURE:
			if (!IsRemoveMode && !m_currentOption.m_SortOptions.bIncludeSeizure)
			{
				return;
			}
			break;
		default:
			if (m_currentOption.m_SortOptions.bUseDeckedState)
			{
				return;
			}
			break;
		case NKCUnitSortSystem.eUnitState.NONE:
			break;
		}
		if (IsLockMode)
		{
			if (selectedUnit != null)
			{
				NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(selectedUnit.m_UnitID);
				if (unitTempletBase2 != null && unitTempletBase2.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_OPERATOR)
				{
					NKCPacketSender.Send_NKMPacket_OPERATOR_LOCK_REQ(selectedUnit.m_UnitUID, !selectedUnit.m_bLock);
				}
				else
				{
					NKCPacketSender.Send_NKMPacket_LOCK_UNIT_REQ(selectedUnit.m_UnitUID, !selectedUnit.m_bLock);
				}
			}
			return;
		}
		if (m_currentOption.bMultipleSelect)
		{
			OnSlotSelectedInMuiltiSelect(selectedUnit, selectedUnitDeckIndex);
			return;
		}
		List<long> singleSelectedList = new List<long>();
		if (selectedUnit != null)
		{
			singleSelectedList.Add(selectedUnit.m_UnitUID);
		}
		else
		{
			singleSelectedList.Add(0L);
		}
		if (m_BeforeUnit == null || m_BeforeUnit.m_UnitUID == 0L || selectedUnit == null)
		{
			OnUnitSelectComplete(singleSelectedList);
			return;
		}
		OpenUnitChangePopup(m_BeforeUnit, m_BeforeUnitDeckIndex, selectedUnit, selectedUnitDeckIndex, delegate
		{
			OnUnitSelectComplete(singleSelectedList);
		});
	}

	private void OnOperatorSlotSelected(NKMOperator selectedOperator, NKMUnitTempletBase unitTempletBase, NKMDeckIndex selectedUnitDeckIndex, NKCUnitSortSystem.eUnitState unitSlotState, eUnitSlotSelectState unitSlotSelectState)
	{
		if (unitSlotSelectState != eUnitSlotSelectState.SELECTED && unitSlotSelectState == eUnitSlotSelectState.DISABLE)
		{
			return;
		}
		switch (unitSlotState)
		{
		case NKCUnitSortSystem.eUnitState.DUPLICATE:
		case NKCUnitSortSystem.eUnitState.DECKED:
		case NKCUnitSortSystem.eUnitState.MAINUNIT:
		case NKCUnitSortSystem.eUnitState.LOCKED:
		case NKCUnitSortSystem.eUnitState.DUNGEON_RESTRICTED:
		case NKCUnitSortSystem.eUnitState.LEAGUE_BANNED:
		case NKCUnitSortSystem.eUnitState.LEAGUE_DECKED_LEFT:
		case NKCUnitSortSystem.eUnitState.LEAGUE_DECKED_RIGHT:
			return;
		case NKCUnitSortSystem.eUnitState.CITY_MISSION:
		case NKCUnitSortSystem.eUnitState.WARFARE_BATCH:
		case NKCUnitSortSystem.eUnitState.DIVE_BATCH:
			if (!m_currentOption.bCanSelectUnitInMission)
			{
				return;
			}
			break;
		case NKCUnitSortSystem.eUnitState.SEIZURE:
			if (!IsRemoveMode && !m_currentOption.m_SortOptions.bIncludeSeizure)
			{
				return;
			}
			break;
		default:
			if (m_currentOption.m_SortOptions.bUseDeckedState)
			{
				return;
			}
			break;
		case NKCUnitSortSystem.eUnitState.NONE:
			break;
		}
		if (IsLockMode)
		{
			if (selectedOperator != null)
			{
				NKCPacketSender.Send_NKMPacket_OPERATOR_LOCK_REQ(selectedOperator.uid, !selectedOperator.bLock);
			}
			return;
		}
		if (m_currentOption.bMultipleSelect)
		{
			OnSlotSelectedInMuiltiSelect(selectedOperator, selectedUnitDeckIndex);
			return;
		}
		List<long> singleSelectedList = new List<long>();
		if (selectedOperator != null)
		{
			singleSelectedList.Add(selectedOperator.uid);
		}
		else
		{
			singleSelectedList.Add(0L);
		}
		if (m_BeforeOperator == null || m_BeforeOperator.uid == 0L || selectedOperator == null)
		{
			if (m_currentTargetUnitType == TargetTabType.Operator && singleSelectedList[0] != 0L && string.Equals(m_currentOption.m_strCachingUIName, NKCUtilString.GET_STRING_EVENT_DECK))
			{
				NKCUIOperatorPopupConfirm.Instance.Open(singleSelectedList[0], delegate
				{
					OnOperatorSelectComplete(singleSelectedList);
				});
			}
			else
			{
				OnOperatorSelectComplete(singleSelectedList);
			}
		}
		else
		{
			OpenUnitChangePopup(m_BeforeOperator, m_BeforeUnitDeckIndex, selectedOperator, selectedUnitDeckIndex, delegate
			{
				OnOperatorSelectComplete(singleSelectedList);
			});
		}
	}

	private void OpenUnitChangePopup(NKMUnitData beforeUnit, NKMDeckIndex beforeUnitDeckIndex, NKMUnitData afterUnit, NKMDeckIndex afterUnitDeckIndex, NKCUIUnitSelectListChangePopup.OnUnitChangePopupOK onOK)
	{
		if (m_currentTargetUnitType == TargetTabType.Unit)
		{
			NKCUIUnitSelectListChangePopup.Instance.Open(beforeUnit, beforeUnitDeckIndex, afterUnit, afterUnitDeckIndex, onOK);
		}
		else if (m_currentTargetUnitType == TargetTabType.Operator)
		{
			NKCUIOperatorPopupChange.Instance.Open(NKMDeckIndex.None, beforeUnit.m_UnitUID, afterUnit.m_UnitUID, delegate
			{
				onOK?.Invoke();
			});
		}
		else
		{
			onOK?.Invoke();
		}
	}

	private void OpenUnitChangePopup(NKMOperator beforeOperator, NKMDeckIndex beforeUnitDeckIndex, NKMOperator afterOperator, NKMDeckIndex afterUnitDeckIndex, NKCUIUnitSelectListChangePopup.OnUnitChangePopupOK onOK)
	{
		NKCUIOperatorPopupChange.Instance.Open(NKMDeckIndex.None, beforeOperator.uid, afterOperator.uid, delegate
		{
			onOK();
		});
	}

	public void OnLockModeButton(bool bValue)
	{
		m_ctgLockUnit.Select(bValue, bForce: true);
		m_bLockModeEnabled = bValue;
		NKCUtil.SetGameobjectActive(m_objLockMsg, bValue);
		m_canvasRemoveUnit.alpha = (bValue ? 0.3f : 1f);
		m_btnRemoveUnit.enabled = !bValue;
		ProcessByType(m_currentTargetUnitType, m_currentTargetUnitType == TargetTabType.Operator);
	}

	private void OnTouchMultiCancel()
	{
		if (m_currentOption.bShowRemoveItem)
		{
			OnRemoveMode(bValue: false);
		}
		else
		{
			Close();
		}
	}

	public void OnRemoveMode(bool bValue)
	{
		if (bValue)
		{
			m_bLockModeEnabled = false;
			m_prevOption = m_currentOption;
			m_currentOption = new UnitSelectListOptions(m_currentTargetUnitType, _bMultipleSelect: true, NKM_DECK_TYPE.NDT_NORMAL);
			m_currentOption.bDescending = false;
			m_currentOption.bExcludeLockedUnit = false;
			m_currentOption.bExcludeDeckedUnit = false;
			m_currentOption.bHideDeckedUnit = false;
			m_currentOption.m_SortOptions.bUseLockedState = true;
			m_currentOption.m_SortOptions.bUseDeckedState = true;
			m_currentOption.m_SortOptions.bUseDormInState = true;
			m_currentOption.m_SortOptions.bIncludeSeizure = true;
			m_currentOption.m_OperatorSortOptions.SetBuildOption(true, BUILD_OPTIONS.USE_LOCKED_STATE, BUILD_OPTIONS.USE_DECKED_STATE, BUILD_OPTIONS.INCLUDE_SEIZURE);
			m_currentOption.bShowHideDeckedUnitMenu = false;
			m_currentOption.bShowRemoveItem = true;
			m_currentOption.bEnableRemoveUnitSystem = true;
			m_currentOption.setExcludeUnitID = NKCUnitSortSystem.GetDefaultExcludeUnitIDs();
			m_currentOption.bUseRemoveSmartAutoSelect = m_prevOption.bUseRemoveSmartAutoSelect;
			m_currentOption.m_SortOptions.AdditionalExcludeFilterFunc = CheckCanRemove;
			m_currentOption.m_SortOptions.PreemptiveSortFunc = SortRemoveUnitBySeized;
			m_currentOption.dOnAutoSelectFilter = m_prevOption.dOnAutoSelectFilter;
			m_currentOption.dOnClose = m_prevOption.dOnClose;
			m_currentOption.setUnitFilterCategory = m_prevOption.setUnitFilterCategory;
			m_currentOption.setUnitSortCategory = m_prevOption.setUnitSortCategory;
			m_currentOption.setShipFilterCategory = m_prevOption.setShipFilterCategory;
			m_currentOption.setShipSortCategory = m_prevOption.setShipSortCategory;
			m_currentOption.setOperatorFilterCategory = m_prevOption.setOperatorFilterCategory;
			m_currentOption.setOperatorSortCategory = m_prevOption.setOperatorSortCategory;
			m_prevOnUnitSelectCommand = m_dOnUnitSelectCommand;
			m_dOnUnitSelectCommand = RemoveUIDList;
			switch (m_currentTargetUnitType)
			{
			case TargetTabType.Trophy:
				m_currentOption.strEmptyMessage = NKCUtilString.GET_STRING_REMOVE_UNIT_NO_EXIST_TROPHY;
				m_txtRemoveMsg.text = NKCUtilString.GET_STRING_REMOVE_UNIT_SELECT;
				m_currentOption.iMaxMultipleSelect = 1000;
				break;
			case TargetTabType.Unit:
				m_currentOption.strEmptyMessage = NKCUtilString.GET_STRING_REMOVE_UNIT_NO_EXIST_UNIT;
				m_txtRemoveMsg.text = NKCUtilString.GET_STRING_REMOVE_UNIT_SELECT;
				m_currentOption.iMaxMultipleSelect = 1000;
				break;
			case TargetTabType.Ship:
				m_currentOption.strEmptyMessage = NKCUtilString.GET_STRING_REMOVE_SHIP_NO_EXIST_SHIP;
				m_txtRemoveMsg.text = NKCUtilString.GET_STRING_REMOVE_SHIP_SELECT;
				m_currentOption.iMaxMultipleSelect = 100;
				break;
			case TargetTabType.Operator:
				m_currentOption.strEmptyMessage = NKCUtilString.GET_STRING_REMOVE_UNIT_NO_EXIST_OPERATOR;
				m_txtRemoveMsg.text = NKCUtilString.GET_STRING_REMOVE_UNIT_SELECT;
				m_currentOption.iMaxMultipleSelect = 1000;
				break;
			}
			ClearMultipleSelect();
		}
		else
		{
			ClearMultipleSelect();
			m_dOnUnitSelectCommand = m_prevOnUnitSelectCommand;
			m_prevOnUnitSelectCommand = null;
			m_currentOption = m_prevOption;
		}
		NKCUIManager.UpdateUpsideMenu();
		ChangeUI();
		ProcessByType(m_currentTargetUnitType, bForceRebuildList: true);
		UpdateUnitCount();
	}

	private bool CheckCanRemove(NKMUnitData unitData)
	{
		if (!unitData.IsSeized)
		{
			return true;
		}
		if (unitData.m_bLock)
		{
			return false;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return false;
		}
		long unitUID = unitData.m_UnitUID;
		for (int i = 0; i < 8; i++)
		{
			NKMBackgroundUnitInfo backgroundUnitInfo = nKMUserData.GetBackgroundUnitInfo(i);
			if (backgroundUnitInfo != null && backgroundUnitInfo.unitUid == unitUID)
			{
				return false;
			}
		}
		if (nKMUserData.m_ArmyData.GetDeckDataByUnitUID(unitUID) != null)
		{
			return false;
		}
		foreach (NKMWorldMapCityData value in nKMUserData.m_WorldmapData.worldMapCityDataMap.Values)
		{
			if (value.leaderUnitUID == unitUID)
			{
				return false;
			}
		}
		return true;
	}

	private int SortRemoveUnitBySeized(NKMUnitData a, NKMUnitData b)
	{
		return b.IsSeized.CompareTo(a.IsSeized);
	}

	public void CloseRemoveMode()
	{
		if (m_currentOption.bShowRemoveItem)
		{
			OnRemoveMode(bValue: false);
		}
	}

	public void CloseExtractMode()
	{
		if (m_currentOption.bEnableExtractOperatorSystem)
		{
			OnExtractOperatorMode(bValue: false);
		}
	}

	public void RemoveUIDList(List<long> list)
	{
		switch (m_currentTargetUnitType)
		{
		case TargetTabType.Trophy:
			RemoveTrophyList(list);
			break;
		case TargetTabType.Unit:
			RemoveUnitList(list);
			break;
		case TargetTabType.Ship:
			RemoveShipList(list);
			break;
		case TargetTabType.Operator:
			RemoveOperatorList(list);
			break;
		}
	}

	private void RemoveUnitList(List<long> list)
	{
		if (list == null || list.Count <= 0)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_NO_EXIST_SELECTED_UNIT);
			return;
		}
		NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		bool flag = false;
		foreach (long item in list)
		{
			NKMUnitData unitFromUID = armyData.GetUnitFromUID(item);
			if (unitFromUID != null && unitFromUID.reactorLevel > 0)
			{
				flag = true;
				break;
			}
		}
		foreach (long item2 in list)
		{
			NKMUnitData unitFromUID2 = armyData.GetUnitFromUID(item2);
			if (unitFromUID2 == null)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_NO_EXIST_UNIT);
				return;
			}
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitFromUID2.m_UnitID);
			if (unitTempletBase != null && (unitFromUID2.m_UnitLevel > 1 || unitFromUID2.m_LimitBreakLevel > 0 || unitTempletBase.m_NKM_UNIT_GRADE >= NKM_UNIT_GRADE.NUG_SR))
			{
				string content = NKCUtilString.GET_STRING_REMOVE_UNIT_WARNING;
				if (flag)
				{
					content = $"{NKCUtilString.GET_STRING_REMOVE_UNIT_WARNING}\n{NKCUtilString.GET_STRING_UNIT_REACTOR_REMOVE_WARNING_DESC}";
				}
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, content, delegate
				{
					OpenRemoveConfirmPopup(list);
				});
				return;
			}
		}
		OpenRemoveConfirmPopup(list);
	}

	private void RemoveTrophyList(List<long> list)
	{
		if (list == null || list.Count <= 0)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_NO_EXIST_SELECTED_UNIT);
			return;
		}
		NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		foreach (long item in list)
		{
			if (armyData.GetTrophyFromUID(item) == null)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_NO_EXIST_UNIT);
				return;
			}
		}
		OpenRemoveConfirmPopup(list);
	}

	private void RemoveOperatorList(List<long> list)
	{
		if (list == null || list.Count <= 0)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_NO_EXIST_SELECTED_OPERATOR);
			return;
		}
		NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		foreach (long item in list)
		{
			NKMOperator operatorFromUId = armyData.GetOperatorFromUId(item);
			if (operatorFromUId == null)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_NO_EXIST_OPERATOR);
				return;
			}
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(operatorFromUId.id);
			if (unitTempletBase != null && (operatorFromUId.level > 1 || unitTempletBase.m_NKM_UNIT_GRADE >= NKM_UNIT_GRADE.NUG_SR))
			{
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_REMOVE_OPERATOR_WARNING, delegate
				{
					OpenRemoveConfirmPopup(list);
				});
				return;
			}
		}
		OpenRemoveConfirmPopup(list);
	}

	private void OpenRemoveConfirmPopup(List<long> unitList)
	{
		List<NKCUISlot.SlotData> lstSlot = MakeRemoveUnitItemGainList(m_listSelectedUnit, m_currentTargetUnitType);
		switch (m_currentTargetUnitType)
		{
		case TargetTabType.Unit:
		case TargetTabType.Trophy:
			NKCPopupResourceConfirmBox.Instance.OpenItemSlotList(NKCUtilString.GET_STRING_NOTICE, string.Format(NKCStringTable.GetString("SI_DP_POPUP_NOTICE_UNIT_REMOVE_ONE_PARAM"), unitList.Count), lstSlot, delegate
			{
				NKCPacketSender.Send_NKMPacket_REMOVE_UNIT_REQ(unitList);
			});
			break;
		case TargetTabType.Operator:
			NKCPopupResourceConfirmBox.Instance.OpenItemSlotList(NKCUtilString.GET_STRING_NOTICE, string.Format(NKCUtilString.GET_STRING_OPERATOR_REMOVE_CONFIRM_ONE_PARAM, unitList.Count), lstSlot, delegate
			{
				NKCPacketSender.Send_NKMPacket_OPERATOR_REMOVE_REQ(unitList);
			});
			break;
		case TargetTabType.Ship:
			break;
		}
	}

	private void RemoveShipList(List<long> list)
	{
		if (list == null || list.Count <= 0)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_REMOVE_SHIP_NO_EXIST_SHIP);
			return;
		}
		foreach (long item in list)
		{
			if (UserData == null)
			{
				break;
			}
			NKMUnitData shipFromUID = UserData.m_ArmyData.GetShipFromUID(item);
			if (shipFromUID == null)
			{
				Debug.LogError("Ship not exist! shipUID : " + item);
				continue;
			}
			switch (NKMUnitManager.GetUnitTempletBase(shipFromUID.m_UnitID).m_NKM_UNIT_GRADE)
			{
			case NKM_UNIT_GRADE.NUG_R:
			case NKM_UNIT_GRADE.NUG_SR:
			case NKM_UNIT_GRADE.NUG_SSR:
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_REMOVE_SHIP_WARNING_MSG, delegate
				{
					NKCPacketSender.Send_NKMPacket_SHIP_DIVISION_REQ(list);
				});
				return;
			default:
				Debug.LogWarning("Unit Grade undefined! unitID : " + shipFromUID.m_UnitID);
				break;
			case NKM_UNIT_GRADE.NUG_N:
				break;
			}
		}
		NKCPacketSender.Send_NKMPacket_SHIP_DIVISION_REQ(list);
	}

	public void OnExtractOperatorMode(bool bValue)
	{
		if (bValue)
		{
			NKCUtil.SetLabelText(m_lbOperatorExtractCostCnt, "0");
			m_bLockModeEnabled = false;
			m_prevOption = m_currentOption;
			m_currentOption = new UnitSelectListOptions(m_currentTargetUnitType, _bMultipleSelect: true, NKM_DECK_TYPE.NDT_NORMAL);
			m_currentOption.bDescending = false;
			m_currentOption.bExcludeLockedUnit = false;
			m_currentOption.bExcludeDeckedUnit = false;
			m_currentOption.bHideDeckedUnit = false;
			m_currentOption.m_SortOptions.bUseLockedState = true;
			m_currentOption.m_SortOptions.bUseDeckedState = true;
			m_currentOption.m_SortOptions.bUseDormInState = true;
			m_currentOption.m_SortOptions.bIncludeSeizure = true;
			m_currentOption.m_OperatorSortOptions.SetBuildOption(true, BUILD_OPTIONS.USE_LOCKED_STATE, BUILD_OPTIONS.USE_DECKED_STATE, BUILD_OPTIONS.INCLUDE_SEIZURE);
			m_currentOption.bShowHideDeckedUnitMenu = false;
			m_currentOption.bShowRemoveItem = true;
			m_currentOption.bEnableRemoveUnitSystem = false;
			m_currentOption.bEnableExtractOperatorSystem = true;
			m_currentOption.bUseRemoveSmartAutoSelect = m_prevOption.bUseRemoveSmartAutoSelect;
			m_currentOption.m_SortOptions.PreemptiveSortFunc = SortRemoveUnitBySeized;
			m_currentOption.dOnAutoSelectFilter = m_prevOption.dOnAutoSelectFilter;
			m_currentOption.dOnClose = m_prevOption.dOnClose;
			m_currentOption.setUnitFilterCategory = m_prevOption.setUnitFilterCategory;
			m_currentOption.setUnitSortCategory = m_prevOption.setUnitSortCategory;
			m_currentOption.setShipFilterCategory = m_prevOption.setShipFilterCategory;
			m_currentOption.setShipSortCategory = m_prevOption.setShipSortCategory;
			m_currentOption.setOperatorFilterCategory = m_prevOption.setOperatorFilterCategory;
			m_currentOption.setOperatorSortCategory = m_prevOption.setOperatorSortCategory;
			m_prevOnUnitSelectCommand = m_dOnUnitSelectCommand;
			m_dOnUnitSelectCommand = ExtractOperatorList;
			if (m_currentTargetUnitType == TargetTabType.Operator)
			{
				m_currentOption.strEmptyMessage = NKCUtilString.GET_STRING_EXTRACT_UNIT_NO_EXIST_OPERATOR;
				m_txtRemoveMsg.text = NKCUtilString.GET_STRING_EXTRACT_OPEATOR_SELECT;
				m_currentOption.iMaxMultipleSelect = 1000;
			}
			ClearMultipleSelect();
		}
		else
		{
			ClearMultipleSelect();
			m_dOnUnitSelectCommand = m_prevOnUnitSelectCommand;
			m_prevOnUnitSelectCommand = null;
			m_currentOption = m_prevOption;
		}
		NKCUIManager.UpdateUpsideMenu();
		ChangeUI();
		ProcessByType(m_currentTargetUnitType, bForceRebuildList: true);
		UpdateUnitCount();
	}

	private void ExtractOperatorList(List<long> list)
	{
		if (list == null || list.Count <= 0)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_NO_EXIST_SELECTED_OPERATOR);
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		int num = m_iOperatorExtractCostCount - (int)myUserData.m_InventoryData.GetCountMiscItem(3);
		if (num > 0)
		{
			NKCPopupItemLack.Instance.OpenItemMiscLackPopup(3, num);
			return;
		}
		NKMArmyData armyData = myUserData.m_ArmyData;
		foreach (long item in list)
		{
			NKMOperator operatorFromUId = armyData.GetOperatorFromUId(item);
			if (operatorFromUId == null)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_NO_EXIST_OPERATOR);
				return;
			}
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(operatorFromUId.id);
			if (unitTempletBase != null && (operatorFromUId.level > 1 || unitTempletBase.m_NKM_UNIT_GRADE >= NKM_UNIT_GRADE.NUG_SR))
			{
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_EXTRACT_OPERATOR_WARNING, delegate
				{
					NKCPopupOperatorExtract.Instance.Open(list);
				});
				return;
			}
		}
		NKCPopupOperatorExtract.Instance.Open(list);
	}

	private void OnShopShortcut()
	{
		NKCUIShop.ShopShortcut(m_currentOption.ShopShortcutTargetTab);
	}

	private void OnShipBuildShortcut()
	{
		NKCUIHangarBuild.Instance.Open();
	}

	private void OnTouchAutoSelect()
	{
		if (m_currentTargetUnitType != TargetTabType.Ship && m_currentOption.bUseRemoveSmartAutoSelect && m_popupSmartSelect != null)
		{
			m_popupSmartSelect.Open(OnOKAutoSelectPopup, m_currentTargetUnitType != TargetTabType.Unit, m_currentOption.bEnableExtractOperatorSystem);
		}
		else
		{
			new HashSet<long>();
			while (m_listSelectedUnit.Count < m_currentOption.iMaxMultipleSelect)
			{
				NKMUnitData nKMUnitData = m_ssActive.AutoSelect(new HashSet<long>(m_listSelectedUnit), (NKMUnitData unit) => AutoSelectFilter(unit));
				if (nKMUnitData == null)
				{
					break;
				}
				m_listSelectedUnit.Add(nKMUnitData.m_UnitUID);
				NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase = FindSlotFromCurrentList(nKMUnitData.m_UnitUID);
				if (nKCUIUnitSelectListSlotBase != null)
				{
					nKCUIUnitSelectListSlotBase.SetSlotSelectState((!m_currentOption.bShowRemoveItem) ? eUnitSlotSelectState.SELECTED : eUnitSlotSelectState.DELETE);
				}
			}
		}
		UpdateMultipleSelectCountUI();
		UpdateMultiSelectGetItemResult();
	}

	private void OnOKAutoSelectPopup(HashSet<NKM_UNIT_GRADE> setGrade, bool bSmart)
	{
		if (bSmart)
		{
			OnSmartRemoveSelectByGrade(setGrade, bIncludeTranscendence: true);
		}
		else
		{
			OnRemoveAllSelectByGrade(setGrade);
		}
	}

	private void OnRemoveAllSelectByGrade(HashSet<NKM_UNIT_GRADE> setGrade)
	{
		foreach (long item in m_listSelectedUnit)
		{
			NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase = FindSlotFromCurrentList(item);
			if (nKCUIUnitSelectListSlotBase != null)
			{
				nKCUIUnitSelectListSlotBase.SetSlotSelectState(eUnitSlotSelectState.NONE);
			}
		}
		m_listSelectedUnit.Clear();
		if (m_currentTargetUnitType == TargetTabType.Operator)
		{
			foreach (NKMOperator currentOperator in m_OperatorSortSystem.GetCurrentOperatorList())
			{
				if (currentOperator == null || currentOperator.bLock || currentOperator.level > 1 || m_OperatorSortSystem.GetUnitSlotState(currentOperator.uid) != NKCUnitSortSystem.eUnitState.NONE)
				{
					continue;
				}
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(currentOperator.id);
				if (unitTempletBase != null && setGrade.Contains(unitTempletBase.m_NKM_UNIT_GRADE) && unitTempletBase.m_NKM_UNIT_STYLE_TYPE != NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
				{
					m_listSelectedUnit.Add(currentOperator.uid);
					NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase2 = FindSlotFromCurrentList(currentOperator.uid);
					if (nKCUIUnitSelectListSlotBase2 != null)
					{
						nKCUIUnitSelectListSlotBase2.SetSlotSelectState((!m_currentOption.bShowRemoveItem) ? eUnitSlotSelectState.SELECTED : eUnitSlotSelectState.DELETE);
					}
					if (m_listSelectedUnit.Count >= m_currentOption.iMaxMultipleSelect)
					{
						break;
					}
				}
			}
		}
		else
		{
			foreach (NKMUnitData currentUnit in m_ssActive.GetCurrentUnitList())
			{
				if (currentUnit == null || currentUnit.m_bLock || currentUnit.m_UnitLevel > 1 || m_ssActive.GetUnitSlotState(currentUnit.m_UnitUID) != NKCUnitSortSystem.eUnitState.NONE)
				{
					continue;
				}
				NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(currentUnit);
				if (unitTempletBase2 != null && setGrade.Contains(unitTempletBase2.m_NKM_UNIT_GRADE))
				{
					m_listSelectedUnit.Add(currentUnit.m_UnitUID);
					NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase3 = FindSlotFromCurrentList(currentUnit.m_UnitUID);
					if (nKCUIUnitSelectListSlotBase3 != null)
					{
						nKCUIUnitSelectListSlotBase3.SetSlotSelectState((!m_currentOption.bShowRemoveItem) ? eUnitSlotSelectState.SELECTED : eUnitSlotSelectState.DELETE);
					}
					if (m_listSelectedUnit.Count >= m_currentOption.iMaxMultipleSelect)
					{
						break;
					}
				}
			}
		}
		UpdateMultipleSelectCountUI();
		UpdateMultiSelectGetItemResult();
	}

	private void OnSmartRemoveSelectByGrade(HashSet<NKM_UNIT_GRADE> setGrade, bool bIncludeTranscendence)
	{
		foreach (long item in m_listSelectedUnit)
		{
			NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase = FindSlotFromCurrentList(item);
			if (nKCUIUnitSelectListSlotBase != null)
			{
				nKCUIUnitSelectListSlotBase.SetSlotSelectState(eUnitSlotSelectState.NONE);
			}
		}
		m_listSelectedUnit.Clear();
		List<NKMUnitData> currentUnitList = m_ssActive.GetCurrentUnitList();
		Dictionary<int, List<NKMUnitData>> dictionary = new Dictionary<int, List<NKMUnitData>>();
		Dictionary<int, List<NKMUnitData>> dictionary2 = new Dictionary<int, List<NKMUnitData>>();
		HashSet<long> hashSet = new HashSet<long>();
		foreach (NKMUnitData item2 in currentUnitList)
		{
			if (item2 != null)
			{
				hashSet.Add(item2.m_UnitUID);
			}
		}
		foreach (NKMUnitData value3 in ((m_currentTargetUnitType == TargetTabType.Ship) ? NKCScenManager.CurrentUserData().m_ArmyData.m_dicMyShip : NKCScenManager.CurrentUserData().m_ArmyData.m_dicMyUnit).Values)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(value3);
			if (unitTempletBase == null)
			{
				continue;
			}
			bool flag = m_ssActive.GetDeckIndexCache(value3.m_UnitUID).m_eDeckType != NKM_DECK_TYPE.NDT_NONE;
			int key;
			if (unitTempletBase.IsRearmUnit)
			{
				NKMUnitTempletBase baseUnit = unitTempletBase.BaseUnit;
				key = ((baseUnit != null && baseUnit.m_bContractable) ? baseUnit.m_BaseUnitID : value3.m_UnitID);
			}
			else
			{
				key = value3.m_UnitID;
			}
			Dictionary<int, List<NKMUnitData>> dictionary3;
			if (value3.IsActiveUnit || flag)
			{
				dictionary3 = dictionary;
			}
			else
			{
				if (!setGrade.Contains(unitTempletBase.m_NKM_UNIT_GRADE))
				{
					continue;
				}
				dictionary3 = dictionary2;
			}
			if (!dictionary3.ContainsKey(key))
			{
				dictionary3[key] = new List<NKMUnitData>();
			}
			dictionary3[key].Add(value3);
		}
		Dictionary<int, int> dictionary4 = new Dictionary<int, int>();
		foreach (KeyValuePair<int, List<NKMUnitData>> item3 in dictionary)
		{
			NKMUnitManager.GetUnitTempletBase(item3.Key);
			if (!dictionary4.TryGetValue(item3.Key, out var value))
			{
				value = 0;
			}
			foreach (NKMUnitData item4 in item3.Value)
			{
				value += 6 - item4.tacticLevel;
			}
			dictionary4[item3.Key] = value;
		}
		foreach (KeyValuePair<int, List<NKMUnitData>> item5 in dictionary2)
		{
			item5.Value.Sort(delegate(NKMUnitData a, NKMUnitData b)
			{
				NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(a);
				NKMUnitTempletBase unitTempletBase3 = NKMUnitManager.GetUnitTempletBase(b);
				bool flag2 = unitTempletBase2?.IsRearmUnit ?? false;
				bool flag3 = unitTempletBase3?.IsRearmUnit ?? false;
				return (flag2 != flag3) ? flag2.CompareTo(flag3) : b.FromContract.CompareTo(a.FromContract);
			});
			if (!dictionary4.TryGetValue(item5.Key, out var value2))
			{
				value2 = 0;
			}
			int num = item5.Value.Count - value2;
			if (num < 0)
			{
				continue;
			}
			for (int num2 = 0; num2 < num; num2++)
			{
				if (item5.Value[num2] != null && hashSet.Contains(item5.Value[num2].m_UnitUID) && m_ssActive.GetUnitSlotState(item5.Value[num2].m_UnitUID) == NKCUnitSortSystem.eUnitState.NONE)
				{
					m_listSelectedUnit.Add(item5.Value[num2].m_UnitUID);
					NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase2 = FindSlotFromCurrentList(item5.Value[num2].m_UnitUID);
					if (nKCUIUnitSelectListSlotBase2 != null)
					{
						nKCUIUnitSelectListSlotBase2.SetSlotSelectState((!m_currentOption.bShowRemoveItem) ? eUnitSlotSelectState.SELECTED : eUnitSlotSelectState.DELETE);
					}
					if (m_listSelectedUnit.Count >= m_currentOption.iMaxMultipleSelect)
					{
						break;
					}
				}
			}
			if (m_listSelectedUnit.Count >= m_currentOption.iMaxMultipleSelect)
			{
				break;
			}
		}
		UpdateMultipleSelectCountUI();
		UpdateMultiSelectGetItemResult();
	}

	private void OnAutoSelectByGrade(NKM_UNIT_GRADE grade)
	{
		HashSet<long> hashSet = new HashSet<long>();
		int num = 0;
		while (num + m_listSelectedUnit.Count < m_currentOption.iMaxMultipleSelect)
		{
			NKMUnitData nKMUnitData = m_ssActive.AutoSelect(hashSet, (NKMUnitData unit) => AutoSelectFilter(unit, grade));
			if (nKMUnitData == null)
			{
				break;
			}
			hashSet.Add(nKMUnitData.m_UnitUID);
			if (!m_listSelectedUnit.Contains(nKMUnitData.m_UnitUID))
			{
				num++;
			}
		}
		if (num > 0)
		{
			foreach (long item in hashSet)
			{
				if (!m_listSelectedUnit.Contains(item))
				{
					m_listSelectedUnit.Add(item);
					NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase = FindSlotFromCurrentList(item);
					if (nKCUIUnitSelectListSlotBase != null)
					{
						nKCUIUnitSelectListSlotBase.SetSlotSelectState((!m_currentOption.bShowRemoveItem) ? eUnitSlotSelectState.SELECTED : eUnitSlotSelectState.DELETE);
					}
				}
			}
		}
		else
		{
			NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
			hashSet.Clear();
			for (int num2 = 0; num2 < m_listSelectedUnit.Count; num2++)
			{
				long num3 = m_listSelectedUnit[num2];
				NKMUnitData unitFromUID = armyData.GetUnitFromUID(num3);
				if (unitFromUID != null)
				{
					NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitFromUID.m_UnitID);
					if (unitTempletBase != null && unitTempletBase.m_NKM_UNIT_GRADE == grade)
					{
						hashSet.Add(num3);
					}
				}
			}
			foreach (long item2 in hashSet)
			{
				m_listSelectedUnit.Remove(item2);
				NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase2 = FindSlotFromCurrentList(item2);
				if (nKCUIUnitSelectListSlotBase2 != null)
				{
					nKCUIUnitSelectListSlotBase2.SetSlotSelectState(eUnitSlotSelectState.NONE);
				}
			}
		}
		UpdateMultipleSelectCountUI();
		UpdateMultiSelectGetItemResult();
	}

	private bool AutoSelectFilter(NKMUnitData unitData, NKM_UNIT_GRADE grade = NKM_UNIT_GRADE.NUG_COUNT)
	{
		if (grade != NKM_UNIT_GRADE.NUG_COUNT)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
			if (unitTempletBase != null && unitTempletBase.m_NKM_UNIT_GRADE != grade)
			{
				return false;
			}
		}
		if (!CanRemoveBasicShip(unitData))
		{
			return false;
		}
		if (m_currentOption.dOnAutoSelectFilter != null)
		{
			return m_currentOption.dOnAutoSelectFilter(unitData);
		}
		return false;
	}

	public override void OnBackButton()
	{
		if (m_popupSmartSelect != null && m_popupSmartSelect.IsOpen)
		{
			m_popupSmartSelect.Close();
			return;
		}
		if (IsRemoveMode)
		{
			OnRemoveMode(bValue: false);
			return;
		}
		if (IsLockMode)
		{
			OnLockModeButton(bValue: false);
			return;
		}
		if (IsExtractOperatorMode)
		{
			OnExtractOperatorMode(bValue: false);
			return;
		}
		base.OnBackButton();
		if (m_currentOption.dOnClose != null)
		{
			m_currentOption.dOnClose();
		}
	}

	public override void UnHide()
	{
		base.UnHide();
		SetUnitListAddEffect(bActive: false);
		if (!m_bDataValid)
		{
			ProcessByType(m_currentTargetUnitType, bForceRebuildList: true);
			SetUnitCount(m_currentTargetUnitType);
		}
	}

	public override void OnCloseInstance()
	{
		if (m_stkUnitSlotPool != null)
		{
			while (m_stkUnitSlotPool.Count > 0)
			{
				Object.Destroy(m_stkUnitSlotPool.Pop().gameObject);
			}
		}
		if (m_stkUnitCastingBanSlotPool != null)
		{
			while (m_stkUnitCastingBanSlotPool.Count > 0)
			{
				Object.Destroy(m_stkUnitCastingBanSlotPool.Pop().gameObject);
			}
		}
		if (m_stkShipSlotPool != null)
		{
			while (m_stkShipSlotPool.Count > 0)
			{
				Object.Destroy(m_stkShipSlotPool.Pop().gameObject);
			}
		}
		if (m_stkOperCastingBanSlotPool != null)
		{
			while (m_stkOperCastingBanSlotPool.Count > 0)
			{
				Object.Destroy(m_stkOperCastingBanSlotPool.Pop().gameObject);
			}
		}
		if (m_lstVisibleSlot != null)
		{
			for (int i = 0; i < m_lstVisibleSlot.Count; i++)
			{
				Object.Destroy(m_lstVisibleSlot[i].gameObject);
			}
			m_lstVisibleSlot.Clear();
		}
	}

	public override void CloseInternal()
	{
		SetUnitListAddEffect(bActive: false);
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
		m_ssActive = null;
		m_dicUnitSortSystem.Clear();
		for (int i = 0; i < m_listBotSlot.Count; i++)
		{
			Object.Destroy(m_listBotSlot[i].gameObject);
		}
		m_listBotSlot.Clear();
	}

	private void OnSortChanged(bool bResetScroll)
	{
		if (m_currentTargetUnitType == TargetTabType.Operator && m_OperatorSortSystem != null)
		{
			if (m_currentOption.bShowRemoveSlot)
			{
				m_LoopScrollRect.TotalCount = m_OperatorSortSystem.SortedOperatorList.Count + 1;
			}
			else
			{
				m_LoopScrollRect.TotalCount = m_OperatorSortSystem.SortedOperatorList.Count;
			}
			if (bResetScroll)
			{
				m_LoopScrollRect.SetIndexPosition(0);
			}
			else
			{
				m_LoopScrollRect.RefreshCells();
			}
			NKCUtil.SetLabelText(m_lbEmptyMessage, m_currentOption.strEmptyMessage);
			NKCUtil.SetGameobjectActive(m_objEmpty, m_LoopScrollRect.TotalCount == 0);
		}
		else if (m_ssActive != null)
		{
			if (m_currentOption.bShowRemoveSlot)
			{
				m_LoopScrollRect.TotalCount = m_ssActive.SortedUnitList.Count + 1;
			}
			else
			{
				m_LoopScrollRect.TotalCount = m_ssActive.SortedUnitList.Count;
			}
			if (bResetScroll)
			{
				m_LoopScrollRect.SetIndexPosition(0);
			}
			else
			{
				m_LoopScrollRect.RefreshCells();
			}
			NKCUtil.SetLabelText(m_lbEmptyMessage, m_currentOption.strEmptyMessage);
			NKCUtil.SetGameobjectActive(m_objEmpty, m_LoopScrollRect.TotalCount == 0);
		}
	}

	public void OnSelectUnitMode(bool value)
	{
		if (value)
		{
			SetUnitListAddEffect(bActive: false);
			m_currentOption.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
			m_currentOption.strEmptyMessage = NKCUtilString.GET_STRING_UNIT_SELECT_UNIT_NO_EXIST;
			ProcessByType(TargetTabType.Unit);
			SetUnitCount(TargetTabType.Unit);
		}
	}

	public void OnSelectShipMode(bool value)
	{
		if (value)
		{
			SetUnitListAddEffect(bActive: false);
			m_currentOption.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
			m_currentOption.strEmptyMessage = NKCUtilString.GET_STRING_UNIT_SELECT_SHIP_NO_EXIST;
			ProcessByType(TargetTabType.Ship);
			SetUnitCount(TargetTabType.Ship);
		}
	}

	public void OnSelectOperatorMode(bool value)
	{
		if (!NKCOperatorUtil.IsHide() && !NKCOperatorUtil.IsActive())
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.OPERATOR);
			return;
		}
		if (value)
		{
			SetUnitListAddEffect(bActive: false);
			m_currentOption.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
			m_currentOption.strEmptyMessage = NKCUtilString.GET_STRING_UNIT_SELECT_OPERATOR_NO_EXIST;
			ProcessByType(TargetTabType.Operator);
			SetUnitCount(TargetTabType.Operator);
		}
		NKCUtil.SetGameobjectActive(m_btnOperatorExtract.gameObject, m_bOpenOperatorExtractMode && m_currentTargetUnitType == TargetTabType.Operator && value);
		CheckTutorial();
	}

	public void OnSelectTrophyMode(bool value)
	{
		if (value)
		{
			SetUnitListAddEffect(bActive: false);
			m_currentOption.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
			m_currentOption.strEmptyMessage = NKCUtilString.GET_STRING_UNIT_SELECT_TROPHY_NO_EXIST;
			ProcessByType(TargetTabType.Trophy);
			SetUnitCount(TargetTabType.Trophy);
		}
	}

	public void OnExpandInventoryPopup()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		NKM_INVENTORY_EXPAND_TYPE inventoryType = NKM_INVENTORY_EXPAND_TYPE.NIET_NONE;
		string text = "";
		NKCPopupInventoryAdd.SliderInfo sliderInfo = default(NKCPopupInventoryAdd.SliderInfo);
		int num = 0;
		switch (m_currentTargetUnitType)
		{
		default:
			return;
		case TargetTabType.Unit:
			inventoryType = NKM_INVENTORY_EXPAND_TYPE.NIET_UNIT;
			text = NKCUtilString.GET_STRING_INVENTORY_UNIT;
			sliderInfo.increaseCount = 5;
			sliderInfo.maxCount = 1100;
			sliderInfo.currentCount = myUserData.m_ArmyData.m_MaxUnitCount;
			num = 100;
			break;
		case TargetTabType.Ship:
			inventoryType = NKM_INVENTORY_EXPAND_TYPE.NIET_SHIP;
			text = NKCUtilString.GET_STRING_INVENTORY_SHIP;
			sliderInfo.increaseCount = 1;
			sliderInfo.maxCount = 60;
			sliderInfo.currentCount = myUserData.m_ArmyData.m_MaxShipCount;
			num = 100;
			break;
		case TargetTabType.Operator:
			inventoryType = NKM_INVENTORY_EXPAND_TYPE.NIET_OPERATOR;
			text = NKCUtilString.GET_STRING_INVEITORY_OPERATOR_TITLE;
			sliderInfo.increaseCount = 5;
			sliderInfo.maxCount = 500;
			sliderInfo.currentCount = myUserData.m_ArmyData.m_MaxOperatorCount;
			num = 100;
			break;
		case TargetTabType.Trophy:
			inventoryType = NKM_INVENTORY_EXPAND_TYPE.NIET_TROPHY;
			text = NKCUtilString.GET_STRING_TROPHY_UNIT;
			sliderInfo.increaseCount = 10;
			sliderInfo.maxCount = 2000;
			sliderInfo.currentCount = myUserData.m_ArmyData.m_MaxTrophyCount;
			num = 50;
			break;
		}
		sliderInfo.inventoryType = inventoryType;
		int count = 1;
		int resultCount;
		bool flag = !NKCAdManager.IsAdRewardInventory(inventoryType) || !NKMInventoryManager.CanExpandInventoryByAd(inventoryType, myUserData, count, out resultCount);
		if (!NKMInventoryManager.CanExpandInventory(inventoryType, myUserData, count, out resultCount) && flag)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_CANNOT_EXPAND_INVENTORY));
			return;
		}
		string expandDesc = NKCUtilString.GetExpandDesc(inventoryType);
		NKCPopupInventoryAdd.Instance.Open(text, expandDesc, sliderInfo, num, 101, delegate(int value)
		{
			NKCPacketSender.Send_NKMPacket_INVENTORY_EXPAND_REQ(inventoryType, value);
		});
	}

	public void OnExpandInventory()
	{
		SetUnitListAddEffect(bActive: false);
		SetUnitListAddEffect(bActive: true);
	}

	public void UpdateUnitCount()
	{
		SetUnitCount(m_currentTargetUnitType);
	}

	private void SetUnitCount(TargetTabType type)
	{
		if (m_currentOption.m_UnitSelectListMode == eUnitSelectListMode.Normal)
		{
			NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
			switch (type)
			{
			default:
				SetUnitCount($"{armyData.GetCurrentUnitCount()}/{armyData.m_MaxUnitCount}", NKCUtilString.GET_STRING_UNIT_SELECT_HAVE_COUNT);
				break;
			case TargetTabType.Ship:
				SetUnitCount($"{armyData.GetCurrentShipCount()}/{armyData.m_MaxShipCount}", NKCUtilString.GET_STRING_UNIT_SELECT_HAVE_COUNT);
				break;
			case TargetTabType.Operator:
				SetUnitCount($"{armyData.GetCurrentOperatorCount()}/{armyData.m_MaxOperatorCount}", NKCUtilString.GET_STRING_UNIT_SELECT_HAVE_COUNT);
				break;
			case TargetTabType.Trophy:
				SetUnitCount($"{armyData.GetCurrentTrophyCount()}/{armyData.m_MaxTrophyCount}", NKCUtilString.GET_STRING_UNIT_SELECT_HAVE_COUNT);
				break;
			}
		}
		else
		{
			SetUnitCount("", "");
		}
	}

	private void SetUnitCount(string unitCnt, string unitCntDesc)
	{
		NKCUtil.SetLabelText(m_lbUnitCount, unitCnt);
		NKCUtil.SetLabelText(m_lbUnitCountDesc, unitCntDesc);
	}

	public void ChangeUnitDeckIndex(long UID, NKMDeckIndex deckIndex)
	{
		foreach (KeyValuePair<TargetTabType, NKCUnitSortSystem> item in m_dicUnitSortSystem)
		{
			item.Value?.SetDeckIndexCache(UID, deckIndex);
		}
		NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase = FindSlotFromCurrentList(UID);
		if (nKCUIUnitSelectListSlotBase != null)
		{
			nKCUIUnitSelectListSlotBase.SetDeckIndex(deckIndex);
		}
	}

	private NKCUIUnitSelectListSlotBase FindSlotFromCurrentList(long UID)
	{
		if (m_currentTargetUnitType == TargetTabType.Operator)
		{
			return m_lstVisibleSlot.Find((NKCUIUnitSelectListSlotBase x) => x.gameObject.activeSelf && x.NKMOperatorData != null && x.NKMOperatorData.uid == UID);
		}
		return m_lstVisibleSlot.Find((NKCUIUnitSelectListSlotBase x) => x.gameObject.activeSelf && x.NKMUnitData != null && x.NKMUnitData.m_UnitUID == UID);
	}

	public override void OnUnitUpdate(NKMUserData.eChangeNotifyType eEventType, NKM_UNIT_TYPE eUnitType, long uid, NKMUnitData unitData)
	{
		TargetTabType targetTabType;
		switch (eUnitType)
		{
		default:
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
			targetTabType = ((unitTempletBase != null && unitTempletBase.IsTrophy) ? TargetTabType.Trophy : TargetTabType.Unit);
			break;
		}
		case NKM_UNIT_TYPE.NUT_OPERATOR:
			targetTabType = TargetTabType.Operator;
			break;
		case NKM_UNIT_TYPE.NUT_SHIP:
			targetTabType = TargetTabType.Ship;
			break;
		}
		switch (eEventType)
		{
		case NKMUserData.eChangeNotifyType.Add:
		case NKMUserData.eChangeNotifyType.Remove:
			if (targetTabType == m_currentTargetUnitType)
			{
				if (m_bHide)
				{
					m_bDataValid = false;
				}
				else
				{
					ProcessByType(m_currentTargetUnitType, bForceRebuildList: true);
				}
			}
			else
			{
				m_dicUnitSortSystem.Remove(targetTabType);
			}
			break;
		case NKMUserData.eChangeNotifyType.Update:
			if (m_dicUnitSortSystem.ContainsKey(targetTabType))
			{
				m_dicUnitSortSystem[targetTabType].UpdateLimitBreakProcessCache();
				m_dicUnitSortSystem[targetTabType].UpdateTacticUpdateProcessCache();
				m_dicUnitSortSystem[targetTabType].UpdateUnitData(unitData);
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_UNIT_LIST)
			{
				NKCScenManager.GetScenManager().GET_NKC_SCEN_UNIT_LIST().OnUnitUpdate(uid, unitData);
			}
			if (targetTabType == m_currentTargetUnitType)
			{
				NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase = FindSlotFromCurrentList(unitData.m_UnitUID);
				if (nKCUIUnitSelectListSlotBase != null)
				{
					SetSlotData(nKCUIUnitSelectListSlotBase, unitData);
				}
			}
			break;
		}
	}

	public override void OnOperatorUpdate(NKMUserData.eChangeNotifyType eEventType, long uid, NKMOperator operatorData)
	{
		switch (eEventType)
		{
		case NKMUserData.eChangeNotifyType.Add:
		case NKMUserData.eChangeNotifyType.Remove:
			if (TargetTabType.Operator == m_currentTargetUnitType)
			{
				if (m_bHide)
				{
					m_bDataValid = false;
				}
				else
				{
					ProcessByType(m_currentTargetUnitType, bForceRebuildList: true);
				}
			}
			else
			{
				m_dicUnitSortSystem.Remove(TargetTabType.Operator);
			}
			break;
		case NKMUserData.eChangeNotifyType.Update:
		{
			if (TargetTabType.Operator != m_currentTargetUnitType)
			{
				break;
			}
			NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase = FindSlotFromCurrentList(uid);
			if (nKCUIUnitSelectListSlotBase != null)
			{
				SetSlotData(nKCUIUnitSelectListSlotBase, operatorData);
			}
			{
				foreach (NKMOperator sortedOperator in m_OperatorSortSystem.SortedOperatorList)
				{
					if (sortedOperator.uid == operatorData.uid)
					{
						sortedOperator.level = operatorData.level;
						sortedOperator.exp = operatorData.exp;
						sortedOperator.mainSkill = operatorData.mainSkill;
						sortedOperator.subSkill = operatorData.subSkill;
					}
				}
				break;
			}
		}
		}
	}

	public override void OnDeckUpdate(NKMDeckIndex deckIndex, NKMDeckData deckData)
	{
		ProcessByType(m_currentTargetUnitType, bForceRebuildList: true);
	}

	private void UpdateMultiOkButton()
	{
		if (m_currentOption.bMultipleSelect)
		{
			if (m_currentOption.bShowRemoveItem)
			{
				m_cbtnMultipleSelectOK_img.sprite = m_spriteButton_03;
				m_cbtnMultipleSelectOK_text.color = Color.white;
			}
			else
			{
				m_cbtnMultipleSelectOK_img.sprite = m_spriteButton_01;
				m_cbtnMultipleSelectOK_text.color = NKCUtil.GetColor("#582817");
			}
		}
	}

	public static NKCUnitSortSystem GetUnitDummySortSystem(UnitSelectListOptions options)
	{
		switch (options.m_UnitSelectListMode)
		{
		default:
			return options.eTargetUnitType switch
			{
				TargetTabType.Ship => new NKCShipSort(NKCScenManager.CurrentUserData(), options.m_SortOptions), 
				TargetTabType.Trophy => new NKCGenericUnitSort(NKCScenManager.CurrentUserData(), options.m_SortOptions, NKCScenManager.CurrentUserData().m_ArmyData.m_dicMyTrophy.Values), 
				_ => new NKCUnitSort(NKCScenManager.CurrentUserData(), options.m_SortOptions), 
			};
		case eUnitSelectListMode.ALLUNIT_DEV:
			switch (options.eTargetUnitType)
			{
			default:
				return new NKCAllUnitSort(NKCScenManager.CurrentUserData(), options.m_SortOptions);
			case TargetTabType.Ship:
				return new NKCAllShipSort(NKCScenManager.CurrentUserData(), options.m_SortOptions);
			case TargetTabType.Trophy:
			{
				List<NKMUnitData> list2 = new List<NKMUnitData>();
				foreach (NKMUnitTempletBase value in NKMTempletContainer<NKMUnitTempletBase>.Values)
				{
					if (value.IsTrophy)
					{
						list2.Add(NKCUnitSortSystem.MakeTempUnitData(value.m_UnitID, 1, 0));
					}
				}
				return new NKCGenericUnitSort(NKCScenManager.CurrentUserData(), options.m_SortOptions, list2);
			}
			}
		case eUnitSelectListMode.ALLSKIN_DEV:
			switch (options.eTargetUnitType)
			{
			default:
			{
				List<NKMUnitData> list = new List<NKMUnitData>();
				foreach (NKMSkinTemplet value2 in NKMSkinManager.m_dicSkinTemplet.Values)
				{
					list.Add(NKCUnitSortSystem.MakeTempUnitData(value2, 1, 0));
				}
				return new NKCGenericUnitSort(NKCScenManager.CurrentUserData(), options.m_SortOptions, list);
			}
			case TargetTabType.Ship:
				return new NKCAllShipSort(NKCScenManager.CurrentUserData(), options.m_SortOptions);
			case TargetTabType.Trophy:
				return new NKCGenericUnitSort(NKCScenManager.CurrentUserData(), options.m_SortOptions, new List<NKMUnitData>());
			}
		}
	}

	private void SetUnitListAddEffect(bool bActive)
	{
		NKCUtil.SetGameobjectActive(m_objUnitListAddEffect, bActive);
	}

	public void UpdateOperatorLockState(long operatorUID, bool bLock)
	{
		if (NKCOperatorUtil.IsActive())
		{
			NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase = m_lstVisibleSlot.Find((NKCUIUnitSelectListSlotBase x) => x.NKMUnitData.m_UnitUID == operatorUID);
			if (nKCUIUnitSelectListSlotBase != null)
			{
				nKCUIUnitSelectListSlotBase.SetLock(bLock);
			}
		}
	}

	private void MoveToScrollRectSelectedUnit()
	{
		if (m_lastSelectedUnitUID != 0L && m_currentTargetUnitType == TargetTabType.Unit && NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(m_lastSelectedUnitUID) != null)
		{
			ScrollToUnitAndGetRect(m_lastSelectedUnitUID);
		}
	}

	public List<long> GetSelectedUnitList()
	{
		return m_listSelectedUnit;
	}

	public RectTransform ScrollToUnitAndGetRect(long UID)
	{
		int num = m_ssActive.SortedUnitList.FindIndex((NKMUnitData x) => x.m_UnitUID == UID);
		if (num < 0)
		{
			Debug.LogError("Target unit not found!!");
			return null;
		}
		if (m_currentOption.bShowRemoveSlot)
		{
			num++;
		}
		m_LoopScrollRect.SetIndexPosition(num);
		NKCUIUnitSelectListSlotBase nKCUIUnitSelectListSlotBase = m_lstVisibleSlot.Find((NKCUIUnitSelectListSlotBase x) => x.NKMUnitData.m_UnitUID == UID);
		if (nKCUIUnitSelectListSlotBase == null)
		{
			return null;
		}
		return nKCUIUnitSelectListSlotBase.gameObject.GetComponent<RectTransform>();
	}

	public void CheckTutorial()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.OperatorList);
	}
}
