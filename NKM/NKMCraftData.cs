using System;
using System.Collections.Generic;
using Cs.Logging;
using Cs.Protocol;

namespace NKM;

public class NKMCraftData : ISerializable
{
	public static int MAX_CRAFT_SLOT_DATA = 5;

	private Dictionary<int, NKMMoldItemData> m_dicMoldItem = new Dictionary<int, NKMMoldItemData>();

	private Dictionary<byte, NKMCraftSlotData> m_dicSlot = new Dictionary<byte, NKMCraftSlotData>();

	public Dictionary<byte, NKMCraftSlotData> SlotList => m_dicSlot;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_dicMoldItem);
		stream.PutOrGet(ref m_dicSlot);
	}

	public Dictionary<int, NKMMoldItemData> GetDicMoldItemData()
	{
		return m_dicMoldItem;
	}

	public NKMMoldItemData GetMoldItemDataByID(int id)
	{
		NKMMoldItemData value = null;
		if (!m_dicMoldItem.TryGetValue(id, out value))
		{
			return null;
		}
		return value;
	}

	public NKMMoldItemData GetMoldItemDataByIndex(int index)
	{
		if (index < 0 || index >= GetTotalMoldCount())
		{
			return null;
		}
		int num = 0;
		foreach (KeyValuePair<int, NKMMoldItemData> item in m_dicMoldItem)
		{
			if (num == index)
			{
				return item.Value;
			}
			num++;
		}
		return null;
	}

	public void AddMoldItem(List<NKMMoldItemData> moldItemDataList)
	{
		foreach (NKMMoldItemData moldItemData in moldItemDataList)
		{
			AddMoldItem(moldItemData.m_MoldID, moldItemData.m_Count);
		}
	}

	public void AddMoldItem(NKMMoldItemData moldItemData)
	{
		AddMoldItem(moldItemData.m_MoldID, moldItemData.m_Count);
	}

	public void AddMoldItem(int moldID, long count)
	{
		if (m_dicMoldItem.TryGetValue(moldID, out var value))
		{
			value.m_Count = m_dicMoldItem[moldID].m_Count + count;
		}
		else
		{
			m_dicMoldItem.Add(moldID, new NKMMoldItemData(moldID, count));
		}
	}

	public void DecMoldItem(NKMMoldItemData moldItemData)
	{
		DecMoldItem(moldItemData.m_MoldID, moldItemData.m_Count);
	}

	public void DecMoldItem(int moldID, long count)
	{
		if (m_dicMoldItem.TryGetValue(moldID, out var value))
		{
			value.m_Count = Math.Max(0L, m_dicMoldItem[moldID].m_Count - count);
		}
	}

	public void UpdateMoldItem(List<NKMMoldItemData> moldItemDataList)
	{
		foreach (NKMMoldItemData moldItemData in moldItemDataList)
		{
			UpdateMoldItem(moldItemData.m_MoldID, moldItemData.m_Count);
		}
	}

	public void UpdateMoldItem(int moldID, long count)
	{
		if (m_dicMoldItem.TryGetValue(moldID, out var value))
		{
			value.m_Count = count;
		}
		else
		{
			m_dicMoldItem.Add(moldID, new NKMMoldItemData(moldID, count));
		}
	}

	public long GetMoldCount(int moldID)
	{
		long result = 0L;
		if (m_dicMoldItem.ContainsKey(moldID))
		{
			result = m_dicMoldItem[moldID].m_Count;
		}
		return result;
	}

	public int GetTotalMoldCount()
	{
		return m_dicMoldItem.Count;
	}

	public void AddSlotData(NKMCraftSlotData slotData)
	{
		if (m_dicSlot.ContainsKey(slotData.Index))
		{
			Log.Error($"CraftSlot AddSlotData Failed! slot already exists! index : {slotData.Index}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCraftManager.cs", 201);
		}
		else
		{
			m_dicSlot.Add(slotData.Index, slotData);
		}
	}

	public void AddSlotData(byte index, int moldID, int count, long completeDate)
	{
		if (m_dicSlot.ContainsKey(index))
		{
			Log.Error($"CraftSlot AddSlotData Failed! slot already exists! index : {index}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCraftManager.cs", 212);
			return;
		}
		NKMCraftSlotData nKMCraftSlotData = new NKMCraftSlotData();
		nKMCraftSlotData.Index = index;
		nKMCraftSlotData.MoldID = moldID;
		nKMCraftSlotData.Count = count;
		nKMCraftSlotData.CompleteDate = completeDate;
		m_dicSlot.Add(nKMCraftSlotData.Index, nKMCraftSlotData);
	}

	public NKM_ERROR_CODE UpdateSlotData(NKMCraftSlotData slotData)
	{
		return UpdateSlotData(slotData.Index, slotData.MoldID, slotData.Count, slotData.CompleteDate);
	}

	public NKM_ERROR_CODE UpdateSlotData(byte index, int moldID, int count, long completeDate)
	{
		if (index <= 0 || MAX_CRAFT_SLOT_DATA < index)
		{
			return NKM_ERROR_CODE.NEC_FAIL_CRAFT_INVALID_SLOT_INDEX;
		}
		if (!m_dicSlot.TryGetValue(index, out var value))
		{
			return NKM_ERROR_CODE.NEC_FAIL_CRAFT_INVALID_SLOT_INDEX;
		}
		value.Index = index;
		value.MoldID = moldID;
		value.Count = count;
		value.CompleteDate = completeDate;
		return NKM_ERROR_CODE.NEC_OK;
	}

	public NKMCraftSlotData GetSlotData(byte index)
	{
		NKMCraftSlotData value = null;
		if (!m_dicSlot.TryGetValue(index, out value))
		{
			return null;
		}
		return value;
	}

	public int GetReservedMoldCount()
	{
		int num = 0;
		foreach (NKMCraftSlotData value in m_dicSlot.Values)
		{
			if (value.MoldID > 0)
			{
				num += value.Count;
			}
		}
		return num;
	}

	public int GetEmptyMoldSlotCount()
	{
		int num = 0;
		foreach (NKMCraftSlotData value in m_dicSlot.Values)
		{
			if (value.MoldID == 0)
			{
				num++;
			}
		}
		return num;
	}

	public int GetFirstEmptySlotIndex()
	{
		foreach (NKMCraftSlotData value in m_dicSlot.Values)
		{
			if (value.MoldID == 0)
			{
				return value.Index;
			}
		}
		return -1;
	}

	public int GetUsedMoldSlotCount()
	{
		int num = 0;
		foreach (NKMCraftSlotData value in m_dicSlot.Values)
		{
			if (value.MoldID != 0)
			{
				num++;
			}
		}
		return num;
	}

	public void ClearMoldItem()
	{
		m_dicMoldItem.Clear();
	}
}
