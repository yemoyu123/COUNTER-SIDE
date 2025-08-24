using NKC.UI.NPC;
using NKM;
using NKM.Shop;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public class NKCUIShopSingleTab : NKCUIShop
{
	public eTabMode m_eShopTabMode = eTabMode.All;

	[Header("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public TMP_Text m_lbTitle;

	public TMP_Text m_lbSubTitle;

	public Image m_imgBG;

	private string m_MenuNameStringKey;

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.DEFAULT;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => NKCStringTable.GetString(m_MenuNameStringKey);

	protected override bool AlwaysShowNPC => true;

	protected override bool UseTabVisible => false;

	public static NKCUIShopSingleTab GetInstance(string bundleName, string assetName)
	{
		NKCUIShopSingleTab instance = NKCUIManager.OpenNewInstance<NKCUIShopSingleTab>(bundleName, assetName, NKCUIManager.eUIBaseRect.UIFrontCommon, null).GetInstance<NKCUIShopSingleTab>();
		instance.Init();
		return instance;
	}

	public override void OnBackButton()
	{
		Close();
	}

	public void Open(string title, string subTitle, int resourceID, NKMAssetName cNKMAssetName, NKCShopManager.ShopTabCategory category, string selectedTab = "TAB_NONE", int subTabIndex = 0)
	{
		base.gameObject.SetActive(value: true);
		NKCUtil.SetLabelText(m_lbTitle, title);
		NKCUtil.SetLabelText(m_lbSubTitle, subTitle);
		if (m_imgBG != null && cNKMAssetName != null)
		{
			NKCUtil.SetImageSprite(m_imgBG, NKCResourceUtility.GetOrLoadAssetResource<Sprite>(cNKMAssetName));
		}
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
