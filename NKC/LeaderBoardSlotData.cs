using System;
using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.LeaderBoard;
using ClientPacket.Raid;
using NKM.Templet;

namespace NKC;

public class LeaderBoardSlotData
{
	public LeaderBoardType boardType = LeaderBoardType.BT_NONE;

	public NKMCommonProfile Profile;

	public NKMGuildSimpleData GuildData;

	public string score = "";

	public int memberCount;

	public bool bIsGuild;

	public int rank;

	public int raidTryCount;

	public int raidTryMaxCount;

	public NKMTournamentCountryCode CountryCode;

	public long userUid
	{
		get
		{
			if (Profile != null)
			{
				return Profile.userUid;
			}
			return 0L;
		}
	}

	public long friendCode
	{
		get
		{
			if (Profile != null)
			{
				return Profile.friendCode;
			}
			return 0L;
		}
	}

	public string nickname
	{
		get
		{
			if (Profile != null)
			{
				return Profile.nickname;
			}
			return "";
		}
	}

	public int level
	{
		get
		{
			if (Profile != null)
			{
				return Profile.level;
			}
			return 1;
		}
	}

	public int mainUnitId
	{
		get
		{
			if (Profile != null)
			{
				return Profile.mainUnitId;
			}
			return 0;
		}
	}

	public int mainUnitSkinId
	{
		get
		{
			if (Profile != null)
			{
				return Profile.mainUnitSkinId;
			}
			return 0;
		}
	}

	public int frameID
	{
		get
		{
			if (Profile != null)
			{
				return Profile.frameId;
			}
			return 0;
		}
	}

	public static LeaderBoardSlotData MakeMySlotData(LeaderBoardType type, int rank, string score, bool bIsGuild)
	{
		LeaderBoardSlotData leaderBoardSlotData = new LeaderBoardSlotData();
		leaderBoardSlotData.bIsGuild = bIsGuild;
		leaderBoardSlotData.boardType = type;
		leaderBoardSlotData.Profile = new NKMCommonProfile();
		if (NKCScenManager.CurrentUserData().UserProfileData != null)
		{
			leaderBoardSlotData.Profile = NKCScenManager.CurrentUserData().UserProfileData.commonProfile;
		}
		else
		{
			leaderBoardSlotData.Profile.level = NKCScenManager.CurrentUserData().m_UserLevel;
			leaderBoardSlotData.Profile.nickname = NKCScenManager.CurrentUserData().m_UserNickName;
			leaderBoardSlotData.Profile.userUid = NKCScenManager.CurrentUserData().m_UserUID;
			leaderBoardSlotData.Profile.friendCode = NKCScenManager.CurrentUserData().m_FriendCode;
		}
		leaderBoardSlotData.GuildData = NKCGuildManager.GetMyGuildSimpleData();
		leaderBoardSlotData.score = score;
		leaderBoardSlotData.rank = rank;
		return leaderBoardSlotData;
	}

	public static LeaderBoardSlotData MakeSlotData(LeaderBoardType type, bool bIsGuild, int rank)
	{
		return new LeaderBoardSlotData
		{
			bIsGuild = bIsGuild,
			boardType = type,
			Profile = new NKMCommonProfile(),
			GuildData = new NKMGuildSimpleData(),
			score = GetScoreByBoardType(type, 0L),
			rank = rank
		};
	}

	public static LeaderBoardSlotData MakeSlotData(NKMAchieveData achieveData, int rank)
	{
		return new LeaderBoardSlotData
		{
			bIsGuild = false,
			boardType = LeaderBoardType.BT_ACHIEVE,
			Profile = achieveData.commonProfile,
			GuildData = achieveData.guildData,
			score = GetScoreByBoardType(LeaderBoardType.BT_ACHIEVE, achieveData.achievePoint),
			rank = rank
		};
	}

