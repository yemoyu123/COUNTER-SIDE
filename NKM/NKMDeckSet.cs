using System;
using System.Collections.Generic;
using Cs.Protocol;

namespace NKM;

public sealed class NKMDeckSet : ISerializable
{
	private NKM_DECK_TYPE type;

	private List<NKMDeckData> decks = new List<NKMDeckData>();

	public int Count => decks.Count;

	public IReadOnlyList<NKMDeckData> Values => decks;

	public NKM_DECK_TYPE DeckType => type;

	public NKMDeckSet()
	{
	}

	public NKMDeckSet(NKM_DECK_TYPE type)
	{
		this.type = type;
		if (this.type == NKM_DECK_TYPE.NDT_RAID)
		{
			for (int i = 0; i < NKMCommonConst.Deck.DefaultRaidDeckCount; i++)
			{
				decks.Add(new NKMDeckData(this.type));
			}
		}
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref type);
		stream.PutOrGet(ref decks);
	}

	public void AddDeck(NKMDeckData deck)
	{
		decks.Add(deck);
	}

	public void SetDeck(int index, NKMDeckData deck)
	{
		decks[index] = deck;
	}

	public bool FindDeckByUnitUid(long unitUid, out NKMDeckData result)
	{
		foreach (NKMDeckData deck in decks)
		{
			if (deck != null && deck.HasUnitUid(unitUid, out var _))
			{
				result = deck;
				return true;
			}
		}
		result = null;
		return false;
	}

	public bool FindDeckByShipUid(long shipUid, out NKMDeckData result)
	{
		foreach (NKMDeckData deck in decks)
		{
			if (deck != null && deck.m_ShipUID == shipUid)
			{
				result = deck;
				return true;
			}
		}
		result = null;
		return false;
	}

	public bool FindDeckByOperatporUid(long operatorUID)
	{
		foreach (NKMDeckData deck in decks)
		{
			if (deck != null && deck.m_OperatorUID == operatorUID)
			{
				return true;
			}
		}
		return false;
	}

	public bool FindDeckIndexByUnitUid(long unitUid, out NKMDeckIndex result)
	{
		for (int i = 0; i < decks.Count; i++)
		{
			NKMDeckData nKMDeckData = decks[i];
			if (nKMDeckData != null && nKMDeckData.HasUnitUid(unitUid, out var _))
			{
				result = new NKMDeckIndex(type, i);
				return true;
			}
		}
		result = NKMDeckIndex.None;
		return false;
	}

	public bool FindDeckIndexByUnitUid(long unitUid, out NKMDeckIndex deckIndex, out sbyte unitSlotIndex)
	{
		for (int i = 0; i < decks.Count; i++)
		{
			NKMDeckData nKMDeckData = decks[i];
			if (nKMDeckData != null && nKMDeckData.HasUnitUid(unitUid, out var index))
			{
				deckIndex = new NKMDeckIndex(type, i);
				unitSlotIndex = (sbyte)index;
				return true;
			}
		}
		deckIndex = default(NKMDeckIndex);
		unitSlotIndex = 0;
		return false;
	}

	public bool FindDeckIndexByShipUid(long shipUid, out NKMDeckIndex result)
	{
		for (int i = 0; i < decks.Count; i++)
		{
			NKMDeckData nKMDeckData = decks[i];
			if (nKMDeckData != null && nKMDeckData.m_ShipUID == shipUid)
			{
				result = new NKMDeckIndex(type, i);
				return true;
			}
		}
		result = default(NKMDeckIndex);
		return false;
	}

	public bool FindDeckIndexByOperatorUid(long operatorUid, out NKMDeckIndex result)
	{
		for (int i = 0; i < decks.Count; i++)
		{
			NKMDeckData nKMDeckData = decks[i];
			if (nKMDeckData != null && nKMDeckData.m_OperatorUID == operatorUid)
			{
				result = new NKMDeckIndex(type, i);
				return true;
			}
		}
		result = default(NKMDeckIndex);
		return false;
	}

	public int GetAvailableDeckIndex(NKMArmyData armyData)
	{
		for (int i = 0; i < decks.Count; i++)
		{
			NKMDeckData nKMDeckData = decks[i];
			if (nKMDeckData != null && !NKMMain.IsBusyDeck(nKMDeckData) && NKMMain.IsValidDeck(armyData, type, (byte)i) == NKM_ERROR_CODE.NEC_OK)
			{
				return i;
			}
		}
		return -1;
	}

	public void Clear()
	{
		if (type != NKM_DECK_TYPE.NDT_FRIEND)
		{
			throw new Exception($"[NKMDeckSet] only friend deck can clear. DeckType:{type}");
		}
		decks.Clear();
	}

	public void ClearDeck()
	{
		foreach (NKMDeckData deck in decks)
		{
			deck.m_ShipUID = 0L;
			deck.m_OperatorUID = 0L;
			for (int i = 0; i < deck.m_listDeckUnitUID.Count; i++)
			{
				deck.m_listDeckUnitUID[i] = 0L;
			}
			deck.m_LeaderIndex = -1;
		}
	}
}
