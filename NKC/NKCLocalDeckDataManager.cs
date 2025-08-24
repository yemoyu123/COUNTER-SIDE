using System.Collections.Generic;
using Cs.Logging;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public static class NKCLocalDeckDataManager
{
	public const int NoLeaderIndex = -1;

	private static Dictionary<int, NKMEventDeckData> m_localDeckData = new Dictionary<int, NKMEventDeckData>();

	private static Dictionary<int, string> m_deckKey = new Dictionary<int, string>();

	private static int m_unitSlotCount;

	private static bool m_dataLoaded = false;

	private static NKMDungeonEventDeckTemplet m_EmptyEventDeckTemplet;

	public static NKMDungeonEventDeckTemplet FreeEventDeckTemplet
	{
		get
		{
			if (m_EmptyEventDeckTemplet != null)
			{
				return m_EmptyEventDeckTemplet;
			}
			m_EmptyEventDeckTemplet = new NKMDungeonEventDeckTemplet();
			NKMDungeonEventDeckTemplet.EventDeckSlot eventDeckSlot = new NKMDungeonEventDeckTemplet.EventDeckSlot
			{
				m_eType = NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE
			};
			m_EmptyEventDeckTemplet.ShipSlot = eventDeckSlot;
			m_EmptyEventDeckTemplet.OperatorSlot = eventDeckSlot;
			for (int i = 0; i < m_EmptyEventDeckTemplet.m_lstUnitSlot.Count; i++)
			{
				m_EmptyEventDeckTemplet.m_lstUnitSlot[i] = eventDeckSlot;
			}
			return m_EmptyEventDeckTemplet;
		}
	}

	public static bool DataLoaded => m_dataLoaded;

	public static void SetDataLoadedState(bool value)
	{
		m_dataLoaded = value;
	}

	public static void Clear()
	{
		m_localDeckData.Clear();
		m_deckKey.Clear();
		m_unitSlotCount = 0;
		m_dataLoaded = false;
	}

	public static void LoadLocalDeckData(string localDeckKey, int deckIndex, int unitSlotCount)
	{
		if (m_dataLoaded)
		{
			return;
		}
		if (!m_deckKey.ContainsKey(deckIndex))
		{
			m_deckKey.Add(deckIndex, localDeckKey);
			m_unitSlotCount = unitSlotCount;
			string text = PlayerPrefs.GetString(localDeckKey);
			NKMEventDeckData nKMEventDeckData = new NKMEventDeckData();
			if (!string.IsNullOrEmpty(text))
			{
				nKMEventDeckData.FromBase64(text);
				ValidateDeck(nKMEventDeckData);
			}
			else
			{
				for (int i = 0; i < unitSlotCount; i++)
				{
					nKMEventDeckData.m_dicUnit.Add(i, 0L);
				}
			}
			if (m_localDeckData.ContainsKey(deckIndex))
			{
				m_localDeckData[deckIndex] = nKMEventDeckData;
			}
			else
			{
				m_localDeckData.Add(deckIndex, nKMEventDeckData);
			}
		}
		else
		{
			Log.Error("Load Error: \ufffd\ufffd \ufffdε\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdߺ\ufffd \ufffdԷµǾ\ufffd\ufffd\ufffd\ufffdϴ\ufffd.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCLocalDeckDataManager.cs", 70);
		}
	}

	public static void SaveLocalDeck()
	{
		foreach (KeyValuePair<int, NKMEventDeckData> localDeckDatum in m_localDeckData)
		{
			if (m_deckKey.ContainsKey(localDeckDatum.Key))
			{
				string value = localDeckDatum.Value.ToBase64();
				PlayerPrefs.SetString(m_deckKey[localDeckDatum.Key], value);
			}
		}
	}

	public static void SetLocalUnitUId(int deckIndex, int slotIndex, long unitUId, bool prohibitSameUnitId)
	{
		if (!m_localDeckData.ContainsKey(deckIndex))
		{
			return;
		}
		if (m_localDeckData[deckIndex].m_dicUnit.ContainsKey(slotIndex))
		{
			m_localDeckData[deckIndex].m_dicUnit[slotIndex] = unitUId;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		NKMUnitData nKMUnitData = nKMUserData?.m_ArmyData.GetUnitFromUID(unitUId);
		if (nKMUnitData == null)
		{
			return;
		}
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, NKMEventDeckData> kvPair in m_localDeckData)
		{
			bool flag = kvPair.Key == deckIndex;
			foreach (KeyValuePair<int, long> item in kvPair.Value.m_dicUnit)
			{
				if (flag && item.Key == slotIndex)
				{
					continue;
				}
				long value = item.Value;
				if (value <= 0)
				{
					continue;
				}
				if (value == unitUId)
				{
					list.Add(item.Key);
				}
				else if (prohibitSameUnitId)
				{
					NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(nKMUserData?.m_ArmyData.GetUnitFromUID(value));
					if (unitTempletBase != null && unitTempletBase.IsSameBaseUnit(nKMUnitData.m_UnitID))
					{
						list.Add(item.Key);
					}
				}
			}
			list.ForEach(delegate(int e)
			{
				kvPair.Value.m_dicUnit[e] = 0L;
			});
			list.Clear();
		}
	}

	public static List<bool> SetLocalAutoDeckUnitUId(int deckIndex, List<long> unitList)
	{
		List<bool> list = new List<bool>();
		int count = unitList.Count;
		for (int i = 0; i < count; i++)
		{
			list.Add(item: false);
		}
		if (!m_localDeckData.ContainsKey(deckIndex) || unitList == null)
		{
			return list;
		}
		if (m_localDeckData[deckIndex] == null)
		{
			return list;
		}
		for (int j = 0; j < count; j++)
		{
			if (m_localDeckData[deckIndex].m_dicUnit.ContainsKey(j))
			{
				list[j] = m_localDeckData[deckIndex].m_dicUnit[j] != unitList[j];
				m_localDeckData[deckIndex].m_dicUnit[j] = unitList[j];
			}
		}
		return list;
	}

	public static bool IsNextLocalSlotEmpty(int deckIndex, int currentIndex)
	{
		if (!m_localDeckData.ContainsKey(deckIndex))
		{
			return false;
		}
		if (!m_localDeckData[deckIndex].m_dicUnit.ContainsKey(currentIndex + 1))
		{
			return false;
		}
		if (m_localDeckData[deckIndex].m_dicUnit[currentIndex + 1] != 0L)
		{
			return false;
		}
		return true;
	}

	public static List<long> GetLocalUnitDeckData(int deckIndex)
	{
		List<long> list = new List<long>();
		if (m_localDeckData.ContainsKey(deckIndex))
		{
			foreach (KeyValuePair<int, long> item in m_localDeckData[deckIndex].m_dicUnit)
			{
				list.Add(item.Value);
			}
			return list;
		}
		for (int i = 0; i < m_unitSlotCount; i++)
		{
			list.Add(0L);
		}
		return list;
	}

	public static List<NKMUnitData> GetLocalUnitDeckUnitData(int deckIndex)
	{
		List<NKMUnitData> list = new List<NKMUnitData>();
		List<long> localUnitDeckData = GetLocalUnitDeckData(deckIndex);
		int count = localUnitDeckData.Count;
		for (int i = 0; i < count; i++)
		{
			NKMUnitData item = NKCScenManager.CurrentUserData()?.m_ArmyData.GetUnitFromUID(localUnitDeckData[i]);
			list.Add(item);
		}
		return list;
	}

	public static long GetLocalUnitData(int deckIndex, int slotIndex)
	{
		GetLocalUnitDeckData(deckIndex);
		if (slotIndex >= 0 && m_localDeckData[deckIndex].m_dicUnit.ContainsKey(slotIndex))
		{
			return m_localDeckData[deckIndex].m_dicUnit[slotIndex];
		}
		return 0L;
	}

	public static Dictionary<int, NKMEventDeckData> GetAllLocalDeckData()
	{
		return m_localDeckData;
	}

	public static NKMEventDeckData GetLocalDeckData(int index)
	{
		if (m_localDeckData.TryGetValue(index, out var value))
		{
			return value;
		}
		return null;
	}

	public static int SetLeaderIndex(int deckIndex, int index)
	{
		int num = -1;
		if (index >= 0 && m_localDeckData.ContainsKey(deckIndex) && m_localDeckData[deckIndex].m_dicUnit.ContainsKey(index))
		{
			if (m_localDeckData[deckIndex].m_dicUnit[index] > 0)
			{
				num = index;
			}
			else
			{
				foreach (KeyValuePair<int, long> item in m_localDeckData[deckIndex].m_dicUnit)
				{
					if (item.Value > 0)
					{
						num = item.Key;
						break;
					}
				}
			}
		}
		if (m_localDeckData.ContainsKey(deckIndex))
		{
			m_localDeckData[deckIndex].m_LeaderIndex = num;
		}
		return num;
	}

	public static int GetLocalLeaderIndex(int deckIndex)
	{
		if (m_localDeckData.ContainsKey(deckIndex))
		{
			return m_localDeckData[deckIndex].m_LeaderIndex;
		}
		return -1;
	}

	public static long GetLocalLeaderUnitUId(int deckIndex)
	{
		int localLeaderIndex = GetLocalLeaderIndex(deckIndex);
		return GetLocalUnitData(deckIndex, localLeaderIndex);
	}

	public static void SwapSlotData(int deckIndex, int firstIndex, int secondIndex)
	{
		if (firstIndex >= 0 && secondIndex >= 0 && deckIndex >= 0 && m_localDeckData.Count > deckIndex && m_localDeckData[deckIndex].m_dicUnit.Count > firstIndex && m_localDeckData[deckIndex].m_dicUnit.Count > secondIndex)
		{
			NKMEventDeckData nKMEventDeckData = m_localDeckData[deckIndex];
			long value = nKMEventDeckData.m_dicUnit[firstIndex];
			nKMEventDeckData.m_dicUnit[firstIndex] = nKMEventDeckData.m_dicUnit[secondIndex];
			nKMEventDeckData.m_dicUnit[secondIndex] = value;
			if (nKMEventDeckData.m_LeaderIndex == firstIndex)
			{
				nKMEventDeckData.m_LeaderIndex = secondIndex;
			}
			else if (nKMEventDeckData.m_LeaderIndex == secondIndex)
			{
				nKMEventDeckData.m_LeaderIndex = firstIndex;
			}
		}
	}

	public static void SetLocalShipUId(int deckIndex, long shipUId, bool prohibitSameUnitId)
	{
		if (!m_localDeckData.ContainsKey(deckIndex))
		{
			return;
		}
		m_localDeckData[deckIndex].m_ShipUID = shipUId;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		NKMUnitData nKMUnitData = nKMUserData?.m_ArmyData.GetShipFromUID(shipUId);
		if (nKMUnitData == null)
		{
			return;
		}
		foreach (KeyValuePair<int, NKMEventDeckData> localDeckDatum in m_localDeckData)
		{
			if (localDeckDatum.Key == deckIndex)
			{
				continue;
			}
			long shipUID = localDeckDatum.Value.m_ShipUID;
			if (shipUID <= 0)
			{
				continue;
			}
			if (shipUID == shipUId)
			{
				localDeckDatum.Value.m_ShipUID = 0L;
			}
			else if (prohibitSameUnitId)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(nKMUserData?.m_ArmyData.GetShipFromUID(shipUID));
				NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(nKMUnitData);
				if (unitTempletBase != null && unitTempletBase2 != null && unitTempletBase.m_ShipGroupID == unitTempletBase2.m_ShipGroupID)
				{
					localDeckDatum.Value.m_ShipUID = 0L;
				}
			}
		}
	}

	public static void SetLocalAutoDeckShipUId(int deckIndex, long shipUId)
	{
		if (m_localDeckData.ContainsKey(deckIndex))
		{
			m_localDeckData[deckIndex].m_ShipUID = shipUId;
		}
	}

	public static long GetShipUId(int deckIndex)
	{
		if (m_localDeckData.ContainsKey(deckIndex))
		{
			return m_localDeckData[deckIndex].m_ShipUID;
		}
		return 0L;
	}

	public static void SetLocalOperatorUId(int deckIndex, long operatorUId, bool prohibitSameUnitId)
	{
		if (!m_localDeckData.ContainsKey(deckIndex))
		{
			return;
		}
		m_localDeckData[deckIndex].m_OperatorUID = operatorUId;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		NKMOperator nKMOperator = nKMUserData?.m_ArmyData.GetOperatorFromUId(operatorUId);
		if (nKMOperator == null)
		{
			return;
		}
		foreach (KeyValuePair<int, NKMEventDeckData> localDeckDatum in m_localDeckData)
		{
			if (localDeckDatum.Key == deckIndex)
			{
				continue;
			}
			long operatorUID = localDeckDatum.Value.m_OperatorUID;
			if (operatorUID <= 0)
			{
				continue;
			}
			if (operatorUID == operatorUId)
			{
				localDeckDatum.Value.m_OperatorUID = 0L;
			}
			else if (prohibitSameUnitId)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(nKMUserData?.m_ArmyData.GetOperatorFromUId(operatorUID));
				if (unitTempletBase != null && unitTempletBase.IsSameBaseUnit(nKMOperator.id))
				{
					localDeckDatum.Value.m_OperatorUID = 0L;
				}
			}
		}
	}

	public static void SetLocalAutoDeckOperatorUId(int deckIndex, long uid)
	{
		if (m_localDeckData.ContainsKey(deckIndex))
		{
			m_localDeckData[deckIndex].m_OperatorUID = uid;
		}
	}

	public static long GetOperatorUId(int deckIndex)
	{
		if (m_localDeckData.ContainsKey(deckIndex))
		{
			return m_localDeckData[deckIndex].m_OperatorUID;
		}
		return 0L;
	}

	public static List<bool> ClearDeck(int deckIndex)
	{
		List<bool> list = new List<bool>();
		for (int i = 0; i < m_unitSlotCount; i++)
		{
			list.Add(item: false);
		}
		if (m_localDeckData.ContainsKey(deckIndex))
		{
			List<int> list2 = new List<int>();
			foreach (KeyValuePair<int, long> item in m_localDeckData[deckIndex].m_dicUnit)
			{
				if (item.Key < m_unitSlotCount)
				{
					list[item.Key] = item.Value > 0;
				}
				list2.Add(item.Key);
			}
			list2.ForEach(delegate(int e)
			{
				m_localDeckData[deckIndex].m_dicUnit[e] = 0L;
			});
			m_localDeckData[deckIndex].m_ShipUID = 0L;
			m_localDeckData[deckIndex].m_OperatorUID = 0L;
		}
		return list;
	}

	public static bool IsSameUnitIdInUnitDeckSlot(int deckIndex, int slotIndex, NKMUnitTempletBase unitTempletBase)
	{
		if (!m_localDeckData.ContainsKey(deckIndex) || m_localDeckData[deckIndex] == null || unitTempletBase == null)
		{
			return false;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return false;
		}
		if (m_localDeckData[deckIndex].m_dicUnit.ContainsKey(slotIndex))
		{
			long unitUid = m_localDeckData[deckIndex].m_dicUnit[slotIndex];
			NKMUnitData unitFromUID = nKMUserData.m_ArmyData.GetUnitFromUID(unitUid);
			if (unitFromUID == null)
			{
				return false;
			}
			if (unitTempletBase.IsSameBaseUnit(unitFromUID.m_UnitID))
			{
				return true;
			}
		}
		return false;
	}

	public static HashSet<int> GetAllUnitDeckUnitIdList()
	{
		HashSet<int> hashSet = new HashSet<int>();
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return hashSet;
		}
		foreach (KeyValuePair<int, NKMEventDeckData> allLocalDeckDatum in GetAllLocalDeckData())
		{
			_ = allLocalDeckDatum.Value.m_dicUnit.Count;
			foreach (KeyValuePair<int, long> item in allLocalDeckDatum.Value.m_dicUnit)
			{
				long value = item.Value;
				if (value != 0L)
				{
					NKMUnitData unitFromUID = nKMUserData.m_ArmyData.GetUnitFromUID(value);
					if (unitFromUID != null)
					{
						hashSet.Add(unitFromUID.m_UnitID);
					}
				}
			}
		}
		return hashSet;
	}

	public static int GetOperationPower(int deckIndex, bool bPVP, bool bPossibleShowBan, bool bPossibleShowUp)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return 0;
		}
		long shipUId = GetShipUId(deckIndex);
		NKMUnitData shipFromUID = nKMUserData.m_ArmyData.GetShipFromUID(shipUId);
		List<NKMUnitData> localUnitDeckUnitData = GetLocalUnitDeckUnitData(deckIndex);
		int operatorPower = 0;
		long operatorUId = GetOperatorUId(deckIndex);
		if (operatorUId != 0L)
		{
			NKMOperator operatorFromUId = nKMUserData.m_ArmyData.GetOperatorFromUId(operatorUId);
			if (operatorFromUId != null)
			{
				operatorPower = operatorFromUId.CalculateOperatorOperationPower();
			}
		}
		return NKMOperationPower.Calculate(leaderUnitUID: GetLocalLeaderUnitUId(deckIndex), dicNKMBanData: bPossibleShowBan ? NKCBanManager.GetBanData() : null, dicNKMUpData: bPossibleShowUp ? NKCBanManager.m_dicNKMUpData : null, ship: shipFromUID, units: localUnitDeckUnitData, invenData: nKMUserData.m_InventoryData, bPVP: bPVP, operatorPower: operatorPower);
	}

	private static bool IsValideUnit(NKM_UNIT_TYPE unitType, long uid)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		switch (unitType)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
			return nKMUserData.m_ArmyData.GetUnitFromUID(uid) != null;
		case NKM_UNIT_TYPE.NUT_SHIP:
			return nKMUserData.m_ArmyData.GetShipFromUID(uid) != null;
		case NKM_UNIT_TYPE.NUT_OPERATOR:
			return nKMUserData.m_ArmyData.GetOperatorFromUId(uid) != null;
		default:
			Log.Error("Invalid Unit Type", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCLocalDeckDataManager.cs", 560);
			return false;
		}
	}

	private static void ValidateDeck(NKMEventDeckData eventDeckData)
	{
		ValidateUnit(eventDeckData);
		ValidateShip(eventDeckData);
		ValidateOperator(eventDeckData);
	}

	private static void ValidateUnit(NKMEventDeckData eventDeckData)
	{
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, long> item in eventDeckData.m_dicUnit)
		{
			if (!IsValideUnit(NKM_UNIT_TYPE.NUT_NORMAL, item.Value))
			{
				list.Add(item.Key);
			}
		}
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			if (eventDeckData.m_dicUnit.ContainsKey(list[i]))
			{
				eventDeckData.m_dicUnit[list[i]] = 0L;
			}
		}
		ValidateLeaderIndex(eventDeckData);
	}

	private static void ValidateShip(NKMEventDeckData eventDeckData)
	{
		if (!IsValideUnit(NKM_UNIT_TYPE.NUT_SHIP, eventDeckData.m_ShipUID))
		{
			eventDeckData.m_ShipUID = 0L;
		}
	}

	private static void ValidateOperator(NKMEventDeckData eventDeckData)
	{
		if (!IsValideUnit(NKM_UNIT_TYPE.NUT_OPERATOR, eventDeckData.m_OperatorUID))
		{
			eventDeckData.m_OperatorUID = 0L;
		}
	}

	private static void ValidateLeaderIndex(NKMEventDeckData eventDeckData)
	{
		int leaderIndex = -1;
		int leaderIndex2 = eventDeckData.m_LeaderIndex;
		if (leaderIndex2 >= 0 && eventDeckData.m_dicUnit.ContainsKey(leaderIndex2))
		{
			if (eventDeckData.m_dicUnit[leaderIndex2] > 0)
			{
				return;
			}
			foreach (KeyValuePair<int, long> item in eventDeckData.m_dicUnit)
			{
				if (item.Value > 0)
				{
					leaderIndex = item.Key;
					break;
				}
			}
		}
		eventDeckData.m_LeaderIndex = leaderIndex;
	}
}
