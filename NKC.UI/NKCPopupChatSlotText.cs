using System;
using ClientPacket.Common;
using ClientPacket.Guild;
using DG.Tweening;
using NKC.Publisher;
using NKC.UI.Component;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupChatSlotText : MonoBehaviour
{
	public NKCUIComStateButton m_btnText;

	public Text m_lbName;

	public NKCUISlotProfile m_slot;

	public Image m_imgLeader;

	public Text m_lbMessage;

	public NKCUIComStateButton m_btnReport;

	public Text m_lbTime;

	public Image m_imgBubble;

	public NKCUIComTitlePanel m_TitlePanel;

	[Header("번역 관련")]
	public VerticalLayoutGroup m_LayoutGroup;

	public NKCUIComStateButton m_btnTranslate;

	public GameObject m_objTranslateLine;

	public Text m_lbTranslated;

	public GameObject m_objTranslateProgress;

	public GameObject m_objTranslateComplete;

	[Header("채팅 배경 색")]
	public string PRIVATE_CHAT_MY_BG_COLOR = "#3394FF";

	public string PRIVATE_CHAT_TARGET_BG_COLOR = "#FFFFFF";

	public string GUILD_CHAT_LEADER_BG_COLOR = "#FFA21D";

	public string GUILD_CHAT_MY_BG_COLOR = "#3394FF";

	public string GUILD_CHAT_OTHER_BG_COLOR = "#EAEFF3";

	private long m_ChannelUid;

	private NKMChatMessageData m_NKMChatMessageData;

	private bool m_bDisableTranslate;

	private int DEFAULT_LEFT_PADDING = 30;

	private int TRANSLATE_LEFT_PADDING = 121;

	private bool m_bPressed;

	private float m_fDeltaTime;

	public void SetData(long channelUid, NKMChatMessageData data, bool disableTranslate = false)
	{
		m_ChannelUid = channelUid;
		m_NKMChatMessageData = data;
		m_bDisableTranslate = disableTranslate;
		string translatedMessage = NKCChatManager.GetTranslatedMessage(m_NKMChatMessageData.messageUid);
		if (m_btnReport != null)
		{
			m_btnReport.PointerClick.RemoveAllListeners();
			m_btnReport.PointerClick.AddListener(OnClickReport);
			NKCUtil.SetGameobjectActive(m_btnReport, data.commonProfile.userUid != NKCScenManager.CurrentUserData().m_UserUID);
		}
		if (m_btnText != null)
		{
			m_btnText.PointerDown.RemoveAllListeners();
			m_btnText.PointerDown.AddListener(OnTextDown);
			m_btnText.PointerUp.RemoveAllListeners();
			m_btnText.PointerUp.AddListener(OnTextUp);
		}
		if (NKCPublisherModule.Localization.UseTranslation && !disableTranslate)
		{
			if (m_btnTranslate != null)
			{
				m_btnTranslate.PointerClick.RemoveAllListeners();
				m_btnTranslate.PointerClick.AddListener(OnClickTranslate);
				NKCUtil.SetGameobjectActive(m_btnTranslate, string.IsNullOrEmpty(translatedMessage));
				if (m_LayoutGroup != null)
				{
					if (m_btnTranslate.gameObject.activeSelf)
					{
						m_LayoutGroup.padding.left = TRANSLATE_LEFT_PADDING;
					}
					else
					{
						m_LayoutGroup.padding.left = DEFAULT_LEFT_PADDING;
					}
				}
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_btnTranslate, bValue: false);
			if (m_LayoutGroup != null)
			{
				m_LayoutGroup.padding.left = DEFAULT_LEFT_PADDING;
			}
		}
		NKMGuildMemberData nKMGuildMemberData = NKCGuildManager.MyGuildData?.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == data.commonProfile.userUid);
		if (NKCGuildManager.HasGuild() && NKCGuildManager.MyData.guildUid == channelUid && nKMGuildMemberData != null)
		{
			switch (nKMGuildMemberData.grade)
			{
			case GuildMemberGrade.Master:
				NKCUtil.SetGameobjectActive(m_imgLeader, bValue: true);
				NKCUtil.SetImageSprite(m_imgLeader, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_consortium_sprite", "AB_UI_NKM_UI_CONSORTIUM_ICON_LEADER"));
				NKCUtil.SetImageColor(m_imgBubble, NKCUtil.GetColor(GUILD_CHAT_LEADER_BG_COLOR));
				break;
			case GuildMemberGrade.Staff:
				NKCUtil.SetGameobjectActive(m_imgLeader, bValue: true);
				NKCUtil.SetImageSprite(m_imgLeader, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_consortium_sprite", "AB_UI_NKM_UI_CONSORTIUM_ICON_OFFICER"));
				if (data.commonProfile.userUid == NKCScenManager.CurrentUserData().m_UserUID)
				{
					NKCUtil.SetImageColor(m_imgBubble, NKCUtil.GetColor(GUILD_CHAT_MY_BG_COLOR));
				}
				else
				{
					NKCUtil.SetImageColor(m_imgBubble, NKCUtil.GetColor(GUILD_CHAT_OTHER_BG_COLOR));
				}
				break;
			case GuildMemberGrade.Member:
				NKCUtil.SetGameobjectActive(m_imgLeader, bValue: false);
				if (data.commonProfile.userUid == NKCScenManager.CurrentUserData().m_UserUID)
				{
					NKCUtil.SetImageColor(m_imgBubble, NKCUtil.GetColor(GUILD_CHAT_MY_BG_COLOR));
				}
				else
				{
					NKCUtil.SetImageColor(m_imgBubble, NKCUtil.GetColor(GUILD_CHAT_OTHER_BG_COLOR));
				}
				break;
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_imgLeader, bValue: false);
			if (data.commonProfile.userUid == NKCScenManager.CurrentUserData().m_UserUID)
			{
				NKCUtil.SetImageColor(m_imgBubble, NKCUtil.GetColor(PRIVATE_CHAT_MY_BG_COLOR));
			}
			else
			{
				NKCUtil.SetImageColor(m_imgBubble, NKCUtil.GetColor(PRIVATE_CHAT_TARGET_BG_COLOR));
			}
		}
		NKCUtil.SetLabelText(m_lbName, data.commonProfile.nickname);
		m_slot.SetProfiledata(data.commonProfile, null);
		if (data.blocked)
		{
			NKCUtil.SetLabelText(m_lbMessage, NKCUtilString.GET_STRING_CONSORTIUM_CHAT_ACCUMELATED_RECEIPT_REPORT_TEXT);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbMessage, data.message);
		}
		NKCUtil.SetGameobjectActive(m_objTranslateProgress, bValue: false);
		if (!NKCPublisherModule.Localization.UseTranslation || string.IsNullOrEmpty(translatedMessage))
		{
			NKCUtil.SetGameobjectActive(m_objTranslateLine, bValue: false);
			NKCUtil.SetGameobjectActive(m_lbTranslated, bValue: false);
			NKCUtil.SetGameobjectActive(m_objTranslateComplete, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objTranslateLine, bValue: true);
			NKCUtil.SetGameobjectActive(m_lbTranslated, bValue: true);
			NKCUtil.SetGameobjectActive(m_objTranslateComplete, bValue: true);
			NKCUtil.SetLabelText(m_lbTranslated, translatedMessage);
		}
		DateTime systemLocalTime = NKCSynchronizedTime.GetSystemLocalTime(data.createdAt, NKMTime.INTERVAL_FROM_UTC);
		NKCUtil.SetLabelText(m_lbTime, systemLocalTime.ToString());
		m_TitlePanel?.SetData(data.commonProfile);
	}

	private void OnTextDown(PointerEventData eventData)
	{
		m_fDeltaTime = 0f;
		m_bPressed = true;
		m_imgBubble.DOColor(Color.gray, 1f);
	}

	private void OnTextUp()
	{
		m_imgBubble.DOKill();
		m_bPressed = false;
		m_btnText.Select(bSelect: false, bForce: true, bImmediate: true);
		SetData(m_ChannelUid, m_NKMChatMessageData, m_bDisableTranslate);
	}

	private void OnClickReport()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_CONSORTIUM_CHAT_REPORT_POPUP_TITLE_DESC, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_CHAT_REPORT_POPUP_BODY_DESC, m_NKMChatMessageData.commonProfile.nickname), OnConfirmReport);
	}

	private void OnConfirmReport()
	{
		NKCPacketSender.Send_NKMPacket_GUILD_CHAT_COMPLAIN_REQ(m_ChannelUid, m_NKMChatMessageData.messageUid);
	}

	private void OnClickTranslate()
	{
		NKCUtil.SetGameobjectActive(m_objTranslateLine, bValue: true);
		NKCUtil.SetGameobjectActive(m_objTranslateProgress, bValue: true);
		NKCUtil.SetGameobjectActive(m_btnTranslate, bValue: false);
		if (m_LayoutGroup != null)
		{
			m_LayoutGroup.padding.left = DEFAULT_LEFT_PADDING;
		}
		NKCPublisherModule.Localization.Translate(m_NKMChatMessageData.messageUid, m_NKMChatMessageData.message, NKCPublisherModule.Localization.GetDefaultLanguage(), NKCChatManager.OnRecv);
	}

	private void Update()
	{
		if (m_bPressed)
		{
			m_fDeltaTime += Time.deltaTime;
			if (m_fDeltaTime > 1f)
			{
				OnTextUp();
				GUIUtility.systemCopyBuffer = m_lbMessage.text;
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_CHAT_COPY_COMPLETE);
			}
		}
	}
}
