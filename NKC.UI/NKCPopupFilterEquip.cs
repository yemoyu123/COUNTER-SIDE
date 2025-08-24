using System.Collections.Generic;

namespace NKC.UI;

public class NKCPopupFilterEquip : NKCUIBase
{
	public enum FILTER_OPEN_TYPE
	{
		NORMAL,
		COLLECTION,
		SELECTION
	}

	public delegate void OnEquipFilterSetChange(NKCEquipSortSystem equipSortSystem);

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_selection";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_FILTER_EQUIP";

	private static NKCPopupFilterEquip m_Instance;

	public NKCUIComStateButton m_btnBackground;

	public NKCUIComStateButton m_btnOk;

	public NKCUIComStateButton m_btnReset;

	public NKCPopupFilterSubUIEquip m_cNKCPopupFilterSubUIEquip;

	private OnEquipFilterSetChange dOnEquipFilterSetChange;

	private NKCEquipSortSystem m_ssActive;

	private bool m_bInitComplete;

	public static NKCPopupFilterEquip Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupFilterEquip>("ab_ui_nkm_ui_popup_selection", "NKM_UI_POPUP_FILTER_EQUIP", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupFilterEquip>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Invalid;

	public override string MenuName => "필 터";

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
		m_ssActive = null;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void Init()
	{
		if (m_btnBackground != null)
		{
			m_btnBackground.PointerClick.RemoveAllListeners();
			m_btnBackground.PointerClick.AddListener(OnClickOk);
		}
		if (m_btnOk != null)
		{
			m_btnOk.PointerClick.RemoveAllListeners();
			m_btnOk.PointerClick.AddListener(OnClickOk);
			NKCUtil.SetHotkey(m_btnOk, HotkeyEventType.Confirm);
		}
		if (m_btnReset != null)
		{
			m_btnReset.PointerClick.RemoveAllListeners();
			m_btnReset.PointerClick.AddListener(OnClickReset);
		}
		m_bInitComplete = true;
	}

	public void Open(HashSet<NKCEquipSortSystem.eFilterCategory> setFilterCategory, NKCEquipSortSystem ssActive, OnEquipFilterSetChange onFilterSetChange, bool bEnableEnchantModuleFilter = false)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
		dOnEquipFilterSetChange = onFilterSetChange;
		m_ssActive = ssActive;
		if (ssActive != null)
		{
			m_ssActive.FilterSet = ssActive.m_EquipListOptions.setFilterOption;
		}
		else
		{
			m_ssActive.FilterSet = new HashSet<NKCEquipSortSystem.eFilterOption>();
		}
		m_cNKCPopupFilterSubUIEquip.OpenFilterPopup(m_ssActive, setFilterCategory, OnSelectFilterOption, bEnableEnchantModuleFilter);
		UIOpened();
	}

	public void OnSelectFilterOption(NKCEquipSortSystem ssActive, NKCEquipSortSystem.eFilterOption selectOption)
	{
		if (ssActive != null)
		{
			m_ssActive = ssActive;
		}
		dOnEquipFilterSetChange?.Invoke(m_ssActive);
	}

	public void OnClickOk()
	{
		Close();
	}

	public void OnClickReset()
	{
		m_cNKCPopupFilterSubUIEquip.ResetFilter();
		m_ssActive.FilterSet.Clear();
		dOnEquipFilterSetChange?.Invoke(m_ssActive);
	}
}
