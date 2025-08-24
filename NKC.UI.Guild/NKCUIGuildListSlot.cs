using System.Collections.Generic;
using ClientPacket.Guild;
using NKM.Guild;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCUIGuildListSlot : MonoBehaviour
{
	public delegate void OnSelectedSlot(GuildListData guildData);

	public NKCUIComStateButton m_btnSlot;

	public NKCUIGuildBadge m_Badge;

	public Text m_lbGuildLevel;

	public Text m_lbGuildName;

	public Text m_lbGuildMasterName;

	public Text m_lbGuildMemberCount;

	public Text m_lbGuildDesc;

	public NKCUIComStateButton m_btnJoin;

	public Text m_lbBtnText;

	public NKCUIComStateButton m_btnCancel;

	public List<GameObject> m_lstInviteOnly = new List<GameObject>();

	private GuildListData m_GuildData;

	private OnSelectedSlot m_dOnSelectedSlot;

	private NKCAssetInstanceData m_instance;

	private NKCUIGuildJoin.GuildJoinUIType m_GuildJoinUIType;

	private void OnDestroy()
	{
		NKCAssetResourceManager.CloseInstance(m_instance);
	}

	public static NKCUIGuildListSlot GetNewInstance(Transform parent, OnSelectedSlot selectedSlot = null)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_CONSORTIUM", "NKM_UI_CONSORTIUM_LIST_SLOT");
		NKCUIGuildListSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIGuildListSlot>();
		if (component == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCUIGuildListSlot Prefab null!");
			return null;
		}
		component.m_instance = nKCAssetInstanceData;
		component.SetOnSelectedSlot(selectedSlot);
		component.m_btnSlot.PointerClick.RemoveAllListeners();
		component.m_btnSlot.PointerClick.AddListener(component.OnClickSlot);
		component.m_btnJoin.PointerClick.RemoveAllListeners();
		component.m_btnJoin.PointerClick.AddListener(component.OnClickJoinBtn);
		component.m_btnCancel.PointerClick.RemoveAllListeners();
		component.m_btnCancel.PointerClick.AddListener(component.OnClickCancel);
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.gameObject.SetActive(value: false);
		return component;
	}

	private void SetOnSelectedSlot(OnSelectedSlot selectedSlot)
	{
		m_dOnSelectedSlot = selectedSlot;
	}

	public void SetData(GuildListData guildData, NKCUIGuildJoin.GuildJoinUIType guildJoinUIType)
	{
		m_GuildData = guildData;
		if (guildData == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		m_GuildJoinUIType = guildJoinUIType;
		for (int i = 0; i < m_lstInviteOnly.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstInviteOnly[i], bValue: false);
		}
		m_Badge.SetData(guildData.badgeId);
		NKCUtil.SetLabelText(m_lbGuildLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, guildData.guildLevel));
		NKCUtil.SetLabelText(m_lbGuildName, guildData.name);
		NKCUtil.SetLabelText(m_lbGuildMasterName, guildData.masterNickname);
		NKCUtil.SetLabelText(m_lbGuildMemberCount, $"{guildData.memberCount}/{NKMTempletContainer<GuildExpTemplet>.Find(guildData.guildLevel).MaxMemberCount}");
		if (m_GuildJoinUIType == NKCUIGuildJoin.GuildJoinUIType.Search)
		{
			switch (guildData.guildJoinType)
			{
			case GuildJoinType.DirectJoin:
				NKCUtil.SetLabelText(m_lbBtnText, NKCUtilString.GET_STRING_CONSORTIUM_CREATE_JOIN_METHOD_RIGHTOFF_DESC);
				break;
			case GuildJoinType.NeedApproval:
				NKCUtil.SetLabelText(m_lbBtnText, NKCUtilString.GET_STRING_CONSORTIUM_CREATE_JOIN_METHOD_CONFIRM_DESC);
				break;
			case GuildJoinType.Closed:
				NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
				break;
			}
		}
		else if (m_GuildJoinUIType == NKCUIGuildJoin.GuildJoinUIType.Requested)
		{
			NKCUtil.SetLabelText(m_lbBtnText, NKCUtilString.GET_STRING_FRIEND_REQ_CANCEL);
		}
		else if (m_GuildJoinUIType == NKCUIGuildJoin.GuildJoinUIType.Invited)
		{
			NKCUtil.SetLabelText(m_lbBtnText, NKCUtilString.GET_STRING_CONSORTIUM_CREATE_JOIN_METHOD_RIGHTOFF_DESC);
			NKCUtil.SetGameobjectActive(m_btnCancel, bValue: true);
		}
	}

	public void OnClickSlot()
	{
		m_dOnSelectedSlot?.Invoke(m_GuildData);
	}

	private void OnClickJoinBtn()
	{
		switch (m_GuildJoinUIType)
		{
		case NKCUIGuildJoin.GuildJoinUIType.Invited:
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_CONSORTIUM_JOIN_INVITE_JOIN_AGREE_POPUP_TITLE_DESC, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_JOIN_RIGHTOFF_JOIN_POPUP_BODY_DESC, m_GuildData.name), delegate
			{
				NKCPacketSender.Send_NKMPacket_GUILD_ACCEPT_INVITE_REQ(m_GuildData.guildUid, bAllow: true);
			});
			break;
		case NKCUIGuildJoin.GuildJoinUIType.Requested:
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_CONSORTIUM_JOIN_CONFIRM_JOIN_CANCEL_POPUP_TITLE_DESC, string.Format(NKCUtilString.GET_CONSORTIUM_JOIN_CONFIRM_JOIN_CANCEL_POPUP_BODY_DESC, m_GuildData.name), delegate
			{
				NKCPacketSender.Send_NKMPacket_GUILD_CANCEL_JOIN_REQ(m_GuildData.guildUid);
			});
			break;
		case NKCUIGuildJoin.GuildJoinUIType.Search:
		{
			string oKButtonStr = string.Empty;
			switch (m_GuildData.guildJoinType)
			{
			case GuildJoinType.DirectJoin:
				oKButtonStr = NKCUtilString.GET_CONSORTIUM_JOIN_RIGHTOFF_JOIN_POPUP_APPROVE_BTN_DESC;
				break;
			case GuildJoinType.NeedApproval:
				oKButtonStr = NKCUtilString.GET_CONSORTIUM_JOIN_CONFIRM_JOIN_POPUP_APPROVE_BTN_DESC;
				break;
			}
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_CONSORTIUM_JOIN_CONFIRM_JOIN_POPUP_TITLE_DESC, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_JOIN_RIGHTOFF_JOIN_POPUP_BODY_DESC, m_GuildData.name), delegate
			{
				NKCGuildManager.Send_GUILD_JOIN_REQ(m_GuildData.guildUid, m_GuildData.name, m_GuildData.guildJoinType);
			}, null, oKButtonStr);
			break;
		}
		}
	}

	private void OnClickCancel()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_CONSORTIUM_INVITE_JOIN_REJECT_POPUP_TITLE_DESC, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_INVITE_JOIN_REJECT_POPUP_BODY_DESC, m_GuildData.name), delegate
		{
			NKCPacketSender.Send_NKMPacket_GUILD_ACCEPT_INVITE_REQ(m_GuildData.guildUid, bAllow: false);
		});
	}
}
