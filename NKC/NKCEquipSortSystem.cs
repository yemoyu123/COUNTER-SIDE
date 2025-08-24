using System;
using System.Collections.Generic;
using NKC.Sort;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public class NKCEquipSortSystem
{
	public enum eSortCategory
	{
		None,
		Enhance,
		Tier,
		Rarity,
		UID,
		OperationPower,
		OptionWeightByClassType,
		Equipped,
		UnitType,
		EquipType,
		ID,
		Craftable,
		SetOption,
		Custom1,
		Custom2,
		Custom3
	}

	public enum eSortOption
	{
		Enhance_High,
		Enhance_Low,
		Tier_High,
		Tier_Low,
		Rarity_High,
		Rarity_Low,
		UID_First,
		UID_Last,
		SetOption_High,
		SetOption_Low,
		OperationPower_High,
		OperationPower_Low,
		OptionWeightByClassType_High,
		OptionWeightByClassType_Low,
		Equipped_First,
		Equipped_Last,
		UnitType_First,
		UnitType_Last,
		EquipType_FIrst,
		EquipType_Last,
		ID_First,
		ID_Last,
		PrivateEquip_First,
		PrivateEquip_Last,
		Craftable_First,
		Craftable_Last,
		CustomAscend1,
		CustomDescend1,
		CustomAscend2,
		CustomDescend2,
		CustomAscend3,
		CustomDescend3,
		None
	}

	public enum eFilterCategory
	{
		UnitType,
		EquipType,
		Tier,
		Rarity,
		Equipped,
		Locked,
		Have,
		SetOptionPart,
		SetOptionType,
		Upgrade,
		StatType,
		PrivateEquip
	}

	public enum eFilterOption
	{
		Nothing,
		All,
		Equip_Counter,
		Equip_Soldier,
		Equip_Mechanic,
		Equip_Weapon,
		Equip_Armor,
		Equip_Acc,
		Equip_Enchant,
		Equip_Tier_7,
		Equip_Tier_6,
		Equip_Tier_5,
		Equip_Tier_4,
		Equip_Tier_3,
		Equip_Tier_2,
		Equip_Tier_1,
		Equip_Rarity_SSR,
		Equip_Rarity_SR,
		Equip_Rarity_R,
		Equip_Rarity_N,
		Equip_Set_Part_2,
		Equip_Set_Part_3,
		Equip_Set_Part_4,
		Equip_Set_Effect_Red,
		Equip_Set_Effect_Blue,
		Equip_Set_Effect_Yellow,
		Equip_Equipped,
		Equip_Unused,
		Equip_Locked,
		Equip_Unlocked,
		Equip_Have,
		Equip_NotHave,
		Equip_CanUpgrade,
		Equip_CannotUpgrade,
		Equip_Stat_01,
		Equip_Stat_02,
		Equip_Stat_Potential,
		Equip_Stat_SetOption,
		Equip_Private,
		Equip_Private_Awaken,
		Equip_Non_Private,
		Equip_Relic
	}

	public struct EquipListOptions
	{
		public delegate bool CustomFilterFunc(NKMEquipItemData equipData);

		public HashSet<int> setOnlyIncludeEquipID;

		public HashSet<int> setExcludeEquipID;

		public HashSet<long> setExcludeEquipUID;

		public HashSet<eFilterOption> setExcludeFilterOption;

		public HashSet<eFilterOption> setFilterOption;

		public List<eSortOption> lstSortOption;

		public NKCUnitSortSystem.NKCDataComparerer<NKMEquipItemData>.CompareFunc PreemptiveSortFunc;

		public CustomFilterFunc AdditionalExcludeFilterFunc;

		public Dictionary<eSortCategory, KeyValuePair<string, NKCUnitSortSystem.NKCDataComparerer<NKMEquipItemData>.CompareFunc>> lstCustomSortFunc;

		public bool bHideEquippedItem;

		public bool bPushBackUnselectable;

		public bool bHideLockItem;

		public bool bHideMaxLvItem;

		public bool bLockMaxItem;

		public int OwnerUnitID;

		public bool bHideNotPossibleSetOptionItem;

		public int iTargetUnitID;

		public NKM_STAT_TYPE FilterStatType_01;

		public NKM_STAT_TYPE FilterStatType_02;

		public NKM_STAT_TYPE FilterStatType_Potential;

		public int FilterSetOptionID;

		public bool bHideTokenFiltering;
	}

	public delegate bool AutoSelectExtraFilter(NKMEquipItemData equipData);

	private const int ITEM_TIER_7 = 7;

	private const int ITEM_TIER_6 = 6;

	private const int ITEM_TIER_5 = 5;

	private const int ITEM_TIER_4 = 4;

	private const int ITEM_TIER_3 = 3;

	private const int ITEM_TIER_2 = 2;

	private const int ITEM_TIER_1 = 1;

	private static readonly Dictionary<eSortCategory, Tuple<eSortOption, eSortOption>> s_dicSortCategory = new Dictionary<eSortCategory, Tuple<eSortOption, eSortOption>>
	{
		{
			eSortCategory.None,
			new Tuple<eSortOption, eSortOption>(eSortOption.None, eSortOption.None)
		},
		{
			eSortCategory.Enhance,
			new Tuple<eSortOption, eSortOption>(eSortOption.Enhance_Low, eSortOption.Enhance_High)
		},
		{
			eSortCategory.Tier,
			new Tuple<eSortOption, eSortOption>(eSortOption.Tier_Low, eSortOption.Tier_High)
		},
		{
			eSortCategory.Rarity,
			new Tuple<eSortOption, eSortOption>(eSortOption.Rarity_Low, eSortOption.Rarity_High)
		},
		{
			eSortCategory.UID,
			new Tuple<eSortOption, eSortOption>(eSortOption.UID_First, eSortOption.UID_Last)
		},
		{
			eSortCategory.OperationPower,
			new Tuple<eSortOption, eSortOption>(eSortOption.OperationPower_Low, eSortOption.OperationPower_High)
		},
		{
			eSortCategory.OptionWeightByClassType,
			new Tuple<eSortOption, eSortOption>(eSortOption.OptionWeightByClassType_Low, eSortOption.OptionWeightByClassType_High)
		},
		{
			eSortCategory.Equipped,
			new Tuple<eSortOption, eSortOption>(eSortOption.Equipped_First, eSortOption.Equipped_Last)
		},
		{
			eSortCategory.UnitType,
			new Tuple<eSortOption, eSortOption>(eSortOption.UnitType_First, eSortOption.UnitType_Last)
		},
		{
			eSortCategory.EquipType,
			new Tuple<eSortOption, eSortOption>(eSortOption.EquipType_FIrst, eSortOption.EquipType_Last)
		},
		{
			eSortCategory.ID,
			new Tuple<eSortOption, eSortOption>(eSortOption.ID_First, eSortOption.ID_Last)
		},
		{
			eSortCategory.Craftable,
			new Tuple<eSortOption, eSortOption>(eSortOption.Craftable_First, eSortOption.Craftable_Last)
		},
		{
			eSortCategory.SetOption,
			new Tuple<eSortOption, eSortOption>(eSortOption.SetOption_Low, eSortOption.SetOption_High)
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

	public static readonly List<eSortOption> DEFAULT_EQUIP_SORT_LIST = new List<eSortOption>
	{
		eSortOption.Equipped_First,
		eSortOption.Enhance_High,
		eSortOption.Tier_High,
		eSortOption.Rarity_High,
		eSortOption.UnitType_First,
		eSortOption.EquipType_FIrst,
		eSortOption.ID_First,
		eSortOption.UID_First,
		eSortOption.SetOption_High
	};

	public static readonly List<eSortOption> DEFAULT_EQUIP_SELECTION_SORT_LIST = new List<eSortOption>
	{
		eSortOption.Tier_High,
		eSortOption.Rarity_High
	};

	public static readonly List<eSortOption> DEFAULT_EQUIP_OPERATION_POWER_SORT_LIST = new List<eSortOption>
	{
		eSortOption.OperationPower_High,
		eSortOption.UID_Last
	};

	public static readonly List<eSortOption> DEFAULT_EQUIP_OPTION_WEIGHT_SORT_LIST = new List<eSortOption>
	{
		eSortOption.PrivateEquip_First,
		eSortOption.OptionWeightByClassType_High,
		eSortOption.Tier_High,
		eSortOption.Rarity_High,
		eSortOption.UID_Last
	};

	public static readonly List<eSortOption> FORGE_TARGET_SORT_LIST = new List<eSortOption>
	{
		eSortOption.Enhance_High,
		eSortOption.Equipped_First,
		eSortOption.Tier_High,
		eSortOption.Rarity_High,
		eSortOption.UnitType_First,
		eSortOption.EquipType_FIrst,
		eSortOption.ID_First,
		eSortOption.UID_First
	};

	public static readonly List<eSortOption> FORGE_MATERIAL_SORT_LIST = new List<eSortOption>
	{
		eSortOption.Enhance_Low,
		eSortOption.Rarity_Low,
		eSortOption.Tier_Low,
		eSortOption.UnitType_First,
		eSortOption.EquipType_FIrst,
		eSortOption.ID_First,
		eSortOption.UID_First
	};

	public static readonly List<eSortOption> EQUIP_BREAK_SORT_LIST = new List<eSortOption>
	{
		eSortOption.Enhance_Low,
		eSortOption.Rarity_Low,
		eSortOption.Tier_Low,
		eSortOption.UnitType_First,
		eSortOption.EquipType_FIrst,
		eSortOption.ID_First
	};

	public static readonly List<eSortOption> EQUIP_UPGRADE_SORT_LIST = new List<eSortOption>
	{
		eSortOption.CustomDescend1,
		eSortOption.ID_First
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_EquipUnitType = new HashSet<eFilterOption>
	{
		eFilterOption.Equip_Counter,
		eFilterOption.Equip_Soldier,
		eFilterOption.Equip_Mechanic
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_EquipType = new HashSet<eFilterOption>
	{
		eFilterOption.Equip_Weapon,
		eFilterOption.Equip_Armor,
		eFilterOption.Equip_Acc,
		eFilterOption.Equip_Enchant
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Tier = new HashSet<eFilterOption>
	{
		eFilterOption.Equip_Tier_7,
		eFilterOption.Equip_Tier_6,
		eFilterOption.Equip_Tier_5,
		eFilterOption.Equip_Tier_4,
		eFilterOption.Equip_Tier_3,
		eFilterOption.Equip_Tier_2,
		eFilterOption.Equip_Tier_1
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Rarity = new HashSet<eFilterOption>
	{
		eFilterOption.Equip_Rarity_SSR,
		eFilterOption.Equip_Rarity_SR,
		eFilterOption.Equip_Rarity_R,
		eFilterOption.Equip_Rarity_N
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_SetOptionPart = new HashSet<eFilterOption>
	{
		eFilterOption.Equip_Set_Part_2,
		eFilterOption.Equip_Set_Part_3,
		eFilterOption.Equip_Set_Part_4
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_SetOptionType = new HashSet<eFilterOption>
	{
		eFilterOption.Equip_Set_Effect_Red,
		eFilterOption.Equip_Set_Effect_Blue,
		eFilterOption.Equip_Set_Effect_Yellow
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Equipped = new HashSet<eFilterOption>
	{
		eFilterOption.Equip_Equipped,
		eFilterOption.Equip_Unused
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Locked = new HashSet<eFilterOption>
	{
		eFilterOption.Equip_Locked,
		eFilterOption.Equip_Unlocked
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Have = new HashSet<eFilterOption>
	{
		eFilterOption.Equip_Have,
		eFilterOption.Equip_NotHave
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Upgrade = new HashSet<eFilterOption>
	{
		eFilterOption.Equip_CanUpgrade,
		eFilterOption.Equip_CannotUpgrade
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_StatType = new HashSet<eFilterOption>
	{
		eFilterOption.Equip_Stat_01,
		eFilterOption.Equip_Stat_02
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_StatType_Potential = new HashSet<eFilterOption> { eFilterOption.Equip_Stat_Potential };

	private static readonly HashSet<eFilterOption> m_setFilterCategory_StatType_SetOption = new HashSet<eFilterOption> { eFilterOption.Equip_Stat_SetOption };

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Private = new HashSet<eFilterOption>
	{
		eFilterOption.Equip_Private,
		eFilterOption.Equip_Private_Awaken,
		eFilterOption.Equip_Non_Private,
		eFilterOption.Equip_Relic
	};

	private static List<HashSet<eFilterOption>> m_lstFilterCategory = new List<HashSet<eFilterOption>>
	{
		m_setFilterCategory_EquipUnitType, m_setFilterCategory_EquipType, m_setFilterCategory_Tier, m_setFilterCategory_Rarity, m_setFilterCategory_Equipped, m_setFilterCategory_Locked, m_setFilterCategory_Have, m_setFilterCategory_SetOptionType, m_setFilterCategory_SetOptionPart, m_setFilterCategory_Upgrade,
		m_setFilterCategory_StatType, m_setFilterCategory_StatType_SetOption, m_setFilterCategory_StatType_Potential, m_setFilterCategory_Private
	};

	public static readonly HashSet<eFilterCategory> m_hsEquipUpgradeFilterSet = new HashSet<eFilterCategory>
	{
		eFilterCategory.UnitType,
		eFilterCategory.EquipType,
		eFilterCategory.Tier,
		eFilterCategory.PrivateEquip
	};

	public static readonly HashSet<eSortCategory> m_hsEquipUpgradeSortSet = new HashSet<eSortCategory>
	{
		eSortCategory.Custom1,
		eSortCategory.ID
	};

	protected EquipListOptions m_Options;

	protected Dictionary<long, NKMEquipItemData> m_dicAllEquipList;

	protected List<NKMEquipItemData> m_lstCurrentEquipList;

	protected NKCEquipSortSystemFilterTokens m_FilterToken = new NKCEquipSortSystemFilterTokens();

	private Dictionary<long, float> m_dicCacheOperationPower = new Dictionary<long, float>();

	private Dictionary<long, float> m_dicCachePriorityByClassValue = new Dictionary<long, float>();

	public EquipListOptions m_EquipListOptions
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
			FilterList(m_Options.setFilterOption, m_Options.bHideEquippedItem);
		}
	}

	public List<NKMEquipItemData> SortedEquipList
	{
		get
		{
			if (m_lstCurrentEquipList == null)
			{
				if (m_Options.setFilterOption == null)
				{
					m_Options.setFilterOption = new HashSet<eFilterOption>();
					FilterList(m_Options.setFilterOption, m_Options.bHideEquippedItem);
				}
				else
				{
					FilterList(m_Options.setFilterOption, m_Options.bHideEquippedItem);
				}
			}
			return m_lstCurrentEquipList;
		}
	}

	public HashSet<eFilterOption> ExcludeFilterSet => m_Options.setExcludeFilterOption;

	public HashSet<eFilterOption> FilterSet
	{
		get
		{
			return m_Options.setFilterOption;
		}
		set
		{
			FilterList(value, m_Options.bHideEquippedItem);
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
			SortList(value, GetDescendingBySorting(value));
		}
	}

	public bool bHideEquippedItem
	{
		get
		{
			return m_Options.bHideEquippedItem;
		}
		set
		{
			if (m_Options.setFilterOption != null)
			{
				FilterList(m_Options.setFilterOption, value);
				return;
			}
			m_Options.setFilterOption = new HashSet<eFilterOption>();
			FilterList(m_Options.setFilterOption, value);
		}
	}

	public NKM_STAT_TYPE FilterStatType_01
	{
		get
		{
			return m_Options.FilterStatType_01;
		}
		set
		{
			m_Options.FilterStatType_01 = value;
		}
	}

	public NKM_STAT_TYPE FilterStatType_02
	{
		get
		{
			return m_Options.FilterStatType_02;
		}
		set
		{
			m_Options.FilterStatType_02 = value;
		}
	}

	public NKM_STAT_TYPE FilterStatType_Potential
	{
		get
		{
			return m_Options.FilterStatType_Potential;
		}
		set
		{
			m_Options.FilterStatType_Potential = value;
		}
	}

	public int FilterStatType_SetOptionID
	{
		get
		{
			return m_Options.FilterSetOptionID;
		}
		set
		{
			m_Options.FilterSetOptionID = value;
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

	private NKCEquipSortSystem()
	{
	}

	public NKCEquipSortSystem(NKMUserData userData, EquipListOptions options)
	{
		m_Options = options;
		m_FilterToken.Clear();
		List<NKMEquipItemData> lstTargetEquips = new List<NKMEquipItemData>(userData.m_InventoryData.EquipItems.Values);
		m_dicAllEquipList = BuildFullEquipList(userData, lstTargetEquips, options);
	}

	public NKCEquipSortSystem(NKMUserData userData, EquipListOptions options, IEnumerable<NKMEquipItemData> lstEquipData)
	{
		m_Options = options;
		m_FilterToken.Clear();
		m_dicAllEquipList = BuildFullEquipList(userData, lstEquipData, options);
	}

	public virtual void BuildFilterAndSortedList(HashSet<eFilterOption> setfilterType, List<eSortOption> lstSortOption, bool bHideEquippedItem)
	{
		m_Options.bHideEquippedItem = bHideEquippedItem;
		m_Options.setFilterOption = setfilterType;
		m_Options.lstSortOption = lstSortOption;
		FilterList(setfilterType, bHideEquippedItem);
	}

	private Dictionary<long, NKMEquipItemData> BuildFullEquipList(NKMUserData userData, IEnumerable<NKMEquipItemData> lstTargetEquips, EquipListOptions options)
	{
		Dictionary<long, NKMEquipItemData> dictionary = new Dictionary<long, NKMEquipItemData>();
		HashSet<int> setOnlyIncludeEquipID = options.setOnlyIncludeEquipID;
		HashSet<int> setExcludeEquipID = options.setExcludeEquipID;
		foreach (NKMEquipItemData lstTargetEquip in lstTargetEquips)
		{
			long itemUid = lstTargetEquip.m_ItemUid;
			if ((options.AdditionalExcludeFilterFunc != null && !options.AdditionalExcludeFilterFunc(lstTargetEquip)) || (options.setExcludeEquipUID != null && options.setExcludeEquipUID.Contains(itemUid)) || (options.bHideLockItem && lstTargetEquip.m_bLock))
			{
				continue;
			}
			if (options.bHideMaxLvItem)
			{
				NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(lstTargetEquip.m_ItemEquipID);
				if (equipTemplet != null && lstTargetEquip.m_EnchantLevel >= NKMItemManager.GetMaxEquipEnchantLevel(equipTemplet.m_NKM_ITEM_TIER))
				{
					continue;
				}
			}
			if ((setOnlyIncludeEquipID == null || setOnlyIncludeEquipID.Count <= 0 || setOnlyIncludeEquipID.Contains(lstTargetEquip.m_ItemEquipID)) && (setExcludeEquipID == null || setExcludeEquipID.Count <= 0 || !setExcludeEquipID.Contains(lstTargetEquip.m_ItemEquipID)) && (options.setExcludeFilterOption == null || options.setExcludeFilterOption.Count <= 0 || !CheckFilter(lstTargetEquip, options.setExcludeFilterOption, options)))
			{
				dictionary.Add(itemUid, lstTargetEquip);
			}
		}
		return dictionary;
	}

	public void UpdateEquipData(NKMEquipItemData equipData)
	{
		if (m_dicAllEquipList.ContainsKey(equipData.m_ItemUid))
		{
			m_dicAllEquipList[equipData.m_ItemUid] = equipData;
		}
	}

	protected bool FilterData(NKMEquipItemData equipData, List<HashSet<eFilterOption>> setFilter)
	{
		if (m_Options.bHideEquippedItem && equipData.m_OwnerUnitUID <= 0)
		{
			return false;
		}
		if (m_Options.bHideNotPossibleSetOptionItem)
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(equipData.m_ItemEquipID);
			if (equipTemplet != null && (equipTemplet.SetGroupList == null || equipTemplet.SetGroupList.Count <= 0))
			{
				return false;
			}
		}
		if (m_Options.iTargetUnitID != 0)
		{
			NKMEquipTemplet equipTemplet2 = NKMItemManager.GetEquipTemplet(equipData.m_ItemEquipID);
			if (equipTemplet2 != null && equipTemplet2.IsPrivateEquip() && !equipTemplet2.IsPrivateEquipForUnit(m_Options.iTargetUnitID))
			{
				return false;
			}
		}
		if (setFilter != null)
		{
			for (int i = 0; i < setFilter.Count; i++)
			{
				if (!CheckFilter(equipData, setFilter[i], m_Options))
				{
					return false;
				}
			}
		}
		if (!m_FilterToken.CheckFilter(equipData, m_Options))
		{
			return false;
		}
		return true;
	}

	protected bool IsEquipSelectable(NKMEquipItemData equipData)
	{
		return true;
	}

	private static bool CheckFilter(NKMEquipItemData equipData, HashSet<eFilterOption> setFilter, EquipListOptions equipListOptions)
	{
		foreach (eFilterOption item in setFilter)
		{
			if (CheckFilter(equipData, item, equipListOptions))
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsSameStatType(NKM_STAT_TYPE lhs, NKM_STAT_TYPE rhs)
	{
		return lhs == rhs;
	}

	private static bool CheckFilter(NKMEquipItemData equipData, eFilterOption filterOption, EquipListOptions equipListOptions)
	{
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(equipData.m_ItemEquipID);
		if (equipTemplet == null)
		{
			Debug.LogError($"UnitTemplet Null! unitID : {equipData.m_ItemEquipID}");
			return false;
		}
		switch (filterOption)
		{
		default:
			return false;
		case eFilterOption.All:
			return true;
		case eFilterOption.Equip_Counter:
			if (equipTemplet.m_EquipUnitStyleType == NKM_UNIT_STYLE_TYPE.NUST_COUNTER)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_Soldier:
			if (equipTemplet.m_EquipUnitStyleType == NKM_UNIT_STYLE_TYPE.NUST_SOLDIER)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_Mechanic:
			if (equipTemplet.m_EquipUnitStyleType == NKM_UNIT_STYLE_TYPE.NUST_MECHANIC)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_Weapon:
			if (equipTemplet.m_ItemEquipPosition == ITEM_EQUIP_POSITION.IEP_WEAPON)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_Armor:
			if (equipTemplet.m_ItemEquipPosition == ITEM_EQUIP_POSITION.IEP_DEFENCE)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_Acc:
			if (equipTemplet.m_ItemEquipPosition == ITEM_EQUIP_POSITION.IEP_ACC || equipTemplet.m_ItemEquipPosition == ITEM_EQUIP_POSITION.IEP_ACC2)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_Enchant:
			if (equipTemplet.m_ItemEquipPosition == ITEM_EQUIP_POSITION.IEP_ENCHANT)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_Tier_7:
			if (equipTemplet.m_NKM_ITEM_TIER == 7)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_Tier_6:
			if (equipTemplet.m_NKM_ITEM_TIER == 6)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_Tier_5:
			if (equipTemplet.m_NKM_ITEM_TIER == 5)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_Tier_4:
			if (equipTemplet.m_NKM_ITEM_TIER == 4)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_Tier_3:
			if (equipTemplet.m_NKM_ITEM_TIER == 3)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_Tier_2:
			if (equipTemplet.m_NKM_ITEM_TIER == 2)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_Tier_1:
			if (equipTemplet.m_NKM_ITEM_TIER == 1)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_Rarity_SSR:
			if (equipTemplet.m_NKM_ITEM_GRADE == NKM_ITEM_GRADE.NIG_SSR)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_Rarity_SR:
			if (equipTemplet.m_NKM_ITEM_GRADE == NKM_ITEM_GRADE.NIG_SR)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_Rarity_R:
			if (equipTemplet.m_NKM_ITEM_GRADE == NKM_ITEM_GRADE.NIG_R)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_Rarity_N:
			if (equipTemplet.m_NKM_ITEM_GRADE == NKM_ITEM_GRADE.NIG_N)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_Set_Part_2:
			if (equipData.m_SetOptionId > 0)
			{
				NKMItemEquipSetOptionTemplet equipSetOptionTemplet5 = NKMItemManager.GetEquipSetOptionTemplet(equipData.m_SetOptionId);
				if (equipSetOptionTemplet5 != null && equipSetOptionTemplet5.m_EquipSetPart == 2)
				{
					return true;
				}
			}
			break;
		case eFilterOption.Equip_Set_Part_3:
			if (equipData.m_SetOptionId > 0)
			{
				NKMItemEquipSetOptionTemplet equipSetOptionTemplet4 = NKMItemManager.GetEquipSetOptionTemplet(equipData.m_SetOptionId);
				if (equipSetOptionTemplet4 != null && equipSetOptionTemplet4.m_EquipSetPart == 3)
				{
					return true;
				}
			}
			break;
		case eFilterOption.Equip_Set_Part_4:
			if (equipData.m_SetOptionId > 0)
			{
				NKMItemEquipSetOptionTemplet equipSetOptionTemplet3 = NKMItemManager.GetEquipSetOptionTemplet(equipData.m_SetOptionId);
				if (equipSetOptionTemplet3 != null && equipSetOptionTemplet3.m_EquipSetPart == 4)
				{
					return true;
				}
			}
			break;
		case eFilterOption.Equip_Set_Effect_Red:
			if (equipData.m_SetOptionId > 0)
			{
				NKMItemEquipSetOptionTemplet equipSetOptionTemplet2 = NKMItemManager.GetEquipSetOptionTemplet(equipData.m_SetOptionId);
				if (equipSetOptionTemplet2 != null && string.Equals(equipSetOptionTemplet2.m_EquipSetIconEffect, "EFFECT_RED"))
				{
					return true;
				}
			}
			break;
		case eFilterOption.Equip_Set_Effect_Blue:
			if (equipData.m_SetOptionId > 0)
			{
				NKMItemEquipSetOptionTemplet equipSetOptionTemplet = NKMItemManager.GetEquipSetOptionTemplet(equipData.m_SetOptionId);
				if (equipSetOptionTemplet != null && string.Equals(equipSetOptionTemplet.m_EquipSetIconEffect, "EFFECT_BLUE"))
				{
					return true;
				}
			}
			break;
		case eFilterOption.Equip_Set_Effect_Yellow:
			if (equipData.m_SetOptionId > 0)
			{
				NKMItemEquipSetOptionTemplet equipSetOptionTemplet6 = NKMItemManager.GetEquipSetOptionTemplet(equipData.m_SetOptionId);
				if (equipSetOptionTemplet6 != null && string.Equals(equipSetOptionTemplet6.m_EquipSetIconEffect, "EFFECT_YELLOW"))
				{
					return true;
				}
			}
			break;
		case eFilterOption.Equip_Equipped:
			if (equipData.m_OwnerUnitUID > 0)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_Unused:
			if (equipData.m_OwnerUnitUID <= 0)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_Locked:
			if (equipData.m_bLock)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_Unlocked:
			if (!equipData.m_bLock)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_Have:
			if (NKCScenManager.CurrentUserData().m_InventoryData.GetSameKindEquipCount(equipData.m_ItemEquipID) > 0)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_NotHave:
			if (NKCScenManager.CurrentUserData().m_InventoryData.GetSameKindEquipCount(equipData.m_ItemEquipID) == 0)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_Stat_SetOption:
			if (equipListOptions.FilterSetOptionID > 0)
			{
				return equipData.m_SetOptionId == equipListOptions.FilterSetOptionID;
			}
			break;
		case eFilterOption.Equip_Stat_01:
			if (equipListOptions.FilterStatType_01 != NKM_STAT_TYPE.NST_RANDOM)
			{
				return equipData.m_Stat.Find((EQUIP_ITEM_STAT x) => IsSameStatType(x.type, equipListOptions.FilterStatType_01)) != null;
			}
			break;
		case eFilterOption.Equip_Stat_02:
			if (equipListOptions.FilterStatType_02 != NKM_STAT_TYPE.NST_RANDOM)
			{
				return equipData.m_Stat.Find((EQUIP_ITEM_STAT x) => IsSameStatType(x.type, equipListOptions.FilterStatType_02)) != null;
			}
			break;
		case eFilterOption.Equip_Stat_Potential:
			if (equipListOptions.FilterStatType_Potential == NKM_STAT_TYPE.NST_RANDOM)
			{
				break;
			}
			if (equipData.potentialOptions.Count > 0)
			{
				return equipData.potentialOptions.Find((NKMPotentialOption x) => x.statType == equipListOptions.FilterStatType_Potential) != null;
			}
			return false;
		case eFilterOption.Equip_Private:
		{
			if (!equipTemplet.IsPrivateEquip())
			{
				return false;
			}
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(equipTemplet.GetPrivateUnitID());
			if (unitTempletBase2 != null && !unitTempletBase2.m_bAwaken)
			{
				return true;
			}
			break;
		}
		case eFilterOption.Equip_Private_Awaken:
		{
			if (!equipTemplet.IsPrivateEquip())
			{
				return false;
			}
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(equipTemplet.GetPrivateUnitID());
			if (unitTempletBase != null && unitTempletBase.m_bAwaken)
			{
				return true;
			}
			break;
		}
		case eFilterOption.Equip_Non_Private:
			if (!equipTemplet.IsPrivateEquip() && !equipTemplet.m_bRelic)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_Relic:
			if (equipTemplet.m_bRelic)
			{
				return true;
			}
			break;
		case eFilterOption.Equip_CanUpgrade:
			return NKMItemManager.CanUpgradeEquipByCoreID(equipData) == NKC_EQUIP_UPGRADE_STATE.UPGRADABLE;
		case eFilterOption.Equip_CannotUpgrade:
			return NKMItemManager.CanUpgradeEquipByCoreID(equipData) != NKC_EQUIP_UPGRADE_STATE.UPGRADABLE;
		}
		return false;
	}

	public void FilterList(eFilterOption filterOption, bool bHideDeckedUnit)
	{
		HashSet<eFilterOption> hashSet = new HashSet<eFilterOption>();
		hashSet.Add(filterOption);
		FilterList(hashSet, bHideDeckedUnit);
	}

	public virtual void FilterList(HashSet<eFilterOption> setFilter, bool bHideEquippedItem)
	{
		m_Options.setFilterOption = setFilter;
		m_Options.bHideEquippedItem = bHideEquippedItem;
		if (m_lstCurrentEquipList == null)
		{
			m_lstCurrentEquipList = new List<NKMEquipItemData>();
		}
		m_lstCurrentEquipList.Clear();
		List<HashSet<eFilterOption>> setFilter2 = SetFilterCategory(setFilter);
		foreach (KeyValuePair<long, NKMEquipItemData> dicAllEquip in m_dicAllEquipList)
		{
			NKMEquipItemData value = dicAllEquip.Value;
			if (FilterData(value, setFilter2))
			{
				m_lstCurrentEquipList.Add(value);
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

	private static List<HashSet<eFilterOption>> SetFilterCategory(HashSet<eFilterOption> setFilter)
	{
		List<HashSet<eFilterOption>> list = new List<HashSet<eFilterOption>>();
		if (setFilter == null || setFilter.Count == 0)
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

	public void SortList(List<eSortOption> lstSortOption, bool bForce = false)
	{
		if (m_lstCurrentEquipList == null)
		{
			m_Options.lstSortOption = lstSortOption;
			if (m_Options.setFilterOption != null)
			{
				FilterList(m_Options.setFilterOption, m_Options.bHideEquippedItem);
				return;
			}
			m_Options.setFilterOption = new HashSet<eFilterOption>();
			FilterList(m_Options.setFilterOption, m_Options.bHideEquippedItem);
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
		SortEquipDataList(ref m_lstCurrentEquipList, lstSortOption);
		m_Options.lstSortOption = lstSortOption;
	}

	private void SortEquipDataList(ref List<NKMEquipItemData> lstEquipData, List<eSortOption> lstSortOption)
	{
		NKCUnitSortSystem.NKCDataComparerer<NKMEquipItemData> nKCDataComparerer = new NKCUnitSortSystem.NKCDataComparerer<NKMEquipItemData>();
		if (m_Options.PreemptiveSortFunc != null)
		{
			nKCDataComparerer.AddFunc(m_Options.PreemptiveSortFunc);
		}
		if (m_Options.bPushBackUnselectable)
		{
			nKCDataComparerer.AddFunc(CompareBySelectable);
		}
		m_dicCacheOperationPower.Clear();
		m_dicCachePriorityByClassValue.Clear();
		foreach (eSortOption item in lstSortOption)
		{
			if (item == eSortOption.OperationPower_High || item == eSortOption.OperationPower_Low)
			{
				UpdateCacheOperationPowerData(lstEquipData);
			}
			if (item == eSortOption.OptionWeightByClassType_High || item == eSortOption.OptionWeightByClassType_Low)
			{
				UpdateCachePriorityByClassData(lstEquipData);
			}
			NKCUnitSortSystem.NKCDataComparerer<NKMEquipItemData>.CompareFunc dataComparer = GetDataComparer(item);
			if (dataComparer != null)
			{
				nKCDataComparerer.AddFunc(dataComparer);
			}
		}
		if (!lstSortOption.Contains(eSortOption.UID_First) && !lstSortOption.Contains(eSortOption.UID_Last))
		{
			nKCDataComparerer.AddFunc(CompareByUIDAscending);
		}
		lstEquipData.Sort(nKCDataComparerer);
	}

	private void UpdateCacheOperationPowerData(List<NKMEquipItemData> lstEquipData)
	{
		if (m_dicCacheOperationPower.Count > 0)
		{
			return;
		}
		foreach (NKMEquipItemData lstEquipDatum in lstEquipData)
		{
			if (lstEquipDatum != null)
			{
				NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(lstEquipDatum.m_ItemEquipID);
				if (equipTemplet != null)
				{
					float equipOperationPower = GetEquipOperationPower(equipTemplet.m_NKM_ITEM_TIER, (int)equipTemplet.m_NKM_ITEM_GRADE, lstEquipDatum.m_EnchantLevel, lstEquipDatum.m_Precision, lstEquipDatum.m_Precision2);
					Debug.Log($"작전능력 테스트 - 아이템 : {equipTemplet.GetItemName()}, 티어 : {equipTemplet.m_NKM_ITEM_TIER} 등급: {(int)equipTemplet.m_NKM_ITEM_GRADE}, 레벨:{lstEquipDatum.m_EnchantLevel} 조정률1:{lstEquipDatum.m_Precision} 조정률2:{lstEquipDatum.m_Precision2} 작전능력 : {equipOperationPower.ToString()}");
					m_dicCacheOperationPower.Add(lstEquipDatum.m_ItemUid, equipOperationPower);
				}
			}
		}
	}

	private float GetEquipOperationPower(int Tier, int Grade, int level, int Precision1, int Precision2)
	{
		float num = 100 * (Tier + 1);
		float num2 = Grade * (30 + 15 * (Tier - 1));
		float num3 = (num + num2) * (1f + 0.075f * (float)level);
		if (Precision2 > 0)
		{
			float num4 = (float)(100 * (Tier + 1) + Grade * (30 + 15 * (Tier - 1))) * (0.5f + (float)Precision1 * 0.01f * 0.5f);
			float num5 = (float)(100 * (Tier + 1) + Grade * (30 + 15 * (Tier - 1))) * (0.5f + (float)Precision2 * 0.01f * 0.5f);
			return num3 + num4 + num5;
		}
		return num3 + (float)((4 + Tier * 5) * 8) * (0.5f + (float)Precision1 * 0.01f * 0.5f);
	}

	private void UpdateCachePriorityByClassData(List<NKMEquipItemData> lstEquipData)
	{
		if (m_dicCachePriorityByClassValue.Count > 0)
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		NKMInventoryData nKMInventoryData = null;
		if (nKMUserData != null)
		{
			nKMInventoryData = nKMUserData.m_InventoryData;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_Options.OwnerUnitID);
		foreach (NKMEquipItemData lstEquipDatum in lstEquipData)
		{
			if (lstEquipDatum == null)
			{
				continue;
			}
			NKMEquipItemData nKMEquipItemData = null;
			if (nKMInventoryData != null)
			{
				nKMEquipItemData = nKMInventoryData.GetItemEquip(lstEquipDatum.m_ItemUid);
				if (nKMEquipItemData == null)
				{
					continue;
				}
			}
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(lstEquipDatum.m_ItemEquipID);
			if (equipTemplet != null)
			{
				float equipPriorityValueByClass = GetEquipPriorityValueByClass(unitTempletBase, nKMEquipItemData, equipTemplet);
				Debug.Log($"테스트 적용 정보 : {equipTemplet.GetItemName()}, 장착 부위 : {equipTemplet.m_ItemEquipPosition} 조정률1:{lstEquipDatum.m_Precision} 조정률2:{lstEquipDatum.m_Precision2} 결과 : {equipPriorityValueByClass}");
				m_dicCachePriorityByClassValue.Add(lstEquipDatum.m_ItemUid, equipPriorityValueByClass);
			}
		}
	}

	private float GetEquipPriorityValueByClass(NKMUnitTempletBase unitTemplet, NKMEquipItemData itemData, NKMEquipTemplet equipTemplet)
	{
		NKM_STAT_TYPE nKM_STAT_TYPE = NKM_STAT_TYPE.NST_RANDOM;
		if (equipTemplet.m_ItemEquipPosition == ITEM_EQUIP_POSITION.IEP_ACC || equipTemplet.m_ItemEquipPosition == ITEM_EQUIP_POSITION.IEP_ACC2)
		{
			nKM_STAT_TYPE = ((unitTemplet.m_NKM_UNIT_ROLE_TYPE != NKM_UNIT_ROLE_TYPE.NURT_RANGER && unitTemplet.m_NKM_UNIT_ROLE_TYPE != NKM_UNIT_ROLE_TYPE.NURT_SNIPER) ? NKM_STAT_TYPE.NST_EVADE : NKM_STAT_TYPE.NST_HIT);
		}
		float num = 1f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		NKM_FIND_TARGET_TYPE nKM_FIND_TARGET_TYPE = ((unitTemplet.m_NKM_FIND_TARGET_TYPE_Desc == NKM_FIND_TARGET_TYPE.NFTT_INVALID) ? unitTemplet.m_NKM_FIND_TARGET_TYPE : unitTemplet.m_NKM_FIND_TARGET_TYPE_Desc);
		for (int i = 0; i < itemData.m_Stat.Count; i++)
		{
			EQUIP_ITEM_STAT eQUIP_ITEM_STAT = itemData.m_Stat[i];
			if (i == 0)
			{
				if (nKM_STAT_TYPE != NKM_STAT_TYPE.NST_RANDOM && eQUIP_ITEM_STAT.type != nKM_STAT_TYPE)
				{
					num = 0.5f;
				}
				num2 = ((!NKMUnitStatManager.IsPercentStat(eQUIP_ITEM_STAT.type)) ? (eQUIP_ITEM_STAT.stat_value + (float)itemData.m_EnchantLevel * eQUIP_ITEM_STAT.stat_level_value) : ((float)(Math.Round(new decimal(eQUIP_ITEM_STAT.stat_value + (float)itemData.m_EnchantLevel * eQUIP_ITEM_STAT.stat_level_value) * 1000m) / 1000m)));
			}
			else if ((eQUIP_ITEM_STAT.type != NKM_STAT_TYPE.NST_MOVE_TYPE_LAND_DAMAGE_RATE || unitTemplet.m_NKM_FIND_TARGET_TYPE != NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_AIR) && (eQUIP_ITEM_STAT.type != NKM_STAT_TYPE.NST_MOVE_TYPE_AIR_DAMAGE_RATE || nKM_FIND_TARGET_TYPE != NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND))
			{
				switch (i)
				{
				case 1:
					num3 = NKMItemManager.GetOptionWeight(eQUIP_ITEM_STAT.type, unitTemplet.m_NKM_UNIT_ROLE_TYPE);
					break;
				case 2:
					num4 = NKMItemManager.GetOptionWeight(eQUIP_ITEM_STAT.type, unitTemplet.m_NKM_UNIT_ROLE_TYPE);
					break;
				}
			}
		}
		Debug.Log($"주옵션 능력치 : {num2}, a : {num}, 옵션1계수 : {num3} , 옵션1정밀도 : {(float)itemData.m_Precision * 0.01f}, 옵션2계수 : {num4} , 옵션1정밀도 : {(float)itemData.m_Precision2 * 0.01f}");
		return num2 * (num + num3 * ((float)itemData.m_Precision * 0.01f) + num4 * ((float)itemData.m_Precision2 * 0.01f));
	}

	private NKCUnitSortSystem.NKCDataComparerer<NKMEquipItemData>.CompareFunc GetDataComparer(eSortOption sortOption)
	{
		switch (sortOption)
		{
		default:
			return CompareByEnhanceDescending;
		case eSortOption.Enhance_Low:
			return CompareByEnhanceAscending;
		case eSortOption.Tier_High:
			return CompareByTierDescending;
		case eSortOption.Tier_Low:
			return CompareByTierAscending;
		case eSortOption.Rarity_High:
			return CompareByRarityDescending;
		case eSortOption.Rarity_Low:
			return CompareByRarityAscending;
		case eSortOption.UID_First:
			return CompareByUIDAscending;
		case eSortOption.UID_Last:
			return CompareByUIDDescending;
		case eSortOption.SetOption_High:
			return CompareBySetOptionHigh;
		case eSortOption.SetOption_Low:
			return CompareBySetOptionLow;
		case eSortOption.Equipped_First:
			return CompareByEquippedFirst;
		case eSortOption.Equipped_Last:
			return CompareByEquippedLast;
		case eSortOption.ID_First:
			return CompareByIDAscending;
		case eSortOption.ID_Last:
			return CompareByIDDescending;
		case eSortOption.OperationPower_High:
			return CompareByOperationPowerHigh;
		case eSortOption.OperationPower_Low:
			return CompareByOperationPowerLow;
		case eSortOption.OptionWeightByClassType_High:
			return CompareByPriorityByClassHigh;
		case eSortOption.OptionWeightByClassType_Low:
			return CompareByPriorityByClassLow;
		case eSortOption.PrivateEquip_First:
			return CompareByPrivateEquipFirst;
		case eSortOption.PrivateEquip_Last:
			return CompareByPrivateEquipLast;
		case eSortOption.CustomDescend1:
		case eSortOption.CustomDescend2:
		case eSortOption.CustomDescend3:
			if (m_Options.lstCustomSortFunc != null && m_Options.lstCustomSortFunc.ContainsKey(GetSortCategoryFromOption(sortOption)))
			{
				return m_Options.lstCustomSortFunc[GetSortCategoryFromOption(sortOption)].Value;
			}
			return null;
		case eSortOption.CustomAscend1:
		case eSortOption.CustomAscend2:
		case eSortOption.CustomAscend3:
			if (m_Options.lstCustomSortFunc != null && m_Options.lstCustomSortFunc.ContainsKey(GetSortCategoryFromOption(sortOption)))
			{
				return (NKMEquipItemData a, NKMEquipItemData b) => m_Options.lstCustomSortFunc[GetSortCategoryFromOption(sortOption)].Value(b, a);
			}
			return null;
		}
	}

	private int CompareByUIDAscending(NKMEquipItemData lhs, NKMEquipItemData rhs)
	{
		return lhs.m_ItemUid.CompareTo(rhs.m_ItemUid);
	}

	private int CompareByUIDDescending(NKMEquipItemData lhs, NKMEquipItemData rhs)
	{
		return rhs.m_ItemUid.CompareTo(lhs.m_ItemUid);
	}

	private int CompareByEquippedFirst(NKMEquipItemData lhs, NKMEquipItemData rhs)
	{
		if (lhs.m_OwnerUnitUID != -1 && rhs.m_OwnerUnitUID != -1)
		{
			NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
			NKMDeckIndex deckIndexByUnitUID = armyData.GetDeckIndexByUnitUID(NKM_DECK_TYPE.NDT_NORMAL, lhs.m_OwnerUnitUID);
			NKMDeckIndex deckIndexByUnitUID2 = armyData.GetDeckIndexByUnitUID(NKM_DECK_TYPE.NDT_NORMAL, rhs.m_OwnerUnitUID);
			if (deckIndexByUnitUID.m_eDeckType != NKM_DECK_TYPE.NDT_NONE && deckIndexByUnitUID2.m_eDeckType != NKM_DECK_TYPE.NDT_NONE)
			{
				return deckIndexByUnitUID.m_iIndex.CompareTo(deckIndexByUnitUID2.m_iIndex);
			}
			return deckIndexByUnitUID2.m_eDeckType.CompareTo(deckIndexByUnitUID.m_eDeckType);
		}
		return rhs.m_OwnerUnitUID.CompareTo(lhs.m_OwnerUnitUID);
	}

	private int CompareByEquippedLast(NKMEquipItemData lhs, NKMEquipItemData rhs)
	{
		if (lhs.m_OwnerUnitUID != -1 && rhs.m_OwnerUnitUID != -1)
		{
			NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
			NKMDeckIndex deckIndexByUnitUID = armyData.GetDeckIndexByUnitUID(NKM_DECK_TYPE.NDT_NORMAL, lhs.m_OwnerUnitUID);
			NKMDeckIndex deckIndexByUnitUID2 = armyData.GetDeckIndexByUnitUID(NKM_DECK_TYPE.NDT_NORMAL, rhs.m_OwnerUnitUID);
			if (deckIndexByUnitUID.m_eDeckType != NKM_DECK_TYPE.NDT_NONE && deckIndexByUnitUID2.m_eDeckType != NKM_DECK_TYPE.NDT_NONE)
			{
				return deckIndexByUnitUID.m_iIndex.CompareTo(deckIndexByUnitUID2.m_iIndex);
			}
			return deckIndexByUnitUID.m_eDeckType.CompareTo(deckIndexByUnitUID2.m_eDeckType);
		}
		return lhs.m_OwnerUnitUID.CompareTo(rhs.m_OwnerUnitUID);
	}

	private int CompareByEnhanceAscending(NKMEquipItemData lhs, NKMEquipItemData rhs)
	{
		return lhs.m_EnchantLevel.CompareTo(rhs.m_EnchantLevel);
	}

	private int CompareByEnhanceDescending(NKMEquipItemData lhs, NKMEquipItemData rhs)
	{
		return rhs.m_EnchantLevel.CompareTo(lhs.m_EnchantLevel);
	}

	private int CompareByTierAscending(NKMEquipItemData lhs, NKMEquipItemData rhs)
	{
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(lhs.m_ItemEquipID);
		NKMEquipTemplet equipTemplet2 = NKMItemManager.GetEquipTemplet(rhs.m_ItemEquipID);
		if (equipTemplet == null || equipTemplet2 == null)
		{
			return -1;
		}
		return equipTemplet.m_NKM_ITEM_TIER.CompareTo(equipTemplet2.m_NKM_ITEM_TIER);
	}

	private int CompareByTierDescending(NKMEquipItemData lhs, NKMEquipItemData rhs)
	{
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(lhs.m_ItemEquipID);
		NKMEquipTemplet equipTemplet2 = NKMItemManager.GetEquipTemplet(rhs.m_ItemEquipID);
		if (equipTemplet == null || equipTemplet2 == null)
		{
			return -1;
		}
		return equipTemplet2.m_NKM_ITEM_TIER.CompareTo(equipTemplet.m_NKM_ITEM_TIER);
	}

	private int CompareByRarityAscending(NKMEquipItemData lhs, NKMEquipItemData rhs)
	{
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(lhs.m_ItemEquipID);
		NKMEquipTemplet equipTemplet2 = NKMItemManager.GetEquipTemplet(rhs.m_ItemEquipID);
		if (equipTemplet == null || equipTemplet2 == null)
		{
			return -1;
		}
		return equipTemplet.m_NKM_ITEM_GRADE.CompareTo(equipTemplet2.m_NKM_ITEM_GRADE);
	}

	private int CompareByRarityDescending(NKMEquipItemData lhs, NKMEquipItemData rhs)
	{
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(lhs.m_ItemEquipID);
		NKMEquipTemplet equipTemplet2 = NKMItemManager.GetEquipTemplet(rhs.m_ItemEquipID);
		if (equipTemplet == null || equipTemplet2 == null)
		{
			return -1;
		}
		return equipTemplet2.m_NKM_ITEM_GRADE.CompareTo(equipTemplet.m_NKM_ITEM_GRADE);
	}

	private int CompareByIDAscending(NKMEquipItemData lhs, NKMEquipItemData rhs)
	{
		return lhs.m_ItemEquipID.CompareTo(rhs.m_ItemEquipID);
	}

	private int CompareByIDDescending(NKMEquipItemData lhs, NKMEquipItemData rhs)
	{
		return rhs.m_ItemEquipID.CompareTo(lhs.m_ItemEquipID);
	}

	private int CompareBySelectable(NKMEquipItemData lhs, NKMEquipItemData rhs)
	{
		return IsEquipSelectable(rhs).CompareTo(IsEquipSelectable(lhs));
	}

	private int CompareByOperationPowerHigh(NKMEquipItemData lhs, NKMEquipItemData rhs)
	{
		if (!m_dicCacheOperationPower.ContainsKey(lhs.m_ItemUid) || !m_dicCacheOperationPower.ContainsKey(rhs.m_ItemUid))
		{
			return 0;
		}
		float value = m_dicCacheOperationPower[lhs.m_ItemUid];
		return m_dicCacheOperationPower[rhs.m_ItemUid].CompareTo(value);
	}

	private int CompareByOperationPowerLow(NKMEquipItemData lhs, NKMEquipItemData rhs)
	{
		if (!m_dicCacheOperationPower.ContainsKey(lhs.m_ItemUid) || !m_dicCacheOperationPower.ContainsKey(rhs.m_ItemUid))
		{
			return 0;
		}
		float num = m_dicCacheOperationPower[lhs.m_ItemUid];
		float value = m_dicCacheOperationPower[rhs.m_ItemUid];
		return num.CompareTo(value);
	}

	private int CompareByPriorityByClassHigh(NKMEquipItemData lhs, NKMEquipItemData rhs)
	{
		if (!m_dicCachePriorityByClassValue.ContainsKey(lhs.m_ItemUid) || !m_dicCachePriorityByClassValue.ContainsKey(rhs.m_ItemUid))
		{
			return 0;
		}
		float value = m_dicCachePriorityByClassValue[lhs.m_ItemUid];
		return m_dicCachePriorityByClassValue[rhs.m_ItemUid].CompareTo(value);
	}

	private int CompareByPriorityByClassLow(NKMEquipItemData lhs, NKMEquipItemData rhs)
	{
		if (!m_dicCachePriorityByClassValue.ContainsKey(lhs.m_ItemUid) || !m_dicCachePriorityByClassValue.ContainsKey(rhs.m_ItemUid))
		{
			return 0;
		}
		float num = m_dicCachePriorityByClassValue[lhs.m_ItemUid];
		float value = m_dicCachePriorityByClassValue[rhs.m_ItemUid];
		return num.CompareTo(value);
	}

	private int CompareByPrivateEquipFirst(NKMEquipItemData lhs, NKMEquipItemData rhs)
	{
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(lhs.m_ItemEquipID);
		NKMEquipTemplet equipTemplet2 = NKMItemManager.GetEquipTemplet(rhs.m_ItemEquipID);
		bool value = ((m_Options.iTargetUnitID == 0) ? equipTemplet.IsPrivateEquip() : equipTemplet.IsPrivateEquipForUnit(m_Options.iTargetUnitID));
		return ((m_Options.iTargetUnitID == 0) ? equipTemplet2.IsPrivateEquip() : equipTemplet2.IsPrivateEquipForUnit(m_Options.iTargetUnitID)).CompareTo(value);
	}

	private int CompareByPrivateEquipLast(NKMEquipItemData lhs, NKMEquipItemData rhs)
	{
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(lhs.m_ItemEquipID);
		NKMEquipTemplet equipTemplet2 = NKMItemManager.GetEquipTemplet(rhs.m_ItemEquipID);
		bool flag = ((m_Options.iTargetUnitID == 0) ? equipTemplet.IsPrivateEquip() : equipTemplet.IsPrivateEquipForUnit(m_Options.iTargetUnitID));
		bool value = ((m_Options.iTargetUnitID == 0) ? equipTemplet2.IsPrivateEquip() : equipTemplet2.IsPrivateEquipForUnit(m_Options.iTargetUnitID));
		return flag.CompareTo(value);
	}

	private int CompareBySetOptionHigh(NKMEquipItemData lhs, NKMEquipItemData rhs)
	{
		if (lhs.m_SetOptionId <= 0 || rhs.m_SetOptionId <= 0)
		{
			return 0;
		}
		return rhs.m_SetOptionId.CompareTo(lhs.m_SetOptionId);
	}

	private int CompareBySetOptionLow(NKMEquipItemData lhs, NKMEquipItemData rhs)
	{
		if (lhs.m_SetOptionId <= 0 || rhs.m_SetOptionId <= 0)
		{
			return 0;
		}
		return lhs.m_SetOptionId.CompareTo(rhs.m_SetOptionId);
	}

	public static string GetFilterName(eFilterOption type)
	{
		if (type == eFilterOption.All)
		{
			return NKCUtilString.GET_STRING_FILTER_ALL;
		}
		return "";
	}

	public static eFilterOption GetFilterOptionByEquipPosition(ITEM_EQUIP_POSITION m_ITEM_EQUIP_POSITION)
	{
		switch (m_ITEM_EQUIP_POSITION)
		{
		case ITEM_EQUIP_POSITION.IEP_WEAPON:
			return eFilterOption.Equip_Weapon;
		case ITEM_EQUIP_POSITION.IEP_DEFENCE:
			return eFilterOption.Equip_Armor;
		case ITEM_EQUIP_POSITION.IEP_ACC:
		case ITEM_EQUIP_POSITION.IEP_ACC2:
			return eFilterOption.Equip_Acc;
		default:
			return eFilterOption.All;
		}
	}

	public static eFilterOption GetFilterOption(NKM_ITEM_GRADE grade)
	{
		return grade switch
		{
			NKM_ITEM_GRADE.NIG_N => eFilterOption.Equip_Rarity_N, 
			NKM_ITEM_GRADE.NIG_R => eFilterOption.Equip_Rarity_R, 
			NKM_ITEM_GRADE.NIG_SR => eFilterOption.Equip_Rarity_SR, 
			NKM_ITEM_GRADE.NIG_SSR => eFilterOption.Equip_Rarity_SSR, 
			_ => eFilterOption.All, 
		};
	}

	public static eFilterOption GetFilterOptionByEquipTier(int tier)
	{
		return tier switch
		{
			1 => eFilterOption.Equip_Tier_1, 
			2 => eFilterOption.Equip_Tier_2, 
			3 => eFilterOption.Equip_Tier_3, 
			4 => eFilterOption.Equip_Tier_4, 
			5 => eFilterOption.Equip_Tier_5, 
			6 => eFilterOption.Equip_Tier_6, 
			7 => eFilterOption.Equip_Tier_7, 
			_ => eFilterOption.All, 
		};
	}

	public static string GetSortName(eSortOption option)
	{
		switch (option)
		{
		default:
			return NKCUtilString.GET_STRING_SORT_ENHANCE;
		case eSortOption.Tier_High:
		case eSortOption.Tier_Low:
			return NKCUtilString.GET_STRING_SORT_TIER;
		case eSortOption.Rarity_High:
		case eSortOption.Rarity_Low:
			return NKCUtilString.GET_STRING_SORT_RARITY;
		case eSortOption.UID_First:
		case eSortOption.UID_Last:
			return NKCUtilString.GET_STRING_SORT_UID;
		case eSortOption.SetOption_High:
		case eSortOption.SetOption_Low:
			return NKCUtilString.GET_STRING_SORT_SETOPTION;
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
		case eSortOption.Enhance_Low:
		case eSortOption.Tier_Low:
		case eSortOption.Rarity_Low:
		case eSortOption.UID_First:
		case eSortOption.SetOption_Low:
		case eSortOption.OperationPower_Low:
		case eSortOption.OptionWeightByClassType_Low:
		case eSortOption.Equipped_Last:
		case eSortOption.UnitType_Last:
		case eSortOption.EquipType_Last:
		case eSortOption.ID_Last:
			return false;
		}
	}

	public NKMEquipItemData AutoSelect(HashSet<long> setExcludeEquipUid, AutoSelectExtraFilter extrafilter = null)
	{
		for (int i = 0; i < SortedEquipList.Count; i++)
		{
			NKMEquipItemData nKMEquipItemData = SortedEquipList[i];
			if (nKMEquipItemData != null && (setExcludeEquipUid == null || setExcludeEquipUid.Count <= 0 || !setExcludeEquipUid.Contains(nKMEquipItemData.m_ItemUid)) && (extrafilter == null || extrafilter(nKMEquipItemData)) && IsEquipSelectable(nKMEquipItemData))
			{
				return nKMEquipItemData;
			}
		}
		return null;
	}

	public static NKMEquipItemData MakeTempEquipData(int equipID, int setOptionID = 0, bool bMaxValue = false)
	{
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(equipID);
		if (equipTemplet != null)
		{
			NKMEquipItemData nKMEquipItemData = new NKMEquipItemData(equipID, equipTemplet);
			nKMEquipItemData.m_EnchantLevel = 0;
			nKMEquipItemData.m_EnchantExp = 0;
			nKMEquipItemData.m_OwnerUnitUID = -1L;
			nKMEquipItemData.m_bLock = false;
			nKMEquipItemData.m_Precision = (bMaxValue ? 100 : 0);
			nKMEquipItemData.m_Precision2 = (bMaxValue ? 100 : 0);
			nKMEquipItemData.m_SetOptionId = setOptionID;
			if (bMaxValue)
			{
				NKMEquipTemplet equipTemplet2 = NKMItemManager.GetEquipTemplet(equipID);
				if (equipTemplet2 != null)
				{
					nKMEquipItemData.m_EnchantLevel = equipTemplet2.m_MaxEnchantLevel;
				}
			}
			if (equipTemplet.m_ItemEquipPosition == ITEM_EQUIP_POSITION.IEP_ENCHANT)
			{
				return nKMEquipItemData;
			}
			if (equipTemplet.m_StatGroupID > 0)
			{
				EQUIP_ITEM_STAT stat = new EQUIP_ITEM_STAT();
				IReadOnlyList<NKMEquipRandomStatTemplet> equipRandomStatGroupList = NKMEquipTuningManager.GetEquipRandomStatGroupList(equipTemplet.m_StatGroupID);
				if (equipRandomStatGroupList != null && equipRandomStatGroupList.Count == 1)
				{
					stat.type = equipRandomStatGroupList[0].m_StatType;
					stat.stat_value = 0f;
					if (bMaxValue)
					{
						SetMaximumStatValue(ref stat, equipRandomStatGroupList[0]);
					}
				}
				else
				{
					stat.type = NKM_STAT_TYPE.NST_RANDOM;
				}
				nKMEquipItemData.m_Stat.Add(stat);
			}
			if (equipTemplet.m_StatGroupID_2 > 0)
			{
				EQUIP_ITEM_STAT stat2 = new EQUIP_ITEM_STAT();
				IReadOnlyList<NKMEquipRandomStatTemplet> equipRandomStatGroupList2 = NKMEquipTuningManager.GetEquipRandomStatGroupList(equipTemplet.m_StatGroupID_2);
				if (equipRandomStatGroupList2 != null && equipRandomStatGroupList2.Count == 1)
				{
					stat2.type = equipRandomStatGroupList2[0].m_StatType;
					stat2.stat_value = 0f;
					if (bMaxValue)
					{
						SetMaximumStatValue(ref stat2, equipRandomStatGroupList2[0]);
					}
				}
				else
				{
					stat2.type = NKM_STAT_TYPE.NST_RANDOM;
				}
				nKMEquipItemData.m_Stat.Add(stat2);
			}
			return nKMEquipItemData;
		}
		return new NKMEquipItemData();
	}

	public static NKMEquipItemData MakeDummyEquipData(long equipUID)
	{
		NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(equipUID);
		if (itemEquip != null)
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
			if (equipTemplet != null)
			{
				NKMEquipItemData nKMEquipItemData = new NKMEquipItemData(itemEquip.m_ItemEquipID, equipTemplet);
				nKMEquipItemData.m_EnchantLevel = itemEquip.m_EnchantLevel;
				nKMEquipItemData.m_EnchantExp = itemEquip.m_EnchantExp;
				nKMEquipItemData.m_Stat.Clear();
				for (int i = 0; i < itemEquip.m_Stat.Count; i++)
				{
					EQUIP_ITEM_STAT eQUIP_ITEM_STAT = new EQUIP_ITEM_STAT();
					eQUIP_ITEM_STAT.type = itemEquip.m_Stat[i].type;
					eQUIP_ITEM_STAT.stat_value = itemEquip.m_Stat[i].stat_value;
					eQUIP_ITEM_STAT.stat_level_value = itemEquip.m_Stat[i].stat_level_value;
					nKMEquipItemData.m_Stat.Add(eQUIP_ITEM_STAT);
				}
				nKMEquipItemData.m_OwnerUnitUID = itemEquip.m_OwnerUnitUID;
				nKMEquipItemData.m_bLock = itemEquip.m_bLock;
				nKMEquipItemData.m_Precision = itemEquip.m_Precision;
				nKMEquipItemData.m_Precision2 = itemEquip.m_Precision2;
				nKMEquipItemData.m_SetOptionId = itemEquip.m_SetOptionId;
				nKMEquipItemData.m_ImprintUnitId = itemEquip.m_ImprintUnitId;
				nKMEquipItemData.potentialOptions = itemEquip.potentialOptions;
				nKMEquipItemData.m_ItemUid = 0L;
				return nKMEquipItemData;
			}
		}
		return null;
	}

	private static void SetMaximumStatValue(ref EQUIP_ITEM_STAT stat, NKMEquipRandomStatTemplet randomStatTemplet)
	{
		if (randomStatTemplet.m_MaxStatValue > 0f)
		{
			stat.stat_value = randomStatTemplet.m_MaxStatValue;
		}
	}

	public static List<NKMEquipItemData> MakeTempEquipDataWithAllSet(int equipID)
	{
		List<NKMEquipItemData> list = new List<NKMEquipItemData>();
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(equipID);
		if (equipTemplet != null && equipTemplet.SetGroupList != null && equipTemplet.SetGroupList.Count > 0)
		{
			for (int i = 0; i < equipTemplet.SetGroupList.Count; i++)
			{
				NKMEquipItemData nKMEquipItemData = MakeTempEquipData(equipID, equipTemplet.SetGroupList[i]);
				nKMEquipItemData.m_ItemUid = equipID * 100 + i;
				list.Add(nKMEquipItemData);
			}
		}
		return list;
	}

	public void GetCurrentEquipList(ref List<NKMEquipItemData> currentList)
	{
		currentList = m_lstCurrentEquipList;
	}

	public static List<eSortOption> GetDefaultSortOption(NKCPopupEquipSort.SORT_OPEN_TYPE openType = NKCPopupEquipSort.SORT_OPEN_TYPE.NORMAL)
	{
		return openType switch
		{
			NKCPopupEquipSort.SORT_OPEN_TYPE.SELECTION => DEFAULT_EQUIP_SELECTION_SORT_LIST, 
			NKCPopupEquipSort.SORT_OPEN_TYPE.OPERATION_POWER => DEFAULT_EQUIP_OPERATION_POWER_SORT_LIST, 
			NKCPopupEquipSort.SORT_OPEN_TYPE.OPTION_WEIGHT => DEFAULT_EQUIP_OPTION_WEIGHT_SORT_LIST, 
			_ => DEFAULT_EQUIP_SORT_LIST, 
		};
	}

	public static List<eSortOption> AddDefaultSortOptions(List<eSortOption> sortOptions)
	{
		sortOptions.AddRange(GetDefaultSortOption());
		return sortOptions;
	}
}
