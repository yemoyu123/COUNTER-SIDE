using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Common;
using ClientPacket.Guild;
using Cs.Core.Util;
using Cs.Shared.Time;
using NKC.UI;
using NKC.UI.Guild;
using NKM;
using NKM.Guild;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKC;

public class NKCGuildManager
{
	public const int MaxGuildNoticeLength = 36;

	public const int MaxGuildGreetingLength = 40;

	public const int MaxUserGreetingLength = 13;

	private static List<NKMGuildBadgeFrameTemplet> m_lstFrameTemplet = new List<NKMGuildBadgeFrameTemplet>();

	private static List<NKMGuildBadgeMarkTemplet> m_lstMarkTemplet = new List<NKMGuildBadgeMarkTemplet>();

	private static List<NKMGuildBadgeColorTemplet> m_lstBadgeColorTemplet = new List<NKMGuildBadgeColorTemplet>();

	public static List<GuildListData> m_lstSearchData = new List<GuildListData>();

	public static List<GuildListData> m_lstRequestedData = new List<GuildListData>();

	public static List<GuildListData> m_lstInvitedData = new List<GuildListData>();

	private static GuildListType m_GuildListType;

	public static bool m_bShowChatNotice = true;

	private static string m_lastJoinRequestedGuildName = "";

	private static long m_lastJoinRequestedGuildUid = 0L;

	public static PrivateGuildData MyData { get; private set; }

	public static DateTime LastGuildNoticeChangedTimeUTC { get; private set; }

	public static DateTime LastDungeonNoticeChangedTimeUTC { get; private set; }

	public static NKMGuildData MyGuildData { get; private set; }

	public static List<NKMGuildMemberData> MyGuildMemberDataList
	{
		get
		{
			if (MyGuildData == null)
			{
				return new List<NKMGuildMemberData>();
			}
			return MyGuildData.members.Where((NKMGuildMemberData e) => e.commonProfile.userUid != NKCScenManager.CurrentUserData().m_UserUID).ToList();
		}
	}

	public static void Initialize()
	{
		m_lstFrameTemplet = NKMTempletContainer<NKMGuildBadgeFrameTemplet>.Values.ToList();
		m_lstMarkTemplet = NKMTempletContainer<NKMGuildBadgeMarkTemplet>.Values.ToList();
		m_lstBadgeColorTemplet = NKMTempletContainer<NKMGuildBadgeColorTemplet>.Values.ToList();
		m_lstSearchData = new List<GuildListData>();
		m_lstRequestedData = new List<GuildListData>();
		m_lstInvitedData = new List<GuildListData>();
		MyData = new PrivateGuildData();
		MyGuildData = null;
		m_bShowChatNotice = true;
	}

	public static void SetMyData(PrivateGuildData data)
	{
		MyData = data;
	}

	public static void SetGuildJoinDisableTime(DateTime serviceTime)
	{
		MyData.guildJoinDisableTime = serviceTime;
	}

