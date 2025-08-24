using System;
using System.Collections.Generic;
using NKC.Sort;
using NKC.Templet;
using NKM;
using NKM.Templet;
using NKM.Templet.Office;
using UnityEngine;

namespace NKC;

public class NKCMiscSortSystem
{
	public enum eFilterCategory
	{
		InteriorTarget,
		InteriorCategory,
		InteriorCanPlace,
		Theme,
		Interaction,
		Have,
		Tier
	}

	public enum eFilterOption
	{
		Nothing,
		Everything,
		InteriorTarget_Floor,
		InteriorTarget_Tile,
		InteriorTarget_Wall,
		InteriorTarget_Background,
		InteriorCategory_DECO,
		InteriorCategory_FURNITURE,
		InteriorCanPlace,
		InteriorCannotPlace,
		Theme,
		Have,
		NotHave,
		Tier_SSR,
		Tier_SR,
		Tier_R,
		Tier_N
	}

	public enum eSortCategory
	{
		None,
		ID,
		Point,
		Rarity,
		RegDate,
		CanPlace,
		Custom1,
		Custom2,
		Custom3
	}

	public enum eSortOption
	{
		None,
		ID_First,
		ID_Last,
		Point_High,
		Point_Low,
		Rarity_High,
		Rarity_Low,
		RegDate_First,
		RegDate_Last,
		CanPlace,
		CannotPlace,
		CustomAscend1,
		CustomDescend1,
		CustomAscend2,
		CustomDescend2,
		CustomAscend3,
		CustomDescend3
	}

	public struct MiscListOptions
	{
		public delegate bool CustomFilterFunc(NKMItemMiscTemplet miscTemplet);

		public HashSet<int> setOnlyIncludeMiscID;

		public HashSet<int> setExcludeMiscID;

		public HashSet<eFilterOption> setOnlyIncludeFilterOption;

		public HashSet<eFilterOption> setFilterOption;

		public HashSet<int> setFilterOptionTitleCategoryKey;

		public List<eSortOption> lstSortOption;

		public NKCDataComparerer<NKMItemMiscTemplet>.CompareFunc PreemptiveSortFunc;

		public Dictionary<eSortCategory, KeyValuePair<string, NKCDataComparerer<NKMItemMiscTemplet>.CompareFunc>> lstCustomSortFunc;

		public CustomFilterFunc AdditionalExcludeFilterFunc;

		public List<eSortOption> lstDefaultSortOption;

		public bool bPushBackUnselectable;

		public int m_filterThemeID;

		public bool bHideTokenFiltering;

		public bool bHideDescendingOption;

		public bool bHideSortOption;

		public bool bHideFilterOption;
	}

	public class NKCDataComparerer<T> : Comparer<T>
	{
		public delegate int CompareFunc(T lhs, T rhs);

		private List<CompareFunc> m_lstFunc = new List<CompareFunc>();

		public NKCDataComparerer(params CompareFunc[] comparers)
		{
			foreach (CompareFunc item in comparers)
			{
				m_lstFunc.Add(item);
			}
		}

		public void AddFunc(CompareFunc func)
		{
			m_lstFunc.Add(func);
		}

		public override int Compare(T lhs, T rhs)
		{
			foreach (CompareFunc item in m_lstFunc)
			{
				int num = item(lhs, rhs);
				if (num != 0)
				{
					return num;
				}
			}
			return 0;
		}
	}

