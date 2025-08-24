using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.WorldMap;
using NKC.Sort;
using NKC.UI;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC;

public abstract class NKCUnitSortSystem
{
	public enum eFilterCategory
	{
		UnitType,
		ShipType,
		UnitRole,
		UnitTargetType,
		Rarity,
		Cost,
		TacticLv,
		Level,
		Decked,
		Locked,
		Have,
		UnitMoveType,
		InRoom,
		Loyalty,
		LifeContract,
		Scout,
		Collected,
		SpecialType,
		Skin,
		Collection_Achieve,
		MonsterType,
		SourceType,
		SkillType,
		AttackRange
	}

	public enum eFilterOption
	{
		Nothing,
		Everything,
		Unit_Counter,
		Unit_Mechanic,
		Unit_Soldier,
		Unit_Corrupted,
		Unit_Replacer,
		Unit_Trainer,
		Unit_Target_Ground,
		Unit_Target_Air,
		Unit_Target_All,
		Unit_Move_Ground,
		Unit_Move_Air,
		Ship_Assault,
		Ship_Heavy,
		Ship_Cruiser,
		Ship_Special,
		Ship_Patrol,
		Ship_Etc,
		Role_Striker,
		Role_Ranger,
		Role_Sniper,
		Role_Defender,
		Role_Siege,
		Role_Supporter,
		Role_Tower,
		Rarily_SSR,
		Rarily_SR,
		Rarily_R,
		Rarily_N,
		Unit_Cost_10,
		Unit_Cost_9,
		Unit_Cost_8,
		Unit_Cost_7,
		Unit_Cost_6,
		Unit_Cost_5,
		Unit_Cost_4,
		Unit_Cost_3,
		Unit_Cost_2,
		Unit_Cost_1,
		Unit_TacticLv_6,
		Unit_TacticLv_5,
		Unit_TacticLv_4,
		Unit_TacticLv_3,
		Unit_TacticLv_2,
		Unit_TacticLv_1,
		Unit_TacticLv_0,
		Level_1,
		Level_other,
		Level_Max,
		Decked,
		NotDecked,
		Locked,
		Unlocked,
		Have,
		NotHave,
		InRoom,
		OutRoom,
		Loyalty_Zero,
		Loyalty_Intermediate,
		Loyalty_Max,
		LifeContract_Unsigned,
		LifeContract_Signed,
		CanScout,
		NoScout,
		SpecialType_Rearm,
		SpecialType_Awaken,
		SpecialType_Normal,
		Skin_Have,
		TacticUpdate_Possible,
		UnitReactor_Possible,
		Collected,
		NotCollected,
		Collection_HasAchieve,
		Collection_CompleteAchieve,
		Favorite,
		Favorite_Not,
		SourceType_Conflict,
		SourceType_Stable,
		SourceType_Liberal,
		Skill_Normal,
		Skill_Fury,
		Range_Melee,
		Range_Distant
	}

	public enum eSortCategory
	{
		None,
		Level,
		Rarity,
		UnitSummonCost,
		UnitPower,
		UID,
		ID,
		IDX,
		UnitAttack,
		UnitHealth,
		UnitDefense,
		UnitCrit,
		UnitHit,
		UnitEvade,
		UnitReduceSkillCool,
		Decked,
		PlayerLevel,
		LoginTime,
		ScoutProgress,
		SetOption,
		LimitBreakPossible,
		Transcendence,
		UnitLoyalty,
		Squad_Dungeon,
		Squad_Gauntlet,
		Deploy_Status,
		GuildGrade,
		GuildWeeklyPoint,
		GuildTotalPoint,
		Favorite,
		TacticUpdatePossible,
		TacticUpdateLevel,
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
		Unit_SummonCost_Low,
		Unit_SummonCost_High,
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
		Unit_Crit_Low,
		Unit_Crit_High,
		Unit_Hit_Low,
		Unit_Hit_High,
		Unit_Evade_Low,
		Unit_Evade_High,
		Unit_ReduceSkillCool_Low,
		Unit_ReduceSkillCool_High,
		Decked_First,
		Decked_Last,
		Player_Level_Low,
		Player_Level_High,
		LoginTime_Latly,
		LoginTime_Old,
		ScoutProgress_High,
		ScoutProgress_Low,
		LimitBreak_High,
		LimitBreak_Low,
		Transcendence_High,
		Transcendence_Low,
		Unit_Loyalty_High,
		Unit_Loyalty_Low,
		Squad_Dungeon_High,
		Squad_Dungeon_Low,
		Squad_Gauntlet_High,
		Squad_Gauntlet_Low,
		Deploy_Status_High,
		Deploy_Status_Low,
		Guild_Grade_High,
		Guild_Grade_Low,
		Guild_WeeklyPoint_High,
		Guild_WeeklyPoint_Low,
		Guild_TotalPoint_High,
		Guild_TotalPoint_Low,
		Favorite_First,
		Favorite_Last,
		CustomAscend1,
		CustomDescend1,
		CustomAscend2,
		CustomDescend2,
		CustomAscend3,
		CustomDescend3
	}

	public enum eUnitState
	{
		NONE,
		DUPLICATE,
		CITY_SET,
		CITY_MISSION,
		WARFARE_BATCH,
		DIVE_BATCH,
		DECKED,
		MAINUNIT,
		LOCKED,
		SEIZURE,
		LOBBY_UNIT,
		DUNGEON_RESTRICTED,
		CHECKED,
		LEAGUE_BANNED,
		LEAGUE_DECKED_LEFT,
		LEAGUE_DECKED_RIGHT,
		OFFICE_DORM_IN
	}

	public enum eWorldmapLeaderDataProcessType
	{
		Ignore,
		DisableOnMission,
		DisableLeader,
		DisableOnlyOtherCity
	}

	public struct UnitListOptions
	{
		public delegate bool CustomFilterFunc(NKMUnitData unitData);

		public delegate eUnitState CustomUnitStateFunc(NKMUnitData unitData);

		public NKM_DECK_TYPE eDeckType;

		public List<NKM_DECK_TYPE> lstDeckTypeOrder;

		public HashSet<int> setExcludeUnitID;

		public HashSet<int> setExcludeUnitBaseID;

		public HashSet<int> setOnlyIncludeUnitID;

		public HashSet<int> setOnlyIncludeUnitBaseID;

		public HashSet<int> setDuplicateUnitID;

		public HashSet<long> setExcludeUnitUID;

		public HashSet<eFilterOption> setOnlyIncludeFilterOption;

		public bool bExcludeLockedUnit;

		public bool bExcludeDeckedUnit;

		public HashSet<eFilterOption> setFilterOption;

		public List<eSortOption> lstSortOption;

		public List<eSortOption> lstForceSortOption;

		public NKCDataComparerer<NKMUnitData>.CompareFunc PreemptiveSortFunc;

		public Dictionary<eSortCategory, KeyValuePair<string, NKCDataComparerer<NKMUnitData>.CompareFunc>> lstCustomSortFunc;

		public CustomFilterFunc AdditionalExcludeFilterFunc;

		public List<eSortOption> lstDefaultSortOption;

		public bool bHideDeckedUnit;

		public bool bPushBackUnselectable;

		public bool bIncludeUndeckableUnit;

		public bool bIgnoreCityState;

		public bool bIgnoreWorldMapLeader;

		public bool bIncludeSeizure;

		public bool bIgnoreMissionState;

		public bool bUseUpData;

		public bool bUseBanData;

		public bool bDescending;

		public bool bUseDeckedState;

		public bool bUseLockedState;

		public bool bUseLobbyState;

		public bool bUseDormInState;

		public bool bHideTokenFiltering;

		public CustomUnitStateFunc AdditionalUnitStateFunc;

		public void MakeDuplicateUnitSet(NKMDeckIndex currentDeckIndex, long selectedUnitUID, NKMArmyData armyData)
		{
			NKM_DECK_TYPE nKM_DECK_TYPE = currentDeckIndex.m_eDeckType;
			bool flag = nKM_DECK_TYPE - 7 <= NKM_DECK_TYPE.NDT_NORMAL;
			if (setDuplicateUnitID == null)
			{
				setDuplicateUnitID = new HashSet<int>();
			}
			if (flag)
			{
				IReadOnlyList<NKMDeckData> deckList = armyData.GetDeckList(currentDeckIndex.m_eDeckType);
				for (int i = 0; i < deckList.Count; i++)
				{
					NKMDeckData nKMDeckData = deckList[i];
					if (nKMDeckData == null)
					{
						break;
					}
					if (nKMDeckData.m_ShipUID != selectedUnitUID)
					{
						NKMUnitData shipFromUID = armyData.GetShipFromUID(nKMDeckData.m_ShipUID);
						if (shipFromUID != null)
						{
							NKMUnitTempletBase shipTempletBase = NKMUnitManager.GetUnitTempletBase(shipFromUID.m_UnitID);
							if (shipTempletBase != null)
							{
								foreach (NKMUnitTempletBase item in NKMUnitTempletBase.Values.Where((NKMUnitTempletBase e) => e.m_ShipGroupID == shipTempletBase.m_ShipGroupID))
								{
									setDuplicateUnitID.Add(item.m_UnitID);
								}
							}
						}
					}
					for (int num = 0; num < nKMDeckData.m_listDeckUnitUID.Count; num++)
					{
						long num2 = nKMDeckData.m_listDeckUnitUID[num];
						if (num2 != 0L && selectedUnitUID != num2)
						{
							NKMUnitData unitFromUID = armyData.GetUnitFromUID(num2);
							if (unitFromUID != null)
							{
								setDuplicateUnitID.Add(unitFromUID.m_UnitID);
							}
						}
					}
				}
				return;
			}
			NKMDeckData deckData = armyData.GetDeckData(currentDeckIndex);
			if (deckData == null)
			{
				return;
			}
			if (deckData.m_ShipUID != selectedUnitUID)
			{
				NKMUnitData shipFromUID2 = armyData.GetShipFromUID(deckData.m_ShipUID);
				if (shipFromUID2 != null)
				{
					setDuplicateUnitID.Add(shipFromUID2.m_UnitID);
				}
			}
			for (int num3 = 0; num3 < deckData.m_listDeckUnitUID.Count; num3++)
			{
				long num4 = deckData.m_listDeckUnitUID[num3];
				if (num4 != 0L && selectedUnitUID != num4)
				{
					NKMUnitData unitFromUID2 = armyData.GetUnitFromUID(num4);
					if (unitFromUID2 != null)
					{
						setDuplicateUnitID.Add(unitFromUID2.m_UnitID);
					}
				}
			}
		}
	}

	private class UnitInfoCache
	{
		public Dictionary<NKM_DECK_TYPE, byte> dicDeckIndex;

		public NKMWorldMapManager.WorldMapLeaderState CityState;

		public eUnitState UnitSlotState;

		public int Power;

		public int Attack;

		public int HP;

		public int Defense;

		public int Critical;

		public int Hit;

		public int Evade;

		public int ReduceSkillCollTime;

		public float EnhanceProgress;

		public float ScoutProgress;

		public int LimitBreakProgress;

		public int TacticUpdateProcess;

		public int Loyalty;

		public int OfficeRoomID;
	}

	public class NKCDataComparerer<T> : Comparer<T>
	{
		public delegate int CompareFunc(T lhs, T rhs);

		protected List<CompareFunc> m_lstFunc = new List<CompareFunc>();

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

		public int GetComparerCount()
		{
			return m_lstFunc.Count;
		}
	}

	public delegate bool AutoSelectExtraFilter(NKMUnitData unitData);

	private class AutoCompleteComparerer : NKCDataComparerer<NKMUnitData>
	{
		private NKM_UNIT_GRADE m_TargetGrade = NKM_UNIT_GRADE.NUG_SSR;

		private NKM_UNIT_ROLE_TYPE m_TargetRole;

		private int m_targetCost = 3;

		private bool m_bUseBanData;

		private bool m_bUseUpData;

		private int GetFrontlinePriority(NKM_UNIT_ROLE_TYPE roleType)
		{
			if (roleType == m_TargetRole)
			{
				return 0;
			}
			switch (roleType)
			{
			case NKM_UNIT_ROLE_TYPE.NURT_STRIKER:
			case NKM_UNIT_ROLE_TYPE.NURT_DEFENDER:
				return 1;
			case NKM_UNIT_ROLE_TYPE.NURT_SIEGE:
				return 2;
			default:
				return 3;
			}
		}

		private int GetBacklinePriority(NKM_UNIT_ROLE_TYPE roleType)
		{
			if (roleType == m_TargetRole)
			{
				return 0;
			}
			switch (roleType)
			{
			case NKM_UNIT_ROLE_TYPE.NURT_RANGER:
			case NKM_UNIT_ROLE_TYPE.NURT_SNIPER:
				return 1;
			case NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER:
				return 2;
			case NKM_UNIT_ROLE_TYPE.NURT_TOWER:
				return 3;
			default:
				return 4;
			}
		}

		public override int Compare(NKMUnitData lhs, NKMUnitData rhs)
		{
			if (lhs == null && rhs == null)
			{
				return 0;
			}
			if (lhs == null)
			{
				return 1;
			}
			if (rhs == null)
			{
				return -1;
			}
			return base.Compare(lhs, rhs);
		}

		public void BuildComparer(bool bAwakenFirst, NKM_UNIT_GRADE targetGrade, NKM_UNIT_ROLE_TYPE targetRole, int targetCost, bool bUseBanData, bool bUseUpData, NKCUnitSortSystem ss)
		{
			m_lstFunc.Clear();
			m_bUseBanData = bUseBanData;
			m_bUseUpData = bUseUpData;
			if (bAwakenFirst)
			{
				m_lstFunc.Add(CompareAwakenFirst);
			}
			else
			{
				m_lstFunc.Add(CompareAwakenLast);
			}
			m_TargetRole = targetRole;
			switch (m_TargetRole)
			{
			default:
				m_lstFunc.Add(CompareFrontlinePriority);
				break;
			case NKM_UNIT_ROLE_TYPE.NURT_RANGER:
			case NKM_UNIT_ROLE_TYPE.NURT_SNIPER:
			case NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER:
			case NKM_UNIT_ROLE_TYPE.NURT_TOWER:
				m_lstFunc.Add(CompareBacklinePriority);
				break;
			}
			m_TargetGrade = targetGrade;
			m_lstFunc.Add(CompareRarity);
			m_lstFunc.Add(CompareRearmFirst);
			m_targetCost = targetCost;
			if (bAwakenFirst)
			{
				m_lstFunc.Add(CompareCostProximity);
			}
			else
			{
				m_lstFunc.Add(CompareCostProximitySmallOnly);
			}
			m_lstFunc.Add(CompareTargetType);
			m_lstFunc.Add(ss.CompareByPowerDescending);
			m_lstFunc.Add(ss.CompareByIDAscending);
		}

		private int CompareAwakenFirst(NKMUnitData lhs, NKMUnitData rhs)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(lhs);
			return NKMUnitManager.GetUnitTempletBase(rhs).m_bAwaken.CompareTo(unitTempletBase.m_bAwaken);
		}

