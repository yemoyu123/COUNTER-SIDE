using System;
using System.Collections.Generic;
using ClientPacket.Chat;
using ClientPacket.Common;
using ClientPacket.Guild;
using NKC.UI.Friend;
using NKC.UI.Guild;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupPrivateChatLobby : NKCUIBase
{
	public enum CHAT_LOBBY_TAB_TYPE
	{
		FRIEND,
		GUILD_MEMBER
	}

	private const string ASSET_BUNDLE_NAME = "AB_UI_CHAT";

	private const string UI_ASSET_NAME = "AB_UI_CHAT";

	private static NKCPopupPrivateChatLobby m_Instance;

	public GameObject m_objLobby;

	public NKCUIComChat m_Chat;

	public NKCPopupPrivateChatLobbySlot m_pfbSlot;

	public NKCUIComStateButton m_btnClose;

	public NKCUIComToggle m_tglFriend;

	public GameObject m_objFriendRedDot;

	public NKCUIComToggle m_tglGuildMember;

	public GameObject m_objGuildMemberRedDot;

	public LoopScrollRect m_loop;

	public Transform m_trContent;

	public Text m_lbCountDesc;

	public Text m_lbCount;

	public GameObject m_objGuildChatShortcut;

	public NKCUIComStateButton m_btnGuildChat;

	public NKCUIGuildBadge m_GuildBadge;

	public Text m_lbGuildName;

	public GameObject m_objRedDotGuildChat;

	public GameObject m_objNone;

	public float m_AlarmCooltimeSecond = 30f;

	private Stack<NKCPopupPrivateChatLobbySlot> m_stkSlot = new Stack<NKCPopupPrivateChatLobbySlot>();

	private CHAT_LOBBY_TAB_TYPE m_curTabType;

	private List<PrivateChatListData> m_lstFriends = new List<PrivateChatListData>();

	private List<PrivateChatListData> m_lstGuilds = new List<PrivateChatListData>();

	private DateTime m_LastAlarmTime = DateTime.MinValue;

	private DateTime m_MyMessageAlarmTime = DateTime.MinValue;

	private int m_SoundID = -1;

	public static NKCPopupPrivateChatLobby Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupPrivateChatLobby>("AB_UI_CHAT", "AB_UI_CHAT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupPrivateChatLobby>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		if (m_SoundID >= 0)
		{
			NKCSoundManager.StopSound(m_SoundID);
			m_SoundID = -1;
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void InitUI()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_objLobby, bValue: true);
		m_Chat.InitUI(OnSendMessage, OnCloseChat, bDisableTranslate: true);
		if (m_btnClose != null)
		{
			m_btnClose.PointerClick.RemoveAllListeners();
			m_btnClose.PointerClick.AddListener(base.Close);
		}
		if (m_loop != null)
		{
			NKCUtil.SetGameobjectActive(m_loop, bValue: true);
			m_loop.dOnGetObject += GetObject;
			m_loop.dOnReturnObject += ReturnObject;
			m_loop.dOnProvideData += ProvideData;
			m_loop.PrepareCells();
		}
		if (m_tglFriend != null)
		{
			m_tglFriend.OnValueChanged.RemoveAllListeners();
			m_tglFriend.OnValueChanged.AddListener(OnChangedFriend);
		}
		if (m_tglGuildMember != null)
		{
			m_tglGuildMember.OnValueChanged.RemoveAllListeners();
			m_tglGuildMember.OnValueChanged.AddListener(OnChangedGuildMember);
			m_tglGuildMember.m_bGetCallbackWhileLocked = true;
		}
		NKCUtil.SetGameobjectActive(m_btnGuildChat, bValue: false);
		if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.HIDE_GUILD_CHAT) && m_btnGuildChat != null)
		{
			NKCUtil.SetGameobjectActive(m_btnGuildChat, bValue: true);
			m_btnGuildChat.PointerClick.RemoveAllListeners();
			m_btnGuildChat.PointerClick.AddListener(OnClickGuildChat);
			m_btnGuildChat.m_bGetCallbackWhileLocked = true;
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void OnBackButton()
	{
		if (m_Chat.gameObject.activeSelf)
		{
			m_Chat.Close();
		}
		else
		{
			Close();
		}
	}

	private void OnSendMessage(long channelUid, ChatMessageType messageType, string message, int emotionId)
	{
		NKCChatManager.SetCurrentChatRoomUid(channelUid);
		NKCPacketSender.Send_NKMPacket_PRIVATE_CHAT_REQ(channelUid, message, emotionId);
	}

	private void OnCloseChat()
	{
		NKCUtil.SetGameobjectActive(m_objLobby, bValue: true);
		RefreshUI();
	}

	private RectTransform GetObject(int idx)
	{
		NKCPopupPrivateChatLobbySlot nKCPopupPrivateChatLobbySlot = null;
		if (m_stkSlot.Count > 0)
		{
			nKCPopupPrivateChatLobbySlot = m_stkSlot.Pop();
		}
		else
		{
			nKCPopupPrivateChatLobbySlot = UnityEngine.Object.Instantiate(m_pfbSlot, m_trContent);
			nKCPopupPrivateChatLobbySlot.Init();
		}
		return nKCPopupPrivateChatLobbySlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCPopupPrivateChatLobbySlot component = tr.GetComponent<NKCPopupPrivateChatLobbySlot>();
		NKCUtil.SetGameobjectActive(component, bValue: false);
		m_stkSlot.Push(component);
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCPopupPrivateChatLobbySlot component = tr.GetComponent<NKCPopupPrivateChatLobbySlot>();
		NKCUtil.SetGameobjectActive(component, bValue: true);
		if (m_curTabType == CHAT_LOBBY_TAB_TYPE.FRIEND)
		{
			if (m_lstFriends.Count > idx)
			{
				component.SetData(m_curTabType, m_lstFriends[idx], OnClickChatSlot, NKCChatManager.CheckPrivateChatNotify(NKCScenManager.CurrentUserData(), m_lstFriends[idx].commonProfile.userUid));
			}
			else
			{
				NKCUtil.SetGameobjectActive(component, bValue: false);
			}
		}
		else if (m_lstGuilds.Count > idx)
		{
			component.SetData(m_curTabType, m_lstGuilds[idx], OnClickChatSlot, NKCChatManager.CheckPrivateChatNotify(NKCScenManager.CurrentUserData(), m_lstGuilds[idx].commonProfile.userUid));
		}
		else
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
		}
	}

	public void Open(long reservedChatRoomUid = 0L)
	{
		m_SoundID = NKCSoundManager.PlaySound("FX_UI_CHAT_INTRO", 1f, 0f, 0f);
		NKCUtil.SetGameobjectActive(m_Chat, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLobby, bValue: true);
		m_LastAlarmTime = DateTime.MinValue;
		if (NKCGuildManager.HasGuild())
		{
			m_GuildBadge.SetData(NKCGuildManager.MyGuildData.badgeId);
			NKCUtil.SetLabelText(m_lbGuildName, NKCGuildManager.MyGuildData.name);
			NKCUtil.SetGameobjectActive(m_objRedDotGuildChat, NKCChatManager.CheckGuildChatNotify());
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_curTabType = CHAT_LOBBY_TAB_TYPE.FRIEND;
		m_tglFriend.Select(bSelect: true, bForce: true, bImmediate: true);
		m_tglGuildMember.Select(bSelect: false, bForce: true, bImmediate: true);
		if (NKCGuildManager.HasGuild())
		{
			m_btnGuildChat.UnLock();
			m_tglGuildMember.UnLock();
		}
		else
		{
			m_btnGuildChat.Lock();
			m_tglGuildMember.Lock();
		}
		OnChangedFriend(bValue: true);
		if (reservedChatRoomUid > 0)
		{
			ShowPrivateChat(reservedChatRoomUid);
		}
		UIOpened();
		if (!NKCChatManager.bAllListRequested)
		{
			NKCChatManager.bAllListRequested = true;
			NKCPacketSender.Send_NKMPacket_PRIVATE_CHAT_ALL_LIST_REQ();
		}
	}

	public void ShowPrivateChat(long userUid)
	{
		NKCUtil.SetGameobjectActive(m_objLobby, bValue: false);
		m_Chat.SetData(userUid, bEnableMute: false, FindNicknameByUserUid(userUid) ?? "");
	}

	private string FindNicknameByUserUid(long userUid)
	{
		string result = "";
		if (NKCGuildManager.IsGuildMemberByUID(userUid))
		{
			NKMGuildMemberData nKMGuildMemberData = NKCGuildManager.MyGuildData.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == userUid);
			if (nKMGuildMemberData != null)
			{
				return nKMGuildMemberData.commonProfile.nickname;
			}
		}
		else
		{
			FriendListData friendListData = NKCFriendManager.FriendListData.Find((FriendListData x) => x.commonProfile.userUid == userUid);
			if (friendListData != null)
			{
				return friendListData.commonProfile.nickname;
			}
		}
		return result;
	}

	private void OnClickChatSlot(NKMCommonProfile commonProfile)
	{
		NKCPacketSender.Send_NKMPacket_PRIVATE_CHAT_LIST_REQ(commonProfile.userUid);
	}

	private void OnChangedFriend(bool bValue)
	{
		if (bValue)
		{
			m_curTabType = CHAT_LOBBY_TAB_TYPE.FRIEND;
			RefreshUI(bResetScroll: true);
		}
	}

	private void OnChangedGuildMember(bool bValue)
	{
		if (m_tglGuildMember.m_bLock)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_CHAT_CONSORTIUM_JOIN_REQ_DESC, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else if (bValue)
		{
			m_curTabType = CHAT_LOBBY_TAB_TYPE.GUILD_MEMBER;
			RefreshUI(bResetScroll: true);
		}
	}

	private int SortByMessageTime(PrivateChatListData lData, PrivateChatListData rData)
	{
		if (rData == null || rData.lastMessage == null)
		{
			return -1;
		}
		if (lData == null || lData.lastMessage == null)
		{
			return 1;
		}
		if (rData.lastMessage.createdAt == lData.lastMessage.createdAt)
		{
			return rData.commonProfile.nickname.CompareTo(lData.commonProfile.nickname);
		}
		return rData.lastMessage.createdAt.CompareTo(lData.lastMessage.createdAt);
	}

	public void RefreshUI(bool bResetScroll = false)
	{
		if (m_curTabType == CHAT_LOBBY_TAB_TYPE.FRIEND)
		{
			m_lstFriends = NKCChatManager.FriendChatList;
			for (int num = m_lstFriends.Count - 1; num >= 0; num--)
			{
				if (NKCFriendManager.IsBlockedUser(m_lstFriends[num].commonProfile.friendCode))
				{
					m_lstFriends.RemoveAt(num);
				}
			}
			m_lstFriends.Sort(SortByMessageTime);
			m_loop.TotalCount = m_lstFriends.Count;
			NKCUtil.SetLabelText(m_lbCountDesc, NKCUtilString.GET_STRING_CHAT_FRIEND_COUNT_TEXT);
			NKCUtil.SetLabelText(m_lbCount, $"{NKCFriendManager.FriendListData.Count}/{60}");
		}
		else if (m_curTabType == CHAT_LOBBY_TAB_TYPE.GUILD_MEMBER)
		{
			m_lstGuilds = NKCChatManager.GuildChatList;
			for (int num2 = m_lstGuilds.Count - 1; num2 >= 0; num2--)
			{
				if (NKCFriendManager.IsBlockedUser(m_lstGuilds[num2].commonProfile.friendCode))
				{
					m_lstGuilds.RemoveAt(num2);
				}
			}
			m_lstGuilds.Sort(SortByMessageTime);
			m_loop.TotalCount = m_lstGuilds.Count;
			NKCUtil.SetLabelText(m_lbCountDesc, NKCUtilString.GET_STRING_CHAT_CONSORTIUM_MEMBER_COUNT_TEXT);
			NKCUtil.SetLabelText(m_lbCount, $"{NKCGuildManager.MyGuildData.members.Count}/{NKCGuildManager.GetMaxGuildMemberCount(NKCGuildManager.MyGuildData.guildLevel)}");
		}
		NKCUtil.SetGameobjectActive(m_loop, m_loop.TotalCount > 0);
		NKCUtil.SetGameobjectActive(m_objNone, m_loop.TotalCount == 0);
		if (bResetScroll && m_loop.TotalCount > 0)
		{
			m_loop.SetIndexPosition(0);
		}
		else
		{
			m_loop.RefreshCells();
		}
		NKCUtil.SetGameobjectActive(m_objFriendRedDot, NKCChatManager.CheckPrivateChatNotifyByTabType(CHAT_LOBBY_TAB_TYPE.FRIEND, 0L));
		NKCUtil.SetGameobjectActive(m_objGuildMemberRedDot, NKCChatManager.CheckPrivateChatNotifyByTabType(CHAT_LOBBY_TAB_TYPE.GUILD_MEMBER, 0L));
		if (NKCGuildManager.HasGuild())
		{
			m_btnGuildChat.UnLock();
			m_tglGuildMember.UnLock();
			NKCUtil.SetGameobjectActive(m_objRedDotGuildChat, NKCChatManager.CheckGuildChatNotify());
		}
		else
		{
			m_btnGuildChat.Lock();
			m_tglGuildMember.Lock();
			NKCUtil.SetGameobjectActive(m_objRedDotGuildChat, bValue: false);
		}
		foreach (NKCPopupPrivateChatLobbySlot item in m_stkSlot)
		{
			item.OnRefreshUI();
		}
	}

	private void OnClickGuildChat()
	{
		if (m_btnGuildChat.m_bLock)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_CHAT_CONSORTIUM_JOIN_REQ_DESC, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			return;
		}
		NKCUtil.SetGameobjectActive(m_objRedDotGuildChat, bValue: false);
		NKCPopupGuildChat.Instance.Open(NKCGuildManager.MyGuildData.guildUid);
	}

	public void AddMessage(NKMChatMessageData data, bool bIsMyMessage)
	{
		CheckAlarmSound(data.commonProfile.userUid, bIsMyMessage);
		if (m_Chat.gameObject.activeSelf)
		{
			m_Chat.AddMessage(data, bIsMyMessage, bForceResetScroll: true);
			return;
		}
		if (NKCFriendManager.IsFriend(data.commonProfile.friendCode))
		{
			NKCUtil.SetGameobjectActive(m_objFriendRedDot, bValue: true);
		}
		if (NKCGuildManager.IsGuildMember(data.commonProfile.friendCode))
		{
			NKCUtil.SetGameobjectActive(m_objGuildMemberRedDot, bValue: true);
		}
		RefreshUI();
	}

	private bool CheckAlarmSound(long userUid, bool bIsMyMessage)
	{
		if (!NKCScenManager.GetScenManager().GetGameOptionData().UseChatNotifySound)
		{
			return false;
		}
		if (bIsMyMessage)
		{
			if ((NKCSynchronizedTime.ServiceTime - m_MyMessageAlarmTime).TotalSeconds >= (double)m_AlarmCooltimeSecond)
			{
				m_MyMessageAlarmTime = NKCSynchronizedTime.ServiceTime;
				NKCSoundManager.PlaySound("FX_UI_CHAT_SEND", 1f, 0f, 0f);
				return true;
			}
		}
		else if ((NKCSynchronizedTime.ServiceTime - m_LastAlarmTime).TotalSeconds >= (double)m_AlarmCooltimeSecond)
		{
			if (!m_Chat.gameObject.activeInHierarchy)
			{
				m_LastAlarmTime = NKCSynchronizedTime.ServiceTime;
				NKCSoundManager.PlaySound("FX_UI_CHAT_MAIN_ALARM", 1f, 0f, 0f);
				return true;
			}
			if (m_Chat.GetChannelUid() == userUid)
			{
				m_LastAlarmTime = NKCSynchronizedTime.ServiceTime;
				NKCSoundManager.PlaySound("FX_UI_CHAT_RECEIVE", 1f, 0f, 0f);
				return true;
			}
		}
		return false;
	}

	public override void OnGuildDataChanged()
	{
		if (m_Chat.gameObject.activeInHierarchy)
		{
			m_Chat.OnGuildDataChanged();
		}
		else
		{
			RefreshUI();
		}
	}

	public NKCUIFriendSlot.FRIEND_SLOT_TYPE GetFriendSlotType()
	{
		return m_curTabType switch
		{
			CHAT_LOBBY_TAB_TYPE.FRIEND => NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_FRIEND_LIST, 
			CHAT_LOBBY_TAB_TYPE.GUILD_MEMBER => NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_GUILD_LIST, 
			_ => NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_NONE, 
		};
	}

	public void OnRecvFriendBlock(long friendCode)
	{
		if (m_Chat.gameObject.activeSelf)
		{
			PrivateChatListData privateChatListData = m_lstFriends.Find((PrivateChatListData x) => x.commonProfile.friendCode == friendCode);
			if (privateChatListData != null && m_Chat.GetChannelUid() == privateChatListData.commonProfile.userUid)
			{
				m_Chat.Close();
				return;
			}
			PrivateChatListData privateChatListData2 = m_lstGuilds.Find((PrivateChatListData x) => x.commonProfile.friendCode == friendCode);
			if (privateChatListData2 != null && m_Chat.GetChannelUid() == privateChatListData2.commonProfile.userUid)
			{
				m_Chat.Close();
				return;
			}
		}
		RefreshUI();
	}

	public override void OnScreenResolutionChanged()
	{
		base.OnScreenResolutionChanged();
		m_Chat.OnScreenResolutionChanged();
	}

	public void RefreshEmoticonList()
	{
		m_Chat.RefreshEmoticonList();
	}
}
