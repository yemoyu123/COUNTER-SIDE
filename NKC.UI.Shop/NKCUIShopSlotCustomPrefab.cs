using System.Collections.Generic;
using NKC.Templet;
using NKM.Shop;
using UnityEngine;

namespace NKC.UI.Shop;

public class NKCUIShopSlotCustomPrefab : MonoBehaviour
{
	public List<NKCUIShopSlotBase> m_lstShopSlot;

	private bool m_bInit;

	public void Init(NKCUIShopSlotBase.OnBuy onBuy, NKCUIShopSlotBase.OnRefreshRequired onRefreshRequired)
	{
		if (m_bInit)
		{
			return;
		}
		m_bInit = true;
		foreach (NKCUIShopSlotBase item in m_lstShopSlot)
		{
			item.Init(onBuy, onRefreshRequired);
		}
	}

	public void SetData(NKCUIShop uiShop, NKCShopCustomTabTemplet tabTemplet, NKCUIShopSlotBase.OnBuy onBuy, NKCUIShopSlotBase.OnRefreshRequired onRefreshRequired)
	{
		if (tabTemplet.m_UseProductID.Count != m_lstShopSlot.Count)
		{
			Debug.LogError("NKCShopCustomTabTemplet에 지정된 상품 수가 프리팹의 슬롯과 수가 맞지 않음");
		}
		if (!m_bInit)
		{
			Init(onBuy, onRefreshRequired);
		}
		for (int i = 0; i < m_lstShopSlot.Count; i++)
		{
			NKCUIShopSlotBase nKCUIShopSlotBase = m_lstShopSlot[i];
			if (i < tabTemplet.m_UseProductID.Count)
			{
				NKCUtil.SetGameobjectActive(nKCUIShopSlotBase, bValue: true);
				int num = tabTemplet.m_UseProductID[i];
				ShopItemTemplet shopTemplet = ShopItemTemplet.Find(num);
				bool bFirstBuy = NKCShopManager.IsFirstBuy(num, NKCScenManager.GetScenManager().GetMyUserData());
				int buyCountLeft = NKCShopManager.GetBuyCountLeft(num);
				nKCUIShopSlotBase.SetData(uiShop, shopTemplet, buyCountLeft, bFirstBuy);
			}
			else
			{
				NKCUtil.SetGameobjectActive(nKCUIShopSlotBase, bValue: false);
			}
		}
	}
}
