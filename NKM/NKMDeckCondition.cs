using System;
using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet;

namespace NKM;

public class NKMDeckCondition
{
	public enum DECK_CONDITION
	{
		UNIT_STYLE,
		UNIT_GRADE,
		UNIT_COST,
		UNIT_ROLE,
		UNIT_LEVEL,
		SHIP_STYLE,
		SHIP_LEVEL,
		UNIT_ID_NOT
	}

	public enum ALL_DECK_CONDITION
	{
		UNIT_COST_TOTAL,
		AWAKEN_COUNT,
		REARM_COUNT,
		UNIT_GROUND_COUNT,
		UNIT_AIR_COUNT,
		UNIT_GRADE_COUNT
	}

	public enum GAME_CONDITION
	{
		LEVEL_CAP,
		MODIFY_START_COST,
		FORCE_REARM_TO_BASIC,
		SET_START_COST
	}

	public enum MORE_LESS
	{
		EQUAL,
		NOT,
		MORE,
		LESS
	}

	public abstract class EventDeckCondition
	{
		public MORE_LESS eMoreLess;

		public int Value;

		public List<int> lstValue;

		public abstract string ConditionName { get; }

		public bool IsValueOk(bool currentValue)
		{
			switch (eMoreLess)
			{
			case MORE_LESS.EQUAL:
				return currentValue;
			case MORE_LESS.NOT:
				return !currentValue;
			default:
				Log.Error("bool Value with non EQUAL-NOT condition??", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDeckCondition.cs", 74);
				return false;
			}
		}

		public bool IsValueOk(int currentValue)
		{
			switch (eMoreLess)
			{
			default:
				if (lstValue != null)
				{
					for (int j = 0; j < lstValue.Count; j++)
					{
						if (lstValue[j] == currentValue)
						{
							return true;
						}
					}
					return false;
				}
				return currentValue == Value;
			case MORE_LESS.LESS:
				return currentValue <= Value;
			case MORE_LESS.MORE:
				return currentValue >= Value;
			case MORE_LESS.NOT:
				if (lstValue != null)
				{
					for (int i = 0; i < lstValue.Count; i++)
					{
						if (lstValue[i] == currentValue)
						{
							return false;
						}
					}
					return true;
				}
				return currentValue != Value;
			}
		}

		public abstract bool IsProhibited();
	}

	public class SingleCondition : EventDeckCondition, IComparable<SingleCondition>
	{
		public DECK_CONDITION eCondition;

		public override string ConditionName => eCondition.ToString();

		public NKM_ERROR_CODE IsValueOk(NKMUnitData unitData)
		{
			if (unitData == null)
			{
				return NKM_ERROR_CODE.NEC_OK;
			}
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
			switch (eCondition)
			{
			case DECK_CONDITION.SHIP_STYLE:
			case DECK_CONDITION.SHIP_LEVEL:
				if (unitTempletBase.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_SHIP)
				{
					return NKM_ERROR_CODE.NEC_OK;
				}
				break;
			case DECK_CONDITION.UNIT_STYLE:
			case DECK_CONDITION.UNIT_GRADE:
			case DECK_CONDITION.UNIT_COST:
			case DECK_CONDITION.UNIT_ROLE:
			case DECK_CONDITION.UNIT_LEVEL:
				if (unitTempletBase.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_NORMAL)
				{
					return NKM_ERROR_CODE.NEC_OK;
				}
				break;
			case DECK_CONDITION.UNIT_ID_NOT:
			{
				NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(Value);
				if (unitTempletBase2 == null)
				{
					Log.Error($"Target Unit {Value} not exist!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDeckCondition.cs", 163);
					return NKM_ERROR_CODE.NEC_OK;
				}
				if (unitTempletBase2.m_NKM_UNIT_TYPE != unitTempletBase.m_NKM_UNIT_TYPE)
				{
					return NKM_ERROR_CODE.NEC_OK;
				}
				break;
			}
			default:
				return NKM_ERROR_CODE.NEC_OK;
			}
			switch (eCondition)
			{
			default:
				return NKM_ERROR_CODE.NEC_OK;
			case DECK_CONDITION.UNIT_COST:
			{
				NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitData.m_UnitID);
				if (!IsValueOk(unitStatTemplet.GetRespawnCost(bLeader: false, null, null)))
				{
					return NKM_ERROR_CODE.NEC_FAIL_EVENTDECK_CONDITION_UNIT_COST;
				}
				break;
			}
			case DECK_CONDITION.UNIT_LEVEL:
				if (!IsValueOk(unitData.m_UnitLevel))
				{
					return NKM_ERROR_CODE.NEC_FAIL_EVENTDECK_CONDITION_UNIT_LEVEL;
				}
				break;
			case DECK_CONDITION.UNIT_GRADE:
				if (!IsValueOk((int)unitTempletBase.m_NKM_UNIT_GRADE))
				{
					return NKM_ERROR_CODE.NEC_FAIL_EVENTDECK_CONDITION_UNIT_RARITY;
				}
				break;
			case DECK_CONDITION.UNIT_ROLE:
				if (!IsValueOk((int)unitTempletBase.m_NKM_UNIT_ROLE_TYPE))
				{
					return NKM_ERROR_CODE.NEC_FAIL_EVENTDECK_CONDITION_UNIT_ROLE;
				}
				break;
			case DECK_CONDITION.UNIT_STYLE:
				if (!IsValueOk((int)unitTempletBase.m_NKM_UNIT_STYLE_TYPE))
				{
					return NKM_ERROR_CODE.NEC_FAIL_EVENTDECK_CONDITION_UNIT_STYLE;
				}
				break;
			case DECK_CONDITION.SHIP_LEVEL:
				if (!IsValueOk(unitData.m_UnitLevel))
				{
					return NKM_ERROR_CODE.NEC_FAIL_EVENTDECK_CONDITION_SHIP_LEVEL;
				}
				break;
			case DECK_CONDITION.SHIP_STYLE:
				if (!IsValueOk((int)unitTempletBase.m_NKM_UNIT_STYLE_TYPE))
				{
					return NKM_ERROR_CODE.NEC_FAIL_EVENTDECK_CONDITION_SHIP_STYLE;
				}
				break;
			case DECK_CONDITION.UNIT_ID_NOT:
				if (!IsValueOk(unitData.m_UnitID))
				{
					return NKM_ERROR_CODE.NEC_FAIL_EVENTDECK_CONDITION_UNIT_ID;
				}
				break;
			}
			return NKM_ERROR_CODE.NEC_OK;
		}

