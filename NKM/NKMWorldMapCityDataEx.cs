using System;
using System.Collections.Generic;
using ClientPacket.WorldMap;
using Cs.Logging;
using NKM.Templet;

namespace NKM;

public static class NKMWorldMapCityDataEx
{
	public const int BUILD_MAX_COUNT = 10;

	public static void AddBuild(this NKMWorldMapCityData cityData, NKMWorldmapCityBuildingData data)
	{
		cityData.worldMapCityBuildingDataMap.Add(data.id, data);
	}

	public static float CalcBuildStat(this NKMWorldMapCityData data, NKM_CITY_BUILDING_STAT buildingStat, float value)
	{
		switch (buildingStat)
		{
		case NKM_CITY_BUILDING_STAT.CBS_MISSION_CITY_EXP_RATE:
		case NKM_CITY_BUILDING_STAT.CBS_MISSION_UNIT_EXP_RATE:
		case NKM_CITY_BUILDING_STAT.CBS_MISSION_CREDIT_RATE:
		case NKM_CITY_BUILDING_STAT.CBS_MISSION_ETERNIUM_RATE:
		case NKM_CITY_BUILDING_STAT.CBS_MISSION_INFORMATION_RATE:
		case NKM_CITY_BUILDING_STAT.CBS_MISSION_TIME_REDUCE_RATE:
		case NKM_CITY_BUILDING_STAT.CBS_MISSION_RANK_SEARCH_RATE:
		case NKM_CITY_BUILDING_STAT.CBS_DIVE_INFORMATION_REDUCE_RATE:
		case NKM_CITY_BUILDING_STAT.CBS_RAID_DEFENCE_COST_REDUCE_RATE:
		{
			int num = 0;
			if (!NKMWorldMapManager.m_dicBuildTempletByStatEnum.TryGetValue(buildingStat, out var value3))
			{
				return 0f;
			}
			foreach (NKMWorldMapBuildingTemplet item in value3)
			{
				NKMWorldmapCityBuildingData buildingData2 = data.GetBuildingData(item.Key);
				if (buildingData2 != null)
				{
					NKMWorldMapBuildingTemplet.LevelTemplet levelTemplet2 = item.GetLevelTemplet(buildingData2.level);
					if (levelTemplet2 == null)
					{
						Log.Error($"city level templet not found. cityId:{data.cityID} build id : {item.Key}, level:{buildingData2.level}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMWorldMap.cs", 48);
						return 0f;
					}
					num += levelTemplet2.cityStatValue;
				}
			}
			return value * (float)num / 100f;
		}
		case NKM_CITY_BUILDING_STAT.CBS_MISSION_SUCCSSES_RATE:
		case NKM_CITY_BUILDING_STAT.CBS_EVENT_SPECIAL_SEARCH_RATE:
		case NKM_CITY_BUILDING_STAT.CBS_DIVE_SEARCH_RATE:
		case NKM_CITY_BUILDING_STAT.CBS_RAID_DEFENCE_LEVEL:
		case NKM_CITY_BUILDING_STAT.CBS_RAID_SEARCH_RATE:
		{
			if (!NKMWorldMapManager.m_dicBuildTempletByStatEnum.TryGetValue(buildingStat, out var value2))
			{
				return value;
			}
			{
				foreach (NKMWorldMapBuildingTemplet item2 in value2)
				{
					NKMWorldmapCityBuildingData buildingData = data.GetBuildingData(item2.Key);
					if (buildingData != null)
					{
						NKMWorldMapBuildingTemplet.LevelTemplet levelTemplet = item2.GetLevelTemplet(buildingData.level);
						if (levelTemplet == null)
						{
							Log.Error($"city level templet not found. cityId:{data.cityID} build id : {item2.Key}, level:{buildingData.level}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMWorldMap.cs", 79);
							return 0f;
						}
						value += (float)levelTemplet.cityStatValue;
					}
				}
				return value;
			}
		}
		default:
			throw new Exception("Invalid Stat.");
		}
	}

