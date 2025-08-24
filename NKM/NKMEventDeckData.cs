using System.Collections.Generic;
using Cs.Protocol;

namespace NKM;

public class NKMEventDeckData : ISerializable
{
	public Dictionary<int, long> m_dicUnit = new Dictionary<int, long>();

	public long m_ShipUID;

	public long m_OperatorUID;

	public int m_LeaderIndex;

	public int m_OperationPower;

	public long GetUnitUID(int index)
	{
		if (m_dicUnit.TryGetValue(index, out var value))
		{
			return value;
		}
		return 0L;
	}

	public NKMEventDeckData()
	{
	}

	public void DeepCopy(NKMEventDeckData src)
	{
		m_ShipUID = src.m_ShipUID;
		m_OperatorUID = src.m_OperatorUID;
		m_LeaderIndex = src.m_LeaderIndex;
		m_dicUnit.Clear();
		foreach (KeyValuePair<int, long> item in src.m_dicUnit)
		{
			m_dicUnit.Add(item.Key, item.Value);
		}
	}

	public long CompareTo(NKMEventDeckData other)
	{
		long num = 0L;
		num = m_ShipUID.CompareTo(other.m_ShipUID);
		num = m_OperatorUID.CompareTo(other.m_OperatorUID);
		num = m_LeaderIndex.CompareTo(other.m_LeaderIndex);
		if (num != 0L)
		{
			return -1L;
		}
		foreach (int key in other.m_dicUnit.Keys)
		{
			if (!m_dicUnit.ContainsKey(key))
			{
				num = 1L;
				break;
			}
			if (m_dicUnit[key] != other.m_dicUnit[key])
			{
				num = 1L;
				break;
			}
		}
		return num;
	}

	public NKMEventDeckData(Dictionary<int, long> dicUnit, long shipUID, long operatorUID, int leaderIndex)
	{
		m_dicUnit = dicUnit;
		m_ShipUID = shipUID;
		m_OperatorUID = operatorUID;
		m_LeaderIndex = leaderIndex;
	}

	public IEnumerable<NKMUnitData> GetUnits(NKMArmyData armyData)
	{
		foreach (long value in m_dicUnit.Values)
		{
			if (value != 0L)
			{
				yield return armyData.GetUnitFromUID(value);
			}
		}
	}

	public long GetFirstUnitUID()
	{
		using (Dictionary<int, long>.Enumerator enumerator = m_dicUnit.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current.Value;
			}
		}
		return 0L;
	}

	public long GetLeaderUID()
	{
		return GetUnitUID(m_LeaderIndex);
	}

	public long GetLeaderUID(List<GameUnitData> lstUnitData, NKMDungeonEventDeckTemplet deckTemplet)
	{
		if (deckTemplet == null || lstUnitData == null)
		{
			return 0L;
		}
		if (m_LeaderIndex < 0)
		{
			return 0L;
		}
		long unitUID = GetUnitUID(m_LeaderIndex);
		if (unitUID > 0)
		{
			return unitUID;
		}
		NKMDungeonEventDeckTemplet.EventDeckSlot slot = deckTemplet.GetUnitSlot(m_LeaderIndex);
		return lstUnitData.Find((GameUnitData x) => x.unit != null && x.unit.IsSameBaseUnit(slot.m_ID))?.unit.m_UnitUID ?? 0;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_ShipUID);
		stream.PutOrGet(ref m_dicUnit);
		stream.PutOrGet(ref m_OperatorUID);
		stream.PutOrGet(ref m_LeaderIndex);
	}
}