		public NKM_ERROR_CODE IsValueOk(NKMOperator operatorData)
		{
			if (!IsConditionApplyToOperator())
			{
				return NKM_ERROR_CODE.NEC_OK;
			}
			if (operatorData == null)
			{
				return NKM_ERROR_CODE.NEC_OK;
			}
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(operatorData);
			if (eCondition == DECK_CONDITION.UNIT_ID_NOT)
			{
				NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(Value);
				if (unitTempletBase2 == null)
				{
					Log.Error($"Target Unit {Value} not exist!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDeckCondition.cs", 261);
					return NKM_ERROR_CODE.NEC_OK;
				}
				if (unitTempletBase2.m_NKM_UNIT_TYPE != unitTempletBase.m_NKM_UNIT_TYPE)
				{
					return NKM_ERROR_CODE.NEC_OK;
				}
				if (!IsValueOk(operatorData.id))
				{
					return NKM_ERROR_CODE.NEC_FAIL_EVENTDECK_CONDITION_UNIT_ID;
				}
				return NKM_ERROR_CODE.NEC_OK;
			}
			return NKM_ERROR_CODE.NEC_OK;
		}

		public int CompareTo(SingleCondition other)
		{
			int num = eCondition.CompareTo(other.eCondition);
			if (num != 0)
			{
				return num;
			}
			int num2 = eMoreLess.CompareTo(other.eMoreLess);
			if (num2 != 0)
			{
				return num2;
			}
			return Value.CompareTo(other.Value);
		}

		public override bool IsProhibited()
		{
			return eMoreLess == MORE_LESS.NOT;
		}

		public bool IsConditionApplyToOperator()
		{
			if (eCondition == DECK_CONDITION.UNIT_ID_NOT)
			{
				return true;
			}
			return false;
		}

		public NKM_ERROR_CODE GetFailErrorCode()
		{
			return NKM_ERROR_CODE.NEC_FAIL_EVENTDECK_CONDITION;
		}
	}

	public class AllDeckCondition : EventDeckCondition, IComparable<AllDeckCondition>
	{
		public ALL_DECK_CONDITION eCondition;

		public override string ConditionName => eCondition.ToString();

		public NKM_ERROR_CODE IsValueOk(NKMUnitData unitData)
		{
			if (unitData == null)
			{
				return NKM_ERROR_CODE.NEC_OK;
			}
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
			ALL_DECK_CONDITION aLL_DECK_CONDITION = eCondition;
			if ((uint)(aLL_DECK_CONDITION - 1) <= 4u && unitTempletBase.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_NORMAL)
			{
				return NKM_ERROR_CODE.NEC_OK;
			}
			if (!IsProhibited())
			{
				return NKM_ERROR_CODE.NEC_OK;
			}
			if (GetAllDeckConditionValue(unitData) > 0)
			{
				return GetFailErrorCode();
			}
			return NKM_ERROR_CODE.NEC_OK;
		}