	public static LeaderBoardSlotData MakeSlotData(NKMShadowPalaceData shadowPalaceData, int rank)
	{
		return new LeaderBoardSlotData
		{
			bIsGuild = false,
			boardType = LeaderBoardType.BT_SHADOW,
			Profile = shadowPalaceData.commonProfile,
			GuildData = shadowPalaceData.guildData,
			score = GetScoreByBoardType(LeaderBoardType.BT_SHADOW, shadowPalaceData.bestTime),
			rank = rank
		};
	}

	public static LeaderBoardSlotData MakeSlotData(NKMFierceData cNKMFierceData, int rank)
	{
		return new LeaderBoardSlotData
		{
			bIsGuild = false,
			boardType = LeaderBoardType.BT_FIERCE,
			Profile = cNKMFierceData.commonProfile,
			GuildData = cNKMFierceData.guildData,
			score = GetScoreByBoardType(LeaderBoardType.BT_FIERCE, cNKMFierceData.fiercePoint),
			rank = rank
		};
	}

	public static LeaderBoardSlotData MakeSlotData(NKMTimeAttackData timeAttackData, int rank)
	{
		return new LeaderBoardSlotData
		{
			bIsGuild = false,
			boardType = LeaderBoardType.BT_TIMEATTACK,
			Profile = timeAttackData.commonProfile,
			GuildData = timeAttackData.guildData,
			score = GetScoreByBoardType(LeaderBoardType.BT_TIMEATTACK, timeAttackData.bestTime),
			rank = rank
		};
	}

	public static LeaderBoardSlotData MakeSlotData(NKMDefenceRankData defenceData, int rank)
	{
		LeaderBoardSlotData leaderBoardSlotData = new LeaderBoardSlotData();
		if (defenceData != null)
		{
			leaderBoardSlotData.bIsGuild = false;
			leaderBoardSlotData.boardType = LeaderBoardType.BT_DEFENCE;
			leaderBoardSlotData.Profile = defenceData.commonProfile;
			leaderBoardSlotData.GuildData = defenceData.guildData;
			leaderBoardSlotData.score = GetScoreByBoardType(LeaderBoardType.BT_DEFENCE, defenceData.bestScore);
			leaderBoardSlotData.rank = rank;
		}
		return leaderBoardSlotData;
	}

	public static List<LeaderBoardSlotData> MakeSlotDataList(NKMLeaderBoardFierceData cNKMLeaderBoardFierceData)
	{
		List<LeaderBoardSlotData> list = new List<LeaderBoardSlotData>();
		for (int i = 0; i < cNKMLeaderBoardFierceData.fierceData.Count; i++)
		{
			LeaderBoardSlotData item = MakeSlotData(cNKMLeaderBoardFierceData.fierceData[i], i + 1);
			list.Add(item);
		}
		return list;
	}

	public static LeaderBoardSlotData MakeSlotData(NKMGuildRankData rankData, int rank)
	{
		LeaderBoardSlotData leaderBoardSlotData = new LeaderBoardSlotData();
		leaderBoardSlotData.bIsGuild = true;
		leaderBoardSlotData.boardType = LeaderBoardType.BT_GUILD;
		leaderBoardSlotData.GuildData = new NKMGuildSimpleData();
		leaderBoardSlotData.GuildData.badgeId = rankData.badgeId;
		leaderBoardSlotData.GuildData.guildName = rankData.guildName;
		leaderBoardSlotData.GuildData.guildUid = rankData.guildUid;
		leaderBoardSlotData.Profile = new NKMCommonProfile();
		leaderBoardSlotData.Profile.level = rankData.guildLevel;
		leaderBoardSlotData.Profile.nickname = rankData.masterNickname;
		leaderBoardSlotData.score = GetScoreByBoardType(LeaderBoardType.BT_GUILD, rankData.rankValue);
		leaderBoardSlotData.memberCount = rankData.memberCount;
		leaderBoardSlotData.rank = rank;
		return leaderBoardSlotData;
	}

