using NKM;
using NKM.Shop;
using UnityEngine;

namespace NKC.UI.Shop;

public class NKCUIShopSlotPrefab : NKCUIShopSlotCard
{
	[Header("프리팹 슬롯 대응")]
	public RectTransform m_rtPrefabRoot;

	public GameObject m_objUIRoot;

	private NKCAssetInstanceData m_prefabInstance;

	protected override void SetGoodsImage(ShopItemTemplet shopTemplet, bool bFirstBuy)
	{
		if (!string.IsNullOrEmpty(shopTemplet.m_CardPrefab))
		{
			NKMAssetName assetName = NKMAssetName.ParseBundleName(shopTemplet.m_CardPrefab, shopTemplet.m_CardPrefab);
			SetPrefabData(assetName, shopTemplet);
		}
		else
		{
			NKMAssetName imageData = NKMAssetName.ParseBundleName("AB_UI_NKM_UI_SHOP_IMG", shopTemplet.m_CardImage);
			SetImageData(imageData);
		}
	}

	private void SetPrefabData(NKMAssetName assetName, ShopItemTemplet shopTemplet)
	{
		CleanUp();
		m_prefabInstance = NKCAssetResourceManager.OpenInstance<GameObject>(assetName);
		if (m_prefabInstance != null && m_prefabInstance.m_Instant != null)
		{
			m_prefabInstance.m_Instant.transform.SetParent(m_rtPrefabRoot, worldPositionStays: false);
			m_prefabInstance.m_Instant.GetComponent<IShopPrefab>()?.SetData(shopTemplet);
			IShopDataInjector[] componentsInChildren = m_prefabInstance.m_Instant.GetComponentsInChildren<IShopDataInjector>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].TriggerInjectData(shopTemplet);
			}
		}
		else
		{
			Debug.Log($"SetPrefabData Fail, file : {assetName}");
		}
		NKCUtil.SetGameobjectActive(m_imgItem, bValue: false);
		NKCUtil.SetGameobjectActive(m_rtPrefabRoot, bValue: true);
	}

	public void SetImageData(NKMAssetName assetName)
	{
		CleanUp();
		NKCUtil.SetImageSprite(m_imgItem, NKCResourceUtility.GetOrLoadAssetResource<Sprite>(assetName));
		NKCUtil.SetGameobjectActive(m_imgItem, bValue: true);
		NKCUtil.SetGameobjectActive(m_rtPrefabRoot, bValue: false);
	}

	protected override void PostSetData(ShopItemTemplet shopTemplet)
	{
		bool bIsFirstBuy = NKCShopManager.IsFirstBuy(shopTemplet.m_ProductID, NKCScenManager.CurrentUserData());
		if (m_prefabInstance != null && m_prefabInstance.m_Instant != null)
		{
			IShopPrefab component = m_prefabInstance.m_Instant.GetComponent<IShopPrefab>();
			if (component != null && component.IsHideLockObject())
			{
				NKCUtil.SetGameobjectActive(m_objLocked, bValue: false);
				NKCUtil.SetGameobjectActive(m_objLockedTime, bValue: false);
			}
			NKCUIComShopBuyButton componentInChildren = m_prefabInstance.m_Instant.GetComponentInChildren<NKCUIComShopBuyButton>();
			if (componentInChildren != null)
			{
				componentInChildren.SetData(shopTemplet, base.OnBtnBuy, bIsFirstBuy);
				NKCUtil.SetGameobjectActive(m_cbtnBuy, bValue: false);
				NKCUtil.SetGameobjectActive(m_objSoldOut, bValue: false);
			}
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

	public void ShowBannerOnly(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objUIRoot, !value);
	}
}