	public static void SetMyGuildData(NKMGuildData data)
	{
		if (MyGuildData != null && data != null && !string.Equals(MyGuildData.notice, data.notice))
		{
			ShowChatNotice(bValue: true);
		}
		MyGuildData = data;
		if (data != null && data.guildUid > 0 && MyData.guildUid == 0L)
		{
			SetMyDataFromMyGuildData(data);
		}
		if (MyData.guildUid == 0L && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_LOBBY)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_CONFORTIUM_FAIL_GUILD_NOT_BELONG_AT_PRESENT_POPUP_TEXT, delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GUILD_INTRO);
			});
		}
		NKCUIManager.OnGuildDataChanged();
	}

	private static void SetMyDataFromMyGuildData(NKMGuildData data)
	{
		MyData.guildUid = data.guildUid;
	}

	public static void ShowChatNotice(bool bValue)
	{
		m_bShowChatNotice = bValue;
	}

	public static bool HasGuild()
	{
		if (NKCContentManager.IsContentsUnlocked(ContentsType.GUILD) && MyData.guildUid > 0)
		{
			return MyGuildData != null;
		}
		return false;
	}

	public static bool IsGuildMember(long friendCode)
	{
		if (!HasGuild())
		{
			return false;
		}
		return MyGuildData.members.Find((NKMGuildMemberData x) => x.commonProfile.friendCode == friendCode) != null;
	}

	public static bool IsGuildMemberByUID(long userUid)
	{
		if (!HasGuild())
		{
			return false;
		}
		return MyGuildData.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == userUid) != null;
	}

	public static bool IsFirstDay()
	{
		NKMGuildMemberData nKMGuildMemberData = MyGuildData.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == NKCScenManager.CurrentUserData().m_UserUID);
		return !DailyReset.IsOutOfDate(ServiceTime.Recent, nKMGuildMemberData.createdAt);
	}

	public static void RemoveSearchData(long guildUid)
	{
		GuildListData guildListData = m_lstSearchData.Find((GuildListData x) => x.guildUid == guildUid);
		if (guildListData != null)
		{
			m_lstSearchData.Remove(guildListData);
		}
	}

	public static void RemoveRequestedData(long guildUid)
	{
		GuildListData guildListData = m_lstRequestedData.Find((GuildListData x) => x.guildUid == guildUid);
		if (guildListData != null)
		{
			m_lstRequestedData.Remove(guildListData);
		}
	}

	public static void RemoveInvitedData(long guildUid)
	{
		GuildListData guildListData = m_lstInvitedData.Find((GuildListData x) => x.guildUid == guildUid);
		if (guildListData != null)
		{
			m_lstInvitedData.Remove(guildListData);
		}
	}

	public static void RemoveLastJoinRequestedGuildData()
	{
		RemoveSearchData(m_lastJoinRequestedGuildUid);
		m_lastJoinRequestedGuildUid = 0L;
	}

	public static bool AlreadyRequested(long guildUid)
	{
		if (m_lstRequestedData.Find((GuildListData x) => x.guildUid == guildUid) == null)
		{
			return false;
		}
		return true;
	}

	public static bool AlreadyInvited(long guildUid)
	{
		if (m_lstInvitedData.Find((GuildListData x) => x.guildUid == guildUid) == null)
		{
			return false;
		}
		return true;
	}

	public static NKMGuildBadgeFrameTemplet GetFrameTempletByIndex(int idx)
	{
		if (m_lstFrameTemplet.Count > idx)
		{
			return m_lstFrameTemplet[idx];
		}
		return null;
	}

	public static NKMGuildBadgeMarkTemplet GetMarkTempletByIndex(int idx)
	{
		if (m_lstMarkTemplet.Count > idx)
		{
			return m_lstMarkTemplet[idx];
		}
		return null;
	}

	public static NKMGuildBadgeColorTemplet GetBadgeColorTempletByIndex(int idx)
	{
		if (m_lstBadgeColorTemplet.Count > idx)
		{
			return m_lstBadgeColorTemplet[idx];
		}
		return null;
	}

	public static void Send_GUILD_LIST_REQ(GuildListType guildListType)
	{
		m_GuildListType = guildListType;
		NKCPacketSender.Send_NKMPacket_GUILD_LIST_REQ(guildListType);
	}

	public static void ChangeGuildMemberData(NKMCommonProfile commonProfile, DateTime lastOnlineTime)
	{
		if (MyGuildData == null)
		{
			return;
		}
		bool flag = false;
		NKMGuildMemberData nKMGuildMemberData = MyGuildData.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == commonProfile.userUid);
		if (nKMGuildMemberData != null)
		{
			nKMGuildMemberData.commonProfile = commonProfile;
			nKMGuildMemberData.lastOnlineTime = lastOnlineTime;
			flag = true;
		}
		if (!flag)
		{
			FriendListData friendListData = MyGuildData.joinWaitingList.Find((FriendListData x) => x.commonProfile.userUid == commonProfile.userUid);
			if (friendListData != null)
			{
				friendListData.commonProfile = commonProfile;
				friendListData.lastLoginDate = lastOnlineTime;
				flag = true;
			}
			FriendListData friendListData2 = MyGuildData.inviteList.Find((FriendListData x) => x.commonProfile.userUid == commonProfile.userUid);
			if (friendListData2 != null)
			{
				friendListData2.commonProfile = commonProfile;
				friendListData2.lastLoginDate = lastOnlineTime;
				flag = true;
			}
		}
		if (flag)
		{
			NKCUIManager.OnGuildDataChanged();
		}
	}

	public static void OnRecv(NKMPacket_GUILD_CREATE_ACK cNKMPacket_GUILD_CREATE_ACK)
	{
		if (cNKMPacket_GUILD_CREATE_ACK.guildData != null)
		{
			if (NKCUIGuildCreate.IsInstanceOpen)
			{
				NKCUIGuildCreate.Instance.Close();
			}
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GUILD_LOBBY);
		}
	}

	public static void OnRecv(NKMPacket_GUILD_SEARCH_ACK cNKMPacket_GUILD_SEARCH_ACK)
	{
		m_lstSearchData = cNKMPacket_GUILD_SEARCH_ACK.list;
		if (NKCUIGuildJoin.IsInstanceOpen)
		{
			NKCUIGuildJoin.Instance.RefreshUI();
		}
	}

	public static void OnRecv(NKMPacket_GUILD_LIST_ACK cNKMPacket_GUILD_LIST_ACK)
	{
		switch (m_GuildListType)
		{
		case GuildListType.ReceiveInvite:
			m_lstInvitedData = cNKMPacket_GUILD_LIST_ACK.list;
			break;
		case GuildListType.SendRequest:
			m_lstRequestedData = cNKMPacket_GUILD_LIST_ACK.list;
			break;
		}
		if (NKCUIGuildJoin.IsInstanceOpen)
		{
			NKCUIGuildJoin.Instance.RefreshUI();
		}
	}

	public static void Send_GUILD_JOIN_REQ(long guildUid, string guildName, GuildJoinType joinType)
	{
		m_lastJoinRequestedGuildName = guildName;
		m_lastJoinRequestedGuildUid = guildUid;
		NKCPacketSender.Send_NKMPacket_GUILD_JOIN_REQ(guildUid, joinType);
	}

	public static void OnRecv(NKMPacket_GUILD_JOIN_ACK cNKMPacket_GUILD_JOIN_ACK)
	{
		if (cNKMPacket_GUILD_JOIN_ACK.needApproval)
		{
			RemoveSearchData(cNKMPacket_GUILD_JOIN_ACK.guildUid);
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_CONSORTIUM_JOIN_CONFIRM_JOIN_SUCCESS_POPUP_TITLE_DESC, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_JOIN_CONFIRM_JOIN_SUCCESS_POPUP_BODY_DESC, m_lastJoinRequestedGuildName));
			Send_GUILD_LIST_REQ(GuildListType.SendRequest);
		}
		m_lastJoinRequestedGuildUid = 0L;
		m_lastJoinRequestedGuildName = string.Empty;
	}

	public static void OnRecv(NKMPacket_GUILD_CANCEL_JOIN_ACK cNKMPacket_GUILD_CANCEL_JOIN_ACK)
	{
		GuildListData guildListData = m_lstRequestedData.Find((GuildListData x) => x.guildUid == cNKMPacket_GUILD_CANCEL_JOIN_ACK.guildUid);
		if (guildListData != null)
		{
			m_lstRequestedData.Remove(guildListData);
		}
		if (NKCUIGuildJoin.IsInstanceOpen)
		{
			NKCUIGuildJoin.Instance.RefreshUI();
		}
	}

	public static bool CheckNameLength(string nickName, int minByte, int maxByte)
	{
		if (string.IsNullOrEmpty(nickName))
		{
			return false;
		}
		int nickNameLength = NKM_USER_COMMON.GetNickNameLength(nickName);
		if (nickNameLength < minByte || nickNameLength > maxByte)
		{
			return false;
		}
		return true;
	}

	public static void SetLastGuildNoticeChangedTimeUTC(DateTime changedTimeUTC)
	{
		LastGuildNoticeChangedTimeUTC = changedTimeUTC;
	}

	public static void SetLastDungeonNoticeChangedTimeUTC(DateTime changedTimeUTC)
	{
		LastDungeonNoticeChangedTimeUTC = changedTimeUTC;
	}

	public static int GetRemainDonationCount()
	{
		if (IsFirstDay())
		{
			return 0;
		}
		if (DailyReset.CalcNextReset(MyData.lastDailyResetDate) > ServiceTime.Recent)
		{
			return NKMCommonConst.Guild.DailyDonationCount - MyData.donationCount;
		}
		return NKMCommonConst.Guild.DailyDonationCount;
	}

	public static NKMGuildSimpleData GetMyGuildSimpleData()
	{
		NKMGuildSimpleData nKMGuildSimpleData = new NKMGuildSimpleData();
		if (MyGuildData == null || MyGuildData.guildUid <= 0)
		{
			return nKMGuildSimpleData;
		}
		nKMGuildSimpleData.badgeId = MyGuildData.badgeId;
		nKMGuildSimpleData.guildUid = MyGuildData.guildUid;
		nKMGuildSimpleData.guildName = MyGuildData.name;
		return nKMGuildSimpleData;
	}

	public static int GetMaxGuildMemberCount(int guildLevel)
	{
		return NKMTempletContainer<GuildExpTemplet>.Find(guildLevel)?.MaxMemberCount ?? 0;
	}

	public static GuildMemberGrade GetMyGuildGrade()
	{
		if (MyGuildData != null)
		{
			NKMGuildMemberData nKMGuildMemberData = MyGuildData.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == NKCScenManager.CurrentUserData().m_UserUID);
			if (nKMGuildMemberData != null)
			{
				return nKMGuildMemberData.grade;
			}
		}
		return GuildMemberGrade.Member;
	}

	public static string GetNotice()
	{
		GuildChatNoticeType chatNoticeType = MyGuildData.chatNoticeType;
		if (chatNoticeType == GuildChatNoticeType.Normal || chatNoticeType != GuildChatNoticeType.Dungeon)
		{
			if (!string.IsNullOrEmpty(MyGuildData.notice))
			{
				return MyGuildData.notice;
			}
			return NKCStringTable.GetString("SI_PF_CONSORTIUM_LOBBY_NOTICE_EMPTY_TEXT");
		}
		if (!string.IsNullOrEmpty(MyGuildData.dungeonNotice))
		{
			return MyGuildData.dungeonNotice;
		}
		return NKCStringTable.GetString("SI_PF_CONSORTIUM_DUGEON_NOTICE_EMPTY_TEXT");
	}
}
