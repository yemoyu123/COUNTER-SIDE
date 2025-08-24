using System;
using ClientPacket.Common;
using ClientPacket.Guild;
using Cs.Core.Util;
using NKC.UI.Friend;
using NKM;
using NKM.Guild;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildUserInfo : MonoBehaviour
{
	[Header("길드 관련")]
	public GameObject m_objState;

	public Image m_imgState;

	public Text m_lbState;

	public GameObject m_objPoint;

	public Text m_lbWeeklyPoint;

	public Text m_lbTotalPoint;

	public GameObject m_objButtons;

	public NKCUIComStateButton m_btnChangeGrade;

	public Text m_lbChangeGrade;

	public NKCUIComStateButton m_btnGiveMaster;

	public NKCUIComStateButton m_btnExit;

	public NKCUIComStateButton m_btnBan;

	public NKCUIComStateButton m_btnChat;

	public GameObject m_objChecklist;

	public GameObject m_objCheckAttendance;

	public GameObject m_objCheckMission;

	private bool m_bGradeUp = true;

	private string m_TargetUserName = "";

	private long m_TargetUserUid;

	public void InitUI()
	{
		m_btnChangeGrade.PointerClick.RemoveAllListeners();
		m_btnChangeGrade.PointerClick.AddListener(OnClickChangeGrade);
		m_btnGiveMaster.PointerClick.RemoveAllListeners();
		m_btnGiveMaster.PointerClick.AddListener(OnClickGiveMaster);
		m_btnExit.PointerClick.RemoveAllListeners();
		m_btnExit.PointerClick.AddListener(OnClickExit);
		m_btnBan.PointerClick.RemoveAllListeners();
		m_btnBan.PointerClick.AddListener(OnClickBan);
		m_btnChat.PointerClick.RemoveAllListeners();
		m_btnChat.PointerClick.AddListener(OnClickChat);
		m_btnChat.m_bGetCallbackWhileLocked = true;
	}

	public void SetData(NKMUserProfileData cNKMUserProfileData)
	{
		NKMGuildMemberData myData = NKCGuildManager.MyGuildData?.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == NKCScenManager.CurrentUserData().m_UserUID);
		NKMGuildMemberData userData = NKCGuildManager.MyGuildData?.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == cNKMUserProfileData.commonProfile.userUid);
		m_TargetUserName = cNKMUserProfileData.commonProfile.nickname;
		m_TargetUserUid = cNKMUserProfileData.commonProfile.userUid;
		SetButtons(myData, userData);
	}

	private void SetButtons(NKMGuildMemberData myData, NKMGuildMemberData userData)
	{
		if (myData != null && userData != null)
		{
			NKCUtil.SetGameobjectActive(m_objState, bValue: true);
			NKCUtil.SetGameobjectActive(m_objButtons, bValue: true);
			NKCUtil.SetGameobjectActive(m_objPoint, bValue: true);
			NKCUtil.SetGameobjectActive(m_objChecklist, bValue: true);
			TimeSpan timeSpan = NKCSynchronizedTime.GetServerUTCTime() - userData.lastOnlineTime;
			NKCUtil.SetLabelText(m_lbState, NKCUtilString.GetLastTimeString(userData.lastOnlineTime));
			if (timeSpan.TotalDays > 3.0)
			{
				NKCUtil.SetImageColor(m_imgState, NKCUtil.GetColor("#de0a0a"));
			}
			else if (timeSpan.TotalDays > 1.0)
			{
				NKCUtil.SetImageColor(m_imgState, NKCUtil.GetColor("#db650e"));
			}
			else
			{
				NKCUtil.SetImageColor(m_imgState, NKCUtil.GetColor("#0aca0a"));
			}
			NKCUtil.SetGameobjectActive(m_btnChangeGrade, myData.grade == GuildMemberGrade.Master && myData.commonProfile.userUid != userData.commonProfile.userUid);
			NKCUtil.SetGameobjectActive(m_btnGiveMaster, myData.grade == GuildMemberGrade.Master && userData.grade == GuildMemberGrade.Staff);
			NKCUtil.SetGameobjectActive(m_btnBan, myData.grade != GuildMemberGrade.Member && myData.grade < userData.grade);
			NKCUtil.SetGameobjectActive(m_btnExit, userData.commonProfile.userUid == NKCScenManager.CurrentUserData().m_UserUID && myData.grade != GuildMemberGrade.Master);
			NKCUtil.SetGameobjectActive(m_btnChat, userData.commonProfile.userUid != NKCScenManager.CurrentUserData().m_UserUID && NKCGuildManager.IsGuildMemberByUID(userData.commonProfile.userUid));
			if (NKCFriendManager.IsBlockedUser(userData.commonProfile.friendCode))
			{
				m_btnChat.Lock();
			}
			else
			{
				m_btnChat.UnLock();
			}
			if (m_btnChangeGrade.gameObject.activeSelf)
			{
				if (userData.grade == GuildMemberGrade.Staff)
				{
					m_bGradeUp = false;
					NKCUtil.SetLabelText(m_lbChangeGrade, NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_GRADE_DOWN);
				}
				else if (userData.grade == GuildMemberGrade.Member)
				{
					m_bGradeUp = true;
					NKCUtil.SetLabelText(m_lbChangeGrade, NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_GRADE_UP);
				}
			}
			NKCUtil.SetGameobjectActive(m_objCheckAttendance, userData.HasAttendanceData(ServiceTime.Recent));
			NKCUtil.SetGameobjectActive(m_objCheckMission, bValue: false);
			NKCUtil.SetLabelText(m_lbWeeklyPoint, userData.weeklyContributionPoint.ToString());
			NKCUtil.SetLabelText(m_lbTotalPoint, userData.totalContributionPoint.ToString());
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objState, bValue: false);
			NKCUtil.SetGameobjectActive(m_objButtons, bValue: false);
			NKCUtil.SetGameobjectActive(m_objPoint, bValue: false);
			NKCUtil.SetGameobjectActive(m_objChecklist, bValue: false);
			NKCUtil.SetGameobjectActive(m_btnChat, bValue: false);
		}
	}

	private void OnClickChangeGrade()
	{
		GuildMemberGrade targetGrade = GuildMemberGrade.Member;
		string title;
		string format;
		if (m_bGradeUp)
		{
			title = NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_GRADE_UP;
			format = NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_GRADE_UP_CONFIRM_POPUP_BODY_DESC;
			targetGrade = GuildMemberGrade.Staff;
		}
		else
		{
			title = NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_GRADE_DOWN;
			format = NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_GRADE_DOWN_CONFIRM_POPUP_BODY_DESC;
			targetGrade = GuildMemberGrade.Member;
		}
		NKCPopupOKCancel.OpenOKCancelBox(title, string.Format(format, m_TargetUserName), delegate
		{
			OnConfirmChangeGrade(targetGrade);
		});
	}

	private void OnConfirmChangeGrade(GuildMemberGrade targetGrade)
	{
		NKCPopupFriendInfo.Instance.Close();
		NKCPacketSender.Send_NKMPacket_GUILD_SET_MEMBER_GRADE_REQ(NKCGuildManager.MyGuildData.guildUid, m_TargetUserUid, targetGrade);
	}

	private void OnClickGiveMaster()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_GRADE_HANDOVER_CONFIRM_POPUP_TITLE_DESC, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_GRADE_HANDOVER_CONFIRM_POPUP_BODY_DESC, m_TargetUserName), OnGiveMaster);
	}

	private void OnGiveMaster()
	{
		NKCPopupFriendInfo.Instance.Close();
		NKCPacketSender.Send_NKMPacket_GUILD_MASTER_SPECIFIED_MIGRATION_REQ(NKCGuildManager.MyData.guildUid, m_TargetUserUid);
	}

	private void OnClickBan()
	{
		NKCPopupGuildKick.Instance.Open(m_TargetUserName, OnBan);
	}

	private void OnBan(int banReason)
	{
		NKCPopupFriendInfo.Instance.Close();
		NKCPacketSender.Send_NKMPacket_GUILD_BAN_REQ(NKCGuildManager.MyGuildData.guildUid, m_TargetUserUid, banReason);
	}

	private void OnClickExit()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_EXIT_CONFIRM_POPUP_TITLE_DESC, NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_EXIT_CONFIRM_POPUP_BODY_DESC, delegate
		{
			NKCPacketSender.Send_NKMPacket_GUILD_EXIT_REQ(NKCGuildManager.MyGuildData.guildUid);
		});
	}

	private void OnClickChat()
	{
		if (NKMOpenTagManager.IsOpened("CHAT_PRIVATE"))
		{
			if (m_btnChat.m_bLock)
			{
				NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_CHAT_BLOCKED, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			}
			else
			{
				if (!NKCGuildManager.IsGuildMemberByUID(m_TargetUserUid))
				{
					return;
				}
				bool bAdmin;
				switch (NKCContentManager.CheckContentStatus(ContentsType.FRIENDS, out bAdmin))
				{
				case NKCContentManager.eContentStatus.Open:
					if (NKCScenManager.GetScenManager().GetGameOptionData().UseChatContent)
					{
						NKCPopupFriendInfo.Instance.Close();
						NKCPacketSender.Send_NKMPacket_PRIVATE_CHAT_LIST_REQ(m_TargetUserUid);
					}
					else
					{
						NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_OPTION_GAME_CHAT_NOTICE);
					}
					break;
				case NKCContentManager.eContentStatus.Lock:
					NKCContentManager.ShowLockedMessagePopup(ContentsType.FRIENDS);
					break;
				}
			}
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_COMING_SOON_SYSTEM);
		}
	}
}
