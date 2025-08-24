using System.Collections.Generic;
using NKM;
using NKM.Item;
using NKM.Shop;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public class NKCPopupShopCustomPackage : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_shop";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_SHOP_BUY_PACKAGE_CUSTOM";

	private static NKCPopupShopCustomPackage m_Instance;

	[Header("왼쪽 패키지 내용")]
	public List<NKCUISlotCustomData> m_lstPackageSlot;

	public Sprite m_spEmpty;

	[Header("왼쪽 아래 아이템 설명부")]
	public GameObject m_objRootDescItemNotSelected;

	public GameObject m_objRootDescItemSelected;

	public Text m_lbItemTitle;

	public Text m_lbItemDesc;

	[Header("오른쪽 스크롤바")]
	public NKCUISlotCustomData m_pfbSlot;

	public LoopScrollRect m_srSelection;

	[Header("아래쪽 버튼")]
	public NKCUIComStateButton m_csbtnCancel;

	public NKCUIComStateButton m_csbtnConfirm;

	[Header("디테일 버튼")]
	public NKCUIComStateButton m_csbtnSkinPreview;

	public NKCUIComStateButton m_csbtnProbability;

	public NKCUIComStateButton m_csbtnDetail;

	private NKCUIShop.OnProductBuyDelegate dOnProductBuy;

	private List<int> m_lstSelectedItems = new List<int>();

	private ShopItemTemplet m_cNKMShopItemTemplet;

	private NKMItemMiscTemplet m_targetItemTemplet;

	private int m_currentPackageSlotIndex = -1;

	private int m_customItemStartIndex;

	private List<NKMCustomPackageElement> m_lstCurrentSelectableElements;

	public static NKCPopupShopCustomPackage Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupShopCustomPackage>("ab_ui_nkm_ui_shop", "NKM_UI_POPUP_SHOP_BUY_PACKAGE_CUSTOM", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupShopCustomPackage>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.DEFAULT;

	public static bool HasInstance => m_Instance != null;

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

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCStringTable.GetString("SI_PF_SHOP_ITEM_PURCHASE_CONFIRM");

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

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnCancel, base.Close);
		NKCUtil.SetButtonClickDelegate(m_csbtnConfirm, OnOK);
		NKCUtil.SetHotkey(m_csbtnConfirm, HotkeyEventType.Confirm);
		NKCUtil.SetButtonClickDelegate(m_csbtnSkinPreview, OnDetail);
		NKCUtil.SetButtonClickDelegate(m_csbtnProbability, OnDetail);
		NKCUtil.SetButtonClickDelegate(m_csbtnDetail, OnDetail);
		foreach (NKCUISlotCustomData item in m_lstPackageSlot)
		{
			if (item != null)
			{
				item.Init();
				if (m_spEmpty != null)
				{
					item.Slot.SetCustomizedEmptySP(m_spEmpty);
				}
			}
		}
		m_srSelection.dOnGetObject += GetObject;
		m_srSelection.dOnReturnObject += ReturnObject;
		m_srSelection.dOnProvideData += ProvideData;
		m_srSelection.PrepareCells();
		NKCUtil.SetScrollHotKey(m_srSelection);
	}

	private void OnOK()
	{
		if (!NKCShopManager.IsAllCustomSlotSelected(m_targetItemTemplet, m_lstSelectedItems))
		{
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_CUSTOM_PACKAGE_INVALID_SELECTION_DATA);
		}
		else
		{
			NKCPopupShopPackageConfirm.Instance.Open(m_cNKMShopItemTemplet, OnBuyConfirm, m_lstSelectedItems);
		}
	}

	private void OnBuyConfirm(int ProductID, int ProductCount, List<int> lstSelection)
	{
		Close();
		dOnProductBuy?.Invoke(m_cNKMShopItemTemplet.m_ProductID, 1, m_lstSelectedItems);
	}

	public void Open(ShopItemTemplet shopItemTemplet, NKCUIShop.OnProductBuyDelegate onProductBuy)
	{
		if (shopItemTemplet == null)
		{
			Debug.LogError("NKCPopupShopCustomPackage opened null ShopItemTemplet");
			base.gameObject.SetActive(value: false);
			return;
		}
		if (shopItemTemplet.m_ItemType != NKM_REWARD_TYPE.RT_MISC)
		{
			Debug.LogError("NKCPopupShopCustomPackage opened with non-misc ShopItemTemplet");
			base.gameObject.SetActive(value: false);
			return;
		}
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(shopItemTemplet.m_ItemID);
		if (itemMiscTempletByID == null || !itemMiscTempletByID.IsCustomPackageItem)
		{
			Debug.LogError("NKCPopupShopCustomPackage opened with bad misc item");
			base.gameObject.SetActive(value: false);
			return;
		}
		if (itemMiscTempletByID.CustomPackageTemplets == null || itemMiscTempletByID.CustomPackageTemplets.Count == 0)
		{
			Debug.LogError("NKCPopupShopCustomPackage opened but customPackage has no selectable Items");
			base.gameObject.SetActive(value: false);
			return;
		}
		m_currentPackageSlotIndex = -1;
		m_customItemStartIndex = 0;
		m_lstCurrentSelectableElements = null;
		m_cNKMShopItemTemplet = shopItemTemplet;
		m_targetItemTemplet = itemMiscTempletByID;
		dOnProductBuy = onProductBuy;
		m_lstSelectedItems.Clear();
		for (int i = 0; i < m_targetItemTemplet.CustomPackageTemplets.Count; i++)
		{
			m_lstSelectedItems.Add(-1);
		}
		UIOpened();
		UpdateCurrentPackageItems(m_targetItemTemplet);
		UpdateSelectionItems(bResetScrollPosition: true);
		UpdateConfirmButton();
		NKCUtil.SetGameobjectActive(m_objRootDescItemSelected, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRootDescItemNotSelected, bValue: true);
	}

	private void UpdateCurrentPackageItems(NKMItemMiscTemplet itemTemplet)
	{
		if (itemTemplet == null)
		{
			return;
		}
		List<NKMRandomBoxItemTemplet> randomBoxItemTempletList = NKCRandomBoxManager.GetRandomBoxItemTempletList(itemTemplet.m_RewardGroupID);
		if (itemTemplet.m_RewardGroupID != 0 && randomBoxItemTempletList == null)
		{
			Debug.LogError("rewardgroup null! ID : " + itemTemplet.m_RewardGroupID);
		}
		int num = 0;
		if (randomBoxItemTempletList != null)
		{
			for (int i = 0; i < randomBoxItemTempletList.Count; i++)
			{
				if (num >= m_lstPackageSlot.Count)
				{
					Debug.LogError("Package Item has too much item!");
					return;
				}
				NKCUISlotCustomData nKCUISlotCustomData = m_lstPackageSlot[num];
				NKCUtil.SetGameobjectActive(nKCUISlotCustomData, bValue: true);
				num++;
				NKMRandomBoxItemTemplet nKMRandomBoxItemTemplet = randomBoxItemTempletList[i];
				NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeRewardTypeData(nKMRandomBoxItemTemplet.m_reward_type, nKMRandomBoxItemTemplet.m_RewardID, nKMRandomBoxItemTemplet.TotalQuantity_Max);
				nKCUISlotCustomData.SetData(-1, slotData, bShowName: false, slotData.eType == NKCUISlot.eSlotMode.ItemMisc, bEnableLayoutElement: false, null);
				nKCUISlotCustomData.Slot.SetOnClickAction(default(NKCUISlot.SlotClickType));
				bool redudantMark = NKCShopManager.WillOverflowOnGain(nKMRandomBoxItemTemplet.m_reward_type, nKMRandomBoxItemTemplet.m_RewardID, nKMRandomBoxItemTemplet.TotalQuantity_Max) || NKCShopManager.IsHaveUnit(nKMRandomBoxItemTemplet.m_reward_type, nKMRandomBoxItemTemplet.m_RewardID);
				nKCUISlotCustomData.Slot.SetRedudantMark(redudantMark);
				NKCShopManager.ShowShopItemCashCount(nKCUISlotCustomData.Slot, slotData, nKMRandomBoxItemTemplet.FreeQuantity_Max, nKMRandomBoxItemTemplet.PaidQuantity_Max);
			}
		}
		m_customItemStartIndex = num;
		if (itemTemplet.CustomPackageTemplets != null)
		{
			for (int j = 0; j < itemTemplet.CustomPackageTemplets.Count; j++)
			{
				if (num >= m_lstPackageSlot.Count)
				{
					Debug.LogError("Package Item has too much item!");
					return;
				}
				NKCUISlotCustomData nKCUISlotCustomData2 = m_lstPackageSlot[num];
				NKCUtil.SetGameobjectActive(nKCUISlotCustomData2, bValue: true);
				num++;
				int index = m_lstSelectedItems[j];
				NKMCustomPackageElement nKMCustomPackageElement = itemTemplet.CustomPackageTemplets[j].Get(index);
				if (nKMCustomPackageElement != null)
				{
					NKCUISlot.SlotData slotData2 = NKCUISlot.SlotData.MakeRewardTypeData(nKMCustomPackageElement.RewardType, nKMCustomPackageElement.RewardId, nKMCustomPackageElement.TotalRewardCount);
					nKCUISlotCustomData2.SetData(j, slotData2, bShowName: false, slotData2.eType == NKCUISlot.eSlotMode.ItemMisc, bEnableLayoutElement: false, OnSelectPackageSlot);
					NKCShopManager.ShowShopItemCashCount(nKCUISlotCustomData2.Slot, slotData2, nKMCustomPackageElement.FreeRewardCount, nKMCustomPackageElement.PaidRewardCount);
					if (NKCShopManager.WillOverflowOnGain(nKMCustomPackageElement.RewardType, nKMCustomPackageElement.RewardId, nKMCustomPackageElement.TotalRewardCount))
					{
						nKCUISlotCustomData2.Slot.SetHaveCountString(bShow: true, NKCStringTable.GetString("SI_DP_ICON_SLOT_ALREADY_HAVE"));
					}
					else if (NKCShopManager.IsHaveUnit(nKMCustomPackageElement.RewardType, nKMCustomPackageElement.RewardId))
					{
						nKCUISlotCustomData2.Slot.SetHaveCountString(bShow: true, NKCStringTable.GetString("SI_DP_ICON_SLOT_ALREADY_HAVE"));
					}
					else if (NKCShopManager.IsCustomPackageSelectionHasDuplicate(itemTemplet, j, m_lstSelectedItems, bIgnoreIfFirstItem: false))
					{
						nKCUISlotCustomData2.Slot.SetHaveCountString(bShow: true, NKCStringTable.GetString("SI_DP_SHOP_CUSTOM_DUPLICATE"));
					}
					else
					{
						nKCUISlotCustomData2.Slot.SetHaveCountString(bShow: false, null);
					}
					nKCUISlotCustomData2.Slot.SetShowArrowBGText(bSet: true);
					nKCUISlotCustomData2.Slot.SetArrowBGText(NKCStringTable.GetString("SI_DP_SHOP_SLOT_CHOICE"), new Color(2f / 3f, 0.03137255f, 0.03137255f));
				}
				else
				{
					nKCUISlotCustomData2.Slot.SetEmpty();
					nKCUISlotCustomData2.SetOnClick(OnSelectPackageSlot);
					nKCUISlotCustomData2.m_Data = j;
				}
				nKCUISlotCustomData2.Slot.SetSelected(m_currentPackageSlotIndex == j);
			}
		}
		for (int k = num; k < m_lstPackageSlot.Count; k++)
		{
			NKCUtil.SetGameobjectActive(m_lstPackageSlot[k], bValue: false);
		}
	}

	private void OnSelectPackageSlot(NKCUISlot.SlotData slotData, bool bLocked, int index)
	{
		if (index != m_currentPackageSlotIndex)
		{
			NKCUISlotCustomData customSlot = GetCustomSlot(m_currentPackageSlotIndex);
			if (customSlot != null)
			{
				customSlot.Slot.SetSelected(bSelected: false);
			}
			m_currentPackageSlotIndex = index;
			NKCUISlotCustomData customSlot2 = GetCustomSlot(index);
			if (customSlot2 != null)
			{
				customSlot2.Slot.SetSelected(bSelected: true);
			}
			SetDescriptionData(index);
			UpdateSelectionItems(bResetScrollPosition: true);
		}
	}

	private void SetDescriptionData(int packageSlotIndex)
	{
		int index = m_lstSelectedItems[packageSlotIndex];
		NKMCustomPackageElement nKMCustomPackageElement = m_targetItemTemplet.CustomPackageTemplets[packageSlotIndex].Get(index);
		if (nKMCustomPackageElement != null)
		{
			NKCUtil.SetGameobjectActive(m_objRootDescItemSelected, bValue: true);
			NKCUtil.SetGameobjectActive(m_objRootDescItemNotSelected, bValue: false);
			NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeRewardTypeData(nKMCustomPackageElement.RewardType, nKMCustomPackageElement.RewardId, nKMCustomPackageElement.FreeRewardCount);
			NKCUtil.SetLabelText(m_lbItemTitle, NKCUISlot.GetName(slotData));
			NKCUtil.SetLabelText(m_lbItemDesc, NKCUISlot.GetDesc(slotData, bFull: true));
			NKCUtil.SetGameobjectActive(m_csbtnSkinPreview, nKMCustomPackageElement.RewardType == NKM_REWARD_TYPE.RT_SKIN);
			if (nKMCustomPackageElement.RewardType == NKM_REWARD_TYPE.RT_MISC)
			{
				NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(nKMCustomPackageElement.RewardId);
				NKCUtil.SetGameobjectActive(m_csbtnProbability, itemMiscTempletByID.IsUsable() && itemMiscTempletByID.IsRatioOpened());
				NKCUtil.SetGameobjectActive(m_csbtnDetail, itemMiscTempletByID.IsUsable() && itemMiscTempletByID.IsChoiceItem());
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_csbtnProbability, bValue: false);
				NKCUtil.SetGameobjectActive(m_csbtnDetail, bValue: false);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objRootDescItemSelected, bValue: false);
			NKCUtil.SetGameobjectActive(m_objRootDescItemNotSelected, bValue: true);
			NKCUtil.SetGameobjectActive(m_csbtnSkinPreview, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnProbability, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnDetail, bValue: false);
		}
	}

	private NKCUISlotCustomData GetCustomSlot(int customSlotIndex)
	{
		if (customSlotIndex < 0)
		{
			return null;
		}
		int num = m_customItemStartIndex + customSlotIndex;
		if (num >= m_lstPackageSlot.Count)
		{
			return null;
		}
		return m_lstPackageSlot[num];
	}

	private RectTransform GetObject(int idx)
	{
		NKCUISlotCustomData nKCUISlotCustomData = Object.Instantiate(m_pfbSlot);
		nKCUISlotCustomData.Init();
		return nKCUISlotCustomData.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		if (tr != null)
		{
			tr.SetParent(null);
		}
		tr.gameObject.SetActive(value: false);
		Object.Destroy(tr.gameObject);
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCUISlotCustomData component = tr.GetComponent<NKCUISlotCustomData>();
		if (component == null)
		{
			tr.gameObject.SetActive(value: false);
			return;
		}
		if (idx < 0)
		{
			tr.gameObject.SetActive(value: false);
			return;
		}
		if (m_lstCurrentSelectableElements == null || idx >= m_lstCurrentSelectableElements.Count)
		{
			tr.gameObject.SetActive(value: false);
			return;
		}
		NKMCustomPackageElement nKMCustomPackageElement = m_lstCurrentSelectableElements[idx];
		NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeRewardTypeData(nKMCustomPackageElement.RewardType, nKMCustomPackageElement.RewardId, nKMCustomPackageElement.TotalRewardCount);
		component.SetData(nKMCustomPackageElement.Index, slotData, bShowName: false, slotData.eType == NKCUISlot.eSlotMode.ItemMisc, bEnableLayoutElement: false, OnSelectSelectableSlot);
		bool redudantMark = NKCShopManager.WillOverflowOnGain(nKMCustomPackageElement.RewardType, nKMCustomPackageElement.RewardId, nKMCustomPackageElement.TotalRewardCount) || NKCShopManager.IsHaveUnit(nKMCustomPackageElement.RewardType, nKMCustomPackageElement.RewardId);
		component.Slot.SetRedudantMark(redudantMark);
		component.Slot.SetSelected(m_lstSelectedItems[m_currentPackageSlotIndex] == nKMCustomPackageElement.Index);
		NKCShopManager.ShowShopItemCashCount(component.Slot, slotData, nKMCustomPackageElement.FreeRewardCount, nKMCustomPackageElement.PaidRewardCount);
	}

	private void UpdateSelectionItems(bool bResetScrollPosition)
	{
		NKMCustomPackageGroupTemplet selectedCustomPackageGroup = GetSelectedCustomPackageGroup();
		if (selectedCustomPackageGroup == null)
		{
			m_lstCurrentSelectableElements = null;
			m_srSelection.TotalCount = 0;
			m_srSelection.RefreshCells();
			return;
		}
		m_lstCurrentSelectableElements = new List<NKMCustomPackageElement>(selectedCustomPackageGroup.OpenedElements);
		m_srSelection.TotalCount = m_lstCurrentSelectableElements.Count;
		if (bResetScrollPosition)
		{
			m_srSelection.SetIndexPosition(0);
		}
		else
		{
			m_srSelection.RefreshCells();
		}
	}

	private void OnSelectSelectableSlot(NKCUISlot.SlotData slotData, bool bLocked, int data)
	{
		m_lstSelectedItems[m_currentPackageSlotIndex] = data;
		UpdateCurrentPackageItems(m_targetItemTemplet);
		SetDescriptionData(m_currentPackageSlotIndex);
		UpdateConfirmButton();
		m_srSelection.RefreshCells();
	}

	private NKMCustomPackageGroupTemplet GetSelectedCustomPackageGroup()
	{
		if (m_currentPackageSlotIndex < 0)
		{
			return null;
		}
		if (m_currentPackageSlotIndex >= m_targetItemTemplet.CustomPackageTemplets.Count)
		{
			return null;
		}
		return m_targetItemTemplet.CustomPackageTemplets[m_currentPackageSlotIndex];
	}

	private void UpdateConfirmButton()
	{
		m_csbtnConfirm.SetLock(!NKCShopManager.IsAllCustomSlotSelected(m_targetItemTemplet, m_lstSelectedItems));
	}

	private void OnDetail()
	{
		int index = m_lstSelectedItems[m_currentPackageSlotIndex];
		NKMCustomPackageElement nKMCustomPackageElement = m_targetItemTemplet.CustomPackageTemplets[m_currentPackageSlotIndex].Get(index);
		switch (nKMCustomPackageElement.RewardType)
		{
		case NKM_REWARD_TYPE.RT_SKIN:
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(nKMCustomPackageElement.RewardId);
			if (skinTemplet != null)
			{
				NKCUIShopSkinPopup.Instance.OpenForSkinInfo(skinTemplet, m_cNKMShopItemTemplet.m_ProductID);
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_MISC:
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(nKMCustomPackageElement.RewardId);
			if (itemMiscTempletByID.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_RANDOMBOX && itemMiscTempletByID.IsRatioOpened())
			{
				NKCUISlotListViewer newInstance = NKCUISlotListViewer.GetNewInstance();
				if (newInstance != null)
				{
					newInstance.OpenItemBoxRatio(nKMCustomPackageElement.RewardId);
				}
			}
			else if (itemMiscTempletByID.IsChoiceItem())
			{
				NKCUISlotListViewer newInstance2 = NKCUISlotListViewer.GetNewInstance();
				if (newInstance2 != null)
				{
					newInstance2.OpenChoiceInfo(nKMCustomPackageElement.RewardId);
				}
			}
			break;
		}
		}
	}
}
