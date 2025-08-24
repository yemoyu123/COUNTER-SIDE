using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupInventoryAdd : NKCUIBase, IScrollHandler, IEventSystemHandler
{
	public struct SliderInfo
	{
		public NKM_INVENTORY_EXPAND_TYPE inventoryType;

		public int increaseCount;

		public int maxCount;

		public int currentCount;
	}

	public delegate void OnClickOK(int value);

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_INVENTORY_ADD";

	private static NKCPopupInventoryAdd m_Instance;

	public Text m_lbTopText;

	public Text m_lbInfoText;

	public Text m_lbCurrentCount;

	public Text m_lbNextCount;

	public Text m_lbAddCount;

	public Text m_lbPlusText;

	public Text m_lbMinusText;

	public Text m_lbAdRemainCount;

	[Header("슬라이더")]
	public GameObject m_objGauge;

	public Slider m_sliderGauge;

	[Header("소모 아이템 아이콘")]
	public NKCUIItemCostSlot m_ItemCostSlot;

	[Header("버튼들")]
	public NKCUIComStateButton m_sbtnPlusButton;

	public NKCUIComStateButton m_sbtnMinusButton;

	public NKCUIComStateButton m_sbtnOkButton;

	public NKCUIComStateButton m_sbtnCancleButton;

	public NKCUIComStateButton m_sbtnCloseButton;

	public NKCUIComStateButton m_sbtnAdOnButton;

	public NKCUIComStateButton m_sbtnAdOffButton;

	public NKCUIComStateButton m_sbtnMoveButton;

	[Header("일본 법무 대응")]
	public GameObject m_JPN_POLICY;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	private NKM_INVENTORY_EXPAND_TYPE m_eInventoryType;

	private int m_iCurrentMaxCount;

	private int m_iIncreaseCount;

	private int m_iIncreaseMaxCount;

	private int m_iNextMaxCount;

	private int m_iFirstIncreaseCount;

	private int m_iRequiredResourceCount;

	private int m_iNeedResourceID;

	private int m_iNeedResourceCount;

	private bool m_bResourceEnough;

	private bool m_bShowResource;

	public OnClickOK m_dOnClickOK;

	public static NKCPopupInventoryAdd Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupInventoryAdd>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX", "NKM_UI_POPUP_INVENTORY_ADD", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupInventoryAdd>();
				m_Instance?.Init();
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

	public override string MenuName => string.Empty;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode
	{
		get
		{
			if (m_bShowResource)
			{
				return NKCUIUpsideMenu.eMode.ResourceOnly;
			}
			return base.eUpsideMenuMode;
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void Init()
	{
		if (m_NKCUIOpenAnimator == null)
		{
			m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		}
		NKCUtil.SetButtonClickDelegate(m_sbtnPlusButton, OnPlus);
		NKCUtil.SetHotkey(m_sbtnPlusButton, HotkeyEventType.Plus);
		NKCUtil.SetButtonClickDelegate(m_sbtnMinusButton, OnMinus);
		NKCUtil.SetHotkey(m_sbtnMinusButton, HotkeyEventType.Minus);
		NKCUtil.SetButtonClickDelegate(m_sbtnOkButton, OnOk);
		NKCUtil.SetHotkey(m_sbtnOkButton, HotkeyEventType.Confirm);
		NKCUtil.SetButtonClickDelegate(m_sbtnCancleButton, OnCancle);
		NKCUtil.SetButtonClickDelegate(m_sbtnCloseButton, OnCancle);
		NKCUtil.SetSliderValueChangedDelegate(m_sliderGauge, OnSliderChanged);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		NKCUtil.SetButtonClickDelegate(m_sbtnMoveButton, OnClickMove);
		NKCUtil.SetButtonClickDelegate(m_sbtnAdOnButton, OnClickAd);
	}

	public void Open(string title, string contentText, SliderInfo sliderInfo, int requiredItemCount, int requiredItemID, OnClickOK onClickOK = null, bool showResource = false)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			if (sliderInfo.currentCount >= sliderInfo.maxCount)
			{
				requiredItemCount = 0;
			}
			long countMiscItem = nKMUserData.m_InventoryData.GetCountMiscItem(requiredItemID);
			m_ItemCostSlot.SetData(requiredItemID, requiredItemCount, countMiscItem);
			m_eInventoryType = sliderInfo.inventoryType;
			m_iCurrentMaxCount = sliderInfo.currentCount;
			m_iIncreaseCount = sliderInfo.increaseCount;
			m_iIncreaseMaxCount = sliderInfo.maxCount;
			m_iFirstIncreaseCount = Mathf.Min(sliderInfo.maxCount, sliderInfo.currentCount + sliderInfo.increaseCount);
			m_iNextMaxCount = m_iFirstIncreaseCount;
			m_iRequiredResourceCount = requiredItemCount;
			m_iNeedResourceID = requiredItemID;
			NKCUtil.SetLabelText(m_lbTopText, title);
			NKCUtil.SetLabelText(m_lbInfoText, contentText);
			NKCUtil.SetLabelText(m_lbCurrentCount, sliderInfo.currentCount.ToString());
			NKCUtil.SetLabelText(m_lbMinusText, $"-{sliderInfo.increaseCount}");
			NKCUtil.SetLabelText(m_lbPlusText, $"+{sliderInfo.increaseCount}");
			int num = (m_iIncreaseMaxCount - m_iFirstIncreaseCount) / m_iIncreaseCount;
			NKCUtil.SetSliderMinMax(m_sliderGauge, 1f, num + 1);
			NKCUtil.SetSliderValue(m_sliderGauge, 1f);
			UpdateNextMaxCountText(m_lbNextCount);
			UpdateAddCountText(m_lbAddCount);
			CheckResourceIsEnough(requiredItemCount, countMiscItem);
			m_bShowResource = showResource;
			m_dOnClickOK = onClickOK;
			NKCUtil.SetGameobjectActive(m_JPN_POLICY, NKCUtil.IsJPNPolicyRelatedItem(requiredItemID));
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			SetAdButtonState(sliderInfo.inventoryType);
			SetMoveButtonState();
			m_sbtnOkButton?.SetLock(m_iCurrentMaxCount > m_iIncreaseMaxCount - m_iIncreaseCount);
			m_NKCUIOpenAnimator?.PlayOpenAni();
			UIOpened();
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator.Update();
		}
	}

	private void UpdateNextMaxCountText(Text nextCountText)
	{
		NKCUtil.SetLabelText(nextCountText, m_iNextMaxCount.ToString());
	}

	private void UpdateAddCountText(Text addCountText)
	{
		NKCUtil.SetLabelText(addCountText, $"+{m_iNextMaxCount - m_iCurrentMaxCount}");
	}

	private int GetRequiredResourceCount()
	{
		return m_iRequiredResourceCount * GetExpandTimes();
	}

	private int GetExpandTimes()
	{
		return Mathf.RoundToInt(m_sliderGauge.value);
	}

	private void CheckResourceIsEnough(int requiredResourceCount, long currentItemCount)
	{
		m_bResourceEnough = requiredResourceCount <= currentItemCount;
		if (!m_bResourceEnough)
		{
			m_iNeedResourceCount = requiredResourceCount - (int)currentItemCount;
		}
	}

	private void UpdateItemCostSlotCount()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			long countMiscItem = nKMUserData.m_InventoryData.GetCountMiscItem(m_iNeedResourceID);
			int requiredResourceCount = GetRequiredResourceCount();
			m_ItemCostSlot.SetCount(requiredResourceCount, countMiscItem);
			CheckResourceIsEnough(requiredResourceCount, countMiscItem);
		}
	}

	private int GetSliderStepValue(int nextMaxCount)
	{
		return 1 + (nextMaxCount - m_iFirstIncreaseCount) / m_iIncreaseCount;
	}

	private void SetAdButtonState(NKM_INVENTORY_EXPAND_TYPE inventoryType)
	{
		if (!NKCAdManager.IsAdRewardInventory(inventoryType))
		{
			NKCUtil.SetGameobjectActive(m_sbtnAdOnButton, bValue: false);
			NKCUtil.SetGameobjectActive(m_sbtnAdOffButton, bValue: false);
			return;
		}
		if (NKCAdManager.InventoryRewardReceived(inventoryType) || m_iCurrentMaxCount >= m_iIncreaseMaxCount)
		{
			NKCUtil.SetGameobjectActive(m_sbtnAdOnButton, bValue: false);
			NKCUtil.SetGameobjectActive(m_sbtnAdOffButton, bValue: true);
			return;
		}
		NKCUtil.SetGameobjectActive(m_sbtnAdOnButton, bValue: true);
		NKCUtil.SetGameobjectActive(m_sbtnAdOffButton, bValue: false);
		NKCUtil.SetLabelText(m_lbAdRemainCount, "(1/1)");
		m_sbtnAdOnButton.SetLock(value: false);
	}

	private void SetMoveButtonState()
	{
		if (m_sbtnMoveButton == null)
		{
			return;
		}
		NKM_SCEN_ID nowScenID = NKCScenManager.GetScenManager().GetNowScenID();
		if (nowScenID == NKM_SCEN_ID.NSI_UNIT_LIST || nowScenID == NKM_SCEN_ID.NSI_INVENTORY)
		{
			NKCUtil.SetGameobjectActive(m_sbtnMoveButton, bValue: false);
			return;
		}
		switch (m_eInventoryType)
		{
		case NKM_INVENTORY_EXPAND_TYPE.NIET_EQUIP:
			m_sbtnMoveButton.SetTitleText(NKCUtilString.GET_STRING_INVEN);
			NKCUtil.SetGameobjectActive(m_sbtnMoveButton, bValue: true);
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_UNIT:
		case NKM_INVENTORY_EXPAND_TYPE.NIET_SHIP:
		case NKM_INVENTORY_EXPAND_TYPE.NIET_OPERATOR:
			m_sbtnMoveButton.SetTitleText(NKCStringTable.GetString("SI_LOBBY_RIGHT_MENU_2_UNITLIST_TEXT"));
			NKCUtil.SetGameobjectActive(m_sbtnMoveButton, bValue: true);
			break;
		default:
			NKCUtil.SetGameobjectActive(m_sbtnMoveButton, bValue: false);
			break;
		}
	}

	private void OnOk()
	{
		if (!m_bResourceEnough)
		{
			Close();
			NKCShopManager.OpenItemLackPopup(m_iNeedResourceID, m_iNeedResourceCount);
		}
		else if (m_iCurrentMaxCount < m_iIncreaseMaxCount)
		{
			Close();
			if (m_dOnClickOK != null)
			{
				m_dOnClickOK(GetExpandTimes());
			}
		}
	}

	private void OnCancle()
	{
		Close();
	}

	private void OnPlus()
	{
		m_iNextMaxCount = Mathf.Min(m_iIncreaseMaxCount, m_iNextMaxCount + m_iIncreaseCount);
		UpdateNextMaxCountText(m_lbNextCount);
		NKCUtil.SetSliderValue(m_sliderGauge, GetSliderStepValue(m_iNextMaxCount));
		UpdateItemCostSlotCount();
		UpdateAddCountText(m_lbAddCount);
	}

	private void OnMinus()
	{
		m_iNextMaxCount = Mathf.Max(m_iFirstIncreaseCount, m_iNextMaxCount - m_iIncreaseCount);
		UpdateNextMaxCountText(m_lbNextCount);
		NKCUtil.SetSliderValue(m_sliderGauge, GetSliderStepValue(m_iNextMaxCount));
		UpdateItemCostSlotCount();
		UpdateAddCountText(m_lbAddCount);
	}

	private void OnSliderChanged(float sliderValue)
	{
		int iNextMaxCount = Mathf.Min(m_iIncreaseMaxCount, m_iCurrentMaxCount + Mathf.RoundToInt(sliderValue) * m_iIncreaseCount);
		m_iNextMaxCount = iNextMaxCount;
		UpdateNextMaxCountText(m_lbNextCount);
		UpdateItemCostSlotCount();
		UpdateAddCountText(m_lbAddCount);
	}

	private void OnClickAd()
	{
		Close();
		NKCAdManager.WatchInventoryRewardAd(m_eInventoryType);
	}

	private void OnClickMove()
	{
		switch (m_eInventoryType)
		{
		case NKM_INVENTORY_EXPAND_TYPE.NIET_EQUIP:
			NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_INVENTORY, "NIT_EQUIP");
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_UNIT:
			NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_UNITLIST, "");
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_SHIP:
			NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_UNITLIST, "ULT_SHIP");
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_OPERATOR:
			NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_UNITLIST, "ULT_OPERATOR");
			break;
		}
	}

	public void OnScroll(PointerEventData eventData)
	{
		if (eventData.scrollDelta.y < 0f)
		{
			m_iNextMaxCount -= m_iIncreaseCount;
		}
		else if (eventData.scrollDelta.y > 0f)
		{
			m_iNextMaxCount += m_iIncreaseCount;
		}
		m_iNextMaxCount = Mathf.Clamp(m_iNextMaxCount, m_iFirstIncreaseCount, m_iIncreaseMaxCount);
		UpdateNextMaxCountText(m_lbNextCount);
		NKCUtil.SetSliderValue(m_sliderGauge, GetSliderStepValue(m_iNextMaxCount));
		UpdateItemCostSlotCount();
		UpdateAddCountText(m_lbAddCount);
	}
}
