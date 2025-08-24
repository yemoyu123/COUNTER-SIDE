using System.Collections.Generic;
using NKC.UI.NPC;
using NKM.Shop;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI.Shop;

public class NKCUIShopSingle : NKCUIShop
{
	[Header("싱글샵 관련")]
	public string m_MenuNameStringKey;

	public eTabMode m_eShopTabMode = eTabMode.All;

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.DEFAULT;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => NKCStringTable.GetString(m_MenuNameStringKey);

	protected override bool AlwaysShowNPC => true;

	protected override bool UseTabVisible => false;

	public static NKCUIShopSingle GetInstance(string bundleName, string assetName)
	{
		NKCUIShopSingle instance = NKCUIManager.OpenNewInstance<NKCUIShopSingle>(bundleName, assetName, NKCUIManager.eUIBaseRect.UIFrontCommon, null).GetInstance<NKCUIShopSingle>();
		instance.Init();
		return instance;
	}

	public override void OnBackButton()
	{
		Close();
	}

	public void Open(NKCShopManager.ShopTabCategory category, string selectedTab = "TAB_NONE", int subTabIndex = 0)
	{
		base.gameObject.SetActive(value: true);
		m_eTabMode = m_eShopTabMode;
		NKCShopManager.FetchShopItemList(NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID, delegate(bool bSuccess)
		{
			if (bSuccess)
			{
				OpenWithItemList(category, selectedTab, subTabIndex);
			}
			else
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_SHOP_WAS_NOT_ABLE_TO_GET_PRODUCT_LIST_FROM_SERVER, base.Close);
			}
		});
	}

	private void OpenWithItemList(NKCShopManager.ShopTabCategory category, string selectedTab = "TAB_NONE", int subTabIndex = 0)
	{
		BuildProductList(bForce: false);
		if (m_eCategory != category)
		{
			CleanupTab();
		}
		m_eCategory = category;
		List<ShopTabTemplet> useTabList = NKCShopManager.GetUseTabList(m_eCategory);
		if (m_dicTab.Count == 0)
		{
			BuildTabs(useTabList);
		}
		if (!string.IsNullOrEmpty(selectedTab) && selectedTab != "TAB_NONE" && !useTabList.Exists((ShopTabTemplet x) => x.TabType == selectedTab))
		{
			Debug.LogError($"Tab {selectedTab} does not exist in category {category}!");
			selectedTab = "TAB_NONE";
		}
		if (selectedTab == "TAB_NONE")
		{
			selectedTab = useTabList[0].TabType;
		}
		SelectTab(selectedTab, subTabIndex, bForce: true);
		NKCUtil.SetLabelText(m_lbSupplyRefreshCost, 15.ToString());
		base.gameObject.SetActive(value: true);
		UIOpened();
		m_UINPCShop?.PlayAni(NPC_ACTION_TYPE.START);
	}

	public override void OnProductBuy(ShopItemTemplet productTemplet)
	{
		if (productTemplet != null && productTemplet.m_ItemType != NKM_REWARD_TYPE.RT_SKIN)
		{
			m_UINPCShop?.PlayAni(NPC_ACTION_TYPE.THANKS);
		}
	}
}
