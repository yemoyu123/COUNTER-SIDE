using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Common;
using ClientPacket.Community;
using ClientPacket.Contract;
using ClientPacket.Event;
using ClientPacket.Item;
using ClientPacket.Mode;
using ClientPacket.Shop;
using ClientPacket.User;
using ClientPacket.WorldMap;
using Cs.Logging;
using Cs.Protocol;
using NKC;
using NKC.Office;
using NKC.Publisher;
using NKC.Trim;
using NKC.UI;
using NKC.UI.Warfare;
using NKM.Guild;
using NKM.Shop;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKM;

public class NKMUserData : ISerializable
{
	public enum eChangeNotifyType
	{
		Add,
		Update,
		Remove
	}

	public delegate void OnUserLevelUpdate(NKMUserData userData);

	public delegate void OnCompanyBuffUpdate(NKMUserData userData);

	public struct strMentoringData
	{
		public int SeasonId;

		public List<MenteeInfo> lstMenteeMatch;

		public List<FriendListData> lstRecommend;

		public List<FriendListData> lstInvited;

		public FriendListData MyMentor;

		public bool bMenteeGraduate;

		public bool bMentoringNotify;

		public strMentoringData(bool bInit = false)
		{
			SeasonId = 0;
			lstMenteeMatch = null;
			lstRecommend = null;
			lstInvited = null;
			MyMentor = null;
			bMenteeGraduate = false;
			bMentoringNotify = false;
		}
	}

	public class RaceData
	{
		private int m_iEventID;

		private int m_iRaceIndex;

		private NKMEventBetResult m_UserBetLastInfo;

		private NKMEventBetPrivate m_UserBetCurInfo;

		private List<NKMEventBetPrivateResult> m_lstUserBetJoinHistory;

		public NKMEventBetRecord BetRecord;

		public int CurEventID => m_iEventID;

		public int CurRaceIndex => m_iRaceIndex;

		public NKMEventBetPrivate CurUserBetInfo => m_UserBetCurInfo;

		public int UserJoinCount => m_lstUserBetJoinHistory.Where((NKMEventBetPrivateResult x) => x != null).Count();

		public List<NKMEventBetPrivateResult> ListUserJoinHistory => m_lstUserBetJoinHistory;

		public NKMEventBetRecord CurRaceBetInfo => BetRecord;

		public void UpdateCurrentRaceBetInfo(int _raceIndex, NKMEventBetRecord _betRecord)
		{
			m_iRaceIndex = _raceIndex;
			BetRecord = _betRecord;
		}

		public void SetRaceEventBetPrivate(NKMEventBetPrivate betPrivate)
		{
			m_UserBetCurInfo = betPrivate;
		}

		public void SetRaceData(int iCurEventID, int iCurRaceIndex, NKMEventBetSummary betSummary)
		{
			m_iEventID = iCurEventID;
			m_iRaceIndex = iCurRaceIndex;
			if (betSummary != null)
			{
				m_UserBetLastInfo = betSummary.betResult;
				m_UserBetCurInfo = betSummary.betPrivate;
				m_lstUserBetJoinHistory = betSummary.betPrivateResult;
			}
		}

		public NKMEventBetPrivateResult GetBetPrivateResult(int eventIdx)
		{
			return m_lstUserBetJoinHistory.Find((NKMEventBetPrivateResult x) => x != null && x.eventIndex == eventIdx);
		}

		public NKMEventBetPrivateResult GetYesterDayRaceData()
		{
			return m_lstUserBetJoinHistory.Find((NKMEventBetPrivateResult x) => x != null && x.eventIndex == m_iRaceIndex - 1);
		}
	}

	public List<NKMCompanyBuffData> m_companyBuffDataList = new List<NKMCompanyBuffData>();

	public long m_UserUID;

	public long m_FriendCode;

	public string m_UserNickName = "";

	public NKM_PUBLISHER_TYPE m_NKM_PUBLISHER_TYPE;

	public UserState m_UserState;

	public NKM_USER_AUTH_LEVEL m_eAuthLevel = NKM_USER_AUTH_LEVEL.NORMAL_USER;

	public NKMUserDateData m_NKMUserDateData = new NKMUserDateData();

	public NKMInventoryData m_InventoryData = new NKMInventoryData();

	public NKMArmyData m_ArmyData = new NKMArmyData();

	public NKMUserOption m_UserOption = new NKMUserOption();

	public Dictionary<int, NKMDungeonClearData> m_dicNKMDungeonClearData = new Dictionary<int, NKMDungeonClearData>();

	public NKMWorldMapData m_WorldmapData = new NKMWorldMapData();

	public Dictionary<int, NKMWarfareClearData> m_dicNKMWarfareClearData = new Dictionary<int, NKMWarfareClearData>();

	public NKMShopData m_ShopData = new NKMShopData();

	public NKMUserMissionData m_MissionData = new NKMUserMissionData();

	public Dictionary<int, NKMCounterCaseData> m_dicNKMCounterCaseData = new Dictionary<int, NKMCounterCaseData>();

	public NKMCraftData m_CraftData = new NKMCraftData();

	public NKMEquipTuningCandidate m_EquipTuningCandidate = new NKMEquipTuningCandidate();

	public NKMShipModuleCandidate m_ShipCmdModuleCandidate = new NKMShipModuleCandidate();

	public Dictionary<long, NKMEpisodeCompleteData> m_dicEpisodeCompleteData = new Dictionary<long, NKMEpisodeCompleteData>();

	public PvpState m_PvpData = new PvpState();

	public PvpHistoryList m_SyncPvpHistory = new PvpHistoryList();

	public PvpHistoryList m_AsyncPvpHistory = new PvpHistoryList();

	public PvpHistoryList m_EventPvpHistory = new PvpHistoryList();

	public NKMDiveGameData m_DiveGameData;

	public HashSet<int> m_DiveClearData = new HashSet<int>();

	public HashSet<int> m_DiveHistoryData = new HashSet<int>();

	public NKMAttendanceData m_AttendanceData = new NKMAttendanceData();

	public NKMShadowPalace m_ShadowPalace = new NKMShadowPalace();

	public NKMBackgroundInfo backGroundInfo = new NKMBackgroundInfo();

	public Dictionary<int, RecallHistoryInfo> m_RecallHistoryData = new Dictionary<int, RecallHistoryInfo>();

	public NKMUserBirthDayData m_BirthDayData;

	public NKMJukeboxData m_JukeboxData = new NKMJukeboxData();

	public int m_UserLevel = 1;

	public int m_lUserLevelEXP;

	public OnUserLevelUpdate dOnUserLevelUpdate;

	public PvpState m_AsyncData = new PvpState();

	public PvpState m_LeagueData = new PvpState();

	public PvpState m_eventPvpData = new PvpState();

	public NpcPvpData m_NpcData = new NpcPvpData();

	public NKMPotentialOptionChangeCandidate m_PotentialOptionCandidate = new NKMPotentialOptionChangeCandidate();

	public bool m_enableAccountLink;

	public bool m_RankOpen;

	public bool m_LeagueOpen;

	public DateTime LastPvpPointChargeTimeUTC;

	public OnCompanyBuffUpdate dOnCompanyBuffUpdate;

