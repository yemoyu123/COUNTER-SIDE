using System;
using System.Collections.Generic;
using NKC.Publisher;
using NKC.UI.NPC;
using NKM;
using NKM.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public class NKCPopupShopBannerNotice : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_SHOP";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_SHOP_UNLOCK_BANNER";

	private static NKCPopupShopBannerNotice m_Instance;

	public NKCUIComStateButton m_btnClose;

	public NKCUINPCShop m_NPCShop;

	public NKCUIShopSlotPrefab m_PrefabSlot;

	public NKCUIComStateButton m_btnBuy;

	public NKCUIPriceTag m_PriceTag;

	private ShopItemTemplet m_NKMShopItemTemplet;

	private NKCAssetInstanceData m_prefabInstance;

	public Transform m_trContent;

	public Image m_imgBG;

	private Action dOnClose;

	public NKCUIShopSlotCard m_pfbNKM_UI_SHOP_SKIN_SLOT;

	private List<NKCUIShopSlotCard> m_lstShopSlotCard = new List<NKCUIShopSlotCard>();

	private List<ShopItemTemplet> m_lstShopItemTemplet = new List<ShopItemTemplet>();

	public static NKCPopupShopBannerNotice Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupShopBannerNotice>("AB_UI_NKM_UI_SHOP", "NKM_UI_POPUP_SHOP_UNLOCK_BANNER", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupShopBannerNotice>();
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

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Normal;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => string.Empty;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void Init()
	{
		if (m_NPCShop != null)
		{
			m_NPCShop.Init();
		}
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
		NKCUtil.SetBindFunction(m_btnClose, base.Close);
		NKCUtil.SetBindFunction(m_btnBuy, OnProductBuy);
		m_PrefabSlot?.Init(OnBtnProductBuy, null);
	}

	public static void Open(List<int> lstProductIDs, Action onClose)
	{
		foreach (int lstProductID in lstProductIDs)
		{
			ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(lstProductID);
			if (shopItemTemplet != null && shopItemTemplet.m_bUnlockBanner)
			{
				switch (shopItemTemplet.m_UnlockInfo.eReqType)
				{
				case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_STAGE:
					NKCUtil.SetImageSprite(Instance.m_imgBG, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_shop_thumbnail", "NKM_UI_POPUP_SHOP_UNLOCK_BANNER_EPISODE", tryParseAssetName: true));
					break;
				case STAGE_UNLOCK_REQ_TYPE.SURT_PLAYER_LEVEL:
					NKCUtil.SetImageSprite(Instance.m_imgBG, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_shop_thumbnail", "NKM_UI_POPUP_SHOP_UNLOCK_BANNER_LEVELUP", tryParseAssetName: true));
					break;
				default:
					NKCUtil.SetImageSprite(Instance.m_imgBG, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_shop_thumbnail", "NKM_UI_POPUP_SHOP_UNLOCK_BANNER_COMMON", tryParseAssetName: true));
					break;
				}
			}
		}
		NKCShopManager.FetchShopItemList(NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL, delegate(bool bSuccess)
		{
			if (bSuccess)
			{
				if (!NKCPublisherModule.InAppPurchase.CheckReceivedBillingProductList)
				{
					NKCPublisherModule.InAppPurchase.RequestBillingProductList(delegate(NKC_PUBLISHER_RESULT_CODE resultCode, string add)
					{
						if (resultCode == NKC_PUBLISHER_RESULT_CODE.NPRC_OK)
						{
							Instance._Open(lstProductIDs, onClose);
						}
					});
				}
				else
				{
					Instance._Open(lstProductIDs, onClose);
				}
			}
			else
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_SHOP_WAS_NOT_ABLE_TO_GET_PRODUCT_LIST_FROM_SERVER);
			}
		}, bForceRefreshServerItemList: true);
	}

	public static void Open(int productID, Action onClose)
	{
		NKCShopManager.FetchShopItemList(NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL, delegate(bool bSuccess)
		{
			if (bSuccess)
			{
				if (!NKCPublisherModule.InAppPurchase.CheckReceivedBillingProductList)
				{
					NKCPublisherModule.InAppPurchase.RequestBillingProductList(delegate(NKC_PUBLISHER_RESULT_CODE resultCode, string add)
					{
						if (resultCode == NKC_PUBLISHER_RESULT_CODE.NPRC_OK)
						{
							Instance._Open(productID, onClose);
						}
					});
				}
				else
				{
					Instance._Open(productID, onClose);
				}
			}
			else
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_SHOP_WAS_NOT_ABLE_TO_GET_PRODUCT_LIST_FROM_SERVER);
			}
		}, bForceRefreshServerItemList: true);
	}

	private void _Open(int productID, Action onClose)
	{
		ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(productID);
		if (shopItemTemplet != null)
		{
			_Open(shopItemTemplet, onClose);
		}
	}

	private void _Open(List<int> lstShopIds, Action onClose)
	{
		CleanUp();
		m_lstShopItemTemplet.Clear();
		foreach (int lstShopId in lstShopIds)
		{
			ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(lstShopId);
			if (shopItemTemplet != null)
			{
				m_lstShopItemTemplet.Add(shopItemTemplet);
			}
		}
		if (m_lstShopItemTemplet.Count > 0)
		{
			m_NKMShopItemTemplet = null;
			dOnClose = onClose;
			RefreshUI();
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			UIOpened();
		}
	}

	private void _Open(ShopItemTemplet shopTemplet, Action onClose)
	{
		CleanUp();
		m_NKMShopItemTemplet = shopTemplet;
		dOnClose = onClose;
		m_PrefabSlot.SetData(null, shopTemplet);
		SetButton();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		UIOpened();
	}

	private void OnBtnProductBuy(int ProductID)
	{
		NKCShopManager.OnBtnProductBuy(ProductID, bSupply: false);
	}

	private void CleanUp()
	{
		if (m_prefabInstance != null)
		{
			NKCAssetResourceManager.CloseInstance(m_prefabInstance);
			m_prefabInstance = null;
		}
	}

	private void SetButton()
	{
		m_PriceTag?.SetData(m_NKMShopItemTemplet, showMinus: false, changeColor: false);
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		CleanUp();
		foreach (NKCUIShopSlotCard item in m_lstShopSlotCard)
		{
			NKCUtil.SetGameobjectActive(item.gameObject, bValue: false);
		}
		dOnClose?.Invoke();
	}

	public void OnProductBuy()
	{
		NKCPopupShopBuyConfirm.Instance.Open(m_NKMShopItemTemplet, OnBuyConrifim);
	}

	private void OnBuyConrifim(int id, int count, List<int> lstSelection)
	{
		NKCShopManager.TryProductBuy(id, count);
		Close();
	}

	public void RefreshUI()
	{
		while (m_lstShopSlotCard.Count < m_lstShopItemTemplet.Count)
		{
			NKCUIShopSlotCard nKCUIShopSlotCard = UnityEngine.Object.Instantiate(m_pfbNKM_UI_SHOP_SKIN_SLOT);
			if (nKCUIShopSlotCard != null)
			{
				nKCUIShopSlotCard.Init(OnBtnProductBuy, null);
				RectTransform component = nKCUIShopSlotCard.GetComponent<RectTransform>();
				if (component != null)
				{
					component.localScale = Vector2.one;
				}
				nKCUIShopSlotCard.transform.SetParent(m_trContent, worldPositionStays: false);
				nKCUIShopSlotCard.transform.localPosition = Vector3.zero;
				m_lstShopSlotCard.Add(nKCUIShopSlotCard);
			}
		}
		for (int i = 0; i < m_lstShopSlotCard.Count; i++)
		{
			NKCUIShopSlotCard nKCUIShopSlotCard2 = m_lstShopSlotCard[i];
			if (i < m_lstShopItemTemplet.Count)
			{
				ShopItemTemplet shopItemTemplet = m_lstShopItemTemplet[i];
				NKCUtil.SetGameobjectActive(nKCUIShopSlotCard2, bValue: true);
				nKCUIShopSlotCard2.SetData(null, shopItemTemplet, NKCShopManager.GetBuyCountLeft(shopItemTemplet.m_ProductID), NKCUIShop.IsFirstBuy(shopItemTemplet.m_ProductID));
			}
			else
			{
				NKCUtil.SetGameobjectActive(nKCUIShopSlotCard2, bValue: false);
			}
		}
		SetButton();
	}
}
