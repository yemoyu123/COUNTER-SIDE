using NKM;
using NKM.Event;
using NKM.Shop;
using UnityEngine;

namespace NKC.UI.Event;

public class NKCUIEventSubUIAlipayPromo : NKCUIEventSubUIBase
{
	public NKCUIComStateButton m_csbtnBuyPkg;

	public NKCUIComStateButton m_csbtnGetHongBao;

	public NKCUIComStateButton m_csbtnEventHelp;

	private const int ALIPAY_PROMO_PKG_PRODUCT_ID = 160372;

	public override void Init()
	{
		base.Init();
		if (m_csbtnBuyPkg != null)
		{
			m_csbtnBuyPkg.SetLock(value: false);
			m_csbtnBuyPkg.PointerClick.RemoveAllListeners();
			m_csbtnBuyPkg.PointerClick.AddListener(OnClickBuyPkg);
		}
		if (m_csbtnGetHongBao != null)
		{
			m_csbtnGetHongBao.SetLock(value: false);
			m_csbtnGetHongBao.PointerClick.RemoveAllListeners();
			m_csbtnGetHongBao.PointerClick.AddListener(OnClickBuyHongbao);
		}
		if (m_csbtnEventHelp != null)
		{
			m_csbtnEventHelp.SetLock(value: false);
			m_csbtnEventHelp.PointerClick.RemoveAllListeners();
			m_csbtnEventHelp.PointerClick.AddListener(OnClickEventHelp);
		}
	}

	public override void Open(NKMEventTabTemplet tabTemplet)
	{
		m_tabTemplet = tabTemplet;
		UpdateUI();
	}

	private bool GetPossibleBuyPkg()
	{
		NKMShopData shopData = NKCScenManager.CurrentUserData().m_ShopData;
		if (shopData == null)
		{
			return false;
		}
		ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(160372);
		if (shopItemTemplet != null && shopData.GetPurchasedCount(shopItemTemplet) > 0)
		{
			return false;
		}
		return true;
	}

	private void UpdateUI()
	{
		if (m_csbtnBuyPkg != null)
		{
			m_csbtnBuyPkg.SetLock(!GetPossibleBuyPkg());
		}
		if (m_csbtnGetHongBao != null)
		{
			m_csbtnGetHongBao.SetLock(!GetPossibleBuyPkg());
		}
	}

	private void OnClickBuyPkg()
	{
		if (GetPossibleBuyPkg())
		{
			ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(160372);
			if (shopItemTemplet != null)
			{
				NKCPacketSender.Send_NKMPacket_SHOP_FIX_SHOP_CASH_BUY_POSSIBLE_REQ(shopItemTemplet.m_MarketID, null);
			}
		}
	}

	private void OnClickBuyHongbao()
	{
		Application.OpenURL("https://render.alipay.com/p/c/18lg98p47vog/page_u748a05713109410089df61f17a7e0f54.html");
	}

	private void OnClickEventHelp()
	{
		NKCPopupEventHelp.Instance.Open(m_tabTemplet.m_EventID);
	}

	public override void Refresh()
	{
		UpdateUI();
	}
}
