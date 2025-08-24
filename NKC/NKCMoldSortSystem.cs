using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;

namespace NKC;

public class NKCMoldSortSystem
{
	public enum eSortOption
	{
		None,
		Craftable_High,
		Craftable_Low,
		Tier_High,
		Tier_Low,
		Rarity_High,
		Rarity_Low,
		HaveMold_First,
		HaveMold_Last,
		EquipType_FIrst,
		EquipType_Last,
		UnitType_First,
		UnitType_Last,
		ID_First,
		ID_Last
	}

	public enum eFilterOption
	{
		Mold_Parts_All,
		Mold_Parts_Weapon,
		Mold_Parts_Defence,
		Mold_Parts_Acc,
		Mold_Tier_1,
		Mold_Tier_2,
		Mold_Tier_3,
		Mold_Tier_4,
		Mold_Tier_5,
		Mold_Tier_6,
		Mold_Tier_7,
		Mold_Type_Normal,
		Mold_Type_Raid,
		Mold_Type_Etc,
		Mold_Status_Enable,
		Mold_Status_Disable,
		Mold_Unit_Counter,
		Mold_Unit_Soldier,
		Mold_Unit_Mechanic,
		Mold_Unit_Etc,
		Mold_Grade_SSR,
		Mold_Grade_SR,
		Mold_Grade_R,
		Mold_Grade_N
	}

	public struct MoldListOptions
	{
		public HashSet<eFilterOption> setFilterOption;

		public List<eSortOption> lstSortOption;
	}

	private static Dictionary<string, List<eSortOption>> m_dicMoldSort = new Dictionary<string, List<eSortOption>>
	{
		{
			"ST_Makeable",
			new List<eSortOption>
			{
				eSortOption.HaveMold_First,
				eSortOption.Craftable_High,
				eSortOption.Tier_High,
				eSortOption.Rarity_High,
				eSortOption.UnitType_First,
				eSortOption.EquipType_FIrst,
				eSortOption.ID_First
			}
		},
		{
			"ST_Makeable_ASC",
			new List<eSortOption>
			{
				eSortOption.HaveMold_Last,
				eSortOption.Craftable_Low,
				eSortOption.Tier_Low,
				eSortOption.Rarity_Low,
				eSortOption.UnitType_Last,
				eSortOption.EquipType_Last,
				eSortOption.ID_Last
			}
		},
		{
			"ST_Tier",
			new List<eSortOption>
			{
				eSortOption.HaveMold_First,
				eSortOption.Tier_High,
				eSortOption.Rarity_High,
				eSortOption.UnitType_First,
				eSortOption.EquipType_FIrst,
				eSortOption.ID_First
			}
		},
		{
			"ST_Tier_ASC",
			new List<eSortOption>
			{
				eSortOption.HaveMold_Last,
				eSortOption.Tier_Low,
				eSortOption.Rarity_Low,
				eSortOption.UnitType_Last,
				eSortOption.EquipType_Last,
				eSortOption.ID_Last
			}
		},
		{
			"ST_Grade",
			new List<eSortOption>
			{
				eSortOption.HaveMold_First,
				eSortOption.Rarity_High,
				eSortOption.Tier_High,
				eSortOption.UnitType_First,
				eSortOption.EquipType_FIrst,
				eSortOption.ID_First
			}
		},
		{
			"ST_Grade_ASC",
			new List<eSortOption>
			{
				eSortOption.HaveMold_Last,
				eSortOption.Rarity_Low,
				eSortOption.Tier_Low,
				eSortOption.UnitType_Last,
				eSortOption.EquipType_Last,
				eSortOption.ID_Last
			}
		}
	};

	public const string SORT_MAKEABLE_DESC = "ST_Makeable";

	private const string SORT_MAKEABLE_ASC = "ST_Makeable_ASC";

	public const string SORT_TIER_DESC = "ST_Tier";

	private const string SORT_TIER_ASC = "ST_Tier_ASC";

	public const string SORT_GRADE_DESC = "ST_Grade";

	private const string SORT_GRADE_ASC = "ST_Grade_ASC";

