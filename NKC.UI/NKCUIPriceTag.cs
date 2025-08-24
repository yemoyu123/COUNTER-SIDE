using NKC.Publisher;
using NKC.UI.Tooltip;
using NKM;
using NKM.Shop;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIPriceTag : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	public struct ConsumedResource
	{
		public int m_priceItemID;

		public int m_Count;

		public ConsumedResource(int priceItemID, int count)
		{
			m_priceItemID = priceItemID;
			m_Count = count;
		}
	}

	public int m_priceItemID;

	public Image m_imgIcon;

	public Text m_lbPrice;

	public Color m_colHasEnough = Color.white;

	public Color m_colNotEnough = Color.red;

	public bool SetData(ConsumedResource consumedResource, bool showMinus = false, bool changeColor = true)
	{
		return SetData(consumedResource.m_priceItemID, consumedResource.m_Count, showMinus, changeColor);
	}

	public bool SetData(int priceItemID, int price, bool showMinus = false, bool changeColor = true, bool bHidePriceIcon = false)
	{
		return SetData(priceItemID, (long)price, showMinus, changeColor, bHidePriceIcon);
	}

	public bool SetData(int priceItemID, long price, bool showMinus = false, bool changeColor = true, bool bHidePriceIcon = false)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		m_priceItemID = priceItemID;
		bool flag = myUserData.CheckPrice(price, priceItemID);
		if (m_lbPrice != null)
		{
			if (price == 0L && bHidePriceIcon)
			{
				NKCUtil.SetGameobjectActive(m_imgIcon, bValue: false);
				m_lbPrice.text = NKCUtilString.GET_STRING_SHOP_FREE;
			}
			else
			{
				SetPriceSprite(priceItemID);
				if (priceItemID == 0)
				{
					Debug.LogError("NKCUIPriceTag : Inapp Purchase item should not passed by PriceItemID");
					if (changeColor)
					{
						m_lbPrice.color = m_colHasEnough;
					}
					m_lbPrice.text = NKCUtilString.GetInAppPurchasePriceString(price.ToString("N0"), 0);
				}
				else
				{
					if (changeColor)
					{
						m_lbPrice.color = (flag ? m_colHasEnough : m_colNotEnough);
					}
					m_lbPrice.text = (showMinus ? ("-" + price.ToString("N0")) : price.ToString("N0"));
				}
			}
		}
		return flag;
	}

	public bool SetData(ShopItemTemplet itemTemplet, bool showMinus = false, bool changeColor = true)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		m_priceItemID = itemTemplet.m_PriceItemID;
		SetPriceSprite(itemTemplet.m_PriceItemID);
		bool flag = myUserData.CheckPrice(itemTemplet.m_Price, itemTemplet.m_PriceItemID);
		if (m_lbPrice != null)
		{
			if (itemTemplet.m_Price == 0)
			{
				NKCUtil.SetGameobjectActive(m_imgIcon, bValue: false);
				m_lbPrice.text = NKCUtilString.GET_STRING_SHOP_FREE;
			}
			else if (itemTemplet.m_PriceItemID == 0)
			{
				if (changeColor)
				{
					m_lbPrice.color = m_colHasEnough;
				}
				m_lbPrice.text = NKCPublisherModule.InAppPurchase.GetLocalPriceString(itemTemplet.m_MarketID, itemTemplet.m_ProductID);
			}
			else
			{
				if (changeColor)
				{
					m_lbPrice.color = (flag ? m_colHasEnough : m_colNotEnough);
				}
				m_lbPrice.text = (showMinus ? ("-" + itemTemplet.m_Price.ToString("N0")) : itemTemplet.m_Price.ToString("N0"));
			}
		}
		return flag;
	}

	public bool SetDataByHaveCount(int price, int haveCount, bool showMinus = false, bool changeColor = true)
	{
		bool flag = price <= haveCount;
		if (m_lbPrice != null)
		{
			if (changeColor)
			{
				m_lbPrice.color = (flag ? m_colHasEnough : m_colNotEnough);
			}
			m_lbPrice.text = (showMinus ? ("-" + price.ToString("N0")) : price.ToString("N0"));
		}
		return flag;
	}

	public void SetLabelTextColor(Color color)
	{
		if (m_lbPrice != null)
		{
			m_lbPrice.color = color;
		}
	}

	private void SetPriceSprite(int itemID)
	{
		if (!(m_imgIcon == null))
		{
			Sprite orLoadMiscItemSmallIcon = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(itemID);
			NKCUtil.SetImageSprite(m_imgIcon, orLoadMiscItemSmallIcon, bDisableIfSpriteNull: true);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (m_priceItemID != 0)
		{
			NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeMiscItemData(m_priceItemID, 1L);
			NKCUITooltip.Instance.Open(slotData, eventData.position);
		}
	}
}
