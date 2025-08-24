using System.Collections.Generic;
using Cs.Protocol;

namespace NKM;

public sealed class NKMDeckData : ISerializable
{
	public const int InvalidIndex = -1;

	public string m_DeckName = string.Empty;

	public long m_ShipUID;

	public long m_OperatorUID;

	public List<long> m_listDeckUnitUID = new List<long>();

	public sbyte m_LeaderIndex = -1;

	public NKM_DECK_STATE m_DeckState;

	public int power;

	public NKMDeckData()
	{
	}

	public NKMDeckData(NKM_DECK_TYPE deckType)
	{
		int num = ((deckType == NKM_DECK_TYPE.NDT_RAID) ? 16 : 8);
		while (num > m_listDeckUnitUID.Count)
		{
			m_listDeckUnitUID.Add(0L);
		}
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_DeckName);
		stream.PutOrGet(ref m_ShipUID);
		stream.PutOrGet(ref m_OperatorUID);
		stream.PutOrGet(ref m_listDeckUnitUID);
		stream.PutOrGet(ref m_LeaderIndex);
		stream.PutOrGetEnum(ref m_DeckState);
	}

	public bool CheckHasDuplicateUnit(NKMArmyData armyData)
	{
		HashSet<int> hashSet = new HashSet<int>();
		for (int i = 0; i < m_listDeckUnitUID.Count; i++)
		{
			long num = m_listDeckUnitUID[i];
			if (num != 0L)
			{
				NKMUnitData unitFromUID = armyData.GetUnitFromUID(num);
				if (NKMUnitManager.CheckContainsBaseUnit(hashSet, unitFromUID.m_UnitID))
				{
					return true;
				}
				if (!hashSet.Add(unitFromUID.m_UnitID))
				{
					return true;
				}
			}
		}
		return false;
	}

	public long GetLeaderUnitUID()
	{
		if (m_LeaderIndex < 0 || !IsValidSlotIndex((byte)m_LeaderIndex))
		{
			return 0L;
		}
		return m_listDeckUnitUID[m_LeaderIndex];
	}

	public bool IsLeaderUnit(long unitUID)
	{
		if (GetLeaderUnitUID() == unitUID)
		{
			return true;
		}
		return false;
	}

	public bool SetUnitUID(byte slotIndex, long unitUID)
	{
		if (!IsValidSlotIndex(slotIndex))
		{
			return false;
		}
		int num = m_listDeckUnitUID.FindIndex((long x) => x == unitUID);
		if (num != -1)
		{
			m_listDeckUnitUID[num] = 0L;
		}
		m_listDeckUnitUID[slotIndex] = unitUID;
		return true;
	}

	public bool SetOperatorUID(long operatorUID)
	{
		m_OperatorUID = operatorUID;
		return true;
	}

	public NKM_DECK_STATE GetState()
	{
		return m_DeckState;
	}

	public void SetState(NKM_DECK_STATE state)
	{
		m_DeckState = state;
	}

	public NKM_ERROR_CODE IsValidState()
	{
		return m_DeckState switch
		{
			NKM_DECK_STATE.DECK_STATE_NORMAL => NKM_ERROR_CODE.NEC_OK, 
			NKM_DECK_STATE.DECK_STATE_WORLDMAP_MISSION => NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_MISSION_DOING, 
			NKM_DECK_STATE.DECK_STATE_WARFARE => NKM_ERROR_CODE.NEC_FAIL_WARFARE_DOING, 
			NKM_DECK_STATE.DECK_STATE_DIVE => NKM_ERROR_CODE.NEC_FAIL_DIVE_DOING, 
			_ => NKM_ERROR_CODE.NEC_FAIL_DECK_STATE_INVALID, 
		};
	}

	public bool IsValidGame(NKM_GAME_TYPE gameType)
	{
		if (NKMGame.IsPVP(gameType))
		{
			return m_DeckState == NKM_DECK_STATE.DECK_STATE_NORMAL;
		}
		switch (gameType)
		{
		case NKM_GAME_TYPE.NGT_INVALID:
		case NKM_GAME_TYPE.NGT_DEV:
		case NKM_GAME_TYPE.NGT_PRACTICE:
		case NKM_GAME_TYPE.NGT_DUNGEON:
		case NKM_GAME_TYPE.NGT_PVP_RANK:
		case NKM_GAME_TYPE.NGT_TUTORIAL:
		case NKM_GAME_TYPE.NGT_RAID:
		case NKM_GAME_TYPE.NGT_ASYNC_PVP:
		case NKM_GAME_TYPE.NGT_RAID_SOLO:
		case NKM_GAME_TYPE.NGT_SHADOW_PALACE:
		case NKM_GAME_TYPE.NGT_PHASE:
		case NKM_GAME_TYPE.NGT_GUILD_DUNGEON_ARENA:
		case NKM_GAME_TYPE.NGT_GUILD_DUNGEON_BOSS:
		case NKM_GAME_TYPE.NGT_PVP_PRIVATE:
		case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
		case NKM_GAME_TYPE.NGT_TRIM:
		case NKM_GAME_TYPE.NGT_GUILD_DUNGEON_BOSS_PRACTICE:
			return m_DeckState == NKM_DECK_STATE.DECK_STATE_NORMAL;
		case NKM_GAME_TYPE.NGT_WARFARE:
			return m_DeckState == NKM_DECK_STATE.DECK_STATE_WARFARE;
		case NKM_GAME_TYPE.NGT_DIVE:
			return m_DeckState == NKM_DECK_STATE.DECK_STATE_DIVE;
		case NKM_GAME_TYPE.NGT_CUTSCENE:
		case NKM_GAME_TYPE.NGT_WORLDMAP:
			return false;
		default:
			return false;
		}
	}

	private bool IsValidSlotIndex(byte slotIndex)
	{
		return slotIndex < m_listDeckUnitUID.Count;
	}

	public IEnumerable<NKMUnitData> GetUnits(NKMArmyData armyData)
	{
		foreach (long item in m_listDeckUnitUID)
		{
			if (item != 0L)
			{
				yield return armyData.GetUnitFromUID(item);
			}
		}
	}

	public bool HasUnitUid(long unitUid, out int index)
	{
		for (int i = 0; i < m_listDeckUnitUID.Count; i++)
		{
			if (m_listDeckUnitUID[i] == unitUid)
			{
				index = i;
				return true;
			}
		}
		index = -1;
		return false;
	}

	public bool CheckAllSlotFilled()
	{
		if (m_ShipUID != 0L)
		{
			return m_listDeckUnitUID.TrueForAll((long e) => e != 0);
		}
		return false;
	}
}
