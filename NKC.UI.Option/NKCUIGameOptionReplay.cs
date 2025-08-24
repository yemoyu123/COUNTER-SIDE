using ClientPacket.Pvp;
using NKM;
using UnityEngine.UI;

namespace NKC.UI.Option;

public class NKCUIGameOptionReplay : NKCUIGameOptionContentBase
{
	public Text m_NKM_UI_GAME_OPTION_REPLAY_TEXT_TITLE;

	public Text m_NKM_UI_GAME_OPTION_REPLAY_TEXT_SUB_TITLE;

	public Text m_NKM_UI_GAME_OPTION_REPLAY_TEXT_DESC;

	public NKCUIComStateButton m_csbtnExit;

	private string REPLAY_LEAVE_POPUP_TITLE => NKCUtilString.GET_STRING_REPLAY_OPTION_LEAVE_TITLE;

	private string REPLAY_LEAVE_POPUP_DESC => NKCUtilString.GET_STRING_REPLAY_OPTION_LEAVE_DESC;

	public override void Init()
	{
		m_csbtnExit.PointerClick.RemoveAllListeners();
		m_csbtnExit.PointerClick.AddListener(LeaveReplay);
	}

	public override void SetContent()
	{
		NKCUtil.SetLabelText(m_NKM_UI_GAME_OPTION_REPLAY_TEXT_TITLE, NKCUtilString.GET_STRING_GAUNTLET.ToUpper());
		NKCUtil.SetLabelText(m_NKM_UI_GAME_OPTION_REPLAY_TEXT_SUB_TITLE, "");
		NKCUtil.SetLabelText(m_NKM_UI_GAME_OPTION_REPLAY_TEXT_DESC, "");
		NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
		ReplayData currentReplayData = NKCReplayMgr.GetNKCReplaMgr().CurrentReplayData;
		if (currentReplayData != null && gameClient != null)
		{
			switch (currentReplayData.gameData.GetGameType())
			{
			case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
				NKCUtil.SetLabelText(m_NKM_UI_GAME_OPTION_REPLAY_TEXT_SUB_TITLE, NKCUtilString.GET_STRING_GAUNTLET_ASYNC_GAME);
				break;
			case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
				NKCUtil.SetLabelText(m_NKM_UI_GAME_OPTION_REPLAY_TEXT_SUB_TITLE, NKCUtilString.GET_STRING_GAUNTLET_ASYNC_GAME);
				break;
			case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
				NKCUtil.SetLabelText(m_NKM_UI_GAME_OPTION_REPLAY_TEXT_SUB_TITLE, NKCUtilString.GET_STRING_GAUNTLET_ASYNC_GAME);
				break;
			case NKM_GAME_TYPE.NGT_ASYNC_PVP:
				NKCUtil.SetLabelText(m_NKM_UI_GAME_OPTION_REPLAY_TEXT_SUB_TITLE, NKCUtilString.GET_STRING_GAUNTLET_ASYNC_GAME);
				break;
			case NKM_GAME_TYPE.NGT_PVP_RANK:
				NKCUtil.SetLabelText(m_NKM_UI_GAME_OPTION_REPLAY_TEXT_SUB_TITLE, NKCUtilString.GET_STRING_GAUNTLET_RANK_GAME);
				break;
			case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
				NKCUtil.SetLabelText(m_NKM_UI_GAME_OPTION_REPLAY_TEXT_SUB_TITLE, NKCUtilString.GET_STRING_GAUNTLET_LEAGUE_TITLE);
				break;
			case NKM_GAME_TYPE.NGT_PVP_UNLIMITED:
				NKCUtil.SetLabelText(m_NKM_UI_GAME_OPTION_REPLAY_TEXT_SUB_TITLE, NKCUtilString.GET_STRING_GAUNTLET_UNLIMITED_TITLE);
				break;
			case NKM_GAME_TYPE.NGT_PVP_PRIVATE:
				NKCUtil.SetLabelText(m_NKM_UI_GAME_OPTION_REPLAY_TEXT_SUB_TITLE, NKCUtilString.GET_STRING_PRIVATE_PVP);
				break;
			}
			string userNickname = NKCUtilString.GetUserNickname(currentReplayData.gameData.GetEnemyTeamData(gameClient.m_MyTeam).m_UserNickname, bOpponent: true);
			NKCUtil.SetLabelText(m_NKM_UI_GAME_OPTION_REPLAY_TEXT_DESC, NKCStringTable.GetString("SI_GAUNTLET_ASYNC_READY_VS") + " " + userNickname);
		}
	}

	public void LeaveReplay()
	{
		NKCPopupOKCancel.OpenOKCancelBox(REPLAY_LEAVE_POPUP_TITLE, REPLAY_LEAVE_POPUP_DESC, OnClickLeaveReplayOkButton);
	}

	private static void OnClickLeaveReplayOkButton()
	{
		if (NKCReplayMgr.IsPlayingReplay())
		{
			NKCReplayMgr.GetNKCReplaMgr().LeavePlaying();
		}
	}
}
