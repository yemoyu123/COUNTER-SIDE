using System.Collections.Generic;
using Cs.Logging;
using NKC.UI.Shop;
using NKM;
using NKM.Shop;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class NKCUIPointExchangeSlot : MonoBehaviour
{
	public GameObject m_specialSlotPrefab;

	public GameObject m_normalSlotPrefab;

	public Transform m_specialRoot;

	public Transform m_normalRoot;

	private NKCAssetInstanceData m_InstanceData;

	public static NKCUIPointExchangeSlot GetNewInstance(Transform parent, string bundleName, string assetName)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(bundleName, assetName);
		NKCUIPointExchangeSlot nKCUIPointExchangeSlot = nKCAssetInstanceData?.m_Instant.GetComponent<NKCUIPointExchangeSlot>();
		if (nKCUIPointExchangeSlot == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCUIPointExchangeSlot Prefab null!");
			return null;
		}
		nKCUIPointExchangeSlot.m_InstanceData = nKCAssetInstanceData;
		nKCUIPointExchangeSlot.Init();
		if (parent != null)
		{
			nKCUIPointExchangeSlot.transform.SetParent(parent);
		}
		nKCUIPointExchangeSlot.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		nKCUIPointExchangeSlot.gameObject.SetActive(value: false);
		return nKCUIPointExchangeSlot;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		Object.Destroy(base.gameObject);
	}

	private void Init()
	{
		NKMPointExchangeTemplet byTime = NKMPointExchangeTemplet.GetByTime(NKCSynchronizedTime.ServiceTime);
		if (byTime == null)
		{
			return;
		}
		List<ShopItemTemplet> itemTempletListByTab = NKCShopManager.GetItemTempletListByTab(ShopTabTemplet.Find(byTime.ShopTabStrId, byTime.ShopTabSubIndex));
		if (itemTempletListByTab == null)
		{
			Log.Debug("ShopItemTemplet not exist", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUIPointExchangeSlot.cs", 68);
			return;
		}
		int num = itemTempletListByTab.FindAll((ShopItemTemplet e) => e.m_PointExchangeSpecial)?.Count ?? 0;
		InitProductSlot(num, isSpecialProduct: true);
		InitProductSlot(itemTempletListByTab.Count - num, isSpecialProduct: false);
	}

	private void InitProductSlot(int productCount, bool isSpecialProduct)
	{
		Transform transform = (isSpecialProduct ? m_specialRoot : m_normalRoot);
		if (transform == null)
		{
			return;
		}
		int childCount = transform.childCount;
		int num = productCount - childCount;
		for (int i = 0; i < num; i++)
		{
			if (childCount <= 0)
			{
				Object.Instantiate(isSpecialProduct ? m_specialSlotPrefab : m_normalSlotPrefab, transform);
			}
			else
			{
				Object.Instantiate(transform.GetChild(0).gameObject, transform);
			}
		}
		int childCount2 = transform.childCount;
		for (int j = 0; j < childCount2; j++)
		{
			transform.GetChild(j).GetComponent<NKCUIShopSlotSmall>()?.Init(OnClickProductBuy, null);
			NKCUtil.SetGameobjectActive(transform.GetChild(j).gameObject, bValue: false);
		}
	}

	public void SetData(NKMPointExchangeTemplet pointExchangeTemplet)
	{
		SetProductSlotData(pointExchangeTemplet, isSpecial: true);
		SetProductSlotData(pointExchangeTemplet, isSpecial: false);
	}

	private void SetProductSlotData(NKMPointExchangeTemplet pointExchangeTemplet, bool isSpecial)
	{
		if (pointExchangeTemplet == null)
		{
			return;
		}
		Transform transform = (isSpecial ? m_specialRoot : m_normalRoot);
		if (transform == null)
		{
			return;
		}
		List<ShopItemTemplet> productList = GetProductList(pointExchangeTemplet, isSpecial);
		int count = productList.Count;
		int childCount = transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			if (i >= count)
			{
				NKCUtil.SetGameobjectActive(transform.GetChild(i).gameObject, bValue: false);
				continue;
			}
			NKCUtil.SetGameobjectActive(transform.GetChild(i).gameObject, bValue: true);
			transform.GetChild(i).GetComponent<NKCUIShopSlotSmall>()?.SetData(null, productList[i], NKCShopManager.GetBuyCountLeft(productList[i].m_ProductID));
		}
	}

	private List<ShopItemTemplet> GetProductList(NKMPointExchangeTemplet pointExchangeTemplet, bool isSpecial)
	{
		if (pointExchangeTemplet == null)
		{
			return new List<ShopItemTemplet>();
		}
		List<ShopItemTemplet> itemTempletListByTab = NKCShopManager.GetItemTempletListByTab(ShopTabTemplet.Find(pointExchangeTemplet.ShopTabStrId, pointExchangeTemplet.ShopTabSubIndex));
		if (itemTempletListByTab == null)
		{
			return new List<ShopItemTemplet>();
		}
		List<ShopItemTemplet> list = itemTempletListByTab.FindAll((ShopItemTemplet e) => e.m_PointExchangeSpecial == isSpecial);
		if (list == null)
		{
			return new List<ShopItemTemplet>();
		}
		return list;
	}

	private void OnClickProductBuy(int ProductID)
	{
		NKM_ERROR_CODE nKM_ERROR_CODE = NKCShopManager.OnBtnProductBuy(ProductID, bSupply: false);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK && nKM_ERROR_CODE == NKM_ERROR_CODE.NKE_FAIL_SHOP_INVALID_CHAIN_TAB)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_SHOP_CHAIN_LOCKED, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
	}
}