	public static readonly List<eSortOption> EQUIP_CRAFTABLE_SORT_LIST_DESC = new List<eSortOption>
	{
		eSortOption.HaveMold_First,
		eSortOption.Craftable_High,
		eSortOption.Tier_High,
		eSortOption.Rarity_High,
		eSortOption.EquipType_FIrst
	};

	public static List<string> m_lstMoldFilter = new List<string> { "FT_EquipPosition", "FT_Tier", "FT_ContentType", "FT_Makeable", "FT_UnitType", "FT_Grade", "FT_Makeable" };

	protected MoldListOptions m_Options;

	private List<NKMMoldItemData> m_lstNKMMoldItemData = new List<NKMMoldItemData>();

	private bool m_bDescendingOrder = true;

	public static Dictionary<string, List<eSortOption>> MoldSortData => m_dicMoldSort;

	public static List<string> MoldFilterData => m_lstMoldFilter;

	public HashSet<eFilterOption> FilterSet
	{
		get
		{
			if (m_Options.setFilterOption == null)
			{
				m_Options.setFilterOption = new HashSet<eFilterOption>();
			}
			return m_Options.setFilterOption;
		}
	}

	public List<eSortOption> lstSortOption => m_Options.lstSortOption;

	public List<NKMMoldItemData> lstSortedList => m_lstNKMMoldItemData;

	private NKCMoldSortSystem()
	{
	}

	public NKCMoldSortSystem(List<eSortOption> lstMoldOption)
	{
		if (lstMoldOption != null)
		{
			m_Options.lstSortOption = lstMoldOption;
		}
		else
		{
			m_Options.lstSortOption = GetDefaultSortOption();
		}
	}

	public void FilterList(HashSet<eFilterOption> setFilterOption, NKM_CRAFT_TAB_TYPE type)
	{
		if (setFilterOption != null)
		{
			m_Options.setFilterOption = setFilterOption;
			Filtering(type);
			Sort(GetCurActiveOption());
		}
	}

	private NKCUnitSortSystem.NKCDataComparerer<NKMMoldItemData>.CompareFunc GetDataComparer(eSortOption sortOption)
	{
		return sortOption switch
		{
			eSortOption.Craftable_High => CompareByCraftbaleDescending, 
			eSortOption.Craftable_Low => CompareByCraftbaleAscending, 
			eSortOption.Tier_High => CompareByTierDescending, 
			eSortOption.Tier_Low => CompareByTierAscending, 
			eSortOption.Rarity_High => CompareByRarityDescending, 
			eSortOption.Rarity_Low => CompareByRarityAscending, 
			eSortOption.EquipType_FIrst => CompareByEquipTypeDescending, 
			eSortOption.EquipType_Last => CompareByEquipTypeAscending, 
			eSortOption.HaveMold_First => CompareByHaveFirst, 
			eSortOption.HaveMold_Last => CompareByHaveLast, 
			eSortOption.UnitType_First => CompareByUnitTypeFirst, 
			eSortOption.UnitType_Last => CompareByUnitTypeLast, 
			eSortOption.ID_First => CompareByIDFirst, 
			eSortOption.ID_Last => CompareByIDLast, 
			_ => null, 
		};
	}

