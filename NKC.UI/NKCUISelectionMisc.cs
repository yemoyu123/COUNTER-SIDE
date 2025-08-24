using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUISelectionMisc : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_UNIT_SELECTION";

	private const string UI_ASSET_NAME = "NKM_UI_MISC_SELECTION";

	private static NKCUISelectionMisc m_Instance;

	public NKCUIComSafeArea m_SafeArea;

	[Header("Misc")]
	public GameObject m_objMiscChoice;

	public LoopScrollRect m_loopScrollRectMisc;

	public Transform m_trContentParentMisc;

	public Image m_imgBannerMisc;

	[Header("프리팹")]
	public NKCUISlot m_pfbMiscSlot;

	[Header("필터")]
	public NKCUIComMiscSortOptions m_miscSortOptions;

	private NKM_ITEM_MISC_TYPE m_NKM_ITEM_MISC_TYPE = NKM_ITEM_MISC_TYPE.IMT_CHOICE_UNIT;

	private List<int> m_lstRewardId = new List<int>();

	private List<NKMRandomBoxItemTemplet> m_lstRandomBoxItemTemplet = new List<NKMRandomBoxItemTemplet>();

	private List<NKMItemMiscTemplet> m_lstMiscTemplet = new List<NKMItemMiscTemplet>();

	private List<NKCUISlot> m_lstVisibleMiscSlot = new List<NKCUISlot>();

	private Stack<NKCUISlot> m_stkMiscSlotPool = new Stack<NKCUISlot>();

	private List<NKCUISlot.SlotData> m_lstFilteredMiscSlotData = new List<NKCUISlot.SlotData>();

	private NKMItemMiscTemplet m_NKMItemMiscTemplet;

	private NKCMiscSortSystem m_miscSortSystem;

	public static NKCUISelectionMisc Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUISelectionMisc>("AB_UI_NKM_UI_UNIT_SELECTION", "NKM_UI_MISC_SELECTION", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUISelectionMisc>();
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

	public override string MenuName
	{
		get
		{
			if (m_NKM_ITEM_MISC_TYPE == NKM_ITEM_MISC_TYPE.IMT_CHOICE_MISC)
			{
				return NKCUtilString.GET_STRING_CHOICE_MISC;
			}
			return NKCUtilString.GET_STRING_USE_CHOICE;
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

	private void InitUI()
	{
		m_loopScrollRectMisc.dOnGetObject += GetObject;
		m_loopScrollRectMisc.dOnReturnObject += ReturnObject;
		m_loopScrollRectMisc.dOnProvideData += ProvideData;
		m_loopScrollRectMisc.dOnRepopulate += CalculateContentRectSize;
		NKCUtil.SetScrollHotKey(m_loopScrollRectMisc);
	}

	public override void CloseInternal()
	{
		m_miscSortOptions.ResetUI();
		m_lstRewardId = new List<int>();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open(NKMItemMiscTemplet itemMiscTemplet)
	{
		if (itemMiscTemplet == null)
		{
			return;
		}
		m_NKMItemMiscTemplet = itemMiscTemplet;
		List<NKMRandomBoxItemTemplet> randomBoxItemTempletList = NKCRandomBoxManager.GetRandomBoxItemTempletList(itemMiscTemplet.m_RewardGroupID);
		if (randomBoxItemTempletList == null)
		{
			return;
		}
		for (int i = 0; i < randomBoxItemTempletList.Count; i++)
		{
			m_lstRewardId.Add(randomBoxItemTempletList[i].m_RewardID);
		}
		m_NKM_ITEM_MISC_TYPE = itemMiscTemplet.m_ItemMiscType;
		NKCUtil.SetGameobjectActive(m_objMiscChoice, m_NKM_ITEM_MISC_TYPE == NKM_ITEM_MISC_TYPE.IMT_CHOICE_MISC);
		NKCScenManager.CurrentUserData();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		CalculateContentRectSize();
		m_lstRandomBoxItemTemplet = randomBoxItemTempletList;
		m_lstRandomBoxItemTemplet.Sort(CompOrderList);
		m_lstMiscTemplet.Clear();
		for (int j = 0; j < m_lstRandomBoxItemTemplet.Count; j++)
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(m_lstRandomBoxItemTemplet[j].m_RewardID);
			if (itemMiscTempletByID != null)
			{
				m_lstMiscTemplet.Add(itemMiscTempletByID);
			}
		}
		NKCUtil.SetImageSprite(m_imgBannerMisc, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_UNIT_SELECTION_TEXTURE", itemMiscTemplet.m_BannerImage));
		SetSortSystem();
		UIOpened();
	}

	private void CalculateContentRectSize()
	{
		m_SafeArea?.SetSafeAreaBase();
		Vector2 cellSize = m_trContentParentMisc.GetComponent<GridLayoutGroup>().cellSize;
		Vector2 spacing = m_trContentParentMisc.GetComponent<GridLayoutGroup>().spacing;
		NKCUtil.CalculateContentRectSize(m_loopScrollRectMisc, m_trContentParentMisc.GetComponent<GridLayoutGroup>(), 5, cellSize, spacing);
		m_loopScrollRectMisc.PrepareCells();
	}

	private int CompOrderList(NKMRandomBoxItemTemplet lItem, NKMRandomBoxItemTemplet rItem)
	{
		if (lItem.m_OrderList == rItem.m_OrderList)
		{
			return lItem.m_RewardID.CompareTo(rItem.m_RewardID);
		}
		return lItem.m_OrderList.CompareTo(rItem.m_OrderList);
	}

	private void SetSortSystem()
	{
		m_miscSortSystem = new NKCMiscSortSystem(options: new NKCMiscSortSystem.MiscListOptions
		{
			lstSortOption = NKCMiscSortSystem.GetDefaultSortList(),
			setFilterOption = new HashSet<NKCMiscSortSystem.eFilterOption>(),
			lstCustomSortFunc = new Dictionary<NKCMiscSortSystem.eSortCategory, KeyValuePair<string, NKCMiscSortSystem.NKCDataComparerer<NKMItemMiscTemplet>.CompareFunc>>(),
			bHideDescendingOption = true,
			bHideFilterOption = true,
			bHideSortOption = true
		}, userData: NKCScenManager.CurrentUserData(), lstTargetMiscs: m_lstMiscTemplet);
		m_miscSortOptions.Init(OnSorted);
		m_miscSortOptions.RegisterCategories(NKCMiscSortSystem.GetDefaultFilterCategory(), NKCMiscSortSystem.GetDefaultSortCategory());
		m_miscSortOptions.RegisterMiscSort(m_miscSortSystem);
		m_miscSortOptions.ResetUI();
		OnSorted(bResetScroll: true);
	}

	private RectTransform GetObject(int index)
	{
		NKCUISlot nKCUISlot = null;
		nKCUISlot = ((m_stkMiscSlotPool.Count <= 0) ? Object.Instantiate(m_pfbMiscSlot) : m_stkMiscSlotPool.Pop());
		NKCUtil.SetGameobjectActive(nKCUISlot, bValue: true);
		m_lstVisibleMiscSlot.Add(nKCUISlot);
		return nKCUISlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform go)
	{
		NKCUISlot component = go.GetComponent<NKCUISlot>();
		NKCUtil.SetGameobjectActive(go, bValue: false);
		go.SetParent(base.transform);
		if (component != null)
		{
			m_lstVisibleMiscSlot.Remove(component);
			m_stkMiscSlotPool.Push(component);
		}
	}

	private void ProvideData(Transform tr, int idx)
	{
		if (m_lstFilteredMiscSlotData.Count != 0)
		{
			NKMRandomBoxItemTemplet nKMRandomBoxItemTemplet = m_lstRandomBoxItemTemplet.Find((NKMRandomBoxItemTemplet x) => x.m_RewardID == m_lstFilteredMiscSlotData[idx].ID);
			if (nKMRandomBoxItemTemplet != null)
			{
				NKCUISlot component = tr.GetComponent<NKCUISlot>();
				component.Init();
				component.SetData(m_lstFilteredMiscSlotData[idx], bShowName: true, bShowNumber: true, bEnableLayoutElement: true, OnSelectMiscSlot);
				component.SetCountRange(nKMRandomBoxItemTemplet.TotalQuantity_Min, nKMRandomBoxItemTemplet.TotalQuantity_Max);
				component.SetHaveCount(NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(nKMRandomBoxItemTemplet.m_RewardID));
			}
		}
	}

	public void OnSelectMiscSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		NKCPopupSelectionConfirm.Instance.Open(m_NKMItemMiscTemplet, slotData.ID, slotData.Count);
	}

	private void OnSorted(bool bResetScroll)
	{
		m_lstFilteredMiscSlotData.Clear();
		int i;
		for (i = 0; i < m_miscSortSystem.SortedMiscList.Count; i++)
		{
			NKMRandomBoxItemTemplet nKMRandomBoxItemTemplet = m_lstRandomBoxItemTemplet.Find((NKMRandomBoxItemTemplet x) => x.m_RewardID == m_miscSortSystem.SortedMiscList[i].m_ItemMiscID);
			if (nKMRandomBoxItemTemplet != null)
			{
				NKCUISlot.SlotData item = NKCUISlot.SlotData.MakeMiscItemData(nKMRandomBoxItemTemplet.m_RewardID, nKMRandomBoxItemTemplet.TotalQuantity_Max);
				m_lstFilteredMiscSlotData.Add(item);
			}
		}
		m_loopScrollRectMisc.TotalCount = m_lstFilteredMiscSlotData.Count;
		m_loopScrollRectMisc.RefreshCells(bResetScroll);
	}
}
