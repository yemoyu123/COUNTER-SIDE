using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Common;
using ClientPacket.Guild;
using Cs.Logging;
using NKM;
using NKM.Guild;
using NKM.Templet;

namespace NKC;

public static class NKCGuildCoopManager
{
	public enum BOSS_ORDER_TYPE
	{
		NONE,
		STOP,
		START
	}

	public static List<GuildDungeonArena> m_lstGuildDungeonArena = new List<GuildDungeonArena>();

	public static List<NKMRewardData> m_lstKillPointReward = new List<NKMRewardData>();

	public static List<NKMRewardData> m_lstTryCountReward = new List<NKMRewardData>();

	public static List<GuildDungeonSeasonRewardData> m_LastReceivedSeasonRewardData = new List<GuildDungeonSeasonRewardData>();

	public static Dictionary<int, GuildDungeonInfoTemplet> m_dicGuildDungeonInfoTemplet = new Dictionary<int, GuildDungeonInfoTemplet>();

	private static List<GuildDungeonMemberInfo> m_lstGuildDungeonMemberInfo = new List<GuildDungeonMemberInfo>();

	public static int m_ArenaTicketBuyCount = 0;

	public static GuildDungeonBoss m_BossData = new GuildDungeonBoss();

	private static int m_LastPlayedArenaID = 0;

	public const int EXTRA_BOSS_IDX = 15;

	public static GuildDungeonState m_GuildDungeonState { get; private set; }

	public static int m_SeasonId { get; private set; }

	public static int m_SessionId { get; private set; }

	public static DateTime m_SessionEndDateUTC { get; private set; }

	public static DateTime m_NextSessionStartDateUTC { get; private set; }

	public static long m_KillPoint { get; private set; }

	public static int m_TryCount { get; private set; }

	public static bool m_bCanReward { get; private set; }

	public static int m_ArenaPlayableCount { get; private set; }

	public static BOSS_ORDER_TYPE BossOrderIndex => (BOSS_ORDER_TYPE)m_BossData.orderIndex;

	public static GuildRaidTemplet m_cGuildRaidTemplet { get; private set; }

	public static float m_BossMaxHp { get; private set; }

	public static bool m_bGuildCoopDataRecved { get; private set; }

	public static bool m_bGuildCoopMemberDataRecved { get; private set; }

	public static bool m_bArenaResultRecved { get; private set; }

	public static bool m_bWaitForArenaResult { get; private set; }

	public static bool m_bGetArtifact { get; private set; }

	public static List<GuildDungeonMemberInfo> GetGuildDungeonMemberInfo()
	{
		return m_lstGuildDungeonMemberInfo;
	}

	public static void Initialize()
	{
		m_GuildDungeonState = GuildDungeonState.Invalid;
		m_SessionEndDateUTC = default(DateTime);
		m_NextSessionStartDateUTC = default(DateTime);
		m_SeasonId = 0;
		m_SessionId = 0;
		m_BossMaxHp = 0f;
		m_TryCount = 0;
		m_KillPoint = 0L;
		m_ArenaTicketBuyCount = 0;
		m_BossData = new GuildDungeonBoss();
		m_dicGuildDungeonInfoTemplet.Clear();
		m_lstGuildDungeonArena.Clear();
		m_lstGuildDungeonMemberInfo.Clear();
		m_LastReceivedSeasonRewardData.Clear();
		m_lstKillPointReward.Clear();
		m_lstTryCountReward.Clear();
		m_bCanReward = false;
		m_cGuildRaidTemplet = null;
		m_bGuildCoopDataRecved = false;
		m_bGuildCoopMemberDataRecved = false;
		m_bWaitForArenaResult = false;
		m_bArenaResultRecved = false;
		m_LastPlayedArenaID = 0;
	}

	public static void ResetGuildCoopState()
	{
		m_bGuildCoopDataRecved = false;
		m_bGuildCoopMemberDataRecved = false;
		m_bWaitForArenaResult = false;
		m_bArenaResultRecved = false;
		m_LastPlayedArenaID = 0;
		m_GuildDungeonState = GuildDungeonState.Invalid;
	}

