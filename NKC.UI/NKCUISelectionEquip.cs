using System.Collections.Generic;
using Cs.Logging;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUISelectionEquip : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_UNIT_SELECTION";

	private const string UI_ASSET_NAME = "NKM_UI_EQUIP_SELECTION";

	private static NKCUISelectionEquip m_Instance;

	public NKCUIComSafeArea m_SafeArea;

	[Header("장비")]
	public GameObject m_objEquipChoice;

	public LoopScrollRect m_loopScrollRectEquip;

	public Transform m_trContentParentEquip;

	public Image m_imgBannerEquip;

	[Header("프리팹")]
	public NKCUISlotEquip m_pfbEquipSlot;

	[Header("우측 정보")]
	public GameObject m_objEquipInfo;

	public NKCUIInvenEquipSlot m_InvenEquipSlot;

	public NKCUIComStateButton m_btnOK;

	[Header("필터")]
	public NKCUIComEquipSortOptions m_SortUI;

	private NKM_ITEM_MISC_TYPE m_NKM_ITEM_MISC_TYPE = NKM_ITEM_MISC_TYPE.IMT_CHOICE_UNIT;

	private List<int> m_lstRewardId = new List<int>();

	private NKCEquipSortSystem m_ssEquip;

	private List<NKCUISlotEquip> m_lstVisibleEquipSlot = new List<NKCUISlotEquip>();

	private Stack<NKCUISlotEquip> m_stkEquipSlotPool = new Stack<NKCUISlotEquip>();

	private NKMItemMiscTemplet m_NKMItemMiscTemplet;

	private NKCUISlotEquip m_LatestSelectedSlot;

	private readonly HashSet<NKCEquipSortSystem.eFilterCategory> m_setEquipFilterCategory = new HashSet<NKCEquipSortSystem.eFilterCategory>
	{
		NKCEquipSortSystem.eFilterCategory.UnitType,
		NKCEquipSortSystem.eFilterCategory.EquipType,
		NKCEquipSortSystem.eFilterCategory.Rarity,
		NKCEquipSortSystem.eFilterCategory.Tier,
		NKCEquipSortSystem.eFilterCategory.Have
	};

	private readonly HashSet<NKCEquipSortSystem.eSortCategory> m_setEquipSortCategory = new HashSet<NKCEquipSortSystem.eSortCategory>
	{
		NKCEquipSortSystem.eSortCategory.Tier,
		NKCEquipSortSystem.eSortCategory.Rarity
	};

	public static NKCUISelectionEquip Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUISelectionEquip>("AB_UI_NKM_UI_UNIT_SELECTION", "NKM_UI_EQUIP_SELECTION", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUISelectionEquip>();
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
			if (m_NKM_ITEM_MISC_TYPE == NKM_ITEM_MISC_TYPE.IMT_CHOICE_EQUIP)
			{
				return NKCUtilString.GET_STRING_CHOICE_EQUIP;
			}
			return NKCUtilString.GET_STRING_USE_CHOICE;
		}
	}

	private bool CanChoiceSetOption
	{
		get
		{
			if (m_NKMItemMiscTemplet != null)
			{
				return m_NKMItemMiscTemplet.m_ItemMiscSubType == NKM_ITEM_MISC_SUBTYPE.IMST_EQUIP_CHOICE_SET_OPTION;
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

	private void InitUI()
	{
		m_loopScrollRectEquip.dOnGetObject += GetObject;
		m_loopScrollRectEquip.dOnReturnObject += ReturnObject;
		m_loopScrollRectEquip.dOnProvideData += ProvideData;
		m_loopScrollRectEquip.dOnRepopulate += CalculateContentRectSize;
		NKCUtil.SetScrollHotKey(m_loopScrollRectEquip);
		m_btnOK.PointerClick.RemoveAllListeners();
		m_btnOK.PointerClick.AddListener(OnClickOk);
		if (m_SortUI != null)
		{
			m_SortUI.Init(ProcessUIFromCurrentDisplayedSortData);
		}
	}

	public override void CloseInternal()
	{
		m_lstRewardId = new List<int>();
		m_ssEquip = null;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		if (m_SortUI != null)
		{
			m_SortUI.ResetUI();
		}
	}

	public override void OnCloseInstance()
	{
		m_ssEquip = null;
	}

	public void Open(NKMItemMiscTemplet itemMiscTemplet)
	{
		if (itemMiscTemplet == null)
		{
			return;
		}
		m_NKMItemMiscTemplet = itemMiscTemplet;
		m_LatestSelectedSlot = null;
		NKCUtil.SetGameobjectActive(m_imgBannerEquip, bValue: true);
		NKCUtil.SetGameobjectActive(m_objEquipInfo, bValue: false);
		List<NKMRandomBoxItemTemplet> randomBoxItemTempletList = NKCRandomBoxManager.GetRandomBoxItemTempletList(m_NKMItemMiscTemplet.m_RewardGroupID);
		if (randomBoxItemTempletList != null)
		{
			for (int i = 0; i < randomBoxItemTempletList.Count; i++)
			{
				m_lstRewardId.Add(randomBoxItemTempletList[i].m_RewardID);
			}
			m_NKM_ITEM_MISC_TYPE = m_NKMItemMiscTemplet.m_ItemMiscType;
			NKCUtil.SetGameobjectActive(m_objEquipChoice, m_NKM_ITEM_MISC_TYPE == NKM_ITEM_MISC_TYPE.IMT_CHOICE_EQUIP);
			NKCScenManager.CurrentUserData();
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			CalculateContentRectSize();
			SetEquipChoiceList();
			NKCUtil.SetImageSprite(m_imgBannerEquip, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_UNIT_SELECTION_TEXTURE", itemMiscTemplet.m_BannerImage));
			UIOpened();
		}
	}

	private void CalculateContentRectSize()
	{
		m_SafeArea?.SetSafeAreaBase();
		int minColumn = 4;
		Vector2 cellSize = m_trContentParentEquip.GetComponent<GridLayoutGroup>().cellSize;
		Vector2 spacing = m_trContentParentEquip.GetComponent<GridLayoutGroup>().spacing;
		NKCUtil.CalculateContentRectSize(m_loopScrollRectEquip, m_trContentParentEquip.GetComponent<GridLayoutGroup>(), minColumn, cellSize, spacing);
	}

	private RectTransform GetObject(int index)
	{
		NKCUISlotEquip nKCUISlotEquip = null;
		nKCUISlotEquip = ((m_stkEquipSlotPool.Count <= 0) ? Object.Instantiate(m_pfbEquipSlot) : m_stkEquipSlotPool.Pop());
		NKCUtil.SetGameobjectActive(nKCUISlotEquip, bValue: true);
		m_lstVisibleEquipSlot.Add(nKCUISlotEquip);
		return nKCUISlotEquip.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform go)
	{
		NKCUISlotEquip component = go.GetComponent<NKCUISlotEquip>();
		if (component != null)
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
			m_lstVisibleEquipSlot.Remove(component);
			m_stkEquipSlotPool.Push(component);
		}
	}

	private void ProvideData(Transform tr, int idx)
	{
		if (m_ssEquip == null)
		{
			Debug.LogError("Slot Sort System Null!!");
			return;
		}
		NKCUISlotEquip component = tr.GetComponent<NKCUISlotEquip>();
		NKMEquipItemData nKMEquipItemData = new NKMEquipItemData();
		if (m_ssEquip.SortedEquipList.Count > idx)
		{
			nKMEquipItemData = m_ssEquip.SortedEquipList[idx];
			NKCUtil.SetGameobjectActive(component.gameObject, bValue: true);
			component.SetData(nKMEquipItemData, OnSelectEquipSlot);
			component.SetHaveCount(NKCScenManager.CurrentUserData().m_InventoryData.GetSameKindEquipCount(nKMEquipItemData.m_ItemEquipID, nKMEquipItemData.m_SetOptionId));
		}
	}

	private void SetEquipChoiceList()
	{
		NKCEquipSortSystem.EquipListOptions options = new NKCEquipSortSystem.EquipListOptions
		{
			setOnlyIncludeEquipID = new HashSet<int>(),
			lstSortOption = NKCEquipSortSystem.GetDefaultSortOption(NKCPopupEquipSort.SORT_OPEN_TYPE.SELECTION)
		};
		List<NKMEquipItemData> list = new List<NKMEquipItemData>();
		if (CanChoiceSetOption)
		{
			for (int i = 0; i < m_lstRewardId.Count; i++)
			{
				List<NKMEquipItemData> collection = NKCEquipSortSystem.MakeTempEquipDataWithAllSet(m_lstRewardId[i]);
				list.AddRange(collection);
				if (!options.setOnlyIncludeEquipID.Contains(m_lstRewardId[i]))
				{
					options.setOnlyIncludeEquipID.Add(m_lstRewardId[i]);
				}
			}
		}
		else
		{
			for (int j = 0; j < m_lstRewardId.Count; j++)
			{
				NKMEquipItemData item = NKCEquipSortSystem.MakeTempEquipData(m_lstRewardId[j]);
				list.Add(item);
				if (!options.setOnlyIncludeEquipID.Contains(m_lstRewardId[j]))
				{
					options.setOnlyIncludeEquipID.Add(m_lstRewardId[j]);
				}
			}
		}
		if (m_ssEquip == null)
		{
			m_ssEquip = new NKCEquipSortSystem(NKCScenManager.CurrentUserData(), options, list);
		}
		else
		{
			m_ssEquip.BuildFilterAndSortedList(m_ssEquip.FilterSet, m_ssEquip.lstSortOption, bHideEquippedItem: false);
		}
		m_SortUI.RegisterCategories(m_setEquipFilterCategory, m_setEquipSortCategory);
		m_SortUI.RegisterEquipSort(m_ssEquip);
		m_SortUI.ResetUI();
		m_loopScrollRectEquip.PrepareCells();
		m_loopScrollRectEquip.TotalCount = list.Count;
		m_loopScrollRectEquip.RefreshCells(bForce: true);
	}

	private void ProcessUIFromCurrentDisplayedSortData(bool bResetScroll = true)
	{
		m_loopScrollRectEquip.TotalCount = m_ssEquip.SortedEquipList.Count;
		if (bResetScroll)
		{
			m_loopScrollRectEquip.SetIndexPosition(0);
		}
		else
		{
			m_loopScrollRectEquip.RefreshCells();
		}
	}

	public void OnSelectEquipSlot(NKCUISlotEquip cItemSlot, NKMEquipItemData equipData)
	{
		if (m_LatestSelectedSlot != null)
		{
			m_LatestSelectedSlot.SetSelected(bSelected: false);
		}
		m_LatestSelectedSlot = cItemSlot;
		cItemSlot.SetSelected(bSelected: true);
		m_InvenEquipSlot?.SetData(equipData);
		NKCUtil.SetGameobjectActive(m_imgBannerEquip, cItemSlot == null || equipData == null);
		NKCUtil.SetGameobjectActive(m_objEquipInfo, cItemSlot != null && equipData != null);
	}

	private void OnClickOk()
	{
		if (m_LatestSelectedSlot != null)
		{
			if (m_NKMItemMiscTemplet.m_ItemMiscSubType == NKM_ITEM_MISC_SUBTYPE.IMST_EQUIP_CHOICE_OPTION_CUSTOM)
			{
				NKCUISelectionEquipDetail.Instance.Open(m_NKMItemMiscTemplet, m_LatestSelectedSlot.GetNKMEquipItemData().m_ItemEquipID);
			}
			else
			{
				NKCPopupSelectionConfirm.Instance.Open(m_NKMItemMiscTemplet, m_LatestSelectedSlot.GetNKMEquipItemData().m_ItemEquipID, 1L, m_LatestSelectedSlot.GetNKMEquipItemData().m_SetOptionId, 0, new List<NKM_STAT_TYPE>());
			}
		}
		else
		{
			Log.Debug("Selected Slot is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUISelectionEquip.cs", 332);
		}
	}
}