	public DateTime m_GuildJoinDisableTime;

	public PvpHistoryList m_LeaguePvpHistory = new PvpHistoryList();

	public PvpHistoryList m_PrivatePvpHistory = new PvpHistoryList();

	public HashSet<int> m_LastDiveHistoryData = new HashSet<int>();

	public NKCOfficeData OfficeData = new NKCOfficeData();

	public NKCTrimData TrimData = new NKCTrimData();

	private Dictionary<int, NKMConsumerPackageData> m_dicConsumerPackageData = new Dictionary<int, NKMConsumerPackageData>();

	private NKMEventCollectionInfo m_eventCollectionInfo;

	public NKMShortCutInfo m_LastPlayInfo = new NKMShortCutInfo();

	private NKMUserProfileData m_UserProfileData;

	private NKMSupportUnitData m_SupportUnitData;

	private List<NKMMiniGameData> m_lstMiniGameDatas = new List<NKMMiniGameData>();

	private Dictionary<int, int> m_dicMiniGameScoreReward = new Dictionary<int, int>();

	private const string BGM_CONTINUE_KEY = "BGM_CONTINUE_KEY";

	private List<int> m_lstActiveMiniGameTempletIDs = new List<int>();

	private List<int> m_lstRecivedRewardIds = new List<int>();

	private DateTime m_lastUpdateAsyncTicket = DateTime.MinValue;

	private DateTime m_lastUpdateEterniumCap = DateTime.MinValue;

	public Dictionary<ReturningUserType, NKMReturningUserState> m_dicReturningUserState = new Dictionary<ReturningUserType, NKMReturningUserState>();

	private Dictionary<int, NKMStagePlayData> m_dicStagePlayData = new Dictionary<int, NKMStagePlayData>();

	private strMentoringData m_MyMentoringData;

	public KakaoMissionData kakaoMissionData;

	private RaceData m_RaceEventData = new RaceData();

	public string CountryCode { get; }

	public string MarketId { get; }

	public NKMUserProfileData UserProfileData => m_UserProfileData;

	public NKMSupportUnitData SupportUnitData => m_SupportUnitData;

	public int UserLevel
	{
		get
		{
			return m_UserLevel;
		}
		set
		{
			m_UserLevel = value;
			dOnUserLevelUpdate?.Invoke(this);
		}
	}

	public int UserLevelEXP
	{
		get
		{
			return m_lUserLevelEXP;
		}
		set
		{
			m_lUserLevelEXP = value;
			dOnUserLevelUpdate?.Invoke(this);
		}
	}

	public NKMEventCollectionInfo EventCollectionInfo
	{
		get
		{
			if (m_eventCollectionInfo == null)
			{
				return new NKMEventCollectionInfo();
			}
			return m_eventCollectionInfo;
		}
		set
		{
			m_eventCollectionInfo = value;
		}
	}

	public int BackgroundID
	{
		get
		{
			if (backGroundInfo == null || backGroundInfo.backgroundItemId == 0)
			{
				return 9001;
			}
			return backGroundInfo.backgroundItemId;
		}
	}

	public int BackgroundBGMID
	{
		get
		{
			if (backGroundInfo == null || backGroundInfo.backgroundBgmId == 0)
			{
				return 9001;
			}
			return backGroundInfo.backgroundBgmId;
		}
	}

	public bool BackgroundBGMContinue
	{
		get
		{
			if (PlayerPrefs.HasKey("BGM_CONTINUE_KEY"))
			{
				if (PlayerPrefs.GetInt("BGM_CONTINUE_KEY") != 1)
				{
					return false;
				}
				return true;
			}
			return false;
		}
		set
		{
			PlayerPrefs.SetInt("BGM_CONTINUE_KEY", value ? 1 : 0);
		}
	}

	public DateTime lastAsyncTicketUpdateDate => m_lastUpdateAsyncTicket;

	public DateTime lastEterniumUpdateDate => m_lastUpdateEterniumCap;

	public strMentoringData MentoringData => m_MyMentoringData;

	public long GetCash()
	{
		return m_InventoryData.GetItemMisc(101)?.TotalCount ?? 0;
	}

	public long GetCashPaid()
	{
		return m_InventoryData.GetItemMisc(101)?.CountPaid ?? 0;
	}

	public long GetCashBonus()
	{
		return m_InventoryData.GetItemMisc(101)?.CountFree ?? 0;
	}

	public long GetCredit()
	{
		return m_InventoryData.GetItemMisc(1)?.TotalCount ?? 0;
	}

	public long GetEternium()
	{
		return m_InventoryData.GetItemMisc(2)?.TotalCount ?? 0;
	}

	public long GetInformation()
	{
		return m_InventoryData.GetItemMisc(3)?.TotalCount ?? 0;
	}

	public long GetMissionAchievePoint()
	{
		return m_MissionData.GetAchiecePoint();
	}

	public long GetDailyTicket()
	{
		return m_InventoryData.GetItemMisc(4)?.CountPaid ?? 0;
	}

	public long GetDailyTicket_A()
	{
		return m_InventoryData.GetItemMisc(15)?.CountPaid ?? 0;
	}

	public long GetDailyTicket_B()
	{
		return m_InventoryData.GetItemMisc(16)?.CountPaid ?? 0;
	}

	public long GetDailyTicket_C()
	{
		return m_InventoryData.GetItemMisc(17)?.CountPaid ?? 0;
	}

	public long GetDailyTicketBonus()
	{
		return m_InventoryData.GetItemMisc(4)?.CountFree ?? 0;
	}

	public long GetDailyTicketA_Bonus()
	{
		return m_InventoryData.GetItemMisc(15)?.CountFree ?? 0;
	}

	public long GetDailyTicketB_Bonus()
	{
		return m_InventoryData.GetItemMisc(16)?.CountFree ?? 0;
	}

	public long GetDailyTicketC_Bonus()
	{
		return m_InventoryData.GetItemMisc(17)?.CountFree ?? 0;
	}

	public void SetCash(long cash)
	{
		m_InventoryData.UpdateItemInfo(101, cash, NKM_ITEM_PAYMENT_TYPE.NIPT_PAID);
	}

	public void SetCashBonus(long cash_bonus)
	{
		m_InventoryData.UpdateItemInfo(101, cash_bonus, NKM_ITEM_PAYMENT_TYPE.NIPT_FREE);
	}

	public void SetCredit(long credit)
	{
		m_InventoryData.UpdateItemInfo(1, credit, NKM_ITEM_PAYMENT_TYPE.NIPT_FREE);
	}

	public void SetEternium(long eternium)
	{
		m_InventoryData.UpdateItemInfo(2, eternium, NKM_ITEM_PAYMENT_TYPE.NIPT_FREE);
	}

	public void SetInformation(long information)
	{
		m_InventoryData.UpdateItemInfo(3, information, NKM_ITEM_PAYMENT_TYPE.NIPT_FREE);
	}

	public void SetMissionAchievePoint(long achieve_point)
	{
		m_MissionData.SetAchievePoint(achieve_point);
	}

	public void SetDailyTicketBonus(long count)
	{
		m_InventoryData.UpdateItemInfo(4, count, NKM_ITEM_PAYMENT_TYPE.NIPT_FREE);
	}

