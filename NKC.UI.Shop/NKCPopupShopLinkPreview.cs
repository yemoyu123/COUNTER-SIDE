using System.Collections.Generic;
using NKM;
using NKM.Shop;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public class NKCPopupShopLinkPreview : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_shop";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_SHOP_BUY_PACKAGE_PREVIEW";

	private static NKCPopupShopLinkPreview m_Instance;

	public RectTransform m_rtSlotPool;

	public NKCUIShopSlotPreview m_pfbPreviewSlot;

	public LoopScrollRect m_loopScroll;

	public NKCUIComStateButton m_csbtnOK;

	private List<ShopItemTemplet> m_lstLinkedItems;

	private NKCPopupShopPackageConfirm m_UIPackagePreview;

	private Stack<RectTransform> m_stkObj = new Stack<RectTransform>();

	public static NKCPopupShopLinkPreview Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupShopLinkPreview>("ab_ui_nkm_ui_shop", "NKM_UI_POPUP_SHOP_BUY_PACKAGE_PREVIEW", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupShopLinkPreview>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "LinkPreview";

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

	public override void CloseInternal()
	{
		if (m_UIPackagePreview != null)
		{
			m_UIPackagePreview.Close();
			m_UIPackagePreview = null;
		}
		base.gameObject.SetActive(value: false);
	}

	private void InitUI()
	{
		m_loopScroll.dOnGetObject += GetObject;
		m_loopScroll.dOnReturnObject += ReturnObject;
		m_loopScroll.dOnProvideData += ProvideData;
		m_loopScroll.PrepareCells();
		NKCUtil.SetScrollHotKey(m_loopScroll);
		NKCUtil.SetButtonClickDelegate(m_csbtnOK, base.Close);
		NKCUtil.SetHotkey(m_csbtnOK, HotkeyEventType.Confirm);
	}

	public void Open(int ProductID)
	{
		m_lstLinkedItems = NKCShopManager.GetLinkedItem(ProductID);
		if (m_lstLinkedItems != null && m_lstLinkedItems.Count != 0)
		{
			base.gameObject.SetActive(value: true);
			m_loopScroll.TotalCount = m_lstLinkedItems.Count;
			m_loopScroll.SetIndexPosition(0);
			UIOpened();
		}
	}

	private RectTransform GetObject(int index)
	{
		if (m_stkObj.Count > 0)
		{
			RectTransform rectTransform = m_stkObj.Pop();
			NKCUtil.SetGameobjectActive(rectTransform, bValue: true);
			return rectTransform;
		}
		if (m_pfbPreviewSlot == null)
		{
			Debug.LogError("Scout slot prefab null!");
			return null;
		}
		NKCUIShopSlotPreview nKCUIShopSlotPreview = Object.Instantiate(m_pfbPreviewSlot);
		nKCUIShopSlotPreview.Init(OnSelectSlot, null);
		return nKCUIShopSlotPreview.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform go)
	{
		NKCUtil.SetGameobjectActive(go, bValue: false);
		go.SetParent(m_rtSlotPool);
		m_stkObj.Push(go.GetComponent<RectTransform>());
	}

	private void ProvideData(Transform tr, int idx)
	{
		if (idx < 0)
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
			return;
		}
		NKCUIShopSlotPreview component = tr.GetComponent<NKCUIShopSlotPreview>();
		if (!(component == null))
		{
			ShopItemTemplet shopItemTemplet = m_lstLinkedItems[idx];
			component.SetData(null, shopItemTemplet, NKCShopManager.GetBuyCountLeft(shopItemTemplet.m_ProductID));
		}
	}

	private void OnSelectSlot(int productID)
	{
		ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(productID);
		if (shopItemTemplet == null || shopItemTemplet.m_ItemType != NKM_REWARD_TYPE.RT_MISC)
		{
			return;
		}
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(shopItemTemplet.m_ItemID);
		if (itemMiscTempletByID != null && itemMiscTempletByID.IsPackageItem)
		{
			if (m_UIPackagePreview == null)
			{
				m_UIPackagePreview = NKCPopupShopPackageConfirm.OpenNewInstance();
			}
			m_UIPackagePreview.OpenPreview(shopItemTemplet);
		}
	}
}