	private static readonly Dictionary<eSortCategory, Tuple<eSortOption, eSortOption>> s_dicSortCategory = new Dictionary<eSortCategory, Tuple<eSortOption, eSortOption>>
	{
		{
			eSortCategory.None,
			new Tuple<eSortOption, eSortOption>(eSortOption.None, eSortOption.None)
		},
		{
			eSortCategory.ID,
			new Tuple<eSortOption, eSortOption>(eSortOption.ID_First, eSortOption.ID_Last)
		},
		{
			eSortCategory.Point,
			new Tuple<eSortOption, eSortOption>(eSortOption.Point_Low, eSortOption.Point_High)
		},
		{
			eSortCategory.Rarity,
			new Tuple<eSortOption, eSortOption>(eSortOption.Rarity_Low, eSortOption.Rarity_High)
		},
		{
			eSortCategory.RegDate,
			new Tuple<eSortOption, eSortOption>(eSortOption.RegDate_First, eSortOption.RegDate_Last)
		},
		{
			eSortCategory.CanPlace,
			new Tuple<eSortOption, eSortOption>(eSortOption.CannotPlace, eSortOption.CanPlace)
		},
		{
			eSortCategory.Custom1,
			new Tuple<eSortOption, eSortOption>(eSortOption.CustomAscend1, eSortOption.CustomDescend1)
		},
		{
			eSortCategory.Custom2,
			new Tuple<eSortOption, eSortOption>(eSortOption.CustomAscend2, eSortOption.CustomDescend2)
		},
		{
			eSortCategory.Custom3,
			new Tuple<eSortOption, eSortOption>(eSortOption.CustomAscend3, eSortOption.CustomDescend3)
		}
	};

	private static readonly List<eSortOption> DEFAULT_MISC_SORT_OPTION_LIST = new List<eSortOption>
	{
		eSortOption.Rarity_High,
		eSortOption.ID_First
	};