	public static List<LeaderBoardSlotData> MakeSlotDataList(NKMLeaderBoardGuildData cNKMLeaderBoardGuildData)
	{
		List<LeaderBoardSlotData> list = new List<LeaderBoardSlotData>();
		for (int i = 0; i < cNKMLeaderBoardGuildData.rankDatas.Count; i++)
		{
			LeaderBoardSlotData item = MakeSlotData(cNKMLeaderBoardGuildData.rankDatas[i], i + 1);
			list.Add(item);
		}
		return list;
	}

	public static LeaderBoardSlotData MakeSlotData(LeaderBoardType boardType, NKMUserProfileData cNKMUserProfileData, int rank, int seasonId = 0)
	{
		LeaderBoardSlotData leaderBoardSlotData = new LeaderBoardSlotData();
		leaderBoardSlotData.bIsGuild = false;
		leaderBoardSlotData.boardType = boardType;
		leaderBoardSlotData.GuildData = cNKMUserProfileData.guildData;
		leaderBoardSlotData.Profile = new NKMCommonProfile();
		if (cNKMUserProfileData != null && cNKMUserProfileData.commonProfile != null)
		{
			leaderBoardSlotData.Profile = cNKMUserProfileData.commonProfile;
		}
		leaderBoardSlotData.score = GetScoreByBoardType(boardType, cNKMUserProfileData.leaguePvpData.score);
		leaderBoardSlotData.memberCount = seasonId;
		leaderBoardSlotData.rank = rank;
		return leaderBoardSlotData;
	}

	public static LeaderBoardSlotData MakeSlotData(LeaderBoardType boardType, NKMTournamentProfileData profile, int rank, int tournamentId)
	{
		LeaderBoardSlotData leaderBoardSlotData = new LeaderBoardSlotData();
		leaderBoardSlotData.bIsGuild = false;
		leaderBoardSlotData.boardType = boardType;
		leaderBoardSlotData.GuildData = new NKMGuildSimpleData();
		leaderBoardSlotData.Profile = new NKMCommonProfile();
		if (profile != null)
		{
			leaderBoardSlotData.GuildData = profile.guildData;
			leaderBoardSlotData.Profile.level = profile.commonProfile.level;
			leaderBoardSlotData.Profile.nickname = profile.commonProfile.nickname;
			leaderBoardSlotData.Profile.mainUnitId = profile.commonProfile.mainUnitId;
			leaderBoardSlotData.Profile.mainUnitSkinId = profile.commonProfile.mainUnitSkinId;
			leaderBoardSlotData.Profile.frameId = profile.commonProfile.frameId;
			leaderBoardSlotData.Profile.mainUnitTacticLevel = profile.commonProfile.mainUnitTacticLevel;
			leaderBoardSlotData.Profile.titleId = profile.commonProfile.titleId;
			leaderBoardSlotData.Profile.friendCode = profile.commonProfile.friendCode;
			leaderBoardSlotData.Profile.userUid = profile.commonProfile.userUid;
		}
		leaderBoardSlotData.score = "";
		leaderBoardSlotData.memberCount = tournamentId;
		leaderBoardSlotData.rank = rank;
		leaderBoardSlotData.CountryCode = profile.countryCode;
		return leaderBoardSlotData;
	}

