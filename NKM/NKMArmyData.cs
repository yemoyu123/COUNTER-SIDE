using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ClientPacket.Common;
using Cs.Core.Util;
using Cs.Logging;
using Cs.Protocol;
using NKC;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

[DataContract]
public class NKMArmyData : Cs.Protocol.ISerializable
{
	public enum UNIT_SEARCH_OPTION
	{
		None,
		Level,
		LimitLevel,
		Devotion,
		StarGrade
	}

	public delegate void OnUnitUpdate(NKMUserData.eChangeNotifyType eEventType, NKM_UNIT_TYPE eUnitType, long uid, NKMUnitData unitData);

	public delegate void OnDeckUpdate(NKMDeckIndex deckIndex, NKMDeckData deckData);

	public delegate void OnOperatorUpdate(NKMUserData.eChangeNotifyType eEventType, long uid, NKMOperator operatorData);

	private class DeckIndexWithAvgOperationPower : IComparable<DeckIndexWithAvgOperationPower>
	{
		public byte m_Index;

		public int m_AvgOperationPower;

		public int CompareTo(DeckIndexWithAvgOperationPower other)
		{
			if (m_AvgOperationPower > other.m_AvgOperationPower)
			{
				return -1;
			}
			if (other.m_AvgOperationPower > m_AvgOperationPower)
			{
				return 1;
			}
			return 0;
		}
	}

	[DataMember]
	private NKMDeckSet[] deckSets = new NKMDeckSet[EnumUtil<NKM_DECK_TYPE>.Count];

	[DataMember]
	private Dictionary<int, NKMTeamCollectionData> m_dicTeamCollectionData = new Dictionary<int, NKMTeamCollectionData>();

	private NKMUserData owner;

	public int m_MaxUnitCount = 200;

	public int m_MaxShipCount = 10;

	public int m_MaxOperatorCount = 10;

	public int m_MaxTrophyCount = 2000;

	[DataMember]
	public Dictionary<long, NKMUnitData> m_dicMyShip = new Dictionary<long, NKMUnitData>();

	[DataMember]
	public Dictionary<long, NKMUnitData> m_dicMyUnit = new Dictionary<long, NKMUnitData>();

	[DataMember]
	public Dictionary<long, NKMOperator> m_dicMyOperator = new Dictionary<long, NKMOperator>();

	[DataMember]
	public Dictionary<long, NKMUnitData> m_dicMyTrophy = new Dictionary<long, NKMUnitData>();

	[DataMember]
	public HashSet<int> m_illustrateUnit = new HashSet<int>();

	public OnUnitUpdate dOnUnitUpdate;

	public OnDeckUpdate dOnDeckUpdate;

	public OnOperatorUpdate dOnOperatorUpdate;

	private List<long> listUnitDelete = new List<long>();

	private List<NKMItemMiscData> listUnitDeleteReward = new List<NKMItemMiscData>();

	private const int UNIT_DELETE_COUNT = 100;

	public NKMUserData Owner => owner;

	public IEnumerable<NKMDeckSet> DeckSets => deckSets;

	public bool IsEmptyUnitDeleteList => listUnitDelete.Count == 0;

	public NKMArmyData()
	{
		for (int i = 0; i < deckSets.Length; i++)
		{
			deckSets[i] = new NKMDeckSet((NKM_DECK_TYPE)i);
		}
	}

	public List<NKMDeckSet> GetDeckList()
	{
		return deckSets.ToList();
	}

	public void UpdateData(NKMUnitData UnitData)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(UnitData);
		if (unitTempletBase == null)
		{
			return;
		}
		switch (unitTempletBase.m_NKM_UNIT_TYPE)
		{
		case NKM_UNIT_TYPE.NUT_SHIP:
			UpdateShipData(UnitData);
			return;
		case NKM_UNIT_TYPE.NUT_OPERATOR:
			return;
		}
		if (unitTempletBase.m_NKM_UNIT_STYLE_TYPE != NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
		{
			UpdateUnitData(UnitData);
		}
		else
		{
			UpdateTrophyData(UnitData);
		}
	}

