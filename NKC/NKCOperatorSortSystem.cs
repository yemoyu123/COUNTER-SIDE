using System;
using System.Collections.Generic;
using ClientPacket.WorldMap;
using NKC.Sort;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC;

public abstract class NKCOperatorSortSystem
{
	public enum FILTER_OPEN_TYPE
	{
		NONE,
		NORMAL,
		COLLECTION,
		SELECTION,
		ALLUNIT_DEV
	}

	public enum eFilterCategory
	{
		Rarity,
		Level,
		Decked,
		Locked,
		Have,
		Collected,
		PassiveSkill
	}

	public enum eFilterOption
	{
		Nothing,
		Everything,
		Rarily_SSR,
		Rarily_SR,
		Rarily_R,
		Rarily_N,
		Level_1,
		Level_other,
		Level_Max,
		Decked,
		NotDecked,
		Locked,
		Unlocked,
		Have,
		NotHave,
		Collected,
		NotCollected,
		PassiveSkill
	}

	public enum eSortCategory
	{
		None,
		Level,
		Rarity,
		UnitPower,
		UID,
		ID,
		IDX,
		UnitAttack,
		UnitHealth,
		UnitDefense,
		UnitReduceSkillCool,
		Decked,
		Custom1,
		Custom2,
		Custom3
	}

	public enum eSortOption
	{
		None,
		Level_Low,
		Level_High,
		Rarity_Low,
		Rarity_High,
		Power_Low,
		Power_High,
		UID_First,
		UID_Last,
		ID_First,
		ID_Last,
		IDX_First,
		IDX_Last,
		Attack_Low,
		Attack_High,
		Health_Low,
		Health_High,
		Unit_Defense_Low,
		Unit_Defense_High,
		Unit_ReduceSkillCool_Low,
		Unit_ReduceSkillCool_High,
		CustomAscend1,
		CustomDescend1,
		CustomAscend2,
		CustomDescend2,
		CustomAscend3,
		CustomDescend3
	}

	public struct OperatorListOptions
	{
		public delegate bool CustomFilterFunc(NKMOperator operatorData);

		public delegate NKCUnitSortSystem.eUnitState CustomUnitStateFunc(NKMOperator unitData);

		public NKM_DECK_TYPE eDeckType;

		public HashSet<int> setExcludeOperatorID;

		public HashSet<int> setOnlyIncludeOperatorID;

		public HashSet<int> setDuplicateOperatorID;

		public HashSet<long> setExcludeOperatorUID;

		public HashSet<eFilterOption> setOnlyIncludeFilterOption;

		public HashSet<eFilterOption> setFilterOption;

		public List<eSortOption> lstSortOption;

		public NKCUnitSortSystem.NKCDataComparerer<NKMOperator>.CompareFunc PreemptiveSortFunc;

		public Dictionary<eSortCategory, KeyValuePair<string, NKCUnitSortSystem.NKCDataComparerer<NKMOperator>.CompareFunc>> lstCustomSortFunc;

		public CustomFilterFunc AdditionalExcludeFilterFunc;

		public List<eSortOption> lstDefaultSortOption;

		public CustomUnitStateFunc AdditionalUnitStateFunc;

		public bool bHideDeckedUnit;

		public bool bIgnoreMissionState;

		public bool bHideFilter;

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

		public OperatorListOptions(NKM_DECK_TYPE deckType = NKM_DECK_TYPE.NDT_NONE)
		{
			eDeckType = deckType;
			m_BuildOptions = new HashSet<BUILD_OPTIONS>();
			m_BuildOptions.Add(BUILD_OPTIONS.DESCENDING);
			m_BuildOptions.Add(BUILD_OPTIONS.PUSHBACK_UNSELECTABLE);
			m_BuildOptions.Add(BUILD_OPTIONS.INCLUDE_UNDECKABLE_UNIT);
			lstSortOption = new List<eSortOption>();
			lstSortOption = AddDefaultSortOptions(lstSortOption, bIsCollection: false);
			lstDefaultSortOption = null;
			setFilterOption = new HashSet<eFilterOption>();
			lstCustomSortFunc = new Dictionary<eSortCategory, KeyValuePair<string, NKCUnitSortSystem.NKCDataComparerer<NKMOperator>.CompareFunc>>();
			setOnlyIncludeFilterOption = null;
			PreemptiveSortFunc = null;
			AdditionalExcludeFilterFunc = null;
			setExcludeOperatorUID = null;
			setExcludeOperatorID = null;
			setOnlyIncludeOperatorID = null;
			setDuplicateOperatorID = null;
			AdditionalUnitStateFunc = null;
			bIgnoreMissionState = false;
			bHideDeckedUnit = false;
			bHideFilter = false;
			passiveSkillID = 0;
		}

		public void MakeDuplicateSetFromAllDeck(NKMDeckIndex currentDeckIndex, long selectedOperUID, NKMArmyData armyData)
		{
			NKM_DECK_TYPE nKM_DECK_TYPE = currentDeckIndex.m_eDeckType;
			if (nKM_DECK_TYPE - 7 > NKM_DECK_TYPE.NDT_NORMAL)
			{
				return;
			}
			if (setDuplicateOperatorID == null)
			{
				setDuplicateOperatorID = new HashSet<int>();
			}
			IReadOnlyList<NKMDeckData> deckList = armyData.GetDeckList(currentDeckIndex.m_eDeckType);
			for (int i = 0; i < deckList.Count; i++)
			{
				NKMDeckData nKMDeckData = deckList[i];
				if (nKMDeckData == null)
				{
					break;
				}
				if (nKMDeckData.m_OperatorUID != selectedOperUID)
				{
					NKMOperator operatorFromUId = armyData.GetOperatorFromUId(nKMDeckData.m_OperatorUID);
					if (operatorFromUId != null)
					{
						setDuplicateOperatorID.Add(operatorFromUId.id);
					}
				}
			}
		}
	}

	private class OperatorInfoCache
	{
		public NKMDeckIndex DeckIndex;

		public NKMWorldMapManager.WorldMapLeaderState CityState;

		public NKCUnitSortSystem.eUnitState UnitSlotState;

		public int Power;

		public int Attack;

		public int HP;

		public int Defense;

		public int ReduceSkillCollTime;
	}

	public delegate bool AutoSelectExtraFilter(NKMOperator operatorData);

