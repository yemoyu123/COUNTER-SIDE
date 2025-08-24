using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Guild;
using ClientPacket.Office;
using ClientPacket.WorldMap;
using Cs.Core.Util;
using NKC.UI.Worldmap;
using NKM;
using NKM.Event;
using NKM.Guild;
using NKM.Shop;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC.UI;

public static class NKCAlarmManager
{
	public enum ALARM_TYPE
	{
		MAIL,
		FRIEND,
		HANGAR,
		INVENTORY,
		COLLECTION,
		FACTORY,
		WORLDMAP,
		MISSION,
		CONTRACT,
		OFFICE_DORM,
		CHAT,
		LEADERBOARD,
		PVP,
		BASE,
		ALL,
		Count,
		AT_END
	}

	public delegate bool OnNotify(NKMUserData userData);

	private static Dictionary<ALARM_TYPE, OnNotify> m_dicNotify = new Dictionary<ALARM_TYPE, OnNotify>(21);

	private static bool m_bInitComplete = false;

	private static bool s_bServerShutdownScheduled = false;

	private static DateTime s_dtServerShutdownUTCTime;

	private static DateTime s_dtServerShutdownNextAlarmUTCTime;

	public static void Update(float deltaTime)
	{
		if (s_bServerShutdownScheduled && s_dtServerShutdownUTCTime.AddMinutes(-1.0) <= NKCSynchronizedTime.GetServerUTCTime())
		{
			s_bServerShutdownScheduled = false;
			NKCUIPopupMessageServer.Instance.Open(NKCUIPopupMessageServer.eMessageStyle.Slide, NKCStringTable.GetString("SI_SERVER_SHUTDOWN_NOW"));
		}
	}

	public static void Init()
	{
		m_dicNotify.Clear();
		m_dicNotify.Add(ALARM_TYPE.FRIEND, CheckFriendNotify);
		m_dicNotify.Add(ALARM_TYPE.HANGAR, CheckHangarNotify);
		m_dicNotify.Add(ALARM_TYPE.INVENTORY, CheckInventoryNotify);
		m_dicNotify.Add(ALARM_TYPE.COLLECTION, CheckCollectionNotify);
		m_dicNotify.Add(ALARM_TYPE.FACTORY, CheckFactoryNotify);
		m_dicNotify.Add(ALARM_TYPE.WORLDMAP, CheckWorldMapNotify);
		m_dicNotify.Add(ALARM_TYPE.MISSION, CheckMissionNotify);
		m_dicNotify.Add(ALARM_TYPE.CONTRACT, CheckContractNotify);
		m_dicNotify.Add(ALARM_TYPE.OFFICE_DORM, CheckOfficeDormNotify);
		m_dicNotify.Add(ALARM_TYPE.CHAT, CheckChatNotify);
		m_dicNotify.Add(ALARM_TYPE.LEADERBOARD, CheckleaderBoardNotify);
		m_dicNotify.Add(ALARM_TYPE.PVP, CheckPVPNotify);
		m_dicNotify.Add(ALARM_TYPE.BASE, CheckBaseNotify);
		m_dicNotify.Add(ALARM_TYPE.ALL, CheckAllNotify);
		m_bInitComplete = true;
	}

