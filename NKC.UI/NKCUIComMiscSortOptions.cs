using System.Collections.Generic;
using NKC.UI.Guide;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIComMiscSortOptions : MonoBehaviour
{
	public delegate void OnSorted(bool bResetScroll);

	[Header("내림차순/오름차순 토글")]
	public NKCUIComToggle m_ctgDescending;

	[Header("필터링 선택")]
	public GameObject m_objMenuFilter;

	public NKCUIComStateButton m_btnFilterOption;

	public NKCUIComStateButton m_btnFilterSelected;

	[Header("정렬 방식 선택")]
	public GameObject m_objMenuSrot;

	public NKCPopupMiscSort m_NKCPopupSort;

	public NKCUIComToggle m_cbtnSortTypeMenu;

	public GameObject m_objSortSelect;

	public Text m_lbSortType;

	public Text m_lbSelectedSortType;

	[Header("텍스트 인풋")]
	public bool m_bShowSearch;

	public NKCUIComToggle m_ctglSearch;

	public TMP_InputField m_ifFilterTokens;

	public NKCUIComStateButton m_csbtnClearText;

	[Header("도움말")]
	public NKCUIComStateButton m_csbtnGuide;

	public string m_strGuideArticleID;

	[Header("활성화")]
	public GameObject m_objActive;

	private OnSorted dOnSorted;

	private NKCMiscSortSystem.eSortOption defaultSortOption = NKCMiscSortSystem.eSortOption.Rarity_High;

	private NKCMiscSortSystem m_SSCurrent;

	protected HashSet<NKCMiscSortSystem.eSortCategory> m_setSortCategory;

	protected HashSet<NKCMiscSortSystem.eFilterCategory> m_setFilterCategory;

	public void ClearFilterSet()
	{
		if (m_SSCurrent != null)
		{
			m_SSCurrent.FilterSet.Clear();
		}
	}

	public HashSet<NKCMiscSortSystem.eFilterOption> GetFilterOption()
	{
		if (m_SSCurrent != null)
		{
			return m_SSCurrent.FilterSet;
		}
		return null;
	}

	public List<NKCMiscSortSystem.eSortOption> GetSortOption()
	{
		if (m_SSCurrent != null)
		{
			return m_SSCurrent.lstSortOption;
		}
		return null;
	}

	public void Init(OnSorted onSorted)
	{
		dOnSorted = onSorted;
		if (m_ctgDescending != null)
		{
			m_ctgDescending.OnValueChanged.RemoveAllListeners();
			m_ctgDescending.OnValueChanged.AddListener(OnCheckAscend);
		}
		if (m_btnFilterOption != null)
		{
			m_btnFilterOption.PointerClick.RemoveAllListeners();
			m_btnFilterOption.PointerClick.AddListener(OnClickFilterBtn);
		}
		if (m_btnFilterSelected != null)
		{
			m_btnFilterSelected.PointerClick.RemoveAllListeners();
			m_btnFilterSelected.PointerClick.AddListener(OnClickFilterBtn);
		}
		if (null != m_cbtnSortTypeMenu)
		{
			m_cbtnSortTypeMenu.OnValueChanged.RemoveAllListeners();
			m_cbtnSortTypeMenu.OnValueChanged.AddListener(OnSortMenuOpen);
		}
		if (m_ifFilterTokens != null)
		{
			NKCUtil.SetGameobjectActive(m_ifFilterTokens, bValue: false);
			m_ifFilterTokens.onValueChanged.RemoveAllListeners();
			m_ifFilterTokens.onValueChanged.AddListener(OnFilterTokenChanged);
		}
		NKCUtil.SetToggleValueChangedDelegate(m_ctglSearch, OnToggleEnable);
		NKCUtil.SetGameobjectActive(m_ctglSearch, m_bShowSearch);
		NKCUtil.SetBindFunction(m_csbtnClearText, OnClickClear);
		NKCUtil.SetBindFunction(m_csbtnGuide, OnClickInformation);
		SetOpenSortingMenu(bOpen: false, bAnimate: false);
	}

	public void RegisterCategories(HashSet<NKCMiscSortSystem.eFilterCategory> filterCategory, HashSet<NKCMiscSortSystem.eSortCategory> sortCategory)
	{
		m_setFilterCategory = filterCategory;
		m_setSortCategory = sortCategory;
	}

	public void RegisterMiscSort(NKCMiscSortSystem sortSystem)
	{
		m_SSCurrent = sortSystem;
		m_SSCurrent.FilterTokenString = string.Empty;
		if (m_ifFilterTokens != null)
		{
			m_ifFilterTokens.SetTextWithoutNotify(string.Empty);
		}
		SetUIByCurrentSortSystem();
	}

	private void OnCheckAscend(bool bValue)
	{
		if (m_SSCurrent != null && m_SSCurrent.lstSortOption.Count > 0)
		{
			m_SSCurrent.lstSortOption = NKCMiscSortSystem.ChangeAscend(m_SSCurrent.lstSortOption);
			ProcessUIFromCurrentDisplayedSortData(bResetScroll: false, m_SSCurrent.FilterSet.Count > 0);
		}
	}

	private void OnClickFilterBtn()
	{
		if (m_SSCurrent != null)
		{
			NKCPopupFilterMisc.Instance.Open(m_setFilterCategory, m_SSCurrent.FilterSet, OnSelectFilter, m_SSCurrent.FilterStatType_ThemeID, m_SSCurrent.FilterSetTitlecategory, OnSelectFilterTitleCategory);
		}
	}

	private void OnSelectFilter(HashSet<NKCMiscSortSystem.eFilterOption> setFilterOption, int selectedTheme)
	{
		if (m_SSCurrent != null)
		{
			m_SSCurrent.FilterStatType_ThemeID = selectedTheme;
			m_SSCurrent.FilterSet = setFilterOption;
			ProcessUIFromCurrentDisplayedSortData(bResetScroll: true, setFilterOption.Count > 0);
		}
	}

	private void OnSelectFilterTitleCategory(HashSet<int> setFilterOptionTitleCategory)
	{
		if (m_SSCurrent != null)
		{
			m_SSCurrent.FilterSetTitlecategory = setFilterOptionTitleCategory;
			ProcessUIFromCurrentDisplayedSortData(bResetScroll: true, setFilterOptionTitleCategory.Count > 0);
		}
	}

	private void OnSortMenuOpen(bool bValue)
	{
		if (m_SSCurrent != null)
		{
			NKCMiscSortSystem.eSortOption selectedSortOption = ((m_SSCurrent.lstSortOption.Count > 0) ? m_SSCurrent.lstSortOption[0] : defaultSortOption);
			new List<string>();
			m_NKCPopupSort.OpenSortMenu(m_setSortCategory, selectedSortOption, OnSort, bOpen: true);
			NKCUtil.SetGameobjectActive(m_objSortSelect, bValue: false);
			SetOpenSortingMenu(bValue);
		}
	}

	private void SetOpenSortingMenu(bool bOpen, bool bAnimate = true)
	{
		m_cbtnSortTypeMenu.Select(bOpen, bForce: true);
		m_NKCPopupSort.StartRectMove(bOpen, bAnimate);
	}

	private void OnSort(List<NKCMiscSortSystem.eSortOption> sortOptionList)
	{
		if (m_SSCurrent != null)
		{
			m_SSCurrent.lstSortOption = sortOptionList;
			ProcessUIFromCurrentDisplayedSortData(bResetScroll: false, m_SSCurrent.FilterSet.Count > 0);
			NKCUtil.SetGameobjectActive(m_objSortSelect, bValue: true);
			SetOpenSortingMenu(bOpen: false);
		}
	}

	private void OnFilterTokenChanged(string str)
	{
		if (m_SSCurrent != null)
		{
			m_SSCurrent.FilterTokenString = str;
			ProcessUIFromCurrentDisplayedSortData(bResetScroll: true, m_SSCurrent.FilterSet.Count > 0);
			NKCUtil.SetGameobjectActive(m_objActive, !string.IsNullOrEmpty(str));
		}
	}

	private void ProcessUIFromCurrentDisplayedSortData(bool bResetScroll, bool bSelectedAnyFilter)
	{
		NKCUtil.SetGameobjectActive(m_btnFilterSelected, bSelectedAnyFilter);
		SetUIByCurrentSortSystem();
		dOnSorted?.Invoke(bResetScroll);
	}

	private string GetSortName(NKCMiscSortSystem.eSortOption sortOption)
	{
		if (m_SSCurrent != null)
		{
			return NKCMiscSortSystem.GetSortName(m_SSCurrent.lstSortOption[0]);
		}
		return string.Empty;
	}

	private void SetUIByCurrentSortSystem()
	{
		m_ctgDescending?.Select(NKCMiscSortSystem.IsDescending(m_SSCurrent.lstSortOption[0]), bForce: true);
		if (m_SSCurrent != null && m_SSCurrent.lstSortOption.Count > 0)
		{
			string sortName = GetSortName(m_SSCurrent.lstSortOption[0]);
			NKCUtil.SetLabelText(m_lbSortType, sortName);
			NKCUtil.SetLabelText(m_lbSelectedSortType, sortName);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbSortType, NKCUtilString.GET_STRING_SORT_RARITY);
			NKCUtil.SetLabelText(m_lbSelectedSortType, NKCUtilString.GET_STRING_SORT_RARITY);
		}
	}

	public void ResetUI()
	{
		if (m_SSCurrent != null)
		{
			NKCUtil.SetGameobjectActive(m_btnFilterSelected, m_SSCurrent.FilterSet != null && m_SSCurrent.FilterSet.Count > 0);
			NKCUtil.SetGameobjectActive(m_objSortSelect, bValue: false);
			SetOpenSortingMenu(bOpen: false, bAnimate: false);
			m_ctglSearch.Select(bSelect: false, bForce: true);
			NKCUtil.SetGameobjectActive(m_objActive, bValue: false);
			m_ctgDescending?.Select(NKCMiscSortSystem.IsDescending(m_SSCurrent.lstSortOption[0]), bForce: true);
			NKCUtil.SetGameobjectActive(m_ifFilterTokens, !m_SSCurrent.Options.bHideTokenFiltering);
			NKCUtil.SetGameobjectActive(m_ctgDescending.gameObject, !m_SSCurrent.Options.bHideDescendingOption);
			NKCUtil.SetGameobjectActive(m_objMenuFilter, !m_SSCurrent.Options.bHideFilterOption);
			NKCUtil.SetGameobjectActive(m_objMenuSrot, !m_SSCurrent.Options.bHideSortOption);
			if (m_SSCurrent.lstSortOption != null && m_SSCurrent.lstSortOption.Count > 0)
			{
				string sortName = GetSortName(m_SSCurrent.lstSortOption[0]);
				NKCUtil.SetLabelText(m_lbSortType, sortName);
				NKCUtil.SetLabelText(m_lbSelectedSortType, sortName);
			}
			else
			{
				NKCUtil.SetLabelText(m_lbSortType, NKCUtilString.GET_STRING_SORT_RARITY);
				NKCUtil.SetLabelText(m_lbSelectedSortType, NKCUtilString.GET_STRING_SORT_RARITY);
			}
		}
	}

	public void AddSortOptionDetail(NKCMiscSortSystem.eSortOption sortOption, List<NKCMiscSortSystem.eSortOption> lstDetail)
	{
	}

	private void OnToggleEnable(bool enable)
	{
		if (enable)
		{
			m_ifFilterTokens.Select();
		}
		else
		{
			m_ifFilterTokens.ReleaseSelection();
		}
	}

	private void OnClickClear()
	{
		m_ifFilterTokens.text = "";
	}

	private void OnClickInformation()
	{
		if (!string.IsNullOrEmpty(m_strGuideArticleID))
		{
			NKCUIPopUpGuide.Instance.Open(m_strGuideArticleID);
		}
	}
}
