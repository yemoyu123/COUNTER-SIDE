using System.Collections.Generic;

namespace NKC.UI;

public class NKCPopupFilterMold : NKCUIBase
{
	public enum FILTER_OPEN_TYPE
	{
		NORMAL,
		COLLECTION,
		SELECTION
	}

	public delegate void OnMoldFilterSetChange(HashSet<NKCMoldSortSystem.eFilterOption> setFilterOption);

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_selection";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_FILTER_MOLD";

	private static NKCPopupFilterMold m_Instance;

	public NKCUIComStateButton m_btnBackground;

	public NKCUIComStateButton m_btnOk;

	public NKCUIComStateButton m_btnReset;

	public NKCPopupFilterSubUIMold m_cNKCPopupFilterSubUIMold;

	private OnMoldFilterSetChange dOnMoldFilterSetChange;

	private HashSet<NKCMoldSortSystem.eFilterOption> m_setMoldFilterOption;

	private bool m_bInitComplete;

	public static NKCPopupFilterMold Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupFilterMold>("ab_ui_nkm_ui_popup_selection", "NKM_UI_POPUP_FILTER_MOLD", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupFilterMold>();
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

	public void Open(HashSet<NKCMoldSortSystem.eFilterOption> setFilterOption, OnMoldFilterSetChange onMoldFilterSetChange, List<string> lstFilter)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
		dOnMoldFilterSetChange = onMoldFilterSetChange;
		m_setMoldFilterOption = setFilterOption;
		m_cNKCPopupFilterSubUIMold.OpenFilterPopup(m_setMoldFilterOption, OnSelectFilterOption, lstFilter);
		UIOpened();
	}

	public void OnSelectFilterOption(NKCMoldSortSystem.eFilterOption selectOption)
	{
		if (m_setMoldFilterOption == null)
		{
			m_setMoldFilterOption = new HashSet<NKCMoldSortSystem.eFilterOption>();
		}
		if (m_setMoldFilterOption.Contains(selectOption))
		{
			m_setMoldFilterOption.Remove(selectOption);
		}
		else
		{
			switch (selectOption)
			{
			case NKCMoldSortSystem.eFilterOption.Mold_Status_Enable:
				m_setMoldFilterOption.Remove(NKCMoldSortSystem.eFilterOption.Mold_Status_Disable);
				break;
			case NKCMoldSortSystem.eFilterOption.Mold_Status_Disable:
				m_setMoldFilterOption.Remove(NKCMoldSortSystem.eFilterOption.Mold_Status_Enable);
				break;
			}
			m_cNKCPopupFilterSubUIMold.ResetMoldPartFilter(selectOption);
			m_setMoldFilterOption.Add(selectOption);
		}
		dOnMoldFilterSetChange?.Invoke(m_setMoldFilterOption);
	}

	public void OnClickOk()
	{
		Close();
	}

	public void OnClickReset()
	{
		m_cNKCPopupFilterSubUIMold.ResetFilter();
		m_setMoldFilterOption.Clear();
		dOnMoldFilterSetChange?.Invoke(m_setMoldFilterOption);
	}
}
