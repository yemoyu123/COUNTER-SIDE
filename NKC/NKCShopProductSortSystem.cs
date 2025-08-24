using System;
using System.Collections.Generic;
using NKC.Templet;
using NKM;
using NKM.Shop;
using NKM.Templet;
using NKM.Templet.Office;
using UnityEngine;

namespace NKC;

public class NKCShopProductSortSystem
{
	public enum eFilterCategory
	{
		Theme
	}

	public enum eFilterOption
	{
		Nothing,
		Everything,
		Theme
	}

	public enum eSortCategory
	{
		None,
		Default,
		ProductID,
		ItemID,
		Rarity,
		Custom1,
		Custom2,
		Custom3
	}

	public enum eSortOption
	{
		None,
		Default_First,
		Default_Last,
		ProductID_First,
		ProductID_Last,
		ItemID_First,
		ItemID_Last,
		Rarity_High,
		Rarity_Low,
		CustomAscend1,
		CustomDescend1,
		CustomAscend2,
		CustomDescend2,
		CustomAscend3,
		CustomDescend3
	}

	public struct ShopProductListOptions
	{
		public delegate bool CustomFilterFunc(ShopItemTemplet miscTemplet);

		public HashSet<eFilterOption> setFilterOption;

		public List<eSortOption> lstSortOption;

		public NKCUnitSortSystem.NKCDataComparerer<ShopItemTemplet>.CompareFunc PreemptiveSortFunc;

		public Dictionary<eSortCategory, KeyValuePair<string, NKCUnitSortSystem.NKCDataComparerer<ShopItemTemplet>.CompareFunc>> lstCustomSortFunc;

		public CustomFilterFunc AdditionalExcludeFilterFunc;

		public List<eSortOption> lstDefaultSortOption;

		public int m_filterThemeID;
	}

