using System.Collections.Generic;
using NKC.UI.Guide;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIComUnitSortOptions : MonoBehaviour
{
	public delegate void OnSorted(bool bResetScroll);

	[Header("내림차순/오름차순 토글")]
	public NKCUIComToggle m_ctgDescending;

	[Header("필터링 선택")]
	public NKCUIComStateButton m_btnFilterOption;

	public NKCUIComStateButton m_btnFilterSelected;

	public bool m_bShowTrophyFilter;

	[Header("정렬 방식 선택")]
	public NKCPopupSort m_NKCPopupSort;

	public NKCUIComToggle m_cbtnSortTypeMenu;

	public GameObject m_objSortSelect;

	public Text m_lbSortType;

	public Text m_lbSelectedSortType;

	[Header("즐겨찾기")]
	public NKCUIComToggle m_tglFavorite;

	private bool m_bUseFavorite;

	private bool m_bFavoriteFilterActive;

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

	protected HashSet<NKCUnitSortSystem.eSortCategory> m_setSortCategory;

	protected HashSet<NKCUnitSortSystem.eFilterCategory> m_setFilterCategory;

	protected HashSet<NKCOperatorSortSystem.eSortCategory> m_setOprSortCategory;

	protected HashSet<NKCOperatorSortSystem.eFilterCategory> m_setOprFilterCategory;

	protected HashSet<NKCOperatorTokenSortSystem.eSortCategory> m_setOprTokenSortCategory;

	protected HashSet<NKCOperatorTokenSortSystem.eFilterCategory> m_setOprTokenFilterCategory;

	protected NKCUnitSortSystem.eSortOption defaultSortOption = NKCUnitSortSystem.eSortOption.Rarity_High;

	protected NKCOperatorSortSystem.eSortOption defaultOperatorSortOption = NKCOperatorSortSystem.eSortOption.Rarity_High;

	private NKCUnitSortSystem m_SSCurrent;

	private NKCOperatorSortSystem m_OperatorSSCurrent;

	private NKCOperatorTokenSortSystem m_OperatorTokenSSCurrent;

	private bool m_bIsCollection;

	public void ClearFilterSet()
	{
		if (m_SSCurrent != null)
		{
			m_SSCurrent.FilterSet.Clear();
		}
		if (m_OperatorSSCurrent != null)
		{
			m_OperatorSSCurrent.FilterSet.Clear();
		}
	}

	public HashSet<NKCUnitSortSystem.eFilterOption> GetUnitFilterOption()
	{
		if (m_SSCurrent != null)
		{
			return m_SSCurrent.FilterSet;
		}
		return null;
	}

	public HashSet<NKCOperatorSortSystem.eFilterOption> GetOperatorFilterOption()
	{
		if (m_OperatorSSCurrent != null)
		{
			return m_OperatorSSCurrent.FilterSet;
		}
		return null;
	}

	public List<NKCUnitSortSystem.eSortOption> GetUnitSortOption()
	{
		if (m_SSCurrent != null)
		{
			return m_SSCurrent.lstSortOption;
		}
		return null;
	}

	public List<NKCOperatorSortSystem.eSortOption> GetOperatorSortOption()
	{
		if (m_OperatorSSCurrent != null)
		{
			return m_OperatorSSCurrent.lstSortOption;
		}
		return null;
	}

	public void Init(OnSorted onSorted, bool bIsCollection)
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
		NKCUtil.SetGameobjectActive(m_ctglSearch, m_bShowSearch);
		NKCUtil.SetToggleValueChangedDelegate(m_ctglSearch, OnToggleEnable);
		NKCUtil.SetBindFunction(m_csbtnClearText, OnClickClear);
		NKCUtil.SetBindFunction(m_csbtnGuide, OnClickInformation);
		NKCUtil.SetButtonClickDelegate(m_tglFavorite, OnTglFavorite);
		if (m_tglFavorite != null)
		{
			m_tglFavorite.Select(bSelect: false, bForce: true);
		}
		m_bUseFavorite = false;
		m_bFavoriteFilterActive = false;
		m_bIsCollection = bIsCollection;
		SetOpenSortingMenu(bOpen: false, bAnimate: false);
	}

	public void RegisterCategories(HashSet<NKCUnitSortSystem.eFilterCategory> filterCategory, HashSet<NKCUnitSortSystem.eSortCategory> sortCategory, bool bFavoriteFilterActive)
	{
		m_setFilterCategory = filterCategory;
		m_setSortCategory = sortCategory;
		m_setOprFilterCategory = new HashSet<NKCOperatorSortSystem.eFilterCategory>();
		m_setOprSortCategory = new HashSet<NKCOperatorSortSystem.eSortCategory>();
		m_bFavoriteFilterActive = bFavoriteFilterActive;
	}

	public void RegisterCategories(HashSet<NKCOperatorSortSystem.eFilterCategory> filterCategory, HashSet<NKCOperatorSortSystem.eSortCategory> sortCategory, bool bFavoriteFilterActive)
	{
		m_setFilterCategory = new HashSet<NKCUnitSortSystem.eFilterCategory>();
		m_setSortCategory = new HashSet<NKCUnitSortSystem.eSortCategory>();
		m_setOprFilterCategory = filterCategory;
		m_setOprSortCategory = sortCategory;
		m_bFavoriteFilterActive = bFavoriteFilterActive;
	}

	public void RegisterCategories(HashSet<NKCOperatorTokenSortSystem.eFilterCategory> filterCategory, HashSet<NKCOperatorTokenSortSystem.eSortCategory> sortCategory, bool bFavoriteFilterActive)
	{
		m_setFilterCategory = new HashSet<NKCUnitSortSystem.eFilterCategory>();
		m_setSortCategory = new HashSet<NKCUnitSortSystem.eSortCategory>();
		m_setOprFilterCategory = new HashSet<NKCOperatorSortSystem.eFilterCategory>();
		m_setOprSortCategory = new HashSet<NKCOperatorSortSystem.eSortCategory>();
		m_setOprTokenFilterCategory = filterCategory;
		m_setOprTokenSortCategory = sortCategory;
		m_bFavoriteFilterActive = bFavoriteFilterActive;
	}

	public void RegisterUnitSort(NKCUnitSortSystem sortSystem)
	{
		m_SSCurrent = sortSystem;
		m_SSCurrent.FilterTokenString = string.Empty;
		if (m_ifFilterTokens != null)
		{
			m_ifFilterTokens.SetTextWithoutNotify(string.Empty);
		}
		m_OperatorSSCurrent = null;
		m_OperatorTokenSSCurrent = null;
		SetUIByCurrentSortSystem();
	}

	public void RegisterOperatorSort(NKCOperatorSortSystem operatorSortSystem)
	{
		m_OperatorSSCurrent = operatorSortSystem;
		if (m_ifFilterTokens != null)
		{
			m_ifFilterTokens.SetTextWithoutNotify(string.Empty);
		}
		m_OperatorTokenSSCurrent = null;
		m_SSCurrent = null;
		SetUIByCurrentOperatorSortSystem();
	}

	public void RegisterOperatorTokenSort(NKCOperatorTokenSortSystem operatorTokenSortSystem)
	{
		m_OperatorTokenSSCurrent = operatorTokenSortSystem;
		m_OperatorTokenSSCurrent.FilterTokenString = string.Empty;
		if (m_ifFilterTokens != null)
		{
			m_ifFilterTokens.SetTextWithoutNotify(string.Empty);
		}
		m_OperatorSSCurrent = null;
		m_SSCurrent = null;
		SetUIByCurrentOperatorTokenSortSystem();
	}

	private void OnCheckAscend(bool bValue)
	{
		if (m_SSCurrent != null && m_SSCurrent.lstSortOption.Count > 0)
		{
			m_SSCurrent.Descending = bValue;
			m_SSCurrent.lstSortOption = NKCUnitSortSystem.ChangeAscend(m_SSCurrent.lstSortOption);
			ProcessUIFromCurrentDisplayedSortData(bResetScroll: false, CheckAnyFilterSelected(m_SSCurrent.FilterSet));
		}
		else if (m_OperatorSSCurrent != null && m_OperatorSSCurrent.lstSortOption.Count > 0)
		{
			m_OperatorSSCurrent.Descending = bValue;
			m_OperatorSSCurrent.lstSortOption = NKCOperatorSortSystem.ChangeAscend(m_OperatorSSCurrent.lstSortOption);
			ProcessUIFromCurrentDisplayedSortData(bResetScroll: false, m_OperatorSSCurrent.FilterSet.Count > 0);
		}
		else if (m_OperatorTokenSSCurrent != null && m_OperatorTokenSSCurrent.lstSortOption.Count > 0)
		{
			m_OperatorTokenSSCurrent.Descending = bValue;
			m_OperatorTokenSSCurrent.lstSortOption = NKCOperatorTokenSortSystem.ChangeAscend(m_OperatorTokenSSCurrent.lstSortOption);
			ProcessUIFromCurrentDisplayedSortData(bResetScroll: false, m_OperatorTokenSSCurrent.FilterSet.Count > 0);
		}
	}

	private void OnClickFilterBtn()
	{
		if (m_SSCurrent != null)
		{
			NKCPopupFilterUnit.Instance.Open(m_setFilterCategory, m_SSCurrent.FilterSet, OnSelectFilter, NKCPopupFilterUnit.FILTER_TYPE.UNIT, m_bShowTrophyFilter);
		}
		else if (m_OperatorSSCurrent != null)
		{
			NKCPopupFilterOperator.Instance.Open(m_OperatorSSCurrent, m_setOprFilterCategory, OnSelectFilter);
		}
		else if (m_OperatorTokenSSCurrent != null)
		{
			NKCPopupFilterOperatorToken.Instance.Open(m_OperatorTokenSSCurrent, m_setOprTokenFilterCategory, OnSelectFilter);
		}
	}

	private void OnSelectFilter(HashSet<NKCUnitSortSystem.eFilterOption> setFilterOption)
	{
		if (m_SSCurrent != null)
		{
			if (m_bFavoriteFilterActive)
			{
				m_SSCurrent.FilterSet.Add(NKCUnitSortSystem.eFilterOption.Favorite);
			}
			m_SSCurrent.FilterSet = setFilterOption;
			ProcessUIFromCurrentDisplayedSortData(bResetScroll: true, CheckAnyFilterSelected(setFilterOption));
		}
	}

	private void OnSelectFilter(NKCOperatorSortSystem ssActive)
	{
		if (m_OperatorSSCurrent != null)
		{
			m_OperatorSSCurrent = ssActive;
			ProcessUIFromCurrentDisplayedSortData(bResetScroll: true, ssActive.FilterSet.Count > 0);
		}
	}

	private void OnSelectFilter(NKCOperatorTokenSortSystem ssActive)
	{
		if (m_OperatorTokenSSCurrent != null)
		{
			m_OperatorTokenSSCurrent = ssActive;
			ProcessUIFromCurrentDisplayedSortData(bResetScroll: true, ssActive.FilterSet.Count > 0);
		}
	}

	private void OnTglFavorite(bool value)
	{
		m_bFavoriteFilterActive = value && m_bUseFavorite;
		if (m_bFavoriteFilterActive)
		{
			m_SSCurrent.FilterSet.Add(NKCUnitSortSystem.eFilterOption.Favorite);
		}
		else
		{
			m_SSCurrent.FilterSet.Remove(NKCUnitSortSystem.eFilterOption.Favorite);
		}
		m_SSCurrent.FilterList(m_SSCurrent.FilterSet, m_SSCurrent.bHideDeckedUnit);
		bool bSelectedAnyFilter = (m_bFavoriteFilterActive ? (m_SSCurrent.FilterSet.Count > 1) : (m_SSCurrent.FilterSet.Count > 0));
		ProcessUIFromCurrentDisplayedSortData(bResetScroll: true, bSelectedAnyFilter);
	}

	private bool CheckAnyFilterSelected(HashSet<NKCUnitSortSystem.eFilterOption> setFilter)
	{
		if (setFilter == null)
		{
			return false;
		}
		if (setFilter.Count == 1 && setFilter.Contains(NKCUnitSortSystem.eFilterOption.Collection_HasAchieve))
		{
			return false;
		}
		if (!m_bFavoriteFilterActive)
		{
			return setFilter.Count > 0;
		}
		return setFilter.Count > 1;
	}

	private void OnFilterTokenChanged(string str)
	{
		if (m_SSCurrent != null)
		{
			m_SSCurrent.FilterTokenString = str;
			ProcessUIFromCurrentDisplayedSortData(bResetScroll: true, CheckAnyFilterSelected(m_SSCurrent.FilterSet));
			NKCUtil.SetGameobjectActive(m_objActive, !string.IsNullOrEmpty(str));
		}
		else if (m_OperatorSSCurrent != null)
		{
			m_OperatorSSCurrent.FilterTokenString = str;
			ProcessUIFromCurrentDisplayedSortData(bResetScroll: true, m_OperatorSSCurrent.FilterSet.Count > 0);
			NKCUtil.SetGameobjectActive(m_objActive, !string.IsNullOrEmpty(str));
		}
		else if (m_OperatorTokenSSCurrent != null)
		{
			m_OperatorTokenSSCurrent.FilterTokenString = str;
			ProcessUIFromCurrentDisplayedSortData(bResetScroll: true, m_OperatorTokenSSCurrent.FilterSet.Count > 0);
			NKCUtil.SetGameobjectActive(m_objActive, !string.IsNullOrEmpty(str));
		}
	}

	private void OnSortMenuOpen(bool bValue)
	{
		if (m_SSCurrent != null)
		{
			NKCUnitSortSystem.eSortOption selectedSortOption = ((m_SSCurrent.lstSortOption.Count > 0) ? m_SSCurrent.lstSortOption[0] : defaultSortOption);
			List<string> list = new List<string>();
			if (m_SSCurrent.Options.lstCustomSortFunc != null)
			{
				foreach (KeyValuePair<NKCUnitSortSystem.eSortCategory, KeyValuePair<string, NKCUnitSortSystem.NKCDataComparerer<NKM.NKMUnitData>.CompareFunc>> item in m_SSCurrent.Options.lstCustomSortFunc)
				{
					list.Add(item.Value.Key);
				}
			}
			m_NKCPopupSort.OpenSortMenu(m_setSortCategory, selectedSortOption, OnSort, bOpen: true, NKM_UNIT_TYPE.NUT_NORMAL, m_bIsCollection, list);
		}
		else
		{
			if (m_OperatorSSCurrent == null)
			{
				return;
			}
			NKCOperatorSortSystem.eSortOption option = ((m_OperatorSSCurrent.lstSortOption.Count > 0) ? m_OperatorSSCurrent.lstSortOption[0] : defaultOperatorSortOption);
			List<string> list2 = new List<string>();
			if (m_OperatorSSCurrent.Options.lstCustomSortFunc != null)
			{
				foreach (KeyValuePair<NKCOperatorSortSystem.eSortCategory, KeyValuePair<string, NKCUnitSortSystem.NKCDataComparerer<NKM.NKMOperator>.CompareFunc>> item2 in m_OperatorSSCurrent.Options.lstCustomSortFunc)
				{
					list2.Add(item2.Value.Key);
				}
			}
			m_NKCPopupSort.OpenSortMenu(NKCOperatorSortSystem.ConvertSortCategory(m_setOprSortCategory), NKCOperatorSortSystem.ConvertSortOption(option), OnSort, bOpen: true, NKM_UNIT_TYPE.NUT_OPERATOR, m_bIsCollection, list2);
		}
		NKCUtil.SetGameobjectActive(m_objSortSelect, bValue: false);
		SetOpenSortingMenu(bValue);
	}

	private void SetOpenSortingMenu(bool bOpen, bool bAnimate = true)
	{
		m_cbtnSortTypeMenu.Select(bOpen, bForce: true);
		m_NKCPopupSort.StartRectMove(bOpen, bAnimate);
	}

	private void OnSort(List<NKCUnitSortSystem.eSortOption> sortOptionList)
	{
		if (m_SSCurrent != null)
		{
			m_SSCurrent.lstSortOption = sortOptionList;
			ProcessUIFromCurrentDisplayedSortData(bResetScroll: false, CheckAnyFilterSelected(m_SSCurrent.FilterSet));
		}
		else
		{
			if (m_OperatorSSCurrent == null)
			{
				return;
			}
			m_OperatorSSCurrent.lstSortOption = NKCOperatorSortSystem.ConvertSortOption(sortOptionList);
			ProcessUIFromCurrentDisplayedSortData(bResetScroll: false, m_OperatorSSCurrent.FilterSet.Count > 0);
		}
		NKCUtil.SetGameobjectActive(m_objSortSelect, bValue: true);
		SetOpenSortingMenu(bOpen: false);
	}

	private void ProcessUIFromCurrentDisplayedSortData(bool bResetScroll, bool bSelectedAnyFilter)
	{
		NKCUtil.SetGameobjectActive(m_btnFilterSelected, bSelectedAnyFilter);
		if (m_SSCurrent != null)
		{
			SetUIByCurrentSortSystem();
		}
		else if (m_OperatorSSCurrent != null)
		{
			SetUIByCurrentOperatorSortSystem();
		}
		else if (m_OperatorTokenSSCurrent != null)
		{
			SetUIByCurrentOperatorTokenSortSystem();
		}
		dOnSorted?.Invoke(bResetScroll);
	}

	private string GetSortName(NKCUnitSortSystem.eSortOption sortOption)
	{
		if (m_SSCurrent != null)
		{
			string text = NKCUnitSortSystem.GetSortName(m_SSCurrent.lstSortOption[0]);
			if (string.IsNullOrEmpty(text))
			{
				NKCUnitSortSystem.eSortCategory sortCategoryFromOption = NKCUnitSortSystem.GetSortCategoryFromOption(sortOption);
				if (m_SSCurrent.Options.lstCustomSortFunc.ContainsKey(sortCategoryFromOption))
				{
					text = m_SSCurrent.Options.lstCustomSortFunc[sortCategoryFromOption].Key;
				}
			}
			return text;
		}
		if (m_OperatorSSCurrent != null)
		{
			string text2 = NKCOperatorSortSystem.GetSortName(m_OperatorSSCurrent.lstSortOption[0]);
			if (string.IsNullOrEmpty(text2))
			{
				NKCOperatorSortSystem.eSortCategory key = NKCOperatorSortSystem.ConvertSortCategory(NKCUnitSortSystem.GetSortCategoryFromOption(sortOption));
				if (m_OperatorSSCurrent.Options.lstCustomSortFunc.ContainsKey(key))
				{
					text2 = m_OperatorSSCurrent.Options.lstCustomSortFunc[key].Key;
				}
			}
			return text2;
		}
		return string.Empty;
	}

	private string GetSortName(NKCOperatorSortSystem.eSortOption sortOption)
	{
		if (m_OperatorSSCurrent != null)
		{
			string text = NKCOperatorSortSystem.GetSortName(m_OperatorSSCurrent.lstSortOption[0]);
			if (string.IsNullOrEmpty(text))
			{
				NKCOperatorSortSystem.eSortCategory sortCategoryFromOption = NKCOperatorSortSystem.GetSortCategoryFromOption(sortOption);
				if (m_OperatorSSCurrent.Options.lstCustomSortFunc.ContainsKey(sortCategoryFromOption))
				{
					text = m_OperatorSSCurrent.Options.lstCustomSortFunc[sortCategoryFromOption].Key;
				}
			}
			return text;
		}
		return string.Empty;
	}

	private string GetSortName(NKCOperatorTokenSortSystem.eSortOption sortOption)
	{
		if (m_OperatorTokenSSCurrent != null)
		{
			string sortName = NKCOperatorTokenSortSystem.GetSortName(m_OperatorTokenSSCurrent.lstSortOption[0]);
			if (string.IsNullOrEmpty(sortName))
			{
				NKCOperatorTokenSortSystem.GetSortCategoryFromOption(sortOption);
			}
			return sortName;
		}
		return string.Empty;
	}

	private void SetUIByCurrentSortSystem()
	{
		if (m_ctgDescending != null)
		{
			m_ctgDescending.Select((m_SSCurrent != null) ? m_SSCurrent.Descending : m_OperatorSSCurrent.Descending, bForce: true);
		}
		if (((m_SSCurrent != null) ? m_SSCurrent.lstSortOption.Count : m_OperatorSSCurrent.lstSortOption.Count) == 0)
		{
			NKCUtil.SetLabelText(m_lbSortType, NKCUtilString.GET_STRING_I_D);
			NKCUtil.SetLabelText(m_lbSelectedSortType, NKCUtilString.GET_STRING_I_D);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbSortType, GetSortName(m_SSCurrent.lstSortOption[0]));
			NKCUtil.SetLabelText(m_lbSelectedSortType, GetSortName(m_SSCurrent.lstSortOption[0]));
		}
		SetFilterByCurrentOption();
	}

	private void SetFilterByCurrentOption()
	{
		if (m_tglFavorite != null)
		{
			if (m_SSCurrent != null && m_SSCurrent.FilterSet != null)
			{
				m_tglFavorite.Select(m_SSCurrent.FilterSet.Contains(NKCUnitSortSystem.eFilterOption.Favorite), bForce: true);
			}
			else
			{
				m_tglFavorite.Select(bSelect: false, bForce: true);
			}
		}
	}

	private void SetUIByCurrentOperatorSortSystem()
	{
		if (m_ctgDescending != null)
		{
			m_ctgDescending.Select(m_OperatorSSCurrent.Descending, bForce: true);
		}
		if (m_OperatorSSCurrent.lstSortOption.Count == 0)
		{
			NKCUtil.SetLabelText(m_lbSortType, NKCUtilString.GET_STRING_I_D);
			NKCUtil.SetLabelText(m_lbSelectedSortType, NKCUtilString.GET_STRING_I_D);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbSortType, GetSortName(m_OperatorSSCurrent.lstSortOption[0]));
			NKCUtil.SetLabelText(m_lbSelectedSortType, GetSortName(m_OperatorSSCurrent.lstSortOption[0]));
		}
	}

	private void SetUIByCurrentOperatorTokenSortSystem()
	{
		if (m_ctgDescending != null)
		{
			m_ctgDescending.Select(m_OperatorTokenSSCurrent.Descending, bForce: true);
		}
		if (m_OperatorTokenSSCurrent.lstSortOption.Count == 0)
		{
			NKCUtil.SetLabelText(m_lbSortType, NKCUtilString.GET_STRING_I_D);
			NKCUtil.SetLabelText(m_lbSelectedSortType, NKCUtilString.GET_STRING_I_D);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbSortType, GetSortName(m_OperatorTokenSSCurrent.lstSortOption[0]));
			NKCUtil.SetLabelText(m_lbSelectedSortType, GetSortName(m_OperatorTokenSSCurrent.lstSortOption[0]));
		}
	}

	public void ResetUI(bool bUseFavorite = false, bool bClearFilterSet = false)
	{
		if (m_SSCurrent != null)
		{
			m_bUseFavorite = bUseFavorite;
			NKCUtil.SetGameobjectActive(m_tglFavorite, m_bUseFavorite);
			SetFilterByCurrentOption();
			NKCUtil.SetGameobjectActive(m_btnFilterSelected, m_SSCurrent.FilterSet != null && m_SSCurrent.FilterSet.Count > 0);
			NKCUtil.SetGameobjectActive(m_objSortSelect, bValue: false);
			SetOpenSortingMenu(bOpen: false, bAnimate: false);
			if (m_ctgDescending != null)
			{
				m_ctgDescending.Select(m_SSCurrent.Descending, bForce: true);
			}
			NKCUtil.SetGameobjectActive(m_ifFilterTokens, !m_SSCurrent.Options.bHideTokenFiltering);
			if (m_SSCurrent.lstSortOption == null || m_SSCurrent.lstSortOption.Count == 0)
			{
				NKCUtil.SetLabelText(m_lbSortType, NKCUtilString.GET_STRING_I_D);
				NKCUtil.SetLabelText(m_lbSelectedSortType, NKCUtilString.GET_STRING_I_D);
			}
			else
			{
				string sortName = GetSortName(m_SSCurrent.lstSortOption[0]);
				NKCUtil.SetLabelText(m_lbSortType, sortName);
				NKCUtil.SetLabelText(m_lbSelectedSortType, sortName);
			}
		}
		else if (m_OperatorSSCurrent != null)
		{
			m_bUseFavorite = false;
			NKCUtil.SetGameobjectActive(m_tglFavorite, bValue: false);
			if (m_OperatorSSCurrent != null)
			{
				if (bClearFilterSet && m_OperatorSSCurrent.FilterSet != null)
				{
					m_OperatorSSCurrent.FilterSet.Clear();
				}
				NKCUtil.SetGameobjectActive(m_btnFilterSelected, m_OperatorSSCurrent.FilterSet != null && m_OperatorSSCurrent.FilterSet.Count > 0);
				NKCUtil.SetGameobjectActive(m_objSortSelect, bValue: false);
				SetOpenSortingMenu(bOpen: false, bAnimate: false);
				if (m_ctgDescending != null)
				{
					m_ctgDescending.Select(m_OperatorSSCurrent.Descending, bForce: true);
				}
				NKCUtil.SetGameobjectActive(m_ifFilterTokens, bValue: true);
				if (m_OperatorSSCurrent.lstSortOption == null || m_OperatorSSCurrent.lstSortOption.Count == 0)
				{
					NKCUtil.SetLabelText(m_lbSortType, NKCUtilString.GET_STRING_I_D);
					NKCUtil.SetLabelText(m_lbSelectedSortType, NKCUtilString.GET_STRING_I_D);
				}
				else
				{
					string sortName2 = GetSortName(m_OperatorSSCurrent.lstSortOption[0]);
					NKCUtil.SetLabelText(m_lbSortType, sortName2);
					NKCUtil.SetLabelText(m_lbSelectedSortType, sortName2);
				}
			}
		}
		else if (m_OperatorTokenSSCurrent != null)
		{
			m_bUseFavorite = false;
			NKCUtil.SetGameobjectActive(m_tglFavorite, bValue: false);
			if (m_OperatorTokenSSCurrent != null)
			{
				if (bClearFilterSet && m_OperatorTokenSSCurrent.FilterSet != null)
				{
					m_OperatorTokenSSCurrent.FilterSet.Clear();
				}
				NKCUtil.SetGameobjectActive(m_btnFilterSelected, m_OperatorTokenSSCurrent.FilterSet != null && m_OperatorTokenSSCurrent.FilterSet.Count > 0);
				NKCUtil.SetGameobjectActive(m_objSortSelect, bValue: false);
				SetOpenSortingMenu(bOpen: false, bAnimate: false);
				if (m_ctgDescending != null)
				{
					m_ctgDescending.Select(m_OperatorTokenSSCurrent.Descending, bForce: true);
				}
				NKCUtil.SetGameobjectActive(m_ifFilterTokens, bValue: true);
				if (m_OperatorTokenSSCurrent.lstSortOption == null || m_OperatorTokenSSCurrent.lstSortOption.Count == 0)
				{
					NKCUtil.SetLabelText(m_lbSortType, NKCUtilString.GET_STRING_I_D);
					NKCUtil.SetLabelText(m_lbSelectedSortType, NKCUtilString.GET_STRING_I_D);
				}
				else
				{
					string sortName3 = GetSortName(m_OperatorTokenSSCurrent.lstSortOption[0]);
					NKCUtil.SetLabelText(m_lbSortType, sortName3);
					NKCUtil.SetLabelText(m_lbSelectedSortType, sortName3);
				}
			}
		}
		m_ctglSearch?.Select(bSelect: false, bForce: true);
		NKCUtil.SetGameobjectActive(m_objActive, bValue: false);
	}

	public void SetEnableFilter(bool bActive)
	{
		NKCUtil.SetGameobjectActive(m_btnFilterOption, bActive);
	}

	public void AddSortOptionDetail(NKCUnitSortSystem.eSortOption sortOption, List<NKCUnitSortSystem.eSortOption> lstDetail)
	{
		m_NKCPopupSort?.AddSortOptionDetail(sortOption, lstDetail);
	}

	public bool IsLimitBreakState()
	{
		if (m_SSCurrent != null && m_SSCurrent.lstSortOption != null && m_SSCurrent.lstSortOption.Count > 0)
		{
			if (m_SSCurrent.lstSortOption[0] != NKCUnitSortSystem.eSortOption.LimitBreak_High && m_SSCurrent.lstSortOption[0] != NKCUnitSortSystem.eSortOption.LimitBreak_Low && m_SSCurrent.lstSortOption[0] != NKCUnitSortSystem.eSortOption.Transcendence_High)
			{
				return m_SSCurrent.lstSortOption[0] == NKCUnitSortSystem.eSortOption.Transcendence_Low;
			}
			return true;
		}
		return false;
	}

	public bool IsTacticUpdateState()
	{
		if (m_SSCurrent != null && m_SSCurrent.FilterSet != null && m_SSCurrent.FilterSet.Contains(NKCUnitSortSystem.eFilterOption.TacticUpdate_Possible))
		{
			return true;
		}
		return false;
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
