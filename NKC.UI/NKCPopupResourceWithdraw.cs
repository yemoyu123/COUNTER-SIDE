using NKM;
using NKM.Shop;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupResourceWithdraw : NKCUIBase
{
	public delegate void OnOk();

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_RESOURCE_WITHDRAW";

	private static NKCPopupResourceWithdraw m_Instance;

	public NKCUIComStateButton m_btnBG;

	public NKCUIComStateButton m_csbtnClose;

	public Text m_lbTitle;

	public Text m_lbText;

	public GameObject m_objRegain;

	public Text m_lbRegainDesc;

	public Image m_imgRegainItem;

	public Text m_lbRegainItemCount;

	public NKCUIComStateButton m_csbtnCancel;

	public NKCUIComStateButton m_csbtnOK;

	public Text m_lbOK;

	public NKCUIPriceTag m_tagPrice;

	private OnOk dOnOk;

	public static NKCPopupResourceWithdraw Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupResourceWithdraw>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_RESOURCE_WITHDRAW", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupResourceWithdraw>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCUtilString.GET_STRING_POPUP_RESOURCE_WITHDRAW;

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

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	private void Init()
	{
		if (m_csbtnCancel != null)
		{
			m_csbtnCancel.PointerClick.RemoveAllListeners();
			m_csbtnCancel.PointerClick.AddListener(base.Close);
		}
		else
		{
			Debug.LogError("Cancel Button Not Found!");
		}
		if (m_csbtnClose != null)
		{
			m_csbtnClose.PointerClick.RemoveAllListeners();
			m_csbtnClose.PointerClick.AddListener(base.Close);
		}
		else
		{
			Debug.LogError("Close Button Not Found!");
		}
		if (m_csbtnOK != null)
		{
			m_csbtnOK.PointerClick.RemoveAllListeners();
			m_csbtnOK.PointerClick.AddListener(OnBtnOK);
			NKCUtil.SetHotkey(m_csbtnOK, HotkeyEventType.Confirm);
		}
		else
		{
			Debug.LogError("OK Button Not Found!");
		}
		if (m_btnBG != null)
		{
			m_btnBG.PointerClick.RemoveAllListeners();
			m_btnBG.PointerClick.AddListener(base.Close);
		}
	}

	public void OpenForShopBuyAll(ShopTabTemplet tabTemplet, OnOk onOK)
	{
		if (tabTemplet != null)
		{
			dOnOk = onOK;
			NKCUIBase.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_SHOP_BUY_ALL_TITLE);
			NKCUIBase.SetLabelText(m_lbText, NKCUtilString.GET_STRING_SHOP_BUY_ALL_DESC);
			NKCUtil.SetGameobjectActive(m_objRegain, bValue: false);
			_ = NKCScenManager.CurrentUserData().m_ShopData;
			m_tagPrice.SetData(NKCShopManager.GetBundleItemPriceItemID(tabTemplet), NKCShopManager.GetBundleItemPrice(tabTemplet), showMinus: false, changeColor: false, bHidePriceIcon: true);
			NKCUtil.SetLabelText(m_lbOK, NKCUtilString.GET_STRING_SHOP_BUY_ALL_TITLE);
			UIOpened();
		}
	}

	public void OpenForWorldmapBuildingRemove(NKMWorldMapBuildingTemplet.LevelTemplet targetBuildingTemplet, OnOk onOK)
	{
		if (targetBuildingTemplet != null)
		{
			dOnOk = onOK;
			NKCUIBase.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_WORLDMAP_BUILDING_REMOVE);
			NKCUIBase.SetLabelText(m_lbText, NKCUtilString.GET_STRING_WORLDMAP_BUILDING_REMOVE_DESC_TWO_PARAM, targetBuildingTemplet.level, targetBuildingTemplet.GetName());
			NKCUtil.SetGameobjectActive(m_objRegain, bValue: true);
			NKCUIBase.SetLabelText(m_lbRegainDesc, NKCUtilString.GET_STRING_WORLDMAP_BUILDING_REMOVE_POINT);
			Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_INVEN_ICON_ITEM_MISC_SMALL", "AB_INVEN_ICON_ITEM_MISC_RESOURCE_BUILDING_POINT");
			NKCUIBase.SetImageSprite(m_imgRegainItem, orLoadAssetResource, bDisableIfSpriteNull: true);
			NKCUIBase.SetLabelText(m_lbRegainItemCount, NKMWorldMapManager.GetTotalBuildingPointUsed(targetBuildingTemplet).ToString());
			m_tagPrice.SetData(targetBuildingTemplet.ClearCostItem.ItemID, targetBuildingTemplet.ClearCostItem.Count, showMinus: false, changeColor: false);
			NKCUtil.SetLabelText(m_lbOK, NKCStringTable.GetString("SI_PF_COMMON_OK_2"));
			UIOpened();
		}
	}

	public void OpenForRestoreEnterLimit(NKMStageTempletV2 stageTemplet, OnOk onOK, int restoreCnt = 0)
	{
		if (stageTemplet != null)
		{
			dOnOk = onOK;
			string text = "";
			text = stageTemplet.EnterLimitCond switch
			{
				NKMStageTempletV2.RESET_TYPE.DAY => string.Format(NKCUtilString.GET_STRING_WARFARE_GAME_HUD_RESTORE_LIMIT_DESC_DAY, stageTemplet.RestoreLimitEnterCount, stageTemplet.RestoreLimit - restoreCnt, stageTemplet.RestoreLimit), 
				NKMStageTempletV2.RESET_TYPE.WEEK => string.Format(NKCUtilString.GET_STRING_WARFARE_GAME_HUD_RESTORE_LIMIT_DESC_WEEK, stageTemplet.RestoreLimitEnterCount, stageTemplet.RestoreLimit - restoreCnt, stageTemplet.RestoreLimit), 
				NKMStageTempletV2.RESET_TYPE.MONTH => string.Format(NKCUtilString.GET_STRING_WARFARE_GAME_HUD_RESTORE_LIMIT_DESC_MONTH, stageTemplet.RestoreLimitEnterCount, stageTemplet.RestoreLimit - restoreCnt, stageTemplet.RestoreLimit), 
				_ => string.Format(NKCUtilString.GET_STRING_WARFARE_GAME_HUD_RESTORE_LIMIT_DESC_DAY, stageTemplet.RestoreLimitEnterCount, stageTemplet.RestoreLimit - restoreCnt, stageTemplet.RestoreLimit), 
			};
			NKCUtil.SetGameobjectActive(m_objRegain, bValue: false);
			NKCUIBase.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_NOTICE);
			NKCUIBase.SetLabelText(m_lbText, text);
			m_tagPrice.SetData(stageTemplet.RestoreReqItem.ItemId, stageTemplet.RestoreReqItem.Count32, showMinus: false, changeColor: false);
			UIOpened();
		}
	}

	private void OnBtnOK()
	{
		dOnOk?.Invoke();
		Close();
	}
}
