using System;
using ClientPacket.Chat;
using ClientPacket.Common;
using NKC.UI.Component;
using NKC.UI.Guild;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupPrivateChatLobbySlot : MonoBehaviour
{
	public delegate void OnClickChat(NKMCommonProfile commonProfile);

	[Header("프로필 슬롯")]
	public NKCUISlotProfile m_Slot;

	[Header("기본정보")]
	public Text m_lbLevel;

	public Text m_lbName;

	[Header("유저 상세정보")]
	public NKCUIComStateButton m_btnInfo;

	[Header("컨소시움")]
	public GameObject m_objGuild;

	public NKCUIGuildBadge m_GuildBadge;

	public Text m_lbGuildName;

	[Header("사업자 등록 번호")]
	public Text m_lbFriendCode;

	[Header("시간")]
	public Text m_lbLastChatTime;

	[Header("우측 버튼")]
	public NKCUIComStateButton m_btnChat;

	public GameObject m_objRedDot;

	[Header("칭호")]
	public NKCUIComTitlePanel m_titlePanel;

	private OnClickChat m_dOnClickChat;

	private NKMCommonProfile m_commonProfile;

	private DateTime m_LastChatTime;

	private DateTime m_prevChatTimeUpdate;

	private const float LASTCHAT_UPDATE_INTERVAL = 3f;

	private float m_deltaTime;

	public void Init()
	{
		m_Slot?.Init();
		m_btnChat.PointerClick.RemoveAllListeners();
		m_btnChat.PointerClick.AddListener(OnClickSlot);
		if (m_btnInfo != null)
		{
			m_btnInfo.PointerClick.RemoveAllListeners();
			m_btnInfo.PointerClick.AddListener(OnClickInfo);
		}
	}

	public void OnRefreshUI()
	{
		m_deltaTime = 3f;
	}

	public void SetData(NKCPopupPrivateChatLobby.CHAT_LOBBY_TAB_TYPE lobbyTabType, PrivateChatListData privateChatData, OnClickChat dOnClickChat, bool bEnableRedDot = false)
	{
		m_prevChatTimeUpdate = DateTime.MinValue;
		NKCUtil.SetLabelText(m_lbLastChatTime, "-");
		m_dOnClickChat = dOnClickChat;
		m_commonProfile = privateChatData.commonProfile;
		m_Slot.SetProfiledata(m_commonProfile, delegate
		{
			OnClickSlot();
		});
		NKCUtil.SetLabelText(m_lbLevel, NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, m_commonProfile.level);
		NKCUtil.SetLabelText(m_lbName, m_commonProfile.nickname);
		NKCUtil.SetLabelText(m_lbFriendCode, $"#{m_commonProfile.friendCode}");
		if (privateChatData.lastMessage != null)
		{
			m_LastChatTime = privateChatData.lastMessage.createdAt;
		}
		else
		{
			m_LastChatTime = DateTime.MinValue;
		}
		SetLastChatTime();
		switch (lobbyTabType)
		{
		case NKCPopupPrivateChatLobby.CHAT_LOBBY_TAB_TYPE.FRIEND:
		{
			FriendListData friendListData = NKCFriendManager.FriendListData.Find((FriendListData x) => x.commonProfile.userUid == privateChatData.commonProfile.userUid);
			if (friendListData != null)
			{
				if (friendListData.guildData != null)
				{
					NKCUtil.SetGameobjectActive(m_objGuild, friendListData.guildData.guildUid > 0);
					if (friendListData.guildData.guildUid > 0)
					{
						m_GuildBadge.SetData(friendListData.guildData.badgeId);
						NKCUtil.SetLabelText(m_lbGuildName, friendListData.guildData.guildName);
					}
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_objGuild, bValue: false);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			}
			break;
		}
		case NKCPopupPrivateChatLobby.CHAT_LOBBY_TAB_TYPE.GUILD_MEMBER:
			NKCUtil.SetGameobjectActive(m_objGuild, NKCGuildManager.MyData.guildUid > 0 && NKCGuildManager.MyGuildData != null);
			if (NKCGuildManager.MyData.guildUid > 0)
			{
				m_GuildBadge.SetData(NKCGuildManager.MyGuildData.badgeId);
				NKCUtil.SetLabelText(m_lbGuildName, NKCGuildManager.MyGuildData.name);
			}
			break;
		}
		NKCUtil.SetGameobjectActive(m_objRedDot, bEnableRedDot);
		m_titlePanel?.SetData(m_commonProfile);
	}

	private void SetCommonData()
	{
		m_Slot.SetProfiledata(m_commonProfile, delegate
		{
			OnClickSlot();
		});
		NKCUtil.SetLabelText(m_lbLevel, NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, m_commonProfile.level);
		NKCUtil.SetLabelText(m_lbName, m_commonProfile.nickname);
		NKCUtil.SetLabelText(m_lbFriendCode, $"#{m_commonProfile.friendCode}");
		m_LastChatTime = NKCChatManager.GetLastChatTime(m_commonProfile.userUid);
		SetLastChatTime();
	}

	private void SetLastChatTime()
	{
		m_LastChatTime = NKCChatManager.GetLastChatTime(m_commonProfile.userUid);
		if (m_prevChatTimeUpdate != m_LastChatTime && m_LastChatTime > DateTime.MinValue)
		{
			m_prevChatTimeUpdate = m_LastChatTime;
			NKCUtil.SetLabelText(m_lbLastChatTime, NKCUtilString.GetLastTimeString(NKMTime.LocalToUTC(m_LastChatTime)));
		}
	}

	private void OnClickInfo()
	{
		NKCPacketSender.Send_NKMPacket_USER_PROFILE_INFO_REQ(m_commonProfile.userUid, NKM_DECK_TYPE.NDT_NORMAL);
	}

	private void OnClickSlot()
	{
		m_dOnClickChat?.Invoke(m_commonProfile);
	}

	private void Update()
	{
		if (m_commonProfile != null)
		{
			m_deltaTime += Time.deltaTime;
			if (m_deltaTime >= 3f)
			{
				m_deltaTime = 0f;
				SetLastChatTime();
			}
		}
	}
}