	private int CompareByCraftbaleDescending(NKMMoldItemData lhs, NKMMoldItemData rhs)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (NKCUtil.GetEquipCreatableCount(lhs, myUserData.m_InventoryData) > 0 && NKCUtil.GetEquipCreatableCount(rhs, myUserData.m_InventoryData) <= 0)
		{
			return -1;
		}
		if (NKCUtil.GetEquipCreatableCount(lhs, myUserData.m_InventoryData) <= 0 && NKCUtil.GetEquipCreatableCount(rhs, myUserData.m_InventoryData) > 0)
		{
			return 1;
		}
		return 0;
	}

	private int CompareByCraftbaleAscending(NKMMoldItemData lhs, NKMMoldItemData rhs)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (NKCUtil.GetEquipCreatableCount(lhs, myUserData.m_InventoryData) > 0 && NKCUtil.GetEquipCreatableCount(rhs, myUserData.m_InventoryData) <= 0)
		{
			return 1;
		}
		if (NKCUtil.GetEquipCreatableCount(lhs, myUserData.m_InventoryData) <= 0 && NKCUtil.GetEquipCreatableCount(rhs, myUserData.m_InventoryData) > 0)
		{
			return -1;
		}
		return 0;
	}

	private int CompareByTierAscending(NKMMoldItemData lhs, NKMMoldItemData rhs)
	{
		NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(lhs.m_MoldID);
		NKMItemMoldTemplet itemMoldTempletByID2 = NKMItemManager.GetItemMoldTempletByID(rhs.m_MoldID);
		if (itemMoldTempletByID == null || itemMoldTempletByID2 == null)
		{
			return 0;
		}
		return itemMoldTempletByID.m_Tier.CompareTo(itemMoldTempletByID2.m_Tier);
	}

	private int CompareByTierDescending(NKMMoldItemData lhs, NKMMoldItemData rhs)
	{
		NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(lhs.m_MoldID);
		NKMItemMoldTemplet itemMoldTempletByID2 = NKMItemManager.GetItemMoldTempletByID(rhs.m_MoldID);
		if (itemMoldTempletByID == null || itemMoldTempletByID2 == null)
		{
			return 0;
		}
		return itemMoldTempletByID2.m_Tier.CompareTo(itemMoldTempletByID.m_Tier);
	}

	private int CompareByRarityAscending(NKMMoldItemData lhs, NKMMoldItemData rhs)
	{
		NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(lhs.m_MoldID);
		NKMItemMoldTemplet itemMoldTempletByID2 = NKMItemManager.GetItemMoldTempletByID(rhs.m_MoldID);
		if (itemMoldTempletByID == null || itemMoldTempletByID2 == null)
		{
			return 0;
		}
		return itemMoldTempletByID.m_Grade.CompareTo(itemMoldTempletByID2.m_Grade);
	}

	private int CompareByRarityDescending(NKMMoldItemData lhs, NKMMoldItemData rhs)
	{
		NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(lhs.m_MoldID);
		NKMItemMoldTemplet itemMoldTempletByID2 = NKMItemManager.GetItemMoldTempletByID(rhs.m_MoldID);
		if (itemMoldTempletByID == null || itemMoldTempletByID2 == null)
		{
			return 0;
		}
		return itemMoldTempletByID2.m_Grade.CompareTo(itemMoldTempletByID.m_Grade);
	}

	private int CompareByEquipTypeAscending(NKMMoldItemData lhs, NKMMoldItemData rhs)
	{
		NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(lhs.m_MoldID);
		NKMItemMoldTemplet itemMoldTempletByID2 = NKMItemManager.GetItemMoldTempletByID(rhs.m_MoldID);
		if (itemMoldTempletByID == null || itemMoldTempletByID2 == null)
		{
			return 0;
		}
		return itemMoldTempletByID2.m_RewardEquipPosition.CompareTo(itemMoldTempletByID.m_RewardEquipPosition);
	}

	private int CompareByEquipTypeDescending(NKMMoldItemData lhs, NKMMoldItemData rhs)
	{
		NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(lhs.m_MoldID);
		NKMItemMoldTemplet itemMoldTempletByID2 = NKMItemManager.GetItemMoldTempletByID(rhs.m_MoldID);
		if (itemMoldTempletByID == null || itemMoldTempletByID2 == null)
		{
			return 0;
		}
		return itemMoldTempletByID.m_RewardEquipPosition.CompareTo(itemMoldTempletByID2.m_RewardEquipPosition);
	}

	private int CompareByHaveFirst(NKMMoldItemData lhs, NKMMoldItemData rhs)
	{
		NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(lhs.m_MoldID);
		NKMItemMoldTemplet itemMoldTempletByID2 = NKMItemManager.GetItemMoldTempletByID(rhs.m_MoldID);
		if (itemMoldTempletByID != null && itemMoldTempletByID.m_bPermanent && itemMoldTempletByID2 != null && itemMoldTempletByID2.m_bPermanent)
		{
			return 0;
		}
		if (lhs.m_Count > 0 && rhs.m_Count <= 0)
		{
			return -1;
		}
		if (rhs.m_Count > 0 && lhs.m_Count <= 0)
		{
			return 1;
		}
		return 0;
	}

	private int CompareByHaveLast(NKMMoldItemData lhs, NKMMoldItemData rhs)
	{
		NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(lhs.m_MoldID);
		NKMItemMoldTemplet itemMoldTempletByID2 = NKMItemManager.GetItemMoldTempletByID(rhs.m_MoldID);
		if (itemMoldTempletByID != null && itemMoldTempletByID.m_bPermanent && itemMoldTempletByID2 != null && itemMoldTempletByID2.m_bPermanent)
		{
			return 0;
		}
		if (lhs.m_Count > 0 && rhs.m_Count <= 0)
		{
			return 1;
		}
		if (rhs.m_Count > 0 && lhs.m_Count <= 0)
		{
			return -1;
		}
		return 0;
	}

	private int CompareByUnitTypeFirst(NKMMoldItemData lhs, NKMMoldItemData rhs)
	{
		NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(lhs.m_MoldID);
		NKMItemMoldTemplet itemMoldTempletByID2 = NKMItemManager.GetItemMoldTempletByID(rhs.m_MoldID);
		if (itemMoldTempletByID == null || itemMoldTempletByID2 == null)
		{
			return 0;
		}
		return itemMoldTempletByID.m_RewardEquipUnitType.CompareTo(itemMoldTempletByID2.m_RewardEquipUnitType);
	}

	private int CompareByUnitTypeLast(NKMMoldItemData lhs, NKMMoldItemData rhs)
	{
		NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(lhs.m_MoldID);
		NKMItemMoldTemplet itemMoldTempletByID2 = NKMItemManager.GetItemMoldTempletByID(rhs.m_MoldID);
		if (itemMoldTempletByID == null || itemMoldTempletByID2 == null)
		{
			return 0;
		}
		return itemMoldTempletByID2.m_RewardEquipUnitType.CompareTo(itemMoldTempletByID.m_RewardEquipUnitType);
	}

	private int CompareByIDFirst(NKMMoldItemData lhs, NKMMoldItemData rhs)
	{
		return lhs.m_MoldID.CompareTo(rhs.m_MoldID);
	}

	private int CompareByIDLast(NKMMoldItemData lhs, NKMMoldItemData rhs)
	{
		return rhs.m_MoldID.CompareTo(lhs.m_MoldID);
	}

	public string GetSortName(eSortOption option)
	{
		switch (option)
		{
		default:
			return NKCUtilString.GET_STRING_SORT_CRAFTABLE;
		case eSortOption.Tier_High:
		case eSortOption.Tier_Low:
			return NKCUtilString.GET_STRING_SORT_TIER;
		case eSortOption.Rarity_High:
		case eSortOption.Rarity_Low:
			return NKCUtilString.GET_STRING_SORT_RARITY;
		}
	}

	public string GetSortName()
	{
		return GetSortName(GetCurActiveOption());
	}

	public eSortOption GetCurActiveOption()
	{
		if (m_Options.lstSortOption.Count > 0)
		{
			return m_Options.lstSortOption[1];
		}
		return eSortOption.None;
	}

	public static bool GetDescendingBySorting(List<eSortOption> lstSortOption)
	{
		if (lstSortOption.Count > 0)
		{
			return GetDescendingBySorting(lstSortOption[0]);
		}
		return true;
	}

	private static bool GetDescendingBySorting(eSortOption sortOption)
	{
		switch (sortOption)
		{
		default:
			return true;
		case eSortOption.Craftable_Low:
		case eSortOption.Tier_Low:
		case eSortOption.Rarity_Low:
		case eSortOption.EquipType_Last:
			return false;
		}
	}

	public static NKMMoldItemData MakeTempMoldData(int moldID, int level = 1)
	{
		if (NKMItemManager.GetItemMoldTempletByID(moldID) != null)
		{
			return new NKMMoldItemData(moldID, 1L);
		}
		return new NKMMoldItemData();
	}

	public static List<eSortOption> GetDefaultSortOption(string sortKey)
	{
		if (m_dicMoldSort.ContainsKey(sortKey))
		{
			return m_dicMoldSort[sortKey];
		}
		return null;
	}

	public static List<eSortOption> GetDefaultSortOption()
	{
		return EQUIP_CRAFTABLE_SORT_LIST_DESC;
	}

	public void Update(NKM_CRAFT_TAB_TYPE type)
	{
		Filtering(type);
		SortMoldDataList();
	}

	private void Filtering(NKM_CRAFT_TAB_TYPE type)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		m_lstNKMMoldItemData = NKMItemManager.GetMoldItemData(type);
		for (int i = 0; i < m_lstNKMMoldItemData.Count; i++)
		{
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(m_lstNKMMoldItemData[i].m_MoldID);
			if (itemMoldTempletByID == null)
			{
				m_lstNKMMoldItemData.RemoveAt(i);
				i--;
			}
			else if (!itemMoldTempletByID.EnableByTag)
			{
				m_lstNKMMoldItemData.RemoveAt(i);
				i--;
			}
			else if (!itemMoldTempletByID.m_bPermanent && nKMUserData.m_CraftData.GetMoldItemDataByID(itemMoldTempletByID.m_MoldID) == null)
			{
				m_lstNKMMoldItemData.RemoveAt(i);
				i--;
			}
		}
		HashSet<eFilterOption> setFilterOption = m_Options.setFilterOption;
		if (setFilterOption == null || setFilterOption.Count <= 0)
		{
			return;
		}
		bool flag = setFilterOption.Contains(eFilterOption.Mold_Parts_All);
		bool flag2 = setFilterOption.Contains(eFilterOption.Mold_Parts_Weapon);
		bool flag3 = setFilterOption.Contains(eFilterOption.Mold_Parts_Defence);
		bool flag4 = setFilterOption.Contains(eFilterOption.Mold_Parts_Acc);
		bool flag5 = setFilterOption.Contains(eFilterOption.Mold_Tier_1);
		bool flag6 = setFilterOption.Contains(eFilterOption.Mold_Tier_2);
		bool flag7 = setFilterOption.Contains(eFilterOption.Mold_Tier_3);
		bool flag8 = setFilterOption.Contains(eFilterOption.Mold_Tier_4);
		bool flag9 = setFilterOption.Contains(eFilterOption.Mold_Tier_5);
		bool flag10 = setFilterOption.Contains(eFilterOption.Mold_Tier_6);
		bool flag11 = setFilterOption.Contains(eFilterOption.Mold_Tier_7);
		bool flag12 = setFilterOption.Contains(eFilterOption.Mold_Type_Normal);
		bool flag13 = setFilterOption.Contains(eFilterOption.Mold_Type_Raid);
		bool flag14 = setFilterOption.Contains(eFilterOption.Mold_Type_Etc);
		bool flag15 = setFilterOption.Contains(eFilterOption.Mold_Status_Enable);
		bool flag16 = setFilterOption.Contains(eFilterOption.Mold_Status_Disable);
		bool flag17 = setFilterOption.Contains(eFilterOption.Mold_Unit_Counter);
		bool flag18 = setFilterOption.Contains(eFilterOption.Mold_Unit_Soldier);
		bool flag19 = setFilterOption.Contains(eFilterOption.Mold_Unit_Mechanic);
		bool flag20 = setFilterOption.Contains(eFilterOption.Mold_Unit_Etc);
		bool flag21 = setFilterOption.Contains(eFilterOption.Mold_Grade_SSR);
		bool flag22 = setFilterOption.Contains(eFilterOption.Mold_Grade_SR);
		bool flag23 = setFilterOption.Contains(eFilterOption.Mold_Grade_R);
		bool flag24 = setFilterOption.Contains(eFilterOption.Mold_Grade_N);
		for (int j = 0; j < m_lstNKMMoldItemData.Count; j++)
		{
			NKMItemMoldTemplet itemMoldTempletByID2 = NKMItemManager.GetItemMoldTempletByID(m_lstNKMMoldItemData[j].m_MoldID);
			if (itemMoldTempletByID2 == null)
			{
				continue;
			}
			if ((flag || flag2 || flag3 || flag4) && (0u | ((flag && itemMoldTempletByID2.m_RewardEquipPosition == ITEM_EQUIP_POSITION.IEP_MAX) ? 1u : 0u) | ((flag2 && itemMoldTempletByID2.m_RewardEquipPosition == ITEM_EQUIP_POSITION.IEP_WEAPON) ? 1u : 0u) | ((flag3 && itemMoldTempletByID2.m_RewardEquipPosition == ITEM_EQUIP_POSITION.IEP_DEFENCE) ? 1u : 0u) | ((flag4 && (itemMoldTempletByID2.m_RewardEquipPosition == ITEM_EQUIP_POSITION.IEP_ACC || itemMoldTempletByID2.m_RewardEquipPosition == ITEM_EQUIP_POSITION.IEP_ACC2)) ? 1u : 0u)) == 0)
			{
				m_lstNKMMoldItemData.RemoveAt(j);
				j--;
				continue;
			}
			if ((flag12 || flag13 || flag14) && (0u | ((flag12 && itemMoldTempletByID2.m_ContentType == NKM_ITEM_DROP_POSITION.NORMAL) ? 1u : 0u) | ((flag13 && itemMoldTempletByID2.m_ContentType == NKM_ITEM_DROP_POSITION.RAID) ? 1u : 0u) | ((flag14 && itemMoldTempletByID2.m_ContentType == NKM_ITEM_DROP_POSITION.ETC) ? 1u : 0u)) == 0)
			{
				m_lstNKMMoldItemData.RemoveAt(j);
				j--;
				continue;
			}
			if ((flag5 || flag6 || flag7 || flag8 || flag9 || flag10 || flag11) && (0u | ((flag5 && itemMoldTempletByID2.m_Tier == 1) ? 1u : 0u) | ((flag6 && itemMoldTempletByID2.m_Tier == 2) ? 1u : 0u) | ((flag7 && itemMoldTempletByID2.m_Tier == 3) ? 1u : 0u) | ((flag8 && itemMoldTempletByID2.m_Tier == 4) ? 1u : 0u) | ((flag9 && itemMoldTempletByID2.m_Tier == 5) ? 1u : 0u) | ((flag10 && itemMoldTempletByID2.m_Tier == 6) ? 1u : 0u) | ((flag11 && itemMoldTempletByID2.m_Tier == 7) ? 1u : 0u)) == 0)
			{
				m_lstNKMMoldItemData.RemoveAt(j);
				j--;
				continue;
			}
			if (flag15)
			{
				if (NKCUtil.GetEquipCreatableCount(m_lstNKMMoldItemData[j], nKMUserData.m_InventoryData) <= 0)
				{
					m_lstNKMMoldItemData.RemoveAt(j);
					j--;
					continue;
				}
			}
			else if (flag16 && NKCUtil.GetEquipCreatableCount(m_lstNKMMoldItemData[j], nKMUserData.m_InventoryData) > 0)
			{
				m_lstNKMMoldItemData.RemoveAt(j);
				j--;
				continue;
			}
			if ((flag17 || flag18 || flag19 || flag20) && (0u | ((flag17 && itemMoldTempletByID2.m_RewardEquipUnitType == NKM_UNIT_STYLE_TYPE.NUST_COUNTER) ? 1u : 0u) | ((flag18 && itemMoldTempletByID2.m_RewardEquipUnitType == NKM_UNIT_STYLE_TYPE.NUST_SOLDIER) ? 1u : 0u) | ((flag19 && itemMoldTempletByID2.m_RewardEquipUnitType == NKM_UNIT_STYLE_TYPE.NUST_MECHANIC) ? 1u : 0u) | ((flag20 && itemMoldTempletByID2.m_RewardEquipUnitType == NKM_UNIT_STYLE_TYPE.NUST_ETC) ? 1u : 0u)) == 0)
			{
				m_lstNKMMoldItemData.RemoveAt(j);
				j--;
			}
			else if ((flag24 || flag23 || flag22 || flag21) && (0u | ((flag21 && itemMoldTempletByID2.m_Grade == NKM_ITEM_GRADE.NIG_SSR) ? 1u : 0u) | ((flag22 && itemMoldTempletByID2.m_Grade == NKM_ITEM_GRADE.NIG_SR) ? 1u : 0u) | ((flag23 && itemMoldTempletByID2.m_Grade == NKM_ITEM_GRADE.NIG_R) ? 1u : 0u) | ((flag24 && itemMoldTempletByID2.m_Grade == NKM_ITEM_GRADE.NIG_N) ? 1u : 0u)) == 0)
			{
				m_lstNKMMoldItemData.RemoveAt(j);
				j--;
			}
		}
	}

	public bool Sort(eSortOption selectedSortOption)
	{
		if (GetDescendingBySorting(selectedSortOption) != m_bDescendingOrder)
		{
			selectedSortOption = ChangeSortOption(selectedSortOption);
		}
		List<eSortOption> sortOption = GetSortOption(selectedSortOption);
		if (sortOption.Count > 0)
		{
			m_Options.lstSortOption = sortOption;
			SortMoldDataList();
		}
		return true;
	}

	private eSortOption ChangeSortOption(eSortOption option)
	{
		return option switch
		{
			eSortOption.Craftable_High => eSortOption.Craftable_Low, 
			eSortOption.Craftable_Low => eSortOption.Craftable_High, 
			eSortOption.Rarity_High => eSortOption.Rarity_Low, 
			eSortOption.Rarity_Low => eSortOption.Rarity_High, 
			eSortOption.Tier_High => eSortOption.Tier_Low, 
			eSortOption.Tier_Low => eSortOption.Tier_High, 
			_ => eSortOption.None, 
		};
	}

	private List<eSortOption> GetSortOption(eSortOption targetOption)
	{
		string sortKey = GetSortKey(targetOption);
		if (string.IsNullOrEmpty(sortKey))
		{
			Debug.LogError("허용되는 키가 없습니다! 확인해주세요!");
		}
		else if (m_dicMoldSort.ContainsKey(sortKey))
		{
			return m_dicMoldSort[sortKey];
		}
		return new List<eSortOption>();
	}

	private string GetSortKey(eSortOption targetOption)
	{
		return targetOption switch
		{
			eSortOption.Craftable_High => "ST_Makeable", 
			eSortOption.Craftable_Low => "ST_Makeable_ASC", 
			eSortOption.Tier_High => "ST_Tier", 
			eSortOption.Tier_Low => "ST_Tier_ASC", 
			eSortOption.Rarity_High => "ST_Grade", 
			eSortOption.Rarity_Low => "ST_Grade_ASC", 
			_ => null, 
		};
	}

	private void SortMoldDataList()
	{
		if (m_lstNKMMoldItemData.Count < 2)
		{
			return;
		}
		NKCUnitSortSystem.NKCDataComparerer<NKMMoldItemData> nKCDataComparerer = new NKCUnitSortSystem.NKCDataComparerer<NKMMoldItemData>();
		foreach (eSortOption item in m_Options.lstSortOption)
		{
			nKCDataComparerer.AddFunc(GetDataComparer(item));
		}
		m_lstNKMMoldItemData.Sort(nKCDataComparerer);
	}

	public void OnCheckAscend(bool bAscend, UnityAction sortEndCallBack = null)
	{
		eSortOption selectedSortOption = eSortOption.Craftable_High;
		switch (GetCurActiveOption())
		{
		case eSortOption.Craftable_High:
		case eSortOption.Craftable_Low:
			selectedSortOption = ((!bAscend) ? eSortOption.Craftable_High : eSortOption.Craftable_Low);
			break;
		case eSortOption.Tier_High:
		case eSortOption.Tier_Low:
			selectedSortOption = (bAscend ? eSortOption.Tier_Low : eSortOption.Tier_High);
			break;
		case eSortOption.Rarity_High:
		case eSortOption.Rarity_Low:
			selectedSortOption = (bAscend ? eSortOption.Rarity_Low : eSortOption.Rarity_High);
			break;
		}
		m_bDescendingOrder = !bAscend;
		Sort(selectedSortOption);
		sortEndCallBack?.Invoke();
	}
}
