using System.Collections.Generic;
using ClientPacket.Common;
using NKC.UI.Guild;
using TMPro;

namespace NKC.UI;

public class NKCPopupGuildChat : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM";

	private const string UI_ASSET_NAME = "NKM_UI_CONSORTIUM_CHAT";

	private static NKCPopupGuildChat m_Instance;

	public NKCUIComChat m_Chat;

	public NKCUIGuildBadge m_GuildBadgeUI;

	public NKCUIComToggle m_tglNotice;

	public TMP_Text m_lbNotice;

	private long m_CurChannelUid;

	public static NKCPopupGuildChat Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGuildChat>("AB_UI_NKM_UI_CONSORTIUM", "NKM_UI_CONSORTIUM_CHAT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupGuildChat>();
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
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void InitUI()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_Chat.InitUI(OnSendMessage, OnCloseChat, bDisableTranslate: false);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_tglNotice.OnValueChanged.RemoveAllListeners();
		m_tglNotice.OnValueChanged.AddListener(OnClickNotice);
	}

	private void OnSendMessage(long channelUid, ChatMessageType messageType, string message, int emotionId)
	{
		NKCPacketSender.Send_NKMPacket_GUILD_CHAT_REQ(channelUid, messageType, message, emotionId);
	}

	private void OnCloseChat()
	{
		Close();
	}

	public void Open(long defaultChannel = 0L)
	{
		m_CurChannelUid = defaultChannel;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (NKCGuildManager.HasGuild())
		{
			NKCUtil.SetGameobjectActive(m_GuildBadgeUI, bValue: true);
			m_GuildBadgeUI.SetData(NKCGuildManager.MyGuildData.badgeId);
			m_Chat.SetData(m_CurChannelUid, bEnableMute: true, NKCGuildManager.MyGuildData.name);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_GuildBadgeUI, bValue: false);
			m_Chat.SetData(m_CurChannelUid, bEnableMute: true);
		}
		m_tglNotice.Select(NKCGuildManager.m_bShowChatNotice, bForce: true);
		NKCUtil.SetLabelText(m_lbNotice, NKCGuildManager.GetNotice());
		UIOpened();
	}

	public void AddMessage(NKMChatMessageData data)
	{
		m_Chat.AddMessage(data, bIsMyMessage: true);
	}

	public void RefreshList(bool bResetPosition = false)
	{
		m_Chat.RefreshList(bResetPosition);
	}

	public void OnChatDataReceived(long channelUid, List<NKMChatMessageData> lstData, bool bRefresh = false)
	{
		m_Chat.OnChatDataReceived(channelUid, lstData, bRefresh);
	}

	public void CheckMute()
	{
		m_Chat.CheckMute();
	}

	private void OnClickNotice(bool bValue)
	{
		NKCGuildManager.ShowChatNotice(bValue);
	}

	public override void OnGuildDataChanged()
	{
		base.OnGuildDataChanged();
		if (!NKCGuildManager.HasGuild())
		{
			Close();
		}
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
