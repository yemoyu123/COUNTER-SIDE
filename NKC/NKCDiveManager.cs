using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.WorldMap;
using Cs.Logging;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKC;

public class NKCDiveManager
{
	public const int MAX_SQUAD_COUNT = 5;

	public const int NO_SUPPLY_DISADVANTAGE_RATE = 100;

	public const int MAX_RESERVED_ARTIFACT_COUNT = 3;

	public static int BeginIndex { get; private set; }

	public static int EndIndex { get; private set; }

	public static IReadOnlyList<NKMDiveTemplet> SortedTemplates { get; private set; }

	public static NKM_ERROR_CODE CanStart(int cityID, int stageID, List<int> deckIndexes, NKMUserData userData, DateTime curTimeUTC, bool bJump)
	{
		NKMDiveTemplet nKMDiveTemplet = NKMDiveTemplet.Find(stageID);
		if (nKMDiveTemplet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_DIVE_INVALID_STAGE_ID;
		}
		if (!nKMDiveTemplet.IsEventDive && userData.CheckDiveClear(stageID))
		{
			return NKM_ERROR_CODE.NEC_FAIL_DIVE_ALREADY_CLEARED;
		}
		if (userData.m_DiveGameData != null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_DIVE_ALREADY_STARTED;
		}
		if (deckIndexes.Count <= 0)
		{
			return NKM_ERROR_CODE.NEC_FAIL_DIVE_NOT_ENOUGH_SQUAD_COUNT;
		}
		if (!NKMContentUnlockManager.IsContentUnlocked(userData, new UnlockInfo(nKMDiveTemplet.StageUnlockReqType, nKMDiveTemplet.StageUnlockReqValue)))
		{
			return NKM_ERROR_CODE.NEC_FAIL_DIVE_LOCKED_STAGE;
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = NKM_ERROR_CODE.NEC_OK;
		for (int i = 0; i < deckIndexes.Count; i++)
		{
			nKM_ERROR_CODE = NKMMain.IsValidDeck(userData.m_ArmyData, NKM_DECK_TYPE.NDT_DIVE, (byte)deckIndexes[i]);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				return nKM_ERROR_CODE;
			}
		}
		NKMWorldMapCityData cityData = userData.m_WorldmapData.GetCityData(cityID);
		if (cityData != null)
		{
			if (cityData.worldMapEventGroup.worldmapEventID == 0)
			{
				return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_EVENT_GROUP_ID;
			}
			if (cityData.worldMapEventGroup.eventGroupEndDate < curTimeUTC)
			{
				return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_EXPIRE_EVENT;
			}
			NKMWorldMapEventTemplet nKMWorldMapEventTemplet = NKMWorldMapEventTemplet.Find(cityData.worldMapEventGroup.worldmapEventID);
			if (nKMWorldMapEventTemplet == null)
			{
				Log.Error($"Invalid Templet City ID. CityID : {cityData.cityID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCDiveManager.cs", 81);
				return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_CITY_ID;
			}
			if (nKMWorldMapEventTemplet.eventType != NKM_WORLDMAP_EVENT_TYPE.WET_DIVE || nKMWorldMapEventTemplet.stageID != stageID)
			{
				return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_EVENT_GROUP_ID;
			}
		}
		if (bJump && !nKMDiveTemplet.IsEventDive)
		{
			return NKM_ERROR_CODE.NEC_FAIL_DIVE_INVALID_STAGE_ID;
		}
		int diveCost = GetDiveCost(bDiveCleared: false, nKMDiveTemplet, cityID, bJump);
		if (!userData.CheckPrice(diveCost, nKMDiveTemplet.StageReqItemId))
		{
			return NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_ITEM;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKMDiveTemplet GetCurrNormalDiveTemplet(out int selectedIndex)
	{
		selectedIndex = -1;
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return null;
		}
		NKMDiveTemplet nKMDiveTemplet = null;
		List<NKMDiveTemplet> list = new List<NKMDiveTemplet>();
		foreach (NKMDiveTemplet sortedTemplate in SortedTemplates)
		{
			if (!sortedTemplate.IsEventDive)
			{
				list.Add(sortedTemplate);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			NKMDiveTemplet nKMDiveTemplet2 = list[i];
			bool num = NKMContentUnlockManager.IsContentUnlocked(myUserData, new UnlockInfo(nKMDiveTemplet2.StageUnlockReqType, nKMDiveTemplet2.StageUnlockReqValue));
			bool flag = false;
			if (myUserData.m_DiveClearData != null && myUserData.m_DiveClearData.Contains(nKMDiveTemplet2.StageID))
			{
				flag = true;
			}
			if (num && !flag)
			{
				selectedIndex = i;
				nKMDiveTemplet = nKMDiveTemplet2;
				break;
			}
		}
		if (nKMDiveTemplet == null && list.Count > 0)
		{
			selectedIndex = list.Count - 1;
			nKMDiveTemplet = list[selectedIndex];
		}
		return nKMDiveTemplet;
	}

	public static bool IsGauntletSectorType(NKM_DIVE_SECTOR_TYPE type)
	{
		if (type == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_GAUNTLET || type == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_GAUNTLET_HARD)
		{
			return true;
		}
		return false;
	}

	public static bool IsBossSectorType(NKM_DIVE_SECTOR_TYPE type)
	{
		if (type == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_BOSS || type == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_BOSS_HARD)
		{
			return true;
		}
		return false;
	}

	public static bool IsPoincareSectorType(NKM_DIVE_SECTOR_TYPE type)
	{
		if (type == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_POINCARE || type == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_POINCARE_HARD)
		{
			return true;
		}
		return false;
	}

	public static bool IsReimannSectorType(NKM_DIVE_SECTOR_TYPE type)
	{
		if (type == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_REIMANN || type == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_REIMANN_HARD)
		{
			return true;
		}
		return false;
	}

	public static bool IsEuclidSectorType(NKM_DIVE_SECTOR_TYPE type)
	{
		if (type == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_EUCLID || type == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_EUCLID_HARD)
		{
			return true;
		}
		return false;
	}

	public static bool IsSectorHardType(NKM_DIVE_SECTOR_TYPE type)
	{
		if (type == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_BOSS_HARD || type == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_GAUNTLET_HARD || type == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_POINCARE_HARD || type == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_REIMANN_HARD || type == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_EUCLID_HARD)
		{
			return true;
		}
		return false;
	}

	public static bool IsItemEventType(NKM_DIVE_EVENT_TYPE type)
	{
		if (type == NKM_DIVE_EVENT_TYPE.NDET_ITEM)
		{
			return true;
		}
		return false;
	}

	public static bool IsLostContainerEventType(NKM_DIVE_EVENT_TYPE type)
	{
		if (type == NKM_DIVE_EVENT_TYPE.NDET_SUPPLY)
		{
			return true;
		}
		return false;
	}

	public static bool IsRandomEventType(NKM_DIVE_EVENT_TYPE type)
	{
		if (type == NKM_DIVE_EVENT_TYPE.NDET_BEACON_RANDOM || type == NKM_DIVE_EVENT_TYPE.NDET_BEACON_DUNGEON || type == NKM_DIVE_EVENT_TYPE.NDET_BEACON_BLANK || type == NKM_DIVE_EVENT_TYPE.NDET_BEACON_ITEM || type == NKM_DIVE_EVENT_TYPE.NDET_BEACON_UNIT || type == NKM_DIVE_EVENT_TYPE.NDET_BEACON_STORM)
		{
			return true;
		}
		return false;
	}

	public static bool IsRescueSignalEventType(NKM_DIVE_EVENT_TYPE type)
	{
		if (type == NKM_DIVE_EVENT_TYPE.NDET_UNIT)
		{
			return true;
		}
		return false;
	}

	public static bool IsLostShipEventType(NKM_DIVE_EVENT_TYPE type)
	{
		if (type == NKM_DIVE_EVENT_TYPE.NDET_LOSTSHIP_RANDOM || type == NKM_DIVE_EVENT_TYPE.NDET_LOSTSHIP_ITEM || type == NKM_DIVE_EVENT_TYPE.NDET_LOSTSHIP_UNIT || type == NKM_DIVE_EVENT_TYPE.NDET_LOSTSHIP_REPAIR || type == NKM_DIVE_EVENT_TYPE.NDET_LOSTSHIP_SUPPLY)
		{
			return true;
		}
		return false;
	}

	public static bool IsSafetyEventType(NKM_DIVE_EVENT_TYPE type)
	{
		if (type == NKM_DIVE_EVENT_TYPE.NDET_BLANK)
		{
			return true;
		}
		return false;
	}

	public static bool IsRepairKitEventType(NKM_DIVE_EVENT_TYPE type)
	{
		if (type == NKM_DIVE_EVENT_TYPE.NDET_REPAIR)
		{
			return true;
		}
		return false;
	}

	public static bool IsArtifactEventType(NKM_DIVE_EVENT_TYPE type)
	{
		if (type == NKM_DIVE_EVENT_TYPE.NDET_ARTIFACT)
		{
			return true;
		}
		return false;
	}

	public static bool IsDiveJump()
	{
		NKMDiveGameData diveGameData = NKCScenManager.CurrentUserData().m_DiveGameData;
		if (diveGameData == null)
		{
			return false;
		}
		return diveGameData.Floor.SlotSets.Count == 1;
	}

	public static NKMDiveTemplet GetTempletByUnlockData(STAGE_UNLOCK_REQ_TYPE type, int value)
	{
		return NKMTempletContainer<NKMDiveTemplet>.Find((NKMDiveTemplet e) => e.StageUnlockReqType == type && e.StageUnlockReqValue == value);
	}

	public static int GetDiveCost(bool bDiveCleared, NKMDiveTemplet templet, int cityID, bool bJump)
	{
		if (templet == null)
		{
			return 0;
		}
		int num = templet.StageReqItemCount;
		if (bDiveCleared)
		{
			num = templet.SafeMineReqItemCount;
		}
		if (bJump)
		{
			num += templet.GetDiveJumpPlusCost();
		}
		int diveDiscountCost = GetDiveDiscountCost(cityID, num);
		return num - diveDiscountCost;
	}

	public static int GetDiveDiscountCost(int cityID, int costCount)
	{
		if (cityID == 0)
		{
			return 0;
		}
		NKMWorldMapCityData cityData = NKCScenManager.CurrentUserData().m_WorldmapData.GetCityData(cityID);
		if (cityData != null)
		{
			float num = cityData.CalcBuildStat(NKM_CITY_BUILDING_STAT.CBS_DIVE_INFORMATION_REDUCE_RATE, costCount);
			return Math.Min(costCount, (int)Math.Ceiling(num));
		}
		return 0;
	}

	public static void Initialize()
	{
		BeginIndex = int.MaxValue;
		EndIndex = int.MinValue;
		foreach (NKMDiveTemplet value in NKMTempletContainer<NKMDiveTemplet>.Values)
		{
			if (!value.IsEventDive)
			{
				if (BeginIndex > value.IndexID)
				{
					BeginIndex = value.IndexID;
				}
				if (EndIndex < value.IndexID)
				{
					EndIndex = value.IndexID;
				}
			}
		}
		SortedTemplates = NKMTempletContainer<NKMDiveTemplet>.Values.OrderBy((NKMDiveTemplet e) => e.IndexID).ToList();
	}
}
