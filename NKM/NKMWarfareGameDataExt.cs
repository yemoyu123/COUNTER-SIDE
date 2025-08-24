using System.Collections.Generic;
using System.Linq;
using ClientPacket.Warfare;

namespace NKM;

public static class NKMWarfareGameDataExt
{
	public static void UpdateData(this WarfareGameData self, WarfareSyncData syncData)
	{
		if (syncData == null)
		{
			return;
		}
		if (syncData.gameState != null)
		{
			self.UpdateGameState(syncData.gameState);
		}
		foreach (WarfareUnitSyncData updatedUnit in syncData.updatedUnits)
		{
			if (updatedUnit != null)
			{
				WarfareUnitData unitData = self.GetUnitData(updatedUnit.warfareGameUnitUID);
				if (unitData != null)
				{
					self.UpdateUnitData(unitData, updatedUnit);
				}
			}
		}
		foreach (WarfareTileData tile in syncData.tiles)
		{
			WarfareTileData tileData = self.GetTileData(tile.index);
			if (tileData != null)
			{
				self.UpdateTileData(tileData, tile);
			}
		}
	}

	public static void UpdateUnitData(this WarfareGameData self, WarfareUnitData unitData, WarfareUnitSyncData syncData)
	{
		unitData.hp = syncData.hp;
		unitData.isTurnEnd = syncData.isTurnEnd;
		unitData.supply = syncData.supply;
	}

	public static void UpdateGameState(this WarfareGameData self, WarfareGameSyncData cNKMWarfareGameSyncData)
	{
		self.warfareGameState = cNKMWarfareGameSyncData.warfareGameState;
		self.isTurnA = cNKMWarfareGameSyncData.isTurnA;
		self.turnCount = cNKMWarfareGameSyncData.turnCount;
		self.firstAttackCount = cNKMWarfareGameSyncData.firstAttackCount;
		self.assistCount = cNKMWarfareGameSyncData.assistCount;
		self.battleAllyUid = cNKMWarfareGameSyncData.battleAllyUid;
		self.battleMonsterUid = cNKMWarfareGameSyncData.battleMonsterUid;
		self.isWinTeamA = cNKMWarfareGameSyncData.isWinTeamA;
		self.holdCount = cNKMWarfareGameSyncData.holdCount;
		self.containerCount = cNKMWarfareGameSyncData.containerCount;
		self.enemiesKillCount = cNKMWarfareGameSyncData.enemiesKillCount;
		self.alliesKillCount = cNKMWarfareGameSyncData.alliesKillCount;
		self.targetKillCount = cNKMWarfareGameSyncData.targetKillCount;
	}

	public static void UpdateTileData(this WarfareGameData self, WarfareTileData currentTile, WarfareTileData syncTile)
	{
		currentTile.tileType = syncTile.tileType;
		currentTile.battleConditionId = syncTile.battleConditionId;
	}

	public static WarfareTileData GetTileData(this WarfareGameData self, int index)
	{
		if (index < 0 || index >= self.warfareTileDataList.Count)
		{
			return null;
		}
		return self.warfareTileDataList[index];
	}

	public static bool CheckTeamA_By_GameUnitUID(this WarfareGameData self, int guuid)
	{
		if (self.warfareTeamDataA == null)
		{
			return false;
		}
		if (self.warfareTeamDataA.warfareUnitDataByUIDMap.ContainsKey(guuid))
		{
			return true;
		}
		return false;
	}

	public static WarfareUnitData GetUnitData(this WarfareGameData self, int guuid)
	{
		if (self.warfareTeamDataA != null && self.warfareTeamDataA.warfareUnitDataByUIDMap.ContainsKey(guuid))
		{
			return self.warfareTeamDataA.warfareUnitDataByUIDMap[guuid];
		}
		if (self.warfareTeamDataB != null && self.warfareTeamDataB.warfareUnitDataByUIDMap.ContainsKey(guuid))
		{
			return self.warfareTeamDataB.warfareUnitDataByUIDMap[guuid];
		}
		return null;
	}

	public static WarfareUnitData GetUnitDataByNormalDeckIndex(this WarfareGameData self, byte normalDeckIndex)
	{
		if (self.warfareTeamDataA != null)
		{
			foreach (WarfareUnitData value in self.warfareTeamDataA.warfareUnitDataByUIDMap.Values)
			{
				if (value.deckIndex.m_eDeckType == NKM_DECK_TYPE.NDT_NORMAL && value.deckIndex.m_iIndex == normalDeckIndex)
				{
					return value;
				}
			}
		}
		return null;
	}

	public static WarfareUnitData GetUnitDataByTileIndex(this WarfareGameData self, int tileIndex)
	{
		WarfareUnitData unitDataByTileIndex_TeamA = self.GetUnitDataByTileIndex_TeamA(tileIndex);
		if (unitDataByTileIndex_TeamA != null)
		{
			return unitDataByTileIndex_TeamA;
		}
		unitDataByTileIndex_TeamA = self.GetUnitDataByTileIndex_TeamB(tileIndex);
		if (unitDataByTileIndex_TeamA != null)
		{
			return unitDataByTileIndex_TeamA;
		}
		return null;
	}

	public static WarfareUnitData GetUnitDataByTileIndex_TeamA(this WarfareGameData self, int tileIndex)
	{
		if (self.warfareTeamDataA != null)
		{
			foreach (WarfareUnitData value in self.warfareTeamDataA.warfareUnitDataByUIDMap.Values)
			{
				if (value.tileIndex == tileIndex && value.hp > 0f)
				{
					return value;
				}
			}
		}
		return null;
	}

	public static WarfareUnitData GetUnitDataByTileIndex_TeamB(this WarfareGameData self, int tileIndex)
	{
		if (self.warfareTeamDataB != null)
		{
			foreach (WarfareUnitData value in self.warfareTeamDataB.warfareUnitDataByUIDMap.Values)
			{
				if (value.tileIndex == tileIndex && value.hp > 0f)
				{
					return value;
				}
			}
		}
		return null;
	}

	public static List<WarfareUnitData> GetUnitDataList(this WarfareGameData self)
	{
		List<WarfareUnitData> list = new List<WarfareUnitData>();
		if (self.warfareTeamDataA != null)
		{
			list.AddRange(self.warfareTeamDataA.warfareUnitDataByUIDMap.Values.ToList());
		}
		if (self.warfareTeamDataB != null)
		{
			list.AddRange(self.warfareTeamDataB.warfareUnitDataByUIDMap.Values.ToList());
		}
		return list;
	}

	public static void SetUnitTurnEnd(this WarfareGameData self, bool bTurnEnd)
	{
		foreach (WarfareUnitData value in self.warfareTeamDataB.warfareUnitDataByUIDMap.Values)
		{
			value.isTurnEnd = bTurnEnd;
		}
	}
}
