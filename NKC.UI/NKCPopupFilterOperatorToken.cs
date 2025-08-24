using System.Collections.Generic;

namespace NKC.UI;

public class NKCPopupFilterOperatorToken : NKCUIBase
{
	public delegate void OnFilterSetChange(NKCOperatorTokenSortSystem ssActive);

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_selection";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_FILTER_OPERATOR_TOKEN";

	private static NKCPopupFilterOperatorToken m_Instance;

	public NKCUIComStateButton m_btnBackground;

	public NKCUIComStateButton m_btnOk;

	public NKCUIComStateButton m_btnReset;

	public NKCPopupFilterSubUIOperatorToken m_cNKCPopupFilterSubUIOperatorToken;

	private OnFilterSetChange dOnFilterSetChange;

	private NKCOperatorTokenSortSystem m_ssActive;

	private bool m_bInitComplete;

	public static NKCPopupFilterOperatorToken Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupFilterOperatorToken>("ab_ui_nkm_ui_popup_selection", "NKM_UI_POPUP_FILTER_OPERATOR_TOKEN", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupFilterOperatorToken>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Invalid;

	public override string MenuName => "\ufffd\ufffd \ufffd\ufffd";

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
		m_cNKCPopupFilterSubUIOperatorToken.Close();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void OnBackButton()
	{
		if (m_cNKCPopupFilterSubUIOperatorToken.IsSubfilterOpened)
		{
			m_cNKCPopupFilterSubUIOperatorToken.CloseSubFilter();
		}
		else
		{
			base.OnBackButton();
		}
	}

	private void Init()
	{
		NKCUtil.SetBindFunction(m_btnBackground, OnClickOk);
		NKCUtil.SetBindFunction(m_btnOk, OnClickOk);
		NKCUtil.SetHotkey(m_btnOk, HotkeyEventType.Confirm);
		NKCUtil.SetBindFunction(m_btnReset, OnClickReset);
		m_bInitComplete = true;
	}

	public void Open(NKCOperatorTokenSortSystem ssActive, HashSet<NKCOperatorTokenSortSystem.eFilterCategory> setFilterCategory, OnFilterSetChange onFilterSetChange)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
		m_ssActive = ssActive;
		dOnFilterSetChange = onFilterSetChange;
		m_cNKCPopupFilterSubUIOperatorToken.OpenFilterPopup(m_ssActive, setFilterCategory, OnSelectFilterOption);
		UIOpened();
	}

	public void OnSelectFilterOption(NKCOperatorTokenSortSystem ssActive)
	{
		if (m_ssActive.FilterSet == null)
		{
			m_ssActive.FilterSet = new HashSet<NKCOperatorTokenSortSystem.eFilterOption>();
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
		m_cNKCPopupFilterSubUIOperatorToken.ResetFilterSlot();
		m_ssActive.FilterSet = new HashSet<NKCOperatorTokenSortSystem.eFilterOption>();
		m_ssActive.m_PassiveSkillID = 0;
		dOnFilterSetChange?.Invoke(m_ssActive);
	}

	public static HashSet<NKCOperatorTokenSortSystem.eFilterCategory> MakeDefaultFilterCategory()
	{
		return new HashSet<NKCOperatorTokenSortSystem.eFilterCategory>
		{
			NKCOperatorTokenSortSystem.eFilterCategory.Rarity,
			NKCOperatorTokenSortSystem.eFilterCategory.PassiveSkill
		};
	}
}