	public NKMUserData()
	{
		m_ArmyData.SetOwner(this);
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_UserUID);
		stream.PutOrGet(ref m_FriendCode);
		stream.PutOrGet(ref m_UserNickName);
		stream.PutOrGet(ref m_UserLevel);
		stream.PutOrGet(ref m_lUserLevelEXP);
		stream.PutOrGetEnum(ref m_eAuthLevel);
		stream.PutOrGet(ref m_NKMUserDateData);
		stream.PutOrGet(ref m_InventoryData);
		stream.PutOrGet(ref m_ArmyData);
		stream.PutOrGet(ref m_UserOption);
		stream.PutOrGet(ref m_dicNKMDungeonClearData);
		stream.PutOrGet(ref m_WorldmapData);
		stream.PutOrGet(ref m_dicNKMWarfareClearData);
		stream.PutOrGet(ref m_ShopData);
		stream.PutOrGet(ref m_MissionData);
		stream.PutOrGet(ref m_dicNKMCounterCaseData);
		stream.PutOrGet(ref m_CraftData);
		stream.PutOrGet(ref m_dicEpisodeCompleteData);
		stream.PutOrGet(ref m_PvpData);
		stream.PutOrGet(ref m_SyncPvpHistory);
		stream.PutOrGet(ref m_AsyncPvpHistory);
		stream.PutOrGet(ref m_EventPvpHistory);
		stream.PutOrGet(ref m_DiveGameData);
		stream.PutOrGet(ref m_DiveClearData);
		stream.PutOrGet(ref m_DiveHistoryData);
		stream.PutOrGet(ref m_AttendanceData);
		stream.PutOrGetEnum(ref m_UserState);
		stream.PutOrGet(ref m_companyBuffDataList);
		stream.PutOrGet(ref m_ShadowPalace);
		stream.PutOrGet(ref backGroundInfo);
		stream.PutOrGet(ref m_RecallHistoryData);
		stream.PutOrGet(ref m_BirthDayData);
		stream.PutOrGet(ref m_JukeboxData);
	}

	public bool IsAdminUser()
	{
		if (m_eAuthLevel == NKM_USER_AUTH_LEVEL.NORMAL_ADMIN || m_eAuthLevel == NKM_USER_AUTH_LEVEL.SUPER_ADMIN)
		{
			return true;
		}
		return false;
	}

	public bool IsSuperUser()
	{
		if (m_eAuthLevel == NKM_USER_AUTH_LEVEL.SUPER_USER || m_eAuthLevel == NKM_USER_AUTH_LEVEL.SUPER_ADMIN)
		{
			return true;
		}
		return false;
	}

	public bool CheckDungeonClear(int dungeonID)
	{
		if (dungeonID == 0)
		{
			return true;
		}
		if (m_dicNKMDungeonClearData.ContainsKey(dungeonID))
		{
			return true;
		}
		return false;
	}

	public bool CheckDungeonClear(string dungeonStrID)
	{
		if (string.IsNullOrEmpty(dungeonStrID))
		{
			return true;
		}
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(dungeonStrID);
		if (dungeonTempletBase == null)
		{
			return false;
		}
		return CheckDungeonClear(dungeonTempletBase.m_DungeonID);
	}

	public bool CheckWarfareClear(string warfareStrID)
	{
		if (string.IsNullOrEmpty(warfareStrID))
		{
			return true;
		}
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(warfareStrID);
		if (nKMWarfareTemplet == null)
		{
			return false;
		}
		return CheckWarfareClear(nKMWarfareTemplet.m_WarfareID);
	}

	public bool CheckWarfareClear(int warfareID)
	{
		if (warfareID == 0)
		{
			return true;
		}
		if (m_dicNKMWarfareClearData.ContainsKey(warfareID))
		{
			return true;
		}
		return false;
	}

	public bool CheckDiveClear(int stageID)
	{
		return m_DiveClearData.Contains(stageID);
	}

	public NKMDungeonClearData GetDungeonClearData(string dungeonStrID)
	{
		if (string.IsNullOrEmpty(dungeonStrID))
		{
			return null;
		}
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(dungeonStrID);
		if (dungeonTempletBase == null)
		{
			return null;
		}
		return GetDungeonClearData(dungeonTempletBase.m_DungeonID);
	}

	public NKMDungeonClearData GetDungeonClearData(int dungeonID)
	{
		if (m_dicNKMDungeonClearData.ContainsKey(dungeonID))
		{
			return m_dicNKMDungeonClearData[dungeonID];
		}
		return null;
	}

	public void SetDungeonClearData(NKMDungeonClearData cNKMDungeonClearData)
	{
		if (cNKMDungeonClearData != null && cNKMDungeonClearData.dungeonId > 0)
		{
			if (m_dicNKMDungeonClearData.ContainsKey(cNKMDungeonClearData.dungeonId))
			{
				m_dicNKMDungeonClearData[cNKMDungeonClearData.dungeonId] = cNKMDungeonClearData;
			}
			else
			{
				m_dicNKMDungeonClearData.Add(cNKMDungeonClearData.dungeonId, cNKMDungeonClearData);
			}
		}
	}

	public NKMWarfareClearData GetWarfareClearData(string warfareStrID)
	{
		if (string.IsNullOrEmpty(warfareStrID))
		{
			return null;
		}
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(warfareStrID);
		if (nKMWarfareTemplet == null)
		{
			return null;
		}
		return GetWarfareClearData(nKMWarfareTemplet.m_WarfareID);
	}

	public NKMWarfareClearData GetWarfareClearData(int warfareID)
	{
		if (m_dicNKMWarfareClearData.ContainsKey(warfareID))
		{
			return m_dicNKMWarfareClearData[warfareID];
		}
		return null;
	}

	public bool CheckPrice(long price, int itemID)
	{
		if (price < 0)
		{
			return false;
		}
		if (itemID == 0)
		{
			return true;
		}
		return price <= m_InventoryData.GetCountMiscItem(itemID);
	}

	public void AddCounterCaseData(int dungeonID, bool unlocked)
	{
		if (!m_dicNKMCounterCaseData.ContainsKey(dungeonID))
		{
			NKMCounterCaseData value = new NKMCounterCaseData(dungeonID, unlocked);
			m_dicNKMCounterCaseData.Add(dungeonID, value);
		}
	}

	public bool CheckUnlockedCounterCase(string dungeonStrID)
	{
		if (string.IsNullOrEmpty(dungeonStrID))
		{
			return true;
		}
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(dungeonStrID);
		if (dungeonTempletBase == null)
		{
			return false;
		}
		return CheckUnlockedCounterCase(dungeonTempletBase.m_DungeonID);
	}

	public bool CheckUnlockedCounterCase(int dungeonID)
	{
		NKMCounterCaseData value = null;
		if (m_dicNKMCounterCaseData.TryGetValue(dungeonID, out value))
		{
			return value.m_Unlocked;
		}
		return false;
	}

	public void UpdateEpisodeCompleteData(NKMEpisodeCompleteData episodeCompleteData)
	{
		if (episodeCompleteData != null)
		{
			EpisodeCompleteKey episodeCompleteKey = new EpisodeCompleteKey(episodeCompleteData.m_EpisodeID, (int)episodeCompleteData.m_EpisodeDifficulty);
			if (m_dicEpisodeCompleteData.TryGetValue(episodeCompleteKey.m_EpisodeKey, out var value))
			{
				value.DeepCopyFromSource(episodeCompleteData);
			}
			else
			{
				m_dicEpisodeCompleteData.Add(episodeCompleteKey.m_EpisodeKey, episodeCompleteData);
			}
		}
	}

	public NKMEpisodeCompleteData GetEpisodeCompleteData(int episodeID, EPISODE_DIFFICULTY episodeDifficulty)
	{
		EpisodeCompleteKey episodeCompleteKey = new EpisodeCompleteKey(episodeID, (int)episodeDifficulty);
		m_dicEpisodeCompleteData.TryGetValue(episodeCompleteKey.m_EpisodeKey, out var value);
		return value;
	}

	public int GetCounterCaseClearCount(int unitId)
	{
		List<NKMStageTempletV2> list = null;
		list = ((unitId != 0) ? NKMEpisodeMgr.GetCounterCaseTemplets(unitId) : NKMEpisodeMgr.GetAllCounterCaseTemplets());
		if (list == null || list.Count <= 0)
		{
			return 0;
		}
		int num = 0;
		foreach (NKMStageTempletV2 item in list)
		{
			switch (item.m_STAGE_TYPE)
			{
			case STAGE_TYPE.ST_DUNGEON:
				if (item.DungeonTempletBase != null && CheckDungeonClear(item.DungeonTempletBase.m_DungeonID))
				{
					num++;
				}
				break;
			case STAGE_TYPE.ST_WARFARE:
				if (item.WarfareTemplet != null && CheckWarfareClear(item.WarfareTemplet.m_WarfareID))
				{
					num++;
				}
				break;
			case STAGE_TYPE.ST_PHASE:
				Log.Error("CounterCase Can't have phase!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUserData.cs", 1105);
				break;
			}
		}
		return num;
	}

	public bool CanDelete(NKMUnitData unitData)
	{
		if (m_ArmyData.GetShipFromUID(unitData.m_UnitUID) != null)
		{
			if (m_ArmyData.IsShipInAnyDeck(unitData.m_UnitUID))
			{
				return false;
			}
		}
		else
		{
			if (m_ArmyData.GetUnitFromUID(unitData.m_UnitUID) == null)
			{
				return false;
			}
			if (backGroundInfo.unitInfoList.Find((NKMBackgroundUnitInfo e) => e.unitUid == unitData.m_UnitUID) != null)
			{
				return false;
			}
			if (m_ArmyData.IsUnitInAnyDeck(unitData.m_UnitUID))
			{
				return false;
			}
			if (unitData.GetEquipItemAccessoryUid() != 0L || unitData.GetEquipItemDefenceUid() != 0L || unitData.GetEquipItemWeaponUid() != 0L || unitData.GetEquipItemAccessory2Uid() != 0L)
			{
				return false;
			}
			foreach (NKMWorldMapCityData value in m_WorldmapData.worldMapCityDataMap.Values)
			{
				if (value.leaderUnitUID == unitData.m_UnitUID)
				{
					return false;
				}
			}
		}
		return true;
	}

	public void SetMyUserProfileInfo(NKMUserProfileData cData)
	{
		m_UserProfileData = cData;
	}

	public void SetLastPlayInfo(NKM_GAME_TYPE gameType, int stageID)
	{
		switch (gameType)
		{
		case NKM_GAME_TYPE.NGT_PRACTICE:
		case NKM_GAME_TYPE.NGT_DUNGEON:
		case NKM_GAME_TYPE.NGT_TUTORIAL:
		case NKM_GAME_TYPE.NGT_CUTSCENE:
		case NKM_GAME_TYPE.NGT_SHADOW_PALACE:
		case NKM_GAME_TYPE.NGT_FIERCE:
		case NKM_GAME_TYPE.NGT_PHASE:
		case NKM_GAME_TYPE.NGT_TRIM:
			Log.Debug($"[LastPlayInfo][SetLastPlayInfo] GameType[{gameType}] StageId[{stageID}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMUserDataEx.cs", 333);
			m_LastPlayInfo.gameType = (int)gameType;
			m_LastPlayInfo.stageId = stageID;
			break;
		}
	}

	public NKMBackgroundUnitInfo GetBackgroundUnitInfo(int index)
	{
		if (backGroundInfo == null)
		{
			return null;
		}
		if (backGroundInfo.unitInfoList == null)
		{
			return null;
		}
		if (index < backGroundInfo.unitInfoList.Count)
		{
			return backGroundInfo.unitInfoList[index];
		}
		return null;
	}

	public int GetBackgroundUnitIndex(long uid)
	{
		if (backGroundInfo == null)
		{
			return -1;
		}
		if (backGroundInfo.unitInfoList == null)
		{
			return -1;
		}
		for (int i = 0; i < backGroundInfo.unitInfoList.Count; i++)
		{
			if (backGroundInfo.unitInfoList[i].unitUid == uid)
			{
				return i;
			}
		}
		return -1;
	}

	public bool PredictUseCash(int CashCost, out long newCash, out long newBonusCash)
	{
		long cashPaid = GetCashPaid();
		long cashBonus = GetCashBonus();
		bool num = CashCost <= cashPaid + cashBonus;
		if (num)
		{
			if (cashBonus >= CashCost)
			{
				newBonusCash = cashBonus - CashCost;
				newCash = cashPaid;
				return num;
			}
			newCash = cashPaid + cashBonus - CashCost;
			newBonusCash = 0L;
			return num;
		}
		newCash = cashPaid;
		newBonusCash = cashBonus;
		return num;
	}

	public bool CheckDungeonOneTimeReward(int warfareID, int index)
	{
		if (warfareID == 0)
		{
			return true;
		}
		if (index < 0)
		{
			return false;
		}
		if (m_dicNKMDungeonClearData.TryGetValue(warfareID, out var value))
		{
			if (value.onetimeRewardResults == null || value.onetimeRewardResults.Count <= index)
			{
				return false;
			}
			return value.onetimeRewardResults[index];
		}
		return false;
	}

	public bool CheckWarfareOneTimeReward(int warfareID, int index)
	{
		if (warfareID == 0)
		{
			return true;
		}
		if (index < 0)
		{
			return false;
		}
		if (m_dicNKMWarfareClearData.TryGetValue(warfareID, out var value))
		{
			if (value.m_OnetimeRewardResults == null || value.m_OnetimeRewardResults.Count <= index)
			{
				return false;
			}
			return value.m_OnetimeRewardResults[index];
		}
		return false;
	}

	public void GetReward(NKMRewardData rewardData, NKMAdditionalReward additionalRewardData)
	{
		GetReward(rewardData);
		GetReward(additionalRewardData);
	}

	public void GetReward(List<NKMRewardData> lstRewardData)
	{
		if (lstRewardData == null)
		{
			return;
		}
		foreach (NKMRewardData lstRewardDatum in lstRewardData)
		{
			GetReward(lstRewardDatum);
		}
	}

	public void GetReward(NKMRewardData rewardData)
	{
		if (rewardData == null)
		{
			return;
		}
		int futureUserLevel = NKCExpManager.GetFutureUserLevel(this, rewardData.UserExp);
		if (futureUserLevel > UserLevel)
		{
			NKCContentManager.SetLevelChanged(bValue: true);
			NKCPublisherModule.Statistics.OnUserLevelUp(futureUserLevel);
			for (int i = UserLevel + 1; i <= futureUserLevel; i++)
			{
				NKCMMPManager.OnUserLevelUp(i);
			}
		}
		UserLevelEXP = NKCExpManager.GetFutureUserRemainEXP(this, rewardData.UserExp);
		UserLevel = futureUserLevel;
		if (rewardData.UnitDataList != null)
		{
			foreach (NKMUnitData unitData in rewardData.UnitDataList)
			{
				switch (NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID).m_NKM_UNIT_TYPE)
				{
				case NKM_UNIT_TYPE.NUT_SHIP:
					m_ArmyData.AddNewShip(unitData);
					break;
				case NKM_UNIT_TYPE.NUT_NORMAL:
					m_ArmyData.AddNewUnit(unitData);
					break;
				default:
					Log.Error("Undefined Unittype, unitID : " + unitData.m_UnitID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMUserDataEx.cs", 529);
					break;
				}
			}
		}
		if (rewardData.ContractList != null)
		{
			foreach (MiscContractResult contract in rewardData.contractList)
			{
				if (contract == null)
				{
					continue;
				}
				foreach (NKMUnitData unit in contract.units)
				{
					switch (NKMUnitManager.GetUnitTempletBase(unit.m_UnitID).m_NKM_UNIT_TYPE)
					{
					case NKM_UNIT_TYPE.NUT_SHIP:
						m_ArmyData.AddNewShip(unit);
						break;
					case NKM_UNIT_TYPE.NUT_NORMAL:
						m_ArmyData.AddNewUnit(unit);
						break;
					default:
						Log.Error("Undefined Unittype, unitID : " + unit.m_UnitID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMUserDataEx.cs", 556);
						break;
					}
				}
			}
		}
		if (rewardData.OperatorList != null)
		{
			foreach (NKMOperator @operator in rewardData.OperatorList)
			{
				m_ArmyData.AddNewOperator(@operator);
			}
		}
		if (rewardData.MiscItemDataList != null)
		{
			foreach (NKMItemMiscData miscItemData in rewardData.MiscItemDataList)
			{
				m_InventoryData.AddItemMisc(miscItemData);
			}
		}
		if (rewardData.EquipItemDataList != null)
		{
			foreach (NKMEquipItemData equipItemData in rewardData.EquipItemDataList)
			{
				m_InventoryData.AddItemEquip(equipItemData);
			}
		}
		if (rewardData.UnitExpDataList != null)
		{
			foreach (NKMRewardUnitExpData unitExpData in rewardData.UnitExpDataList)
			{
				NKMUnitData unitFromUID = m_ArmyData.GetUnitFromUID(unitExpData.m_UnitUid);
				if (unitFromUID != null)
				{
					NKCExpManager.CalculateFutureUnitExpAndLevel(unitFromUID, unitExpData.m_Exp + unitExpData.m_BonusExp, out unitFromUID.m_UnitLevel, out unitFromUID.m_iUnitLevelEXP);
				}
			}
		}
		if (rewardData.SkinIdList != null)
		{
			foreach (int skinId in rewardData.SkinIdList)
			{
				m_InventoryData.AddItemSkin(skinId);
			}
		}
		if (rewardData.MoldItemDataList != null)
		{
			foreach (NKMMoldItemData moldItemData in rewardData.MoldItemDataList)
			{
				m_CraftData.AddMoldItem(moldItemData);
			}
		}
		if (rewardData.CompanyBuffDataList != null)
		{
			foreach (NKMCompanyBuffData companyBuffData in rewardData.CompanyBuffDataList)
			{
				NKCCompanyBuff.UpsertCompanyBuffData(m_companyBuffDataList, companyBuffData);
			}
		}
		if (rewardData.EmoticonList != null && NKCEmoticonManager.m_bReceivedEmoticonData)
		{
			foreach (int emoticon in rewardData.EmoticonList)
			{
				NKCEmoticonManager.AddEmoticonData(emoticon);
			}
		}
		if (rewardData.BingoTileList != null)
		{
			foreach (NKMBingoTile bingoTile in rewardData.BingoTileList)
			{
				NKMEventManager.GetBingoData(bingoTile.eventId)?.MarkToLine(bingoTile.tileIndex);
			}
		}
		if (rewardData.Interiors != null)
		{
			OfficeData.AddInteriorData(rewardData.Interiors);
		}
		if (rewardData.AchievePoint > 0)
		{
			m_MissionData.AddAchievePoint(rewardData.AchievePoint);
		}
	}

	public void GetReward(NKMAdditionalReward rewardData)
	{
	}

	public bool CheckDiveHistory(int stageID)
	{
		return m_DiveHistoryData.Contains(stageID);
	}

	public bool IsCurrentDiveGameIsWorldmapEventDive()
	{
		if (m_DiveGameData == null)
		{
			return false;
		}
		return m_WorldmapData.GetCityIDByEventData(NKM_WORLDMAP_EVENT_TYPE.WET_DIVE, m_DiveGameData.DiveUid) >= 0;
	}

	public void SetDungeonClearDataOnlyTrue(NKMDungeonClearData cNKMDungeonClearData)
	{
		if (cNKMDungeonClearData == null || cNKMDungeonClearData.dungeonId <= 0)
		{
			return;
		}
		if (m_dicNKMDungeonClearData.ContainsKey(cNKMDungeonClearData.dungeonId))
		{
			NKMDungeonClearData nKMDungeonClearData = m_dicNKMDungeonClearData[cNKMDungeonClearData.dungeonId];
			if (cNKMDungeonClearData.missionResult1)
			{
				nKMDungeonClearData.missionResult1 = true;
			}
			if (cNKMDungeonClearData.missionResult2)
			{
				nKMDungeonClearData.missionResult2 = true;
			}
			if (cNKMDungeonClearData.onetimeRewardResults == null)
			{
				return;
			}
			for (int i = 0; i < cNKMDungeonClearData.onetimeRewardResults.Count; i++)
			{
				if (cNKMDungeonClearData.onetimeRewardResults[i] && i < nKMDungeonClearData.onetimeRewardResults.Count)
				{
					nKMDungeonClearData.onetimeRewardResults[i] = true;
				}
			}
		}
		else
		{
			m_dicNKMDungeonClearData.Add(cNKMDungeonClearData.dungeonId, cNKMDungeonClearData);
			NKCContentManager.SetUnlockedContent(STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON, cNKMDungeonClearData.dungeonId);
		}
	}

	public void SetWarfareClearDataOnlyTrue(NKMWarfareClearData cNKMWarfareClearData)
	{
		if (cNKMWarfareClearData == null || cNKMWarfareClearData.m_WarfareID <= 0)
		{
			return;
		}
		if (m_dicNKMWarfareClearData.ContainsKey(cNKMWarfareClearData.m_WarfareID))
		{
			NKMWarfareClearData nKMWarfareClearData = m_dicNKMWarfareClearData[cNKMWarfareClearData.m_WarfareID];
			if (cNKMWarfareClearData.m_mission_result_1)
			{
				nKMWarfareClearData.m_mission_result_1 = true;
			}
			if (cNKMWarfareClearData.m_mission_result_2)
			{
				nKMWarfareClearData.m_mission_result_2 = true;
			}
			if (cNKMWarfareClearData.m_OnetimeRewardResults == null)
			{
				return;
			}
			for (int i = 0; i < cNKMWarfareClearData.m_OnetimeRewardResults.Count; i++)
			{
				if (cNKMWarfareClearData.m_OnetimeRewardResults[i] && i < nKMWarfareClearData.m_OnetimeRewardResults.Count)
				{
					nKMWarfareClearData.m_OnetimeRewardResults[i] = true;
				}
			}
		}
		else
		{
			m_dicNKMWarfareClearData.Add(cNKMWarfareClearData.m_WarfareID, cNKMWarfareClearData);
		}
	}

	public void ClearDiveGameData()
	{
		m_DiveGameData = null;
		m_ArmyData.ResetDeckStateOf(NKM_DECK_STATE.DECK_STATE_DIVE);
	}

	public void SetEquipTuningData(NKMEquipTuningCandidate tuningData)
	{
		m_EquipTuningCandidate = tuningData;
	}

	public bool IsPossibleTuning(long itemUID, int idx = 0)
	{
		if (itemUID != m_EquipTuningCandidate.equipUid)
		{
			return false;
		}
		return idx switch
		{
			0 => m_EquipTuningCandidate.option1 != NKM_STAT_TYPE.NST_RANDOM, 
			1 => m_EquipTuningCandidate.option2 != NKM_STAT_TYPE.NST_RANDOM, 
			_ => false, 
		};
	}

	public NKMEquipTuningCandidate GetTuiningData()
	{
		return m_EquipTuningCandidate;
	}

	public bool hasReservedEquipCandidate()
	{
		if (m_EquipTuningCandidate != null)
		{
			if (!m_InventoryData.ItemEquipData.ContainsKey(m_EquipTuningCandidate.equipUid))
			{
				return false;
			}
			if (m_EquipTuningCandidate.option1 != NKM_STAT_TYPE.NST_RANDOM || m_EquipTuningCandidate.option2 != NKM_STAT_TYPE.NST_RANDOM)
			{
				return true;
			}
			if (m_EquipTuningCandidate.setOptionId != 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool hasReservedEquipTuningData()
	{
		if ((m_EquipTuningCandidate != null && m_EquipTuningCandidate.option1 != NKM_STAT_TYPE.NST_RANDOM) || m_EquipTuningCandidate.option2 != NKM_STAT_TYPE.NST_RANDOM)
		{
			return true;
		}
		return false;
	}

	public void SetEquipPotentialData(NKMPotentialOptionChangeCandidate potentialData)
	{
		m_PotentialOptionCandidate = potentialData;
	}

	public NKMPotentialOptionChangeCandidate GetPotentialData()
	{
		return m_PotentialOptionCandidate;
	}

	public bool hasReservedHiddenOptionRerollData()
	{
		if (m_PotentialOptionCandidate != null && m_PotentialOptionCandidate.equipUid > 0)
		{
			return true;
		}
		return false;
	}

	public void SetShipCandidateData(NKMShipModuleCandidate candidateData)
	{
		m_ShipCmdModuleCandidate = candidateData;
	}

	public NKMShipModuleCandidate GetShipCandidateData()
	{
		return m_ShipCmdModuleCandidate;
	}

	public bool HaveEnoughResourceToBuy(ShopItemTemplet productTemplet, int ProductCount)
	{
		if (productTemplet == null)
		{
			return false;
		}
		int realPrice = m_ShopData.GetRealPrice(productTemplet, ProductCount);
		return CheckPrice(realPrice, productTemplet.m_PriceItemID);
	}

	public bool HasBuff(int buffId)
	{
		if (m_companyBuffDataList != null)
		{
			for (int i = 0; i < m_companyBuffDataList.Count; i++)
			{
				if (m_companyBuffDataList[i].Id == buffId)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool HasBuffGroup(int groupId)
	{
		if (m_companyBuffDataList != null)
		{
			int i;
			for (i = 0; i < m_companyBuffDataList.Count; i++)
			{
				GuildWelfareTemplet guildWelfareTemplet = NKMTempletContainer<GuildWelfareTemplet>.Find((GuildWelfareTemplet x) => x.CompanyBuffID == m_companyBuffDataList[i].Id);
				if (guildWelfareTemplet != null && guildWelfareTemplet.CompanyBuffGroupID == groupId)
				{
					return true;
				}
			}
		}
		return false;
	}

	public DateTime GetBuffExpireTimeByGroupId(int buffGroupId)
	{
		if (HasBuffGroup(buffGroupId))
		{
			int i;
			for (i = 0; i < m_companyBuffDataList.Count; i++)
			{
				GuildWelfareTemplet guildWelfareTemplet = NKMTempletContainer<GuildWelfareTemplet>.Find((GuildWelfareTemplet x) => x.CompanyBuffID == m_companyBuffDataList[i].Id);
				if (guildWelfareTemplet != null && guildWelfareTemplet.CompanyBuffGroupID == buffGroupId)
				{
					return m_companyBuffDataList[i].ExpireDate;
				}
			}
		}
		return default(DateTime);
	}

	public void SetMyUserProfileInfo(NKMSupportUnitData supportUnitData)
	{
		m_SupportUnitData = supportUnitData;
	}

	public void ActiveMiniGameTemplets(List<int> lstActiveMiniGameIds)
	{
		m_lstActiveMiniGameTempletIDs = lstActiveMiniGameIds;
	}

	public bool IsActiveMiniGame(int miniGameTempletId)
	{
		return m_lstActiveMiniGameTempletIDs.Contains(miniGameTempletId);
	}

	public void SetMiniGameData(NKMMiniGameData miniGameData)
	{
		foreach (NKMMiniGameData lstMiniGameData in m_lstMiniGameDatas)
		{
			if (lstMiniGameData.templetId == miniGameData.templetId)
			{
				lstMiniGameData.score = miniGameData.score;
				return;
			}
		}
		m_lstMiniGameDatas.Add(miniGameData);
	}

	public void SetMiniGameData(List<NKMMiniGameData> lstMiniGameData)
	{
		m_lstMiniGameDatas = lstMiniGameData;
	}

	public NKMMiniGameData GetMiniGameData(NKM_MINI_GAME_TYPE gameType, int tempetID)
	{
		foreach (NKMMiniGameData lstMiniGameData in m_lstMiniGameDatas)
		{
			if (gameType == lstMiniGameData.type && lstMiniGameData.templetId == tempetID)
			{
				return lstMiniGameData;
			}
		}
		return null;
	}

	public void SetMiniGameReceviedIds(int rewardID)
	{
		if (m_lstRecivedRewardIds.Contains(rewardID))
		{
			Debug.Log($"<color=red>[SetMiniGameReceviedIds]dobule reward? - {rewardID}</color>");
		}
		else
		{
			m_lstRecivedRewardIds.Add(rewardID);
		}
	}

	public void SetMiniGameReceviedIds(List<int> lstRewardIds)
	{
		foreach (int lstRewardId in lstRewardIds)
		{
			SetMiniGameReceviedIds(lstRewardId);
		}
	}

	public bool GetIsMiniGameReceviedID(int rewardID)
	{
		return m_lstRecivedRewardIds.Contains(rewardID);
	}

	public long GetEterniumCap()
	{
		return NKCExpManager.GetUserExpTemplet(this).m_EterniumCap;
	}

	public float GetEterniumCapProgress()
	{
		float num = GetEternium();
		float num2 = GetEterniumCap();
		if (num >= num2 || num2 == 0f)
		{
			return 1f;
		}
		return num / num2;
	}

	public void SetUpdateDate(NKMPacket_CHARGE_ITEM_NOT sPacket)
	{
		if (sPacket.itemData.ItemID == 2)
		{
			m_lastUpdateEterniumCap = sPacket.lastUpdateDate;
		}
		else if (sPacket.itemData.ItemID == 13)
		{
			m_lastUpdateAsyncTicket = sPacket.lastUpdateDate;
		}
	}

	public void SetReturningUserStates(List<NKMReturningUserState> lstStates)
	{
		m_dicReturningUserState.Clear();
		for (int i = 0; i < lstStates.Count; i++)
		{
			if (!m_dicReturningUserState.ContainsKey(lstStates[i].type))
			{
				m_dicReturningUserState.Add(lstStates[i].type, lstStates[i]);
			}
			else
			{
				Log.Error($"{lstStates[i].type} is already exist!!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMUserDataEx.cs", 1029);
			}
		}
	}

	public DateTime GetReturnStartDate(ReturningUserType state)
	{
		if (state == ReturningUserType.None)
		{
			return default(DateTime);
		}
		if (!m_dicReturningUserState.ContainsKey(state))
		{
			return default(DateTime);
		}
		return m_dicReturningUserState[state].startDate;
	}

	public DateTime GetReturnEndDate(ReturningUserType state)
	{
		if (state == ReturningUserType.None)
		{
			return default(DateTime);
		}
		if (!m_dicReturningUserState.ContainsKey(state))
		{
			return default(DateTime);
		}
		return m_dicReturningUserState[state].endDate;
	}

	public bool IsReturnUser()
	{
		foreach (NKMReturningUserState value in m_dicReturningUserState.Values)
		{
			if (NKCSynchronizedTime.IsEventTime(value.startDate, value.endDate))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsReturnUser(ReturningUserType type)
	{
		if (m_dicReturningUserState.TryGetValue(type, out var value) && NKCSynchronizedTime.IsEventTime(value.startDate, value.endDate))
		{
			return true;
		}
		return false;
	}

	public bool IsNewbieUser(int newbieDay)
	{
		return NKCSynchronizedTime.GetServerUTCTime() <= GetNewbieEndDate(newbieDay);
	}

	public DateTime GetNewbieEndDate(int newbieDay)
	{
		return m_NKMUserDateData.m_RegisterTime.AddHours(newbieDay * 24);
	}

	public void SetStagePlayData(List<NKMStagePlayData> lstStagePlayData)
	{
		if (lstStagePlayData == null)
		{
			return;
		}
		m_dicStagePlayData.Clear();
		foreach (NKMStagePlayData lstStagePlayDatum in lstStagePlayData)
		{
			m_dicStagePlayData.Add(lstStagePlayDatum.stageId, lstStagePlayDatum);
		}
	}

	public void UpdateStagePlayData(NKMStagePlayData stagePlayData)
	{
		if (stagePlayData == null)
		{
			return;
		}
		CheckFierceDailyRewardReset(stagePlayData);
		m_dicStagePlayData[stagePlayData.stageId] = stagePlayData;
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME)
		{
			NKC_SCEN_WARFARE_GAME nKC_SCEN_WARFARE_GAME = NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME();
			if (nKC_SCEN_WARFARE_GAME != null)
			{
				NKCWarfareGame warfareGame = nKC_SCEN_WARFARE_GAME.GetWarfareGame();
				if (warfareGame != null)
				{
					warfareGame.m_NKCWarfareGameHUD.UpdateStagePlayState();
					warfareGame.m_NKCWarfareGameHUD.SelfUpdateAttackCost();
				}
			}
		}
		if (NKCUIPrepareEventDeck.IsInstanceOpen)
		{
			NKCUIPrepareEventDeck.Instance.UpdateEnterLimitUI();
		}
		if (NKCUIDeckViewer.IsInstanceOpen)
		{
			NKCUIDeckViewer.Instance.UpdateEnterLimitUI();
		}
		if (NKCPopupFavorite.isOpen())
		{
			NKCPopupFavorite.Instance.RefreshData();
		}
	}

	public bool IsHaveStatePlayData(int stageID)
	{
		return m_dicStagePlayData.ContainsKey(stageID);
	}

	public int GetStatePlayCnt(int stageID, bool IsServiceTime = false, bool bSkipNextResetData = false, bool bTotalCnt = false)
	{
		if (m_dicStagePlayData.TryGetValue(stageID, out var value))
		{
			_ = value.nextResetDate;
			if (!bSkipNextResetData)
			{
				DateTime finishTime = value.nextResetDate;
				if (IsServiceTime)
				{
					finishTime = NKMTime.LocalToUTC(value.nextResetDate);
				}
				if (NKCSynchronizedTime.GetTimeLeft(finishTime).Ticks <= 0)
				{
					return 0;
				}
			}
			if (!bTotalCnt)
			{
				return (int)value.playCount;
			}
			return (int)value.totalPlayCount;
		}
		return 0;
	}

	private void CheckFierceDailyRewardReset(NKMStagePlayData stagePlayData)
	{
		if (stagePlayData != null && stagePlayData.stageId == NKMFierceConst.StageId && m_dicStagePlayData.ContainsKey(NKMFierceConst.StageId) && m_dicStagePlayData[stagePlayData.stageId].nextResetDate != stagePlayData.nextResetDate)
		{
			NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr().SetDailyRewardReceived(bfierceDailyRewardReceived: false);
		}
	}

	public long GetStageKillCountBest(int stageID)
	{
		if (m_dicStagePlayData.TryGetValue(stageID, out var value))
		{
			return value.bestKillCount;
		}
		return 0L;
	}

	public int GetStageRestoreCnt(int stageID)
	{
		if (m_dicStagePlayData.TryGetValue(stageID, out var value) && value != null)
		{
			_ = value.nextResetDate;
			if (NKCSynchronizedTime.GetTimeLeft(value.nextResetDate).Ticks <= 0)
			{
				return 0;
			}
			return (int)value.restoreCount;
		}
		return 0;
	}

	public int GetStageBestClearSec(int stageID)
	{
		if (m_dicStagePlayData.TryGetValue(stageID, out var value) && value != null)
		{
			_ = value.nextResetDate;
			if (value.nextResetDate.Ticks > 0 && NKCSynchronizedTime.GetTimeLeft(value.nextResetDate).Ticks != 0L)
			{
				return 0;
			}
			return value.bestClearTimeSec;
		}
		return 0;
	}

	public bool CheckStageCleared(int stageID)
	{
		NKMStageTempletV2 stageTemplet = NKMStageTempletV2.Find(stageID);
		return CheckStageCleared(stageTemplet);
	}

	public bool CheckStageCleared(NKMGameData gameData)
	{
		if (gameData == null)
		{
			return false;
		}
		switch (gameData.GetGameType())
		{
		default:
			return CheckDungeonClear(gameData.m_DungeonID);
		case NKM_GAME_TYPE.NGT_WARFARE:
			return CheckWarfareClear(gameData.m_WarfareID);
		case NKM_GAME_TYPE.NGT_PHASE:
			if (NKCPhaseManager.IsCurrentPhaseDungeon(gameData.m_DungeonID))
			{
				return NKCPhaseManager.CheckPhaseStageClear(NKCPhaseManager.GetStageTemplet());
			}
			return false;
		}
	}

	public bool CheckStageCleared(NKMStageTempletV2 stageTemplet)
	{
		if (stageTemplet == null)
		{
			return false;
		}
		return stageTemplet.m_STAGE_TYPE switch
		{
			STAGE_TYPE.ST_DUNGEON => CheckDungeonClear(stageTemplet.DungeonTempletBase.m_DungeonID), 
			STAGE_TYPE.ST_WARFARE => CheckWarfareClear(stageTemplet.WarfareTemplet.m_WarfareID), 
			STAGE_TYPE.ST_PHASE => NKCPhaseManager.CheckPhaseStageClear(stageTemplet), 
			_ => false, 
		};
	}

	public bool CheckPhaseClear(int phaseID)
	{
		if (phaseID == 0)
		{
			return true;
		}
		NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(phaseID);
		if (nKMStageTempletV == null)
		{
			return false;
		}
		if (nKMStageTempletV.m_STAGE_TYPE != STAGE_TYPE.ST_PHASE)
		{
			return false;
		}
		return NKCPhaseManager.CheckPhaseStageClear(nKMStageTempletV);
	}

	public bool hasReservedSetOptionData()
	{
		if (m_EquipTuningCandidate != null && m_EquipTuningCandidate.setOptionId != 0)
		{
			return true;
		}
		return false;
	}

	public int GetReservedSetOption(long ItemUID)
	{
		if (m_EquipTuningCandidate != null && m_EquipTuningCandidate.equipUid == ItemUID)
		{
			return m_EquipTuningCandidate.setOptionId;
		}
		return 0;
	}

	public void UpdateMyMenteeMatchList(List<MenteeInfo> lstMatch)
	{
		m_MyMentoringData.lstMenteeMatch = lstMatch;
	}

	public void UpdateReceiveList(List<FriendListData> recommend, List<FriendListData> invited = null)
	{
		m_MyMentoringData.lstRecommend = recommend;
		m_MyMentoringData.lstInvited = invited;
	}

	public void UpdateMyMentor(FriendListData myMentor, bool bGraduate = false)
	{
		m_MyMentoringData.MyMentor = myMentor;
		m_MyMentoringData.bMenteeGraduate = bGraduate;
	}

	public void UpdateMentoringSeasonID(int seasonID)
	{
		m_MyMentoringData.SeasonId = seasonID;
	}

	public void DeleteMentee(long deleteMenteeUID)
	{
		foreach (MenteeInfo item in m_MyMentoringData.lstMenteeMatch)
		{
			if (item.data.commonProfile.userUid == deleteMenteeUID)
			{
				m_MyMentoringData.lstMenteeMatch.Remove(item);
				break;
			}
		}
	}

	public void DeleteRecommandMentee(FriendListData menteeInfo)
	{
		if (m_MyMentoringData.lstRecommend == null)
		{
			return;
		}
		foreach (FriendListData item in m_MyMentoringData.lstRecommend)
		{
			if (item.commonProfile.userUid == menteeInfo.commonProfile.userUid)
			{
				m_MyMentoringData.lstRecommend.Remove(item);
				break;
			}
		}
	}

	public int GetMenteeMissionCompletCnt()
	{
		int num = 0;
		foreach (MenteeInfo item in m_MyMentoringData.lstMenteeMatch)
		{
			if (item.state == MentoringState.Graduated)
			{
				num++;
			}
		}
		return num;
	}

	public void DeleteInvitedMentor(long deleteMentorUID)
	{
		if (m_MyMentoringData.lstInvited == null)
		{
			return;
		}
		foreach (FriendListData item in m_MyMentoringData.lstInvited)
		{
			if (item.commonProfile.userUid == deleteMentorUID)
			{
				m_MyMentoringData.lstInvited.Remove(item);
				break;
			}
		}
	}

	public string GetMenteeName(long menteeUID)
	{
		string result = "";
		foreach (MenteeInfo item in m_MyMentoringData.lstMenteeMatch)
		{
			if (item.data.commonProfile.userUid == menteeUID)
			{
				result = item.data.commonProfile.nickname;
				break;
			}
		}
		return result;
	}

	public string GetMentorName(long _mentorUID)
	{
		string text = "";
		if (m_MyMentoringData.lstInvited != null)
		{
			foreach (FriendListData item in m_MyMentoringData.lstInvited)
			{
				if (item.commonProfile.userUid == _mentorUID)
				{
					text = item.commonProfile.nickname;
					break;
				}
			}
		}
		if (string.IsNullOrEmpty(text) && m_MyMentoringData.lstRecommend != null)
		{
			foreach (FriendListData item2 in m_MyMentoringData.lstRecommend)
			{
				if (item2.commonProfile.userUid == _mentorUID)
				{
					text = item2.commonProfile.nickname;
					break;
				}
			}
		}
		return text;
	}

	public void SetMentoringNotify(bool bSet)
	{
		m_MyMentoringData.bMentoringNotify = bSet;
	}

	public void UpdateConsumerPackageData(NKMConsumerPackageData packageData)
	{
		if (packageData != null)
		{
			m_dicConsumerPackageData[packageData.productId] = packageData;
		}
	}

	public void UpdateConsumerPackageData(IEnumerable<NKMConsumerPackageData> lstPackageData)
	{
		if (lstPackageData == null)
		{
			return;
		}
		foreach (NKMConsumerPackageData lstPackageDatum in lstPackageData)
		{
			m_dicConsumerPackageData[lstPackageDatum.productId] = lstPackageDatum;
		}
	}

	public bool GetConsumerPackageData(int productID, out NKMConsumerPackageData data)
	{
		return m_dicConsumerPackageData.TryGetValue(productID, out data);
	}

	public void RemoveConsumerPackageData(int productID)
	{
		m_dicConsumerPackageData.Remove(productID);
	}

	public void RemoveConsumerPackageData(IEnumerable<int> lstProductID)
	{
		if (lstProductID == null)
		{
			return;
		}
		foreach (int item in lstProductID)
		{
			m_dicConsumerPackageData.Remove(item);
		}
	}

	public bool IsKakaoMissionOngoing()
	{
		if (kakaoMissionData != null)
		{
			return kakaoMissionData.state != KakaoMissionState.OutOfDate;
		}
		return false;
	}

	public RaceData GetRaceData()
	{
		return m_RaceEventData;
	}
}
