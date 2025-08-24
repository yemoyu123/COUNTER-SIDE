using System.Collections.Generic;
using System.Linq;
using ClientPacket.Mode;
using Cs.Logging;
using NKC;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public class NKMShipManager
{
	public const int LEVELUP_MATERIAL_MAX_COUNT = 3;

	public const int UPGRADE_MATERIAL_MAX_COUNT = 4;

	public const int BUILD_MATERIAL_MAX_COUNT = 4;

	public static Dictionary<int, NKMShipBuildTemplet> m_DicNKMShipBuildTemplet;

	public const string SHIP_LIMITBREAK_TAG = "SHIP_LIMITBREAK";

	public const string SHIP_COMMONDMODULE_TAG = "SHIP_COMMANDMODULE";

	public static Dictionary<int, NKMShipBuildTemplet> DicNKMShipBuildTemplet => m_DicNKMShipBuildTemplet;

	public static NKMShipBuildTemplet GetShipBuildTemplet(int ship_id)
	{
		return NKMTempletContainer<NKMShipBuildTemplet>.Find(ship_id);
	}

	public static NKMShipLevelUpTemplet GetShipLevelupTemplet(int ship_star_grade, int limitBreakeLevel)
	{
		return NKMShipLevelUpTemplet.Find(ship_star_grade, NKM_UNIT_GRADE.NUG_N, limitBreakeLevel);
	}

	public static NKMShipLevelUpTemplet GetShipLevelupTempletByLevel(int level, NKM_UNIT_GRADE unitGrade = NKM_UNIT_GRADE.NUG_N, int ShipLimitBreakGrade = 0)
	{
		return NKMShipLevelUpTemplet.GetShipLevelupTempletByLevel(level, unitGrade, ShipLimitBreakGrade);
	}

	public static NKM_ERROR_CODE CanShipLevelup(NKMUserData userData, NKMUnitData shipData, int nextLevel)
	{
		if (shipData == null || userData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_NOT_EXIST;
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = NKMUnitManager.IsUnitBusy(userData, shipData);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			return nKM_ERROR_CODE;
		}
		if (NKMShipLevelUpTemplet.GetMaxLevel(shipData.GetStarGrade(), shipData.GetUnitGrade(), shipData.m_LimitBreakLevel) < nextLevel)
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_MAX_LEVEL;
		}
		if (nextLevel < 1)
		{
			Log.Debug($"[CanShipLevelUp] Invalid nextLevel Request:{nextLevel}, currentShipLevel:{shipData.m_UnitLevel}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 1004);
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_INVALID_LEVEL;
		}
		if (nextLevel < shipData.m_UnitLevel)
		{
			Log.Debug($"[CanShipLevelUp] Invalid nextLevel Request:{nextLevel}, currentShipLevel:{shipData.m_UnitLevel}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 1009);
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_INVALID_LEVEL;
		}
		foreach (KeyValuePair<int, int> item in GetMaterialListInLevelup(shipData.m_UnitID, shipData.m_UnitLevel, nextLevel, shipData.m_LimitBreakLevel))
		{
			if (userData.m_InventoryData.GetCountMiscItem(item.Key) < item.Value)
			{
				return NKM_ERROR_CODE.NEC_FAIL_INVALID_ITEM_ID;
			}
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE CanShipUpgrade(NKMUserData user_data, NKMUnitData ship_data, int next_ship_id)
	{
		if (NKMUnitManager.GetUnitTempletBase(next_ship_id) == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_INVALID_SHIP_ID;
		}
		if (ship_data == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_INVALID_SHIP_UID;
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = NKMUnitManager.IsUnitBusy(user_data, ship_data);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			return nKM_ERROR_CODE;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(ship_data.m_UnitID);
		if (unitTempletBase == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_GET_UNIT_BASE_TEMPLET_NULL;
		}
		ship_data.GetStarGrade(unitTempletBase);
		int maxLevel = NKMShipLevelUpTemplet.GetMaxLevel(ship_data.GetStarGrade(), ship_data.GetUnitGrade(), ship_data.m_LimitBreakLevel);
		if (ship_data.m_UnitLevel < maxLevel)
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_REMODEL_NOT_ENOUGH_LEVEL;
		}
		NKMShipBuildTemplet shipBuildTemplet = GetShipBuildTemplet(ship_data.m_UnitID);
		if (shipBuildTemplet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_INVALID_SHIP_ID;
		}
		if (shipBuildTemplet.ShipUpgradeTarget1 != next_ship_id && shipBuildTemplet.ShipUpgradeTarget2 != next_ship_id)
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_INVALID_SHIP_ID;
		}
		NKMShipBuildTemplet shipBuildTemplet2 = GetShipBuildTemplet(next_ship_id);
		if (shipBuildTemplet2 == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_INVALID_SHIP_ID;
		}
		NKMInventoryData inventoryData = user_data.m_InventoryData;
		if (inventoryData.GetCountMiscItem(1) < shipBuildTemplet2.ShipUpgradeCredit)
		{
			return NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_ITEM;
		}
		foreach (UpgradeMaterial upgradeMaterial in shipBuildTemplet2.UpgradeMaterialList)
		{
			if (inventoryData.GetCountMiscItem(upgradeMaterial.m_ShipUpgradeMaterial) < upgradeMaterial.m_ShipUpgradeMaterialCount)
			{
				return NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_ITEM;
			}
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE CanShipDivision(NKMUserData user_data, NKMUnitData ship_data)
	{
		if (ship_data == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_INVALID_SHIP_UID;
		}
		if (NKMConst.Ship.BaseShipGroupIds.Contains(ship_data.GetShipGroupId()) && !NKMOpenTagManager.IsOpened("TAG_DELETE_BASIC_SHIP"))
		{
			Log.Error($"Invalid Ship Data. groupId:{ship_data.GetShipGroupId()} shipId:{ship_data.m_UnitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 1108);
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_INVALID_SHIP_ID;
		}
		if (user_data.m_ArmyData.GetDeckDataByShipUID(ship_data.m_UnitUID) != null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_IN_DECK;
		}
		if (ship_data.m_bLock)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_LOCKED;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static Dictionary<int, int> GetMaterialListInLevelup(int shipId, int startLevel, int endLevel, int limitBreakLevel = 0)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(shipId);
		if (unitTempletBase == null)
		{
			return dictionary;
		}
		for (int i = startLevel; i < endLevel; i++)
		{
			if (startLevel == endLevel)
			{
				break;
			}
			NKMShipLevelUpTemplet shipLevelupTempletByLevel = GetShipLevelupTempletByLevel(i, unitTempletBase.m_NKM_UNIT_GRADE, limitBreakLevel);
			foreach (LevelupMaterial shipLevelupMaterial in shipLevelupTempletByLevel.ShipLevelupMaterialList)
			{
				if (!dictionary.TryGetValue(shipLevelupMaterial.m_LevelupMaterialItemID, out var value))
				{
					dictionary.Add(shipLevelupMaterial.m_LevelupMaterialItemID, shipLevelupMaterial.m_LevelupMaterialCount);
				}
				else
				{
					dictionary[shipLevelupMaterial.m_LevelupMaterialItemID] = value + shipLevelupMaterial.m_LevelupMaterialCount;
				}
			}
			if (!dictionary.TryGetValue(1, out var value2))
			{
				dictionary.Add(1, shipLevelupTempletByLevel.ShipUpgradeCredit);
			}
			else
			{
				dictionary[1] = value2 + shipLevelupTempletByLevel.ShipUpgradeCredit;
			}
		}
		return dictionary;
	}

	public static bool LoadFromLUA()
	{
		return true;
	}

	public static bool IsSameKindShip(int shipID, int targetShipID)
	{
		if (shipID == targetShipID)
		{
			return true;
		}
		if (shipID < 10000 && targetShipID < 10000)
		{
			return false;
		}
		return shipID % 1000 == targetShipID % 1000;
	}

	public static bool CheckShipLevel(NKMUserData userData, int shipID, int targetLevel)
	{
		foreach (NKMUnitData value in userData.m_ArmyData.m_dicMyShip.Values)
		{
			if (IsSameKindShip(shipID, value.m_UnitID) && value.m_UnitLevel >= targetLevel)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsMaxLimitBreak(NKMUnitData unitData)
	{
		IEnumerable<NKMShipLimitBreakTemplet> enumerable = NKMTempletContainer<NKMShipLimitBreakTemplet>.Values.Where((NKMShipLimitBreakTemplet x) => x.ShipId == unitData.m_UnitID);
		if (enumerable != null && enumerable.Count() > 0)
		{
			return unitData.m_LimitBreakLevel >= enumerable.Count();
		}
		return false;
	}

	public static bool IsMaxLimitBreak(int shipId, int limitBreakCount)
	{
		IEnumerable<NKMShipLimitBreakTemplet> enumerable = NKMTempletContainer<NKMShipLimitBreakTemplet>.Values.Where((NKMShipLimitBreakTemplet x) => x.ShipId == shipId);
		if (enumerable != null && enumerable.Count() > 0)
		{
			return limitBreakCount >= enumerable.Count();
		}
		return false;
	}

	public static NKMShipLimitBreakTemplet GetShipLimitBreakTemplet(int shipId, int nextLimitBreakLevel)
	{
		if (NKMOpenTagManager.IsOpened("SHIP_LIMITBREAK"))
		{
			return NKMTempletContainer<NKMShipLimitBreakTemplet>.Find((NKMShipLimitBreakTemplet x) => x.ShipId == shipId && x.ShipLimitBreakGrade == nextLimitBreakLevel);
		}
		return null;
	}

	public static bool IsModuleUnlocked(NKMUnitData shipData)
	{
		if (shipData == null)
		{
			return false;
		}
		if (!NKMOpenTagManager.IsOpened("SHIP_LIMITBREAK"))
		{
			return false;
		}
		if (!NKMOpenTagManager.IsOpened("SHIP_COMMANDMODULE"))
		{
			return false;
		}
		if (shipData.m_LimitBreakLevel <= 0)
		{
			return false;
		}
		return true;
	}

	public static NKMUnitTempletBase GetMaxGradeShipTemplet(NKMUnitTempletBase curShipTempletBase)
	{
		int num = curShipTempletBase.m_UnitID;
		while (true)
		{
			NKMShipBuildTemplet shipBuildTemplet = GetShipBuildTemplet(num);
			if (shipBuildTemplet.ShipUpgradeTarget1 <= 0)
			{
				break;
			}
			num = shipBuildTemplet.ShipUpgradeTarget1;
		}
		return NKMUnitManager.GetUnitTempletBase(num);
	}

	public static NKM_ERROR_CODE CanModuleOptionChange(NKMUnitData shipData)
	{
		if (shipData.IsSeized)
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_IS_SEIZED;
		}
		return NKCScenManager.CurrentUserData().m_ArmyData.GetUnitDeckState(shipData) switch
		{
			NKM_DECK_STATE.DECK_STATE_WARFARE => NKM_ERROR_CODE.NEC_FAIL_WARFARE_DOING, 
			NKM_DECK_STATE.DECK_STATE_DIVE => NKM_ERROR_CODE.NEC_FAIL_DIVE_DOING, 
			_ => NKM_ERROR_CODE.NEC_OK, 
		};
	}

	public static NKM_ERROR_CODE CanShipLimitBreak(NKMUserData user_data, NKMUnitData ship_data, long costShipUID)
	{
		if (ship_data == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_NOT_EXISTS;
		}
		if (costShipUID < 0)
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_LIMITBREAK_INVALID_CONSUMED_SHIP_ID;
		}
		if (NKMUnitManager.GetUnitTempletBase(ship_data.m_UnitID) == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_GET_UNIT_BASE_TEMPLET_NULL;
		}
		int maxLevel = NKMShipLevelUpTemplet.GetMaxLevel(ship_data.GetStarGrade(), ship_data.GetUnitGrade(), ship_data.m_LimitBreakLevel);
		if (ship_data.m_UnitLevel < maxLevel)
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_REMODEL_NOT_ENOUGH_LEVEL;
		}
		NKMUnitData shipFromUID = user_data.m_ArmyData.GetShipFromUID(costShipUID);
		if (shipFromUID == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_LIMITBREAK_INVALID_CONSUMED_SHIP_ID;
		}
		if (shipFromUID.m_bLock)
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_LIMITBREAK_LOCKED_CONSUMED_SHIP;
		}
		NKMShipLimitBreakTemplet shipLimitBreakTemplet = GetShipLimitBreakTemplet(ship_data.m_UnitID, ship_data.m_LimitBreakLevel + 1);
		if (shipLimitBreakTemplet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_LIMITBREAK_TEMPLET;
		}
		if (!shipLimitBreakTemplet.ListMaterialShipId.Contains(shipFromUID.m_UnitID))
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_LIMITBREAK_INVALID_CONSUMED_SHIP_ID;
		}
		for (int i = 0; i < shipLimitBreakTemplet.ShipLimitBreakItems.Count; i++)
		{
			if (user_data.m_InventoryData.GetCountMiscItem(shipLimitBreakTemplet.ShipLimitBreakItems[i].ItemId) < shipLimitBreakTemplet.ShipLimitBreakItems[i].Count)
			{
				return NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_ITEM;
			}
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static bool CanUseForShipLimitBreakMaterial(NKMUnitData shipData, int costShipId)
	{
		return GetShipLimitBreakTemplet(shipData.m_UnitID, shipData.m_LimitBreakLevel + 1)?.ListMaterialShipId.Contains(costShipId) ?? false;
	}

	public static bool HasNKMShipCommandModuleTemplet(NKMUnitData shipData)
	{
		if (shipData == null)
		{
			return false;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(shipData.m_UnitID);
		if (unitTempletBase == null)
		{
			return false;
		}
		return GetNKMShipCommandModuleTemplet(unitTempletBase.m_NKM_UNIT_STYLE_TYPE, unitTempletBase.m_NKM_UNIT_GRADE, shipData.m_LimitBreakLevel) != null;
	}

	public static NKMShipCommandModuleTemplet GetNKMShipCommandModuleTemplet(NKM_UNIT_STYLE_TYPE unitStyleType, NKM_UNIT_GRADE unitGrade, int moduleNum)
	{
		NKMShipCommandModuleTemplet nKMShipCommandModuleTemplet = NKMTempletContainer<NKMShipCommandModuleTemplet>.Values.FirstOrDefault((NKMShipCommandModuleTemplet e) => e.IsTargetTemplet(unitStyleType, unitGrade, moduleNum) && e.LimitBreakGrade == moduleNum);
		if (nKMShipCommandModuleTemplet == null)
		{
			return null;
		}
		return nKMShipCommandModuleTemplet;
	}

	public static bool IsMaxStat(int id, NKMShipCmdSlot slot)
	{
		if (slot == null || slot.statType == NKM_STAT_TYPE.NST_RANDOM)
		{
			return false;
		}
		int statGroupId = 0;
		IReadOnlyList<NKMCommandModulePassiveTemplet> passiveListsByGroupId = NKMShipModuleGroupTemplet.GetPassiveListsByGroupId(id);
		if (passiveListsByGroupId != null)
		{
			for (int i = 0; i < passiveListsByGroupId.Count; i++)
			{
				if (passiveListsByGroupId[i].RoleTypes.SetEquals(slot.targetRoleType) && passiveListsByGroupId[i].StyleTypes.SetEquals(slot.targetStyleType))
				{
					statGroupId = passiveListsByGroupId[i].StatGroupId;
				}
			}
		}
		IReadOnlyList<NKMCommandModuleRandomStatTemplet> statListsByGroupId = NKMShipModuleGroupTemplet.GetStatListsByGroupId(statGroupId);
		if (statListsByGroupId != null)
		{
			for (int j = 0; j < statListsByGroupId.Count; j++)
			{
				if (statListsByGroupId[j].StatType != slot.statType)
				{
					continue;
				}
				if (NKCUtilString.IsNameReversedIfNegative(slot.statType))
				{
					if (!(statListsByGroupId[j].MinStatValue < slot.statValue))
					{
						return true;
					}
				}
				else if (!(statListsByGroupId[j].MaxStatValue > slot.statValue))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static int GetShipMaxLevel(NKMUnitData shipData)
	{
		return NKMShipLevelUpTemplet.GetMaxLevel(shipData.GetStarGrade(), shipData.GetUnitGrade(), shipData.m_LimitBreakLevel);
	}

	public static int GetMaxLevelShipID(int ShipID)
	{
		if ((int)((float)ShipID * 0.001f % 10f) == 100)
		{
			return ShipID;
		}
		return 26000 + ShipID % 1000;
	}

	public static int GetMinLevelShipID(int ShipID)
	{
		return 21000 + ShipID % 1000;
	}

	public static bool IsPercentStat(NKMCommandModuleRandomStatTemplet statTemplet)
	{
		if (statTemplet != null)
		{
			return NKMUnitStatManager.IsPercentStat(statTemplet.StatType);
		}
		return false;
	}

	public static bool IsPercentStat(NKM_STAT_TYPE statType, float statFactor)
	{
		if (NKMUnitStatManager.IsPercentStat(statType))
		{
			return true;
		}
		if (statFactor > 0f)
		{
			return true;
		}
		return false;
	}

	public static NKM_ERROR_CODE CanShipBuild(NKMUserData user_data, int ship_id)
	{
		if (!user_data.m_ArmyData.CanGetMoreShip(0))
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_FULL;
		}
		if (NKMUnitManager.GetUnitTempletBase(ship_id) == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_INVALID_SHIP_ID;
		}
		NKMShipBuildTemplet shipBuildTemplet = GetShipBuildTemplet(ship_id);
		if (shipBuildTemplet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_INVALID_SHIP_ID;
		}
		foreach (BuildMaterial buildMaterial in shipBuildTemplet.BuildMaterialList)
		{
			if (user_data.m_InventoryData.GetCountMiscItem(buildMaterial.m_ShipBuildMaterialID) < buildMaterial.m_ShipBuildMaterialCount)
			{
				return NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_ITEM;
			}
		}
		if (!CanUnlockShip(user_data, shipBuildTemplet))
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_NOT_UNLOCKED;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static bool CanUnlockShip(NKMUserData userData, NKMShipBuildTemplet shipBuildTemplet)
	{
		if (shipBuildTemplet == null)
		{
			return false;
		}
		if (userData == null)
		{
			return false;
		}
		switch (shipBuildTemplet.ShipBuildUnlockType)
		{
		case NKMShipBuildTemplet.BuildUnlockType.BUT_ALWAYS:
			return true;
		case NKMShipBuildTemplet.BuildUnlockType.BUT_UNABLE:
			return false;
		case NKMShipBuildTemplet.BuildUnlockType.BUT_PLAYER_LEVEL:
			return shipBuildTemplet.UnlockValue <= userData.m_UserLevel;
		case NKMShipBuildTemplet.BuildUnlockType.BUT_DUNGEON_CLEAR:
			return userData.CheckDungeonClear(shipBuildTemplet.UnlockValue);
		case NKMShipBuildTemplet.BuildUnlockType.BUT_SHIP_GET:
			return userData.m_ArmyData.IsCollectedUnit(shipBuildTemplet.UnlockValue);
		case NKMShipBuildTemplet.BuildUnlockType.BUT_WARFARE_CLEAR:
			return userData.CheckWarfareClear(shipBuildTemplet.UnlockValue);
		case NKMShipBuildTemplet.BuildUnlockType.BUT_SHIP_LV100:
			return CheckShipLevel(userData, shipBuildTemplet.UnlockValue, 100);
		case NKMShipBuildTemplet.BuildUnlockType.BUT_WORLDMAP_CITY_COUNT:
			return userData.m_WorldmapData.GetUnlockedCityCount() >= shipBuildTemplet.UnlockValue;
		case NKMShipBuildTemplet.BuildUnlockType.BUT_SHADOW_CLEAR:
		{
			NKMShadowPalaceTemplet shadowPalaceTemplet = NKMTempletContainer<NKMShadowPalaceTemplet>.Find(shipBuildTemplet.UnlockValue);
			if (shadowPalaceTemplet != null)
			{
				List<NKMShadowBattleTemplet> battleTemplets = NKMShadowPalaceManager.GetBattleTemplets(shadowPalaceTemplet.PALACE_ID);
				if (battleTemplets != null)
				{
					NKMPalaceData nKMPalaceData = userData.m_ShadowPalace.palaceDataList.Find((NKMPalaceData x) => x.palaceId == shadowPalaceTemplet.PALACE_ID);
					if (nKMPalaceData != null)
					{
						foreach (NKMShadowBattleTemplet battleTemplet in battleTemplets)
						{
							NKMPalaceDungeonData nKMPalaceDungeonData = nKMPalaceData.dungeonDataList.Find((NKMPalaceDungeonData x) => x.dungeonId == battleTemplet.DUNGEON_ID);
							if (nKMPalaceDungeonData == null)
							{
								return false;
							}
							if (nKMPalaceDungeonData.bestTime == 0)
							{
								return false;
							}
						}
						return true;
					}
				}
			}
			return false;
		}
		default:
			Log.Error("Undefined Unlock type", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMShipSkillManagerEx.cs", 345);
			return false;
		}
	}

	public static int GetShipCountByGroupID(int groupID)
	{
		return NKCScenManager.CurrentUserData().m_ArmyData.GetSameKindShipCountFromID(groupID);
	}

	public static int GetShipGroupID(long shipUid)
	{
		return NKCScenManager.CurrentArmyData().GetShipFromUID(shipUid).GetShipGroupId();
	}
}
