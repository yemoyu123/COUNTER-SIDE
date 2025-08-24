using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Common;
using ClientPacket.Mode;
using Cs.Core.Util;
using Cs.Logging;
using NKC;
using NKM.Shop;
using NKM.Templet;

namespace NKM;

public static class NKMContentUnlockManager
{
	public static bool IsValidMissionUnlockType(UnlockInfo unlockInfo)
	{
		return IsValidMissionUnlockType(unlockInfo.eReqType, unlockInfo.reqValue, unlockInfo.reqValueStr, unlockInfo.reqDateTime);
	}

	private static bool IsValidMissionUnlockType(STAGE_UNLOCK_REQ_TYPE reqType, int reqValue, string reqValueStr, DateTime reqDateTime)
	{
		switch (reqType)
		{
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE:
			if (NKMWarfareTemplet.Find(reqValue) == null)
			{
				Log.Error($"[ContentUnlock] 전역 클리어 대상이 존재하지 않음 m_StageUnlockReqType : {reqType}, m_MissionUnlockReqValue : {reqValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMContentUnlockManager.cs", 226);
				return false;
			}
			break;
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_GET:
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_LEVEL_20:
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_LEVEL_25:
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_LEVEL_50:
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_LEVEL_80:
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_LEVEL_100:
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_LIMIT_GARDE_1:
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_LIMIT_GARDE_2:
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_LIMIT_GARDE_3:
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_DEVOTION:
			if (NKMUnitManager.GetUnitTempletBase(reqValue) == null)
			{
				Log.Error($"[ContentUnlock] 유닛 획득 대상이 존재하지 않음 m_StageUnlockReqType : {reqType}, m_MissionUnlockReqValue : {reqValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMContentUnlockManager.cs", 244);
				return false;
			}
			break;
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON:
			if (NKMDungeonManager.GetDungeonTempletBase(reqValue) == null)
			{
				Log.Error($"[ContentUnlock] 던전 클리어 대상이 존재하지 않음 m_StageUnlockReqType : {reqType}, m_MissionUnlockReqValue : {reqValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMContentUnlockManager.cs", 254);
				return false;
			}
			break;
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_PHASE:
			if (NKMPhaseTemplet.Find(reqValue) == null)
			{
				Log.Error($"[ContentUnlock] 페이즈 클리어 대상이 존재하지 않음 m_StageUnlockReqType : {reqType}, m_MissionUnlockReqValue : {reqValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMContentUnlockManager.cs", 263);
				return false;
			}
			break;
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DIVE:
			if (NKMDiveTemplet.Find(reqValue) == null)
			{
				Log.Error($"[ContentUnlock] 컨텐츠 해제조건(다이브 클리어) 대상이 존재하지 않음 m_StageUnlockReqType : {reqType}, m_MissionUnlockReqValue : {reqValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMContentUnlockManager.cs", 272);
				return false;
			}
			break;
		case STAGE_UNLOCK_REQ_TYPE.SURT_START_DATETIME:
			if (reqDateTime <= DateTime.MinValue)
			{
				Log.Error($"[ContentUnlock] 컨텐츠 해제조건(시작시간 설정) 시간이 존재하지 않거나 잘못입력됨 m_StageUnlockReqType : {reqType}, m_MissionUnlockReqValueStr : {reqValueStr}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMContentUnlockManager.cs", 282);
				return false;
			}
			break;
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE_START_DATETIME:
			if (IsValidMissionUnlockType(STAGE_UNLOCK_REQ_TYPE.SURT_START_DATETIME, reqValue, reqValueStr, reqDateTime) && IsValidMissionUnlockType(STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE, reqValue, reqValueStr, reqDateTime))
			{
				return true;
			}
			return false;
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON_START_DATETIME:
			if (IsValidMissionUnlockType(STAGE_UNLOCK_REQ_TYPE.SURT_START_DATETIME, reqValue, reqValueStr, reqDateTime) && IsValidMissionUnlockType(STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON, reqValue, reqValueStr, reqDateTime))
			{
				return true;
			}
			return false;
		case STAGE_UNLOCK_REQ_TYPE.SURT_RETURN_USER:
		{
			if (!string.IsNullOrEmpty(reqValueStr) && !Enum.TryParse<ReturningUserType>(reqValueStr, out var _))
			{
				Log.Error($"[ContentUnlock] 컨텐츠 해제조건(리턴유저) ReturningUserType 타입이 존재하지 않거나 잘못입력됨 m_StageUnlockReqType : {reqType}, m_MissionUnlockReqValueStr : {reqValueStr}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMContentUnlockManager.cs", 312);
				return false;
			}
			break;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_LAST_STAGE:
			if (NKMStageTempletV2.Find(reqValue) != null)
			{
				return true;
			}
			return false;
		case STAGE_UNLOCK_REQ_TYPE.SURT_SHOP_BUY_ITEM_ALL:
		{
			ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(reqValue);
			if (shopItemTemplet == null)
			{
				Log.Error($"[ContentUnlock] 컨텐츠 해제조건 SURT_SHOP_BUY_ITEM_ALL : 상품이 존재하지 않음. m_StageUnlockReqType : {reqType}, m_MissionUnlockReqValue : {reqValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMContentUnlockManager.cs", 331);
				return false;
			}
			if (shopItemTemplet.resetType == SHOP_RESET_TYPE.Unlimited)
			{
				Log.Error($"[ContentUnlock] 컨텐츠 해제조건 SURT_SHOP_BUY_ITEM_ALL : 목표 상품이 무한 구매 가능함. m_StageUnlockReqType : {reqType}, m_MissionUnlockReqValue : {reqValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMContentUnlockManager.cs", 337);
				return false;
			}
			break;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_INTERVAL:
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE_INTERVAL:
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON_INTERVAL:
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_PHASE_INTERVAL:
			if (NKMIntervalTemplet.Find(reqValueStr) == null)
			{
				Log.Error($"[ContentUnlock] 인터벌 템플릿이 존재하지 않음. reqType : {reqType}, reqValueStr : {reqValueStr}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMContentUnlockManager.cs", 349);
				return false;
			}
			break;
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_TRIM:
		{
			NKMTrimTemplet nKMTrimTemplet = NKMTrimTemplet.Find(reqValue);
			if (nKMTrimTemplet == null)
			{
				Log.Error($"[ContentUnlock] 컨텐츠 해제조건 SURT_CLEAR_TRIM : 트리밍 목표 던전 {reqValue}의 템플릿이 존재하지 않음", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMContentUnlockManager.cs", 359);
				return false;
			}
			if (!int.TryParse(reqValueStr, out var result))
			{
				Log.Error("[ContentUnlock] 컨텐츠 해제조건 SURT_CLEAR_TRIM : 트리밍 목표 레벨 " + reqValueStr + "의 파싱 실패. 반드시 숫자 형식이어야 함", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMContentUnlockManager.cs", 365);
				return false;
			}
			if (nKMTrimTemplet.MaxTrimLevel < result)
			{
				Log.Error($"[ContentUnlock] 컨텐츠 해제조건 SURT_CLEAR_TRIM : 트리밍 목표 레벨 {reqValueStr}이 해당 트리밍 던전의 최대 레벨 {nKMTrimTemplet.MaxTrimLevel}보다 큼", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMContentUnlockManager.cs", 371);
				return false;
			}
			break;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_DIVE_HISTORY_CLEARED:
			if (NKMDiveTemplet.Find(reqValue) == null)
			{
				Log.Error($"[ContentUnlock] 컨텐츠 해제조건(다이브 클리어 기록) 대상이 존재하지 않음 m_StageUnlockReqType : {reqType}, m_MissionUnlockReqValue : {reqValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMContentUnlockManager.cs", 380);
				return false;
			}
			break;
		}
		return true;
	}

	public static bool IsContentUnlocked(NKMUserData cNKMUserData, in UnlockInfo unlockInfo, out bool bAdmin)
	{
		if (IsContentUnlocked(cNKMUserData, in unlockInfo, ignoreSuperUser: true))
		{
			bAdmin = false;
			return true;
		}
		if (cNKMUserData.IsSuperUser())
		{
			bAdmin = true;
			return true;
		}
		bAdmin = false;
		return false;
	}

	public static bool IsContentUnlocked(NKMUserData cNKMUserData, in List<UnlockInfo> lstUnlockInfo, out bool bAdmin)
	{
		foreach (UnlockInfo item in lstUnlockInfo)
		{
			if (!IsContentUnlocked(cNKMUserData, item, out var bAdmin2))
			{
				bAdmin = false;
				return false;
			}
			if (bAdmin2)
			{
				bAdmin = true;
				return true;
			}
		}
		bAdmin = false;
		return true;
	}

	public static bool IsContentUnlocked(NKMUserData cNKMUserData, in List<UnlockInfo> lstUnlockInfo, bool ignoreSuperUser = false)
	{
		foreach (UnlockInfo item in lstUnlockInfo)
		{
			if (!IsContentUnlocked(cNKMUserData, item, ignoreSuperUser))
			{
				return false;
			}
		}
		return true;
	}

	public static bool IsContentUnlocked(NKMUserData cNKMUserData, in UnlockInfo unlockInfo, bool ignoreSuperUser = false)
	{
		if (cNKMUserData == null)
		{
			return false;
		}
		if (cNKMUserData.IsSuperUser() && !ignoreSuperUser)
		{
			return true;
		}
		switch (unlockInfo.eReqType)
		{
		case STAGE_UNLOCK_REQ_TYPE.SURT_ALWAYS_UNLOCKED:
			return true;
		case STAGE_UNLOCK_REQ_TYPE.SURT_ALWAYS_LOCKED:
		case STAGE_UNLOCK_REQ_TYPE.SURT_ALWAYS_HIDDEN:
			return false;
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE:
			return cNKMUserData.CheckWarfareClear(unlockInfo.reqValue);
		case STAGE_UNLOCK_REQ_TYPE.SURT_CITY_COUNT:
			return cNKMUserData.m_WorldmapData.GetUnlockedCityCount() >= unlockInfo.reqValue;
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_GET:
			return cNKMUserData.m_ArmyData.IsCollectedUnit(unlockInfo.reqValue);
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_LIMIT_GARDE_1:
			return cNKMUserData.m_ArmyData.SearchUnitByID(NKM_UNIT_TYPE.NUT_NORMAL, unlockInfo.reqValue, NKMArmyData.UNIT_SEARCH_OPTION.LimitLevel, 1);
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_LIMIT_GARDE_2:
			return cNKMUserData.m_ArmyData.SearchUnitByID(NKM_UNIT_TYPE.NUT_NORMAL, unlockInfo.reqValue, NKMArmyData.UNIT_SEARCH_OPTION.LimitLevel, 2);
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_LIMIT_GARDE_3:
			return cNKMUserData.m_ArmyData.SearchUnitByID(NKM_UNIT_TYPE.NUT_NORMAL, unlockInfo.reqValue, NKMArmyData.UNIT_SEARCH_OPTION.LimitLevel, 3);
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_DEVOTION:
			return cNKMUserData.m_ArmyData.SearchUnitByID(NKM_UNIT_TYPE.NUT_NORMAL, unlockInfo.reqValue, NKMArmyData.UNIT_SEARCH_OPTION.Devotion, 0);
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_LEVEL_20:
			return cNKMUserData.m_ArmyData.SearchUnitByID(NKM_UNIT_TYPE.NUT_NORMAL, unlockInfo.reqValue, NKMArmyData.UNIT_SEARCH_OPTION.Level, 20);
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_LEVEL_25:
			return cNKMUserData.m_ArmyData.SearchUnitByID(NKM_UNIT_TYPE.NUT_NORMAL, unlockInfo.reqValue, NKMArmyData.UNIT_SEARCH_OPTION.Level, 25);
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_LEVEL_50:
			return cNKMUserData.m_ArmyData.SearchUnitByID(NKM_UNIT_TYPE.NUT_NORMAL, unlockInfo.reqValue, NKMArmyData.UNIT_SEARCH_OPTION.Level, 50);
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_LEVEL_80:
			return cNKMUserData.m_ArmyData.SearchUnitByID(NKM_UNIT_TYPE.NUT_NORMAL, unlockInfo.reqValue, NKMArmyData.UNIT_SEARCH_OPTION.Level, 80);
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_LEVEL_100:
			return cNKMUserData.m_ArmyData.SearchUnitByID(NKM_UNIT_TYPE.NUT_NORMAL, unlockInfo.reqValue, NKMArmyData.UNIT_SEARCH_OPTION.Level, 100);
		case STAGE_UNLOCK_REQ_TYPE.SURT_PLAYER_LEVEL:
			return cNKMUserData.m_UserLevel >= unlockInfo.reqValue;
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON:
			return cNKMUserData.CheckDungeonClear(unlockInfo.reqValue);
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_PHASE:
			return NKCPhaseManager.CheckPhaseClear(unlockInfo.reqValue);
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DIVE:
			return cNKMUserData.CheckDiveClear(unlockInfo.reqValue);
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_STAGE:
		{
			NKMStageTempletV2 nKMStageTempletV3 = NKMStageTempletV2.Find(unlockInfo.reqValue);
			if (nKMStageTempletV3 != null)
			{
				return nKMStageTempletV3.m_STAGE_TYPE switch
				{
					STAGE_TYPE.ST_DUNGEON => cNKMUserData.CheckDungeonClear(nKMStageTempletV3.m_StageBattleStrID), 
					STAGE_TYPE.ST_WARFARE => cNKMUserData.CheckWarfareClear(nKMStageTempletV3.m_StageBattleStrID), 
					STAGE_TYPE.ST_PHASE => NKCPhaseManager.CheckPhaseStageClear(nKMStageTempletV3), 
					_ => false, 
				};
			}
			Log.Error($"NKMStageTemplet is null : stageId : {unlockInfo.reqValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMContentUnlockManagerEx.cs", 184);
			return false;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_TRIM:
		{
			if (!int.TryParse(unlockInfo.reqValueStr, out var result3))
			{
				return false;
			}
			return cNKMUserData.TrimData.GetTrimClearData(unlockInfo.reqValue, result3) != null;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_START_DATETIME:
			return unlockInfo.reqDateTime < ServiceTime.Recent;
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE_START_DATETIME:
			if (cNKMUserData.CheckWarfareClear(unlockInfo.reqValue))
			{
				return unlockInfo.reqDateTime < ServiceTime.Recent;
			}
			return false;
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON_START_DATETIME:
			if (cNKMUserData.CheckDungeonClear(unlockInfo.reqValue))
			{
				return unlockInfo.reqDateTime < ServiceTime.Recent;
			}
			return false;
		case STAGE_UNLOCK_REQ_TYPE.SURT_INTERVAL:
			return NKCSynchronizedTime.IsEventTime(unlockInfo.reqValueStr);
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON_INTERVAL:
			if (NKCSynchronizedTime.IsEventTime(unlockInfo.reqValueStr))
			{
				return cNKMUserData.CheckDungeonClear(unlockInfo.reqValue);
			}
			return false;
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE_INTERVAL:
			if (NKCSynchronizedTime.IsEventTime(unlockInfo.reqValueStr))
			{
				return cNKMUserData.CheckWarfareClear(unlockInfo.reqValue);
			}
			return false;
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_PHASE_INTERVAL:
			if (NKCSynchronizedTime.IsEventTime(unlockInfo.reqValueStr))
			{
				return NKCPhaseManager.CheckPhaseClear(unlockInfo.reqValue);
			}
			return false;
		case STAGE_UNLOCK_REQ_TYPE.SURT_REGISTER_DATE:
			return ServiceTime.FromUtcTime(cNKMUserData.m_NKMUserDateData.m_RegisterTime) >= unlockInfo.reqDateTime;
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_PALACE:
		{
			int palaceId = unlockInfo.reqValue;
			List<NKMShadowBattleTemplet> battleTemplets = NKMShadowPalaceManager.GetBattleTemplets(palaceId);
			if (battleTemplets == null)
			{
				return false;
			}
			NKMPalaceData nKMPalaceData = cNKMUserData.m_ShadowPalace.palaceDataList.Find((NKMPalaceData e) => e.palaceId == palaceId);
			if (nKMPalaceData == null)
			{
				return false;
			}
			if (battleTemplets.Count != nKMPalaceData.dungeonDataList.Where((NKMPalaceDungeonData e) => e.bestTime != 0).Count())
			{
				return false;
			}
			return true;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_NEWBIE_USER:
		{
			if (int.TryParse(unlockInfo.reqValueStr, out var result2))
			{
				DateTime registerTime = cNKMUserData.m_NKMUserDateData.m_RegisterTime;
				if (registerTime.AddDays(result2) < ServiceTime.ToUtcTime(ServiceTime.Recent))
				{
					return cNKMUserData.m_NKMUserDateData.m_RegisterTime.AddDays(unlockInfo.reqValue) > ServiceTime.ToUtcTime(ServiceTime.Recent);
				}
				return false;
			}
			return cNKMUserData.m_NKMUserDateData.m_RegisterTime.AddDays(unlockInfo.reqValue) > ServiceTime.ToUtcTime(ServiceTime.Recent);
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_MISSION_CLEAR:
			return cNKMUserData.m_MissionData.GetCompletedMissionData(unlockInfo.reqValue) != null;
		case STAGE_UNLOCK_REQ_TYPE.SURT_MISSION_TAB_ALL_CLEAR:
			if (cNKMUserData.m_MissionData.IsTabComplete(unlockInfo.reqValue))
			{
				return true;
			}
			foreach (NKMMissionData allMission in cNKMUserData.m_MissionData.GetAllMissionList(unlockInfo.reqValue))
			{
				if (!allMission.IsComplete)
				{
					return false;
				}
			}
			cNKMUserData.m_MissionData.SetTabComplete(unlockInfo.reqValue);
			return true;
		case STAGE_UNLOCK_REQ_TYPE.SURT_CONTENT_TAG:
			return NKMContentsVersionManager.HasTag(unlockInfo.reqValueStr);
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNIT_COLLECTION_RARITY_COUNT:
		{
			if (!Enum.TryParse<NKM_UNIT_GRADE>(unlockInfo.reqValueStr, out var result))
			{
				return false;
			}
			int num = 0;
			foreach (int item in cNKMUserData.m_ArmyData.m_illustrateUnit)
			{
				if (NKMUnitManager.GetUnitTempletBase(item).m_NKM_UNIT_GRADE == result)
				{
					num++;
					if (num >= unlockInfo.reqValue)
					{
						return true;
					}
				}
			}
			return false;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_PVP_RANK_SCORE:
			return cNKMUserData.m_PvpData.Score >= unlockInfo.reqValue;
		case STAGE_UNLOCK_REQ_TYPE.SURT_PVP_RANK_SCORE_RECORD:
			return cNKMUserData.m_PvpData.MaxScore >= unlockInfo.reqValue;
		case STAGE_UNLOCK_REQ_TYPE.SURT_PVP_ASYNC_SCORE:
			return cNKMUserData.m_AsyncData.Score >= unlockInfo.reqValue;
		case STAGE_UNLOCK_REQ_TYPE.SURT_PVP_ASYNC_SCORE_RECORD:
			return cNKMUserData.m_AsyncData.MaxScore >= unlockInfo.reqValue;
		case STAGE_UNLOCK_REQ_TYPE.SURT_GUILD_LEVEL:
			if (NKCGuildManager.MyGuildData != null)
			{
				return NKCGuildManager.MyGuildData.guildLevel >= unlockInfo.reqValue;
			}
			return false;
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_LAST_STAGE:
		{
			NKMStageTempletV2 nKMStageTempletV2 = NKMStageTempletV2.Find(unlockInfo.reqValue);
			if (nKMStageTempletV2 == null)
			{
				return false;
			}
			switch (nKMStageTempletV2.m_STAGE_TYPE)
			{
			case STAGE_TYPE.ST_WARFARE:
				if (nKMStageTempletV2.WarfareTemplet != null)
				{
					return IsContentUnlocked(cNKMUserData, new UnlockInfo(STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE, nKMStageTempletV2.WarfareTemplet.m_WarfareID));
				}
				break;
			case STAGE_TYPE.ST_DUNGEON:
				if (nKMStageTempletV2.DungeonTempletBase != null)
				{
					return IsContentUnlocked(cNKMUserData, new UnlockInfo(STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON, nKMStageTempletV2.DungeonTempletBase.m_DungeonID));
				}
				break;
			case STAGE_TYPE.ST_PHASE:
				if (nKMStageTempletV2.PhaseTemplet != null)
				{
					return IsContentUnlocked(cNKMUserData, new UnlockInfo(STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_PHASE, nKMStageTempletV2.PhaseTemplet.Id));
				}
				break;
			}
			return false;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_SHOP_BUY_ITEM_ALL:
			return NKCShopManager.GetBuyCountLeft(unlockInfo.reqValue) == 0;
		case STAGE_UNLOCK_REQ_TYPE.SURT_MISSION_TAB_UNLOCKED:
			return NKMMissionManager.CheckMissionTabUnlocked(unlockInfo.reqValue, cNKMUserData);
		case STAGE_UNLOCK_REQ_TYPE.SURT_DIVE_HISTORY_CLEARED:
			return cNKMUserData.CheckDiveHistory(unlockInfo.reqValue);
		case STAGE_UNLOCK_REQ_TYPE.SURT_OPEN_SECTION:
			return cNKMUserData.OfficeData.IsOpenedSection(unlockInfo.reqValue);
		case STAGE_UNLOCK_REQ_TYPE.SURT_OPEN_ROOM:
			return cNKMUserData.OfficeData.IsOpenedRoom(unlockInfo.reqValue);
		case STAGE_UNLOCK_REQ_TYPE.SURT_UNLOCK_STAGE:
		{
			NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(unlockInfo.reqValue);
			if (nKMStageTempletV == null)
			{
				return false;
			}
			return IsContentUnlocked(NKCScenManager.CurrentUserData(), in nKMStageTempletV.m_UnlockInfo);
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_SKIN_GET:
			if (unlockInfo.reqValue <= 0)
			{
				return false;
			}
			return cNKMUserData.m_InventoryData.HasItemSkin(unlockInfo.reqValue);
		case STAGE_UNLOCK_REQ_TYPE.SURT_HISTORY_BIRTHDAY:
			if (cNKMUserData.m_BirthDayData == null)
			{
				return false;
			}
			return unlockInfo.reqValue <= cNKMUserData.m_BirthDayData.Years;
		default:
			Log.Debug("NKMContentUnlockManager::IsContentUnlocked() Undefined Type:" + unlockInfo.eReqType, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMContentUnlockManagerEx.cs", 420);
			return false;
		}
	}

	public static ContentUnlockStatus CheckMissionUnlocked(NKMUserData userData, STAGE_BASIC_UNLOCK_TYPE basicUnlockType, UnlockInfo unlockInfo)
	{
		if (IsContentUnlocked(userData, in unlockInfo))
		{
			return ContentUnlockStatus.Unlocked;
		}
		if (basicUnlockType == STAGE_BASIC_UNLOCK_TYPE.SBUT_LOCK)
		{
			return ContentUnlockStatus.Locked_Visible;
		}
		return ContentUnlockStatus.Locked_Invisible;
	}

	public static bool IsTimeLimitCondition(UnlockInfo unlockInfo)
	{
		return GetConditionTimeLimit(unlockInfo) < DateTime.MaxValue;
	}

	public static bool IsStarted(UnlockInfo unlockInfo)
	{
		return GetConditionStartTime(unlockInfo) <= NKCSynchronizedTime.GetServerUTCTime();
	}

	public static bool IsStarted(List<UnlockInfo> unlockInfoList)
	{
		bool flag = true;
		foreach (UnlockInfo unlockInfo in unlockInfoList)
		{
			DateTime conditionStartTime = GetConditionStartTime(unlockInfo);
			flag &= conditionStartTime <= NKCSynchronizedTime.GetServerUTCTime();
		}
		return flag;
	}

	public static DateTime GetConditionStartTime(List<UnlockInfo> lstUnlockInfo)
	{
		DateTime dateTime = DateTime.MinValue;
		foreach (UnlockInfo item in lstUnlockInfo)
		{
			DateTime conditionStartTime = GetConditionStartTime(item);
			if (conditionStartTime > dateTime)
			{
				dateTime = conditionStartTime;
			}
		}
		return dateTime;
	}

	public static DateTime GetConditionStartTime(UnlockInfo unlockInfo)
	{
		switch (unlockInfo.eReqType)
		{
		case STAGE_UNLOCK_REQ_TYPE.SURT_NEWBIE_USER:
		{
			if (int.TryParse(unlockInfo.reqValueStr, out var result))
			{
				DateTime registerTime = NKCScenManager.CurrentUserData().m_NKMUserDateData.m_RegisterTime;
				return registerTime.AddDays(result);
			}
			return NKCScenManager.CurrentUserData().m_NKMUserDateData.m_RegisterTime;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_RETURN_USER:
		{
			if (!string.IsNullOrEmpty(unlockInfo.reqValueStr))
			{
				if (Enum.TryParse<ReturningUserType>(unlockInfo.reqValueStr, out var result2))
				{
					return NKCScenManager.CurrentUserData().GetReturnStartDate(result2);
				}
				return DateTime.MaxValue;
			}
			DateTime dateTime = DateTime.MaxValue;
			{
				foreach (ReturningUserType value in Enum.GetValues(typeof(ReturningUserType)))
				{
					DateTime returnStartDate = NKCScenManager.CurrentUserData().GetReturnStartDate(value);
					if (dateTime > returnStartDate)
					{
						dateTime = returnStartDate;
					}
				}
				return dateTime;
			}
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_START_DATETIME:
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE_START_DATETIME:
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON_START_DATETIME:
			return ServiceTime.ToUtcTime(unlockInfo.reqDateTime);
		case STAGE_UNLOCK_REQ_TYPE.SURT_INTERVAL:
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE_INTERVAL:
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON_INTERVAL:
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_PHASE_INTERVAL:
		{
			NKMIntervalTemplet nKMIntervalTemplet = NKMIntervalTemplet.Find(unlockInfo.reqValueStr);
			if (nKMIntervalTemplet != null)
			{
				return nKMIntervalTemplet.GetStartDateUtc();
			}
			break;
		}
		}
		return DateTime.MinValue;
	}

	public static DateTime GetConditionTimeLimit(UnlockInfo unlockInfo)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		switch (unlockInfo.eReqType)
		{
		case STAGE_UNLOCK_REQ_TYPE.SURT_NEWBIE_USER:
			return nKMUserData.m_NKMUserDateData.m_RegisterTime.AddDays(unlockInfo.reqValue);
		case STAGE_UNLOCK_REQ_TYPE.SURT_RETURN_USER:
		{
			if (!string.IsNullOrEmpty(unlockInfo.reqValueStr))
			{
				if (Enum.TryParse<ReturningUserType>(unlockInfo.reqValueStr, out var result))
				{
					return nKMUserData.GetReturnEndDate(result);
				}
				break;
			}
			DateTime dateTime = DateTime.MinValue;
			{
				foreach (ReturningUserType value in Enum.GetValues(typeof(ReturningUserType)))
				{
					DateTime returnEndDate = nKMUserData.GetReturnEndDate(value);
					if (dateTime < returnEndDate)
					{
						dateTime = returnEndDate;
					}
				}
				return dateTime;
			}
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_INTERVAL:
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE_INTERVAL:
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON_INTERVAL:
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_PHASE_INTERVAL:
		{
			NKMIntervalTemplet nKMIntervalTemplet = NKMIntervalTemplet.Find(unlockInfo.reqValueStr);
			if (nKMIntervalTemplet != null)
			{
				return nKMIntervalTemplet.GetEndDateUtc();
			}
			break;
		}
		}
		return DateTime.MaxValue;
	}
}