	public static int GetBuildStatRewardRate(this NKMWorldMapCityData data, NKM_CITY_BUILDING_STAT buildingStat)
	{
		if ((uint)(buildingStat - 1) <= 3u)
		{
			int num = 0;
			if (!NKMWorldMapManager.m_dicBuildTempletByStatEnum.TryGetValue(buildingStat, out var value))
			{
				return 0;
			}
			{
				foreach (NKMWorldMapBuildingTemplet item in value)
				{
					NKMWorldmapCityBuildingData buildingData = data.GetBuildingData(item.Key);
					if (buildingData != null)
					{
						NKMWorldMapBuildingTemplet.LevelTemplet levelTemplet = item.GetLevelTemplet(buildingData.level);
						if (levelTemplet == null)
						{
							Log.Error($"city level templet not found. cityId:{data.cityID} build id : {item.Key}, level:{buildingData.level}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMWorldMap.cs", 118);
							return 0;
						}
						num += levelTemplet.cityStatValue;
					}
				}
				return num;
			}
		}
		throw new Exception("Invalid Stat.");
	}

	public static int CalculateBuildPointLeft(this NKMWorldMapCityData data)
	{
		int num = 0;
		foreach (KeyValuePair<int, NKMWorldmapCityBuildingData> item in data.worldMapCityBuildingDataMap)
		{
			num += item.Value.level;
		}
		return data.level - num;
	}

	public static NKM_ERROR_CODE CanCancelMission(this NKMWorldMapCityData data, int missionID)
	{
		return data.worldMapMission.CanCancelMission(missionID);
	}

	public static NKM_ERROR_CODE CanLevelupCity(this NKMWorldMapCityData data, NKMWorldMapCityTemplet cityTemplet, NKMUserData userData)
	{
		if (cityTemplet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_CITY_ID;
		}
		if (data.worldMapMission != null && data.worldMapMission.currentMissionID != 0)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_CITY_LEVELUP_FAIL_MISSION;
		}
		if (!NKMWorldMapManager.CanLevelup(data.level, data.exp, out var need_credit))
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_CITY_LEVELUP_FAIL;
		}
		if (userData.GetCredit() < need_credit)
		{
			return NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_CREDIT;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE CanStartMission(this NKMWorldMapCityData data, NKMUserData userData, int missionID, NKMDeckIndex deckIndex)
	{
		if (data.leaderUnitUID == 0L)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_MISSION_DO_NOT_SET_LEADER;
		}
		NKMUnitData unitFromUID = userData.m_ArmyData.GetUnitFromUID(data.leaderUnitUID);
		if (unitFromUID == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_MISSION_DO_NOT_SET_LEADER;
		}
		return data.worldMapMission.CanStartMission(userData, missionID, deckIndex, unitFromUID);
	}

	public static NKMWorldmapCityBuildingData GetBuildingData(this NKMWorldMapCityData data, int buildID)
	{
		data.worldMapCityBuildingDataMap.TryGetValue(buildID, out var value);
		return value;
	}

	public static bool HasMission(this NKMWorldMapCityData data)
	{
		if (data.worldMapMission != null)
		{
			return data.worldMapMission.currentMissionID != 0;
		}
		return false;
	}

	public static bool IsMissionFinished(this NKMWorldMapCityData data, DateTime CurrentTime)
	{
		if (data.worldMapMission.currentMissionID != 0)
		{
			return data.worldMapMission.completeTime < CurrentTime.Ticks;
		}
		return false;
	}

	public static void Levelup(this NKMWorldMapCityData data)
	{
		data.exp = 0;
		data.level++;
	}

	public static void RemoveBuild(this NKMWorldMapCityData cityData, int buildID)
	{
		cityData.worldMapCityBuildingDataMap.Remove(buildID);
	}

	public static NKM_ERROR_CODE UpdateBuildingData(this NKMWorldMapCityData data, NKMWorldmapCityBuildingData newData)
	{
		if (newData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_BUILD_ID;
		}
		if (!data.worldMapCityBuildingDataMap.ContainsKey(newData.id))
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_BUILD_ID;
		}
		data.worldMapCityBuildingDataMap[newData.id] = newData;
		return NKM_ERROR_CODE.NEC_OK;
	}
}
