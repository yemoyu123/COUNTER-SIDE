using System;
using NKM;
using NKM.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIComShopItem : MonoBehaviour
{
	public NKCUIComStateButton m_btnShopPackageItem;

	public Image m_imgShopItemIcon;

	public Image m_imgShopItemIconOff;

	public Text m_lbShopItemName;

	public Image m_imgShopPackageItemCost;

	public Text m_lbShopPackageItemCost;

	public Text m_ShopPackageRemainTime;

	[Header("실제 사용하는 프리팹에서 따로 세팅")]
	public int m_ShopPackageItemId;

	public Sprite m_sprShopItemIcon;

	private DateTime m_ShopPackageNextResetTime;

	private bool m_bUpdateTime;

	private float m_fDeltaTime;

	private void OnEnable()
	{
		SetData();
	}

	public void SetData()
	{
		m_bUpdateTime = false;
		m_fDeltaTime = 0f;
		m_btnShopPackageItem.PointerClick.RemoveAllListeners();
		m_btnShopPackageItem.PointerClick.AddListener(OnClickShopItem);
		ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(m_ShopPackageItemId);
		if (shopItemTemplet == null)
		{
			return;
		}
		NKMItemMiscTemplet nKMItemMiscTemplet = NKMItemMiscTemplet.Find(shopItemTemplet.m_PriceItemID);
		if (nKMItemMiscTemplet == null)
		{
			return;
		}
		NKCUtil.SetLabelText(m_lbShopItemName, shopItemTemplet.GetItemName());
		if (m_sprShopItemIcon != null)
		{
			NKCUtil.SetImageSprite(m_imgShopItemIcon, m_sprShopItemIcon);
			NKCUtil.SetImageSprite(m_imgShopItemIconOff, m_sprShopItemIcon);
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (!shopItemTemplet.m_bEnabled || !shopItemTemplet.EnableByTag || !shopItemTemplet.ItemEnableByTag)
		{
			return;
		}
		NKCUtil.SetImageSprite(m_imgShopPackageItemCost, NKCResourceUtility.GetOrLoadMiscItemIcon(nKMItemMiscTemplet));
		NKCUtil.SetLabelText(m_lbShopPackageItemCost, shopItemTemplet.m_Price.ToString("#,##0"));
		if (NKCShopManager.GetBuyCountLeft(m_ShopPackageItemId) > 0)
		{
			m_btnShopPackageItem.UnLock();
			if (NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(shopItemTemplet.m_PriceItemID) >= shopItemTemplet.m_Price)
			{
				NKCUtil.SetLabelTextColor(m_lbShopPackageItemCost, Color.white);
			}
			else
			{
				NKCUtil.SetLabelTextColor(m_lbShopPackageItemCost, Color.red);
			}
			return;
		}
		NKMShopData shopData = NKCScenManager.CurrentUserData().m_ShopData;
		if (shopData.histories.ContainsKey(m_ShopPackageItemId) && !NKCSynchronizedTime.IsFinished(shopData.histories[m_ShopPackageItemId].nextResetDate))
		{
			NKCUtil.SetLabelText(m_ShopPackageRemainTime, NKCSynchronizedTime.GetTimeLeftString(shopData.histories[m_ShopPackageItemId].nextResetDate));
			m_ShopPackageNextResetTime = new DateTime(shopData.histories[m_ShopPackageItemId].nextResetDate);
			m_btnShopPackageItem.Lock();
			m_bUpdateTime = true;
			UpdateShopPackageRemainTime();
		}
		else
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		}
	}

	private void UpdateShopPackageRemainTime()
	{
		NKCUtil.SetLabelText(m_ShopPackageRemainTime, NKCSynchronizedTime.GetTimeLeftString(m_ShopPackageNextResetTime));
		if (NKCSynchronizedTime.IsFinished(m_ShopPackageNextResetTime))
		{
			m_bUpdateTime = false;
			SetData();
		}
	}

	private void Update()
	{
		m_fDeltaTime += Time.deltaTime;
		if (m_fDeltaTime > 1f)
		{
			m_fDeltaTime -= 1f;
			if (m_bUpdateTime && m_btnShopPackageItem != null && m_btnShopPackageItem.m_bLock)
			{
				UpdateShopPackageRemainTime();
			}
		}
	}

	private void OnClickShopItem()
	{
		NKCShopManager.OnBtnProductBuy(m_ShopPackageItemId, bSupply: false);
	}

	public void Refresh()
	{
		SetData();
	}
}