	public static bool CheckNotify(NKMUserData userData, ALARM_TYPE alarmType)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
		if (m_dicNotify.ContainsKey(alarmType))
		{
			return m_dicNotify[alarmType](userData);
		}
		return false;
	}

	public static bool CheckAllNotify(NKMUserData userData)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
		foreach (KeyValuePair<ALARM_TYPE, OnNotify> item in m_dicNotify)
		{
			if (item.Key != ALARM_TYPE.ALL && item.Value(userData))
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckMailNotify(NKMUserData userData)
	{
		return NKCMailManager.HasNewMail();
	}

	public static bool CheckGuildNotify(NKMUserData userData)
	{
		if (userData == null)
		{
			return false;
		}
		if (!NKCGuildManager.HasGuild())
		{
			return false;
		}
		NKMGuildMemberData nKMGuildMemberData = NKCGuildManager.MyGuildData.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == userData.m_UserUID);
		if (nKMGuildMemberData == null)
		{
			return false;
		}
		if (!nKMGuildMemberData.HasAttendanceData(ServiceTime.Recent))
		{
			return true;
		}
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(NKM_MISSION_TYPE.GUILD);
		if (missionTabTemplet != null && userData.m_MissionData.CheckCompletableMission(userData, missionTabTemplet.m_tabID))
		{
			return true;
		}
		if (NKCContentManager.CheckContentStatus(ContentsType.GUILD_DUNGEON, out var _) == NKCContentManager.eContentStatus.Open && NKCGuildCoopManager.CheckFirstSeasonStarted() && NKCGuildCoopManager.CheckSeasonRewardEnable())
		{
			return true;
		}
		return false;
	}

	public static bool CheckChatNotify(NKMUserData userData)
	{
		if (userData == null)
		{
			return false;
		}
		if (!NKCChatManager.IsContentsUnlocked())
		{
			return false;
		}
		if (!NKCScenManager.GetScenManager().GetGameOptionData().UseChatContent)
		{
			return false;
		}
		if (!NKCChatManager.CheckPrivateChatNotify(userData, 0L))
		{
			return NKCChatManager.CheckGuildChatNotify();
		}
		return true;
	}

	public static bool CheckFriendNotify(NKMUserData userData)
	{
		if (NKCContentManager.CheckContentStatus(ContentsType.FRIENDS, out var _) != NKCContentManager.eContentStatus.Open)
		{
			return false;
		}
		return NKCScenManager.GetScenManager().Get_SCEN_HOME().GetHasNewFriendRequest();
	}

	public static bool CheckHangarNotify(NKMUserData userData)
	{
		if (NKCContentManager.CheckContentStatus(ContentsType.HANGER_SHIPBUILD, out var _) != NKCContentManager.eContentStatus.Open)
		{
			return false;
		}
		foreach (NKMShipBuildTemplet value in NKMTempletContainer<NKMShipBuildTemplet>.Values)
		{
			if (value.ShipBuildUnlockType == NKMShipBuildTemplet.BuildUnlockType.BUT_UNABLE)
			{
				continue;
			}
			bool flag = false;
			foreach (KeyValuePair<long, NKMUnitData> item in NKCScenManager.CurrentUserData().m_ArmyData.m_dicMyShip)
			{
				if (NKMShipManager.IsSameKindShip(item.Value.m_UnitID, value.Key))
				{
					flag = true;
					break;
				}
			}
			if (!flag && NKMShipManager.CanUnlockShip(userData, value) && !PlayerPrefs.HasKey(string.Format("{0}_{1}_{2}", "SHIP_BUILD_SLOT_CHECK", userData.m_UserUID, value.ShipID)))
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckInventoryNotify(NKMUserData userData)
	{
		foreach (NKMItemMiscData value in userData.m_InventoryData.MiscItems.Values)
		{
			if (value.TotalCount > 0)
			{
				NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(value.ItemID);
				if (itemMiscTempletByID != null && itemMiscTempletByID.IsUsable() && itemMiscTempletByID.IsTimeIntervalItem)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool CheckCollectionNotify(NKMUserData userData)
	{
		if (NKCUnitMissionManager.GetOpenTagCollectionTeamUp())
		{
			foreach (NKMCollectionTeamUpGroupTemplet value in NKMTempletContainer<NKMCollectionTeamUpGroupTemplet>.Values)
			{
				if (value != null)
				{
					int unitCollectCount = userData.m_ArmyData.GetUnitCollectCount(value.UnitIDList);
					if (userData.m_ArmyData.GetTeamCollectionData(value.TeamID) == null && value.RewardCriteria <= unitCollectCount)
					{
						return true;
					}
				}
			}
		}
		if (NKCUnitMissionManager.HasRewardEnableMission())
		{
			return true;
		}
		if (NKCCollectionManager.IsMiscCollectionOpened && NKCCollectionManager.IsMiscRewardEnable())
		{
			return true;
		}
		return false;
	}

	public static bool CheckFactoryNotify(NKMUserData userData)
	{
		if (NKCContentManager.CheckContentStatus(ContentsType.BASE_FACTORY, out var _) != NKCContentManager.eContentStatus.Open)
		{
			return false;
		}
		foreach (KeyValuePair<byte, NKMCraftSlotData> slot in userData.m_CraftData.SlotList)
		{
			if (slot.Value.GetState(NKCSynchronizedTime.GetServerUTCTime()) == NKM_CRAFT_SLOT_STATE.NECSS_COMPLETED)
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckScoutNotify(NKMUserData userData)
	{
		foreach (NKMPieceTemplet value in NKMTempletContainer<NKMPieceTemplet>.Values)
		{
			if (NKCUIScout.IsReddotNeeded(userData, value.Key))
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckWorldMapNotify(NKMUserData userData)
	{
		if (NKCContentManager.CheckContentStatus(ContentsType.WORLDMAP, out var _) != NKCContentManager.eContentStatus.Open)
		{
			return false;
		}
		foreach (KeyValuePair<int, NKMWorldMapCityData> item in userData.m_WorldmapData.worldMapCityDataMap)
		{
			NKMWorldMapCityData value = item.Value;
			if (value != null && value.HasMission() && value.IsMissionFinished(NKCSynchronizedTime.GetServerUTCTime()))
			{
				return true;
			}
		}
		if (CheckRaidSeasonRewardNotify())
		{
			return true;
		}
		if (CheckRaidSeasonExtraRewardNotify())
		{
			return true;
		}
		return false;
	}

	public static bool CheckMissionNotify(NKMUserData userData)
	{
		if (NKCContentManager.CheckContentStatus(ContentsType.LOBBY_SUBMENU, out var _) != NKCContentManager.eContentStatus.Open)
		{
			return false;
		}
		return NKMMissionManager.GetHaveClearedMission();
	}

	public static bool CheckContractNotify(NKMUserData userData)
	{
		if (NKCContentManager.CheckContentStatus(ContentsType.CONTRACT, out var _) != NKCContentManager.eContentStatus.Open)
		{
			return false;
		}
		NKCContractDataMgr nKCContractDataMgr = NKCScenManager.GetScenManager().GetNKCContractDataMgr();
		if (nKCContractDataMgr.IsPossibleFreeChance() || nKCContractDataMgr.IsActiveNewFreeChance())
		{
			return true;
		}
		return false;
	}

	public static bool CheckOfficeDormNotify(NKMUserData userData)
	{
		if (NKCContentManager.CheckContentStatus(ContentsType.OFFICE, out var _) != NKCContentManager.eContentStatus.Open)
		{
			return false;
		}
		if (CheckOfficeCommunityNotify(userData))
		{
			return true;
		}
		if (CheckOfficeLoyaltyNotify(userData))
		{
			return true;
		}
		return false;
	}

	public static bool CheckOfficeCommunityNotify(NKMUserData userData)
	{
		if (NKCContentManager.CheckContentStatus(ContentsType.OFFICE, out var _) != NKCContentManager.eContentStatus.Open)
		{
			return false;
		}
		bool num = userData.OfficeData.BizcardCount != 0;
		bool canReceiveBizcard = userData.OfficeData.CanReceiveBizcard;
		if (num && canReceiveBizcard)
		{
			return true;
		}
		if (NKCFriendManager.FriendList.Count > 0 && userData.OfficeData.CanSendBizcardBroadcast)
		{
			return true;
		}
		return false;
	}

	public static bool CheckOfficeLoyaltyNotify(NKMUserData userData)
	{
		if (NKCContentManager.CheckContentStatus(ContentsType.OFFICE, out var _) != NKCContentManager.eContentStatus.Open)
		{
			return false;
		}
		foreach (NKMOfficeRoom room in userData.OfficeData.Rooms)
		{
			if (room == null || room.unitUids == null)
			{
				continue;
			}
			foreach (long unitUid in room.unitUids)
			{
				NKMUnitData unitFromUID = userData.m_ArmyData.GetUnitFromUID(unitUid);
				if (unitFromUID != null && unitFromUID.CheckOfficeRoomHeartFull())
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool CheckBaseNotify(NKMUserData userData)
	{
		if (NKCContentManager.CheckContentStatus(ContentsType.BASE, out var _) != NKCContentManager.eContentStatus.Open)
		{
			return false;
		}
		if (!CheckHangarNotify(userData) && !CheckFactoryNotify(userData))
		{
			return CheckScoutNotify(userData);
		}
		return true;
	}

	public static bool CheckShopNotify(NKMUserData userData)
	{
		if (NKCContentManager.CheckContentStatus(ContentsType.LOBBY_SUBMENU, out var _) != NKCContentManager.eContentStatus.Open)
		{
			return false;
		}
		ShopReddotType reddotType;
		return NKCShopManager.CheckTabReddotCount(out reddotType) > 0;
	}

	public static bool CheckJukeBoxNotifiy()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BASE_PERSONNAL))
		{
			return false;
		}
		return NKCUIJukeBox.HasNewMusic();
	}

	public static bool CheckleaderBoardNotify(NKMUserData userData)
	{
		if (userData == null)
		{
			return false;
		}
		NKCContentManager.CheckContentStatus(ContentsType.LEADERBOARD, out var _);
		return false;
	}

	public static bool CheckPVPNotify(NKMUserData userData)
	{
		DateTime serverUTCTime = NKCSynchronizedTime.GetServerUTCTime();
		PvpState pvpData = userData.m_PvpData;
		int num = NKCUtil.FindPVPSeasonIDForRank(serverUTCTime);
		if (num != 0)
		{
			int weekIDForRank = NKCPVPManager.GetWeekIDForRank(serverUTCTime, num);
			if (NKCPVPManager.CanRewardWeek(NKM_GAME_TYPE.NGT_PVP_RANK, pvpData, num, weekIDForRank, serverUTCTime) == NKM_ERROR_CODE.NEC_OK)
			{
				return true;
			}
			if (NKCPVPManager.CanRewardSeason(pvpData, num, serverUTCTime) == NKM_ERROR_CODE.NEC_OK)
			{
				return true;
			}
		}
		PvpState asyncData = userData.m_AsyncData;
		int num2 = NKCUtil.FindPVPSeasonIDForAsync(serverUTCTime);
		if (num2 != 0)
		{
			int weekIDForAsync = NKCPVPManager.GetWeekIDForAsync(serverUTCTime, num2);
			if (NKCPVPManager.CanRewardWeek(NKM_GAME_TYPE.NGT_ASYNC_PVP, asyncData, num2, weekIDForAsync, serverUTCTime) == NKM_ERROR_CODE.NEC_OK)
			{
				return true;
			}
			if (NKCPVPManager.CanRewardSeason(asyncData, num2, serverUTCTime) == NKM_ERROR_CODE.NEC_OK)
			{
				return true;
			}
		}
		if (NKCEventPvpMgr.CanGetReward())
		{
			return true;
		}
		return false;
	}

	public static bool CheckFierceRedDot()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return false;
		}
		if (CheckFierceDailyRewardNotify(nKMUserData))
		{
			return true;
		}
		if (CheckFierceRewardNotify(nKMUserData))
		{
			return true;
		}
		return false;
	}

	public static bool CheckFierceDailyRewardNotify(NKMUserData userData)
	{
		NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
		if (nKCFierceBattleSupportDataMgr != null)
		{
			if (NKCContentManager.CheckContentStatus(ContentsType.FIERCE, out var _) != NKCContentManager.eContentStatus.Open || nKCFierceBattleSupportDataMgr.GetStatus() != NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_ACTIVATE)
			{
				return false;
			}
			if (NKCScenManager.CurrentUserData().GetStatePlayCnt(NKMFierceConst.StageId, IsServiceTime: true) > 0)
			{
				return !nKCFierceBattleSupportDataMgr.m_fierceDailyRewardReceived;
			}
			return false;
		}
		return false;
	}

	public static bool CheckFierceRewardNotify(NKMUserData userData)
	{
		NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
		if (nKCFierceBattleSupportDataMgr != null)
		{
			switch (nKCFierceBattleSupportDataMgr.GetStatus())
			{
			case NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_ACTIVATE:
				return nKCFierceBattleSupportDataMgr.IsCanReceivePointReward();
			case NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_REWARD:
				return nKCFierceBattleSupportDataMgr.IsPossibleRankReward();
			}
		}
		return false;
	}

	public static bool CheckRaidSeasonRewardNotify()
	{
		NKMRaidSeasonTemplet nowSeasonTemplet = NKCRaidSeasonManager.GetNowSeasonTemplet();
		if (nowSeasonTemplet == null)
		{
			return false;
		}
		IReadOnlyList<NKMRaidSeasonRewardTemplet> raidSeasonRewardTemplet = nowSeasonTemplet.RaidSeasonRewardTemplet;
		if (raidSeasonRewardTemplet == null)
		{
			return false;
		}
		foreach (NKMRaidSeasonRewardTemplet item in raidSeasonRewardTemplet)
		{
			if (item.RaidPoint > NKCRaidSeasonManager.RaidSeason.recvRewardRaidPoint && item.RaidPoint <= NKCRaidSeasonManager.RaidSeason.monthlyPoint)
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckRaidSeasonExtraRewardNotify()
	{
		NKMRaidSeasonTemplet seasonTemplet = NKCRaidSeasonManager.GetNowSeasonTemplet();
		if (seasonTemplet != null)
		{
			NKMRaidSeasonRewardTemplet nKMRaidSeasonRewardTemplet = (from e in NKMRaidSeasonRewardTemplet.Values
				where e.RewardBoardId == seasonTemplet.RaidBoardId
				where e.ExtraRewardId > 0
				select e).FirstOrDefault();
			if (nKMRaidSeasonRewardTemplet != null)
			{
				int maxSeasonPoint = NKCPopupWorldMapEventList.GetMaxSeasonPoint();
				if (NKCRaidSeasonManager.RaidSeason.recvRewardRaidPoint < maxSeasonPoint)
				{
					return false;
				}
				return nKMRaidSeasonRewardTemplet.ExtraRewardPoint <= NKCRaidSeasonManager.RaidSeason.monthlyPoint - NKCRaidSeasonManager.RaidSeason.recvRewardRaidPoint;
			}
		}
		return false;
	}

	public static void SetServiceShutdownTime(DateTime UTCTime)
	{
		s_bServerShutdownScheduled = true;
		s_dtServerShutdownUTCTime = UTCTime;
		DateTime dateTime = UTCTime.ToLocalTime();
		string message = string.Format(NKCStringTable.GetString("SI_SERVER_SHUTDOWN_TIME"), dateTime.Hour, dateTime.Minute);
		NKCUIPopupMessageServer.Instance.Open(NKCUIPopupMessageServer.eMessageStyle.Slide, message);
	}

	public static bool CheckAlarmByShortcut(NKM_SHORTCUT_TYPE shortcutType, string shortCutParam)
	{
		if (!int.TryParse(shortCutParam, out var result))
		{
			result = -1;
		}
		switch (shortcutType)
		{
		case NKM_SHORTCUT_TYPE.SHORTCUT_PT_EXCHANGE:
		{
			NKMPointExchangeTemplet nKMPointExchangeTemplet = ((result <= 0) ? NKMPointExchangeTemplet.GetByTime(NKCSynchronizedTime.ServiceTime) : NKMPointExchangeTemplet.Find(result));
			if (nKMPointExchangeTemplet == null)
			{
				return false;
			}
			return CheckShopCanPurchase(nKMPointExchangeTemplet.ShopTabStrId, nKMPointExchangeTemplet.ShopTabSubIndex);
		}
		case NKM_SHORTCUT_TYPE.SHORTCUT_EVENT_COLLECTION:
		{
			NKMEventCollectionIndexTemplet nKMEventCollectionIndexTemplet = ((result <= 0) ? GetEventCollectionIndexTemplet() : NKMEventCollectionIndexTemplet.Find(result));
			if (nKMEventCollectionIndexTemplet == null)
			{
				return false;
			}
			NKMEventTabTemplet nKMEventTabTemplet = NKMEventTabTemplet.Find(NKCUtil.GetIntValue(nKMEventCollectionIndexTemplet.m_Option, "EventTabID", 0));
			bool flag = false;
			if (nKMEventTabTemplet != null)
			{
				flag = NKMEventManager.CheckRedDot(nKMEventTabTemplet);
			}
			return HasCompletableMission(nKMEventCollectionIndexTemplet.MissionTabIds) || flag;
		}
		case NKM_SHORTCUT_TYPE.SHORTCUT_EVENT:
			if (result > 0)
			{
				return NKMEventManager.CheckRedDot(NKMEventTabTemplet.Find(result));
			}
			break;
		}
		return false;
	}

	public static bool CheckShopCanPurchase(string shopTabStrId, int shopTabSubIndex)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		List<ShopItemTemplet> itemTempletListByTab = NKCShopManager.GetItemTempletListByTab(ShopTabTemplet.Find(shopTabStrId, shopTabSubIndex));
		if (itemTempletListByTab == null || nKMUserData == null)
		{
			return false;
		}
		int count = itemTempletListByTab.Count;
		for (int i = 0; i < count; i++)
		{
			if (itemTempletListByTab[i] != null)
			{
				NKMShopData shopData = nKMUserData.m_ShopData;
				bool flag = nKMUserData.m_InventoryData.GetCountMiscItem(itemTempletListByTab[i].m_PriceItemID) >= itemTempletListByTab[i].m_Price;
				bool flag2 = true;
				if (shopData.histories.ContainsKey(itemTempletListByTab[i].m_ProductID))
				{
					flag2 = shopData.histories[itemTempletListByTab[i].m_ProductID].purchaseCount < itemTempletListByTab[i].m_QuantityLimit;
				}
				if (flag2 && flag)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool HasCompletableMission(IEnumerable<int> lstMissionTab)
	{
		if (lstMissionTab == null)
		{
			return false;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return false;
		}
		NKMUserMissionData missionData = myUserData.m_MissionData;
		if (missionData == null)
		{
			return false;
		}
		foreach (int item in lstMissionTab)
		{
			if (missionData.CheckCompletableMission(myUserData, item))
			{
				return true;
			}
		}
		return false;
	}

	public static NKMEventCollectionIndexTemplet GetEventCollectionIndexTemplet()
	{
		foreach (NKMEventCollectionIndexTemplet value in NKMEventCollectionIndexTemplet.Values)
		{
			if (value != null && value.IsOpen)
			{
				NKMIntervalTemplet nKMIntervalTemplet = NKMIntervalTemplet.Find(value.DateStrId);
				if (nKMIntervalTemplet != null && nKMIntervalTemplet.IsValidTime(NKCSynchronizedTime.ServiceTime))
				{
					return value;
				}
			}
		}
		return null;
	}
}
