using System;
using System.Collections.Generic;
using Cs.Logging;
using NKM;
using NKM.Shop;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIPointExchangeLobby : MonoBehaviour
{
	public Text m_lbRemainTime;

	public Image m_buttonImage;

	public Image m_buttonTitleImage;

	public NKCUIComStateButton m_csbtnButton;

	public GameObject m_objRedDot;

	public GameObject m_objEventClosed;

	public void Init()
	{
		if (!IsEventTime())
		{
			SetAsEmpty();
			return;
		}
		NKMPointExchangeTemplet byTime = NKMPointExchangeTemplet.GetByTime(NKCSynchronizedTime.ServiceTime);
		if (byTime == null)
		{
			SetAsEmpty();
			return;
		}
		base.gameObject.SetActive(value: true);
		NKCUtil.SetGameobjectActive(m_buttonImage, bValue: true);
		NKCUtil.SetGameobjectActive(m_objEventClosed, bValue: false);
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(NKMAssetName.ParseBundleName("ab_ui_nkm_ui_lobby_texture", byTime.BannerId));
		NKCUtil.SetImageSprite(m_buttonImage, orLoadAssetResource);
	}

	public void SetData()
	{
		if (!IsEventTime())
		{
			SetAsEmpty();
			return;
		}
		NKMPointExchangeTemplet byTime = NKMPointExchangeTemplet.GetByTime(NKCSynchronizedTime.ServiceTime);
		if (byTime == null)
		{
			SetAsEmpty();
			return;
		}
		NKMIntervalTemplet eventRemainTime = NKMIntervalTemplet.Find(byTime.IntervalTag);
		SetEventRemainTime(eventRemainTime);
		NKMUserData userData = NKCScenManager.CurrentUserData();
		CheckRedDot(userData, byTime.ShopTabStrId, byTime.ShopTabSubIndex);
		_ = NKCPopupPointExchange.Instance;
		NKCUtil.SetButtonClickDelegate(m_csbtnButton, OnClickButton);
	}

	public static void OpenPtExchangePopup()
	{
		NKCUIPointExchangeTransition.MakeInstance(NKMPointExchangeTemplet.GetByTime(NKCSynchronizedTime.ServiceTime)).Open();
	}

	private bool IsEventTime()
	{
		bool flag = false;
		foreach (NKMPointExchangeTemplet value in NKMTempletContainer<NKMPointExchangeTemplet>.Values)
		{
			if (value == null || !value.EnableByTag)
			{
				continue;
			}
			if (string.IsNullOrEmpty(value.IntervalTag))
			{
				flag = true;
				break;
			}
			NKMIntervalTemplet nKMIntervalTemplet = NKMIntervalTemplet.Find(value.IntervalTag);
			if (nKMIntervalTemplet == null)
			{
				Log.Debug("IntervalTemplet with " + value.IntervalTag + " not exist", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUIPointExchangeLobby.cs", 106);
				continue;
			}
			flag = nKMIntervalTemplet.IsValidTime(NKCSynchronizedTime.ServiceTime);
			if (!flag)
			{
				continue;
			}
			break;
		}
		return flag;
	}

	private void SetEventRemainTime(NKMIntervalTemplet intervalTemplet)
	{
		if (intervalTemplet == null)
		{
			NKCUtil.SetGameobjectActive(m_lbRemainTime, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_lbRemainTime, bValue: true);
		TimeSpan timeLeft = NKCSynchronizedTime.GetTimeLeft(NKCSynchronizedTime.ToUtcTime(intervalTemplet.EndDate));
		string text = "";
		NKCUtil.SetLabelText(msg: (timeLeft.Days > 0) ? string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_DAYS"), timeLeft.Days) : ((timeLeft.Hours <= 0) ? string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_MINUTES"), timeLeft.Minutes) : string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_HOURS"), timeLeft.Hours)), label: m_lbRemainTime);
	}

	private void CheckRedDot(NKMUserData userData, string shopTabStrId, int shopTabSubIndex)
	{
		bool bValue = false;
		List<ShopItemTemplet> itemTempletListByTab = NKCShopManager.GetItemTempletListByTab(ShopTabTemplet.Find(shopTabStrId, shopTabSubIndex));
		if (itemTempletListByTab == null || userData == null)
		{
			NKCUtil.SetGameobjectActive(m_objRedDot, bValue: false);
			return;
		}
		int count = itemTempletListByTab.Count;
		for (int i = 0; i < count; i++)
		{
			if (itemTempletListByTab[i] != null)
			{
				NKMShopData shopData = userData.m_ShopData;
				bool flag = userData.m_InventoryData.GetCountMiscItem(itemTempletListByTab[i].m_PriceItemID) >= itemTempletListByTab[i].m_Price;
				bool flag2 = true;
				if (shopData.histories.ContainsKey(itemTempletListByTab[i].m_ProductID))
				{
					flag2 = shopData.histories[itemTempletListByTab[i].m_ProductID].purchaseCount < itemTempletListByTab[i].m_QuantityLimit;
				}
				if (flag2 && flag)
				{
					bValue = true;
					break;
				}
			}
		}
		NKCUtil.SetGameobjectActive(m_objRedDot, bValue);
	}

	private Sprite GetEmptyButtonImage()
	{
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>(NKMAssetName.ParseBundleName("AB_UI_LOBBY_THUMB_EVENT", "THUMB_EVENT_EMPTY"));
	}

	private void SetAsEmpty()
	{
		GetEmptyButtonImage();
		NKCUtil.SetGameobjectActive(m_buttonImage, bValue: false);
		NKCUtil.SetGameobjectActive(m_objEventClosed, bValue: true);
		m_csbtnButton?.PointerClick.RemoveAllListeners();
	}

	private void OnClickButton()
	{
		OpenPtExchangePopup();
	}
}
