using System;
using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Logging;
using NKC.Publisher;
using NKC.Templet;
using NKC.UI.Component;
using NKC.UI.NPC;
using NKM;
using NKM.Shop;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public class NKCUIShop : NKCUIBase
{
	[Serializable]
	public struct DisplaySet
	{
		public ShopDisplayType slotType;

		public NKCUIShopSlotBase slotPrefab;

		public LoopScrollRect scrollRect;

		public RectTransform m_rtParent;

		public override string ToString()
		{
			return "LoopScroll " + slotType;
		}
	}

	[Serializable]
	public struct FullDisplaySet
	{
		public ShopDisplayType slotType;

		public NKCUIShopSlotBase slotPrefab;

		public NKCUIComDragSelectablePanel draggablePanel;

		public RectTransform m_rtParent;

		public override string ToString()
		{
			return "FullDisplay " + slotType;
		}
	}

	public enum eTabMode
	{
		Fold,
		All
	}

	public delegate void OnProductBuyDelegate(int ProductID, int ProductCount = 1, List<int> lstSelection = null);

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_shop";

	private const string UI_ASSET_NAME = "NKM_UI_SHOP";

	private static NKCUIShop m_Instance;

	private const string ASSET_BUNDLE_TOP_BANNER = "ab_ui_nkm_ui_shop_thumbnail";

	private List<int> RESOURCE_LIST = new List<int>();

	private Dictionary<ShopDisplayType, DisplaySet> m_dicLoopScrollSet = new Dictionary<ShopDisplayType, DisplaySet>();

	private Dictionary<ShopDisplayType, FullDisplaySet> m_dicFullDisplaySet = new Dictionary<ShopDisplayType, FullDisplaySet>();

	public float m_fChainTabOffsetX = 135f;

	public bool m_bUseSlideBanner;

	[Header("슬롯/탭 프리팹")]
	public NKCUIShopTab m_pfbTab;

	public NKCUIShopTabSlot m_pfbTabSubSlot;

	[Header("좌측 탭 메뉴")]
	public ScrollRect m_srTab;

	public Transform m_trTabRoot;

	public NKCUIComToggleGroup m_tgTabGroup;

	[Header("상단 배너")]
	public GameObject m_objTopBanner;

	public Text m_lbTopBanner;

	public NKCUIComStateButton m_btnBuyAll;

	[Header("우측 상품 스크롤뷰")]
	public List<DisplaySet> m_lstDisplaySet;

	public List<FullDisplaySet> m_lstFullDisplaySet;

	public NKCUIComDragSelectablePanel m_CommonFullDisplayDragSelectPanel;

	public Transform m_ReturnedSlotParent;

	private ShopDisplayType m_eCurrentCommonFulldisplaySet;

	[Header("일본 법무 대응")]
	public GameObject m_objJPNPolicy;

	public NKCUIComStateButton m_csbtnJPNPaymentLaw;

	public NKCUIComStateButton m_csbtnJPNCommecialLaw;

	[Header("단계형 상품 관련")]
	public GameObject m_objChainTab;

	public NKCUIComToggle m_tglChain_01;

	public NKCUIComToggle m_tglChain_02;

	public NKCUIComToggle m_tglChain_03;

	public GameObject m_objChainLocked;

	public GameObject m_objTabResetTime;

	public Text m_lbTabRemainTime;

	[Header("홈 화면 추가 구성")]
	public NKCUIComDragSelectablePanel m_HomePackageSlidePanel;

	public NKCUIShopSlotHomeBanner m_pfbHomeLimitPackageSlot;

	public GameObject m_objFeaturedEmpty;

	[Header("NPC")]
	public NKCUINPCSpine m_UINPCShop;

	public GameObject m_objNPCFront;

	[Header("보급소 갱신")]
	public Text m_lbSupplyTimeLeft;

	public Text m_lbSupplyCountLeft;

	public Text m_lbSupplyRefreshCost;

	public NKCUIComButton m_cbtnSupplyRefresh;

	[Header("다중 구매")]
	public GameObject m_objMultiBuy;

	public NKCUIComStateButton m_csbtnMultiBuy;

	public NKCUIComStateButton m_csbtnMultiBuyClear;

	[Header("상품이 없을 경우")]
	public GameObject m_objEmptyList;

	[Header("상품 받아오는 중")]
	public GameObject m_objFetchItem;

	[Header("카테고리 변경 UI")]
	public NKCUIComStateButton m_csbtnChangeCategory;

	public NKCUIShopCategoryChange m_uiShopCategoryChange;

	public GameObject m_objCategoryReddot;

	public GameObject m_objReddot_RED;

	public GameObject m_objReddot_YELLOW;

	public Text m_lbCategoryReddotCount;

	[Header("필터")]
	public NKCUIComStateButton m_csbtnFilter;

	public NKCUIComStateButton m_csbtnFilterActive;

	private NKCShopProductSortSystem m_ssProduct;

	[Header("기타")]
	public NKCUIComStateButton m_csbtnOperatorSubSkillPool;

	private List<NKCShopBannerTemplet> m_lstHomeBannerTemplet = new List<NKCShopBannerTemplet>();

	private Stack<NKCUIShopSlotHomeBanner> m_stkLevelPackageObjects = new Stack<NKCUIShopSlotHomeBanner>();

	private Dictionary<ShopDisplayType, List<NKCUIShopSlotBase>> m_dicCardSlot = new Dictionary<ShopDisplayType, List<NKCUIShopSlotBase>>();

	private Dictionary<ShopDisplayType, Stack<NKCUIShopSlotBase>> m_dicSlotStack = new Dictionary<ShopDisplayType, Stack<NKCUIShopSlotBase>>();

	private HashSet<int> m_hsMultiBuy = new HashSet<int>();

	protected List<ShopTabTemplet> m_lstEnabledTabs = new List<ShopTabTemplet>();

	protected Dictionary<string, NKCUIShopTab> m_dicTab = new Dictionary<string, NKCUIShopTab>();

	protected Dictionary<ShopTabTemplet, List<ShopItemTemplet>> m_dicProducts = new Dictionary<ShopTabTemplet, List<ShopItemTemplet>>();

	private Color BUY_ALL_TEXT_COLOR_DEFAULT = new Color(0.34509805f, 8f / 51f, 0.09019608f);

	private string m_eCurrentTab = "TAB_NONE";

	private int m_CurrentSubIndex;

	private int m_CurrentChainIndex;

	private long m_NextSupplyUpdateTick;

	private long m_NextChainUpdateTick;

	private long m_TabEndTick;

	private bool m_bUseTabEndTimer;

	private long ShopItemUpdateTimeStamp;

	private ShopDisplayType m_eDisplayType = ShopDisplayType.None;

	private List<ShopItemTemplet> m_lstShopItem = new List<ShopItemTemplet>();

	private List<NKCShopFeaturedTemplet> m_lstFeatured = new List<NKCShopFeaturedTemplet>();

	private bool m_bPlayedScenMusic;

	protected eTabMode m_eTabMode;

	protected NKCShopManager.ShopTabCategory m_eCategory = NKCShopManager.ShopTabCategory.NONE;

	private bool playingTutorial;

	private float UPDATE_INTERVAL = 1f;

	private float updateTimer;

	public static NKCUIShop Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIShop>("ab_ui_nkm_ui_shop", "NKM_UI_SHOP", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIShop>();
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

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.ON_PLAY_GAME;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => NKCUtilString.GET_STRING_SHOP;

	public override List<int> UpsideMenuShowResourceList => RESOURCE_LIST;

	protected virtual bool AlwaysShowNPC => false;

	protected virtual bool UseTabVisible => true;

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

	public static void ShopShortcut(string tabType = "TAB_NONE", int subIndex = 0, int reservedProductID = 0)
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.LOBBY_SUBMENU))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.LOBBY_SUBMENU);
			return;
		}
		NKCShopCategoryTemplet categoryFromTab = NKCShopManager.GetCategoryFromTab(tabType);
		if (categoryFromTab != null)
		{
			ShopShortcut(categoryFromTab.m_eCategory, tabType, subIndex, reservedProductID);
		}
		else
		{
			Debug.LogError("Category not found from tab " + tabType + "!!");
		}
	}

	public static void ShopShortcut(NKCShopManager.ShopTabCategory category, string tabType = "TAB_NONE", int subIndex = 0, int reservedProductID = 0)
	{
		if (!IsInstanceOpen)
		{
			Instance.Open(category, tabType, subIndex, reservedProductID);
			return;
		}
		Instance.ChangeCategory(category, tabType, subIndex);
		if (ShopItemTemplet.Find(reservedProductID) != null)
		{
			NKCShopManager.OnBtnProductBuy(reservedProductID, bSupply: false);
		}
	}

	public override void CloseInternal()
	{
		NKCSoundManager.StopAllSound(SOUND_TRACK.VOICE);
		NKCShopManager.ClearLinkedItemCache();
		if (m_bPlayedScenMusic)
		{
			NKCSoundManager.PlayScenMusic();
			m_bPlayedScenMusic = false;
		}
		base.gameObject.SetActive(value: false);
		NKCShopManager.SetLastCheckedUTCTime(m_eCurrentTab, m_CurrentSubIndex);
		m_eCurrentTab = string.Empty;
		m_CurrentSubIndex = 0;
	}

	public override void OnBackButton()
	{
		if (m_uiShopCategoryChange != null && m_uiShopCategoryChange.gameObject.activeSelf)
		{
			NKCUtil.SetGameobjectActive(m_uiShopCategoryChange, bValue: false);
		}
		else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_SHOP)
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME, bForce: false);
		}
		else
		{
			Close();
		}
	}

	private IEnumerable<DisplaySet> GetAllDisplaySet()
	{
		foreach (KeyValuePair<ShopDisplayType, DisplaySet> item in m_dicLoopScrollSet)
		{
			if (item.Value.scrollRect != null)
			{
				yield return item.Value;
			}
		}
	}

	private IEnumerable<FullDisplaySet> GetAllFullDisplaySet()
	{
		foreach (KeyValuePair<ShopDisplayType, FullDisplaySet> item in m_dicFullDisplaySet)
		{
			if (item.Value.draggablePanel != null)
			{
				yield return item.Value;
			}
		}
	}

	private NKCUIShopTab GetUIShopTab(ShopTabTemplet shopTabTemplet)
	{
		eTabMode eTabMode = m_eTabMode;
		string text = ((eTabMode != eTabMode.Fold && eTabMode == eTabMode.All) ? MakeTabUIKey(shopTabTemplet) : $"{shopTabTemplet.TabType}[{0}]");
		if (text == null)
		{
			return null;
		}
		if (m_dicTab.ContainsKey(text))
		{
			return m_dicTab[text];
		}
		return null;
	}

	private string MakeTabUIKey(string tabType, int subIndex)
	{
		return $"{tabType}[{subIndex}]";
	}

	private string MakeTabUIKey(ShopTabTemplet tabTemplet)
	{
		if (tabTemplet == null)
		{
			return null;
		}
		return $"{tabTemplet.TabType}[{tabTemplet.SubIndex}]";
	}

	public string GetShortcutParam()
	{
		return $"{m_eCurrentTab},{m_CurrentSubIndex}";
	}

	private RectTransform GetRecommendObject()
	{
		if (m_stkLevelPackageObjects.Count > 0)
		{
			return m_stkLevelPackageObjects.Pop().GetComponent<RectTransform>();
		}
		return UnityEngine.Object.Instantiate(m_pfbHomeLimitPackageSlot).GetComponent<RectTransform>();
	}

	private void ReturnRecommendObject(RectTransform rect)
	{
		NKCUIShopSlotHomeBanner component = rect.GetComponent<NKCUIShopSlotHomeBanner>();
		if (component != null)
		{
			m_stkLevelPackageObjects.Push(component);
		}
		rect.gameObject.SetActive(value: false);
		rect.parent = base.transform;
	}

	private void ProvideRecommendData(RectTransform rect, int idx)
	{
		NKCUIShopSlotHomeBanner component = rect.GetComponent<NKCUIShopSlotHomeBanner>();
		if (component != null)
		{
			rect.SetParent(m_HomePackageSlidePanel.transform);
			NKCUtil.SetGameobjectActive(component, bValue: true);
			component.SetData(m_lstHomeBannerTemplet[idx], OnBtnBanner);
		}
	}

	private void OnRecommendFocus(RectTransform rect, bool bFocus)
	{
		if (bFocus)
		{
			rect.GetComponent<NKCUIShopSlotHomeBanner>();
		}
	}

	private List<NKCUIShopSlotBase> GetSlotList(ShopDisplayType type)
	{
		if (m_dicCardSlot.TryGetValue(type, out var value))
		{
			return value;
		}
		List<NKCUIShopSlotBase> list = new List<NKCUIShopSlotBase>();
		m_dicCardSlot.Add(type, list);
		return list;
	}

	private LoopScrollRect GetLoopScrollRect(ShopDisplayType type)
	{
		if (m_dicLoopScrollSet.TryGetValue(type, out var value))
		{
			return value.scrollRect;
		}
		return null;
	}

	private NKCUIComDragSelectablePanel GetDragPanel(ShopDisplayType type)
	{
		if (m_dicFullDisplaySet.TryGetValue(type, out var value))
		{
			return value.draggablePanel;
		}
		return null;
	}

	private RectTransform GetShopSlot(ShopDisplayType type, int index)
	{
		NKCUIShopSlotBase nKCUIShopSlotBase;
		if (m_dicSlotStack.ContainsKey(type) && m_dicSlotStack[type] != null && m_dicSlotStack[type].Count > 0)
		{
			nKCUIShopSlotBase = m_dicSlotStack[type].Pop();
		}
		else
		{
			NKCUIShopSlotBase slotPrefab;
			if (m_dicLoopScrollSet.TryGetValue(type, out var value))
			{
				slotPrefab = value.slotPrefab;
			}
			else
			{
				if (!m_dicFullDisplaySet.TryGetValue(type, out var value2))
				{
					return null;
				}
				slotPrefab = value2.slotPrefab;
			}
			if (slotPrefab == null)
			{
				Debug.LogError("shop slot prefab null!");
				return null;
			}
			nKCUIShopSlotBase = UnityEngine.Object.Instantiate(slotPrefab);
		}
		nKCUIShopSlotBase.Init(OnBtnProductBuy, ForceUpdateItemList);
		GetSlotList(type)?.Add(nKCUIShopSlotBase);
		return nKCUIShopSlotBase.GetComponent<RectTransform>();
	}

	private void ReturnShopSlot(ShopDisplayType type, Transform tr)
	{
		NKCUIShopSlotBase component = tr.GetComponent<NKCUIShopSlotBase>();
		GetSlotList(type).Remove(component);
		tr.gameObject.SetActive(value: false);
		tr.SetParent(m_ReturnedSlotParent);
		if (!m_dicSlotStack.ContainsKey(type))
		{
			m_dicSlotStack.Add(type, new Stack<NKCUIShopSlotBase>());
		}
		m_dicSlotStack[type].Push(component);
	}

	private void ReturnCommonFullDisplayShopSlot(Transform tr)
	{
		ReturnShopSlot(m_eCurrentCommonFulldisplaySet, tr);
	}

	private void ProvideShopSlotData(Transform transform, int idx)
	{
		NKCUIShopSlotBase component = transform.GetComponent<NKCUIShopSlotBase>();
		if (component == null)
		{
			return;
		}
		component.SetOverrideImageAsset("");
		ShopTabTemplet shopTabTemplet = CurrentTabTemplet();
		component.ActivateSelection(shopTabTemplet.m_MultiBuy);
		if (m_eCurrentTab == "TAB_SUPPLY")
		{
			NKMShopRandomData randomShop = NKCScenManager.GetScenManager().GetMyUserData().m_ShopData.randomShop;
			int num = idx + 1;
			if (randomShop.datas.ContainsKey(num))
			{
				NKMShopRandomListData shopRandomTemplet = randomShop.datas[num];
				bool bValue = component.SetData(this, shopRandomTemplet, num);
				NKCUtil.SetGameobjectActive(component, bValue);
				if (shopTabTemplet.m_MultiBuy)
				{
					component.SetSelection(m_hsMultiBuy.Contains(num));
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(component, bValue: false);
			}
		}
		else if (m_lstFeatured.Count > 0)
		{
			NKCShopFeaturedTemplet nKCShopFeaturedTemplet = m_lstFeatured[idx];
			ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(nKCShopFeaturedTemplet.m_PackageID);
			component.SetOverrideImageAsset(nKCShopFeaturedTemplet.m_FeaturedImage);
			bool bValue2 = component.SetData(this, shopItemTemplet, NKCShopManager.GetBuyCountLeft(shopItemTemplet.m_ProductID), IsFirstBuy(shopItemTemplet.m_ProductID));
			NKCUtil.SetGameobjectActive(component, bValue2);
		}
		else if (m_ssProduct != null)
		{
			if (idx < m_ssProduct.SortedProductList.Count)
			{
				ShopItemTemplet shopItemTemplet2 = m_ssProduct.SortedProductList[idx];
				bool bValue3 = component.SetData(this, shopItemTemplet2, NKCShopManager.GetBuyCountLeft(shopItemTemplet2.m_ProductID), IsFirstBuy(shopItemTemplet2.m_ProductID));
				NKCUtil.SetGameobjectActive(component, bValue3);
				if (shopTabTemplet.m_MultiBuy)
				{
					component.SetSelection(m_hsMultiBuy.Contains(shopItemTemplet2.m_ProductID));
				}
			}
		}
		else if (m_lstShopItem != null && idx < m_lstShopItem.Count)
		{
			ShopItemTemplet shopItemTemplet3 = m_lstShopItem[idx];
			bool bValue4 = component.SetData(this, shopItemTemplet3, NKCShopManager.GetBuyCountLeft(shopItemTemplet3.m_ProductID), IsFirstBuy(shopItemTemplet3.m_ProductID));
			NKCUtil.SetGameobjectActive(component, bValue4);
			if (shopTabTemplet.m_MultiBuy)
			{
				component.SetSelection(m_hsMultiBuy.Contains(shopItemTemplet3.m_ProductID));
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
		}
	}

	public void Init()
	{
		m_dicLoopScrollSet.Clear();
		foreach (DisplaySet displaySet in m_lstDisplaySet)
		{
			m_dicLoopScrollSet.Add(displaySet.slotType, displaySet);
			LoopScrollRect scrollRect = displaySet.scrollRect;
			if (scrollRect != null)
			{
				scrollRect.dOnGetObject += (int index) => GetShopSlot(displaySet.slotType, index);
				scrollRect.dOnReturnObject += delegate(Transform tr)
				{
					ReturnShopSlot(displaySet.slotType, tr);
				};
				scrollRect.dOnProvideData += ProvideShopSlotData;
				scrollRect.PrepareCells();
				NKCUtil.SetGameobjectActive(scrollRect, bValue: false);
				if (displaySet.slotType != ShopDisplayType.Main)
				{
					NKCUtil.SetScrollHotKey(scrollRect);
				}
			}
		}
		m_dicFullDisplaySet.Clear();
		foreach (FullDisplaySet fulldisplaySet in m_lstFullDisplaySet)
		{
			m_dicFullDisplaySet.Add(fulldisplaySet.slotType, fulldisplaySet);
			NKCUIComDragSelectablePanel draggablePanel = fulldisplaySet.draggablePanel;
			if (draggablePanel != null)
			{
				draggablePanel.Init();
				draggablePanel.dOnGetObject += () => GetShopSlot(fulldisplaySet.slotType, 0);
				draggablePanel.dOnReturnObject += delegate(RectTransform tr)
				{
					ReturnShopSlot(fulldisplaySet.slotType, tr);
				};
				draggablePanel.dOnProvideData += ProvideFullDisplayData;
				NKCUtil.SetGameobjectActive(draggablePanel, bValue: false);
			}
		}
		if (m_CommonFullDisplayDragSelectPanel != null)
		{
			m_CommonFullDisplayDragSelectPanel.Init();
			m_CommonFullDisplayDragSelectPanel.dOnGetObject += () => GetShopSlot(m_eDisplayType, 0);
			m_CommonFullDisplayDragSelectPanel.dOnReturnObject += ReturnCommonFullDisplayShopSlot;
			m_CommonFullDisplayDragSelectPanel.dOnProvideData += ProvideFullDisplayData;
		}
		NKCUtil.SetButtonClickDelegate(m_cbtnSupplyRefresh, OnBtnSupplyRefresh);
		NKCUtil.SetHotkey(m_cbtnSupplyRefresh, HotkeyEventType.Plus);
		NKCUtil.SetButtonClickDelegate(m_csbtnMultiBuy, OnBtnMultiBuy);
		NKCUtil.SetHotkey(m_csbtnMultiBuy, HotkeyEventType.Confirm);
		NKCUtil.SetButtonClickDelegate(m_csbtnMultiBuyClear, OnBtnMultiBuyClear);
		m_UINPCShop?.Init();
		if (m_HomePackageSlidePanel != null)
		{
			m_HomePackageSlidePanel.Init(rotation: true);
			m_HomePackageSlidePanel.dOnGetObject += GetRecommendObject;
			m_HomePackageSlidePanel.dOnReturnObject += ReturnRecommendObject;
			m_HomePackageSlidePanel.dOnProvideData += ProvideRecommendData;
			m_HomePackageSlidePanel.dOnFocus += OnRecommendFocus;
		}
		NKCUtil.SetToggleValueChangedDelegate(m_tglChain_01, OnChainTab_01);
		NKCUtil.SetToggleValueChangedDelegate(m_tglChain_02, OnChainTab_02);
		NKCUtil.SetToggleValueChangedDelegate(m_tglChain_03, OnChainTab_03);
		NKCUtil.SetButtonClickDelegate(m_btnBuyAll, OnBtnBuyAll);
		NKCUtil.SetButtonClickDelegate(m_csbtnChangeCategory, OnChangeCategory);
		NKCUtil.SetButtonClickDelegate(m_csbtnFilter, OnBtnFilter);
		NKCUtil.SetButtonClickDelegate(m_csbtnFilterActive, OnBtnFilter);
		m_uiShopCategoryChange?.Init(_ChangeCategory);
		NKCUtil.SetGameobjectActive(m_uiShopCategoryChange, bValue: false);
		NKCUtil.SetButtonClickDelegate(m_csbtnOperatorSubSkillPool, OnClickOperatorSubSkillPool);
		SetJPNPolicyTabUI();
	}

	public void Open(NKCShopManager.ShopTabCategory category, string selectedTab = "TAB_MAIN", int subTabIndex = 0, int reservedProductID = 0, eTabMode tabMode = eTabMode.Fold)
	{
		if (NKCShopCategoryTemplet.Find(category) == null)
		{
			foreach (NKCShopManager.ShopTabCategory value in Enum.GetValues(typeof(NKCShopManager.ShopTabCategory)))
			{
				if (NKCShopCategoryTemplet.Find(value) != null)
				{
					category = value;
				}
			}
		}
		base.gameObject.SetActive(value: true);
		m_eTabMode = tabMode;
		NKCSoundManager.PlayScenMusic(NKM_SCEN_ID.NSI_SHOP);
		m_bPlayedScenMusic = true;
		SetOffAllObjects();
		NKCUtil.SetGameobjectActive(m_objFetchItem, bValue: true);
		NKCShopManager.FetchShopItemList(NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID, delegate(bool bSuccess)
		{
			if (bSuccess)
			{
				OpenProcess(category, selectedTab, subTabIndex, reservedProductID, tabMode);
			}
			else
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_SHOP_WAS_NOT_ABLE_TO_GET_PRODUCT_LIST_FROM_SERVER, base.Close);
			}
		});
	}

	private void SetOffAllObjects()
	{
		foreach (DisplaySet item in GetAllDisplaySet())
		{
			NKCUtil.SetGameobjectActive(item.m_rtParent, bValue: false);
			NKCUtil.SetGameobjectActive(item.scrollRect, bValue: false);
		}
		foreach (FullDisplaySet item2 in GetAllFullDisplaySet())
		{
			NKCUtil.SetGameobjectActive(item2.m_rtParent, bValue: false);
			NKCUtil.SetGameobjectActive(item2.draggablePanel, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_lbSupplyTimeLeft, bValue: false);
		NKCUtil.SetGameobjectActive(m_cbtnSupplyRefresh, bValue: false);
		NKCUtil.SetGameobjectActive(m_lbSupplyCountLeft, bValue: false);
		NKCUtil.SetGameobjectActive(m_objTabResetTime, bValue: false);
		NKCUtil.SetGameobjectActive(m_objChainTab, bValue: false);
		NKCUtil.SetGameobjectActive(m_objTopBanner, bValue: false);
		NKCUtil.SetGameobjectActive(m_objEmptyList, bValue: false);
		NKCUtil.SetGameobjectActive(m_objChainLocked, bValue: false);
		NKCUtil.SetGameobjectActive(m_btnBuyAll, bValue: false);
		NKCUtil.SetGameobjectActive(m_csbtnFilter, bValue: false);
		NKCUtil.SetGameobjectActive(m_csbtnFilterActive, bValue: false);
		NKCUtil.SetGameobjectActive(m_uiShopCategoryChange, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCategoryReddot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objMultiBuy, bValue: false);
	}

	private void OpenProcess(NKCShopManager.ShopTabCategory category, string selectedTab, int subTabIndex, int reservedProductID, eTabMode tabMode)
	{
		NKCUtil.SetGameobjectActive(m_objFetchItem, bValue: false);
		BuildProductList(bForce: false);
		BuildTabs(category, selectedTab, subTabIndex, bForceRebuildTabs: true);
		NKCUtil.SetLabelText(m_lbSupplyRefreshCost, 15.ToString());
		base.gameObject.SetActive(value: true);
		UIOpened();
		playingTutorial = TutorialCheck();
		if (!playingTutorial)
		{
			if (ShopItemTemplet.Find(reservedProductID) != null)
			{
				NKCShopManager.OnBtnProductBuy(reservedProductID, bSupply: false);
			}
			else
			{
				m_UINPCShop?.PlayAni(NPC_ACTION_TYPE.START);
			}
		}
	}

	private void BuildTabs(NKCShopManager.ShopTabCategory category, string selectedTab, int subTabIndex, bool bForceRebuildTabs = false)
	{
		if (m_eCategory != category || bForceRebuildTabs)
		{
			CleanupTab();
		}
		m_eCategory = category;
		List<ShopTabTemplet> useTabList = NKCShopManager.GetUseTabList(m_eCategory);
		if (m_dicTab.Count == 0)
		{
			BuildTabs(useTabList);
		}
		if (!string.IsNullOrEmpty(selectedTab) && selectedTab != "TAB_NONE" && !useTabList.Exists((ShopTabTemplet x) => x.TabType == selectedTab))
		{
			Debug.LogError($"Tab {selectedTab} does not exist in category {category}!");
			selectedTab = "TAB_NONE";
		}
		if (selectedTab == "TAB_NONE")
		{
			if (m_lstEnabledTabs != null && m_lstEnabledTabs.Count > 0)
			{
				selectedTab = m_lstEnabledTabs[0].TabType;
			}
			else if (useTabList.Count > 0)
			{
				selectedTab = useTabList[0].TabType;
			}
		}
		SelectTab(selectedTab, subTabIndex, bForce: true, bAnimate: false);
	}

	private void SetJPNPolicyTabUI()
	{
		if (!(m_srTab != null))
		{
			return;
		}
		RectTransform component = m_srTab.GetComponent<RectTransform>();
		if (component != null)
		{
			if (NKCPublisherModule.InAppPurchase.ShowJPNPaymentPolicy())
			{
				NKCUtil.SetButtonClickDelegate(m_csbtnJPNPaymentLaw, OnBtnJPNPaymentLaw);
				NKCUtil.SetButtonClickDelegate(m_csbtnJPNCommecialLaw, OnBtnJPNCommercialLaw);
				component.offsetMin = new Vector2(component.offsetMin.x, 220f);
				NKCUtil.SetGameobjectActive(m_objJPNPolicy, bValue: true);
			}
			else
			{
				component.offsetMin = new Vector2(component.offsetMin.x, 116f);
				NKCUtil.SetGameobjectActive(m_objJPNPolicy, bValue: false);
			}
		}
	}

	public override void Hide()
	{
		base.Hide();
		NKCInputManager.ClearIgnoreKey();
	}

	public override void UnHide()
	{
		base.UnHide();
		if (playingTutorial)
		{
			m_UINPCShop?.PlayAni(NPC_ACTION_TYPE.START);
			playingTutorial = false;
		}
		SetIgnoreHotkey();
	}

	private void Update()
	{
		if (m_eCurrentTab == "TAB_SUPPLY")
		{
			updateTimer += Time.deltaTime;
			if (UPDATE_INTERVAL < updateTimer)
			{
				updateTimer = 0f;
				if (NKCSynchronizedTime.IsFinished(m_NextSupplyUpdateTick))
				{
					TrySupplyRefresh(useCash: false);
				}
				else
				{
					UpdateRefreshTimer(m_NextSupplyUpdateTick);
				}
			}
			ProcessSupplyHotkey();
		}
		else if (m_CurrentChainIndex > 0)
		{
			updateTimer += Time.deltaTime;
			if (UPDATE_INTERVAL < updateTimer)
			{
				updateTimer = 0f;
				if (NKCSynchronizedTime.IsFinished(m_NextChainUpdateTick))
				{
					TryChainRefresh();
				}
				else
				{
					UpdateChainRefreshTimer(m_NextChainUpdateTick);
				}
			}
		}
		else
		{
			if (!m_bUseTabEndTimer)
			{
				return;
			}
			updateTimer += Time.deltaTime;
			if (UPDATE_INTERVAL < updateTimer)
			{
				updateTimer = 0f;
				UpdateTabEndTimer(m_TabEndTick);
				if (NKCSynchronizedTime.IsFinished(m_TabEndTick))
				{
					m_bUseTabEndTimer = false;
					RefreshCurrentTab();
				}
			}
		}
	}

	private void UpdateRefreshTimer(long endTick)
	{
		NKCUtil.SetLabelText(m_lbSupplyTimeLeft, string.Format(NKCUtilString.GET_STRING_SHOP_NEXT_REFRESH_ONE_PARAM, NKCSynchronizedTime.GetTimeLeftString(endTick)));
	}

	private void UpdateChainRefreshTimer(long endTick)
	{
		string arg = "<color=#FFDF5D>" + NKCUtilString.GetRemainTimeString(new DateTime(endTick), 3) + "</color>";
		NKCUtil.SetLabelText(m_lbTabRemainTime, string.Format(NKCUtilString.GET_STRING_SHOP_CHAIN_NEXT_RESET_ONE_PARAM, arg));
	}

	private void UpdateTabEndTimer(long endTick)
	{
		string empty = string.Empty;
		empty = ((NKCSynchronizedTime.GetTimeLeft(endTick).Days >= 1) ? NKCUtilString.GetRemainTimeString(new DateTime(endTick), 2) : ("<color=#FF0000>" + NKCUtilString.GetRemainTimeString(new DateTime(endTick), 2) + "</color>"));
		NKCUtil.SetLabelText(m_lbTabRemainTime, string.Format(NKCUtilString.GET_STRING_SHOP_CHAIN_NEXT_RESET_ONE_PARAM_CLOSE, empty));
	}

	protected void BuildProductList(bool bForce)
	{
		if ((!bForce && ShopItemUpdateTimeStamp >= NKCShopManager.ShopItemUpdatedTimestamp) || NKCShopManager.ShopItemList == null || NKCScenManager.CurrentUserData() == null)
		{
			return;
		}
		m_dicProducts = new Dictionary<ShopTabTemplet, List<ShopItemTemplet>>();
		foreach (int shopItem in NKCShopManager.ShopItemList)
		{
			ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(shopItem);
			if (shopItemTemplet == null)
			{
				Debug.LogError("Product Templet null! ID : " + shopItem);
			}
			else
			{
				if (!NKCShopManager.CanExhibitItem(shopItemTemplet, bIncludeLockedItemWithReason: true))
				{
					continue;
				}
				ShopTabTemplet shopTabTemplet = ShopTabTemplet.Find(shopItemTemplet.m_TabID, shopItemTemplet.m_TabSubIndex);
				if (shopTabTemplet != null)
				{
					if (m_dicProducts.ContainsKey(shopTabTemplet))
					{
						m_dicProducts[shopTabTemplet].Add(shopItemTemplet);
						continue;
					}
					List<ShopItemTemplet> list = new List<ShopItemTemplet>();
					list.Add(shopItemTemplet);
					m_dicProducts.Add(shopTabTemplet, list);
				}
			}
		}
		ShopItemUpdateTimeStamp = NKCShopManager.ShopItemUpdatedTimestamp;
	}

	public void ForceUpdateItemList()
	{
		BuildProductList(bForce: true);
		SelectTab(m_eCurrentTab, m_CurrentSubIndex, bForce: true);
	}

	public void ClearMultibuySelection()
	{
		m_hsMultiBuy.Clear();
	}

	public void RefreshRandomShopItem(int slotIndex)
	{
		if (!(m_eCurrentTab != "TAB_SUPPLY"))
		{
			if (NKCScenManager.GetScenManager().GetMyUserData().m_ShopData.randomShop == null)
			{
				SelectTab("TAB_SUPPLY", 0, bForce: true);
			}
			else
			{
				GetLoopScrollRect(GetDisplayType(m_eCurrentTab, m_CurrentSubIndex))?.RefreshCells();
			}
		}
	}

	public void RefreshShopItem(int shop_id)
	{
		ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(shop_id);
		if (shopItemTemplet == null)
		{
			return;
		}
		string eCurrentTab = m_eCurrentTab;
		if (eCurrentTab != null && eCurrentTab == "TAB_SUPPLY")
		{
			return;
		}
		ShopDisplayType displayType = GetDisplayType(m_eCurrentTab, m_CurrentSubIndex);
		if (displayType == ShopDisplayType.Custom)
		{
			ForceUpdateItemList();
			return;
		}
		if (shopItemTemplet.m_PriceItemID == 0)
		{
			ForceUpdateItemList();
			return;
		}
		if (NKCShopManager.GetBuyCountLeft(shopItemTemplet.m_ProductID) == 0 && NKCShopManager.HasLinkedItem(shopItemTemplet.m_ProductID))
		{
			ForceUpdateItemList();
			return;
		}
		ShopTabTemplet shopTabTemplet = ShopTabTemplet.Find(m_eCurrentTab, m_CurrentSubIndex);
		if (shopTabTemplet != null)
		{
			if (shopTabTemplet.IsChainTab)
			{
				if (NKCShopManager.GetCurrentTargetChainIndex(ShopTabTemplet.Find(m_eCurrentTab, m_CurrentSubIndex)) != m_CurrentChainIndex)
				{
					SelectTab(m_eCurrentTab, m_CurrentSubIndex, bForce: true);
					return;
				}
			}
			else if (shopTabTemplet.IsBundleTab)
			{
				UpdateBuyAllBtn(shopTabTemplet);
			}
		}
		List<NKCUIShopSlotBase> slotList = GetSlotList(displayType);
		if (slotList == null)
		{
			return;
		}
		foreach (NKCUIShopSlotBase item in slotList)
		{
			if (item.gameObject.activeSelf && item.ProductID == shop_id && !item.SetData(this, shopItemTemplet, NKCShopManager.GetBuyCountLeft(item.ProductID), IsFirstBuy(item.ProductID)))
			{
				NKCUtil.SetGameobjectActive(item, bValue: false);
			}
		}
	}

	public void RefreshShopRedDot()
	{
		foreach (KeyValuePair<string, NKCUIShopTab> item in m_dicTab)
		{
			item.Value.SetRedDot();
		}
	}

	public void RefreshHomePackageBanner()
	{
		m_lstHomeBannerTemplet = NKCShopManager.GetHomeBannerTemplet();
		if (m_HomePackageSlidePanel != null && m_lstHomeBannerTemplet.Count > 0)
		{
			m_HomePackageSlidePanel.TotalCount = m_lstHomeBannerTemplet.Count;
			m_HomePackageSlidePanel.SetIndex(0);
		}
	}

	private void _SetSlotPoolSize(ref List<NKCUIShopSlotBase> lstSlot, NKCUIShopSlotBase pfbNewSlot, Transform parent, int count)
	{
		int num = count - lstSlot.Count;
		for (int i = 0; i < num; i++)
		{
			NKCUIShopSlotBase nKCUIShopSlotBase = UnityEngine.Object.Instantiate(pfbNewSlot);
			nKCUIShopSlotBase.Init(OnBtnProductBuy, ForceUpdateItemList);
			nKCUIShopSlotBase.transform.SetParent(parent, worldPositionStays: false);
			nKCUIShopSlotBase.transform.localPosition = Vector3.zero;
			lstSlot.Add(nKCUIShopSlotBase);
			NKCUtil.SetGameobjectActive(nKCUIShopSlotBase, bValue: false);
		}
	}

	protected void BuildTabs(List<string> lstUseTab)
	{
		if (lstUseTab == null)
		{
			BuildTabs();
			return;
		}
		List<ShopTabTemplet> list = new List<ShopTabTemplet>();
		foreach (string item in lstUseTab)
		{
			IEnumerable<ShopTabTemplet> allSubtabs = ShopTabTempletContainer.GetAllSubtabs(item);
			if (allSubtabs == null)
			{
				continue;
			}
			foreach (ShopTabTemplet item2 in allSubtabs)
			{
				if (item2 != null)
				{
					list.Add(item2);
				}
			}
		}
		BuildTabs(list);
	}

	protected void BuildTabs(IEnumerable<ShopTabTemplet> lstShopTabTemplet = null)
	{
		switch (m_eTabMode)
		{
		case eTabMode.All:
			BuildAllTabs(lstShopTabTemplet);
			break;
		case eTabMode.Fold:
			BuildFoldTabs(lstShopTabTemplet);
			break;
		}
	}

	protected void CleanupTab()
	{
		foreach (KeyValuePair<string, NKCUIShopTab> item in m_dicTab)
		{
			item.Value.Clear();
			UnityEngine.Object.Destroy(item.Value.gameObject);
		}
		m_dicTab.Clear();
	}

	protected void BuildFoldTabs(IEnumerable<ShopTabTemplet> lstShopTabTemplet = null)
	{
		if (lstShopTabTemplet == null)
		{
			lstShopTabTemplet = ShopTabTemplet.Values;
		}
		m_lstEnabledTabs.Clear();
		Debug.Log("Shop Fold Tab Building");
		foreach (ShopTabTemplet item in lstShopTabTemplet)
		{
			if ((!string.IsNullOrEmpty(item.intervalId) && !NKCSynchronizedTime.IsEventTime(item.intervalId, item.EventDateStartUtc, item.EventDateEndUtc)) || (UseTabVisible && !item.m_Visible) || !item.EnableByTag)
			{
				continue;
			}
			if (GetUIShopTab(item) == null)
			{
				if (item.SubIndex == 0)
				{
					if (item.m_ShopDisplay != ShopDisplayType.None || NKCShopManager.CanDisplayTab(item.TabType, UseTabVisible))
					{
						NKCUIShopTab nKCUIShopTab = UnityEngine.Object.Instantiate(m_pfbTab);
						nKCUIShopTab.SetData(item, onSelectTab, m_tgTabGroup);
						nKCUIShopTab.transform.SetParent(m_trTabRoot, worldPositionStays: false);
						nKCUIShopTab.transform.localPosition = Vector3.zero;
						m_dicTab.Add(MakeTabUIKey(item.TabType, 0), nKCUIShopTab);
					}
				}
				else
				{
					Log.Error("ShopTabTemplet : SubIndex must start with 0 - TabType : " + item.TabType, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Shop/NKCUIShop.cs", 1307);
				}
			}
			else
			{
				NKCUIShopTabSlot nKCUIShopTabSlot = UnityEngine.Object.Instantiate(m_pfbTabSubSlot);
				nKCUIShopTabSlot.SetData(item, onSelectTab);
				nKCUIShopTabSlot.transform.SetParent(m_trTabRoot, worldPositionStays: false);
				nKCUIShopTabSlot.transform.localPosition = Vector3.zero;
				GetUIShopTab(item).AddSubSlot(nKCUIShopTabSlot);
			}
		}
		foreach (ShopTabTemplet item2 in lstShopTabTemplet)
		{
			string tabType = item2.TabType;
			if (!m_dicTab.TryGetValue(MakeTabUIKey(tabType, 0), out var value))
			{
				continue;
			}
			if (item2.m_HideWhenSoldOut)
			{
				if (item2.SubIndex == 0 && value.HideTabRequired())
				{
					NKCUtil.SetGameobjectActive(value, bValue: false);
					continue;
				}
				if (!string.IsNullOrEmpty(item2.m_PackageGroupID))
				{
					if (value.HideTabRequired())
					{
						NKCUtil.SetGameobjectActive(value, bValue: false);
						continue;
					}
				}
				else
				{
					if (!m_dicProducts.ContainsKey(item2))
					{
						NKCUtil.SetGameobjectActive(value.GetSubSlotObject(item2.SubIndex), bValue: false);
						continue;
					}
					GameObject subSlotObject = value.GetSubSlotObject(item2.SubIndex);
					NKCUIShopTabSlot nKCUIShopTabSlot2 = ((subSlotObject != null) ? subSlotObject.GetComponent<NKCUIShopTabSlot>() : null);
					if (nKCUIShopTabSlot2 != null && nKCUIShopTabSlot2.HideTabRequired())
					{
						NKCUtil.SetGameobjectActive(value.GetSubSlotObject(item2.SubIndex), bValue: false);
						continue;
					}
				}
			}
			if (!NKCSynchronizedTime.IsEventTime(item2.intervalId, item2.EventDateStartUtc, item2.EventDateEndUtc))
			{
				NKCUtil.SetGameobjectActive(value.GetSubSlotObject(item2.SubIndex), bValue: false);
				continue;
			}
			if (UseTabVisible && !item2.m_Visible)
			{
				NKCUtil.SetGameobjectActive(value.GetSubSlotObject(item2.SubIndex), bValue: false);
				continue;
			}
			if (!item2.EnableByTag)
			{
				NKCUtil.SetGameobjectActive(value.GetSubSlotObject(item2.SubIndex), bValue: false);
				continue;
			}
			if (item2.m_ShopDisplay != ShopDisplayType.None)
			{
				m_lstEnabledTabs.Add(item2);
			}
			NKCUtil.SetGameobjectActive(value, bValue: true);
		}
		RefreshShopRedDot();
	}

	protected void BuildAllTabs(IEnumerable<ShopTabTemplet> lstShopTabTemplet = null)
	{
		if (lstShopTabTemplet == null)
		{
			lstShopTabTemplet = ShopTabTemplet.Values;
		}
		m_lstEnabledTabs.Clear();
		Debug.Log("Shop All Tab Building");
		foreach (ShopTabTemplet item in lstShopTabTemplet)
		{
			if ((string.IsNullOrEmpty(item.intervalId) || NKCSynchronizedTime.IsEventTime(item.intervalId, item.EventDateStartUtc, item.EventDateEndUtc)) && (!UseTabVisible || item.m_Visible) && item.EnableByTag && GetUIShopTab(item) == null)
			{
				NKCUIShopTab nKCUIShopTab = UnityEngine.Object.Instantiate(m_pfbTab);
				nKCUIShopTab.SetData(item, onSelectTab, m_tgTabGroup);
				nKCUIShopTab.transform.SetParent(m_trTabRoot, worldPositionStays: false);
				nKCUIShopTab.transform.localPosition = Vector3.zero;
				m_dicTab.Add(MakeTabUIKey(item), nKCUIShopTab);
			}
		}
		foreach (ShopTabTemplet item2 in lstShopTabTemplet)
		{
			string text = MakeTabUIKey(item2);
			if (text == null || !m_dicTab.TryGetValue(text, out var value))
			{
				continue;
			}
			int count = NKCShopManager.GetItemTempletListByTab(item2, bIncludeLockedItemWithReason: true).Count;
			if (item2.SubIndex == 0 && count == 0)
			{
				IEnumerable<ShopTabTemplet> allSubtabs = ShopTabTempletContainer.GetAllSubtabs(item2.TabType);
				if (allSubtabs != null)
				{
					bool flag = false;
					foreach (ShopTabTemplet item3 in allSubtabs)
					{
						if (item3.SubIndex != 0)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						NKCUtil.SetGameobjectActive(value, bValue: false);
						continue;
					}
				}
			}
			if (item2.m_HideWhenSoldOut)
			{
				if (!m_dicProducts.ContainsKey(item2))
				{
					NKCUtil.SetGameobjectActive(value, bValue: false);
					continue;
				}
				if (m_dicProducts[item2].Count == 0)
				{
					NKCUtil.SetGameobjectActive(value, bValue: false);
					continue;
				}
				if (count == 0)
				{
					NKCUtil.SetGameobjectActive(value, bValue: false);
					continue;
				}
				if (NKCShopManager.IsTabSoldOut(item2) && !NKCUtil.IsUsingSuperUserFunction())
				{
					NKCUtil.SetGameobjectActive(value, bValue: false);
					continue;
				}
			}
			if (!string.IsNullOrEmpty(item2.intervalId) && !NKCSynchronizedTime.IsEventTime(item2.intervalId, item2.EventDateStartUtc, item2.EventDateEndUtc))
			{
				NKCUtil.SetGameobjectActive(value, bValue: false);
				continue;
			}
			if (UseTabVisible && !item2.m_Visible)
			{
				NKCUtil.SetGameobjectActive(value, bValue: false);
				continue;
			}
			if (!item2.EnableByTag)
			{
				NKCUtil.SetGameobjectActive(value, bValue: false);
				continue;
			}
			m_lstEnabledTabs.Add(item2);
			NKCUtil.SetGameobjectActive(value, bValue: true);
		}
		RefreshShopRedDot();
	}

	protected void onSelectTab(string targetTab, int subIndex = 0)
	{
		SelectTab(targetTab, subIndex);
		if (targetTab != "TAB_MAIN")
		{
			NKCUIManager.NKCUIOverlayCaption.CloseAllCaption();
		}
	}

	public void RefreshCurrentTab()
	{
		SelectTab(m_eCurrentTab, m_CurrentSubIndex, bForce: true);
	}

	public void SelectTab(string targetTab, int targetSubTabIndex = 0, bool bForce = false, bool bAnimate = true)
	{
		ShopTabTemplet shopTabTemplet = ShopTabTemplet.Find(targetTab, targetSubTabIndex);
		if (shopTabTemplet == null)
		{
			Debug.LogError("ShopTemplet for " + targetTab + " not exist. fallback to main tab!");
			targetTab = "TAB_MAIN";
			shopTabTemplet = ShopTabTemplet.Find("TAB_MAIN", 0);
			if (shopTabTemplet == null)
			{
				Debug.LogError("MainTab is null - tabType : TAB_MAIN, tabIndex = 0");
				return;
			}
		}
		m_hsMultiBuy.Clear();
		NKCUIShopSkinPopup.CheckInstanceAndClose();
		if ((targetTab != "TAB_MAIN" && shopTabTemplet.m_ShopDisplay == ShopDisplayType.None) || (shopTabTemplet.HasDateLimit && !NKCSynchronizedTime.IsEventTime(shopTabTemplet.intervalId, shopTabTemplet.EventDateStartUtc, shopTabTemplet.EventDateEndUtc)))
		{
			bool flag = false;
			int num = int.MaxValue;
			foreach (ShopTabTemplet value in ShopTabTempletContainer.Values)
			{
				if (!(value.TabType == targetTab))
				{
					continue;
				}
				NKCUIShopTab uIShopTab = GetUIShopTab(shopTabTemplet);
				if (uIShopTab == null || (value.SubIndex == 0 && shopTabTemplet.m_ShopDisplay == ShopDisplayType.None))
				{
					continue;
				}
				if (m_eTabMode == eTabMode.Fold)
				{
					if (value.HasDateLimit && !NKCSynchronizedTime.IsEventTime(value.intervalId, value.EventDateStartUtc, value.EventDateEndUtc))
					{
						NKCUtil.SetGameobjectActive(uIShopTab.GetSubSlotObject(value.SubIndex), bValue: false);
						continue;
					}
					if (value.m_HideWhenSoldOut)
					{
						GameObject subSlotObject = uIShopTab.GetSubSlotObject(value.SubIndex);
						NKCUIShopTabSlot nKCUIShopTabSlot = ((subSlotObject != null) ? uIShopTab.GetSubSlotObject(value.SubIndex).GetComponent<NKCUIShopTabSlot>() : null);
						if (nKCUIShopTabSlot != null && nKCUIShopTabSlot.HideTabRequired())
						{
							NKCUtil.SetGameobjectActive(subSlotObject, bValue: false);
							continue;
						}
					}
				}
				if (value.SubIndex < num)
				{
					num = value.SubIndex;
				}
				flag = true;
			}
			if (!flag)
			{
				NKCUtil.SetGameobjectActive(GetUIShopTab(shopTabTemplet), bValue: false);
				NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NKE_FAIL_SHOP_NOT_EVENT_TIME, delegate
				{
					SelectTab("TAB_MAIN");
				});
			}
			else
			{
				SelectTab(targetTab, num, bForce, bAnimate);
			}
			return;
		}
		if (targetTab == "TAB_SUPPLY")
		{
			NKMShopRandomData randomShop = NKCScenManager.GetScenManager().GetMyUserData().m_ShopData.randomShop;
			if (randomShop == null || randomShop.datas.Count == 0 || NKCSynchronizedTime.IsFinished(randomShop.nextRefreshDate))
			{
				TrySupplyRefresh(useCash: false);
				return;
			}
			m_NextSupplyUpdateTick = randomShop.nextRefreshDate;
			NKCUtil.SetGameobjectActive(m_lbSupplyTimeLeft, bValue: true);
			NKCUtil.SetGameobjectActive(m_cbtnSupplyRefresh, bValue: true);
			NKCUtil.SetGameobjectActive(m_lbSupplyCountLeft, bValue: true);
			NKCUtil.SetLabelText(m_lbSupplyCountLeft, string.Format(NKCUtilString.GET_STRING_SHOP_REMAIN_NUMBER_TWO_PARAM, randomShop.refreshCount, 5));
			UpdateRefreshTimer(randomShop.nextRefreshDate);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_lbSupplyTimeLeft, bValue: false);
			NKCUtil.SetGameobjectActive(m_cbtnSupplyRefresh, bValue: false);
			NKCUtil.SetGameobjectActive(m_lbSupplyCountLeft, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objMultiBuy, shopTabTemplet.m_MultiBuy);
		if (!bForce && m_eCurrentTab == targetTab && m_CurrentSubIndex == targetSubTabIndex)
		{
			return;
		}
		if (m_eCurrentTab != targetTab || m_CurrentSubIndex != targetSubTabIndex)
		{
			NKCShopManager.SetLastCheckedUTCTime(m_eCurrentTab, m_CurrentSubIndex);
		}
		m_eCurrentTab = targetTab;
		m_CurrentSubIndex = targetSubTabIndex;
		m_eDisplayType = shopTabTemplet.m_ShopDisplay;
		if (GetDisplayType(targetTab, targetSubTabIndex) == ShopDisplayType.Main)
		{
			RefreshHomePackageBanner();
		}
		NKCUtil.SetGameobjectActive(m_objTabResetTime, shopTabTemplet.IsCountResetType || shopTabTemplet.HasDateLimit);
		NKCUtil.SetGameobjectActive(m_objChainTab, shopTabTemplet.IsChainTab);
		if (shopTabTemplet.IsChainTab)
		{
			NKMShopData shopData = NKCScenManager.CurrentUserData().m_ShopData;
			long val = (shopTabTemplet.HasDateLimit ? shopTabTemplet.EventDateEndUtc.Ticks : DateTime.MaxValue.Ticks);
			long ticks = shopData.GetChainTabResetTime(targetTab, targetSubTabIndex).Ticks;
			m_NextChainUpdateTick = Math.Min(val, ticks);
			if (m_NextChainUpdateTick < NKCSynchronizedTime.GetServerUTCTime().Ticks)
			{
				TryChainRefresh();
				return;
			}
			if (m_NextChainUpdateTick == 0L)
			{
				m_NextChainUpdateTick = DateTime.MaxValue.Ticks;
			}
			m_CurrentChainIndex = NKCShopManager.GetCurrentTargetChainIndex(shopTabTemplet);
			switch (m_CurrentChainIndex)
			{
			case 1:
				m_tglChain_01.Select(bSelect: true, bForce: true, bImmediate: true);
				break;
			case 2:
				m_tglChain_02.Select(bSelect: true, bForce: true, bImmediate: true);
				break;
			case 3:
				m_tglChain_03.Select(bSelect: true, bForce: true, bImmediate: true);
				break;
			}
			UpdateChainRefreshTimer(m_NextChainUpdateTick);
			foreach (DisplaySet item in GetAllDisplaySet())
			{
				RectTransform rectTransform = item.scrollRect?.GetComponent<RectTransform>();
				if (rectTransform != null)
				{
					rectTransform.GetComponent<RectTransform>().offsetMin = new Vector2(m_fChainTabOffsetX, rectTransform.offsetMin.y);
				}
			}
			m_bUseTabEndTimer = false;
			m_TabEndTick = 0L;
		}
		else
		{
			m_CurrentChainIndex = 0;
			foreach (DisplaySet item2 in GetAllDisplaySet())
			{
				RectTransform rectTransform2 = item2.scrollRect?.GetComponent<RectTransform>();
				if (rectTransform2 != null)
				{
					rectTransform2.GetComponent<RectTransform>().offsetMin = new Vector2(0f, rectTransform2.offsetMin.y);
				}
			}
			m_bUseTabEndTimer = shopTabTemplet.HasDateLimit;
			if (m_bUseTabEndTimer)
			{
				m_TabEndTick = shopTabTemplet.EventDateEndUtc.Ticks;
				UpdateTabEndTimer(shopTabTemplet.EventDateEndUtc.Ticks);
			}
			else
			{
				m_TabEndTick = 0L;
			}
		}
		ShowItemList(targetTab, targetSubTabIndex);
		NKCUtil.SetGameobjectActive(m_objTopBanner, !string.IsNullOrEmpty(shopTabTemplet.m_TopBannerText));
		if (!string.IsNullOrEmpty(shopTabTemplet.m_TopBannerText))
		{
			NKCUtil.SetLabelText(m_lbTopBanner, NKCStringTable.GetString(shopTabTemplet.m_TopBannerText));
		}
		UpdateBuyAllBtn(shopTabTemplet);
		RESOURCE_LIST = SetResourceList(shopTabTemplet);
		NKCUIManager.UpdateUpsideMenu();
		if (m_eCurrentTab != targetTab)
		{
			m_UINPCShop?.PlayAni(GetNPCActionType(targetTab));
		}
		switch (m_eTabMode)
		{
		case eTabMode.Fold:
			foreach (KeyValuePair<string, NKCUIShopTab> item3 in m_dicTab)
			{
				item3.Value.SelectSubSlot(targetTab, targetSubTabIndex, bAnimate);
			}
			break;
		case eTabMode.All:
			foreach (KeyValuePair<string, NKCUIShopTab> item4 in m_dicTab)
			{
				string text = MakeTabUIKey(targetTab, targetSubTabIndex);
				item4.Value.m_ctglTab.Select(text != null && text == item4.Key, bForce: true);
			}
			break;
		}
		if (!AlwaysShowNPC)
		{
			NKCUtil.SetGameobjectActive(m_UINPCShop, targetTab == "TAB_MAIN" || shopTabTemplet.m_ShopDisplay == ShopDisplayType.Item_Extend);
			NKCUtil.SetGameobjectActive(m_objNPCFront, targetTab == "TAB_MAIN");
		}
		if (shopTabTemplet.m_ShopDisplay == ShopDisplayType.Interior)
		{
			SetFilterButton();
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_csbtnFilter, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnFilterActive, bValue: false);
		}
		ShopReddotType reddotType;
		int reddotCount = NKCShopManager.CheckTabReddotCount(out reddotType);
		NKCUtil.SetShopReddotImage(reddotType, m_objCategoryReddot, m_objReddot_RED, m_objReddot_YELLOW);
		NKCUtil.SetShopReddotLabel(reddotType, m_lbCategoryReddotCount, reddotCount);
		bool bValue = false;
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.OPEN_TAG_RATE_INFO))
		{
			foreach (ShopItemTemplet item5 in m_lstShopItem)
			{
				if (item5.m_ItemType == NKM_REWARD_TYPE.RT_OPERATOR)
				{
					bValue = true;
					break;
				}
			}
		}
		NKCUtil.SetGameobjectActive(m_csbtnOperatorSubSkillPool, bValue);
		SetIgnoreHotkey();
	}

	private void ShowItemList(string targetTab, int targetSubTabIndex, bool bKeepSortedList = false)
	{
		ShopTabTemplet shopTabTemplet = ShopTabTemplet.Find(targetTab, targetSubTabIndex);
		if (shopTabTemplet == null)
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		LoopScrollRect loopScrollRect = GetLoopScrollRect(shopTabTemplet.m_ShopDisplay);
		NKCUIComDragSelectablePanel nKCUIComDragSelectablePanel = GetDragPanel(shopTabTemplet.m_ShopDisplay);
		if (m_dicFullDisplaySet.ContainsKey(shopTabTemplet.m_ShopDisplay) && nKCUIComDragSelectablePanel == null)
		{
			nKCUIComDragSelectablePanel = m_CommonFullDisplayDragSelectPanel;
			m_CommonFullDisplayDragSelectPanel.CleanUp();
			m_eCurrentCommonFulldisplaySet = shopTabTemplet.m_ShopDisplay;
			m_CommonFullDisplayDragSelectPanel.Prepare();
			NKCUtil.SetGameobjectActive(m_CommonFullDisplayDragSelectPanel, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_CommonFullDisplayDragSelectPanel, bValue: false);
		}
		foreach (DisplaySet item in GetAllDisplaySet())
		{
			bool bValue = loopScrollRect != null && item.scrollRect == loopScrollRect;
			NKCUtil.SetGameobjectActive(item.scrollRect, bValue);
			NKCUtil.SetGameobjectActive(item.m_rtParent, bValue);
		}
		foreach (FullDisplaySet item2 in GetAllFullDisplaySet())
		{
			bool bValue2 = nKCUIComDragSelectablePanel != null && item2.draggablePanel == nKCUIComDragSelectablePanel;
			NKCUtil.SetGameobjectActive(item2.draggablePanel, bValue2);
			NKCUtil.SetGameobjectActive(item2.m_rtParent, bValue2);
		}
		m_lstFeatured.Clear();
		if (loopScrollRect != null || nKCUIComDragSelectablePanel != null)
		{
			if (targetTab == "TAB_SUPPLY")
			{
				NKMShopRandomData randomShop = NKCScenManager.GetScenManager().GetMyUserData().m_ShopData.randomShop;
				if (loopScrollRect != null)
				{
					loopScrollRect.TotalCount = randomShop.datas.Count;
					loopScrollRect.SetIndexPosition(0);
				}
				NKCUtil.SetGameobjectActive(m_objEmptyList, bValue: false);
			}
			else if (!string.IsNullOrEmpty(shopTabTemplet.m_PackageGroupID))
			{
				m_lstFeatured = NKCShopManager.GetFeaturedList(NKCScenManager.CurrentUserData(), shopTabTemplet.m_PackageGroupID, targetTab == "TAB_MAIN");
				loopScrollRect.TotalCount = m_lstFeatured.Count;
				loopScrollRect.SetIndexPosition(0);
				NKCUtil.SetGameobjectActive(m_objFeaturedEmpty, targetTab == "TAB_MAIN" && m_lstFeatured.Count == 0);
				NKCUtil.SetGameobjectActive(m_objEmptyList, bValue: false);
				NKCUtil.SetGameobjectActive(m_objChainLocked, bValue: false);
			}
			else
			{
				m_lstShopItem = GetSortedTabItemList(targetTab, targetSubTabIndex);
				if (shopTabTemplet.m_ShopDisplay == ShopDisplayType.Interior)
				{
					if (!bKeepSortedList)
					{
						m_ssProduct = new NKCShopProductSortSystem(nKMUserData, m_lstShopItem, InteriorSortOption());
					}
				}
				else
				{
					m_ssProduct = null;
				}
				int totalCount = ((m_ssProduct != null) ? m_ssProduct.SortedProductList.Count : ((shopTabTemplet.m_ShopDisplay != ShopDisplayType.Custom) ? m_lstShopItem.Count : (NKCShopCustomTabTemplet.Find(shopTabTemplet.TabId)?.Count ?? 0)));
				if (loopScrollRect != null)
				{
					loopScrollRect.TotalCount = totalCount;
					loopScrollRect.SetIndexPosition(0);
				}
				if (nKCUIComDragSelectablePanel != null)
				{
					nKCUIComDragSelectablePanel.TotalCount = totalCount;
					nKCUIComDragSelectablePanel.SetIndex(0);
				}
				ShopDisplayType eDisplayType = m_eDisplayType;
				if ((uint)(eDisplayType - -1) <= 1u || eDisplayType == ShopDisplayType.Custom)
				{
					NKCUtil.SetGameobjectActive(m_objEmptyList, bValue: false);
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_objEmptyList, m_lstShopItem.Count == 0);
				}
			}
			NKCUtil.SetGameobjectActive(m_objChainLocked, shopTabTemplet.IsChainTab && m_CurrentChainIndex > NKCShopManager.GetCurrentTargetChainIndex(shopTabTemplet));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objEmptyList, bValue: false);
			NKCUtil.SetGameobjectActive(m_objChainLocked, bValue: false);
		}
	}

	private List<int> SetResourceList(ShopTabTemplet tabTemplet)
	{
		return NKCShopManager.MakeShopTabResourceList(tabTemplet);
	}

	public List<ShopItemTemplet> GetSortedTabItemList(string tabType, int subTab = 0)
	{
		ShopTabTemplet shopTabTemplet = ShopTabTemplet.Find(tabType, subTab);
		if (shopTabTemplet == null)
		{
			return new List<ShopItemTemplet>();
		}
		if (!m_dicProducts.ContainsKey(shopTabTemplet))
		{
			return new List<ShopItemTemplet>();
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return new List<ShopItemTemplet>();
		}
		List<ShopItemTemplet> list = m_dicProducts[shopTabTemplet];
		List<ShopItemTemplet> list2 = new List<ShopItemTemplet>();
		List<ShopItemTemplet> list3 = new List<ShopItemTemplet>();
		List<ShopItemTemplet> list4 = new List<ShopItemTemplet>();
		List<ShopItemTemplet> list5 = new List<ShopItemTemplet>();
		List<ShopItemTemplet> list6 = new List<ShopItemTemplet>();
		List<ShopItemTemplet> list7 = new List<ShopItemTemplet>();
		List<ShopItemTemplet> list8 = new List<ShopItemTemplet>();
		List<ShopItemTemplet> list9 = new List<ShopItemTemplet>();
		List<ShopItemTemplet> list10 = new List<ShopItemTemplet>();
		foreach (ShopItemTemplet item in list)
		{
			if (item.m_TabSubIndex == subTab && NKCShopManager.CanExhibitItem(item, bIncludeLockedItemWithReason: true) && (item.m_ChainIndex <= 0 || item.m_ChainIndex == m_CurrentChainIndex))
			{
				if (!NKMContentUnlockManager.IsContentUnlocked(nKMUserData, in item.m_UnlockInfo))
				{
					list9.Add(item);
				}
				else if (NKCShopManager.GetBuyCountLeft(item.m_ProductID) == 0)
				{
					list4.Add(item);
				}
				else if (NKCShopManager.GetReddotType(item) != ShopReddotType.NONE)
				{
					list10.Add(item);
				}
				else if (item.m_Price == 0)
				{
					list2.Add(item);
				}
				else if (item.IsReturningProduct)
				{
					list8.Add(item);
				}
				else if (item.HasDateLimit)
				{
					list3.Add(item);
				}
				else if (IsOnPromotion(item))
				{
					list5.Add(item);
				}
				else if (item.m_OrderList > 0)
				{
					list6.Add(item);
				}
				else
				{
					list7.Add(item);
				}
			}
		}
		list4.Sort(CompByOrderList);
		list2.Sort(CompByOrderList);
		list3.Sort(CompByOrderList);
		list5.Sort(CompByOrderList);
		list6.Sort(CompByOrderList);
		list7.Sort(CompByOrderList);
		list8.Sort(CompByOrderList);
		list10.Sort(CompByReddot);
		List<ShopItemTemplet> list11 = new List<ShopItemTemplet>();
		list11.AddRange(list10);
		list11.AddRange(list2);
		list11.AddRange(list8);
		list11.AddRange(list3);
		list11.AddRange(list5);
		list11.AddRange(list6);
		list11.AddRange(list7);
		list11.AddRange(list9);
		list11.AddRange(list4);
		return list11;
	}

	private int CompByReddot(ShopItemTemplet lItem, ShopItemTemplet rItem)
	{
		ShopReddotType reddotType = NKCShopManager.GetReddotType(lItem);
		ShopReddotType reddotType2 = NKCShopManager.GetReddotType(rItem);
		if (reddotType == reddotType2)
		{
			return CompByOrderList(lItem, rItem);
		}
		return reddotType2.CompareTo(reddotType);
	}

	private int CompByOrderList(ShopItemTemplet lItem, ShopItemTemplet rItem)
	{
		if (lItem.m_OrderList == rItem.m_OrderList)
		{
			return lItem.m_ItemID.CompareTo(rItem.m_ItemID);
		}
		if (rItem.m_OrderList == 0)
		{
			return -1;
		}
		if (lItem.m_OrderList == 0)
		{
			return 1;
		}
		return lItem.m_OrderList.CompareTo(rItem.m_OrderList);
	}

	private bool IsOnPromotion(ShopItemTemplet productTemplet)
	{
		switch (productTemplet.m_TagImage)
		{
		case ShopItemRibbon.ONE_PLUS_ONE:
			if (productTemplet.m_PurchaseEventType == PURCHASE_EVENT_REWARD_TYPE.FIRST_PURCHASE_CHANGE_REWARD_VALUE)
			{
				return IsFirstBuy(productTemplet.m_ProductID);
			}
			return true;
		case ShopItemRibbon.NEW:
		case ShopItemRibbon.LIMITED:
		case ShopItemRibbon.TIME_LIMITED:
		case ShopItemRibbon.POPULAR:
			return true;
		default:
			if (productTemplet.m_PurchaseEventType == PURCHASE_EVENT_REWARD_TYPE.FIRST_PURCHASE_CHANGE_REWARD_VALUE)
			{
				return IsFirstBuy(productTemplet.m_ProductID);
			}
			return false;
		}
	}

	private ShopDisplayType GetDisplayType(string type, int subIndex)
	{
		ShopTabTemplet shopTabTemplet = ShopTabTemplet.Find(type, subIndex);
		if (shopTabTemplet == null)
		{
			Debug.LogError("Tab does not exist : " + type);
			return ShopDisplayType.Item;
		}
		return shopTabTemplet.m_ShopDisplay;
	}

	private void OnBtnSupplyRefresh()
	{
		int leftCount = NKCScenManager.GetScenManager().GetMyUserData().m_ShopData.randomShop?.refreshCount ?? 5;
		NKCPopupResourceConfirmBox.Instance.OpenWithLeftCount(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_SHOP_SUPPLY_LIST_INSTANTLY_REFRESH_REQ, 101, 15, leftCount, 5, delegate
		{
			TrySupplyRefresh(useCash: true);
		});
	}

	private void TrySupplyRefresh(bool useCash)
	{
		NKCPopupItemBox.CheckInstanceAndClose();
		NKCScenManager.GetScenManager().Get_NKC_SCEN_SHOP().Send_NKMPacket_SHOP_REFRESH_REQ(useCash);
	}

	private void TryChainRefresh()
	{
		NKCPopupItemBox.CheckInstanceAndClose();
		NKCScenManager.GetScenManager().Get_NKC_SCEN_SHOP().Send_NKMPacket_SHOP_CHAIN_TAB_RESET_TIME_REQ();
	}

	private void OnBtnProductBuy(int ProductID)
	{
		bool flag = CurrentTabTemplet()?.m_MultiBuy ?? false;
		if (flag && m_eCurrentTab != "TAB_SUPPLY" && ShopItemTemplet.Find(ProductID).IsInAppProduct)
		{
			flag = false;
		}
		if (flag)
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			if (m_eCurrentTab == "TAB_SUPPLY")
			{
				if (!m_hsMultiBuy.Contains(ProductID))
				{
					NKMShopRandomData randomShop = myUserData.m_ShopData.randomShop;
					if (!randomShop.datas.ContainsKey(ProductID))
					{
						Log.Error("invalid index " + ProductID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Shop/NKCUIShop.cs", 2236);
						return;
					}
					if (randomShop.datas[ProductID].isBuy)
					{
						return;
					}
					m_hsMultiBuy.Add(ProductID);
				}
				else
				{
					m_hsMultiBuy.Remove(ProductID);
				}
				RefreshRandomShopItem(ProductID);
				return;
			}
			ShopItemTemplet shop_templet = ShopItemTemplet.Find(ProductID);
			if (!m_hsMultiBuy.Contains(ProductID))
			{
				if (NKCShopManager.CanBuyFixShop(myUserData, shop_templet, out var _, out var _) != NKM_ERROR_CODE.NEC_OK)
				{
					return;
				}
				m_hsMultiBuy.Add(ProductID);
			}
			else
			{
				m_hsMultiBuy.Remove(ProductID);
			}
			RefreshShopItem(ProductID);
			return;
		}
		switch (NKCShopManager.OnBtnProductBuy(ProductID, m_eCurrentTab == "TAB_SUPPLY"))
		{
		case NKM_ERROR_CODE.NKE_FAIL_SHOP_INVALID_CHAIN_TAB:
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_SHOP_CHAIN_LOCKED, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			break;
		case NKM_ERROR_CODE.NEC_OK:
			if (m_eCurrentTab != "TAB_SUPPLY")
			{
				ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(ProductID);
				if (shopItemTemplet != null)
				{
					m_UINPCShop?.PlayAni(GetNPCActionType(shopItemTemplet.m_TagImage));
				}
			}
			break;
		}
	}

	private void OnBtnMultiBuy()
	{
		if (m_hsMultiBuy.Count != 0)
		{
			NKM_ERROR_CODE nKM_ERROR_CODE = NKCShopManager.OnMultipleProductBuy(m_hsMultiBuy, IsSupplyTab());
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				NKCPopupOKCancel.OpenOKBox(nKM_ERROR_CODE);
			}
		}
	}

	private void OnBtnMultiBuyClear()
	{
		m_hsMultiBuy.Clear();
		RefreshCurrentTab();
	}

	private int GetRewardCountByItemID(int itemID)
	{
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(itemID);
		if (itemMiscTempletByID != null)
		{
			List<NKMRandomBoxItemTemplet> randomBoxItemTempletList = NKCRandomBoxManager.GetRandomBoxItemTempletList(itemMiscTempletByID.m_RewardGroupID);
			if (randomBoxItemTempletList != null)
			{
				return randomBoxItemTempletList.Count;
			}
		}
		return 0;
	}

	private void OnBtnBanner(NKCShopBannerTemplet bannerTemplet)
	{
		if (bannerTemplet == null)
		{
			return;
		}
		if (!NKCSynchronizedTime.IsEventTime(bannerTemplet.m_DateStrID))
		{
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_DEACTIVATED_EVENT_POST, delegate
			{
				SelectTab("TAB_MAIN", 0, bForce: true);
			});
			return;
		}
		if (bannerTemplet.m_ProductID > 0)
		{
			if (NKCShopManager.CanBuyFixShop(NKCScenManager.CurrentUserData(), ShopItemTemplet.Find(bannerTemplet.m_ProductID), out var _, out var _) == NKM_ERROR_CODE.NEC_OK)
			{
				OnBtnProductBuy(bannerTemplet.m_ProductID);
			}
			return;
		}
		NKCShopCategoryTemplet categoryFromTab = NKCShopManager.GetCategoryFromTab(bannerTemplet.m_TabID);
		if (categoryFromTab != null)
		{
			ChangeCategory(categoryFromTab.m_eCategory, bannerTemplet.m_TabID, bannerTemplet.m_TabSubIndex);
		}
		else
		{
			SelectTab(bannerTemplet.m_TabID, bannerTemplet.m_TabSubIndex, bForce: true);
		}
	}

	private void OnBtnBuyAll()
	{
		ShopTabTemplet tabTemplet = ShopTabTemplet.Find(m_eCurrentTab, m_CurrentSubIndex);
		if (tabTemplet == null)
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		int bundleItemPrice = NKCShopManager.GetBundleItemPrice(tabTemplet);
		int bundleItemPriceItemID = NKCShopManager.GetBundleItemPriceItemID(tabTemplet);
		if (nKMUserData != null && !nKMUserData.CheckPrice(bundleItemPrice, bundleItemPriceItemID))
		{
			OpenResourceAddPopup(bundleItemPriceItemID, bundleItemPrice);
		}
		else if (bundleItemPrice != 0)
		{
			NKCPopupResourceWithdraw.Instance.OpenForShopBuyAll(tabTemplet, delegate
			{
				TryBuyAll(tabTemplet);
			});
		}
	}

	private void TryBuyAll(ShopTabTemplet tabTemplet)
	{
		List<ShopItemTemplet> itemTempletListByTab = NKCShopManager.GetItemTempletListByTab(tabTemplet);
		HashSet<int> hashSet = new HashSet<int>();
		for (int i = 0; i < itemTempletListByTab.Count; i++)
		{
			if (!hashSet.Contains(itemTempletListByTab[i].m_ProductID))
			{
				hashSet.Add(itemTempletListByTab[i].m_ProductID);
			}
			else
			{
				Log.Error($"번들탭에 동일한 상품 아이디가 존재함 - {itemTempletListByTab[i].m_ProductID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Shop/NKCUIShop.cs", 2394);
			}
		}
		NKCShopManager.SetBundleItemIds(hashSet);
		for (int j = 0; j < itemTempletListByTab.Count; j++)
		{
			NKCPacketSender.Send_NKMPacket_SHOP_FIX_SHOP_BUY_REQ(itemTempletListByTab[j].m_ProductID, NKCShopManager.GetBuyCountLeft(itemTempletListByTab[j].m_ProductID));
		}
	}

	private static void OpenResourceAddPopup(int priceItemID, int price)
	{
		NKCShopManager.OpenItemLackPopup(priceItemID, price);
	}

	public static bool IsFirstBuy(int ProductID)
	{
		return NKCShopManager.IsFirstBuy(ProductID, NKCScenManager.GetScenManager().GetMyUserData());
	}

	public void RandomShopItemUpdateComplete()
	{
		NKMShopRandomData randomShop = NKCScenManager.GetScenManager().GetMyUserData().m_ShopData.randomShop;
		if (randomShop == null || randomShop.datas.Count == 0 || NKCSynchronizedTime.IsFinished(randomShop.nextRefreshDate))
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_SHOP_SUPPLY_LIST_GET_FAIL);
			SelectTab("TAB_MAIN", 0, bForce: true);
		}
		else
		{
			SelectTab("TAB_SUPPLY", 0, bForce: true);
		}
	}

	public void ChainRefreshComplete(List<ShopChainTabNextResetData> lstChainResetData)
	{
		for (int i = 0; i < lstChainResetData.Count; i++)
		{
			if (lstChainResetData[i].tabType == m_eCurrentTab && lstChainResetData[i].subIndex == m_CurrentSubIndex)
			{
				SelectTab(m_eCurrentTab, m_CurrentSubIndex, bForce: true);
				return;
			}
		}
		SelectTab("TAB_MAIN", 0, bForce: true);
	}

	private void OnChainTab_01(bool bSelect)
	{
		if (m_CurrentChainIndex != 1 && bSelect)
		{
			m_CurrentChainIndex = 1;
			ShowItemList(m_eCurrentTab, m_CurrentSubIndex);
		}
	}

	private void OnChainTab_02(bool bSelect)
	{
		if (m_CurrentChainIndex != 2 && bSelect)
		{
			m_CurrentChainIndex = 2;
			ShowItemList(m_eCurrentTab, m_CurrentSubIndex);
		}
	}

	private void OnChainTab_03(bool bSelect)
	{
		if (m_CurrentChainIndex != 3 && bSelect)
		{
			m_CurrentChainIndex = 3;
			ShowItemList(m_eCurrentTab, m_CurrentSubIndex);
		}
	}

	private void UpdateBuyAllBtn(ShopTabTemplet tabTemplet)
	{
		if (tabTemplet == null)
		{
			NKCUtil.SetGameobjectActive(m_btnBuyAll, bValue: false);
		}
		else if (tabTemplet.IsBundleTab)
		{
			int bundleItemPriceItemID = NKCShopManager.GetBundleItemPriceItemID(tabTemplet);
			int bundleItemPrice = NKCShopManager.GetBundleItemPrice(tabTemplet);
			if (NKCScenManager.CurrentUserData().CheckPrice(bundleItemPrice, bundleItemPriceItemID))
			{
				NKCUtil.SetGameobjectActive(m_btnBuyAll, bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_btnBuyAll, bValue: false);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_btnBuyAll, bValue: false);
		}
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		ShopTabTemplet shopTabTemplet = ShopTabTemplet.Find(m_eCurrentTab, m_CurrentSubIndex);
		if (shopTabTemplet != null && shopTabTemplet.IsBundleTab)
		{
			UpdateBuyAllBtn(shopTabTemplet);
		}
	}

	public virtual void OnProductBuy(ShopItemTemplet productTemplet)
	{
		if (productTemplet != null && productTemplet.m_ItemType != NKM_REWARD_TYPE.RT_SKIN && m_UINPCShop != null)
		{
			m_UINPCShop.PlayAni(NPC_ACTION_TYPE.THANKS);
		}
		NKCUIShopSkinPopup.CheckInstanceAndClose();
	}

	private void OnBtnJPNPaymentLaw()
	{
		NKCPublisherModule.InAppPurchase.OpenPaymentLaw(null);
	}

	private void OnBtnJPNCommercialLaw()
	{
		NKCPublisherModule.InAppPurchase.OpenCommercialLaw(null);
	}

	public void OnClickOperatorSubSkillPool()
	{
		NKCPopupOperatorSubSkillList.Instance.Open();
	}

	private void ProvideFullDisplayData(Transform tr, int idx)
	{
		ShopTabTemplet shopTabTemplet = ShopTabTemplet.Find(m_eCurrentTab, m_CurrentSubIndex);
		if (shopTabTemplet != null && shopTabTemplet.m_ShopDisplay == ShopDisplayType.Custom)
		{
			NKCUIShopSlotCustomPrefabAdapter component = tr.GetComponent<NKCUIShopSlotCustomPrefabAdapter>();
			if (component != null)
			{
				NKCShopCustomTabTemplet tabTemplet = NKCShopCustomTabTemplet.Find(shopTabTemplet.TabId, idx);
				component.SetData(this, tabTemplet, OnBtnProductBuy, ForceUpdateItemList);
			}
		}
		else
		{
			NKCUIShopSlotBase component2 = tr.GetComponent<NKCUIShopSlotBase>();
			if (component2 != null)
			{
				ShopItemTemplet shopItemTemplet = m_lstShopItem[idx];
				component2.SetData(this, shopItemTemplet, NKCShopManager.GetBuyCountLeft(shopItemTemplet.m_ProductID), IsFirstBuy(shopItemTemplet.m_ProductID));
			}
		}
	}

	private ShopTabTemplet CurrentTabTemplet()
	{
		return ShopTabTemplet.Find(m_eCurrentTab, m_CurrentSubIndex);
	}

	private bool IsSupplyTab()
	{
		return m_eCurrentTab == "TAB_SUPPLY";
	}

	private void ProcessSupplyHotkey()
	{
		if (Input.GetKeyUp(KeyCode.Keypad7))
		{
			OnBtnProductBuy(1);
		}
		if (Input.GetKeyUp(KeyCode.Keypad8))
		{
			OnBtnProductBuy(2);
		}
		if (Input.GetKeyUp(KeyCode.Keypad9))
		{
			OnBtnProductBuy(3);
		}
		if (Input.GetKeyUp(KeyCode.Keypad4))
		{
			OnBtnProductBuy(4);
		}
		if (Input.GetKeyUp(KeyCode.Keypad5))
		{
			OnBtnProductBuy(5);
		}
		if (Input.GetKeyUp(KeyCode.Keypad6))
		{
			OnBtnProductBuy(6);
		}
		if (Input.GetKeyUp(KeyCode.Keypad1))
		{
			OnBtnProductBuy(7);
		}
		if (Input.GetKeyUp(KeyCode.Keypad2))
		{
			OnBtnProductBuy(8);
		}
		if (Input.GetKeyUp(KeyCode.Keypad3))
		{
			OnBtnProductBuy(9);
		}
	}

	public NPC_ACTION_TYPE GetNPCActionType(string tabType)
	{
		return tabType switch
		{
			"TAB_CASH" => NPC_ACTION_TYPE.SELECT_TAB_CASH, 
			"TAB_PACKAGE" => NPC_ACTION_TYPE.SELECT_TAB_PACKAGE, 
			"TAB_RESOURCE" => NPC_ACTION_TYPE.SELECT_TAB_RESOURCE, 
			"TAB_FUNCTION" => NPC_ACTION_TYPE.SELECT_TAB_FUNCTION, 
			"TAB_FACILITIES" => NPC_ACTION_TYPE.SELECT_TAB_FACILITIES, 
			"TAB_SKIN" => NPC_ACTION_TYPE.SELECT_TAB_SKIN, 
			"TAB_COUPON" => NPC_ACTION_TYPE.SELECT_TAB_COUPON, 
			"TAB_SUPPLY" => NPC_ACTION_TYPE.SELECT_TAB_SUPPLY, 
			"TAB_EVENT" => NPC_ACTION_TYPE.SELECT_TAB_EVENT, 
			"TAB_PVP" => NPC_ACTION_TYPE.SELECT_TAB_PVP, 
			"TAB_DIVE" => NPC_ACTION_TYPE.SELECT_TAB_DIVE, 
			"TAB_HR" => NPC_ACTION_TYPE.SELECT_TAB_HR, 
			_ => NPC_ACTION_TYPE.NONE, 
		};
	}

	public NPC_ACTION_TYPE GetNPCActionType(ShopItemRibbon type)
	{
		return type switch
		{
			ShopItemRibbon.LIMITED => NPC_ACTION_TYPE.SELECT_GOODS_LIMITED, 
			ShopItemRibbon.ONE_PLUS_ONE => NPC_ACTION_TYPE.SELECT_GOODS_ONE_PLUS_ONE, 
			ShopItemRibbon.POPULAR => NPC_ACTION_TYPE.SELECT_GOODS_POPULAR, 
			_ => NPC_ACTION_TYPE.NONE, 
		};
	}

	public void OnChangeCategory()
	{
		m_uiShopCategoryChange?.Open();
	}

	private void _ChangeCategory(NKCShopManager.ShopTabCategory category)
	{
		ChangeCategory(category);
	}

	public void ChangeCategory(NKCShopManager.ShopTabCategory category, string selectedTab = "TAB_NONE", int subTabIndex = 0)
	{
		NKCUtil.SetGameobjectActive(m_uiShopCategoryChange, bValue: false);
		BuildTabs(category, selectedTab, subTabIndex);
	}

	public override bool OnHotkey(HotkeyEventType hotkey)
	{
		switch (hotkey)
		{
		case HotkeyEventType.NextTab:
		{
			int num = 8;
			for (int j = 1; j < num; j++)
			{
				NKCShopManager.ShopTabCategory category = (NKCShopManager.ShopTabCategory)((int)(m_eCategory + j) % num);
				if (NKCShopCategoryTemplet.Find(category) != null)
				{
					_ChangeCategory(category);
					break;
				}
			}
			return true;
		}
		case HotkeyEventType.PrevTab:
		{
			int num2 = 8;
			for (int k = 1; k < num2; k++)
			{
				NKCShopManager.ShopTabCategory category2 = (NKCShopManager.ShopTabCategory)((int)(m_eCategory + num2 - k) % num2);
				if (NKCShopCategoryTemplet.Find(category2) != null)
				{
					_ChangeCategory(category2);
					break;
				}
			}
			return true;
		}
		case HotkeyEventType.Down:
			return MoveTab(1);
		case HotkeyEventType.Up:
			return MoveTab(-1);
		case HotkeyEventType.ShowHotkey:
			if (m_srTab != null)
			{
				NKCUIComHotkeyDisplay.OpenInstance(m_srTab.transform, HotkeyEventType.Up, HotkeyEventType.Down);
			}
			if (m_csbtnChangeCategory != null)
			{
				NKCUIComHotkeyDisplay.OpenInstance(m_csbtnChangeCategory.transform, HotkeyEventType.NextTab);
			}
			if (IsSupplyTab())
			{
				NKCUIShopSlotBase[] componentsInChildren = GetLoopScrollRect(GetDisplayType(m_eCurrentTab, m_CurrentSubIndex)).transform.GetComponentsInChildren<NKCUIShopSlotBase>();
				foreach (NKCUIShopSlotBase nKCUIShopSlotBase in componentsInChildren)
				{
					NKCUIComHotkeyDisplay.OpenInstance(text: $"Num{((nKCUIShopSlotBase.ProductID < 7) ? ((nKCUIShopSlotBase.ProductID > 3) ? nKCUIShopSlotBase.ProductID : (nKCUIShopSlotBase.ProductID + 6)) : (nKCUIShopSlotBase.ProductID - 6))}", parent: nKCUIShopSlotBase.transform);
				}
			}
			return false;
		default:
			return false;
		}
	}

	private void SetIgnoreHotkey()
	{
		if (IsSupplyTab())
		{
			NKCInputManager.AddIgnoreKey(KeyCode.Keypad0, KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3, KeyCode.Keypad4, KeyCode.Keypad5, KeyCode.Keypad6, KeyCode.Keypad7, KeyCode.Keypad8, KeyCode.Keypad9);
		}
		else
		{
			NKCInputManager.ClearIgnoreKey();
		}
	}

	private bool MoveTab(int moveCount)
	{
		int num = m_lstEnabledTabs.FindIndex((ShopTabTemplet x) => x.TabType == m_eCurrentTab && x.SubIndex == m_CurrentSubIndex);
		if (num >= 0)
		{
			int index = (num + moveCount + m_lstEnabledTabs.Count) % m_lstEnabledTabs.Count;
			ShopTabTemplet shopTabTemplet = m_lstEnabledTabs[index];
			SelectTab(shopTabTemplet.TabType, shopTabTemplet.SubIndex);
			return true;
		}
		return false;
	}

	private void OnThemeSelected(int themeID)
	{
		if (m_ssProduct != null)
		{
			m_ssProduct.FilterStatType_ThemeID = themeID;
			m_ssProduct.FilterList(m_ssProduct.FilterSet);
		}
		SetFilterButton();
		ShowItemList(m_eCurrentTab, m_CurrentSubIndex, bKeepSortedList: true);
	}

	private void OnBtnFilter()
	{
		int currentSelectedThemeID = 0;
		if (m_ssProduct != null)
		{
			currentSelectedThemeID = m_ssProduct.FilterStatType_ThemeID;
		}
		NKCPopupFilterTheme.Instance.Open(OnThemeSelected, currentSelectedThemeID);
	}

	private NKCShopProductSortSystem.ShopProductListOptions InteriorSortOption()
	{
		return new NKCShopProductSortSystem.ShopProductListOptions
		{
			setFilterOption = new HashSet<NKCShopProductSortSystem.eFilterOption> { NKCShopProductSortSystem.eFilterOption.Theme },
			lstSortOption = new List<NKCShopProductSortSystem.eSortOption>(),
			m_filterThemeID = 0
		};
	}

	private void SetFilterButton()
	{
		bool flag = m_ssProduct != null && m_ssProduct.FilterStatType_ThemeID != 0;
		NKCUtil.SetGameobjectActive(m_csbtnFilterActive, flag);
		NKCUtil.SetGameobjectActive(m_csbtnFilter, !flag);
	}

	public bool TutorialCheck()
	{
		return NKCTutorialManager.TutorialRequired(TutorialPoint.Shop) != TutorialStep.None;
	}
}
