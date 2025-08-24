using System;
using System.Collections.Generic;
using ClientPacket.WorldMap;
using Cs.Logging;
using NKM.Templet;

namespace NKM;

public class NKMDiveGameManager
{
	public static int GetCost(NKMDiveTemplet templet, NKMWorldMapCityData cityData)
	{
		int num = templet.StageReqItemCount;
		if (cityData != null)
		{
			float num2 = cityData.CalcBuildStat(NKM_CITY_BUILDING_STAT.CBS_DIVE_INFORMATION_REDUCE_RATE, num);
			int num3 = Math.Min(num, (int)Math.Ceiling(num2));
			num -= num3;
		}
		return num;
	}

	public static NKM_ERROR_CODE CanStart(int cityID, int stageID, List<int> deckIndexes, NKMUserData userData, DateTime curTimeUTC)
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
				Log.Error($"Invalid Templet City ID. CityID : {cityData.cityID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Dive/NKMDiveGameManager.cs", 85);
				return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_CITY_ID;
			}
			if (nKMWorldMapEventTemplet.eventType != NKM_WORLDMAP_EVENT_TYPE.WET_DIVE || nKMWorldMapEventTemplet.stageID != stageID)
			{
				return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_EVENT_GROUP_ID;
			}
		}
		int cost = GetCost(nKMDiveTemplet, cityData);
		if (!userData.CheckPrice(cost, nKMDiveTemplet.StageReqItemId))
		{
			return NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_ITEM;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE CanMoveForward(int nextSlotIndex, NKMUserData userData)
	{
		if (userData.m_DiveGameData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_DIVE_HAS_NOT_STARTED_YET;
		}
		NKMDiveFloor floor = userData.m_DiveGameData.Floor;
		NKMDivePlayer player = userData.m_DiveGameData.Player;
		int nextSlotSetIndex = player.GetNextSlotSetIndex();
		if (player.PlayerBase.State != NKMDivePlayerState.Exploring || !player.CanMove(floor, nextSlotSetIndex, nextSlotIndex))
		{
			return NKM_ERROR_CODE.NEC_FAIL_DIVE_CANNOT_MOVE_FORWARD;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE CanGiveUp(NKMUserData userData)
	{
		if (userData.m_DiveGameData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_DIVE_HAS_NOT_STARTED_YET;
		}
		if (userData.m_DiveGameData.Player.IsInBattle())
		{
			return NKM_ERROR_CODE.NEC_FAIL_DIVE_CANNOT_GIVE_UP;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static void UpdateAllDiveDeckState(NKM_DECK_STATE deckState, NKMUserData userData)
	{
		foreach (NKMDiveSquad value in userData.m_DiveGameData.Player.Squads.Values)
		{
			NKMDeckData deckData = userData.m_ArmyData.GetDeckData(NKM_DECK_TYPE.NDT_DIVE, value.DeckIndex);
			if (deckData == null)
			{
				Log.Error($"Invalid Deck Index. UserUid:{userData.m_UserUID}, DeckType:{NKM_DECK_TYPE.NDT_DIVE}, DeckIndex:{value.DeckIndex}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Dive/NKMDiveGameManager.cs", 159);
				continue;
			}
			if (deckData.GetState() != NKM_DECK_STATE.DECK_STATE_DIVE)
			{
				deckData.SetState(deckState);
			}
			userData.m_ArmyData.DeckUpdated(new NKMDeckIndex(NKM_DECK_TYPE.NDT_DIVE, value.DeckIndex), deckData);
		}
	}
}
