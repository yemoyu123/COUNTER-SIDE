using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupItemSlider : NKCUIBase, IScrollHandler, IEventSystemHandler
{
	public delegate void OnConfirm(int count);

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_ITEM_SLIDER";

	private static NKCPopupItemSlider m_Instance;

	public Text m_lbTitle;

	public NKCUISlot m_Slot;

	public Text m_lbDescription;

	public Text m_lbHaveCount;

	public Text m_lbCurrentCount;

	public Slider m_Slider;

	public GameObject m_objGauge;

	public NKCUIComStateButton m_csbtnPlus;

	public NKCUIComStateButton m_csbtnMinus;

	public NKCUIComStateButton m_csbtnConfirm;

	public NKCUIComStateButton m_csbtnCancel;

	public NKCUIComStateButton m_csbtnClose;

	private int m_minValue;

	private int m_maxValue = 1;

	private int m_destMax;

	private int m_currentValue;

	private bool m_bShowCount;

	private OnConfirm dOnConfirm;

	private NKCUISlot.SlotData m_originalSlotData;

	public static NKCPopupItemSlider Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupItemSlider>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_ITEM_SLIDER", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupItemSlider>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public static bool HasInstance => m_Instance != null;

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

	public override string MenuName => "";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnPlus, OnPlus);
		NKCUtil.SetHotkey(m_csbtnPlus, HotkeyEventType.Plus);
		NKCUtil.SetButtonClickDelegate(m_csbtnMinus, OnMinus);
		NKCUtil.SetHotkey(m_csbtnMinus, HotkeyEventType.Minus);
		NKCUtil.SetButtonClickDelegate(m_csbtnConfirm, OnOK);
		NKCUtil.SetHotkey(m_csbtnConfirm, HotkeyEventType.Confirm);
		NKCUtil.SetButtonClickDelegate(m_csbtnCancel, base.Close);
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, base.Close);
		NKCUtil.SetSliderValueChangedDelegate(m_Slider, OnSliderChange);
	}

	public void Open(string title, string desc, NKCUISlot.SlotData itemSlotData, int minValue, int maxValue, int destMax, bool bShowHaveCount, OnConfirm onConfirm, int currentValue = 1)
	{
		NKCUtil.SetLabelText(m_lbTitle, title);
		NKCUtil.SetLabelText(m_lbDescription, desc);
		m_minValue = minValue;
		m_maxValue = maxValue;
		m_destMax = destMax;
		m_bShowCount = bShowHaveCount && itemSlotData.eType == NKCUISlot.eSlotMode.ItemMisc;
		dOnConfirm = onConfirm;
		NKCUtil.SetGameobjectActive(m_lbHaveCount, bShowHaveCount);
		if (bShowHaveCount)
		{
			long countMiscItem = NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(itemSlotData.ID);
			NKCUtil.SetLabelText(m_lbHaveCount, NKCUtilString.GET_STRING_TOOLTIP_QUANTITY_ONE_PARAM, countMiscItem);
		}
		m_originalSlotData = itemSlotData;
		m_currentValue = Mathf.Clamp(currentValue, minValue, maxValue);
		if (m_Slider != null)
		{
			m_Slider.wholeNumbers = true;
			m_Slider.maxValue = maxValue;
			m_Slider.minValue = minValue;
		}
		NKCUtil.SetGameobjectActive(m_objGauge, maxValue > 1);
		UpdateValue(m_currentValue);
		UIOpened();
	}

	private void OnOK()
	{
		Close();
		dOnConfirm?.Invoke(m_currentValue);
	}

	private void OnSliderChange(float value)
	{
		UpdateValue((int)value);
	}

	private void OnPlus()
	{
		UpdateValue(m_currentValue + 1);
	}

	private void OnMinus()
	{
		UpdateValue(m_currentValue - 1);
	}

	private void UpdateValue(int newValue)
	{
		m_currentValue = Mathf.Clamp(newValue, m_minValue, m_maxValue);
		if (m_Slot != null)
		{
			NKCUISlot.SlotData slotData = new NKCUISlot.SlotData(m_originalSlotData);
			slotData.Count = m_originalSlotData.Count * m_currentValue;
			m_Slot.SetMiscItemData(slotData, bShowName: false, m_bShowCount, bEnableLayoutElement: false, null);
			m_Slot.SetOnClickAction(default(NKCUISlot.SlotClickType));
		}
		NKCUtil.SetLabelText(m_lbCurrentCount, $"{m_currentValue}/{m_destMax}");
		if (m_Slider != null)
		{
			m_Slider.value = m_currentValue;
		}
	}

	public void OnScroll(PointerEventData eventData)
	{
		if (eventData.scrollDelta.y < 0f)
		{
			UpdateValue(m_currentValue - 1);
		}
		else if (eventData.scrollDelta.y > 0f)
		{
			UpdateValue(m_currentValue + 1);
		}
	}
}
