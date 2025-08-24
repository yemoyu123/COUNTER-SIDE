using NKC.Templet;
using NKM;
using NKM.Shop;
using UnityEngine;

namespace NKC.UI.Shop;

public class NKCUIShopSlotCustomPrefabAdapter : NKCUIShopSlotBase
{
	public RectTransform m_rtPrefabRoot;

	private NKCAssetInstanceData m_prefabInstance;

	public void SetData(NKCUIShop uiShop, NKCShopCustomTabTemplet tabTemplet, OnBuy onBuy, OnRefreshRequired onRefreshRequired)
	{
		CleanUp();
		NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName(tabTemplet.m_UsePrefabName, tabTemplet.m_UsePrefabName);
		m_prefabInstance = NKCAssetResourceManager.OpenInstance<GameObject>(nKMAssetName);
		if (m_prefabInstance != null && m_prefabInstance.m_Instant != null)
		{
			m_prefabInstance.m_Instant.transform.SetParent(m_rtPrefabRoot, worldPositionStays: false);
			NKCUIShopSlotCustomPrefab component = m_prefabInstance.m_Instant.GetComponent<NKCUIShopSlotCustomPrefab>();
			if (component != null)
			{
				component.SetData(uiShop, tabTemplet, onBuy, onRefreshRequired);
			}
		}
		else
		{
			Debug.Log($"SetData Fail, file : {nKMAssetName}");
		}
	}

	private void CleanUp()
	{
		if (m_prefabInstance != null)
		{
			NKCAssetResourceManager.CloseInstance(m_prefabInstance);
			m_prefabInstance = null;
		}
	}

	protected override void SetGoodsImage(ShopItemTemplet shopTemplet, bool bFirstBuy)
	{
	}

	protected override void SetInappPurchasePrice(ShopItemTemplet cShopItemTemplet, int price, bool bSale = false, int oldPrice = 0)
	{
	}

	protected override void SetPrice(int priceItemID, int Price, bool bSale = false, int oldPrice = 0)
	{
	}
}
