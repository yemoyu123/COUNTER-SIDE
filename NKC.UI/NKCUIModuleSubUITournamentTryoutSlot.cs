using System;
using ClientPacket.Common;
using ClientPacket.Game;
using NKC.UI.Component;
using NKC.UI.Event;
using NKC.UI.Guild;
using NKM;
using UnityEngine;

namespace NKC.UI;

public class NKCUIModuleSubUITournamentTryoutSlot : MonoBehaviour
{
	public delegate void OnResult(int idx);

	[Header("\ufffd\ufffd\ufffdƮ \ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_objWin;

	public GameObject m_objWinNonLoop;

	public GameObject m_objLose;

	public GameObject m_objLoseNonLoop;

	public NKCComTMPUIText m_lbRound;

	public NKCComTMPUIText m_lbTime;

	public NKCUIComStateButton m_btnResult;

	public NKCUIComStateButton m_btnDetail;

	[Header("\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_objBlind;

	public GameObject m_objUserInfo;

	public GameObject m_objWalkOver;

	public NKCUISlotTitle m_slotTitle;

	public NKCUISlotProfile m_slotProfile;

	public GameObject m_objGuildInfo;

	public NKCUIGuildBadge m_GuildBadge;

	public GameObject m_objGuildName;

	public NKCComTMPUIText m_lbGuildName;

	public NKCComTMPUIText m_lbName;

	public NKCComTMPUIText m_lbLevel;

	private OnResult m_dOnClickResult;

	private NKMTournamentPlayInfo m_History;

	private int m_Index;

	public int GetIndex()
	{
		return m_Index;
	}

	public void SetData(NKMTournamentPlayInfo history, int round, bool bShowResult, bool bShowFx, OnResult dOnClickResult)
	{
		m_btnResult.PointerClick.RemoveAllListeners();
		m_btnResult.PointerClick.AddListener(OnClickResult);
		m_btnDetail.PointerClick.RemoveAllListeners();
		m_btnDetail.PointerClick.AddListener(OnClickDetail);
		m_dOnClickResult = dOnClickResult;
		m_Index = round - 1;
		m_History = history;
		NKCUtil.SetLabelText(m_lbRound, round.ToString());
		DateTime dateTime = new DateTime(history.history.RegdateTick).ToLocalTime();
		NKCUtil.SetLabelText(m_lbTime, string.Format(NKCUtilString.GET_STRING_DATE_FOUR_PARAM, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute.ToString("#00")));
		NKCUtil.SetGameobjectActive(m_objBlind, !bShowResult);
		if (history.history.TargetUserLevel < 0)
		{
			NKCUtil.SetGameobjectActive(m_objUserInfo, bValue: false);
			NKCUtil.SetGameobjectActive(m_objWalkOver, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnDetail, bValue: false);
			NKCUtil.SetGameobjectActive(m_btnResult, !bShowResult);
			history.history.Result = PVP_RESULT.WIN;
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objUserInfo, bValue: true);
			NKCUtil.SetGameobjectActive(m_objWalkOver, bValue: false);
			if (history.history.TargetTitleId > 0)
			{
				NKCUtil.SetGameobjectActive(m_slotTitle, bValue: true);
				m_slotTitle.SetData(history.history.TargetTitleId, showEmpty: true, showLock: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_slotTitle, bValue: false);
			}
			m_slotProfile.SetProfiledata(history.profile, null);
			if (history.history.TargetGuildBadgeId > 0)
			{
				NKCUtil.SetGameobjectActive(m_objGuildInfo, bValue: true);
				m_GuildBadge.SetData(history.history.TargetGuildBadgeId);
				NKCUtil.SetLabelText(m_lbGuildName, history.history.TargetGuildName);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objGuildInfo, bValue: false);
			}
			NKCUtil.SetLabelText(m_lbName, history.history.TargetNickName);
			NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, history.history.TargetUserLevel));
			NKCUtil.SetGameobjectActive(m_btnDetail, bShowResult);
			NKCUtil.SetGameobjectActive(m_btnResult, !bShowResult);
		}
		if (bShowResult)
		{
			ShowResult(history.history.Result == PVP_RESULT.WIN, bShowFx);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objWin, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLose, bValue: false);
	}

	public void ShowResult(bool bWin, bool bShowFx)
	{
		NKCUtil.SetGameobjectActive(m_btnResult, bValue: false);
		NKCUtil.SetGameobjectActive(m_btnDetail, m_History.history.TargetUserLevel >= 0);
		NKCUtil.SetGameobjectActive(m_objWinNonLoop, bWin && bShowFx);
		NKCUtil.SetGameobjectActive(m_objWin, bWin);
		NKCUtil.SetGameobjectActive(m_objLoseNonLoop, !bWin && bShowFx);
		NKCUtil.SetGameobjectActive(m_objLose, !bWin);
	}

	private void OnClickResult()
	{
		ShowResult(m_History.history.Result == PVP_RESULT.WIN, bShowFx: true);
		m_dOnClickResult?.Invoke(m_Index);
	}

	private void OnClickDetail()
	{
		if (m_History.profile != null)
		{
			NKMTournamentProfileData nKMTournamentProfileData = new NKMTournamentProfileData();
			nKMTournamentProfileData.deck = m_History.history.MyDeckData;
			nKMTournamentProfileData.guildData = NKCScenManager.CurrentUserData().UserProfileData.guildData;
			nKMTournamentProfileData.commonProfile = NKCScenManager.CurrentUserData().UserProfileData.commonProfile;
			NKMTournamentProfileData nKMTournamentProfileData2 = new NKMTournamentProfileData();
			nKMTournamentProfileData2.deck = m_History.history.TargetDeckData;
			nKMTournamentProfileData2.guildData = new NKMGuildSimpleData();
			nKMTournamentProfileData2.guildData.guildName = m_History.history.TargetGuildName;
			nKMTournamentProfileData2.guildData.badgeId = m_History.history.TargetGuildBadgeId;
			nKMTournamentProfileData2.guildData.guildUid = m_History.history.TargetGuildUid;
			nKMTournamentProfileData2.commonProfile = m_History.profile;
			NKCUITournamentPlayoff.GetInstanceGauntletAsyncReady().Open(m_Index, NKMTournamentGroups.None, nKMTournamentProfileData, nKMTournamentProfileData2, NKM_GAME_TYPE.NGT_PVE_SIMULATED, replayActive: true);
		}
	}
}
