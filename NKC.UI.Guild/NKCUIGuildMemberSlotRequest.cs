using ClientPacket.Common;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCUIGuildMemberSlotRequest : MonoBehaviour
{
	public NKCUISlot m_slot;

	public Text m_lbLevel;

	public Text m_lbName;

	public Text m_lbFriendCode;

	public Text m_lbLastLoginTime;

	public NKCUIComStateButton m_btnInvite;

	public Text m_lbInviteBtn;

	public NKCUIComStateButton m_btnAccept;

	public NKCUIComStateButton m_btnDenied;

	private long m_UserUid;

	private string m_UserName;

	public void InitUI()
	{
		m_slot.Init();
		m_btnInvite.PointerClick.RemoveAllListeners();
		m_btnInvite.PointerClick.AddListener(OnClickInvite);
		m_btnAccept.PointerClick.RemoveAllListeners();
		m_btnAccept.PointerClick.AddListener(OnClickAccept);
		m_btnDenied.PointerClick.RemoveAllListeners();
		m_btnDenied.PointerClick.AddListener(OnClickDenied);
	}

	public void SetData(FriendListData userData, NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE lobbyUIType)
	{
		m_UserUid = userData.commonProfile.userUid;
		m_UserName = userData.commonProfile.nickname;
		m_slot.SetUnitData(userData.commonProfile.mainUnitId, 1, userData.commonProfile.mainUnitSkinId, bShowName: false, bShowLevel: false, bEnableLayoutElement: false, null);
		m_slot.SetMaxLevelTacticFX(userData.commonProfile.mainUnitTacticLevel == 6);
		NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, userData.commonProfile.level));
		NKCUtil.SetLabelText(m_lbName, userData.commonProfile.nickname);
		NKCUtil.SetLabelText(m_lbFriendCode, $"#{userData.commonProfile.friendCode}");
		NKCUtil.SetLabelText(m_lbLastLoginTime, NKCUtilString.GetLastTimeString(userData.lastLoginDate));
		NKCUtil.SetGameobjectActive(m_btnInvite, lobbyUIType == NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE.Invite);
		NKCUtil.SetGameobjectActive(m_btnAccept, lobbyUIType == NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE.Member);
		NKCUtil.SetGameobjectActive(m_btnDenied, lobbyUIType == NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE.Member);
		if (lobbyUIType == NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE.Invite)
		{
			NKCUtil.SetLabelText(m_lbInviteBtn, NKCUtilString.GET_STRING_CONSORTIUM_INVITE);
			if (NKCGuildManager.MyGuildData != null && NKCGuildManager.MyGuildData.inviteList.Find((FriendListData x) => x.commonProfile.userUid == userData.commonProfile.userUid) != null)
			{
				m_btnInvite.Lock();
			}
			else
			{
				m_btnInvite.UnLock();
			}
		}
	}

	private void OnClickInvite()
	{
		if (NKCGuildManager.MyGuildData.inviteList.Find((FriendListData x) => x.commonProfile.userUid == m_UserUid) == null)
		{
			if (NKCGuildManager.MyGuildData.inviteList.Count == NKMCommonConst.Guild.MaxInviteCount)
			{
				NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_GUILD_MAX_INVITE_COUNT);
				return;
			}
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_CONSORTIUM_POPUP_INVITE_TITLE, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_INVITE_SEND_POPUP_BODY_DESC, m_UserName), delegate
			{
				NKCPacketSender.Send_NKMPacket_GUILD_INVITE_REQ(NKCGuildManager.MyData.guildUid, m_UserUid);
			}, null, NKCUtilString.GET_STRING_CONSORTIUM_INVITE);
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_GUILD_ALREADY_INVITED);
		}
	}

	private void OnClickAccept()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_CONFIRM_JOIN_CONFIRM_POPUP_TITLE_DESC, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_CONFIRM_JOIN_CONFIRM_POPUP_BODY_DESC, m_UserName), delegate
		{
			NKCPacketSender.Send_NKMPacket_GUILD_ACCEPT_JOIN_REQ(NKCGuildManager.MyData.guildUid, m_UserUid, bAllow: true);
		}, null, NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_CONFIRM_JOIN_CONFIRM_POPUP_TITLE_DESC);
	}

	private void OnClickDenied()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_CONFIRM_JOIN_REFUSE_POPUP_TITLE_DESC, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_CONFIRM_JOIN_REFUSE_POPUP_BODY_DESC, m_UserName), delegate
		{
			NKCPacketSender.Send_NKMPacket_GUILD_ACCEPT_JOIN_REQ(NKCGuildManager.MyData.guildUid, m_UserUid, bAllow: false);
		}, null, NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_CONFIRM_JOIN_REFUSE_POPUP_TITLE_DESC, "", bUseRed: true);
	}
}
