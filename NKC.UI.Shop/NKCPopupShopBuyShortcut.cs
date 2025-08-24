using System.Collections.Generic;
using NKM;
using NKM.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public class NKCPopupShopBuyShortcut : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_SHOP_BUY_SHORTCUT";

	private static NKCPopupShopBuyShortcut m_Instance;

	public Text m_NKM_UI_POPUP_SHOP_BUY_SHORTCUT_TOP_TEXT;

	public GameObject m_RESOURCE_1;

	public Image m_RESOURCE_ICON1;

	public Text m_RESOURCE_TEXT1;

	public GameObject m_RESOURCE_2;

	public Image m_RESOURCE_ICON2;

	public Text m_RESOURCE_TEXT2;

	public RectTransform m_Content;

	public NKCUIShopSlotCard m_pfbNKM_UI_SHOP_SKIN_SLOT;

	public NKCUIComStateButton m_NKM_UI_POPUP_CLOSEBUTTON;

	public NKCUIComStateButton m_NKM_UI_POPUP_SHOP_BUY_SHORTCUT_BG;

	private NKMItemMiscTemplet m_CurMiscTemplet;

	private List<NKCUIShopSlotCard> m_lstShopSlotCard = new List<NKCUIShopSlotCard>();

	public RectTransform m_RESOURCE_LIST_LayoutGruop;

	public NKCUIComResourceButton m_pbfNKM_UI_COMMON_RESOURCE;

	private List<NKCUIComResourceButton> m_lstResourceBtn = new List<NKCUIComResourceButton>();

	[Space]
	[Header("UI 사이즈 조절용")]
	public HorizontalLayoutGroup m_LayOutGroup;

	public RectTransform m_NKM_UI_SHOP_CARD_SLOT;

	private float m_ContectRectWidth;

	private float m_Spacing;

	private float m_CardSlotRectWidth;

	public static NKCPopupShopBuyShortcut Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupShopBuyShortcut>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_SHOP_BUY_SHORTCUT", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCPopupShopBuyShortcut>();
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

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "상품 직접 구매 팝업";

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
		base.gameObject.SetActive(value: false);
		foreach (NKCUIShopSlotCard item in m_lstShopSlotCard)
		{
			NKCUtil.SetGameobjectActive(item.gameObject, bValue: false);
		}
		foreach (NKCUIComResourceButton item2 in m_lstResourceBtn)
		{
			Object.Destroy(item2.gameObject);
		}
		m_lstResourceBtn.Clear();
	}

	private void Init()
	{
		NKCUtil.SetBindFunction(m_NKM_UI_POPUP_CLOSEBUTTON, base.Close);
		NKCUtil.SetBindFunction(m_NKM_UI_POPUP_SHOP_BUY_SHORTCUT_BG, base.Close);
		if (m_LayOutGroup != null)
		{
			m_Spacing = m_LayOutGroup.spacing;
			RectTransform component = m_LayOutGroup.GetComponent<RectTransform>();
			if (component != null)
			{
				m_ContectRectWidth = component.rect.width;
			}
		}
		if (m_NKM_UI_SHOP_CARD_SLOT != null)
		{
			m_CardSlotRectWidth = m_NKM_UI_SHOP_CARD_SLOT.rect.width;
		}
	}

	public static void Open(int itemID)
	{
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(itemID);
		if (itemMiscTempletByID != null)
		{
			Open(itemMiscTempletByID);
		}
	}

	public static void Open(NKMItemMiscTemplet templet)
	{
		NKCShopManager.FetchShopItemList(NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL, delegate(bool bSuccess)
		{
			if (bSuccess)
			{
				Instance.OpenWindow(templet);
			}
			else
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_SHOP_WAS_NOT_ABLE_TO_GET_PRODUCT_LIST_FROM_SERVER);
			}
		});
	}

	private void OpenWindow(NKMItemMiscTemplet templet)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_CurMiscTemplet = templet;
		UpdateUI();
		UIOpened();
	}

	public void UpdateUI()
	{
		if (m_CurMiscTemplet == null || m_CurMiscTemplet.m_lstRecommandProductItemIfNotEnough == null || m_CurMiscTemplet.m_lstRecommandProductItemIfNotEnough.Count <= 0)
		{
			return;
		}
		List<ShopItemTemplet> list = new List<ShopItemTemplet>();
		List<ShopItemTemplet> list2 = new List<ShopItemTemplet>();
		for (int i = 0; i < m_CurMiscTemplet.m_lstRecommandProductItemIfNotEnough.Count; i++)
		{
			int num = m_CurMiscTemplet.m_lstRecommandProductItemIfNotEnough[i];
			ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(num);
			if (NKCShopManager.CanExhibitItem(shopItemTemplet, bIncludeLockedItemWithReason: true))
			{
				if (NKCShopManager.GetBuyCountLeft(num) == 0)
				{
					list2.Add(shopItemTemplet);
				}
				else
				{
					list.Add(shopItemTemplet);
				}
			}
		}
		list.AddRange(list2);
		while (m_lstShopSlotCard.Count < list.Count)
		{
			NKCUIShopSlotCard nKCUIShopSlotCard = Object.Instantiate(m_pfbNKM_UI_SHOP_SKIN_SLOT);
			if (nKCUIShopSlotCard != null)
			{
				nKCUIShopSlotCard.Init(OnBtnProductBuy, null);
				RectTransform component = nKCUIShopSlotCard.GetComponent<RectTransform>();
				if (component != null)
				{
					component.localScale = Vector2.one;
				}
				nKCUIShopSlotCard.transform.SetParent(m_Content, worldPositionStays: false);
				nKCUIShopSlotCard.transform.localPosition = Vector3.zero;
				m_lstShopSlotCard.Add(nKCUIShopSlotCard);
			}
		}
		for (int j = 0; j < m_lstShopSlotCard.Count; j++)
		{
			NKCUIShopSlotCard nKCUIShopSlotCard2 = m_lstShopSlotCard[j];
			if (j < list.Count)
			{
				ShopItemTemplet shopItemTemplet2 = list[j];
				NKCUtil.SetGameobjectActive(nKCUIShopSlotCard2, bValue: true);
				nKCUIShopSlotCard2.SetData(null, shopItemTemplet2, NKCShopManager.GetBuyCountLeft(shopItemTemplet2.m_ProductID), NKCUIShop.IsFirstBuy(shopItemTemplet2.m_ProductID));
			}
			else
			{
				NKCUtil.SetGameobjectActive(nKCUIShopSlotCard2, bValue: false);
			}
		}
		NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
		NKCUtil.SetGameobjectActive(m_RESOURCE_1, bValue: true);
		NKCUtil.SetGameobjectActive(m_RESOURCE_2, bValue: true);
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(102);
		if (itemMiscTempletByID != null)
		{
			Sprite orLoadMiscItemSmallIcon = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(itemMiscTempletByID);
			NKCUtil.SetImageSprite(m_RESOURCE_ICON1, orLoadMiscItemSmallIcon);
			NKCUtil.SetLabelText(m_RESOURCE_TEXT1, inventoryData.GetCountMiscItem(102).ToString("#,##0"));
		}
		itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(101);
		if (itemMiscTempletByID != null)
		{
			Sprite orLoadMiscItemSmallIcon2 = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(itemMiscTempletByID);
			NKCUtil.SetImageSprite(m_RESOURCE_ICON2, orLoadMiscItemSmallIcon2);
			NKCUtil.SetLabelText(m_RESOURCE_TEXT2, inventoryData.GetCountMiscItem(101).ToString("#,##0"));
		}
		if ((float)list.Count * (m_CardSlotRectWidth + m_Spacing) >= m_ContectRectWidth)
		{
			m_Content.pivot = new Vector2(0f, 0.5f);
		}
		else
		{
			m_Content.pivot = new Vector2(0.5f, 0.5f);
		}
		list.Sort((ShopItemTemplet a, ShopItemTemplet b) => a.m_OrderList.CompareTo(b.m_OrderList));
		HashSet<int> hashSet = new HashSet<int>();
		foreach (ShopItemTemplet item in list)
		{
			hashSet.Add(item.m_PriceItemID);
		}
		foreach (int item2 in hashSet)
		{
			if (item2 != 101 && item2 != 102 && item2 != 0)
			{
				NKCUIComResourceButton nKCUIComResourceButton = Object.Instantiate(m_pbfNKM_UI_COMMON_RESOURCE);
				if (nKCUIComResourceButton != null)
				{
					nKCUIComResourceButton.gameObject.transform.SetParent(m_RESOURCE_LIST_LayoutGruop, worldPositionStays: false);
					nKCUIComResourceButton.SetData(item2, (int)inventoryData.GetCountMiscItem(item2));
					m_lstResourceBtn.Add(nKCUIComResourceButton);
				}
			}
		}
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_SHOP_BUY_SHORTCUT_TOP_TEXT, NKCStringTable.GetString("SI_DP_ITEM_NOT_ENOUGH_PRODUCT_POPUP_DESC", false, m_CurMiscTemplet.GetItemName()));
	}

	private void OnBtnProductBuy(int ProductID)
	{
		NKCShopManager.OnBtnProductBuy(ProductID, bSupply: false);
	}
}
