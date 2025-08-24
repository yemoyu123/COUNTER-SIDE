using System.Collections.Generic;
using System.Linq;
using ClientPacket.Community;
using ClientPacket.Warfare;
using Cs.Logging;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKC;

public static class NKCWarfareManager
{
	public static List<WarfareSupporterListData> SupporterList { get; private set; } = new List<WarfareSupporterListData>();

	public static NKM_WARFARE_SERVICE_TYPE UseServiceType { get; private set; } = NKM_WARFARE_SERVICE_TYPE.NWST_NONE;

	public static void SetSupportList(List<WarfareSupporterListData> friends, List<WarfareSupporterListData> guests)
	{
		SupporterList.Clear();
		SupporterList.AddRange(guests);
		SupporterList.AddRange(friends);
	}

	public static WarfareSupporterListData FindSupporter(long friendCode)
	{
		return SupporterList.Find((WarfareSupporterListData v) => v.commonProfile.friendCode == friendCode);
	}

	public static bool IsGeustSupporter(long friendCode)
	{
		if (friendCode == 0L)
		{
			return false;
		}
		return !NKCFriendManager.IsFriend(friendCode);
	}

	public static void CheckValidClientOnly()
	{
		foreach (NKMWarfareTemplet value in NKMTempletContainer<NKMWarfareTemplet>.Values)
		{
			value.ValidateClientOnly();
		}
		NKCCutScenManager.ClearCacheData();
	}

	public static bool CheckValidSpawnPoint(NKMWarfareMapTemplet cNKMWarfareMapTemplet, NKMWarfareTileTemplet cNKMWarfareTile, NKMUserData cNKMUserData, NKMDeckIndex cNKMDeckIndex, out bool bAssultPoint)
	{
		bAssultPoint = false;
		if (cNKMWarfareMapTemplet == null || cNKMWarfareTile == null || cNKMUserData == null)
		{
			return false;
		}
		if (cNKMWarfareTile.m_NKM_WARFARE_SPAWN_POINT_TYPE == NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_NONE)
		{
			return false;
		}
		if (cNKMWarfareTile.m_NKM_WARFARE_SPAWN_POINT_TYPE == NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_ASSAULT)
		{
			bAssultPoint = true;
			if (!CheckAssaultShip(cNKMUserData, cNKMDeckIndex))
			{
				return false;
			}
		}
		return true;
	}