	public static LeaderBoardSlotData MakeSlotData(LeaderBoardType boardType, NKMUserSimpleProfileData profile, int rank)
	{
		LeaderBoardSlotData leaderBoardSlotData = new LeaderBoardSlotData();
		leaderBoardSlotData.bIsGuild = false;
		leaderBoardSlotData.boardType = boardType;
		leaderBoardSlotData.GuildData = new NKMGuildSimpleData();
		leaderBoardSlotData.Profile = new NKMCommonProfile();
		if (profile != null)
		{
			leaderBoardSlotData.GuildData = profile.guildData;
			leaderBoardSlotData.Profile.level = profile.level;
			leaderBoardSlotData.Profile.nickname = profile.nickname;
			leaderBoardSlotData.Profile.mainUnitId = profile.mainUnitId;
			leaderBoardSlotData.Profile.mainUnitSkinId = profile.mainUnitSkinId;
			leaderBoardSlotData.Profile.frameId = profile.frameId;
			leaderBoardSlotData.Profile.mainUnitTacticLevel = profile.mainUnitTacticLevel;
			leaderBoardSlotData.Profile.titleId = profile.titleId;
			leaderBoardSlotData.Profile.friendCode = profile.friendCode;
			leaderBoardSlotData.Profile.userUid = profile.userUid;
		}
		leaderBoardSlotData.score = profile.pvpScore.ToString();
		leaderBoardSlotData.memberCount = profile.pvpTier;
		leaderBoardSlotData.rank = rank;
		return leaderBoardSlotData;
	}

	public static List<LeaderBoardSlotData> MakeSlotDataList(LeaderBoardType boardType, List<NKMUserProfileData> lstData)
	{
		List<LeaderBoardSlotData> list = new List<LeaderBoardSlotData>();
		for (int i = 0; i < lstData.Count; i++)
		{
			LeaderBoardSlotData item = MakeSlotData(boardType, lstData[i], i + 1);
			list.Add(item);
		}
		return list;
	}

	public static LeaderBoardSlotData MakeSlotData(NKMRaidJoinData raidJoinData, int rank, int raidMaxCount)
	{
		LeaderBoardSlotData leaderBoardSlotData = new LeaderBoardSlotData();
		leaderBoardSlotData.bIsGuild = false;
		leaderBoardSlotData.boardType = LeaderBoardType.BT_ACHIEVE;
		leaderBoardSlotData.GuildData = raidJoinData.guildData;
		leaderBoardSlotData.Profile = new NKMCommonProfile();
		leaderBoardSlotData.Profile.userUid = raidJoinData.userUID;
		leaderBoardSlotData.Profile.level = raidJoinData.level;
		leaderBoardSlotData.Profile.nickname = raidJoinData.nickName;
		leaderBoardSlotData.Profile.mainUnitId = raidJoinData.mainUnitID;
		leaderBoardSlotData.Profile.mainUnitSkinId = raidJoinData.mainUnitSkinID;
		leaderBoardSlotData.Profile.friendCode = raidJoinData.friendCode;
		leaderBoardSlotData.score = raidJoinData.damage.ToString("N0");
		leaderBoardSlotData.memberCount = raidJoinData.tryCount;
		leaderBoardSlotData.raidTryCount = raidJoinData.tryCount;
		leaderBoardSlotData.raidTryMaxCount = raidMaxCount;
		leaderBoardSlotData.rank = rank;
		return leaderBoardSlotData;
	}

	public static List<LeaderBoardSlotData> MakeSlotDataList(List<NKMRaidJoinData> lstData, int maxCount)
	{
		List<LeaderBoardSlotData> list = new List<LeaderBoardSlotData>();
		lstData.Sort(CompByDamage);
		for (int i = 0; i < lstData.Count; i++)
		{
			LeaderBoardSlotData item = MakeSlotData(lstData[i], i + 1, maxCount);
			list.Add(item);
		}
		return list;
	}

	private static int CompByDamage(NKMRaidJoinData lData, NKMRaidJoinData rData)
	{
		return rData.damage.CompareTo(lData.damage);
	}

	public static string GetScoreByBoardType(LeaderBoardType type, long score)
	{
		switch (type)
		{
		default:
			if (score > 0)
			{
				return score.ToString("N0");
			}
			return "-";
		case LeaderBoardType.BT_SHADOW:
		case LeaderBoardType.BT_TIMEATTACK:
			if (score > 0)
			{
				return NKCUtilString.GetTimeSpanString(TimeSpan.FromSeconds(score));
			}
			return "-:--:--";
		}
	}
}
