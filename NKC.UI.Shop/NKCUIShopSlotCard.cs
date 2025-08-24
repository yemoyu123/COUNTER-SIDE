using System;
using NKC.Publisher;
using NKM;
using NKM.Shop;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public class NKCUIShopSlotCard : NKCUIShopSlotBase
{
	[Header("-- 카드형 슬롯 전용 --")]
	public Image m_imgItem;

	public Text m_lbDescription;

	[Header("월정액 등의 남은 기간")]
	public GameObject m_objTimeLeftRoot;

	public Text m_lbTimeLeft;

	[Header("세일/가격 관련")]
	public GameObject m_objSalePriceRoot;

	public Text m_lbOldPrice;

	public Text m_lbPrice;

	public Image m_imgPrice;

	public GameObject m_objPriceParent;

	public Text m_lbFreePrice;

	protected override void SetGoodsImage(ShopItemTemplet shopTemplet, bool bFirstBuy)
	{
		string text = (string.IsNullOrEmpty(m_OverrideImageAsset) ? shopTemplet.m_CardImage : m_OverrideImageAsset);
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(NKMAssetName.ParseBundleName("AB_UI_NKM_UI_SHOP_IMG", text));
		if (orLoadAssetResource == null)
		{
			Debug.LogError($"Shop Sprite {text}(from productID {shopTemplet.m_ProductID}) null");
		}
		NKCUtil.SetImageSprite(m_imgItem, orLoadAssetResource);
		if (m_lbDescription != null)
		{
			m_lbDescription.text = NKCUtilString.GetShopDescriptionText(shopTemplet.GetItemDesc(), bFirstBuy);
		}
	}

	protected override void SetGoodsImage(NKMShopRandomListData shopRandomTemplet)
	{
		Sprite sprite = null;
		switch (shopRandomTemplet.itemType)
		{
		case NKM_REWARD_TYPE.RT_USER_EXP:
		{
			NKMItemMiscTemplet itemMiscTempletByRewardType = NKMItemManager.GetItemMiscTempletByRewardType(shopRandomTemplet.itemType);
			if (itemMiscTempletByRewardType != null)
			{
				sprite = NKCResourceUtility.GetOrLoadMiscItemIcon(itemMiscTempletByRewardType);
				m_lbDescription.text = itemMiscTempletByRewardType.GetItemDesc();
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_MISC:
		case NKM_REWARD_TYPE.RT_MISSION_POINT:
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(shopRandomTemplet.itemId);
			if (itemMiscTempletByID != null)
			{
				sprite = NKCResourceUtility.GetOrLoadMiscItemIcon(itemMiscTempletByID);
				m_lbDescription.text = itemMiscTempletByID.GetItemDesc();
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_EQUIP:
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(shopRandomTemplet.itemId);
			if (equipTemplet != null)
			{
				sprite = NKCResourceUtility.GetOrLoadEquipIcon(equipTemplet);
				m_lbDescription.text = equipTemplet.GetItemDesc();
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_SHIP:
		{
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(shopRandomTemplet.itemId);
			if (unitTempletBase2 != null)
			{
				sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTempletBase2);
				m_lbDescription.text = NKCUtilString.GetGradeString(unitTempletBase2.m_NKM_UNIT_GRADE) + " " + NKCUtilString.GetUnitStyleName(unitTempletBase2.m_NKM_UNIT_STYLE_TYPE);
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_UNIT:
		case NKM_REWARD_TYPE.RT_OPERATOR:
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(shopRandomTemplet.itemId);
			if (unitTempletBase != null)
			{
				sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, unitTempletBase);
				m_lbDescription.text = NKCUtilString.GetGradeString(unitTempletBase.m_NKM_UNIT_GRADE) + " " + NKCUtilString.GetUnitStyleName(unitTempletBase.m_NKM_UNIT_STYLE_TYPE);
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_SKIN:
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(shopRandomTemplet.itemId);
			if (skinTemplet != null)
			{
				sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, skinTemplet);
				m_lbDescription.text = skinTemplet.GetTitle();
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_MOLD:
		{
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(shopRandomTemplet.itemId);
			if (itemMoldTempletByID != null)
			{
				sprite = NKCResourceUtility.GetOrLoadMoldIcon(itemMoldTempletByID);
				m_lbDescription.text = itemMoldTempletByID.GetItemDesc();
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_NONE:
			Debug.LogError("RandomShopTemplet Type None!");
			break;
		}
		m_imgItem.sprite = sprite;
	}

	protected override void UpdateTimeLeft(DateTime eventEndTime)
	{
		m_lbTimeLeft.text = GetUpdateTimeLeftString(eventEndTime);
	}

	protected override void SetShowTimeLeft(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objTimeLeftRoot, bValue);
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

	private string GetUpdateTimeLeftString(DateTime endTime)
	{
		if (NKCSynchronizedTime.IsFinished(endTime))
		{
			return NKCUtilString.GET_STRING_QUIT;
		}
		TimeSpan timeLeft = NKCSynchronizedTime.GetTimeLeft(endTime);
		if (timeLeft.Days > 0)
		{
			return string.Format(NKCUtilString.GET_STRING_TIME_DAY_ONE_PARAM, timeLeft.Days + 1);
		}
		return NKCUtilString.GET_STRING_TIME_REMAIN_SHOP_EXPIRE_TODAY;
	}

	public void SetSlotCardItemImage(Sprite sprite)
	{
		NKCUtil.SetImageSprite(m_imgItem, sprite);
	}
}
