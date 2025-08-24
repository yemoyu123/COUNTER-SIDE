using System.Collections.Generic;

namespace NKC.UI;

public class NKCPopupFilterOperator : NKCUIBase
{
	public enum FILTER_OPEN_TYPE
	{
		NONE,
		NORMAL,
		COLLECTION,
		SELECTION,
		ALLUNIT_DEV
	}

	public delegate void OnFilterSetChange(NKCOperatorSortSystem ssActive);

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_selection";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_FILTER_OPERATOR";

	private static NKCPopupFilterOperator m_Instance;

	public NKCUIComStateButton m_btnBackground;

	public NKCUIComStateButton m_btnOk;

	public NKCUIComStateButton m_btnReset;

	public NKCPopupFilterSubUIOperator m_cNKCPopupFilterSubUIOperator;

	private OnFilterSetChange dOnFilterSetChange;

	private NKCOperatorSortSystem m_ssActive;

	private bool m_bInitComplete;

	public static NKCPopupFilterOperator Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupFilterOperator>("ab_ui_nkm_ui_popup_selection", "NKM_UI_POPUP_FILTER_OPERATOR", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupFilterOperator>();
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
		m_cNKCPopupFilterSubUIOperator.Close();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void OnBackButton()
	{
		if (m_cNKCPopupFilterSubUIOperator.IsSubfilterOpened)
		{
			m_cNKCPopupFilterSubUIOperator.CloseSubFilter();
		}
		else
		{
			base.OnBackButton();
		}
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

	public void Open(NKCOperatorSortSystem ssActive, HashSet<NKCOperatorSortSystem.eFilterCategory> setFilterCategory, OnFilterSetChange onFilterSetChange)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
		m_ssActive = ssActive;
		dOnFilterSetChange = onFilterSetChange;
		m_cNKCPopupFilterSubUIOperator.OpenFilterPopup(m_ssActive, setFilterCategory, OnSelectFilterOption);
		UIOpened();
	}

	public void OnSelectFilterOption(NKCOperatorSortSystem ssActive)
	{
		if (m_ssActive.FilterSet == null)
		{
			m_ssActive.FilterSet = new HashSet<NKCOperatorSortSystem.eFilterOption>();
		}
		m_ssActive.FilterSet = ssActive.FilterSet;
		dOnFilterSetChange?.Invoke(m_ssActive);
	}

	public void OnClickOk()
	{
		Close();
	}

	public void OnClickReset()
	{
		m_cNKCPopupFilterSubUIOperator.ResetFilterSlot();
		m_ssActive.FilterSet = new HashSet<NKCOperatorSortSystem.eFilterOption>();
		m_ssActive.m_PassiveSkillID = 0;
		dOnFilterSetChange?.Invoke(m_ssActive);
	}

	public static HashSet<NKCOperatorSortSystem.eFilterCategory> MakeDefaultFilterCategory(FILTER_OPEN_TYPE filterOpenType)
	{
		HashSet<NKCOperatorSortSystem.eFilterCategory> hashSet = new HashSet<NKCOperatorSortSystem.eFilterCategory>();
		hashSet.Add(NKCOperatorSortSystem.eFilterCategory.Rarity);
		switch (filterOpenType)
		{
		case FILTER_OPEN_TYPE.COLLECTION:
			hashSet.Add(NKCOperatorSortSystem.eFilterCategory.Collected);
			break;
		case FILTER_OPEN_TYPE.NORMAL:
			hashSet.Add(NKCOperatorSortSystem.eFilterCategory.Level);
			hashSet.Add(NKCOperatorSortSystem.eFilterCategory.Decked);
			hashSet.Add(NKCOperatorSortSystem.eFilterCategory.Locked);
			hashSet.Add(NKCOperatorSortSystem.eFilterCategory.PassiveSkill);
			break;
		case FILTER_OPEN_TYPE.SELECTION:
			hashSet.Add(NKCOperatorSortSystem.eFilterCategory.Have);
			break;
		}
		return hashSet;
	}
}
