using System.Collections.Generic;
using NKC.UI.Guide;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIComEquipSortOptions : MonoBehaviour
{
	public delegate void OnSorted(bool bResetScroll);

	public delegate void OnSortOptionChange(bool bUpdate);

	[Header("내림차순/오름차순 토글")]
	public NKCUIComToggle m_ctgDescending;

	[Header("필터링 선택")]
	public NKCUIComStateButton m_btnFilterOption;

	public NKCUIComStateButton m_btnFilterSelected;

	[Header("정렬 방식 선택")]
	public NKCPopupEquipSort m_NKCPopupEquipSort;

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

	private NKCEquipSortSystem.eSortOption defaultSortOption = NKCEquipSortSystem.eSortOption.Rarity_High;

	private NKCEquipSortSystem m_SSCurrent;

	private HashSet<NKCEquipSortSystem.eSortCategory> m_setSortCategory;

	private HashSet<NKCEquipSortSystem.eFilterCategory> m_setFilterCategory;

	private OnSortOptionChange dOnSortOptionChange;

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

	public void RegisterCategories(HashSet<NKCEquipSortSystem.eFilterCategory> filterCategory, HashSet<NKCEquipSortSystem.eSortCategory> sortCategory)
	{
		m_setFilterCategory = filterCategory;
		m_setSortCategory = sortCategory;
	}

	public void RegisterSortOptionUpdate(OnSortOptionChange SortOptionChange)
	{
		dOnSortOptionChange = SortOptionChange;
	}

	public void RegisterEquipSort(NKCEquipSortSystem sortSystem)
	{
		m_SSCurrent = sortSystem;
		if (m_ifFilterTokens != null)
		{
			m_ifFilterTokens.SetTextWithoutNotify(string.Empty);
		}
		SetUIByCurrentSortSystem();
	}

	private void OnCheckAscend(bool bValue)
	{
		if (m_SSCurrent != null && m_SSCurrent.lstSortOption.Count != 0)
		{
			m_SSCurrent.lstSortOption = m_NKCPopupEquipSort.ChangeAscend(m_SSCurrent.lstSortOption);
			ProcessUIFromCurrentDisplayedSortData(bResetScroll: false, m_SSCurrent.FilterSet.Count > 0);
		}
	}

	private void OnClickFilterBtn()
	{
		if (m_SSCurrent != null)
		{
			NKCPopupFilterEquip.Instance.Open(m_setFilterCategory, m_SSCurrent, OnSelectFilter, m_SSCurrent.ExcludeFilterSet != null && !m_SSCurrent.ExcludeFilterSet.Contains(NKCEquipSortSystem.eFilterOption.Equip_Enchant));
		}
	}

	private void OnSelectFilter(NKCEquipSortSystem ssActive)
	{
		if (m_SSCurrent != null && ssActive != null)
		{
			m_SSCurrent.FilterSet = ssActive.FilterSet;
			ProcessUIFromCurrentDisplayedSortData(bResetScroll: true, ssActive.FilterSet.Count > 0);
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

	private void OnSortMenuOpen(bool bValue)
	{
		if (m_SSCurrent != null)
		{
			NKCUtil.SetGameobjectActive(m_objSortSelect, m_SSCurrent.lstSortOption[0] != NKCEquipSortSystem.GetDefaultSortOption()[0]);
			NKCEquipSortSystem.eSortOption selectedSortOption = ((m_SSCurrent.lstSortOption.Count > 0) ? m_SSCurrent.lstSortOption[0] : defaultSortOption);
			m_NKCPopupEquipSort.OpenEquipSortMenu(m_setSortCategory, selectedSortOption, OnSort, m_ctgDescending.m_bSelect, bValue: true);
			NKCUtil.SetGameobjectActive(m_objSortSelect, bValue: false);
			SetOpenSortingMenu(bValue);
		}
	}

	private void SetOpenSortingMenu(bool bOpen, bool bAnimate = true)
	{
		m_cbtnSortTypeMenu.Select(bOpen, bForce: true);
		m_NKCPopupEquipSort.StartRectMove(bOpen, bAnimate);
	}

	private void OnSort(List<NKCEquipSortSystem.eSortOption> sortOptionList)
	{
		if (m_SSCurrent != null)
		{
			m_SSCurrent.lstSortOption = sortOptionList;
			NKCUtil.SetGameobjectActive(m_objSortSelect, bValue: true);
			SetOpenSortingMenu(bOpen: false);
			ProcessUIFromCurrentDisplayedSortData(bResetScroll: false, m_SSCurrent.FilterSet.Count > 0);
		}
	}

	private void ProcessUIFromCurrentDisplayedSortData(bool bResetScroll, bool bSelectedAnyFilter)
	{
		NKCUtil.SetGameobjectActive(m_btnFilterSelected, bSelectedAnyFilter);
		SetUIByCurrentSortSystem();
		dOnSortOptionChange?.Invoke(bUpdate: true);
		dOnSorted?.Invoke(bResetScroll);
	}

	private void SetUIByCurrentSortSystem()
	{
		m_ctgDescending?.Select(NKCEquipSortSystem.GetDescendingBySorting(m_SSCurrent.lstSortOption), bForce: true);
		if (m_SSCurrent.lstSortOption.Count == 0)
		{
			NKCUtil.SetLabelText(m_lbSortType, NKCUtilString.GET_STRING_I_D);
			NKCUtil.SetLabelText(m_lbSelectedSortType, NKCUtilString.GET_STRING_I_D);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbSortType, NKCEquipSortSystem.GetSortName(m_SSCurrent.lstSortOption[0]));
			NKCUtil.SetLabelText(m_lbSelectedSortType, NKCEquipSortSystem.GetSortName(m_SSCurrent.lstSortOption[0]));
		}
	}

	public void ResetUI()
	{
		if (m_SSCurrent != null)
		{
			NKCUtil.SetGameobjectActive(m_btnFilterSelected, m_SSCurrent.FilterSet != null && m_SSCurrent.FilterSet.Count > 0);
			NKCUtil.SetGameobjectActive(m_objSortSelect, bValue: false);
			m_ctglSearch.Select(bSelect: false, bForce: true);
			NKCUtil.SetGameobjectActive(m_objActive, bValue: false);
			SetOpenSortingMenu(bOpen: false, bAnimate: false);
			m_ctgDescending?.Select(NKCEquipSortSystem.GetDescendingBySorting(m_SSCurrent.lstSortOption), bForce: true);
			NKCUtil.SetGameobjectActive(m_ifFilterTokens, !m_SSCurrent.m_EquipListOptions.bHideTokenFiltering);
			if (m_SSCurrent.lstSortOption.Count == 0)
			{
				NKCUtil.SetLabelText(m_lbSortType, NKCUtilString.GET_STRING_I_D);
				NKCUtil.SetLabelText(m_lbSelectedSortType, NKCUtilString.GET_STRING_I_D);
			}
			else
			{
				NKCUtil.SetLabelText(m_lbSortType, NKCEquipSortSystem.GetSortName(m_SSCurrent.lstSortOption[0]));
				NKCUtil.SetLabelText(m_lbSelectedSortType, NKCEquipSortSystem.GetSortName(m_SSCurrent.lstSortOption[0]));
			}
		}
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
