using System.Collections.Generic;
using ClientPacket.Shop;
using NKC.Publisher;
using NKC.UI;
using NKC.UI.Shop;
using NKM;
using NKM.Shop;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_SHOP : NKC_SCEN_BASIC
{
	private const float FIVE_SECONDS = 5f;

	private float m_deltaTime;

	private string m_bReservedOpenTab = "TAB_NONE";

	private int m_ReservedOpenIndex;

	private int m_ReservedOpenProductID;

	public NKC_SCEN_SHOP()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_SHOP;
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		if (!NKCEmoticonManager.m_bReceivedEmoticonData)
		{
			Send_NKMPacket_EMOTICON_DATA_REQ();
		}
		NKCShopManager.RequestShopItemList(NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID, bForceRefreshServerItemList: true);
	}

	public override void ScenLoadUIUpdate()
	{
		m_deltaTime += Time.deltaTime;
		if (m_deltaTime > 5f)
		{
			m_deltaTime = 0f;
			Set_NKC_SCEN_STATE(NKC_SCEN_STATE.NSS_FAIL);
		}
		else if (NKCShopManager.IsShopItemListReady && NKCEmoticonManager.m_bReceivedEmoticonData)
		{
			m_deltaTime = 0f;
			base.ScenLoadUIUpdate();
		}
	}

	public override void ScenLoadUIComplete()
	{
		base.ScenLoadUIComplete();
	}

	public override void ScenStart()
	{
		base.ScenStart();
		if (!NKCShopManager.IsShopItemListReady)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_SHOP_WAS_NOT_ABLE_TO_GET_PRODUCT_LIST_FROM_SERVER, ToHomeScene);
			return;
		}
		if (!NKCEmoticonManager.m_bReceivedEmoticonData)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_SHOP_WAS_NOT_ABLE_TO_GET_PRODUCT_LIST_FROM_SERVER, ToHomeScene);
			return;
		}
		NKCPublisherModule.InAppPurchase.BillingRestore(NKCShopManager.OnBillingRestore);
		if (m_bReservedOpenTab != "TAB_NONE")
		{
			NKCShopManager.ShopTabCategory category = NKCShopManager.GetCategoryFromTab(ShopTabTemplet.Find(m_bReservedOpenTab, m_ReservedOpenIndex).TabType)?.m_eCategory ?? NKCShopManager.ShopTabCategory.PACKAGE;
			NKCUIShop.Instance.Open(category, m_bReservedOpenTab, m_ReservedOpenIndex, m_ReservedOpenProductID);
			m_bReservedOpenTab = "TAB_NONE";
			m_ReservedOpenIndex = 0;
			m_ReservedOpenProductID = 0;
		}
		else
		{
			NKCUIShop.Instance.Open(NKCShopManager.ShopTabCategory.PACKAGE);
		}
		NKCCamera.EnableBloom(bEnable: false);
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		NKCUIShop.CheckInstanceAndClose();
		UnloadUI();
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
	}

	public override void ScenUpdate()
	{
		base.ScenUpdate();
	}

	public override bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		return false;
	}

	private void Send_NKMPacket_EMOTICON_DATA_REQ()
	{
		NKCPacketSender.Send_NKMPacket_EMOTICON_DATA_REQ(NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
	}

	public void OnRecvProductBuyCheck(string productMarketID, List<int> lstSelection)
	{
		ShopItemTemplet productTemplet = NKCShopManager.GetShopTempletByMarketID(productMarketID);
		if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.IGNORE_BILLING_RESTORE) && (NKCDefineManager.DEFINE_NXTOY() || NKCDefineManager.DEFINE_NXTOY_JP()))
		{
			NKCPublisherModule.InAppPurchase.BillingRestore(BillingRestoreComplete);
		}
		else
		{
			InAppPurchase();
		}
		void BillingRestoreComplete(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
		{
			switch (resultCode)
			{
			case NKC_PUBLISHER_RESULT_CODE.NPRC_INAPP_NOT_EXIST_RESTORE_ITEM:
				InAppPurchase();
				break;
			case NKC_PUBLISHER_RESULT_CODE.NPRC_OK:
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_TOY_NOT_EXIST_RESTORE_ITEM);
				break;
			default:
				NKCShopManager.OnBillingRestore(resultCode, additionalError);
				break;
			}
		}
		void InAppPurchase()
		{
			if (productTemplet != null && productTemplet.m_PriceItemID == 0)
			{
				if (NKCPublisherModule.InAppPurchase.IsRegisteredProduct(productMarketID, productTemplet.m_ProductID))
				{
					NKCPublisherModule.InAppPurchase.InappPurchase(productTemplet, NKCShopManager.OnInappPurchase, "", lstSelection);
				}
				else
				{
					NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_SHOP_NOT_REGISTED_PRODUCT);
				}
			}
			else
			{
				Debug.LogWarning("[Shop] OnRecvProductBuyCheck failed marketID[" + productMarketID + "] productTemplet[" + productTemplet?.m_ProductID.ToString() + "]");
			}
		}
	}

	public void Send_NKMPacket_SHOP_RANDOM_SHOP_BUY_REQ(int slotIndex)
	{
		NKMPacket_SHOP_RANDOM_SHOP_BUY_REQ nKMPacket_SHOP_RANDOM_SHOP_BUY_REQ = new NKMPacket_SHOP_RANDOM_SHOP_BUY_REQ();
		nKMPacket_SHOP_RANDOM_SHOP_BUY_REQ.slotIndex = slotIndex;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_SHOP_RANDOM_SHOP_BUY_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public void Send_NKMPacket_SHOP_REFRESH_REQ(bool bUseCash)
	{
		NKMPacket_SHOP_REFRESH_REQ nKMPacket_SHOP_REFRESH_REQ = new NKMPacket_SHOP_REFRESH_REQ();
		nKMPacket_SHOP_REFRESH_REQ.isUseCash = bUseCash;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_SHOP_REFRESH_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public void Send_NKMPacket_SHOP_CHAIN_TAB_RESET_TIME_REQ()
	{
		NKMPacket_SHOP_CHAIN_TAB_RESET_TIME_REQ packet = new NKMPacket_SHOP_CHAIN_TAB_RESET_TIME_REQ();
		NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public void ToHomeScene()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME, bForce: false);
	}

	public void SetReservedOpenTab(string shopType, int tabIndex = 0, int productID = 0)
	{
		m_bReservedOpenTab = shopType;
		m_ReservedOpenIndex = tabIndex;
		m_ReservedOpenProductID = productID;
	}

	public void MoveToReservedTab()
	{
		if (m_bReservedOpenTab != "TAB_NONE")
		{
			NKCUIShop.ShopShortcut(m_bReservedOpenTab, m_ReservedOpenIndex, m_ReservedOpenProductID);
			m_bReservedOpenTab = "TAB_NONE";
			m_ReservedOpenIndex = 0;
			m_ReservedOpenProductID = 0;
		}
	}
}