	private static readonly Dictionary<eSortCategory, Tuple<eSortOption, eSortOption>> s_dicSortCategory = new Dictionary<eSortCategory, Tuple<eSortOption, eSortOption>>
	{
		{
			eSortCategory.None,
			new Tuple<eSortOption, eSortOption>(eSortOption.None, eSortOption.None)
		},
		{
			eSortCategory.Level,
			new Tuple<eSortOption, eSortOption>(eSortOption.Level_Low, eSortOption.Level_High)
		},
		{
			eSortCategory.Rarity,
			new Tuple<eSortOption, eSortOption>(eSortOption.Rarity_Low, eSortOption.Rarity_High)
		},
		{
			eSortCategory.UnitPower,
			new Tuple<eSortOption, eSortOption>(eSortOption.Power_Low, eSortOption.Power_High)
		},
		{
			eSortCategory.UID,
			new Tuple<eSortOption, eSortOption>(eSortOption.UID_First, eSortOption.UID_Last)
		},
		{
			eSortCategory.ID,
			new Tuple<eSortOption, eSortOption>(eSortOption.ID_First, eSortOption.ID_Last)
		},
		{
			eSortCategory.IDX,
			new Tuple<eSortOption, eSortOption>(eSortOption.IDX_First, eSortOption.IDX_Last)
		},
		{
			eSortCategory.UnitAttack,
			new Tuple<eSortOption, eSortOption>(eSortOption.Attack_Low, eSortOption.Attack_High)
		},
		{
			eSortCategory.UnitHealth,
			new Tuple<eSortOption, eSortOption>(eSortOption.Health_Low, eSortOption.Health_High)
		},
		{
			eSortCategory.UnitDefense,
			new Tuple<eSortOption, eSortOption>(eSortOption.Unit_Defense_Low, eSortOption.Unit_Defense_High)
		},
		{
			eSortCategory.UnitReduceSkillCool,
			new Tuple<eSortOption, eSortOption>(eSortOption.Unit_ReduceSkillCool_Low, eSortOption.Unit_ReduceSkillCool_High)
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

	private static readonly List<eSortOption> DEFAULT_OPERATOR_SORT_OPTION_LIST = new List<eSortOption>
	{
		eSortOption.Level_High,
		eSortOption.Power_High,
		eSortOption.Level_High,
		eSortOption.Rarity_High,
		eSortOption.UID_Last
	};

	private static readonly List<eSortOption> DEFAULT_UNIT_SELECTION_SORT_OPTION_LIST = new List<eSortOption>
	{
		eSortOption.Rarity_High,
		eSortOption.IDX_First
	};

	private static readonly List<eSortOption> DEFAULT_UNIT_COLLECTION_SORT_OPTION_LIST = new List<eSortOption> { eSortOption.IDX_First };

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Rarity = new HashSet<eFilterOption>
	{
		eFilterOption.Rarily_SSR,
		eFilterOption.Rarily_SR,
		eFilterOption.Rarily_R,
		eFilterOption.Rarily_N
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Level = new HashSet<eFilterOption>
	{
		eFilterOption.Level_1,
		eFilterOption.Level_other,
		eFilterOption.Level_Max
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Decked = new HashSet<eFilterOption>
	{
		eFilterOption.Decked,
		eFilterOption.NotDecked
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Locked = new HashSet<eFilterOption>
	{
		eFilterOption.Locked,
		eFilterOption.Unlocked
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Have = new HashSet<eFilterOption>
	{
		eFilterOption.Have,
		eFilterOption.NotHave
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Collected = new HashSet<eFilterOption>
	{
		eFilterOption.Collected,
		eFilterOption.NotCollected
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_PassiveSkill = new HashSet<eFilterOption> { eFilterOption.PassiveSkill };

	private static readonly List<HashSet<eFilterOption>> m_lstFilterCategory = new List<HashSet<eFilterOption>> { m_setFilterCategory_Rarity, m_setFilterCategory_Level, m_setFilterCategory_Decked, m_setFilterCategory_Locked, m_setFilterCategory_Have, m_setFilterCategory_Collected, m_setFilterCategory_PassiveSkill };

	public static readonly HashSet<eFilterCategory> setDefaultOperatorFilterCategory = new HashSet<eFilterCategory>
	{
		eFilterCategory.Rarity,
		eFilterCategory.Locked
	};

	public static readonly HashSet<eSortCategory> setDefaultOperatorSortCategory = new HashSet<eSortCategory>
	{
		eSortCategory.Level,
		eSortCategory.Rarity,
		eSortCategory.UID,
		eSortCategory.UnitPower,
		eSortCategory.UnitAttack,
		eSortCategory.UnitHealth,
		eSortCategory.UnitDefense,
		eSortCategory.UnitReduceSkillCool
	};

	protected OperatorListOptions m_Options;

	protected Dictionary<long, NKMOperator> m_dicAllOperatorList;

	protected List<NKMOperator> m_lstCurrentOperatorList;

	protected NKCOperatorSortSystemFilterTokens m_FilterToken = new NKCOperatorSortSystemFilterTokens();

	private Dictionary<long, OperatorInfoCache> m_dicOperatorInfoCache = new Dictionary<long, OperatorInfoCache>();

	private Dictionary<int, HashSet<int>> m_dicDeckedOperatorIdCache = new Dictionary<int, HashSet<int>>();

	public static List<Tuple<eFilterCategory, NKCUnitSortSystem.eFilterCategory>> m_listFilterCategory = new List<Tuple<eFilterCategory, NKCUnitSortSystem.eFilterCategory>>
	{
		new Tuple<eFilterCategory, NKCUnitSortSystem.eFilterCategory>(eFilterCategory.Rarity, NKCUnitSortSystem.eFilterCategory.Rarity),
		new Tuple<eFilterCategory, NKCUnitSortSystem.eFilterCategory>(eFilterCategory.Level, NKCUnitSortSystem.eFilterCategory.Level),
		new Tuple<eFilterCategory, NKCUnitSortSystem.eFilterCategory>(eFilterCategory.Decked, NKCUnitSortSystem.eFilterCategory.Decked),
		new Tuple<eFilterCategory, NKCUnitSortSystem.eFilterCategory>(eFilterCategory.Locked, NKCUnitSortSystem.eFilterCategory.Locked),
		new Tuple<eFilterCategory, NKCUnitSortSystem.eFilterCategory>(eFilterCategory.Have, NKCUnitSortSystem.eFilterCategory.Have),
		new Tuple<eFilterCategory, NKCUnitSortSystem.eFilterCategory>(eFilterCategory.Collected, NKCUnitSortSystem.eFilterCategory.Collected)
	};

	public static List<Tuple<eFilterOption, NKCUnitSortSystem.eFilterOption>> m_listFilterOptions = new List<Tuple<eFilterOption, NKCUnitSortSystem.eFilterOption>>
	{
		new Tuple<eFilterOption, NKCUnitSortSystem.eFilterOption>(eFilterOption.Everything, NKCUnitSortSystem.eFilterOption.Everything),
		new Tuple<eFilterOption, NKCUnitSortSystem.eFilterOption>(eFilterOption.Nothing, NKCUnitSortSystem.eFilterOption.Nothing),
		new Tuple<eFilterOption, NKCUnitSortSystem.eFilterOption>(eFilterOption.Rarily_SSR, NKCUnitSortSystem.eFilterOption.Rarily_SSR),
		new Tuple<eFilterOption, NKCUnitSortSystem.eFilterOption>(eFilterOption.Rarily_SR, NKCUnitSortSystem.eFilterOption.Rarily_SR),
		new Tuple<eFilterOption, NKCUnitSortSystem.eFilterOption>(eFilterOption.Rarily_R, NKCUnitSortSystem.eFilterOption.Rarily_R),
		new Tuple<eFilterOption, NKCUnitSortSystem.eFilterOption>(eFilterOption.Rarily_N, NKCUnitSortSystem.eFilterOption.Rarily_N),
		new Tuple<eFilterOption, NKCUnitSortSystem.eFilterOption>(eFilterOption.Level_1, NKCUnitSortSystem.eFilterOption.Level_1),
		new Tuple<eFilterOption, NKCUnitSortSystem.eFilterOption>(eFilterOption.Level_other, NKCUnitSortSystem.eFilterOption.Level_other),
		new Tuple<eFilterOption, NKCUnitSortSystem.eFilterOption>(eFilterOption.Level_Max, NKCUnitSortSystem.eFilterOption.Level_Max),
		new Tuple<eFilterOption, NKCUnitSortSystem.eFilterOption>(eFilterOption.Decked, NKCUnitSortSystem.eFilterOption.Decked),
		new Tuple<eFilterOption, NKCUnitSortSystem.eFilterOption>(eFilterOption.NotDecked, NKCUnitSortSystem.eFilterOption.NotDecked),
		new Tuple<eFilterOption, NKCUnitSortSystem.eFilterOption>(eFilterOption.Locked, NKCUnitSortSystem.eFilterOption.Locked),
		new Tuple<eFilterOption, NKCUnitSortSystem.eFilterOption>(eFilterOption.Unlocked, NKCUnitSortSystem.eFilterOption.Unlocked),
		new Tuple<eFilterOption, NKCUnitSortSystem.eFilterOption>(eFilterOption.Have, NKCUnitSortSystem.eFilterOption.Have),
		new Tuple<eFilterOption, NKCUnitSortSystem.eFilterOption>(eFilterOption.NotHave, NKCUnitSortSystem.eFilterOption.NotHave),
		new Tuple<eFilterOption, NKCUnitSortSystem.eFilterOption>(eFilterOption.Collected, NKCUnitSortSystem.eFilterOption.Collected),
		new Tuple<eFilterOption, NKCUnitSortSystem.eFilterOption>(eFilterOption.NotCollected, NKCUnitSortSystem.eFilterOption.NotCollected)
	};

	public static List<Tuple<eSortCategory, NKCUnitSortSystem.eSortCategory>> m_listSortCategory = new List<Tuple<eSortCategory, NKCUnitSortSystem.eSortCategory>>
	{
		new Tuple<eSortCategory, NKCUnitSortSystem.eSortCategory>(eSortCategory.None, NKCUnitSortSystem.eSortCategory.None),
		new Tuple<eSortCategory, NKCUnitSortSystem.eSortCategory>(eSortCategory.Level, NKCUnitSortSystem.eSortCategory.Level),
		new Tuple<eSortCategory, NKCUnitSortSystem.eSortCategory>(eSortCategory.Rarity, NKCUnitSortSystem.eSortCategory.Rarity),
		new Tuple<eSortCategory, NKCUnitSortSystem.eSortCategory>(eSortCategory.UnitPower, NKCUnitSortSystem.eSortCategory.UnitPower),
		new Tuple<eSortCategory, NKCUnitSortSystem.eSortCategory>(eSortCategory.UID, NKCUnitSortSystem.eSortCategory.UID),
		new Tuple<eSortCategory, NKCUnitSortSystem.eSortCategory>(eSortCategory.ID, NKCUnitSortSystem.eSortCategory.ID),
		new Tuple<eSortCategory, NKCUnitSortSystem.eSortCategory>(eSortCategory.IDX, NKCUnitSortSystem.eSortCategory.IDX),
		new Tuple<eSortCategory, NKCUnitSortSystem.eSortCategory>(eSortCategory.UnitAttack, NKCUnitSortSystem.eSortCategory.UnitAttack),
		new Tuple<eSortCategory, NKCUnitSortSystem.eSortCategory>(eSortCategory.UnitHealth, NKCUnitSortSystem.eSortCategory.UnitHealth),
		new Tuple<eSortCategory, NKCUnitSortSystem.eSortCategory>(eSortCategory.UnitDefense, NKCUnitSortSystem.eSortCategory.UnitDefense),
		new Tuple<eSortCategory, NKCUnitSortSystem.eSortCategory>(eSortCategory.UnitReduceSkillCool, NKCUnitSortSystem.eSortCategory.UnitReduceSkillCool),
		new Tuple<eSortCategory, NKCUnitSortSystem.eSortCategory>(eSortCategory.Decked, NKCUnitSortSystem.eSortCategory.Decked),
		new Tuple<eSortCategory, NKCUnitSortSystem.eSortCategory>(eSortCategory.Custom1, NKCUnitSortSystem.eSortCategory.Custom1),
		new Tuple<eSortCategory, NKCUnitSortSystem.eSortCategory>(eSortCategory.Custom2, NKCUnitSortSystem.eSortCategory.Custom2),
		new Tuple<eSortCategory, NKCUnitSortSystem.eSortCategory>(eSortCategory.Custom3, NKCUnitSortSystem.eSortCategory.Custom3)
	};

	public static List<Tuple<eSortOption, NKCUnitSortSystem.eSortOption>> m_listSortOptions = new List<Tuple<eSortOption, NKCUnitSortSystem.eSortOption>>
	{
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.None, NKCUnitSortSystem.eSortOption.None),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.Level_Low, NKCUnitSortSystem.eSortOption.Level_Low),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.Level_High, NKCUnitSortSystem.eSortOption.Level_High),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.Rarity_Low, NKCUnitSortSystem.eSortOption.Rarity_Low),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.Rarity_High, NKCUnitSortSystem.eSortOption.Rarity_High),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.Power_Low, NKCUnitSortSystem.eSortOption.Power_Low),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.Power_High, NKCUnitSortSystem.eSortOption.Power_High),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.UID_First, NKCUnitSortSystem.eSortOption.UID_First),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.UID_Last, NKCUnitSortSystem.eSortOption.UID_Last),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.ID_First, NKCUnitSortSystem.eSortOption.ID_First),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.ID_Last, NKCUnitSortSystem.eSortOption.ID_Last),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.IDX_First, NKCUnitSortSystem.eSortOption.IDX_First),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.IDX_Last, NKCUnitSortSystem.eSortOption.IDX_Last),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.Attack_Low, NKCUnitSortSystem.eSortOption.Attack_Low),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.Attack_High, NKCUnitSortSystem.eSortOption.Attack_High),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.Health_Low, NKCUnitSortSystem.eSortOption.Health_Low),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.Health_High, NKCUnitSortSystem.eSortOption.Health_High),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.Unit_Defense_Low, NKCUnitSortSystem.eSortOption.Unit_Defense_Low),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.Unit_Defense_High, NKCUnitSortSystem.eSortOption.Unit_Defense_High),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.Unit_ReduceSkillCool_Low, NKCUnitSortSystem.eSortOption.Unit_ReduceSkillCool_Low),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.Unit_ReduceSkillCool_High, NKCUnitSortSystem.eSortOption.Unit_ReduceSkillCool_High),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.CustomAscend1, NKCUnitSortSystem.eSortOption.CustomAscend1),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.CustomDescend1, NKCUnitSortSystem.eSortOption.CustomDescend1),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.CustomAscend2, NKCUnitSortSystem.eSortOption.CustomAscend2),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.CustomDescend2, NKCUnitSortSystem.eSortOption.CustomDescend2),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.CustomAscend3, NKCUnitSortSystem.eSortOption.CustomAscend3),
		new Tuple<eSortOption, NKCUnitSortSystem.eSortOption>(eSortOption.CustomDescend3, NKCUnitSortSystem.eSortOption.CustomDescend3)
	};

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

	public OperatorListOptions Options
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
			FilterList(m_Options.setFilterOption, m_Options.bHideDeckedUnit);
		}
	}

	public List<NKMOperator> SortedOperatorList
	{
		get
		{
			if (m_lstCurrentOperatorList == null)
			{
				if (m_Options.setFilterOption == null)
				{
					m_Options.setFilterOption = new HashSet<eFilterOption>();
					FilterList(m_Options.setFilterOption, m_Options.IsHasBuildOption(BUILD_OPTIONS.HIDE_DECKED_UNIT));
				}
				else
				{
					FilterList(m_Options.setFilterOption, m_Options.IsHasBuildOption(BUILD_OPTIONS.HIDE_DECKED_UNIT));
				}
			}
			return m_lstCurrentOperatorList;
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
			FilterList(value, m_Options.IsHasBuildOption(BUILD_OPTIONS.HIDE_DECKED_UNIT));
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
				m_Options.lstSortOption = GetDefaultSortOptions(bIsCollection: false);
				m_Options.SetBuildOption(value, BUILD_OPTIONS.DESCENDING);
				SortList(m_Options.lstSortOption);
			}
		}
	}

	public bool bHideDeckedUnit
	{
		get
		{
			return m_Options.IsHasBuildOption(BUILD_OPTIONS.HIDE_DECKED_UNIT);
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

	protected abstract IEnumerable<NKMOperator> GetTargetOperatorList(NKMUserData userData);

	private NKCOperatorSortSystem()
	{
	}

	public NKCOperatorSortSystem(NKMUserData userData, OperatorListOptions options)
	{
		m_Options = options;
		m_FilterToken.Clear();
		BuildUnitStateCache(userData, options.eDeckType);
		m_dicAllOperatorList = BuildFullUnitList(userData, GetTargetOperatorList(userData), options);
	}

	public NKCOperatorSortSystem(NKMUserData userData, OperatorListOptions options, IEnumerable<NKMOperator> lstOperatorData)
	{
		m_Options = options;
		m_FilterToken.Clear();
		BuildUnitStateCache(userData, lstOperatorData, options.eDeckType);
		m_dicAllOperatorList = BuildFullUnitList(userData, lstOperatorData, options);
	}

	public NKCOperatorSortSystem(NKMUserData userData, OperatorListOptions options, bool local)
	{
		m_Options = options;
		m_FilterToken.Clear();
		if (local)
		{
			BuildLocalUnitStateCache(userData, options.eDeckType);
		}
		else
		{
			BuildUnitStateCache(userData, options.eDeckType);
		}
		m_dicAllOperatorList = BuildFullUnitList(userData, GetTargetOperatorList(userData), options);
	}

	public virtual void BuildFilterAndSortedList(HashSet<eFilterOption> setfilterType, List<eSortOption> lstSortOption, bool bHideDeckedOperator)
	{
		m_Options.SetBuildOption(bHideDeckedOperator, BUILD_OPTIONS.HIDE_DECKED_UNIT);
		m_Options.setFilterOption = setfilterType;
		m_Options.lstSortOption = lstSortOption;
		FilterList(setfilterType, bHideDeckedOperator);
	}

	public void SetDeckIndexCache(long uid, NKMDeckIndex deckindex)
	{
		if (m_dicOperatorInfoCache.ContainsKey(uid))
		{
			m_dicOperatorInfoCache[uid].DeckIndex = deckindex;
			return;
		}
		OperatorInfoCache operatorInfoCache = new OperatorInfoCache();
		operatorInfoCache.DeckIndex = deckindex;
		m_dicOperatorInfoCache.Add(uid, operatorInfoCache);
	}

	public NKMDeckIndex GetDeckIndexCache(long uid, bool bTargetDecktypeOnly = false)
	{
		if (m_dicOperatorInfoCache.ContainsKey(uid))
		{
			if (bTargetDecktypeOnly)
			{
				if (m_Options.eDeckType == m_dicOperatorInfoCache[uid].DeckIndex.m_eDeckType)
				{
					return m_dicOperatorInfoCache[uid].DeckIndex;
				}
				return NKMDeckIndex.None;
			}
			return m_dicOperatorInfoCache[uid].DeckIndex;
		}
		return NKMDeckIndex.None;
	}

	private void SetUnitAttackCache(long uid, int atk)
	{
		if (m_dicOperatorInfoCache.ContainsKey(uid))
		{
			m_dicOperatorInfoCache[uid].Attack = atk;
			return;
		}
		OperatorInfoCache operatorInfoCache = new OperatorInfoCache();
		operatorInfoCache.Attack = atk;
		m_dicOperatorInfoCache.Add(uid, operatorInfoCache);
	}

	public int GetUnitAttackCache(long uid)
	{
		if (m_dicOperatorInfoCache.ContainsKey(uid))
		{
			return m_dicOperatorInfoCache[uid].Attack;
		}
		return 0;
	}

	private void SetUnitHPCache(long uid, int hp)
	{
		if (m_dicOperatorInfoCache.ContainsKey(uid))
		{
			m_dicOperatorInfoCache[uid].HP = hp;
			return;
		}
		OperatorInfoCache operatorInfoCache = new OperatorInfoCache();
		operatorInfoCache.HP = hp;
		m_dicOperatorInfoCache.Add(uid, operatorInfoCache);
	}

	public int GetUnitHPCache(long uid)
	{
		if (m_dicOperatorInfoCache.ContainsKey(uid))
		{
			return m_dicOperatorInfoCache[uid].HP;
		}
		return 0;
	}

	private void SetUnitDefCache(long uid, int def)
	{
		if (m_dicOperatorInfoCache.ContainsKey(uid))
		{
			m_dicOperatorInfoCache[uid].Defense = def;
			return;
		}
		OperatorInfoCache operatorInfoCache = new OperatorInfoCache();
		operatorInfoCache.Defense = def;
		m_dicOperatorInfoCache.Add(uid, operatorInfoCache);
	}

	public int GetUnitDefCache(long uid)
	{
		if (m_dicOperatorInfoCache.ContainsKey(uid))
		{
			return m_dicOperatorInfoCache[uid].Defense;
		}
		return 0;
	}

	private void SetUnitSkillCoolCache(long uid, int ReduceSkillColl)
	{
		if (m_dicOperatorInfoCache.ContainsKey(uid))
		{
			m_dicOperatorInfoCache[uid].ReduceSkillCollTime = ReduceSkillColl;
			return;
		}
		OperatorInfoCache operatorInfoCache = new OperatorInfoCache();
		operatorInfoCache.ReduceSkillCollTime = ReduceSkillColl;
		m_dicOperatorInfoCache.Add(uid, operatorInfoCache);
	}

	public int GetUnitSkillCoolCache(long uid)
	{
		if (m_dicOperatorInfoCache.ContainsKey(uid))
		{
			return m_dicOperatorInfoCache[uid].ReduceSkillCollTime;
		}
		return 0;
	}

	private void SetUnitPowerCache(long uid, int Power)
	{
		if (m_dicOperatorInfoCache.ContainsKey(uid))
		{
			m_dicOperatorInfoCache[uid].Power = Power;
			return;
		}
		OperatorInfoCache operatorInfoCache = new OperatorInfoCache();
		operatorInfoCache.Power = Power;
		m_dicOperatorInfoCache.Add(uid, operatorInfoCache);
	}

	public int GetUnitPowerCache(long uid)
	{
		if (m_dicOperatorInfoCache.ContainsKey(uid))
		{
			return m_dicOperatorInfoCache[uid].Power;
		}
		return 0;
	}

	public NKMWorldMapManager.WorldMapLeaderState GetCityStateCache(long uid)
	{
		if (m_dicOperatorInfoCache.ContainsKey(uid))
		{
			return m_dicOperatorInfoCache[uid].CityState;
		}
		return NKMWorldMapManager.WorldMapLeaderState.None;
	}

	private void SetUnitSlotState(long uid, NKCUnitSortSystem.eUnitState state)
	{
		if (m_dicOperatorInfoCache.ContainsKey(uid))
		{
			m_dicOperatorInfoCache[uid].UnitSlotState = state;
			return;
		}
		OperatorInfoCache operatorInfoCache = new OperatorInfoCache();
		operatorInfoCache.UnitSlotState = state;
		m_dicOperatorInfoCache.Add(uid, operatorInfoCache);
	}

	public NKCUnitSortSystem.eUnitState GetUnitSlotState(long uid)
	{
		if (m_dicOperatorInfoCache.ContainsKey(uid))
		{
			return m_dicOperatorInfoCache[uid].UnitSlotState;
		}
		return NKCUnitSortSystem.eUnitState.NONE;
	}

	protected virtual void BuildUnitStateCache(NKMUserData userData, IEnumerable<NKMOperator> lstOperatorData, NKM_DECK_TYPE eNKM_DECK_TYPE)
	{
		m_dicOperatorInfoCache.Clear();
		m_dicDeckedOperatorIdCache.Clear();
		if (lstOperatorData == null || eNKM_DECK_TYPE == NKM_DECK_TYPE.NDT_NONE)
		{
			return;
		}
		if (userData != null)
		{
			NKMArmyData armyData = userData.m_ArmyData;
			foreach (KeyValuePair<NKMDeckIndex, NKMDeckData> allDeck in armyData.GetAllDecks())
			{
				NKMDeckData value = allDeck.Value;
				if (!armyData.m_dicMyOperator.ContainsKey(value.m_OperatorUID))
				{
					continue;
				}
				if (m_dicOperatorInfoCache.ContainsKey(value.m_OperatorUID))
				{
					if (m_dicOperatorInfoCache[value.m_OperatorUID].DeckIndex.m_eDeckType != eNKM_DECK_TYPE)
					{
						SetDeckIndexCache(value.m_OperatorUID, allDeck.Key);
					}
				}
				else
				{
					SetDeckIndexCache(value.m_OperatorUID, allDeck.Key);
				}
			}
		}
		foreach (NKMOperator lstOperatorDatum in lstOperatorData)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(lstOperatorDatum.id);
			NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(lstOperatorDatum.id);
			if (unitTempletBase != null && unitStatTemplet != null)
			{
				NKMStatData targetStatData = new NKMStatData();
				targetStatData.Init();
				MakeOperatorBaseStat(ref targetStatData, lstOperatorDatum, unitStatTemplet.m_StatData);
				SetUnitAttackCache(lstOperatorDatum.uid, (int)targetStatData.GetStatBase(NKM_STAT_TYPE.NST_ATK));
				SetUnitHPCache(lstOperatorDatum.uid, (int)targetStatData.GetStatBase(NKM_STAT_TYPE.NST_HP));
				SetUnitDefCache(lstOperatorDatum.uid, (int)targetStatData.GetStatBase(NKM_STAT_TYPE.NST_DEF));
				SetUnitSkillCoolCache(lstOperatorDatum.uid, (int)targetStatData.GetStatBase(NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE));
			}
		}
	}

	public void MakeOperatorBaseStat(ref NKMStatData targetStatData, NKMOperator operatorData, NKMStatData sourceStatData)
	{
		targetStatData.DeepCopyFromSource(sourceStatData);
		targetStatData.SetStatBase(NKM_STAT_TYPE.NST_ATK, CalculateOperatorStat(NKM_STAT_TYPE.NST_ATK, sourceStatData, operatorData.level));
		targetStatData.SetStatBase(NKM_STAT_TYPE.NST_DEF, CalculateOperatorStat(NKM_STAT_TYPE.NST_DEF, sourceStatData, operatorData.level));
		targetStatData.SetStatBase(NKM_STAT_TYPE.NST_HP, CalculateOperatorStat(NKM_STAT_TYPE.NST_HP, sourceStatData, operatorData.level));
		targetStatData.SetStatBase(NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE, CalculateOperatorStat(NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE, sourceStatData, operatorData.level));
	}

	public float CalculateOperatorStat(NKM_STAT_TYPE type, NKMStatData unitStatData, int level)
	{
		if (unitStatData == null)
		{
			return 0f;
		}
		return unitStatData.GetStatBase(type) + unitStatData.GetStatPerLevel(type) * (float)(level - 1);
	}

	protected virtual void BuildUnitStateCache(NKMUserData userData, NKM_DECK_TYPE eNKM_DECK_TYPE)
	{
		m_dicOperatorInfoCache.Clear();
		m_dicDeckedOperatorIdCache.Clear();
		if (userData == null || eNKM_DECK_TYPE == NKM_DECK_TYPE.NDT_NONE)
		{
			return;
		}
		NKMArmyData armyData = userData.m_ArmyData;
		foreach (KeyValuePair<NKMDeckIndex, NKMDeckData> allDeck in armyData.GetAllDecks())
		{
			NKMDeckData value = allDeck.Value;
			if (!armyData.m_dicMyOperator.ContainsKey(value.m_OperatorUID))
			{
				continue;
			}
			if (m_dicOperatorInfoCache.ContainsKey(value.m_OperatorUID))
			{
				if (m_dicOperatorInfoCache[value.m_OperatorUID].DeckIndex.m_eDeckType != eNKM_DECK_TYPE)
				{
					SetDeckIndexCache(value.m_OperatorUID, allDeck.Key);
				}
			}
			else
			{
				SetDeckIndexCache(value.m_OperatorUID, allDeck.Key);
			}
		}
		foreach (NKMOperator value2 in armyData.m_dicMyOperator.Values)
		{
			NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(value2.id);
			if (unitStatTemplet != null)
			{
				NKMStatData targetStatData = new NKMStatData();
				targetStatData.Init();
				MakeOperatorBaseStat(ref targetStatData, value2, unitStatTemplet.m_StatData);
				SetUnitPowerCache(value2.uid, value2.CalculateOperatorOperationPower());
				SetUnitAttackCache(value2.uid, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_ATK, targetStatData));
				SetUnitHPCache(value2.uid, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_HP, targetStatData));
				SetUnitDefCache(value2.uid, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_DEF, targetStatData));
				SetUnitSkillCoolCache(value2.uid, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE, targetStatData));
			}
		}
	}

	private void SetDeckedOperatorIdCache(int operatorId, int deckIndex)
	{
		if (!m_dicDeckedOperatorIdCache.ContainsKey(operatorId))
		{
			m_dicDeckedOperatorIdCache.Add(operatorId, new HashSet<int>());
		}
		m_dicDeckedOperatorIdCache[operatorId].Add(deckIndex);
	}

	public bool IsDeckedOperatorId(int operatorId)
	{
		if (m_dicDeckedOperatorIdCache.ContainsKey(operatorId))
		{
			return m_dicDeckedOperatorIdCache[operatorId].Count > 0;
		}
		return false;
	}

	protected virtual void BuildLocalUnitStateCache(NKMUserData userData, NKM_DECK_TYPE eNKM_DECK_TYPE)
	{
		m_dicOperatorInfoCache.Clear();
		m_dicDeckedOperatorIdCache.Clear();
		if (userData == null || eNKM_DECK_TYPE == NKM_DECK_TYPE.NDT_NONE)
		{
			return;
		}
		Dictionary<int, NKMEventDeckData> allLocalDeckData = NKCLocalDeckDataManager.GetAllLocalDeckData();
		NKMArmyData armyData = userData.m_ArmyData;
		foreach (KeyValuePair<int, NKMEventDeckData> item in allLocalDeckData)
		{
			long operatorUID = item.Value.m_OperatorUID;
			if (armyData.m_dicMyOperator.ContainsKey(operatorUID))
			{
				NKMDeckIndex deckindex = new NKMDeckIndex(eNKM_DECK_TYPE, item.Key);
				SetDeckIndexCache(operatorUID, deckindex);
			}
			NKMOperator operatorFromUId = armyData.GetOperatorFromUId(operatorUID);
			if (operatorFromUId != null)
			{
				SetDeckedOperatorIdCache(operatorFromUId.id, item.Key);
			}
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(operatorFromUId);
			if (unitTempletBase != null && unitTempletBase.m_BaseUnitID > 0)
			{
				SetDeckedOperatorIdCache(unitTempletBase.m_BaseUnitID, item.Key);
			}
		}
		foreach (NKMOperator value in armyData.m_dicMyOperator.Values)
		{
			NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(value.id);
			if (unitStatTemplet != null)
			{
				NKMStatData targetStatData = new NKMStatData();
				targetStatData.Init();
				MakeOperatorBaseStat(ref targetStatData, value, unitStatTemplet.m_StatData);
				SetUnitPowerCache(value.uid, value.CalculateOperatorOperationPower());
				SetUnitAttackCache(value.uid, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_ATK, targetStatData));
				SetUnitHPCache(value.uid, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_HP, targetStatData));
				SetUnitDefCache(value.uid, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_DEF, targetStatData));
				SetUnitSkillCoolCache(value.uid, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE, targetStatData));
			}
		}
	}

	private Dictionary<long, NKMOperator> BuildFullUnitList(NKMUserData userData, IEnumerable<NKMOperator> lstTargetOperators, OperatorListOptions options)
	{
		Dictionary<long, NKMOperator> dictionary = new Dictionary<long, NKMOperator>();
		HashSet<int> setOnlyIncludeOperatorID = options.setOnlyIncludeOperatorID;
		HashSet<int> setExcludeOperatorID = options.setExcludeOperatorID;
		foreach (NKMOperator lstTargetOperator in lstTargetOperators)
		{
			long uid = lstTargetOperator.uid;
			if ((options.AdditionalExcludeFilterFunc != null && !options.AdditionalExcludeFilterFunc(lstTargetOperator)) || (options.setExcludeOperatorUID != null && options.setExcludeOperatorUID.Contains(uid)) || (options.IsHasBuildOption(BUILD_OPTIONS.EXCLUDE_DECKED_UNIT) && (GetDeckIndexCache(uid).m_eDeckType != NKM_DECK_TYPE.NDT_NONE || GetCityStateCache(uid) != NKMWorldMapManager.WorldMapLeaderState.None || IsMainUnit(uid, userData))) || (options.IsHasBuildOption(BUILD_OPTIONS.EXCLUDE_LOCKED_UNIT) && lstTargetOperator.bLock) || (setOnlyIncludeOperatorID != null && !setOnlyIncludeOperatorID.Contains(lstTargetOperator.id)) || (options.setOnlyIncludeFilterOption != null && !CheckFilter(lstTargetOperator, options.setOnlyIncludeFilterOption)))
			{
				continue;
			}
			NKCUnitSortSystem.eUnitState state = NKCUnitSortSystem.eUnitState.NONE;
			if (userData != null)
			{
				NKMDeckIndex deckIndexCache = GetDeckIndexCache(uid, !m_Options.IsHasBuildOption(BUILD_OPTIONS.USE_DECKED_STATE));
				GetCityStateCache(uid);
				NKM_DECK_STATE operatorDeckState = userData.m_ArmyData.GetOperatorDeckState(lstTargetOperator);
				if (options.setDuplicateOperatorID != null && options.setDuplicateOperatorID.Contains(lstTargetOperator.id))
				{
					state = NKCUnitSortSystem.eUnitState.DUPLICATE;
				}
				else
				{
					switch (operatorDeckState)
					{
					case NKM_DECK_STATE.DECK_STATE_WARFARE:
						state = NKCUnitSortSystem.eUnitState.WARFARE_BATCH;
						break;
					case NKM_DECK_STATE.DECK_STATE_DIVE:
						state = NKCUnitSortSystem.eUnitState.DIVE_BATCH;
						break;
					default:
						if (!options.bIgnoreMissionState && !options.IsHasBuildOption(BUILD_OPTIONS.IGNORE_CITY_STATE) && operatorDeckState == NKM_DECK_STATE.DECK_STATE_WORLDMAP_MISSION)
						{
							state = NKCUnitSortSystem.eUnitState.CITY_MISSION;
						}
						else if (options.IsHasBuildOption(BUILD_OPTIONS.USE_DECKED_STATE) && deckIndexCache.m_eDeckType != NKM_DECK_TYPE.NDT_NONE)
						{
							state = NKCUnitSortSystem.eUnitState.DECKED;
						}
						else if (options.IsHasBuildOption(BUILD_OPTIONS.USE_DECKED_STATE) && IsMainUnit(uid, userData))
						{
							state = NKCUnitSortSystem.eUnitState.MAINUNIT;
						}
						else if (options.IsHasBuildOption(BUILD_OPTIONS.USE_LOCKED_STATE) && lstTargetOperator.bLock)
						{
							state = NKCUnitSortSystem.eUnitState.LOCKED;
						}
						else if (!options.IsHasBuildOption(BUILD_OPTIONS.USE_LOBBY_STATE) || !IsMainUnit(uid, userData))
						{
							state = ((m_Options.AdditionalUnitStateFunc != null) ? m_Options.AdditionalUnitStateFunc(lstTargetOperator) : NKCUnitSortSystem.eUnitState.NONE);
						}
						else if (userData.GetBackgroundUnitIndex(uid) >= 0)
						{
							state = NKCUnitSortSystem.eUnitState.LOBBY_UNIT;
						}
						break;
					}
				}
			}
			SetUnitSlotState(uid, state);
			dictionary.Add(uid, lstTargetOperator);
		}
		return dictionary;
	}

	private bool IsMainUnit(long unitUID, NKMUserData userData)
	{
		if (userData == null)
		{
			return false;
		}
		if (userData.GetBackgroundUnitIndex(unitUID) >= 0)
		{
			return true;
		}
		return false;
	}

	protected bool FilterData(NKMOperator operatorData, List<HashSet<eFilterOption>> setFilter)
	{
		if (m_Options.IsHasBuildOption(BUILD_OPTIONS.HIDE_DECKED_UNIT))
		{
			if (!m_Options.IsHasBuildOption(BUILD_OPTIONS.IGNORE_CITY_STATE) && IsUnitIsCityLeaderOnMission(operatorData.uid))
			{
				return false;
			}
			if (GetDeckIndexCache(operatorData.uid).m_eDeckType == m_Options.eDeckType)
			{
				return false;
			}
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(operatorData.id);
		if (!m_Options.IsHasBuildOption(BUILD_OPTIONS.INCLUDE_UNDECKABLE_UNIT) && !NKMUnitManager.CanUnitUsedInDeck(unitTempletBase))
		{
			return false;
		}
		for (int i = 0; i < setFilter.Count; i++)
		{
			if (!CheckFilter(operatorData, setFilter[i]))
			{
				return false;
			}
		}
		if (!m_FilterToken.CheckFilter(operatorData, m_Options))
		{
			return false;
		}
		return true;
	}

	protected bool FilterData(NKMOperator operatorData, NKM_UNIT_STYLE_TYPE targetType)
	{
		if (m_Options.IsHasBuildOption(BUILD_OPTIONS.HIDE_DECKED_UNIT))
		{
			if (GetCityStateCache(operatorData.uid) != NKMWorldMapManager.WorldMapLeaderState.None)
			{
				return false;
			}
			if (GetDeckIndexCache(operatorData.uid).m_eDeckType == m_Options.eDeckType)
			{
				return false;
			}
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(operatorData.id);
		if (!m_Options.IsHasBuildOption(BUILD_OPTIONS.INCLUDE_UNDECKABLE_UNIT) && !NKMUnitManager.CanUnitUsedInDeck(unitTempletBase))
		{
			return false;
		}
		if (targetType != NKM_UNIT_STYLE_TYPE.NUST_INVALID && unitTempletBase.m_NKM_UNIT_STYLE_TYPE != targetType)
		{
			return false;
		}
		return true;
	}

	protected bool IsOperatorSelectable(NKMOperator operatorData)
	{
		switch (GetUnitSlotState(operatorData.uid))
		{
		default:
			return false;
		case NKCUnitSortSystem.eUnitState.NONE:
		case NKCUnitSortSystem.eUnitState.SEIZURE:
		case NKCUnitSortSystem.eUnitState.LOBBY_UNIT:
			return true;
		}
	}

	private bool CheckFilter(NKMOperator unitData, HashSet<eFilterOption> setFilter)
	{
		foreach (eFilterOption item in setFilter)
		{
			if (CheckFilter(unitData, item))
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckFilter(NKMOperator operatorData, eFilterOption filterOption)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(operatorData.id);
		if (unitTempletBase == null)
		{
			Debug.LogError($"UnitTemplet Null! unitID : {operatorData.id}");
			return false;
		}
		switch (filterOption)
		{
		case eFilterOption.Nothing:
			return false;
		case eFilterOption.Everything:
			return true;
		case eFilterOption.Have:
			if (NKCScenManager.CurrentUserData().m_ArmyData.GetOperatorCountByID(operatorData.id) > 0)
			{
				return true;
			}
			break;
		case eFilterOption.NotHave:
			if (NKCScenManager.CurrentUserData().m_ArmyData.GetOperatorCountByID(operatorData.id) == 0)
			{
				return true;
			}
			break;
		case eFilterOption.Rarily_SSR:
			if (unitTempletBase.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SSR)
			{
				return true;
			}
			break;
		case eFilterOption.Rarily_SR:
			if (unitTempletBase.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SR)
			{
				return true;
			}
			break;
		case eFilterOption.Rarily_R:
			if (unitTempletBase.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_R)
			{
				return true;
			}
			break;
		case eFilterOption.Rarily_N:
			if (unitTempletBase.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_N)
			{
				return true;
			}
			break;
		case eFilterOption.Level_1:
			if (operatorData.level == 1)
			{
				return true;
			}
			break;
		case eFilterOption.Level_other:
			if (operatorData.level == 1)
			{
				return false;
			}
			if (operatorData.level != NKMCommonConst.OperatorConstTemplet.unitMaximumLevel)
			{
				return true;
			}
			break;
		case eFilterOption.Level_Max:
			if (operatorData.level != NKMCommonConst.OperatorConstTemplet.unitMaximumLevel)
			{
				return false;
			}
			return true;
		case eFilterOption.Decked:
			if (GetDeckIndexCache(operatorData.uid) != NKMDeckIndex.None)
			{
				return true;
			}
			break;
		case eFilterOption.NotDecked:
			if (GetDeckIndexCache(operatorData.uid) == NKMDeckIndex.None)
			{
				return true;
			}
			break;
		case eFilterOption.Locked:
			if (operatorData.bLock)
			{
				return true;
			}
			break;
		case eFilterOption.Unlocked:
			if (!operatorData.bLock)
			{
				return true;
			}
			break;
		case eFilterOption.Collected:
			return NKCScenManager.CurrentUserData().m_ArmyData.IsCollectedUnit(operatorData.id);
		case eFilterOption.NotCollected:
			return !NKCScenManager.CurrentUserData().m_ArmyData.IsCollectedUnit(operatorData.id);
		case eFilterOption.PassiveSkill:
			if (m_Options.passiveSkillID > 0 && NKMTempletContainer<NKMOperatorSkillTemplet>.Find(m_Options.passiveSkillID) != null)
			{
				return operatorData.subSkill.id == m_Options.passiveSkillID;
			}
			break;
		}
		return false;
	}

	public void FilterList(eFilterOption filterOption, bool bHideDeckedUnit)
	{
		HashSet<eFilterOption> hashSet = new HashSet<eFilterOption>();
		hashSet.Add(filterOption);
		FilterList(hashSet, bHideDeckedUnit);
	}

	public virtual void FilterList(HashSet<eFilterOption> setFilter, bool bHideDeckedUnit)
	{
		m_Options.setFilterOption = setFilter;
		m_Options.SetBuildOption(bHideDeckedUnit, BUILD_OPTIONS.HIDE_DECKED_UNIT);
		if (m_lstCurrentOperatorList == null)
		{
			m_lstCurrentOperatorList = new List<NKMOperator>();
		}
		m_lstCurrentOperatorList.Clear();
		List<HashSet<eFilterOption>> needFilterSet = new List<HashSet<eFilterOption>>();
		SetFilterCategory(setFilter, ref needFilterSet);
		foreach (KeyValuePair<long, NKMOperator> dicAllOperator in m_dicAllOperatorList)
		{
			NKMOperator value = dicAllOperator.Value;
			if (FilterData(value, needFilterSet))
			{
				m_lstCurrentOperatorList.Add(value);
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

	public void SortList(eSortOption sortOption, bool bForce = false)
	{
		List<eSortOption> list = new List<eSortOption>();
		list.Add(sortOption);
		SortList(list, bForce);
	}

	public void SortList(List<eSortOption> lstSortOption, bool bForce = false)
	{
		if (m_lstCurrentOperatorList == null)
		{
			m_Options.lstSortOption = lstSortOption;
			if (m_Options.setFilterOption != null)
			{
				FilterList(m_Options.setFilterOption, m_Options.IsHasBuildOption(BUILD_OPTIONS.HIDE_DECKED_UNIT));
				return;
			}
			m_Options.setFilterOption = new HashSet<eFilterOption>();
			FilterList(m_Options.setFilterOption, m_Options.IsHasBuildOption(BUILD_OPTIONS.HIDE_DECKED_UNIT));
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
		SortOperatorDataList(ref m_lstCurrentOperatorList, lstSortOption);
		m_Options.lstSortOption = lstSortOption;
	}

	private void SortOperatorDataList(ref List<NKMOperator> lstOperatorData, List<eSortOption> lstSortOption)
	{
		NKCUnitSortSystem.NKCDataComparerer<NKMOperator> nKCDataComparerer = new NKCUnitSortSystem.NKCDataComparerer<NKMOperator>();
		HashSet<eSortCategory> hashSet = new HashSet<eSortCategory>();
		if (m_Options.PreemptiveSortFunc != null)
		{
			nKCDataComparerer.AddFunc(m_Options.PreemptiveSortFunc);
		}
		if (m_Options.IsHasBuildOption(BUILD_OPTIONS.PUSHBACK_UNSELECTABLE))
		{
			nKCDataComparerer.AddFunc(CompareByState);
		}
		foreach (eSortOption item in lstSortOption)
		{
			if (item != eSortOption.None)
			{
				NKCUnitSortSystem.NKCDataComparerer<NKMOperator>.CompareFunc dataComparer = GetDataComparer(item);
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
		if (!hashSet.Contains(eSortCategory.UID))
		{
			nKCDataComparerer.AddFunc(CompareByUIDAscending);
		}
		lstOperatorData.Sort(nKCDataComparerer);
	}

	private NKCUnitSortSystem.NKCDataComparerer<NKMOperator>.CompareFunc GetDataComparer(eSortOption sortOption)
	{
		switch (sortOption)
		{
		case eSortOption.Level_High:
			return CompareByLevelDescending;
		case eSortOption.Level_Low:
			return CompareByLevelAscending;
		case eSortOption.Power_High:
			return CompareByPowerDescending;
		case eSortOption.Power_Low:
			return CompareByPowerAscending;
		case eSortOption.Rarity_High:
			return CompareByRarityDescending;
		case eSortOption.Rarity_Low:
			return CompareByRarityAscending;
		default:
			return CompareByUIDAscending;
		case eSortOption.UID_Last:
			return CompareByUIDDescending;
		case eSortOption.ID_First:
			return CompareByIDAscending;
		case eSortOption.ID_Last:
			return CompareByIDDescending;
		case eSortOption.IDX_First:
			return CompareByIdxAscending;
		case eSortOption.IDX_Last:
			return CompareByIdxDescending;
		case eSortOption.Attack_High:
			return CompareByAttackDescending;
		case eSortOption.Attack_Low:
			return CompareByAttackAscending;
		case eSortOption.Health_High:
			return CompareByHealthDescending;
		case eSortOption.Health_Low:
			return CompareByHealthAscending;
		case eSortOption.Unit_Defense_High:
			return CompareByDefenseDescending;
		case eSortOption.Unit_Defense_Low:
			return CompareByDefenseAscending;
		case eSortOption.Unit_ReduceSkillCool_High:
			return CompareByReduceSkillDescending;
		case eSortOption.Unit_ReduceSkillCool_Low:
			return CompareByReduceSkillAscending;
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
				return (NKMOperator a, NKMOperator b) => m_Options.lstCustomSortFunc[GetSortCategoryFromOption(sortOption)].Value(b, a);
			}
			return null;
		}
	}

	private int CompareByState(NKMOperator lhs, NKMOperator rhs)
	{
		return IsOperatorSelectable(rhs).CompareTo(IsOperatorSelectable(lhs));
	}

	public static int CompareByLevelAscending(NKMOperator lhs, NKMOperator rhs)
	{
		if (lhs.level == rhs.level)
		{
			int num = lhs.exp.CompareTo(rhs.exp);
			if (num != 0)
			{
				return num;
			}
		}
		return lhs.level.CompareTo(rhs.level);
	}

	public static int CompareByLevelDescending(NKMOperator lhs, NKMOperator rhs)
	{
		if (lhs.level == rhs.level)
		{
			int num = rhs.exp.CompareTo(lhs.exp);
			if (num != 0)
			{
				return num;
			}
		}
		return rhs.level.CompareTo(lhs.level);
	}

	public static int CompareByRarityAscending(NKMOperator lhs, NKMOperator rhs)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(lhs.id);
		NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(rhs.id);
		return unitTempletBase.m_NKM_UNIT_GRADE.CompareTo(unitTempletBase2.m_NKM_UNIT_GRADE);
	}

	public static int CompareByRarityDescending(NKMOperator lhs, NKMOperator rhs)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(lhs.id);
		return NKMUnitManager.GetUnitTempletBase(rhs.id).m_NKM_UNIT_GRADE.CompareTo(unitTempletBase.m_NKM_UNIT_GRADE);
	}

	private int CompareByPowerAscending(NKMOperator lhs, NKMOperator rhs)
	{
		return GetUnitPowerCache(lhs.uid).CompareTo(GetUnitPowerCache(rhs.uid));
	}

	private int CompareByPowerDescending(NKMOperator lhs, NKMOperator rhs)
	{
		return GetUnitPowerCache(rhs.uid).CompareTo(GetUnitPowerCache(lhs.uid));
	}

	private int CompareByUIDAscending(NKMOperator lhs, NKMOperator rhs)
	{
		return lhs.uid.CompareTo(rhs.uid);
	}

	private int CompareByUIDDescending(NKMOperator lhs, NKMOperator rhs)
	{
		return rhs.uid.CompareTo(lhs.uid);
	}

	private int CompareByIDAscending(NKMOperator lhs, NKMOperator rhs)
	{
		return lhs.id.CompareTo(rhs.id);
	}

	private int CompareByIDDescending(NKMOperator lhs, NKMOperator rhs)
	{
		return rhs.id.CompareTo(lhs.id);
	}

	private int CompareByIdxAscending(NKMOperator lhs, NKMOperator rhs)
	{
		return NKCCollectionManager.GetEmployeeNumber(lhs.id).CompareTo(NKCCollectionManager.GetEmployeeNumber(rhs.id));
	}

	private int CompareByIdxDescending(NKMOperator lhs, NKMOperator rhs)
	{
		return NKCCollectionManager.GetEmployeeNumber(rhs.id).CompareTo(NKCCollectionManager.GetEmployeeNumber(lhs.id));
	}

	private int CompareByAttackAscending(NKMOperator lhs, NKMOperator rhs)
	{
		return GetUnitAttackCache(lhs.uid).CompareTo(GetUnitAttackCache(rhs.uid));
	}

	private int CompareByAttackDescending(NKMOperator lhs, NKMOperator rhs)
	{
		return GetUnitAttackCache(rhs.uid).CompareTo(GetUnitAttackCache(lhs.uid));
	}

	private int CompareByHealthAscending(NKMOperator lhs, NKMOperator rhs)
	{
		return GetUnitHPCache(lhs.uid).CompareTo(GetUnitHPCache(rhs.uid));
	}

	private int CompareByHealthDescending(NKMOperator lhs, NKMOperator rhs)
	{
		return GetUnitHPCache(rhs.uid).CompareTo(GetUnitHPCache(lhs.uid));
	}

	private int CompareByDefenseAscending(NKMOperator lhs, NKMOperator rhs)
	{
		return GetUnitDefCache(lhs.uid).CompareTo(GetUnitDefCache(rhs.uid));
	}

	private int CompareByDefenseDescending(NKMOperator lhs, NKMOperator rhs)
	{
		return GetUnitDefCache(rhs.uid).CompareTo(GetUnitDefCache(lhs.uid));
	}

	private int CompareByReduceSkillAscending(NKMOperator lhs, NKMOperator rhs)
	{
		return GetUnitSkillCoolCache(lhs.uid).CompareTo(GetUnitSkillCoolCache(rhs.uid));
	}

	private int CompareByReduceSkillDescending(NKMOperator lhs, NKMOperator rhs)
	{
		return GetUnitSkillCoolCache(rhs.uid).CompareTo(GetUnitSkillCoolCache(lhs.uid));
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
		switch (option)
		{
		default:
			return NKCUtilString.GET_STRING_SORT_LEVEL;
		case eSortOption.Rarity_Low:
		case eSortOption.Rarity_High:
			return NKCUtilString.GET_STRING_SORT_RARITY;
		case eSortOption.UID_First:
		case eSortOption.UID_Last:
			return NKCUtilString.GET_STRING_SORT_UID;
		case eSortOption.IDX_First:
		case eSortOption.IDX_Last:
			return NKCUtilString.GET_STRING_SORT_IDX;
		case eSortOption.Attack_Low:
		case eSortOption.Attack_High:
			return NKCUtilString.GET_STRING_SORT_ATTACK;
		case eSortOption.Unit_Defense_Low:
		case eSortOption.Unit_Defense_High:
			return NKCUtilString.GET_STRING_SORT_DEFENSE;
		case eSortOption.Health_Low:
		case eSortOption.Health_High:
			return NKCUtilString.GET_STRING_SORT_HEALTH;
		case eSortOption.Power_Low:
		case eSortOption.Power_High:
			return NKCUtilString.GET_STRING_SORT_POPWER;
		case eSortOption.Unit_ReduceSkillCool_Low:
		case eSortOption.Unit_ReduceSkillCool_High:
			return NKCUtilString.GET_STRING_OPERATOR_SKILL_COOL_REDUCE;
		case eSortOption.CustomAscend1:
		case eSortOption.CustomDescend1:
		case eSortOption.CustomAscend2:
		case eSortOption.CustomDescend2:
		case eSortOption.CustomAscend3:
		case eSortOption.CustomDescend3:
			return string.Empty;
		}
	}

	public NKMOperator AutoSelect(HashSet<long> setExcludeUnitUid, AutoSelectExtraFilter extrafilter = null)
	{
		List<NKMOperator> list = AutoSelect(setExcludeUnitUid, 1, extrafilter);
		if (list.Count > 0)
		{
			return list[0];
		}
		return null;
	}

	public List<NKMOperator> AutoSelect(HashSet<long> setExcludeUnitUid, int count, AutoSelectExtraFilter extrafilter = null)
	{
		List<NKMOperator> list = new List<NKMOperator>();
		for (int i = 0; i < SortedOperatorList.Count; i++)
		{
			if (list.Count >= count)
			{
				break;
			}
			NKMOperator nKMOperator = SortedOperatorList[i];
			if (nKMOperator != null && (setExcludeUnitUid == null || !setExcludeUnitUid.Contains(nKMOperator.uid)) && (extrafilter == null || extrafilter(nKMOperator)) && IsOperatorSelectable(nKMOperator))
			{
				list.Add(nKMOperator);
			}
		}
		return list;
	}

	public static List<eSortOption> GetDefaultSortOptions(bool bIsCollection, bool bIsSelection = false)
	{
		if (bIsCollection)
		{
			if (bIsSelection)
			{
				return DEFAULT_UNIT_SELECTION_SORT_OPTION_LIST;
			}
			return DEFAULT_UNIT_COLLECTION_SORT_OPTION_LIST;
		}
		return DEFAULT_OPERATOR_SORT_OPTION_LIST;
	}

	public static List<eSortOption> AddDefaultSortOptions(List<eSortOption> sortOptions, bool bIsCollection)
	{
		List<eSortOption> defaultSortOptions = GetDefaultSortOptions(bIsCollection);
		if (defaultSortOptions != null)
		{
			sortOptions.AddRange(defaultSortOptions);
		}
		return sortOptions;
	}

	public static HashSet<eFilterCategory> ConvertFilterCategory(HashSet<NKCUnitSortSystem.eFilterCategory> hashSet)
	{
		HashSet<eFilterCategory> hashSet2 = new HashSet<eFilterCategory>();
		foreach (NKCUnitSortSystem.eFilterCategory item in hashSet)
		{
			hashSet2.Add(ConvertFilterCategory(item));
		}
		return hashSet2;
	}

	public static HashSet<NKCUnitSortSystem.eFilterCategory> ConvertFilterCategory(HashSet<eFilterCategory> hashSet)
	{
		HashSet<NKCUnitSortSystem.eFilterCategory> hashSet2 = new HashSet<NKCUnitSortSystem.eFilterCategory>();
		foreach (eFilterCategory item in hashSet)
		{
			hashSet2.Add(ConvertFilterCategory(item));
		}
		return hashSet2;
	}

	public static eFilterCategory ConvertFilterCategory(NKCUnitSortSystem.eFilterCategory option)
	{
		foreach (Tuple<eFilterCategory, NKCUnitSortSystem.eFilterCategory> item in m_listFilterCategory)
		{
			if (item.Item2 == option)
			{
				return item.Item1;
			}
		}
		return eFilterCategory.Rarity;
	}

	public static NKCUnitSortSystem.eFilterCategory ConvertFilterCategory(eFilterCategory option)
	{
		foreach (Tuple<eFilterCategory, NKCUnitSortSystem.eFilterCategory> item in m_listFilterCategory)
		{
			if (item.Item1 == option)
			{
				return item.Item2;
			}
		}
		return NKCUnitSortSystem.eFilterCategory.Rarity;
	}

	public static HashSet<eFilterOption> ConvertFilterOption(HashSet<NKCUnitSortSystem.eFilterOption> hashSet)
	{
		HashSet<eFilterOption> hashSet2 = new HashSet<eFilterOption>();
		foreach (NKCUnitSortSystem.eFilterOption item in hashSet)
		{
			hashSet2.Add(ConvertFilterOption(item));
		}
		return hashSet2;
	}

	public static HashSet<NKCUnitSortSystem.eFilterOption> ConvertFilterOption(HashSet<eFilterOption> hashSet)
	{
		HashSet<NKCUnitSortSystem.eFilterOption> hashSet2 = new HashSet<NKCUnitSortSystem.eFilterOption>();
		foreach (eFilterOption item in hashSet)
		{
			hashSet2.Add(ConvertFilterOption(item));
		}
		return hashSet2;
	}

	public static eFilterOption ConvertFilterOption(NKCUnitSortSystem.eFilterOption option)
	{
		foreach (Tuple<eFilterOption, NKCUnitSortSystem.eFilterOption> listFilterOption in m_listFilterOptions)
		{
			if (listFilterOption.Item2 == option)
			{
				return listFilterOption.Item1;
			}
		}
		return eFilterOption.Nothing;
	}

	public static NKCUnitSortSystem.eFilterOption ConvertFilterOption(eFilterOption option)
	{
		foreach (Tuple<eFilterOption, NKCUnitSortSystem.eFilterOption> listFilterOption in m_listFilterOptions)
		{
			if (listFilterOption.Item1 == option)
			{
				return listFilterOption.Item2;
			}
		}
		return NKCUnitSortSystem.eFilterOption.Nothing;
	}

	public static HashSet<eSortCategory> ConvertSortCategory(HashSet<NKCUnitSortSystem.eSortCategory> hashSet)
	{
		HashSet<eSortCategory> hashSet2 = new HashSet<eSortCategory>();
		foreach (NKCUnitSortSystem.eSortCategory item in hashSet)
		{
			hashSet2.Add(ConvertSortCategory(item));
		}
		return hashSet2;
	}

	public static HashSet<NKCUnitSortSystem.eSortCategory> ConvertSortCategory(HashSet<eSortCategory> hashSet)
	{
		HashSet<NKCUnitSortSystem.eSortCategory> hashSet2 = new HashSet<NKCUnitSortSystem.eSortCategory>();
		foreach (eSortCategory item in hashSet)
		{
			hashSet2.Add(ConvertSortCategory(item));
		}
		return hashSet2;
	}

	public static eSortCategory ConvertSortCategory(NKCUnitSortSystem.eSortCategory option)
	{
		foreach (Tuple<eSortCategory, NKCUnitSortSystem.eSortCategory> item in m_listSortCategory)
		{
			if (item.Item2 == option)
			{
				return item.Item1;
			}
		}
		return eSortCategory.None;
	}

	public static NKCUnitSortSystem.eSortCategory ConvertSortCategory(eSortCategory option)
	{
		foreach (Tuple<eSortCategory, NKCUnitSortSystem.eSortCategory> item in m_listSortCategory)
		{
			if (item.Item1 == option)
			{
				return item.Item2;
			}
		}
		return NKCUnitSortSystem.eSortCategory.None;
	}

	public static List<eSortOption> ConvertSortOption(List<NKCUnitSortSystem.eSortOption> hashSet)
	{
		List<eSortOption> list = new List<eSortOption>();
		foreach (NKCUnitSortSystem.eSortOption item in hashSet)
		{
			list.Add(ConvertSortOption(item));
		}
		return list;
	}

	public static HashSet<eSortOption> ConvertSortOption(HashSet<NKCUnitSortSystem.eSortOption> hashSet)
	{
		HashSet<eSortOption> hashSet2 = new HashSet<eSortOption>();
		foreach (NKCUnitSortSystem.eSortOption item in hashSet)
		{
			hashSet2.Add(ConvertSortOption(item));
		}
		return hashSet2;
	}

	public static HashSet<NKCUnitSortSystem.eSortOption> ConvertSortOption(HashSet<eSortOption> hashSet)
	{
		HashSet<NKCUnitSortSystem.eSortOption> hashSet2 = new HashSet<NKCUnitSortSystem.eSortOption>();
		foreach (eSortOption item in hashSet)
		{
			hashSet2.Add(ConvertSortOption(item));
		}
		return hashSet2;
	}

	public static List<NKCUnitSortSystem.eSortOption> ConvertSortOption(List<eSortOption> hashSet)
	{
		List<NKCUnitSortSystem.eSortOption> list = new List<NKCUnitSortSystem.eSortOption>();
		foreach (eSortOption item in hashSet)
		{
			list.Add(ConvertSortOption(item));
		}
		return list;
	}

	public static eSortOption ConvertSortOption(NKCUnitSortSystem.eSortOption option)
	{
		foreach (Tuple<eSortOption, NKCUnitSortSystem.eSortOption> listSortOption in m_listSortOptions)
		{
			if (listSortOption.Item2 == option)
			{
				return listSortOption.Item1;
			}
		}
		return eSortOption.None;
	}

	public static NKCUnitSortSystem.eSortOption ConvertSortOption(eSortOption option)
	{
		foreach (Tuple<eSortOption, NKCUnitSortSystem.eSortOption> listSortOption in m_listSortOptions)
		{
			if (listSortOption.Item1 == option)
			{
				return listSortOption.Item2;
			}
		}
		return NKCUnitSortSystem.eSortOption.None;
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

	public List<NKMOperator> GetCurrentOperatorList()
	{
		return m_lstCurrentOperatorList;
	}

	private bool IsUnitIsCityLeaderOnMission(long unitUID)
	{
		if (GetCityStateCache(unitUID) == NKMWorldMapManager.WorldMapLeaderState.None)
		{
			return false;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			foreach (NKMWorldMapCityData value in nKMUserData.m_WorldmapData.worldMapCityDataMap.Values)
			{
				if (value.leaderUnitUID == unitUID && value.HasMission())
				{
					return true;
				}
			}
		}
		return false;
	}

	public static HashSet<eFilterCategory> MakeDefaultFilterCategory(FILTER_OPEN_TYPE filterOpenType)
	{
		HashSet<eFilterCategory> hashSet = new HashSet<eFilterCategory>();
		hashSet.Add(eFilterCategory.Rarity);
		switch (filterOpenType)
		{
		case FILTER_OPEN_TYPE.COLLECTION:
			hashSet.Add(eFilterCategory.Collected);
			break;
		case FILTER_OPEN_TYPE.NORMAL:
			hashSet.Add(eFilterCategory.Level);
			hashSet.Add(eFilterCategory.Decked);
			hashSet.Add(eFilterCategory.Locked);
			hashSet.Add(eFilterCategory.PassiveSkill);
			break;
		case FILTER_OPEN_TYPE.SELECTION:
			hashSet.Add(eFilterCategory.Have);
			break;
		}
		return hashSet;
	}
}