	public static NKM_ERROR_CODE CheckWFGameStartCond(NKMUserData cNKMUserData, NKMPacket_WARFARE_GAME_START_REQ startReq)
	{
		if (cNKMUserData == null)
		{
			Log.Error("StartWarfareGame cNKMUserData is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 83);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_INVALID_USER;
		}
		if (startReq == null)
		{
			Log.Error("StartWarfareGame cNKMWarfareGameStartData is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 89);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_INVALID_START_GAME_DATA;
		}
		if (NKCScenManager.GetScenManager().WarfareGameData.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_STOP)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_IS_NOT_STOP;
		}
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(startReq.warfareTempletID);
		if (nKMWarfareTemplet == null)
		{
			Log.Error($"GetWarfareTempletByID null, m_WarfareTempletID: {startReq.warfareTempletID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 104);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_FIND_WARFARE_TEMPLET;
		}
		NKMStageTempletV2 nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(nKMWarfareTemplet.m_WarfareStrID);
		if (nKMStageTempletV == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_FIND_EPISODE_TEMPLET;
		}
		if (!NKMEpisodeMgr.CheckEpisodeMission(cNKMUserData, nKMStageTempletV))
		{
			return NKM_ERROR_CODE.NEC_FAIL_LOCKED_EPISODE;
		}
		if (startReq.unitPositionList.Count > nKMWarfareTemplet.m_UserTeamCount)
		{
			Log.Error($"Warfare, MaxUserUnitCount Overflow, Request Count : {startReq.unitPositionList.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 124);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_START_BY_MAX_USER_UNIT_OVERFLOW;
		}
		if (!startReq.unitPositionList.Any((NKMPacket_WARFARE_GAME_START_REQ.UnitPosition pos) => pos.isFlagShip))
		{
			Log.Error("WarfareGame.CheckWFGameStartCond, Can't Find Flag Ship", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 131);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_NOT_EXIST_USER_FLAG_SHIP;
		}
		if ((from pos in startReq.unitPositionList
			group pos by pos.deckIndex).Any((IGrouping<byte, NKMPacket_WARFARE_GAME_START_REQ.UnitPosition> e) => e.Count() > 1))
		{
			Log.Error("WarfareGame, Duplicate Deck Index Found!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 138);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_START_BY_DUPLICATE_DECK_INDEX;
		}
		NKMWarfareMapTemplet mapTemplet = nKMWarfareTemplet.MapTemplet;
		bool flag = false;
		int num = 0;
		bool flag2 = false;
		bool flag3 = false;
		for (int num2 = 0; num2 < mapTemplet.m_MapSizeX; num2++)
		{
			for (int num3 = 0; num3 < mapTemplet.m_MapSizeY; num3++)
			{
				NKMWarfareTileTemplet tile = mapTemplet.GetTile(num2, num3);
				if (tile == null)
				{
					continue;
				}
				if (tile.m_DungeonStrID != null)
				{
					if (!flag && tile.m_bFlagDungeon)
					{
						flag = true;
					}
					if (tile.m_bTargetUnit)
					{
						num++;
					}
				}
				if (!flag2 && tile.m_TileWinType == WARFARE_GAME_CONDITION.WFC_TILE_ENTER)
				{
					flag2 = true;
				}
				if (!flag3 && tile.m_TileWinType == WARFARE_GAME_CONDITION.WFC_PHASE_TILE_HOLD)
				{
					flag3 = true;
				}
			}
		}
		if (nKMWarfareTemplet.m_WFWinCondition == WARFARE_GAME_CONDITION.WFC_KILL_BOSS)
		{
			if (!flag)
			{
				Log.Error("WarfareGame, Flag Dungeon Not Found", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 188);
				return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_NOT_EXIST_FLAG_DUNGEON;
			}
		}
		else if (nKMWarfareTemplet.m_WFWinCondition == WARFARE_GAME_CONDITION.WFC_KILL_TARGET)
		{
			if (nKMWarfareTemplet.m_WFWinValue != num)
			{
				Log.Error($"WarfareGame, WFWC_Kill - WinValue {nKMWarfareTemplet.m_WFWinValue} != TileTarget {num}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 196);
				return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_WIN_CONDITION_KILL_TARGET_COUNT;
			}
		}
		else if (nKMWarfareTemplet.m_WFWinCondition == WARFARE_GAME_CONDITION.WFC_TILE_ENTER)
		{
			if (!flag2)
			{
				Log.Error("WarfareGame, WFWC_ENTER - Enter Tile Not Found", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 204);
				return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_WIN_CONDITION_ENTER_TILE_NOT;
			}
		}
		else if (nKMWarfareTemplet.m_WFWinCondition == WARFARE_GAME_CONDITION.WFC_PHASE_TILE_HOLD && !flag3)
		{
			Log.Error("WarfareGame, WFWC_HOLD - Hold Tile Not Found", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 212);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_WIN_CONDITION_HOLD_TILE_NOT;
		}
		for (int num4 = 0; num4 < startReq.unitPositionList.Count; num4++)
		{
			NKMPacket_WARFARE_GAME_START_REQ.UnitPosition unitPosition = startReq.unitPositionList[num4];
			NKM_ERROR_CODE nKM_ERROR_CODE = NKMMain.IsValidDeck(cNKMUserData.m_ArmyData, new NKMDeckIndex(NKM_DECK_TYPE.NDT_NORMAL, unitPosition.deckIndex));
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				return nKM_ERROR_CODE;
			}
			NKMWarfareTileTemplet tile2 = mapTemplet.GetTile(unitPosition.tileIndex);
			if (tile2 == null)
			{
				Log.Error($"cNKMWarfareTile is Null tileIndex : {unitPosition.tileIndex}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 233);
				return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_FIND_WARFARE_TILE;
			}
			if (tile2.m_TileType == NKM_WARFARE_MAP_TILE_TYPE.NWMTT_DISABLE)
			{
				Log.Error($"cNKMWarfareTile is disable, tileIndex : {unitPosition.tileIndex}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 239);
				return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_SET_UNIT_ON_DISABLE_TILE;
			}
			if (tile2.m_DungeonStrID != null)
			{
				Log.Error($"NEC_FAIL_WARFARE_GAME_CANNOT_POSITION_BY_DUNGEON, tileIndex : {unitPosition.tileIndex}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 245);
				return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_POSITION_BY_DUNGEON;
			}
			bool bAssultPoint = false;
			if (!CheckValidSpawnPoint(mapTemplet, tile2, cNKMUserData, new NKMDeckIndex(NKM_DECK_TYPE.NDT_NORMAL, unitPosition.deckIndex), out bAssultPoint))
			{
				if (bAssultPoint)
				{
					Log.Error($"NEC_FAIL_WARFARE_GAME_CANNOT_ASSAULT_POSITION, tileIndex : {unitPosition.tileIndex}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 265);
					return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_ASSAULT_POSITION;
				}
				Log.Error($"NEC_FAIL_WARFARE_GAME_CANNOT_POSITION, tileIndex : {unitPosition.tileIndex}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 270);
				return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_POSITION;
			}
		}
		if (startReq.friendCode != 0L)
		{
			if (!nKMWarfareTemplet.m_bFriendSummon)
			{
				Log.Error("you can't play with supporter : " + nKMWarfareTemplet.m_WarfareStrID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 280);
				return NKM_ERROR_CODE.NEC_FAIL_WARFARE_FRIEND_NOT_SUPPOTABLE_MAP;
			}
			if (FindSupporter(startReq.friendCode) == null)
			{
				Log.Error($"fail - friend code : {startReq.friendCode}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 288);
				return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_FIND_UNIT;
			}
			NKMWarfareTileTemplet tile3 = mapTemplet.GetTile(startReq.friendTileIndex);
			if (tile3 == null)
			{
				Log.Error($"cNKMWarfareTile is Null friend tileIndex : {startReq.friendTileIndex}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 295);
				return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_FIND_WARFARE_TILE;
			}
			if (tile3.m_NKM_WARFARE_SPAWN_POINT_TYPE == NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_ASSAULT)
			{
				Log.Error("게스트/친구 소대는 강습지점에 착륙 불가", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 302);
				return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_POSITION;
			}
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static void UseService(NKMUserData userData, WarfareGameData cNKMWarfareGameData, NKMPacket_WARFARE_GAME_USE_SERVICE_ACK sPacket)
	{
		if (userData == null || cNKMWarfareGameData == null)
		{
			return;
		}
		WarfareUnitData unitData = cNKMWarfareGameData.GetUnitData(sPacket.warfareGameUnitUID);
		if (unitData != null)
		{
			UseServiceType = sPacket.warfareServiceType;
			if (UseServiceType == NKM_WARFARE_SERVICE_TYPE.NWST_REPAIR)
			{
				unitData.hp = sPacket.hp;
			}
			else if (UseServiceType == NKM_WARFARE_SERVICE_TYPE.NWST_RESUPPLY)
			{
				unitData.supply = sPacket.supply;
			}
		}
	}

	public static void ResetServiceType()
	{
		UseServiceType = NKM_WARFARE_SERVICE_TYPE.NWST_NONE;
	}

	public static NKM_ERROR_CODE CanTryServiceUse(NKMUserData userData, WarfareGameData cNKMWarfareGameData, int warfareGameUnitUID, NKM_WARFARE_SERVICE_TYPE serviceType)
	{
		if (userData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_USER_DATA_NULL;
		}
		if (cNKMWarfareGameData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_DATA_NULL;
		}
		if (cNKMWarfareGameData.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_PLAYING)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_IS_NOT_PLAYING_STATE;
		}
		if (!cNKMWarfareGameData.isTurnA)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CAN_ONLY_USE_THIS_ITEM_ON_TURN_A;
		}
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(cNKMWarfareGameData.warfareTempletID);
		if (nKMWarfareTemplet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_INVALID_WARFARE_TEMPLET_ID;
		}
		_ = nKMWarfareTemplet.MapTemplet;
		WarfareUnitData unitData = cNKMWarfareGameData.GetUnitData(warfareGameUnitUID);
		if (unitData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_FIND_UNIT;
		}
		int tileIndex = unitData.tileIndex;
		WarfareTileData tileData = cNKMWarfareGameData.GetTileData(tileIndex);
		if (tileData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_FIND_WARFARE_TILE;
		}
		switch (serviceType)
		{
		case NKM_WARFARE_SERVICE_TYPE.NWST_REPAIR:
			if (tileData.tileType != NKM_WARFARE_MAP_TILE_TYPE.NWMTT_REPAIR && tileData.tileType != NKM_WARFARE_MAP_TILE_TYPE.NWNTT_SERVICE)
			{
				return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_NOT_SERVICEABLE_TILE;
			}
			break;
		case NKM_WARFARE_SERVICE_TYPE.NWST_RESUPPLY:
			if (tileData.tileType != NKM_WARFARE_MAP_TILE_TYPE.NWMTT_RESUPPLY && tileData.tileType != NKM_WARFARE_MAP_TILE_TYPE.NWNTT_SERVICE)
			{
				return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_NOT_SERVICEABLE_TILE;
			}
			break;
		}
		switch (serviceType)
		{
		case NKM_WARFARE_SERVICE_TYPE.NWST_REPAIR:
			if (unitData.hp >= unitData.hpMax)
			{
				return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_UNIT_HP_IS_ALREADY_FULL;
			}
			break;
		case NKM_WARFARE_SERVICE_TYPE.NWST_RESUPPLY:
			if (unitData.supply >= 2)
			{
				return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_UNIT_SUPPLY_IS_ALREADY_FULL;
			}
			break;
		}
		if (IsAuto(userData, cNKMWarfareGameData.warfareTempletID))
		{
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_USE_THIS_ITEM_ON_AUTO;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE CheckMoveCond(NKMUserData cNKMUserData, int tileIndexFrom, int tileIndexTo)
	{
		if (cNKMUserData == null)
		{
			Log.Error("CheckMoveCond cNKMUserData is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 434);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_INVALID_USER;
		}
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData == null)
		{
			Log.Error("CheckMoveCond cNKMWarfareGameData is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 441);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_INVALID_WARFARE_GAME_DATA;
		}
		if (warfareGameData.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_PLAYING)
		{
			Log.Error("TurnFinish, game state is not playing", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 447);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_IS_NOT_PLAYING_STATE;
		}
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(warfareGameData.warfareTempletID);
		if (nKMWarfareTemplet == null)
		{
			Log.Error("CheckMoveCond cNKMWarfareTemplet is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 454);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_INVALID_WARFARE_TEMPLET_ID;
		}
		if (IsAuto(cNKMUserData, warfareGameData.warfareTempletID))
		{
			Log.Error("CheckMoveCond cNKMWarfareGameData is auto status", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 460);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_USER_CONTROL_MOVE_ON_AUTO;
		}
		if (tileIndexFrom < 0 || tileIndexFrom >= warfareGameData.warfareTileDataList.Count)
		{
			Log.Error("CheckMoveCond tileIndexFrom is invalid, index : " + tileIndexFrom, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 468);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_MOVE_INVALID_TILE_INDEX;
		}
		if (tileIndexTo < 0 || tileIndexTo >= warfareGameData.warfareTileDataList.Count)
		{
			Log.Error("CheckMoveCond tileIndexTo is invalid, index : " + tileIndexTo, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 474);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_MOVE_INVALID_TILE_INDEX;
		}
		WarfareUnitData unitDataByTileIndex = warfareGameData.GetUnitDataByTileIndex(tileIndexFrom);
		if (unitDataByTileIndex == null)
		{
			Log.Error("CheckMoveCond WarfareGameUnitUID at tileIndexFrom is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 490);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_MOVE_UNIT_NOT_EXIST_ON_FROMTILE;
		}
		if (unitDataByTileIndex.isTurnEnd)
		{
			Log.Error("CheckMoveCond Unit's turn already end, WarfareGameUnitUID at tileIndexFrom : " + unitDataByTileIndex.warfareGameUnitUID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 497);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_MOVE_UNIT_TURN_ALREADY_END;
		}
		_ = nKMWarfareTemplet.MapTemplet;
		WarfareTileData tileData = warfareGameData.GetTileData(tileIndexFrom);
		if (tileData == null)
		{
			Log.Error("CheckMoveCond tileIndexFrom is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 510);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_MOVE_INVALID_TILE_INDEX;
		}
		if (tileData.tileType == NKM_WARFARE_MAP_TILE_TYPE.NWMTT_DISABLE)
		{
			Log.Error("CheckMoveCond tileIndexFrom is disable", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 516);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_MOVE_DISABLE_TILE_INDEX;
		}
		WarfareTileData warfareTileData = warfareGameData.warfareTileDataList[tileIndexTo];
		if (warfareTileData == null)
		{
			Log.Error("CheckMoveCond cNKMWarfareGameTileDataTo is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 523);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_MOVE_INVALID_TILE_INDEX;
		}
		if (warfareTileData.tileType == NKM_WARFARE_MAP_TILE_TYPE.NWMTT_DISABLE)
		{
			Log.Error("CheckMoveCond tileIndexTo is disable", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 539);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_MOVE_DISABLE_TILE_INDEX;
		}
		WarfareTeamData warfareTeamData = null;
		warfareTeamData = ((!warfareGameData.isTurnA) ? warfareGameData.warfareTeamDataB : warfareGameData.warfareTeamDataA);
		if (!warfareTeamData.warfareUnitDataByUIDMap.ContainsKey(unitDataByTileIndex.warfareGameUnitUID))
		{
			Log.Error("CheckMoveCond Can't find Unit in TeamAtThisTurn, WarfareGameUnitUID at tileIndexFrom : " + unitDataByTileIndex.warfareGameUnitUID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 556);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_MOVE_UNIT_NOT_EXIST_ON_FROMTILE;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE CheckPossibleAuto(NKMUserData cNKMUserData, bool bSet, bool bAutoSupply)
	{
		if (cNKMUserData == null)
		{
			Log.Error("CheckPossibleAuto cNKMUserData is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 569);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_INVALID_USER;
		}
		if (NKCScenManager.GetScenManager().WarfareGameData == null)
		{
			Log.Error("CheckPossibleAuto cNKMWarfareGameData is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 577);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_INVALID_WARFARE_GAME_DATA;
		}
		if (cNKMUserData.m_UserOption.m_bAutoWarfare == bSet && cNKMUserData.m_UserOption.m_bAutoWarfareRepair == bAutoSupply)
		{
			Log.Error($"WarfareGame's auto is already set as {bSet}, {bAutoSupply}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 592);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_AUTO_IS_ALREADY_SET;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE CheckGetNextOrderCond(NKMUserData cNKMUserData)
	{
		if (cNKMUserData == null)
		{
			Log.Error("CheckGetNextOrderCond cNKMUserData is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 606);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_INVALID_USER;
		}
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData == null)
		{
			Log.Error("CheckGetNextOrderCond cNKMWarfareGameData is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 613);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_INVALID_WARFARE_GAME_DATA;
		}
		if (warfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_STOP)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_STATE_NWGS_STOP;
		}
		if (warfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_INGAME_PLAYING)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_STATE_NWGS_INGAME_PLAYING;
		}
		if (NKMWarfareTemplet.Find(warfareGameData.warfareTempletID) == null)
		{
			Log.Error("CheckGetNextOrderCond cNKMWarfareTemplet is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 626);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_INVALID_WARFARE_TEMPLET_ID;
		}
		if (warfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_PLAYING && warfareGameData.isTurnA && !cNKMUserData.m_UserOption.m_bAutoWarfare)
		{
			Log.Error($"CheckGetNextOrderCond, CANNOT_GET_NEXT_ORDER_AT_TURN_A - STATE : {warfareGameData.warfareGameState}, TurnA : {warfareGameData.isTurnA}, Auto : {cNKMUserData.m_UserOption.m_bAutoWarfare}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCWarfareManager.cs", 633);
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_GET_NEXT_ORDER_AT_TURN_A;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_UNIT_STYLE_TYPE GetShipStyleTypeByGUUID(NKMUserData cNKMUserData, WarfareGameData cNKMWarfareGameData, int guuid)
	{
		NKM_UNIT_STYLE_TYPE nKM_UNIT_STYLE_TYPE = NKM_UNIT_STYLE_TYPE.NUST_INVALID;
		if (cNKMUserData == null)
		{
			return nKM_UNIT_STYLE_TYPE;
		}
		WarfareUnitData unitData = cNKMWarfareGameData.GetUnitData(guuid);
		if (unitData == null)
		{
			return nKM_UNIT_STYLE_TYPE;
		}
		NKMDeckData deckData = cNKMUserData.m_ArmyData.GetDeckData(unitData.deckIndex);
		if (deckData == null)
		{
			return nKM_UNIT_STYLE_TYPE;
		}
		NKMUnitData shipFromUID = cNKMUserData.m_ArmyData.GetShipFromUID(deckData.m_ShipUID);
		if (shipFromUID == null)
		{
			return nKM_UNIT_STYLE_TYPE;
		}
		return NKMUnitManager.GetUnitTempletBase(shipFromUID.m_UnitID)?.m_NKM_UNIT_STYLE_TYPE ?? nKM_UNIT_STYLE_TYPE;
	}

	public static bool CheckOnTileType(NKMWarfareMapTemplet cNKMWarfareMapTemplet, int tileIndex, NKM_WARFARE_MAP_TILE_TYPE eNKM_WARFARE_MAP_TILE_TYPE)
	{
		if (cNKMWarfareMapTemplet == null)
		{
			return false;
		}
		NKMWarfareTileTemplet tile = cNKMWarfareMapTemplet.GetTile(tileIndex);
		if (tile == null)
		{
			return false;
		}
		if (tile.m_TileType == eNKM_WARFARE_MAP_TILE_TYPE)
		{
			return true;
		}
		return false;
	}

	public static bool CheckOnTileType(WarfareGameData warfareGameData, int tileIndex, NKM_WARFARE_MAP_TILE_TYPE eNKM_WARFARE_MAP_TILE_TYPE)
	{
		if (warfareGameData == null)
		{
			return false;
		}
		WarfareTileData tileData = warfareGameData.GetTileData(tileIndex);
		if (tileData == null)
		{
			return false;
		}
		if (tileData.tileType == eNKM_WARFARE_MAP_TILE_TYPE)
		{
			return true;
		}
		return false;
	}

	public static bool CheckAssaultShip(NKMUserData cNKMUserData, NKMDeckIndex sNKMDeckIndex)
	{
		NKMDeckData deckData = cNKMUserData.m_ArmyData.GetDeckData(sNKMDeckIndex);
		if (deckData == null)
		{
			return false;
		}
		NKMUnitData shipFromUID = cNKMUserData.m_ArmyData.GetShipFromUID(deckData.m_ShipUID);
		if (shipFromUID == null)
		{
			return false;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(shipFromUID.m_UnitID);
		if (unitTempletBase == null)
		{
			return false;
		}
		if (unitTempletBase.m_NKM_UNIT_STYLE_TYPE != NKM_UNIT_STYLE_TYPE.NUST_SHIP_ASSAULT)
		{
			return false;
		}
		return true;
	}

	public static int GetCurrentMissionValue(WarfareGameData warfareGameData, WARFARE_GAME_MISSION_TYPE missionType)
	{
		if (warfareGameData == null)
		{
			return 0;
		}
		int num = 0;
		int num2 = 0;
		foreach (KeyValuePair<int, WarfareUnitData> item in warfareGameData.warfareTeamDataB.warfareUnitDataByUIDMap)
		{
			if (item.Value.hp <= 0f)
			{
				num2++;
			}
		}
		switch (missionType)
		{
		case WARFARE_GAME_MISSION_TYPE.WFMT_CLEAR:
			if (warfareGameData.isWinTeamA)
			{
				num++;
			}
			break;
		case WARFARE_GAME_MISSION_TYPE.WFMT_ALLKILL:
			if (num2 == warfareGameData.warfareTeamDataB.warfareUnitDataByUIDMap.Count)
			{
				num++;
			}
			break;
		case WARFARE_GAME_MISSION_TYPE.WFMT_PHASE:
			num = warfareGameData.turnCount;
			break;
		case WARFARE_GAME_MISSION_TYPE.WFMT_NO_SHIPWRECK:
		{
			int num3 = 0;
			foreach (KeyValuePair<int, WarfareUnitData> item2 in warfareGameData.warfareTeamDataA.warfareUnitDataByUIDMap)
			{
				if (item2.Value.hp > 0f)
				{
					num3++;
				}
			}
			if (num3 == warfareGameData.warfareTeamDataA.warfareUnitDataByUIDMap.Count)
			{
				num++;
			}
			break;
		}
		case WARFARE_GAME_MISSION_TYPE.WFMT_KILL:
			foreach (KeyValuePair<int, WarfareUnitData> item3 in warfareGameData.warfareTeamDataB.warfareUnitDataByUIDMap)
			{
				if (item3.Value.hp <= 0f)
				{
					num++;
				}
			}
			break;
		case WARFARE_GAME_MISSION_TYPE.WFMT_FIRST_ATTACK:
			num = warfareGameData.firstAttackCount;
			break;
		case WARFARE_GAME_MISSION_TYPE.WFMT_NOSUPPLY_WIN:
			if (warfareGameData.supplyUseCount == 0)
			{
				num++;
			}
			break;
		case WARFARE_GAME_MISSION_TYPE.WFMT_NOSUPPLY_ALLKILL:
			if (warfareGameData.supplyUseCount == 0 && num2 == warfareGameData.warfareTeamDataB.warfareUnitDataByUIDMap.Count)
			{
				num++;
			}
			break;
		}
		return num;
	}

	public static int GetWarfareID(string warfareStrID)
	{
		return NKMWarfareTemplet.Find(warfareStrID)?.m_WarfareID ?? 0;
	}

	public static string GetWarfareStrID(int warfareID)
	{
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(warfareID);
		if (nKMWarfareTemplet != null)
		{
			return nKMWarfareTemplet.m_WarfareStrID;
		}
		return "";
	}

	public static bool IsAuto(NKMUserData userData, int warfareTempletID)
	{
		if (!userData.CheckWarfareClear(warfareTempletID))
		{
			return false;
		}
		return userData.m_UserOption.m_bAutoWarfare;
	}

	public static List<WarfareTileData> GetNeighborTiles(NKMWarfareMapTemplet mapTemplet, int tileIndex, bool includeSelf)
	{
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		List<WarfareTileData> list = new List<WarfareTileData>();
		NKMWarfareTileTemplet tile = mapTemplet.GetTile(tileIndex);
		if (tile != null)
		{
			foreach (short neighborTile in tile.NeighborTiles)
			{
				byte b = (byte)neighborTile;
				if (includeSelf || b != tileIndex)
				{
					WarfareTileData tileData = warfareGameData.GetTileData(b);
					list.Add(tileData);
				}
			}
		}
		return list;
	}

	public static void GetCurrWarfareAttackCost(out int itemID, out int itemCount)
	{
		itemID = 0;
		itemCount = 0;
		string warfareStrID = NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareStrID();
		if (NKMWarfareTemplet.Find(warfareStrID) == null)
		{
			return;
		}
		NKMStageTempletV2 nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(warfareStrID);
		if (nKMStageTempletV != null)
		{
			if (nKMStageTempletV.m_StageReqItemID > 0)
			{
				itemID = nKMStageTempletV.m_StageReqItemID;
				itemCount = nKMStageTempletV.m_StageReqItemCount;
			}
			else
			{
				itemID = 2;
				itemCount = 0;
			}
			if (nKMStageTempletV.m_StageReqItemID == 2)
			{
				NKCCompanyBuff.SetDiscountOfEterniumInEnteringWarfare(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref itemCount);
			}
		}
	}

	public static List<WarfareUnitData> GetNeighborFriends(NKMWarfareMapTemplet mapTemplet, int tileIndex)
	{
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		List<WarfareUnitData> list = new List<WarfareUnitData>();
		foreach (WarfareTileData neighborTile in GetNeighborTiles(mapTemplet, tileIndex, includeSelf: false))
		{
			WarfareUnitData unitDataByTileIndex_TeamA = warfareGameData.GetUnitDataByTileIndex_TeamA(neighborTile.index);
			if (unitDataByTileIndex_TeamA != null)
			{
				list.Add(unitDataByTileIndex_TeamA);
			}
		}
		return list;
	}

	public static int GetRecoverableUnitCount(NKMArmyData armyData)
	{
		return GetRecoverableDeckIndexList(armyData).Count;
	}

	public static List<int> GetRecoverableDeckIndexList(NKMArmyData armyData)
	{
		List<WarfareUnitData> list = NKCScenManager.GetScenManager().WarfareGameData.warfareTeamDataA.warfareUnitDataByUIDMap.Values.ToList();
		NKM_DECK_TYPE nKM_DECK_TYPE = NKM_DECK_TYPE.NDT_NORMAL;
		int unlockedDeckCount = armyData.GetUnlockedDeckCount(nKM_DECK_TYPE);
		List<int> list2 = new List<int>();
		for (int i = 0; i < unlockedDeckCount; i++)
		{
			NKMDeckData deckData = armyData.GetDeckData(nKM_DECK_TYPE, i);
			if (deckData != null && deckData.GetState() == NKM_DECK_STATE.DECK_STATE_WARFARE)
			{
				NKMDeckIndex deckIndex = new NKMDeckIndex(nKM_DECK_TYPE, i);
				WarfareUnitData warfareUnitData = list.Find((WarfareUnitData v) => v.deckIndex.Compare(deckIndex));
				if (warfareUnitData == null || (warfareUnitData.unitType == WarfareUnitData.Type.User && !(warfareUnitData.hp > 0f)))
				{
					list2.Add(i);
				}
			}
		}
		return list2;
	}
}
