using System.Collections.Generic;
using Cs.Logging;
using NKM;
using NKM.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public class NKCPopupShopMultiBuy : NKCUIBase
{
	public delegate void OnOK();

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_shop";

	private const string UI_ASSET_NAME = "NKM_UI_SHOP_POPUP_BUY_ALL";

	private static NKCPopupShopMultiBuy m_Instance;

	public LoopScrollRect m_srItems;

	public NKCUISlot m_pfbSlot;

	public NKCUIComStateButton m_csbtnOK;

	public NKCUIComStateButton m_csbtnCancel;

	public List<NKCUIPriceTag> m_lstPriceTag;

	private OnOK dOnOK;

	private List<NKCUISlot.SlotData> m_lstSlotData = new List<NKCUISlot.SlotData>();

	public static NKCPopupShopMultiBuy Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupShopMultiBuy>("ab_ui_nkm_ui_shop", "NKM_UI_SHOP_POPUP_BUY_ALL", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupShopMultiBuy>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCStringTable.GetString("SI_PF_SUPPLY_SHOP_ITEM_PURCHASE_CONFIRM");

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void InitUI()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnOK, OnBtnOK);
		NKCUtil.SetHotkey(m_csbtnOK, HotkeyEventType.Confirm);
		NKCUtil.SetButtonClickDelegate(m_csbtnCancel, base.Close);
		if (m_srItems != null)
		{
			m_srItems.dOnGetObject += GetSlot;
			m_srItems.dOnReturnObject += ReturnSlot;
			m_srItems.dOnProvideData += ProvideData;
			m_srItems.ContentConstraintCount = 1;
			m_srItems.TotalCount = 0;
			m_srItems.SetAutoResize(3);
			m_srItems.PrepareCells();
		}
	}

	public void OpenForSupply(HashSet<int> m_hsIndex, OnOK onOK)
	{
		dOnOK = onOK;
		m_lstSlotData.Clear();
		NKMShopRandomData randomShop = NKCScenManager.CurrentUserData().m_ShopData.randomShop;
		Dictionary<int, long> dictionary = new Dictionary<int, long>();
		foreach (int item in m_hsIndex)
		{
			if (!randomShop.datas.ContainsKey(item))
			{
				Log.Error("invalid index " + item, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Shop/NKCPopupShopMultiBuy.cs", 92);
				return;
			}
			NKMShopRandomListData nKMShopRandomListData = randomShop.datas[item];
			m_lstSlotData.Add(NKCUISlot.SlotData.MakeShopItemData(nKMShopRandomListData));
			int priceItemId = nKMShopRandomListData.priceItemId;
			if (!dictionary.TryGetValue(priceItemId, out var value))
			{
				value = 0L;
			}
			dictionary[priceItemId] = value + nKMShopRandomListData.GetPrice();
		}
		SetPrice(dictionary);
		UIOpened();
		RefreshScroll();
	}

	public void Open(HashSet<int> m_hsProductID, OnOK onOK)
	{
		dOnOK = onOK;
		m_lstSlotData.Clear();
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		Dictionary<int, long> dictionary = new Dictionary<int, long>();
		foreach (int item in m_hsProductID)
		{
			ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(item);
			bool bFirstBuy = shopItemTemplet != null && NKCShopManager.IsFirstBuy(shopItemTemplet.m_ProductID, NKCScenManager.CurrentUserData());
			m_lstSlotData.Add(NKCUISlot.SlotData.MakeShopItemData(shopItemTemplet, bFirstBuy));
			int priceItemID = shopItemTemplet.m_PriceItemID;
			if (!dictionary.TryGetValue(priceItemID, out var value))
			{
				value = 0L;
			}
			dictionary[priceItemID] = value + nKMUserData.m_ShopData.GetRealPrice(shopItemTemplet);
		}
		SetPrice(dictionary);
		UIOpened();
		RefreshScroll();
	}

	private void SetPrice(Dictionary<int, long> dicPrice)
	{
		if (dicPrice == null)
		{
			foreach (NKCUIPriceTag item in m_lstPriceTag)
			{
				NKCUtil.SetGameobjectActive(item, bValue: false);
			}
			return;
		}
		List<int> list = new List<int>(dicPrice.Keys);
		list.Sort();
		for (int i = 0; i < m_lstPriceTag.Count; i++)
		{
			NKCUIPriceTag nKCUIPriceTag = m_lstPriceTag[i];
			if (!(nKCUIPriceTag == null))
			{
				if (i >= list.Count)
				{
					NKCUtil.SetGameobjectActive(nKCUIPriceTag, bValue: false);
					continue;
				}
				NKCUtil.SetGameobjectActive(nKCUIPriceTag, bValue: true);
				int num = list[i];
				long price = dicPrice[num];
				nKCUIPriceTag.SetData(num, price);
			}
		}
	}

	public void OnBtnOK()
	{
		Close();
		dOnOK?.Invoke();
	}

	public override void CloseInternal()
	{
		m_lstSlotData.Clear();
		base.gameObject.SetActive(value: false);
	}

	public override void OnCloseInstance()
	{
		m_srItems.ReturnAllChild();
	}

	private RectTransform GetSlot(int idx)
	{
		Debug.Log("NKCPopupShopMultiBuy : GetObject");
		NKCUISlot nKCUISlot = Object.Instantiate(m_pfbSlot);
		nKCUISlot.Init();
		RectTransform component = nKCUISlot.GetComponent<RectTransform>();
		if (component == null)
		{
			Object.Destroy(nKCUISlot.gameObject);
		}
		return component;
	}

	private void ReturnSlot(Transform tr)
	{
		Debug.Log("NKCPopupShopMultiBuy : ReturnObject");
		tr.SetParent(base.transform);
		tr.gameObject.SetActive(value: false);
		tr.GetComponent<NKCUISlot>();
		Object.Destroy(tr.gameObject);
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCUISlot component = tr.GetComponent<NKCUISlot>();
		if (component == null)
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
			return;
		}
		if (idx < 0)
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
			return;
		}
		if (idx >= m_lstSlotData.Count)
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
			return;
		}
		component.SetData(m_lstSlotData[idx]);
		component.SetOnClickAction(default(NKCUISlot.SlotClickType));
	}

	private void RefreshScroll(bool bResetScroll = true)
	{
		if (m_srItems != null)
		{
			m_srItems.TotalCount = m_lstSlotData.Count;
			if (bResetScroll)
			{
				m_srItems.SetIndexPosition(0);
			}
			else
			{
				m_srItems.RefreshCells();
			}
		}
	}
}