	public static void ResetGuildCoopSessionData()
	{
		m_dicGuildDungeonInfoTemplet.Clear();
		m_lstGuildDungeonMemberInfo.Clear();
		m_ArenaTicketBuyCount = 0;
		m_ArenaPlayableCount = 0;
		m_BossData = new GuildDungeonBoss();
		m_cGuildRaidTemplet = null;
		m_bGuildCoopMemberDataRecved = false;
		m_bWaitForArenaResult = false;
		m_bArenaResultRecved = false;
		m_LastPlayedArenaID = 0;
	}

	public static void AddMyPoint(GuildDungeonRewardCategory category, int point)
	{
		switch (category)
		{
		case GuildDungeonRewardCategory.DUNGEON_TRY:
			m_TryCount += point;
			break;
		case GuildDungeonRewardCategory.RANK:
			m_KillPoint += point;
			break;
		}
	}

	public static long GetMyPoint(GuildDungeonRewardCategory category)
	{
		return category switch
		{
			GuildDungeonRewardCategory.DUNGEON_TRY => m_TryCount, 
			GuildDungeonRewardCategory.RANK => m_KillPoint, 
			_ => 0L, 
		};
	}

	public static int GetLastReceivedPoint(GuildDungeonRewardCategory category)
	{
		return m_LastReceivedSeasonRewardData.Find((GuildDungeonSeasonRewardData x) => x.category == category)?.receivedValue ?? 0;
	}

	public static bool CheckSeasonRewardEnable()
	{
		if (!CheckSeasonRewardEnable(GuildDungeonRewardCategory.DUNGEON_TRY))
		{
			return CheckSeasonRewardEnable(GuildDungeonRewardCategory.RANK);
		}
		return true;
	}

