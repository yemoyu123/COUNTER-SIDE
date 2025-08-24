using System.Collections.Generic;

namespace NKC.UI;

public class NKCPopupFilterMisc : NKCUIBase
{
	public enum FILTER_TYPE
	{
		NONE,
		NORMAL,
		INTERIOR
	}

	public delegate void OnMiscFilterSetChange(HashSet<NKCMiscSortSystem.eFilterOption> setFilterOption, int selectedTheme);

	public delegate void OnMiscFilterSetChangeTitleCategory(HashSet<int> setFilterOption);

	private const string ASSET_BUNDLE_NAME = "AB_UI_FILTER";

	private const string UI_ASSET_NAME = "AB_UI_FILTER_POPUP_MISC";

	private static NKCPopupFilterMisc m_Instance;

	public NKCUIComStateButton m_btnBackground;

	public NKCUIComStateButton m_btnOk;

	public NKCUIComStateButton m_btnReset;

	public NKCPopupFilterSubUIMisc m_cNKCPopupFilterSubUIMisc;

	private OnMiscFilterSetChange dOnMiscFilterSetChange;

	private HashSet<NKCMiscSortSystem.eFilterOption> m_setMiscFilterOption;

	private OnMiscFilterSetChangeTitleCategory dOnMiscFilterSetChangeTitleCategory;

	private HashSet<int> m_setMiscFilterOptionTitleCategory;

	private bool m_bInitComplete;

	public static NKCPopupFilterMisc Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupFilterMisc>("AB_UI_FILTER", "AB_UI_FILTER_POPUP_MISC", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupFilterMisc>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

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
		if (!m_bInitComplete)
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
	}

	public void Open(HashSet<NKCMiscSortSystem.eFilterCategory> setFilterCategory, HashSet<NKCMiscSortSystem.eFilterOption> setCurrentFilterOption, OnMiscFilterSetChange onFilterSetChange, int currentThemeID, HashSet<int> setFilterCategoryTitle = null, OnMiscFilterSetChangeTitleCategory onFilterSetChangeTitleCategory = null)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
		dOnMiscFilterSetChange = onFilterSetChange;
		dOnMiscFilterSetChangeTitleCategory = onFilterSetChangeTitleCategory;
		if (setCurrentFilterOption != null)
		{
			m_setMiscFilterOption = setCurrentFilterOption;
		}
		else
		{
			m_setMiscFilterOption = new HashSet<NKCMiscSortSystem.eFilterOption>();
		}
		if (setFilterCategoryTitle != null)
		{
			m_setMiscFilterOptionTitleCategory = setFilterCategoryTitle;
		}
		else
		{
			m_setMiscFilterOptionTitleCategory = new HashSet<int>();
		}
		m_cNKCPopupFilterSubUIMisc.OpenFilterPopup(m_setMiscFilterOption, setFilterCategory, OnSelectFilterOption, currentThemeID, m_setMiscFilterOptionTitleCategory, OnSelectFilterOption);
		UIOpened();
	}

	public void OnSelectFilterOption(NKCMiscSortSystem.eFilterOption selectOption, int currentSelectedTheme)
	{
		if (m_setMiscFilterOption == null)
		{
			m_setMiscFilterOption = new HashSet<NKCMiscSortSystem.eFilterOption>();
		}
		if (m_setMiscFilterOption.Contains(selectOption))
		{
			m_setMiscFilterOption.Remove(selectOption);
		}
		else
		{
			m_setMiscFilterOption.Add(selectOption);
		}
		dOnMiscFilterSetChange?.Invoke(m_setMiscFilterOption, currentSelectedTheme);
	}

	public void OnSelectFilterOption(int selectTitleCategoryID)
	{
		if (m_setMiscFilterOptionTitleCategory == null)
		{
			m_setMiscFilterOptionTitleCategory = new HashSet<int>();
		}
		if (m_setMiscFilterOptionTitleCategory.Contains(selectTitleCategoryID))
		{
			m_setMiscFilterOptionTitleCategory.Remove(selectTitleCategoryID);
		}
		else
		{
			m_setMiscFilterOptionTitleCategory.Add(selectTitleCategoryID);
		}
		dOnMiscFilterSetChangeTitleCategory?.Invoke(m_setMiscFilterOptionTitleCategory);
	}

	public void OnClickOk()
	{
		Close();
	}

	public void OnClickReset()
	{
		m_cNKCPopupFilterSubUIMisc.ResetFilter(resetSelectedTheme: true);
		m_setMiscFilterOption.Clear();
		dOnMiscFilterSetChange?.Invoke(m_setMiscFilterOption, 0);
		m_setMiscFilterOptionTitleCategory.Clear();
		dOnMiscFilterSetChangeTitleCategory?.Invoke(m_setMiscFilterOptionTitleCategory);
	}
}