		private int CompareAwakenLast(NKMUnitData lhs, NKMUnitData rhs)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(lhs);
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(rhs);
			return unitTempletBase.m_bAwaken.CompareTo(unitTempletBase2.m_bAwaken);
		}

		private int CompareRearmFirst(NKMUnitData lhs, NKMUnitData rhs)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(lhs);
			return NKMUnitManager.GetUnitTempletBase(rhs).IsRearmUnit.CompareTo(unitTempletBase.IsRearmUnit);
		}

		private int CompareRarity(NKMUnitData lhs, NKMUnitData rhs)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(lhs);
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(rhs);
			bool flag = unitTempletBase.m_NKM_UNIT_GRADE == m_TargetGrade;
			bool flag2 = unitTempletBase2.m_NKM_UNIT_GRADE == m_TargetGrade;
			if (flag != flag2)
			{
				return flag2.CompareTo(flag);
			}
			return unitTempletBase2.m_NKM_UNIT_GRADE.CompareTo(unitTempletBase.m_NKM_UNIT_GRADE);
		}

		private int CompareFrontlinePriority(NKMUnitData lhs, NKMUnitData rhs)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(lhs);
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(rhs);
			int frontlinePriority = GetFrontlinePriority(unitTempletBase.m_NKM_UNIT_ROLE_TYPE);
			int frontlinePriority2 = GetFrontlinePriority(unitTempletBase2.m_NKM_UNIT_ROLE_TYPE);
			return frontlinePriority.CompareTo(frontlinePriority2);
		}

		private int CompareBacklinePriority(NKMUnitData lhs, NKMUnitData rhs)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(lhs);
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(rhs);
			int backlinePriority = GetBacklinePriority(unitTempletBase.m_NKM_UNIT_ROLE_TYPE);
			int backlinePriority2 = GetBacklinePriority(unitTempletBase2.m_NKM_UNIT_ROLE_TYPE);
			return backlinePriority.CompareTo(backlinePriority2);
		}

		private int GetCost(NKMUnitData unitData)
		{
			NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitData.m_UnitID);
			if (m_bUseBanData && NKCBanManager.IsBanUnit(unitStatTemplet.m_UnitID))
			{
				return unitStatTemplet.GetRespawnCost(bPVP: true, bLeader: false, NKCBanManager.GetBanData(), null);
			}
			if (m_bUseUpData && NKCBanManager.IsUpUnit(unitStatTemplet.m_UnitID))
			{
				return unitStatTemplet.GetRespawnCost(bPVP: true, bLeader: false, null, NKCBanManager.m_dicNKMUpData);
			}
			return unitStatTemplet.GetRespawnCost(bLeader: false, null, null);
		}

		private int CompareCostProximity(NKMUnitData lhs, NKMUnitData rhs)
		{
			int cost = GetCost(lhs);
			int cost2 = GetCost(rhs);
			int num = Mathf.Abs(m_targetCost - cost);
			int value = Mathf.Abs(m_targetCost - cost2);
			return num.CompareTo(value);
		}

		private int CompareCostProximitySmallOnly(NKMUnitData lhs, NKMUnitData rhs)
		{
			int cost = GetCost(lhs);
			int cost2 = GetCost(rhs);
			int num = m_targetCost - cost;
			int num2 = m_targetCost - cost2;
			if (num < 0)
			{
				num = 100 + cost;
			}
			if (num2 < 0)
			{
				num2 = 100 + cost2;
			}
			return num.CompareTo(num2);
		}

		private int GetUnitTargetTypePriority(NKMUnitData unitData)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
			return ((unitTempletBase.m_NKM_FIND_TARGET_TYPE_Desc == NKM_FIND_TARGET_TYPE.NFTT_INVALID) ? GetFilterOption(unitTempletBase.m_NKM_FIND_TARGET_TYPE) : GetFilterOption(unitTempletBase.m_NKM_FIND_TARGET_TYPE_Desc)) switch
			{
				eFilterOption.Unit_Target_All => 0, 
				eFilterOption.Unit_Target_Ground => 1, 
				eFilterOption.Unit_Target_Air => 2, 
				_ => 3, 
			};
		}

		private int CompareTargetType(NKMUnitData lhs, NKMUnitData rhs)
		{
			int unitTargetTypePriority = GetUnitTargetTypePriority(lhs);
			int unitTargetTypePriority2 = GetUnitTargetTypePriority(rhs);
			return unitTargetTypePriority.CompareTo(unitTargetTypePriority2);
		}

		public AutoCompleteComparerer()
			: base(Array.Empty<CompareFunc>())
		{
		}
	}

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
			eSortCategory.UnitSummonCost,
			new Tuple<eSortOption, eSortOption>(eSortOption.Unit_SummonCost_Low, eSortOption.Unit_SummonCost_High)
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
			eSortCategory.UnitCrit,
			new Tuple<eSortOption, eSortOption>(eSortOption.Unit_Crit_Low, eSortOption.Unit_Crit_High)
		},
		{
			eSortCategory.UnitHit,
			new Tuple<eSortOption, eSortOption>(eSortOption.Unit_Hit_Low, eSortOption.Unit_Hit_High)
		},
		{
			eSortCategory.UnitEvade,
			new Tuple<eSortOption, eSortOption>(eSortOption.Unit_Evade_Low, eSortOption.Unit_Evade_High)
		},
		{
			eSortCategory.UnitReduceSkillCool,
			new Tuple<eSortOption, eSortOption>(eSortOption.Unit_ReduceSkillCool_Low, eSortOption.Unit_ReduceSkillCool_High)
		},
		{
			eSortCategory.Decked,
			new Tuple<eSortOption, eSortOption>(eSortOption.Decked_Last, eSortOption.Decked_First)
		},
		{
			eSortCategory.PlayerLevel,
			new Tuple<eSortOption, eSortOption>(eSortOption.Player_Level_Low, eSortOption.Player_Level_High)
		},
		{
			eSortCategory.LoginTime,
			new Tuple<eSortOption, eSortOption>(eSortOption.LoginTime_Latly, eSortOption.LoginTime_Old)
		},
		{
			eSortCategory.ScoutProgress,
			new Tuple<eSortOption, eSortOption>(eSortOption.ScoutProgress_Low, eSortOption.ScoutProgress_High)
		},
		{
			eSortCategory.GuildGrade,
			new Tuple<eSortOption, eSortOption>(eSortOption.Guild_Grade_High, eSortOption.Guild_Grade_Low)
		},
		{
			eSortCategory.GuildWeeklyPoint,
			new Tuple<eSortOption, eSortOption>(eSortOption.Guild_WeeklyPoint_High, eSortOption.Guild_WeeklyPoint_Low)
		},
		{
			eSortCategory.GuildTotalPoint,
			new Tuple<eSortOption, eSortOption>(eSortOption.Guild_TotalPoint_High, eSortOption.Guild_TotalPoint_Low)
		},
		{
			eSortCategory.LimitBreakPossible,
			new Tuple<eSortOption, eSortOption>(eSortOption.LimitBreak_High, eSortOption.LimitBreak_Low)
		},
		{
			eSortCategory.Transcendence,
			new Tuple<eSortOption, eSortOption>(eSortOption.Transcendence_High, eSortOption.Transcendence_Low)
		},
		{
			eSortCategory.UnitLoyalty,
			new Tuple<eSortOption, eSortOption>(eSortOption.Unit_Loyalty_Low, eSortOption.Unit_Loyalty_High)
		},
		{
			eSortCategory.Squad_Dungeon,
			new Tuple<eSortOption, eSortOption>(eSortOption.Squad_Dungeon_Low, eSortOption.Squad_Dungeon_High)
		},
		{
			eSortCategory.Squad_Gauntlet,
			new Tuple<eSortOption, eSortOption>(eSortOption.Squad_Gauntlet_Low, eSortOption.Squad_Gauntlet_High)
		},
		{
			eSortCategory.Deploy_Status,
			new Tuple<eSortOption, eSortOption>(eSortOption.Deploy_Status_Low, eSortOption.Deploy_Status_High)
		},
		{
			eSortCategory.Favorite,
			new Tuple<eSortOption, eSortOption>(eSortOption.Favorite_First, eSortOption.Favorite_Last)
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

	private static readonly List<eSortOption> DEFAULT_UNIT_SORT_OPTION_LIST = new List<eSortOption>
	{
		eSortOption.Level_High,
		eSortOption.Rarity_High,
		eSortOption.Unit_SummonCost_High,
		eSortOption.ID_First,
		eSortOption.UID_Last
	};

	private static readonly List<eSortOption> DEFAULT_SHIP_SORT_OPTION_LIST = new List<eSortOption>
	{
		eSortOption.Level_High,
		eSortOption.Rarity_High,
		eSortOption.UID_Last
	};

	private static readonly List<eSortOption> DEFAULT_UNIT_COLLECTION_SORT_OPTION_LIST = new List<eSortOption> { eSortOption.IDX_First };

	private static readonly List<eSortOption> DEFAULT_UNIT_SELECTION_SORT_OPTION_LIST = new List<eSortOption>
	{
		eSortOption.Rarity_High,
		eSortOption.IDX_First
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Monster = new HashSet<eFilterOption>
	{
		eFilterOption.Unit_Corrupted,
		eFilterOption.Unit_Replacer
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_UnitType = new HashSet<eFilterOption>
	{
		eFilterOption.Unit_Counter,
		eFilterOption.Unit_Mechanic,
		eFilterOption.Unit_Soldier,
		eFilterOption.Unit_Corrupted,
		eFilterOption.Unit_Replacer,
		eFilterOption.Unit_Trainer
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_ShipType = new HashSet<eFilterOption>
	{
		eFilterOption.Ship_Assault,
		eFilterOption.Ship_Heavy,
		eFilterOption.Ship_Cruiser,
		eFilterOption.Ship_Special,
		eFilterOption.Ship_Patrol,
		eFilterOption.Ship_Etc
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_UnitRole = new HashSet<eFilterOption>
	{
		eFilterOption.Role_Striker,
		eFilterOption.Role_Ranger,
		eFilterOption.Role_Sniper,
		eFilterOption.Role_Defender,
		eFilterOption.Role_Siege,
		eFilterOption.Role_Supporter,
		eFilterOption.Role_Tower
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_UnitTargetType = new HashSet<eFilterOption>
	{
		eFilterOption.Unit_Target_Ground,
		eFilterOption.Unit_Target_Air,
		eFilterOption.Unit_Target_All
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_UnitMoveType = new HashSet<eFilterOption>
	{
		eFilterOption.Unit_Move_Ground,
		eFilterOption.Unit_Move_Air
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Rarity = new HashSet<eFilterOption>
	{
		eFilterOption.Rarily_SSR,
		eFilterOption.Rarily_SR,
		eFilterOption.Rarily_R,
		eFilterOption.Rarily_N
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Cost = new HashSet<eFilterOption>
	{
		eFilterOption.Unit_Cost_10,
		eFilterOption.Unit_Cost_9,
		eFilterOption.Unit_Cost_8,
		eFilterOption.Unit_Cost_7,
		eFilterOption.Unit_Cost_6,
		eFilterOption.Unit_Cost_5,
		eFilterOption.Unit_Cost_4,
		eFilterOption.Unit_Cost_3,
		eFilterOption.Unit_Cost_2,
		eFilterOption.Unit_Cost_1
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_TacticLevel = new HashSet<eFilterOption>
	{
		eFilterOption.Unit_TacticLv_6,
		eFilterOption.Unit_TacticLv_5,
		eFilterOption.Unit_TacticLv_4,
		eFilterOption.Unit_TacticLv_3,
		eFilterOption.Unit_TacticLv_2,
		eFilterOption.Unit_TacticLv_1,
		eFilterOption.Unit_TacticLv_0
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

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Scout = new HashSet<eFilterOption>
	{
		eFilterOption.CanScout,
		eFilterOption.NoScout
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Collected = new HashSet<eFilterOption>
	{
		eFilterOption.Collected,
		eFilterOption.NotCollected
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_SpecialType = new HashSet<eFilterOption>
	{
		eFilterOption.SpecialType_Rearm,
		eFilterOption.SpecialType_Awaken,
		eFilterOption.SpecialType_Normal,
		eFilterOption.TacticUpdate_Possible,
		eFilterOption.UnitReactor_Possible
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Skin = new HashSet<eFilterOption> { eFilterOption.Skin_Have };

	private static readonly HashSet<eFilterOption> m_setFilterCategory_RoomIn = new HashSet<eFilterOption>
	{
		eFilterOption.InRoom,
		eFilterOption.OutRoom
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Loyalty = new HashSet<eFilterOption>
	{
		eFilterOption.Loyalty_Zero,
		eFilterOption.Loyalty_Intermediate,
		eFilterOption.Loyalty_Max
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_LifeContract = new HashSet<eFilterOption>
	{
		eFilterOption.LifeContract_Unsigned,
		eFilterOption.LifeContract_Signed
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_CollectionAchieve = new HashSet<eFilterOption>
	{
		eFilterOption.Collection_HasAchieve,
		eFilterOption.Collection_CompleteAchieve
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_Favorite = new HashSet<eFilterOption>
	{
		eFilterOption.Favorite,
		eFilterOption.Favorite_Not
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_SourceType = new HashSet<eFilterOption>
	{
		eFilterOption.SourceType_Conflict,
		eFilterOption.SourceType_Stable,
		eFilterOption.SourceType_Liberal
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_SkillType = new HashSet<eFilterOption>
	{
		eFilterOption.Skill_Normal,
		eFilterOption.Skill_Fury
	};

	private static readonly HashSet<eFilterOption> m_setFilterCategory_RangeType = new HashSet<eFilterOption>
	{
		eFilterOption.Range_Melee,
		eFilterOption.Range_Distant
	};

	private static readonly List<HashSet<eFilterOption>> m_lstFilterCategory = new List<HashSet<eFilterOption>>
	{
		m_setFilterCategory_Monster, m_setFilterCategory_UnitType, m_setFilterCategory_ShipType, m_setFilterCategory_UnitRole, m_setFilterCategory_UnitMoveType, m_setFilterCategory_UnitTargetType, m_setFilterCategory_Rarity, m_setFilterCategory_Cost, m_setFilterCategory_TacticLevel, m_setFilterCategory_Level,
		m_setFilterCategory_Decked, m_setFilterCategory_Locked, m_setFilterCategory_Have, m_setFilterCategory_Scout, m_setFilterCategory_Collected, m_setFilterCategory_RoomIn, m_setFilterCategory_Loyalty, m_setFilterCategory_LifeContract, m_setFilterCategory_CollectionAchieve, m_setFilterCategory_SpecialType,
		m_setFilterCategory_Favorite, m_setFilterCategory_SourceType, m_setFilterCategory_SkillType, m_setFilterCategory_RangeType, m_setFilterCategory_Skin
	};

	public static readonly HashSet<eFilterCategory> setDefaultUnitFilterCategory = new HashSet<eFilterCategory>
	{
		eFilterCategory.UnitType,
		eFilterCategory.UnitRole,
		eFilterCategory.UnitMoveType,
		eFilterCategory.UnitTargetType,
		eFilterCategory.Rarity,
		eFilterCategory.Cost,
		eFilterCategory.TacticLv,
		eFilterCategory.Level,
		eFilterCategory.Decked,
		eFilterCategory.Locked,
		eFilterCategory.SpecialType,
		eFilterCategory.SourceType
	};

	public static readonly HashSet<eSortCategory> setDefaultUnitSortCategory = new HashSet<eSortCategory>
	{
		eSortCategory.Level,
		eSortCategory.Rarity,
		eSortCategory.UnitSummonCost,
		eSortCategory.UID,
		eSortCategory.UnitPower,
		eSortCategory.UnitAttack,
		eSortCategory.UnitHealth,
		eSortCategory.UnitDefense,
		eSortCategory.UnitHit,
		eSortCategory.UnitEvade,
		eSortCategory.UnitCrit,
		eSortCategory.UnitLoyalty,
		eSortCategory.LimitBreakPossible,
		eSortCategory.Transcendence,
		eSortCategory.Squad_Dungeon,
		eSortCategory.Deploy_Status
	};

	public static readonly HashSet<eFilterCategory> setDefaultShipFilterCategory = new HashSet<eFilterCategory>
	{
		eFilterCategory.ShipType,
		eFilterCategory.Rarity,
		eFilterCategory.Level,
		eFilterCategory.Decked,
		eFilterCategory.Locked
	};

	public static readonly HashSet<eSortCategory> setDefaultShipSortCategory = new HashSet<eSortCategory>
	{
		eSortCategory.Level,
		eSortCategory.Rarity,
		eSortCategory.UID,
		eSortCategory.UnitPower,
		eSortCategory.UnitAttack,
		eSortCategory.UnitHealth
	};

	public static readonly HashSet<eFilterCategory> setDefaultTrophyFilterCategory = new HashSet<eFilterCategory>
	{
		eFilterCategory.UnitRole,
		eFilterCategory.Rarity,
		eFilterCategory.Locked
	};

	public static readonly HashSet<eSortCategory> setDefaultTrophySortCategory = new HashSet<eSortCategory>
	{
		eSortCategory.Rarity,
		eSortCategory.UID
	};

	protected UnitListOptions m_Options;

	protected Dictionary<long, NKMUnitData> m_dicAllUnitList;

	protected List<NKMUnitData> m_lstCurrentUnitList;

	protected NKCUnitSortSystemFilterTokens m_FilterToken = new NKCUnitSortSystemFilterTokens();

	private Dictionary<long, UnitInfoCache> m_dicUnitInfoCache = new Dictionary<long, UnitInfoCache>();

	private Dictionary<int, HashSet<int>> m_dicDeckedUnitIdCache = new Dictionary<int, HashSet<int>>();

	private Dictionary<int, HashSet<int>> m_dicDeckedShipGroupIdCache = new Dictionary<int, HashSet<int>>();

	public UnitListOptions Options => m_Options;

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

	public List<NKMUnitData> SortedUnitList
	{
		get
		{
			if (m_lstCurrentUnitList == null)
			{
				if (m_Options.setFilterOption == null)
				{
					m_Options.setFilterOption = new HashSet<eFilterOption>();
					FilterList(m_Options.setFilterOption, m_Options.bHideDeckedUnit);
				}
				else
				{
					FilterList(m_Options.setFilterOption, m_Options.bHideDeckedUnit);
				}
			}
			return m_lstCurrentUnitList;
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
			FilterList(value, m_Options.bHideDeckedUnit);
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
			return m_Options.bDescending;
		}
		set
		{
			if (m_Options.lstSortOption != null)
			{
				m_Options.bDescending = value;
				SortList(m_Options.lstSortOption);
			}
			else
			{
				m_Options.lstSortOption = GetDefaultSortOptions(NKM_UNIT_TYPE.NUT_NORMAL, bIsCollection: false);
				m_Options.bDescending = value;
				SortList(m_Options.lstSortOption);
			}
		}
	}

	public bool bHideDeckedUnit
	{
		get
		{
			return m_Options.bHideDeckedUnit;
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

	public static HashSet<int> GetDefaultExcludeUnitIDs()
	{
		HashSet<int> hashSet = new HashSet<int>();
		if (!NKMOpenTagManager.IsOpened("TAG_DELETE_BASIC_SHIP"))
		{
			hashSet = new HashSet<int>(NKMMain.excludeUnitID);
		}
		if (!NKMOpenTagManager.IsOpened("TAG_DELETE_YOO_MI_NA"))
		{
			hashSet.Add(1001);
		}
		if (!NKMOpenTagManager.IsOpened("TAG_DELETE_TEAM_FENRIR"))
		{
			hashSet.Add(1002);
		}
		if (!NKMOpenTagManager.IsOpened("TAG_DELETE_JOO_SHI_YOON"))
		{
			hashSet.Add(1003);
		}
		return hashSet;
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

	public void SetEnableShowBan(bool bSet)
	{
		m_Options.bUseBanData = bSet;
	}

	public void SetEnableShowUpUnit(bool bSet)
	{
		m_Options.bUseUpData = bSet;
	}

	protected abstract IEnumerable<NKMUnitData> GetTargetUnitList(NKMUserData userData);

	private NKCUnitSortSystem()
	{
	}

	public NKCUnitSortSystem(NKMUserData userData, UnitListOptions options)
	{
		m_Options = options;
		m_FilterToken.Clear();
		BuildUnitStateCache(userData, options.eDeckType);
		m_dicAllUnitList = BuildFullUnitList(userData, GetTargetUnitList(userData), options);
	}

	public NKCUnitSortSystem(NKMUserData userData, UnitListOptions options, IEnumerable<NKMUnitData> lstUnitData)
	{
		m_Options = options;
		m_FilterToken.Clear();
		BuildUnitStateCache(userData, lstUnitData, options.eDeckType);
		m_dicAllUnitList = BuildFullUnitList(userData, lstUnitData, options);
	}

	public NKCUnitSortSystem(NKMUserData userData, UnitListOptions options, bool useLocal)
	{
		m_Options = options;
		m_FilterToken.Clear();
		if (useLocal)
		{
			BuildLocalUnitStateCache(userData, options.eDeckType);
		}
		else
		{
			BuildUnitStateCache(userData, options.eDeckType);
		}
		m_dicAllUnitList = BuildFullUnitList(userData, GetTargetUnitList(userData), options);
	}

	public NKCUnitSortSystem(UnitListOptions options, IEnumerable<NKMUnitData> lstUnitData)
	{
		m_Options = options;
		m_FilterToken.Clear();
		foreach (NKMUnitData lstUnitDatum in lstUnitData)
		{
			SetUnitPowerCache(lstUnitDatum.m_UnitUID, lstUnitDatum.unitPower);
		}
		Dictionary<long, NKMUnitData> dictionary = new Dictionary<long, NKMUnitData>();
		foreach (NKMUnitData lstUnitDatum2 in lstUnitData)
		{
			dictionary.Add(lstUnitDatum2.m_UserUID, lstUnitDatum2);
		}
		m_dicAllUnitList = dictionary;
	}

	public virtual void BuildFilterAndSortedList(HashSet<eFilterOption> setfilterType, List<eSortOption> lstSortOption, bool bHideDeckedUnit)
	{
		m_Options.bHideDeckedUnit = bHideDeckedUnit;
		m_Options.setFilterOption = setfilterType;
		m_Options.lstSortOption = lstSortOption;
		FilterList(setfilterType, bHideDeckedUnit);
	}

	public void SetDeckIndexCache(long uid, NKMDeckIndex deckindex)
	{
		if (!m_dicUnitInfoCache.ContainsKey(uid))
		{
			UnitInfoCache value = new UnitInfoCache();
			m_dicUnitInfoCache.Add(uid, value);
		}
		if (m_dicUnitInfoCache[uid].dicDeckIndex == null)
		{
			m_dicUnitInfoCache[uid].dicDeckIndex = new Dictionary<NKM_DECK_TYPE, byte>();
		}
		Dictionary<NKM_DECK_TYPE, byte> dicDeckIndex = m_dicUnitInfoCache[uid].dicDeckIndex;
		if (dicDeckIndex.TryGetValue(deckindex.m_eDeckType, out var value2))
		{
			if (deckindex.m_iIndex < value2)
			{
				dicDeckIndex[deckindex.m_eDeckType] = deckindex.m_iIndex;
			}
		}
		else
		{
			dicDeckIndex.Add(deckindex.m_eDeckType, deckindex.m_iIndex);
		}
	}

	public NKMDeckIndex GetDeckIndexCache(long uid, bool bTargetDecktypeOnly = false)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			if (m_dicUnitInfoCache[uid].dicDeckIndex == null)
			{
				return NKMDeckIndex.None;
			}
			if (bTargetDecktypeOnly)
			{
				if (m_dicUnitInfoCache[uid].dicDeckIndex.TryGetValue(m_Options.eDeckType, out var value))
				{
					return new NKMDeckIndex(m_Options.eDeckType, value);
				}
				if (m_Options.lstDeckTypeOrder != null)
				{
					foreach (NKM_DECK_TYPE item in m_Options.lstDeckTypeOrder)
					{
						if (m_dicUnitInfoCache[uid].dicDeckIndex.TryGetValue(item, out value))
						{
							return new NKMDeckIndex(item, value);
						}
					}
				}
				return NKMDeckIndex.None;
			}
			if (m_Options.lstDeckTypeOrder != null)
			{
				foreach (NKM_DECK_TYPE item2 in m_Options.lstDeckTypeOrder)
				{
					if (m_dicUnitInfoCache[uid].dicDeckIndex.TryGetValue(item2, out var value2))
					{
						return new NKMDeckIndex(item2, value2);
					}
				}
			}
			foreach (NKM_DECK_TYPE value4 in Enum.GetValues(typeof(NKM_DECK_TYPE)))
			{
				if (m_dicUnitInfoCache[uid].dicDeckIndex.TryGetValue(value4, out var value3))
				{
					return new NKMDeckIndex(value4, value3);
				}
			}
		}
		return NKMDeckIndex.None;
	}

	public NKMDeckIndex GetDeckIndexCache(long uid, NKM_DECK_TYPE deckType)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			if (m_dicUnitInfoCache[uid].dicDeckIndex == null)
			{
				return NKMDeckIndex.None;
			}
			if (m_dicUnitInfoCache[uid].dicDeckIndex.TryGetValue(deckType, out var value))
			{
				return new NKMDeckIndex(deckType, value);
			}
			if (m_Options.lstDeckTypeOrder != null)
			{
				foreach (NKM_DECK_TYPE item in m_Options.lstDeckTypeOrder)
				{
					if (m_dicUnitInfoCache[uid].dicDeckIndex.TryGetValue(item, out value))
					{
						return new NKMDeckIndex(item, value);
					}
				}
			}
			foreach (NKM_DECK_TYPE value2 in Enum.GetValues(typeof(NKM_DECK_TYPE)))
			{
				if (m_dicUnitInfoCache[uid].dicDeckIndex.TryGetValue(value2, out value))
				{
					return new NKMDeckIndex(value2, value);
				}
			}
		}
		return NKMDeckIndex.None;
	}

	public NKMDeckIndex GetDeckIndexCacheByOption(long uid, bool bTargetDeckTypeOnly)
	{
		if (m_Options.lstSortOption.Contains(eSortOption.Squad_Dungeon_High) || m_Options.lstSortOption.Contains(eSortOption.Squad_Dungeon_Low))
		{
			return GetDeckIndexCache(uid, NKM_DECK_TYPE.NDT_DAILY);
		}
		if (m_Options.lstSortOption.Contains(eSortOption.Squad_Gauntlet_High) || m_Options.lstSortOption.Contains(eSortOption.Squad_Gauntlet_Low) || m_Options.lstSortOption.Contains(eSortOption.Deploy_Status_High) || m_Options.lstSortOption.Contains(eSortOption.Deploy_Status_Low))
		{
			NKMDeckIndex deckIndexCache = GetDeckIndexCache(uid, NKM_DECK_TYPE.NDT_PVP);
			if (deckIndexCache.m_eDeckType == NKM_DECK_TYPE.NDT_PVP)
			{
				return deckIndexCache;
			}
			return GetDeckIndexCache(uid, NKM_DECK_TYPE.NDT_PVP_DEFENCE);
		}
		return GetDeckIndexCache(uid, bTargetDeckTypeOnly);
	}

	private void SetUnitAttackCache(long uid, int atk)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			m_dicUnitInfoCache[uid].Attack = atk;
			return;
		}
		UnitInfoCache unitInfoCache = new UnitInfoCache();
		unitInfoCache.Attack = atk;
		m_dicUnitInfoCache.Add(uid, unitInfoCache);
	}

	public int GetUnitAttackCache(long uid)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			return m_dicUnitInfoCache[uid].Attack;
		}
		return 0;
	}

	private void SetUnitHPCache(long uid, int hp)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			m_dicUnitInfoCache[uid].HP = hp;
			return;
		}
		UnitInfoCache unitInfoCache = new UnitInfoCache();
		unitInfoCache.HP = hp;
		m_dicUnitInfoCache.Add(uid, unitInfoCache);
	}

	public int GetUnitHPCache(long uid)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			return m_dicUnitInfoCache[uid].HP;
		}
		return 0;
	}

	private void SetUnitDefCache(long uid, int def)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			m_dicUnitInfoCache[uid].Defense = def;
			return;
		}
		UnitInfoCache unitInfoCache = new UnitInfoCache();
		unitInfoCache.Defense = def;
		m_dicUnitInfoCache.Add(uid, unitInfoCache);
	}

	public int GetUnitDefCache(long uid)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			return m_dicUnitInfoCache[uid].Defense;
		}
		return 0;
	}

	private void SetUnitCritCache(long uid, int crit)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			m_dicUnitInfoCache[uid].Critical = crit;
			return;
		}
		UnitInfoCache unitInfoCache = new UnitInfoCache();
		unitInfoCache.Critical = crit;
		m_dicUnitInfoCache.Add(uid, unitInfoCache);
	}

	public int GetUnitCritCache(long uid)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			return m_dicUnitInfoCache[uid].Critical;
		}
		return 0;
	}

	private void SetUnitOfficeIDCache(long uid, int officeRoomID)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			m_dicUnitInfoCache[uid].OfficeRoomID = officeRoomID;
			return;
		}
		UnitInfoCache unitInfoCache = new UnitInfoCache();
		unitInfoCache.OfficeRoomID = officeRoomID;
		m_dicUnitInfoCache.Add(uid, unitInfoCache);
	}

	public int GetUnitOfficeIDCacheByOption(long uid)
	{
		if ((m_Options.lstSortOption.Contains(eSortOption.Deploy_Status_High) || m_Options.lstSortOption.Contains(eSortOption.Deploy_Status_Low)) && m_dicUnitInfoCache.ContainsKey(uid))
		{
			return m_dicUnitInfoCache[uid].OfficeRoomID;
		}
		return 0;
	}

	private void SetUnitHitCache(long uid, int Hit)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			m_dicUnitInfoCache[uid].Hit = Hit;
			return;
		}
		UnitInfoCache unitInfoCache = new UnitInfoCache();
		unitInfoCache.Hit = Hit;
		m_dicUnitInfoCache.Add(uid, unitInfoCache);
	}

	public int GetUnitHitCache(long uid)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			return m_dicUnitInfoCache[uid].Hit;
		}
		return 0;
	}

	private void SetUnitEvadeCache(long uid, int Evade)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			m_dicUnitInfoCache[uid].Evade = Evade;
			return;
		}
		UnitInfoCache unitInfoCache = new UnitInfoCache();
		unitInfoCache.Evade = Evade;
		m_dicUnitInfoCache.Add(uid, unitInfoCache);
	}

	public int GetUnitEvadeCache(long uid)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			return m_dicUnitInfoCache[uid].Evade;
		}
		return 0;
	}

	private void SetUnitSkillCoolCache(long uid, int ReduceSkillColl)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			m_dicUnitInfoCache[uid].ReduceSkillCollTime = ReduceSkillColl;
			return;
		}
		UnitInfoCache unitInfoCache = new UnitInfoCache();
		unitInfoCache.ReduceSkillCollTime = ReduceSkillColl;
		m_dicUnitInfoCache.Add(uid, unitInfoCache);
	}

	public int GetUnitSkillCoolCache(long uid)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			return m_dicUnitInfoCache[uid].ReduceSkillCollTime;
		}
		return 0;
	}

	private void SetUnitPowerCache(long uid, int Power)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			m_dicUnitInfoCache[uid].Power = Power;
			return;
		}
		UnitInfoCache unitInfoCache = new UnitInfoCache();
		unitInfoCache.Power = Power;
		m_dicUnitInfoCache.Add(uid, unitInfoCache);
	}

	public int GetUnitPowerCache(long uid)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			return m_dicUnitInfoCache[uid].Power;
		}
		return 0;
	}

	private void SetEnhanceProgressCache(long uid, float value)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			m_dicUnitInfoCache[uid].EnhanceProgress = value;
			return;
		}
		UnitInfoCache unitInfoCache = new UnitInfoCache();
		unitInfoCache.EnhanceProgress = value;
		m_dicUnitInfoCache.Add(uid, unitInfoCache);
	}

	public float GetEnhanceProgressCache(long uid)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			return m_dicUnitInfoCache[uid].EnhanceProgress;
		}
		return 0f;
	}

	private float MakeEnhanceProgressValue(NKMUnitData cNKMUnitData)
	{
		if (cNKMUnitData.m_UnitLevel == 1)
		{
			return 0f;
		}
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < cNKMUnitData.m_listStatEXP.Count; i++)
		{
			int num3 = NKMEnhanceManager.CalculateMaxEXP(cNKMUnitData, (NKM_STAT_TYPE)i);
			num += num3;
			num2 += Math.Min(cNKMUnitData.m_listStatEXP[i], num3);
		}
		if (num == 0)
		{
			return 0f;
		}
		return (float)num2 / (float)num;
	}

	private void SetScoutProgressCache(long uid, float value)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			m_dicUnitInfoCache[uid].ScoutProgress = value;
			return;
		}
		UnitInfoCache unitInfoCache = new UnitInfoCache();
		unitInfoCache.ScoutProgress = value;
		m_dicUnitInfoCache.Add(uid, unitInfoCache);
	}

	public float GetScoutProgressCache(long uid)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			return m_dicUnitInfoCache[uid].ScoutProgress;
		}
		return 0f;
	}

	private void SetLimitBreakProgressCache(long uid, int value)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			m_dicUnitInfoCache[uid].LimitBreakProgress = value;
			return;
		}
		UnitInfoCache unitInfoCache = new UnitInfoCache();
		unitInfoCache.LimitBreakProgress = value;
		m_dicUnitInfoCache.Add(uid, unitInfoCache);
	}

	public int GetLimitBreakCache(long uid)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			return m_dicUnitInfoCache[uid].LimitBreakProgress;
		}
		return 0;
	}

	private void SetLoyaltyCache(long uid, int value)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			m_dicUnitInfoCache[uid].Loyalty = value / 100;
			return;
		}
		UnitInfoCache unitInfoCache = new UnitInfoCache();
		unitInfoCache.Loyalty = value / 100;
		m_dicUnitInfoCache.Add(uid, unitInfoCache);
	}

	public int GetLoyaltyCache(long uid)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			return m_dicUnitInfoCache[uid].Loyalty;
		}
		return 0;
	}

	private float MakeScoutProgressValue(NKMUnitData cNKMUnitData)
	{
		NKMPieceTemplet nKMPieceTemplet = NKMTempletContainer<NKMPieceTemplet>.Find((int)cNKMUnitData.m_UnitUID);
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		long num = (nKMUserData.m_ArmyData.IsCollectedUnit(nKMPieceTemplet.m_PieceGetUintId) ? nKMPieceTemplet.m_PieceReq : nKMPieceTemplet.m_PieceReqFirst);
		return (float)nKMUserData.m_InventoryData.GetCountMiscItem(nKMPieceTemplet.m_PieceId) / (float)num;
	}

	private void SetCityInfoCache(long uid, NKMWorldMapManager.WorldMapLeaderState state)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			m_dicUnitInfoCache[uid].CityState = state;
			return;
		}
		UnitInfoCache unitInfoCache = new UnitInfoCache();
		unitInfoCache.CityState = state;
		m_dicUnitInfoCache.Add(uid, unitInfoCache);
	}

	public NKMWorldMapManager.WorldMapLeaderState GetCityStateCache(long uid)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			return m_dicUnitInfoCache[uid].CityState;
		}
		return NKMWorldMapManager.WorldMapLeaderState.None;
	}

	private void SetUnitSlotState(long uid, eUnitState state)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			m_dicUnitInfoCache[uid].UnitSlotState = state;
			return;
		}
		UnitInfoCache unitInfoCache = new UnitInfoCache();
		unitInfoCache.UnitSlotState = state;
		m_dicUnitInfoCache.Add(uid, unitInfoCache);
	}

	public eUnitState GetUnitSlotState(long uid)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			return m_dicUnitInfoCache[uid].UnitSlotState;
		}
		return eUnitState.NONE;
	}

	private void SetTacticUpdateProgressCache(long uid, int value)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			m_dicUnitInfoCache[uid].TacticUpdateProcess = value;
			return;
		}
		UnitInfoCache unitInfoCache = new UnitInfoCache();
		unitInfoCache.TacticUpdateProcess = value;
		m_dicUnitInfoCache.Add(uid, unitInfoCache);
	}

	public int GetTacticUpdateCache(long uid)
	{
		if (m_dicUnitInfoCache.ContainsKey(uid))
		{
			return m_dicUnitInfoCache[uid].TacticUpdateProcess;
		}
		return 0;
	}

	protected virtual void BuildUnitStateCache(NKMUserData userData, IEnumerable<NKMUnitData> lstUnitData, NKM_DECK_TYPE eNKM_DECK_TYPE)
	{
		m_dicUnitInfoCache.Clear();
		m_dicDeckedUnitIdCache.Clear();
		m_dicDeckedShipGroupIdCache.Clear();
		if (lstUnitData == null || eNKM_DECK_TYPE == NKM_DECK_TYPE.NDT_NONE)
		{
			return;
		}
		bool bPvP = false;
		foreach (NKMUnitData lstUnitDatum in lstUnitData)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(lstUnitDatum);
			NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(lstUnitDatum.m_UnitID);
			if (unitTempletBase != null && unitStatTemplet != null)
			{
				if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL)
				{
					NKMStatData nKMStatData = new NKMStatData();
					nKMStatData.Init();
					nKMStatData.MakeBaseStat(null, bPvP, lstUnitDatum, unitStatTemplet.m_StatData);
					nKMStatData.MakeBaseBonusFactor(lstUnitDatum, NKCScenManager.GetScenManager().GetMyUserData()?.m_InventoryData.EquipItems, null, null);
					SetUnitAttackCache(lstUnitDatum.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_ATK, nKMStatData));
					SetUnitHPCache(lstUnitDatum.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_HP, nKMStatData));
					SetUnitDefCache(lstUnitDatum.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_DEF, nKMStatData));
					SetUnitCritCache(lstUnitDatum.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_CRITICAL, nKMStatData));
					SetUnitHitCache(lstUnitDatum.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_HIT, nKMStatData));
					SetUnitEvadeCache(lstUnitDatum.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_EVADE, nKMStatData));
					SetUnitCritCache(lstUnitDatum.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_CRITICAL, nKMStatData));
					SetEnhanceProgressCache(lstUnitDatum.m_UnitUID, MakeEnhanceProgressValue(lstUnitDatum));
					int power = lstUnitDatum.CalculateOperationPower(userData?.m_InventoryData);
					SetUnitPowerCache(lstUnitDatum.m_UnitUID, power);
				}
				else if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
				{
					NKMStatData statData = NKMUnitStatManager.MakeFinalStat(lstUnitDatum, null, null);
					SetUnitAttackCache(lstUnitDatum.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_ATK, statData));
					SetUnitHPCache(lstUnitDatum.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_HP, statData));
					int power2 = lstUnitDatum.CalculateOperationPower(userData?.m_InventoryData);
					SetUnitPowerCache(lstUnitDatum.m_UnitUID, power2);
				}
			}
		}
	}

	public void UpdateScoutProgressCache()
	{
		foreach (KeyValuePair<long, NKMUnitData> dicAllUnit in m_dicAllUnitList)
		{
			SetScoutProgressCache(dicAllUnit.Key, MakeScoutProgressValue(dicAllUnit.Value));
		}
	}

	public void UpdateLimitBreakProcessCache()
	{
		foreach (KeyValuePair<long, NKMUnitData> dicAllUnit in m_dicAllUnitList)
		{
			SetLimitBreakProgressCache(dicAllUnit.Key, NKMUnitLimitBreakManager.CanThisUnitLimitBreakNow(dicAllUnit.Value, NKCScenManager.CurrentUserData()));
		}
	}

	public void UpdateLoyaltyCache()
	{
		foreach (KeyValuePair<long, NKMUnitData> dicAllUnit in m_dicAllUnitList)
		{
			SetLoyaltyCache(dicAllUnit.Key, dicAllUnit.Value.loyalty);
		}
	}

	public void UpdateTacticUpdateProcessCache()
	{
		foreach (KeyValuePair<long, NKMUnitData> dicAllUnit in m_dicAllUnitList)
		{
			SetLimitBreakProgressCache(dicAllUnit.Key, NKMUnitLimitBreakManager.CanThisUnitLimitBreakNow(dicAllUnit.Value, NKCScenManager.CurrentUserData()));
		}
	}

	public void UpdateUnitData(NKMUnitData unitData)
	{
		if (m_dicAllUnitList.ContainsKey(unitData.m_UnitUID))
		{
			m_dicAllUnitList[unitData.m_UnitUID] = unitData;
		}
	}

	private void SetDeckedUnitIdCache(int unitId, int deckIndex)
	{
		if (!m_dicDeckedUnitIdCache.ContainsKey(unitId))
		{
			m_dicDeckedUnitIdCache.Add(unitId, new HashSet<int>());
		}
		m_dicDeckedUnitIdCache[unitId].Add(deckIndex);
	}

	private void SetDeckedShipIdCache(int shipGroupId, int deckIndex)
	{
		if (!m_dicDeckedShipGroupIdCache.ContainsKey(shipGroupId))
		{
			m_dicDeckedShipGroupIdCache.Add(shipGroupId, new HashSet<int>());
		}
		m_dicDeckedShipGroupIdCache[shipGroupId].Add(deckIndex);
	}

	public bool IsDeckedUnitId(NKM_UNIT_TYPE unitType, int unitId)
	{
		return unitType switch
		{
			NKM_UNIT_TYPE.NUT_NORMAL => GetDeckedUnitIdCache(unitId).Count > 0, 
			NKM_UNIT_TYPE.NUT_SHIP => GetDeckedShipIdCache(unitId).Count > 0, 
			_ => false, 
		};
	}

	private HashSet<int> GetDeckedUnitIdCache(int unitId)
	{
		if (m_dicDeckedUnitIdCache.ContainsKey(unitId) && m_dicDeckedUnitIdCache[unitId] != null)
		{
			return m_dicDeckedUnitIdCache[unitId];
		}
		return new HashSet<int>();
	}

	private HashSet<int> GetDeckedShipIdCache(int shipId)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(shipId);
		if (unitTempletBase == null)
		{
			return new HashSet<int>();
		}
		if (m_dicDeckedShipGroupIdCache.ContainsKey(unitTempletBase.m_ShipGroupID) && m_dicDeckedShipGroupIdCache[unitTempletBase.m_ShipGroupID] != null)
		{
			return m_dicDeckedShipGroupIdCache[unitTempletBase.m_ShipGroupID];
		}
		return new HashSet<int>();
	}

	protected virtual void BuildUnitStateCache(NKMUserData userData, NKM_DECK_TYPE eNKM_DECK_TYPE)
	{
		m_dicUnitInfoCache.Clear();
		m_dicDeckedUnitIdCache.Clear();
		m_dicDeckedShipGroupIdCache.Clear();
		if (userData == null || eNKM_DECK_TYPE == NKM_DECK_TYPE.NDT_NONE)
		{
			return;
		}
		NKMArmyData armyData = userData.m_ArmyData;
		foreach (KeyValuePair<NKMDeckIndex, NKMDeckData> allDeck in armyData.GetAllDecks())
		{
			NKMDeckData value = allDeck.Value;
			if (eNKM_DECK_TYPE == NKM_DECK_TYPE.NDT_TOURNAMENT && allDeck.Key.m_eDeckType != eNKM_DECK_TYPE)
			{
				continue;
			}
			long shipUID = value.m_ShipUID;
			if (shipUID != 0L && armyData.m_dicMyShip.ContainsKey(shipUID))
			{
				SetDeckIndexCache(shipUID, allDeck.Key);
			}
			for (int i = 0; i < value.m_listDeckUnitUID.Count; i++)
			{
				long num = value.m_listDeckUnitUID[i];
				if (armyData.m_dicMyUnit.ContainsKey(num))
				{
					SetDeckIndexCache(num, allDeck.Key);
				}
			}
		}
		if (eNKM_DECK_TYPE == NKM_DECK_TYPE.NDT_NORMAL)
		{
			foreach (NKMWorldMapCityData value4 in userData.m_WorldmapData.worldMapCityDataMap.Values)
			{
				if (value4.leaderUnitUID != 0L)
				{
					SetCityInfoCache(value4.leaderUnitUID, NKMWorldMapManager.WorldMapLeaderState.CityLeader);
				}
			}
		}
		foreach (KeyValuePair<long, NKMUnitData> item in armyData.m_dicMyUnit)
		{
			NKMUnitData value2 = item.Value;
			NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(value2.m_UnitID);
			SetTacticUpdateProgressCache(value2.m_UnitUID, NKCUITacticUpdate.CanThisUnitTactocUpdateNow(value2, NKCScenManager.CurrentUserData()));
			SetLimitBreakProgressCache(value2.m_UnitUID, NKMUnitLimitBreakManager.CanThisUnitLimitBreakNow(value2, NKCScenManager.CurrentUserData()));
			SetLoyaltyCache(value2.m_UnitUID, value2.loyalty);
			SetUnitOfficeIDCache(value2.m_UnitUID, value2.OfficeRoomId);
			if (unitStatTemplet != null)
			{
				bool bPvP = false;
				NKMStatData nKMStatData = new NKMStatData();
				nKMStatData.Init();
				nKMStatData.MakeBaseStat(null, bPvP, value2, unitStatTemplet.m_StatData);
				nKMStatData.MakeBaseBonusFactor(value2, NKCScenManager.GetScenManager().GetMyUserData()?.m_InventoryData.EquipItems, null, null);
				SetUnitAttackCache(value2.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_ATK, nKMStatData));
				SetUnitHPCache(value2.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_HP, nKMStatData));
				SetUnitDefCache(value2.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_DEF, nKMStatData));
				SetUnitCritCache(value2.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_CRITICAL, nKMStatData));
				SetUnitHitCache(value2.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_HIT, nKMStatData));
				SetUnitEvadeCache(value2.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_EVADE, nKMStatData));
				SetUnitCritCache(value2.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_CRITICAL, nKMStatData));
				SetEnhanceProgressCache(value2.m_UnitUID, MakeEnhanceProgressValue(value2));
				int power = value2.CalculateOperationPower(userData.m_InventoryData);
				SetUnitPowerCache(value2.m_UnitUID, power);
			}
		}
		foreach (KeyValuePair<long, NKMUnitData> item2 in armyData.m_dicMyShip)
		{
			NKMUnitData value3 = item2.Value;
			if (NKMUnitManager.GetUnitStatTemplet(value3.m_UnitID) != null)
			{
				NKMStatData statData = NKMUnitStatManager.MakeFinalStat(value3, null, null);
				SetUnitAttackCache(value3.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_ATK, statData));
				SetUnitHPCache(value3.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_HP, statData));
				int power2 = value3.CalculateOperationPower(userData.m_InventoryData);
				SetUnitPowerCache(value3.m_UnitUID, power2);
			}
		}
	}

	protected virtual void BuildLocalUnitStateCache(NKMUserData userData, NKM_DECK_TYPE eNKM_DECK_TYPE)
	{
		m_dicUnitInfoCache.Clear();
		m_dicDeckedUnitIdCache.Clear();
		m_dicDeckedShipGroupIdCache.Clear();
		if (userData == null)
		{
			return;
		}
		NKMArmyData armyData = userData.m_ArmyData;
		Dictionary<int, NKMEventDeckData> allLocalDeckData = NKCLocalDeckDataManager.GetAllLocalDeckData();
		foreach (KeyValuePair<int, NKMEventDeckData> item in allLocalDeckData)
		{
			NKMDeckIndex deckindex = new NKMDeckIndex(eNKM_DECK_TYPE, item.Key);
			foreach (KeyValuePair<int, long> item2 in item.Value.m_dicUnit)
			{
				long value = item2.Value;
				if (armyData.m_dicMyUnit.ContainsKey(value))
				{
					SetDeckIndexCache(value, deckindex);
				}
				NKMUnitData unitFromUID = armyData.GetUnitFromUID(value);
				if (unitFromUID != null)
				{
					SetDeckedUnitIdCache(unitFromUID.m_UnitID, item.Key);
				}
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitFromUID);
				if (unitTempletBase != null && unitTempletBase.m_BaseUnitID > 0)
				{
					SetDeckedUnitIdCache(unitTempletBase.m_BaseUnitID, item.Key);
				}
			}
		}
		foreach (KeyValuePair<int, NKMEventDeckData> item3 in allLocalDeckData)
		{
			NKMDeckIndex deckindex2 = new NKMDeckIndex(eNKM_DECK_TYPE, item3.Key);
			long shipUID = item3.Value.m_ShipUID;
			if (armyData.m_dicMyShip.ContainsKey(shipUID))
			{
				SetDeckIndexCache(shipUID, deckindex2);
			}
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(armyData.GetShipFromUID(shipUID));
			if (unitTempletBase2 != null)
			{
				SetDeckedShipIdCache(unitTempletBase2.m_ShipGroupID, item3.Key);
			}
		}
		if (eNKM_DECK_TYPE == NKM_DECK_TYPE.NDT_NORMAL)
		{
			foreach (NKMWorldMapCityData value4 in userData.m_WorldmapData.worldMapCityDataMap.Values)
			{
				if (value4.leaderUnitUID != 0L)
				{
					SetCityInfoCache(value4.leaderUnitUID, NKMWorldMapManager.WorldMapLeaderState.CityLeader);
				}
			}
		}
		foreach (KeyValuePair<long, NKMUnitData> item4 in armyData.m_dicMyUnit)
		{
			NKMUnitData value2 = item4.Value;
			NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(value2.m_UnitID);
			SetLimitBreakProgressCache(value2.m_UnitUID, NKMUnitLimitBreakManager.CanThisUnitLimitBreakNow(value2, NKCScenManager.CurrentUserData()));
			SetTacticUpdateProgressCache(value2.m_UnitUID, NKMUnitLimitBreakManager.CanThisUnitLimitBreakNow(value2, NKCScenManager.CurrentUserData()));
			SetLoyaltyCache(value2.m_UnitUID, value2.loyalty);
			if (unitStatTemplet != null)
			{
				bool bPvP = false;
				NKMStatData nKMStatData = new NKMStatData();
				nKMStatData.Init();
				nKMStatData.MakeBaseStat(null, bPvP, value2, unitStatTemplet.m_StatData);
				nKMStatData.MakeBaseBonusFactor(value2, NKCScenManager.GetScenManager().GetMyUserData()?.m_InventoryData.EquipItems, null, null);
				SetUnitAttackCache(value2.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_ATK, nKMStatData));
				SetUnitHPCache(value2.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_HP, nKMStatData));
				SetUnitDefCache(value2.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_DEF, nKMStatData));
				SetUnitCritCache(value2.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_CRITICAL, nKMStatData));
				SetUnitHitCache(value2.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_HIT, nKMStatData));
				SetUnitEvadeCache(value2.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_EVADE, nKMStatData));
				SetUnitCritCache(value2.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_CRITICAL, nKMStatData));
				SetEnhanceProgressCache(value2.m_UnitUID, MakeEnhanceProgressValue(value2));
				int power = value2.CalculateOperationPower(userData.m_InventoryData);
				SetUnitPowerCache(value2.m_UnitUID, power);
			}
		}
		foreach (KeyValuePair<long, NKMUnitData> item5 in armyData.m_dicMyShip)
		{
			NKMUnitData value3 = item5.Value;
			if (NKMUnitManager.GetUnitStatTemplet(value3.m_UnitID) != null)
			{
				NKMStatData statData = NKMUnitStatManager.MakeFinalStat(value3, null, null);
				SetUnitAttackCache(value3.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_ATK, statData));
				SetUnitHPCache(value3.m_UnitUID, (int)NKMUnitStatManager.GetFinalStatForUIOutput(NKM_STAT_TYPE.NST_HP, statData));
				int power2 = value3.CalculateOperationPower(userData.m_InventoryData);
				SetUnitPowerCache(value3.m_UnitUID, power2);
			}
		}
	}

	private Dictionary<long, NKMUnitData> BuildFullUnitList(NKMUserData userData, IEnumerable<NKMUnitData> lstTargetUnits, UnitListOptions options)
	{
		Dictionary<long, NKMUnitData> dictionary = new Dictionary<long, NKMUnitData>();
		HashSet<int> setOnlyIncludeUnitID = options.setOnlyIncludeUnitID;
		HashSet<int> setOnlyIncludeUnitBaseID = options.setOnlyIncludeUnitBaseID;
		HashSet<int> setExcludeUnitID = options.setExcludeUnitID;
		HashSet<int> setExcludeUnitBaseID = options.setExcludeUnitBaseID;
		foreach (NKMUnitData lstTargetUnit in lstTargetUnits)
		{
			long unitUID = lstTargetUnit.m_UnitUID;
			if ((options.AdditionalExcludeFilterFunc != null && !options.AdditionalExcludeFilterFunc(lstTargetUnit)) || (options.setExcludeUnitUID != null && options.setExcludeUnitUID.Contains(unitUID)) || (options.bExcludeDeckedUnit && (GetDeckIndexCache(unitUID).m_eDeckType != NKM_DECK_TYPE.NDT_NONE || GetCityStateCache(unitUID) != NKMWorldMapManager.WorldMapLeaderState.None || IsMainUnit(unitUID, userData) || lstTargetUnit.OfficeRoomId > 0)) || (options.bExcludeLockedUnit && lstTargetUnit.m_bLock) || (setOnlyIncludeUnitID != null && !setOnlyIncludeUnitID.Contains(lstTargetUnit.m_UnitID)))
			{
				continue;
			}
			if (setOnlyIncludeUnitBaseID != null)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(lstTargetUnit);
				if (unitTempletBase == null || !setOnlyIncludeUnitBaseID.Any((int x) => unitTempletBase.IsSameBaseUnit(x)))
				{
					continue;
				}
			}
			if ((setExcludeUnitID != null && setExcludeUnitID.Contains(lstTargetUnit.m_UnitID) && (!options.bIncludeSeizure || !lstTargetUnit.IsSeized)) || (setExcludeUnitBaseID != null && NKMUnitManager.CheckContainsBaseUnit(setExcludeUnitBaseID, lstTargetUnit.m_UnitID)) || (options.setOnlyIncludeFilterOption != null && !CheckFilter(lstTargetUnit, options.setOnlyIncludeFilterOption)))
			{
				continue;
			}
			eUnitState state = eUnitState.NONE;
			if (userData != null)
			{
				NKMDeckIndex deckIndexCache = GetDeckIndexCache(unitUID, !m_Options.bUseDeckedState);
				NKMWorldMapManager.WorldMapLeaderState cityStateCache = GetCityStateCache(unitUID);
				NKM_DECK_STATE unitDeckState = userData.m_ArmyData.GetUnitDeckState(unitUID);
				if (lstTargetUnit.IsSeized)
				{
					state = eUnitState.SEIZURE;
				}
				else if (options.setDuplicateUnitID != null && NKMUnitManager.CheckContainsBaseUnit(options.setDuplicateUnitID, lstTargetUnit.m_UnitID))
				{
					state = eUnitState.DUPLICATE;
				}
				else if (!m_Options.bIgnoreMissionState && unitDeckState == NKM_DECK_STATE.DECK_STATE_WARFARE)
				{
					state = eUnitState.WARFARE_BATCH;
				}
				else if (!m_Options.bIgnoreMissionState && unitDeckState == NKM_DECK_STATE.DECK_STATE_DIVE)
				{
					state = eUnitState.DIVE_BATCH;
				}
				else if (!m_Options.bIgnoreMissionState && !m_Options.bIgnoreCityState && unitDeckState == NKM_DECK_STATE.DECK_STATE_WORLDMAP_MISSION)
				{
					state = eUnitState.CITY_MISSION;
				}
				else if ((!m_Options.bIgnoreCityState || !m_Options.bIgnoreWorldMapLeader) && cityStateCache != NKMWorldMapManager.WorldMapLeaderState.None)
				{
					switch (cityStateCache)
					{
					case NKMWorldMapManager.WorldMapLeaderState.CityLeader:
					case NKMWorldMapManager.WorldMapLeaderState.CityLeaderOther:
						foreach (NKMWorldMapCityData value in userData.m_WorldmapData.worldMapCityDataMap.Values)
						{
							if (value.leaderUnitUID != unitUID)
							{
								continue;
							}
							if (value.HasMission())
							{
								if (!m_Options.bIgnoreMissionState && !m_Options.bIgnoreCityState)
								{
									state = eUnitState.CITY_MISSION;
								}
							}
							else if (!m_Options.bIgnoreWorldMapLeader)
							{
								state = eUnitState.CITY_SET;
							}
							break;
						}
						break;
					default:
						Debug.LogError("City Setstate added?");
						break;
					case NKMWorldMapManager.WorldMapLeaderState.None:
						break;
					}
				}
				else if (m_Options.bUseDeckedState && deckIndexCache.m_eDeckType != NKM_DECK_TYPE.NDT_NONE)
				{
					state = eUnitState.DECKED;
				}
				else if (m_Options.bUseDeckedState && IsMainUnit(unitUID, userData))
				{
					state = eUnitState.MAINUNIT;
				}
				else if (m_Options.bUseLockedState && lstTargetUnit.m_bLock)
				{
					state = eUnitState.LOCKED;
				}
				else if (!m_Options.bUseLobbyState || !IsMainUnit(unitUID, userData))
				{
					state = ((m_Options.AdditionalUnitStateFunc != null) ? m_Options.AdditionalUnitStateFunc(lstTargetUnit) : ((m_Options.bUseDormInState && lstTargetUnit.OfficeRoomId > 0) ? eUnitState.OFFICE_DORM_IN : eUnitState.NONE));
				}
				else if (userData.GetBackgroundUnitIndex(unitUID) >= 0)
				{
					state = eUnitState.LOBBY_UNIT;
				}
			}
			SetUnitSlotState(unitUID, state);
			dictionary.Add(unitUID, lstTargetUnit);
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

	protected bool FilterData(NKMUnitData unitData, List<HashSet<eFilterOption>> setFilter)
	{
		if (!m_Options.bIncludeSeizure && unitData.IsSeized)
		{
			return false;
		}
		if (m_Options.bHideDeckedUnit)
		{
			if (!m_Options.bIgnoreCityState && IsUnitIsCityLeaderOnMission(unitData.m_UnitUID))
			{
				return false;
			}
			if (GetDeckIndexCache(unitData.m_UnitUID).m_eDeckType == m_Options.eDeckType)
			{
				return false;
			}
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
		if (!m_Options.bIncludeUndeckableUnit && !NKMUnitManager.CanUnitUsedInDeck(unitTempletBase))
		{
			return false;
		}
		if (setFilter != null)
		{
			for (int i = 0; i < setFilter.Count; i++)
			{
				if (!CheckFilter(unitData, setFilter[i]))
				{
					return false;
				}
			}
		}
		if (!m_FilterToken.CheckFilter(unitData, m_Options))
		{
			return false;
		}
		return true;
	}

	protected bool FilterData(NKMUnitData unitData, NKM_UNIT_STYLE_TYPE targetType)
	{
		if (m_Options.bHideDeckedUnit)
		{
			if (GetCityStateCache(unitData.m_UnitUID) != NKMWorldMapManager.WorldMapLeaderState.None)
			{
				return false;
			}
			if (GetDeckIndexCache(unitData.m_UnitUID).m_eDeckType == m_Options.eDeckType)
			{
				return false;
			}
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
		if (!m_Options.bIncludeUndeckableUnit && !NKMUnitManager.CanUnitUsedInDeck(unitTempletBase))
		{
			return false;
		}
		if (targetType != NKM_UNIT_STYLE_TYPE.NUST_INVALID && unitTempletBase.m_NKM_UNIT_STYLE_TYPE != targetType)
		{
			return false;
		}
		return true;
	}

	protected bool IsUnitSelectable(NKMUnitData unitData)
	{
		switch (GetUnitSlotState(unitData.m_UnitUID))
		{
		default:
			return false;
		case eUnitState.NONE:
		case eUnitState.SEIZURE:
		case eUnitState.LOBBY_UNIT:
			return true;
		}
	}

	private bool CheckFilter(NKMUnitData unitData, HashSet<eFilterOption> setFilter)
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

	private bool CheckFinalUnitCost(NKMUnitStatTemplet unitStatTemplet, int cost)
	{
		if (m_Options.bUseBanData && NKCBanManager.IsBanUnit(unitStatTemplet.m_UnitID))
		{
			if (unitStatTemplet.GetRespawnCost(bPVP: true, bLeader: false, NKCBanManager.GetBanData(), null) == cost)
			{
				return true;
			}
		}
		else if (m_Options.bUseUpData && NKCBanManager.IsUpUnit(unitStatTemplet.m_UnitID))
		{
			if (unitStatTemplet.GetRespawnCost(bPVP: true, bLeader: false, null, NKCBanManager.m_dicNKMUpData) == cost)
			{
				return true;
			}
		}
		else if (unitStatTemplet.GetRespawnCost(bLeader: false, null, null) == cost)
		{
			return true;
		}
		return false;
	}

	private bool CheckFilter(NKMUnitData unitData, eFilterOption filterOption)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
		if (unitTempletBase == null)
		{
			Debug.LogError($"UnitTemplet Null! unitID : {unitData.m_UnitID}");
			return false;
		}
		switch (filterOption)
		{
		case eFilterOption.Nothing:
			return false;
		case eFilterOption.Everything:
			return true;
		case eFilterOption.Have:
			if (unitTempletBase.IsShip())
			{
				if (NKCScenManager.CurrentUserData().m_ArmyData.GetSameKindShipCountFromID(unitData.m_UnitID) > 0)
				{
					return true;
				}
			}
			else if (NKCScenManager.CurrentUserData().m_ArmyData.GetUnitCountByID(unitData.m_UnitID) > 0)
			{
				return true;
			}
			break;
		case eFilterOption.NotHave:
			if (unitTempletBase.IsShip())
			{
				if (NKCScenManager.CurrentUserData().m_ArmyData.GetSameKindShipCountFromID(unitData.m_UnitID) == 0)
				{
					return true;
				}
			}
			else if (NKCScenManager.CurrentUserData().m_ArmyData.GetUnitCountByID(unitData.m_UnitID) == 0)
			{
				return true;
			}
			break;
		case eFilterOption.Unit_Counter:
			if (unitTempletBase.HasUnitStyleType(NKM_UNIT_STYLE_TYPE.NUST_COUNTER))
			{
				return true;
			}
			break;
		case eFilterOption.Unit_Mechanic:
			if (unitTempletBase.HasUnitStyleType(NKM_UNIT_STYLE_TYPE.NUST_MECHANIC))
			{
				return true;
			}
			break;
		case eFilterOption.Unit_Soldier:
			if (unitTempletBase.HasUnitStyleType(NKM_UNIT_STYLE_TYPE.NUST_SOLDIER))
			{
				return true;
			}
			break;
		case eFilterOption.Unit_Corrupted:
			if (unitTempletBase.HasUnitStyleType(NKM_UNIT_STYLE_TYPE.NUST_CORRUPTED))
			{
				return true;
			}
			break;
		case eFilterOption.Unit_Replacer:
			if (unitTempletBase.HasUnitStyleType(NKM_UNIT_STYLE_TYPE.NUST_REPLACER))
			{
				return true;
			}
			break;
		case eFilterOption.Unit_Trainer:
			if (unitTempletBase.HasUnitStyleType(NKM_UNIT_STYLE_TYPE.NUST_TRAINER))
			{
				return true;
			}
			break;
		case eFilterOption.Unit_Move_Air:
			return unitTempletBase.m_bAirUnit;
		case eFilterOption.Unit_Move_Ground:
			return !unitTempletBase.m_bAirUnit;
		case eFilterOption.Unit_Target_Ground:
		case eFilterOption.Unit_Target_Air:
		case eFilterOption.Unit_Target_All:
			if (unitTempletBase.m_NKM_FIND_TARGET_TYPE_Desc != NKM_FIND_TARGET_TYPE.NFTT_INVALID)
			{
				if (GetFilterOption(unitTempletBase.m_NKM_FIND_TARGET_TYPE_Desc) == filterOption)
				{
					return true;
				}
			}
			else if (GetFilterOption(unitTempletBase.m_NKM_FIND_TARGET_TYPE) == filterOption)
			{
				return true;
			}
			break;
		case eFilterOption.Ship_Assault:
			if (unitTempletBase.HasUnitStyleType(NKM_UNIT_STYLE_TYPE.NUST_SHIP_ASSAULT))
			{
				return true;
			}
			break;
		case eFilterOption.Ship_Heavy:
			if (unitTempletBase.HasUnitStyleType(NKM_UNIT_STYLE_TYPE.NUST_SHIP_HEAVY))
			{
				return true;
			}
			break;
		case eFilterOption.Ship_Cruiser:
			if (unitTempletBase.HasUnitStyleType(NKM_UNIT_STYLE_TYPE.NUST_SHIP_CRUISER))
			{
				return true;
			}
			break;
		case eFilterOption.Ship_Special:
			if (unitTempletBase.HasUnitStyleType(NKM_UNIT_STYLE_TYPE.NUST_SHIP_SPECIAL))
			{
				return true;
			}
			break;
		case eFilterOption.Ship_Patrol:
			if (unitTempletBase.HasUnitStyleType(NKM_UNIT_STYLE_TYPE.NUST_SHIP_PATROL))
			{
				return true;
			}
			break;
		case eFilterOption.Ship_Etc:
			if (unitTempletBase.HasUnitStyleType(NKM_UNIT_STYLE_TYPE.NUST_SHIP_ETC))
			{
				return true;
			}
			break;
		case eFilterOption.Role_Striker:
			if (unitTempletBase.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_STRIKER)
			{
				return true;
			}
			break;
		case eFilterOption.Role_Ranger:
			if (unitTempletBase.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_RANGER)
			{
				return true;
			}
			break;
		case eFilterOption.Role_Sniper:
			if (unitTempletBase.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_SNIPER)
			{
				return true;
			}
			break;
		case eFilterOption.Role_Defender:
			if (unitTempletBase.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_DEFENDER)
			{
				return true;
			}
			break;
		case eFilterOption.Role_Siege:
			if (unitTempletBase.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_SIEGE)
			{
				return true;
			}
			break;
		case eFilterOption.Role_Supporter:
			if (unitTempletBase.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER)
			{
				return true;
			}
			break;
		case eFilterOption.Role_Tower:
			if (unitTempletBase.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_TOWER)
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
		case eFilterOption.Unit_Cost_10:
		{
			NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitTempletBase.m_UnitID);
			if (unitStatTemplet == null)
			{
				return false;
			}
			if (CheckFinalUnitCost(unitStatTemplet, 10))
			{
				return true;
			}
			break;
		}
		case eFilterOption.Unit_Cost_9:
		{
			NKMUnitStatTemplet unitStatTemplet10 = NKMUnitManager.GetUnitStatTemplet(unitTempletBase.m_UnitID);
			if (unitStatTemplet10 == null)
			{
				return false;
			}
			if (CheckFinalUnitCost(unitStatTemplet10, 9))
			{
				return true;
			}
			break;
		}
		case eFilterOption.Unit_Cost_8:
		{
			NKMUnitStatTemplet unitStatTemplet9 = NKMUnitManager.GetUnitStatTemplet(unitTempletBase.m_UnitID);
			if (unitStatTemplet9 == null)
			{
				return false;
			}
			if (CheckFinalUnitCost(unitStatTemplet9, 8))
			{
				return true;
			}
			break;
		}
		case eFilterOption.Unit_Cost_7:
		{
			NKMUnitStatTemplet unitStatTemplet8 = NKMUnitManager.GetUnitStatTemplet(unitTempletBase.m_UnitID);
			if (unitStatTemplet8 == null)
			{
				return false;
			}
			if (CheckFinalUnitCost(unitStatTemplet8, 7))
			{
				return true;
			}
			break;
		}
		case eFilterOption.Unit_Cost_6:
		{
			NKMUnitStatTemplet unitStatTemplet7 = NKMUnitManager.GetUnitStatTemplet(unitTempletBase.m_UnitID);
			if (unitStatTemplet7 == null)
			{
				return false;
			}
			if (CheckFinalUnitCost(unitStatTemplet7, 6))
			{
				return true;
			}
			break;
		}
		case eFilterOption.Unit_Cost_5:
		{
			NKMUnitStatTemplet unitStatTemplet6 = NKMUnitManager.GetUnitStatTemplet(unitTempletBase.m_UnitID);
			if (unitStatTemplet6 == null)
			{
				return false;
			}
			if (CheckFinalUnitCost(unitStatTemplet6, 5))
			{
				return true;
			}
			break;
		}
		case eFilterOption.Unit_Cost_4:
		{
			NKMUnitStatTemplet unitStatTemplet5 = NKMUnitManager.GetUnitStatTemplet(unitTempletBase.m_UnitID);
			if (unitStatTemplet5 == null)
			{
				return false;
			}
			if (CheckFinalUnitCost(unitStatTemplet5, 4))
			{
				return true;
			}
			break;
		}
		case eFilterOption.Unit_Cost_3:
		{
			NKMUnitStatTemplet unitStatTemplet4 = NKMUnitManager.GetUnitStatTemplet(unitTempletBase.m_UnitID);
			if (unitStatTemplet4 == null)
			{
				return false;
			}
			if (CheckFinalUnitCost(unitStatTemplet4, 3))
			{
				return true;
			}
			break;
		}
		case eFilterOption.Unit_Cost_2:
		{
			NKMUnitStatTemplet unitStatTemplet3 = NKMUnitManager.GetUnitStatTemplet(unitTempletBase.m_UnitID);
			if (unitStatTemplet3 == null)
			{
				return false;
			}
			if (CheckFinalUnitCost(unitStatTemplet3, 2))
			{
				return true;
			}
			break;
		}
		case eFilterOption.Unit_Cost_1:
		{
			NKMUnitStatTemplet unitStatTemplet2 = NKMUnitManager.GetUnitStatTemplet(unitTempletBase.m_UnitID);
			if (unitStatTemplet2 == null)
			{
				return false;
			}
			if (CheckFinalUnitCost(unitStatTemplet2, 1))
			{
				return true;
			}
			break;
		}
		case eFilterOption.Unit_TacticLv_6:
			return unitData.tacticLevel == 6;
		case eFilterOption.Unit_TacticLv_5:
			return unitData.tacticLevel == 5;
		case eFilterOption.Unit_TacticLv_4:
			return unitData.tacticLevel == 4;
		case eFilterOption.Unit_TacticLv_3:
			return unitData.tacticLevel == 3;
		case eFilterOption.Unit_TacticLv_2:
			return unitData.tacticLevel == 2;
		case eFilterOption.Unit_TacticLv_1:
			return unitData.tacticLevel == 1;
		case eFilterOption.Unit_TacticLv_0:
			return unitData.tacticLevel == 0;
		case eFilterOption.Level_1:
			if (unitData.m_UnitLevel == 1)
			{
				return true;
			}
			break;
		case eFilterOption.Level_other:
		{
			if (unitData.m_UnitLevel == 1)
			{
				return false;
			}
			NKMUnitTempletBase unitTempletBase4 = NKMUnitManager.GetUnitTempletBase(unitData);
			if (unitTempletBase4 == null)
			{
				return false;
			}
			if (unitTempletBase4.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
			{
				NKMShipBuildTemplet shipBuildTemplet2 = NKMShipManager.GetShipBuildTemplet(unitData.m_UnitID);
				if (shipBuildTemplet2 == null)
				{
					return false;
				}
				if (shipBuildTemplet2.ShipUpgradeTarget1 > 0)
				{
					return true;
				}
			}
			if (unitData.m_UnitLevel != NKCExpManager.GetUnitMaxLevel(unitData))
			{
				return true;
			}
			break;
		}
		case eFilterOption.Level_Max:
		{
			if (unitData.m_UnitLevel != NKCExpManager.GetUnitMaxLevel(unitData))
			{
				return false;
			}
			NKMUnitTempletBase unitTempletBase3 = NKMUnitManager.GetUnitTempletBase(unitData);
			if (unitTempletBase3 == null)
			{
				return false;
			}
			if (unitTempletBase3.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
			{
				NKMShipBuildTemplet shipBuildTemplet = NKMShipManager.GetShipBuildTemplet(unitData.m_UnitID);
				if (shipBuildTemplet == null || shipBuildTemplet.ShipUpgradeTarget1 > 0)
				{
					return false;
				}
			}
			return true;
		}
		case eFilterOption.Decked:
			if (GetDeckIndexCache(unitData.m_UnitUID) != NKMDeckIndex.None)
			{
				return true;
			}
			break;
		case eFilterOption.NotDecked:
			if (GetDeckIndexCache(unitData.m_UnitUID) == NKMDeckIndex.None)
			{
				return true;
			}
			break;
		case eFilterOption.Locked:
			if (unitData.m_bLock)
			{
				return true;
			}
			break;
		case eFilterOption.Unlocked:
			if (!unitData.m_bLock)
			{
				return true;
			}
			break;
		case eFilterOption.NoScout:
		{
			NKMPieceTemplet nKMPieceTemplet2 = NKMTempletContainer<NKMPieceTemplet>.Find((int)unitData.m_UnitUID);
			if (nKMPieceTemplet2 != null && nKMPieceTemplet2.CanExchange(NKCScenManager.CurrentUserData()) != NKM_ERROR_CODE.NEC_OK)
			{
				return true;
			}
			break;
		}
		case eFilterOption.CanScout:
		{
			NKMPieceTemplet nKMPieceTemplet = NKMTempletContainer<NKMPieceTemplet>.Find((int)unitData.m_UnitUID);
			if (nKMPieceTemplet != null && nKMPieceTemplet.CanExchange(NKCScenManager.CurrentUserData()) == NKM_ERROR_CODE.NEC_OK)
			{
				return true;
			}
			break;
		}
		case eFilterOption.Collected:
			return NKCUtil.IsUnitObtainedAtLeastOnce(unitTempletBase.m_NKM_UNIT_TYPE, unitData.m_UnitID);
		case eFilterOption.NotCollected:
			return !NKCUtil.IsUnitObtainedAtLeastOnce(unitTempletBase.m_NKM_UNIT_TYPE, unitData.m_UnitID);
		case eFilterOption.InRoom:
			return unitData.OfficeRoomId > 0;
		case eFilterOption.OutRoom:
			return unitData.OfficeRoomId <= 0;
		case eFilterOption.Loyalty_Zero:
			return unitData.loyalty <= 0;
		case eFilterOption.Loyalty_Intermediate:
			if (unitData.loyalty > 0)
			{
				return unitData.loyalty < 10000;
			}
			return false;
		case eFilterOption.Loyalty_Max:
			return unitData.loyalty >= 10000;
		case eFilterOption.LifeContract_Unsigned:
			return !unitData.IsPermanentContract;
		case eFilterOption.LifeContract_Signed:
			return unitData.IsPermanentContract;
		case eFilterOption.Collection_HasAchieve:
			return NKCUnitMissionManager.HasRewardEnableMission(unitData.m_UnitID);
		case eFilterOption.Collection_CompleteAchieve:
		{
			int total = 0;
			int completed = 0;
			int rewardEnable = 0;
			NKCUnitMissionManager.GetUnitMissionRewardEnableCount(unitData.m_UnitID, ref total, ref completed, ref rewardEnable);
			if (total == completed)
			{
				return rewardEnable == 0;
			}
			return false;
		}
		case eFilterOption.SpecialType_Awaken:
			return NKMUnitManager.GetUnitTempletBase(unitData)?.m_bAwaken ?? false;
		case eFilterOption.SpecialType_Rearm:
			return NKMUnitManager.GetUnitTempletBase(unitData)?.IsRearmUnit ?? false;
		case eFilterOption.SpecialType_Normal:
		{
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(unitData);
			if (unitTempletBase2 == null)
			{
				return false;
			}
			if (!unitTempletBase2.m_bAwaken)
			{
				return !unitTempletBase2.IsRearmUnit;
			}
			return false;
		}
		case eFilterOption.Skin_Have:
			return unitData.m_SkinID > 0;
		case eFilterOption.TacticUpdate_Possible:
			return NKCUITacticUpdate.CanThisUnitTactocUpdateNow(unitData, NKCScenManager.CurrentUserData()) > 0;
		case eFilterOption.UnitReactor_Possible:
			return NKCReactorUtil.IsReactorUnit(unitData.m_UnitID);
		case eFilterOption.Favorite:
			return unitData?.isFavorite ?? false;
		case eFilterOption.Favorite_Not:
			if (unitData != null)
			{
				return !unitData.isFavorite;
			}
			return true;
		case eFilterOption.SourceType_Conflict:
			return NKMUnitManager.GetUnitTempletBase(unitData)?.HasSourceType(NKM_UNIT_SOURCE_TYPE.NUST_CONFLICT) ?? false;
		case eFilterOption.SourceType_Stable:
			return NKMUnitManager.GetUnitTempletBase(unitData)?.HasSourceType(NKM_UNIT_SOURCE_TYPE.NUST_STABLE) ?? false;
		case eFilterOption.SourceType_Liberal:
			return NKMUnitManager.GetUnitTempletBase(unitData)?.HasSourceType(NKM_UNIT_SOURCE_TYPE.NUST_LIBERAL) ?? false;
		case eFilterOption.Skill_Normal:
			return !unitTempletBase.StopDefaultCoolTime;
		case eFilterOption.Skill_Fury:
			return unitTempletBase.StopDefaultCoolTime;
		case eFilterOption.Range_Melee:
		{
			NKMUnitTemplet unitTemplet2 = NKMUnitManager.GetUnitTemplet(unitTempletBase.m_UnitID);
			if (unitTemplet2 == null)
			{
				return false;
			}
			return unitTemplet2.m_TargetNearRange <= 500f;
		}
		case eFilterOption.Range_Distant:
		{
			NKMUnitTemplet unitTemplet = NKMUnitManager.GetUnitTemplet(unitTempletBase.m_UnitID);
			if (unitTemplet == null)
			{
				return false;
			}
			return unitTemplet.m_TargetNearRange > 500f;
		}
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
		m_Options.bHideDeckedUnit = bHideDeckedUnit;
		if (m_lstCurrentUnitList == null)
		{
			m_lstCurrentUnitList = new List<NKMUnitData>();
		}
		m_lstCurrentUnitList.Clear();
		List<HashSet<eFilterOption>> needFilterSet = new List<HashSet<eFilterOption>>();
		SetFilterCategory(setFilter, ref needFilterSet);
		foreach (KeyValuePair<long, NKMUnitData> dicAllUnit in m_dicAllUnitList)
		{
			NKMUnitData value = dicAllUnit.Value;
			if (FilterData(value, needFilterSet))
			{
				m_lstCurrentUnitList.Add(value);
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
		if (setFilter == null || setFilter.Count == 0)
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
		if (m_lstCurrentUnitList == null)
		{
			m_Options.lstSortOption = lstSortOption;
			if (m_Options.setFilterOption != null)
			{
				FilterList(m_Options.setFilterOption, m_Options.bHideDeckedUnit);
				return;
			}
			m_Options.setFilterOption = new HashSet<eFilterOption>();
			FilterList(m_Options.setFilterOption, m_Options.bHideDeckedUnit);
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
		SortUnitDataList(ref m_lstCurrentUnitList, lstSortOption);
		m_Options.lstSortOption = lstSortOption;
	}

	private void SortUnitDataList(ref List<NKMUnitData> lstUnitData, List<eSortOption> lstSortOption)
	{
		NKCDataComparerer<NKMUnitData> nKCDataComparerer = new NKCDataComparerer<NKMUnitData>();
		HashSet<eSortCategory> hashSet = new HashSet<eSortCategory>();
		if (m_Options.PreemptiveSortFunc != null)
		{
			nKCDataComparerer.AddFunc(m_Options.PreemptiveSortFunc);
		}
		if (m_Options.bPushBackUnselectable)
		{
			nKCDataComparerer.AddFunc(CompareByState);
		}
		foreach (eSortOption item in lstSortOption)
		{
			if (item != eSortOption.None)
			{
				NKCDataComparerer<NKMUnitData>.CompareFunc dataComparer = GetDataComparer(item);
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
		lstUnitData.Sort(nKCDataComparerer);
	}

	private NKCDataComparerer<NKMUnitData>.CompareFunc GetDataComparer(eSortOption sortOption)
	{
		switch (sortOption)
		{
		case eSortOption.Unit_SummonCost_High:
			return CompareByCostDescending;
		case eSortOption.Unit_SummonCost_Low:
			return CompareByCostAscending;
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
		case eSortOption.Unit_Crit_High:
			return CompareByCriticalDescending;
		case eSortOption.Unit_Crit_Low:
			return CompareByCriticalAscending;
		case eSortOption.Unit_Hit_High:
			return CompareByHitDescending;
		case eSortOption.Unit_Hit_Low:
			return CompareByHitAscending;
		case eSortOption.Unit_Evade_High:
			return CompareByEvadeDescending;
		case eSortOption.Unit_Evade_Low:
			return CompareByEvadeAscending;
		case eSortOption.Unit_ReduceSkillCool_High:
			return CompareByReduceSkillDescending;
		case eSortOption.Unit_ReduceSkillCool_Low:
			return CompareByReduceSkillAscending;
		case eSortOption.Decked_First:
			return CompareByDeckedFirst;
		case eSortOption.Decked_Last:
			return CompareByDeckedLast;
		case eSortOption.ScoutProgress_High:
			return CompareByScoutProgressDescending;
		case eSortOption.ScoutProgress_Low:
			return CompareByScoutProgressAscending;
		case eSortOption.LimitBreak_High:
			return CompareByLimitBreakUnitDescending;
		case eSortOption.LimitBreak_Low:
			return CompareByLimitBreakUnitAscending;
		case eSortOption.Transcendence_High:
			return CompareByTranscendenceDescending;
		case eSortOption.Transcendence_Low:
			return CompareByTranscendenceAscending;
		case eSortOption.Unit_Loyalty_High:
			return CompareByLoyaltyDescending;
		case eSortOption.Unit_Loyalty_Low:
			return CompareByLoyaltyAscending;
		case eSortOption.Squad_Dungeon_High:
			return CompareBySquadDungeonDescending;
		case eSortOption.Squad_Dungeon_Low:
			return CompareBySquadDungeonAscending;
		case eSortOption.Squad_Gauntlet_High:
			return CompareBySquadPvPDescending;
		case eSortOption.Squad_Gauntlet_Low:
			return CompareBySquadPvPAscending;
		case eSortOption.Deploy_Status_High:
			return CompareByDeployStatusDescending;
		case eSortOption.Deploy_Status_Low:
			return CompareByDeployStatusAscending;
		case eSortOption.Favorite_First:
			return CompareByFavoriteFirst;
		case eSortOption.Favorite_Last:
			return CompareByFavoriteLast;
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
				return (NKMUnitData a, NKMUnitData b) => m_Options.lstCustomSortFunc[GetSortCategoryFromOption(sortOption)].Value(b, a);
			}
			return null;
		}
	}

	private int CompareByDeckedFirst(NKMUnitData lhs, NKMUnitData rhs)
	{
		NKMDeckIndex deckIndexCache = GetDeckIndexCache(lhs.m_UnitUID, !m_Options.bUseDeckedState);
		NKMDeckIndex deckIndexCache2 = GetDeckIndexCache(rhs.m_UnitUID, !m_Options.bUseDeckedState);
		if (deckIndexCache.m_eDeckType != NKM_DECK_TYPE.NDT_NONE && deckIndexCache2.m_eDeckType != NKM_DECK_TYPE.NDT_NONE)
		{
			return deckIndexCache.m_iIndex.CompareTo(deckIndexCache2.m_iIndex);
		}
		return deckIndexCache2.m_eDeckType.CompareTo(deckIndexCache.m_eDeckType);
	}

	private int CompareByDeckedLast(NKMUnitData lhs, NKMUnitData rhs)
	{
		NKMDeckIndex deckIndexCache = GetDeckIndexCache(lhs.m_UnitUID, !m_Options.bUseDeckedState);
		NKMDeckIndex deckIndexCache2 = GetDeckIndexCache(rhs.m_UnitUID, !m_Options.bUseDeckedState);
		if (deckIndexCache.m_eDeckType != NKM_DECK_TYPE.NDT_NONE && deckIndexCache2.m_eDeckType != NKM_DECK_TYPE.NDT_NONE)
		{
			return deckIndexCache.m_iIndex.CompareTo(deckIndexCache2.m_iIndex);
		}
		return deckIndexCache.m_eDeckType.CompareTo(deckIndexCache2.m_eDeckType);
	}

	private int CompareByState(NKMUnitData lhs, NKMUnitData rhs)
	{
		return IsUnitSelectable(rhs).CompareTo(IsUnitSelectable(lhs));
	}

	public static int CompareByLevelAscending(NKMUnitData lhs, NKMUnitData rhs)
	{
		if (lhs.m_UnitLevel == rhs.m_UnitLevel)
		{
			int num = lhs.m_iUnitLevelEXP.CompareTo(rhs.m_iUnitLevelEXP);
			if (num != 0)
			{
				return num;
			}
			return lhs.m_LimitBreakLevel.CompareTo(rhs.m_LimitBreakLevel);
		}
		return lhs.m_UnitLevel.CompareTo(rhs.m_UnitLevel);
	}

	public static int CompareByLevelDescending(NKMUnitData lhs, NKMUnitData rhs)
	{
		if (lhs.m_UnitLevel == rhs.m_UnitLevel)
		{
			int num = rhs.m_iUnitLevelEXP.CompareTo(lhs.m_iUnitLevelEXP);
			if (num != 0)
			{
				return num;
			}
			return rhs.m_LimitBreakLevel.CompareTo(lhs.m_LimitBreakLevel);
		}
		return rhs.m_UnitLevel.CompareTo(lhs.m_UnitLevel);
	}

	public static int CompareByRarityAscending(NKMUnitData lhs, NKMUnitData rhs)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(lhs.m_UnitID);
		NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(rhs.m_UnitID);
		if (unitTempletBase.m_NKM_UNIT_GRADE != unitTempletBase2.m_NKM_UNIT_GRADE)
		{
			return unitTempletBase.m_NKM_UNIT_GRADE.CompareTo(unitTempletBase2.m_NKM_UNIT_GRADE);
		}
		if (unitTempletBase.m_bAwaken != unitTempletBase2.m_bAwaken)
		{
			return unitTempletBase.m_bAwaken.CompareTo(unitTempletBase2.m_bAwaken);
		}
		return unitTempletBase.IsRearmUnit.CompareTo(unitTempletBase2.IsRearmUnit);
	}

	public static int CompareByRarityDescending(NKMUnitData lhs, NKMUnitData rhs)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(lhs.m_UnitID);
		NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(rhs.m_UnitID);
		if (unitTempletBase.m_NKM_UNIT_GRADE != unitTempletBase2.m_NKM_UNIT_GRADE)
		{
			return unitTempletBase2.m_NKM_UNIT_GRADE.CompareTo(unitTempletBase.m_NKM_UNIT_GRADE);
		}
		if (unitTempletBase.m_bAwaken != unitTempletBase2.m_bAwaken)
		{
			return unitTempletBase2.m_bAwaken.CompareTo(unitTempletBase.m_bAwaken);
		}
		return unitTempletBase2.IsRearmUnit.CompareTo(unitTempletBase.IsRearmUnit);
	}

	private int CompareByCostAscending(NKMUnitData lhs, NKMUnitData rhs)
	{
		NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(lhs.m_UnitID);
		NKMUnitStatTemplet unitStatTemplet2 = NKMUnitManager.GetUnitStatTemplet(rhs.m_UnitID);
		if (unitStatTemplet == null || unitStatTemplet2 == null)
		{
			return -1;
		}
		int num = 0;
		int num2 = 0;
		num = ((m_Options.bUseBanData && NKCBanManager.IsBanUnit(unitStatTemplet.m_UnitID)) ? unitStatTemplet.GetRespawnCost(bPVP: true, bLeader: false, NKCBanManager.GetBanData(), null) : ((!m_Options.bUseUpData || !NKCBanManager.IsUpUnit(unitStatTemplet.m_UnitID)) ? unitStatTemplet.GetRespawnCost(bLeader: false, null, null) : unitStatTemplet.GetRespawnCost(bPVP: true, bLeader: false, null, NKCBanManager.m_dicNKMUpData)));
		num2 = ((m_Options.bUseBanData && NKCBanManager.IsBanUnit(unitStatTemplet2.m_UnitID)) ? unitStatTemplet2.GetRespawnCost(bPVP: true, bLeader: false, NKCBanManager.GetBanData(), null) : ((!m_Options.bUseUpData || !NKCBanManager.IsUpUnit(unitStatTemplet2.m_UnitID)) ? unitStatTemplet2.GetRespawnCost(bLeader: false, null, null) : unitStatTemplet2.GetRespawnCost(bPVP: true, bLeader: false, null, NKCBanManager.m_dicNKMUpData)));
		return num.CompareTo(num2);
	}

	private int CompareByCostDescending(NKMUnitData lhs, NKMUnitData rhs)
	{
		NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(lhs.m_UnitID);
		NKMUnitStatTemplet unitStatTemplet2 = NKMUnitManager.GetUnitStatTemplet(rhs.m_UnitID);
		if (unitStatTemplet == null || unitStatTemplet2 == null)
		{
			return -1;
		}
		int num = 0;
		int num2 = 0;
		num = ((m_Options.bUseBanData && NKCBanManager.IsBanUnit(unitStatTemplet.m_UnitID)) ? unitStatTemplet.GetRespawnCost(bPVP: true, bLeader: false, NKCBanManager.GetBanData(), null) : ((!m_Options.bUseUpData || !NKCBanManager.IsUpUnit(unitStatTemplet.m_UnitID)) ? unitStatTemplet.GetRespawnCost(bLeader: false, null, null) : unitStatTemplet.GetRespawnCost(bPVP: true, bLeader: false, null, NKCBanManager.m_dicNKMUpData)));
		return ((m_Options.bUseBanData && NKCBanManager.IsBanUnit(unitStatTemplet2.m_UnitID)) ? unitStatTemplet2.GetRespawnCost(bPVP: true, bLeader: false, NKCBanManager.GetBanData(), null) : ((!m_Options.bUseUpData || !NKCBanManager.IsUpUnit(unitStatTemplet2.m_UnitID)) ? unitStatTemplet2.GetRespawnCost(bLeader: false, null, null) : unitStatTemplet2.GetRespawnCost(bPVP: true, bLeader: false, null, NKCBanManager.m_dicNKMUpData))).CompareTo(num);
	}

	private int CompareByPowerAscending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return GetUnitPowerCache(lhs.m_UnitUID).CompareTo(GetUnitPowerCache(rhs.m_UnitUID));
	}

	private int CompareByPowerDescending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return GetUnitPowerCache(rhs.m_UnitUID).CompareTo(GetUnitPowerCache(lhs.m_UnitUID));
	}

	private int CompareByUIDAscending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return lhs.m_UnitUID.CompareTo(rhs.m_UnitUID);
	}

	private int CompareByUIDDescending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return rhs.m_UnitUID.CompareTo(lhs.m_UnitUID);
	}

	private int CompareByIDAscending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return lhs.m_UnitID.CompareTo(rhs.m_UnitID);
	}

	private int CompareByIDDescending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return rhs.m_UnitID.CompareTo(lhs.m_UnitID);
	}

	private int CompareByIdxAscending(NKMUnitData lhs, NKMUnitData rhs)
	{
		NKCCollectionUnitTemplet unitTemplet = NKCCollectionManager.GetUnitTemplet(lhs.m_UnitID);
		NKCCollectionUnitTemplet unitTemplet2 = NKCCollectionManager.GetUnitTemplet(rhs.m_UnitID);
		if (unitTemplet == null || unitTemplet2 == null)
		{
			if (unitTemplet == null && unitTemplet2 != null)
			{
				return 1;
			}
			if (unitTemplet != null && unitTemplet2 == null)
			{
				return -1;
			}
			return 0;
		}
		return unitTemplet.Idx.CompareTo(unitTemplet2.Idx);
	}

	private int CompareByIdxDescending(NKMUnitData lhs, NKMUnitData rhs)
	{
		NKCCollectionUnitTemplet unitTemplet = NKCCollectionManager.GetUnitTemplet(lhs.m_UnitID);
		NKCCollectionUnitTemplet unitTemplet2 = NKCCollectionManager.GetUnitTemplet(rhs.m_UnitID);
		if (unitTemplet == null || unitTemplet2 == null)
		{
			if (unitTemplet == null && unitTemplet2 != null)
			{
				return 1;
			}
			if (unitTemplet != null && unitTemplet2 == null)
			{
				return -1;
			}
			return 0;
		}
		return unitTemplet2.Idx.CompareTo(unitTemplet.Idx);
	}

	private int CompareByAttackAscending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return GetUnitAttackCache(lhs.m_UnitUID).CompareTo(GetUnitAttackCache(rhs.m_UnitUID));
	}

	private int CompareByAttackDescending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return GetUnitAttackCache(rhs.m_UnitUID).CompareTo(GetUnitAttackCache(lhs.m_UnitUID));
	}

	private int CompareByHealthAscending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return GetUnitHPCache(lhs.m_UnitUID).CompareTo(GetUnitHPCache(rhs.m_UnitUID));
	}

	private int CompareByHealthDescending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return GetUnitHPCache(rhs.m_UnitUID).CompareTo(GetUnitHPCache(lhs.m_UnitUID));
	}

	private int CompareByDefenseAscending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return GetUnitDefCache(lhs.m_UnitUID).CompareTo(GetUnitDefCache(rhs.m_UnitUID));
	}

	private int CompareByDefenseDescending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return GetUnitDefCache(rhs.m_UnitUID).CompareTo(GetUnitDefCache(lhs.m_UnitUID));
	}

	private int CompareByCriticalAscending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return GetUnitCritCache(lhs.m_UnitUID).CompareTo(GetUnitCritCache(rhs.m_UnitUID));
	}

	private int CompareByCriticalDescending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return GetUnitCritCache(rhs.m_UnitUID).CompareTo(GetUnitCritCache(lhs.m_UnitUID));
	}

	private int CompareByHitAscending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return GetUnitHitCache(lhs.m_UnitUID).CompareTo(GetUnitHitCache(rhs.m_UnitUID));
	}

	private int CompareByHitDescending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return GetUnitHitCache(rhs.m_UnitUID).CompareTo(GetUnitHitCache(lhs.m_UnitUID));
	}

	private int CompareByEvadeAscending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return GetUnitEvadeCache(lhs.m_UnitUID).CompareTo(GetUnitEvadeCache(rhs.m_UnitUID));
	}

	private int CompareByEvadeDescending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return GetUnitEvadeCache(rhs.m_UnitUID).CompareTo(GetUnitEvadeCache(lhs.m_UnitUID));
	}

	private int CompareByReduceSkillAscending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return GetUnitSkillCoolCache(lhs.m_UnitUID).CompareTo(GetUnitSkillCoolCache(rhs.m_UnitUID));
	}

	private int CompareByReduceSkillDescending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return GetUnitSkillCoolCache(rhs.m_UnitUID).CompareTo(GetUnitSkillCoolCache(lhs.m_UnitUID));
	}

	private int CompareByScoutProgressDescending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return GetScoutProgressCache(rhs.m_UnitUID).CompareTo(GetScoutProgressCache(lhs.m_UnitUID));
	}

	private int CompareByScoutProgressAscending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return GetScoutProgressCache(lhs.m_UnitUID).CompareTo(GetScoutProgressCache(rhs.m_UnitUID));
	}

	private int CompareByLimitBreakUnitDescending(NKMUnitData lhs, NKMUnitData rhs)
	{
		int num = CompareLBStatus(lhs, rhs);
		if (num != 0)
		{
			return num;
		}
		return GetLimitBreakCache(lhs.m_UnitUID).CompareTo(GetLimitBreakCache(rhs.m_UnitUID));
	}

	private int CompareByLimitBreakUnitAscending(NKMUnitData lhs, NKMUnitData rhs)
	{
		int num = CompareLBStatus(lhs, rhs);
		if (num != 0)
		{
			return num;
		}
		return GetLimitBreakCache(rhs.m_UnitUID).CompareTo(GetLimitBreakCache(lhs.m_UnitUID));
	}

	private int GetLBStatus(NKMUnitData unitData)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
		if (unitTempletBase != null && unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
		{
			return 100;
		}
		if (GetLimitBreakCache(unitData.m_UnitUID) == -1)
		{
			return 5;
		}
		if (unitData.m_LimitBreakLevel < 3)
		{
			return 0;
		}
		if (unitData.m_LimitBreakLevel < 8)
		{
			return 1;
		}
		if (unitData.m_LimitBreakLevel < 13)
		{
			return 2;
		}
		return 3;
	}

	private int CompareLBStatus(NKMUnitData lhs, NKMUnitData rhs)
	{
		return GetLBStatus(lhs).CompareTo(GetLBStatus(rhs));
	}

	private int CompareByTranscendenceDescending(NKMUnitData lhs, NKMUnitData rhs)
	{
		int num = CompareTrStatus(lhs, rhs);
		if (num != 0)
		{
			return num;
		}
		return GetLimitBreakCache(lhs.m_UnitUID).CompareTo(GetLimitBreakCache(rhs.m_UnitUID));
	}

	private int CompareByTranscendenceAscending(NKMUnitData lhs, NKMUnitData rhs)
	{
		int num = CompareTrStatus(lhs, rhs);
		if (num != 0)
		{
			return num;
		}
		return GetLimitBreakCache(rhs.m_UnitUID).CompareTo(GetLimitBreakCache(lhs.m_UnitUID));
	}

	private int GetTranscendenceStatus(NKMUnitData unitData)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
		if (unitTempletBase != null && unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
		{
			return 100;
		}
		if (GetLimitBreakCache(unitData.m_UnitUID) == -1)
		{
			return 5;
		}
		if (unitData.m_LimitBreakLevel < 3)
		{
			return 2;
		}
		if (unitData.m_LimitBreakLevel < 8)
		{
			return 1;
		}
		if (unitData.m_LimitBreakLevel < 13)
		{
			return 0;
		}
		return 3;
	}

	private int CompareTrStatus(NKMUnitData lhs, NKMUnitData rhs)
	{
		return GetTranscendenceStatus(lhs).CompareTo(GetTranscendenceStatus(rhs));
	}

	private int CompareByLoyaltyAscending(NKMUnitData lhs, NKMUnitData rhs)
	{
		if (GetLoyaltyCache(lhs.m_UnitUID) == GetLoyaltyCache(rhs.m_UnitUID))
		{
			return rhs.IsPermanentContract.CompareTo(lhs.IsPermanentContract);
		}
		return GetLoyaltyCache(lhs.m_UnitUID).CompareTo(GetLoyaltyCache(rhs.m_UnitUID));
	}

	private int CompareByLoyaltyDescending(NKMUnitData lhs, NKMUnitData rhs)
	{
		if (GetLoyaltyCache(lhs.m_UnitUID) == GetLoyaltyCache(rhs.m_UnitUID))
		{
			return rhs.IsPermanentContract.CompareTo(lhs.IsPermanentContract);
		}
		return GetLoyaltyCache(rhs.m_UnitUID).CompareTo(GetLoyaltyCache(lhs.m_UnitUID));
	}

	private int CompareByDeckTypeIndexAscending(NKMUnitData lhs, NKMUnitData rhs, NKM_DECK_TYPE deckType)
	{
		NKMDeckIndex deckIndexCache = GetDeckIndexCache(lhs.m_UnitUID, deckType);
		NKMDeckIndex deckIndexCache2 = GetDeckIndexCache(rhs.m_UnitUID, deckType);
		if (deckIndexCache.m_eDeckType == deckIndexCache2.m_eDeckType)
		{
			if (deckIndexCache.m_eDeckType == NKM_DECK_TYPE.NDT_NONE)
			{
				return 0;
			}
			int num = deckIndexCache.m_iIndex.CompareTo(deckIndexCache2.m_iIndex);
			if (num != 0)
			{
				return num;
			}
			NKMDeckData deckData = NKCScenManager.CurrentUserData().m_ArmyData.GetDeckData(deckIndexCache);
			if (deckData == null)
			{
				return 0;
			}
			deckData.HasUnitUid(lhs.m_UnitUID, out var index);
			deckData.HasUnitUid(rhs.m_UnitUID, out var index2);
			return index.CompareTo(index2);
		}
		if (deckIndexCache.m_eDeckType == deckType)
		{
			return -1;
		}
		if (deckIndexCache2.m_eDeckType == deckType)
		{
			return 1;
		}
		if (deckIndexCache.m_eDeckType == NKM_DECK_TYPE.NDT_NONE)
		{
			return 1;
		}
		if (deckIndexCache2.m_eDeckType == NKM_DECK_TYPE.NDT_NONE)
		{
			return -1;
		}
		return deckIndexCache.m_eDeckType.CompareTo(deckIndexCache2.m_eDeckType);
	}

	private int CompareByDeckTypeIndexDescending(NKMUnitData lhs, NKMUnitData rhs, NKM_DECK_TYPE deckType)
	{
		NKMDeckIndex deckIndexCache = GetDeckIndexCache(lhs.m_UnitUID, deckType);
		NKMDeckIndex deckIndexCache2 = GetDeckIndexCache(rhs.m_UnitUID, deckType);
		if (deckIndexCache.m_eDeckType == deckIndexCache2.m_eDeckType)
		{
			if (deckIndexCache.m_eDeckType == NKM_DECK_TYPE.NDT_NONE)
			{
				return 0;
			}
			int num = deckIndexCache2.m_iIndex.CompareTo(deckIndexCache.m_iIndex);
			if (num != 0)
			{
				return num;
			}
			NKMDeckData deckData = NKCScenManager.CurrentUserData().m_ArmyData.GetDeckData(deckIndexCache);
			if (deckData == null)
			{
				return 0;
			}
			deckData.HasUnitUid(lhs.m_UnitUID, out var index);
			deckData.HasUnitUid(rhs.m_UnitUID, out var index2);
			return index2.CompareTo(index);
		}
		if (deckIndexCache.m_eDeckType == deckType)
		{
			return -1;
		}
		if (deckIndexCache2.m_eDeckType == deckType)
		{
			return 1;
		}
		if (deckIndexCache.m_eDeckType == NKM_DECK_TYPE.NDT_NONE)
		{
			return 1;
		}
		if (deckIndexCache2.m_eDeckType == NKM_DECK_TYPE.NDT_NONE)
		{
			return -1;
		}
		return deckIndexCache.m_eDeckType.CompareTo(deckIndexCache2.m_eDeckType);
	}

	private int CompareBySquadDungeonDescending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return CompareByDeckTypeIndexDescending(lhs, rhs, NKM_DECK_TYPE.NDT_DAILY);
	}

	private int CompareBySquadDungeonAscending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return CompareByDeckTypeIndexAscending(lhs, rhs, NKM_DECK_TYPE.NDT_DAILY);
	}

	private int CompareBySquadWarfareDescending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return CompareByDeckTypeIndexDescending(lhs, rhs, NKM_DECK_TYPE.NDT_NORMAL);
	}

	private int CompareBySquadWarfareAscending(NKMUnitData lhs, NKMUnitData rhs)
	{
		return CompareByDeckTypeIndexAscending(lhs, rhs, NKM_DECK_TYPE.NDT_NORMAL);
	}

	private int CompareBySquadPvPDescending(NKMUnitData lhs, NKMUnitData rhs)
	{
		NKMDeckIndex deckIndexCache = GetDeckIndexCache(lhs.m_UnitUID, NKM_DECK_TYPE.NDT_PVP);
		NKMDeckIndex deckIndexCache2 = GetDeckIndexCache(rhs.m_UnitUID, NKM_DECK_TYPE.NDT_PVP);
		if (deckIndexCache.m_eDeckType == NKM_DECK_TYPE.NDT_PVP && deckIndexCache2.m_eDeckType == NKM_DECK_TYPE.NDT_PVP)
		{
			int num = deckIndexCache2.m_iIndex.CompareTo(deckIndexCache.m_iIndex);
			if (num != 0)
			{
				return num;
			}
			NKMDeckData deckData = NKCScenManager.CurrentUserData().m_ArmyData.GetDeckData(deckIndexCache);
			if (deckData == null)
			{
				return 0;
			}
			deckData.HasUnitUid(lhs.m_UnitUID, out var index);
			deckData.HasUnitUid(rhs.m_UnitUID, out var index2);
			return index2.CompareTo(index);
		}
		if (deckIndexCache.m_eDeckType == NKM_DECK_TYPE.NDT_PVP)
		{
			return -1;
		}
		if (deckIndexCache2.m_eDeckType == NKM_DECK_TYPE.NDT_PVP)
		{
			return 1;
		}
		return CompareByDeckTypeIndexDescending(lhs, rhs, NKM_DECK_TYPE.NDT_PVP_DEFENCE);
	}

	private int CompareBySquadPvPAscending(NKMUnitData lhs, NKMUnitData rhs)
	{
		NKMDeckIndex deckIndexCache = GetDeckIndexCache(lhs.m_UnitUID, NKM_DECK_TYPE.NDT_PVP);
		NKMDeckIndex deckIndexCache2 = GetDeckIndexCache(rhs.m_UnitUID, NKM_DECK_TYPE.NDT_PVP);
		if (deckIndexCache.m_eDeckType == NKM_DECK_TYPE.NDT_PVP && deckIndexCache2.m_eDeckType == NKM_DECK_TYPE.NDT_PVP)
		{
			int num = deckIndexCache.m_iIndex.CompareTo(deckIndexCache2.m_iIndex);
			if (num != 0)
			{
				return num;
			}
			NKMDeckData deckData = NKCScenManager.CurrentUserData().m_ArmyData.GetDeckData(deckIndexCache);
			if (deckData == null)
			{
				return 0;
			}
			deckData.HasUnitUid(lhs.m_UnitUID, out var index);
			deckData.HasUnitUid(rhs.m_UnitUID, out var index2);
			return index.CompareTo(index2);
		}
		if (deckIndexCache.m_eDeckType == NKM_DECK_TYPE.NDT_PVP)
		{
			return -1;
		}
		if (deckIndexCache2.m_eDeckType == NKM_DECK_TYPE.NDT_PVP)
		{
			return 1;
		}
		return CompareByDeckTypeIndexAscending(lhs, rhs, NKM_DECK_TYPE.NDT_PVP_DEFENCE);
	}

	private int CompareByDeployStatusDescending(NKMUnitData lhs, NKMUnitData rhs)
	{
		if (lhs.OfficeRoomId > 0 && rhs.OfficeRoomId > 0)
		{
			return rhs.OfficeRoomId.CompareTo(lhs.OfficeRoomId);
		}
		if (lhs.OfficeRoomId > 0)
		{
			return -1;
		}
		if (rhs.OfficeRoomId > 0)
		{
			return 1;
		}
		return CompareBySquadPvPDescending(lhs, rhs);
	}

	private int CompareByDeployStatusAscending(NKMUnitData lhs, NKMUnitData rhs)
	{
		if (lhs.OfficeRoomId > 0 && rhs.OfficeRoomId > 0)
		{
			return lhs.OfficeRoomId.CompareTo(rhs.OfficeRoomId);
		}
		if (lhs.OfficeRoomId > 0)
		{
			return 1;
		}
		if (rhs.OfficeRoomId > 0)
		{
			return -1;
		}
		return CompareByDeployStatusAscending(lhs, rhs);
	}

	private int CompareByFavoriteFirst(NKMUnitData lhs, NKMUnitData rhs)
	{
		return rhs.isFavorite.CompareTo(lhs.isFavorite);
	}

	private int CompareByFavoriteLast(NKMUnitData lhs, NKMUnitData rhs)
	{
		return lhs.isFavorite.CompareTo(rhs.isFavorite);
	}

	public static int CompareByRoleAscending(NKMUnitData lhs, NKMUnitData rhs)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(lhs.m_UnitID);
		NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(rhs.m_UnitID);
		return unitTempletBase.m_NKM_UNIT_ROLE_TYPE.CompareTo(unitTempletBase2.m_NKM_UNIT_ROLE_TYPE);
	}

	public static int CompareByRoleDescending(NKMUnitData lhs, NKMUnitData rhs)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(lhs.m_UnitID);
		return NKMUnitManager.GetUnitTempletBase(rhs.m_UnitID).m_NKM_UNIT_ROLE_TYPE.CompareTo(unitTempletBase.m_NKM_UNIT_ROLE_TYPE);
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
		case eSortOption.ID_First:
		case eSortOption.ID_Last:
		case eSortOption.IDX_First:
		case eSortOption.IDX_Last:
			return NKCUtilString.GET_STRING_SORT_IDX;
		case eSortOption.Attack_Low:
		case eSortOption.Attack_High:
			return NKCUtilString.GET_STRING_SORT_ATTACK;
		case eSortOption.Unit_Crit_Low:
		case eSortOption.Unit_Crit_High:
			return NKCUtilString.GET_STRING_SORT_CRIT;
		case eSortOption.Unit_Defense_Low:
		case eSortOption.Unit_Defense_High:
			return NKCUtilString.GET_STRING_SORT_DEFENSE;
		case eSortOption.Unit_Evade_Low:
		case eSortOption.Unit_Evade_High:
			return NKCUtilString.GET_STRING_SORT_EVADE;
		case eSortOption.Health_Low:
		case eSortOption.Health_High:
			return NKCUtilString.GET_STRING_SORT_HEALTH;
		case eSortOption.Unit_Hit_Low:
		case eSortOption.Unit_Hit_High:
			return NKCUtilString.GET_STRING_SORT_HIT;
		case eSortOption.Power_Low:
		case eSortOption.Power_High:
			return NKCUtilString.GET_STRING_SORT_POPWER;
		case eSortOption.Unit_SummonCost_Low:
		case eSortOption.Unit_SummonCost_High:
			return NKCUtilString.GET_STRING_SORT_COST;
		case eSortOption.Player_Level_Low:
		case eSortOption.Player_Level_High:
			return NKCUtilString.GET_STRING_SORT_PLAYER_LEVEL;
		case eSortOption.LoginTime_Latly:
		case eSortOption.LoginTime_Old:
			return NKCUtilString.GET_STRING_SORT_LOGIN_TIME;
		case eSortOption.ScoutProgress_High:
		case eSortOption.ScoutProgress_Low:
			return NKCUtilString.GET_STRING_SORT_SCOUT_PROGRESS;
		case eSortOption.Guild_Grade_High:
		case eSortOption.Guild_Grade_Low:
			return NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_SORT_LIST_GRADE;
		case eSortOption.Guild_WeeklyPoint_High:
		case eSortOption.Guild_WeeklyPoint_Low:
			return NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_SORT_LIST_SCORE;
		case eSortOption.Guild_TotalPoint_High:
		case eSortOption.Guild_TotalPoint_Low:
			return NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_SORT_LIST_SCORE_ALL;
		case eSortOption.Unit_ReduceSkillCool_Low:
		case eSortOption.Unit_ReduceSkillCool_High:
			return NKCUtilString.GET_STRING_OPERATOR_SKILL_COOL_REDUCE;
		case eSortOption.LimitBreak_High:
		case eSortOption.LimitBreak_Low:
			return NKCUtilString.GET_STRING_SORT_LIMIT_BREAK_PROGRESS;
		case eSortOption.Transcendence_High:
		case eSortOption.Transcendence_Low:
			return NKCUtilString.GET_STRING_SORT_TRANSCENDENCE;
		case eSortOption.Unit_Loyalty_High:
		case eSortOption.Unit_Loyalty_Low:
			return NKCUtilString.GET_STRING_SORT_LOYALTY;
		case eSortOption.Deploy_Status_High:
		case eSortOption.Deploy_Status_Low:
			return NKCUtilString.GET_STRING_SORT_DEPLOY_STATUS;
		case eSortOption.Squad_Dungeon_High:
		case eSortOption.Squad_Dungeon_Low:
			return NKCUtilString.GET_STRING_SORT_SQUAD_DUNGEON;
		case eSortOption.Squad_Gauntlet_High:
		case eSortOption.Squad_Gauntlet_Low:
			return NKCUtilString.GET_STRING_SORT_SQUAD_PVP;
		case eSortOption.CustomAscend1:
		case eSortOption.CustomDescend1:
		case eSortOption.CustomAscend2:
		case eSortOption.CustomDescend2:
		case eSortOption.CustomAscend3:
		case eSortOption.CustomDescend3:
			return string.Empty;
		}
	}

	public NKMUnitData AutoSelect(HashSet<long> setExcludeUnitUid, AutoSelectExtraFilter extrafilter = null)
	{
		List<NKMUnitData> list = AutoSelect(setExcludeUnitUid, 1, extrafilter);
		if (list.Count > 0)
		{
			return list[0];
		}
		return null;
	}

	public List<NKMUnitData> AutoSelect(HashSet<long> setExcludeUnitUid, int count, AutoSelectExtraFilter extrafilter = null)
	{
		List<NKMUnitData> list = new List<NKMUnitData>();
		for (int i = 0; i < SortedUnitList.Count; i++)
		{
			if (list.Count >= count)
			{
				break;
			}
			NKMUnitData nKMUnitData = SortedUnitList[i];
			if (nKMUnitData != null && (setExcludeUnitUid == null || !setExcludeUnitUid.Contains(nKMUnitData.m_UnitUID)) && (extrafilter == null || extrafilter(nKMUnitData)) && IsUnitSelectable(nKMUnitData))
			{
				list.Add(nKMUnitData);
			}
		}
		return list;
	}

	public static NKMUnitData MakeTempUnitData(int unitID, int level = 1, short limitBreakLevel = 0)
	{
		return new NKMUnitData
		{
			m_UnitID = unitID,
			m_UnitUID = unitID,
			m_UserUID = 0L,
			m_UnitLevel = level,
			m_iUnitLevelEXP = 0,
			m_LimitBreakLevel = limitBreakLevel,
			m_bLock = false
		};
	}

	public static NKMUnitData MakeTempUnitData(NKMSkinTemplet skinTemplet, int level = 1, short limitBreakLevel = 0)
	{
		return new NKMUnitData
		{
			m_UnitID = skinTemplet.m_SkinEquipUnitID,
			m_UnitUID = skinTemplet.m_SkinID,
			m_SkinID = skinTemplet.m_SkinID,
			m_UserUID = 0L,
			m_UnitLevel = level,
			m_iUnitLevelEXP = 0,
			m_LimitBreakLevel = limitBreakLevel,
			m_bLock = false
		};
	}

	public static eFilterOption GetFilterOption(NKM_UNIT_STYLE_TYPE styleType)
	{
		return styleType switch
		{
			NKM_UNIT_STYLE_TYPE.NUST_INVALID => eFilterOption.Everything, 
			NKM_UNIT_STYLE_TYPE.NUST_COUNTER => eFilterOption.Unit_Counter, 
			NKM_UNIT_STYLE_TYPE.NUST_MECHANIC => eFilterOption.Unit_Mechanic, 
			NKM_UNIT_STYLE_TYPE.NUST_SOLDIER => eFilterOption.Unit_Soldier, 
			NKM_UNIT_STYLE_TYPE.NUST_CORRUPTED => eFilterOption.Unit_Corrupted, 
			NKM_UNIT_STYLE_TYPE.NUST_REPLACER => eFilterOption.Unit_Replacer, 
			NKM_UNIT_STYLE_TYPE.NUST_TRAINER => eFilterOption.Unit_Trainer, 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_ASSAULT => eFilterOption.Ship_Assault, 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_HEAVY => eFilterOption.Ship_Heavy, 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_CRUISER => eFilterOption.Ship_Cruiser, 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_SPECIAL => eFilterOption.Ship_Special, 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_PATROL => eFilterOption.Ship_Patrol, 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_ETC => eFilterOption.Ship_Etc, 
			_ => eFilterOption.Nothing, 
		};
	}

	public static eFilterOption GetFilterOption(NKM_UNIT_ROLE_TYPE roleType)
	{
		return roleType switch
		{
			NKM_UNIT_ROLE_TYPE.NURT_STRIKER => eFilterOption.Role_Striker, 
			NKM_UNIT_ROLE_TYPE.NURT_RANGER => eFilterOption.Role_Ranger, 
			NKM_UNIT_ROLE_TYPE.NURT_DEFENDER => eFilterOption.Role_Defender, 
			NKM_UNIT_ROLE_TYPE.NURT_SNIPER => eFilterOption.Role_Sniper, 
			NKM_UNIT_ROLE_TYPE.NURT_SIEGE => eFilterOption.Role_Siege, 
			NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER => eFilterOption.Role_Supporter, 
			NKM_UNIT_ROLE_TYPE.NURT_TOWER => eFilterOption.Role_Tower, 
			_ => eFilterOption.Everything, 
		};
	}

	public static eFilterOption GetFilterOption(NKM_FIND_TARGET_TYPE targetType)
	{
		switch (targetType)
		{
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_AIR_BOSS_LAST:
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_MY_TEAM_AIR:
			return eFilterOption.Unit_Target_Air;
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND_RANGER_SUPPORTER_SNIPER_FIRST:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_LAND_BOSS_LAST:
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_MY_TEAM_LAND:
			return eFilterOption.Unit_Target_Ground;
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_AIR_FIRST:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_BOSS_LAST:
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_ONLY:
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_MY_TEAM:
			return eFilterOption.Unit_Target_All;
		default:
			return eFilterOption.Nothing;
		}
	}

	public static List<long> MakeAutoCompleteDeck(NKMUserData userData, NKMDeckIndex currentDeckIndex, NKMDeckData deckData, bool bUseBanData, bool bUseUpData)
	{
		if (deckData == null)
		{
			Debug.LogError("deckData is null");
			return new List<long>();
		}
		UnitListOptions options = new UnitListOptions
		{
			eDeckType = currentDeckIndex.m_eDeckType,
			setExcludeUnitID = null,
			setOnlyIncludeUnitID = null,
			setDuplicateUnitID = new HashSet<int>(),
			setExcludeUnitUID = new HashSet<long>(),
			bExcludeLockedUnit = false,
			bExcludeDeckedUnit = false,
			setFilterOption = null,
			lstSortOption = new List<eSortOption>
			{
				eSortOption.Power_High,
				eSortOption.UID_First
			},
			bDescending = true,
			bHideDeckedUnit = !NKMArmyData.IsAllowedSameUnitInMultipleDeck(currentDeckIndex.m_eDeckType),
			bPushBackUnselectable = true,
			bIncludeUndeckableUnit = false,
			bIgnoreCityState = true,
			bIgnoreWorldMapLeader = true
		};
		if (currentDeckIndex.m_eDeckType == NKM_DECK_TYPE.NDT_TOURNAMENT)
		{
			options.setExcludeUnitID = NKCTournamentManager.GetTournamentFinalBanIds(NKM_UNIT_TYPE.NUT_NORMAL);
			options.setExcludeUnitBaseID = NKCTournamentManager.GetTournamentFinalBanIds(NKM_UNIT_TYPE.NUT_SHIP);
		}
		if (userData != null)
		{
			options.MakeDuplicateUnitSet(currentDeckIndex, 0L, userData.m_ArmyData);
		}
		return AutoCompleteDeck(userData, deckData.m_listDeckUnitUID, options, bLocal: false);
	}

	public static List<long> MakeLocalAutoCompleteDeck(NKMUserData userData, NKMDeckIndex deckIndex, List<long> deckUnitList, bool prohibitSameUnitId)
	{
		if (deckUnitList == null)
		{
			Debug.LogError("deckData is null");
			return new List<long>();
		}
		UnitListOptions options = new UnitListOptions
		{
			eDeckType = deckIndex.m_eDeckType,
			setExcludeUnitID = null,
			setOnlyIncludeUnitID = null,
			setDuplicateUnitID = new HashSet<int>(),
			setExcludeUnitUID = new HashSet<long>(),
			bExcludeLockedUnit = false,
			bExcludeDeckedUnit = false,
			setFilterOption = null,
			lstSortOption = new List<eSortOption>
			{
				eSortOption.Power_High,
				eSortOption.UID_First
			},
			bDescending = true,
			bHideDeckedUnit = !NKMArmyData.IsAllowedSameUnitInMultipleDeck(deckIndex.m_eDeckType),
			bPushBackUnselectable = true,
			bIncludeUndeckableUnit = false,
			bIgnoreCityState = true,
			bIgnoreWorldMapLeader = true
		};
		foreach (KeyValuePair<int, NKMEventDeckData> allLocalDeckDatum in NKCLocalDeckDataManager.GetAllLocalDeckData())
		{
			if (!prohibitSameUnitId && allLocalDeckDatum.Key == deckIndex.m_iIndex)
			{
				continue;
			}
			foreach (KeyValuePair<int, long> item in allLocalDeckDatum.Value.m_dicUnit)
			{
				long value = item.Value;
				if (value != 0L && userData != null)
				{
					NKMUnitData unitFromUID = userData.m_ArmyData.GetUnitFromUID(value);
					if (NKMUnitManager.GetUnitTempletBase(unitFromUID) != null)
					{
						options.setExcludeUnitUID.Add(value);
						options.setDuplicateUnitID.Add(unitFromUID.m_UnitID);
					}
				}
			}
		}
		return AutoCompleteDeck(userData, deckUnitList, options, bLocal: true);
	}

	private static List<long> AutoCompleteDeck(NKMUserData userData, List<long> deckUnitList, UnitListOptions options, bool bLocal)
	{
		List<long> retLst = new List<long>();
		options.setFilterOption = new HashSet<eFilterOption> { GetFilterOption(NKM_UNIT_ROLE_TYPE.NURT_INVALID) };
		NKCUnitSort ssUnitSort = new NKCUnitSort(userData, options, bLocal);
		NKMUnitData nKMUnitData = ssUnitSort.AutoSelect(null);
		if (nKMUnitData == null)
		{
			Halt();
			return retLst;
		}
		float num = ssUnitSort.GetUnitPowerCache(nKMUnitData.m_UnitUID);
		float unitPowerLowerLimit = ((num > 9000f) ? (num * 0.5f) : (num * 0.1f));
		List<NKMUnitData> list = ssUnitSort.AutoSelect(null, ssUnitSort.GetCurrentUnitList().Count, AutoSelectPowerFilter);
		if (list.Count == 0)
		{
			Halt();
			return retLst;
		}
		for (int i = 0; i < deckUnitList.Count; i++)
		{
			if (deckUnitList[i] != 0L)
			{
				retLst.Add(deckUnitList[i]);
				continue;
			}
			NKMUnitData nKMUnitData2 = null;
			AutoCompleteComparerer autoCompleteComparerer = new AutoCompleteComparerer();
			switch (i % 8)
			{
			default:
				autoCompleteComparerer.BuildComparer(bAwakenFirst: true, NKM_UNIT_GRADE.NUG_SSR, NKM_UNIT_ROLE_TYPE.NURT_DEFENDER, 6, bUseBanData: false, bUseUpData: false, ssUnitSort);
				break;
			case 1:
				autoCompleteComparerer.BuildComparer(bAwakenFirst: true, NKM_UNIT_GRADE.NUG_SSR, NKM_UNIT_ROLE_TYPE.NURT_RANGER, 6, bUseBanData: false, bUseUpData: false, ssUnitSort);
				break;
			case 2:
				autoCompleteComparerer.BuildComparer(bAwakenFirst: false, NKM_UNIT_GRADE.NUG_SR, NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER, 3, bUseBanData: false, bUseUpData: false, ssUnitSort);
				break;
			case 3:
				autoCompleteComparerer.BuildComparer(bAwakenFirst: false, NKM_UNIT_GRADE.NUG_SSR, NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER, 3, bUseBanData: false, bUseUpData: false, ssUnitSort);
				break;
			case 4:
				autoCompleteComparerer.BuildComparer(bAwakenFirst: false, NKM_UNIT_GRADE.NUG_SSR, NKM_UNIT_ROLE_TYPE.NURT_DEFENDER, 3, bUseBanData: false, bUseUpData: false, ssUnitSort);
				break;
			case 5:
				autoCompleteComparerer.BuildComparer(bAwakenFirst: false, NKM_UNIT_GRADE.NUG_SSR, NKM_UNIT_ROLE_TYPE.NURT_RANGER, 3, bUseBanData: false, bUseUpData: false, ssUnitSort);
				break;
			case 6:
				autoCompleteComparerer.BuildComparer(bAwakenFirst: false, NKM_UNIT_GRADE.NUG_SSR, NKM_UNIT_ROLE_TYPE.NURT_STRIKER, 3, bUseBanData: false, bUseUpData: false, ssUnitSort);
				break;
			case 7:
				autoCompleteComparerer.BuildComparer(bAwakenFirst: false, NKM_UNIT_GRADE.NUG_SR, NKM_UNIT_ROLE_TYPE.NURT_RANGER, 4, bUseBanData: false, bUseUpData: false, ssUnitSort);
				break;
			}
			list.Sort(autoCompleteComparerer);
			nKMUnitData2 = list.First();
			if (nKMUnitData2 == null)
			{
				Halt();
				return retLst;
			}
			Debug.Log($"AutoSelect : {NKMUnitManager.GetUnitTempletBase(nKMUnitData2.m_UnitID).GetUnitName()} Selected for index {i}");
			options.setDuplicateUnitID.Add(nKMUnitData2.m_UnitID);
			if (nKMUnitData2.GetUnitTempletBase().m_BaseUnitID != nKMUnitData2.m_UnitID)
			{
				options.setDuplicateUnitID.Add(nKMUnitData2.GetUnitTempletBase().m_BaseUnitID);
			}
			options.setExcludeUnitUID.Add(nKMUnitData2.m_UnitUID);
			retLst.Add(nKMUnitData2.m_UnitUID);
			list.RemoveAll(FilterDuplicate);
			if (list.Count == 0)
			{
				Halt();
				return retLst;
			}
		}
		return retLst;
		bool AutoSelectPowerFilter(NKMUnitData unitData)
		{
			if (unitData == null)
			{
				return false;
			}
			if (!ssUnitSort.IsUnitSelectable(unitData))
			{
				return false;
			}
			return (float)ssUnitSort.GetUnitPowerCache(unitData.m_UnitUID) >= unitPowerLowerLimit;
		}
		bool FilterDuplicate(NKMUnitData unitData)
		{
			if (unitData == null)
			{
				return true;
			}
			if (options.setExcludeUnitUID.Contains(unitData.m_UnitUID))
			{
				return true;
			}
			if (unitData.GetUnitTempletBase().m_BaseUnitID != unitData.m_UnitID && options.setDuplicateUnitID.Contains(unitData.GetUnitTempletBase().m_BaseUnitID))
			{
				return true;
			}
			if (options.setDuplicateUnitID.Contains(unitData.m_UnitID))
			{
				return true;
			}
			return false;
		}
		void Halt()
		{
			while (retLst.Count < deckUnitList.Count)
			{
				retLst.Add(deckUnitList[retLst.Count]);
			}
		}
	}

	public static long LocalAutoSelectShip(NKMUserData userData, NKMDeckIndex selectedDeckIndex, long deckShip, bool prohibitSameUnitId)
	{
		if (deckShip > 0)
		{
			return deckShip;
		}
		long result = 0L;
		UnitListOptions options = new UnitListOptions
		{
			eDeckType = selectedDeckIndex.m_eDeckType,
			setExcludeUnitID = null,
			setOnlyIncludeUnitID = null,
			setDuplicateUnitID = new HashSet<int>(),
			setExcludeUnitUID = new HashSet<long>(),
			bExcludeLockedUnit = false,
			bExcludeDeckedUnit = false,
			setFilterOption = null,
			lstSortOption = new List<eSortOption>
			{
				eSortOption.Power_High,
				eSortOption.UID_First
			},
			bDescending = true,
			bHideDeckedUnit = !NKMArmyData.IsAllowedSameUnitInMultipleDeck(selectedDeckIndex.m_eDeckType),
			bPushBackUnselectable = true,
			bIncludeUndeckableUnit = false
		};
		foreach (KeyValuePair<int, NKMEventDeckData> allLocalDeckDatum in NKCLocalDeckDataManager.GetAllLocalDeckData())
		{
			NKMUnitData shipFromUID = userData.m_ArmyData.GetShipFromUID(allLocalDeckDatum.Value.m_ShipUID);
			if (shipFromUID == null)
			{
				continue;
			}
			options.setExcludeUnitUID.Add(shipFromUID.m_UnitUID);
			NKMUnitTempletBase shipTempletBase = NKMUnitManager.GetUnitTempletBase(shipFromUID.m_UnitID);
			if (shipTempletBase == null)
			{
				continue;
			}
			foreach (NKMUnitTempletBase item in NKMUnitTempletBase.Values.Where((NKMUnitTempletBase e) => e.m_ShipGroupID == shipTempletBase.m_ShipGroupID))
			{
				options.setDuplicateUnitID.Add(item.m_UnitID);
			}
		}
		NKMUnitData nKMUnitData = new NKCShipSort(userData, options, useLocal: true).AutoSelect(null);
		if (nKMUnitData != null)
		{
			result = nKMUnitData.m_UnitUID;
		}
		return result;
	}

	public static long LocalAutoSelectOperator(NKMUserData userData, NKMDeckIndex selectedDeckIndex, long deckOperator, bool prohibitSameUnitId)
	{
		if (deckOperator > 0)
		{
			return deckOperator;
		}
		NKCOperatorSortSystem.OperatorListOptions options = new NKCOperatorSortSystem.OperatorListOptions
		{
			eDeckType = selectedDeckIndex.m_eDeckType,
			setExcludeOperatorID = null,
			setOnlyIncludeOperatorID = null,
			setDuplicateOperatorID = new HashSet<int>(),
			setExcludeOperatorUID = new HashSet<long>(),
			setFilterOption = null,
			lstSortOption = new List<NKCOperatorSortSystem.eSortOption>
			{
				NKCOperatorSortSystem.eSortOption.Power_High,
				NKCOperatorSortSystem.eSortOption.UID_First
			}
		};
		options.SetBuildOption(true, BUILD_OPTIONS.DESCENDING, BUILD_OPTIONS.PUSHBACK_UNSELECTABLE);
		foreach (KeyValuePair<int, NKMEventDeckData> allLocalDeckDatum in NKCLocalDeckDataManager.GetAllLocalDeckData())
		{
			NKMOperator operatorFromUId = userData.m_ArmyData.GetOperatorFromUId(allLocalDeckDatum.Value.m_OperatorUID);
			if (operatorFromUId != null)
			{
				options.setExcludeOperatorUID.Add(operatorFromUId.uid);
				options.setDuplicateOperatorID.Add(operatorFromUId.id);
			}
		}
		return new NKCOperatorSort(userData, options, local: true).AutoSelect(null)?.uid ?? 0;
	}

	private static NKM_UNIT_ROLE_TYPE GetAutoUnitRoleNext(NKM_UNIT_ROLE_TYPE current)
	{
		return current switch
		{
			NKM_UNIT_ROLE_TYPE.NURT_STRIKER => NKM_UNIT_ROLE_TYPE.NURT_DEFENDER, 
			NKM_UNIT_ROLE_TYPE.NURT_DEFENDER => NKM_UNIT_ROLE_TYPE.NURT_RANGER, 
			NKM_UNIT_ROLE_TYPE.NURT_RANGER => NKM_UNIT_ROLE_TYPE.NURT_SNIPER, 
			NKM_UNIT_ROLE_TYPE.NURT_SNIPER => NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER, 
			_ => NKM_UNIT_ROLE_TYPE.NURT_STRIKER, 
		};
	}

	public static List<eSortOption> GetDefaultSortOptions(NKM_UNIT_TYPE unitType, bool bIsCollection, bool bIsSelection = false)
	{
		if (bIsCollection)
		{
			if (bIsSelection)
			{
				return DEFAULT_UNIT_SELECTION_SORT_OPTION_LIST;
			}
			return DEFAULT_UNIT_COLLECTION_SORT_OPTION_LIST;
		}
		return unitType switch
		{
			NKM_UNIT_TYPE.NUT_NORMAL => DEFAULT_UNIT_SORT_OPTION_LIST, 
			NKM_UNIT_TYPE.NUT_SHIP => DEFAULT_SHIP_SORT_OPTION_LIST, 
			_ => null, 
		};
	}

	public static List<eSortOption> AddDefaultSortOptions(List<eSortOption> sortOptions, NKM_UNIT_TYPE unitType, bool bIsCollection)
	{
		List<eSortOption> defaultSortOptions = GetDefaultSortOptions(unitType, bIsCollection);
		if (defaultSortOptions != null)
		{
			sortOptions.AddRange(defaultSortOptions);
		}
		return sortOptions;
	}

	public List<NKMUnitData> GetCurrentUnitList()
	{
		return m_lstCurrentUnitList;
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
}
