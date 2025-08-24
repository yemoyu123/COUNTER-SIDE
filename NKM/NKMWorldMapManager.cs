using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.WorldMap;
using Cs.Logging;
using NKC;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public sealed class NKMWorldMapManager
{
	public enum WorldMapLeaderState
	{
		None,
		CityLeader,
		CityLeaderOther
	}

	public const int CITY_MAX_BUILDING_SLOT = 10;

	public const int CITY_MISSION_GROUP_COUNT = 3;

	public const int MAX_MISSION_REWARD_COUNT = 4;

	public const int MISSION_REFRESH_REQUIRE_INFORMATION = 50;

	public const int MINIMUM_START_MISSION_SUCCESS_RATE = 20;

	public const int MAXINUM_SUCCESS_RATE = 70;

	public const int MINIMUM_SUCCESS_RATE = 30;

	public const int CITY_COMMAND_BUILDING_ID = 1;

	public static Dictionary<int, NKMWorldMapCityTemplet> m_dicCityTemplet = null;

	public static Dictionary<int, NKMWorldMapCityExpTemplet> m_dicCityExpTemplet = null;

	public static Dictionary<NKM_CITY_BUILDING_STAT, List<NKMWorldMapBuildingTemplet>> m_dicBuildTempletByStatEnum = null;

	private static readonly List<int> lstCityUnlockLevel = new List<int> { 0, 1, 10, 25, 35, 45, 55 };

	public static bool LoadFromLUA()
	{
		m_dicCityExpTemplet = NKMTempletLoader.LoadDictionary("AB_SCRIPT", "LUA_WORLDMAP_CITY_EXP_TABLE", "m_WorldmapCityExpTable", NKMWorldMapCityExpTemplet.LoadFromLUA);
		int num = 1 & ((m_dicCityExpTemplet != null) ? 1 : 0);
		m_dicCityTemplet = NKMTempletLoader.LoadDictionary("AB_SCRIPT", "LUA_WORLDMAP_CITY_TEMPLET", "m_WorldmapCityTemplet", NKMWorldMapCityTemplet.LoadFromLUA);
		int result = num & ((m_dicCityTemplet != null) ? 1 : 0);
		NKMTempletContainer<NKMWorldMapMissionTemplet>.Load("AB_SCRIPT", "LUA_WORLDMAP_MISSION_TEMPLET", "m_WorldmapMissionTemplet", NKMWorldMapMissionTemplet.LoadFromLUA);
		NKMTempletContainer<NKMWorldMapEventTemplet>.Load("AB_SCRIPT", "LUA_WORLDMAP_EVENT_GROUP", "m_WorldmapEventGroup", NKMWorldMapEventTemplet.LoadFromLUA);
		IEnumerable<NKMWorldMapBuildingTemplet> enumerable = from e in NKMTempletLoader<NKMWorldMapBuildingTemplet.LevelTemplet>.LoadGroup("AB_SCRIPT", "LUA_WORLDMAP_CITY_BUILDING", "m_WorldmapCityBuildingTemplet", NKMWorldMapBuildingTemplet.LevelTemplet.LoadFromLUA)
			select new NKMWorldMapBuildingTemplet(e.Key, e.Value);
		NKMTempletContainer<NKMWorldMapBuildingTemplet>.Load(enumerable, null);
		m_dicBuildTempletByStatEnum = (from e in enumerable
			group e by e.StatType).ToDictionary((IGrouping<NKM_CITY_BUILDING_STAT, NKMWorldMapBuildingTemplet> e) => e.Key, (IGrouping<NKM_CITY_BUILDING_STAT, NKMWorldMapBuildingTemplet> e) => e.ToList());
		return (byte)result != 0;
	}

	public static NKMWorldMapCityTemplet GetCityTemplet(int id)
	{
		if (m_dicCityTemplet.TryGetValue(id, out var value))
		{
			return value;
		}
		return null;
	}

	public static NKMWorldMapCityExpTemplet GetCityExpTable(int level)
	{
		if (m_dicCityExpTemplet.TryGetValue(level, out var value))
		{
			return value;
		}
		return null;
	}

	public static NKMWorldMapMissionTemplet GetMissionTemplet(int id)
	{
		return NKMTempletContainer<NKMWorldMapMissionTemplet>.Find(id);
	}

	public static NKMWorldMapBuildingTemplet.LevelTemplet GetCityBuildingTemplet(int id, int level)
	{
		return NKMWorldMapBuildingTemplet.Find(id)?.GetLevelTemplet(level) ?? null;
	}

	public static int GetPossibleCityCount(int userLevel)
	{
		if (userLevel == 0)
		{
			return 0;
		}
		for (int i = 0; i < lstCityUnlockLevel.Count; i++)
		{
			if (userLevel < lstCityUnlockLevel[i])
			{
				return i - 1;
			}
		}
		return 6;
	}

	public static int GetNextAreaUnlockLevel(int currentCityCount)
	{
		if (currentCityCount < lstCityUnlockLevel.Count - 1)
		{
			return lstCityUnlockLevel[currentCityCount + 1];
		}
		return 99;
	}

	public static int GetTotalBuildingPointUsed(int buildingID, int currentLevel)
	{
		int num = 0;
		for (int i = 1; i <= currentLevel; i++)
		{
			NKMWorldMapBuildingTemplet.LevelTemplet cityBuildingTemplet = GetCityBuildingTemplet(buildingID, i);
			if (cityBuildingTemplet != null)
			{
				num += cityBuildingTemplet.reqBuildingPoint;
			}
		}
		return num;
	}

	public static int GetTotalBuildingPointUsed(NKMWorldMapBuildingTemplet.LevelTemplet levelTemplet)
	{
		return GetTotalBuildingPointUsed(levelTemplet.id, levelTemplet.level);
	}

	public static int GetTotalBuildingPointUsed(NKMWorldmapCityBuildingData buildingData)
	{
		return GetTotalBuildingPointUsed(buildingData.id, buildingData.level);
	}

	public static int GetUsableBuildPoint(NKMWorldMapCityData cityData)
	{
		if (cityData == null)
		{
			return 0;
		}
		int num = 0;
		foreach (KeyValuePair<int, NKMWorldmapCityBuildingData> item in cityData.worldMapCityBuildingDataMap)
		{
			num += GetTotalBuildingPointUsed(item.Value);
		}
		return cityData.level - num;
	}

	public static bool IsMissionLeaderOnly(NKMWorldMapMissionTemplet.WorldMapMissionType missionType)
	{
		return true;
	}

	public static int GetCityOpenCost(NKMWorldMapData worldMapData, bool isCash)
	{
		int unlockedCityCount = worldMapData.GetUnlockedCityCount();
		if (isCash)
		{
			if (unlockedCityCount < NKMConst.Worldmap.CITY_OPEN_CASH_COST.Count)
			{
				return NKMConst.Worldmap.CITY_OPEN_CASH_COST[unlockedCityCount];
			}
		}
		else if (unlockedCityCount < NKMConst.Worldmap.CITY_OPEN_CREDIT_COST.Count)
		{
			return NKMConst.Worldmap.CITY_OPEN_CREDIT_COST[unlockedCityCount];
		}
		return 0;
	}

	public static bool CanLevelup(int lev, int exp, out int need_credit)
	{
		need_credit = 0;
		if (m_dicCityExpTemplet.TryGetValue(lev, out var value) && value.m_ExpRequired != 0 && value.m_ExpRequired <= exp)
		{
			need_credit = value.m_LevelUpReqCredit;
			return true;
		}
		return false;
	}

	public static NKM_ERROR_CODE CanSetLeader(NKMUserData userData, long leaderUID)
	{
		if (leaderUID == 0L)
		{
			return NKM_ERROR_CODE.NEC_OK;
		}
		NKMUnitData unitFromUID = userData.m_ArmyData.GetUnitFromUID(leaderUID);
		if (unitFromUID == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_NOT_EXIST;
		}
		if (unitFromUID.IsSeized)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_IS_SEIZED;
		}
		if (!NKMUnitManager.CanUnitUsedInDeck(unitFromUID))
		{
			return NKM_ERROR_CODE.NEC_FAIL_NPT_DECK_UNIT_NOALLOWED_TYPE;
		}
		foreach (NKMWorldMapCityData value in userData.m_WorldmapData.worldMapCityDataMap.Values)
		{
			if (value.leaderUnitUID == leaderUID)
			{
				return NKM_ERROR_CODE.NEC_FAIL_UNIT_ALREADY_USE;
			}
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static bool IsMissionRunning(NKMWorldMapCityData cityData)
	{
		return cityData?.HasMission() ?? false;
	}

	public static bool IsMissionFinished(NKMWorldMapCityData cityData, DateTime CurrentTime)
	{
		return cityData?.IsMissionFinished(CurrentTime) ?? false;
	}

	public static int GetMissionSuccessRate(NKMWorldMapMissionTemplet missionTemplet, NKMArmyData armyData, NKMWorldMapCityData cityData)
	{
		if (armyData == null || cityData == null)
		{
			return 0;
		}
		NKMUnitData unitFromUID = armyData.GetUnitFromUID(cityData.leaderUnitUID);
		if (unitFromUID == null)
		{
			Log.Error($"Invalid UnitData. leaderUnitUid : {cityData.leaderUnitUID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMWorldMapManager.cs", 406);
			return 0;
		}
		float num = (((float)unitFromUID.m_UnitLevel - (float)missionTemplet.m_ReqManagerLevel) / 100f + 0.3f) * 100f;
		int num2 = (int)cityData.CalcBuildStat(NKM_CITY_BUILDING_STAT.CBS_MISSION_SUCCSSES_RATE, (int)num);
		if (num2 > 70)
		{
			num2 = 70;
		}
		if (num2 < 30)
		{
			num2 = 30;
		}
		return num2;
	}

	public static float GetCityExpPercent(NKMWorldMapCityData cityData)
	{
		if (cityData == null)
		{
			return 0f;
		}
		NKMWorldMapCityExpTemplet cityExpTable = GetCityExpTable(cityData.level);
		if (cityExpTable != null)
		{
			if (cityExpTable.m_ExpRequired == 0)
			{
				return 1f;
			}
			return (float)cityData.exp / (float)cityExpTable.m_ExpRequired;
		}
		Log.Error("City Exp Table not found! city Lv : " + cityData.level, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMWorldMapManager.cs", 446);
		return 0f;
	}

	public static WorldMapLeaderState GetUnitWorldMapLeaderState(NKMUserData userData, long unitUID, int currentCityID = -1)
	{
		if (unitUID == 0L || userData == null)
		{
			return WorldMapLeaderState.None;
		}
		foreach (NKMWorldMapCityData value in userData.m_WorldmapData.worldMapCityDataMap.Values)
		{
			if (value.leaderUnitUID == unitUID)
			{
				if (currentCityID != -1 && currentCityID != value.cityID)
				{
					return WorldMapLeaderState.CityLeaderOther;
				}
				return WorldMapLeaderState.CityLeader;
			}
		}
		return WorldMapLeaderState.None;
	}

	public static int GetUnitWorldMapLeaderCity(NKMUserData userData, long unitUID)
	{
		if (unitUID == 0L || userData == null)
		{
			return -1;
		}
		foreach (NKMWorldMapCityData value in userData.m_WorldmapData.worldMapCityDataMap.Values)
		{
			if (value.leaderUnitUID == unitUID)
			{
				return value.cityID;
			}
		}
		return -1;
	}

	public static NKM_ERROR_CODE CanBuild(NKMUserData userData, int cityID, int buildID)
	{
		if (userData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_USER_DATA_NULL;
		}
		NKMWorldMapCityData cityData = userData.m_WorldmapData.GetCityData(cityID);
		if (cityData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_CITY_ID;
		}
		NKMWorldMapBuildingTemplet nKMWorldMapBuildingTemplet = NKMWorldMapBuildingTemplet.Find(buildID);
		if (nKMWorldMapBuildingTemplet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_BUILD_ID;
		}
		NKMWorldMapBuildingTemplet.LevelTemplet levelTemplet = nKMWorldMapBuildingTemplet.GetLevelTemplet(1);
		if (levelTemplet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_BUILD_LEVEL;
		}
		if (cityData.GetBuildingData(buildID) != null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_BUILD_AREADY_EXIST;
		}
		if (cityData.level < levelTemplet.reqCityLevel)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_BUILD_NOT_ENOUGH_LEVEL;
		}
		if (levelTemplet.reqBuildingID != 0)
		{
			NKMWorldmapCityBuildingData buildingData = cityData.GetBuildingData(levelTemplet.reqBuildingID);
			if (buildingData == null)
			{
				return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_BUILD_NOT_EXIST_REQ_BUILDING;
			}
			if (buildingData.level < levelTemplet.reqBuildingLevel)
			{
				return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_BUILD_NOT_ENOUGH_LEVEL;
			}
		}
		if (levelTemplet.reqClearDiveId != 0 && !NKCScenManager.CurrentUserData().CheckDiveHistory(levelTemplet.reqClearDiveId))
		{
			return NKM_ERROR_CODE.NEC_FAIL_DIVE_NOT_CLEARED;
		}
		if (levelTemplet.notBuildingTogether != 0 && cityData.GetBuildingData(levelTemplet.notBuildingTogether) != null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_NOT_BUILDING_TOGETHER;
		}
		foreach (NKMWorldMapBuildingTemplet.LevelTemplet.CostItem buildCostItem in levelTemplet.BuildCostItems)
		{
			if (!userData.CheckPrice(buildCostItem.Count, buildCostItem.ItemID))
			{
				return NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_CREDIT;
			}
		}
		int reqBuildingPoint = levelTemplet.reqBuildingPoint;
		int usableBuildPoint = GetUsableBuildPoint(cityData);
		if (reqBuildingPoint > usableBuildPoint)
		{
			return NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_BUILD_POINT;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE CanLevelUpBuilding(NKMUserData userData, int cityID, int buildID)
	{
		if (userData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_USER_DATA_NULL;
		}
		NKMWorldMapCityData cityData = userData.m_WorldmapData.GetCityData(cityID);
		if (cityData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_CITY_ID;
		}
		NKMWorldMapBuildingTemplet nKMWorldMapBuildingTemplet = NKMWorldMapBuildingTemplet.Find(buildID);
		if (nKMWorldMapBuildingTemplet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_BUILD_ID;
		}
		NKMWorldmapCityBuildingData buildingData = cityData.GetBuildingData(buildID);
		if (buildingData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_BUILD_NOT_EXIST;
		}
		if (buildingData.level >= 10)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_BUILD_ALREADY_MAX_LEVEL;
		}
		int level = buildingData.level + 1;
		NKMWorldMapBuildingTemplet.LevelTemplet levelTemplet = nKMWorldMapBuildingTemplet.GetLevelTemplet(level);
		if (levelTemplet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_BUILD_LEVEL;
		}
		if (cityData.level < levelTemplet.reqCityLevel)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_BUILD_NOT_ENOUGH_LEVEL;
		}
		if (levelTemplet.reqBuildingID != 0)
		{
			NKMWorldmapCityBuildingData buildingData2 = cityData.GetBuildingData(levelTemplet.reqBuildingID);
			if (buildingData2 == null)
			{
				return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_BUILD_NOT_EXIST_REQ_BUILDING;
			}
			if (buildingData2.level < levelTemplet.reqBuildingLevel)
			{
				return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_BUILD_NOT_ENOUGH_LEVEL;
			}
		}
		if (levelTemplet.reqClearDiveId != 0 && !NKCScenManager.CurrentUserData().CheckDiveHistory(levelTemplet.reqClearDiveId))
		{
			return NKM_ERROR_CODE.NEC_FAIL_DIVE_NOT_CLEARED;
		}
		foreach (NKMWorldMapBuildingTemplet.LevelTemplet.CostItem buildCostItem in levelTemplet.BuildCostItems)
		{
			if (!userData.CheckPrice(buildCostItem.Count, buildCostItem.ItemID))
			{
				return NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_CREDIT;
			}
		}
		int reqBuildingPoint = levelTemplet.reqBuildingPoint;
		int usableBuildPoint = GetUsableBuildPoint(cityData);
		if (reqBuildingPoint > usableBuildPoint)
		{
			return NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_BUILD_POINT;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE CanExpireBuilding(NKMUserData userData, int cityID, int buildID)
	{
		if (userData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_USER_DATA_NULL;
		}
		if (buildID == 1)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_CANNOT_EXPIRE_COMMAND;
		}
		NKMWorldMapCityData cityData = userData.m_WorldmapData.GetCityData(cityID);
		if (cityData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_CITY_ID;
		}
		NKMWorldMapBuildingTemplet nKMWorldMapBuildingTemplet = NKMWorldMapBuildingTemplet.Find(buildID);
		if (nKMWorldMapBuildingTemplet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_BUILD_ID;
		}
		NKMWorldmapCityBuildingData buildingData = cityData.GetBuildingData(buildID);
		if (buildingData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_BUILD_NOT_EXIST;
		}
		NKMWorldMapBuildingTemplet.LevelTemplet levelTemplet = nKMWorldMapBuildingTemplet.GetLevelTemplet(buildingData.level);
		if (levelTemplet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_BUILD_LEVEL;
		}
		if (!userData.CheckPrice(levelTemplet.ClearCostItem.Count, levelTemplet.ClearCostItem.ItemID))
		{
			return NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_CREDIT;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKMWorldMapCityTemplet GetCityTemplet(string strID)
	{
		foreach (NKMWorldMapCityTemplet value in m_dicCityTemplet.Values)
		{
			if (value.m_StrID == strID)
			{
				return value;
			}
		}
		return null;
	}

	public static NKM_ERROR_CODE IsValidDeckForWorldMapMission(NKMUserData userData, NKMDeckIndex selectDeckIndex, int cityID)
	{
		if (userData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_USER_DATA_NULL;
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = NKMMain.IsValidDeck(userData.m_ArmyData, selectDeckIndex);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			return nKM_ERROR_CODE;
		}
		NKMDeckData deckData = userData.m_ArmyData.GetDeckData(selectDeckIndex);
		if (deckData != null)
		{
			foreach (long item in deckData.m_listDeckUnitUID)
			{
				if (GetUnitWorldMapLeaderState(userData, item, cityID) == WorldMapLeaderState.CityLeaderOther)
				{
					return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_MISSION_DECK_HAS_UNIT_FROM_ANOTHER_CITY;
				}
			}
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKMWorldMapEventTemplet GetWorldMapEventTempletByStageID(int stageID)
	{
		return NKMTempletContainer<NKMWorldMapEventTemplet>.Find((NKMWorldMapEventTemplet x) => x.stageID == stageID);
	}
}
