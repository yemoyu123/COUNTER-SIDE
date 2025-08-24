using System;
using System.Collections.Generic;
using NKC.Sort;
using NKM;
using NKM.Templet;

namespace NKC;

public class NKCOperatorTokenSortSystem
{
	public enum eFilterCategory
	{
		Rarity,
		PassiveSkill
	}

	public enum eFilterOption
	{
		Rarily_SSR,
		Rarily_SR,
		Rarily_R,
		Rarily_N,
		PassiveSkill
	}

	public enum eSortCategory
	{
		None,
		Rarity
	}

	public enum eSortOption
	{
		None,
		Rarity_Low,
		Rarity_High
	}

	public struct OperatorTokenListOptions
	{
		public HashSet<int> setExcludeOperatorTokenID;

		public HashSet<int> setOnlyIncludeOperatorTokenID;

		public HashSet<eFilterOption> setOnlyIncludeFilterOption;

		public HashSet<eFilterOption> setFilterOption;

		public List<eSortOption> lstSortOption;

		public List<eSortOption> lstDefaultSortOption;

		public int passiveSkillID;

		private HashSet<BUILD_OPTIONS> m_BuildOptions;

		private HashSet<BUILD_OPTIONS> BuildOption
		{
			get
			{
				if (m_BuildOptions == null)
				{
					m_BuildOptions = new HashSet<BUILD_OPTIONS>();
				}
				return m_BuildOptions;
			}
		}

		public bool IsHasBuildOption(BUILD_OPTIONS option)
		{
			return BuildOption.Contains(option);
		}

		public void SetBuildOption(bool bSet, params BUILD_OPTIONS[] options)
		{
			foreach (BUILD_OPTIONS item in options)
			{
				if (bSet && !BuildOption.Contains(item))
				{
					BuildOption.Add(item);
				}
				else if (!bSet && BuildOption.Contains(item))
				{
					BuildOption.Remove(item);
				}
			}
		}

		public OperatorTokenListOptions(NKM_DECK_TYPE deckType = NKM_DECK_TYPE.NDT_NONE)
		{
			m_BuildOptions = new HashSet<BUILD_OPTIONS>();
			m_BuildOptions.Add(BUILD_OPTIONS.DESCENDING);
			lstSortOption = new List<eSortOption>();
			lstSortOption = AddDefaultSortOptions(lstSortOption);
			lstDefaultSortOption = null;
			setFilterOption = new HashSet<eFilterOption>();
			setOnlyIncludeFilterOption = null;
			setExcludeOperatorTokenID = null;
			setOnlyIncludeOperatorTokenID = null;
			passiveSkillID = 0;
		}
	}

	private static readonly Dictionary<eSortCategory, Tuple<eSortOption, eSortOption>> s_dicSortCategory = new Dictionary<eSortCategory, Tuple<eSortOption, eSortOption>>
	{
		{
			eSortCategory.None,
			new Tuple<eSortOption, eSortOption>(eSortOption.None, eSortOption.None)
		},
		{
			eSortCategory.Rarity,
			new Tuple<eSortOption, eSortOption>(eSortOption.Rarity_Low, eSortOption.Rarity_High)
		}
	};

