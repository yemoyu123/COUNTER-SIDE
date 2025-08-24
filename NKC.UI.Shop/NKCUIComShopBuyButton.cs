using System;
using NKC.Publisher;
using NKM;
using NKM.Shop;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public class NKCUIComShopBuyButton : MonoBehaviour
{
	public NKCUIComButton m_cbtnBuy;

	public GameObject m_objPriceRoot;

	[Header("할인 관련")]
	public GameObject m_objDiscountDay;

	public Text m_txtDiscountDay;

	public GameObject m_objDiscountRate;

	public Text m_txtDiscountRate;

	[Header("세일/가격 관련")]
	public GameObject m_objSalePriceRoot;

	public Text m_lbOldPrice;

	public Text m_lbPrice;

	public Image m_imgPrice;

	public GameObject m_objPriceParent;

	public Text m_lbFreePrice;

	public GameObject m_objReddot;

	public GameObject m_objReddot_RED;

	public GameObject m_objReddot_YELLOW;

	private DateTime m_tEndDateDiscountTime;

	private float m_updateTimer;

	private bool m_bTimerUpdate;

	private const float TIMER_UPDATE_INTERVAL = 1f;

	private ShopReddotType m_ReddotType;

	private UnityAction m_onBtnBuy;

	public bool SetData(ShopItemTemplet shopTemplet, UnityAction onBtnBuy, bool bIsFirstBuy)
	{
		int buyCountLeft = NKCShopManager.GetBuyCountLeft(shopTemplet.m_ProductID);
		NKCUtil.SetButtonClickDelegate(m_cbtnBuy, OnClickBtn);
		m_ReddotType = NKCShopManager.GetReddotType(shopTemplet);
		m_onBtnBuy = onBtnBuy;
		_ = shopTemplet.m_ProductID;
		bool bAdmin;
		bool flag = NKCShopManager.IsProductAvailable(shopTemplet, out bAdmin, bIncludeLockedItemWithReason: true);
		switch (buyCountLeft)
		{
		case 0:
			NKCUtil.SetGameobjectActive(m_objPriceRoot, bValue: false);
			break;
		default:
			NKCUtil.SetGameobjectActive(m_objPriceRoot, bValue: true);
			break;
		case -1:
			NKCUtil.SetGameobjectActive(m_objPriceRoot, bValue: true);
			break;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return false;
		}
		int realPrice = nKMUserData.m_ShopData.GetRealPrice(shopTemplet);
		bool flag2 = realPrice < shopTemplet.m_Price;
		if (shopTemplet.m_PriceItemID == 0)
		{
			if (flag2)
			{
				SetInappPurchasePrice(shopTemplet, realPrice, bSale: true, shopTemplet.m_Price);
			}
			else
			{
				SetInappPurchasePrice(shopTemplet, shopTemplet.m_Price);
			}
		}
		else if (flag2)
		{
			SetPrice(shopTemplet.m_PriceItemID, realPrice, bSale: true, shopTemplet.m_Price);
		}
		else
		{
			SetPrice(shopTemplet.m_PriceItemID, realPrice);
		}
		bool flag3 = false;
		if (shopTemplet.m_DiscountRate > 0f && NKCSynchronizedTime.IsEventTime(shopTemplet.discountIntervalId, shopTemplet.DiscountStartDateUtc, shopTemplet.DiscountEndDateUtc) && shopTemplet.DiscountEndDateUtc != DateTime.MinValue && shopTemplet.DiscountEndDateUtc != DateTime.MaxValue)
		{
			flag3 = true;
			m_tEndDateDiscountTime = shopTemplet.DiscountEndDateUtc;
			UpdateDiscountTime(m_tEndDateDiscountTime);
		}
		else
		{
			m_tEndDateDiscountTime = default(DateTime);
		}
		if (!shopTemplet.HasDiscountDateLimit)
		{
			NKCUtil.SetGameobjectActive(m_objDiscountRate, shopTemplet.m_DiscountRate > 0f);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objDiscountRate, shopTemplet.m_DiscountRate > 0f && flag3);
		}
		float num = (100f - shopTemplet.m_DiscountRate) / 10f;
		NKCUtil.SetLabelText(m_txtDiscountRate, NKCStringTable.GetString("SI_DP_SHOP_DISCOUNT_RATE", (int)shopTemplet.m_DiscountRate, num));
		if (!flag)
		{
			NKCUtil.SetGameobjectActive(m_objPriceRoot, bValue: false);
		}
		m_bTimerUpdate = flag3;
		SetShowBadgeTime(flag3);
		NKCUtil.SetShopReddotImage(NKCShopManager.GetReddotType(shopTemplet), m_objReddot, m_objReddot_RED, m_objReddot_YELLOW);
		return true;
	}

	private void SetPrice(int priceItemID, int Price, bool bSale = false, int oldPrice = 0)
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
				m_lbOldPrice.text = oldPrice.ToString("#,##0");
				m_lbPrice.text = Price.ToString("#,##0");
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_imgPrice, bValue: true);
				Sprite priceImage2 = GetPriceImage(priceItemID);
				NKCUtil.SetImageSprite(m_imgPrice, priceImage2, bDisableIfSpriteNull: true);
				m_lbPrice.text = Price.ToString("#,##0");
			}
		}
		else
		{
			NKCUtil.SetLabelText(m_lbFreePrice, NKCUtilString.GET_STRING_SHOP_FREE);
		}
	}

	private void SetInappPurchasePrice(ShopItemTemplet cShopItemTemplet, int price, bool bSale = false, int oldPrice = 0)
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

	private Sprite GetPriceImage(int priceItemID)
	{
		return NKCResourceUtility.GetOrLoadMiscItemSmallIcon(priceItemID);
	}

	private void SetShowBadgeTime(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objDiscountDay, bValue);
	}

	private void UpdateDiscountTime(DateTime endTime)
	{
		NKCUtil.SetLabelText(msg: (!NKCSynchronizedTime.IsFinished(endTime)) ? NKCUtilString.GetRemainTimeStringOneParam(endTime) : NKCUtilString.GET_STRING_QUIT, label: m_txtDiscountDay);
	}

	private void OnClickBtn()
	{
		m_onBtnBuy?.Invoke();
		if (m_ReddotType != ShopReddotType.REDDOT_PURCHASED)
		{
			NKCUtil.SetGameobjectActive(m_objReddot, bValue: false);
		}
	}

	private void Update()
	{
		if (!m_bTimerUpdate)
		{
			return;
		}
		m_updateTimer += Time.deltaTime;
		if (1f < m_updateTimer)
		{
			m_updateTimer = 0f;
			if (m_tEndDateDiscountTime != DateTime.MinValue)
			{
				UpdateDiscountTime(m_tEndDateDiscountTime);
			}
		}
	}
}
