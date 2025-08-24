using System;
using ClientPacket.Common;
using ClientPacket.Guild;
using NKC.UI.Component;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupChatSlotEmoticon : MonoBehaviour
{
	public Text m_lbName;

	public NKCUISlotProfile m_slot;

	public Image m_imgLeader;

	public Text m_lbTime;

	public NKCPopupEmoticonSlotSD m_slotEmoticon;

	public NKCGameHudEmoticonComment m_slotComment;

	public NKCUIComTitlePanel m_titlePanel;

	public void SetData(long channelUid, NKMChatMessageData data)
	{
		NKMEmoticonTemplet nKMEmoticonTemplet = NKMEmoticonTemplet.Find(data.emotionId);
		if (nKMEmoticonTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_slotEmoticon, nKMEmoticonTemplet.m_EmoticonType == NKM_EMOTICON_TYPE.NET_ANI);
		NKCUtil.SetGameobjectActive(m_slotComment, nKMEmoticonTemplet.m_EmoticonType == NKM_EMOTICON_TYPE.NET_TEXT);
		if (nKMEmoticonTemplet.m_EmoticonType == NKM_EMOTICON_TYPE.NET_ANI)
		{
			if (m_slotEmoticon != null)
			{
				m_slotEmoticon.SetClickEvent(OnClickEmoticon);
				m_slotEmoticon.SetClickEventForChange(null);
				m_slotEmoticon.SetSelected(bSet: false);
				m_slotEmoticon.SetSelectedWithChangeButton(bSet: false);
				m_slotEmoticon.SetUI(data.emotionId);
			}
		}
		else if (m_slotComment != null)
		{
			m_slotComment.SetEnableBtn(bSet: false);
			m_slotComment.Play(data.emotionId);
		}
		NKMGuildMemberData nKMGuildMemberData = NKCGuildManager.MyGuildData?.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == data.commonProfile.userUid);
		if (NKCGuildManager.HasGuild() && NKCGuildManager.MyData.guildUid == channelUid && nKMGuildMemberData != null)
		{
			switch (nKMGuildMemberData.grade)
			{
			case GuildMemberGrade.Master:
				NKCUtil.SetGameobjectActive(m_imgLeader, bValue: true);
				NKCUtil.SetImageSprite(m_imgLeader, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_consortium_sprite", "AB_UI_NKM_UI_CONSORTIUM_ICON_LEADER"));
				break;
			case GuildMemberGrade.Staff:
				NKCUtil.SetGameobjectActive(m_imgLeader, bValue: true);
				NKCUtil.SetImageSprite(m_imgLeader, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_consortium_sprite", "AB_UI_NKM_UI_CONSORTIUM_ICON_OFFICER"));
				break;
			case GuildMemberGrade.Member:
				NKCUtil.SetGameobjectActive(m_imgLeader, bValue: false);
				break;
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_imgLeader, bValue: false);
		}
		NKCUtil.SetLabelText(m_lbName, data.commonProfile.nickname);
		m_slot.SetProfiledata(data.commonProfile, null);
		DateTime systemLocalTime = NKCSynchronizedTime.GetSystemLocalTime(data.createdAt, NKMTime.INTERVAL_FROM_UTC);
		NKCUtil.SetLabelText(m_lbTime, systemLocalTime.ToString());
		m_titlePanel?.SetData(data.commonProfile);
	}

	public void PlaySDAni()
	{
		m_slotEmoticon.PlaySDAni();
	}

	private void OnClickEmoticon(NKCUISlot.SlotData slotData, bool bLocked)
	{
		m_slotEmoticon.PlaySDAni();
	}
}