	public static bool CheckSeasonRewardEnable(GuildDungeonRewardCategory category)
	{
		GuildSeasonTemplet guildSeasonTemplet = GuildSeasonTemplet.Find(m_SeasonId);
		if (guildSeasonTemplet != null)
		{
			List<GuildSeasonRewardTemplet> list = GuildDungeonTempletManager.GetSeasonRewardList(guildSeasonTemplet.GetSeasonRewardGroup())?.FindAll((GuildSeasonRewardTemplet x) => x.GetRewardCategory() == category);
			if (list != null)
			{
				for (int num = 0; num < list.Count && list[num].GetRewardCountValue() <= GetMyPoint(category); num++)
				{
					if (list[num].GetRewardCountValue() > m_LastReceivedSeasonRewardData.Find((GuildDungeonSeasonRewardData x) => x.category == category).receivedValue)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public static int GetLastIndexRewardEnabled(GuildDungeonRewardCategory category)
	{
		int num = -1;
		GuildSeasonTemplet guildSeasonTemplet = GuildSeasonTemplet.Find(m_SeasonId);
		if (guildSeasonTemplet != null)
		{
			List<GuildSeasonRewardTemplet> list = GuildDungeonTempletManager.GetSeasonRewardList(guildSeasonTemplet.GetSeasonRewardGroup()).FindAll((GuildSeasonRewardTemplet x) => x.GetRewardCategory() == category);
			for (int num2 = 0; num2 < list.Count && list[num2].GetRewardCountValue() <= GetMyPoint(category); num2++)
			{
				if (list[num2].GetRewardCountValue() > m_LastReceivedSeasonRewardData.Find((GuildDungeonSeasonRewardData x) => x.category == category).receivedValue && num < num2)
				{
					num = num2;
				}
			}
		}
		return num;
	}

	public static int GetNextArtifactID(int arenaIdx)
	{
		if (m_dicGuildDungeonInfoTemplet.ContainsKey(arenaIdx))
		{
			return GuildDungeonTempletManager.GetDungeonArtifactList(m_dicGuildDungeonInfoTemplet[arenaIdx].GetStageRewardArtifactGroup()).Find((GuildDungeonArtifactTemplet x) => x.GetOrder() == GetCurrentArtifactCountByArena(arenaIdx) + 1)?.GetArtifactId() ?? 0;
		}
		return 0;
	}

	public static GuildDungeonArtifactTemplet GetNextArtifactTemplet(int arenaIdx)
	{
		if (m_dicGuildDungeonInfoTemplet.ContainsKey(arenaIdx))
		{
			List<GuildDungeonArtifactTemplet> dungeonArtifactList = GuildDungeonTempletManager.GetDungeonArtifactList(m_dicGuildDungeonInfoTemplet[arenaIdx].GetStageRewardArtifactGroup());
			if (!m_bGetArtifact)
			{
				return dungeonArtifactList.Find((GuildDungeonArtifactTemplet x) => x.GetOrder() == GetCurrentArtifactCountByArena(arenaIdx) + 1);
			}
			return dungeonArtifactList.Find((GuildDungeonArtifactTemplet x) => x.GetOrder() == GetCurrentArtifactCountByArena(arenaIdx));
		}
		return null;
	}

	public static float GetClearPointPercentage(int arenaIdx)
	{
		GuildDungeonArena guildDungeonArena = m_lstGuildDungeonArena.Find((GuildDungeonArena x) => x.arenaIndex == arenaIdx);
		if (guildDungeonArena != null)
		{
			return (float)(guildDungeonArena.totalMedalCount % NKMCommonConst.GuildDungeonConstTemplet.ArtifactFulificationCount) / (float)NKMCommonConst.GuildDungeonConstTemplet.ArtifactFulificationCount;
		}
		return 0f;
	}

	public static int GetTotalMedalCountByArena(int arenaIdx)
	{
		return m_lstGuildDungeonArena.Find((GuildDungeonArena x) => x.arenaIndex == arenaIdx)?.totalMedalCount ?? 0;
	}

	public static int GetCurrentArtifactCountByArena(int arenaIdx)
	{
		GuildDungeonArena guildDungeonArena = m_lstGuildDungeonArena.Find((GuildDungeonArena x) => x.arenaIndex == arenaIdx);
		if (guildDungeonArena != null)
		{
			return guildDungeonArena.totalMedalCount / NKMCommonConst.GuildDungeonConstTemplet.ArtifactFulificationCount;
		}
		return 0;
	}

	public static int GetCurrentArtifactFlagIndex(int arenaIdx)
	{
		return m_lstGuildDungeonArena.Find((GuildDungeonArena x) => x.arenaIndex == arenaIdx)?.flagIndex ?? (-1);
	}

	public static Dictionary<int, List<GuildDungeonArtifactTemplet>> GetAllArtifactDictionary()
	{
		Dictionary<int, List<GuildDungeonArtifactTemplet>> dictionary = new Dictionary<int, List<GuildDungeonArtifactTemplet>>();
		for (int i = 0; i < m_lstGuildDungeonArena.Count; i++)
		{
			int arenaIndex = m_lstGuildDungeonArena[i].arenaIndex;
			if (m_dicGuildDungeonInfoTemplet.ContainsKey(arenaIndex))
			{
				List<GuildDungeonArtifactTemplet> dungeonArtifactList = GuildDungeonTempletManager.GetDungeonArtifactList(m_dicGuildDungeonInfoTemplet[arenaIndex].GetStageRewardArtifactGroup());
				dictionary.Add(arenaIndex, dungeonArtifactList);
			}
		}
		return dictionary;
	}

	public static Dictionary<int, List<GuildDungeonArtifactTemplet>> GetMyArtifactDictionary()
	{
		Dictionary<int, List<GuildDungeonArtifactTemplet>> dictionary = new Dictionary<int, List<GuildDungeonArtifactTemplet>>();
		for (int i = 0; i < m_lstGuildDungeonArena.Count; i++)
		{
			int arenaIndex = m_lstGuildDungeonArena[i].arenaIndex;
			int currentArtifactCountByArena = GetCurrentArtifactCountByArena(arenaIndex);
			if (currentArtifactCountByArena > 0 && m_dicGuildDungeonInfoTemplet.ContainsKey(arenaIndex))
			{
				List<GuildDungeonArtifactTemplet> dungeonArtifactList = GuildDungeonTempletManager.GetDungeonArtifactList(m_dicGuildDungeonInfoTemplet[arenaIndex].GetStageRewardArtifactGroup());
				dictionary.Add(arenaIndex, dungeonArtifactList.GetRange(0, currentArtifactCountByArena));
			}
		}
		return dictionary;
	}

	public static NKM_ERROR_CODE CanStartBoss()
	{
		if (m_GuildDungeonState != GuildDungeonState.PlayableGuildDungeon)
		{
			return NKM_ERROR_CODE.NEC_FAIL_GUILD_DUNGEON_SESSION_OUT;
		}
		NKMUserData userData = NKCScenManager.CurrentUserData();
		if (m_lstGuildDungeonMemberInfo.Find((GuildDungeonMemberInfo x) => x.profile.userUid == userData.m_UserUID) == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_GUILD_DUNGEON_INVALID_SESSION_DUNGEON_ID;
		}
		if (m_BossData.playCount == 0)
		{
			return NKM_ERROR_CODE.NEC_FAIL_GUILD_DUNGEON_BOSS_PLAYABLE;
		}
		if (m_BossData.playUserUid > 0)
		{
			return NKM_ERROR_CODE.NEC_FAIL_GUILD_DUNGEON_ARENA_PLAYING;
		}
		if (m_BossData.remainHp == 0f)
		{
			return NKM_ERROR_CODE.NEC_FAIL_GUILD_DUNGEON_BOSS_ALL_CLEAR;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE CanStartArena(NKMDungeonTempletBase dungeonTempletBase)
	{
		foreach (KeyValuePair<int, GuildDungeonInfoTemplet> item in m_dicGuildDungeonInfoTemplet)
		{
			if (item.Value.GetSeasonDungeonId() == dungeonTempletBase.Key)
			{
				return CanStartArena(item.Key);
			}
		}
		return NKM_ERROR_CODE.NEC_FAIL_GUILD_DUNGEON_INVALID_SESSION_DUNGEON_ID;
	}

	public static NKM_ERROR_CODE CanStartArena(int arenaIdx)
	{
		if (m_GuildDungeonState != GuildDungeonState.PlayableGuildDungeon)
		{
			return NKM_ERROR_CODE.NEC_FAIL_GUILD_DUNGEON_SESSION_OUT;
		}
		NKMUserData userData = NKCScenManager.CurrentUserData();
		if (m_lstGuildDungeonMemberInfo.Find((GuildDungeonMemberInfo x) => x.profile.userUid == userData.m_UserUID) == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_GUILD_DUNGEON_INVALID_SESSION_DUNGEON_ID;
		}
		if (m_ArenaPlayableCount <= 0 && m_ArenaTicketBuyCount >= NKMCommonConst.GuildDungeonConstTemplet.ArenaTicketBuyCount)
		{
			return NKM_ERROR_CODE.NEC_FAIL_GUILD_DUNGEON_ARENA_OVER_PLAY_COUNT;
		}
		GuildDungeonArena guildDungeonArena = m_lstGuildDungeonArena.Find((GuildDungeonArena x) => x.arenaIndex == arenaIdx);
		if (guildDungeonArena != null)
		{
			if (guildDungeonArena.playUserUid > 0)
			{
				return NKM_ERROR_CODE.NEC_FAIL_GUILD_DUNGEON_ARENA_PLAYING;
			}
			List<GuildDungeonArtifactTemplet> list = GetAllArtifactDictionary()[arenaIdx];
			if (guildDungeonArena.totalMedalCount >= list.Count * NKMCommonConst.GuildDungeonConstTemplet.ArtifactFulificationCount)
			{
				return NKM_ERROR_CODE.NEC_FAIL_GUILD_DUNGEON_ARENA_MAX_ARTIFACT;
			}
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static void SetArenaPlayStart(int arenaIdx, long userUid)
	{
		if (m_bGuildCoopDataRecved)
		{
			GuildDungeonArena guildDungeonArena = m_lstGuildDungeonArena.Find((GuildDungeonArena x) => x.arenaIndex == arenaIdx);
			if (guildDungeonArena != null)
			{
				guildDungeonArena.playUserUid = userUid;
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_COOP().RefreshArenaSlot(arenaIdx);
			}
		}
	}

	public static void SetArenaPlayEnd(NKMPacket_GUILD_DUNGEON_ARENA_PLAY_END_NOT sPacket)
	{
		if (!m_bGuildCoopDataRecved)
		{
			return;
		}
		GuildDungeonArena guildDungeonArena = m_lstGuildDungeonArena.Find((GuildDungeonArena x) => x.arenaIndex == sPacket.arenaId);
		if (guildDungeonArena == null)
		{
			return;
		}
		if (sPacket.errorCode == NKM_ERROR_CODE.NEC_OK)
		{
			int grade = sPacket.totalGrade - guildDungeonArena.totalMedalCount;
			GuildDungeonMemberInfo guildDungeonMemberInfo = m_lstGuildDungeonMemberInfo.Find((GuildDungeonMemberInfo x) => x.profile.userUid == sPacket.playedUserUid);
			if (guildDungeonMemberInfo != null)
			{
				GuildDungeonMemberArena guildDungeonMemberArena = new GuildDungeonMemberArena();
				guildDungeonMemberArena.arenaId = sPacket.arenaId;
				guildDungeonMemberArena.grade = grade;
				guildDungeonMemberInfo.arenaList.Add(guildDungeonMemberArena);
				if (sPacket.playedUserUid == NKCScenManager.CurrentUserData().m_UserUID)
				{
					m_ArenaPlayableCount--;
					m_TryCount++;
					SetArenaResult(sPacket.arenaId, guildDungeonArena.totalMedalCount, sPacket.totalGrade);
				}
			}
			guildDungeonArena.totalMedalCount = sPacket.totalGrade;
		}
		guildDungeonArena.playUserUid = 0L;
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_COOP().RefreshArenaSlot(sPacket.arenaId);
	}

	public static void SetWaitForArenaResult(bool bValue)
	{
		m_bWaitForArenaResult = bValue;
	}

	private static void SetArenaResult(int arenaID, int lastMedalCount, int curMedalCount)
	{
		if (curMedalCount / NKMCommonConst.GuildDungeonConstTemplet.ArtifactFulificationCount != lastMedalCount / NKMCommonConst.GuildDungeonConstTemplet.ArtifactFulificationCount)
		{
			m_bGetArtifact = true;
		}
		else
		{
			m_bGetArtifact = false;
		}
		m_LastPlayedArenaID = arenaID;
		m_bArenaResultRecved = true;
	}

	public static int GetLastPlayedArenaID()
	{
		return m_LastPlayedArenaID;
	}

	public static void SetArenaPlayCancel(NKMPacket_GUILD_DUNGEON_ARENA_PLAY_CANCEL_NOT sPacket)
	{
		if (m_bGuildCoopDataRecved)
		{
			GuildDungeonArena guildDungeonArena = m_lstGuildDungeonArena.Find((GuildDungeonArena x) => x.arenaIndex == sPacket.arenaIndex);
			if (guildDungeonArena != null)
			{
				guildDungeonArena.playUserUid = 0L;
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_COOP().RefreshArenaSlot(sPacket.arenaIndex);
			}
		}
	}

	public static void SetBossPlayCancel(NKMPacket_GUILD_DUNGEON_BOSS_PLAY_CANCEL_NOT sPacket)
	{
		if (m_bGuildCoopDataRecved)
		{
			m_BossData.playUserUid = 0L;
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_COOP().RefreshBossInfo();
		}
	}

	public static void SetBossPlayStart(long userUid)
	{
		if (m_bGuildCoopDataRecved)
		{
			m_BossData.playUserUid = userUid;
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_COOP().RefreshBossInfo();
		}
	}

	public static void SetBossPlayEnd(NKMPacket_GUILD_DUNGEON_BOSS_PLAY_END_NOT sPacket)
	{
		if (!m_bGuildCoopDataRecved)
		{
			return;
		}
		m_cGuildRaidTemplet.GetStageIndex();
		m_cGuildRaidTemplet = GuildDungeonTempletManager.GetGuildRaidTemplet(m_cGuildRaidTemplet.GetSeasonRaidGrouop(), sPacket.bossStageId);
		if (m_cGuildRaidTemplet != null)
		{
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(sPacket.bossStageId);
			if (dungeonTempletBase != null)
			{
				GuildDungeonMemberInfo guildDungeonMemberInfo = m_lstGuildDungeonMemberInfo.Find((GuildDungeonMemberInfo x) => x.profile.userUid == sPacket.playedUserUid);
				if (guildDungeonMemberInfo != null)
				{
					guildDungeonMemberInfo.bossPoint += sPacket.point;
					if (sPacket.playedUserUid == NKCScenManager.CurrentUserData().m_UserUID)
					{
						m_TryCount++;
						m_BossData.playCount--;
					}
				}
				m_BossData.playUserUid = 0L;
				m_BossData.remainHp = sPacket.remainHp;
				m_BossData.extraPoint = sPacket.extraPoint;
				m_BossMaxHp = NKMDungeonManager.GetBossHp(sPacket.bossStageId, dungeonTempletBase.m_DungeonLevel);
			}
			else
			{
				Log.Error($"NKMDungeonTempletBase is null - bossStageID : {sPacket.bossStageId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCGuildCoopManager.cs", 558);
			}
		}
		else
		{
			Log.Error($"GuildRaidTemplet is null - GroupID : {m_cGuildRaidTemplet.GetSeasonRaidGrouop()}, bossStageID : {sPacket.bossStageId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCGuildCoopManager.cs", 563);
		}
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_COOP().RefreshBossInfo();
	}

	public static void SetArenaFlag(int arenaNum, int flagIndex)
	{
		GuildDungeonArena guildDungeonArena = m_lstGuildDungeonArena.Find((GuildDungeonArena x) => x.arenaIndex == arenaNum);
		if (guildDungeonArena != null)
		{
			guildDungeonArena.flagIndex = flagIndex;
		}
	}

	public static void SetBossOrderIndex(short orderIndex)
	{
		m_BossData.orderIndex = orderIndex;
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_COOP)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_COOP().RefreshBossInfo();
		}
	}

	public static void SetMyData(GuildDungeonRewardInfo myData)
	{
		Initialize();
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.GUILD_DUNGEON))
		{
			return;
		}
		m_SeasonId = myData.currentSeasonId;
		if (m_SeasonId < 0)
		{
			GuildSeasonTemplet firstSeasonTemplet = GetFirstSeasonTemplet();
			if (firstSeasonTemplet == null)
			{
				return;
			}
			m_SeasonId = firstSeasonTemplet.Key;
			m_NextSessionStartDateUTC = NKMTime.LocalToUTC(firstSeasonTemplet.GetSeasonStartDate());
		}
		m_KillPoint = myData.lastSeasonRewardData.Find((GuildDungeonSeasonRewardData x) => x.category == GuildDungeonRewardCategory.RANK).totalValue;
		m_TryCount = myData.lastSeasonRewardData.Find((GuildDungeonSeasonRewardData x) => x.category == GuildDungeonRewardCategory.DUNGEON_TRY).totalValue;
		m_LastReceivedSeasonRewardData = myData.lastSeasonRewardData;
		m_bCanReward = myData.canReward;
	}

	public static GuildSeasonTemplet GetFirstSeasonTemplet()
	{
		if (NKMContentsVersionManager.HasDFChangeTagType(DataFormatChangeTagType.OPEN_TAG_GUILD_DUNGEON))
		{
			return (from e in GuildSeasonTemplet.Values
				where e.EnableByTag
				orderby e.GetSeasonStartDate()
				select e).FirstOrDefault();
		}
		return GuildSeasonTemplet.Values.OrderBy((GuildSeasonTemplet e) => e.GetSeasonStartDate()).FirstOrDefault();
	}

	public static bool HasNextSessionData(DateTime nextSessionStartTime)
	{
		if (m_bGuildCoopDataRecved && nextSessionStartTime <= DateTime.MinValue)
		{
			return false;
		}
		if (NKMContentsVersionManager.HasDFChangeTagType(DataFormatChangeTagType.OPEN_TAG_GUILD_DUNGEON))
		{
			return (from e in GuildSeasonTemplet.Values
				where e.EnableByTag && e.GetSeasonEndDate() > nextSessionStartTime
				orderby e.GetSeasonEndDate()
				select e).FirstOrDefault() != null;
		}
		GuildSeasonTemplet.Values.Where((GuildSeasonTemplet e) => e.GetSeasonEndDate() > nextSessionStartTime);
		return GuildSeasonTemplet.Values.OrderBy((GuildSeasonTemplet e) => e.GetSeasonEndDate()).FirstOrDefault() != null;
	}

	public static bool CheckFirstSeasonStarted()
	{
		if (GetFirstSeasonTemplet() == null)
		{
			return false;
		}
		if (!NKCSynchronizedTime.IsFinished(NKMTime.LocalToUTC(GetFirstSeasonTemplet().GetSeasonStartDate())))
		{
			return false;
		}
		return true;
	}

	public static void SetArenaTicketBuyCount(int count)
	{
		m_ArenaPlayableCount = m_ArenaPlayableCount - m_ArenaTicketBuyCount + count;
		m_ArenaTicketBuyCount = count;
	}

	public static void SetArenaPlayableCount(int count)
	{
		if (count < 0)
		{
			count = 0;
		}
		m_ArenaPlayableCount = count;
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_COOP)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_COOP().Refresh();
		}
	}

	public static void SetBossPlayableCount(int count)
	{
		m_BossData.playCount = count;
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_COOP)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_COOP().Refresh();
		}
	}

