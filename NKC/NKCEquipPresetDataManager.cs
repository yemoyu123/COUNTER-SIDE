using System.Collections.Generic;
using ClientPacket.Item;
using NKM;
using UnityEngine;

namespace NKC;

public static class NKCEquipPresetDataManager
{
	public static bool OpenUI;

	private static List<NKMEquipPresetData> m_listEquipPresetData;

	private static HashSet<long> m_hsPresetEquipUId = new HashSet<long>();

	private static List<NKMEquipPresetData> m_listTempPresetData = new List<NKMEquipPresetData>();

	private static List<int> m_lstRemoveSlotTarget = new List<int>();

	public static List<NKMEquipPresetData> ListEquipPresetData
	{
		get
		{
			return m_listEquipPresetData;
		}
		set
		{
			m_listEquipPresetData?.Clear();
			m_listEquipPresetData = value;
			m_listEquipPresetData?.Sort(delegate(NKMEquipPresetData e1, NKMEquipPresetData e2)
			{
				if (e1.presetIndex > e2.presetIndex)
				{
					return 1;
				}
				return (e1.presetIndex < e2.presetIndex) ? (-1) : 0;
			});
		}
	}

	public static HashSet<long> HSPresetEquipUId => m_hsPresetEquipUId;

	public static List<NKMEquipPresetData> ListTempPresetData => m_listTempPresetData;

	public static void RequestPresetData(bool _openUI)
	{
		ListEquipPresetData = null;
		m_hsPresetEquipUId.Clear();
		OpenUI = _openUI;
		NKCPacketSender.Send_NKMPacket_EQUIP_PRESET_LIST_REQ(_openUI ? NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL : NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
	}

	public static bool HasData()
	{
		if (ListEquipPresetData != null && ListEquipPresetData.Count > 0)
		{
			return true;
		}
		return false;
	}

	public static void RefreshEquipUidHash()
	{
		m_hsPresetEquipUId.Clear();
		if (m_listEquipPresetData == null)
		{
			return;
		}
		m_listEquipPresetData.ForEach(delegate(NKMEquipPresetData e)
		{
			int count = e.equipUids.Count;
			for (int i = 0; i < count; i++)
			{
				if (e.equipUids[i] > 0)
				{
					m_hsPresetEquipUId.Add(e.equipUids[i]);
				}
			}
		});
	}

	public static List<NKMEquipPresetData> GetPresetDataListForPage(int page, int maxSlotCountPerPage, bool slotMoveState)
	{
		List<NKMEquipPresetData> list = new List<NKMEquipPresetData>();
		List<NKMEquipPresetData> list2 = (slotMoveState ? m_listTempPresetData : m_listEquipPresetData);
		if (list2 == null)
		{
			return list;
		}
		int num = page * maxSlotCountPerPage;
		if (num >= list2.Count)
		{
			return list;
		}
		int num2 = Mathf.Min((page + 1) * maxSlotCountPerPage, list2.Count);
		for (int i = num; i < num2; i++)
		{
			list.Add(list2[i]);
		}
		return list;
	}

	public static bool IsLastPage(int page, int maxSlotCountPerPage)
	{
		if (m_listEquipPresetData == null)
		{
			return true;
		}
		int num = (page + 1) * maxSlotCountPerPage - 1;
		if (num >= m_listEquipPresetData.Count || num >= NKMCommonConst.EQUIP_PRESET_MAX_COUNT - 1)
		{
			return true;
		}
		return false;
	}

	public static void SwapTempPresetData(int index1, int index2)
	{
		int count = m_listTempPresetData.Count;
		if (index1 >= 0 && index2 >= 0 && index1 != index2 && count > index1 && count > index2)
		{
			NKMEquipPresetData value = m_listTempPresetData[index1];
			m_listTempPresetData[index1] = m_listTempPresetData[index2];
			m_listTempPresetData[index2] = value;
		}
	}

	public static int GetTempPresetDataIndex(NKMEquipPresetData presetData)
	{
		if (presetData == null)
		{
			return -1;
		}
		return m_listTempPresetData.IndexOf(presetData);
	}

	public static List<NKMPacket_EQUIP_PRESET_CHANGE_INDEX_REQ.PresetIndexData> GetMovedSlotIndexList()
	{
		List<NKMPacket_EQUIP_PRESET_CHANGE_INDEX_REQ.PresetIndexData> list = new List<NKMPacket_EQUIP_PRESET_CHANGE_INDEX_REQ.PresetIndexData>();
		int count = m_listTempPresetData.Count;
		for (int i = 0; i < count; i++)
		{
			if (m_listTempPresetData[i] != null && i != m_listTempPresetData[i].presetIndex)
			{
				NKMPacket_EQUIP_PRESET_CHANGE_INDEX_REQ.PresetIndexData presetIndexData = new NKMPacket_EQUIP_PRESET_CHANGE_INDEX_REQ.PresetIndexData();
				presetIndexData.beforeIndex = m_listTempPresetData[i].presetIndex;
				presetIndexData.afterIndex = i;
				list.Add(presetIndexData);
			}
		}
		return list;
	}

	public static void UpdateTempPresetSlotData(NKMEquipPresetData equipPresetData)
	{
		int num = m_listTempPresetData.FindIndex((NKMEquipPresetData e) => e.presetIndex == equipPresetData.presetIndex);
		if (num >= 0 && num < m_listTempPresetData.Count)
		{
			m_listTempPresetData[num] = equipPresetData;
		}
	}

	public static List<int> GetRemoveTargetList()
	{
		return m_lstRemoveSlotTarget;
	}

	public static void ResetRemoveTargetIndexList()
	{
		m_lstRemoveSlotTarget.Clear();
	}

	public static void AddRemoveTargetIndex(int idx)
	{
		m_lstRemoveSlotTarget.Add(idx);
	}

	public static void RemoveTargetIndex(int idx)
	{
		if (m_lstRemoveSlotTarget.Contains(idx))
		{
			m_lstRemoveSlotTarget.Remove(idx);
		}
	}
}
