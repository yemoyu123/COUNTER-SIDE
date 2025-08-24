using NKM;
using NKM.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public class NKCUIShopSlotHomeBanner : MonoBehaviour
{
	public delegate void OnBtn(NKCShopBannerTemplet bannerTemplet);

	public NKCUIComStateButton m_btn;

	public Image m_Image;

	public RectTransform m_rtPrefabRoot;

	private NKCAssetInstanceData m_prefabInstance;

	private OnBtn dOnBtn;

	private NKCShopBannerTemplet cNKCShopBannerTemplet;

	private bool m_bInitComplete;

	public void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_btn, OnClickBtn);
		if (m_rtPrefabRoot == null)
		{
			m_rtPrefabRoot = GetComponent<RectTransform>();
		}
		m_bInitComplete = true;
	}

	public void SetData(NKCShopBannerTemplet bannerTemplet, OnBtn onBtn)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
		if (bannerTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		cNKCShopBannerTemplet = bannerTemplet;
		dOnBtn = onBtn;
		if (!string.IsNullOrEmpty(bannerTemplet.m_ShopHome_BannerPrefab))
		{
			NKMAssetName prefabData = NKMAssetName.ParseBundleName(bannerTemplet.m_ShopHome_BannerPrefab, bannerTemplet.m_ShopHome_BannerPrefab);
			SetPrefabData(prefabData);
		}
		else
		{
			NKMAssetName imageData = NKMAssetName.ParseBundleName("AB_UI_NKM_UI_SHOP_THUMBNAIL", bannerTemplet.m_ShopHome_BannerImage);
			SetImageData(imageData);
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
	}

	public void SetPrefabData(NKMAssetName assetName)
	{
		CleanUp();
		m_prefabInstance = NKCAssetResourceManager.OpenInstance<GameObject>(assetName);
		if (m_prefabInstance != null && m_prefabInstance.m_Instant != null)
		{
			m_prefabInstance.m_Instant.transform.SetParent(m_rtPrefabRoot, worldPositionStays: false);
			ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(cNKCShopBannerTemplet.m_ProductID);
			m_prefabInstance.m_Instant.GetComponent<IShopPrefab>()?.SetData(shopItemTemplet);
			IShopDataInjector[] componentsInChildren = m_prefabInstance.m_Instant.GetComponentsInChildren<IShopDataInjector>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].TriggerInjectData(shopItemTemplet);
			}
		}
		else
		{
			Debug.Log($"SetPrefabData Fail, file : {assetName}");
		}
		NKCUtil.SetGameobjectActive(m_rtPrefabRoot, bValue: true);
		NKCUtil.SetGameobjectActive(m_Image, bValue: false);
	}

	public void SetImageData(NKMAssetName assetName)
	{
		CleanUp();
		NKCUtil.SetImageSprite(m_Image, NKCResourceUtility.GetOrLoadAssetResource<Sprite>(assetName));
		NKCUtil.SetGameobjectActive(m_Image, bValue: true);
		NKCUtil.SetGameobjectActive(m_rtPrefabRoot, bValue: false);
	}

	private void CleanUp()
	{
		if (m_prefabInstance != null)
		{
			NKCAssetResourceManager.CloseInstance(m_prefabInstance);
			m_prefabInstance = null;
		}
	}

	private void OnClickBtn()
	{
		dOnBtn?.Invoke(cNKCShopBannerTemplet);
	}

	private void OnDestroy()
	{
		CleanUp();
	}
}