	public static void OnRecv(NKMPacket_GUILD_DUNGEON_INFO_ACK sPacket)
	{
		if (sPacket.guildDungeonState == GuildDungeonState.Invalid)
		{
			m_GuildDungeonState = sPacket.guildDungeonState;
			m_NextSessionStartDateUTC = NKMTime.LocalToUTC(sPacket.NextSessionStartDate);
			m_bGuildCoopDataRecved = true;
			return;
		}
		ResetGuildCoopSessionData();
		m_GuildDungeonState = sPacket.guildDungeonState;
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(sPacket.bossData.stageId);
		if (dungeonTempletBase == null)
		{
			Log.Error($"NKMDungeonTempletBase is null - id : {sPacket.bossData.stageId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCGuildCoopManager.cs", 719);
			return;
		}
		m_SeasonId = sPacket.seasonId;
		m_SessionId = sPacket.sessionId;
		m_lstGuildDungeonArena = sPacket.arenaList;
		m_BossMaxHp = NKMDungeonManager.GetBossHp(sPacket.bossData.stageId, dungeonTempletBase.m_DungeonLevel);
		m_BossData = sPacket.bossData;
		m_KillPoint = sPacket.lastSeasonRewardData.Find((GuildDungeonSeasonRewardData e) => e.category == GuildDungeonRewardCategory.RANK).totalValue;
		m_TryCount = sPacket.lastSeasonRewardData.Find((GuildDungeonSeasonRewardData e) => e.category == GuildDungeonRewardCategory.DUNGEON_TRY).totalValue;
		m_SessionEndDateUTC = NKMTime.LocalToUTC(sPacket.currentSessionEndDate);
		m_NextSessionStartDateUTC = NKMTime.LocalToUTC(sPacket.NextSessionStartDate);
		m_LastReceivedSeasonRewardData = sPacket.lastSeasonRewardData;
		m_bCanReward = sPacket.canReward;
		m_ArenaTicketBuyCount = sPacket.arenaTicketBuyCount;
		m_dicGuildDungeonInfoTemplet.Clear();
		GuildSeasonTemplet guildSeasonTemplet = GuildDungeonTempletManager.GetGuildSeasonTemplet(m_SeasonId);
		if (guildSeasonTemplet != null)
		{
			GuildDungeonScheduleTemplet guildDungeonScheduleTemplet = GuildDungeonTempletManager.GetDungeonScheduleList(guildSeasonTemplet.GetSeasonDungeonGroup()).Find((GuildDungeonScheduleTemplet x) => x.GetSessionIndex() == m_SessionId);
			if (guildDungeonScheduleTemplet != null)
			{
				List<int> lstDungeonId = guildDungeonScheduleTemplet.GetDungeonList();
				int i;
				for (i = 0; i < lstDungeonId.Count; i++)
				{
					GuildDungeonInfoTemplet guildDungeonInfoTemplet = GuildDungeonTempletManager.GetDungeonInfoList(m_SeasonId).Find((GuildDungeonInfoTemplet x) => x.GetSeasonDungeonId() == lstDungeonId[i]);
					m_dicGuildDungeonInfoTemplet.Add(guildDungeonInfoTemplet.GetArenaIndex(), guildDungeonInfoTemplet);
				}
			}
			m_cGuildRaidTemplet = GuildDungeonTempletManager.GetGuildRaidTemplet(guildSeasonTemplet.GetSeasonRaidGroup(), sPacket.bossData.stageId);
		}
		m_bGuildCoopDataRecved = true;
	}

