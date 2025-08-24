using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public sealed class NKMBattleConditionManager
{
	private static Dictionary<int, NKMBattleConditionTemplet> m_dicBattleConditionTemplet;

	private static Dictionary<string, NKMBattleConditionTemplet> m_dicBattleConditionTempletByStrID;

	private static Dictionary<int, List<NKMPreconditionBCGroupTemplet>> m_dicPreconditionTemplet;

	public static Dictionary<int, NKMBattleConditionTemplet> Dic => m_dicBattleConditionTemplet;

	public static bool LoadFromLua()
	{
		bool flag = false;
		m_dicBattleConditionTemplet = NKMTempletLoader.LoadDictionary("AB_SCRIPT", "LUA_BATTLE_CONDITION_TEMPLET", "m_dicNKMBcondTemplet", NKMBattleConditionTemplet.LoadFromLUA);
		flag = m_dicBattleConditionTemplet != null;
		if (flag)
		{
			m_dicBattleConditionTempletByStrID = m_dicBattleConditionTemplet.ToDictionary((KeyValuePair<int, NKMBattleConditionTemplet> t) => t.Value.BattleCondStrID, (KeyValuePair<int, NKMBattleConditionTemplet> t) => t.Value);
		}
		m_dicPreconditionTemplet = NKMTempletLoader<NKMPreconditionBCGroupTemplet>.LoadGroup("AB_SCRIPT", "LUA_PRECONDITION_TEMPLET", "PRECONDITION_TEMPLET", NKMPreconditionBCGroupTemplet.LoadFromLua);
		foreach (KeyValuePair<int, List<NKMPreconditionBCGroupTemplet>> item in m_dicPreconditionTemplet)
		{
			item.Value.Sort((NKMPreconditionBCGroupTemplet a, NKMPreconditionBCGroupTemplet b) => a.m_BCondID.CompareTo(b.m_BCondID));
		}
		return flag;
	}

	public static void Join()
	{
		foreach (List<NKMPreconditionBCGroupTemplet> value in m_dicPreconditionTemplet.Values)
		{
			foreach (NKMPreconditionBCGroupTemplet item in value)
			{
				item.Join();
			}
		}
	}

	public static void Validate()
	{
		foreach (List<NKMPreconditionBCGroupTemplet> value in m_dicPreconditionTemplet.Values)
		{
			foreach (NKMPreconditionBCGroupTemplet item in value)
			{
				item.Validate();
			}
		}
		foreach (NKMBattleConditionTemplet value2 in m_dicBattleConditionTemplet.Values)
		{
			value2.Validate();
		}
	}

	public static NKMBattleConditionTemplet GetTempletByID(int bCondID)
	{
		NKMBattleConditionTemplet value = null;
		if (!m_dicBattleConditionTemplet.TryGetValue(bCondID, out value))
		{
			return null;
		}
		return value;
	}

	public static NKMBattleConditionTemplet GetTempletByStrID(string bCondStrID)
	{
		m_dicBattleConditionTempletByStrID.TryGetValue(bCondStrID, out var value);
		return value;
	}

	public static int GetBattleConditionIDByStrID(string id)
	{
		if (!m_dicBattleConditionTempletByStrID.TryGetValue(id, out var value))
		{
			return 0;
		}
		return value.BattleCondID;
	}

	public static List<string> GetBattleConditionStrIDList()
	{
		List<string> list = new List<string>();
		Dictionary<string, NKMBattleConditionTemplet>.Enumerator enumerator = m_dicBattleConditionTempletByStrID.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMBattleConditionTemplet value = enumerator.Current.Value;
			list.Add(value.BattleCondStrID);
		}
		return list;
	}

	public static List<NKMPreconditionBCGroupTemplet> GetPreConditionTypeByGroupId(int id)
	{
		m_dicPreconditionTemplet.TryGetValue(id, out var value);
		return value;
	}

	public static string GetBattleConditionStrIdById(int id)
	{
		if (!m_dicBattleConditionTemplet.TryGetValue(id, out var value))
		{
			return null;
		}
		return value.BattleCondStrID;
	}

	public static List<NKMBattleConditionTemplet> GetEnabledPreBCList(List<int> lstPreConditionGroupID, NKMArmyData armyData, NKMInventoryData invenData, NKMDeckData deckData)
	{
		List<GameUnitData> lstUnit = NKMDungeonManager.MakeDeckUnitDataList(armyData, deckData, invenData);
		NKMUnitData shipFromUID = armyData.GetShipFromUID(deckData.m_ShipUID);
		NKMOperator operatorFromUId = armyData.GetOperatorFromUId(deckData.m_OperatorUID);
		List<NKMBattleConditionTemplet> list = new List<NKMBattleConditionTemplet>();
		foreach (int item in lstPreConditionGroupID)
		{
			NKMBattleConditionTemplet enabledBCByPreCondition = GetEnabledBCByPreCondition(item, invenData, lstUnit, shipFromUID, operatorFromUId, deckData.GetLeaderUnitUID());
			if (enabledBCByPreCondition != null)
			{
				list.Add(enabledBCByPreCondition);
			}
		}
		return list;
	}

	public static List<NKMBattleConditionTemplet> GetEnabledPreBCList(List<int> lstPreConditionGroupID, NKMArmyData armyData, NKMInventoryData invenData, NKMEventDeckData deckData, NKMDungeonEventDeckTemplet eventDeckTemplet)
	{
		List<GameUnitData> list = NKMDungeonManager.MakeEventDeckUnitDataList(armyData, eventDeckTemplet, null, deckData, invenData, NKM_TEAM_TYPE.NTT_A1, bSkipRandomSlot: true);
		NKMUnitData shipData = NKMDungeonManager.MakeEventDeckUnit(armyData, eventDeckTemplet, null, eventDeckTemplet.ShipSlot, deckData.m_ShipUID, NKM_UNIT_TYPE.NUT_SHIP, NKM_TEAM_TYPE.NTT_A1, null, bSkipRandomSlot: true);
		NKMOperator operatorData = NKMDungeonManager.MakeEventDeckOperatorData(armyData, eventDeckTemplet, null, deckData, NKM_TEAM_TYPE.NTT_A1, bSkipRandomSlot: true);
		List<NKMBattleConditionTemplet> list2 = new List<NKMBattleConditionTemplet>();
		foreach (int item in lstPreConditionGroupID)
		{
			NKMBattleConditionTemplet enabledBCByPreCondition = GetEnabledBCByPreCondition(item, invenData, list, shipData, operatorData, deckData.GetLeaderUID(list, eventDeckTemplet));
			if (enabledBCByPreCondition != null)
			{
				list2.Add(enabledBCByPreCondition);
			}
		}
		return list2;
	}

	public static int GetPreconditionCurrentValue(NKMPreconditionBCGroupTemplet templet, NKMInventoryData invenData, List<GameUnitData> lstUnit, NKMUnitData shipData, NKMOperator operatorData, long leaderUID)
	{
		if (templet == null)
		{
			return 0;
		}
		switch (templet.m_PreConditionType)
		{
		case NKMPreconditionBCGroupTemplet.PRE_COND.UNIT_ID:
		{
			if (lstUnit == null)
			{
				return 0;
			}
			HashSet<int> hsUnitId = new HashSet<int>(lstUnit.Select((GameUnitData gameUnit) => gameUnit.unit.m_UnitID));
			return templet.PreConditionList.Count((int unitID) => NKMUnitManager.CheckContainsBaseUnit(hsUnitId, unitID));
		}
		case NKMPreconditionBCGroupTemplet.PRE_COND.UNITLEVEL_COUNT:
		{
			if (lstUnit == null)
			{
				return 0;
			}
			if (templet.PreConditionList == null || templet.PreConditionList.Count == 0)
			{
				return 0;
			}
			int targetLevel = templet.PreConditionList[0];
			return lstUnit.Count((GameUnitData e) => e.unit.m_UnitLevel >= targetLevel);
		}
		default:
			return GetPreconditionCurrentValue(templet.m_PreConditionType, invenData, lstUnit, shipData, operatorData, leaderUID);
		}
	}

	private static int GetPreconditionCurrentValue(NKMPreconditionBCGroupTemplet.PRE_COND type, NKMInventoryData invenData, List<GameUnitData> lstUnit, NKMUnitData shipData, NKMOperator operatorData, long leaderUID)
	{
		switch (type)
		{
		case NKMPreconditionBCGroupTemplet.PRE_COND.COMBATPOINT:
		{
			if (lstUnit == null)
			{
				return 0;
			}
			IEnumerable<NKMUnitData> units = lstUnit.Select((GameUnitData x) => x.unit);
			int operatorPower = operatorData?.CalculateOperatorOperationPower() ?? 0;
			return NKMOperationPower.Calculate(shipData, units, leaderUID, invenData, bPVP: false, null, null, operatorPower);
		}
		case NKMPreconditionBCGroupTemplet.PRE_COND.UNITLEVEL110_COUNT:
			return lstUnit?.Count((GameUnitData e) => e.unit.m_UnitLevel >= 110) ?? 0;
		case NKMPreconditionBCGroupTemplet.PRE_COND.UNITLEVEL_COUNT:
		case NKMPreconditionBCGroupTemplet.PRE_COND.UNIT_ID:
			Log.Warn($"[NKMBattleConditionManager]NKMPreconditionBCGroupTemplet logic error. {type} should not called with PRE_COND type only", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMBattleConditionManager.cs", 518);
			return -1;
		default:
			return -1;
		}
	}

	public static bool CheckPrecondition(NKMPreconditionBCGroupTemplet templet, NKMInventoryData invenData, List<GameUnitData> lstUnit, NKMUnitData shipData, NKMOperator operatorData, long leaderUID)
	{
		if (templet == null)
		{
			return false;
		}
		int preconditionCurrentValue = GetPreconditionCurrentValue(templet, invenData, lstUnit, shipData, operatorData, leaderUID);
		return templet.PreConditionValue <= preconditionCurrentValue;
	}

	public static NKMBattleConditionTemplet GetEnabledBCByPreCondition(int groupId, NKMInventoryData invenData, List<GameUnitData> lstUnit, NKMUnitData shipData, NKMOperator operatorData, long leaderUID)
	{
		if (!m_dicPreconditionTemplet.ContainsKey(groupId))
		{
			return null;
		}
		List<NKMPreconditionBCGroupTemplet> list = m_dicPreconditionTemplet[groupId];
		NKMPreconditionBCGroupTemplet.PRE_COND preConditionType = list[0].m_PreConditionType;
		NKMBattleConditionTemplet nKMBattleConditionTemplet = null;
		if ((uint)(preConditionType - 3) <= 1u)
		{
			if (lstUnit == null)
			{
				return null;
			}
			new HashSet<int>(lstUnit.Select((GameUnitData gameUnit) => gameUnit.unit.m_UnitID));
			return (from e in list
				where CheckPrecondition(e, invenData, lstUnit, shipData, operatorData, leaderUID)
				orderby e.m_BCondID descending
				select e).FirstOrDefault()?.BattleConditionTemplet;
		}
		int currentValue = GetPreconditionCurrentValue(preConditionType, invenData, lstUnit, shipData, operatorData, leaderUID);
		return (from e in list
			where e.PreConditionValue <= currentValue
			orderby e.m_BCondID descending
			select e).FirstOrDefault()?.BattleConditionTemplet;
	}
}