	private static readonly List<eSortOption> DEFAULT_OPERATOR_TOKEN_SORT_OPTION_LIST = new List<eSortOption> { eSortOption.Rarity_High };

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Rarity = new HashSet<eFilterOption>
	{
		eFilterOption.Rarily_SSR,
		eFilterOption.Rarily_SR,
		eFilterOption.Rarily_R,
		eFilterOption.Rarily_N
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_PassiveSkill = new HashSet<eFilterOption> { eFilterOption.PassiveSkill };

	private static readonly List<HashSet<eFilterOption>> m_lstFilterCategory = new List<HashSet<eFilterOption>> { m_setFilterCategory_Rarity, m_setFilterCategory_PassiveSkill };

	public static readonly HashSet<eFilterCategory> setDefaultOperatorFilterCategory = new HashSet<eFilterCategory>
	{
		eFilterCategory.Rarity,
		eFilterCategory.PassiveSkill
	};

	public static readonly HashSet<eSortCategory> setDefaultOperatorSortCategory = new HashSet<eSortCategory> { eSortCategory.Rarity };

	protected OperatorTokenListOptions m_Options;

	protected List<NKCOperatorPassiveToken> m_lstAllOperatorTokenList;

	protected List<NKCOperatorPassiveToken> m_lstCurrentOperatorTokenList;

	protected NKCOperatorTokenSortSystemFilterTokens m_FilterToken = new NKCOperatorTokenSortSystemFilterTokens();

	public int m_PassiveSkillID
	{
		get
		{
			return m_Options.passiveSkillID;
		}
		set
		{
			m_Options.passiveSkillID = value;
		}
	}

	public OperatorTokenListOptions Options
	{
		get
		{
			return m_Options;
		}
		set
		{
			m_Options = value;
		}
	}

	public string FilterTokenString
	{
		get
		{
			return m_FilterToken.TokenString;
		}
		set
		{
			m_FilterToken.SetStringToken(value);
			FilterList(m_Options.setFilterOption);
		}
	}

	public List<NKCOperatorPassiveToken> SortedOperatorTokenList
	{
		get
		{
			if (m_lstCurrentOperatorTokenList == null)
			{
				if (m_Options.setFilterOption == null)
				{
					m_Options.setFilterOption = new HashSet<eFilterOption>();
					FilterList(m_Options.setFilterOption);
				}
				else
				{
					FilterList(m_Options.setFilterOption);
				}
			}
			return m_lstCurrentOperatorTokenList;
		}
	}

	public HashSet<eFilterOption> FilterSet
	{
		get
		{
			return m_Options.setFilterOption;
		}
		set
		{
			FilterList(value);
		}
	}

	public List<eSortOption> lstSortOption
	{
		get
		{
			return m_Options.lstSortOption;
		}
		set
		{
			SortList(value, Descending);
		}
	}

	public bool Descending
	{
		get
		{
			return m_Options.IsHasBuildOption(BUILD_OPTIONS.DESCENDING);
		}
		set
		{
			if (m_Options.lstSortOption != null)
			{
				m_Options.SetBuildOption(value, BUILD_OPTIONS.DESCENDING);
				SortList(m_Options.lstSortOption);
			}
			else
			{
				m_Options.lstSortOption = GetDefaultSortOptions();
				m_Options.SetBuildOption(value, BUILD_OPTIONS.DESCENDING);
				SortList(m_Options.lstSortOption);
			}
		}
	}

	public static eSortCategory GetSortCategoryFromOption(eSortOption option)
	{
		foreach (KeyValuePair<eSortCategory, Tuple<eSortOption, eSortOption>> item in s_dicSortCategory)
		{
			if (item.Value.Item1 == option)
			{
				return item.Key;
			}
			if (item.Value.Item2 == option)
			{
				return item.Key;
			}
		}
		return eSortCategory.None;
	}

	public static eSortOption GetSortOptionByCategory(eSortCategory category, bool bDescending)
	{
		Tuple<eSortOption, eSortOption> tuple = s_dicSortCategory[category];
		if (!bDescending)
		{
			return tuple.Item1;
		}
		return tuple.Item2;
	}

	public static bool IsDescending(eSortOption option)
	{
		foreach (KeyValuePair<eSortCategory, Tuple<eSortOption, eSortOption>> item in s_dicSortCategory)
		{
			if (item.Value.Item1 == option)
			{
				return false;
			}
			if (item.Value.Item2 == option)
			{
				return true;
			}
		}
		return false;
	}

	public static eSortOption GetInvertedAscendOption(eSortOption option)
	{
		foreach (KeyValuePair<eSortCategory, Tuple<eSortOption, eSortOption>> item in s_dicSortCategory)
		{
			if (item.Value.Item1 == option)
			{
				return item.Value.Item2;
			}
			if (item.Value.Item2 == option)
			{
				return item.Value.Item1;
			}
		}
		return option;
	}

	public static List<eSortOption> GetDefaultSortOptions()
	{
		return DEFAULT_OPERATOR_TOKEN_SORT_OPTION_LIST;
	}

	public static List<eSortOption> AddDefaultSortOptions(List<eSortOption> sortOptions)
	{
		List<eSortOption> defaultSortOptions = GetDefaultSortOptions();
		if (defaultSortOptions != null)
		{
			sortOptions.AddRange(defaultSortOptions);
		}
		return sortOptions;
	}

	private NKCOperatorTokenSortSystem()
	{
	}

	public NKCOperatorTokenSortSystem(OperatorTokenListOptions options)
	{
		m_Options = options;
		NKCOperatorUtil.UpdateOperatorPassiveToken();
		m_lstAllOperatorTokenList = BuildFullTokenList(NKCOperatorUtil.m_lstPassiveToken, options);
	}

	protected List<NKCOperatorPassiveToken> BuildFullTokenList(IEnumerable<NKCOperatorPassiveToken> lstTargetPassiveToken, OperatorTokenListOptions options)
	{
		List<NKCOperatorPassiveToken> list = new List<NKCOperatorPassiveToken>();
		HashSet<int> setOnlyIncludeOperatorTokenID = options.setOnlyIncludeOperatorTokenID;
		HashSet<int> setExcludeOperatorTokenID = options.setExcludeOperatorTokenID;
		foreach (NKCOperatorPassiveToken item in lstTargetPassiveToken)
		{
			if ((setOnlyIncludeOperatorTokenID == null || setOnlyIncludeOperatorTokenID.Count <= 0 || setOnlyIncludeOperatorTokenID.Contains(item.ItemID)) && (setExcludeOperatorTokenID == null || setExcludeOperatorTokenID.Count <= 0 || !setExcludeOperatorTokenID.Contains(item.ItemID)))
			{
				list.Add(item);
			}
		}
		return list;
	}

	public virtual void BuildFilterAndSortedList(HashSet<eFilterOption> setfilterType, List<eSortOption> lstSortOption)
	{
		m_Options.setFilterOption = setfilterType;
		m_Options.lstSortOption = lstSortOption;
		FilterList(setfilterType);
	}

	public void FilterList(eFilterOption filterOption)
	{
		HashSet<eFilterOption> hashSet = new HashSet<eFilterOption>();
		hashSet.Add(filterOption);
		FilterList(hashSet);
	}

	public virtual void FilterList(HashSet<eFilterOption> setFilter)
	{
		m_Options.setFilterOption = setFilter;
		if (m_lstCurrentOperatorTokenList == null)
		{
			m_lstCurrentOperatorTokenList = new List<NKCOperatorPassiveToken>();
		}
		m_lstCurrentOperatorTokenList.Clear();
		List<HashSet<eFilterOption>> setFilter2 = SetFilterCategory(setFilter);
		foreach (NKCOperatorPassiveToken lstAllOperatorToken in m_lstAllOperatorTokenList)
		{
			if (FilterData(lstAllOperatorToken, setFilter2))
			{
				m_lstCurrentOperatorTokenList.Add(lstAllOperatorToken);
			}
		}
		if (m_Options.lstSortOption != null)
		{
			SortList(m_Options.lstSortOption, bForce: true);
			return;
		}
		m_Options.lstSortOption = new List<eSortOption>();
		SortList(m_Options.lstSortOption, bForce: true);
	}

	public void UpdateEquipData(NKCOperatorPassiveToken passiveTokenData)
	{
	}

	protected bool FilterData(NKCOperatorPassiveToken passiveTokenData, List<HashSet<eFilterOption>> setFilter)
	{
		for (int i = 0; i < setFilter.Count; i++)
		{
			if (!CheckFilter(passiveTokenData, setFilter[i], m_Options))
			{
				return false;
			}
		}
		if (!m_FilterToken.CheckFilter(passiveTokenData, m_Options))
		{
			return false;
		}
		return true;
	}

	private static bool CheckFilter(NKCOperatorPassiveToken passiveToken, HashSet<eFilterOption> setFilter, OperatorTokenListOptions options)
	{
		foreach (eFilterOption item in setFilter)
		{
			if (CheckFilter(passiveToken, item, options))
			{
				return true;
			}
		}
		return false;
	}

	private static bool CheckFilter(NKCOperatorPassiveToken tokenData, eFilterOption filterOption, OperatorTokenListOptions tokenListOptions)
	{
		switch (filterOption)
		{
		default:
			if (tokenData.ItemGrade == NKM_ITEM_GRADE.NIG_SSR)
			{
				return true;
			}
			break;
		case eFilterOption.Rarily_SR:
			if (tokenData.ItemGrade == NKM_ITEM_GRADE.NIG_SR)
			{
				return true;
			}
			break;
		case eFilterOption.Rarily_R:
			if (tokenData.ItemGrade == NKM_ITEM_GRADE.NIG_R)
			{
				return true;
			}
			break;
		case eFilterOption.Rarily_N:
			if (tokenData.ItemGrade == NKM_ITEM_GRADE.NIG_N)
			{
				return true;
			}
			break;
		case eFilterOption.PassiveSkill:
			return tokenListOptions.passiveSkillID == tokenData.PassiveSkillID;
		}
		return false;
	}

	private static List<HashSet<eFilterOption>> SetFilterCategory(HashSet<eFilterOption> setFilter)
	{
		List<HashSet<eFilterOption>> list = new List<HashSet<eFilterOption>>();
		if (setFilter.Count == 0)
		{
			return list;
		}
		for (int i = 0; i < m_lstFilterCategory.Count; i++)
		{
			HashSet<eFilterOption> hashSet = new HashSet<eFilterOption>();
			foreach (eFilterOption item in setFilter)
			{
				hashSet.Add(item);
			}
			hashSet.IntersectWith(m_lstFilterCategory[i]);
			if (hashSet.Count > 0)
			{
				list.Add(hashSet);
			}
		}
		return list;
	}

	public void SortList(eSortOption sortOption, bool bForce = false)
	{
		List<eSortOption> list = new List<eSortOption>();
		list.Add(sortOption);
		SortList(list, bForce);
	}

	public static List<eSortOption> ChangeAscend(List<eSortOption> targetList)
	{
		List<eSortOption> list = new List<eSortOption>(targetList);
		if (list == null || list.Count == 0)
		{
			return list;
		}
		list[0] = GetInvertedAscendOption(list[0]);
		return list;
	}

	public void SortList(List<eSortOption> lstSortOption, bool bForce = false)
	{
		if (m_lstCurrentOperatorTokenList == null)
		{
			m_Options.lstSortOption = lstSortOption;
			if (m_Options.setFilterOption != null)
			{
				FilterList(m_Options.setFilterOption);
				return;
			}
			m_Options.setFilterOption = new HashSet<eFilterOption>();
			FilterList(m_Options.setFilterOption);
			return;
		}
		if (!bForce && lstSortOption.Count == m_Options.lstSortOption.Count)
		{
			bool flag = true;
			for (int i = 0; i < lstSortOption.Count; i++)
			{
				if (lstSortOption[i] != m_Options.lstSortOption[i])
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				return;
			}
		}
		SortOperatorTokenDataList(ref m_lstCurrentOperatorTokenList, lstSortOption);
		m_Options.lstSortOption = lstSortOption;
	}

	private void SortOperatorTokenDataList(ref List<NKCOperatorPassiveToken> lstEquipData, List<eSortOption> lstSortOption)
	{
		NKCUnitSortSystem.NKCDataComparerer<NKCOperatorPassiveToken> nKCDataComparerer = new NKCUnitSortSystem.NKCDataComparerer<NKCOperatorPassiveToken>();
		foreach (eSortOption item in lstSortOption)
		{
			NKCUnitSortSystem.NKCDataComparerer<NKCOperatorPassiveToken>.CompareFunc dataComparer = GetDataComparer(item);
			if (dataComparer != null)
			{
				nKCDataComparerer.AddFunc(dataComparer);
			}
		}
		lstEquipData.Sort(nKCDataComparerer);
	}

	private NKCUnitSortSystem.NKCDataComparerer<NKCOperatorPassiveToken>.CompareFunc GetDataComparer(eSortOption sortOption)
	{
		if (sortOption != eSortOption.Rarity_Low)
		{
			_ = 2;
			return CompareByRarityDescending;
		}
		return CompareByRarityAscending;
	}

	private int CompareByRarityAscending(NKCOperatorPassiveToken lhs, NKCOperatorPassiveToken rhs)
	{
		if (lhs == null || rhs == null)
		{
			return -1;
		}
		return lhs.ItemGrade.CompareTo(rhs.ItemGrade);
	}

	private int CompareByRarityDescending(NKCOperatorPassiveToken lhs, NKCOperatorPassiveToken rhs)
	{
		if (lhs == null || rhs == null)
		{
			return -1;
		}
		return rhs.ItemGrade.CompareTo(lhs.ItemGrade);
	}

	public static string GetFilterName(NKM_UNIT_STYLE_TYPE type)
	{
		if (type == NKM_UNIT_STYLE_TYPE.NUST_INVALID)
		{
			return NKCUtilString.GET_STRING_FILTER_ALL;
		}
		return NKCUtilString.GetUnitStyleName(type);
	}

	public static string GetSortName(eSortOption option)
	{
		if ((uint)(option - 1) <= 1u)
		{
			return NKCUtilString.GET_STRING_SORT_RARITY;
		}
		return string.Empty;
	}
}