	private static readonly Dictionary<eSortCategory, Tuple<eSortOption, eSortOption>> s_dicSortCategory = new Dictionary<eSortCategory, Tuple<eSortOption, eSortOption>>
	{
		{
			eSortCategory.None,
			new Tuple<eSortOption, eSortOption>(eSortOption.None, eSortOption.None)
		},
		{
			eSortCategory.Default,
			new Tuple<eSortOption, eSortOption>(eSortOption.Default_First, eSortOption.Default_Last)
		},
		{
			eSortCategory.ProductID,
			new Tuple<eSortOption, eSortOption>(eSortOption.ProductID_First, eSortOption.ProductID_Last)
		},
		{
			eSortCategory.ItemID,
			new Tuple<eSortOption, eSortOption>(eSortOption.ItemID_First, eSortOption.ItemID_Last)
		},
		{
			eSortCategory.Rarity,
			new Tuple<eSortOption, eSortOption>(eSortOption.Rarity_Low, eSortOption.Rarity_High)
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

	private static readonly List<eSortOption> DEFAULT_SHOP_SORT_OPTION_LIST = new List<eSortOption>
	{
		eSortOption.Default_First,
		eSortOption.ItemID_First
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Theme = new HashSet<eFilterOption> { eFilterOption.Theme };

	private static readonly List<HashSet<eFilterOption>> m_lstFilterCategory = new List<HashSet<eFilterOption>> { m_setFilterCategory_Theme };

	protected ShopProductListOptions m_Options;

	protected Dictionary<int, ShopItemTemplet> m_dicAllProductList;

	protected List<ShopItemTemplet> m_lstCurrentProductList;

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

	public List<ShopItemTemplet> SortedProductList
	{
		get
		{
			if (m_lstCurrentProductList == null)
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
			return m_lstCurrentProductList;
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
		case eSortOption.Default_Last:
		case eSortOption.ProductID_Last:
		case eSortOption.ItemID_Last:
		case eSortOption.Rarity_Low:
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

	protected NKCShopProductSortSystem()
	{
	}

	public NKCShopProductSortSystem(NKMUserData userData, IEnumerable<ShopItemTemplet> lstTargetProducts, ShopProductListOptions options)
	{
		m_Options = options;
		m_dicAllProductList = BuildFullList(userData, lstTargetProducts, options);
	}

	public void BuildFilterAndSortedList(HashSet<eFilterOption> setfilterType, List<eSortOption> lstSortOption)
	{
		m_Options.setFilterOption = setfilterType;
		m_Options.lstSortOption = lstSortOption;
		FilterList(setfilterType);
	}

	private Dictionary<int, ShopItemTemplet> BuildFullList(NKMUserData userData, IEnumerable<ShopItemTemplet> lstTarget, ShopProductListOptions options)
	{
		Dictionary<int, ShopItemTemplet> dictionary = new Dictionary<int, ShopItemTemplet>();
		foreach (ShopItemTemplet item in lstTarget)
		{
			if (options.AdditionalExcludeFilterFunc == null || options.AdditionalExcludeFilterFunc(item))
			{
				dictionary.Add(item.Key, item);
			}
		}
		return dictionary;
	}

	protected bool FilterData(ShopItemTemplet shopTemplet, List<HashSet<eFilterOption>> setFilter)
	{
		if (setFilter == null || setFilter.Count == 0)
		{
			return true;
		}
		for (int i = 0; i < setFilter.Count; i++)
		{
			if (!CheckFilter(shopTemplet, setFilter[i]))
			{
				return false;
			}
		}
		return true;
	}

	private bool CheckFilter(ShopItemTemplet shopTemplet, HashSet<eFilterOption> setFilter)
	{
		foreach (eFilterOption item in setFilter)
		{
			if (CheckFilter(shopTemplet, item))
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckFilter(ShopItemTemplet shopTemplet, eFilterOption filterOption)
	{
		switch (filterOption)
		{
		case eFilterOption.Nothing:
			return false;
		case eFilterOption.Everything:
			return true;
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
			if (shopTemplet.m_ItemType != NKM_REWARD_TYPE.RT_MISC)
			{
				return false;
			}
			NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = NKMOfficeInteriorTemplet.Find(shopTemplet.m_ItemID);
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
		if (m_lstCurrentProductList == null)
		{
			m_lstCurrentProductList = new List<ShopItemTemplet>();
		}
		m_lstCurrentProductList.Clear();
		List<HashSet<eFilterOption>> needFilterSet = new List<HashSet<eFilterOption>>();
		SetFilterCategory(setFilter, ref needFilterSet);
		foreach (KeyValuePair<int, ShopItemTemplet> dicAllProduct in m_dicAllProductList)
		{
			ShopItemTemplet value = dicAllProduct.Value;
			if (FilterData(value, needFilterSet))
			{
				m_lstCurrentProductList.Add(value);
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
		if (m_lstCurrentProductList == null)
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
		SortDataList(ref m_lstCurrentProductList, lstSortOption);
		m_Options.lstSortOption = lstSortOption;
	}

	public List<ShopItemTemplet> GetCurrentProductList()
	{
		return m_lstCurrentProductList;
	}

	private void SortDataList(ref List<ShopItemTemplet> lstMiscTemplet, List<eSortOption> lstSortOption)
	{
		NKCUnitSortSystem.NKCDataComparerer<ShopItemTemplet> nKCDataComparerer = new NKCUnitSortSystem.NKCDataComparerer<ShopItemTemplet>();
		HashSet<eSortCategory> hashSet = new HashSet<eSortCategory>();
		if (m_Options.PreemptiveSortFunc != null)
		{
			nKCDataComparerer.AddFunc(m_Options.PreemptiveSortFunc);
		}
		foreach (eSortOption item in lstSortOption)
		{
			if (item != eSortOption.None)
			{
				NKCUnitSortSystem.NKCDataComparerer<ShopItemTemplet>.CompareFunc dataComparer = GetDataComparer(item);
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
		if (!hashSet.Contains(eSortCategory.Default))
		{
			nKCDataComparerer.AddFunc(CompareByDefaultAscending);
		}
		if (nKCDataComparerer.GetComparerCount() > 0)
		{
			lstMiscTemplet.Sort(nKCDataComparerer);
		}
	}

	private NKCUnitSortSystem.NKCDataComparerer<ShopItemTemplet>.CompareFunc GetDataComparer(eSortOption sortOption)
	{
		switch (sortOption)
		{
		default:
			return CompareByDefaultAscending;
		case eSortOption.Default_Last:
			return CompareByDefaultDescending;
		case eSortOption.Rarity_High:
			return CompareByRarityDescending;
		case eSortOption.Rarity_Low:
			return CompareByRarityAscending;
		case eSortOption.ProductID_First:
			return CompareByProductIDAscending;
		case eSortOption.ProductID_Last:
			return CompareByProductIDDescending;
		case eSortOption.ItemID_First:
			return CompareByItemIDAscending;
		case eSortOption.ItemID_Last:
			return CompareByItemIDDescending;
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
				return (ShopItemTemplet a, ShopItemTemplet b) => m_Options.lstCustomSortFunc[GetSortCategoryFromOption(sortOption)].Value(b, a);
			}
			return null;
		}
	}

	public static int CompareByRarityAscending(ShopItemTemplet lhs, ShopItemTemplet rhs)
	{
		NKM_ITEM_GRADE grade = NKCShopManager.GetGrade(lhs);
		NKM_ITEM_GRADE grade2 = NKCShopManager.GetGrade(rhs);
		return grade.CompareTo(grade2);
	}

	public static int CompareByRarityDescending(ShopItemTemplet lhs, ShopItemTemplet rhs)
	{
		NKM_ITEM_GRADE grade = NKCShopManager.GetGrade(lhs);
		return NKCShopManager.GetGrade(rhs).CompareTo(grade);
	}

	private int CompareByProductIDAscending(ShopItemTemplet lhs, ShopItemTemplet rhs)
	{
		return lhs.m_ProductID.CompareTo(rhs.Key);
	}

	private int CompareByProductIDDescending(ShopItemTemplet lhs, ShopItemTemplet rhs)
	{
		return rhs.m_ProductID.CompareTo(lhs.Key);
	}

	private int CompareByItemIDAscending(ShopItemTemplet lhs, ShopItemTemplet rhs)
	{
		return lhs.m_ItemID.CompareTo(rhs.Key);
	}

	private int CompareByItemIDDescending(ShopItemTemplet lhs, ShopItemTemplet rhs)
	{
		return rhs.m_ItemID.CompareTo(lhs.Key);
	}

	private int CompareByDefaultAscending(ShopItemTemplet lhs, ShopItemTemplet rhs)
	{
		return lhs.m_OrderList.CompareTo(rhs.m_OrderList);
	}

	private int CompareByDefaultDescending(ShopItemTemplet lhs, ShopItemTemplet rhs)
	{
		return rhs.m_OrderList.CompareTo(lhs.m_OrderList);
	}

	public static string GetSortName(eSortOption sortOption)
	{
		return GetSortName(GetSortCategoryFromOption(sortOption));
	}

	public static string GetSortName(eSortCategory sortCategory)
	{
		return "";
	}
}
