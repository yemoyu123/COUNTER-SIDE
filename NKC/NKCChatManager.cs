using System;
using System.Collections.Generic;
using ClientPacket.Chat;
using ClientPacket.Common;
using ClientPacket.Community;
using ClientPacket.Guild;
using NKC.Publisher;
using NKC.UI;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public static class NKCChatManager
{
	public const int MAX_CHAT_COUNT = 100;

	private static Dictionary<long, List<NKMChatMessageData>> m_dicChatList = new Dictionary<long, List<NKMChatMessageData>>();

	private static Dictionary<long, string> m_dicTranslatedMessage = new Dictionary<long, string>();

	private static Dictionary<long, long> m_dicLastCheckedMessageUid = new Dictionary<long, long>();

	private static List<PrivateChatListData> m_friends = new List<PrivateChatListData>();

	private static List<PrivateChatListData> m_guilds = new List<PrivateChatListData>();

	private static long m_LastSendChatRoomUid = 0L;

	private static bool m_bAllListRequested = false;

	public static List<PrivateChatListData> FriendChatList
	{
		get
		{
			if (NKCFriendManager.FriendListData.Count > m_friends.Count)
			{
				foreach (FriendListData friendListData in NKCFriendManager.FriendListData)
				{
					if (m_friends.Find((PrivateChatListData x) => x.commonProfile.friendCode == friendListData.commonProfile.friendCode) == null)
					{
						PrivateChatListData privateChatListData = new PrivateChatListData();
						privateChatListData.commonProfile = friendListData.commonProfile;
						m_friends.Add(privateChatListData);
						AddPrivateChatListData(privateChatListData);
					}
				}
			}
			return m_friends;
		}
	}

	public static List<PrivateChatListData> GuildChatList
	{
		get
		{
			if (NKCGuildManager.MyGuildMemberDataList.Count > m_guilds.Count)
			{
				foreach (NKMGuildMemberData guildMemberData in NKCGuildManager.MyGuildMemberDataList)
				{
					if (m_guilds.Find((PrivateChatListData x) => x.commonProfile.friendCode == guildMemberData.commonProfile.friendCode) == null)
					{
						PrivateChatListData privateChatListData = new PrivateChatListData();
						privateChatListData.commonProfile = guildMemberData.commonProfile;
						m_guilds.Add(privateChatListData);
						AddPrivateChatListData(privateChatListData);
					}
				}
			}
			return m_guilds;
		}
	}

	public static DateTime m_MuteEndDate { get; private set; }

	public static bool HasAnyNewMessage { get; private set; }

	public static bool bAllListRequested
	{
		get
		{
			return m_bAllListRequested;
		}
		set
		{
			m_bAllListRequested = value;
		}
	}

	public static void Initialize()
	{
		m_dicChatList.Clear();
		m_dicTranslatedMessage.Clear();
		m_dicLastCheckedMessageUid.Clear();
		m_friends.Clear();
		m_guilds.Clear();
		m_LastSendChatRoomUid = 0L;
		m_MuteEndDate = default(DateTime);
		HasAnyNewMessage = false;
		m_bAllListRequested = false;
	}

	public static bool IsContentsUnlocked()
	{
		if (NKCContentManager.CheckContentStatus(ContentsType.FRIENDS, out var _) == NKCContentManager.eContentStatus.Open)
		{
			return NKMOpenTagManager.IsOpened("CHAT_PRIVATE");
		}
		return false;
	}

	private static string GetLastCheckedChatUidKey(long channelId)
	{
		return $"{NKCScenManager.CurrentUserData().m_UserUID}_{channelId}_LAST_CHECKED_CHAT_UID";
	}

	public static long GetLastCheckedMessageUid(long channelUid)
	{
		if (m_dicLastCheckedMessageUid.ContainsKey(channelUid))
		{
			return m_dicLastCheckedMessageUid[channelUid];
		}
		return 0L;
	}

	public static DateTime GetLastChatTime(long userUid)
	{
		if (m_dicChatList.TryGetValue(userUid, out var value) && value.Count > 0)
		{
			return value[value.Count - 1].createdAt;
		}
		return DateTime.MinValue;
	}

	public static List<NKMChatMessageData> GetChatList(long channelUid, out bool bWaitForData)
	{
		if (m_dicChatList.ContainsKey(channelUid))
		{
			bWaitForData = false;
			return m_dicChatList[channelUid];
		}
		m_dicChatList.Add(channelUid, new List<NKMChatMessageData>());
		bWaitForData = true;
		NKCPacketSender.Send_NKMPacket_GUILD_CHAT_LIST_REQ(channelUid);
		return null;
	}

	public static string GetTranslatedMessage(long chatUid)
	{
		if (m_dicTranslatedMessage.ContainsKey(chatUid))
		{
			return m_dicTranslatedMessage[chatUid];
		}
		return string.Empty;
	}

	public static void OnRecvGuildChatList(long channelUid, List<NKMChatMessageData> lstchatData, bool bRefreshUI)
	{
		if (lstchatData == null)
		{
			lstchatData = new List<NKMChatMessageData>();
		}
		if (!m_dicChatList.ContainsKey(channelUid))
		{
			m_dicChatList.Add(channelUid, lstchatData);
		}
		else
		{
			m_dicChatList[channelUid] = lstchatData;
		}
		long num = 0L;
		if (NKCPopupGuildChat.IsInstanceOpen)
		{
			if (lstchatData.Count > 0)
			{
				num = lstchatData[lstchatData.Count - 1].messageUid;
			}
		}
		else if (PlayerPrefs.HasKey(GetLastCheckedChatUidKey(NKCGuildManager.MyData.guildUid)))
		{
			num = long.Parse(PlayerPrefs.GetString(GetLastCheckedChatUidKey(NKCGuildManager.MyData.guildUid)));
		}
		if (num > 0)
		{
			if (m_dicLastCheckedMessageUid.ContainsKey(NKCGuildManager.MyData.guildUid))
			{
				m_dicLastCheckedMessageUid[NKCGuildManager.MyData.guildUid] = num;
			}
			else
			{
				m_dicLastCheckedMessageUid.Add(NKCGuildManager.MyData.guildUid, num);
			}
		}
		if (NKCPopupGuildChat.IsInstanceOpen)
		{
			NKCPopupGuildChat.Instance.OnChatDataReceived(channelUid, m_dicChatList[channelUid], bRefreshUI);
		}
		else
		{
			Debug.LogWarning("안읽은 메세지 : " + GetUncheckedMessageCount(channelUid));
		}
	}

	public static void OnRecv(NKMPacket_GUILD_CHAT_NOT cNKMPacket_GUILD_CHAT_NOT)
	{
		if (!m_dicChatList.ContainsKey(NKCGuildManager.MyData.guildUid))
		{
			m_dicChatList.Add(NKCGuildManager.MyData.guildUid, new List<NKMChatMessageData> { cNKMPacket_GUILD_CHAT_NOT.message });
		}
		else
		{
			if (m_dicChatList[NKCGuildManager.MyData.guildUid].Find((NKMChatMessageData x) => x.messageUid == cNKMPacket_GUILD_CHAT_NOT.message.messageUid && x.message != null) != null)
			{
				return;
			}
			m_dicChatList[NKCGuildManager.MyData.guildUid].Add(cNKMPacket_GUILD_CHAT_NOT.message);
			if (m_dicChatList[NKCGuildManager.MyData.guildUid].Count > 100)
			{
				int num = m_dicChatList[NKCGuildManager.MyData.guildUid].Count - 100;
				for (int num2 = 0; num2 < num; num2++)
				{
					m_dicChatList[NKCGuildManager.MyData.guildUid].RemoveAt(0);
				}
			}
		}
		if (NKCPopupGuildChat.IsInstanceOpen)
		{
			NKCPopupGuildChat.Instance.AddMessage(cNKMPacket_GUILD_CHAT_NOT.message);
			return;
		}
		if (NKCPopupPrivateChatLobby.IsInstanceOpen)
		{
			NKCPopupPrivateChatLobby.Instance.RefreshUI();
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_LOBBY)
		{
			HasAnyNewMessage = true;
		}
	}

	public static bool CheckGuildChatNotify()
	{
		if (!NKCGuildManager.HasGuild())
		{
			return false;
		}
		long guildUid = NKCGuildManager.MyGuildData.guildUid;
		if (m_dicChatList.ContainsKey(guildUid))
		{
			List<NKMChatMessageData> list = m_dicChatList[guildUid];
			if (list.Count > 0)
			{
				NKMChatMessageData nKMChatMessageData = list[list.Count - 1];
				if (nKMChatMessageData.createdAt > NKMTime.UTCtoLocal(NKCScenManager.CurrentUserData().m_NKMUserDateData.m_LastLogOutTime))
				{
					if (!m_dicLastCheckedMessageUid.ContainsKey(guildUid))
					{
						return true;
					}
					if (nKMChatMessageData.messageUid != m_dicLastCheckedMessageUid[guildUid])
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public static void SetCurrentChatRoomUid(long chatRoomUid)
	{
		m_LastSendChatRoomUid = chatRoomUid;
	}

	public static void Init()
	{
	}

	public static void AddPrivateChatListData(PrivateChatListData privateChatListData)
	{
		if (privateChatListData != null && privateChatListData.lastMessage != null)
		{
			if (m_dicChatList.ContainsKey(privateChatListData.commonProfile.userUid))
			{
				m_dicChatList[privateChatListData.commonProfile.userUid] = new List<NKMChatMessageData> { privateChatListData.lastMessage };
			}
			else
			{
				m_dicChatList.Add(privateChatListData.commonProfile.userUid, new List<NKMChatMessageData> { privateChatListData.lastMessage });
			}
		}
	}

	public static void OnRecvAllChat(NKMPacket_PRIVATE_CHAT_ALL_LIST_ACK sPacket)
	{
		m_friends = sPacket.friends;
		m_guilds = sPacket.guildMembers;
		foreach (PrivateChatListData friend in sPacket.friends)
		{
			AddPrivateChatListData(friend);
		}
		foreach (PrivateChatListData guildMember in sPacket.guildMembers)
		{
			AddPrivateChatListData(guildMember);
		}
		if (NKCPopupPrivateChatLobby.IsInstanceOpen)
		{
			NKCPopupPrivateChatLobby.Instance.RefreshUI();
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_CHAT_NOT sPacket)
	{
		bool flag = false;
		bool flag2 = sPacket.message.commonProfile.userUid == NKCScenManager.CurrentUserData().m_UserUID;
		long targetRoomUid = 0L;
		if (flag2)
		{
			targetRoomUid = m_LastSendChatRoomUid;
		}
		else
		{
			targetRoomUid = sPacket.message.commonProfile.userUid;
		}
		if (targetRoomUid == 0L)
		{
			return;
		}
		if (!m_dicChatList.ContainsKey(targetRoomUid))
		{
			m_dicChatList.Add(targetRoomUid, new List<NKMChatMessageData> { sPacket.message });
		}
		else
		{
			if (m_dicChatList[targetRoomUid].Find((NKMChatMessageData x) => x.messageUid == sPacket.message.messageUid && x.message != null) != null)
			{
				return;
			}
			m_dicChatList[targetRoomUid].Add(sPacket.message);
			if (m_dicChatList[targetRoomUid].Count > 100)
			{
				int num = m_dicChatList[targetRoomUid].Count - 100;
				for (int num2 = 0; num2 < num; num2++)
				{
					m_dicChatList[targetRoomUid].RemoveAt(0);
				}
			}
		}
		if (NKCContentManager.CheckContentStatus(ContentsType.FRIENDS, out var _) == NKCContentManager.eContentStatus.Open)
		{
			FriendListData friendListData = NKCFriendManager.FriendListData.Find((FriendListData x) => x.commonProfile.userUid == targetRoomUid);
			if (friendListData != null)
			{
				PrivateChatListData privateChatListData = m_friends.Find((PrivateChatListData x) => x.commonProfile.userUid == targetRoomUid);
				if (privateChatListData != null)
				{
					privateChatListData.lastMessage = sPacket.message;
				}
				else
				{
					PrivateChatListData privateChatListData2 = new PrivateChatListData();
					privateChatListData2.commonProfile = friendListData.commonProfile;
					privateChatListData2.lastMessage = sPacket.message;
					m_friends.Add(privateChatListData2);
				}
				flag = true;
			}
		}
		if (NKCGuildManager.HasGuild())
		{
			NKMGuildMemberData nKMGuildMemberData = NKCGuildManager.MyGuildMemberDataList.Find((NKMGuildMemberData x) => x.commonProfile.userUid == targetRoomUid);
			if (nKMGuildMemberData != null)
			{
				PrivateChatListData privateChatListData3 = m_guilds.Find((PrivateChatListData x) => x.commonProfile.userUid == targetRoomUid);
				if (privateChatListData3 != null)
				{
					privateChatListData3.lastMessage = sPacket.message;
				}
				else
				{
					PrivateChatListData privateChatListData4 = new PrivateChatListData();
					privateChatListData4.commonProfile = nKMGuildMemberData.commonProfile;
					privateChatListData4.lastMessage = sPacket.message;
					m_guilds.Add(privateChatListData4);
				}
				flag = true;
			}
		}
		if (NKCPopupPrivateChatLobby.IsInstanceOpen)
		{
			NKCPopupPrivateChatLobby.Instance.AddMessage(sPacket.message, flag2);
		}
		else if (flag && NKCScenManager.GetScenManager().GetGameOptionData().UseChatContent)
		{
			if (NKCPopupHamburgerMenu.IsInstanceOpen)
			{
				NKCPopupHamburgerMenu.instance.Refresh();
			}
			else
			{
				NKCUIManager.NKCUIUpsideMenu.SetHamburgerNotify(bValue: true);
			}
		}
	}

	public static void OnRecv(NKMPacket_PRIVATE_CHAT_LIST_ACK sPacket)
	{
		if (sPacket.messages == null)
		{
			sPacket.messages = new List<NKMChatMessageData>();
		}
		if (m_dicChatList.ContainsKey(sPacket.userUid))
		{
			m_dicChatList[sPacket.userUid] = sPacket.messages;
		}
		else
		{
			m_dicChatList.Add(sPacket.userUid, sPacket.messages);
		}
		if (sPacket.messages.Count > 0)
		{
			FriendListData friendListData = NKCFriendManager.FriendListData.Find((FriendListData x) => x.commonProfile.userUid == sPacket.userUid);
			if (friendListData != null)
			{
				PrivateChatListData privateChatListData = m_friends.Find((PrivateChatListData x) => x.commonProfile.userUid == sPacket.userUid);
				if (privateChatListData != null)
				{
					privateChatListData.lastMessage = sPacket.messages[sPacket.messages.Count - 1];
				}
				else
				{
					PrivateChatListData privateChatListData2 = new PrivateChatListData();
					privateChatListData2.commonProfile = friendListData.commonProfile;
					privateChatListData2.lastMessage = sPacket.messages[sPacket.messages.Count - 1];
					m_friends.Add(privateChatListData2);
				}
			}
			if (NKCGuildManager.HasGuild())
			{
				NKMGuildMemberData nKMGuildMemberData = NKCGuildManager.MyGuildMemberDataList.Find((NKMGuildMemberData x) => x.commonProfile.userUid == sPacket.userUid);
				if (nKMGuildMemberData != null)
				{
					PrivateChatListData privateChatListData3 = m_guilds.Find((PrivateChatListData x) => x.commonProfile.userUid == sPacket.userUid);
					if (privateChatListData3 != null)
					{
						privateChatListData3.lastMessage = sPacket.messages[sPacket.messages.Count - 1];
					}
					else
					{
						PrivateChatListData privateChatListData4 = new PrivateChatListData();
						privateChatListData4.commonProfile = nKMGuildMemberData.commonProfile;
						privateChatListData4.lastMessage = sPacket.messages[sPacket.messages.Count - 1];
						m_guilds.Add(privateChatListData4);
					}
				}
			}
		}
		if (NKCPopupPrivateChatLobby.IsInstanceOpen)
		{
			NKCPopupPrivateChatLobby.Instance.ShowPrivateChat(sPacket.userUid);
		}
		else
		{
			NKCPopupPrivateChatLobby.Instance.Open(sPacket.userUid);
		}
	}

	public static void AddFriend(NKMCommonProfile commonProfile)
	{
		if (m_friends.Find((PrivateChatListData x) => x.commonProfile.userUid == commonProfile.userUid) == null)
		{
			PrivateChatListData privateChatListData = new PrivateChatListData();
			privateChatListData.commonProfile = commonProfile;
			privateChatListData.lastMessage = new NKMChatMessageData();
			privateChatListData.lastMessage.commonProfile = commonProfile;
			m_friends.Add(privateChatListData);
		}
	}

	public static void RemoveFriendByFriendCode(long friendCode)
	{
		PrivateChatListData privateChatListData = m_friends.Find((PrivateChatListData x) => x.commonProfile.friendCode == friendCode);
		if (privateChatListData != null)
		{
			m_friends.Remove(privateChatListData);
		}
	}

	public static bool CheckPrivateChatNotify(NKMUserData userData, long userUid = 0L)
	{
		if (!CheckPrivateChatNotifyByTabType(NKCPopupPrivateChatLobby.CHAT_LOBBY_TAB_TYPE.FRIEND, userUid))
		{
			return CheckPrivateChatNotifyByTabType(NKCPopupPrivateChatLobby.CHAT_LOBBY_TAB_TYPE.GUILD_MEMBER, userUid);
		}
		return true;
	}

	public static bool CheckPrivateChatNotifyByTabType(NKCPopupPrivateChatLobby.CHAT_LOBBY_TAB_TYPE tabType, long userUid = 0L)
	{
		if (!NKCScenManager.GetScenManager().GetGameOptionData().UseChatContent)
		{
			return false;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		switch (tabType)
		{
		case NKCPopupPrivateChatLobby.CHAT_LOBBY_TAB_TYPE.FRIEND:
		{
			for (int j = 0; j < m_friends.Count; j++)
			{
				if (m_friends[j].lastMessage != null && m_friends[j].lastMessage.message != null && (userUid <= 0 || m_friends[j].commonProfile.userUid == userUid) && m_friends[j].lastMessage.createdAt > NKMTime.UTCtoLocal(nKMUserData.m_NKMUserDateData.m_LastLogOutTime))
				{
					if (!m_dicLastCheckedMessageUid.ContainsKey(m_friends[j].commonProfile.userUid))
					{
						return true;
					}
					if (m_dicLastCheckedMessageUid[m_friends[j].commonProfile.userUid] != m_friends[j].lastMessage.messageUid)
					{
						return true;
					}
				}
			}
			break;
		}
		case NKCPopupPrivateChatLobby.CHAT_LOBBY_TAB_TYPE.GUILD_MEMBER:
		{
			for (int i = 0; i < m_guilds.Count; i++)
			{
				if (m_guilds[i].lastMessage != null && m_guilds[i].lastMessage.message != null && (userUid <= 0 || m_guilds[i].commonProfile.userUid == userUid) && m_guilds[i].lastMessage.createdAt > NKMTime.UTCtoLocal(nKMUserData.m_NKMUserDateData.m_LastLogOutTime))
				{
					if (!m_dicLastCheckedMessageUid.ContainsKey(m_guilds[i].commonProfile.userUid))
					{
						return true;
					}
					if (m_dicLastCheckedMessageUid[m_guilds[i].commonProfile.userUid] != m_guilds[i].lastMessage.messageUid)
					{
						return true;
					}
				}
			}
			break;
		}
		}
		return false;
	}

	private static void OnRecv(long uid, NKMChatMessageData message)
	{
		if (m_dicChatList.ContainsKey(uid))
		{
			m_dicChatList.Add(uid, new List<NKMChatMessageData> { message });
		}
		else
		{
			if (m_dicChatList[uid].Find((NKMChatMessageData x) => x.messageUid == message.messageUid) != null)
			{
				return;
			}
			m_dicChatList[uid].Add(message);
			if (m_dicChatList[uid].Count > 100)
			{
				int num = m_dicChatList[uid].Count - 100;
				for (int num2 = 0; num2 < num; num2++)
				{
					m_dicChatList[uid].RemoveAt(0);
				}
			}
		}
	}

	public static void OnRecv(NKC_PUBLISHER_RESULT_CODE resultCode, string translatedString, long chatUID, string additionalError)
	{
		if (NKCPublisherModule.CheckError(resultCode, additionalError))
		{
			if (!m_dicTranslatedMessage.ContainsKey(chatUID))
			{
				m_dicTranslatedMessage.Add(chatUID, translatedString);
			}
			NKCPopupGuildChat.Instance.RefreshList();
		}
	}

	public static int GetUncheckedMessageCount(long channelUid)
	{
		if (!m_dicChatList.ContainsKey(channelUid))
		{
			return 0;
		}
		long messageUid = 0L;
		if (m_dicLastCheckedMessageUid.ContainsKey(channelUid))
		{
			messageUid = m_dicLastCheckedMessageUid[channelUid];
		}
		int num = m_dicChatList[channelUid].FindIndex((NKMChatMessageData x) => x.messageUid == messageUid);
		return m_dicChatList[channelUid].Count - (num + 1);
	}

	public static void SetLastCheckedMeesageUid(long channelUid, long messageUid)
	{
		if (channelUid == NKCGuildManager.MyData.guildUid)
		{
			PlayerPrefs.SetString(GetLastCheckedChatUidKey(NKCGuildManager.MyData.guildUid), messageUid.ToString());
		}
		if (m_dicLastCheckedMessageUid.ContainsKey(channelUid))
		{
			m_dicLastCheckedMessageUid[channelUid] = messageUid;
		}
		else
		{
			m_dicLastCheckedMessageUid.Add(channelUid, messageUid);
		}
	}

	public static void SetMuteEndDate(DateTime endTime)
	{
		m_MuteEndDate = endTime;
		if (NKCPopupGuildChat.IsInstanceOpen)
		{
			NKCPopupGuildChat.Instance.CheckMute();
		}
	}

	public static List<int> GetEmoticons(NKM_EMOTICON_TYPE emoticonType, bool favoriteOnly = false)
	{
		List<int> list = new List<int>();
		foreach (NKMEmoticonData emoticonData in NKCEmoticonManager.EmoticonDatas)
		{
			if (emoticonData != null)
			{
				NKMEmoticonTemplet nKMEmoticonTemplet = NKMEmoticonTemplet.Find(emoticonData.emoticonId);
				if (nKMEmoticonTemplet != null && (!favoriteOnly || emoticonData.isFavorites) && nKMEmoticonTemplet.m_EmoticonType == emoticonType)
				{
					list.Add(emoticonData.emoticonId);
				}
			}
		}
		list.Sort();
		return list;
	}

	public static void ResetGuildMemberChatList()
	{
		m_guilds.Clear();
	}
}