	public void SetOwner(NKMUserData owner)
	{
		this.owner = owner;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_MaxUnitCount);
		stream.PutOrGet(ref m_MaxShipCount);
		stream.PutOrGet(ref m_MaxOperatorCount);
		stream.PutOrGet(ref m_MaxTrophyCount);
		stream.PutOrGet(ref deckSets);
		stream.PutOrGet(ref m_dicMyShip);
		stream.PutOrGet(ref m_dicMyUnit);
		stream.PutOrGet(ref m_dicMyOperator);
		stream.PutOrGet(ref m_dicMyTrophy);
		stream.PutOrGet(ref m_illustrateUnit);
		stream.PutOrGet(ref m_dicTeamCollectionData);
	}

	public NKMDeckData GetDeckData(NKMDeckIndex deckIndex)
	{
		return GetDeckData(deckIndex.m_eDeckType, deckIndex.m_iIndex);
	}

	public NKMDeckData GetDeckData(NKM_DECK_TYPE type, int index)
	{
		IReadOnlyList<NKMDeckData> deckList = GetDeckList(type);
		if (index >= 0 && index < deckList.Count)
		{
			return deckList[index];
		}
		return null;
	}

	public bool SetPvpDefenceDeck(NKMDeckData deckData)
	{
		if (deckData == null || !deckData.CheckAllSlotFilled())
		{
			return false;
		}
		deckSets[6].SetDeck(0, deckData);
		return true;
	}

	public void SetTrimDeck(byte index, NKMDeckData deckData)
	{
		deckSets[7].SetDeck(index, deckData);
	}

	public NKMDeckData GetDeckDataByUnitUID(NKM_DECK_TYPE deckType, long unitUID)
	{
		GetDeckSet(deckType).FindDeckByUnitUid(unitUID, out var result);
		return result;
	}

	public NKMDeckData GetDeckDataByUnitUID(long unitUID)
	{
		if (unitUID == 0L)
		{
			return null;
		}
		NKMDeckSet[] array = deckSets;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].FindDeckByUnitUid(unitUID, out var result))
			{
				return result;
			}
		}
		return null;
	}

	public NKMDeckData GetDeckDataByShipUID(long shipUID)
	{
		if (shipUID == 0L)
		{
			return null;
		}
		NKMDeckSet[] array = deckSets;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].FindDeckByShipUid(shipUID, out var result))
			{
				return result;
			}
		}
		return null;
	}

	public int GetDeckCount(NKM_DECK_TYPE eType)
	{
		return GetDeckSet(eType).Count;
	}

	public NKMUnitData GetDeckShip(NKMDeckIndex deckIndex)
	{
		return GetDeckShip(deckIndex.m_eDeckType, deckIndex.m_iIndex);
	}

	public NKMUnitData GetDeckShip(NKM_DECK_TYPE eType, int deckIndex)
	{
		NKMDeckData deckData = GetDeckData(eType, deckIndex);
		if (deckData == null)
		{
			return null;
		}
		if (m_dicMyShip.ContainsKey(deckData.m_ShipUID))
		{
			return m_dicMyShip[deckData.m_ShipUID];
		}
		return null;
	}

	public NKMOperator GetDeckOperator(NKMDeckIndex deckIndex)
	{
		return GetDeckOperator(deckIndex.m_eDeckType, deckIndex.m_iIndex);
	}

	public NKMOperator GetDeckOperator(NKM_DECK_TYPE eType, int deckIndex)
	{
		NKMDeckData deckData = GetDeckData(eType, deckIndex);
		if (deckData == null)
		{
			return null;
		}
		if (m_dicMyOperator.ContainsKey(deckData.m_OperatorUID))
		{
			return m_dicMyOperator[deckData.m_OperatorUID];
		}
		return null;
	}

	public NKMUnitData GetUnitFromUID(long unitUid)
	{
		m_dicMyUnit.TryGetValue(unitUid, out var value);
		return value;
	}

	public NKMUnitData GetShipFromUID(long shipUid)
	{
		m_dicMyShip.TryGetValue(shipUid, out var value);
		return value;
	}

	public NKMUnitData GetTrophyFromUID(long trophy)
	{
		m_dicMyTrophy.TryGetValue(trophy, out var value);
		return value;
	}

	public NKMOperator GetOperatorFromUId(long operatorUid)
	{
		m_dicMyOperator.TryGetValue(operatorUid, out var value);
		return value;
	}

	public List<NKMUnitData> GetTrophyListByUnitID(int unitID)
	{
		List<NKMUnitData> list = new List<NKMUnitData>();
		foreach (NKMUnitData value in m_dicMyTrophy.Values)
		{
			if (value.m_UnitID == unitID)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public List<int> GetAmryIndexListByCategory(EPISODE_CATEGORY episodeCategory, int limitCount)
	{
		switch (episodeCategory)
		{
		case EPISODE_CATEGORY.EC_MAINSTREAM:
		case EPISODE_CATEGORY.EC_FIELD:
			return GetDeckIndexList(NKM_DECK_TYPE.NDT_NORMAL, NKM_DECK_STATE.DECK_STATE_WARFARE, limitCount);
		case EPISODE_CATEGORY.EC_DAILY:
			return GetDeckIndexList(NKM_DECK_TYPE.NDT_DAILY, NKM_DECK_STATE.DECK_STATE_NORMAL, limitCount);
		default:
			return null;
		}
	}

	public List<int> GetDeckIndexList(NKM_DECK_TYPE deckType, NKM_DECK_STATE deckState, int limit)
	{
		IReadOnlyList<NKMDeckData> deckList = GetDeckList(deckType);
		List<int> list = new List<int>();
		for (int i = 0; i < deckList.Count; i++)
		{
			if (list.Count >= limit)
			{
				break;
			}
			if (deckList[i].GetState() == deckState)
			{
				list.Add(i);
			}
		}
		return list;
	}

	public static Predicate<NKMUnitData> MakeSearchPredicate(int unitID, UNIT_SEARCH_OPTION searchType, int searchValue)
	{
		return searchType switch
		{
			UNIT_SEARCH_OPTION.None => (NKMUnitData unit) => unit.m_UnitID == unitID, 
			UNIT_SEARCH_OPTION.Level => (NKMUnitData unit) => unit.m_UnitID == unitID && unit.m_UnitLevel >= searchValue, 
			UNIT_SEARCH_OPTION.LimitLevel => (NKMUnitData unit) => unit.m_UnitID == unitID && unit.m_LimitBreakLevel >= searchValue, 
			UNIT_SEARCH_OPTION.Devotion => (NKMUnitData unit) => unit.m_UnitID == unitID && unit.IsPermanentContract, 
			UNIT_SEARCH_OPTION.StarGrade => (NKMUnitData unit) => unit.m_UnitID == unitID && unit.GetStarGrade() >= searchValue, 
			_ => (NKMUnitData unit) => false, 
		};
	}

	private static Func<NKMUnitData, long> MakeUnitStatusSelector(UNIT_SEARCH_OPTION searchType)
	{
		return searchType switch
		{
			UNIT_SEARCH_OPTION.None => (NKMUnitData unit) => unit.m_UnitUID, 
			UNIT_SEARCH_OPTION.Level => (NKMUnitData unit) => unit.m_UnitLevel, 
			UNIT_SEARCH_OPTION.LimitLevel => (NKMUnitData unit) => unit.m_LimitBreakLevel, 
			UNIT_SEARCH_OPTION.Devotion => (NKMUnitData unit) => unit.IsPermanentContract ? 1 : 0, 
			UNIT_SEARCH_OPTION.StarGrade => (NKMUnitData unit) => unit.GetStarGrade(), 
			_ => (NKMUnitData unit) => 0L, 
		};
	}

	public long SearchMaxUnitStatusByID(NKM_UNIT_TYPE unitType, int unitID, UNIT_SEARCH_OPTION searchType)
	{
		IEnumerable<NKMUnitData> enumerable = null;
		switch (unitType)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
			enumerable = m_dicMyUnit.Values;
			break;
		case NKM_UNIT_TYPE.NUT_SHIP:
			enumerable = m_dicMyShip.Values;
			break;
		}
		if (enumerable == null)
		{
			return 0L;
		}
		IEnumerable<NKMUnitData> source = enumerable.Where((NKMUnitData unit) => unit.m_UnitID == unitID);
		if (source.Count() <= 0)
		{
			return 0L;
		}
		Func<NKMUnitData, long> selector = MakeUnitStatusSelector(searchType);
		return source.Max((NKMUnitData unit) => selector(unit));
	}

	public NKMUnitData GetUnitOrShipFromUID(long UID)
	{
		if (m_dicMyUnit.ContainsKey(UID))
		{
			return m_dicMyUnit[UID];
		}
		if (m_dicMyTrophy.ContainsKey(UID))
		{
			return m_dicMyTrophy[UID];
		}
		if (m_dicMyShip.ContainsKey(UID))
		{
			return m_dicMyShip[UID];
		}
		return null;
	}

	public NKMUnitData GetUnitOrTrophyFromUID(long UID)
	{
		if (m_dicMyUnit.ContainsKey(UID))
		{
			return m_dicMyUnit[UID];
		}
		if (m_dicMyTrophy.ContainsKey(UID))
		{
			return m_dicMyTrophy[UID];
		}
		return null;
	}

	public NKMUnitData GetDeckUnitByIndex(NKMDeckIndex deckIndex, int slotIndex)
	{
		return GetDeckUnitByIndex(deckIndex.m_eDeckType, deckIndex.m_iIndex, slotIndex);
	}

	public NKMUnitData GetDeckUnitByIndex(NKM_DECK_TYPE eType, int deckIndex, int slotIndex)
	{
		NKMDeckData deckData = GetDeckData(eType, deckIndex);
		return GetUnitData(deckData, slotIndex);
	}

	private NKMUnitData GetUnitData(NKMDeckData cNKMDeckData, int slotIndex)
	{
		if (cNKMDeckData == null)
		{
			return null;
		}
		if (slotIndex >= 0 && slotIndex < cNKMDeckData.m_listDeckUnitUID.Count)
		{
			long key = cNKMDeckData.m_listDeckUnitUID[slotIndex];
			if (m_dicMyUnit.ContainsKey(key))
			{
				return m_dicMyUnit[key];
			}
		}
		return null;
	}

	public NKMOperator GetDeckOperatorByIndex(NKMDeckIndex deckIndex)
	{
		return GetDeckOperatorByIndex(deckIndex.m_eDeckType, deckIndex.m_iIndex);
	}

	public NKMOperator GetDeckOperatorByIndex(NKM_DECK_TYPE eType, int deckIndex)
	{
		NKMDeckData deckData = GetDeckData(eType, deckIndex);
		if (deckData == null)
		{
			return null;
		}
		if (!m_dicMyOperator.ContainsKey(deckData.m_OperatorUID))
		{
			return null;
		}
		return m_dicMyOperator[deckData.m_OperatorUID];
	}

	public NKMUnitData GetDeckLeaderUnitData(NKM_DECK_TYPE deckType, byte deckIndex)
	{
		NKMDeckData deckData = GetDeckData(deckType, deckIndex);
		if (deckData != null)
		{
			return GetUnitData(deckData, deckData.m_LeaderIndex);
		}
		return null;
	}

	public NKMUnitData GetDeckLeaderUnitData(NKMDeckIndex deckIndex)
	{
		NKMDeckData deckData = GetDeckData(deckIndex);
		if (deckData != null)
		{
			return GetUnitData(deckData, deckData.m_LeaderIndex);
		}
		return null;
	}

	public IReadOnlyList<NKMDeckData> GetDeckList(NKM_DECK_TYPE eType)
	{
		int num = (int)eType;
		if (num >= deckSets.Length)
		{
			num = 0;
		}
		return deckSets[num].Values;
	}

	public NKMDeckSet GetDeckSet(NKM_DECK_TYPE eType)
	{
		int num = (int)eType;
		if (num >= deckSets.Length)
		{
			num = 0;
		}
		return deckSets[num];
	}

	public NKMDeckIndex GetShipDeckIndex(NKM_DECK_TYPE eType, long shipUID)
	{
		GetDeckSet(eType).FindDeckIndexByShipUid(shipUID, out var result);
		return result;
	}

	public NKMDeckIndex GetOperatorDeckIndex(NKM_DECK_TYPE eType, long operatorUid)
	{
		GetDeckSet(eType).FindDeckIndexByOperatorUid(operatorUid, out var result);
		return result;
	}

	public bool GetUnitDeckPosition(NKM_DECK_TYPE eType, long unitUID, out NKMDeckIndex unitDeckIndex, out sbyte unitSlotIndex)
	{
		return GetDeckSet(eType).FindDeckIndexByUnitUid(unitUID, out unitDeckIndex, out unitSlotIndex);
	}

	public void AddDeck(NKM_DECK_TYPE eType, NKMDeckData newDeck)
	{
		GetDeckSet(eType).AddDeck(newDeck);
	}

	public int GetCurrentUnitCount()
	{
		return m_dicMyUnit.Count;
	}

	public int GetCurrentShipCount()
	{
		return m_dicMyShip.Count;
	}

	public int GetCurrentOperatorCount()
	{
		return m_dicMyOperator.Count;
	}

	public int GetCurrentTrophyCount()
	{
		return m_dicMyTrophy.Count;
	}

	public bool CanGetMoreUnit(int addCount)
	{
		return GetCurrentUnitCount() + addCount <= m_MaxUnitCount;
	}

	public bool CanGetMoreShip(int addCount)
	{
		return GetCurrentShipCount() + addCount <= m_MaxShipCount;
	}

	public bool CanGetMoreOperator(int addCount)
	{
		return GetCurrentOperatorCount() + addCount <= m_MaxOperatorCount;
	}

	public bool CanGetMoreTrophy(int addCount)
	{
		return GetCurrentTrophyCount() + addCount <= m_MaxTrophyCount;
	}

	public bool IsValidDeckIndex(NKMDeckIndex deckIndex)
	{
		return IsValidDeckIndex(deckIndex.m_eDeckType, deckIndex.m_iIndex);
	}

	public bool IsValidDeckIndex(NKM_DECK_TYPE eType, int deckIndex)
	{
		if (eType == NKM_DECK_TYPE.NDT_NONE)
		{
			return false;
		}
		if (0 <= deckIndex)
		{
			return deckIndex < GetUnlockedDeckCount(eType);
		}
		return false;
	}

	public byte GetUnlockedDeckCount(NKM_DECK_TYPE eType)
	{
		if (eType == NKM_DECK_TYPE.NDT_TRIM)
		{
			return GetMaxDeckCount(NKM_DECK_TYPE.NDT_TRIM);
		}
		return (byte)GetDeckList(eType).Count;
	}

	public void UnlockDeck(NKM_DECK_TYPE eType, int newDeckSize)
	{
		if (eType == NKM_DECK_TYPE.NDT_NONE)
		{
			Log.Error("Trying unlock nonexistent deck", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMArmyData.cs", 574);
			return;
		}
		if (newDeckSize > GetMaxDeckCount(eType))
		{
			Log.Error("Trying unlock beyond max size", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMArmyData.cs", 580);
			newDeckSize = GetMaxDeckCount(eType);
		}
		while (GetDeckCount(eType) < newDeckSize)
		{
			NKMDeckData newDeck = new NKMDeckData(eType);
			AddDeck(eType, newDeck);
		}
	}

	public void RemoveUnit(IEnumerable<long> lstUnitUID)
	{
		if (lstUnitUID == null)
		{
			return;
		}
		foreach (long item in lstUnitUID)
		{
			if (m_dicMyUnit.ContainsKey(item))
			{
				RemoveUnit(item);
			}
			else if (m_dicMyTrophy.ContainsKey(item))
			{
				RemoveTrophy(item);
			}
		}
	}

	public void RemoveShip(IEnumerable<long> lstShipUID)
	{
		if (lstShipUID == null)
		{
			return;
		}
		foreach (long item in lstShipUID)
		{
			RemoveShip(item);
		}
	}

	public void RemoveOperator(IEnumerable<long> lstOperatorUID)
	{
		if (lstOperatorUID == null)
		{
			return;
		}
		foreach (long item in lstOperatorUID)
		{
			RemoveOperator(item);
		}
	}

	public void RemoveOperator(long operatorUid)
	{
		m_dicMyOperator.Remove(operatorUid);
	}

	public bool IsUnitInAnyDeck(long unitUID)
	{
		NKMDeckSet[] array = deckSets;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].FindDeckByUnitUid(unitUID, out var _))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsOperatorAnyDeck(long operatorUId)
	{
		NKMDeckSet[] array = deckSets;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].FindDeckByOperatporUid(operatorUId))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsShipInAnyDeck(long shipUid)
	{
		NKMDeckSet[] array = deckSets;
		for (int i = 0; i < array.Length; i++)
		{
			foreach (NKMDeckData value in array[i].Values)
			{
				if (value.m_ShipUID == shipUid)
				{
					return true;
				}
			}
		}
		return false;
	}

	public NKM_DECK_STATE GetUnitDeckState(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return NKM_DECK_STATE.DECK_STATE_NORMAL;
		}
		return GetUnitDeckState(unitData.m_UnitUID);
	}

	public NKM_DECK_STATE GetUnitDeckState(long unitUID)
	{
		foreach (NKMDeckData value in GetDeckSet(NKM_DECK_TYPE.NDT_DIVE).Values)
		{
			if (value.m_ShipUID == unitUID || value.HasUnitUid(unitUID, out var _))
			{
				return value.m_DeckState;
			}
		}
		return NKM_DECK_STATE.DECK_STATE_NORMAL;
	}

	public NKM_DECK_STATE GetOperatorDeckState(NKMOperator operatorData)
	{
		if (operatorData == null)
		{
			return NKM_DECK_STATE.DECK_STATE_NORMAL;
		}
		return GetOperatorDeckState(operatorData.uid);
	}

	public NKM_DECK_STATE GetOperatorDeckState(long operatorUID)
	{
		foreach (NKMDeckData value in GetDeckSet(NKM_DECK_TYPE.NDT_DIVE).Values)
		{
			if (value.m_OperatorUID == operatorUID)
			{
				return value.m_DeckState;
			}
		}
		return NKM_DECK_STATE.DECK_STATE_NORMAL;
	}

	public int GetArmyAvarageOperationPower(NKMDeckIndex index, bool bPVP = false, Dictionary<int, NKMBanData> dicNKMBanData = null, Dictionary<int, NKMUnitUpData> dicNKMUpData = null)
	{
		NKMDeckData deckData = GetDeckData(index);
		if (deckData == null)
		{
			return 0;
		}
		return GetArmyAvarageOperationPower(deckData, bPVP, dicNKMBanData, dicNKMUpData);
	}

	public int GetArmyAvarageOperationPower(NKMDeckData deckData, bool bPVP = false, Dictionary<int, NKMBanData> dicNKMBanData = null, Dictionary<int, NKMUnitUpData> dicNKMUpData = null)
	{
		if (deckData == null)
		{
			return 0;
		}
		NKMUnitData shipFromUID = GetShipFromUID(deckData.m_ShipUID);
		IEnumerable<NKMUnitData> units = deckData.GetUnits(this);
		int operatorPower = 0;
		if (deckData.m_OperatorUID != 0L)
		{
			NKMOperator operatorFromUId = GetOperatorFromUId(deckData.m_OperatorUID);
			if (operatorFromUId != null)
			{
				operatorPower = operatorFromUId.CalculateOperatorOperationPower();
			}
		}
		return NKMOperationPower.Calculate(shipFromUID, units, deckData.GetLeaderUnitUID(), owner.m_InventoryData, bPVP, dicNKMBanData, dicNKMUpData, operatorPower);
	}

	public int GetArmyAvarageOperationPower(NKMEventDeckData deckData, bool bPVP = false, Dictionary<int, NKMBanData> dicNKMBanData = null, Dictionary<int, NKMUnitUpData> dicNKMUpData = null)
	{
		if (deckData == null)
		{
			return 0;
		}
		NKMUnitData shipFromUID = GetShipFromUID(deckData.m_ShipUID);
		IEnumerable<NKMUnitData> units = deckData.GetUnits(this);
		int operatorPower = 0;
		if (deckData.m_OperatorUID != 0L)
		{
			NKMOperator operatorFromUId = GetOperatorFromUId(deckData.m_OperatorUID);
			if (operatorFromUId != null)
			{
				operatorPower = operatorFromUId.CalculateOperatorOperationPower();
			}
		}
		return NKMOperationPower.Calculate(shipFromUID, units, deckData.m_dicUnit[deckData.m_LeaderIndex], owner.m_InventoryData, bPVP, dicNKMBanData, dicNKMUpData, operatorPower);
	}

	public bool CanSetLeader(NKMDeckIndex deckIndex)
	{
		if (!IsValidDeckIndex(deckIndex))
		{
			return false;
		}
		NKMDeckData deckData = GetDeckData(deckIndex);
		if (deckData == null)
		{
			return false;
		}
		if (deckData.IsValidState() != NKM_ERROR_CODE.NEC_OK)
		{
			return false;
		}
		return true;
	}

	public NKM_ERROR_CODE CanModifyOrPlayDeck(NKMDeckIndex deckIndex)
	{
		if (!IsValidDeckIndex(deckIndex))
		{
			return NKM_ERROR_CODE.NEC_FAIL_DECK_DATA_INVALID;
		}
		NKMDeckData deckData = GetDeckData(deckIndex);
		if (deckData == null)
		{
			Log.Error($"Invalid DeckIndex. userUid:{owner.m_UserUID}, deckType:{deckIndex.m_eDeckType}, deckIndex:{deckIndex.m_iIndex}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMArmyData.cs", 840);
			return NKM_ERROR_CODE.NEC_FAIL_DECK_DATA_INVALID;
		}
		return deckData.IsValidState();
	}

	public void AddTeamCollectionData(NKMTeamCollectionData data)
	{
		if (!m_dicTeamCollectionData.ContainsKey(data.TeamID))
		{
			m_dicTeamCollectionData.Add(data.TeamID, data);
		}
	}

	public NKMTeamCollectionData GetTeamCollectionData(int teamID)
	{
		m_dicTeamCollectionData.TryGetValue(teamID, out var value);
		return value;
	}

	public float CalculateDeckAvgSummonCost(NKMDeckIndex deckIndex, Dictionary<int, NKMBanData> dicNKMBanData = null, Dictionary<int, NKMUnitUpData> dicNKMUpData = null)
	{
		NKMDeckData deckData = GetDeckData(deckIndex);
		int num = 0;
		int num2 = 0;
		if (deckData == null)
		{
			return 0f;
		}
		for (int i = 0; i < 8; i++)
		{
			long unitUid = deckData.m_listDeckUnitUID[i];
			NKMUnitData unitFromUID = GetUnitFromUID(unitUid);
			if (unitFromUID != null)
			{
				num++;
				NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitFromUID.m_UnitID);
				if (unitStatTemplet == null)
				{
					Log.Error($"Cannot found UnitStatTemplet. UserUid:{owner.m_UserUID}, UnitId:{unitFromUID.m_UnitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMArmyData.cs", 883);
				}
				else
				{
					num2 += unitStatTemplet.GetRespawnCost(i == deckData.m_LeaderIndex, dicNKMBanData, dicNKMUpData);
				}
			}
		}
		if (num == 0)
		{
			return 0f;
		}
		return (float)num2 / (float)num;
	}

	public void ResetDeckStateOf(NKM_DECK_STATE targetState)
	{
		foreach (NKMDeckData value in deckSets[1].Values)
		{
			if (value != null && value.m_DeckState == targetState)
			{
				value.m_DeckState = NKM_DECK_STATE.DECK_STATE_NORMAL;
			}
		}
		foreach (NKMDeckData value2 in deckSets[8].Values)
		{
			if (value2 != null && value2.m_DeckState == targetState)
			{
				value2.m_DeckState = NKM_DECK_STATE.DECK_STATE_NORMAL;
			}
		}
	}

	public int GetUnitCountByLevel(int level)
	{
		return m_dicMyUnit.Values.Where((NKMUnitData e) => e.m_UnitLevel >= level).Count();
	}

	public int GetShipCountByLevel(int level)
	{
		return m_dicMyShip.Values.Where((NKMUnitData e) => e.m_UnitLevel >= level).Count();
	}

	public int GetOperatorCountByLevel(int level)
	{
		return m_dicMyOperator.Values.Where((NKMOperator e) => e.level >= level).Count();
	}

	public int GetUnitPermanentCount()
	{
		return m_dicMyUnit.Values.Count((NKMUnitData e) => e.IsPermanentContract);
	}

	public int GetUnitCountByTactic(int tacticLevel)
	{
		return m_dicMyUnit.Values.Where((NKMUnitData e) => e.tacticLevel >= tacticLevel).Count();
	}

	public byte GetMaxDeckCount(NKM_DECK_TYPE type)
	{
		return type switch
		{
			NKM_DECK_TYPE.NDT_NORMAL => (byte)NKMCommonConst.Deck.MaxNormalDeckCount, 
			NKM_DECK_TYPE.NDT_PVP => (byte)NKMCommonConst.Deck.MaxPvpDeckCount, 
			NKM_DECK_TYPE.NDT_DAILY => (byte)NKMCommonConst.Deck.MaxDailyDeckCount, 
			NKM_DECK_TYPE.NDT_RAID => (byte)NKMCommonConst.Deck.MaxRaidDeckCount, 
			NKM_DECK_TYPE.NDT_FRIEND => (byte)NKMCommonConst.Deck.MaxFriendDeckCount, 
			NKM_DECK_TYPE.NDT_PVP_DEFENCE => (byte)NKMCommonConst.Deck.MaxPvpDefenceDeckCount, 
			NKM_DECK_TYPE.NDT_TRIM => (byte)NKMCommonConst.Deck.MaxTrimingDeckCount, 
			NKM_DECK_TYPE.NDT_DIVE => (byte)NKMCommonConst.Deck.MaxDiveDeckCount, 
			_ => 0, 
		};
	}

	public bool IsFirstGetUnit(int unitID)
	{
		if (!m_illustrateUnit.Contains(unitID))
		{
			return true;
		}
		return false;
	}

	public void AddNewUnit(NKMUnitData newUnit)
	{
		if (newUnit == null)
		{
			Log.Error("Trying to add null unit", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMArmyDataEx.cs", 43);
			return;
		}
		if (TryCollectUnit(newUnit.m_UnitID))
		{
			NKCContentManager.SetUnlockedContent(STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_COLLECTION_RARITY_COUNT);
			NKCContentManager.SetUnlockedContent(STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_GET, newUnit.m_UnitID);
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(newUnit);
		if (unitTempletBase == null)
		{
			Log.Error($"unit has no templetbase! id : {newUnit.m_UnitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMArmyDataEx.cs", 57);
			return;
		}
		if (unitTempletBase.IsTrophy)
		{
			if (m_dicMyTrophy.ContainsKey(newUnit.m_UnitUID))
			{
				Log.Error("Trying to add duplicated trophy", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMArmyDataEx.cs", 69);
				return;
			}
			m_dicMyTrophy.Add(newUnit.m_UnitUID, newUnit);
		}
		else
		{
			if (m_dicMyUnit.ContainsKey(newUnit.m_UnitUID))
			{
				Log.Error("Trying to add duplicated unit", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMArmyDataEx.cs", 81);
				return;
			}
			m_dicMyUnit.Add(newUnit.m_UnitUID, newUnit);
		}
		dOnUnitUpdate?.Invoke(NKMUserData.eChangeNotifyType.Add, NKM_UNIT_TYPE.NUT_NORMAL, newUnit.m_UnitUID, newUnit);
	}

	public bool SearchUnitByID(NKM_UNIT_TYPE unitType, int unitID, UNIT_SEARCH_OPTION searchType, int searchValue)
	{
		IEnumerable<NKMUnitData> enumerable = null;
		switch (unitType)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
			enumerable = ((unitTempletBase == null || !unitTempletBase.IsTrophy) ? m_dicMyUnit.Values : m_dicMyTrophy.Values);
			break;
		}
		case NKM_UNIT_TYPE.NUT_SHIP:
			enumerable = m_dicMyShip.Values;
			break;
		}
		if (enumerable == null)
		{
			return false;
		}
		if (searchType == UNIT_SEARCH_OPTION.Level && unitType == NKM_UNIT_TYPE.NUT_NORMAL && IsHasRearmUnit(unitID))
		{
			return true;
		}
		Predicate<NKMUnitData> predicate = MakeSearchPredicate(unitID, searchType, searchValue);
		return enumerable.Any((NKMUnitData unit) => predicate(unit));
	}

	private bool IsHasRearmUnit(int unitID)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
		if (unitTempletBase != null)
		{
			foreach (NKMUnitRearmamentTemplet value in NKMTempletContainer<NKMUnitRearmamentTemplet>.Values)
			{
				if (value.EnableByTag && value.FromUnitTemplet.m_UnitID == unitID && IsCollectedUnit(value.ToUnitTemplet.m_UnitID))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void AddNewShip(NKMUnitData newShip)
	{
		if (newShip == null)
		{
			Log.Error("Trying to add null ship", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMArmyDataEx.cs", 149);
			return;
		}
		TryCollectUnit(newShip.m_UnitID);
		if (!m_dicMyShip.ContainsKey(newShip.m_UnitUID))
		{
			m_dicMyShip.Add(newShip.m_UnitUID, newShip);
			dOnUnitUpdate?.Invoke(NKMUserData.eChangeNotifyType.Add, NKM_UNIT_TYPE.NUT_SHIP, newShip.m_UnitUID, newShip);
		}
		else
		{
			Log.Error("Trying to add duplicated ship", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMArmyDataEx.cs", 161);
		}
	}

	public void AddNewOperator(NKMOperator newOperator)
	{
		if (newOperator == null)
		{
			Log.Error("Trying to add null operator", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMArmyDataEx.cs", 171);
			return;
		}
		TryCollectUnit(newOperator.id);
		if (!m_dicMyOperator.ContainsKey(newOperator.uid))
		{
			m_dicMyOperator.Add(newOperator.uid, newOperator);
			dOnOperatorUpdate?.Invoke(NKMUserData.eChangeNotifyType.Add, newOperator.uid, newOperator);
		}
		else
		{
			Log.Error("Trying to add duplicated operator", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMArmyDataEx.cs", 183);
		}
	}

	public int GetUnitCollectCount(IEnumerable<int> unitIDList)
	{
		int num = 0;
		foreach (int unitID in unitIDList)
		{
			if (IsCollectedUnit(unitID))
			{
				num++;
			}
		}
		return num;
	}

	public bool IsCollectedUnit(int unitID)
	{
		return m_illustrateUnit.Contains(unitID);
	}

	public bool TryCollectUnit(int unitID)
	{
		if (IsCollectedUnit(unitID))
		{
			return false;
		}
		NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(unitID);
		if (nKMUnitTempletBase != null && nKMUnitTempletBase.IsRearmUnit)
		{
			m_illustrateUnit.Add(nKMUnitTempletBase.m_BaseUnitID);
		}
		m_illustrateUnit.Add(unitID);
		return true;
	}

	public void UpdateUnitData(List<NKMUnitData> lstUnitData)
	{
		foreach (NKMUnitData lstUnitDatum in lstUnitData)
		{
			UpdateUnitData(lstUnitDatum);
		}
	}

	public void UpdateUnitData(NKMUnitData UnitData)
	{
		if (UnitData == null)
		{
			return;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(UnitData);
		if (unitTempletBase != null && unitTempletBase.IsTrophy)
		{
			if (!m_dicMyTrophy.ContainsKey(UnitData.m_UnitUID))
			{
				Log.Error("Tried to update nonexist unit", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMArmyDataEx.cs", 245);
				return;
			}
			m_dicMyTrophy[UnitData.m_UnitUID] = UnitData;
		}
		else
		{
			if (!m_dicMyUnit.ContainsKey(UnitData.m_UnitUID))
			{
				Log.Error("Tried to update nonexist unit", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMArmyDataEx.cs", 254);
				return;
			}
			m_dicMyUnit[UnitData.m_UnitUID] = UnitData;
		}
		dOnUnitUpdate?.Invoke(NKMUserData.eChangeNotifyType.Update, NKM_UNIT_TYPE.NUT_NORMAL, UnitData.m_UnitUID, UnitData);
	}

	public void UpdateShipData(NKMUnitData ShipData)
	{
		if (ShipData != null)
		{
			if (!m_dicMyShip.ContainsKey(ShipData.m_UnitUID))
			{
				Log.Error("Tried to update nonexist ship", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMArmyDataEx.cs", 270);
				return;
			}
			m_dicMyShip[ShipData.m_UnitUID] = ShipData;
			dOnUnitUpdate?.Invoke(NKMUserData.eChangeNotifyType.Update, NKM_UNIT_TYPE.NUT_SHIP, ShipData.m_UnitUID, ShipData);
		}
	}

	public bool UpdateOperatorData(NKMOperator OperatorData)
	{
		bool flag = false;
		if (OperatorData == null)
		{
			return false;
		}
		if (!m_dicMyOperator.ContainsKey(OperatorData.uid))
		{
			Log.Error("Tried to update nonexist operator", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMArmyDataEx.cs", 285);
			return false;
		}
		if (m_dicMyOperator[OperatorData.uid].level < OperatorData.level)
		{
			flag = true;
		}
		flag = (((((flag || m_dicMyOperator[OperatorData.uid].mainSkill.level >= OperatorData.mainSkill.level) && 0 == 0 && m_dicMyOperator[OperatorData.uid].subSkill.id != OperatorData.subSkill.id) || false || m_dicMyOperator[OperatorData.uid].subSkill.level >= OperatorData.subSkill.level) && 0 == 0 && m_dicMyOperator[OperatorData.uid].subSkill.exp < OperatorData.subSkill.exp) ? true : false);
		m_dicMyOperator[OperatorData.uid] = OperatorData;
		dOnOperatorUpdate?.Invoke(NKMUserData.eChangeNotifyType.Update, OperatorData.uid, OperatorData);
		return flag;
	}

	public void UpdateTrophyData(NKMUnitData trophyData)
	{
		if (trophyData != null)
		{
			if (!m_dicMyTrophy.ContainsKey(trophyData.m_UnitUID))
			{
				Log.Error("Tried to update nonexist trophy", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMArmyDataEx.cs", 311);
				return;
			}
			m_dicMyTrophy[trophyData.m_UnitUID] = trophyData;
			dOnUnitUpdate?.Invoke(NKMUserData.eChangeNotifyType.Update, NKM_UNIT_TYPE.NUT_NORMAL, trophyData.m_UnitUID, trophyData);
		}
	}

	public void RemoveUnitOrShip(long unitUid)
	{
		if (m_dicMyUnit.ContainsKey(unitUid))
		{
			RemoveUnit(unitUid);
		}
		else if (m_dicMyShip.ContainsKey(unitUid))
		{
			RemoveShip(unitUid);
		}
		else if (m_dicMyTrophy.ContainsKey(unitUid))
		{
			RemoveTrophy(unitUid);
		}
		else
		{
			Log.Error($"{unitUid} 에 해당하는 유닛/함선이 없음", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMArmyDataEx.cs", 329);
		}
	}

	public void RemoveUnit(long unitUID)
	{
		m_dicMyUnit.Remove(unitUID);
		m_dicMyTrophy.Remove(unitUID);
		dOnUnitUpdate?.Invoke(NKMUserData.eChangeNotifyType.Remove, NKM_UNIT_TYPE.NUT_NORMAL, unitUID, null);
	}

	public void RemoveUnitList(IEnumerable<long> lstUnitUID)
	{
		foreach (long item in lstUnitUID)
		{
			m_dicMyUnit.Remove(item);
			m_dicMyTrophy.Remove(item);
		}
		dOnUnitUpdate?.Invoke(NKMUserData.eChangeNotifyType.Remove, NKM_UNIT_TYPE.NUT_NORMAL, 0L, null);
	}

	public void RemoveShip(long shipUID)
	{
		m_dicMyShip.Remove(shipUID);
		if (NKCScenManager.CurrentUserData().GetShipCandidateData().shipUid == shipUID)
		{
			NKCPacketSender.Send_NKMPacket_SHIP_SLOT_OPTION_CANCEL_REQ();
		}
		dOnUnitUpdate?.Invoke(NKMUserData.eChangeNotifyType.Remove, NKM_UNIT_TYPE.NUT_SHIP, shipUID, null);
	}

	public void RemoveOperatorEx(long operatorUID)
	{
		RemoveOperator(operatorUID);
		dOnOperatorUpdate?.Invoke(NKMUserData.eChangeNotifyType.Remove, operatorUID, null);
	}

	public void RemoveTrophy(long unitUID)
	{
		m_dicMyTrophy.Remove(unitUID);
		dOnUnitUpdate?.Invoke(NKMUserData.eChangeNotifyType.Remove, NKM_UNIT_TYPE.NUT_NORMAL, unitUID, null);
	}

	public void SetTournamentDeck(NKMDeckData deckData)
	{
		deckSets[9].SetDeck(0, deckData);
	}

	public void DeckUpdated(NKMDeckIndex deckIndex, NKMDeckData deckData)
	{
		if (deckData != null)
		{
			dOnDeckUpdate?.Invoke(deckIndex, deckData);
		}
	}

	public void RemoveUnitInDeckByUnitUid(long unitUid)
	{
		NKMDeckSet[] array = deckSets;
		for (int i = 0; i < array.Length; i++)
		{
			foreach (NKMDeckData value in array[i].Values)
			{
				if (value.HasUnitUid(unitUid, out var index))
				{
					value.m_listDeckUnitUID[index] = 0L;
				}
			}
		}
	}

	public void GetDeckList(NKM_DECK_TYPE eType, int slotIndex, ref List<long> unitList)
	{
		NKMDeckData deckData = GetDeckData(eType, slotIndex);
		if (deckData == null)
		{
			return;
		}
		for (int i = 0; i < deckData.m_listDeckUnitUID.Count; i++)
		{
			if (deckData.m_listDeckUnitUID[i] != 0L)
			{
				unitList.Add(deckData.m_listDeckUnitUID[i]);
			}
		}
	}

	public void SetDeckUnitByIndex(NKMDeckIndex deckIndex, byte slotIndex, long unitUID)
	{
		SetDeckUnitByIndex(deckIndex.m_eDeckType, deckIndex.m_iIndex, slotIndex, unitUID);
	}

	public void SetDeckUnitByIndex(NKM_DECK_TYPE eType, int deckIndex, byte slotIndex, long unitUID)
	{
		if (m_dicMyUnit.ContainsKey(unitUID) || unitUID == 0L)
		{
			GetDeckData(eType, deckIndex).SetUnitUID(slotIndex, unitUID);
		}
	}

	public void SetDeckOperatorByIndex(NKM_DECK_TYPE eType, int deckIndex, long unitUID)
	{
		if (m_dicMyOperator.ContainsKey(unitUID) || unitUID == 0L)
		{
			GetDeckData(eType, deckIndex)?.SetOperatorUID(unitUID);
		}
	}

	public void SetDeckLeader(NKMDeckIndex deckIndex, sbyte leaderSlotIndex)
	{
		if (deckIndex.m_eDeckType != NKM_DECK_TYPE.NDT_NONE)
		{
			GetDeckData(deckIndex.m_eDeckType, deckIndex.m_iIndex).m_LeaderIndex = leaderSlotIndex;
		}
	}

	public Dictionary<NKMDeckIndex, NKMDeckData> GetAllDecks()
	{
		Dictionary<NKMDeckIndex, NKMDeckData> dictionary = new Dictionary<NKMDeckIndex, NKMDeckData>();
		foreach (NKM_DECK_TYPE value in Enum.GetValues(typeof(NKM_DECK_TYPE)))
		{
			int deckCount = GetDeckCount(value);
			for (int i = 0; i < deckCount; i++)
			{
				NKMDeckIndex nKMDeckIndex = new NKMDeckIndex(value, i);
				dictionary.Add(nKMDeckIndex, GetDeckData(nKMDeckIndex));
			}
		}
		return dictionary;
	}

	public int GetAvailableDeckIndex(NKM_DECK_TYPE eType)
	{
		return GetDeckSet(eType).GetAvailableDeckIndex(this);
	}

	public NKMDeckIndex GetDeckIndexByUnitUID(NKM_DECK_TYPE deckType, long unitUID)
	{
		if (unitUID == 0L)
		{
			return NKMDeckIndex.None;
		}
		GetDeckSet(deckType).FindDeckIndexByUnitUid(unitUID, out var result);
		return result;
	}

	public bool IsHaveUnitFromUID(long unitUID)
	{
		return m_dicMyUnit.ContainsKey(unitUID);
	}

	public bool IsHaveShipFromUID(long shipUID)
	{
		return m_dicMyShip.ContainsKey(shipUID);
	}

	public bool IsHaveOperatorFromUID(long operatorUID)
	{
		return m_dicMyOperator.ContainsKey(operatorUID);
	}

	public int GetSameKindShipCountFromID(int shipID)
	{
		int num = 0;
		foreach (NKMUnitData value in m_dicMyShip.Values)
		{
			if (NKMShipManager.IsSameKindShip(shipID, value.m_UnitID))
			{
				num++;
			}
		}
		return num;
	}

	public static bool IsAllowedSameUnitInMultipleDeck(NKM_DECK_TYPE eType)
	{
		return eType != NKM_DECK_TYPE.NDT_NORMAL;
	}

	public List<byte> GetValidDeckIndexByOperationPowerDecendSort(NKM_DECK_TYPE eNKM_DECK_TYPE)
	{
		int deckCount = GetDeckCount(eNKM_DECK_TYPE);
		List<byte> list = new List<byte>();
		List<DeckIndexWithAvgOperationPower> list2 = new List<DeckIndexWithAvgOperationPower>();
		for (byte b = 0; b < deckCount; b++)
		{
			NKMDeckIndex nKMDeckIndex = new NKMDeckIndex(eNKM_DECK_TYPE, b);
			if (NKMMain.IsValidDeck(this, nKMDeckIndex) == NKM_ERROR_CODE.NEC_OK)
			{
				DeckIndexWithAvgOperationPower deckIndexWithAvgOperationPower = new DeckIndexWithAvgOperationPower();
				deckIndexWithAvgOperationPower.m_Index = b;
				deckIndexWithAvgOperationPower.m_AvgOperationPower = GetArmyAvarageOperationPower(nKMDeckIndex);
				list2.Add(deckIndexWithAvgOperationPower);
			}
		}
		list2.Sort();
		for (int i = 0; i < list2.Count; i++)
		{
			list.Add(list2[i].m_Index);
		}
		return list;
	}

	public bool HaveUnit(int unitID, bool bIncludeRearm)
	{
		NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(unitID);
		if (nKMUnitTempletBase != null && nKMUnitTempletBase.IsTrophy)
		{
			foreach (NKMUnitData value in m_dicMyTrophy.Values)
			{
				if (value.m_UnitID == unitID)
				{
					return true;
				}
			}
		}
		else
		{
			foreach (NKMUnitData value2 in m_dicMyUnit.Values)
			{
				if (value2.m_UnitID == unitID)
				{
					return true;
				}
				if (bIncludeRearm && nKMUnitTempletBase.IsSameBaseUnit(value2.m_UnitID))
				{
					return true;
				}
			}
		}
		return false;
	}

	public int GetUnitCountByID(int unitID)
	{
		int num = 0;
		NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(unitID);
		if (nKMUnitTempletBase != null && nKMUnitTempletBase.IsTrophy)
		{
			foreach (NKMUnitData value in m_dicMyTrophy.Values)
			{
				if (value.m_UnitID == unitID)
				{
					num++;
				}
			}
		}
		else
		{
			foreach (NKMUnitData value2 in m_dicMyUnit.Values)
			{
				if (value2.m_UnitID == unitID)
				{
					num++;
				}
			}
		}
		return num;
	}

	public int GetOperatorCountByID(int unitID)
	{
		int num = 0;
		foreach (NKMOperator value in m_dicMyOperator.Values)
		{
			if (value.id == unitID)
			{
				num++;
			}
		}
		return num;
	}

	public List<NKMUnitData> GetUnitListByUnitID(int unitID)
	{
		List<NKMUnitData> list = new List<NKMUnitData>();
		NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(unitID);
		if (nKMUnitTempletBase != null && nKMUnitTempletBase.IsTrophy)
		{
			foreach (NKMUnitData value in m_dicMyTrophy.Values)
			{
				if (value.m_UnitID == unitID)
				{
					list.Add(value);
				}
			}
		}
		else
		{
			foreach (NKMUnitData value2 in m_dicMyUnit.Values)
			{
				if (value2.m_UnitID == unitID)
				{
					list.Add(value2);
				}
			}
		}
		return list;
	}

	public int GetUnitTypeCount()
	{
		HashSet<int> hashSet = new HashSet<int>();
		foreach (KeyValuePair<long, NKMUnitData> item in m_dicMyUnit)
		{
			NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(item.Value.m_UnitID);
			if (nKMUnitTempletBase != null && NKMUnitManager.CanUnitUsedInDeck(nKMUnitTempletBase) && CanBeDraftPickCandidate(nKMUnitTempletBase) && !hashSet.Contains(item.Value.m_UnitID))
			{
				hashSet.Add(item.Value.m_UnitID);
			}
		}
		return hashSet.Count;
	}

	private bool CanBeDraftPickCandidate(NKMUnitTempletBase templetBase)
	{
		if (NKCCollectionManager.GetUnitTemplet(templetBase.m_UnitID) == null)
		{
			return false;
		}
		if (templetBase.IsUnitStyleType() && templetBase.CollectionEnableByTag && templetBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL)
		{
			return !templetBase.m_bMonster;
		}
		return false;
	}

	public int GetShipTypeCount()
	{
		HashSet<int> hashSet = new HashSet<int>();
		foreach (KeyValuePair<long, NKMUnitData> item in m_dicMyShip)
		{
			if (!hashSet.Contains(item.Value.m_UnitID))
			{
				hashSet.Add(item.Value.m_UnitID);
			}
		}
		return hashSet.Count;
	}

	public void InitUnitDelete()
	{
		listUnitDelete.Clear();
		listUnitDeleteReward.Clear();
	}

	public void SetUnitDeleteList(List<long> list)
	{
		listUnitDelete.AddRange(list);
	}

	public List<long> GetUnitDeleteList()
	{
		List<long> list = new List<long>();
		if (listUnitDelete.Count > 100)
		{
			list = listUnitDelete.GetRange(0, 100);
			listUnitDelete.RemoveRange(0, 100);
		}
		else
		{
			list.AddRange(listUnitDelete);
			listUnitDelete.Clear();
		}
		return list;
	}

	public void AddUnitDeleteRewardList(List<NKMItemMiscData> list)
	{
		listUnitDeleteReward.AddRange(list);
	}

	public List<NKMItemMiscData> GetUnitDeleteReward()
	{
		return listUnitDeleteReward;
	}

	public static bool IsAllUnitsEquipedAllGears(NKMDeckIndex _targetDeck)
	{
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
		if (armyData == null || inventoryData == null)
		{
			return false;
		}
		for (int i = 0; i < 8; i++)
		{
			NKMUnitData deckUnitByIndex = armyData.GetDeckUnitByIndex(_targetDeck, i);
			if (deckUnitByIndex != null)
			{
				if (inventoryData.GetItemEquip(deckUnitByIndex.GetEquipItemWeaponUid()) == null)
				{
					return false;
				}
				if (inventoryData.GetItemEquip(deckUnitByIndex.GetEquipItemDefenceUid()) == null)
				{
					return false;
				}
				if (inventoryData.GetItemEquip(deckUnitByIndex.GetEquipItemAccessoryUid()) == null)
				{
					return false;
				}
				if (deckUnitByIndex.IsUnlockAccessory2() && inventoryData.GetItemEquip(deckUnitByIndex.GetEquipItemAccessory2Uid()) == null)
				{
					return false;
				}
			}
		}
		return true;
	}
}
