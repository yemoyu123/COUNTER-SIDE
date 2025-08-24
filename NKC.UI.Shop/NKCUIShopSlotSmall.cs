using System;
using NKC.Publisher;
using NKM;
using NKM.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public class NKCUIShopSlotSmall : NKCUIShopSlotBase
{
	[Header("카드형 슬롯 전용")]
	public NKCUISlot m_Slot;

	public Image m_Image;

	public GameObject m_objSalePriceRoot;

	public Text m_lbSaleOldPrice;

	public Image m_imgSalePrice;

	public Text m_lbSalePrice;

	public GameObject m_objNormalPriceRoot;

	public Text m_lbNormalPrice;

	public Image m_imgPrice;

	public GameObject m_objPriceParent;

	public Text m_lbFreePrice;

	public override void Init(OnBuy onBuy, OnRefreshRequired onRefreshRequired)
	{
		base.Init(onBuy, onRefreshRequired);
		m_Slot.Init();
	}

	protected override void SetGoodsImage(ShopItemTemplet shopTemplet, bool bFirstBuy)
	{
		if (!string.IsNullOrEmpty(m_OverrideImageAsset) && m_Image != null)
		{
			NKCUtil.SetGameobjectActive(m_Slot, bValue: false);
			NKCUtil.SetGameobjectActive(m_Image, bValue: true);
			Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(NKMAssetName.ParseBundleName("AB_UI_NKM_UI_SHOP_IMG", m_OverrideImageAsset));
			if (orLoadAssetResource == null)
			{
				Debug.LogError($"Shop Sprite {m_OverrideImageAsset}(from productID {shopTemplet.m_ProductID}) null");
			}
			m_Image.sprite = orLoadAssetResource;
			return;
		}
		NKCUtil.SetGameobjectActive(m_Slot, bValue: true);
		NKCUtil.SetGameobjectActive(m_Image, bValue: false);
		NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeShopItemData(shopTemplet, bFirstBuy);
		m_Slot.SetData(slotData, bShowName: false, bShowNumber: true, bEnableLayoutElement: false, OnSlotClick);
		NKCShopManager.ShowShopItemCashCount(m_Slot, slotData, shopTemplet.m_FreeValue, shopTemplet.m_PaidValue);
		if (shopTemplet.m_bSpoiler)
		{
			m_Slot.SetIconImage(GetSpoilerSprite());
		}
	}

	protected override void SetGoodsImage(NKMShopRandomListData shopRandomTemplet)
	{
		NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeShopItemData(shopRandomTemplet);
		m_Slot.SetData(data, bShowName: false, bShowNumber: true, bEnableLayoutElement: false, OnSlotClick);
	}

	private void OnSlotClick(NKCUISlot.SlotData slotData, bool bLocked)
	{
		OnBtnBuy();
	}

	protected override void SetPrice(int priceItemID, int Price, bool bSale = false, int oldPrice = 0)
	{
		NKCUtil.SetGameobjectActive(m_objNormalPriceRoot, !bSale);
		NKCUtil.SetGameobjectActive(m_objSalePriceRoot, bSale);
		NKCUtil.SetGameobjectActive(m_objPriceParent, Price > 0);
		NKCUtil.SetGameobjectActive(m_lbFreePrice, Price <= 0);
		if (Price > 0)
		{
			if (bSale)
			{
				NKCUtil.SetGameobjectActive(m_imgSalePrice, bValue: true);
				Sprite priceImage = GetPriceImage(priceItemID);
				m_imgSalePrice.sprite = priceImage;
				m_lbSaleOldPrice.text = oldPrice.ToString("N0");
				m_lbSalePrice.text = Price.ToString("N0");
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_imgPrice, bValue: true);
				Sprite priceImage2 = GetPriceImage(priceItemID);
				m_imgPrice.sprite = priceImage2;
				m_lbNormalPrice.text = Price.ToString("N0");
			}
		}
		else
		{
			NKCUtil.SetLabelText(m_lbFreePrice, NKCUtilString.GET_STRING_SHOP_FREE);
		}
	}

	protected override void SetInappPurchasePrice(ShopItemTemplet cShopItemTemplet, int price, bool bSale = false, int oldPrice = 0)
	{
		if (cShopItemTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_objNormalPriceRoot, !bSale);
			NKCUtil.SetGameobjectActive(m_objSalePriceRoot, bSale);
			NKCUtil.SetGameobjectActive(m_objPriceParent, price > 0);
			NKCUtil.SetGameobjectActive(m_lbFreePrice, price <= 0);
			if (bSale)
			{
				NKCUtil.SetGameobjectActive(m_imgPrice, bValue: false);
				NKCUtil.SetGameobjectActive(m_imgSalePrice, bValue: false);
				m_lbSaleOldPrice.text = NKCUtilString.GetInAppPurchasePriceString(oldPrice.ToString("N0"), cShopItemTemplet.m_ProductID);
				m_lbNormalPrice.text = NKCPublisherModule.InAppPurchase.GetLocalPriceString(cShopItemTemplet.m_MarketID, cShopItemTemplet.m_ProductID);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_imgPrice, bValue: false);
				NKCUtil.SetGameobjectActive(m_imgSalePrice, bValue: false);
				m_lbNormalPrice.text = NKCPublisherModule.InAppPurchase.GetLocalPriceString(cShopItemTemplet.m_MarketID, cShopItemTemplet.m_ProductID);
			}
		}
	}

	protected override void UpdateTimeLeft(DateTime eventEndTime)
	{
	}

	protected override void SetShowTimeLeft(bool bValue)
	{
	}
}
