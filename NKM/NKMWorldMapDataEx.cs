using System.Collections.Generic;
using ClientPacket.WorldMap;
using NKM.Templet;

namespace NKM;

public static class NKMWorldMapDataEx
{
	public static NKM_ERROR_CODE CanOpenCity(this NKMWorldMapData data, NKMWorldMapCityTemplet cityTemplet, NKMUserData userData, bool bCash)
	{
		if (cityTemplet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_CITY_ID;
		}
		if (data.IsCityUnlocked(cityTemplet.m_ID))
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_CITY_ALREADY_OPENED;
		}
		int num = 0;
		long num2 = 0L;
		if (!bCash)
		{
			int unlockedCityCount = data.GetUnlockedCityCount();
			if (NKMWorldMapManager.GetPossibleCityCount(userData.m_UserLevel) <= unlockedCityCount)
			{
				return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_FULL_AREA;
			}
			num = NKMWorldMapManager.GetCityOpenCost(data, isCash: false);
			num2 = userData.GetCredit();
		}
		else
		{
			num = NKMWorldMapManager.GetCityOpenCost(data, isCash: true);
			num2 = userData.GetCash();
		}
		if (num2 < num)
		{
			if (bCash)
			{
				return NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_CASH;
			}
			return NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_CREDIT;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static bool CheckOpenCount(this NKMWorldMapData data, int lev)
	{
		int possibleCityCount = NKMWorldMapManager.GetPossibleCityCount(lev);
		if (data.worldMapCityDataMap.Count < possibleCityCount)
		{
			return true;
		}
		return false;
	}

	public static NKMWorldMapCityData GetCityData(this NKMWorldMapData data, int cityID)
	{
		if (data.worldMapCityDataMap.TryGetValue(cityID, out var value))
		{
			return value;
		}
		return null;
	}

	public static int GetCityID(this NKMWorldMapData data, long diveUid)
	{
		foreach (NKMWorldMapCityData value in data.worldMapCityDataMap.Values)
		{
			NKMWorldMapEventTemplet nKMWorldMapEventTemplet = NKMWorldMapEventTemplet.Find(value.worldMapEventGroup.worldmapEventID);
			if (nKMWorldMapEventTemplet != null && nKMWorldMapEventTemplet.eventType == NKM_WORLDMAP_EVENT_TYPE.WET_DIVE && value.worldMapEventGroup.eventUid == diveUid)
			{
				return value.cityID;
			}
		}
		return 0;
	}

	public static int GetUnlockedCityCount(this NKMWorldMapData data)
	{
		return data.worldMapCityDataMap.Count;
	}

	public static bool IsCityUnlocked(this NKMWorldMapData data, int CityID)
	{
		return data.worldMapCityDataMap.ContainsKey(CityID);
	}

	public static int GetCityIDByEventData(this NKMWorldMapData data, NKM_WORLDMAP_EVENT_TYPE eventType, long eventUID)
	{
		foreach (KeyValuePair<int, NKMWorldMapCityData> item in data.worldMapCityDataMap)
		{
			if (item.Value.worldMapEventGroup != null && item.Value.worldMapEventGroup.eventUid == eventUID)
			{
				NKMWorldMapEventTemplet nKMWorldMapEventTemplet = NKMWorldMapEventTemplet.Find(item.Value.worldMapEventGroup.worldmapEventID);
				if (nKMWorldMapEventTemplet != null && nKMWorldMapEventTemplet.eventType == eventType)
				{
					return item.Key;
				}
			}
		}
		return -1;
	}

	public static int GetStartedEventCount(this NKMWorldMapData data, NKM_WORLDMAP_EVENT_TYPE eventType)
	{
		int num = 0;
		foreach (KeyValuePair<int, NKMWorldMapCityData> item in data.worldMapCityDataMap)
		{
			if (item.Value.worldMapEventGroup != null)
			{
				NKMWorldMapEventTemplet nKMWorldMapEventTemplet = NKMWorldMapEventTemplet.Find(item.Value.worldMapEventGroup.worldmapEventID);
				if (nKMWorldMapEventTemplet != null && nKMWorldMapEventTemplet.eventType == eventType && item.Value.worldMapEventGroup.eventUid > 0)
				{
					num++;
				}
			}
		}
		return num;
	}

	public static int GetStartedEventCityID(this NKMWorldMapData data, NKM_WORLDMAP_EVENT_TYPE eventType, int targetIndex)
	{
		int num = 0;
		foreach (KeyValuePair<int, NKMWorldMapCityData> item in data.worldMapCityDataMap)
		{
			if (item.Value.worldMapEventGroup == null)
			{
				continue;
			}
			NKMWorldMapEventTemplet nKMWorldMapEventTemplet = NKMWorldMapEventTemplet.Find(item.Value.worldMapEventGroup.worldmapEventID);
			if (nKMWorldMapEventTemplet != null && nKMWorldMapEventTemplet.eventType == eventType && item.Value.worldMapEventGroup.eventUid > 0)
			{
				if (num == targetIndex)
				{
					return item.Key;
				}
				num++;
			}
		}
		return -1;
	}

	public static bool CheckIfHaveSpecificEvent(this NKMWorldMapData data, NKM_WORLDMAP_EVENT_TYPE eventType)
	{
		if (data == null)
		{
			return false;
		}
		foreach (KeyValuePair<int, NKMWorldMapCityData> item in data.worldMapCityDataMap)
		{
			NKMWorldMapCityData value = item.Value;
			if (value != null && value.worldMapEventGroup != null)
			{
				NKMWorldMapEventTemplet nKMWorldMapEventTemplet = NKMWorldMapEventTemplet.Find(value.worldMapEventGroup.worldmapEventID);
				if (nKMWorldMapEventTemplet != null && nKMWorldMapEventTemplet.eventType == eventType)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool RemoveEvent(this NKMWorldMapData data, NKM_WORLDMAP_EVENT_TYPE eventType, long eventUID, out int cityID)
	{
		cityID = -1;
		if (data == null)
		{
			return false;
		}
		foreach (KeyValuePair<int, NKMWorldMapCityData> item in data.worldMapCityDataMap)
		{
			NKMWorldMapCityData value = item.Value;
			if (value != null && value.worldMapEventGroup != null)
			{
				NKMWorldMapEventTemplet nKMWorldMapEventTemplet = NKMWorldMapEventTemplet.Find(value.worldMapEventGroup.worldmapEventID);
				if (nKMWorldMapEventTemplet != null && nKMWorldMapEventTemplet.eventType == eventType && value.worldMapEventGroup.eventUid == eventUID)
				{
					cityID = value.cityID;
					value.worldMapEventGroup.Clear();
					return true;
				}
			}
		}
		return false;
	}

	public static bool ClearEvent(this NKMWorldMapData data, int cityID)
	{
		if (data == null)
		{
			return false;
		}
		foreach (KeyValuePair<int, NKMWorldMapCityData> item in data.worldMapCityDataMap)
		{
			NKMWorldMapCityData value = item.Value;
			if (value != null && value.worldMapEventGroup != null && value.cityID == cityID)
			{
				value.worldMapEventGroup.Clear();
				return true;
			}
		}
		return false;
	}
}
