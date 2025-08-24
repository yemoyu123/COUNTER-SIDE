using NKC.UI.Shop;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupItemLack : NKCUIBase
{
	public delegate void OnCancel();

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_BOX_ITEM_LACK";

	private static NKCPopupItemLack m_Instance;

	public Text m_lbTitle;

	public Text m_lbDesc;

	public NKCUIItemCostSlot m_slot;

	public NKCUIComStateButton m_btnOK;

	public NKCUIComStateButton m_btnBuy;

	public NKCUIComStateButton m_btnCancel_OkCancel;

	public EventTrigger m_eventTriggerBG;

	public GameObject m_objOkRoot;

	public GameObject m_objOkCancelRoot;

	public GameObject m_objLayout;

	[Header("아이템 획득처")]
	public GameObject m_objDummy;

	public NKCUIComItemDropInfo m_itemDropInfo;

	[Header("광고")]
	public GameObject m_objAdImage;

	public NKCUIComStateButton m_csbtnAdOn;

	public NKCUIComStateButton m_csbtnAdOff;

	public Text m_lbAdRemainCount;

	public Text m_lbAdCoolTime;

	private NKCUIOpenAnimator m_openAni;

	private TabId shopTab;

	private int m_needItemId;

	private int m_needItemCount;

	private NKCUISlot.SlotData m_slotData;

	private OnCancel m_dOnCancel;

	public static NKCPopupItemLack Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupItemLack>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_BOX_ITEM_LACK", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupItemLack>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCUtilString.GET_STRING_POPUP_ITEM_LACK;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_needItemId = 0;
		m_needItemCount = 0;
		m_slotData = null;
	}

	private void InitUI()
	{
		m_btnOK.PointerClick.RemoveAllListeners();
		m_btnOK.PointerClick.AddListener(OnClickOK);
		NKCUtil.SetHotkey(m_btnOK, HotkeyEventType.Confirm);
		m_btnBuy.PointerClick.RemoveAllListeners();
		m_btnBuy.PointerClick.AddListener(OnClickBuy);
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			Close();
		});
		m_eventTriggerBG.triggers.Add(entry);
		m_openAni = new NKCUIOpenAnimator(base.gameObject);
		m_itemDropInfo?.Init();
		NKCUtil.SetButtonClickDelegate(m_csbtnAdOn, OnClickAd);
		NKCUtil.SetButtonClickDelegate(m_btnCancel_OkCancel, OnClickCancel);
	}

	public void OpenItemMiscLackPopup(int needItemID, int needItemCount)
	{
		NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeMiscItemData(needItemID, needItemCount);
		OpenItemMiscLackPopup(slotData, needItemCount, NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(slotData.ID));
	}

	public void OpenItemMiscLackPopup(int needItemID, int needItemCount, long curItemCount)
	{
		NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeMiscItemData(needItemID, needItemCount);
		OpenItemMiscLackPopup(slotData, needItemCount, curItemCount);
	}

	public void OpenItemMiscLackPopup(NKCUISlot.SlotData slotData, int needCount, long curItemCount)
	{
		if (m_openAni != null)
		{
			m_openAni.PlayOpenAni();
		}
		NKCUtil.SetGameobjectActive(m_objOkRoot, bValue: true);
		NKCUtil.SetGameobjectActive(m_objOkCancelRoot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLayout, bValue: true);
		NKCUtil.SetGameobjectActive(m_objAdImage, bValue: false);
		NKCUtil.SetGameobjectActive(m_csbtnAdOn, bValue: false);
		NKCUtil.SetGameobjectActive(m_csbtnAdOff, bValue: false);
		m_needItemId = slotData.ID;
		m_needItemCount = needCount;
		m_slotData = slotData;
		m_slot.SetData(slotData.ID, needCount, curItemCount);
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(slotData.ID);
		m_lbTitle.text = NKCUtilString.GET_STRING_NOTICE;
		if (itemMiscTempletByID != null)
		{
			m_lbDesc.text = string.Format(NKCUtilString.GET_STRING_ITEM_LACK_DESC_ONE_PARAM, itemMiscTempletByID.GetItemName());
		}
		shopTab = NKCShopManager.GetShopMoveTab(slotData.ID);
		NKCUtil.SetGameobjectActive(m_btnBuy, shopTab.Type != "TAB_NONE");
		bool valueOrDefault = m_itemDropInfo?.SetData(slotData) == true;
		NKCUtil.SetGameobjectActive(m_objDummy, valueOrDefault);
		UIOpened();
	}

	public void OpenItemLackAdRewardPopup(int itemId, OnCancel onCancel)
	{
		if (m_openAni != null)
		{
			m_openAni.PlayOpenAni();
		}
		m_slot.SetData(itemId, 0, 0L);
		NKCUtil.SetGameobjectActive(m_objOkRoot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objOkCancelRoot, bValue: true);
		NKCUtil.SetGameobjectActive(m_objLayout, bValue: false);
		NKCUtil.SetGameobjectActive(m_objAdImage, bValue: true);
		NKCUtil.SetGameobjectActive(m_btnBuy, bValue: false);
		NKCAdManager.SetItemRewardAdButtonState(NKCPopupItemBox.eMode.MoveToShop, itemId, m_csbtnAdOn, m_csbtnAdOff, m_lbAdRemainCount);
		m_dOnCancel = onCancel;
		UIOpened();
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		if (IsInstanceOpen)
		{
			if (m_itemDropInfo != null && m_itemDropInfo.gameObject.activeSelf)
			{
				m_itemDropInfo.SetData(m_slotData, initScrollPosition: false);
			}
			long countMiscItem = NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(m_needItemId);
			m_slot.SetData(m_needItemId, m_needItemCount, countMiscItem);
		}
	}

	public void OnClickOK()
	{
		Close();
	}

	public void OnClickBuy()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.LOBBY_SUBMENU))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.LOBBY_SUBMENU);
			return;
		}
		Close();
		if (!(shopTab.Type == "TAB_NONE"))
		{
			NKCUIShop.ShopShortcut(shopTab.Type, shopTab.SubIndex);
		}
	}

	private void Update()
	{
		if (base.IsOpen)
		{
			if (m_openAni != null)
			{
				m_openAni.Update();
			}
			if (m_slot != null)
			{
				NKCAdManager.UpdateItemRewardAdCoolTime(m_slot.ItemID, m_csbtnAdOn, m_csbtnAdOff, m_lbAdCoolTime, m_lbAdRemainCount);
			}
		}
	}

	private void OnClickAd()
	{
		Close();
		if (m_slot != null)
		{
			NKCAdManager.WatchItemRewardAd(m_slot.ItemID);
		}
	}

	private void OnClickCancel()
	{
		Close();
		if (m_dOnCancel != null)
		{
			m_dOnCancel();
		}
	}
}
