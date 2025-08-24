using ClientPacket.Common;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupChatSlot : MonoBehaviour
{
	public NKCPopupChatSlotText m_MyTextChat;

	public NKCPopupChatSlotText m_UserTextChat;

	public NKCPopupChatSlotEmoticon m_MyEmoticonChat;

	public NKCPopupChatSlotEmoticon m_UserEmoticonChat;

	public NKCPopupChatSlotEmoticon m_MyCommentChat;

	public NKCPopupChatSlotEmoticon m_UserCommentChat;

	public NKCPopupChatSlotSystem m_SystemChat;

	public NKCpopupChatSlotSpecial m_EventChat;

	public LayoutGroup m_LayoutGroup;

	[Header("내 채팅")]
	public int MY_CHAT_PADDING_LEFT;

	public int MY_CHAT_PADDING_RIGHT = 155;

	public int MY_CHAT_PADDING_TOP = 59;

	public int MY_CHAT_PADDING_BOTTOM = 22;

	public TextAnchor MY_CHAT_ANCHOR = TextAnchor.UpperRight;

	[Header("상대방 채팅")]
	public int USER_CHAT_PADDING_LEFT = 155;

	public int USER_CHAT_PADDING_RIGHT;

	public int USER_CHAT_PADDING_TOP = 59;

	public int USER_CHAT_PADDING_BOTTOM = 22;

	public TextAnchor USER_CHAT_ANCHOR;

	[Header("레벨업")]
	public int LEVEL_UP_CHAT_PADDING_LEFT;

	public int LEVEL_UP_CHAT_PADDING_RIGHT;

	public int LEVEL_UP_CHAT_PADDING_TOP = 26;

	public int LEVEL_UP_CHAT_PADDING_BOTTOM = 26;

	public TextAnchor LEVEL_UP_CHAT_ANCHOR = TextAnchor.UpperRight;

	[Header("시스템 메세지")]
	public int SYSTEM_CHAT_PADDING_LEFT;

	public int SYSTEM_CHAT_PADDING_RIGHT;

	public int SYSTEM_CHAT_PADDING_TOP = 26;

	public int SYSTEM_CHAT_PADDING_BOTTOM = 26;

	public TextAnchor SYSTEM_CHAT_ANCHOR = TextAnchor.UpperRight;

	private long m_ChannelUid;

	private ChatMessageType m_ChatMessageType;

	public void SetData(long channelUid, NKMChatMessageData data, bool disableTranslate = false)
	{
		m_ChannelUid = channelUid;
		m_ChatMessageType = data.messageType;
		NKCUtil.SetGameobjectActive(m_MyTextChat, bValue: false);
		NKCUtil.SetGameobjectActive(m_UserTextChat, bValue: false);
		NKCUtil.SetGameobjectActive(m_MyEmoticonChat, bValue: false);
		NKCUtil.SetGameobjectActive(m_UserEmoticonChat, bValue: false);
		NKCUtil.SetGameobjectActive(m_MyCommentChat, bValue: false);
		NKCUtil.SetGameobjectActive(m_UserCommentChat, bValue: false);
		NKCUtil.SetGameobjectActive(m_SystemChat, bValue: false);
		NKCUtil.SetGameobjectActive(m_EventChat, bValue: false);
		switch (data.messageType)
		{
		case ChatMessageType.Normal:
			if (data.commonProfile.userUid == NKCScenManager.CurrentUserData().m_UserUID)
			{
				if (data.emotionId > 0)
				{
					NKMEmoticonTemplet nKMEmoticonTemplet = NKMEmoticonTemplet.Find(data.emotionId);
					if (nKMEmoticonTemplet != null)
					{
						if (nKMEmoticonTemplet.m_EmoticonType == NKM_EMOTICON_TYPE.NET_ANI)
						{
							NKCUtil.SetGameobjectActive(m_MyEmoticonChat, bValue: true);
							m_MyEmoticonChat.SetData(m_ChannelUid, data);
						}
						else
						{
							NKCUtil.SetGameobjectActive(m_MyCommentChat, bValue: true);
							m_MyCommentChat.SetData(m_ChannelUid, data);
						}
					}
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_MyTextChat, bValue: true);
					m_MyTextChat.SetData(m_ChannelUid, data, disableTranslate);
				}
				m_LayoutGroup.padding = new RectOffset(MY_CHAT_PADDING_LEFT, MY_CHAT_PADDING_RIGHT, MY_CHAT_PADDING_TOP, MY_CHAT_PADDING_BOTTOM);
				m_LayoutGroup.childAlignment = MY_CHAT_ANCHOR;
				break;
			}
			if (data.emotionId > 0)
			{
				NKMEmoticonTemplet nKMEmoticonTemplet2 = NKMEmoticonTemplet.Find(data.emotionId);
				if (nKMEmoticonTemplet2 != null)
				{
					if (nKMEmoticonTemplet2.m_EmoticonType == NKM_EMOTICON_TYPE.NET_ANI)
					{
						NKCUtil.SetGameobjectActive(m_UserEmoticonChat, bValue: true);
						m_UserEmoticonChat.SetData(m_ChannelUid, data);
					}
					else
					{
						NKCUtil.SetGameobjectActive(m_UserCommentChat, bValue: true);
						m_UserCommentChat.SetData(m_ChannelUid, data);
					}
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_UserTextChat, bValue: true);
				m_UserTextChat.SetData(m_ChannelUid, data, disableTranslate);
			}
			m_LayoutGroup.padding = new RectOffset(USER_CHAT_PADDING_LEFT, USER_CHAT_PADDING_RIGHT, USER_CHAT_PADDING_TOP, USER_CHAT_PADDING_BOTTOM);
			m_LayoutGroup.childAlignment = USER_CHAT_ANCHOR;
			break;
		case ChatMessageType.SystemLevelUp:
			NKCUtil.SetGameobjectActive(m_EventChat, bValue: true);
			m_EventChat.SetData(data);
			m_LayoutGroup.padding = new RectOffset(LEVEL_UP_CHAT_PADDING_LEFT, LEVEL_UP_CHAT_PADDING_RIGHT, LEVEL_UP_CHAT_PADDING_TOP, LEVEL_UP_CHAT_PADDING_BOTTOM);
			m_LayoutGroup.childAlignment = LEVEL_UP_CHAT_ANCHOR;
			break;
		case ChatMessageType.System:
		case ChatMessageType.SystemJoin:
		case ChatMessageType.SystemExit:
		case ChatMessageType.SystemBan:
		case ChatMessageType.SystemPromotion:
		case ChatMessageType.SystemMasterMigration:
		case ChatMessageType.SystemNotice:
		case ChatMessageType.SystemNoticeDungeon:
		case ChatMessageType.SystemRename:
			NKCUtil.SetGameobjectActive(m_SystemChat, bValue: true);
			m_SystemChat.SetData(data);
			m_LayoutGroup.padding = new RectOffset(SYSTEM_CHAT_PADDING_LEFT, SYSTEM_CHAT_PADDING_RIGHT, SYSTEM_CHAT_PADDING_TOP, SYSTEM_CHAT_PADDING_BOTTOM);
			m_LayoutGroup.childAlignment = SYSTEM_CHAT_ANCHOR;
			break;
		default:
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			break;
		}
	}

	public void PlaySDAni()
	{
		if (m_ChatMessageType == ChatMessageType.Normal)
		{
			if (m_MyEmoticonChat.gameObject.activeSelf)
			{
				m_MyEmoticonChat.PlaySDAni();
			}
			else if (m_UserEmoticonChat.gameObject.activeSelf)
			{
				m_UserEmoticonChat.PlaySDAni();
			}
		}
	}
}
