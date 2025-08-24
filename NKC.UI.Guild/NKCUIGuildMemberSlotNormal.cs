using System;
using ClientPacket.Guild;
using Cs.Core.Util;
using NKC.UI.Component;
using NKC.UI.Office;
using NKM;
using NKM.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCUIGuildMemberSlotNormal : MonoBehaviour
{
	public NKCUISlotProfile m_slot;

	public Image m_imgLeader;

	public Text m_lbLevel;

	public Text m_lbName;

	public GameObject m_objPointParent;

	public Text m_lbWeeklyPoint;

	public Text m_lbTotalPoint;

	public Text m_lbComment;

	public Image m_imgState;

	public Text m_lbState;

	public GameObject m_objMyGuildOnly;

	public GameObject m_objAttendanceDone;

	public GameObject m_objMissionDone;

	public GameObject m_objMySlot;

	public GameObject m_objRedDot;

	public NKCUIComStateButton m_csbtnDormitory;

	public NKCUIComTitlePanel m_titlePanel;

	private long m_lUserUId;

	private bool m_bHasOffice;

	public void InitUI()
	{
		m_lUserUId = 0L;
		m_bHasOffice = false;
		NKCUtil.SetButtonClickDelegate(m_csbtnDormitory, OnClickDormitory);
	}

	public void SetData(NKMGuildMemberData guildMemberData, bool bIsMyGuild)
	{
		m_slot.Init();
		m_slot.SetProfiledata(guildMemberData.commonProfile, null);
		switch (guildMemberData.grade)
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
		NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, guildMemberData.commonProfile.level));
		NKCUtil.SetLabelText(m_lbName, guildMemberData.commonProfile.nickname);
		NKCUtil.SetGameobjectActive(m_objPointParent, bValue: true);
		NKCUtil.SetLabelText(m_lbWeeklyPoint, guildMemberData.weeklyContributionPoint.ToString());
		NKCUtil.SetLabelText(m_lbTotalPoint, guildMemberData.totalContributionPoint.ToString());
		NKCUtil.SetLabelText(m_lbComment, guildMemberData.greeting);
		TimeSpan timeSpan = NKCSynchronizedTime.GetServerUTCTime() - guildMemberData.lastOnlineTime;
		NKCUtil.SetLabelText(m_lbState, NKCUtilString.GetLastTimeString(guildMemberData.lastOnlineTime));
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
		NKCUtil.SetGameobjectActive(m_objMyGuildOnly, bIsMyGuild);
		if (bIsMyGuild)
		{
			NKCUtil.SetGameobjectActive(m_objAttendanceDone, guildMemberData.HasAttendanceData(ServiceTime.Recent));
			NKCUtil.SetGameobjectActive(m_objMissionDone, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objMySlot, guildMemberData.commonProfile.userUid == NKCScenManager.CurrentUserData().m_UserUID);
		NKCUtil.SetGameobjectActive(m_objRedDot, bValue: false);
		m_lUserUId = guildMemberData.commonProfile.userUid;
		m_bHasOffice = guildMemberData.hasOffice;
		m_titlePanel?.SetData(guildMemberData.commonProfile);
	}

	private void OnClickDormitory()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null && nKMUserData.m_UserUID == m_lUserUId)
		{
			NKCUIOfficeMapFront.ReserveScenID = NKCScenManager.GetScenManager().GetNowScenID();
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_LOBBY().SetReserveLobbyTab(NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE.Member);
			NKCScenManager.GetScenManager()?.ScenChangeFade(NKM_SCEN_ID.NSI_OFFICE, bForce: false);
		}
		else if (m_bHasOffice)
		{
			NKCPacketSender.Send_NKMPacket_OFFICE_STATE_REQ(m_lUserUId);
		}
		else
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_OFFICE_FRIEND_CANNOT_VISIT, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
	}
}
