using System;
using System.Collections.Generic;
using NKC.Publisher;
using NKM;
using NKM.Shop;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public class NKCUIShopSlotCustom : NKCUIShopSlotBase
{
	[Header("커스텀 슬롯 전용")]
	public Image m_imgItem;

	public List<NKCUISlot> m_lstItemSlot;

	public GameObject m_objSalePriceRoot;

	public Text m_lbOldPrice;

	public Text m_lbPrice;

	public Image m_imgPrice;

	public Sprite m_spEmptyCustom;

	public GameObject m_objPriceParent;

	public Text m_lbFreePrice;

	public override void Init(OnBuy onBuy, OnRefreshRequired onRefreshRequired)
	{
		base.Init(onBuy, onRefreshRequired);
		foreach (NKCUISlot item in m_lstItemSlot)
		{
			item.Init();
			if (m_spEmptyCustom != null)
			{
				item.SetCustomizedEmptySP(m_spEmptyCustom);
			}
		}
	}

	protected override void SetGoodsImage(ShopItemTemplet shopTemplet, bool bFirstBuy)
	{
		if (shopTemplet.m_ItemType != NKM_REWARD_TYPE.RT_MISC)
		{
			DisableSlot();
			return;
		}
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(shopTemplet.m_ItemID);
		if (itemMiscTempletByID.m_ItemMiscType != NKM_ITEM_MISC_TYPE.IMT_CUSTOM_PACKAGE)
		{
			DisableSlot();
			return;
		}
		if (!string.IsNullOrEmpty(shopTemplet.m_CardImage))
		{
			Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(NKMAssetName.ParseBundleName("AB_UI_NKM_UI_SHOP_IMG", shopTemplet.m_CardImage));
			if (orLoadAssetResource == null)
			{
				Debug.LogError($"Shop Sprite {shopTemplet.m_CardImage}(from productID {shopTemplet.m_ProductID}) null");
			}
			m_imgItem.sprite = orLoadAssetResource;
			NKCUtil.SetGameobjectActive(m_imgItem, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_imgItem, bValue: false);
		}
		SetSlot(itemMiscTempletByID);
	}

	private void SetSlot(NKMItemMiscTemplet itemTemplet)
	{
		List<NKCUISlot.SlotData> list = MakeCustomPackageItemList(itemTemplet);
		for (int i = 0; i < m_lstItemSlot.Count; i++)
		{
			NKCUISlot nKCUISlot = m_lstItemSlot[i];
			if (i < list.Count)
			{
				NKCUISlot.SlotData slotData = list[i];
				NKCUtil.SetGameobjectActive(nKCUISlot, bValue: true);
				if (slotData != null)
				{
					nKCUISlot.SetData(slotData, bShowName: false, slotData.eType == NKCUISlot.eSlotMode.ItemMisc, bEnableLayoutElement: false, OnSlotClick);
				}
				else
				{
					nKCUISlot.SetEmpty(OnSlotClick);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(nKCUISlot, bValue: false);
			}
		}
	}

	private List<NKCUISlot.SlotData> MakeCustomPackageItemList(NKMItemMiscTemplet itemTemplet)
	{
		List<NKCUISlot.SlotData> list = new List<NKCUISlot.SlotData>();
		if (itemTemplet == null)
		{
			return list;
		}
		List<NKMRandomBoxItemTemplet> randomBoxItemTempletList = NKCRandomBoxManager.GetRandomBoxItemTempletList(itemTemplet.m_RewardGroupID);
		if (itemTemplet.m_RewardGroupID != 0 && randomBoxItemTempletList == null)
		{
			Debug.LogError("rewardgroup null! ID : " + itemTemplet.m_RewardGroupID);
		}
		if (randomBoxItemTempletList != null)
		{
			for (int i = 0; i < randomBoxItemTempletList.Count; i++)
			{
				NKMRandomBoxItemTemplet nKMRandomBoxItemTemplet = randomBoxItemTempletList[i];
				NKCUISlot.SlotData item = NKCUISlot.SlotData.MakeRewardTypeData(nKMRandomBoxItemTemplet.m_reward_type, nKMRandomBoxItemTemplet.m_RewardID, nKMRandomBoxItemTemplet.TotalQuantity_Max);
				list.Add(item);
			}
		}
		if (itemTemplet.CustomPackageTemplets != null)
		{
			for (int j = 0; j < itemTemplet.CustomPackageTemplets.Count; j++)
			{
				list.Add(null);
			}
		}
		return list;
	}

	private void DisableSlot()
	{
		foreach (NKCUISlot item in m_lstItemSlot)
		{
			NKCUtil.SetGameobjectActive(item, bValue: false);
		}
	}

	protected override void SetInappPurchasePrice(ShopItemTemplet cShopItemTemplet, int price, bool bSale = false, int oldPrice = 0)
	{
		NKCUtil.SetGameobjectActive(m_objSalePriceRoot, bSale);
		NKCUtil.SetGameobjectActive(m_objPriceParent, price > 0);
		NKCUtil.SetGameobjectActive(m_lbFreePrice, price <= 0);
		if (price > 0)
		{
			if (bSale)
			{
				NKCUtil.SetGameobjectActive(m_imgPrice, bValue: false);
				m_lbOldPrice.text = NKCUtilString.GetInAppPurchasePriceString(oldPrice.ToString("N0"), cShopItemTemplet.m_ProductID);
				m_lbPrice.text = NKCPublisherModule.InAppPurchase.GetLocalPriceString(cShopItemTemplet.m_MarketID, cShopItemTemplet.m_ProductID);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_imgPrice, bValue: false);
				m_lbPrice.text = NKCPublisherModule.InAppPurchase.GetLocalPriceString(cShopItemTemplet.m_MarketID, cShopItemTemplet.m_ProductID);
			}
		}
		else
		{
			NKCUtil.SetLabelText(m_lbFreePrice, NKCUtilString.GET_STRING_SHOP_FREE);
		}
	}

	protected override void SetPrice(int priceItemID, int Price, bool bSale = false, int oldPrice = 0)
	{
		NKCUtil.SetGameobjectActive(m_objSalePriceRoot, bSale);
		NKCUtil.SetGameobjectActive(m_objPriceParent, Price > 0);
		NKCUtil.SetGameobjectActive(m_lbFreePrice, Price <= 0);
		if (Price > 0)
		{
			if (bSale)
			{
				NKCUtil.SetGameobjectActive(m_imgPrice, bValue: true);
				Sprite priceImage = GetPriceImage(priceItemID);
				NKCUtil.SetImageSprite(m_imgPrice, priceImage, bDisableIfSpriteNull: true);
				m_lbOldPrice.text = oldPrice.ToString("N0");
				m_lbPrice.text = Price.ToString("N0");
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_imgPrice, bValue: true);
				Sprite priceImage2 = GetPriceImage(priceItemID);
				NKCUtil.SetImageSprite(m_imgPrice, priceImage2, bDisableIfSpriteNull: true);
				m_lbPrice.text = Price.ToString("N0");
			}
		}
		else
		{
			NKCUtil.SetLabelText(m_lbFreePrice, NKCUtilString.GET_STRING_SHOP_FREE);
		}
	}

	protected override void UpdateTimeLeft(DateTime eventEndTime)
	{
	}

	protected override void SetShowTimeLeft(bool bValue)
	{
	}

	private string GetUpdateTimeLeftString(DateTime endTime)
	{
		if (NKCSynchronizedTime.IsFinished(endTime))
		{
			return NKCUtilString.GET_STRING_QUIT;
		}
		return NKCStringTable.GetString("SI_DP_SHOP_CUSTOM_TIME_LEFT", NKCSynchronizedTime.GetTimeLeftString(endTime));
	}

	private void OnSlotClick(NKCUISlot.SlotData slotData, bool bLocked)
	{
		OnBtnBuy();
	}

	protected override void SetRibbon(ShopItemRibbon ribbonType)
	{
		NKCUtil.SetLabelText(m_lbRibbon, NKCShopManager.GetRibbonString(ribbonType));
		NKCUtil.SetGameobjectActive(m_lbRibbon, ribbonType != ShopItemRibbon.None);
		NKCUtil.SetGameobjectActive(m_imgRibbon, ribbonType != ShopItemRibbon.None);
	}
}