		public NKM_ERROR_CODE IsValueOk(NKMOperator operatorData)
		{
			return NKM_ERROR_CODE.NEC_OK;
		}

		public int CompareTo(AllDeckCondition other)
		{
			int num = eCondition.CompareTo(other.eCondition);
			if (num != 0)
			{
				return num;
			}
			int num2 = eMoreLess.CompareTo(other.eMoreLess);
			if (num2 != 0)
			{
				return num2;
			}
			return Value.CompareTo(other.Value);
		}

		public override bool IsProhibited()
		{
			if (eMoreLess == MORE_LESS.EQUAL || eMoreLess == MORE_LESS.LESS)
			{
				return Value == 0;
			}
			return false;
		}

		public bool IsConditionApplyToOperator()
		{
			return false;
		}

		public NKM_ERROR_CODE GetFailErrorCode()
		{
			switch (eCondition)
			{
			case ALL_DECK_CONDITION.REARM_COUNT:
				return NKM_ERROR_CODE.NEC_FAIL_EVENTDECK_CONDITION;
			case ALL_DECK_CONDITION.UNIT_COST_TOTAL:
				return NKM_ERROR_CODE.NEC_FAIL_EVENTDECK_CONDITION_UNIT_COST_TOTAL;
			case ALL_DECK_CONDITION.AWAKEN_COUNT:
				return NKM_ERROR_CODE.NEC_FAIL_EVENTDECK_CONDITION_AWAKEN_COUNT;
			case ALL_DECK_CONDITION.UNIT_GROUND_COUNT:
			case ALL_DECK_CONDITION.UNIT_AIR_COUNT:
				return NKM_ERROR_CODE.NEC_FAIL_EVENTDECK_CONDITION_UNIT_MOVE_TYPE;
			case ALL_DECK_CONDITION.UNIT_GRADE_COUNT:
				return NKM_ERROR_CODE.NEC_FAIL_EVENTDECK_CONDITION_UNIT_RARITY;
			default:
				return NKM_ERROR_CODE.NEC_FAIL_EVENTDECK_CONDITION;
			}
		}

		public int GetAllDeckConditionValue(NKMUnitTempletBase unitTempletBase)
		{
			if (unitTempletBase == null)
			{
				return 0;
			}
			switch (eCondition)
			{
			case ALL_DECK_CONDITION.UNIT_COST_TOTAL:
				return NKMUnitManager.GetUnitStatTemplet(unitTempletBase.m_UnitID).GetRespawnCost(bLeader: false, null, null);
			case ALL_DECK_CONDITION.REARM_COUNT:
				if (!unitTempletBase.IsRearmUnit)
				{
					return 0;
				}
				return 1;
			case ALL_DECK_CONDITION.AWAKEN_COUNT:
				if (!unitTempletBase.m_bAwaken)
				{
					return 0;
				}
				return 1;
			case ALL_DECK_CONDITION.UNIT_GROUND_COUNT:
				if (!unitTempletBase.m_bAirUnit)
				{
					return 1;
				}
				return 0;
			case ALL_DECK_CONDITION.UNIT_AIR_COUNT:
				if (!unitTempletBase.m_bAirUnit)
				{
					return 0;
				}
				return 1;
			case ALL_DECK_CONDITION.UNIT_GRADE_COUNT:
				if (!lstValue.Contains((int)unitTempletBase.m_NKM_UNIT_GRADE))
				{
					return 0;
				}
				return 1;
			default:
				return 0;
			}
		}

		public int GetAllDeckConditionValue(NKMUnitData unitData)
		{
			if (unitData == null)
			{
				return 0;
			}
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
			return GetAllDeckConditionValue(unitTempletBase);
		}

		public bool CanAddThisUnit(NKMUnitData unitData, int currentValue)
		{
			int allDeckConditionValue = GetAllDeckConditionValue(unitData);
			int num = currentValue + allDeckConditionValue;
			return eMoreLess switch
			{
				MORE_LESS.MORE => true, 
				MORE_LESS.NOT => num != Value, 
				_ => num <= Value, 
			};
		}
	}

	public class GameCondition
	{
		public GAME_CONDITION eCondition;

		public int Value;

		public bool IsPenalty()
		{
			return Value < 0;
		}
	}

	private enum MORE_LESS_TYPE
	{
		NO_USE,
		EQUAL_OR_NOT,
		NO_NOT,
		BOOL,
		ALL
	}

