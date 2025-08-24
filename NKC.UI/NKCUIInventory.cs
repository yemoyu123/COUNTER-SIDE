using System.Collections.Generic;
using Cs.Logging;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIInventory : NKCUIBase
{
	public struct EquipSelectListOptions
	{
		public delegate void OnSlotSetData(NKCUISlotEquip cEquipSlot, NKMEquipItemData cNKMEquipData, NKMDeckIndex deckIndex);

		public delegate void OnClose();

		public delegate bool CustomFilterFunc(int id, NKC_INVENTORY_OPEN_TYPE type);

		public NKCEquipSortSystem.EquipListOptions m_EquipListOptions;

		public NKC_INVENTORY_OPEN_TYPE m_NKC_INVENTORY_OPEN_TYPE;

		public bool bShowRemoveSlot;

		public bool bMultipleSelect;

		public int iMaxMultipleSelect;

		public bool bEnableLockEquipSystem;

		public bool bUseMainEquipMark;

		public bool bShowRemoveItem;

		public bool bSkipItemEquipBox;

		public bool bShowFierceUI;

		public string strUpsideMenuName;

		public string strEmptyMessage;

		public string strGuideTempletID;

		public ITEM_EQUIP_POSITION equipChangeTargetPosition;

		public long lastSelectedItemUID;

		public long lEquipOptionCachingByUnitUID;

		public HashSet<long> m_hsSelectedEquipUIDToShow;

		public NKM_UNIT_STYLE_TYPE presetUnitStyeType;

		public int iPresetIndex;

		public OnClose dOnClose;

		public NKCUISlot.OnClick m_dOnClickItemSlot;

		public NKCUISlotEquip.OnSelectedEquipSlot m_dOnSelectedEquipSlot;

		public NKCUISlot.OnClick m_dOnClickItemMoldSlot;

		public OnFinishMultiSelection m_dOnFinishMultiSelection;

		public NKCUISlotEquip.OnSelectedEquipSlot m_dOnClickEmptySlot;

		public OnGetItemListAfterSelected m_dOnGetItemListAfterSelected;

		public CustomFilterFunc AdditionalExcludeFilterFunc;

		public NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE m_ButtonMenuType;

		public bool bShowEquipUpgradeState;

		public HashSet<NKCEquipSortSystem.eFilterOption> setFilterOption
		{
			get
			{
				return m_EquipListOptions.setFilterOption;
			}
			set
			{
				m_EquipListOptions.setFilterOption = value;
			}
		}

		public List<NKCEquipSortSystem.eSortOption> lstSortOption
		{
			get
			{
				return m_EquipListOptions.lstSortOption;
			}
			set
			{
				m_EquipListOptions.lstSortOption = value;
			}
		}

		public bool bPushBackUnselectable
		{
			get
			{
				return m_EquipListOptions.bPushBackUnselectable;
			}
			set
			{
				m_EquipListOptions.bPushBackUnselectable = value;
			}
		}

		public bool bHideEquippedItem
		{
			get
			{
				return m_EquipListOptions.bHideEquippedItem;
			}
			set
			{
				m_EquipListOptions.bHideEquippedItem = value;
			}
		}

		public bool bHideLockItem
		{
			get
			{
				return m_EquipListOptions.bHideLockItem;
			}
			set
			{
				m_EquipListOptions.bHideLockItem = value;
			}
		}

		public bool bLockMaxItem
		{
			get
			{
				return m_EquipListOptions.bLockMaxItem;
			}
			set
			{
				m_EquipListOptions.bLockMaxItem = value;
			}
		}

		public bool bHideMaxLvItem
		{
			get
			{
				return m_EquipListOptions.bHideMaxLvItem;
			}
			set
			{
				m_EquipListOptions.bHideMaxLvItem = value;
			}
		}

		public HashSet<long> setExcludeEquipUID
		{
			get
			{
				return m_EquipListOptions.setExcludeEquipUID;
			}
			set
			{
				m_EquipListOptions.setExcludeEquipUID = value;
			}
		}

		public HashSet<int> setExcludeEquipID
		{
			get
			{
				return m_EquipListOptions.setExcludeEquipID;
			}
			set
			{
				m_EquipListOptions.setExcludeEquipID = value;
			}
		}

		public HashSet<int> setOnlyIncludeEquipID
		{
			get
			{
				return m_EquipListOptions.setOnlyIncludeEquipID;
			}
			set
			{
				m_EquipListOptions.setOnlyIncludeEquipID = value;
			}
		}

		public EquipSelectListOptions(NKC_INVENTORY_OPEN_TYPE inventoryOpenType, bool _bMultipleSelect, bool bUseDefaultString = true)
		{
			m_NKC_INVENTORY_OPEN_TYPE = inventoryOpenType;
			bShowRemoveSlot = false;
			bMultipleSelect = _bMultipleSelect;
			iMaxMultipleSelect = 8;
			bEnableLockEquipSystem = false;
			bUseMainEquipMark = false;
			bShowRemoveItem = false;
			bSkipItemEquipBox = false;
			bShowFierceUI = false;
			presetUnitStyeType = NKM_UNIT_STYLE_TYPE.NUST_INVALID;
			iPresetIndex = -1;
			if (bUseDefaultString)
			{
				strUpsideMenuName = NKCUtilString.GET_STRING_INVEN_EQUIP_SELECT;
			}
			else
			{
				strUpsideMenuName = "";
			}
			strEmptyMessage = "";
			strGuideTempletID = "";
			equipChangeTargetPosition = ITEM_EQUIP_POSITION.IEP_NONE;
			m_hsSelectedEquipUIDToShow = new HashSet<long>();
			dOnClose = null;
			m_dOnClickItemSlot = null;
			m_dOnSelectedEquipSlot = null;
			m_dOnClickItemMoldSlot = null;
			m_dOnFinishMultiSelection = null;
			m_dOnClickEmptySlot = null;
			m_dOnGetItemListAfterSelected = null;
			AdditionalExcludeFilterFunc = null;
			m_EquipListOptions = default(NKCEquipSortSystem.EquipListOptions);
			m_EquipListOptions.setOnlyIncludeEquipID = null;
			m_EquipListOptions.setExcludeEquipID = null;
			m_EquipListOptions.setExcludeEquipUID = null;
			m_EquipListOptions.setExcludeFilterOption = null;
			m_EquipListOptions.setFilterOption = new HashSet<NKCEquipSortSystem.eFilterOption>();
			m_EquipListOptions.lstSortOption = new List<NKCEquipSortSystem.eSortOption>();
			m_EquipListOptions.lstSortOption = NKCEquipSortSystem.AddDefaultSortOptions(m_EquipListOptions.lstSortOption);
			m_EquipListOptions.PreemptiveSortFunc = null;
			m_EquipListOptions.AdditionalExcludeFilterFunc = null;
			m_EquipListOptions.bHideEquippedItem = false;
			m_EquipListOptions.bPushBackUnselectable = true;
			m_EquipListOptions.bHideLockItem = false;
			m_EquipListOptions.bHideMaxLvItem = false;
			m_EquipListOptions.bLockMaxItem = false;
			m_EquipListOptions.OwnerUnitID = 0;
			m_EquipListOptions.bHideNotPossibleSetOptionItem = false;
			m_EquipListOptions.iTargetUnitID = 0;
			m_EquipListOptions.FilterStatType_01 = NKM_STAT_TYPE.NST_RANDOM;
			m_EquipListOptions.FilterStatType_02 = NKM_STAT_TYPE.NST_RANDOM;
			m_EquipListOptions.FilterStatType_Potential = NKM_STAT_TYPE.NST_RANDOM;
			m_EquipListOptions.FilterSetOptionID = 0;
			m_EquipListOptions.lstCustomSortFunc = new Dictionary<NKCEquipSortSystem.eSortCategory, KeyValuePair<string, NKCUnitSortSystem.NKCDataComparerer<NKMEquipItemData>.CompareFunc>>();
			m_EquipListOptions.bHideTokenFiltering = false;
			lastSelectedItemUID = 0L;
			lEquipOptionCachingByUnitUID = 0L;
			m_ButtonMenuType = NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_ENFORCE_AND_EQUIP;
			bShowEquipUpgradeState = false;
		}
	}

	public delegate void OnFinishMultiSelection(List<long> listEquipSlot);

	public delegate void OnGetItemListAfterSelected(List<NKMEquipItemData> lstItemData);

	public enum NKC_INVENTORY_TAB
	{
		NIT_NONE = -1,
		NIT_MISC,
		NIT_EQUIP,
		NIT_MOLD
	}

	public delegate void dChangeOptionNotify(HashSet<NKCEquipSortSystem.eFilterOption> setFilterOptions = null, int filterSetOptionID = 0);

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_inventory";

	private const string UI_ASSET_NAME = "NKM_UI_INVENTORY_V2";

	private static NKCUIInventory m_Instance;

	private GameObject m_NKM_UI_UNIT_INVENTORY;

	public RectTransform m_rtNKM_UI_INVENTORY_LIST_ITEM;

	public RectTransform m_RectMask;

	[Header("멀티 선택")]
	public GameObject m_NKM_UI_INVENTORY_MENU_CHOICE;

	public Text m_NKM_UI_INVENTORY_MENU_CHOICE_NUMBER_TEXT;

	public GameObject m_NKM_UI_INVENTORY_MENU_CHOICE_GET_ITEM;

	public Transform m_NKM_UI_UNIT_SELECT_LIST_CHOICE_GET_ITEM_LIST_Content;

	public NKCUIComButton m_cbtnFinishMultiSelect;

	public NKCUIComButton NKM_UI_INVENTORY_MENU_AUTO_BUTTON;

	public NKCUIComButton m_NKM_UI_INVENTORY_MENU_CANCEL_BUTTON;

	[Header("탭 버튼들")]
	public NKCUIComStateButton m_NKM_UI_INVENTORY_TAP_MISC;

	public NKCUIComStateButton m_NKM_UI_INVENTORY_TAP_GEAR;

	[Header("탭별 보이는 화면")]
	public GameObject m_objMisc;

	public GameObject m_objGear;

	[Header("비품탭에서 비활성화될것들")]
	public GameObject m_NKM_UI_INVENTORY_TEXTS;

	public GameObject m_NKM_UI_INVENTORY_TEXT1;

	public GameObject m_NKM_UI_INVENTORY_LINE;

	public GameObject m_NKM_UI_INVENTORY_TEXT;

	public GameObject m_NKM_UI_INVENTORY_NUMBER_TEXT;

	public NKCUIComToggle m_NKM_UI_INVENTORY_MENU_LOCK;

	public NKCUIComStateButton m_NKM_UI_INVENTORY_MENU_DELETE;

	public CanvasGroup m_NKM_UI_INVENTORY_MENU_DELETE_canvas;

	public NKCUIComStateButton m_NKM_UI_INVENTORY_ADD;

	public GameObject m_NKM_UI_INVENTORY_EQUIPMENTS_UNLOCK;

	public GameObject m_NKM_UI_INVENTORY_MENU_LOCK_MSG;

	public GameObject m_NKM_UI_INVENTORY_MENU_DELETE_MSG;

	[Header("필터링 유닛 타입 선택")]
	public NKCUIComStateButton m_btnFilterOption;

	public GameObject m_objFilterSelected;

	[Header("리스트가 비었을 때")]
	public GameObject m_objEmpty;

	public Text m_lbEmptyMessage;

	[Header("정렬")]
	public NKCUIComEquipSortOptions m_SortUI;

	public NKCUIComMiscSortOptions m_SortMiscUI;

	[Header("우측 정보창")]
	public GameObject m_objEquipNotSelected;

	public GameObject m_objEquipInfo;

	public NKCUIInvenEquipSlot m_slotEquipInfo;

	public NKCUIComStateButton m_UnEquipButton;

	public NKCUIComStateButton m_ReinforceButton;

	public NKCUIComStateButton m_ReinforceButtonLock;

	public NKCUIComStateButton m_EquipButton;

	public NKCUIComStateButton m_ChangeButton;

	public NKCUIComStateButton m_OkButton;

	[Header("Misc Loop Scroll")]
	public RectTransform m_rectContentRect;

	public RectTransform m_rectSlotPoolRect;

	public LoopScrollRect m_LoopScrollRect;

	public GridLayoutGroup m_GridLayoutGroup;

	public RectTransform m_NKM_UI_INVENTORY_LIST_ITEM;

	public NKCUIComSafeArea m_safeArea;

	[Header("Equip Loop Scroll")]
	public RectTransform m_rectContentRectEquip;

	public RectTransform m_rectSlotPoolRectEquip;

	public LoopScrollRect m_LoopScrollRectEquip;

	public GridLayoutGroup m_GridLayoutGroupEquip;

	public RectTransform m_NKM_UI_INVENTORY_LIST_ITEM_EQUIP;

	public NKCUIComSafeArea m_safeAreaEquip;

	[Header("장비 분해")]
	public NKCUIInventoryRemovePopup m_NKCUIInventoryRemovePopup;

	private EquipSelectListOptions m_currentOption = new EquipSelectListOptions(NKC_INVENTORY_OPEN_TYPE.NIOT_NORMAL, _bMultipleSelect: false, bUseDefaultString: false);

	private EquipSelectListOptions m_prevOption = new EquipSelectListOptions(NKC_INVENTORY_OPEN_TYPE.NIOT_NORMAL, _bMultipleSelect: false, bUseDefaultString: false);

	private NKCUISlotEquip m_LatestSelectedSlot;

	private List<NKCUISlot> m_lstBotNKCUISlot = new List<NKCUISlot>();

	private int m_LastMaxEnchantLevel;

	private HashSet<NKCEquipSortSystem.eFilterOption> m_hsLastAutoSelectFilter = new HashSet<NKCEquipSortSystem.eFilterOption>();

	private bool m_bNeedRefresh;

	private const int REMOVE_EQUIP_MAX = 100;

	private readonly HashSet<NKCEquipSortSystem.eFilterCategory> m_setEquipFilterCategory = new HashSet<NKCEquipSortSystem.eFilterCategory>
	{
		NKCEquipSortSystem.eFilterCategory.UnitType,
		NKCEquipSortSystem.eFilterCategory.EquipType,
		NKCEquipSortSystem.eFilterCategory.Rarity,
		NKCEquipSortSystem.eFilterCategory.Equipped,
		NKCEquipSortSystem.eFilterCategory.Tier,
		NKCEquipSortSystem.eFilterCategory.Locked,
		NKCEquipSortSystem.eFilterCategory.SetOptionPart,
		NKCEquipSortSystem.eFilterCategory.SetOptionType,
		NKCEquipSortSystem.eFilterCategory.StatType,
		NKCEquipSortSystem.eFilterCategory.PrivateEquip
	};

	private readonly HashSet<NKCEquipSortSystem.eSortCategory> m_setEquipSortCategory = new HashSet<NKCEquipSortSystem.eSortCategory>
	{
		NKCEquipSortSystem.eSortCategory.Enhance,
		NKCEquipSortSystem.eSortCategory.Tier,
		NKCEquipSortSystem.eSortCategory.Rarity,
		NKCEquipSortSystem.eSortCategory.SetOption,
		NKCEquipSortSystem.eSortCategory.UID
	};

	private NKCEquipSortSystem m_ssActive;

	private Dictionary<NKC_INVENTORY_OPEN_TYPE, NKCEquipSortSystem> m_dicEquipSortSystem = new Dictionary<NKC_INVENTORY_OPEN_TYPE, NKCEquipSortSystem>();

	private NKCMiscSortSystem m_ssActiveMisc;

	private List<int> RESOURCE_LIST;

	private NKC_INVENTORY_TAB m_NKC_INVENTORY_TAB = NKC_INVENTORY_TAB.NIT_NONE;

	private HashSet<long> m_hsCurrentSelectedEquips = new HashSet<long>();

	[Header("유닛 슬롯 프리팹 & 사이즈 설정")]
	public NKCUISlot m_pfbUISlot;

	public Vector2 m_vUISlotSize;

	public Vector2 m_vUISlotSpacing;

	public float m_vOffsetX;

	public NKCUISlotEquip m_pfbEquipSlot;

	public Vector2 m_vEquipSlotSize;

	public Vector2 m_vEquipSlotSpacing;

	public float m_vEquipOffsetX;

	private List<NKCUISlotEquip> m_lstVisibleEquipSlot = new List<NKCUISlotEquip>();

	private Stack<NKCUISlotEquip> m_stkEquipSlotPool = new Stack<NKCUISlotEquip>();

	private List<NKCUISlot> m_lstVisibleMiscSlot = new List<NKCUISlot>();

	private Stack<NKCUISlot> m_stkMiscSlotPool = new Stack<NKCUISlot>();

	private bool m_bLockMaxItem;

	private dChangeOptionNotify m_dChangeOptionNotify;

	private OnGetItemListAfterSelected m_dOnGetItemListAfterSelected;

	private NKCUISlotEquip.OnSelectedEquipSlot m_dOnClickEquipSlot;

	private NKCPopupEquipChange m_NKCPopupEquipChange;

	private long lastSelectItemUID;

	private ITEM_EQUIP_POSITION lastSelectEquipPos = ITEM_EQUIP_POSITION.IEP_NONE;

	private long m_beforeItemUID;

	private long m_afterItemUId;

	private ITEM_EQUIP_POSITION m_equipPosition = ITEM_EQUIP_POSITION.IEP_NONE;

	private NKMEquipItemData m_LatestOpenNKMEquipItemData;

	private long m_LatestTargetUnitUIDToEquip;

	private List<NKMItemMiscTemplet> m_lstMiscTempletData = new List<NKMItemMiscTemplet>();

	private List<NKMItemMoldTemplet> m_lstMoldTemplet = new List<NKMItemMoldTemplet>();

	private NKC_INVENTORY_OPEN_TYPE m_eInventoryOpenType;

	public static NKCUIInventory Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIInventory>("ab_ui_nkm_ui_inventory", "NKM_UI_INVENTORY_V2", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIInventory>();
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

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode
	{
		get
		{
			if (m_currentOption.m_NKC_INVENTORY_OPEN_TYPE == NKC_INVENTORY_OPEN_TYPE.NIOT_ITEM_DEV || m_currentOption.m_NKC_INVENTORY_OPEN_TYPE == NKC_INVENTORY_OPEN_TYPE.NIOT_EQUIP_DEV || m_currentOption.m_NKC_INVENTORY_OPEN_TYPE == NKC_INVENTORY_OPEN_TYPE.NIOT_MOLD_DEV)
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
			if (m_currentOption.m_NKC_INVENTORY_OPEN_TYPE == NKC_INVENTORY_OPEN_TYPE.NIOT_NORMAL)
			{
				return NKCUtilString.GET_STRING_INVEN;
			}
			if (m_currentOption.strUpsideMenuName.Length > 0)
			{
				return m_currentOption.strUpsideMenuName;
			}
			if (m_currentOption.m_NKC_INVENTORY_OPEN_TYPE == NKC_INVENTORY_OPEN_TYPE.NIOT_EQUIP_SELECT)
			{
				return NKCUtilString.GET_STRING_INVEN_EQUIP_SELECT;
			}
			return NKCUtilString.GET_STRING_INVEN;
		}
	}

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string GuideTempletID => "ARTICLE_SYSTEM_INVENTORY";

	public override List<int> UpsideMenuShowResourceList => RESOURCE_LIST;

	private NKCPopupEquipChange NKCPopupEquipChange
	{
		get
		{
			if (m_NKCPopupEquipChange == null)
			{
				NKCUIManager.LoadedUIData loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupEquipChange>("AB_UI_NKM_UI_UNIT_CHANGE_POPUP", "NKM_UI_EQUIP_CHANGE_POPUP", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), null);
				m_NKCPopupEquipChange = loadedUIData.GetInstance<NKCPopupEquipChange>();
				m_NKCPopupEquipChange?.InitUI();
			}
			return m_NKCPopupEquipChange;
		}
	}

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

	public static NKCUIInventory OpenNewInstance()
	{
		NKCUIInventory instance = NKCUIManager.OpenNewInstance<NKCUIInventory>("ab_ui_nkm_ui_inventory", "NKM_UI_INVENTORY_V2", NKCUIManager.eUIBaseRect.UIFrontCommon, null).GetInstance<NKCUIInventory>();
		if ((object)instance != null)
		{
			instance.InitUI();
			return instance;
		}
		return instance;
	}

	public EquipSelectListOptions GetNKCUIInventoryOption()
	{
		return m_currentOption;
	}

	private NKCEquipSortSystem GetEquipSortSystem(NKC_INVENTORY_OPEN_TYPE type)
	{
		if (m_dicEquipSortSystem.ContainsKey(type) && m_dicEquipSortSystem[type] != null)
		{
			NKCEquipSortSystem nKCEquipSortSystem = m_dicEquipSortSystem[type];
			nKCEquipSortSystem.BuildFilterAndSortedList(m_currentOption.setFilterOption, m_currentOption.lstSortOption, m_currentOption.bHideEquippedItem);
			return nKCEquipSortSystem;
		}
		if (m_NKC_INVENTORY_TAB == NKC_INVENTORY_TAB.NIT_EQUIP)
		{
			switch (type)
			{
			default:
			{
				NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
				if (nKMUserData != null)
				{
					NKCEquipSortSystem nKCEquipSortSystem2 = new NKCEquipSortSystem(nKMUserData, m_currentOption.m_EquipListOptions);
					m_dicEquipSortSystem[type] = nKCEquipSortSystem2;
					return nKCEquipSortSystem2;
				}
				return null;
			}
			case NKC_INVENTORY_OPEN_TYPE.NIOT_EQUIP_DEV:
			{
				List<NKMEquipItemData> list = new List<NKMEquipItemData>();
				foreach (KeyValuePair<int, NKMEquipTemplet> item2 in NKMItemManager.m_dicItemEquipTempletByID)
				{
					NKMEquipItemData item = NKCEquipSortSystem.MakeTempEquipData(item2.Value.m_ItemEquipID);
					list.Add(item);
				}
				NKCEquipSortSystem nKCEquipSortSystem2 = new NKCEquipSortSystem(NKCScenManager.CurrentUserData(), m_currentOption.m_EquipListOptions, list);
				m_dicEquipSortSystem[type] = nKCEquipSortSystem2;
				return nKCEquipSortSystem2;
			}
			case NKC_INVENTORY_OPEN_TYPE.NIOT_ITEM_DEV:
			case NKC_INVENTORY_OPEN_TYPE.NIOT_MOLD_DEV:
				return null;
			}
		}
		return null;
	}

	private void InitUI()
	{
		m_NKM_UI_UNIT_INVENTORY = base.gameObject;
		m_NKM_UI_UNIT_INVENTORY.SetActive(value: false);
		NKCUtil.SetBindFunction(m_cbtnFinishMultiSelect, FinishMultiSelection);
		NKCUtil.SetBindFunction(m_NKM_UI_INVENTORY_MENU_CANCEL_BUTTON, OnTouchMultiCancel);
		NKCUtil.SetBindFunction(NKM_UI_INVENTORY_MENU_AUTO_BUTTON, OnTouchAutoSelect);
		NKCUtil.SetBindFunction(m_NKM_UI_INVENTORY_ADD, OnExpandInventoryPopup);
		NKCUtil.SetBindFunction(m_NKM_UI_INVENTORY_TAP_MISC, OnSelectMiscTab);
		NKCUtil.SetBindFunction(m_NKM_UI_INVENTORY_TAP_GEAR, OnSelectEquipTab);
		NKCUtil.SetToggleValueChangedDelegate(m_NKM_UI_INVENTORY_MENU_LOCK, OnLockModeToggle);
		NKCUtil.SetBindFunction(m_NKM_UI_INVENTORY_MENU_DELETE, delegate
		{
			OnRemoveMode(bValue: true);
		});
		NKCUtil.SetBindFunction(m_EquipButton, OpenUnitSelect);
		NKCUtil.SetBindFunction(m_UnEquipButton, OnClickUnEquip);
		NKCUtil.SetBindFunction(m_ReinforceButton, OnClickEquipEnhance);
		NKCUtil.SetBindFunction(m_ReinforceButtonLock, OnClickEquipEnhance);
		NKCUtil.SetBindFunction(m_ChangeButton, OpenUnitSelect);
		NKCUtil.SetBindFunction(m_OkButton, OnClickOkButton);
		m_LoopScrollRect.dOnGetObject += GetSlot;
		m_LoopScrollRect.dOnReturnObject += ReturnSlot;
		m_LoopScrollRect.dOnProvideData += ProvideSlotData;
		m_LoopScrollRect.dOnRepopulate += CalculateContentRectSize;
		NKCUtil.SetScrollHotKey(m_LoopScrollRect);
		m_LoopScrollRectEquip.dOnGetObject += GetSlot;
		m_LoopScrollRectEquip.dOnReturnObject += ReturnSlot;
		m_LoopScrollRectEquip.dOnProvideData += ProvideSlotData;
		m_LoopScrollRectEquip.dOnRepopulate += CalculateContentRectSize;
		NKCUtil.SetScrollHotKey(m_LoopScrollRectEquip);
		if (m_NKCUIInventoryRemovePopup != null)
		{
			m_NKCUIInventoryRemovePopup.InitUI(OnAutoSelect);
		}
		if (m_SortUI != null)
		{
			m_SortUI.Init(ResetEquipSlotUI);
		}
		if (null != m_SortMiscUI)
		{
			m_SortMiscUI.Init(ResetMiscSlotUI);
		}
	}

	private void CalculateContentRectSize()
	{
		int minColumn = 7;
		Vector2 vUISlotSize = m_vUISlotSize;
		Vector2 vUISlotSpacing = m_vUISlotSpacing;
		float vOffsetX = m_vOffsetX;
		if (m_NKC_INVENTORY_TAB == NKC_INVENTORY_TAB.NIT_EQUIP)
		{
			minColumn = 5;
			vUISlotSize = m_vEquipSlotSize;
			vUISlotSpacing = m_vEquipSlotSpacing;
			vOffsetX = m_vEquipOffsetX;
			m_NKM_UI_INVENTORY_LIST_ITEM_EQUIP.offsetMin = new Vector2(vOffsetX, m_NKM_UI_INVENTORY_LIST_ITEM_EQUIP.offsetMin.y);
			m_NKM_UI_INVENTORY_LIST_ITEM_EQUIP.offsetMax = new Vector2(0f - vOffsetX, m_NKM_UI_INVENTORY_LIST_ITEM_EQUIP.offsetMax.y);
			if (m_safeAreaEquip != null)
			{
				m_safeAreaEquip.SetSafeAreaBase();
			}
			NKCUtil.CalculateContentRectSize(m_LoopScrollRectEquip, m_GridLayoutGroupEquip, minColumn, vUISlotSize, vUISlotSpacing);
			Debug.Log($"CellSize : {m_GridLayoutGroup.cellSize}, rectContentWidth : {m_rectContentRectEquip.GetWidth()}");
		}
		else
		{
			m_NKM_UI_INVENTORY_LIST_ITEM.offsetMin = new Vector2(vOffsetX, m_NKM_UI_INVENTORY_LIST_ITEM.offsetMin.y);
			m_NKM_UI_INVENTORY_LIST_ITEM.offsetMax = new Vector2(0f - vOffsetX, m_NKM_UI_INVENTORY_LIST_ITEM.offsetMax.y);
			if (m_safeArea != null)
			{
				m_safeArea.SetSafeAreaBase();
			}
			NKCUtil.CalculateContentRectSize(m_LoopScrollRect, m_GridLayoutGroup, minColumn, vUISlotSize, vUISlotSpacing);
			Debug.Log($"CellSize : {m_GridLayoutGroup.cellSize}, rectContentWidth : {m_rectContentRect.GetWidth()}");
		}
	}

	private RectTransform GetSlot(int index)
	{
		if (m_NKC_INVENTORY_TAB == NKC_INVENTORY_TAB.NIT_EQUIP)
		{
			if (m_stkEquipSlotPool.Count > 0)
			{
				NKCUISlotEquip nKCUISlotEquip = m_stkEquipSlotPool.Pop();
				NKCUtil.SetGameobjectActive(nKCUISlotEquip, bValue: true);
				m_lstVisibleEquipSlot.Add(nKCUISlotEquip);
				return nKCUISlotEquip.GetComponent<RectTransform>();
			}
			NKCUISlotEquip nKCUISlotEquip2 = Object.Instantiate(m_pfbEquipSlot);
			nKCUISlotEquip2.transform.SetParent(m_rectSlotPoolRectEquip);
			NKCUtil.SetGameobjectActive(nKCUISlotEquip2, bValue: true);
			m_lstVisibleEquipSlot.Add(nKCUISlotEquip2);
			return nKCUISlotEquip2.GetComponent<RectTransform>();
		}
		if (m_NKC_INVENTORY_TAB == NKC_INVENTORY_TAB.NIT_MISC || m_NKC_INVENTORY_TAB == NKC_INVENTORY_TAB.NIT_MOLD)
		{
			if (m_stkMiscSlotPool.Count > 0)
			{
				NKCUISlot nKCUISlot = m_stkMiscSlotPool.Pop();
				nKCUISlot.transform.SetParent(m_rectSlotPoolRect);
				NKCUtil.SetGameobjectActive(nKCUISlot, bValue: true);
				m_lstVisibleMiscSlot.Add(nKCUISlot);
				return nKCUISlot.GetComponent<RectTransform>();
			}
			NKCUISlot nKCUISlot2 = Object.Instantiate(m_pfbUISlot);
			nKCUISlot2.Init();
			NKCUtil.SetGameobjectActive(nKCUISlot2, bValue: true);
			m_lstVisibleMiscSlot.Add(nKCUISlot2);
			return nKCUISlot2.GetComponent<RectTransform>();
		}
		return null;
	}

	private void ReturnSlot(NKC_INVENTORY_TAB oldType)
	{
		switch (oldType)
		{
		case NKC_INVENTORY_TAB.NIT_EQUIP:
		{
			for (int j = 0; j < m_lstVisibleEquipSlot.Count; j++)
			{
				m_lstVisibleEquipSlot[j].transform.SetParent(m_rectSlotPoolRectEquip);
				NKCUtil.SetGameobjectActive(m_lstVisibleEquipSlot[j].gameObject, bValue: false);
				m_stkEquipSlotPool.Push(m_lstVisibleEquipSlot[j]);
			}
			m_lstVisibleEquipSlot.Clear();
			break;
		}
		case NKC_INVENTORY_TAB.NIT_MISC:
		case NKC_INVENTORY_TAB.NIT_MOLD:
		{
			for (int i = 0; i < m_lstVisibleMiscSlot.Count; i++)
			{
				m_lstVisibleMiscSlot[i].transform.SetParent(m_rectSlotPoolRect);
				NKCUtil.SetGameobjectActive(m_lstVisibleMiscSlot[i].gameObject, bValue: false);
				m_stkMiscSlotPool.Push(m_lstVisibleMiscSlot[i]);
			}
			m_lstVisibleMiscSlot.Clear();
			break;
		}
		}
	}

	private void ReturnSlot(Transform go)
	{
		NKCUtil.SetGameobjectActive(go.gameObject, bValue: false);
		if (m_NKC_INVENTORY_TAB == NKC_INVENTORY_TAB.NIT_EQUIP)
		{
			go.SetParent(m_rectSlotPoolRectEquip);
			NKCUISlotEquip component = go.GetComponent<NKCUISlotEquip>();
			if (component != null)
			{
				m_lstVisibleEquipSlot.Remove(component);
				m_stkEquipSlotPool.Push(component);
			}
		}
		else if (m_NKC_INVENTORY_TAB == NKC_INVENTORY_TAB.NIT_MISC || m_NKC_INVENTORY_TAB == NKC_INVENTORY_TAB.NIT_MOLD)
		{
			go.SetParent(m_rectSlotPoolRect);
			NKCUISlot component2 = go.GetComponent<NKCUISlot>();
			if (component2 != null)
			{
				m_lstVisibleMiscSlot.Remove(component2);
				m_stkMiscSlotPool.Push(component2);
			}
		}
	}

	private void ProvideSlotData(Transform tr, int idx)
	{
		if (m_NKC_INVENTORY_TAB == NKC_INVENTORY_TAB.NIT_MISC)
		{
			NKMItemMiscData nKMItemMiscData = null;
			nKMItemMiscData = ((m_eInventoryOpenType != NKC_INVENTORY_OPEN_TYPE.NIOT_ITEM_DEV) ? NKCScenManager.CurrentUserData().m_InventoryData.GetItemMisc(m_ssActiveMisc.SortedMiscList[idx].m_ItemMiscID) : MakeTempItemMiscData(m_ssActiveMisc.SortedMiscList[idx].m_ItemMiscID));
			if (nKMItemMiscData == null)
			{
				return;
			}
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeMiscItemData(nKMItemMiscData);
			NKCUISlot component = tr.GetComponent<NKCUISlot>();
			component.SetUsable(usable: false);
			if (m_eInventoryOpenType == NKC_INVENTORY_OPEN_TYPE.NIOT_NORMAL)
			{
				NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(nKMItemMiscData.ItemID);
				if (itemMiscTempletByID != null && itemMiscTempletByID.IsUsable())
				{
					component.SetData(data, bShowName: true, bShowNumber: true, bEnableLayoutElement: true, OpenItemUsePopup);
					component.SetUsable(usable: true);
				}
				else
				{
					component.SetData(data, bShowName: true, bShowNumber: true, bEnableLayoutElement: true, null);
					component.SetOpenItemBoxOnClick();
				}
			}
			else if (m_eInventoryOpenType == NKC_INVENTORY_OPEN_TYPE.NIOT_ITEM_DEV)
			{
				component.SetData(data, bShowName: true, bShowNumber: true, bEnableLayoutElement: true, m_currentOption.m_dOnClickItemSlot);
			}
			NKCUtil.SetGameobjectActive(component.gameObject, bValue: true);
		}
		else if (m_NKC_INVENTORY_TAB == NKC_INVENTORY_TAB.NIT_MOLD)
		{
			NKCUISlot.SlotData data2 = NKCUISlot.SlotData.MakeMoldItemData(m_lstMoldTemplet[idx].m_MoldID, 1L);
			NKCUISlot component2 = tr.GetComponent<NKCUISlot>();
			component2.SetUsable(usable: false);
			if (m_eInventoryOpenType == NKC_INVENTORY_OPEN_TYPE.NIOT_MOLD_DEV)
			{
				component2.SetData(data2, bShowName: true, bShowNumber: true, bEnableLayoutElement: true, m_currentOption.m_dOnClickItemMoldSlot);
			}
		}
		else
		{
			if (m_NKC_INVENTORY_TAB != NKC_INVENTORY_TAB.NIT_EQUIP)
			{
				return;
			}
			if (m_ssActive == null)
			{
				Debug.LogError("Slot Sort System Null!!");
				return;
			}
			bool bValue = true;
			_ = m_currentOption;
			NKCUISlotEquip component3 = tr.GetComponent<NKCUISlotEquip>();
			NKMEquipItemData nKMEquipItemData = new NKMEquipItemData();
			if (m_currentOption.m_dOnClickEmptySlot != null)
			{
				if (idx == 0)
				{
					NKCUtil.SetGameobjectActive(component3.gameObject, bValue);
					if (m_currentOption.lastSelectedItemUID > 0)
					{
						NKMEquipItemData itemEquip = NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(m_currentOption.lastSelectedItemUID);
						component3.SetEmpty(OnSelectedSlot, itemEquip);
						if (m_LatestSelectedSlot == null && m_LatestOpenNKMEquipItemData == null)
						{
							m_LatestOpenNKMEquipItemData = itemEquip;
							m_LatestSelectedSlot = component3;
							SetEquipInfo(component3);
						}
					}
					return;
				}
				idx--;
			}
			if (m_ssActive.SortedEquipList.Count <= idx)
			{
				return;
			}
			nKMEquipItemData = m_ssActive.SortedEquipList[idx];
			NKCUtil.SetGameobjectActive(component3.gameObject, bValue);
			NKCUIInvenEquipSlot.EQUIP_SLOT_STATE eQUIP_SLOT_STATE = NKCUIInvenEquipSlot.EQUIP_SLOT_STATE.ESS_NONE;
			if (m_eInventoryOpenType == NKC_INVENTORY_OPEN_TYPE.NIOT_EQUIP_SELECT)
			{
				bool bPresetContained = NKCEquipPresetDataManager.HSPresetEquipUId.Contains(nKMEquipItemData.m_ItemUid);
				component3.SetData(nKMEquipItemData, OnSelectedSlot, m_bLockMaxItem, m_currentOption.bSkipItemEquipBox, m_currentOption.bShowFierceUI, bPresetContained);
				component3.SetSelected(!m_currentOption.bMultipleSelect && m_LatestOpenNKMEquipItemData == null);
				if ((m_currentOption.m_hsSelectedEquipUIDToShow != null && m_currentOption.m_hsSelectedEquipUIDToShow.Contains(nKMEquipItemData.m_ItemUid)) || m_hsCurrentSelectedEquips.Contains(nKMEquipItemData.m_ItemUid))
				{
					eQUIP_SLOT_STATE = ((!m_currentOption.bShowRemoveItem) ? NKCUIInvenEquipSlot.EQUIP_SLOT_STATE.ESS_SELECTED : NKCUIInvenEquipSlot.EQUIP_SLOT_STATE.ESS_DELETE);
				}
				else if (!m_currentOption.bMultipleSelect && (m_LatestOpenNKMEquipItemData == null || m_LatestOpenNKMEquipItemData.m_ItemUid == nKMEquipItemData.m_ItemUid))
				{
					eQUIP_SLOT_STATE = NKCUIInvenEquipSlot.EQUIP_SLOT_STATE.ESS_SELECTED;
				}
			}
			else if (m_eInventoryOpenType == NKC_INVENTORY_OPEN_TYPE.NIOT_EQUIP_DEV)
			{
				component3.SetData(nKMEquipItemData, OnSelectedSlot);
				if (m_LatestOpenNKMEquipItemData == null || m_LatestOpenNKMEquipItemData.m_ItemUid == nKMEquipItemData.m_ItemUid)
				{
					eQUIP_SLOT_STATE = NKCUIInvenEquipSlot.EQUIP_SLOT_STATE.ESS_SELECTED;
				}
			}
			else
			{
				bool bPresetContained2 = NKCEquipPresetDataManager.HSPresetEquipUId.Contains(nKMEquipItemData.m_ItemUid);
				component3.SetData(nKMEquipItemData, OnSelectedSlot, lockMaxItem: false, bSkipEquipBox: false, bShowFierceInfo: false, bPresetContained2);
				if (m_LatestOpenNKMEquipItemData == null || m_LatestOpenNKMEquipItemData.m_ItemUid == nKMEquipItemData.m_ItemUid)
				{
					eQUIP_SLOT_STATE = NKCUIInvenEquipSlot.EQUIP_SLOT_STATE.ESS_SELECTED;
				}
			}
			component3.SetSlotState(eQUIP_SLOT_STATE);
			component3.SetLock(nKMEquipItemData.m_bLock, m_currentOption.bEnableLockEquipSystem);
			if (m_currentOption.bShowEquipUpgradeState)
			{
				component3.SetUpgradeSlotState(NKMItemManager.CanUpgradeEquipByCoreID(nKMEquipItemData));
			}
			if (eQUIP_SLOT_STATE == NKCUIInvenEquipSlot.EQUIP_SLOT_STATE.ESS_SELECTED)
			{
				m_LatestOpenNKMEquipItemData = nKMEquipItemData;
				m_LatestSelectedSlot = component3;
				component3.SetSlotState(NKCUIInvenEquipSlot.EQUIP_SLOT_STATE.ESS_SELECTED);
				SetEquipInfo(component3);
			}
		}
	}

	private void OnClickOkButton()
	{
		if (m_currentOption.m_ButtonMenuType == NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_CHANGE || m_currentOption.m_ButtonMenuType == NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_PRESET_CHANGE)
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(m_LatestOpenNKMEquipItemData.m_ItemEquipID);
			CheckEquipChange(equipTemplet.m_ItemEquipPosition);
		}
	}

	private void OnSelectedSlot(NKCUISlotEquip slot, NKMEquipItemData equipData)
	{
		if (m_currentOption.bShowRemoveSlot || m_currentOption.bEnableLockEquipSystem || !m_currentOption.bMultipleSelect)
		{
			m_LatestSelectedSlot?.SetSelected(bSelected: false);
		}
		if (m_currentOption.bMultipleSelect)
		{
			ToggleSelectedState(slot, equipData);
		}
		else
		{
			slot.SetSelected(bSelected: true);
		}
		m_LatestSelectedSlot = slot;
		m_LatestOpenNKMEquipItemData = slot.GetNKMEquipItemData();
		if (m_currentOption.bEnableLockEquipSystem)
		{
			NKCPacketSender.Send_NKMPacket_LOCK_ITEM_REQ(m_LatestOpenNKMEquipItemData.m_ItemUid, !m_LatestOpenNKMEquipItemData.m_bLock);
		}
		SetEquipInfo(slot);
	}

	public void SetLatestOpenNKMEquipItemDataAndOpenUnitSelect(NKMEquipItemData equipItemData)
	{
		m_LatestOpenNKMEquipItemData = equipItemData;
		OpenUnitSelect();
	}

	private void OpenUnitSelect()
	{
		NKCPopupItemEquipBox.CloseItemBox();
		NKCUIUnitSelectList.UnitSelectListOptions options = new NKCUIUnitSelectList.UnitSelectListOptions(NKM_UNIT_TYPE.NUT_NORMAL, _bMultipleSelect: true, NKM_DECK_TYPE.NDT_NORMAL);
		if (m_LatestOpenNKMEquipItemData != null)
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(m_LatestOpenNKMEquipItemData.m_ItemEquipID);
			if (equipTemplet != null)
			{
				options.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption> { NKCUnitSortSystem.GetFilterOption(equipTemplet.m_EquipUnitStyleType) };
				if (m_LatestOpenNKMEquipItemData.m_OwnerUnitUID > 0)
				{
					options.m_SortOptions.setExcludeUnitUID = new HashSet<long> { m_LatestOpenNKMEquipItemData.m_OwnerUnitUID };
				}
				if (equipTemplet.IsPrivateEquip())
				{
					options.m_SortOptions.setOnlyIncludeUnitBaseID = new HashSet<int>(equipTemplet.PrivateUnitList);
				}
			}
		}
		options.lstSortOption = new List<NKCUnitSortSystem.eSortOption> { NKCUnitSortSystem.eSortOption.Level_High };
		options.bDescending = false;
		options.bShowRemoveSlot = false;
		options.bMultipleSelect = false;
		options.iMaxMultipleSelect = 0;
		options.bExcludeLockedUnit = false;
		options.bExcludeDeckedUnit = false;
		options.bShowHideDeckedUnitMenu = true;
		options.bHideDeckedUnit = false;
		options.m_SortOptions.bIgnoreCityState = true;
		options.m_SortOptions.bIgnoreWorldMapLeader = true;
		options.setUnitFilterCategory = NKCUnitSortSystem.setDefaultUnitFilterCategory;
		options.setUnitSortCategory = NKCUnitSortSystem.setDefaultUnitSortCategory;
		options.m_bUseFavorite = true;
		options.strEmptyMessage = NKCUtilString.GET_STRING_INVEN_THERE_IS_NO_UNIT_TO_EQUIP;
		NKCUIUnitSelectList.Instance.Open(options, OnSelectedUnitToEquip);
	}

	private void OnSelectedUnitToEquip(List<long> lstUnitUID)
	{
		if (lstUnitUID.Count != 1)
		{
			Debug.LogError("Fatal Error : OnSelectedUnitToEquip returned wrong list");
			return;
		}
		long num = lstUnitUID[0];
		if (m_LatestOpenNKMEquipItemData == null)
		{
			return;
		}
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(m_LatestOpenNKMEquipItemData.m_ItemEquipID);
		if (equipTemplet == null)
		{
			return;
		}
		NKMUnitData unitFromUID = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitFromUID(num);
		if (unitFromUID != null)
		{
			NKM_ERROR_CODE nKM_ERROR_CODE = equipTemplet.CanEquipByUnit(NKCScenManager.GetScenManager().GetMyUserData(), unitFromUID);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				NKCPopupMessageManager.AddPopupMessage(nKM_ERROR_CODE);
				return;
			}
			m_LatestTargetUnitUIDToEquip = num;
			m_currentOption.equipChangeTargetPosition = equipTemplet.m_ItemEquipPosition;
			OpenChangeBoxOrChangeDirectIfEmpty();
		}
	}

	private void OnClickUnEquip()
	{
		NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(m_LatestOpenNKMEquipItemData.m_ItemUid);
		if (itemEquip == null || itemEquip.m_OwnerUnitUID <= 0)
		{
			return;
		}
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
		if (equipTemplet == null)
		{
			return;
		}
		NKMUnitData unitFromUID = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitFromUID(itemEquip.m_OwnerUnitUID);
		if (unitFromUID != null)
		{
			NKM_ERROR_CODE nKM_ERROR_CODE = equipTemplet.CanUnEquipByUnit(NKCScenManager.GetScenManager().GetMyUserData(), unitFromUID);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				NKCPopupMessageManager.AddPopupMessage(nKM_ERROR_CODE);
				return;
			}
			ITEM_EQUIP_POSITION itemEquipPosition = NKMItemManager.GetItemEquipPosition(itemEquip.m_ItemUid);
			NKCPacketSender.Send_NKMPacket_EQUIP_ITEM_EQUIP_REQ(bEquip: false, itemEquip.m_OwnerUnitUID, itemEquip.m_ItemUid, itemEquipPosition);
		}
	}

	public void OnClickEquipEnhance()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_ENCHANT))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.FACTORY_ENCHANT);
		}
		else
		{
			if (m_LatestOpenNKMEquipItemData == null)
			{
				return;
			}
			NKM_ERROR_CODE nKM_ERROR_CODE = NKMItemManager.CanEnchantItem(NKCScenManager.GetScenManager().GetMyUserData(), m_LatestOpenNKMEquipItemData);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString(nKM_ERROR_CODE.ToString()));
				return;
			}
			List<NKMEquipItemData> lstItemSortedList = new List<NKMEquipItemData>();
			if (m_ssActive != null)
			{
				lstItemSortedList = m_ssActive.SortedEquipList;
			}
			int filterSetOptionID = 0;
			if (m_currentOption.setFilterOption.Count > 0)
			{
				foreach (NKCEquipSortSystem.eFilterOption item in m_currentOption.setFilterOption)
				{
					if (item == NKCEquipSortSystem.eFilterOption.Equip_Stat_SetOption)
					{
						filterSetOptionID = m_currentOption.m_EquipListOptions.FilterSetOptionID;
						break;
					}
				}
			}
			NKCUIForge.Instance.Open(NKCUIForge.NKC_FORGE_TAB.NFT_ENCHANT, m_LatestOpenNKMEquipItemData.m_ItemUid, m_currentOption.setFilterOption, lstItemSortedList, filterSetOptionID);
		}
	}

	private void OpenItemUsePopup(NKCUISlot.SlotData slotData, bool bLocked)
	{
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(slotData.ID);
		if (itemMiscTempletByID == null)
		{
			return;
		}
		if (itemMiscTempletByID.IsChoiceItem())
		{
			NKCPopupItemBox.Instance.Open(NKCPopupItemBox.eMode.Choice, slotData);
		}
		else if (itemMiscTempletByID.IsContractItem)
		{
			NKCPopupMiscUseCount.Instance.Open(NKCPopupMiscUseCount.USE_ITEM_TYPE.Common, slotData.ID, slotData, delegate(int itemId, int count)
			{
				NKCPacketSender.Send_NKMPacket_MISC_CONTRACT_OPEN_REQ(itemId, count);
			});
		}
		else
		{
			NKCPopupMiscUseCount.Instance.Open(NKCPopupMiscUseCount.USE_ITEM_TYPE.Common, slotData.ID, slotData, delegate(int itemId, int count)
			{
				NKCPacketSender.Send_NKMPacket_RANDOM_ITEM_BOX_OPEN_REQ(itemId, count);
			});
		}
	}

	public void ToggleSelectedState(NKCUISlotEquip cItemSlot, NKMEquipItemData equipData)
	{
		if (cItemSlot != null && equipData != null)
		{
			if (cItemSlot.Get_EQUIP_SLOT_STATE() == NKCUIInvenEquipSlot.EQUIP_SLOT_STATE.ESS_NONE)
			{
				if (m_hsCurrentSelectedEquips != null && m_hsCurrentSelectedEquips.Count < m_currentOption.iMaxMultipleSelect)
				{
					if (m_currentOption.bShowRemoveItem)
					{
						cItemSlot.SetSlotState(NKCUIInvenEquipSlot.EQUIP_SLOT_STATE.ESS_DELETE);
					}
					else
					{
						cItemSlot.SetSlotState(NKCUIInvenEquipSlot.EQUIP_SLOT_STATE.ESS_SELECTED);
					}
					m_hsCurrentSelectedEquips.Add(equipData.m_ItemUid);
				}
			}
			else if (cItemSlot.Get_EQUIP_SLOT_STATE() == NKCUIInvenEquipSlot.EQUIP_SLOT_STATE.ESS_SELECTED || cItemSlot.Get_EQUIP_SLOT_STATE() == NKCUIInvenEquipSlot.EQUIP_SLOT_STATE.ESS_DELETE)
			{
				cItemSlot.SetSlotState(NKCUIInvenEquipSlot.EQUIP_SLOT_STATE.ESS_NONE);
				if (m_hsCurrentSelectedEquips != null)
				{
					m_hsCurrentSelectedEquips.Remove(equipData.m_ItemUid);
				}
			}
		}
		UpdateSelectedEquipCountUI();
		UpdateSelectedEquipBreakupResult();
	}

	public int CompareDescendingMisc(NKMItemMiscTemplet MiscX, NKMItemMiscTemplet MiscY)
	{
		if (MiscX == null)
		{
			return 1;
		}
		if (MiscY == null)
		{
			return -1;
		}
		if (MiscY.WillBeDeletedSoon() && !MiscX.WillBeDeletedSoon())
		{
			return 1;
		}
		if (!MiscY.WillBeDeletedSoon() && MiscX.WillBeDeletedSoon())
		{
			return -1;
		}
		if (!MiscX.IsUsable() && MiscY.IsUsable())
		{
			return 1;
		}
		if (MiscX.IsUsable() && !MiscY.IsUsable())
		{
			return -1;
		}
		return MiscX.m_ItemMiscID.CompareTo(MiscY.m_ItemMiscID);
	}

	public int CompareAscendingMisc(NKMItemMiscTemplet MiscX, NKMItemMiscTemplet MiscY)
	{
		if (MiscX == null)
		{
			return 1;
		}
		if (MiscY == null)
		{
			return -1;
		}
		if (!MiscY.WillBeDeletedSoon() && MiscX.WillBeDeletedSoon())
		{
			return 1;
		}
		if (MiscY.WillBeDeletedSoon() && !MiscX.WillBeDeletedSoon())
		{
			return -1;
		}
		if (!MiscX.IsUsable() && MiscY.IsUsable())
		{
			return 1;
		}
		if (MiscX.IsUsable() && !MiscY.IsUsable())
		{
			return -1;
		}
		return MiscY.m_ItemMiscID.CompareTo(MiscX.m_ItemMiscID);
	}

	public void Load()
	{
		IEnumerator<KeyValuePair<int, NKMItemMiscData>> enumerator = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.MiscItems.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMItemMiscData value = enumerator.Current.Value;
			if (value != null)
			{
				NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(value.ItemID);
				if (itemMiscTempletByID == null)
				{
					Log.Error($"NKMItemMiscTemplet 찾을 수 없음 - id {value.ItemID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUIInventory.cs", 1220);
				}
				else if (!itemMiscTempletByID.IsHideInInven())
				{
					NKCResourceUtility.PreloadMiscItemIcon(itemMiscTempletByID);
				}
			}
		}
	}

	public void Open(EquipSelectListOptions options, List<int> lstUpsideMenuResource = null, long latestTargetUnitUIDToEquip = 0L, NKC_INVENTORY_TAB openTab = NKC_INVENTORY_TAB.NIT_NONE)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_INVENTORY_EQUIPMENTS_UNLOCK, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKCUIInventoryRemovePopup, bValue: false);
		m_hsCurrentSelectedEquips.Clear();
		m_ssActive = null;
		m_ssActiveMisc = null;
		m_dChangeOptionNotify = null;
		if (m_currentOption.lEquipOptionCachingByUnitUID != 0L && m_currentOption.lEquipOptionCachingByUnitUID == options.lEquipOptionCachingByUnitUID)
		{
			options.lstSortOption = m_currentOption.lstSortOption;
			if (m_currentOption.setFilterOption.Count > 0)
			{
				foreach (NKCEquipSortSystem.eFilterOption item in m_currentOption.setFilterOption)
				{
					if ((uint)(item - 2) > 5u && (uint)(item - 34) > 3u)
					{
						options.setFilterOption.Add(item);
					}
				}
			}
		}
		m_currentOption = options;
		m_dOnClickEquipSlot = options.m_dOnSelectedEquipSlot;
		m_dOnGetItemListAfterSelected = options.m_dOnGetItemListAfterSelected;
		m_bLockMaxItem = options.bLockMaxItem;
		m_LatestTargetUnitUIDToEquip = latestTargetUnitUIDToEquip;
		m_NKM_UI_UNIT_INVENTORY.SetActive(value: true);
		m_lbEmptyMessage.text = options.strEmptyMessage;
		if (m_currentOption.bMultipleSelect)
		{
			SetOnClickEquipSlot(ToggleSelectedState);
		}
		else if (m_currentOption.iPresetIndex >= 0)
		{
			SetOnClickEquipSlot(OpenPresetChangeBoxOrChangeDirectIfEmpty);
		}
		else
		{
			SetOnClickEquipSlot(m_currentOption.m_dOnSelectedEquipSlot);
		}
		if (options.m_hsSelectedEquipUIDToShow != null)
		{
			foreach (long item2 in options.m_hsSelectedEquipUIDToShow)
			{
				m_hsCurrentSelectedEquips.Add(item2);
			}
		}
		lastSelectItemUID = options.lastSelectedItemUID;
		lastSelectEquipPos = options.equipChangeTargetPosition;
		NKCUtil.SetGameobjectActive(m_objFilterSelected, m_currentOption.setFilterOption.Count > 0);
		m_NKM_UI_INVENTORY_MENU_LOCK.Select(m_currentOption.bEnableLockEquipSystem);
		NKC_INVENTORY_TAB nKC_INVENTORY_TAB = NKC_INVENTORY_TAB.NIT_NONE;
		switch (m_currentOption.m_NKC_INVENTORY_OPEN_TYPE)
		{
		default:
			nKC_INVENTORY_TAB = NKC_INVENTORY_TAB.NIT_MISC;
			break;
		case NKC_INVENTORY_OPEN_TYPE.NIOT_EQUIP_SELECT:
		case NKC_INVENTORY_OPEN_TYPE.NIOT_EQUIP_DEV:
			nKC_INVENTORY_TAB = NKC_INVENTORY_TAB.NIT_EQUIP;
			break;
		case NKC_INVENTORY_OPEN_TYPE.NIOT_MOLD_DEV:
			nKC_INVENTORY_TAB = NKC_INVENTORY_TAB.NIT_MOLD;
			break;
		}
		if (openTab != NKC_INVENTORY_TAB.NIT_NONE)
		{
			nKC_INVENTORY_TAB = openTab;
		}
		OnSelectTab(nKC_INVENTORY_TAB, bForceOpen: true);
		if (lstUpsideMenuResource != null)
		{
			RESOURCE_LIST = lstUpsideMenuResource;
		}
		else
		{
			RESOURCE_LIST = base.UpsideMenuShowResourceList;
		}
		NKCUIManager.UpdateUpsideMenu();
		UIOpened();
	}

	public void SetOptionChangeNotifyFunc(dChangeOptionNotify ChangeOptionNotify)
	{
		m_dChangeOptionNotify = ChangeOptionNotify;
	}

	private void OnSelectTab(NKC_INVENTORY_TAB newTab, bool bForceOpen = false)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_INVENTORY_EQUIPMENTS_UNLOCK, bValue: false);
		if (m_NKC_INVENTORY_TAB != newTab || bForceOpen)
		{
			ReturnSlot(m_NKC_INVENTORY_TAB);
			m_NKC_INVENTORY_TAB = newTab;
			if (m_currentOption.lstSortOption.Count == 0)
			{
				m_currentOption.lstSortOption = NKCEquipSortSystem.GetDefaultSortOption();
			}
			CalculateContentRectSize();
			if (newTab == NKC_INVENTORY_TAB.NIT_EQUIP)
			{
				m_LoopScrollRectEquip.PrepareCells();
			}
			else
			{
				m_LoopScrollRect.PrepareCells();
			}
			ChangeUI();
			NKCUtil.SetGameobjectActive(m_objMisc, m_NKC_INVENTORY_TAB != NKC_INVENTORY_TAB.NIT_EQUIP);
			NKCUtil.SetGameobjectActive(m_objGear, m_NKC_INVENTORY_TAB == NKC_INVENTORY_TAB.NIT_EQUIP);
			m_eInventoryOpenType = m_currentOption.m_NKC_INVENTORY_OPEN_TYPE;
			ProcessByType(m_eInventoryOpenType, bForceRebuildList: true);
			if (m_NKC_INVENTORY_TAB == NKC_INVENTORY_TAB.NIT_EQUIP)
			{
				UpdateSelectedEquipCountUI();
				UpdateSelectedEquipBreakupResult();
			}
		}
	}

	private void ProcessByType(NKC_INVENTORY_OPEN_TYPE targetType, bool bForceRebuildList = false)
	{
		if (m_currentOption.m_NKC_INVENTORY_OPEN_TYPE != targetType)
		{
			m_currentOption.m_NKC_INVENTORY_OPEN_TYPE = targetType;
			CalculateContentRectSize();
			if (targetType == NKC_INVENTORY_OPEN_TYPE.NIOT_EQUIP_DEV || targetType == NKC_INVENTORY_OPEN_TYPE.NIOT_EQUIP_SELECT)
			{
				m_LoopScrollRectEquip.PrepareCells();
			}
			else
			{
				m_LoopScrollRect.PrepareCells();
			}
		}
		if (m_NKC_INVENTORY_TAB == NKC_INVENTORY_TAB.NIT_EQUIP)
		{
			if (bForceRebuildList)
			{
				m_dicEquipSortSystem.Remove(targetType);
			}
			m_ssActive = GetEquipSortSystem(targetType);
			m_ssActiveMisc = null;
		}
		else if (m_NKC_INVENTORY_TAB == NKC_INVENTORY_TAB.NIT_MISC || m_NKC_INVENTORY_TAB == NKC_INVENTORY_TAB.NIT_MOLD)
		{
			m_ssActive = null;
			m_ssActiveMisc = null;
		}
		ResetEquipSortOption();
		UpdateItemList(m_NKC_INVENTORY_TAB);
	}

	private void ResetEquipSortOption()
	{
		if (m_ssActive != null)
		{
			m_SortUI.RegisterCategories(m_setEquipFilterCategory, m_setEquipSortCategory);
			m_SortUI.RegisterSortOptionUpdate(SortOptionChange);
			m_SortUI.RegisterEquipSort(m_ssActive);
			m_SortUI.ResetUI();
		}
	}

	private NKCMiscSortSystem.MiscListOptions MakeSortOption()
	{
		NKCMiscSortSystem.MiscListOptions result = default(NKCMiscSortSystem.MiscListOptions);
		result.lstSortOption = NKCMiscSortSystem.GetDefaultSortList();
		result.lstSortOption.Insert(0, NKCMiscSortSystem.eSortOption.CustomDescend1);
		result.setFilterOption = new HashSet<NKCMiscSortSystem.eFilterOption>();
		string key = "MISC SORT";
		result.lstCustomSortFunc = new Dictionary<NKCMiscSortSystem.eSortCategory, KeyValuePair<string, NKCMiscSortSystem.NKCDataComparerer<NKMItemMiscTemplet>.CompareFunc>>();
		result.lstCustomSortFunc.Add(NKCMiscSortSystem.eSortCategory.Custom1, new KeyValuePair<string, NKCMiscSortSystem.NKCDataComparerer<NKMItemMiscTemplet>.CompareFunc>(key, CompareDescendingMisc));
		result.bHideTokenFiltering = false;
		result.bHideDescendingOption = true;
		result.bHideFilterOption = true;
		result.bHideSortOption = true;
		return result;
	}

	public void SortOptionChange(bool bUpdate)
	{
		m_currentOption.lstSortOption = m_ssActive.lstSortOption;
		m_currentOption.setFilterOption = m_ssActive.FilterSet;
		m_currentOption.m_EquipListOptions.FilterStatType_01 = m_ssActive.FilterStatType_01;
		m_currentOption.m_EquipListOptions.FilterStatType_02 = m_ssActive.FilterStatType_02;
		m_currentOption.m_EquipListOptions.FilterStatType_Potential = m_ssActive.FilterStatType_Potential;
		m_currentOption.m_EquipListOptions.FilterSetOptionID = m_ssActive.FilterStatType_SetOptionID;
	}

	private void SetEquipInfo(NKCUISlotEquip slot)
	{
		if (slot == null || slot.GetNKMEquipItemData() == null)
		{
			NKCUtil.SetGameobjectActive(m_objEquipNotSelected, bValue: true);
			NKCUtil.SetGameobjectActive(m_slotEquipInfo, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objEquipNotSelected, bValue: false);
		NKCUtil.SetGameobjectActive(m_slotEquipInfo, bValue: true);
		NKMEquipItemData equipItemData = slot.GetNKMEquipItemData();
		m_slotEquipInfo.SetData(equipItemData, m_currentOption.bShowFierceUI);
		if (m_currentOption.bEnableLockEquipSystem)
		{
			NKCUtil.SetGameobjectActive(m_ChangeButton, bValue: false);
			NKCUtil.SetGameobjectActive(m_EquipButton, bValue: false);
			NKCUtil.SetGameobjectActive(m_OkButton, bValue: false);
			NKCUtil.SetGameobjectActive(m_ReinforceButton, bValue: false);
			NKCUtil.SetGameobjectActive(m_ReinforceButtonLock, bValue: false);
			NKCUtil.SetGameobjectActive(m_UnEquipButton, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_UnEquipButton, equipItemData != null && equipItemData.m_OwnerUnitUID > 0);
		NKCUtil.SetGameobjectActive(m_EquipButton, equipItemData == null || equipItemData.m_OwnerUnitUID <= 0);
		if (m_dOnClickEquipSlot != null)
		{
			m_OkButton.PointerClick.RemoveAllListeners();
			m_OkButton.PointerClick.AddListener(delegate
			{
				OnClickEquipSlot(slot, equipItemData);
			});
		}
		SetInventoryButtons(slot);
	}

	private void SetInventoryButtons(NKCUISlotEquip slot)
	{
		NKMEquipItemData cNKMEquipItemData = m_LatestOpenNKMEquipItemData;
		if (cNKMEquipItemData == null)
		{
			return;
		}
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(cNKMEquipItemData.m_ItemEquipID);
		if (equipTemplet == null)
		{
			return;
		}
		if (m_currentOption.m_ButtonMenuType != NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_PRESET_CHANGE && m_LatestTargetUnitUIDToEquip > 0)
		{
			NKMUnitData unitFromUID = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitFromUID(m_LatestTargetUnitUIDToEquip);
			if (m_currentOption.equipChangeTargetPosition != equipTemplet.m_ItemEquipPosition)
			{
				if (equipTemplet.m_ItemEquipPosition == ITEM_EQUIP_POSITION.IEP_ACC || equipTemplet.m_ItemEquipPosition == ITEM_EQUIP_POSITION.IEP_ACC2)
				{
					if (unitFromUID.GetEquipItemAccessoryUid() == 0L)
					{
						m_currentOption.equipChangeTargetPosition = ITEM_EQUIP_POSITION.IEP_ACC;
						m_currentOption.lastSelectedItemUID = 0L;
					}
					else if (unitFromUID.GetEquipItemAccessory2Uid() == 0L)
					{
						m_currentOption.equipChangeTargetPosition = ITEM_EQUIP_POSITION.IEP_ACC2;
						m_currentOption.lastSelectedItemUID = 0L;
					}
					else if (lastSelectEquipPos == ITEM_EQUIP_POSITION.IEP_ACC || lastSelectEquipPos == ITEM_EQUIP_POSITION.IEP_ACC2)
					{
						m_currentOption.equipChangeTargetPosition = lastSelectEquipPos;
						m_currentOption.lastSelectedItemUID = unitFromUID.GetEquipUid(lastSelectEquipPos);
					}
					else
					{
						m_currentOption.equipChangeTargetPosition = ITEM_EQUIP_POSITION.IEP_ACC;
						m_currentOption.lastSelectedItemUID = unitFromUID.GetEquipUid(ITEM_EQUIP_POSITION.IEP_ACC);
					}
				}
				else
				{
					m_currentOption.equipChangeTargetPosition = equipTemplet.m_ItemEquipPosition;
					m_currentOption.lastSelectedItemUID = unitFromUID.GetEquipUid(equipTemplet.m_ItemEquipPosition);
				}
			}
		}
		if (m_currentOption.m_dOnClickEmptySlot != null && m_currentOption.lastSelectedItemUID > 0 && cNKMEquipItemData.m_ItemUid == m_currentOption.lastSelectedItemUID)
		{
			NKCUtil.SetGameobjectActive(m_ChangeButton, bValue: false);
			NKCUtil.SetGameobjectActive(m_EquipButton, bValue: false);
			NKCUtil.SetGameobjectActive(m_OkButton, bValue: false);
			NKCUtil.SetGameobjectActive(m_ReinforceButton, bValue: false);
			NKCUtil.SetGameobjectActive(m_ReinforceButtonLock, bValue: false);
			NKCUtil.SetGameobjectActive(m_UnEquipButton, bValue: true);
			m_UnEquipButton.PointerClick.RemoveAllListeners();
			m_UnEquipButton.PointerClick.AddListener(delegate
			{
				m_currentOption.m_dOnClickEmptySlot(slot, cNKMEquipItemData);
			});
			return;
		}
		switch (m_currentOption.m_ButtonMenuType)
		{
		case NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE:
			NKCUtil.SetGameobjectActive(m_UnEquipButton, bValue: false);
			NKCUtil.SetGameobjectActive(m_ReinforceButton, bValue: false);
			NKCUtil.SetGameobjectActive(m_ReinforceButtonLock, bValue: false);
			NKCUtil.SetGameobjectActive(m_EquipButton, bValue: false);
			NKCUtil.SetGameobjectActive(m_ChangeButton, bValue: false);
			NKCUtil.SetGameobjectActive(m_OkButton, bValue: false);
			break;
		case NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_OK:
			NKCUtil.SetGameobjectActive(m_UnEquipButton, bValue: false);
			if (m_currentOption.bShowEquipUpgradeState)
			{
				NKCUtil.SetGameobjectActive(m_ReinforceButton, NKMItemManager.CanUpgradeEquipByCoreID(cNKMEquipItemData) != NKC_EQUIP_UPGRADE_STATE.UPGRADABLE);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_ReinforceButton, bValue: false);
			}
			NKCUtil.SetGameobjectActive(m_ReinforceButtonLock, bValue: false);
			NKCUtil.SetGameobjectActive(m_EquipButton, bValue: false);
			NKCUtil.SetGameobjectActive(m_ChangeButton, bValue: false);
			if (m_currentOption.bShowEquipUpgradeState)
			{
				NKCUtil.SetGameobjectActive(m_OkButton, NKMItemManager.CanUpgradeEquipByCoreID(cNKMEquipItemData) == NKC_EQUIP_UPGRADE_STATE.UPGRADABLE);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_OkButton, bValue: true);
			}
			break;
		case NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_PRESET_CHANGE:
			NKCUtil.SetGameobjectActive(m_UnEquipButton, m_currentOption.lastSelectedItemUID > 0 && slot.GetNKMEquipItemData().m_ItemUid == m_currentOption.lastSelectedItemUID);
			NKCUtil.SetGameobjectActive(m_ReinforceButton, NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_ENCHANT) && equipTemplet.m_ItemEquipPosition != ITEM_EQUIP_POSITION.IEP_ENCHANT);
			NKCUtil.SetGameobjectActive(m_ReinforceButtonLock, !NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_ENCHANT) && equipTemplet.m_ItemEquipPosition != ITEM_EQUIP_POSITION.IEP_ENCHANT);
			NKCUtil.SetGameobjectActive(m_EquipButton, bValue: false);
			NKCUtil.SetGameobjectActive(m_ChangeButton, bValue: false);
			NKCUtil.SetGameobjectActive(m_OkButton, slot.GetNKMEquipItemData().m_ItemUid != m_currentOption.lastSelectedItemUID);
			break;
		case NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_CHANGE:
			NKCUtil.SetGameobjectActive(m_UnEquipButton, m_LatestOpenNKMEquipItemData.m_OwnerUnitUID == m_LatestTargetUnitUIDToEquip);
			NKCUtil.SetGameobjectActive(m_ReinforceButton, bValue: false);
			NKCUtil.SetGameobjectActive(m_ReinforceButtonLock, bValue: false);
			NKCUtil.SetGameobjectActive(m_EquipButton, bValue: false);
			NKCUtil.SetGameobjectActive(m_ChangeButton, bValue: false);
			NKCUtil.SetGameobjectActive(m_OkButton, m_LatestOpenNKMEquipItemData.m_OwnerUnitUID != m_LatestTargetUnitUIDToEquip);
			break;
		default:
			NKCUtil.SetGameobjectActive(m_UnEquipButton, cNKMEquipItemData.m_OwnerUnitUID > 0);
			NKCUtil.SetGameobjectActive(m_ReinforceButton, NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_ENCHANT) && equipTemplet.m_ItemEquipPosition != ITEM_EQUIP_POSITION.IEP_ENCHANT);
			NKCUtil.SetGameobjectActive(m_ReinforceButtonLock, !NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_ENCHANT) && equipTemplet.m_ItemEquipPosition != ITEM_EQUIP_POSITION.IEP_ENCHANT);
			NKCUtil.SetGameobjectActive(m_EquipButton, cNKMEquipItemData.m_OwnerUnitUID <= 0 && equipTemplet.m_ItemEquipPosition != ITEM_EQUIP_POSITION.IEP_ENCHANT);
			NKCUtil.SetGameobjectActive(m_ChangeButton, cNKMEquipItemData.m_OwnerUnitUID > 0);
			NKCUtil.SetGameobjectActive(m_OkButton, equipTemplet.m_ItemEquipPosition == ITEM_EQUIP_POSITION.IEP_ENCHANT);
			break;
		}
	}

	public void SetOnClickEquipSlot(NKCUISlotEquip.OnSelectedEquipSlot dOnClickEquipSlot = null)
	{
		if (dOnClickEquipSlot != null)
		{
			m_dOnClickEquipSlot = dOnClickEquipSlot;
			return;
		}
		m_dOnClickEquipSlot = delegate
		{
			OpenChangeBoxOrChangeDirectIfEmpty();
		};
	}

	private void OpenPresetChangeBoxOrChangeDirectIfEmpty(NKCUISlotEquip cItemSlot, NKMEquipItemData equipData)
	{
		if (!(cItemSlot == null) && cItemSlot.GetNKMEquipItemData() != null)
		{
			m_LatestSelectedSlot = cItemSlot;
			m_LatestOpenNKMEquipItemData = equipData;
			if (m_currentOption.lastSelectedItemUID > 0 && m_currentOption.m_ButtonMenuType != NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_PRESET_CHANGE)
			{
				NKCPopupItemEquipBox.OpenForConfirm(m_LatestOpenNKMEquipItemData, OpenPresetChangeBoxOrChangeDirectIfEmpty, m_currentOption.bShowFierceUI);
			}
			else
			{
				OpenPresetChangeBoxOrChangeDirectIfEmpty();
			}
		}
	}

	private void OpenChangeBoxOrChangeDirectIfEmpty()
	{
		if (m_LatestOpenNKMEquipItemData == null)
		{
			return;
		}
		NKMEquipTemplet cNKMEquipTemplet = NKMItemManager.GetEquipTemplet(m_LatestOpenNKMEquipItemData.m_ItemEquipID);
		if (cNKMEquipTemplet == null)
		{
			return;
		}
		NKMUnitData unitFromUID = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitFromUID(m_LatestTargetUnitUIDToEquip);
		if (unitFromUID == null)
		{
			return;
		}
		if (cNKMEquipTemplet.IsPrivateEquip() && !cNKMEquipTemplet.IsPrivateEquipForUnit(unitFromUID.m_UnitID))
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_CANNOT_EQUIP_ITEM_PRIVATE);
			return;
		}
		if (m_currentOption.equipChangeTargetPosition == ITEM_EQUIP_POSITION.IEP_ACC || m_currentOption.equipChangeTargetPosition == ITEM_EQUIP_POSITION.IEP_ACC2)
		{
			if (unitFromUID.GetEquipItemAccessoryUid() > 0 && unitFromUID.GetEquipItemAccessory2Uid() > 0)
			{
				if (lastSelectItemUID != 0L && (lastSelectItemUID == unitFromUID.GetEquipItemAccessoryUid() || lastSelectItemUID == unitFromUID.GetEquipItemAccessory2Uid()))
				{
					if (lastSelectItemUID == unitFromUID.GetEquipItemAccessoryUid())
					{
						ChangeEquipAccessory(ITEM_EQUIP_POSITION.IEP_ACC);
					}
					else if (lastSelectItemUID == unitFromUID.GetEquipItemAccessory2Uid())
					{
						ChangeEquipAccessory(ITEM_EQUIP_POSITION.IEP_ACC2);
					}
				}
				else
				{
					NKCPopupItemEquipBox.OpenForSelectItem(unitFromUID.GetEquipItemAccessoryUid(), unitFromUID.GetEquipItemAccessory2Uid(), delegate
					{
						ChangeEquipAccessory(ITEM_EQUIP_POSITION.IEP_ACC);
					}, delegate
					{
						ChangeEquipAccessory(ITEM_EQUIP_POSITION.IEP_ACC2);
					});
				}
			}
			else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_INVENTORY && unitFromUID.IsUnlockAccessory2() && (unitFromUID.GetEquipItemAccessoryUid() > 0 || unitFromUID.GetEquipItemAccessory2Uid() > 0))
			{
				if (unitFromUID.GetEquipItemAccessoryUid() > 0)
				{
					CheckEquipChange(ITEM_EQUIP_POSITION.IEP_ACC2);
				}
				else
				{
					CheckEquipChange(ITEM_EQUIP_POSITION.IEP_ACC);
				}
			}
			else
			{
				CheckEquipChange(m_currentOption.equipChangeTargetPosition);
			}
			return;
		}
		long equipUid = unitFromUID.GetEquipUid(cNKMEquipTemplet.m_ItemEquipPosition);
		if (equipUid > 0)
		{
			NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(equipUid);
			NKCPopupEquipChange?.Open(itemEquip, m_LatestOpenNKMEquipItemData, delegate
			{
				SendEquipPacket(cNKMEquipTemplet.m_ItemEquipPosition);
			}, m_currentOption.bShowFierceUI);
		}
		else
		{
			SendEquipPacket(cNKMEquipTemplet.m_ItemEquipPosition);
		}
	}

	private void OpenPresetChangeBoxOrChangeDirectIfEmpty()
	{
		if (m_LatestOpenNKMEquipItemData == null)
		{
			return;
		}
		long equipUId = m_LatestOpenNKMEquipItemData.m_ItemUid;
		ITEM_EQUIP_POSITION presetPosition = m_currentOption.equipChangeTargetPosition;
		if (m_currentOption.equipChangeTargetPosition == ITEM_EQUIP_POSITION.IEP_ACC || m_currentOption.equipChangeTargetPosition == ITEM_EQUIP_POSITION.IEP_ACC2)
		{
			presetPosition = ITEM_EQUIP_POSITION.IEP_ACC;
		}
		List<long> lstEquipResult = new List<long>();
		if (NKCEquipPresetDataManager.ListEquipPresetData != null && NKCEquipPresetDataManager.ListEquipPresetData.Count > m_currentOption.iPresetIndex)
		{
			lstEquipResult.AddRange(NKCEquipPresetDataManager.ListEquipPresetData[m_currentOption.iPresetIndex].equipUids);
			int equipChangeTargetPosition = (int)m_currentOption.equipChangeTargetPosition;
			if (lstEquipResult.Count > equipChangeTargetPosition)
			{
				lstEquipResult[equipChangeTargetPosition] = equipUId;
			}
		}
		if (lastSelectItemUID > 0)
		{
			NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(lastSelectItemUID);
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
			NKMEquipTemplet equipTemplet2 = NKMItemManager.GetEquipTemplet(m_LatestOpenNKMEquipItemData.m_ItemEquipID);
			if (equipTemplet.m_EquipUnitStyleType != equipTemplet2.m_EquipUnitStyleType)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_EQUIP_PRESET_DIFFERENT_TYPE);
				return;
			}
			if (equipTemplet.m_ItemEquipPosition != equipTemplet2.m_ItemEquipPosition)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_EQUIP_PRESET_DIFFERENT_POSITION);
				return;
			}
			if (NKCUtil.IsPrivateEquipAlreadyEquipped(lstEquipResult))
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_EQUIP_PRIVATE.ToString()));
				return;
			}
			NKCPopupEquipChange?.Open(itemEquip, m_LatestOpenNKMEquipItemData, delegate
			{
				NKCPacketSender.Send_NKMPacket_EQUIP_PRESET_REGISTER_REQ(m_currentOption.iPresetIndex, m_currentOption.equipChangeTargetPosition, equipUId);
			}, m_currentOption.bShowFierceUI);
			return;
		}
		NKCPopupItemEquipBox.OpenForConfirm(m_LatestOpenNKMEquipItemData, delegate
		{
			NKMEquipTemplet equipTemplet3 = NKMItemManager.GetEquipTemplet(m_LatestOpenNKMEquipItemData.m_ItemEquipID);
			if (m_currentOption.presetUnitStyeType != NKM_UNIT_STYLE_TYPE.NUST_INVALID && m_currentOption.presetUnitStyeType != equipTemplet3.m_EquipUnitStyleType)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_EQUIP_PRESET_DIFFERENT_TYPE);
			}
			else if (presetPosition != equipTemplet3.m_ItemEquipPosition)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_EQUIP_PRESET_DIFFERENT_POSITION);
			}
			else if (NKCUtil.IsPrivateEquipAlreadyEquipped(lstEquipResult))
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_EQUIP_PRIVATE.ToString()));
			}
			else
			{
				NKCPacketSender.Send_NKMPacket_EQUIP_PRESET_REGISTER_REQ(m_currentOption.iPresetIndex, m_currentOption.equipChangeTargetPosition, equipUId);
			}
		}, m_currentOption.bShowFierceUI);
	}

	private void CheckEquipChange(ITEM_EQUIP_POSITION targetPosition)
	{
		List<long> list = new List<long>();
		NKMUnitData unitFromUID = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitFromUID(m_LatestTargetUnitUIDToEquip);
		if (unitFromUID != null)
		{
			list.AddRange(unitFromUID.EquipItemUids);
			int equipChangeTargetPosition = (int)m_currentOption.equipChangeTargetPosition;
			if (list.Count > equipChangeTargetPosition)
			{
				list[equipChangeTargetPosition] = m_LatestOpenNKMEquipItemData.m_ItemUid;
			}
		}
		if (m_currentOption.lastSelectedItemUID > 0)
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData == null)
			{
				return;
			}
			NKMEquipItemData itemEquip = nKMUserData.m_InventoryData.GetItemEquip(m_currentOption.lastSelectedItemUID);
			if (itemEquip != null && itemEquip.m_OwnerUnitUID > 0)
			{
				NKCPopupEquipChange?.Open(itemEquip, m_LatestOpenNKMEquipItemData, delegate
				{
					SendEquipPacket(targetPosition);
				});
			}
			else
			{
				NKCPopupItemEquipBox.OpenForConfirm(m_LatestOpenNKMEquipItemData, delegate
				{
					SendEquipPacket(targetPosition);
				});
			}
		}
		else if (NKCUtil.IsPrivateEquipAlreadyEquipped(list))
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_EQUIP_PRIVATE.ToString()));
		}
		else
		{
			SendEquipPacket(targetPosition);
		}
	}

	private void SendEquipPacket(ITEM_EQUIP_POSITION equip_position)
	{
		NKMEquipItemData latestOpenNKMEquipItemData = m_LatestOpenNKMEquipItemData;
		if (latestOpenNKMEquipItemData == null)
		{
			return;
		}
		if (latestOpenNKMEquipItemData.m_OwnerUnitUID <= 0)
		{
			long equipUid = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(m_LatestTargetUnitUIDToEquip).GetEquipUid(equip_position);
			if (equipUid != 0L)
			{
				NKCPacketSender.Send_NKMPacket_EQUIP_ITEM_EQUIP_REQ(bEquip: false, m_LatestTargetUnitUIDToEquip, equipUid, equip_position);
			}
			NKCPacketSender.Send_NKMPacket_EQUIP_ITEM_EQUIP_REQ(bEquip: true, m_LatestTargetUnitUIDToEquip, latestOpenNKMEquipItemData.m_ItemUid, equip_position);
		}
		else
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_INVEN_EQUIP_CHANGE_WARNING, ConfirmChangeEquip);
		}
	}

	public void ConfirmChangeEquip()
	{
		NKMEquipItemData latestOpenNKMEquipItemData = m_LatestOpenNKMEquipItemData;
		if (latestOpenNKMEquipItemData == null)
		{
			return;
		}
		NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(m_LatestTargetUnitUIDToEquip);
		if (unitFromUID != null)
		{
			long equipUid = unitFromUID.GetEquipUid(m_currentOption.equipChangeTargetPosition);
			if (equipUid != 0L)
			{
				NKCPacketSender.Send_NKMPacket_EQUIP_ITEM_EQUIP_REQ(bEquip: false, m_LatestTargetUnitUIDToEquip, equipUid, m_currentOption.equipChangeTargetPosition);
			}
		}
		ITEM_EQUIP_POSITION itemEquipPosition = NKMItemManager.GetItemEquipPosition(latestOpenNKMEquipItemData.m_ItemUid);
		NKCPacketSender.Send_NKMPacket_EQUIP_ITEM_EQUIP_REQ(bEquip: false, latestOpenNKMEquipItemData.m_OwnerUnitUID, latestOpenNKMEquipItemData.m_ItemUid, itemEquipPosition);
		if (m_currentOption.equipChangeTargetPosition != ITEM_EQUIP_POSITION.IEP_NONE)
		{
			NKCPacketSender.Send_NKMPacket_EQUIP_ITEM_EQUIP_REQ(bEquip: true, m_LatestTargetUnitUIDToEquip, latestOpenNKMEquipItemData.m_ItemUid, m_currentOption.equipChangeTargetPosition);
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_EQUIP_ITEM_EQUIP_REQ(bEquip: true, m_LatestTargetUnitUIDToEquip, latestOpenNKMEquipItemData.m_ItemUid, itemEquipPosition);
		}
	}

	private void ChangeEquipAccessory(ITEM_EQUIP_POSITION equipPosition)
	{
		if (equipPosition != ITEM_EQUIP_POSITION.IEP_ACC && equipPosition != ITEM_EQUIP_POSITION.IEP_ACC2)
		{
			return;
		}
		NKMUnitData unitFromUID = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitFromUID(m_LatestTargetUnitUIDToEquip);
		if (unitFromUID == null)
		{
			return;
		}
		List<long> list = new List<long>();
		list.AddRange(unitFromUID.EquipItemUids);
		int num = (int)equipPosition;
		if (list.Count > num)
		{
			list[num] = m_LatestOpenNKMEquipItemData.m_ItemUid;
		}
		if (NKCUtil.IsPrivateEquipAlreadyEquipped(list))
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_EQUIP_PRIVATE.ToString()));
			return;
		}
		long itemUid = ((equipPosition == ITEM_EQUIP_POSITION.IEP_ACC) ? unitFromUID.GetEquipItemAccessoryUid() : unitFromUID.GetEquipItemAccessory2Uid());
		NKMEquipItemData beforeItem = NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(itemUid);
		if (beforeItem != null)
		{
			NKCPopupEquipChange?.Open(beforeItem, m_LatestOpenNKMEquipItemData, delegate
			{
				ChangeEquipItem(beforeItem, m_LatestOpenNKMEquipItemData, equipPosition);
			});
		}
		else
		{
			SendEquipPacket(equipPosition);
		}
	}

	private void ChangeEquipItem(NKMEquipItemData beforeItem, NKMEquipItemData afterItem, ITEM_EQUIP_POSITION equipPosition)
	{
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(beforeItem.m_ItemEquipID);
		NKMEquipTemplet equipTemplet2 = NKMItemManager.GetEquipTemplet(afterItem.m_ItemEquipID);
		if (equipTemplet != null && equipTemplet2 != null)
		{
			Debug.Log("변경 요청한 아이템 명칭 : " + equipTemplet.GetItemName() + " -> " + equipTemplet2.GetItemName());
		}
		m_beforeItemUID = 0L;
		m_afterItemUId = 0L;
		m_equipPosition = ITEM_EQUIP_POSITION.IEP_NONE;
		if (afterItem.m_OwnerUnitUID > 0)
		{
			m_beforeItemUID = beforeItem.m_ItemUid;
			m_afterItemUId = afterItem.m_ItemUid;
			m_equipPosition = equipPosition;
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_INVEN_EQUIP_CHANGE_WARNING, OnChangeEquip);
		}
		else if (beforeItem.m_OwnerUnitUID > 0)
		{
			NKCPacketSender.Send_NKMPacket_EQUIP_ITEM_EQUIP_REQ(bEquip: false, beforeItem.m_OwnerUnitUID, beforeItem.m_ItemUid, equipPosition);
			NKCPacketSender.Send_NKMPacket_EQUIP_ITEM_EQUIP_REQ(bEquip: true, beforeItem.m_OwnerUnitUID, afterItem.m_ItemUid, equipPosition);
		}
	}

	private void OnChangeEquip()
	{
		if (m_beforeItemUID != 0L && m_afterItemUId != 0L && m_equipPosition != ITEM_EQUIP_POSITION.IEP_NONE)
		{
			NKMEquipItemData itemEquip = NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(m_beforeItemUID);
			NKMEquipItemData itemEquip2 = NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(m_afterItemUId);
			if (itemEquip2.m_OwnerUnitUID > 0)
			{
				ITEM_EQUIP_POSITION itemEquipPosition = NKMItemManager.GetItemEquipPosition(itemEquip2.m_ItemUid);
				NKCPacketSender.Send_NKMPacket_EQUIP_ITEM_EQUIP_REQ(bEquip: false, itemEquip2.m_OwnerUnitUID, itemEquip2.m_ItemUid, itemEquipPosition);
			}
			if (itemEquip.m_OwnerUnitUID > 0)
			{
				NKCPacketSender.Send_NKMPacket_EQUIP_ITEM_EQUIP_REQ(bEquip: false, itemEquip.m_OwnerUnitUID, itemEquip.m_ItemUid, m_equipPosition);
				NKCPacketSender.Send_NKMPacket_EQUIP_ITEM_EQUIP_REQ(bEquip: true, itemEquip.m_OwnerUnitUID, itemEquip2.m_ItemUid, m_equipPosition);
			}
		}
	}

	public void FinishMultiSelection()
	{
		if (m_currentOption.m_dOnFinishMultiSelection != null)
		{
			List<long> list = null;
			list = new List<long>(m_hsCurrentSelectedEquips);
			m_currentOption.m_dOnFinishMultiSelection(list);
		}
	}

	public override void OnBackButton()
	{
		if (m_currentOption.bEnableLockEquipSystem)
		{
			OnLockModeToggle(bValue: false);
		}
		else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_OFFICE)
		{
			Close();
		}
		else if (m_currentOption.m_NKC_INVENTORY_OPEN_TYPE == NKC_INVENTORY_OPEN_TYPE.NIOT_NORMAL || m_currentOption.bShowRemoveItem)
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME, bForce: false);
		}
		else if (m_currentOption.m_NKC_INVENTORY_OPEN_TYPE == NKC_INVENTORY_OPEN_TYPE.NIOT_EQUIP_SELECT)
		{
			Close();
		}
		else if (m_currentOption.m_NKC_INVENTORY_OPEN_TYPE == NKC_INVENTORY_OPEN_TYPE.NIOT_EQUIP_DEV || m_currentOption.m_NKC_INVENTORY_OPEN_TYPE == NKC_INVENTORY_OPEN_TYPE.NIOT_ITEM_DEV || m_currentOption.m_NKC_INVENTORY_OPEN_TYPE == NKC_INVENTORY_OPEN_TYPE.NIOT_MOLD_DEV)
		{
			m_currentOption.dOnClose?.Invoke();
			Close();
		}
		else
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME, bForce: false);
		}
	}

	public override void CloseInternal()
	{
		if (m_NKM_UI_UNIT_INVENTORY.activeSelf)
		{
			m_NKM_UI_UNIT_INVENTORY.SetActive(value: false);
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_INVENTORY_EQUIPMENTS_UNLOCK, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKCUIInventoryRemovePopup, bValue: false);
		m_LatestOpenNKMEquipItemData = null;
		m_LatestSelectedSlot = null;
		m_LastMaxEnchantLevel = 0;
		m_hsLastAutoSelectFilter = new HashSet<NKCEquipSortSystem.eFilterOption>();
		if (m_SortUI != null)
		{
			m_SortUI.ResetUI();
		}
		if (null != m_SortMiscUI)
		{
			m_SortMiscUI.ResetUI();
		}
		Clear();
	}

	public override void UnHide()
	{
		base.UnHide();
		NKCUtil.SetGameobjectActive(m_NKM_UI_INVENTORY_EQUIPMENTS_UNLOCK, bValue: false);
		if (!m_bNeedRefresh)
		{
			return;
		}
		m_bNeedRefresh = false;
		UpdateItemList(m_NKC_INVENTORY_TAB);
		if (m_NKC_INVENTORY_TAB != NKC_INVENTORY_TAB.NIT_EQUIP)
		{
			return;
		}
		long equipItemUID = m_slotEquipInfo.GetEquipItemUID();
		NKMEquipItemData itemEquip = NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(equipItemUID);
		if (itemEquip != null)
		{
			m_ssActive.UpdateEquipData(itemEquip);
			if (m_slotEquipInfo.GetEquipItemUID() == equipItemUID)
			{
				m_slotEquipInfo.SetData(itemEquip, m_currentOption.bShowFierceUI);
			}
		}
	}

	private void Clear()
	{
		ReturnSlot(m_NKC_INVENTORY_TAB);
		m_NKC_INVENTORY_TAB = NKC_INVENTORY_TAB.NIT_NONE;
	}

	public void ClearCachingData()
	{
		m_currentOption.lEquipOptionCachingByUnitUID = 0L;
	}

	private void FilterList(HashSet<NKCEquipSortSystem.eFilterOption> setFilterOption, bool bForce = false)
	{
		NKCUtil.SetGameobjectActive(m_objFilterSelected, m_currentOption.setFilterOption.Count > 0);
		m_currentOption.setFilterOption = setFilterOption;
		m_currentOption.m_EquipListOptions.FilterStatType_01 = m_ssActive.FilterStatType_01;
		m_currentOption.m_EquipListOptions.FilterStatType_02 = m_ssActive.FilterStatType_02;
		m_currentOption.m_EquipListOptions.FilterStatType_Potential = m_ssActive.FilterStatType_Potential;
		m_currentOption.m_EquipListOptions.FilterSetOptionID = m_ssActive.FilterStatType_SetOptionID;
		m_currentOption.m_hsSelectedEquipUIDToShow.Clear();
		m_hsCurrentSelectedEquips.Clear();
		m_LatestOpenNKMEquipItemData = null;
		ResetEquipSlotUI(bResetScroll: true);
	}

	public void ResetEquipSlotUI(bool bResetScroll = false)
	{
		UpdateItemList(m_NKC_INVENTORY_TAB);
		if (m_NKC_INVENTORY_TAB == NKC_INVENTORY_TAB.NIT_EQUIP)
		{
			m_LoopScrollRectEquip.TotalCount = GetSlotCount(m_NKC_INVENTORY_TAB);
			SetCurrentEquipCountUI();
			m_LoopScrollRectEquip.SetIndexPosition(0);
			NKCUtil.SetGameobjectActive(m_objEquipInfo, m_LoopScrollRectEquip.TotalCount > 0);
			NKCUtil.SetGameobjectActive(m_objEmpty, m_LoopScrollRectEquip.TotalCount == 0);
			NKCUtil.SetGameobjectActive(m_objGear, m_LoopScrollRectEquip.TotalCount > 0);
			UpdateSelectedEquipCountUI();
			UpdateSelectedEquipBreakupResult();
		}
	}

	public void ResetMiscSlotUI(bool bResetScroll = false)
	{
		if (m_NKC_INVENTORY_TAB == NKC_INVENTORY_TAB.NIT_MISC)
		{
			m_LoopScrollRect.TotalCount = m_ssActiveMisc.SortedMiscList.Count;
			m_LoopScrollRect.SetIndexPosition(0);
			NKCUtil.SetGameobjectActive(m_objEmpty, m_LoopScrollRect.TotalCount == 0);
		}
	}

	public void ResetEquipSlotList()
	{
		m_bNeedRefresh = true;
		m_dicEquipSortSystem.Clear();
		m_ssActive = GetEquipSortSystem(m_currentOption.m_NKC_INVENTORY_OPEN_TYPE);
		ResetEquipSortOption();
		ResetEquipSlotUI();
	}

	private void SortList(List<NKCEquipSortSystem.eSortOption> lstSortOption, bool bForce, bool bResetScroll = false)
	{
		if (m_NKC_INVENTORY_TAB == NKC_INVENTORY_TAB.NIT_EQUIP)
		{
			m_ssActive.SortList(lstSortOption, bForce);
			m_currentOption.lstSortOption = lstSortOption;
		}
		ProcessUIFromCurrentDisplayedSortData(bResetScroll);
	}

	private void ProcessUIFromCurrentDisplayedSortData(bool bResetScroll = false)
	{
		if (m_NKC_INVENTORY_TAB == NKC_INVENTORY_TAB.NIT_EQUIP)
		{
			SetCurrentEquipCountUI();
			if (m_dOnGetItemListAfterSelected != null)
			{
				m_dOnGetItemListAfterSelected(m_ssActive.SortedEquipList);
			}
			m_LatestSelectedSlot = null;
			m_LoopScrollRectEquip.TotalCount = GetSlotCount(m_NKC_INVENTORY_TAB);
			if (bResetScroll)
			{
				m_LatestOpenNKMEquipItemData = null;
				m_LoopScrollRectEquip.SetIndexPosition(0);
			}
			else
			{
				int indexPosition = 0;
				if (m_LatestOpenNKMEquipItemData != null)
				{
					indexPosition = m_ssActive.SortedEquipList.FindIndex((NKMEquipItemData x) => x.m_ItemUid == m_LatestOpenNKMEquipItemData.m_ItemUid);
				}
				m_LoopScrollRectEquip.SetIndexPosition(indexPosition);
			}
			NKCUtil.SetGameobjectActive(m_objFilterSelected, m_currentOption.setFilterOption.Count > 0);
			NKCUtil.SetGameobjectActive(m_objEquipInfo, m_LoopScrollRectEquip.TotalCount > 0);
			NKCUtil.SetGameobjectActive(m_objEmpty, m_LoopScrollRectEquip.TotalCount == 0);
			NKCUtil.SetGameobjectActive(m_objGear, m_LoopScrollRectEquip.TotalCount > 0);
			if (m_LatestSelectedSlot == null && m_ssActive.SortedEquipList.Count > 0 && m_lstVisibleEquipSlot.Count > 0)
			{
				for (int num = 0; num < m_lstVisibleEquipSlot.Count; num++)
				{
					m_lstVisibleEquipSlot[num].SetSelected(bSelected: false);
				}
				SetEquipInfo(null);
			}
		}
		else
		{
			m_LoopScrollRect.TotalCount = GetSlotCount(m_NKC_INVENTORY_TAB);
			m_LoopScrollRect.SetIndexPosition(0);
			NKCUtil.SetGameobjectActive(m_objEmpty, m_LoopScrollRect.TotalCount == 0);
		}
	}

	public void UpdateEquipSlot(long equipUID)
	{
		bool flag = true;
		if (m_currentOption.m_NKC_INVENTORY_OPEN_TYPE != NKC_INVENTORY_OPEN_TYPE.NIOT_NORMAL || m_NKC_INVENTORY_TAB != NKC_INVENTORY_TAB.NIT_EQUIP)
		{
			return;
		}
		for (int i = 0; i < m_lstVisibleEquipSlot.Count; i++)
		{
			NKCUISlotEquip nKCUISlotEquip = m_lstVisibleEquipSlot[i];
			if (nKCUISlotEquip != null && nKCUISlotEquip.IsActive() && nKCUISlotEquip.GetNKMEquipItemData() != null && nKCUISlotEquip.GetNKMEquipItemData().m_ItemUid == equipUID)
			{
				NKMEquipItemData itemEquip = NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(equipUID);
				bool bPresetContained = false;
				if (itemEquip != null)
				{
					bPresetContained = NKCEquipPresetDataManager.HSPresetEquipUId.Contains(itemEquip.m_ItemUid);
				}
				nKCUISlotEquip.SetData(itemEquip, null, m_bLockMaxItem, m_currentOption.bSkipItemEquipBox, m_currentOption.bShowFierceUI, bPresetContained);
				nKCUISlotEquip.SetLock(itemEquip.m_bLock, m_currentOption.bEnableLockEquipSystem);
				if (m_LatestOpenNKMEquipItemData != null && m_LatestOpenNKMEquipItemData.m_ItemUid == equipUID)
				{
					nKCUISlotEquip.SetSelected(bSelected: true);
				}
				flag = false;
			}
		}
		m_LoopScrollRectEquip.TotalCount = m_ssActive.SortedEquipList.Count;
		if (flag)
		{
			int indexPosition = 0;
			if (equipUID > 0)
			{
				indexPosition = m_ssActive.SortedEquipList.FindIndex((NKMEquipItemData x) => x.m_ItemUid == equipUID);
			}
			m_LoopScrollRectEquip.SetIndexPosition(indexPosition);
		}
		else
		{
			m_LoopScrollRectEquip.RefreshCells();
		}
	}

	public void ForceRefreshMiscTab()
	{
		OnSelectTab(NKC_INVENTORY_TAB.NIT_MISC, bForceOpen: true);
	}

	public void ForceRefreshEquipTab()
	{
		OnSelectTab(NKC_INVENTORY_TAB.NIT_EQUIP, bForceOpen: true);
	}

	public void OnSelectMiscTab()
	{
		m_currentOption.strEmptyMessage = NKCUtilString.GET_STRING_INVEN_MISC_NO_EXIST;
		m_lbEmptyMessage.text = m_currentOption.strEmptyMessage;
		OnSelectTab(NKC_INVENTORY_TAB.NIT_MISC);
	}

	public void OnSelectEquipTab()
	{
		m_currentOption.strEmptyMessage = NKCUtilString.GET_STRING_INVEN_EQUIP_NO_EXIST;
		m_lbEmptyMessage.text = m_currentOption.strEmptyMessage;
		OnSelectTab(NKC_INVENTORY_TAB.NIT_EQUIP);
	}

	private List<NKMItemMiscData> GetItemMiscDataList(NKC_INVENTORY_OPEN_TYPE _NKC_INVENTORY_OPEN_TYPE)
	{
		List<NKMItemMiscData> list = null;
		switch (_NKC_INVENTORY_OPEN_TYPE)
		{
		case NKC_INVENTORY_OPEN_TYPE.NIOT_NORMAL:
			list = new List<NKMItemMiscData>(NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.MiscItems.Values);
			break;
		case NKC_INVENTORY_OPEN_TYPE.NIOT_ITEM_DEV:
			list = new List<NKMItemMiscData>();
			foreach (NKMItemMiscTemplet value in NKMItemMiscTemplet.Values)
			{
				NKMItemMiscData item = MakeTempItemMiscData(value.m_ItemMiscID);
				list.Add(item);
			}
			break;
		}
		return list;
	}

	private NKMItemMiscData MakeTempItemMiscData(int itemID)
	{
		return new NKMItemMiscData
		{
			ItemID = itemID,
			CountFree = 1L,
			CountPaid = 0L
		};
	}

	private int GetSlotCount(NKC_INVENTORY_TAB type)
	{
		switch (type)
		{
		case NKC_INVENTORY_TAB.NIT_MISC:
			return m_lstMiscTempletData.Count;
		case NKC_INVENTORY_TAB.NIT_MOLD:
			return m_lstMoldTemplet.Count;
		case NKC_INVENTORY_TAB.NIT_EQUIP:
			if (m_currentOption.m_dOnClickEmptySlot != null)
			{
				return m_ssActive.SortedEquipList.Count + 1;
			}
			return m_ssActive.SortedEquipList.Count;
		default:
			Debug.Log("NKCUIInventory.GetSlotCount() ERROR type : " + type);
			return 0;
		}
	}

	private void UpdateItemList(NKC_INVENTORY_TAB type)
	{
		switch (type)
		{
		case NKC_INVENTORY_TAB.NIT_MISC:
		{
			m_lstMiscTempletData.Clear();
			List<NKMItemMiscData> itemMiscDataList = GetItemMiscDataList(m_eInventoryOpenType);
			if (itemMiscDataList != null)
			{
				for (int i = 0; i < itemMiscDataList.Count; i++)
				{
					NKMItemMiscData nKMItemMiscData = itemMiscDataList[i];
					if (nKMItemMiscData.TotalCount <= 0 || (m_currentOption.AdditionalExcludeFilterFunc != null && !m_currentOption.AdditionalExcludeFilterFunc(nKMItemMiscData.ItemID, m_currentOption.m_NKC_INVENTORY_OPEN_TYPE)))
					{
						continue;
					}
					if (m_eInventoryOpenType == NKC_INVENTORY_OPEN_TYPE.NIOT_NORMAL)
					{
						NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(nKMItemMiscData.ItemID);
						if (itemMiscTempletByID != null && !itemMiscTempletByID.IsHideInInven())
						{
							m_lstMiscTempletData.Add(itemMiscTempletByID);
						}
					}
					else
					{
						NKMItemMiscTemplet itemMiscTempletByID2 = NKMItemManager.GetItemMiscTempletByID(nKMItemMiscData.ItemID);
						m_lstMiscTempletData.Add(itemMiscTempletByID2);
					}
				}
			}
			UpdateMiscSortSystem();
			break;
		}
		case NKC_INVENTORY_TAB.NIT_MOLD:
			m_lstMoldTemplet.Clear();
			foreach (NKMItemMoldTemplet value in NKMItemMoldTemplet.Values)
			{
				if (m_currentOption.AdditionalExcludeFilterFunc == null || m_currentOption.AdditionalExcludeFilterFunc(value.m_MoldID, m_currentOption.m_NKC_INVENTORY_OPEN_TYPE))
				{
					m_lstMoldTemplet.Add(value);
				}
			}
			ProcessUIFromCurrentDisplayedSortData();
			break;
		case NKC_INVENTORY_TAB.NIT_EQUIP:
		{
			List<NKMEquipItemData> currentList = new List<NKMEquipItemData>();
			m_ssActive.FilterList(m_ssActive.FilterSet, m_ssActive.bHideEquippedItem);
			m_ssActive.GetCurrentEquipList(ref currentList);
			SortList(m_currentOption.lstSortOption, bForce: true);
			break;
		}
		default:
			Debug.Log("NKCUIInventory.GetSlotCount() ERROR type : " + type);
			break;
		}
	}

	private void UpdateMiscSortSystem()
	{
		m_ssActiveMisc = new NKCMiscSortSystem(NKCScenManager.CurrentUserData(), m_lstMiscTempletData, MakeSortOption());
		if (m_ssActiveMisc != null)
		{
			m_SortMiscUI.RegisterCategories(NKCMiscSortSystem.GetDefaultInteriorFilterCategory(), NKCMiscSortSystem.GetDefaultInteriorSortCategory());
			m_SortMiscUI.RegisterMiscSort(m_ssActiveMisc);
			m_SortMiscUI.ResetUI();
		}
		m_LoopScrollRect.TotalCount = m_ssActiveMisc.SortedMiscList.Count;
		m_LoopScrollRect.SetIndexPosition(0);
		NKCUtil.SetGameobjectActive(m_objEmpty, m_LoopScrollRect.TotalCount == 0);
	}

	private void ChangeUI()
	{
		bool flag = false;
		if (m_NKC_INVENTORY_TAB == NKC_INVENTORY_TAB.NIT_EQUIP)
		{
			flag = true;
		}
		bool bValue = true;
		if (m_currentOption.m_NKC_INVENTORY_OPEN_TYPE == NKC_INVENTORY_OPEN_TYPE.NIOT_EQUIP_SELECT || m_currentOption.m_NKC_INVENTORY_OPEN_TYPE == NKC_INVENTORY_OPEN_TYPE.NIOT_ITEM_DEV || m_currentOption.m_NKC_INVENTORY_OPEN_TYPE == NKC_INVENTORY_OPEN_TYPE.NIOT_EQUIP_DEV || m_currentOption.m_NKC_INVENTORY_OPEN_TYPE == NKC_INVENTORY_OPEN_TYPE.NIOT_MOLD_DEV)
		{
			bValue = false;
		}
		float y = 0f;
		if (flag)
		{
			y = 131.05f;
		}
		m_RectMask.offsetMin = new Vector2(m_RectMask.offsetMin.x, y);
		NKCUtil.SetGameobjectActive(m_NKM_UI_INVENTORY_TAP_MISC.gameObject, bValue);
		NKCUtil.SetGameobjectActive(m_NKM_UI_INVENTORY_TAP_GEAR.gameObject, bValue);
		NKCUtil.SetGameobjectActive(m_SortUI.gameObject, flag);
		if (flag)
		{
			m_SortUI.ResetUI();
		}
		NKCUtil.SetGameobjectActive(m_SortMiscUI.gameObject, !flag);
		if (!flag)
		{
			m_SortMiscUI.ResetUI();
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_INVENTORY_TEXT1, flag);
		NKCUtil.SetGameobjectActive(m_NKM_UI_INVENTORY_LINE, flag);
		NKCUtil.SetGameobjectActive(m_NKM_UI_INVENTORY_TEXTS, flag);
		NKCUtil.SetGameobjectActive(m_NKM_UI_INVENTORY_TEXT, flag);
		NKCUtil.SetGameobjectActive(m_NKM_UI_INVENTORY_NUMBER_TEXT, flag);
		NKCUtil.SetGameobjectActive(m_NKM_UI_INVENTORY_ADD, flag);
		NKCUtil.SetGameobjectActive(m_NKM_UI_INVENTORY_MENU_LOCK, flag && m_currentOption.m_NKC_INVENTORY_OPEN_TYPE == NKC_INVENTORY_OPEN_TYPE.NIOT_NORMAL);
		NKCUtil.SetGameobjectActive(m_NKM_UI_INVENTORY_MENU_DELETE, flag && m_currentOption.m_NKC_INVENTORY_OPEN_TYPE == NKC_INVENTORY_OPEN_TYPE.NIOT_NORMAL);
		m_NKM_UI_INVENTORY_TAP_MISC.Select(m_NKC_INVENTORY_TAB == NKC_INVENTORY_TAB.NIT_MISC);
		m_NKM_UI_INVENTORY_TAP_GEAR.Select(m_NKC_INVENTORY_TAB == NKC_INVENTORY_TAB.NIT_EQUIP);
		m_NKM_UI_INVENTORY_MENU_LOCK.Select(flag && m_currentOption.bEnableLockEquipSystem);
		NKCUtil.SetGameobjectActive(m_NKM_UI_INVENTORY_MENU_CHOICE, m_currentOption.bMultipleSelect);
		NKCUtil.SetGameobjectActive(m_NKM_UI_INVENTORY_MENU_CHOICE_GET_ITEM, m_currentOption.bShowRemoveItem);
		NKCUtil.SetGameobjectActive(m_NKM_UI_INVENTORY_MENU_LOCK_MSG, flag && m_currentOption.bEnableLockEquipSystem);
		NKCUtil.SetGameobjectActive(m_NKM_UI_INVENTORY_MENU_DELETE_MSG, flag && m_currentOption.bShowRemoveItem);
	}

	private void UpdateSelectedEquipBreakupResult()
	{
		int num = 0;
		if (!m_currentOption.bShowRemoveItem)
		{
			for (num = 0; num < m_lstBotNKCUISlot.Count; num++)
			{
				NKCUtil.SetGameobjectActive(m_lstBotNKCUISlot[num].gameObject, bValue: false);
			}
			return;
		}
		Dictionary<int, NKMEquipTemplet.OnRemoveItemData> dictionary = new Dictionary<int, NKMEquipTemplet.OnRemoveItemData>();
		List<long> list = new List<long>(m_hsCurrentSelectedEquips);
		int num2 = 0;
		NKMInventoryData inventoryData = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData;
		for (num = 0; num < list.Count; num++)
		{
			NKMEquipItemData itemEquip = inventoryData.GetItemEquip(list[num]);
			if (itemEquip == null)
			{
				continue;
			}
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
			if (equipTemplet == null)
			{
				continue;
			}
			for (num2 = 0; num2 < equipTemplet.m_OnRemoveItemList.Count; num2++)
			{
				NKMEquipTemplet.OnRemoveItemData onRemoveItemData = equipTemplet.m_OnRemoveItemList[num2];
				if (dictionary.TryGetValue(onRemoveItemData.m_ItemID, out var value))
				{
					value.m_ItemCount += onRemoveItemData.m_ItemCount;
					dictionary[onRemoveItemData.m_ItemID] = value;
					continue;
				}
				value = new NKMEquipTemplet.OnRemoveItemData
				{
					m_ItemID = onRemoveItemData.m_ItemID,
					m_ItemCount = onRemoveItemData.m_ItemCount
				};
				dictionary.Add(value.m_ItemID, value);
			}
		}
		List<NKMEquipTemplet.OnRemoveItemData> list2 = new List<NKMEquipTemplet.OnRemoveItemData>(dictionary.Values);
		int num3 = list2.Count - m_lstBotNKCUISlot.Count;
		for (num = 0; num < num3; num++)
		{
			NKCUISlot newInstance = NKCUISlot.GetNewInstance(m_NKM_UI_UNIT_SELECT_LIST_CHOICE_GET_ITEM_LIST_Content);
			if (newInstance != null)
			{
				m_lstBotNKCUISlot.Add(newInstance);
			}
		}
		for (num = 0; num < m_lstBotNKCUISlot.Count; num++)
		{
			if (num < list2.Count)
			{
				m_lstBotNKCUISlot[num].transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
				m_lstBotNKCUISlot[num].m_cbtnButton.UpdateOrgSize();
				NKCUtil.SetGameobjectActive(m_lstBotNKCUISlot[num].gameObject, bValue: true);
				m_lstBotNKCUISlot[num].SetData(NKCUISlot.SlotData.MakeMiscItemData(list2[num].m_ItemID, list2[num].m_ItemCount), bShowName: false, bShowNumber: true, bEnableLayoutElement: true, null);
				m_lstBotNKCUISlot[num].SetOpenItemBoxOnClick();
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstBotNKCUISlot[num].gameObject, bValue: false);
			}
		}
	}

	private void UpdateSelectedEquipCountUI()
	{
		if (m_currentOption.bMultipleSelect && m_hsCurrentSelectedEquips != null)
		{
			m_NKM_UI_INVENTORY_MENU_CHOICE_NUMBER_TEXT.text = $"{m_hsCurrentSelectedEquips.Count} / {m_currentOption.iMaxMultipleSelect}";
		}
	}

	public void SetCurrentEquipCountUI()
	{
		NKMInventoryData inventoryData = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData;
		if (inventoryData != null)
		{
			int countEquipItemTypes = inventoryData.GetCountEquipItemTypes();
			m_NKM_UI_INVENTORY_NUMBER_TEXT.GetComponent<Text>().text = $"{countEquipItemTypes}/{inventoryData.m_MaxItemEqipCount}";
		}
	}

	public void OnExpandInventoryPopup()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMInventoryData inventoryData = myUserData.m_InventoryData;
		if (inventoryData == null)
		{
			return;
		}
		int maxItemEqipCount = inventoryData.m_MaxItemEqipCount;
		NKM_INVENTORY_EXPAND_TYPE nKM_INVENTORY_EXPAND_TYPE = NKM_INVENTORY_EXPAND_TYPE.NIET_EQUIP;
		int count = 1;
		int resultCount;
		bool flag = !NKCAdManager.IsAdRewardInventory(nKM_INVENTORY_EXPAND_TYPE) || !NKMInventoryManager.CanExpandInventoryByAd(nKM_INVENTORY_EXPAND_TYPE, myUserData, count, out resultCount);
		if (!NKMInventoryManager.CanExpandInventory(nKM_INVENTORY_EXPAND_TYPE, myUserData, count, out resultCount) && flag)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_CANNOT_EXPAND_INVENTORY));
			return;
		}
		string expandDesc = NKCUtilString.GetExpandDesc(nKM_INVENTORY_EXPAND_TYPE);
		NKCPopupInventoryAdd.SliderInfo sliderInfo = new NKCPopupInventoryAdd.SliderInfo
		{
			increaseCount = 5,
			maxCount = 2000,
			currentCount = maxItemEqipCount,
			inventoryType = NKM_INVENTORY_EXPAND_TYPE.NIET_EQUIP
		};
		NKCPopupInventoryAdd.Instance.Open(NKCUtilString.GET_STRING_INVENTORY_EQUIP, expandDesc, sliderInfo, 50, 101, delegate(int value)
		{
			NKCPacketSender.Send_NKMPacket_INVENTORY_EXPAND_REQ(NKM_INVENTORY_EXPAND_TYPE.NIET_EQUIP, value);
		}, showResource: true);
	}

	public void OnInventoryAdd()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_INVENTORY_EQUIPMENTS_UNLOCK, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_INVENTORY_EQUIPMENTS_UNLOCK, bValue: true);
	}

	public void OnLockModeToggle(bool bValue)
	{
		m_currentOption.bEnableLockEquipSystem = bValue;
		m_NKM_UI_INVENTORY_MENU_LOCK.Select(bValue, bForce: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_INVENTORY_MENU_LOCK_MSG, bValue);
		m_NKM_UI_INVENTORY_MENU_DELETE_canvas.alpha = (bValue ? 0.3f : 1f);
		SetEquipInfo(m_LatestSelectedSlot);
		foreach (NKCUISlotEquip slot in m_lstVisibleEquipSlot)
		{
			if (!(null == slot))
			{
				NKMEquipItemData nKMEquipItemData = m_ssActive.SortedEquipList.Find((NKMEquipItemData x) => x.m_ItemUid == slot.GetEquipItemUID());
				if (nKMEquipItemData != null)
				{
					slot.SetLock(nKMEquipItemData?.m_bLock ?? false, m_currentOption.bEnableLockEquipSystem);
				}
			}
		}
	}

	private void OnClickEquipSlot(NKCUISlotEquip slot, NKMEquipItemData equipData)
	{
		if (m_dChangeOptionNotify != null)
		{
			int filterSetOptionID = 0;
			if (m_currentOption.setFilterOption.Count > 0)
			{
				foreach (NKCEquipSortSystem.eFilterOption item in m_currentOption.setFilterOption)
				{
					if (item == NKCEquipSortSystem.eFilterOption.Equip_Stat_SetOption)
					{
						filterSetOptionID = m_currentOption.m_EquipListOptions.FilterSetOptionID;
						break;
					}
				}
			}
			m_dChangeOptionNotify(m_currentOption.setFilterOption, filterSetOptionID);
		}
		m_dOnClickEquipSlot?.Invoke(slot, equipData);
	}

	private void OnTouchMultiCancel()
	{
		if (m_eInventoryOpenType == NKC_INVENTORY_OPEN_TYPE.NIOT_EQUIP_SELECT)
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
	}

	public void OnRemoveMode(bool bValue)
	{
		if (bValue)
		{
			m_prevOption = m_currentOption;
			m_currentOption = new EquipSelectListOptions(NKC_INVENTORY_OPEN_TYPE.NIOT_EQUIP_SELECT, _bMultipleSelect: true);
			m_currentOption.m_EquipListOptions = m_prevOption.m_EquipListOptions;
			m_eInventoryOpenType = m_currentOption.m_NKC_INVENTORY_OPEN_TYPE;
			m_currentOption.m_dOnSelectedEquipSlot = null;
			m_currentOption.bMultipleSelect = bValue;
			m_currentOption.bShowRemoveItem = bValue;
			m_currentOption.iMaxMultipleSelect = 100;
			m_currentOption.bHideLockItem = true;
			m_currentOption.bHideMaxLvItem = false;
			m_currentOption.bLockMaxItem = false;
			m_currentOption.m_dOnFinishMultiSelection = RemoveItemList;
			m_currentOption.strEmptyMessage = NKCUtilString.GET_STRING_EQUIP_BREAK_UP_NO_EXIST_EQUIP;
			m_currentOption.setFilterOption = m_prevOption.setFilterOption;
			m_currentOption.lstSortOption = m_prevOption.lstSortOption;
			SetOnClickEquipSlot(ToggleSelectedState);
		}
		else
		{
			m_hsCurrentSelectedEquips.Clear();
			m_currentOption = m_prevOption;
			m_currentOption.m_EquipListOptions = m_prevOption.m_EquipListOptions;
			m_currentOption.m_EquipListOptions.FilterStatType_01 = m_ssActive.FilterStatType_01;
			m_currentOption.m_EquipListOptions.FilterStatType_02 = m_ssActive.FilterStatType_02;
			m_currentOption.m_EquipListOptions.FilterStatType_Potential = m_ssActive.FilterStatType_Potential;
			m_currentOption.m_EquipListOptions.FilterSetOptionID = m_ssActive.FilterStatType_SetOptionID;
			m_currentOption.bEnableLockEquipSystem = false;
			SetOnClickEquipSlot(m_currentOption.m_dOnSelectedEquipSlot);
		}
		m_LatestSelectedSlot = null;
		m_LatestOpenNKMEquipItemData = null;
		OnSelectTab(m_NKC_INVENTORY_TAB, bForceOpen: true);
	}

	private void RemoveItemList(List<long> list)
	{
		if (list == null || list.Count <= 0)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_NO_EXIST_SELECTED_EQUIP);
			return;
		}
		NKMInventoryData inventoryData = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData;
		for (int i = 0; i < list.Count; i++)
		{
			NKMEquipItemData itemEquip = inventoryData.GetItemEquip(list[i]);
			if (itemEquip == null)
			{
				continue;
			}
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
			if (equipTemplet != null && equipTemplet.m_NKM_ITEM_GRADE >= NKM_ITEM_GRADE.NIG_SR)
			{
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_EQUIP_BREAK_UP_WARNING, delegate
				{
					NKCPacketSender.Send_NKMPacket_REMOVE_EQUIP_ITEM_REQ(list);
				});
				return;
			}
		}
		NKCPacketSender.Send_NKMPacket_REMOVE_EQUIP_ITEM_REQ(list);
	}

	private void OnTouchAutoSelect()
	{
		m_NKCUIInventoryRemovePopup.Open(m_LastMaxEnchantLevel, m_hsLastAutoSelectFilter);
	}

	private void OnAutoSelect(int maxEnchantLevel, HashSet<NKCEquipSortSystem.eFilterOption> hsOptions)
	{
		m_LastMaxEnchantLevel = maxEnchantLevel;
		m_hsLastAutoSelectFilter = hsOptions;
		List<NKMEquipItemData> currentList = new List<NKMEquipItemData>();
		m_ssActive.FilterList(m_currentOption.setFilterOption, m_ssActive.bHideEquippedItem);
		m_ssActive.GetCurrentEquipList(ref currentList);
		int num = 0;
		while (m_hsCurrentSelectedEquips.Count < m_currentOption.iMaxMultipleSelect && num < currentList.Count)
		{
			NKMEquipItemData nKMEquipItemData = currentList[num];
			if (AutoSelectFilter(nKMEquipItemData, maxEnchantLevel, hsOptions) && !m_hsCurrentSelectedEquips.Contains(nKMEquipItemData.m_ItemUid))
			{
				m_hsCurrentSelectedEquips.Add(nKMEquipItemData.m_ItemUid);
				NKCUISlotEquip invenEquipSlot = GetInvenEquipSlot(nKMEquipItemData.m_ItemUid);
				if (invenEquipSlot != null)
				{
					invenEquipSlot.SetSlotState((!m_currentOption.bShowRemoveItem) ? NKCUIInvenEquipSlot.EQUIP_SLOT_STATE.ESS_SELECTED : NKCUIInvenEquipSlot.EQUIP_SLOT_STATE.ESS_DELETE);
				}
			}
			num++;
		}
		UpdateSelectedEquipCountUI();
		UpdateSelectedEquipBreakupResult();
	}

	private bool AutoSelectFilter(NKMEquipItemData itemData, int maxEnchantLevel, HashSet<NKCEquipSortSystem.eFilterOption> hsOptions)
	{
		if (itemData.m_OwnerUnitUID > 0 || itemData.m_bLock)
		{
			return false;
		}
		if (itemData.m_EnchantLevel > maxEnchantLevel)
		{
			return false;
		}
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemData.m_ItemEquipID);
		if (equipTemplet == null)
		{
			return false;
		}
		if (equipTemplet.m_EquipUnitStyleType == NKM_UNIT_STYLE_TYPE.NUST_ENCHANT)
		{
			return false;
		}
		NKCEquipSortSystem.eFilterOption filterOption = NKCEquipSortSystem.GetFilterOption(equipTemplet.m_NKM_ITEM_GRADE);
		if (!hsOptions.Contains(filterOption))
		{
			return false;
		}
		NKCEquipSortSystem.eFilterOption filterOptionByEquipTier = NKCEquipSortSystem.GetFilterOptionByEquipTier(equipTemplet.m_NKM_ITEM_TIER);
		if (!hsOptions.Contains(filterOptionByEquipTier))
		{
			return false;
		}
		return true;
	}

	private NKCUISlotEquip GetInvenEquipSlot(long uid)
	{
		return m_lstVisibleEquipSlot.Find((NKCUISlotEquip v) => v.GetNKMEquipItemData() != null && v.GetEquipItemUID() == uid);
	}

	public void UpdateEquipment(long equipUID, NKMEquipItemData equipData)
	{
		NKMEquipItemData nKMEquipItemData = m_ssActive.SortedEquipList.Find((NKMEquipItemData x) => x.m_ItemUid == equipUID);
		if (nKMEquipItemData == null || equipData == null)
		{
			ForceRefreshEquipTab();
			return;
		}
		nKMEquipItemData = equipData;
		for (int num = 0; num < m_lstVisibleEquipSlot.Count; num++)
		{
			if (m_lstVisibleEquipSlot[num].GetEquipItemUID() == equipUID)
			{
				m_lstVisibleEquipSlot[num].SetData(nKMEquipItemData, OnSelectedSlot, m_bLockMaxItem, m_currentOption.bSkipItemEquipBox, m_currentOption.bShowFierceUI, NKCEquipPresetDataManager.HSPresetEquipUId.Contains(nKMEquipItemData.m_ItemUid));
				m_lstVisibleEquipSlot[num].SetLock(nKMEquipItemData?.m_bLock ?? false, m_currentOption.bEnableLockEquipSystem);
				if (m_LatestOpenNKMEquipItemData != null && m_LatestOpenNKMEquipItemData.m_ItemUid == equipUID)
				{
					m_lstVisibleEquipSlot[num].SetSelected(bSelected: true);
					SetEquipInfo(m_lstVisibleEquipSlot[num]);
				}
				break;
			}
		}
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		if (m_NKC_INVENTORY_TAB == NKC_INVENTORY_TAB.NIT_MISC)
		{
			if (base.IsHidden)
			{
				m_bNeedRefresh = true;
			}
			else
			{
				UpdateItemList(NKC_INVENTORY_TAB.NIT_MISC);
			}
		}
	}

	public override void OnEquipChange(NKMUserData.eChangeNotifyType eType, long equipUID, NKMEquipItemData equipItem)
	{
		if (m_NKC_INVENTORY_TAB != NKC_INVENTORY_TAB.NIT_EQUIP)
		{
			return;
		}
		if (m_ssActive != null && m_ssActive.SortedEquipList != null)
		{
			if (equipItem != null)
			{
				NKMEquipItemData nKMEquipItemData = m_ssActive.SortedEquipList.Find((NKMEquipItemData x) => x.m_ItemUid == equipUID);
				if (nKMEquipItemData != null)
				{
					m_ssActive.SortedEquipList.IndexOf(nKMEquipItemData);
					m_ssActive.UpdateEquipData(equipItem);
					if (m_slotEquipInfo.GetEquipItemUID() == equipUID)
					{
						m_slotEquipInfo.SetData(equipItem, m_currentOption.bShowFierceUI);
					}
				}
			}
			else
			{
				m_dicEquipSortSystem.Remove(m_eInventoryOpenType);
				m_ssActive = GetEquipSortSystem(m_eInventoryOpenType);
			}
		}
		if (base.IsHidden)
		{
			m_bNeedRefresh = true;
		}
		else
		{
			UpdateEquipment(equipUID, equipItem);
		}
	}

	private void OnFilterTokenChanged(string str)
	{
		if (m_ssActive != null)
		{
			m_ssActive.FilterTokenString = str;
			ResetEquipSlotUI(bResetScroll: true);
		}
	}

	public HashSet<long> GetSelectedEquips()
	{
		return new HashSet<long>(m_hsCurrentSelectedEquips);
	}

	public RectTransform ScrollToUnitAndGetRect(long UID)
	{
		int num = m_ssActive.SortedEquipList.FindIndex((NKMEquipItemData x) => x != null && x.m_ItemUid == UID);
		if (num < 0)
		{
			Debug.LogError("Target unit not found!!");
			return null;
		}
		if (m_NKC_INVENTORY_TAB == NKC_INVENTORY_TAB.NIT_EQUIP)
		{
			m_LoopScrollRectEquip.SetIndexPosition(num);
		}
		else
		{
			m_LoopScrollRect.SetIndexPosition(num);
		}
		NKCUISlotEquip invenEquipSlot = GetInvenEquipSlot(UID);
		if (invenEquipSlot == null)
		{
			return null;
		}
		return invenEquipSlot.gameObject.GetComponent<RectTransform>();
	}
}