	public static void OnRecv(NKMPacket_GUILD_DUNGEON_MEMBER_INFO_ACK sPacket)
	{
		if (sPacket.memberInfoList != null)
		{
			m_lstGuildDungeonMemberInfo = sPacket.memberInfoList;
		}
		GuildDungeonMemberInfo guildDungeonMemberInfo = m_lstGuildDungeonMemberInfo.Find((GuildDungeonMemberInfo x) => x.profile.userUid == NKCScenManager.CurrentUserData().m_UserUID);
		int arenaPlayableCount = 0;
		if (guildDungeonMemberInfo != null)
		{
			arenaPlayableCount = Math.Max(0, NKMCommonConst.GuildDungeonConstTemplet.ArenaPlayCountBasic - guildDungeonMemberInfo.arenaList.Count + m_ArenaTicketBuyCount);
		}
		SetArenaPlayableCount(arenaPlayableCount);
		m_bGuildCoopMemberDataRecved = true;
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_RAID_READY)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID_READY().OnRecv(sPacket);
		}
	}

	public static void OnRecv(NKMPacket_GUILD_DUNGEON_SEASON_REWARD_ACK sPacket)
	{
		GuildDungeonSeasonRewardData guildDungeonSeasonRewardData = m_LastReceivedSeasonRewardData.Find((GuildDungeonSeasonRewardData x) => x.category == sPacket.rewardCategory);
		if (guildDungeonSeasonRewardData != null)
		{
			guildDungeonSeasonRewardData.receivedValue = sPacket.rewardCountValue;
		}
		else
		{
			Log.Error($"m_LastReceivedSeasonRewardData  - {sPacket.rewardCategory} 에 해당하는 값이 없음", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCGuildCoopManager.cs", 788);
		}
	}

	public static int CompMember(GuildDungeonMemberInfo left, GuildDungeonMemberInfo right)
	{
		if (left.bossPoint == right.bossPoint)
		{
			NKMGuildMemberData nKMGuildMemberData = NKCGuildManager.MyGuildData.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == left.profile.userUid);
			NKMGuildMemberData nKMGuildMemberData2 = NKCGuildManager.MyGuildData.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == right.profile.userUid);
			if (nKMGuildMemberData.grade == nKMGuildMemberData2.grade)
			{
				return nKMGuildMemberData.commonProfile.nickname.CompareTo(nKMGuildMemberData2.commonProfile.nickname);
			}
			return nKMGuildMemberData.grade.CompareTo(nKMGuildMemberData2.grade);
		}
		return right.bossPoint.CompareTo(left.bossPoint);
	}

	public static void SetRewarded()
	{
		m_bCanReward = false;
	}

	public static bool IsExtraBoss(int idx)
	{
		return 15 == idx;
	}
}