	public Dictionary<ALL_DECK_CONDITION, AllDeckCondition> m_dicAllDeckCondition;

	public List<SingleCondition> m_lstUnitCondition = new List<SingleCondition>();

	public Dictionary<GAME_CONDITION, GameCondition> m_dicGameCondition = new Dictionary<GAME_CONDITION, GameCondition>();

	public int ConditionCount => ((m_dicAllDeckCondition != null) ? m_dicAllDeckCondition.Count : 0) + m_lstUnitCondition.Count;

	public void AddAllDeckCondition(AllDeckCondition allCondition)
	{
		if (m_dicAllDeckCondition == null)
		{
			m_dicAllDeckCondition = new Dictionary<ALL_DECK_CONDITION, AllDeckCondition>();
		}
		m_dicAllDeckCondition.Add(allCondition.eCondition, allCondition);
	}

	public AllDeckCondition GetAllDeckCondition(ALL_DECK_CONDITION condition)
	{
		if (m_dicAllDeckCondition == null)
		{
			return null;
		}
		if (m_dicAllDeckCondition.TryGetValue(condition, out var value))
		{
			return value;
		}
		return null;
	}

	public GameCondition GetGameCondition(GAME_CONDITION type)
	{
		if (m_dicGameCondition.TryGetValue(type, out var value))
		{
			return value;
		}
		return null;
	}