	private static readonly List<eSortOption> DEFAULT_INTERIOR_SORT_OPTION_LIST = new List<eSortOption>
	{
		eSortOption.Rarity_High,
		eSortOption.Point_High,
		eSortOption.ID_First
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_InteriorTarget = new HashSet<eFilterOption>
	{
		eFilterOption.InteriorTarget_Floor,
		eFilterOption.InteriorTarget_Tile,
		eFilterOption.InteriorTarget_Wall,
		eFilterOption.InteriorTarget_Background
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_InteriorCategory = new HashSet<eFilterOption>
	{
		eFilterOption.InteriorCategory_DECO,
		eFilterOption.InteriorCategory_FURNITURE
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_InteriorCanPlace = new HashSet<eFilterOption>
	{
		eFilterOption.InteriorCanPlace,
		eFilterOption.InteriorCannotPlace
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Have = new HashSet<eFilterOption>
	{
		eFilterOption.Have,
		eFilterOption.NotHave
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Rarity = new HashSet<eFilterOption>
	{
		eFilterOption.Tier_SSR,
		eFilterOption.Tier_SR,
		eFilterOption.Tier_R,
		eFilterOption.Tier_N
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Theme = new HashSet<eFilterOption> { eFilterOption.Theme };

	private static readonly List<HashSet<eFilterOption>> m_lstFilterCategory = new List<HashSet<eFilterOption>> { m_setFilterCategory_InteriorCategory, m_setFilterCategory_InteriorTarget, m_setFilterCategory_InteriorCanPlace, m_setFilterCategory_Have, m_setFilterCategory_Rarity, m_setFilterCategory_Theme };

	private static readonly HashSet<eFilterCategory> setDefaultMiscFilterCategory = new HashSet<eFilterCategory> { eFilterCategory.Tier };

	private static readonly HashSet<eFilterCategory> setDefaultInteriorFilterCategory = new HashSet<eFilterCategory>
	{
		eFilterCategory.InteriorCanPlace,
		eFilterCategory.InteriorCategory,
		eFilterCategory.InteriorTarget,
		eFilterCategory.Tier,
		eFilterCategory.Theme
	};

	private static readonly HashSet<eSortCategory> setDefaultMiscSortCategory = new HashSet<eSortCategory>
	{
		eSortCategory.Rarity,
		eSortCategory.ID
	};

	private static readonly HashSet<eSortCategory> setDefaultInteriorSortCategory = new HashSet<eSortCategory>
	{
		eSortCategory.Rarity,
		eSortCategory.Point,
		eSortCategory.CanPlace
	};

	protected MiscListOptions m_Options;

	protected Dictionary<int, NKMItemMiscTemplet> m_dicAllMiscList;

	protected List<NKMItemMiscTemplet> m_lstCurrentMiscList;

	protected NKCMiscSortSystemFilterTokens m_FilterToken = new NKCMiscSortSystemFilterTokens();

	public int FilterStatType_ThemeID
	{
		get
		{
			return m_Options.m_filterThemeID;
		}
		set
		{
			m_Options.m_filterThemeID = value;
		}
	}

	public MiscListOptions Options => m_Options;

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

	public List<NKMItemMiscTemplet> SortedMiscList
	{
		get
		{
			if (m_lstCurrentMiscList == null)
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
			return m_lstCurrentMiscList;
		}
	}

	public HashSet<eFilterOption> OnlyIncludeFilterSet => m_Options.setOnlyIncludeFilterOption;

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

	public HashSet<int> FilterSetTitlecategory
	{
		get
		{
			return m_Options.setFilterOptionTitleCategoryKey;
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
			SortList(value);
		}
	}

	public static bool GetDescendingBySorting(List<eSortOption> lstSortOption)
	{
		if (lstSortOption.Count > 0)
		{
			return GetDescendingBySorting(lstSortOption[0]);
		}
		return true;
	}

	public static bool GetDescendingBySorting(eSortOption sortOption)
	{
		switch (sortOption)
		{
		default:
			return true;
		case eSortOption.ID_Last:
		case eSortOption.Point_Low:
		case eSortOption.Rarity_Low:
		case eSortOption.RegDate_Last:
		case eSortOption.CannotPlace:
		case eSortOption.CustomAscend1:
		case eSortOption.CustomAscend2:
		case eSortOption.CustomAscend3:
			return false;
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

	public static HashSet<eFilterCategory> GetDefaultFilterCategory()
	{
		return setDefaultMiscFilterCategory;
	}

	public static HashSet<eFilterCategory> GetDefaultInteriorFilterCategory()
	{
		return setDefaultInteriorFilterCategory;
	}

	public static HashSet<eFilterCategory> GetDefaultFrameFilterCategory()
	{
		return new HashSet<eFilterCategory>();
	}

	public static HashSet<eFilterCategory> GetDefaultTitleFilterCategory()
	{
		return new HashSet<eFilterCategory> { eFilterCategory.Have };
	}

	public static HashSet<eSortCategory> GetDefaultSortCategory()
	{
		return setDefaultMiscSortCategory;
	}

	public static HashSet<eSortCategory> GetDefaultInteriorSortCategory()
	{
		return setDefaultInteriorSortCategory;
	}

	public static HashSet<eSortCategory> GetDefaultFrameSortCategory()
	{
		return new HashSet<eSortCategory>
		{
			eSortCategory.ID,
			eSortCategory.RegDate
		};
	}

	public static HashSet<eSortCategory> GetDefaultTitleSortCategory()
	{
		return new HashSet<eSortCategory>
		{
			eSortCategory.ID,
			eSortCategory.Rarity,
			eSortCategory.RegDate
		};
	}

	public static List<eSortOption> GetDefaultSortList()
	{
		return DEFAULT_MISC_SORT_OPTION_LIST;
	}

	public static List<eSortOption> GetDefaultInteriorSortList()
	{
		return DEFAULT_INTERIOR_SORT_OPTION_LIST;
	}

	public static List<eSortOption> GetFrameSortList()
	{
		return new List<eSortOption>
		{
			eSortOption.ID_First,
			eSortOption.RegDate_First
		};
	}

	public static List<eSortOption> GetTitleSortList()
	{
		return new List<eSortOption>
		{
			eSortOption.ID_First,
			eSortOption.Rarity_High,
			eSortOption.RegDate_First
		};
	}

	protected NKCMiscSortSystem()
	{
	}

	public NKCMiscSortSystem(NKMUserData userData, IEnumerable<NKMItemMiscTemplet> lstTargetMiscs, MiscListOptions options)
	{
		m_Options = options;
		m_FilterToken.Clear();
		m_dicAllMiscList = BuildFullMiscList(userData, lstTargetMiscs, options);
	}

	public void BuildFilterAndSortedList(HashSet<eFilterOption> setfilterType, List<eSortOption> lstSortOption)
	{
		m_Options.setFilterOption = setfilterType;
		m_Options.lstSortOption = lstSortOption;
		FilterList(setfilterType);
	}

	private Dictionary<int, NKMItemMiscTemplet> BuildFullMiscList(NKMUserData userData, IEnumerable<NKMItemMiscTemplet> lstTargetMiscs, MiscListOptions options)
	{
		Dictionary<int, NKMItemMiscTemplet> dictionary = new Dictionary<int, NKMItemMiscTemplet>();
		HashSet<int> setOnlyIncludeMiscID = options.setOnlyIncludeMiscID;
		HashSet<int> setExcludeMiscID = options.setExcludeMiscID;
		foreach (NKMItemMiscTemplet lstTargetMisc in lstTargetMiscs)
		{
			if ((options.AdditionalExcludeFilterFunc == null || options.AdditionalExcludeFilterFunc(lstTargetMisc)) && (options.setExcludeMiscID == null || !options.setExcludeMiscID.Contains(lstTargetMisc.m_ItemMiscID)) && (setOnlyIncludeMiscID == null || setOnlyIncludeMiscID.Contains(lstTargetMisc.m_ItemMiscID)) && (setExcludeMiscID == null || !setExcludeMiscID.Contains(lstTargetMisc.m_ItemMiscID)) && (options.setOnlyIncludeFilterOption == null || CheckFilter(lstTargetMisc, options.setOnlyIncludeFilterOption)))
			{
				dictionary.Add(lstTargetMisc.m_ItemMiscID, lstTargetMisc);
			}
		}
		return dictionary;
	}

	protected bool FilterData(NKMItemMiscTemplet miscData, List<HashSet<eFilterOption>> setFilter)
	{
		for (int i = 0; i < setFilter.Count; i++)
		{
			if (!CheckFilter(miscData, setFilter[i]))
			{
				return false;
			}
		}
		if (!m_FilterToken.CheckFilter(miscData, m_Options))
		{
			return false;
		}
		return true;
	}

	protected bool FilterData(NKMItemMiscTemplet miscData, HashSet<int> setFilterTitleCategory)
	{
		if (setFilterTitleCategory.Count > 0 && !CheckFilter(miscData, setFilterTitleCategory))
		{
			return false;
		}
		if (!m_FilterToken.CheckFilter(miscData, m_Options))
		{
			return false;
		}
		return true;
	}

	private bool CheckFilter(NKMItemMiscTemplet miscTmplet, HashSet<int> setFilterTitleCategory)
	{
		foreach (int item in setFilterTitleCategory)
		{
			if (CheckFilter(miscTmplet, item))
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckFilter(NKMItemMiscTemplet miscTemplet, int setFilterTitleCategoryKey)
	{
		NKMTitleTemplet nKMTitleTemplet = NKMTitleTemplet.Find(miscTemplet.Key);
		if (nKMTitleTemplet == null)
		{
			return false;
		}
		return nKMTitleTemplet.TitleCategoryID == setFilterTitleCategoryKey;
	}

	private bool CheckFilter(NKMItemMiscTemplet miscTmplet, HashSet<eFilterOption> setFilter)
	{
		foreach (eFilterOption item in setFilter)
		{
			if (CheckFilter(miscTmplet, item))
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckFilter(NKMItemMiscTemplet miscTemplet, eFilterOption filterOption)
	{
		switch (filterOption)
		{
		case eFilterOption.Nothing:
			return false;
		case eFilterOption.Everything:
			return true;
		case eFilterOption.InteriorCategory_DECO:
		{
			NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet5 = NKMOfficeInteriorTemplet.Find(miscTemplet.m_ItemMiscID);
			if (nKMOfficeInteriorTemplet5 == null)
			{
				return false;
			}
			return nKMOfficeInteriorTemplet5.InteriorCategory == InteriorCategory.DECO;
		}
		case eFilterOption.InteriorCategory_FURNITURE:
		{
			NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet7 = NKMOfficeInteriorTemplet.Find(miscTemplet.m_ItemMiscID);
			if (nKMOfficeInteriorTemplet7 == null)
			{
				return false;
			}
			return nKMOfficeInteriorTemplet7.InteriorCategory == InteriorCategory.FURNITURE;
		}
		case eFilterOption.InteriorTarget_Background:
		{
			NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet3 = NKMOfficeInteriorTemplet.Find(miscTemplet.m_ItemMiscID);
			if (nKMOfficeInteriorTemplet3 == null)
			{
				return false;
			}
			return nKMOfficeInteriorTemplet3.Target == InteriorTarget.Background;
		}
		case eFilterOption.InteriorTarget_Floor:
		{
			NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet6 = NKMOfficeInteriorTemplet.Find(miscTemplet.m_ItemMiscID);
			if (nKMOfficeInteriorTemplet6 == null)
			{
				return false;
			}
			return nKMOfficeInteriorTemplet6.Target == InteriorTarget.Floor;
		}
		case eFilterOption.InteriorTarget_Tile:
		{
			NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet4 = NKMOfficeInteriorTemplet.Find(miscTemplet.m_ItemMiscID);
			if (nKMOfficeInteriorTemplet4 == null)
			{
				return false;
			}
			return nKMOfficeInteriorTemplet4.Target == InteriorTarget.Tile;
		}
		case eFilterOption.InteriorTarget_Wall:
		{
			NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet2 = NKMOfficeInteriorTemplet.Find(miscTemplet.m_ItemMiscID);
			if (nKMOfficeInteriorTemplet2 == null)
			{
				return false;
			}
			return nKMOfficeInteriorTemplet2.Target == InteriorTarget.Wall;
		}
		case eFilterOption.Tier_SSR:
			return miscTemplet.m_NKM_ITEM_GRADE == NKM_ITEM_GRADE.NIG_SSR;
		case eFilterOption.Tier_SR:
			return miscTemplet.m_NKM_ITEM_GRADE == NKM_ITEM_GRADE.NIG_SR;
		case eFilterOption.Tier_R:
			return miscTemplet.m_NKM_ITEM_GRADE == NKM_ITEM_GRADE.NIG_R;
		case eFilterOption.Tier_N:
			return miscTemplet.m_NKM_ITEM_GRADE == NKM_ITEM_GRADE.NIG_N;
		case eFilterOption.InteriorCanPlace:
			return NKCScenManager.CurrentUserData().OfficeData.GetFreeInteriorCount(miscTemplet.m_ItemMiscID) > 0;
		case eFilterOption.InteriorCannotPlace:
			return NKCScenManager.CurrentUserData().OfficeData.GetFreeInteriorCount(miscTemplet.m_ItemMiscID) <= 0;
		case eFilterOption.Have:
			if (NKMOfficeInteriorTemplet.Find(miscTemplet.m_ItemMiscID) != null)
			{
				return NKCScenManager.CurrentUserData().OfficeData.GetInteriorCount(miscTemplet.m_ItemMiscID) > 0;
			}
			return NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(miscTemplet.m_ItemMiscID) > 0;
		case eFilterOption.NotHave:
			if (NKMOfficeInteriorTemplet.Find(miscTemplet.m_ItemMiscID) != null)
			{
				return NKCScenManager.CurrentUserData().OfficeData.GetInteriorCount(miscTemplet.m_ItemMiscID) <= 0;
			}
			return NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(miscTemplet.m_ItemMiscID) <= 0;
		case eFilterOption.Theme:
		{
			if (m_Options.m_filterThemeID == 0)
			{
				return true;
			}
			NKCThemeGroupTemplet nKCThemeGroupTemplet = NKCThemeGroupTemplet.Find(m_Options.m_filterThemeID);
			if (nKCThemeGroupTemplet == null)
			{
				Debug.LogError($"Logic Error : theme templet not found. id : {m_Options.m_filterThemeID}");
				return true;
			}
			NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = NKMOfficeInteriorTemplet.Find(miscTemplet.m_ItemMiscID);
			if (nKMOfficeInteriorTemplet == null)
			{
				return false;
			}
			return nKCThemeGroupTemplet.GroupID.Contains(nKMOfficeInteriorTemplet.GroupID);
		}
		default:
			return false;
		}
	}

	public void FilterList(HashSet<eFilterOption> setFilter)
	{
		if (setFilter == null)
		{
			setFilter = new HashSet<eFilterOption>();
		}
		m_Options.setFilterOption = setFilter;
		if (m_lstCurrentMiscList == null)
		{
			m_lstCurrentMiscList = new List<NKMItemMiscTemplet>();
		}
		m_lstCurrentMiscList.Clear();
		List<HashSet<eFilterOption>> needFilterSet = new List<HashSet<eFilterOption>>();
		SetFilterCategory(setFilter, ref needFilterSet);
		foreach (KeyValuePair<int, NKMItemMiscTemplet> dicAllMisc in m_dicAllMiscList)
		{
			NKMItemMiscTemplet value = dicAllMisc.Value;
			if (FilterData(value, needFilterSet))
			{
				m_lstCurrentMiscList.Add(value);
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

	public void FilterList(HashSet<int> setFilterOptionTitleCategoryKey)
	{
		if (setFilterOptionTitleCategoryKey == null)
		{
			setFilterOptionTitleCategoryKey = new HashSet<int>();
		}
		m_Options.setFilterOptionTitleCategoryKey = setFilterOptionTitleCategoryKey;
		if (m_lstCurrentMiscList == null)
		{
			m_lstCurrentMiscList = new List<NKMItemMiscTemplet>();
		}
		m_lstCurrentMiscList.Clear();
		foreach (KeyValuePair<int, NKMItemMiscTemplet> dicAllMisc in m_dicAllMiscList)
		{
			NKMItemMiscTemplet value = dicAllMisc.Value;
			if (FilterData(value, setFilterOptionTitleCategoryKey))
			{
				m_lstCurrentMiscList.Add(value);
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

	private void SetFilterCategory(HashSet<eFilterOption> setFilter, ref List<HashSet<eFilterOption>> needFilterSet)
	{
		if (setFilter.Count == 0)
		{
			return;
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
				needFilterSet.Add(hashSet);
			}
		}
	}

	public void SortList(List<eSortOption> lstSortOption, bool bForce = false)
	{
		if (m_lstCurrentMiscList == null)
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
		SortMiscDataList(ref m_lstCurrentMiscList, lstSortOption);
		m_Options.lstSortOption = lstSortOption;
	}

	public List<NKMItemMiscTemplet> GetCurrentMiscList()
	{
		return m_lstCurrentMiscList;
	}

	private void SortMiscDataList(ref List<NKMItemMiscTemplet> lstMiscTemplet, List<eSortOption> lstSortOption)
	{
		NKCDataComparerer<NKMItemMiscTemplet> nKCDataComparerer = new NKCDataComparerer<NKMItemMiscTemplet>();
		HashSet<eSortCategory> hashSet = new HashSet<eSortCategory>();
		if (m_Options.PreemptiveSortFunc != null)
		{
			nKCDataComparerer.AddFunc(m_Options.PreemptiveSortFunc);
		}
		foreach (eSortOption item in lstSortOption)
		{
			if (item != eSortOption.None)
			{
				NKCDataComparerer<NKMItemMiscTemplet>.CompareFunc dataComparer = GetDataComparer(item);
				if (dataComparer != null)
				{
					nKCDataComparerer.AddFunc(dataComparer);
					hashSet.Add(GetSortCategoryFromOption(item));
				}
			}
		}
		if (m_Options.lstDefaultSortOption != null)
		{
			foreach (eSortOption item2 in m_Options.lstDefaultSortOption)
			{
				eSortCategory sortCategoryFromOption = GetSortCategoryFromOption(item2);
				if (!hashSet.Contains(sortCategoryFromOption))
				{
					nKCDataComparerer.AddFunc(GetDataComparer(item2));
					hashSet.Add(sortCategoryFromOption);
				}
			}
		}
		if (!hashSet.Contains(eSortCategory.ID))
		{
			nKCDataComparerer.AddFunc(CompareByIDAscending);
		}
		lstMiscTemplet.Sort(nKCDataComparerer);
	}

	private NKCDataComparerer<NKMItemMiscTemplet>.CompareFunc GetDataComparer(eSortOption sortOption)
	{
		switch (sortOption)
		{
		default:
			return CompareByRarityDescending;
		case eSortOption.Rarity_Low:
			return CompareByRarityAscending;
		case eSortOption.ID_First:
			return CompareByIDAscending;
		case eSortOption.ID_Last:
			return CompareByIDDescending;
		case eSortOption.Point_High:
			return CompareByPointDescending;
		case eSortOption.Point_Low:
			return CompareByPointAscending;
		case eSortOption.RegDate_First:
			return CompareByRegDateAscending;
		case eSortOption.RegDate_Last:
			return CompareByRegDateDescending;
		case eSortOption.CanPlace:
			return CompareByCanPlace;
		case eSortOption.CannotPlace:
			return CompareByCannotPlace;
		case eSortOption.CustomDescend1:
		case eSortOption.CustomDescend2:
		case eSortOption.CustomDescend3:
			if (m_Options.lstCustomSortFunc.ContainsKey(GetSortCategoryFromOption(sortOption)))
			{
				return m_Options.lstCustomSortFunc[GetSortCategoryFromOption(sortOption)].Value;
			}
			return null;
		case eSortOption.CustomAscend1:
		case eSortOption.CustomAscend2:
		case eSortOption.CustomAscend3:
			if (m_Options.lstCustomSortFunc.ContainsKey(GetSortCategoryFromOption(sortOption)))
			{
				return (NKMItemMiscTemplet a, NKMItemMiscTemplet b) => m_Options.lstCustomSortFunc[GetSortCategoryFromOption(sortOption)].Value(b, a);
			}
			return null;
		}
	}

	public static int CompareByRarityAscending(NKMItemMiscTemplet lhs, NKMItemMiscTemplet rhs)
	{
		NKMItemMiscTemplet nKMItemMiscTemplet = NKMItemMiscTemplet.Find(lhs.m_ItemMiscID);
		NKMItemMiscTemplet nKMItemMiscTemplet2 = NKMItemMiscTemplet.Find(rhs.m_ItemMiscID);
		return nKMItemMiscTemplet.m_NKM_ITEM_GRADE.CompareTo(nKMItemMiscTemplet2.m_NKM_ITEM_GRADE);
	}

	public static int CompareByRarityDescending(NKMItemMiscTemplet lhs, NKMItemMiscTemplet rhs)
	{
		NKMItemMiscTemplet nKMItemMiscTemplet = NKMItemMiscTemplet.Find(lhs.m_ItemMiscID);
		return NKMItemMiscTemplet.Find(rhs.m_ItemMiscID).m_NKM_ITEM_GRADE.CompareTo(nKMItemMiscTemplet.m_NKM_ITEM_GRADE);
	}

	private int CompareByIDAscending(NKMItemMiscTemplet lhs, NKMItemMiscTemplet rhs)
	{
		NKMTitleTemplet nKMTitleTemplet = NKMTitleTemplet.Find(lhs.m_ItemMiscID);
		NKMTitleTemplet nKMTitleTemplet2 = NKMTitleTemplet.Find(rhs.m_ItemMiscID);
		if (nKMTitleTemplet != null && nKMTitleTemplet2 != null)
		{
			return nKMTitleTemplet.OrderIndex.CompareTo(nKMTitleTemplet2.OrderIndex);
		}
		return lhs.m_ItemMiscID.CompareTo(rhs.m_ItemMiscID);
	}

	private int CompareByIDDescending(NKMItemMiscTemplet lhs, NKMItemMiscTemplet rhs)
	{
		NKMTitleTemplet nKMTitleTemplet = NKMTitleTemplet.Find(lhs.m_ItemMiscID);
		NKMTitleTemplet nKMTitleTemplet2 = NKMTitleTemplet.Find(rhs.m_ItemMiscID);
		if (nKMTitleTemplet != null && nKMTitleTemplet2 != null)
		{
			return nKMTitleTemplet2.OrderIndex.CompareTo(nKMTitleTemplet.OrderIndex);
		}
		return rhs.m_ItemMiscID.CompareTo(lhs.m_ItemMiscID);
	}

	private int CompareByPointAscending(NKMItemMiscTemplet lhs, NKMItemMiscTemplet rhs)
	{
		NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = NKMOfficeInteriorTemplet.Find(lhs.m_ItemMiscID);
		NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet2 = NKMOfficeInteriorTemplet.Find(rhs.m_ItemMiscID);
		return nKMOfficeInteriorTemplet.InteriorScore.CompareTo(nKMOfficeInteriorTemplet2.InteriorScore);
	}

	private int CompareByPointDescending(NKMItemMiscTemplet lhs, NKMItemMiscTemplet rhs)
	{
		NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = NKMOfficeInteriorTemplet.Find(lhs.m_ItemMiscID);
		return NKMOfficeInteriorTemplet.Find(rhs.m_ItemMiscID).InteriorScore.CompareTo(nKMOfficeInteriorTemplet.InteriorScore);
	}

	private int CompareByRegDateAscending(NKMItemMiscTemplet lhs, NKMItemMiscTemplet rhs)
	{
		NKMItemMiscData itemMisc = NKCScenManager.CurrentUserData().m_InventoryData.GetItemMisc(lhs);
		NKMItemMiscData itemMisc2 = NKCScenManager.CurrentUserData().m_InventoryData.GetItemMisc(rhs);
		if (itemMisc == null && itemMisc2 == null)
		{
			return CompareByIDAscending(lhs, rhs);
		}
		if (itemMisc == null)
		{
			return 1;
		}
		if (itemMisc2 == null)
		{
			return -1;
		}
		return itemMisc.RegDate.CompareTo(itemMisc2.RegDate);
	}

	private int CompareByRegDateDescending(NKMItemMiscTemplet lhs, NKMItemMiscTemplet rhs)
	{
		NKMItemMiscData itemMisc = NKCScenManager.CurrentUserData().m_InventoryData.GetItemMisc(lhs);
		NKMItemMiscData itemMisc2 = NKCScenManager.CurrentUserData().m_InventoryData.GetItemMisc(rhs);
		if (itemMisc == null && itemMisc2 == null)
		{
			return CompareByIDAscending(lhs, rhs);
		}
		if (itemMisc == null)
		{
			return 1;
		}
		return itemMisc2?.RegDate.CompareTo(itemMisc.RegDate) ?? (-1);
	}

	private int CompareByCanPlace(NKMItemMiscTemplet lhs, NKMItemMiscTemplet rhs)
	{
		long freeInteriorCount = NKCScenManager.CurrentUserData().OfficeData.GetFreeInteriorCount(lhs.m_ItemMiscID);
		return NKCScenManager.CurrentUserData().OfficeData.GetFreeInteriorCount(rhs.m_ItemMiscID).CompareTo(freeInteriorCount);
	}

	private int CompareByCannotPlace(NKMItemMiscTemplet lhs, NKMItemMiscTemplet rhs)
	{
		long freeInteriorCount = NKCScenManager.CurrentUserData().OfficeData.GetFreeInteriorCount(lhs.m_ItemMiscID);
		long freeInteriorCount2 = NKCScenManager.CurrentUserData().OfficeData.GetFreeInteriorCount(rhs.m_ItemMiscID);
		return freeInteriorCount.CompareTo(freeInteriorCount2);
	}

	public static string GetSortName(eSortOption sortOption)
	{
		return GetSortName(GetSortCategoryFromOption(sortOption));
	}

	public static string GetSortName(eSortCategory sortCategory)
	{
		return sortCategory switch
		{
			eSortCategory.ID => NKCUtilString.GET_STRING_SORT_IDX, 
			eSortCategory.Point => NKCUtilString.GET_STRING_SORT_INTERIOR_POINT, 
			eSortCategory.RegDate => NKCUtilString.GET_STRING_SORT_UID, 
			eSortCategory.CanPlace => NKCUtilString.GET_STRING_SORT_PLACE_TYPE, 
			_ => NKCUtilString.GET_STRING_SORT_RARITY, 
		};
	}
}
