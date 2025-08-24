using NKC.Publisher;
using NKM.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public class NKCUIComShopProductDataInjector : MonoBehaviour, IShopDataInjector
{
	[Header("상품 데이터를 집어넣어 주는 컴포넌트")]
	public Text m_lbName;

	public Text m_lbDesc;

	public Image m_imgPriceItem;

	[Tooltip("세일 전 가격. 세일 안하면 안 보임")]
	public Text m_lbBeforeSalePrice;

	[Tooltip("실제 판매가격")]
	public Text m_lbRealPrice;

	public NKCUIShopSlotBase m_shopSlot;

	private bool m_bSetDataComplete;

	public void TriggerInjectData(ShopItemTemplet productTemplet)
	{
		if (productTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (!m_bSetDataComplete)
		{
			SetData(productTemplet);
		}
	}

	public void SetData(ShopItemTemplet productTemplet)
	{
		if (productTemplet == null)
		{
			Debug.LogError("product not found!");
			base.gameObject.SetActive(value: false);
			return;
		}
		if (m_shopSlot != null)
		{
			m_shopSlot.Init(OnBtnProductBuy, null);
			m_shopSlot.SetData(null, productTemplet);
		}
		NKCUtil.SetLabelText(m_lbName, productTemplet.GetItemName());
		NKCUtil.SetLabelText(m_lbDesc, productTemplet.GetItemDesc());
		int realPrice = NKCScenManager.CurrentUserData().m_ShopData.GetRealPrice(productTemplet);
		bool flag = realPrice < productTemplet.m_Price;
		if (productTemplet.m_PriceItemID == 0)
		{
			if (flag)
			{
				SetInappPurchasePrice(productTemplet, realPrice, bSale: true, productTemplet.m_Price);
			}
			else
			{
				SetInappPurchasePrice(productTemplet, productTemplet.m_Price);
			}
		}
		else if (flag)
		{
			SetPrice(productTemplet.m_PriceItemID, realPrice, bSale: true, productTemplet.m_Price);
		}
		else
		{
			SetPrice(productTemplet.m_PriceItemID, realPrice);
		}
	}

	private void SetInappPurchasePrice(ShopItemTemplet cShopItemTemplet, int price, bool bSale = false, int oldPrice = 0)
	{
		NKCUtil.SetGameobjectActive(m_imgPriceItem, bValue: false);
		if (price > 0)
		{
			if (bSale)
			{
				NKCUtil.SetGameobjectActive(m_lbBeforeSalePrice, bValue: true);
				NKCUtil.SetLabelText(m_lbRealPrice, NKCPublisherModule.InAppPurchase.GetLocalPriceString(cShopItemTemplet.m_MarketID, cShopItemTemplet.m_ProductID));
				NKCUtil.SetLabelText(m_lbBeforeSalePrice, NKCUtilString.GetInAppPurchasePriceString(oldPrice.ToString("N0"), cShopItemTemplet.m_ProductID));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lbBeforeSalePrice, bValue: false);
				NKCUtil.SetLabelText(m_lbRealPrice, NKCPublisherModule.InAppPurchase.GetLocalPriceString(cShopItemTemplet.m_MarketID, cShopItemTemplet.m_ProductID));
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_lbBeforeSalePrice, bValue: false);
			NKCUtil.SetLabelText(m_lbRealPrice, NKCUtilString.GET_STRING_SHOP_FREE);
		}
	}

	private void SetPrice(int priceItemID, int Price, bool bSale = false, int oldPrice = 0)
	{
		if (m_imgPriceItem != null)
		{
			Sprite orLoadMiscItemSmallIcon = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(priceItemID);
			NKCUtil.SetImageSprite(m_imgPriceItem, orLoadMiscItemSmallIcon, bDisableIfSpriteNull: true);
		}
		if (Price > 0)
		{
			if (bSale)
			{
				NKCUtil.SetGameobjectActive(m_lbBeforeSalePrice, bValue: true);
				NKCUtil.SetLabelText(m_lbRealPrice, Price.ToString());
				NKCUtil.SetLabelText(m_lbBeforeSalePrice, oldPrice.ToString());
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lbBeforeSalePrice, bValue: false);
				NKCUtil.SetLabelText(m_lbRealPrice, Price.ToString());
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_lbBeforeSalePrice, bValue: false);
			NKCUtil.SetLabelText(m_lbRealPrice, NKCUtilString.GET_STRING_SHOP_FREE);
		}
	}

	private void OnBtnProductBuy(int ProductID)
	{
		NKCShopManager.OnBtnProductBuy(ProductID, bSupply: false);
	}
}