	public NKM_ERROR_CODE CheckDeckCondition(NKMArmyData armyData, NKMDeckData deckData)
	{
		long shipUID = deckData.m_ShipUID;
		NKMUnitData shipFromUID = armyData.GetShipFromUID(shipUID);
		NKM_ERROR_CODE nKM_ERROR_CODE = CheckUnitCondition(shipFromUID);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			return nKM_ERROR_CODE;
		}
		if (NKMOpenTagManager.IsOpened("OPERATOR"))
		{
			NKMOperator operatorFromUId = armyData.GetOperatorFromUId(deckData.m_OperatorUID);
			NKM_ERROR_CODE nKM_ERROR_CODE2 = CheckOperatorCondition(operatorFromUId);
			if (nKM_ERROR_CODE2 != NKM_ERROR_CODE.NEC_OK)
			{
				return nKM_ERROR_CODE2;
			}
		}
		for (int i = 0; i < deckData.m_listDeckUnitUID.Count; i++)
		{
			long unitUid = deckData.m_listDeckUnitUID[i];
			NKMUnitData unitFromUID = armyData.GetUnitFromUID(unitUid);
			NKM_ERROR_CODE nKM_ERROR_CODE3 = CheckUnitCondition(unitFromUID);
			if (nKM_ERROR_CODE3 != NKM_ERROR_CODE.NEC_OK)
			{
				return nKM_ERROR_CODE3;
			}
		}
		if (m_dicAllDeckCondition != null)
		{
			foreach (AllDeckCondition value in m_dicAllDeckCondition.Values)
			{
				int num = 0;
				for (int j = 0; j < deckData.m_listDeckUnitUID.Count; j++)
				{
					long unitUid2 = deckData.m_listDeckUnitUID[j];
					NKMUnitData unitFromUID2 = armyData.GetUnitFromUID(unitUid2);
					if (unitFromUID2 != null)
					{
						num += value.GetAllDeckConditionValue(unitFromUID2);
					}
				}
				if (!value.IsValueOk(num))
				{
					return value.GetFailErrorCode();
				}
			}
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public NKM_ERROR_CODE CheckUnitCondition(NKMUnitData unitData)
	{
		if (m_lstUnitCondition == null)
		{
			return NKM_ERROR_CODE.NEC_OK;
		}
		if (unitData == null)
		{
			return NKM_ERROR_CODE.NEC_OK;
		}
		if (NKMUnitTempletBase.Find(unitData.m_UnitID) == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_DECK_UNIT_INVALID;
		}
		foreach (SingleCondition item in m_lstUnitCondition)
		{
			NKM_ERROR_CODE nKM_ERROR_CODE = item.IsValueOk(unitData);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				return nKM_ERROR_CODE;
			}
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public NKM_ERROR_CODE CheckOperatorCondition(NKMOperator operatorData)
	{
		if (operatorData == null)
		{
			return NKM_ERROR_CODE.NEC_OK;
		}
		if (NKMUnitManager.GetUnitTempletBase(operatorData) == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_DECK_UNIT_INVALID;
		}
		if (m_lstUnitCondition != null)
		{
			foreach (SingleCondition item in m_lstUnitCondition)
			{
				if (item.IsConditionApplyToOperator())
				{
					NKM_ERROR_CODE nKM_ERROR_CODE = item.IsValueOk(operatorData);
					if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
					{
						return nKM_ERROR_CODE;
					}
				}
			}
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public NKM_ERROR_CODE CheckEventDeckCondition(NKMArmyData armyData, NKMDungeonEventDeckTemplet eventDeckTemplet, NKMEventDeckData eventDeckData)
	{
		Dictionary<int, long> dicUnit = eventDeckData.m_dicUnit;
		new HashSet<int>();
		long shipUID = eventDeckData.m_ShipUID;
		NKMUnitData shipFromUID = armyData.GetShipFromUID(shipUID);
		NKM_ERROR_CODE nKM_ERROR_CODE = CheckEventUnitCondition(shipFromUID, eventDeckTemplet.ShipSlot);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			return nKM_ERROR_CODE;
		}
		if (NKMOpenTagManager.IsOpened("OPERATOR"))
		{
			NKMOperator operatorFromUId = armyData.GetOperatorFromUId(eventDeckData.m_OperatorUID);
			NKM_ERROR_CODE nKM_ERROR_CODE2 = CheckEventOperatorCondition(operatorFromUId, eventDeckTemplet.OperatorSlot);
			if (nKM_ERROR_CODE2 != NKM_ERROR_CODE.NEC_OK)
			{
				return nKM_ERROR_CODE2;
			}
		}
		for (int i = 0; i < 8; i++)
		{
			NKMDungeonEventDeckTemplet.EventDeckSlot unitSlot = eventDeckTemplet.GetUnitSlot(i);
			if (!dicUnit.TryGetValue(i, out var value))
			{
				value = 0L;
			}
			NKMUnitData unitFromUID = armyData.GetUnitFromUID(value);
			NKM_ERROR_CODE nKM_ERROR_CODE3 = CheckEventUnitCondition(unitFromUID, unitSlot);
			if (nKM_ERROR_CODE3 != NKM_ERROR_CODE.NEC_OK)
			{
				return nKM_ERROR_CODE3;
			}
		}
		if (m_dicAllDeckCondition != null)
		{
			foreach (AllDeckCondition value3 in m_dicAllDeckCondition.Values)
			{
				int num = 0;
				for (int j = 0; j < 8; j++)
				{
					NKMDungeonEventDeckTemplet.EventDeckSlot unitSlot2 = eventDeckTemplet.GetUnitSlot(j);
					if (!dicUnit.TryGetValue(j, out var value2))
					{
						value2 = 0L;
					}
					NKMUnitData unitFromUID2 = armyData.GetUnitFromUID(value2);
					switch (unitSlot2.m_eType)
					{
					case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FIXED:
					case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST:
					case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC:
					{
						NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitSlot2.m_ID);
						num += value3.GetAllDeckConditionValue(unitTempletBase);
						break;
					}
					case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE:
					case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE_COUNTER:
					case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE_SOLDIER:
					case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE_MECHANIC:
						if (unitFromUID2 != null)
						{
							num += value3.GetAllDeckConditionValue(unitFromUID2);
						}
						break;
					}
				}
				if (!value3.IsValueOk(num))
				{
					return value3.GetFailErrorCode();
				}
			}
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public NKM_ERROR_CODE CheckEventUnitCondition(NKMUnitData unitData, NKMDungeonEventDeckTemplet.EventDeckSlot eventDeckSlotData)
	{
		if (unitData == null)
		{
			return NKM_ERROR_CODE.NEC_OK;
		}
		if (NKMUnitTempletBase.Find(unitData.m_UnitID) == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_DECK_UNIT_INVALID;
		}
		if (m_lstUnitCondition != null)
		{
			foreach (SingleCondition item in m_lstUnitCondition)
			{
				if (!SlotConditionCheck(item.eCondition, eventDeckSlotData.m_eType))
				{
					NKM_ERROR_CODE nKM_ERROR_CODE = item.IsValueOk(unitData);
					if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
					{
						return nKM_ERROR_CODE;
					}
				}
			}
		}
		if (m_dicAllDeckCondition != null)
		{
			foreach (AllDeckCondition value in m_dicAllDeckCondition.Values)
			{
				if (!SlotConditionCheck(value.eCondition, eventDeckSlotData.m_eType))
				{
					NKM_ERROR_CODE nKM_ERROR_CODE2 = value.IsValueOk(unitData);
					if (nKM_ERROR_CODE2 != NKM_ERROR_CODE.NEC_OK)
					{
						return nKM_ERROR_CODE2;
					}
				}
			}
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public NKM_ERROR_CODE CheckEventOperatorCondition(NKMOperator operatorData, NKMDungeonEventDeckTemplet.EventDeckSlot eventDeckSlotData)
	{
		if (operatorData == null)
		{
			return NKM_ERROR_CODE.NEC_OK;
		}
		if (NKMUnitManager.GetUnitTempletBase(operatorData) == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_DECK_UNIT_INVALID;
		}
		if (m_lstUnitCondition != null)
		{
			foreach (SingleCondition item in m_lstUnitCondition)
			{
				if (item.IsConditionApplyToOperator() && !SlotConditionCheck(item.eCondition, eventDeckSlotData.m_eType))
				{
					NKM_ERROR_CODE nKM_ERROR_CODE = item.IsValueOk(operatorData);
					if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
					{
						return nKM_ERROR_CODE;
					}
				}
			}
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	private bool SlotConditionCheck(DECK_CONDITION eCondition, NKMDungeonEventDeckTemplet.SLOT_TYPE slotType)
	{
		switch (eCondition)
		{
		case DECK_CONDITION.UNIT_GRADE:
		case DECK_CONDITION.UNIT_COST:
		case DECK_CONDITION.UNIT_ROLE:
		case DECK_CONDITION.UNIT_ID_NOT:
			if ((uint)(slotType - 2) <= 2u || slotType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM)
			{
				return true;
			}
			break;
		case DECK_CONDITION.UNIT_STYLE:
		case DECK_CONDITION.SHIP_STYLE:
			if ((uint)slotType > 1u && (uint)(slotType - 2) <= 6u)
			{
				return true;
			}
			break;
		}
		return false;
	}

	private bool SlotConditionCheck(ALL_DECK_CONDITION eCondition, NKMDungeonEventDeckTemplet.SLOT_TYPE slotType)
	{
		switch (eCondition)
		{
		case ALL_DECK_CONDITION.UNIT_COST_TOTAL:
			return true;
		default:
			if ((uint)(slotType - 2) <= 2u || slotType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM)
			{
				return true;
			}
			return false;
		}
	}

	public static bool LoadFromLua(NKMLua cNKMLua, string name, out NKMDeckCondition deckCondition, string parseErrorString)
	{
		if (!cNKMLua.GetData(name, out var rValue, ""))
		{
			deckCondition = null;
			return true;
		}
		if (!ParseDeckCondition(rValue, out deckCondition))
		{
			Log.Error(parseErrorString, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDeckCondition.cs", 980);
			return false;
		}
		return true;
	}

	public static bool ParseDeckCondition(string strCondition, out NKMDeckCondition deckCondition)
	{
		deckCondition = null;
		if (string.IsNullOrEmpty(strCondition))
		{
			return true;
		}
		char[] separator = new char[4] { ',', ' ', '\t', '\n' };
		Queue<string> qTokens = new Queue<string>(strCondition.Split(separator, StringSplitOptions.RemoveEmptyEntries));
		deckCondition = new NKMDeckCondition();
		while (qTokens.Count > 0)
		{
			string data = qTokens.Dequeue();
			if (data.TryParse<DECK_CONDITION>(out var @enum, bSkipError: true))
			{
				if (!ParseSingleCondition(@enum, ref deckCondition, ref qTokens))
				{
					return false;
				}
				continue;
			}
			if (data.TryParse<ALL_DECK_CONDITION>(out var enum2, bSkipError: true))
			{
				if (!ParseAllDeckCondition(enum2, ref deckCondition, ref qTokens))
				{
					return false;
				}
				continue;
			}
			if (data.TryParse<GAME_CONDITION>(out var enum3, bSkipError: true))
			{
				if (ParseGameCondition(enum3, ref qTokens, out var gameCondition))
				{
					deckCondition.m_dicGameCondition.Add(enum3, gameCondition);
					continue;
				}
				return false;
			}
			Log.Error("Parse Error - Unexpected Token : condition token parse failed", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDeckCondition.cs", 1029);
			return false;
		}
		deckCondition.m_lstUnitCondition.Sort();
		return true;
	}

	private static bool ParseSingleCondition(DECK_CONDITION condition, ref NKMDeckCondition deckCondition, ref Queue<string> qTokens)
	{
		try
		{
			SingleCondition singleCondition = new SingleCondition();
			singleCondition.eCondition = condition;
			switch (condition)
			{
			case DECK_CONDITION.UNIT_GRADE:
				if (!ProcessDeckConditionParse<NKM_UNIT_GRADE>(qTokens, singleCondition, MORE_LESS_TYPE.ALL))
				{
					return false;
				}
				break;
			case DECK_CONDITION.UNIT_COST:
			case DECK_CONDITION.UNIT_LEVEL:
			case DECK_CONDITION.SHIP_LEVEL:
				if (!ProcessDeckConditionParse(qTokens, singleCondition, MORE_LESS_TYPE.ALL))
				{
					return false;
				}
				break;
			case DECK_CONDITION.UNIT_ROLE:
				if (!ProcessDeckConditionParse<NKM_UNIT_ROLE_TYPE>(qTokens, singleCondition, MORE_LESS_TYPE.EQUAL_OR_NOT))
				{
					return false;
				}
				break;
			case DECK_CONDITION.UNIT_STYLE:
			case DECK_CONDITION.SHIP_STYLE:
				if (!ProcessDeckConditionParse<NKM_UNIT_STYLE_TYPE>(qTokens, singleCondition, MORE_LESS_TYPE.EQUAL_OR_NOT))
				{
					return false;
				}
				break;
			case DECK_CONDITION.UNIT_ID_NOT:
				if (!ProcessDeckConditionParse(qTokens, singleCondition, MORE_LESS_TYPE.NO_USE))
				{
					return false;
				}
				singleCondition.eMoreLess = MORE_LESS.NOT;
				break;
			}
			deckCondition.m_lstUnitCondition.Add(singleCondition);
			return true;
		}
		catch (InvalidOperationException)
		{
			Log.Error("Parse Error : deck condition detail token parse failed! token count not match.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDeckCondition.cs", 1087);
			return false;
		}
	}

	private static bool ParseAllDeckCondition(ALL_DECK_CONDITION condition, ref NKMDeckCondition deckCondition, ref Queue<string> qTokens)
	{
		try
		{
			switch (condition)
			{
			case ALL_DECK_CONDITION.UNIT_COST_TOTAL:
			case ALL_DECK_CONDITION.AWAKEN_COUNT:
			case ALL_DECK_CONDITION.REARM_COUNT:
			case ALL_DECK_CONDITION.UNIT_GROUND_COUNT:
			case ALL_DECK_CONDITION.UNIT_AIR_COUNT:
			{
				AllDeckCondition allDeckCondition2 = new AllDeckCondition();
				allDeckCondition2.eCondition = condition;
				bool result2 = ProcessDeckConditionParse(qTokens, allDeckCondition2, MORE_LESS_TYPE.ALL);
				deckCondition.AddAllDeckCondition(allDeckCondition2);
				return result2;
			}
			case ALL_DECK_CONDITION.UNIT_GRADE_COUNT:
			{
				AllDeckCondition allDeckCondition = new AllDeckCondition();
				allDeckCondition.eCondition = condition;
				bool result = ProcessMultiCountDeckConditionParse<NKM_UNIT_GRADE>(qTokens, allDeckCondition, MORE_LESS_TYPE.ALL);
				deckCondition.AddAllDeckCondition(allDeckCondition);
				return result;
			}
			default:
				return false;
			}
		}
		catch (InvalidOperationException)
		{
			Log.Error("Parse Error : deck condition detail token parse failed! token count not match.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDeckCondition.cs", 1126);
			return false;
		}
	}

	private static bool ParseGameCondition(GAME_CONDITION condition, ref Queue<string> qTokens, out GameCondition gameCondition)
	{
		gameCondition = new GameCondition();
		gameCondition.eCondition = condition;
		switch (condition)
		{
		default:
		{
			if (!int.TryParse(qTokens.Dequeue(), out var result))
			{
				Log.Error("unexpected Token after " + gameCondition.eCondition, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDeckCondition.cs", 1146);
				return false;
			}
			gameCondition.Value = result;
			break;
		}
		case GAME_CONDITION.FORCE_REARM_TO_BASIC:
			break;
		}
		return true;
	}

	private static bool ProcessDeckConditionParse<T>(Queue<string> qTokens, EventDeckCondition deckCondition, MORE_LESS_TYPE mlType) where T : Enum
	{
		if (mlType != MORE_LESS_TYPE.NO_USE)
		{
			if (!qTokens.Dequeue().TryParse<MORE_LESS>(out var @enum))
			{
				Log.Error("unexpected Token after " + deckCondition.ConditionName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDeckCondition.cs", 1176);
				return false;
			}
			switch (mlType)
			{
			case MORE_LESS_TYPE.EQUAL_OR_NOT:
			case MORE_LESS_TYPE.BOOL:
				if (@enum != MORE_LESS.EQUAL && @enum != MORE_LESS.NOT)
				{
					Log.Error("Equal More or Less is not permitted for " + deckCondition.ConditionName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDeckCondition.cs", 1187);
					return false;
				}
				break;
			case MORE_LESS_TYPE.NO_NOT:
				if (@enum == MORE_LESS.NOT)
				{
					Log.Error("Equal More or Less is not permitted for " + deckCondition.ConditionName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDeckCondition.cs", 1197);
					return false;
				}
				break;
			}
			deckCondition.eMoreLess = @enum;
		}
		else
		{
			deckCondition.eMoreLess = MORE_LESS.EQUAL;
		}
		if (mlType == MORE_LESS_TYPE.BOOL)
		{
			return true;
		}
		if (!qTokens.Dequeue().TryParse<T>(out var enum2))
		{
			Log.Error("unexpected Token after " + deckCondition.ConditionName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDeckCondition.cs", 1220);
			return false;
		}
		deckCondition.Value = Convert.ToInt32(enum2);
		if (deckCondition.eMoreLess == MORE_LESS.EQUAL || deckCondition.eMoreLess == MORE_LESS.NOT)
		{
			T enum3;
			while (qTokens.Count > 0 && qTokens.Peek().TryParse<T>(out enum3, bSkipError: true))
			{
				if (deckCondition.lstValue == null)
				{
					deckCondition.lstValue = new List<int>();
					deckCondition.lstValue.Add(deckCondition.Value);
				}
				deckCondition.lstValue.Add(Convert.ToInt32(enum3));
				qTokens.Dequeue();
			}
		}
		return true;
	}

	private static bool ProcessDeckConditionParse(Queue<string> qTokens, EventDeckCondition deckCondition, MORE_LESS_TYPE mlType)
	{
		if (mlType != MORE_LESS_TYPE.NO_USE)
		{
			if (!qTokens.Dequeue().TryParse<MORE_LESS>(out var @enum))
			{
				Log.Error("unexpected Token after " + deckCondition.ConditionName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDeckCondition.cs", 1259);
				return false;
			}
			if ((mlType == MORE_LESS_TYPE.EQUAL_OR_NOT || mlType == MORE_LESS_TYPE.BOOL) && @enum != MORE_LESS.EQUAL && @enum != MORE_LESS.NOT)
			{
				Log.Error("Equal More or Less is not permitted for " + deckCondition.ConditionName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDeckCondition.cs", 1267);
				return false;
			}
			deckCondition.eMoreLess = @enum;
		}
		else
		{
			deckCondition.eMoreLess = MORE_LESS.EQUAL;
		}
		if (mlType == MORE_LESS_TYPE.BOOL)
		{
			return true;
		}
		if (!int.TryParse(qTokens.Dequeue(), out var result))
		{
			Log.Error("unexpected Token after " + deckCondition.ConditionName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDeckCondition.cs", 1285);
			return false;
		}
		deckCondition.Value = result;
		if (deckCondition.eMoreLess == MORE_LESS.EQUAL || deckCondition.eMoreLess == MORE_LESS.NOT)
		{
			int result2;
			while (qTokens.Count > 0 && int.TryParse(qTokens.Peek(), out result2))
			{
				if (deckCondition.lstValue == null)
				{
					deckCondition.lstValue = new List<int>();
					deckCondition.lstValue.Add(deckCondition.Value);
				}
				deckCondition.lstValue.Add(result2);
				qTokens.Dequeue();
			}
		}
		return true;
	}

	private static bool ProcessMultiCountDeckConditionParse<T>(Queue<string> qTokens, AllDeckCondition deckCondition, MORE_LESS_TYPE mlType) where T : Enum
	{
		if (mlType == MORE_LESS_TYPE.NO_USE)
		{
			Log.Error("NO_USE type cannot used for condition " + deckCondition.eCondition, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDeckCondition.cs", 1320);
			return false;
		}
		deckCondition.lstValue = new List<int>();
		while (qTokens.Count > 0)
		{
			string data = qTokens.Dequeue();
			if (data.TryParse<MORE_LESS>(out var @enum, bSkipError: true))
			{
				deckCondition.eMoreLess = @enum;
				break;
			}
			if (!data.TryParse<T>(out var enum2))
			{
				return false;
			}
			deckCondition.lstValue.Add(Convert.ToInt32(enum2));
		}
		if (!int.TryParse(qTokens.Dequeue(), out var result))
		{
			Log.Error("unexpected Token after " + deckCondition.eCondition, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDeckCondition.cs", 1350);
			return false;
		}
		deckCondition.Value = result;
		return true;
	}

	public IEnumerable<EventDeckCondition> AllConditionEnumerator()
	{
		if (m_lstUnitCondition != null)
		{
			foreach (SingleCondition item in m_lstUnitCondition)
			{
				yield return item;
			}
		}
		if (m_dicAllDeckCondition == null)
		{
			yield break;
		}
		foreach (AllDeckCondition value in m_dicAllDeckCondition.Values)
		{
			yield return value;
		}
	}
}
